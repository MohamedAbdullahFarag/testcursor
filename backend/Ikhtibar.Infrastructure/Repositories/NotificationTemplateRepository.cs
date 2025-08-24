// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using Dapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Notification template repository implementation using Dapper
/// </summary>
public class NotificationTemplateRepository : BaseRepository<NotificationTemplate>, INotificationTemplateRepository
{
    private readonly ILogger<NotificationTemplateRepository> _logger;

    public NotificationTemplateRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<NotificationTemplateRepository> logger) : base(connectionFactory)
    {
        _logger = logger;
    }

    public async Task<NotificationTemplate?> GetByTypeAndLanguageAsync(NotificationType type, string language)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT *
                FROM NotificationTemplates
                WHERE Type = @Type AND Language = @Language AND IsActive = 1 AND IsDeleted = 0";
            
            var result = await connection.QueryFirstOrDefaultAsync<NotificationTemplate>(sql, new { Type = type, Language = language });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template for type {Type} and language {Language}", type, language);
            throw;
        }
    }

    public async Task<IEnumerable<NotificationTemplate>> GetByTypeAsync(NotificationType type)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT *
                FROM NotificationTemplates
                WHERE Type = @Type AND IsActive = 1 AND IsDeleted = 0
                ORDER BY Language";
            
            var result = await connection.QueryAsync<NotificationTemplate>(sql, new { Type = type });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting templates for type {Type}", type);
            throw;
        }
    }

    public async Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            var sql = @"
                SELECT *
                FROM NotificationTemplates
                WHERE IsActive = 1 AND IsDeleted = 0
                ORDER BY Type, Language";
            
            var result = await connection.QueryAsync<NotificationTemplate>(sql);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active templates");
            throw;
        }
    }
}
*/
