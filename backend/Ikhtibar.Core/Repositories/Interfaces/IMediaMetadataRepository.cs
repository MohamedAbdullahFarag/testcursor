using Ikhtibar.Core.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for MediaMetadata entity operations
/// Provides specialized methods for flexible metadata management
/// </summary>
public interface IMediaMetadataRepository : IRepository<MediaMetadata>
{
    /// <summary>
    /// Gets all metadata for a specific media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="publicOnly">Whether to return only public metadata</param>
    /// <returns>Collection of metadata for the media file</returns>
    Task<IEnumerable<MediaMetadata>> GetByMediaFileAsync(Guid mediaFileId, bool publicOnly = false);

    /// <summary>
    /// Gets metadata by key for a specific media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="metadataKey">Metadata key to search for</param>
    /// <returns>Metadata entry if found, null otherwise</returns>
    Task<MediaMetadata?> GetByKeyAsync(Guid mediaFileId, string metadataKey);

    /// <summary>
    /// Gets metadata by group for a specific media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="metadataGroup">Metadata group to filter by</param>
    /// <param name="publicOnly">Whether to return only public metadata</param>
    /// <returns>Collection of metadata in the specified group</returns>
    Task<IEnumerable<MediaMetadata>> GetByGroupAsync(Guid mediaFileId, string metadataGroup, bool publicOnly = false);

    /// <summary>
    /// Searches metadata values across all media files
    /// </summary>
    /// <param name="searchTerm">Search term to match against metadata values</param>
    /// <param name="metadataKey">Optional key to limit search to specific metadata</param>
    /// <param name="searchableOnly">Whether to search only searchable metadata</param>
    /// <returns>Collection of media files with matching metadata</returns>
    Task<IEnumerable<Guid>> SearchMetadataAsync(string searchTerm, string? metadataKey = null, bool searchableOnly = true);

    /// <summary>
    /// Gets all unique metadata keys in the system
    /// </summary>
    /// <param name="searchableOnly">Whether to return only searchable keys</param>
    /// <returns>Collection of unique metadata keys</returns>
    Task<IEnumerable<string>> GetAllKeysAsync(bool searchableOnly = false);

    /// <summary>
    /// Gets all unique metadata groups in the system
    /// </summary>
    /// <returns>Collection of unique metadata groups</returns>
    Task<IEnumerable<string>> GetAllGroupsAsync();

    /// <summary>
    /// Gets metadata statistics by key
    /// </summary>
    /// <returns>Dictionary with key names and their usage counts</returns>
    Task<Dictionary<string, int>> GetKeyStatisticsAsync();

    /// <summary>
    /// Gets metadata statistics by data type
    /// </summary>
    /// <returns>Dictionary with data types and their usage counts</returns>
    Task<Dictionary<MetadataType, int>> GetTypeStatisticsAsync();

    /// <summary>
    /// Bulk inserts metadata for a media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="metadata">Collection of metadata to insert</param>
    /// <returns>Number of metadata entries inserted</returns>
    Task<int> BulkInsertAsync(Guid mediaFileId, IEnumerable<(string Key, string? Value, MetadataType Type, string? Group, bool IsSearchable, bool IsPublic)> metadata);

    /// <summary>
    /// Updates or inserts metadata (upsert operation)
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="metadataKey">Metadata key</param>
    /// <param name="metadataValue">Metadata value</param>
    /// <param name="dataType">Data type of the value</param>
    /// <param name="metadataGroup">Optional metadata group</param>
    /// <param name="isSearchable">Whether the metadata is searchable</param>
    /// <param name="isPublic">Whether the metadata is public</param>
    /// <returns>The updated or created metadata entry</returns>
    Task<MediaMetadata> UpsertAsync(Guid mediaFileId, string metadataKey, string? metadataValue, MetadataType dataType, string? metadataGroup = null, bool isSearchable = false, bool isPublic = true);

    /// <summary>
    /// Deletes all metadata for a media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <returns>Number of metadata entries deleted</returns>
    Task<int> DeleteByMediaFileAsync(Guid mediaFileId);

    /// <summary>
    /// Deletes metadata by key for a media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="metadataKey">Metadata key to delete</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteByKeyAsync(Guid mediaFileId, string metadataKey);

    /// <summary>
    /// Deletes metadata by group for a media file
    /// </summary>
    /// <param name="mediaFileId">Media file identifier</param>
    /// <param name="metadataGroup">Metadata group to delete</param>
    /// <returns>Number of metadata entries deleted</returns>
    Task<int> DeleteByGroupAsync(Guid mediaFileId, string metadataGroup);

    /// <summary>
    /// Gets media files that have specific metadata key/value pairs
    /// </summary>
    /// <param name="criteria">Dictionary of key-value pairs to match</param>
    /// <param name="matchAll">Whether all criteria must match (true) or any (false)</param>
    /// <returns>Collection of media file IDs matching the criteria</returns>
    Task<IEnumerable<Guid>> GetMediaFilesByMetadataAsync(Dictionary<string, string> criteria, bool matchAll = true);

    /// <summary>
    /// Gets the most commonly used metadata values for a specific key
    /// </summary>
    /// <param name="metadataKey">Metadata key to analyze</param>
    /// <param name="limit">Maximum number of values to return</param>
    /// <returns>Dictionary with values and their usage counts</returns>
    Task<Dictionary<string, int>> GetPopularValuesAsync(string metadataKey, int limit = 10);
}
