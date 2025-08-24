

using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for MediaCollection entity operations
/// Provides specialized methods for collection and album management
/// </summary>
public interface IMediaCollectionRepository : IBaseRepository<MediaCollection>
{
    /// <summary>
    /// Gets collections created by a specific user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="includePrivate">Whether to include private collections</param>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of user's media collections</returns>
    Task<IEnumerable<MediaCollection>> GetByUserAsync(int userId, bool includePrivate = true, int offset = 0, int limit = 50);

    /// <summary>
    /// Gets collections by type
    /// </summary>
    /// <param name="collectionType">Type of collection to filter by</param>
    /// <param name="publicOnly">Whether to return only public collections</param>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of media collections of the specified type</returns>
    Task<IEnumerable<MediaCollection>> GetByTypeAsync(CollectionType collectionType, bool publicOnly = true, int offset = 0, int limit = 50);

    /// <summary>
    /// Gets a collection by its slug
    /// </summary>
    /// <param name="slug">Collection slug</param>
    /// <param name="publicOnly">Whether to return only public collections</param>
    /// <returns>Collection if found, null otherwise</returns>
    Task<MediaCollection?> GetBySlugAsync(string slug, bool publicOnly = true);

    /// <summary>
    /// Gets featured collections
    /// </summary>
    /// <param name="collectionType">Optional type filter</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of featured media collections</returns>
    Task<IEnumerable<MediaCollection>> GetFeaturedAsync(CollectionType? collectionType = null, int limit = 10);

    /// <summary>
    /// Gets public collections with pagination
    /// </summary>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <param name="orderBy">Optional ordering</param>
    /// <returns>Collection of public media collections</returns>
    Task<IEnumerable<MediaCollection>> GetPublicAsync(int offset = 0, int limit = 50, string? orderBy = null);

    /// <summary>
    /// Gets most viewed collections
    /// </summary>
    /// <param name="collectionType">Optional type filter</param>
    /// <param name="publicOnly">Whether to return only public collections</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of most viewed media collections</returns>
    Task<IEnumerable<MediaCollection>> GetMostViewedAsync(CollectionType? collectionType = null, bool publicOnly = true, int limit = 10);

    /// <summary>
    /// Gets recently created collections
    /// </summary>
    /// <param name="days">Number of days to look back</param>
    /// <param name="publicOnly">Whether to return only public collections</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of recently created media collections</returns>
    Task<IEnumerable<MediaCollection>> GetRecentAsync(int days = 7, bool publicOnly = true, int limit = 10);

    /// <summary>
    /// Searches collections by name, description, or tags
    /// </summary>
    /// <param name="searchTerm">Search term to match against</param>
    /// <param name="collectionType">Optional type filter</param>
    /// <param name="publicOnly">Whether to search only public collections</param>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of matching media collections</returns>
    Task<IEnumerable<MediaCollection>> SearchAsync(string searchTerm, CollectionType? collectionType = null, bool publicOnly = true, int offset = 0, int limit = 50);

    /// <summary>
    /// Gets collections that contain a specific media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="publicOnly">Whether to return only public collections</param>
    /// <returns>Collection of media collections containing the file</returns>
    Task<IEnumerable<MediaCollection>> GetContainingMediaFileAsync(int mediaFileId, bool publicOnly = true);

    /// <summary>
    /// Gets collections by tags
    /// </summary>
    /// <param name="tags">Tags to search for (comma-separated)</param>
    /// <param name="matchAll">Whether all tags must match (true) or any (false)</param>
    /// <param name="publicOnly">Whether to return only public collections</param>
    /// <returns>Collection of media collections with matching tags</returns>
    Task<IEnumerable<MediaCollection>> GetByTagsAsync(string tags, bool matchAll = false, bool publicOnly = true);

    /// <summary>
    /// Gets the count of media files in a collection
    /// </summary>
    /// <param name="collectionId">Collection identifier</param>
    /// <returns>Number of media files in the collection</returns>
    Task<int> GetMediaFileCountAsync(int collectionId);

    /// <summary>
    /// Gets the total size of all media files in a collection
    /// </summary>
    /// <param name="collectionId">Collection identifier</param>
    /// <returns>Total size in bytes</returns>
    Task<long> GetTotalSizeAsync(int collectionId);

    /// <summary>
    /// Checks if a slug is available for a new collection
    /// </summary>
    /// <param name="slug">Slug to check</param>
    /// <param name="excludeId">Optional collection ID to exclude from check</param>
    /// <returns>True if the slug is available</returns>
    Task<bool> IsSlugAvailableAsync(string slug, int? excludeId = null);

    /// <summary>
    /// Updates the view count for a collection
    /// </summary>
    /// <param name="collectionId">Collection identifier</param>
    /// <returns>New view count</returns>
    Task<int> IncrementViewCountAsync(int collectionId);

    /// <summary>
    /// Adds a media file to a collection
    /// </summary>
    /// <param name="collectionId">Collection identifier</param>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="sortOrder">Sort order within the collection</param>
    /// <param name="caption">Optional caption for the item</param>
    /// <param name="isFeatured">Whether this item is featured</param>
    /// <returns>True if added successfully</returns>
    Task<bool> AddMediaFileAsync(int collectionId, int mediaFileId, int sortOrder = 0, string? caption = null, bool isFeatured = false);

    /// <summary>
    /// Removes a media file from a collection
    /// </summary>
    /// <param name="collectionId">Collection identifier</param>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <returns>True if removed successfully</returns>
    Task<bool> RemoveMediaFileAsync(int collectionId, int mediaFileId);

    /// <summary>
    /// Updates the sort order of media files in a collection
    /// </summary>
    /// <param name="collectionId">Collection identifier</param>
    /// <param name="mediaFileIds">Ordered list of media file IDs</param>
    /// <returns>Number of items updated</returns>
    Task<int> UpdateSortOrderAsync(int collectionId, IEnumerable<int> mediaFileIds);

    /// <summary>
    /// Gets all collection items for a collection with their details
    /// </summary>
    /// <param name="collectionId">Collection identifier</param>
    /// <param name="offset">Number of records to skip</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection items with media file details</returns>
    Task<IEnumerable<MediaCollectionItem>> GetCollectionItemsAsync(int collectionId, int offset = 0, int limit = 100);

    /// <summary>
    /// Gets featured items from a collection
    /// </summary>
    /// <param name="collectionId">Collection identifier</param>
    /// <param name="limit">Maximum number of items to return</param>
    /// <returns>Featured collection items</returns>
    Task<IEnumerable<MediaCollectionItem>> GetFeaturedItemsAsync(int collectionId, int limit = 5);

    /// <summary>
    /// Duplicates a collection with all its items
    /// </summary>
    /// <param name="collectionId">Source collection identifier</param>
    /// <param name="newName">Name for the new collection</param>
    /// <param name="newSlug">Slug for the new collection</param>
    /// <param name="userId">User creating the duplicate</param>
    /// <returns>The new collection</returns>
    Task<MediaCollection> DuplicateCollectionAsync(int collectionId, string newName, string newSlug, int userId);

    /// <summary>
    /// Gets collection statistics
    /// </summary>
    /// <returns>Statistics including total collections, public/private counts, etc.</returns>
    Task<dynamic> GetCollectionStatsAsync();

    /// <summary>
    /// Gets collections that are empty (no media files)
    /// </summary>
    /// <param name="olderThanDays">Only include collections older than this many days</param>
    /// <param name="limit">Maximum number of records to return</param>
    /// <returns>Collection of empty media collections</returns>
    Task<IEnumerable<MediaCollection>> GetEmptyCollectionsAsync(int olderThanDays = 30, int limit = 100);
}
