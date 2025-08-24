using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for MediaThumbnail entity operations
/// </summary>
public class MediaThumbnailRepository : BaseRepository<MediaThumbnail>, IMediaThumbnailRepository
{
    private new readonly ILogger<MediaThumbnailRepository> _logger;

    public MediaThumbnailRepository(IDbConnectionFactory connectionFactory, ILogger<MediaThumbnailRepository> logger)
        : base(connectionFactory, logger, "MediaThumbnails", "Id")
    {
        _logger = logger;
    }

    public async Task<IEnumerable<MediaThumbnail>> GetByMediaFileAsync(int mediaFileId, ThumbnailStatus? statusFilter = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT * FROM MediaThumbnails WHERE MediaFileId = @MediaFileId AND IsDeleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("MediaFileId", mediaFileId);
            
            if (statusFilter.HasValue)
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", (int)statusFilter.Value);
            }
            
            sql += " ORDER BY Size, Width, Height";
            
            var thumbnails = await connection.QueryAsync<MediaThumbnail>(sql, parameters);
            return thumbnails;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving thumbnails for media file {MediaFileId}", mediaFileId);
            throw;
        }
    }

    public async Task<MediaThumbnail?> GetByMediaFileAndSizeAsync(int mediaFileId, ThumbnailSize size, string? format = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT * FROM MediaThumbnails WHERE MediaFileId = @MediaFileId AND Size = @Size AND IsDeleted = 0";
            var parameters = new DynamicParameters();
            parameters.Add("MediaFileId", mediaFileId);
            parameters.Add("Size", (int)size);
            
            if (!string.IsNullOrEmpty(format))
            {
                sql += " AND Format = @Format";
                parameters.Add("Format", format);
            }
            
            sql += " ORDER BY IsDefault DESC, CreatedAt DESC";
            
            var thumbnail = await connection.QueryFirstOrDefaultAsync<MediaThumbnail>(sql, parameters);
            return thumbnail;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving thumbnail for media file {MediaFileId}, size {Size}", mediaFileId, size);
            throw;
        }
    }

    public async Task<IEnumerable<MediaThumbnail>> GetByMediaFileIdAsync(int mediaFileId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT * FROM MediaThumbnails WHERE MediaFileId = @MediaFileId AND IsDeleted = 0";
            var thumbnails = await connection.QueryAsync<MediaThumbnail>(sql, new { MediaFileId = mediaFileId });
            return thumbnails;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving thumbnails for media file {MediaFileId}", mediaFileId);
            throw;
        }
    }

    public async Task<MediaThumbnail?> GetDefaultThumbnailAsync(int mediaFileId, ThumbnailSize size)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT * FROM MediaThumbnails WHERE MediaFileId = @MediaFileId AND Size = @Size AND IsDefault = 1 AND IsDeleted = 0";
            var thumbnail = await connection.QueryFirstOrDefaultAsync<MediaThumbnail>(sql, new { MediaFileId = mediaFileId, Size = (int)size });
            return thumbnail;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving default thumbnail for media file {MediaFileId}, size {Size}", mediaFileId, size);
            throw;
        }
    }

    public async Task<IEnumerable<MediaThumbnail>> GetByStatusAsync(ThumbnailStatus status, int limit = 100)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT TOP (@Limit) * FROM MediaThumbnails WHERE Status = @Status AND IsDeleted = 0 ORDER BY CreatedAt";
            var thumbnails = await connection.QueryAsync<MediaThumbnail>(sql, new { Status = (int)status, Limit = limit });
            return thumbnails;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving thumbnails by status {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<MediaThumbnail>> GetByFormatAsync(string format)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT * FROM MediaThumbnails WHERE Format = @Format AND IsDeleted = 0 ORDER BY CreatedAt DESC";
            var thumbnails = await connection.QueryAsync<MediaThumbnail>(sql, new { Format = format });
            return thumbnails;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving thumbnails by format {Format}", format);
            throw;
        }
    }

    public async Task<IEnumerable<MediaThumbnail>> GetLargerThanAsync(int minWidth, int minHeight)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT * FROM MediaThumbnails WHERE Width >= @MinWidth AND Height >= @MinHeight AND IsDeleted = 0 ORDER BY Width DESC, Height DESC";
            var thumbnails = await connection.QueryAsync<MediaThumbnail>(sql, new { MinWidth = minWidth, MinHeight = minHeight });
            return thumbnails;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving thumbnails larger than {MinWidth}x{MinHeight}", minWidth, minHeight);
            throw;
        }
    }

    public async Task<IEnumerable<MediaThumbnail>> GetNeedingRegenerationAsync(int olderThanDays = 30, int limit = 100)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
            var processingCutoffDate = DateTime.UtcNow.AddHours(-1);
            
            var sql = @"SELECT TOP (@Limit) * FROM MediaThumbnails 
                       WHERE IsDeleted = 0 AND (
                           Status = @FailedStatus OR 
                           (Status = @GeneratingStatus AND CreatedAt < @CutoffDate) OR
                           (Status = @GeneratingStatus AND CreatedAt < @ProcessingCutoffDate)
                       ) ORDER BY CreatedAt";
            
            var thumbnails = await connection.QueryAsync<MediaThumbnail>(sql, new
            {
                Limit = limit,
                FailedStatus = (int)ThumbnailStatus.Failed,
                GeneratingStatus = (int)ThumbnailStatus.Generating,
                CutoffDate = cutoffDate,
                ProcessingCutoffDate = processingCutoffDate
            });
            return thumbnails;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving thumbnails needing regeneration");
            throw;
        }
    }

    public async Task<IEnumerable<MediaThumbnail>> GetByGenerationMethodAsync(ThumbnailGenerationMethod method)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT * FROM MediaThumbnails WHERE GenerationMethod = @Method AND IsDeleted = 0 ORDER BY CreatedAt DESC";
            var thumbnails = await connection.QueryAsync<MediaThumbnail>(sql, new { Method = (int)method });
            return thumbnails;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving thumbnails by generation method {Method}", method);
            throw;
        }
    }

    public async Task<long> GetTotalStorageUsedAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT ISNULL(SUM(FileSizeBytes), 0) FROM MediaThumbnails WHERE IsDeleted = 0";
            var totalStorage = await connection.QuerySingleAsync<long>(sql);
            return totalStorage;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error calculating total thumbnail storage");
            throw;
        }
    }

    public async Task<Dictionary<ThumbnailSize, long>> GetStorageStatsBySizeAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT Size, ISNULL(SUM(FileSizeBytes), 0) as TotalSize FROM MediaThumbnails WHERE IsDeleted = 0 GROUP BY Size";
            var results = await connection.QueryAsync<(int Size, long TotalSize)>(sql);
            var statistics = results.ToDictionary(r => (ThumbnailSize)r.Size, r => r.TotalSize);
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving storage statistics by size");
            throw;
        }
    }

    public async Task<Dictionary<string, long>> GetStorageStatsByFormatAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT Format, ISNULL(SUM(FileSizeBytes), 0) as TotalSize FROM MediaThumbnails WHERE IsDeleted = 0 GROUP BY Format ORDER BY TotalSize DESC";
            var results = await connection.QueryAsync<(string Format, long TotalSize)>(sql);
            var statistics = results.ToDictionary(r => r.Format, r => r.TotalSize);
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving storage statistics by format");
            throw;
        }
    }

    public async Task<Dictionary<ThumbnailSize, double>> GetGenerationStatsAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT Size, AVG(CAST(GenerationTimeMs as float)) as AvgTime FROM MediaThumbnails WHERE IsDeleted = 0 AND GenerationTimeMs IS NOT NULL AND GenerationTimeMs > 0 GROUP BY Size";
            var results = await connection.QueryAsync<(int Size, double AvgTime)>(sql);
            var statistics = results.ToDictionary(r => (ThumbnailSize)r.Size, r => r.AvgTime);
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving generation statistics");
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int mediaFileId, ThumbnailSize size, int width, int height, string format)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT COUNT(1) FROM MediaThumbnails WHERE MediaFileId = @MediaFileId AND Size = @Size AND Width = @Width AND Height = @Height AND Format = @Format AND IsDeleted = 0";
            var count = await connection.QuerySingleAsync<int>(sql, new
            {
                MediaFileId = mediaFileId,
                Size = (int)size,
                Width = width,
                Height = height,
                Format = format
            });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error checking thumbnail existence for media file {MediaFileId}", mediaFileId);
            throw;
        }
    }

    public async Task<bool> SetAsDefaultAsync(int thumbnailId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // First, get the thumbnail to know its MediaFileId and Size
                var getThumbnailSql = "SELECT MediaFileId, Size FROM MediaThumbnails WHERE Id = @ThumbnailId AND IsDeleted = 0";
                var thumbnailInfo = await connection.QueryFirstOrDefaultAsync<(int MediaFileId, int Size)>(getThumbnailSql, new { ThumbnailId = thumbnailId }, transaction);
                
                if (thumbnailInfo.MediaFileId == 0)
                {
                    transaction.Rollback();
                    return false;
                }
                
                // Clear existing default for this media file and size
                var clearDefaultSql = "UPDATE MediaThumbnails SET IsDefault = 0, ModifiedAt = @ModifiedAt WHERE MediaFileId = @MediaFileId AND Size = @Size AND IsDefault = 1 AND IsDeleted = 0";
                await connection.ExecuteAsync(clearDefaultSql, new
                {
                    MediaFileId = thumbnailInfo.MediaFileId,
                    Size = thumbnailInfo.Size,
                    ModifiedAt = DateTime.UtcNow
                }, transaction);
                
                // Set the new default
                var setDefaultSql = "UPDATE MediaThumbnails SET IsDefault = 1, ModifiedAt = @ModifiedAt WHERE Id = @ThumbnailId";
                var affectedRows = await connection.ExecuteAsync(setDefaultSql, new
                {
                    ThumbnailId = thumbnailId,
                    ModifiedAt = DateTime.UtcNow
                }, transaction);
                
                transaction.Commit();
                return affectedRows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error setting thumbnail {ThumbnailId} as default", thumbnailId);
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(int thumbnailId, ThumbnailStatus status, string? errorMessage = null, int? generationTimeMs = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "UPDATE MediaThumbnails SET Status = @Status, ErrorMessage = @ErrorMessage, GenerationTimeMs = COALESCE(@GenerationTimeMs, GenerationTimeMs), ModifiedAt = @ModifiedAt WHERE Id = @ThumbnailId AND IsDeleted = 0";
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                Status = (int)status,
                ErrorMessage = errorMessage,
                GenerationTimeMs = generationTimeMs,
                ModifiedAt = DateTime.UtcNow,
                ThumbnailId = thumbnailId
            });
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error updating thumbnail {ThumbnailId} status", thumbnailId);
            throw;
        }
    }

    public async Task<int> BulkUpdateStatusAsync(IEnumerable<int> thumbnailIds, ThumbnailStatus status)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var idsArray = thumbnailIds.ToArray();
            if (!idsArray.Any())
            {
                return 0;
            }
            
            var sql = "UPDATE MediaThumbnails SET Status = @Status, ModifiedAt = @ModifiedAt WHERE Id IN @ThumbnailIds AND IsDeleted = 0";
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                Status = (int)status,
                ModifiedAt = DateTime.UtcNow,
                ThumbnailIds = idsArray
            });
            return affectedRows;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error bulk updating thumbnail status");
            throw;
        }
    }

    public async Task<int> DeleteByMediaFileAsync(int mediaFileId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "UPDATE MediaThumbnails SET IsDeleted = 1, ModifiedAt = @ModifiedAt WHERE MediaFileId = @MediaFileId AND IsDeleted = 0";
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                MediaFileId = mediaFileId,
                ModifiedAt = DateTime.UtcNow
            });
            return affectedRows;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error deleting thumbnails for media file {MediaFileId}", mediaFileId);
            throw;
        }
    }

    public async Task<int> DeleteByStatusAsync(ThumbnailStatus status, int olderThanDays = 7)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
            var sql = "UPDATE MediaThumbnails SET IsDeleted = 1, ModifiedAt = @ModifiedAt WHERE Status = @Status AND CreatedAt < @CutoffDate AND IsDeleted = 0";
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                Status = (int)status,
                CutoffDate = cutoffDate,
                ModifiedAt = DateTime.UtcNow
            });
            return affectedRows;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error deleting thumbnails by status {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<MediaThumbnail>> GetOrphanedThumbnailsAsync(int limit = 1000)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT TOP (@Limit) t.* FROM MediaThumbnails t LEFT JOIN MediaFiles mf ON t.MediaFileId = mf.Id AND mf.IsDeleted = 0 WHERE t.IsDeleted = 0 AND mf.Id IS NULL ORDER BY t.CreatedAt";
            var orphanedThumbnails = await connection.QueryAsync<MediaThumbnail>(sql, new { Limit = limit });
            return orphanedThumbnails;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, "Error retrieving orphaned thumbnails");
            throw;
        }
    }
}
