using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.Enums;

using AccessType = Ikhtibar.Shared.Entities.AccessType;
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
    Task<MediaFileDto> GetMediaFileAsync(int mediaId);
    Task<PagedResult<MediaFileDto>> GetMediaFilesAsync(MediaFileSearchDto filter);
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
    Task<PagedResult<MediaFileDto>> SearchMediaAsync(MediaFileSearchDto searchDto);
    Task<IEnumerable<MediaFileDto>> GetSimilarMediaAsync(int mediaId);
    Task<IEnumerable<MediaFileDto>> GetRecentMediaAsync(int count = 10, int? userId = null);
    
    // Validation and security
    Task<MediaValidationResult> ValidateFileAsync(IFormFile file, MediaUploadDto uploadDto);
    Task<bool> CheckMediaAccessAsync(int mediaId, int userId);
    Task<MediaAccessLogDto> LogMediaAccessAsync(int mediaId, int userId, AccessType accessType);
    
    // Bulk operations
    Task<bool> BulkDeleteMediaAsync(IEnumerable<int> mediaIds);
    Task<bool> BulkMoveMediaAsync(IEnumerable<int> mediaIds, int targetCategoryId);
    Task<bool> BulkUpdateMediaAsync(BulkMediaFileOperationDto dto);
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
    public int MediaId { get; set; }
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
    public int? CategoryId { get; set; }
    public int? CollectionId { get; set; }
    public bool IsPublic { get; set; } = false;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public string? AltText { get; set; }
    public bool AllowDuplicates { get; set; } = false;
    public int UploadedBy { get; set; }
    public bool RequiresAuthentication { get; set; } = true;
    public Dictionary<string, object>? CustomMetadata { get; set; }
}
