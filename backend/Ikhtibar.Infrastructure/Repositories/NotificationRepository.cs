// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using Dapper;
using Ikhtibar.Core.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Notification repository implementation using Dapper
/// </summary>
public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    private readonly ILogger<NotificationRepository> _logger;

    public NotificationRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<NotificationRepository> logger) : base(connectionFactory)
    {
        _logger = logger;
    }

    public async Task<Notification?> GetByIdWithUserAsync(int id)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT n.*, u.*
                FROM Notifications n
                INNER JOIN Users u ON n.UserId = u.UserId
                WHERE n.Id = @Id AND n.IsDeleted = 0";

            var notificationDictionary = new Dictionary<int, Notification>();
            
            var result = await connection.QueryAsync<Notification, User, Notification>(
                sql,
                (notification, user) =>
                {
                    if (!notificationDictionary.TryGetValue(notification.Id, out var existingNotification))
                    {
                        existingNotification = notification;
                        existingNotification.User = user;
                        notificationDictionary.Add(notification.Id, existingNotification);
                    }
                    return existingNotification;
                },
                new { Id = id },
                splitOn: "UserId");

            return result.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification with user by ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, int page, int pageSize)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var offset = (page - 1) * pageSize;
            
            var sql = @"
                SELECT *
                FROM Notifications
                WHERE UserId = @UserId AND IsDeleted = 0
                ORDER BY CreatedAt DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            var result = await connection.QueryAsync<Notification>(sql, new { UserId = userId, Offset = offset, PageSize = pageSize });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for user {UserId}, page {Page}", userId, page);
            throw;
        }
    }

    public async Task<int> GetUserNotificationCountAsync(int userId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = "SELECT COUNT(*) FROM Notifications WHERE UserId = @UserId AND IsDeleted = 0";
            
            var result = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification count for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> GetUserUnreadCountAsync(int userId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT COUNT(*) 
                FROM Notifications 
                WHERE UserId = @UserId AND IsDeleted = 0 AND ReadAt IS NULL";
            
            var result = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                UPDATE Notifications 
                SET ReadAt = @ReadAt, ModifiedAt = @ModifiedAt
                WHERE Id = @Id AND UserId = @UserId AND IsDeleted = 0";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                Id = notificationId, 
                UserId = userId, 
                ReadAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            });
            
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {Id} as read for user {UserId}", notificationId, userId);
            throw;
        }
    }

    public async Task<int> MarkAllAsReadAsync(int userId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                UPDATE Notifications 
                SET ReadAt = @ReadAt, ModifiedAt = @ModifiedAt
                WHERE UserId = @UserId AND IsDeleted = 0 AND ReadAt IS NULL";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                UserId = userId, 
                ReadAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            });
            
            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<Notification>> GetPendingNotificationsAsync(int batchSize)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT *
                FROM Notifications
                WHERE Status = 1 AND IsDeleted = 0 AND ScheduledAt <= @Now
                ORDER BY Priority DESC, ScheduledAt ASC
                OFFSET 0 ROWS
                FETCH NEXT @BatchSize ROWS ONLY";

            var result = await connection.QueryAsync<Notification>(sql, new { Now = DateTime.UtcNow, BatchSize = batchSize });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending notifications with batch size {BatchSize}", batchSize);
            throw;
        }
    }

    public async Task<IEnumerable<Notification>> GetScheduledNotificationsAsync(DateTime before)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT *
                FROM Notifications
                WHERE Status = 1 AND IsDeleted = 0 AND ScheduledAt <= @Before
                ORDER BY ScheduledAt ASC";

            var result = await connection.QueryAsync<Notification>(sql, new { Before = before });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scheduled notifications before {Before}", before);
            throw;
        }
    }
}
*/
