// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using Dapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Notification history repository implementation using Dapper
/// </summary>
public class NotificationHistoryRepository : BaseRepository<NotificationHistory>, INotificationHistoryRepository
{
    private readonly ILogger<NotificationHistoryRepository> _logger;

    public NotificationHistoryRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<NotificationHistoryRepository> logger) : base(connectionFactory)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<NotificationHistory>> GetByNotificationIdAsync(int notificationId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT *
                FROM NotificationHistory
                WHERE NotificationId = @NotificationId AND IsDeleted = 0
                ORDER BY AttemptedAt DESC";
            
            var result = await connection.QueryAsync<NotificationHistory>(sql, new { NotificationId = notificationId });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting history for notification {NotificationId}", notificationId);
            throw;
        }
    }

    public async Task<IEnumerable<NotificationHistory>> GetFailedDeliveriesAsync(int batchSize)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT *
                FROM NotificationHistory
                WHERE Status = 4 AND IsDeleted = 0
                ORDER BY AttemptedAt ASC
                OFFSET 0 ROWS
                FETCH NEXT @BatchSize ROWS ONLY";
            
            var result = await connection.QueryAsync<NotificationHistory>(sql, new { BatchSize = batchSize });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting failed deliveries with batch size {BatchSize}", batchSize);
            throw;
        }
    }

    public async Task<int> GetDeliverySuccessRateAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT 
                    COUNT(*) as TotalAttempts,
                    SUM(CASE WHEN Status IN (2, 3, 6, 7) THEN 1 ELSE 0 END) as SuccessfulDeliveries
                FROM NotificationHistory
                WHERE AttemptedAt BETWEEN @FromDate AND @ToDate AND IsDeleted = 0";
            
            var result = await connection.QueryFirstAsync(sql, new { FromDate = fromDate, ToDate = toDate });
            
            var totalAttempts = result.TotalAttempts;
            var successfulDeliveries = result.SuccessfulDeliveries;
            
            if (totalAttempts == 0) return 0;
            
            var successRate = (int)((double)successfulDeliveries / totalAttempts * 100);
            return successRate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting delivery success rate from {FromDate} to {ToDate}", fromDate, toDate);
            throw;
        }
    }
}
*/
