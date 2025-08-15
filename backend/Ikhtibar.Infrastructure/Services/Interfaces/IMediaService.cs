using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.Enums;
using Microsoft.AspNetCore.Http;

namespace Ikhtibar.Infrastructure.Services.Interfaces;

/// <summary>
/// Core media management service interface
/// Provides comprehensive media operations including upload, processing, management, and access control
/// </summary>
public interface IMediaService
{
    // File operations
    Task<MediaFileDto> UploadFileAsync(IFormFile file, MediaUploadDto uploadDto);
    Task<MediaFileDto> GetMediaFileAsync(Guid mediaId);
    Task<PagedResult<MediaFileDto>> GetMediaFilesAsync(MediaFileSearchDto filter);
    Task<bool> DeleteMediaFileAsync(Guid mediaId);
    Task<MediaFileDto> UpdateMediaFileAsync(Guid mediaId, UpdateMediaFileDto dto);
    
    // Download and streaming
    Task<Stream> GetFileStreamAsync(Guid mediaId);
    Task<byte[]> GetFileBytesAsync(Guid mediaId);
    Task<string> GetFileUrlAsync(Guid mediaId, TimeSpan? expirationTime = null);
    Task<string> GetThumbnailUrlAsync(Guid mediaId, ThumbnailSize size = ThumbnailSize.Medium);
    
    // Processing
    Task<bool> ProcessMediaFileAsync(Guid mediaId);
    Task<MediaProcessingStatusDto> GetProcessingStatusAsync(Guid mediaId);
    Task<bool> RegenerateMediaProcessingAsync(Guid mediaId);
    
    // Categories and collections
    Task<IEnumerable<MediaCategoryDto>> GetCategoriesAsync();
    Task<IEnumerable<MediaCollectionDto>> GetCollectionsAsync(Guid? userId = null);
    Task<MediaCollectionDto> CreateCollectionAsync(CreateMediaCollectionDto dto);
    Task<bool> AddMediaToCollectionAsync(Guid mediaId, Guid collectionId);
    Task<bool> RemoveMediaFromCollectionAsync(Guid mediaId, Guid collectionId);
    
    // Search and filtering
    Task<PagedResult<MediaFileDto>> SearchMediaAsync(MediaFileSearchDto searchDto);
    Task<IEnumerable<MediaFileDto>> GetSimilarMediaAsync(Guid mediaId);
    Task<IEnumerable<MediaFileDto>> GetRecentMediaAsync(int count = 10, Guid? userId = null);
    
    // Validation and security
    Task<MediaValidationResult> ValidateFileAsync(IFormFile file, MediaUploadDto uploadDto);
    Task<bool> CheckMediaAccessAsync(Guid mediaId, Guid userId);
    Task<MediaAccessLogDto> LogMediaAccessAsync(Guid mediaId, Guid userId, MediaAccessType accessType);
    
    // Bulk operations
    Task<bool> BulkDeleteMediaAsync(IEnumerable<Guid> mediaIds);
    Task<bool> BulkMoveMediaAsync(IEnumerable<Guid> mediaIds, Guid targetCategoryId);
    Task<bool> BulkUpdateMediaAsync(BulkMediaFileOperationDto dto);
}

/// <summary>
/// Media access types for logging
/// </summary>
public enum MediaAccessType
{
    View = 1,
    Download = 2,
    Stream = 3,
    Edit = 4,
    Delete = 5
}

/// <summary>
/// Media validation result for file uploads
/// </summary>
public class MediaValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public long FileSizeBytes { get; set; }
    public string DetectedMediaType { get; set; } = string.Empty;
    public bool IsVirusScanned { get; set; }
    public bool IsVirusFree { get; set; }
}

/// <summary>
/// Media processing status information
/// </summary>
public class MediaProcessingStatusDto
{
    public Guid MediaId { get; set; }
    public MediaFileStatus Status { get; set; }
    public int ProgressPercentage { get; set; }
    public string CurrentStep { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> ProcessingMetadata { get; set; } = new();
}

/// <summary>
/// Media upload request data
/// </summary>
public class MediaUploadDto
{
    public Guid? CategoryId { get; set; }
    public Guid? CollectionId { get; set; }
    public bool IsPublic { get; set; } = false;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public string? AltText { get; set; }
    public bool AllowDuplicates { get; set; } = false;
    public Guid UploadedBy { get; set; }
    public bool RequiresAuthentication { get; set; } = true;
    public Dictionary<string, object>? CustomMetadata { get; set; }
}
