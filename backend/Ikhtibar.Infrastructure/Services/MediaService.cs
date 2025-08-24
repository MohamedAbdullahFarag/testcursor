using AutoMapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;
using MediaType = Ikhtibar.Shared.Entities.MediaType;
using ThumbnailSize = Ikhtibar.Shared.Enums.ThumbnailSize;
using AccessType = Ikhtibar.Shared.Entities.AccessType;
using MediaFileStatus = Ikhtibar.Shared.Enums.MediaFileStatus;
using Ikhtibar.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Main media management service implementation
/// Orchestrates all media operations including upload, processing, management, and access control
/// </summary>
public class MediaService : IMediaService
{
    private readonly IMediaFileRepository _mediaRepository;
    private readonly IMediaCategoryRepository _categoryRepository;
    private readonly IMediaCollectionRepository _collectionRepository;
    private readonly IMediaMetadataRepository _metadataRepository;
    private readonly IMediaAccessLogRepository _accessLogRepository;
    private readonly IFileUploadService _uploadService;
    private readonly IMediaStorageService _storageService;
    private readonly IThumbnailService _thumbnailService;
    private readonly IMediaValidationService _validationService;
    private readonly IMediaSearchService _searchService;
    private readonly ILogger<MediaService> _logger;
    private readonly IMapper _mapper;

    public MediaService(
        IMediaFileRepository mediaRepository,
        IMediaCategoryRepository categoryRepository,
        IMediaCollectionRepository collectionRepository,
        IMediaMetadataRepository metadataRepository,
        IMediaAccessLogRepository accessLogRepository,
        IFileUploadService uploadService,
        IMediaStorageService storageService,
        IThumbnailService thumbnailService,
        IMediaValidationService validationService,
        IMediaSearchService searchService,
        ILogger<MediaService> logger,
        IMapper mapper)
    {
        _mediaRepository = mediaRepository;
        _categoryRepository = categoryRepository;
        _collectionRepository = collectionRepository;
        _metadataRepository = metadataRepository;
        _accessLogRepository = accessLogRepository;
        _uploadService = uploadService;
        _storageService = storageService;
        _thumbnailService = thumbnailService;
        _validationService = validationService;
        _searchService = searchService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<MediaFileDto> UploadFileAsync(IFormFile file, MediaUploadDto uploadDto)
    {
        try
        {
            _logger.LogInformation("Starting file upload: {FileName}, Size: {Size} bytes", 
                file.FileName, file.Length);

            // Validate file
            var validationResult = await _validationService.ValidateFileAsync(file, uploadDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"File validation failed: {string.Join(", ", validationResult.Errors)}");
            }

            // Calculate file hash for duplicate detection
            var fileHash = await _uploadService.CalculateFileHashAsync(file);
            
            // Check for duplicates
            var existingFile = await _mediaRepository.GetByHashAsync(fileHash);
            if (existingFile != null && !uploadDto.AllowDuplicates)
            {
                _logger.LogInformation("Duplicate file detected: {Hash}", fileHash);
                return _mapper.Map<MediaFileDto>(existingFile);
            }

            // Generate unique filename
            var fileName = await _uploadService.GenerateUniqueFileNameAsync(file.FileName);
            
            // Determine media type
            var mediaType = _uploadService.DetermineMediaType(file.ContentType, file.FileName);
            
            // Create media file entity
            var mediaFile = new MediaFile
            {
                StorageFileName = fileName,
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                StoragePath = string.Empty, // Will be set after upload
                MediaType = mediaType,
                Status = MediaStatus.Uploading,
                Title = uploadDto.Title ?? file.FileName,
                Description = uploadDto.Description,
                AltText = uploadDto.AltText,
                FileHash = fileHash,
                CategoryId = uploadDto.CategoryId,
                UploadedBy = uploadDto.UploadedBy,
                IsPublic = uploadDto.IsPublic,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            // Save initial record
            var savedMediaFile = await _mediaRepository.AddAsync(mediaFile);

            // Upload to storage
            var uploadResult = await _storageService.UploadFileAsync(file, fileName, uploadDto);
            
            if (!uploadResult.Success)
            {
                throw new InvalidOperationException($"Storage upload failed: {uploadResult.ErrorMessage}");
            }
            
            // Update file paths and URLs
            savedMediaFile.StoragePath = uploadResult.FilePath;
            savedMediaFile.Status = MediaStatus.Processing;
            
            await _mediaRepository.UpdateAsync(savedMediaFile);

            // Queue for background processing
            _ = Task.Run(async () => await ProcessMediaFileAsync(savedMediaFile.Id));

            _logger.LogInformation("File uploaded successfully: {MediaId}, Path: {FilePath}", 
                savedMediaFile.Id, savedMediaFile.StoragePath);

            return _mapper.Map<MediaFileDto>(savedMediaFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", file.FileName);
            throw;
        }
    }

    public async Task<MediaFileDto> GetMediaFileAsync(int mediaId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                throw new NotFoundException($"Media file with ID {mediaId} not found");
            }

            // Log access
            _ = Task.Run(async () => await LogMediaAccessAsync(mediaId, 0, AccessType.View)); // TODO: Get current user

            return _mapper.Map<MediaFileDto>(mediaFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media file: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<PagedResult<MediaFileDto>> GetMediaFilesAsync(MediaFileSearchDto filter)
    {
        try
        {
            var result = await _searchService.SearchMediaAsync(filter);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media files with filter");
            throw;
        }
    }

    public async Task<bool> DeleteMediaFileAsync(int mediaId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                return false;
            }

            // Delete from storage
            await _storageService.DeleteFileAsync(mediaFile.StoragePath);

            // Delete thumbnails
            await _thumbnailService.DeleteThumbnailsAsync(mediaId);

            // Soft delete from database
            mediaFile.Status = MediaStatus.Deleted;
            mediaFile.ModifiedAt = DateTime.UtcNow;
            
            await _mediaRepository.UpdateAsync(mediaFile);
            
            _logger.LogInformation("Media file deleted: {MediaId}", mediaId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting media file: {MediaId}", mediaId);
            return false;
        }
    }

    public async Task<MediaFileDto> UpdateMediaFileAsync(int mediaId, UpdateMediaFileDto dto)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                throw new NotFoundException($"Media file with ID {mediaId} not found");
            }

            // Update properties
            if (!string.IsNullOrEmpty(dto.Title))
                mediaFile.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.Description))
                mediaFile.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.AltText))
                mediaFile.AltText = dto.AltText;
            if (dto.CategoryId.HasValue)
                mediaFile.CategoryId = dto.CategoryId;
            if (dto.IsPublic.HasValue)
                mediaFile.IsPublic = dto.IsPublic.Value;

            mediaFile.ModifiedAt = DateTime.UtcNow;
            await _mediaRepository.UpdateAsync(mediaFile);

            _logger.LogInformation("Media file updated: {MediaId}", mediaId);
            return _mapper.Map<MediaFileDto>(mediaFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating media file: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<Stream> GetFileStreamAsync(int mediaId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                throw new NotFoundException($"Media file with ID {mediaId} not found");
            }

            if (mediaFile.Status != MediaStatus.Available)
            {
                throw new InvalidOperationException($"Media file is not ready for download. Status: {mediaFile.Status}");
            }

            // Log access
            _ = Task.Run(async () => await LogMediaAccessAsync(mediaId, 0, AccessType.Download)); // TODO: Get current user

            return await _storageService.GetFileStreamAsync(mediaFile.StoragePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file stream: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<byte[]> GetFileBytesAsync(int mediaId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                throw new NotFoundException($"Media file with ID {mediaId} not found");
            }

            if (mediaFile.Status != MediaStatus.Available)
            {
                throw new InvalidOperationException($"Media file is not ready for download. Status: {mediaId}");
            }

            // Log access
            _ = Task.Run(async () => await LogMediaAccessAsync(mediaId, 0, AccessType.Download)); // TODO: Get current user

            return await _storageService.GetFileBytesAsync(mediaFile.StoragePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file bytes: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<string> GetFileUrlAsync(int mediaId, TimeSpan? expirationTime = null)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                throw new NotFoundException($"Media file with ID {mediaId} not found");
            }

            if (mediaFile.Status != MediaStatus.Available)
            {
                throw new InvalidOperationException($"Media file is not ready for access. Status: {mediaFile.Status}");
            }

            // Log access
            _ = Task.Run(async () => await LogMediaAccessAsync(mediaId, 0, AccessType.View)); // TODO: Get current user

            return await _storageService.GetFileUrlAsync(mediaFile.StoragePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file URL: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<string> GetThumbnailUrlAsync(int mediaId, ThumbnailSize size = ThumbnailSize.Medium)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                throw new NotFoundException($"Media file with ID {mediaId} not found");
            }

            if (mediaFile.Status != MediaStatus.Available)
            {
                throw new InvalidOperationException($"Media file is not ready for access. Status: {mediaFile.Status}");
            }

            // Log access
            _ = Task.Run(async () => await LogMediaAccessAsync(mediaId, 0, AccessType.View)); // TODO: Get current user

            return await _thumbnailService.GetThumbnailUrlAsync(mediaId, size);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting thumbnail URL: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<bool> ProcessMediaFileAsync(int mediaId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                _logger.LogWarning("Media file not found for processing: {MediaId}", mediaId);
                return false;
            }

            mediaFile.Status = MediaStatus.Processing;
            await _mediaRepository.UpdateAsync(mediaFile);

            try
            {
                // Generate thumbnails
                await _thumbnailService.GenerateThumbnailsAsync(mediaFile);

                // Extract metadata
                await ExtractMetadataAsync(mediaFile);

                mediaFile.Status = MediaStatus.Available;
                mediaFile.ModifiedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing media file: {MediaId}", mediaId);
                mediaFile.Status = MediaStatus.Failed;
                mediaFile.ModifiedAt = DateTime.UtcNow;
            }

            await _mediaRepository.UpdateAsync(mediaFile);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing media file: {MediaId}", mediaId);
            
            // Update status to failed
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile != null)
            {
                mediaFile.Status = MediaStatus.Failed;
                mediaFile.ModifiedAt = DateTime.UtcNow;
                await _mediaRepository.UpdateAsync(mediaFile);
            }
            
            return false;
        }
    }

    public async Task<MediaProcessingStatusDto> GetProcessingStatusAsync(int mediaId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                throw new NotFoundException($"Media file with ID {mediaId} not found");
            }

            return new MediaProcessingStatusDto
            {
                MediaId = mediaId,
                Status = ConvertToMediaFileStatus(mediaFile.Status),
                ProgressPercentage = mediaFile.Status == MediaStatus.Available ? 100 : 
                                   mediaFile.Status == MediaStatus.Failed ? 0 : 50,
                CurrentStep = mediaFile.Status.ToString(),
                StartedAt = mediaFile.CreatedAt,
                CompletedAt = mediaFile.Status == MediaStatus.Available ? mediaFile.ModifiedAt : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting processing status: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<bool> RegenerateMediaProcessingAsync(int mediaId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                _logger.LogWarning("Media file not found for regeneration: {MediaId}", mediaId);
                return false;
            }

            // Delete existing thumbnails
            await _thumbnailService.DeleteThumbnailsAsync(mediaId);

            // Reprocess the file
            return await ProcessMediaFileAsync(mediaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating media processing: {MediaId}", mediaId);
            return false;
        }
    }

    public async Task<IEnumerable<MediaCategoryDto>> GetCategoriesAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetRootCategoriesAsync();
            return _mapper.Map<IEnumerable<MediaCategoryDto>>(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media categories");
            throw;
        }
    }

    public async Task<IEnumerable<MediaCollectionDto>> GetCollectionsAsync(int? userId = null)
    {
        try
        {
            if (userId.HasValue)
            {
                var userCollections = await _collectionRepository.GetByUserAsync(userId.Value);
                return _mapper.Map<IEnumerable<MediaCollectionDto>>(userCollections);
            }
            else
            {
                var publicCollections = await _collectionRepository.GetPublicAsync();
                return _mapper.Map<IEnumerable<MediaCollectionDto>>(publicCollections);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media collections");
            throw;
        }
    }

    public async Task<MediaCollectionDto> CreateCollectionAsync(CreateMediaCollectionDto dto)
    {
        try
        {
            var collection = _mapper.Map<MediaCollection>(dto);
            collection.CreatedAt = DateTime.UtcNow;
            collection.ModifiedAt = DateTime.UtcNow;
            
            var createdCollection = await _collectionRepository.AddAsync(collection);
            return _mapper.Map<MediaCollectionDto>(createdCollection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating media collection");
            throw;
        }
    }

    public async Task<bool> AddMediaToCollectionAsync(int mediaId, int collectionId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                _logger.LogWarning("Media file not found: {MediaId}", mediaId);
                return false;
            }

            var success = await _collectionRepository.AddMediaFileAsync(collectionId, mediaId);
            if (success)
            {
                _logger.LogInformation("Added media {MediaId} to collection {CollectionId}", mediaId, collectionId);
            }
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding media to collection: {MediaId}, {CollectionId}", mediaId, collectionId);
            return false;
        }
    }

    public async Task<bool> RemoveMediaFromCollectionAsync(int mediaId, int collectionId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                _logger.LogWarning("Media file not found: {MediaId}", mediaId);
                return false;
            }

            var success = await _collectionRepository.RemoveMediaFileAsync(collectionId, mediaId);
            if (success)
            {
                _logger.LogInformation("Removed media {MediaId} from collection {CollectionId}", mediaId, collectionId);
            }
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing media from collection: {MediaId}, {CollectionId}", mediaId, collectionId);
            return false;
        }
    }

    public async Task<PagedResult<MediaFileDto>> SearchMediaAsync(MediaFileSearchDto searchDto)
    {
        try
        {
            return await _searchService.SearchMediaAsync(searchDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching media");
            throw;
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetSimilarMediaAsync(int mediaId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                throw new NotFoundException($"Media file with ID {mediaId} not found");
            }

            // TODO: Implement similarity search logic
            var similarMedia = await _searchService.GetSimilarMediaAsync(mediaId);
            return similarMedia;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting similar media: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetRecentMediaAsync(int count = 10, int? userId = null)
    {
        try
        {
            var recentMedia = await _searchService.GetRecentMediaAsync(count, userId);
            return recentMedia;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent media");
            throw;
        }
    }

    public async Task<MediaValidationResult> ValidateFileAsync(IFormFile file, MediaUploadDto uploadDto)
    {
        try
        {
            var validationResult = await _validationService.ValidateFileAsync(file, uploadDto);
            return validationResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating file");
            throw;
        }
    }

    public async Task<bool> CheckMediaAccessAsync(int mediaId, int userId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                return false;
            }

            // Public files are accessible to everyone
            if (mediaFile.IsPublic)
                return true;

            // Private files are accessible to uploader
            if (mediaFile.UploadedBy == userId)
                return true;

            // Check if user has access through collections
            var userCollections = await _collectionRepository.GetByUserAsync(userId);
            foreach (var collection in userCollections)
            {
                var collectionItems = await _collectionRepository.GetCollectionItemsAsync(collection.Id);
                if (collectionItems.Any(ci => ci.MediaFileId == mediaId))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking media access: {MediaId}, {UserId}", mediaId, userId);
            return false;
        }
    }

    public async Task<MediaAccessLogDto> LogMediaAccessAsync(int mediaId, int userId, AccessType accessType)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                throw new NotFoundException($"Media file with ID {mediaId} not found");
            }

            var accessLog = new MediaAccessLog
            {
                MediaFileId = mediaId,
                UserId = userId,
                AccessType = accessType,
                IpAddress = "0.0.0.0", // TODO: Get from request context
                UserAgent = "Unknown", // TODO: Get from request context
                CreatedAt = DateTime.UtcNow
            };

            var savedAccessLog = await _accessLogRepository.AddAsync(accessLog);

            return new MediaAccessLogDto
            {
                MediaFileId = mediaId,
                UserId = userId,
                Action = (MediaAccessAction)accessType,
                AccessedAt = DateTime.UtcNow,
                IpAddress = accessLog.IpAddress,
                UserAgent = accessLog.UserAgent
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging media access: {MediaId}, {UserId}, {AccessType}", mediaId, userId, accessType);
            throw;
        }
    }

    public async Task<bool> BulkDeleteMediaAsync(IEnumerable<int> mediaIds)
    {
        try
        {
            // TODO: Implement bulk delete
            foreach (var mediaId in mediaIds)
            {
                await DeleteMediaFileAsync(mediaId);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk deleting media");
            return false;
        }
    }

    public async Task<bool> BulkMoveMediaAsync(IEnumerable<int> mediaIds, int targetCategoryId)
    {
        try
        {
            // TODO: Implement bulk move
            foreach (var mediaId in mediaIds)
            {
                var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
                if (mediaFile != null)
                {
                    mediaFile.CategoryId = targetCategoryId;
                    mediaFile.ModifiedAt = DateTime.UtcNow;
                    await _mediaRepository.UpdateAsync(mediaFile);
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk moving media");
            return false;
        }
    }

    public async Task<bool> BulkUpdateMediaAsync(BulkMediaFileOperationDto dto)
    {
        try
        {
            foreach (var mediaId in dto.MediaFileIds)
            {
                var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
                if (mediaFile != null)
                {
                    if (dto.TargetCategoryId.HasValue)
                        mediaFile.CategoryId = dto.TargetCategoryId.Value;
                    
                    if (!string.IsNullOrEmpty(dto.Tags))
                    {
                        // Store tags in metadata if needed
                        // TODO: Implement tag storage in MediaMetadata
                    }
                    
                    mediaFile.ModifiedAt = DateTime.UtcNow;
                    await _mediaRepository.UpdateAsync(mediaFile);
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk updating media");
            return false;
        }
    }

    private async Task<bool> ProcessImageAsync(MediaFile mediaFile)
    {
        try
        {
            // Get image from storage
            using var imageStream = await _storageService.GetFileStreamAsync(mediaFile.StoragePath);
            
            // TODO: Implement image processing logic
            // Extract dimensions, optimize, etc.
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image: {MediaId}", mediaFile.Id);
            return false;
        }
    }

    private async Task<bool> ProcessVideoAsync(MediaFile mediaFile)
    {
        try
        {
            // TODO: Implement video processing logic
            // Extract duration, generate thumbnails, etc.
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing video: {MediaId}", mediaFile.Id);
            return false;
        }
    }

    private async Task<bool> ProcessAudioAsync(MediaFile mediaFile)
    {
        try
        {
            // TODO: Implement audio processing logic
            // Extract duration, bitrate, etc.
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing audio: {MediaId}", mediaFile.Id);
            return false;
        }
    }

    private async Task<bool> ProcessDocumentAsync(MediaFile mediaFile)
    {
        try
        {
            // TODO: Implement document processing logic
            // Extract text, generate previews, etc.
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing document: {MediaId}", mediaFile.Id);
            return false;
        }
    }

    private async Task ExtractMetadataAsync(MediaFile mediaFile)
    {
        try
        {
            var metadata = await _storageService.ExtractMetadataAsync(mediaFile.StoragePath);
            
            foreach (var kvp in metadata)
            {
                var metadataEntity = new MediaMetadata
                {
                    MediaFileId = mediaFile.Id,
                    MetadataKey = kvp.Key,
                    MetadataValue = kvp.Value.ToString() ?? string.Empty,
                    DataType = GetMetadataType(kvp.Value),
                    CreatedAt = DateTime.UtcNow
                };

                await _metadataRepository.AddAsync(metadataEntity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting metadata for media: {MediaId}", mediaFile.Id);
        }
    }

    /// <summary>
    /// Converts MediaStatus from Core to MediaFileStatus for Shared
    /// </summary>
    private MediaFileStatus ConvertToMediaFileStatus(MediaStatus status)
    {
        return status switch
        {
            MediaStatus.Uploading => MediaFileStatus.Uploading,
            MediaStatus.Processing => MediaFileStatus.Processing,
            MediaStatus.Available => MediaFileStatus.Available,
            MediaStatus.Failed => MediaFileStatus.Failed,
            MediaStatus.Quarantined => MediaFileStatus.Quarantined,
            MediaStatus.Archived => MediaFileStatus.Archived,
            MediaStatus.Deleted => MediaFileStatus.MarkedForDeletion,
            _ => MediaFileStatus.Unavailable
        };
    }

    /// <summary>
    /// Converts .NET type to MetadataType enum
    /// </summary>
    private MetadataType GetMetadataType(object value)
    {
        return value switch
        {
            string => MetadataType.String,
            int or long or short or byte => MetadataType.Integer,
            decimal or double or float => MetadataType.Decimal,
            bool => MetadataType.Boolean,
            DateTime => MetadataType.DateTime,
            _ => MetadataType.String
        };
    }
}

// Note: use shared/core exception types instead of local duplicates
