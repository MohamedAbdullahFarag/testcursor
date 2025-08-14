using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Core.Integrations.SmsProviders;
namespace Ikhtibar.Core.Services;

/// <summary>
/// Service interface for notification management operations
/// </summary>
public interface INotificationService
{
    // Core notification operations
    
    /// <summary>
    /// Creates a new notification
    /// </summary>
    /// <param name="dto">Notification creation data</param>
    /// <returns>Created notification details</returns>
    Task<OperationResult<NotificationResponseDto>> CreateNotificationAsync(CreateNotificationDto dto);
    
    /// <summary>
    /// Updates an existing notification
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="dto">Updated notification data</param>
    /// <returns>Updated notification details</returns>
    Task<OperationResult<NotificationResponseDto>> UpdateNotificationAsync(Guid id, UpdateNotificationDto dto);
    
    /// <summary>
    /// Gets a notification by ID
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <returns>Notification details</returns>
    Task<OperationResult<NotificationResponseDto>> GetNotificationAsync(Guid id);
    
    /// <summary>
    /// Gets paginated list of notifications
    /// </summary>
    /// <param name="filters">Filter criteria</param>
    /// <returns>Paginated notification list</returns>
    Task<OperationResult<PagedResult<NotificationResponseDto>>> GetNotificationsAsync(NotificationFilterDto filters);
    
    /// <summary>
    /// Deletes a notification (soft delete)
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <returns>Operation result</returns>
    Task<OperationResult> DeleteNotificationAsync(Guid id);
    
    // Notification processing and delivery
    
    /// <summary>
    /// Sends a notification immediately
    /// </summary>
    /// <param name="notificationId">Notification ID to send</param>
    /// <returns>Delivery result</returns>
    Task<OperationResult<DeliveryResultDto>> SendNotificationAsync(Guid notificationId);
    
    /// <summary>
    /// Sends notification to specific users
    /// </summary>
    /// <param name="dto">Targeted notification data</param>
    /// <returns>Delivery results</returns>
    Task<OperationResult<List<DeliveryResultDto>>> SendToUsersAsync(SendToUsersDto dto);
    
    /// <summary>
    /// Schedules a notification for future delivery
    /// </summary>
    /// <param name="dto">Scheduled notification data</param>
    /// <returns>Scheduled notification details</returns>
    Task<OperationResult<NotificationResponseDto>> ScheduleNotificationAsync(ScheduleNotificationDto dto);
    
    /// <summary>
    /// Cancels a scheduled notification
    /// </summary>
    /// <param name="notificationId">Notification ID to cancel</param>
    /// <returns>Operation result</returns>
    Task<OperationResult> CancelScheduledNotificationAsync(Guid notificationId);
    
    /// <summary>
    /// Sends bulk notifications
    /// </summary>
    /// <param name="dto">Bulk notification data</param>
    /// <returns>Bulk operation result</returns>
    Task<OperationResult<BatchOperationResult>> SendBulkNotificationsAsync(BulkNotificationDto dto);
    
    // Analytics and reporting
    
    /// <summary>
    /// Gets notification analytics for a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="groupBy">Grouping criteria</param>
    /// <returns>Analytics data</returns>
    Task<OperationResult<NotificationAnalyticsDto>> GetAnalyticsAsync(DateTime startDate, DateTime endDate, string? groupBy = null);
    
    /// <summary>
    /// Gets user-specific notification statistics
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="days">Number of days to look back</param>
    /// <returns>User notification statistics</returns>
    Task<OperationResult<UserNotificationStatsDto>> GetUserStatsAsync(int userId, int days = 30);
    
    /// <summary>
    /// Gets notification performance metrics
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Performance metrics</returns>
    Task<OperationResult<NotificationPerformanceDto>> GetPerformanceMetricsAsync(DateTime startDate, DateTime endDate);
    
    // Event-driven operations
    
    /// <summary>
    /// Processes scheduled notifications that are due
    /// </summary>
    /// <returns>Number of notifications processed</returns>
    Task<OperationResult<int>> ProcessScheduledNotificationsAsync();
    
    /// <summary>
    /// Retries failed notifications
    /// </summary>
    /// <param name="maxRetries">Maximum number of retries</param>
    /// <returns>Number of notifications retried</returns>
    Task<OperationResult<int>> RetryFailedNotificationsAsync(int maxRetries = 3);
    
    /// <summary>
    /// Handles notification events (opened, clicked, etc.)
    /// </summary>
    /// <param name="eventType">Type of event</param>
    /// <param name="notificationId">Notification ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="metadata">Additional event data</param>
    /// <returns>Operation result</returns>
    Task<OperationResult> HandleNotificationEventAsync(string eventType, Guid notificationId, int userId, Dictionary<string, object>? metadata = null);
    
    // Health and monitoring
    
    /// <summary>
    /// Checks the health of notification services
    /// </summary>
    /// <returns>Health check results</returns>
    Task<OperationResult<List<HealthCheckResult>>> CheckHealthAsync();
    
    /// <summary>
    /// Gets service status and metrics
    /// </summary>
    /// <returns>Service status information</returns>
    Task<OperationResult<NotificationServiceStatusDto>> GetServiceStatusAsync();
}

/// <summary>
/// Service interface for notification template management
/// </summary>
public interface INotificationTemplateService
{
    // Template CRUD operations
    
    /// <summary>
    /// Creates a new notification template
    /// </summary>
    /// <param name="dto">Template creation data</param>
    /// <returns>Created template details</returns>
    Task<OperationResult<NotificationTemplateResponseDto>> CreateTemplateAsync(CreateNotificationTemplateDto dto);
    
    /// <summary>
    /// Updates an existing template
    /// </summary>
    /// <param name="id">Template ID</param>
    /// <param name="dto">Updated template data</param>
    /// <returns>Updated template details</returns>
    Task<OperationResult<NotificationTemplateResponseDto>> UpdateTemplateAsync(Guid id, UpdateNotificationTemplateDto dto);
    
    /// <summary>
    /// Gets a template by ID
    /// </summary>
    /// <param name="id">Template ID</param>
    /// <returns>Template details</returns>
    Task<OperationResult<NotificationTemplateResponseDto>> GetTemplateAsync(Guid id);
    
    /// <summary>
    /// Gets paginated list of templates
    /// </summary>
    /// <param name="filters">Filter criteria</param>
    /// <returns>Paginated template list</returns>
    Task<OperationResult<PagedResult<NotificationTemplateResponseDto>>> GetTemplatesAsync(TemplateFilterDto filters);
    
    /// <summary>
    /// Deletes a template (soft delete)
    /// </summary>
    /// <param name="id">Template ID</param>
    /// <returns>Operation result</returns>
    Task<OperationResult> DeleteTemplateAsync(Guid id);
    
    // Template processing operations
    
    /// <summary>
    /// Processes a template with variables
    /// </summary>
    /// <param name="dto">Template processing data</param>
    /// <returns>Processed template content</returns>
    Task<OperationResult<ProcessedTemplateDto>> ProcessTemplateAsync(ProcessTemplateDto dto);
    
    /// <summary>
    /// Validates template syntax and variables
    /// </summary>
    /// <param name="dto">Template validation data</param>
    /// <returns>Validation results</returns>
    Task<OperationResult<TemplateValidationResultDto>> ValidateTemplateAsync(ValidateTemplateDto dto);
    
    /// <summary>
    /// Gets template by name and language
    /// </summary>
    /// <param name="name">Template name</param>
    /// <param name="language">Language code</param>
    /// <returns>Template details</returns>
    Task<OperationResult<NotificationTemplateResponseDto>> GetByNameAndLanguageAsync(string name, string language);
    
    /// <summary>
    /// Gets active templates for a notification type
    /// </summary>
    /// <param name="type">Notification type</param>
    /// <returns>List of active templates</returns>
    Task<OperationResult<List<NotificationTemplateResponseDto>>> GetActiveTemplatesAsync(NotificationType type);
    
    // Template management operations
    
    /// <summary>
    /// Sets template active status
    /// </summary>
    /// <param name="id">Template ID</param>
    /// <param name="isActive">Active status</param>
    /// <returns>Operation result</returns>
    Task<OperationResult> SetActiveStatusAsync(Guid id, bool isActive);
    
    /// <summary>
    /// Clones an existing template
    /// </summary>
    /// <param name="id">Template ID to clone</param>
    /// <param name="newName">New template name</param>
    /// <returns>Cloned template details</returns>
    Task<OperationResult<NotificationTemplateResponseDto>> CloneTemplateAsync(Guid id, string newName);
    
    /// <summary>
    /// Imports templates from external source
    /// </summary>
    /// <param name="dto">Import data</param>
    /// <returns>Import result</returns>
    Task<OperationResult<BatchOperationResult>> ImportTemplatesAsync(ImportTemplatesDto dto);
    
    /// <summary>
    /// Exports templates to specified format
    /// </summary>
    /// <param name="dto">Export criteria</param>
    /// <returns>Export data</returns>
    Task<OperationResult<ExportTemplatesResultDto>> ExportTemplatesAsync(ExportTemplatesDto dto);
    
    // Analytics and monitoring
    
    /// <summary>
    /// Gets template usage statistics
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Usage statistics</returns>
    Task<OperationResult<TemplateUsageStats>> GetUsageStatsAsync(Guid templateId, DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Gets all template usage analytics
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Comprehensive usage analytics</returns>
    Task<OperationResult<List<TemplateUsageStats>>> GetAllUsageStatsAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Gets supported languages for templates
    /// </summary>
    /// <returns>List of supported language codes</returns>
    Task<OperationResult<List<string>>> GetSupportedLanguagesAsync();
}

/// <summary>
/// Service interface for notification preference management
/// </summary>
public interface INotificationPreferenceService
{
    // User preference operations
    
    /// <summary>
    /// Gets user notification preferences
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User preferences</returns>
    Task<OperationResult<List<NotificationPreferenceResponseDto>>> GetUserPreferencesAsync(int userId);
    
    /// <summary>
    /// Gets specific preference for user and notification type
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="type">Notification type</param>
    /// <returns>User preference</returns>
    Task<OperationResult<NotificationPreferenceResponseDto>> GetUserPreferenceAsync(int userId, NotificationType type);
    
    /// <summary>
    /// Updates user notification preference
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="dto">Preference update data</param>
    /// <returns>Updated preference</returns>
    Task<OperationResult<NotificationPreferenceResponseDto>> UpdatePreferenceAsync(int userId, UpdateNotificationPreferenceDto dto);
    
    /// <summary>
    /// Creates or updates notification preference
    /// </summary>
    /// <param name="dto">Preference creation data</param>
    /// <returns>Preference details</returns>
    Task<OperationResult<NotificationPreferenceResponseDto>> CreateOrUpdatePreferenceAsync(CreateNotificationPreferenceDto dto);
    
    /// <summary>
    /// Creates default preferences for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Operation result</returns>
    Task<OperationResult> CreateDefaultPreferencesAsync(int userId);
    
    // Bulk operations
    
    /// <summary>
    /// Updates preferences for multiple users
    /// </summary>
    /// <param name="dto">Bulk update data</param>
    /// <returns>Batch operation result</returns>
    Task<OperationResult<BatchOperationResult>> BulkUpdatePreferencesAsync(BulkUpdatePreferencesDto dto);
    
    /// <summary>
    /// Resets user preferences to defaults
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Operation result</returns>
    Task<OperationResult> ResetToDefaultsAsync(int userId);
    
    // Quiet hours management
    
    /// <summary>
    /// Updates user quiet hours settings
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="quietHours">Quiet hours configuration</param>
    /// <returns>Operation result</returns>
    Task<OperationResult> UpdateQuietHoursAsync(int userId, QuietHoursDto quietHours);
    
    /// <summary>
    /// Checks if current time is within user's quiet hours
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="checkTime">Time to check (uses current time if null)</param>
    /// <returns>True if within quiet hours</returns>
    Task<OperationResult<bool>> IsWithinQuietHoursAsync(int userId, DateTime? checkTime = null);
    
    // Preference validation and checking
    
    /// <summary>
    /// Checks if user can receive a specific notification type via channel
    /// </summary>
    /// <param name="dto">Preference check data</param>
    /// <returns>True if user can receive notification</returns>
    Task<OperationResult<bool>> CanReceiveNotificationAsync(CheckPreferencesDto dto);
    
    /// <summary>
    /// Gets preferred delivery channels for user and notification type
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="type">Notification type</param>
    /// <returns>List of preferred channels</returns>
    Task<OperationResult<List<NotificationChannel>>> GetPreferredChannelsAsync(int userId, NotificationType type);
    
    // Analytics and reporting
    
    /// <summary>
    /// Gets preference summary for all users
    /// </summary>
    /// <returns>Preference summary data</returns>
    Task<OperationResult<PreferenceSummary>> GetPreferenceSummaryAsync();
    
    /// <summary>
    /// Gets user preference summary
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User-specific preference summary</returns>
    Task<OperationResult<UserPreferenceSummaryDto>> GetUserPreferenceSummaryAsync(int userId);
    
    /// <summary>
    /// Gets preference analytics for admin dashboard
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Preference analytics</returns>
    Task<OperationResult<PreferenceAnalyticsDto>> GetPreferenceAnalyticsAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// Service interface for communication channels (email, SMS, push)
/// </summary>
public interface ICommunicationService
{
    // Email operations
    
    /// <summary>
    /// Sends an email message
    /// </summary>
    /// <param name="request">Email request data</param>
    /// <returns>Email delivery result</returns>
    Task<OperationResult<EmailResult>> SendEmailAsync(EmailRequest request);
    
    /// <summary>
    /// Sends bulk email messages
    /// </summary>
    /// <param name="request">Bulk email request data</param>
    /// <returns>Bulk email delivery results</returns>
    Task<OperationResult<BulkEmailResult>> SendBulkEmailAsync(BulkEmailRequest request);
    
    /// <summary>
    /// Validates email addresses
    /// </summary>
    /// <param name="emails">List of email addresses to validate</param>
    /// <returns>Validation results</returns>
    Task<OperationResult<List<EmailValidationResult>>> ValidateEmailsAsync(List<string> emails);
    
    /// <summary>
    /// Gets email delivery statistics
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Email statistics</returns>
    Task<OperationResult<EmailStats>> GetEmailStatsAsync(DateTime startDate, DateTime endDate);
    
    // SMS operations
    
    /// <summary>
    /// Sends an SMS message
    /// </summary>
    /// <param name="request">SMS request data</param>
    /// <returns>SMS delivery result</returns>
    Task<OperationResult<SmsResult>> SendSmsAsync(SmsRequest request);
    
    /// <summary>
    /// Sends bulk SMS messages
    /// </summary>
    /// <param name="request">Bulk SMS request data</param>
    /// <returns>Bulk SMS delivery results</returns>
    Task<OperationResult<BulkSmsResult>> SendBulkSmsAsync(BulkSmsRequest request);
    
    /// <summary>
    /// Validates phone numbers
    /// </summary>
    /// <param name="phoneNumbers">List of phone numbers to validate</param>
    /// <returns>Validation results</returns>
    Task<OperationResult<List<PhoneValidationResult>>> ValidatePhoneNumbersAsync(List<string> phoneNumbers);
    
    /// <summary>
    /// Gets SMS delivery statistics
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>SMS statistics</returns>
    Task<OperationResult<SmsStats>> GetSmsStatsAsync(DateTime startDate, DateTime endDate);
    
    // Push notification operations
    
    /// <summary>
    /// Sends a push notification
    /// </summary>
    /// <param name="request">Push notification request data</param>
    /// <returns>Push notification delivery result</returns>
    Task<OperationResult<PushNotificationResult>> SendPushNotificationAsync(PushNotificationRequest request);
    
    /// <summary>
    /// Sends bulk push notifications
    /// </summary>
    /// <param name="request">Bulk push notification request data</param>
    /// <returns>Bulk push notification delivery results</returns>
    Task<OperationResult<BulkPushNotificationResult>> SendBulkPushNotificationAsync(BulkPushNotificationRequest request);
    
    /// <summary>
    /// Registers a device for push notifications
    /// </summary>
    /// <param name="request">Device registration data</param>
    /// <returns>Registration result</returns>
    Task<OperationResult<DeviceRegistrationResult>> RegisterDeviceAsync(DeviceRegistrationRequest request);
    
    /// <summary>
    /// Unregisters a device from push notifications
    /// </summary>
    /// <param name="deviceToken">Device token to unregister</param>
    /// <returns>Operation result</returns>
    Task<OperationResult> UnregisterDeviceAsync(string deviceToken);
    
    /// <summary>
    /// Gets push notification statistics
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Push notification statistics</returns>
    Task<OperationResult<PushNotificationStats>> GetPushNotificationStatsAsync(DateTime startDate, DateTime endDate);
    
    // Health monitoring
    
    /// <summary>
    /// Checks the health of all communication channels
    /// </summary>
    /// <returns>Health check results</returns>
    Task<OperationResult<List<HealthCheckResult>>> CheckCommunicationHealthAsync();
    
    /// <summary>
    /// Gets communication service status
    /// </summary>
    /// <returns>Service status information</returns>
    Task<OperationResult<CommunicationServiceStatusDto>> GetCommunicationStatusAsync();
}
