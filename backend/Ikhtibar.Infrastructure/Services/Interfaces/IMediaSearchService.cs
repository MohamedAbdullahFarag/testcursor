using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Infrastructure.Services.Interfaces;

/// <summary>
/// Service interface for media search operations
/// Handles search, filtering, and discovery of media files
/// </summary>
public interface IMediaSearchService
{
    /// <summary>
    /// Searches media files with advanced filtering
    /// </summary>
    Task<PagedResult<MediaFileDto>> SearchMediaAsync(MediaFileSearchDto searchDto);

    /// <summary>
    /// Gets similar media files based on content or metadata
    /// </summary>
    Task<IEnumerable<MediaFileDto>> GetSimilarMediaAsync(Guid mediaId);

    /// <summary>
    /// Gets recently uploaded or modified media files
    /// </summary>
    Task<IEnumerable<MediaFileDto>> GetRecentMediaAsync(int count = 10, Guid? userId = null);

    /// <summary>
    /// Gets media files by tags
    /// </summary>
    Task<IEnumerable<MediaFileDto>> GetMediaByTagsAsync(string[] tags);

    /// <summary>
    /// Gets media files by category with pagination
    /// </summary>
    Task<PagedResult<MediaFileDto>> GetMediaByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 20);

    /// <summary>
    /// Gets media files by collection
    /// </summary>
    Task<IEnumerable<MediaFileDto>> GetMediaByCollectionAsync(Guid collectionId);

    /// <summary>
    /// Gets media files by user with pagination
    /// </summary>
    Task<PagedResult<MediaFileDto>> GetMediaByUserAsync(Guid userId, int page = 1, int pageSize = 20);

    /// <summary>
    /// Gets media files by type with pagination
    /// </summary>
    Task<PagedResult<MediaFileDto>> GetMediaByTypeAsync(MediaFileType mediaType, int page = 1, int pageSize = 20);

    /// <summary>
    /// Gets media files by date range
    /// </summary>
    Task<IEnumerable<MediaFileDto>> GetMediaByDateRangeAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Gets media files by file size range
    /// </summary>
    Task<IEnumerable<MediaFileDto>> GetMediaBySizeRangeAsync(long minSizeBytes, long maxSizeBytes);

    /// <summary>
    /// Gets popular media files based on access count
    /// </summary>
    Task<IEnumerable<MediaFileDto>> GetPopularMediaAsync(int count = 10, TimeSpan? timeRange = null);
}
