using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Media file management service interface for file operations
/// Handles CRUD operations, search, metadata management, and file processing
/// </summary>
public interface IMediaFileService
{
    /// <summary>
    /// Retrieves a media file by its unique identifier
    /// </summary>
    Task<MediaFileDto?> GetByIdAsync(int id);

    /// <summary>
    /// Searches media files with filtering and pagination
    /// </summary>
    Task<PagedResult<MediaFileDto>> SearchAsync(MediaFileSearchDto searchDto);

    /// <summary>
    /// Creates a new media file record
    /// </summary>
    Task<MediaFileDto> CreateAsync(CreateMediaFileDto createDto);

    /// <summary>
    /// Updates an existing media file record
    /// </summary>
    Task<MediaFileDto> UpdateAsync(int id, UpdateMediaFileDto updateDto);

    /// <summary>
    /// Soft deletes a media file record
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Gets all media files
    /// </summary>
    Task<List<MediaFileDto>> GetAllAsync();

    /// <summary>
    /// Gets media files by category
    /// </summary>
    Task<List<MediaFileDto>> GetByCategoryAsync(int categoryId);

    /// <summary>
    /// Gets media files by type
    /// </summary>
    Task<List<MediaFileDto>> GetByTypeAsync(MediaFileType mediaType);

    /// <summary>
    /// Updates media file status
    /// </summary>
    Task<bool> UpdateStatusAsync(int id, MediaFileStatus status);

    /// <summary>
    /// Gets media files by user
    /// </summary>
    Task<List<MediaFileDto>> GetByUserAsync(int userId);
}

/// <summary>
/// Service interface for media category management operations
/// Handles hierarchical category structure and organization
/// </summary>
public interface IMediaCategoryService
{
    // Category CRUD operations
    Task<MediaCategoryDto> GetByIdAsync(int id);
    Task<List<MediaCategoryDto>> GetAllAsync();
    Task<MediaCategoryDto> CreateAsync(MediaCategoryDto createDto);
    Task<MediaCategoryDto> UpdateAsync(int id, MediaCategoryDto updateDto);
    Task<bool> DeleteAsync(int id);
}

/// <summary>
/// Service interface for media metadata management operations
/// Handles extraction, storage, and querying of file metadata
/// </summary>
public interface IMediaMetadataService
{
    // Metadata CRUD operations
    Task<List<MediaMetadataDto>> GetByMediaFileIdAsync(int mediaFileId);
    Task<MediaMetadataDto> CreateAsync(MediaMetadataDto createDto);
    Task<MediaMetadataDto> UpdateAsync(int id, MediaMetadataDto updateDto);
    Task<bool> DeleteAsync(int id);
}

/// <summary>
/// Service interface for media thumbnail management operations
/// Handles thumbnail generation, storage, and optimization
/// </summary>
public interface IMediaThumbnailService
{
    // Thumbnail CRUD operations
    Task<List<MediaThumbnailDto>> GetByMediaFileIdAsync(int mediaFileId);
    Task<MediaThumbnailDto?> GetByIdAsync(int id);
    Task<MediaThumbnailDto> CreateAsync(MediaThumbnailDto createDto);
    Task<bool> DeleteAsync(int id);
}

/// <summary>
/// Service interface for media access logging and analytics
/// Tracks file access patterns and usage statistics
/// </summary>
public interface IMediaAccessLogService
{
    // Access logging
    Task<MediaAccessLogDto> LogAccessAsync(MediaAccessLogDto logDto);
    Task<List<MediaAccessLogDto>> GetByMediaFileIdAsync(int mediaFileId);
    Task<List<MediaAccessLogDto>> GetByUserIdAsync(int userId);
}

/// <summary>
/// Service interface for media collection management operations
/// Handles playlists, albums, galleries, and other media groupings
/// </summary>
public interface IMediaCollectionService
{
    // Collection CRUD operations
    Task<MediaCollectionDto> GetByIdAsync(int id);
    Task<List<MediaCollectionDto>> GetAllAsync();
    Task<MediaCollectionDto> CreateAsync(MediaCollectionDto createDto);
    Task<MediaCollectionDto> UpdateAsync(int id, MediaCollectionDto updateDto);
    Task<bool> DeleteAsync(int id);
}

/// <summary>
/// Service interface for media processing job management
/// Handles background processing tasks for media files
/// </summary>
public interface IMediaProcessingJobService
{
    // Job CRUD operations
    Task<MediaProcessingJobDto> GetByIdAsync(int id);
    Task<List<MediaProcessingJobDto>> GetByMediaFileIdAsync(int mediaFileId);
    Task<MediaProcessingJobDto> CreateAsync(MediaProcessingJobDto createDto);
    Task<MediaProcessingJobDto> UpdateAsync(int id, MediaProcessingJobDto updateDto);
    Task<bool> DeleteAsync(int id);
}
