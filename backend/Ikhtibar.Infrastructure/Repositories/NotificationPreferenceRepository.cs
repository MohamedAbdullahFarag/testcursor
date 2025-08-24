// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using Dapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Notification preference repository implementation using Dapper
/// </summary>
public class NotificationPreferenceRepository : BaseRepository<NotificationPreference>, INotificationPreferenceRepository
{
    private readonly ILogger<NotificationPreferenceRepository> _logger;

    public NotificationPreferenceRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<NotificationPreferenceRepository> logger) : base(connectionFactory)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<NotificationPreference>> GetByUserIdAsync(int userId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT *
                FROM NotificationPreferences
                WHERE UserId = @UserId AND IsDeleted = 0
                ORDER BY NotificationType";
            
            var result = await connection.QueryAsync<NotificationPreference>(sql, new { UserId = userId });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting preferences for user {UserId}", userId);
            throw;
        }
    }

    public async Task<NotificationPreference?> GetByUserIdAndTypeAsync(int userId, NotificationType type)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT *
                FROM NotificationPreferences
                WHERE UserId = @UserId AND NotificationType = @Type AND IsDeleted = 0";
            
            var result = await connection.QueryFirstOrDefaultAsync<NotificationPreference>(sql, new { UserId = userId, Type = type });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting preference for user {UserId} and type {Type}", userId, type);
            throw;
        }
    }

    public async Task<bool> DeleteByUserIdAsync(int userId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                UPDATE NotificationPreferences 
                SET IsDeleted = 1, DeletedAt = @DeletedAt, ModifiedAt = @ModifiedAt
                WHERE UserId = @UserId AND IsDeleted = 0";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                UserId = userId, 
                DeletedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            });
            
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting preferences for user {UserId}", userId);
            throw;
        }
    }
}
*/
