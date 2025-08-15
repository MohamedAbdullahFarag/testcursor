using AutoMapper;
using Ikhtibar.Core.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;
using MediaType = Ikhtibar.Core.Entities.MediaType;
using ThumbnailSize = Ikhtibar.Shared.Enums.ThumbnailSize;
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
                FileName = fileName,
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
            _ = Task.Run(async () => await LogMediaAccessAsync(mediaId, 0, MediaAccessType.View)); // TODO: Get current user

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

    public async Task<bool> DeleteMediaFileAsync(Guid mediaId)
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

    public async Task<MediaFileDto> UpdateMediaFileAsync(Guid mediaId, UpdateMediaFileDto dto)
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
            if (dto.Status.HasValue)
                mediaFile.Status = dto.Status.Value;
            if (!string.IsNullOrEmpty(dto.Tags))
                mediaFile.Tags = dto.Tags;

            mediaFile.ModifiedAt = DateTime.UtcNow;

            var updatedFile = await _mediaRepository.UpdateAsync(mediaFile);
            return _mapper.Map<MediaFileDto>(updatedFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating media file: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<Stream> GetFileStreamAsync(Guid mediaId)
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
            _ = Task.Run(async () => await LogMediaAccessAsync(mediaId, Guid.Empty, MediaAccessType.Download)); // TODO: Get current user

            return await _storageService.GetFileStreamAsync(mediaFile.StoragePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file stream for media: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<byte[]> GetFileBytesAsync(Guid mediaId)
    {
        try
        {
            using var stream = await GetFileStreamAsync(mediaId);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file bytes for media: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<string> GetFileUrlAsync(Guid mediaId, TimeSpan? expirationTime = null)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                throw new NotFoundException($"Media file with ID {mediaId} not found");
            }

            if (mediaFile.IsPublic)
            {
                return await _storageService.GetFileUrlAsync(mediaFile.StoragePath);
            }

            // Generate temporary signed URL for private files
            var expiration = expirationTime ?? TimeSpan.FromHours(1);
            return await _storageService.GetSignedUrlAsync(mediaFile.StoragePath, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file URL for media: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<string> GetThumbnailUrlAsync(Guid mediaId, ThumbnailSize size = ThumbnailSize.Medium)
    {
        try
        {
            var thumbnailUrl = await _thumbnailService.GetThumbnailUrlAsync(mediaId, size);
            if (string.IsNullOrEmpty(thumbnailUrl))
            {
                // Generate thumbnail if it doesn't exist
                var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
                if (mediaFile != null)
                {
                    await _thumbnailService.GenerateThumbnailAsync(mediaFile, size);
                    thumbnailUrl = await _thumbnailService.GetThumbnailUrlAsync(mediaId, size);
                }
            }
            
            return thumbnailUrl ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting thumbnail URL for media: {MediaId}", mediaId);
            return string.Empty;
        }
    }

    public async Task<bool> ProcessMediaFileAsync(Guid mediaId)
    {
        try
        {
            _logger.LogInformation("Starting media processing for: {MediaId}", mediaId);

            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
            {
                _logger.LogWarning("Media file not found: {MediaId}", mediaId);
                return false;
            }

            mediaFile.Status = MediaStatus.Processing;
            await _mediaRepository.UpdateAsync(mediaFile);

            var processed = false;

            switch (mediaFile.MediaType)
            {
                case MediaType.Image:
                    processed = await ProcessImageAsync(mediaFile);
                    break;
                case MediaType.Video:
                    processed = await ProcessVideoAsync(mediaFile);
                    break;
                case MediaType.Audio:
                    processed = await ProcessAudioAsync(mediaFile);
                    break;
                case MediaType.Document:
                    processed = await ProcessDocumentAsync(mediaFile);
                    break;
                default:
                    processed = true; // No processing needed for other types
                    break;
            }

            if (processed)
            {
                // Generate thumbnails
                await _thumbnailService.GenerateThumbnailsAsync(mediaFile);

                // Extract metadata
                await ExtractMetadataAsync(mediaFile);

                mediaFile.Status = MediaStatus.Available;
                mediaFile.ModifiedAt = DateTime.UtcNow;
            }
            else
            {
                mediaFile.Status = MediaStatus.Failed;
                mediaFile.ModifiedAt = DateTime.UtcNow;
            }

            await _mediaRepository.UpdateAsync(mediaFile);

            _logger.LogInformation("Media processing completed for: {MediaId}, Status: {Status}", 
                mediaId, mediaFile.Status);

            return processed;
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

    public async Task<MediaProcessingStatusDto> GetProcessingStatusAsync(Guid mediaId)
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
                Status = mediaFile.Status,
                ProgressPercentage = mediaFile.Status == MediaStatus.Available ? 100 : 
                                   mediaFile.Status == MediaStatus.Failed ? 0 : 50,
                CurrentStep = mediaFile.Status.ToString(),
                StartedAt = mediaFile.CreatedAt,
                CompletedAt = mediaFile.Status == MediaStatus.Available ? mediaFile.ModifiedAt : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting processing status for media: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<bool> RegenerateMediaProcessingAsync(Guid mediaId)
    {
        try
        {
            return await ProcessMediaFileAsync(mediaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating media processing for: {MediaId}", mediaId);
            return false;
        }
    }

    public async Task<IEnumerable<MediaCategoryDto>> GetCategoriesAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MediaCategoryDto>>(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media categories");
            throw;
        }
    }

    public async Task<IEnumerable<MediaCollectionDto>> GetCollectionsAsync(Guid? userId = null)
    {
        try
        {
            var collections = await _collectionRepository.GetAllAsync();
            if (userId.HasValue)
            {
                collections = collections.Where(c => c.CreatedBy == userId.Value || c.IsPublic);
            }
            return _mapper.Map<IEnumerable<MediaCollectionDto>>(collections);
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

    public async Task<bool> AddMediaToCollectionAsync(Guid mediaId, Guid collectionId)
    {
        try
        {
            // Implementation depends on collection structure
            // This is a simplified version
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding media to collection: {MediaId}, {CollectionId}", mediaId, collectionId);
            return false;
        }
    }

    public async Task<bool> RemoveMediaFromCollectionAsync(Guid mediaId, Guid collectionId)
    {
        try
        {
            // Implementation depends on collection structure
            // This is a simplified version
            return true;
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

    public async Task<IEnumerable<MediaFileDto>> GetSimilarMediaAsync(Guid mediaId)
    {
        try
        {
            return await _searchService.FindSimilarMediaAsync(mediaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting similar media: {MediaId}", mediaId);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFileDto>> GetRecentMediaAsync(int count = 10, Guid? userId = null)
    {
        try
        {
            return await _searchService.GetRecentMediaAsync(count, userId);
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
            return await _validationService.ValidateFileAsync(file, uploadDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating file: {FileName}", file.FileName);
            throw;
        }
    }

    public async Task<bool> CheckMediaAccessAsync(Guid mediaId, Guid userId)
    {
        try
        {
            var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
            if (mediaFile == null)
                return false;

            // Public files are accessible to everyone
            if (mediaFile.IsPublic)
                return true;

            // Private files are accessible to uploader
            if (mediaFile.UploadedBy == userId)
                return true;

            // TODO: Add role-based access control
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking media access: {MediaId}, {UserId}", mediaId, userId);
            return false;
        }
    }

    public async Task<MediaAccessLogDto> LogMediaAccessAsync(Guid mediaId, Guid userId, MediaAccessType accessType)
    {
        try
        {
            var accessLog = new MediaAccessLog
            {
                MediaFileId = mediaId,
                UserId = userId,
                AccessType = (Shared.Enums.MediaAccessType)accessType,
                AccessTimestamp = DateTime.UtcNow,
                IpAddress = string.Empty, // TODO: Get from context
                UserAgent = string.Empty, // TODO: Get from context
                CreatedAt = DateTime.UtcNow
            };

            var createdLog = await _accessLogRepository.AddAsync(accessLog);
            return _mapper.Map<MediaAccessLogDto>(createdLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging media access: {MediaId}, {UserId}", mediaId, userId);
            throw;
        }
    }

    public async Task<bool> BulkDeleteMediaAsync(IEnumerable<Guid> mediaIds)
    {
        try
        {
            var success = true;
            foreach (var mediaId in mediaIds)
            {
                if (!await DeleteMediaFileAsync(mediaId))
                {
                    success = false;
                }
            }
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk deleting media files");
            return false;
        }
    }

    public async Task<bool> BulkMoveMediaAsync(IEnumerable<Guid> mediaIds, Guid targetCategoryId)
    {
        try
        {
            var success = true;
            foreach (var mediaId in mediaIds)
            {
                var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
                if (mediaFile != null)
                {
                    mediaFile.CategoryId = targetCategoryId;
                    mediaFile.ModifiedAt = DateTime.UtcNow;
                    await _mediaRepository.UpdateAsync(mediaFile);
                }
                else
                {
                    success = false;
                }
            }
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk moving media files");
            return false;
        }
    }

    public async Task<bool> BulkUpdateMediaAsync(BulkMediaFileOperationDto dto)
    {
        try
        {
            var success = true;
            foreach (var mediaId in dto.MediaFileIds)
            {
                var mediaFile = await _mediaRepository.GetByIdAsync(mediaId);
                if (mediaFile != null)
                {
                    if (dto.TargetStatus.HasValue)
                        mediaFile.Status = dto.TargetStatus.Value;
                    if (dto.TargetCategoryId.HasValue)
                        mediaFile.CategoryId = dto.TargetCategoryId.Value;
                    
                    mediaFile.ModifiedAt = DateTime.UtcNow;
                    await _mediaRepository.UpdateAsync(mediaFile);
                }
                else
                {
                    success = false;
                }
            }
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk updating media files");
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
                    DataType = kvp.Value.GetType().Name,
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
}

/// <summary>
/// Custom exceptions for media operations
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
