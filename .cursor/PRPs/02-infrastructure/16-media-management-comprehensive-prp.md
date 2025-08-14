# Question Bank Media Management System - Comprehensive Implementation PRP

## 🎯 Executive Summary
Generate a comprehensive media management system for the Ikhtibar question bank module that handles multimedia content including images, videos, audio files, and documents. This system will provide secure file upload, storage, retrieval, and management capabilities with support for question attachments, answer media, and educational content resources.

## 📋 What to Generate

### 1. Backend Media Management System
```
backend/Ikhtibar.Core/Entities/
├── MediaFile.cs                       # Core media file entity
├── MediaCategory.cs                   # Media categorization entity
├── MediaCollection.cs                 # Media collection/album entity
├── MediaMetadata.cs                   # Extended metadata entity
├── MediaProcessingJob.cs              # Media processing queue entity
├── MediaThumbnail.cs                  # Thumbnail management entity
└── MediaAccessLog.cs                  # Media access tracking entity

backend/Ikhtibar.Core/Services/Interfaces/
├── IMediaService.cs                   # Core media operations
├── IFileUploadService.cs              # File upload handling
├── IImageProcessingService.cs         # Image processing operations
├── IVideoProcessingService.cs         # Video processing operations
├── IMediaStorageService.cs            # Storage abstraction
├── IThumbnailService.cs               # Thumbnail generation
├── IMediaValidationService.cs         # Media validation
└── IMediaSearchService.cs             # Media search and indexing

backend/Ikhtibar.Core/Services/Implementations/
├── MediaService.cs                    # Main media logic
├── FileUploadService.cs               # Upload handling
├── ImageProcessingService.cs          # Image operations
├── VideoProcessingService.cs          # Video operations
├── AzureBlobStorageService.cs         # Azure Blob storage
├── LocalFileStorageService.cs         # Local file storage
├── ThumbnailService.cs                # Thumbnail generation
├── MediaValidationService.cs          # Validation logic
└── MediaSearchService.cs              # Search implementation

backend/Ikhtibar.Core/Repositories/Interfaces/
├── IMediaFileRepository.cs            # Media file data access
├── IMediaCategoryRepository.cs        # Category data access
├── IMediaCollectionRepository.cs      # Collection data access
├── IMediaMetadataRepository.cs        # Metadata data access
└── IMediaAccessLogRepository.cs       # Access log data access

backend/Ikhtibar.Infrastructure/Repositories/
├── MediaFileRepository.cs             # Media file repository
├── MediaCategoryRepository.cs         # Category repository
├── MediaCollectionRepository.cs       # Collection repository
├── MediaMetadataRepository.cs         # Metadata repository
└── MediaAccessLogRepository.cs        # Access log repository

backend/Ikhtibar.API/Controllers/
├── MediaController.cs                 # Media management endpoints
├── MediaUploadController.cs           # Upload handling endpoints
├── MediaCategoryController.cs         # Category management
├── MediaCollectionController.cs       # Collection management
└── MediaDownloadController.cs         # Download and streaming

backend/Ikhtibar.API/DTOs/
├── MediaFileDto.cs                    # Media file data objects
├── MediaUploadDto.cs                  # Upload request objects
├── MediaCategoryDto.cs                # Category data objects
├── MediaCollectionDto.cs              # Collection data objects
├── MediaSearchDto.cs                  # Search request objects
├── MediaProcessingDto.cs              # Processing status objects
└── MediaDownloadDto.cs                # Download response objects
```

### 2. Frontend Media Management Interface
```
frontend/src/modules/question-bank/media/
├── components/
│   ├── MediaManager.tsx               # Main media management
│   ├── MediaUploader.tsx              # File upload component
│   ├── MediaGallery.tsx               # Media gallery view
│   ├── MediaGrid.tsx                  # Grid layout component
│   ├── MediaList.tsx                  # List layout component
│   ├── MediaCard.tsx                  # Individual media card
│   ├── MediaPreview.tsx               # Media preview modal
│   ├── MediaEditor.tsx                # Media editing interface
│   ├── MediaSelector.tsx              # Media selection dialog
│   ├── MediaCategoryManager.tsx       # Category management
│   ├── MediaCollectionManager.tsx     # Collection management
│   ├── MediaSearch.tsx                # Search interface
│   ├── MediaFilters.tsx               # Filter controls
│   ├── ImageEditor.tsx                # Image editing tools
│   ├── VideoPlayer.tsx                # Video player component
│   ├── AudioPlayer.tsx                # Audio player component
│   └── DocumentViewer.tsx             # Document viewer
├── hooks/
│   ├── useMediaManager.tsx            # Main media hook
│   ├── useMediaUpload.tsx             # Upload functionality
│   ├── useMediaProcessing.tsx         # Processing status
│   ├── useMediaSearch.tsx             # Search functionality
│   ├── useMediaCategories.tsx         # Category management
│   ├── useMediaCollections.tsx        # Collection management
│   ├── useImageEditor.tsx             # Image editing
│   └── useMediaValidation.tsx         # Validation hooks
├── services/
│   ├── mediaService.ts                # Media API service
│   ├── uploadService.ts               # Upload handling
│   ├── processingService.ts           # Processing status
│   ├── storageService.ts              # Storage operations
│   └── validationService.ts           # Validation service
├── types/
│   ├── media.types.ts                 # Media type definitions
│   ├── upload.types.ts                # Upload types
│   ├── processing.types.ts            # Processing types
│   └── validation.types.ts            # Validation types
├── utils/
│   ├── fileUtils.ts                   # File utility functions
│   ├── imageUtils.ts                  # Image processing utils
│   ├── videoUtils.ts                  # Video utility functions
│   ├── validationUtils.ts             # Validation utilities
│   └── formatUtils.ts                 # Format conversion utils
├── constants/
│   ├── mediaTypes.ts                  # Media type constants
│   ├── fileFormats.ts                 # Supported formats
│   ├── uploadLimits.ts                # Upload limitations
│   └── processingStates.ts            # Processing status
└── locales/
    ├── en.json                        # English translations
    └── ar.json                        # Arabic translations
```

### 3. Storage Integration Components
```
backend/Ikhtibar.Core/Storage/
├── Providers/
│   ├── AzureBlobStorageProvider.cs    # Azure Blob Storage
│   ├── AmazonS3StorageProvider.cs     # AWS S3 Storage
│   ├── LocalFileStorageProvider.cs    # Local file system
│   └── IStorageProvider.cs            # Storage provider interface
├── Processing/
│   ├── ImageProcessor.cs              # Image processing pipeline
│   ├── VideoProcessor.cs              # Video processing pipeline
│   ├── AudioProcessor.cs              # Audio processing pipeline
│   └── DocumentProcessor.cs           # Document processing
└── Security/
    ├── MediaSecurityService.cs        # Security and access control
    ├── VirusScanningService.cs        # Virus scanning integration
    └── WatermarkService.cs            # Watermarking service
```

### 4. Background Processing
```
backend/Ikhtibar.Infrastructure/BackgroundServices/
├── MediaProcessingService.cs          # Background media processing
├── ThumbnailGenerationService.cs      # Thumbnail generation
├── MediaOptimizationService.cs        # Media optimization
└── MediaCleanupService.cs             # Cleanup old/unused media

backend/Ikhtibar.Core/Jobs/
├── MediaProcessingJob.cs              # Processing job definition
├── ThumbnailGenerationJob.cs          # Thumbnail job
├── MediaOptimizationJob.cs            # Optimization job
└── MediaCleanupJob.cs                 # Cleanup job
```

## 🏗 Implementation Architecture

### Entity Design Patterns

#### Media File Entity
```csharp
[Table("MediaFiles")]
public class MediaFile : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(2048)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(2048)]
    public string Url { get; set; } = string.Empty;

    [Required]
    public MediaType MediaType { get; set; }

    [Required]
    [MaxLength(50)]
    public string FileExtension { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string MimeType { get; set; } = string.Empty;

    [Required]
    public long FileSizeBytes { get; set; }

    [Required]
    [MaxLength(64)]
    public string FileHash { get; set; } = string.Empty;

    [Required]
    public int UploadedBy { get; set; }

    [Required]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedAt { get; set; }

    [Required]
    public MediaStatus Status { get; set; } = MediaStatus.Uploading;

    [Required]
    public bool IsPublic { get; set; } = false;

    public int? CategoryId { get; set; }

    public int? CollectionId { get; set; }

    [MaxLength(500)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(1000)]
    public string? Tags { get; set; }

    [MaxLength(100)]
    public string? AltText { get; set; }

    // Media-specific properties
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? DurationSeconds { get; set; }
    public int? Bitrate { get; set; }
    public int? SampleRate { get; set; }
    public int? Channels { get; set; }

    [MaxLength(2000)]
    public string? MetadataJson { get; set; }

    [MaxLength(2048)]
    public string? ThumbnailUrl { get; set; }

    [MaxLength(2048)]
    public string? PreviewUrl { get; set; }

    // Security and access
    [Required]
    public bool RequiresAuthentication { get; set; } = true;

    public DateTime? ExpiresAt { get; set; }

    [Required]
    public int DownloadCount { get; set; } = 0;

    // Navigation properties
    [ForeignKey("UploadedBy")]
    public virtual User UploadedByUser { get; set; } = null!;

    [ForeignKey("CategoryId")]
    public virtual MediaCategory? Category { get; set; }

    [ForeignKey("CollectionId")]
    public virtual MediaCollection? Collection { get; set; }

    public virtual ICollection<MediaMetadata> Metadata { get; set; } = new List<MediaMetadata>();
    public virtual ICollection<MediaThumbnail> Thumbnails { get; set; } = new List<MediaThumbnail>();
    public virtual ICollection<QuestionMedia> QuestionMedia { get; set; } = new List<QuestionMedia>();
    public virtual ICollection<AnswerMedia> AnswerMedia { get; set; } = new List<AnswerMedia>();
    public virtual ICollection<MediaAccessLog> AccessLogs { get; set; } = new List<MediaAccessLog>();
}

public enum MediaType
{
    Image = 1,
    Video = 2,
    Audio = 3,
    Document = 4,
    Archive = 5,
    Other = 6
}

public enum MediaStatus
{
    Uploading = 1,
    Processing = 2,
    Ready = 3,
    Failed = 4,
    Deleted = 5,
    Quarantined = 6
}
```

#### Media Category Entity
```csharp
[Table("MediaCategories")]
public class MediaCategory : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public MediaType AllowedMediaType { get; set; }

    public int? ParentId { get; set; }

    [Required]
    public int SortOrder { get; set; } = 0;

    [Required]
    public bool IsActive { get; set; } = true;

    [MaxLength(1000)]
    public string? AllowedFileExtensions { get; set; }

    public long? MaxFileSizeBytes { get; set; }

    [Required]
    public bool RequiresApproval { get; set; } = false;

    // Navigation properties
    [ForeignKey("ParentId")]
    public virtual MediaCategory? Parent { get; set; }

    public virtual ICollection<MediaCategory> Children { get; set; } = new List<MediaCategory>();
    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
}
```

#### Media Collection Entity
```csharp
[Table("MediaCollections")]
public class MediaCollection : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsPublic { get; set; } = false;

    [Required]
    public bool IsActive { get; set; } = true;

    [MaxLength(2048)]
    public string? CoverImageUrl { get; set; }

    [MaxLength(1000)]
    public string? Tags { get; set; }

    // Navigation properties
    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; } = null!;

    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
}
```

#### Media Metadata Entity
```csharp
[Table("MediaMetadata")]
public class MediaMetadata : BaseEntity
{
    [Required]
    public int MediaFileId { get; set; }

    [Required]
    [MaxLength(100)]
    public string MetadataKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string MetadataValue { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string DataType { get; set; } = string.Empty;

    // Navigation properties
    [ForeignKey("MediaFileId")]
    public virtual MediaFile MediaFile { get; set; } = null!;
}
```

### Service Implementation Patterns

#### Core Media Service
```csharp
public interface IMediaService
{
    // File operations
    Task<MediaFileDto> UploadFileAsync(IFormFile file, MediaUploadDto uploadDto);
    Task<MediaFileDto> GetMediaFileAsync(int mediaId);
    Task<PagedResult<MediaFileDto>> GetMediaFilesAsync(MediaFilterDto filter);
    Task<bool> DeleteMediaFileAsync(int mediaId);
    Task<MediaFileDto> UpdateMediaFileAsync(int mediaId, UpdateMediaFileDto dto);
    
    // Download and streaming
    Task<Stream> GetFileStreamAsync(int mediaId);
    Task<byte[]> GetFileBytesAsync(int mediaId);
    Task<string> GetFileUrlAsync(int mediaId, TimeSpan? expirationTime = null);
    Task<string> GetThumbnailUrlAsync(int mediaId, ThumbnailSize size = ThumbnailSize.Medium);
    
    // Processing
    Task<bool> ProcessMediaFileAsync(int mediaId);
    Task<MediaProcessingStatusDto> GetProcessingStatusAsync(int mediaId);
    Task<bool> RegenerateMediaProcessingAsync(int mediaId);
    
    // Categories and collections
    Task<IEnumerable<MediaCategoryDto>> GetCategoriesAsync();
    Task<IEnumerable<MediaCollectionDto>> GetCollectionsAsync(int? userId = null);
    Task<MediaCollectionDto> CreateCollectionAsync(CreateMediaCollectionDto dto);
    Task<bool> AddMediaToCollectionAsync(int mediaId, int collectionId);
    Task<bool> RemoveMediaFromCollectionAsync(int mediaId, int collectionId);
    
    // Search and filtering
    Task<PagedResult<MediaFileDto>> SearchMediaAsync(MediaSearchDto searchDto);
    Task<IEnumerable<MediaFileDto>> GetSimilarMediaAsync(int mediaId);
    Task<IEnumerable<MediaFileDto>> GetRecentMediaAsync(int count = 10, int? userId = null);
    
    // Validation and security
    Task<MediaValidationResult> ValidateFileAsync(IFormFile file, MediaUploadDto uploadDto);
    Task<bool> CheckMediaAccessAsync(int mediaId, int userId);
    Task<MediaAccessLogDto> LogMediaAccessAsync(int mediaId, int userId, MediaAccessType accessType);
    
    // Bulk operations
    Task<bool> BulkDeleteMediaAsync(IEnumerable<int> mediaIds);
    Task<bool> BulkMoveMediaAsync(IEnumerable<int> mediaIds, int targetCategoryId);
    Task<bool> BulkUpdateMediaAsync(BulkUpdateMediaDto dto);
}

public class MediaService : IMediaService
{
    private readonly IMediaFileRepository _mediaRepository;
    private readonly IFileUploadService _uploadService;
    private readonly IMediaStorageService _storageService;
    private readonly IImageProcessingService _imageProcessor;
    private readonly IVideoProcessingService _videoProcessor;
    private readonly IThumbnailService _thumbnailService;
    private readonly IMediaValidationService _validationService;
    private readonly ILogger<MediaService> _logger;
    private readonly IMapper _mapper;

    public MediaService(
        IMediaFileRepository mediaRepository,
        IFileUploadService uploadService,
        IMediaStorageService storageService,
        IImageProcessingService imageProcessor,
        IVideoProcessingService videoProcessor,
        IThumbnailService thumbnailService,
        IMediaValidationService validationService,
        ILogger<MediaService> logger,
        IMapper mapper)
    {
        _mediaRepository = mediaRepository;
        _uploadService = uploadService;
        _storageService = storageService;
        _imageProcessor = imageProcessor;
        _videoProcessor = videoProcessor;
        _thumbnailService = thumbnailService;
        _validationService = validationService;
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
            if (existingFile != null && uploadDto.AllowDuplicates == false)
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
                FilePath = string.Empty, // Will be set after upload
                Url = string.Empty, // Will be set after upload
                MediaType = mediaType,
                FileExtension = Path.GetExtension(file.FileName).ToLowerInvariant(),
                MimeType = file.ContentType,
                FileSizeBytes = file.Length,
                FileHash = fileHash,
                UploadedBy = uploadDto.UploadedBy,
                UploadedAt = DateTime.UtcNow,
                Status = MediaStatus.Uploading,
                IsPublic = uploadDto.IsPublic,
                CategoryId = uploadDto.CategoryId,
                CollectionId = uploadDto.CollectionId,
                Title = uploadDto.Title,
                Description = uploadDto.Description,
                Tags = uploadDto.Tags,
                AltText = uploadDto.AltText,
                RequiresAuthentication = !uploadDto.IsPublic
            };

            // Save initial record
            var savedMediaFile = await _mediaRepository.AddAsync(mediaFile);

            // Upload to storage
            var uploadResult = await _storageService.UploadFileAsync(file, fileName, uploadDto);
            
            // Update file paths and URLs
            savedMediaFile.FilePath = uploadResult.FilePath;
            savedMediaFile.Url = uploadResult.Url;
            savedMediaFile.Status = MediaStatus.Processing;
            
            await _mediaRepository.UpdateAsync(savedMediaFile);

            // Queue for background processing
            _ = Task.Run(async () => await ProcessMediaFileAsync(savedMediaFile.Id));

            _logger.LogInformation("File uploaded successfully: {MediaId}, Path: {FilePath}", 
                savedMediaFile.Id, savedMediaFile.FilePath);

            return _mapper.Map<MediaFileDto>(savedMediaFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", file.FileName);
            throw;
        }
    }

    public async Task<bool> ProcessMediaFileAsync(int mediaId)
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

                mediaFile.Status = MediaStatus.Ready;
                mediaFile.ProcessedAt = DateTime.UtcNow;
            }
            else
            {
                mediaFile.Status = MediaStatus.Failed;
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
                await _mediaRepository.UpdateAsync(mediaFile);
            }
            
            return false;
        }
    }

    private async Task<bool> ProcessImageAsync(MediaFile mediaFile)
    {
        try
        {
            // Get image from storage
            using var imageStream = await _storageService.GetFileStreamAsync(mediaFile.FilePath);
            
            // Extract image dimensions
            var imageInfo = await _imageProcessor.GetImageInfoAsync(imageStream);
            mediaFile.Width = imageInfo.Width;
            mediaFile.Height = imageInfo.Height;

            // Optimize image if needed
            if (imageInfo.SizeBytes > 2 * 1024 * 1024) // 2MB threshold
            {
                imageStream.Position = 0;
                var optimizedStream = await _imageProcessor.OptimizeImageAsync(imageStream, new ImageOptimizationOptions
                {
                    MaxWidth = 1920,
                    MaxHeight = 1080,
                    Quality = 85,
                    Format = ImageFormat.JPEG
                });

                // Upload optimized version
                var optimizedPath = $"optimized/{mediaFile.FileName}";
                await _storageService.UploadStreamAsync(optimizedStream, optimizedPath);
                
                // Update preview URL
                mediaFile.PreviewUrl = await _storageService.GetFileUrlAsync(optimizedPath);
            }

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
            // Get video info
            var videoInfo = await _videoProcessor.GetVideoInfoAsync(mediaFile.FilePath);
            mediaFile.Width = videoInfo.Width;
            mediaFile.Height = videoInfo.Height;
            mediaFile.DurationSeconds = (int)videoInfo.Duration.TotalSeconds;
            mediaFile.Bitrate = videoInfo.Bitrate;

            // Generate video preview/thumbnail
            var thumbnailPath = $"thumbnails/video/{mediaFile.Id}_preview.jpg";
            await _videoProcessor.GenerateVideoThumbnailAsync(mediaFile.FilePath, thumbnailPath, TimeSpan.FromSeconds(5));
            mediaFile.ThumbnailUrl = await _storageService.GetFileUrlAsync(thumbnailPath);

            // Generate lower resolution preview if video is large
            if (videoInfo.Width > 1280)
            {
                var previewPath = $"previews/video/{mediaFile.Id}_preview.mp4";
                await _videoProcessor.GenerateVideoPreviewAsync(mediaFile.FilePath, previewPath, new VideoPreviewOptions
                {
                    MaxWidth = 1280,
                    MaxHeight = 720,
                    BitrateKbps = 1000
                });
                mediaFile.PreviewUrl = await _storageService.GetFileUrlAsync(previewPath);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing video: {MediaId}", mediaFile.Id);
            return false;
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

            if (mediaFile.Status != MediaStatus.Ready)
            {
                throw new InvalidOperationException($"Media file is not ready for download. Status: {mediaFile.Status}");
            }

            // Log access
            _ = Task.Run(async () => await LogMediaAccessAsync(mediaId, 1, MediaAccessType.Download)); // TODO: Get current user

            return await _storageService.GetFileStreamAsync(mediaFile.FilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file stream for media: {MediaId}", mediaId);
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

            if (mediaFile.IsPublic)
            {
                return mediaFile.Url;
            }

            // Generate temporary signed URL for private files
            var expiration = expirationTime ?? TimeSpan.FromHours(1);
            return await _storageService.GetSignedUrlAsync(mediaFile.FilePath, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file URL for media: {MediaId}", mediaId);
            throw;
        }
    }

    private async Task ExtractMetadataAsync(MediaFile mediaFile)
    {
        try
        {
            var metadata = await _storageService.ExtractMetadataAsync(mediaFile.FilePath);
            
            foreach (var kvp in metadata)
            {
                var metadataEntity = new MediaMetadata
                {
                    MediaFileId = mediaFile.Id,
                    MetadataKey = kvp.Key,
                    MetadataValue = kvp.Value.ToString() ?? string.Empty,
                    DataType = kvp.Value.GetType().Name
                };

                await _mediaRepository.AddMetadataAsync(metadataEntity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting metadata for media: {MediaId}", mediaFile.Id);
        }
    }
}
```

### Frontend Implementation Patterns

#### Media Manager Component
```typescript
// frontend/src/modules/question-bank/media/components/MediaManager.tsx
import React, { useState, useEffect, useCallback } from 'react';
import { Grid, List, Upload, Search, Filter, FolderOpen } from '@mui/icons-material';
import { useMediaManager } from '../hooks/useMediaManager';
import { MediaUploader } from './MediaUploader';
import { MediaGallery } from './MediaGallery';
import { MediaSearch } from './MediaSearch';
import { MediaFilters } from './MediaFilters';
import { MediaPreview } from './MediaPreview';
import { MediaCategoryManager } from './MediaCategoryManager';
import { MediaCollectionManager } from './MediaCollectionManager';
import { Tabs, Tab, TabPanel, Button, IconButton, Tooltip, LinearProgress } from '@mui/material';
import { MediaFileDto, MediaFilterDto, MediaViewMode } from '../types/media.types';

interface MediaManagerProps {
  questionId?: number;
  answerId?: number;
  allowMultipleSelection?: boolean;
  onMediaSelected?: (media: MediaFileDto[]) => void;
  onClose?: () => void;
  readonly?: boolean;
  initialCategory?: number;
  className?: string;
}

export const MediaManager: React.FC<MediaManagerProps> = ({
  questionId,
  answerId,
  allowMultipleSelection = false,
  onMediaSelected,
  onClose,
  readonly = false,
  initialCategory,
  className
}) => {
  const [activeTab, setActiveTab] = useState(0);
  const [viewMode, setViewMode] = useState<MediaViewMode>('grid');
  const [selectedMedia, setSelectedMedia] = useState<MediaFileDto[]>([]);
  const [previewMedia, setPreviewMedia] = useState<MediaFileDto | null>(null);
  const [showUploader, setShowUploader] = useState(false);
  const [showCategoryManager, setShowCategoryManager] = useState(false);
  const [showCollectionManager, setShowCollectionManager] = useState(false);

  const {
    mediaFiles,
    categories,
    collections,
    isLoading,
    uploadProgress,
    error,
    currentFilter,
    totalCount,
    
    refreshMedia,
    uploadFiles,
    deleteMedia,
    updateMedia,
    searchMedia,
    filterMedia,
    createCategory,
    createCollection,
    addToCollection,
    removeFromCollection
  } = useMediaManager({
    initialCategoryId: initialCategory,
    questionId,
    answerId
  });

  useEffect(() => {
    refreshMedia();
  }, [refreshMedia]);

  const handleTabChange = useCallback((event: React.SyntheticEvent, newValue: number) => {
    setActiveTab(newValue);
  }, []);

  const handleMediaSelect = useCallback((media: MediaFileDto) => {
    if (allowMultipleSelection) {
      setSelectedMedia(prev => {
        const isSelected = prev.some(m => m.id === media.id);
        if (isSelected) {
          return prev.filter(m => m.id !== media.id);
        } else {
          return [...prev, media];
        }
      });
    } else {
      setSelectedMedia([media]);
      if (onMediaSelected) {
        onMediaSelected([media]);
      }
    }
  }, [allowMultipleSelection, onMediaSelected]);

  const handleMediaPreview = useCallback((media: MediaFileDto) => {
    setPreviewMedia(media);
  }, []);

  const handleFilesUploaded = useCallback(async (files: File[]) => {
    try {
      await uploadFiles(files, {
        categoryId: initialCategory,
        isPublic: false,
        requiresAuthentication: true
      });
      await refreshMedia();
      setShowUploader(false);
    } catch (error) {
      console.error('Error uploading files:', error);
    }
  }, [uploadFiles, refreshMedia, initialCategory]);

  const handleFilterChange = useCallback(async (filter: MediaFilterDto) => {
    await filterMedia(filter);
  }, [filterMedia]);

  const handleSearchQuery = useCallback(async (query: string) => {
    if (query.trim()) {
      await searchMedia({ query, includeMetadata: true });
    } else {
      await refreshMedia();
    }
  }, [searchMedia, refreshMedia]);

  const handleConfirmSelection = useCallback(() => {
    if (onMediaSelected && selectedMedia.length > 0) {
      onMediaSelected(selectedMedia);
    }
    if (onClose) {
      onClose();
    }
  }, [onMediaSelected, selectedMedia, onClose]);

  const renderMediaContent = useCallback(() => {
    switch (activeTab) {
      case 0: // All Media
        return (
          <MediaGallery
            mediaFiles={mediaFiles}
            viewMode={viewMode}
            selectedMedia={selectedMedia}
            onMediaSelect={handleMediaSelect}
            onMediaPreview={handleMediaPreview}
            onMediaDelete={deleteMedia}
            onMediaUpdate={updateMedia}
            readonly={readonly}
            allowMultipleSelection={allowMultipleSelection}
          />
        );
      case 1: // Categories
        return (
          <MediaCategoryManager
            categories={categories}
            mediaFiles={mediaFiles}
            selectedMedia={selectedMedia}
            onCategoryCreate={createCategory}
            onMediaSelect={handleMediaSelect}
            onMediaPreview={handleMediaPreview}
            readonly={readonly}
          />
        );
      case 2: // Collections
        return (
          <MediaCollectionManager
            collections={collections}
            selectedMedia={selectedMedia}
            onCollectionCreate={createCollection}
            onAddToCollection={addToCollection}
            onRemoveFromCollection={removeFromCollection}
            onMediaSelect={handleMediaSelect}
            onMediaPreview={handleMediaPreview}
            readonly={readonly}
          />
        );
      default:
        return null;
    }
  }, [
    activeTab,
    mediaFiles,
    categories,
    collections,
    viewMode,
    selectedMedia,
    handleMediaSelect,
    handleMediaPreview,
    deleteMedia,
    updateMedia,
    createCategory,
    createCollection,
    addToCollection,
    removeFromCollection,
    readonly,
    allowMultipleSelection
  ]);

  return (
    <div className={`media-manager ${className || ''}`}>
      {/* Header */}
      <div className="media-manager-header p-4 border-b">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold">Media Manager</h2>
          <div className="flex items-center gap-2">
            {/* View Mode Toggle */}
            <div className="flex items-center border rounded">
              <IconButton
                size="small"
                onClick={() => setViewMode('grid')}
                color={viewMode === 'grid' ? 'primary' : 'default'}
              >
                <Grid />
              </IconButton>
              <IconButton
                size="small"
                onClick={() => setViewMode('list')}
                color={viewMode === 'list' ? 'primary' : 'default'}
              >
                <List />
              </IconButton>
            </div>

            {/* Action Buttons */}
            {!readonly && (
              <>
                <Button
                  variant="outlined"
                  startIcon={<Upload />}
                  onClick={() => setShowUploader(true)}
                >
                  Upload
                </Button>
                <Button
                  variant="outlined"
                  startIcon={<FolderOpen />}
                  onClick={() => setShowCategoryManager(true)}
                >
                  Categories
                </Button>
              </>
            )}

            {onClose && (
              <Button variant="outlined" onClick={onClose}>
                Close
              </Button>
            )}
          </div>
        </div>

        {/* Search and Filters */}
        <div className="flex items-center gap-4">
          <div className="flex-1">
            <MediaSearch
              onSearch={handleSearchQuery}
              placeholder="Search media files..."
            />
          </div>
          <MediaFilters
            currentFilter={currentFilter}
            categories={categories}
            onFilterChange={handleFilterChange}
          />
        </div>

        {/* Upload Progress */}
        {uploadProgress > 0 && uploadProgress < 100 && (
          <div className="mt-4">
            <LinearProgress variant="determinate" value={uploadProgress} />
            <p className="text-sm text-gray-600 mt-1">
              Uploading... {uploadProgress.toFixed(0)}%
            </p>
          </div>
        )}

        {/* Selection Info */}
        {selectedMedia.length > 0 && (
          <div className="mt-4 p-3 bg-blue-50 rounded flex items-center justify-between">
            <span className="text-sm text-blue-800">
              {selectedMedia.length} item{selectedMedia.length !== 1 ? 's' : ''} selected
            </span>
            {onMediaSelected && (
              <Button
                variant="contained"
                size="small"
                onClick={handleConfirmSelection}
              >
                Select{selectedMedia.length > 1 ? ` ${selectedMedia.length} items` : ''}
              </Button>
            )}
          </div>
        )}
      </div>

      {/* Tabs */}
      <div className="media-manager-tabs">
        <Tabs value={activeTab} onChange={handleTabChange}>
          <Tab label={`All Media (${totalCount})`} />
          <Tab label={`Categories (${categories.length})`} />
          <Tab label={`Collections (${collections.length})`} />
        </Tabs>
      </div>

      {/* Content */}
      <div className="media-manager-content flex-1 overflow-auto">
        {error && (
          <div className="p-4 bg-red-50 border border-red-200 rounded m-4">
            <p className="text-red-800">{error}</p>
            <Button
              variant="outlined"
              size="small"
              onClick={refreshMedia}
              className="mt-2"
            >
              Retry
            </Button>
          </div>
        )}

        {isLoading ? (
          <div className="flex items-center justify-center p-8">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
            <span className="ml-2">Loading media...</span>
          </div>
        ) : (
          renderMediaContent()
        )}
      </div>

      {/* Modals */}
      {showUploader && (
        <MediaUploader
          onFilesUploaded={handleFilesUploaded}
          onClose={() => setShowUploader(false)}
          allowedTypes={['image', 'video', 'audio', 'document']}
          maxFileSize={50 * 1024 * 1024} // 50MB
          maxFiles={10}
        />
      )}

      {previewMedia && (
        <MediaPreview
          media={previewMedia}
          onClose={() => setPreviewMedia(null)}
          onMediaUpdate={updateMedia}
          onMediaDelete={deleteMedia}
          readonly={readonly}
        />
      )}

      {showCategoryManager && (
        <MediaCategoryManager
          categories={categories}
          onClose={() => setShowCategoryManager(false)}
          onCategoryCreate={createCategory}
          readonly={readonly}
        />
      )}

      {showCollectionManager && (
        <MediaCollectionManager
          collections={collections}
          onClose={() => setShowCollectionManager(false)}
          onCollectionCreate={createCollection}
          readonly={readonly}
        />
      )}
    </div>
  );
};
```

#### Media Upload Hook
```typescript
// frontend/src/modules/question-bank/media/hooks/useMediaUpload.tsx
import { useState, useCallback } from 'react';
import { uploadService } from '../services/uploadService';
import { validationService } from '../services/validationService';
import { MediaUploadDto, MediaValidationResult, UploadProgressDto } from '../types/upload.types';

interface UseMediaUploadResult {
  uploadProgress: number;
  isUploading: boolean;
  uploadError: string | null;
  validationResults: MediaValidationResult[];
  
  uploadFiles: (files: File[], options: MediaUploadOptions) => Promise<MediaFileDto[]>;
  validateFiles: (files: File[]) => Promise<MediaValidationResult[]>;
  cancelUpload: () => void;
  clearError: () => void;
}

interface MediaUploadOptions {
  categoryId?: number;
  collectionId?: number;
  isPublic?: boolean;
  requiresAuthentication?: boolean;
  title?: string;
  description?: string;
  tags?: string;
  altText?: string;
  allowDuplicates?: boolean;
}

export const useMediaUpload = (): UseMediaUploadResult => {
  const [uploadProgress, setUploadProgress] = useState(0);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [validationResults, setValidationResults] = useState<MediaValidationResult[]>([]);
  const [abortController, setAbortController] = useState<AbortController | null>(null);

  const validateFiles = useCallback(async (files: File[]): Promise<MediaValidationResult[]> => {
    try {
      const results = await Promise.all(
        files.map(file => validationService.validateFile(file))
      );
      setValidationResults(results);
      return results;
    } catch (error) {
      console.error('Error validating files:', error);
      return [];
    }
  }, []);

  const uploadFiles = useCallback(async (
    files: File[], 
    options: MediaUploadOptions
  ): Promise<MediaFileDto[]> => {
    try {
      setIsUploading(true);
      setUploadError(null);
      setUploadProgress(0);

      // Create abort controller for cancellation
      const controller = new AbortController();
      setAbortController(controller);

      // Validate files first
      const validationResults = await validateFiles(files);
      const validFiles = files.filter((_, index) => validationResults[index]?.isValid);
      
      if (validFiles.length === 0) {
        throw new Error('No valid files to upload');
      }

      const uploadedFiles: MediaFileDto[] = [];
      const totalFiles = validFiles.length;

      for (let i = 0; i < validFiles.length; i++) {
        if (controller.signal.aborted) {
          throw new Error('Upload cancelled');
        }

        const file = validFiles[i];
        const uploadDto: MediaUploadDto = {
          file,
          categoryId: options.categoryId,
          collectionId: options.collectionId,
          isPublic: options.isPublic || false,
          requiresAuthentication: options.requiresAuthentication ?? true,
          title: options.title || file.name,
          description: options.description,
          tags: options.tags,
          altText: options.altText,
          allowDuplicates: options.allowDuplicates || false,
          uploadedBy: 1 // TODO: Get current user ID
        };

        // Upload individual file with progress tracking
        const uploadedFile = await uploadService.uploadFile(uploadDto, {
          onProgress: (progress: number) => {
            const overallProgress = ((i + progress / 100) / totalFiles) * 100;
            setUploadProgress(overallProgress);
          },
          signal: controller.signal
        });

        uploadedFiles.push(uploadedFile);
      }

      setUploadProgress(100);
      return uploadedFiles;

    } catch (error) {
      if (error instanceof Error) {
        if (error.name === 'AbortError') {
          setUploadError('Upload cancelled');
        } else {
          setUploadError(error.message);
        }
      } else {
        setUploadError('An unknown error occurred during upload');
      }
      throw error;
    } finally {
      setIsUploading(false);
      setAbortController(null);
    }
  }, [validateFiles]);

  const cancelUpload = useCallback(() => {
    if (abortController) {
      abortController.abort();
    }
  }, [abortController]);

  const clearError = useCallback(() => {
    setUploadError(null);
  }, []);

  return {
    uploadProgress,
    isUploading,
    uploadError,
    validationResults,
    
    uploadFiles,
    validateFiles,
    cancelUpload,
    clearError
  };
};
```

## 🔄 Integration Validation Loops

### Backend Validation Loop
```bash
# 1. Entity Validation
dotnet ef migrations add AddMediaManagementEntities
dotnet ef database update

# 2. Storage Service Testing
dotnet test --filter Category=MediaStorageService

# 3. Processing Service Testing
dotnet test --filter Category=MediaProcessingService

# 4. Upload Service Testing
dotnet test --filter Category=MediaUploadService

# 5. Controller Testing
dotnet test --filter Category=MediaController

# 6. Integration Testing
dotnet test --filter Category=MediaIntegration

# 7. File Processing Testing
dotnet test --filter Category=FileProcessing
```

### Frontend Validation Loop
```bash
# 1. Component Testing
npm run test -- --testPathPattern=media

# 2. Upload Hook Testing
npm run test -- --testPathPattern=useMediaUpload

# 3. Service Testing
npm run test -- --testPathPattern=mediaService

# 4. Type Checking
npx tsc --noEmit --project tsconfig.json

# 5. Integration Testing
npm run test:integration -- --testPathPattern=media

# 6. E2E Upload Testing
npm run test:e2e -- --testPathPattern=media-upload
```

### Storage Integration Validation Loop
```bash
# 1. File Upload Test
curl -X POST "http://localhost:5000/api/media/upload" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@test-image.jpg" \
  -F "categoryId=1"
# Expected: 201 Created with media file data

# 2. File Download Test
curl -X GET "http://localhost:5000/api/media/1/download"
# Expected: 200 OK with file content

# 3. Thumbnail Generation Test
curl -X GET "http://localhost:5000/api/media/1/thumbnail/medium"
# Expected: 200 OK with thumbnail image

# 4. Processing Status Test
curl -X GET "http://localhost:5000/api/media/1/processing-status"
# Expected: 200 OK with processing status
```

## 🎯 Success Criteria

### Functional Requirements ✅
- [ ] File upload with multiple format support (images, videos, audio, documents)
- [ ] Automatic thumbnail generation for all media types
- [ ] Video processing with preview generation
- [ ] Image optimization and resizing
- [ ] Media categorization and collections
- [ ] Search functionality with metadata indexing
- [ ] Access control and security permissions
- [ ] Bulk operations (upload, delete, move)
- [ ] Media validation and virus scanning
- [ ] Duplicate detection and handling
- [ ] Integration with question and answer entities

### Performance Requirements ✅
- [ ] File upload < 30 seconds for files up to 50MB
- [ ] Thumbnail generation < 10 seconds for images
- [ ] Video processing < 2 minutes for 5-minute videos
- [ ] Media search results < 2 seconds
- [ ] File download streaming without delays
- [ ] Efficient storage with CDN integration
- [ ] Background processing for large files

### Security Requirements ✅
- [ ] File type validation and restriction
- [ ] Virus scanning for all uploads
- [ ] Access control based on user roles
- [ ] Secure file URLs with expiration
- [ ] Input sanitization and validation
- [ ] Audit logging for media operations
- [ ] Data encryption at rest and in transit

### Integration Requirements ✅
- [ ] Seamless integration with question bank module
- [ ] Connection with existing Media entities
- [ ] Azure Blob Storage integration
- [ ] User management system integration
- [ ] Audit logging system integration
- [ ] Mobile-responsive media interface
- [ ] Question and answer media attachments

## ⚠ Anti-Patterns to Avoid

### Backend Anti-Patterns ❌
```csharp
// ❌ DON'T: Loading entire files into memory
public async Task<byte[]> ProcessLargeVideo(IFormFile file)
{
    var bytes = new byte[file.Length]; // Memory exhaustion for large files
    await file.OpenReadStream().ReadAsync(bytes);
    return ProcessVideo(bytes);
}

// ❌ DON'T: Synchronous file operations
public MediaFile UploadFile(IFormFile file)
{
    var bytes = file.OpenReadStream().ReadAllBytes(); // Blocking operation
    var result = _storageService.Upload(bytes); // Blocking upload
    return result;
}

// ❌ DON'T: Missing file validation
public async Task<MediaFile> UploadFile(IFormFile file)
{
    // No validation - security risk
    var path = Path.Combine("uploads", file.FileName);
    using var stream = File.Create(path);
    await file.CopyToAsync(stream);
}

// ❌ DON'T: Direct file path exposure
public class MediaController
{
    public IActionResult GetFile(int id)
    {
        var media = _mediaService.GetMedia(id);
        return File(media.FilePath, media.MimeType); // Exposes internal paths
    }
}
```

### Frontend Anti-Patterns ❌
```typescript
// ❌ DON'T: Loading all media at once
const MediaGallery = () => {
  const [allMedia, setAllMedia] = useState([]);
  
  useEffect(() => {
    loadAllMedia().then(setAllMedia); // Could load thousands of files
  }, []);
};

// ❌ DON'T: Missing upload progress
const uploadFile = async (file) => {
  const formData = new FormData();
  formData.append('file', file);
  
  // No progress tracking for large files
  const response = await fetch('/api/upload', {
    method: 'POST',
    body: formData
  });
};

// ❌ DON'T: Client-side file processing
const processImage = (file) => {
  const canvas = document.createElement('canvas');
  const ctx = canvas.getContext('2d');
  
  // Processing large images on client - performance issue
  return new Promise(resolve => {
    const img = new Image();
    img.onload = () => {
      canvas.width = img.width;
      canvas.height = img.height;
      ctx.drawImage(img, 0, 0);
      // Heavy processing on main thread
      resolve(canvas.toDataURL());
    };
  });
};

// ❌ DON'T: Uncontrolled memory usage
const MediaPreview = ({ mediaId }) => {
  const [mediaUrl, setMediaUrl] = useState('');
  
  useEffect(() => {
    // Loading full resolution without cleanup
    fetch(`/api/media/${mediaId}/full`)
      .then(response => response.blob())
      .then(blob => setMediaUrl(URL.createObjectURL(blob)));
    // Missing URL.revokeObjectURL cleanup
  }, [mediaId]);
};
```

### Architecture Anti-Patterns ❌
```csharp
// ❌ DON'T: Tight coupling to storage provider
public class MediaService
{
    private readonly AzureBlobClient _blobClient; // Tightly coupled
    
    public async Task<string> UploadFile(IFormFile file)
    {
        return await _blobClient.UploadBlobAsync(file.FileName, file.OpenReadStream());
    }
}

// ❌ DON'T: Missing abstraction layers
public class QuestionService
{
    public async Task<Question> CreateQuestion(CreateQuestionDto dto)
    {
        var question = new Question(dto);
        
        // Direct file system access
        var imagePath = Path.Combine("wwwroot/images", dto.ImageFile.FileName);
        using var stream = File.Create(imagePath);
        await dto.ImageFile.CopyToAsync(stream);
        
        question.ImageUrl = $"/images/{dto.ImageFile.FileName}";
    }
}

// ❌ DON'T: Inconsistent error handling
public class MediaController
{
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            var result = await _mediaService.UploadAsync(file);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest(); // No error details or logging
        }
    }
}
```

## 📚 Implementation Guide

### Phase 1: Core Backend Media System (Week 1)
1. **Entity Implementation**
   - Create MediaFile, MediaCategory, MediaCollection entities
   - Configure Dapper relationships

2. **Storage Abstraction**
   - Implement storage provider interfaces
   - Create Azure Blob Storage provider
   - Add local file storage provider

3. **Basic Operations**
   - Implement file upload service
   - Add file validation service
   - Create media repository layer

### Phase 2: Media Processing (Week 2)
1. **Image Processing**
   - Implement image optimization
   - Add thumbnail generation
   - Create image metadata extraction

2. **Video Processing**
   - Add video information extraction
   - Implement video thumbnail generation
   - Create video preview generation

3. **Background Processing**
   - Setup background job processing
   - Implement media processing queue
   - Add processing status tracking

### Phase 3: Frontend Media Interface (Week 3)
1. **Core Components**
   - Build media manager component
   - Create upload interface
   - Implement media gallery

2. **Interactive Features**
   - Add media preview and editing
   - Implement search and filtering
   - Create category management

3. **Integration Features**
   - Connect with question forms
   - Add media selection dialogs
   - Implement drag & drop upload

### Phase 4: Advanced Features & Integration (Week 4)
1. **Advanced Processing**
   - Add video conversion capabilities
   - Implement advanced image editing
   - Create media optimization tools

2. **Security & Performance**
   - Implement access control
   - Add virus scanning
   - Optimize performance and caching

3. **Testing & Deployment**
   - Comprehensive testing suite
   - Performance optimization
   - Production deployment preparation

## 🛡️ Security Checklist
- [ ] File type validation and whitelist
- [ ] File size limits enforced
- [ ] Virus scanning integration
- [ ] Access control implementation
- [ ] Secure file URL generation
- [ ] Input sanitization and validation
- [ ] Audit logging for media operations
- [ ] Data encryption configuration

## 📊 Performance Checklist
- [ ] Streaming for large file uploads
- [ ] Background processing for media
- [ ] CDN integration for file delivery
- [ ] Thumbnail generation optimization
- [ ] Database indexing for media queries
- [ ] Caching for frequently accessed media
- [ ] Memory-efficient file processing

## 🌐 Integration Points

```yaml
DATABASE:
  - tables: "MediaFiles, MediaCategories, MediaCollections, MediaMetadata"
  - indexes: "IX_Media_Category, IX_Media_Type, IX_Media_Hash"
  - foreign_keys: "Questions, Answers, Users"

STORAGE:
  - azure_blob: "Primary storage for production"
  - local_files: "Development and testing"
  - cdn: "Content delivery optimization"

CONFIG:
  - backend: "Register media services and repositories"
  - frontend: "Configure upload limits and file types"
  - storage: "Azure Blob Storage connection strings"

ROUTES:
  - api: "/api/media, /api/media/upload, /api/media/categories"
  - frontend: "/question-bank/media, /media/manager"

PROCESSING:
  - background_jobs: "Media processing and optimization"
  - queues: "Upload and processing job queues"
  - workers: "Media processing workers"

INTEGRATION:
  - questions: "Question media attachments"
  - answers: "Answer media attachments"
  - users: "User media permissions"
  - audit: "Media operation logging"
```

This comprehensive PRP provides a complete implementation guide for the question bank media management system that handles all aspects of multimedia content management while maintaining security, performance, and integration with the existing system architecture.
