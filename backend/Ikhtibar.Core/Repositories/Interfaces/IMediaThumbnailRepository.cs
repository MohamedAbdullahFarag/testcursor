using Ikhtibar.Core.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for MediaThumbnail entity operations
/// Provides specialized methods for thumbnail management and optimization
/// </summary>
public interface IMediaThumbnailRepository : IRepository<MediaThumbnail>
{
    /// <summary>
    /// Gets all thumbnails for a specific media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="statusFilter">Optional status filter</param>
    /// <returns>Collection of thumbnails for the media file</returns>
    Task<IEnumerable<MediaThumbnail>> GetByMediaFileAsync(Guid mediaFileId, ThumbnailStatus? statusFilter = null);

    /// <summary>
    /// Gets thumbnail by media file and size
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="size">Thumbnail size category</param>
    /// <param name="format">Optional format filter</param>
    /// <returns>Thumbnail if found, null otherwise</returns>
    Task<MediaThumbnail?> GetByMediaFileAndSizeAsync(Guid mediaFileId, ThumbnailSize size, string? format = null);

    /// <summary>
    /// Gets the default thumbnail for a media file and size
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="size">Thumbnail size category</param>
    /// <returns>Default thumbnail if found, null otherwise</returns>
    Task<MediaThumbnail?> GetDefaultThumbnailAsync(Guid mediaFileId, ThumbnailSize size);

    /// <summary>
    /// Gets thumbnails by status
    /// </summary>
    /// <param name="status">Thumbnail status to filter by</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of thumbnails with the specified status</returns>
    Task<IEnumerable<MediaThumbnail>> GetByStatusAsync(ThumbnailStatus status, int limit = 100);

    /// <summary>
    /// Gets thumbnails by format
    /// </summary>
    /// <param name="format">Image format (e.g., webp, jpeg, png)</param>
    /// <returns>Collection of thumbnails in the specified format</returns>
    Task<IEnumerable<MediaThumbnail>> GetByFormatAsync(string format);

    /// <summary>
    /// Gets thumbnails larger than specified dimensions
    /// </summary>
    /// <param name="minWidth">Minimum width in pixels</param>
    /// <param name="minHeight">Minimum height in pixels</param>
    /// <returns>Collection of thumbnails larger than the specified dimensions</returns>
    Task<IEnumerable<MediaThumbnail>> GetLargerThanAsync(int minWidth, int minHeight);

    /// <summary>
    /// Gets thumbnails that need regeneration (failed or outdated)
    /// </summary>
    /// <param name="olderThanDays">Consider thumbnails older than this many days as outdated</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of thumbnails that need regeneration</returns>
    Task<IEnumerable<MediaThumbnail>> GetNeedingRegenerationAsync(int olderThanDays = 30, int limit = 100);

    /// <summary>
    /// Gets thumbnails by generation method
    /// </summary>
    /// <param name="method">Generation method used</param>
    /// <returns>Collection of thumbnails created with the specified method</returns>
    Task<IEnumerable<MediaThumbnail>> GetByGenerationMethodAsync(ThumbnailGenerationMethod method);

    /// <summary>
    /// Gets storage statistics for thumbnails
    /// </summary>
    /// <returns>Total storage used by thumbnails in bytes</returns>
    Task<long> GetTotalStorageUsedAsync();

    /// <summary>
    /// Gets storage statistics grouped by size category
    /// </summary>
    /// <returns>Dictionary with size categories and storage used</returns>
    Task<Dictionary<ThumbnailSize, long>> GetStorageStatsBySizeAsync();

    /// <summary>
    /// Gets storage statistics grouped by format
    /// </summary>
    /// <returns>Dictionary with formats and storage used</returns>
    Task<Dictionary<string, long>> GetStorageStatsByFormatAsync();

    /// <summary>
    /// Gets generation performance statistics
    /// </summary>
    /// <returns>Average generation time by size category</returns>
    Task<Dictionary<ThumbnailSize, double>> GetGenerationStatsAsync();

    /// <summary>
    /// Sets a thumbnail as the default for its size category
    /// </summary>
    /// <param name="thumbnailId">Thumbnail identifier</param>
    /// <returns>True if set successfully</returns>
    Task<bool> SetAsDefaultAsync(Guid thumbnailId);

    /// <summary>
    /// Updates thumbnail status and error information
    /// </summary>
    /// <param name="thumbnailId">Thumbnail identifier</param>
    /// <param name="status">New status</param>
    /// <param name="errorMessage">Optional error message</param>
    /// <param name="generationTimeMs">Optional generation time</param>
    /// <returns>True if updated successfully</returns>
    Task<bool> UpdateStatusAsync(Guid thumbnailId, ThumbnailStatus status, string? errorMessage = null, int? generationTimeMs = null);

    /// <summary>
    /// Bulk updates thumbnail status
    /// </summary>
    /// <param name="thumbnailIds">Collection of thumbnail identifiers</param>
    /// <param name="status">New status</param>
    /// <returns>Number of thumbnails updated</returns>
    Task<int> BulkUpdateStatusAsync(IEnumerable<Guid> thumbnailIds, ThumbnailStatus status);

    /// <summary>
    /// Deletes all thumbnails for a media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <returns>Number of thumbnails deleted</returns>
    Task<int> DeleteByMediaFileAsync(Guid mediaFileId);

    /// <summary>
    /// Deletes thumbnails by status (e.g., cleanup failed thumbnails)
    /// </summary>
    /// <param name="status">Status of thumbnails to delete</param>
    /// <param name="olderThanDays">Only delete thumbnails older than this many days</param>
    /// <returns>Number of thumbnails deleted</returns>
    Task<int> DeleteByStatusAsync(ThumbnailStatus status, int olderThanDays = 7);

    /// <summary>
    /// Gets orphaned thumbnails (thumbnails without corresponding media files)
    /// </summary>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of orphaned thumbnails</returns>
    Task<IEnumerable<MediaThumbnail>> GetOrphanedThumbnailsAsync(int limit = 1000);

    /// <summary>
    /// Checks if a thumbnail exists for the given media file and specifications
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="size">Thumbnail size</param>
    /// <param name="width">Width in pixels</param>
    /// <param name="height">Height in pixels</param>
    /// <param name="format">Image format</param>
    /// <returns>True if thumbnail exists</returns>
    Task<bool> ExistsAsync(Guid mediaFileId, ThumbnailSize size, int width, int height, string format);
}
