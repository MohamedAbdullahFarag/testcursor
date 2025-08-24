using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using MetadataType = Ikhtibar.Shared.Entities.MetadataType;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for MediaMetadata entity operations
/// Provides specialized methods for flexible metadata management using Dapper
/// </summary>
public class MediaMetadataRepository : BaseRepository<MediaMetadata>, IMediaMetadataRepository
{
    private new readonly ILogger<MediaMetadataRepository> _logger;

    public MediaMetadataRepository(IDbConnectionFactory connectionFactory, ILogger<MediaMetadataRepository> logger)
        : base(connectionFactory, logger, "MediaMetadata", "Id")
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets all metadata for a specific media file
    /// </summary>
    public async Task<IEnumerable<MediaMetadata>> GetByMediaFileAsync(int mediaFileId, bool publicOnly = false)
    {
        using var scope = _logger.BeginScope("GetByMediaFileAsync: MediaFileId={MediaFileId}, PublicOnly={PublicOnly}", 
            mediaFileId, publicOnly);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT * FROM MediaMetadata 
                WHERE MediaFileId = @MediaFileId 
                AND IsDeleted = 0";
            
            if (publicOnly)
            {
                sql += " AND IsPublic = 1";
            }
            
            sql += " ORDER BY MetadataGroup, MetadataKey";
            
            var metadata = await connection.QueryAsync<MediaMetadata>(sql, new { MediaFileId = mediaFileId });
            
            _logger.LogInformation("Retrieved {Count} metadata entries for media file {MediaFileId}", 
                metadata.Count(), mediaFileId);
            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata for media file {MediaFileId}", mediaFileId);
            throw;
        }
    }

    /// <summary>
    /// Gets metadata by key for a specific media file
    /// </summary>
    public async Task<MediaMetadata?> GetByKeyAsync(int mediaFileId, string metadataKey)
    {
        using var scope = _logger.BeginScope("GetByKeyAsync: MediaFileId={MediaFileId}, Key={Key}", 
            mediaFileId, metadataKey);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT * FROM MediaMetadata 
                WHERE MediaFileId = @MediaFileId 
                AND MetadataKey = @MetadataKey 
                AND IsDeleted = 0";
            
            var metadata = await connection.QueryFirstOrDefaultAsync<MediaMetadata>(sql, 
                new { MediaFileId = mediaFileId, MetadataKey = metadataKey });
            
            _logger.LogInformation("Retrieved metadata by key {Key} for media file {MediaFileId}: {Found}", 
                metadataKey, mediaFileId, metadata != null);
            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata by key {Key} for media file {MediaFileId}", 
                metadataKey, mediaFileId);
            throw;
        }
    }

    /// <summary>
    /// Gets metadata by group for a specific media file
    /// </summary>
    public async Task<IEnumerable<MediaMetadata>> GetByGroupAsync(int mediaFileId, string metadataGroup, bool publicOnly = false)
    {
        using var scope = _logger.BeginScope("GetByGroupAsync: MediaFileId={MediaFileId}, Group={Group}, PublicOnly={PublicOnly}", 
            mediaFileId, metadataGroup, publicOnly);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT * FROM MediaMetadata 
                WHERE MediaFileId = @MediaFileId 
                AND MetadataGroup = @MetadataGroup 
                AND IsDeleted = 0";
            
            if (publicOnly)
            {
                sql += " AND IsPublic = 1";
            }
            
            sql += " ORDER BY MetadataKey";
            
            var metadata = await connection.QueryAsync<MediaMetadata>(sql, 
                new { MediaFileId = mediaFileId, MetadataGroup = metadataGroup });
            
            _logger.LogInformation("Retrieved {Count} metadata entries in group {Group} for media file {MediaFileId}", 
                metadata.Count(), metadataGroup, mediaFileId);
            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata by group {Group} for media file {MediaFileId}", 
                metadataGroup, mediaFileId);
            throw;
        }
    }

    /// <summary>
    /// Searches metadata values across all media files
    /// </summary>
    public async Task<IEnumerable<int>> SearchMetadataAsync(string searchTerm, string? metadataKey = null, bool searchableOnly = true)
    {
        using var scope = _logger.BeginScope("SearchMetadataAsync: SearchTerm={SearchTerm}, Key={Key}, SearchableOnly={SearchableOnly}", 
            searchTerm, metadataKey, searchableOnly);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT DISTINCT MediaFileId FROM MediaMetadata 
                WHERE MetadataValue LIKE @SearchTerm 
                AND IsDeleted = 0";
            
            object parameters = new { SearchTerm = $"%{searchTerm}%" };
            
            if (!string.IsNullOrEmpty(metadataKey))
            {
                sql += " AND MetadataKey = @MetadataKey";
                parameters = new { SearchTerm = $"%{searchTerm}%", MetadataKey = metadataKey };
            }
            
            if (searchableOnly)
            {
                sql += " AND IsSearchable = 1";
            }
            
            var mediaFileIds = await connection.QueryAsync<int>(sql, parameters);
            
            _logger.LogInformation("Found {Count} media files matching search term '{SearchTerm}'", 
                mediaFileIds.Count(), searchTerm);
            return mediaFileIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching metadata with term '{SearchTerm}'", searchTerm);
            throw;
        }
    }

    /// <summary>
    /// Gets all unique metadata keys in the system
    /// </summary>
    public async Task<IEnumerable<string>> GetAllKeysAsync(bool searchableOnly = false)
    {
        using var scope = _logger.BeginScope("GetAllKeysAsync: SearchableOnly={SearchableOnly}", searchableOnly);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT DISTINCT MetadataKey FROM MediaMetadata 
                WHERE IsDeleted = 0";
            
            if (searchableOnly)
            {
                sql += " AND IsSearchable = 1";
            }
            
            sql += " ORDER BY MetadataKey";
            
            var keys = await connection.QueryAsync<string>(sql);
            
            _logger.LogInformation("Retrieved {Count} unique metadata keys", keys.Count());
            return keys;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata keys");
            throw;
        }
    }

    /// <summary>
    /// Gets all unique metadata groups in the system
    /// </summary>
    public async Task<IEnumerable<string>> GetAllGroupsAsync()
    {
        using var scope = _logger.BeginScope("GetAllGroupsAsync");
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT DISTINCT MetadataGroup FROM MediaMetadata 
                WHERE IsDeleted = 0 
                AND MetadataGroup IS NOT NULL 
                ORDER BY MetadataGroup";
            
            var groups = await connection.QueryAsync<string>(sql);
            
            _logger.LogInformation("Retrieved {Count} unique metadata groups", groups.Count());
            return groups;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata groups");
            throw;
        }
    }

    /// <summary>
    /// Gets metadata statistics by key
    /// </summary>
    public async Task<Dictionary<string, int>> GetKeyStatisticsAsync()
    {
        using var scope = _logger.BeginScope("GetKeyStatisticsAsync");
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT MetadataKey, COUNT(*) as Count
                FROM MediaMetadata 
                WHERE IsDeleted = 0
                GROUP BY MetadataKey
                ORDER BY Count DESC, MetadataKey";
            
            var results = await connection.QueryAsync<(string Key, int Count)>(sql);
            var statistics = results.ToDictionary(r => r.Key, r => r.Count);
            
            _logger.LogInformation("Retrieved statistics for {Count} metadata keys", statistics.Count);
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata key statistics");
            throw;
        }
    }

    /// <summary>
    /// Gets metadata statistics by data type
    /// </summary>
    public async Task<Dictionary<MetadataType, int>> GetTypeStatisticsAsync()
    {
        using var scope = _logger.BeginScope("GetTypeStatisticsAsync");
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT DataType, COUNT(*) as Count
                FROM MediaMetadata 
                WHERE IsDeleted = 0
                GROUP BY DataType
                ORDER BY Count DESC";
            
            var results = await connection.QueryAsync<(int Type, int Count)>(sql);
            var statistics = results.ToDictionary(r => (MetadataType)r.Type, r => r.Count);
            
            _logger.LogInformation("Retrieved statistics for {Count} metadata types", statistics.Count);
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata type statistics");
            throw;
        }
    }

    /// <summary>
    /// Bulk inserts metadata for a media file
    /// </summary>
    public async Task<int> BulkInsertAsync(int mediaFileId, IEnumerable<(string Key, string? Value, MetadataType Type, string? Group, bool IsSearchable, bool IsPublic)> metadata)
    {
        using var scope = _logger.BeginScope("BulkInsertAsync: MediaFileId={MediaFileId}, Count={Count}", 
            mediaFileId, metadata.Count());
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                INSERT INTO MediaMetadata (MediaFileId, MetadataKey, MetadataValue, DataType, MetadataGroup, IsSearchable, IsPublic, CreatedAt, ModifiedAt, IsDeleted)
                VALUES (@MediaFileId, @MetadataKey, @MetadataValue, @DataType, @MetadataGroup, @IsSearchable, @IsPublic, @CreatedAt, @ModifiedAt, @IsDeleted)";
            
            var now = DateTime.UtcNow;
            var metadataEntries = metadata.Select(m => new
            {
                MediaFileId = mediaFileId,
                MetadataKey = m.Key,
                MetadataValue = m.Value,
                DataType = (int)m.Type,
                MetadataGroup = m.Group,
                IsSearchable = m.IsSearchable,
                IsPublic = m.IsPublic,
                CreatedAt = now,
                ModifiedAt = now,
                IsDeleted = false
            });
            
            var affectedRows = await connection.ExecuteAsync(sql, metadataEntries);
            
            _logger.LogInformation("Bulk inserted {Count} metadata entries for media file {MediaFileId}", 
                affectedRows, mediaFileId);
            return affectedRows;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk inserting metadata for media file {MediaFileId}", mediaFileId);
            throw;
        }
    }

    /// <summary>
    /// Updates or inserts metadata (upsert operation)
    /// </summary>
    public async Task<MediaMetadata> UpsertAsync(int mediaFileId, string metadataKey, string? metadataValue, 
        MetadataType dataType, string? metadataGroup = null, bool isSearchable = false, bool isPublic = true)
    {
        using var scope = _logger.BeginScope("UpsertAsync: MediaFileId={MediaFileId}, Key={Key}", 
            mediaFileId, metadataKey);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // Check if metadata already exists
                var existingSql = @"
                    SELECT * FROM MediaMetadata 
                    WHERE MediaFileId = @MediaFileId 
                    AND MetadataKey = @MetadataKey 
                    AND IsDeleted = 0";
                
                var existing = await connection.QueryFirstOrDefaultAsync<MediaMetadata>(existingSql, 
                    new { MediaFileId = mediaFileId, MetadataKey = metadataKey }, transaction);
                
                if (existing != null)
                {
                    // Update existing metadata
                    var updateSql = @"
                        UPDATE MediaMetadata 
                        SET MetadataValue = @MetadataValue, 
                            DataType = @DataType, 
                            MetadataGroup = @MetadataGroup, 
                            IsSearchable = @IsSearchable, 
                            IsPublic = @IsPublic, 
                            ModifiedAt = @ModifiedAt
                        WHERE Id = @Id";
                    
                    await connection.ExecuteAsync(updateSql, new
                    {
                        MetadataValue = metadataValue,
                        DataType = (int)dataType,
                        MetadataGroup = metadataGroup,
                        IsSearchable = isSearchable,
                        IsPublic = isPublic,
                        ModifiedAt = DateTime.UtcNow,
                        Id = existing.Id
                    }, transaction);
                    
                    existing.MetadataValue = metadataValue;
                    existing.DataType = dataType;
                    existing.MetadataGroup = metadataGroup;
                    existing.IsSearchable = isSearchable;
                    existing.IsPublic = isPublic;
                    existing.ModifiedAt = DateTime.UtcNow;
                    
                    transaction.Commit();
                    _logger.LogInformation("Updated metadata {Key} for media file {MediaFileId}", metadataKey, mediaFileId);
                    return existing;
                }
                else
                {
                    // Insert new metadata
                    var metadata = new MediaMetadata
                    {
                        MediaFileId = mediaFileId,
                        MetadataKey = metadataKey,
                        MetadataValue = metadataValue,
                        DataType = dataType,
                        MetadataGroup = metadataGroup,
                        IsSearchable = isSearchable,
                        IsPublic = isPublic,
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    
                    var insertSql = @"
                        INSERT INTO MediaMetadata (MediaFileId, MetadataKey, MetadataValue, DataType, MetadataGroup, IsSearchable, IsPublic, CreatedAt, ModifiedAt, IsDeleted)
                        VALUES (@MediaFileId, @MetadataKey, @MetadataValue, @DataType, @MetadataGroup, @IsSearchable, @IsPublic, @CreatedAt, @ModifiedAt, @IsDeleted)";
                    
                    await connection.ExecuteAsync(insertSql, new
                    {
                        Id = metadata.Id,
                        MediaFileId = metadata.MediaFileId,
                        MetadataKey = metadata.MetadataKey,
                        MetadataValue = metadata.MetadataValue,
                        DataType = (int)metadata.DataType,
                        MetadataGroup = metadata.MetadataGroup,
                        IsSearchable = metadata.IsSearchable,
                        IsPublic = metadata.IsPublic,
                        CreatedAt = metadata.CreatedAt,
                        ModifiedAt = metadata.ModifiedAt,
                        IsDeleted = metadata.IsDeleted
                    }, transaction);
                    
                    transaction.Commit();
                    _logger.LogInformation("Inserted new metadata {Key} for media file {MediaFileId}", metadataKey, mediaFileId);
                    return metadata;
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting metadata {Key} for media file {MediaFileId}", metadataKey, mediaFileId);
            throw;
        }
    }

    /// <summary>
    /// Deletes all metadata for a media file
    /// </summary>
    public async Task<int> DeleteByMediaFileAsync(int mediaFileId)
    {
        using var scope = _logger.BeginScope("DeleteByMediaFileAsync: MediaFileId={MediaFileId}", mediaFileId);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                UPDATE MediaMetadata 
                SET IsDeleted = 1, ModifiedAt = @ModifiedAt
                WHERE MediaFileId = @MediaFileId AND IsDeleted = 0";
            
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                MediaFileId = mediaFileId,
                ModifiedAt = DateTime.UtcNow
            });
            
            _logger.LogInformation("Deleted {Count} metadata entries for media file {MediaFileId}", 
                affectedRows, mediaFileId);
            return affectedRows;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting metadata for media file {MediaFileId}", mediaFileId);
            throw;
        }
    }

    /// <summary>
    /// Deletes metadata by key for a media file
    /// </summary>
    public async Task<bool> DeleteByKeyAsync(int mediaFileId, string metadataKey)
    {
        using var scope = _logger.BeginScope("DeleteByKeyAsync: MediaFileId={MediaFileId}, Key={Key}", 
            mediaFileId, metadataKey);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                UPDATE MediaMetadata 
                SET IsDeleted = 1, ModifiedAt = @ModifiedAt
                WHERE MediaFileId = @MediaFileId 
                AND MetadataKey = @MetadataKey 
                AND IsDeleted = 0";
            
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                MediaFileId = mediaFileId,
                MetadataKey = metadataKey,
                ModifiedAt = DateTime.UtcNow
            });
            
            var success = affectedRows > 0;
            _logger.LogInformation("Deleted metadata {Key} for media file {MediaFileId}: {Success}", 
                metadataKey, mediaFileId, success);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting metadata {Key} for media file {MediaFileId}", metadataKey, mediaFileId);
            throw;
        }
    }

    /// <summary>
    /// Deletes metadata by group for a media file
    /// </summary>
    public async Task<int> DeleteByGroupAsync(int mediaFileId, string metadataGroup)
    {
        using var scope = _logger.BeginScope("DeleteByGroupAsync: MediaFileId={MediaFileId}, Group={Group}", 
            mediaFileId, metadataGroup);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                UPDATE MediaMetadata 
                SET IsDeleted = 1, ModifiedAt = @ModifiedAt
                WHERE MediaFileId = @MediaFileId 
                AND MetadataGroup = @MetadataGroup 
                AND IsDeleted = 0";
            
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                MediaFileId = mediaFileId,
                MetadataGroup = metadataGroup,
                ModifiedAt = DateTime.UtcNow
            });
            
            _logger.LogInformation("Deleted {Count} metadata entries in group {Group} for media file {MediaFileId}", 
                affectedRows, metadataGroup, mediaFileId);
            return affectedRows;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting metadata group {Group} for media file {MediaFileId}", 
                metadataGroup, mediaFileId);
            throw;
        }
    }

    /// <summary>
    /// Gets media files that have specific metadata key/value pairs
    /// </summary>
    public async Task<IEnumerable<int>> GetMediaFilesByMetadataAsync(Dictionary<string, string> criteria, bool matchAll = true)
    {
        using var scope = _logger.BeginScope("GetMediaFilesByMetadataAsync: CriteriaCount={Count}, MatchAll={MatchAll}", 
            criteria.Count, matchAll);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            if (!criteria.Any())
            {
                return Enumerable.Empty<int>();
            }
            
            var conditions = criteria.Select((kvp, index) => 
                $"(MetadataKey = @Key{index} AND MetadataValue = @Value{index})").ToList();
            
            var sql = $@"
                SELECT MediaFileId
                FROM MediaMetadata 
                WHERE IsDeleted = 0
                AND ({string.Join(matchAll ? " AND " : " OR ", conditions.Select(c => $"EXISTS (SELECT 1 FROM MediaMetadata m WHERE m.MediaFileId = MediaMetadata.MediaFileId AND {c})"))})
                GROUP BY MediaFileId";
            
            if (matchAll)
            {
                sql += $" HAVING COUNT(DISTINCT CASE WHEN ({string.Join(" OR ", conditions)}) THEN MetadataKey END) = @CriteriaCount";
            }
            
            var parameters = new DynamicParameters();
            for (int i = 0; i < criteria.Count; i++)
            {
                var kvp = criteria.ElementAt(i);
                parameters.Add($"Key{i}", kvp.Key);
                parameters.Add($"Value{i}", kvp.Value);
            }
            
            if (matchAll)
            {
                parameters.Add("CriteriaCount", criteria.Count);
            }
            
            var mediaFileIds = await connection.QueryAsync<int>(sql, parameters);
            
            _logger.LogInformation("Found {Count} media files matching metadata criteria", mediaFileIds.Count());
            return mediaFileIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media files by metadata criteria");
            throw;
        }
    }

    /// <summary>
    /// Gets the most commonly used metadata values for a specific key
    /// </summary>
    public async Task<Dictionary<string, int>> GetPopularValuesAsync(string metadataKey, int limit = 10)
    {
        using var scope = _logger.BeginScope("GetPopularValuesAsync: Key={Key}, Limit={Limit}", metadataKey, limit);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT TOP (@Limit) MetadataValue, COUNT(*) as Count
                FROM MediaMetadata 
                WHERE MetadataKey = @MetadataKey 
                AND IsDeleted = 0
                AND MetadataValue IS NOT NULL
                GROUP BY MetadataValue
                ORDER BY Count DESC, MetadataValue";
            
            var results = await connection.QueryAsync<(string Value, int Count)>(sql, 
                new { MetadataKey = metadataKey, Limit = limit });
            
            var popularValues = results.ToDictionary(r => r.Value, r => r.Count);
            
            _logger.LogInformation("Retrieved {Count} popular values for metadata key {Key}", 
                popularValues.Count, metadataKey);
            return popularValues;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving popular values for metadata key {Key}", metadataKey);
            throw;
        }
    }
}
