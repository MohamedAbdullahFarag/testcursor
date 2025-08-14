using Ikhtibar.Core.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for MediaFile entity operations
/// Provides specialized methods for media file management and querying
/// </summary>
public interface IMediaFileRepository : IRepository<MediaFile>
{
    /// <summary>
    /// Gets media files by category
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <param name="includeSubcategories">Whether to include files from subcategories</param>
    /// <returns>Collection of media files in the category</returns>
    Task<IEnumerable<MediaFile>> GetByCategoryAsync(Guid categoryId, bool includeSubcategories = false);

    /// <summary>
    /// Gets media files by type with pagination
    /// </summary>
    /// <param name="mediaType">Type of media to filter by</param>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <param name="orderBy">Optional ordering</param>
    /// <returns>Collection of media files of the specified type</returns>
    Task<IEnumerable<MediaFile>> GetByTypeAsync(MediaType mediaType, int offset = 0, int limit = 50, string? orderBy = null);

    /// <summary>
    /// Gets media files by status
    /// </summary>
    /// <param name="status">Media status to filter by</param>
    /// <returns>Collection of media files with the specified status</returns>
    Task<IEnumerable<MediaFile>> GetByStatusAsync(MediaStatus status);

    /// <summary>
    /// Gets media files uploaded by a specific user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of media files uploaded by the user</returns>
    Task<IEnumerable<MediaFile>> GetByUserAsync(Guid userId, int offset = 0, int limit = 50);

    /// <summary>
    /// Searches media files by filename, title, or description
    /// </summary>
    /// <param name="searchTerm">Search term to match against</param>
    /// <param name="mediaType">Optional media type filter</param>
    /// <param name="categoryId">Optional category filter</param>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of media files matching the search criteria</returns>
    Task<IEnumerable<MediaFile>> SearchAsync(string searchTerm, MediaType? mediaType = null, Guid? categoryId = null, int offset = 0, int limit = 50);

    /// <summary>
    /// Gets media files by file hash (for duplicate detection)
    /// </summary>
    /// <param name="fileHash">File hash to search for</param>
    /// <returns>Collection of media files with the same hash</returns>
    Task<IEnumerable<MediaFile>> GetByHashAsync(string fileHash);

    /// <summary>
    /// Gets media files by content type
    /// </summary>
    /// <param name="contentType">MIME content type</param>
    /// <returns>Collection of media files with the specified content type</returns>
    Task<IEnumerable<MediaFile>> GetByContentTypeAsync(string contentType);

    /// <summary>
    /// Gets recently uploaded media files
    /// </summary>
    /// <param name="days">Number of days to look back</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of recently uploaded media files</returns>
    Task<IEnumerable<MediaFile>> GetRecentAsync(int days = 7, int limit = 50);

    /// <summary>
    /// Gets most accessed media files
    /// </summary>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of most accessed media files</returns>
    Task<IEnumerable<MediaFile>> GetMostAccessedAsync(int limit = 50);

    /// <summary>
    /// Gets media files that need processing
    /// </summary>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of media files requiring processing</returns>
    Task<IEnumerable<MediaFile>> GetPendingProcessingAsync(int limit = 100);

    /// <summary>
    /// Gets media files larger than specified size
    /// </summary>
    /// <param name="sizeBytes">Minimum file size in bytes</param>
    /// <returns>Collection of media files larger than the specified size</returns>
    Task<IEnumerable<MediaFile>> GetLargerThanAsync(long sizeBytes);

    /// <summary>
    /// Gets storage statistics grouped by media type
    /// </summary>
    /// <returns>Dictionary with media type and total storage used</returns>
    Task<Dictionary<MediaType, long>> GetStorageStatsByTypeAsync();

    /// <summary>
    /// Gets storage statistics grouped by category
    /// </summary>
    /// <returns>Dictionary with category ID and total storage used</returns>
    Task<Dictionary<Guid, long>> GetStorageStatsByCategoryAsync();

    /// <summary>
    /// Updates the access count and last accessed time
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <returns>True if updated successfully</returns>
    Task<bool> UpdateAccessInfoAsync(Guid mediaFileId);

    /// <summary>
    /// Updates the media file status
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="status">New status</param>
    /// <returns>True if updated successfully</returns>
    Task<bool> UpdateStatusAsync(Guid mediaFileId, MediaStatus status);

    /// <summary>
    /// Bulk updates media file status
    /// </summary>
    /// <param name="mediaFileIds">Collection of media file identifiers</param>
    /// <param name="status">New status</param>
    /// <returns>Number of files updated</returns>
    Task<int> BulkUpdateStatusAsync(IEnumerable<Guid> mediaFileIds, MediaStatus status);

    /// <summary>
    /// Gets media files that haven't been accessed recently (for cleanup)
    /// </summary>
    /// <param name="days">Number of days of inactivity</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of inactive media files</returns>
    Task<IEnumerable<MediaFile>> GetInactiveAsync(int days = 365, int limit = 1000);

    /// <summary>
    /// Checks if a file with the same hash already exists
    /// </summary>
    /// <param name="fileHash">File hash to check</param>
    /// <param name="excludeId">Optional file ID to exclude from the check</param>
    /// <returns>True if a duplicate exists</returns>
    Task<bool> IsDuplicateAsync(string fileHash, Guid? excludeId = null);
}
