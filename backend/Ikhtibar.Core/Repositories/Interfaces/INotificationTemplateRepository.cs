using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for notification template entities
/// Handles template management and retrieval operations
/// </summary>
public interface INotificationTemplateRepository : IRepository<NotificationTemplate>
{
    /// <summary>
    /// Gets template by notification type and language
    /// </summary>
    /// <param name="notificationType">Type of notification</param>
    /// <param name="language">Language code (e.g., "en", "ar")</param>
    /// <returns>Template if found, null otherwise</returns>
    Task<NotificationTemplate?> GetByTypeAndLanguageAsync(NotificationType notificationType, string language);

    /// <summary>
    /// Gets all active templates for a specific notification type
    /// </summary>
    /// <param name="notificationType">Type of notification</param>
    /// <returns>List of active templates for the type</returns>
    Task<IEnumerable<NotificationTemplate>> GetActiveByTypeAsync(NotificationType notificationType);

    /// <summary>
    /// Gets template by name and language
    /// </summary>
    /// <param name="name">Template name</param>
    /// <param name="language">Language code</param>
    /// <returns>Template if found, null otherwise</returns>
    Task<NotificationTemplate?> GetByNameAndLanguageAsync(string name, string language);

    /// <summary>
    /// Gets all templates with pagination and filtering
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of templates per page</param>
    /// <param name="language">Optional language filter</param>
    /// <param name="notificationType">Optional notification type filter</param>
    /// <param name="activeOnly">If true, returns only active templates</param>
    /// <returns>Paginated list of templates</returns>
    Task<(IEnumerable<NotificationTemplate> Templates, int TotalCount)> GetTemplatesAsync(
        int page = 1, 
        int pageSize = 20,
        string? language = null,
        NotificationType? notificationType = null,
        bool activeOnly = true);

    /// <summary>
    /// Gets all supported languages for templates
    /// </summary>
    /// <returns>List of language codes that have templates</returns>
    Task<IEnumerable<string>> GetSupportedLanguagesAsync();

    /// <summary>
    /// Activates or deactivates a template
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="isActive">New active status</param>
    /// <returns>True if successfully updated</returns>
    Task<bool> SetActiveStatusAsync(Guid templateId, bool isActive);

    /// <summary>
    /// Gets template usage statistics
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Usage statistics for the template</returns>
    Task<TemplateUsageStats> GetUsageStatsAsync(Guid templateId, DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Checks if a template name exists for a specific language
    /// </summary>
    /// <param name="name">Template name</param>
    /// <param name="language">Language code</param>
    /// <param name="excludeId">Optional ID to exclude from check (for updates)</param>
    /// <returns>True if template name exists</returns>
    Task<bool> TemplateNameExistsAsync(string name, string language, Guid? excludeId = null);
}
