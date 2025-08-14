using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Notification template service interface for template management
/// Handles template CRUD operations, variable substitution, and multi-language support
/// Following SRP: ONLY template management operations
/// </summary>
public interface INotificationTemplateService
{
    /// <summary>
    /// Creates a new notification template
    /// </summary>
    /// <param name="dto">Template creation data</param>
    /// <returns>Created template data</returns>
    Task<NotificationTemplateDto> CreateTemplateAsync(CreateNotificationTemplateDto dto);

    /// <summary>
    /// Updates an existing template
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="dto">Template update data</param>
    /// <returns>Updated template data</returns>
    Task<NotificationTemplateDto> UpdateTemplateAsync(Guid templateId, UpdateNotificationTemplateDto dto);

    /// <summary>
    /// Gets a template by ID
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <returns>Template data if found</returns>
    Task<NotificationTemplateDto?> GetTemplateAsync(Guid templateId);

    /// <summary>
    /// Gets template by notification type and language
    /// </summary>
    /// <param name="notificationType">Type of notification</param>
    /// <param name="language">Language code (e.g., "en", "ar")</param>
    /// <returns>Template data if found</returns>
    Task<NotificationTemplateDto?> GetTemplateAsync(NotificationType notificationType, string language);

    /// <summary>
    /// Gets all templates with pagination and filtering
    /// </summary>
    /// <param name="filter">Template filtering and pagination options</param>
    /// <returns>Paginated template results</returns>
    Task<PagedResult<NotificationTemplateDto>> GetTemplatesAsync(TemplateFilterDto filter);

    /// <summary>
    /// Activates or deactivates a template
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="isActive">New active status</param>
    /// <returns>True if successfully updated</returns>
    Task<bool> SetTemplateActiveStatusAsync(Guid templateId, bool isActive);

    /// <summary>
    /// Deletes a template (soft delete)
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <returns>True if successfully deleted</returns>
    Task<bool> DeleteTemplateAsync(Guid templateId);

    /// <summary>
    /// Processes template with variable substitution
    /// </summary>
    /// <param name="template">Template to process</param>
    /// <param name="variables">Variables for substitution</param>
    /// <returns>Processed template content</returns>
    Task<ProcessedTemplate> ProcessTemplateAsync(NotificationTemplateDto template, Dictionary<string, object> variables);

    /// <summary>
    /// Processes template content directly without template entity
    /// </summary>
    /// <param name="templateContent">Template content with placeholders</param>
    /// <param name="variables">Variables for substitution</param>
    /// <returns>Processed content</returns>
    Task<string> ProcessTemplateContentAsync(string templateContent, Dictionary<string, object> variables);

    /// <summary>
    /// Validates template syntax and variable placeholders
    /// </summary>
    /// <param name="templateContent">Template content to validate</param>
    /// <returns>Validation result with errors if any</returns>
    Task<TemplateValidationResult> ValidateTemplateAsync(string templateContent);

    /// <summary>
    /// Gets available template variables for a notification type
    /// </summary>
    /// <param name="notificationType">Type of notification</param>
    /// <returns>List of available variables with descriptions</returns>
    Task<List<TemplateVariable>> GetAvailableVariablesAsync(NotificationType notificationType);

    /// <summary>
    /// Creates default templates for all notification types and languages
    /// </summary>
    /// <param name="overwriteExisting">Whether to overwrite existing templates</param>
    /// <returns>Number of templates created</returns>
    Task<int> CreateDefaultTemplatesAsync(bool overwriteExisting = false);

    /// <summary>
    /// Imports templates from JSON or CSV file
    /// </summary>
    /// <param name="fileContent">File content to import</param>
    /// <param name="fileType">Type of import file</param>
    /// <returns>Import result with counts and errors</returns>
    Task<TemplateImportResult> ImportTemplatesAsync(string fileContent, TemplateImportType fileType);

    /// <summary>
    /// Exports templates to JSON format
    /// </summary>
    /// <param name="language">Optional language filter</param>
    /// <param name="notificationType">Optional notification type filter</param>
    /// <returns>JSON representation of templates</returns>
    Task<string> ExportTemplatesAsync(string? language = null, NotificationType? notificationType = null);

    /// <summary>
    /// Gets template usage statistics
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Template usage statistics</returns>
    Task<TemplateUsageStats> GetTemplateUsageStatsAsync(Guid templateId, DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Duplicates a template with new language
    /// </summary>
    /// <param name="sourceTemplateId">Source template ID</param>
    /// <param name="targetLanguage">Target language code</param>
    /// <param name="translateContent">Whether to auto-translate content</param>
    /// <returns>Duplicated template data</returns>
    Task<NotificationTemplateDto> DuplicateTemplateAsync(Guid sourceTemplateId, string targetLanguage, bool translateContent = false);
}

/// <summary>
/// Notification preference service interface for user preference management
/// Handles user notification delivery preferences and quiet hours
/// Following SRP: ONLY preference management operations
/// </summary>
public interface INotificationPreferenceService
{
    /// <summary>
    /// Gets all preferences for a user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <returns>List of user notification preferences</returns>
    Task<List<NotificationPreferenceDto>> GetUserPreferencesAsync(int userId);

    /// <summary>
    /// Gets preference for specific user and notification type
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="notificationType">Type of notification</param>
    /// <returns>Preference data if found</returns>
    Task<NotificationPreferenceDto?> GetUserPreferenceAsync(int userId, NotificationType notificationType);

    /// <summary>
    /// Updates user preference for a notification type
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="dto">Preference update data</param>
    /// <returns>Updated preference data</returns>
    Task<NotificationPreferenceDto> UpdateUserPreferenceAsync(int userId, UpdateNotificationPreferenceDto dto);

    /// <summary>
    /// Updates multiple preferences for a user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="preferences">List of preference updates</param>
    /// <returns>Number of preferences updated</returns>
    Task<int> UpdateUserPreferencesAsync(int userId, List<UpdateNotificationPreferenceDto> preferences);

    /// <summary>
    /// Creates default preferences for a new user
    /// </summary>
    /// <param name="userId">New user ID</param>
    /// <returns>List of created default preferences</returns>
    Task<List<NotificationPreferenceDto>> CreateDefaultPreferencesAsync(int userId);

    /// <summary>
    /// Resets user preferences to system defaults
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <returns>Number of preferences reset</returns>
    Task<int> ResetToDefaultsAsync(int userId);

    /// <summary>
    /// Updates quiet hours for a user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="dto">Quiet hours update data</param>
    /// <returns>Number of preferences updated</returns>
    Task<int> UpdateQuietHoursAsync(int userId, QuietHoursDto dto);

    /// <summary>
    /// Checks if user should receive notification based on preferences and quiet hours
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="notificationType">Type of notification</param>
    /// <param name="channel">Delivery channel</param>
    /// <param name="checkTime">Time to check (defaults to current time)</param>
    /// <returns>True if user should receive notification via this channel</returns>
    Task<bool> ShouldReceiveNotificationAsync(int userId, NotificationType notificationType, NotificationChannel channel, DateTime? checkTime = null);

    /// <summary>
    /// Gets users who have enabled a specific channel for notification type
    /// </summary>
    /// <param name="notificationType">Type of notification</param>
    /// <param name="channel">Delivery channel</param>
    /// <returns>List of user IDs who should receive notifications</returns>
    Task<List<int>> GetUsersWithChannelEnabledAsync(NotificationType notificationType, NotificationChannel channel);

    /// <summary>
    /// Gets preference summary for admin dashboard
    /// </summary>
    /// <returns>Preference statistics across all users</returns>
    Task<PreferenceSummary> GetPreferenceSummaryAsync();

    /// <summary>
    /// Bulk updates preferences for multiple users (admin operation)
    /// </summary>
    /// <param name="userIds">List of user IDs</param>
    /// <param name="notificationType">Type of notification</param>
    /// <param name="channelUpdates">Channel preference updates</param>
    /// <returns>Number of users updated</returns>
    Task<int> BulkUpdatePreferencesAsync(List<int> userIds, NotificationType notificationType, Dictionary<NotificationChannel, bool> channelUpdates);
}

/// <summary>
/// Processed template result
/// </summary>
public class ProcessedTemplate
{
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? EmailHtml { get; set; }
    public string? SmsContent { get; set; }
    public string? PushTitle { get; set; }
    public string? PushBody { get; set; }
}

/// <summary>
/// Template validation result
/// </summary>
public class TemplateValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> FoundVariables { get; set; } = new();
}

/// <summary>
/// Template variable definition
/// </summary>
public class TemplateVariable
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public bool IsRequired { get; set; }
    public string? Example { get; set; }
}

/// <summary>
/// Template import result
/// </summary>
public class TemplateImportResult
{
    public int TemplatesCreated { get; set; }
    public int TemplatesUpdated { get; set; }
    public int TemplatesSkipped { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
