using System.ComponentModel.DataAnnotations;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Shared.DTOs;

#region MediaFile DTOs

/// <summary>
/// DTO for media file responses with complete information
/// Aligned with MediaFile entity properties
/// </summary>
public class MediaFileDto
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StorageFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public MediaFileType MediaType { get; set; }
    public MediaFileStatus Status { get; set; }
    public int? CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string? ChecksumSha256 { get; set; }
    public int UploadedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    // Navigation properties
    public MediaCategoryDto? Category { get; set; }
    public List<MediaThumbnailDto> Thumbnails { get; set; } = new();
    public MediaMetadataDto? Metadata { get; set; }
}

/// <summary>
/// DTO for creating new media files
/// </summary>
public class CreateMediaFileDto
{
    [Required]
    [StringLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string StorageFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    [Required]
    [Range(1, long.MaxValue)]
    public long FileSizeBytes { get; set; }

    [Required]
    [StringLength(500)]
    public string StoragePath { get; set; } = string.Empty;

    [Required]
    public MediaFileType MediaType { get; set; }

    public int? CategoryId { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string? AltText { get; set; }

    [StringLength(500)]
    public string? Tags { get; set; }

    public string? ChecksumSha256 { get; set; }

    [Required]
    public int UploadedByUserId { get; set; }
}

/// <summary>
/// DTO for updating existing media files
/// </summary>
public class UpdateMediaFileDto
{
    [StringLength(255)]
    public string? Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string? AltText { get; set; }

    [StringLength(500)]
    public string? Tags { get; set; }

    public int? CategoryId { get; set; }

    public MediaFileStatus? Status { get; set; }

    public bool? IsPublic { get; set; }
}

/// <summary>
/// DTO for searching and filtering media files
/// </summary>
public class MediaFileSearchDto
{
    public string? SearchTerm { get; set; }
    public MediaFileType? MediaType { get; set; }
    public int? CategoryId { get; set; }
    public MediaFileStatus? Status { get; set; }
    public int? UploadedByUserId { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// DTO for bulk operations on media files
/// </summary>
public class BulkMediaFileOperationDto
{
    [Required]
    public List<int> MediaFileIds { get; set; } = new();

    [Required]
    public string Operation { get; set; } = string.Empty; // "delete", "move", "categorize", "status_change"

    public int? TargetCategoryId { get; set; }
    public MediaFileStatus? TargetStatus { get; set; }
    public string? Tags { get; set; }
}

#endregion

#region MediaCategory DTOs

/// <summary>
/// DTO for media category responses
/// </summary>
public class MediaCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int? ParentId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    // Navigation properties
    public MediaCategoryDto? Parent { get; set; }
    public List<MediaCategoryDto> Children { get; set; } = new();
    public int MediaFileCount { get; set; }
}

/// <summary>
/// DTO for creating new media categories
/// </summary>
public class CreateMediaCategoryDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Icon { get; set; }

    [StringLength(7)]
    public string? Color { get; set; }

    public int? ParentId { get; set; }

    public int SortOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating existing media categories
/// </summary>
public class UpdateMediaCategoryDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Icon { get; set; }

    [StringLength(7)]
    public string? Color { get; set; }

    public int? ParentId { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
}

#endregion

#region MediaMetadata DTOs

/// <summary>
/// DTO for media metadata responses
/// </summary>
public class MediaMetadataDto
{
    public int Id { get; set; }
    public int MediaFileId { get; set; }
    public string MetadataJson { get; set; } = string.Empty;
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Quality { get; set; }
    public string? Format { get; set; }
    public string? ColorProfile { get; set; }
    public string? Compression { get; set; }
    public decimal? FrameRate { get; set; }
    public string? AudioCodec { get; set; }
    public string? VideoCodec { get; set; }
    public int? Bitrate { get; set; }
    public string? CameraModel { get; set; }
    public DateTime? DateTaken { get; set; }
    public string? GpsLocation { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

/// <summary>
/// DTO for creating/updating media metadata
/// </summary>
public class CreateMediaMetadataDto
{
    [Required]
    public int MediaFileId { get; set; }

    public string MetadataJson { get; set; } = "{}";
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Quality { get; set; }
    public string? Format { get; set; }
    public string? ColorProfile { get; set; }
    public string? Compression { get; set; }
    public decimal? FrameRate { get; set; }
    public string? AudioCodec { get; set; }
    public string? VideoCodec { get; set; }
    public int? Bitrate { get; set; }
    public string? CameraModel { get; set; }
    public DateTime? DateTaken { get; set; }
    public string? GpsLocation { get; set; }
}

#endregion

#region MediaThumbnail DTOs

/// <summary>
/// DTO for media thumbnail responses
/// </summary>
public class MediaThumbnailDto
{
    public int Id { get; set; }
    public int MediaFileId { get; set; }
    public ThumbnailSize Size { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for creating thumbnails
/// </summary>
public class CreateMediaThumbnailDto
{
    [Required]
    public int MediaFileId { get; set; }

    [Required]
    public ThumbnailSize Size { get; set; }

    [Required]
    [Range(1, 4000)]
    public int Width { get; set; }

    [Required]
    [Range(1, 4000)]
    public int Height { get; set; }

    [Required]
    [StringLength(500)]
    public string StoragePath { get; set; } = string.Empty;

    [Required]
    [Range(1, long.MaxValue)]
    public long FileSizeBytes { get; set; }

    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;
}

#endregion

#region MediaAccessLog DTOs

/// <summary>
/// DTO for media access log responses
/// </summary>
public class MediaAccessLogDto
{
    public int Id { get; set; }
    public int MediaFileId { get; set; }
    public int? UserId { get; set; }
    public MediaAccessAction Action { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? Referrer { get; set; }
    public DateTime AccessedAt { get; set; }

    // Navigation properties
    public MediaFileDto? MediaFile { get; set; }
    public string? UserName { get; set; }
}

/// <summary>
/// DTO for creating access log entries
/// </summary>
public class CreateMediaAccessLogDto
{
    [Required]
    public int MediaFileId { get; set; }

    public int? UserId { get; set; }

    [Required]
    public MediaAccessAction Action { get; set; }

    [Required]
    [StringLength(45)]
    public string IpAddress { get; set; } = string.Empty;

    [StringLength(500)]
    public string UserAgent { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Referrer { get; set; }
}

#endregion

#region MediaCollection DTOs

/// <summary>
/// DTO for media collection responses
/// </summary>
public class MediaCollectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MediaCollectionType Type { get; set; }
    public bool IsPublic { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    // Navigation properties
    public List<MediaFileDto> MediaFiles { get; set; } = new();
    public int MediaFileCount { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating media collections
/// </summary>
public class CreateMediaCollectionDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public MediaCollectionType Type { get; set; }

    public bool IsPublic { get; set; } = true;

    [Required]
    public int CreatedByUserId { get; set; }

    public List<int> MediaFileIds { get; set; } = new();
}

/// <summary>
/// DTO for updating media collections
/// </summary>
public class UpdateMediaCollectionDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public bool IsPublic { get; set; }

    public List<int> MediaFileIds { get; set; } = new();
}

#endregion

#region MediaProcessingJob DTOs

/// <summary>
/// DTO for media processing job responses
/// </summary>
public class MediaProcessingJobDto
{
    public int Id { get; set; }
    public int MediaFileId { get; set; }
    public ProcessingJobType JobType { get; set; }
    public ProcessingJobStatus Status { get; set; }
    public string? Parameters { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public int Priority { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    // Navigation properties
    public MediaFileDto? MediaFile { get; set; }
}

/// <summary>
/// DTO for creating media processing jobs
/// </summary>
public class CreateMediaProcessingJobDto
{
    [Required]
    public int MediaFileId { get; set; }

    [Required]
    public ProcessingJobType JobType { get; set; }

    public string? Parameters { get; set; }

    [Range(1, 10)]
    public int Priority { get; set; } = 5;
}

/// <summary>
/// DTO for updating processing job status
/// </summary>
public class UpdateMediaProcessingJobDto
{
    [Required]
    public ProcessingJobStatus Status { get; set; }

    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

#endregion

#region Supporting DTOs

/// <summary>
/// Validation result with error details
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// Storage statistics for media files
/// </summary>
public class MediaStorageStatistics
{
    public int TotalFiles { get; set; }
    public long TotalSizeBytes { get; set; }
    public Dictionary<MediaFileType, int> FilesByType { get; set; } = new();
    public Dictionary<MediaFileType, long> StorageByType { get; set; } = new();
    public Dictionary<MediaFileStatus, int> FilesByStatus { get; set; } = new();
}

/// <summary>
/// Access analytics for media files
/// </summary>
public class MediaAccessAnalytics
{
    public int MediaFileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public int TotalAccesses { get; set; }
    public int UniqueUsers { get; set; }
    public DateTime LastAccessed { get; set; }
    public Dictionary<MediaAccessAction, int> AccessesByAction { get; set; } = new();
    public Dictionary<DateTime, int> DailyAccesses { get; set; } = new();
}

#endregion

#region Search Statistics

/// <summary>
/// DTO for search statistics and analytics
/// </summary>
public class SearchStatistics
{
    public int TotalMediaFiles { get; set; }
    public int TotalCategories { get; set; }
    public int TotalCollections { get; set; }
    public DateTime LastUpdated { get; set; }
}

#endregion

#region MediaCollectionItem DTOs

/// <summary>
/// DTO for media collection item responses
/// </summary>
public class MediaCollectionItemDto
{
    public int Id { get; set; }
    public int CollectionId { get; set; }
    public int MediaFileId { get; set; }
    public int SortOrder { get; set; }
    public string? Caption { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public MediaFileDto? MediaFile { get; set; }
    public MediaCollectionDto? Collection { get; set; }
}

#endregion
