using Ikhtibar.Core.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using MediaType = Ikhtibar.Core.Entities.MediaType;
using MediaStatus = Ikhtibar.Core.Entities.MediaStatus;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Media file repository implementation
/// Handles data access for media files using Dapper
/// </summary>
public class MediaFileRepository : BaseRepository<MediaFile>, IMediaFileRepository
{
    public MediaFileRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<MediaFileRepository> logger) 
        : base(connectionFactory, logger, "MediaFiles", "Id")
    {
    }

    public async Task<MediaFile?> GetByIdWithDetailsAsync(int id)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT mf.*, mc.*, mm.*, mt.*
                FROM MediaFiles mf
                LEFT JOIN MediaCategories mc ON mf.CategoryId = mc.Id
                LEFT JOIN MediaMetadata mm ON mf.Id = mm.MediaFileId
                LEFT JOIN MediaThumbnails mt ON mf.Id = mt.MediaFileId
                WHERE mf.Id = @Id AND mf.IsDeleted = 0";

            var mediaFileDictionary = new Dictionary<int, MediaFile>();
            var categoryDictionary = new Dictionary<int, MediaCategory>();
            var metadataDictionary = new Dictionary<int, MediaMetadata>();
            var thumbnailDictionary = new Dictionary<int, MediaThumbnail>();

            await connection.QueryAsync<MediaFile, MediaCategory, MediaMetadata, MediaThumbnail, MediaFile>(
                sql,
                (mediaFile, category, metadata, thumbnail) =>
                {
                    if (!mediaFileDictionary.TryGetValue(mediaFile.Id, out var existingMediaFile))
                    {
                        existingMediaFile = mediaFile;
                        mediaFileDictionary.Add(mediaFile.Id, existingMediaFile);
                    }

                    if (category != null && !categoryDictionary.ContainsKey(category.Id))
                    {
                        existingMediaFile.Category = category;
                        categoryDictionary.Add(category.Id, category);
                    }

                    if (metadata != null && !metadataDictionary.ContainsKey(metadata.Id))
                    {
                        existingMediaFile.ExtendedMetadata.Add(metadata);
                        metadataDictionary.Add(metadata.Id, metadata);
                    }

                    if (thumbnail != null && !thumbnailDictionary.ContainsKey(thumbnail.Id))
                    {
                        existingMediaFile.Thumbnails.Add(thumbnail);
                        thumbnailDictionary.Add(thumbnail.Id, thumbnail);
                    }

                    return existingMediaFile;
                },
                new { Id = id },
                splitOn: "Id"
            );

            return mediaFileDictionary.Values.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media file with details by ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetByCategoryAsync(int categoryId, bool includeSubcategories = false)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT * FROM MediaFiles 
                WHERE CategoryId = @CategoryId AND IsDeleted = 0
                ORDER BY CreatedAt DESC";
            
            return await connection.QueryAsync<MediaFile>(sql, new { CategoryId = categoryId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media files by category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetByTypeAsync(MediaType mediaType, int offset = 0, int limit = 50, string? orderBy = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT * FROM MediaFiles 
                WHERE MediaType = @MediaType AND IsDeleted = 0
                ORDER BY " + (orderBy ?? "CreatedAt DESC") + @"
                OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            
            return await connection.QueryAsync<MediaFile>(sql, new { MediaType = mediaType, Offset = offset, Limit = limit });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media files by type {MediaType}", mediaType);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetByStatusAsync(MediaStatus status)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT * FROM MediaFiles 
                WHERE Status = @Status AND IsDeleted = 0
                ORDER BY CreatedAt DESC";
            
            return await connection.QueryAsync<MediaFile>(sql, new { Status = status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media files by status {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetByUserAsync(int userId, int offset = 0, int limit = 50)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT * FROM MediaFiles 
                WHERE UploadedBy = @UserId AND IsDeleted = 0
                ORDER BY CreatedAt DESC 
                OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            
            return await connection.QueryAsync<MediaFile>(sql, new { UserId = userId, Offset = offset, Limit = limit });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media files by user {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> SearchAsync(string searchTerm, MediaType? mediaType = null, int? categoryId = null, int offset = 0, int limit = 50)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var whereConditions = new List<string> { "IsDeleted = 0" };
            var parameters = new DynamicParameters();

            whereConditions.Add("(OriginalFileName LIKE @SearchTerm OR Description LIKE @SearchTerm OR Tags LIKE @SearchTerm)");
            parameters.Add("@SearchTerm", $"%{searchTerm}%");

            if (mediaType.HasValue)
            {
                whereConditions.Add("MediaType = @MediaType");
                parameters.Add("@MediaType", mediaType.Value);
            }

            if (categoryId.HasValue)
            {
                whereConditions.Add("CategoryId = @CategoryId");
                parameters.Add("@CategoryId", categoryId.Value);
            }

            var whereClause = string.Join(" AND ", whereConditions);
            var sql = $@"
                SELECT * FROM MediaFiles 
                WHERE {whereClause}
                ORDER BY CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

            parameters.Add("@Offset", offset);
            parameters.Add("@Limit", limit);

            return await connection.QueryAsync<MediaFile>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching media files with term {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetByHashAsync(string fileHash)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT * FROM MediaFiles WHERE FileHash = @FileHash AND IsDeleted = 0";
            return await connection.QueryAsync<MediaFile>(sql, new { FileHash = fileHash });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media files by hash {FileHash}", fileHash);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetPendingProcessingAsync(int limit = 100)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT * FROM MediaFiles 
                WHERE Status = @Status AND IsDeleted = 0
                ORDER BY CreatedAt ASC
                OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY";
            
            return await connection.QueryAsync<MediaFile>(sql, new { Status = MediaStatus.Uploading, Limit = limit });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending processing media files with limit {Limit}", limit);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetByContentTypeAsync(string contentType)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT * FROM MediaFiles 
                WHERE MimeType = @ContentType AND IsDeleted = 0
                ORDER BY CreatedAt DESC";
            
            return await connection.QueryAsync<MediaFile>(sql, new { ContentType = contentType });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media files by content type {ContentType}", contentType);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetRecentAsync(int days = 7, int limit = 50)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var fromDate = DateTime.UtcNow.AddDays(-days);
            var sql = @"
                SELECT * FROM MediaFiles 
                WHERE CreatedAt >= @FromDate AND IsDeleted = 0
                ORDER BY CreatedAt DESC
                OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY";
            
            return await connection.QueryAsync<MediaFile>(sql, new { FromDate = fromDate, Limit = limit });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent media files for {Days} days with limit {Limit}", days, limit);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetMostAccessedAsync(int limit = 50)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT TOP(@Limit) * FROM MediaFiles 
                WHERE IsDeleted = 0
                ORDER BY AccessCount DESC, CreatedAt DESC";
            
            return await connection.QueryAsync<MediaFile>(sql, new { Limit = limit });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting most accessed media files with limit {Limit}", limit);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetLargerThanAsync(long sizeBytes)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT * FROM MediaFiles 
                WHERE FileSizeBytes > @SizeBytes AND IsDeleted = 0
                ORDER BY FileSizeBytes DESC";
            
            return await connection.QueryAsync<MediaFile>(sql, new { SizeBytes = sizeBytes });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media files larger than {SizeBytes} bytes", sizeBytes);
            throw;
        }
    }

    public async Task<Dictionary<MediaType, long>> GetStorageStatsByTypeAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT MediaType, SUM(FileSizeBytes) as TotalSize
                FROM MediaFiles 
                WHERE IsDeleted = 0
                GROUP BY MediaType";
            
            var results = await connection.QueryAsync(sql);
            var stats = new Dictionary<MediaType, long>();
            
            foreach (var row in results)
            {
                var mediaType = (MediaType)row.MediaType;
                var totalSize = (long)row.TotalSize;
                stats[mediaType] = totalSize;
            }
            
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting storage stats by media type");
            throw;
        }
    }

    public async Task<Dictionary<int, long>> GetStorageStatsByCategoryAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT CategoryId, SUM(FileSizeBytes) as TotalSize
                FROM MediaFiles 
                WHERE IsDeleted = 0 AND CategoryId IS NOT NULL
                GROUP BY CategoryId";
            
            var results = await connection.QueryAsync(sql);
            var stats = new Dictionary<int, long>();
            
            foreach (var row in results)
            {
                var categoryId = (int)row.CategoryId;
                var totalSize = (long)row.TotalSize;
                stats[categoryId] = totalSize;
            }
            
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting storage stats by category");
            throw;
        }
    }

    public async Task<bool> UpdateAccessInfoAsync(int mediaFileId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                UPDATE MediaFiles 
                SET AccessCount = AccessCount + 1, LastAccessedAt = @LastAccessedAt
                WHERE Id = @Id";
            
            var result = await connection.ExecuteAsync(sql, new 
            { 
                Id = mediaFileId, 
                LastAccessedAt = DateTime.UtcNow 
            });
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating access info for media file {Id}", mediaFileId);
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(int id, MediaStatus status)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "UPDATE MediaFiles SET Status = @Status, ModifiedAt = @ModifiedAt WHERE Id = @Id";
            var result = await connection.ExecuteAsync(sql, new { Id = id, Status = status, ModifiedAt = DateTime.UtcNow });
            return result > 0;
        }
        catch (Exception ex)
        {
            // Log error
            throw;
        }
    }

    public async Task<bool> UpdateProcessingStatusAsync(int id, MediaStatus status, string? errorMessage = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                UPDATE MediaFiles 
                SET Status = @Status, ModifiedAt = @ModifiedAt, MetadataJson = @MetadataJson
                WHERE Id = @Id AND IsDeleted = 0";
            
            var metadata = new { ErrorMessage = errorMessage };
            var result = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                Status = status,
                ModifiedAt = DateTime.UtcNow,
                MetadataJson = System.Text.Json.JsonSerializer.Serialize(metadata)
            });

            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating processing status for media file {Id} to {Status}", id, status);
            throw;
        }
    }

    public async Task<int> BulkUpdateStatusAsync(IEnumerable<int> mediaFileIds, MediaStatus status)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                UPDATE MediaFiles 
                SET Status = @Status, ModifiedAt = @ModifiedAt 
                WHERE Id IN @Ids";
            
            var result = await connection.ExecuteAsync(sql, new 
            { 
                Status = status, 
                ModifiedAt = DateTime.UtcNow,
                Ids = mediaFileIds.ToArray()
            });
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk updating status for {Count} media files to {Status}", mediaFileIds.Count(), status);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetInactiveAsync(int days = 30, int limit = 100)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT TOP(@Limit) * FROM MediaFiles 
                WHERE LastAccessedAt < @FromDate AND IsDeleted = 0
                ORDER BY LastAccessedAt ASC";
            
            var fromDate = DateTime.UtcNow.AddDays(-days);
            return await connection.QueryAsync<MediaFile>(sql, new { FromDate = fromDate, Limit = limit });
        }
        catch (Exception ex)
        {
            // Log error
            throw;
        }
    }

    public async Task<bool> IsDuplicateAsync(string fileHash, int? excludeId = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT COUNT(*) FROM MediaFiles WHERE FileHash = @FileHash AND IsDeleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("@FileHash", fileHash);
            
            if (excludeId.HasValue)
            {
                sql += " AND Id != @ExcludeId";
                parameters.Add("@ExcludeId", excludeId.Value);
            }
            
            var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for duplicate file with hash {FileHash}", fileHash);
            throw;
        }
    }

    // Additional methods for the search service
    public async Task<(IEnumerable<MediaFile> items, int totalCount)> SearchAsync(MediaFileSearchDto searchDto)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var whereConditions = new List<string> { "IsDeleted = 0" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                whereConditions.Add("(OriginalFileName LIKE @SearchTerm OR Description LIKE @SearchTerm OR Tags LIKE @SearchTerm)");
                parameters.Add("@SearchTerm", $"%{searchDto.SearchTerm}%");
            }

            if (searchDto.MediaType.HasValue)
            {
                whereConditions.Add("MediaType = @MediaType");
                parameters.Add("@MediaType", searchDto.MediaType.Value);
            }

            if (searchDto.CategoryId.HasValue)
            {
                whereConditions.Add("CategoryId = @CategoryId");
                parameters.Add("@CategoryId", searchDto.CategoryId.Value);
            }

            if (searchDto.Status.HasValue)
            {
                whereConditions.Add("Status = @Status");
                parameters.Add("@Status", searchDto.Status.Value);
            }

            if (searchDto.UploadedByUserId.HasValue)
            {
                whereConditions.Add("UploadedByUserId = @UploadedByUserId");
                parameters.Add("@UploadedByUserId", searchDto.UploadedByUserId.Value);
            }

            if (searchDto.CreatedAfter.HasValue)
            {
                whereConditions.Add("CreatedAt >= @CreatedAfter");
                parameters.Add("@CreatedAfter", searchDto.CreatedAfter.Value);
            }

            if (searchDto.CreatedBefore.HasValue)
            {
                whereConditions.Add("CreatedAt <= @CreatedBefore");
                parameters.Add("@CreatedBefore", searchDto.CreatedBefore.Value);
            }

            var whereClause = string.Join(" AND ", whereConditions);
            var orderBy = searchDto.SortDescending ? $"{searchDto.SortBy} DESC" : searchDto.SortBy;

            return await GetPagedAsync(searchDto);
        }
        catch (Exception ex)
        {
            // Log error
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetSimilarMediaAsync(int mediaId, int maxResults = 10)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT TOP(@MaxResults) mf.*
                FROM MediaFiles mf
                INNER JOIN MediaFiles current ON current.CategoryId = mf.CategoryId
                WHERE current.Id = @MediaId 
                AND mf.Id != @MediaId 
                AND mf.IsDeleted = 0
                ORDER BY mf.CreatedAt DESC";
            
            return await connection.QueryAsync<MediaFile>(sql, new { MediaId = mediaId, MaxResults = maxResults });
        }
        catch (Exception ex)
        {
            // Log error
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetRecentMediaAsync(int count = 10, Guid? userId = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT TOP(@Count) * FROM MediaFiles WHERE IsDeleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("@Count", count);

            if (userId.HasValue)
            {
                sql += " AND UploadedByUserId = @UserId";
                parameters.Add("@UserId", userId.Value);
            }

            sql += " ORDER BY CreatedAt DESC";
            
            return await connection.QueryAsync<MediaFile>(sql, parameters);
        }
        catch (Exception ex)
        {
            // Log error
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetByTagsAsync(string[] tags)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var tagList = string.Join(",", tags.Select(t => $"'{t}'"));
            var sql = $@"
                SELECT DISTINCT mf.*
                FROM MediaFiles mf
                WHERE mf.IsDeleted = 0
                AND EXISTS (
                    SELECT 1 FROM STRING_SPLIT(mf.Tags, ',') s
                    WHERE s.value IN ({tagList})
                )";
            
            return await connection.QueryAsync<MediaFile>(sql);
        }
        catch (Exception ex)
        {
            // Log error
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetByCollectionAsync(Guid collectionId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT mf.*
                FROM MediaFiles mf
                INNER JOIN MediaCollectionItems mci ON mf.Id = mci.MediaFileId
                WHERE mci.CollectionId = @CollectionId 
                AND mf.IsDeleted = 0
                ORDER BY mci.SortOrder";
            
            return await connection.QueryAsync<MediaFile>(sql, new { CollectionId = collectionId });
        }
        catch (Exception ex)
        {
            // Log error
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT * FROM MediaFiles 
                WHERE CreatedAt >= @FromDate 
                AND CreatedAt <= @ToDate 
                AND IsDeleted = 0
                ORDER BY CreatedAt DESC";
            
            return await connection.QueryAsync<MediaFile>(sql, new { FromDate = fromDate, ToDate = toDate });
        }
        catch (Exception ex)
        {
            // Log error
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetBySizeRangeAsync(long minSizeBytes, long maxSizeBytes)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = @"
                SELECT * FROM MediaFiles 
                WHERE FileSizeBytes >= @MinSize 
                AND FileSizeBytes <= @MaxSize 
                AND IsDeleted = 0
                ORDER BY FileSizeBytes DESC";
            
            return await connection.QueryAsync<MediaFile>(sql, new { MinSize = minSizeBytes, MaxSize = maxSizeBytes });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media files by size range {MinSize} to {MaxSize} bytes", minSizeBytes, maxSizeBytes);
            throw;
        }
    }

    public async Task<IEnumerable<MediaFile>> GetPopularMediaAsync(int count = 10, TimeSpan? timeRange = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT TOP(@Count) * FROM MediaFiles WHERE IsDeleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("@Count", count);

            if (timeRange.HasValue)
            {
                sql += " AND CreatedAt >= @FromDate";
                parameters.Add("@FromDate", DateTime.UtcNow.Subtract(timeRange.Value));
            }

            sql += " ORDER BY DownloadCount DESC, CreatedAt DESC";
            
            return await connection.QueryAsync<MediaFile>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular media files with count {Count}", count);
            throw;
        }
    }

    public async Task<PagedResult<MediaFile>> GetPagedAsync(MediaFileSearchDto searchDto)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var whereConditions = new List<string> { "IsDeleted = 0" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                whereConditions.Add("(OriginalFileName LIKE @SearchTerm OR Description LIKE @SearchTerm OR Tags LIKE @SearchTerm)");
                parameters.Add("@SearchTerm", $"%{searchDto.SearchTerm}%");
            }

            if (searchDto.MediaType.HasValue)
            {
                whereConditions.Add("MediaType = @MediaType");
                parameters.Add("@MediaType", searchDto.MediaType.Value);
            }

            if (searchDto.CategoryId.HasValue)
            {
                whereConditions.Add("CategoryId = @CategoryId");
                parameters.Add("@CategoryId", searchDto.CategoryId.Value);
            }

            if (searchDto.Status.HasValue)
            {
                whereConditions.Add("Status = @Status");
                parameters.Add("@Status", searchDto.Status.Value);
            }

            var whereClause = string.Join(" AND ", whereConditions);
            
            // Count query
            var countSql = $"SELECT COUNT(*) FROM MediaFiles WHERE {whereClause}";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
            
            // Data query with pagination
            var offset = (searchDto.PageNumber - 1) * searchDto.PageSize;
            var dataSql = $@"
                SELECT * FROM MediaFiles 
                WHERE {whereClause}
                ORDER BY {searchDto.OrderBy ?? "CreatedAt DESC"}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pagedParameters = new DynamicParameters(parameters);
            pagedParameters.Add("@Offset", offset);
            pagedParameters.Add("@PageSize", searchDto.PageSize);

            var items = await connection.QueryAsync<MediaFile>(dataSql, pagedParameters);

            return new PagedResult<MediaFile>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged media files");
            throw;
        }
    }
}

// Supporting classes
public class Answer
{
    public Guid QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string? Explanation { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
