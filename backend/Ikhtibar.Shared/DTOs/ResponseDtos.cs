using System.ComponentModel.DataAnnotations;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Response DTO for notification data
/// </summary>
public class NotificationResponseDto
{
    /// <summary>
    /// Notification unique identifier
    /// </summary>
    public int id { get; set; }

    /// <summary>
    /// Notification type
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Notification title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Notification content/body
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Target user ID (null for broadcast notifications)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Template ID used (if any)
    /// </summary>
    public int? TemplateId { get; set; }

    /// <summary>
    /// Template name (if template was used)
    /// </summary>
    public string? TemplateName { get; set; }

    /// <summary>
    /// Notification priority level
    /// </summary>
    public NotificationPriority Priority { get; set; }

    /// <summary>
    /// Current notification status
    /// </summary>
    public NotificationStatus Status { get; set; }

    /// <summary>
    /// When notification is scheduled to be sent (if scheduled)
    /// </summary>
    public DateTime? ScheduledFor { get; set; }

    /// <summary>
    /// When notification was actually sent
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Template variables used
    /// </summary>
    public Dictionary<string, object> Variables { get; set; } = new();

    /// <summary>
    /// Channels used for delivery
    /// </summary>
    public List<NotificationChannel> Channels { get; set; } = new();

    /// <summary>
    /// Delivery attempts and results
    /// </summary>
    public List<DeliveryAttemptDto> DeliveryAttempts { get; set; } = new();

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// When notification was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When notification was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// User who created the notification
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// User who last modified the notification
    /// </summary>
    public int ModifiedBy { get; set; }

    /// <summary>
    /// Whether notification is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Language code for localization
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Read/opened status
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// When notification was read (if applicable)
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Click/interaction status
    /// </summary>
    public bool IsClicked { get; set; }

    /// <summary>
    /// When notification was clicked (if applicable)
    /// </summary>
    public DateTime? ClickedAt { get; set; }

    /// <summary>
    /// Overall delivery success status
    /// </summary>
    public bool IsDelivered { get; set; }

    /// <summary>
    /// Number of delivery retries
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Total cost for all delivery attempts
    /// </summary>
    public decimal TotalCost { get; set; }
}

/// <summary>
/// Response DTO for notification template data
/// </summary>
public class NotificationTemplateResponseDto
{
    /// <summary>
    /// Template unique identifier
    /// </summary>
    public int id { get; set; }

    /// <summary>
    /// Template name/key
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Template display name
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Template description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Notification type this template is for
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Template subject/title
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Template body content
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Content type (text/html)
    /// </summary>
    public string ContentType { get; set; } = "text/plain";

    /// <summary>
    /// Template language
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Template variables and their descriptions
    /// </summary>
    public Dictionary<string, string> Variables { get; set; } = new();

    /// <summary>
    /// Whether template is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Template version
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Parent template ID (for versioning)
    /// </summary>
    public int? ParentTemplateId { get; set; }

    /// <summary>
    /// Usage statistics
    /// </summary>
    public TemplateUsageDto? UsageStats { get; set; }

    /// <summary>
    /// When template was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When template was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// User who created the template
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// User who last modified the template
    /// </summary>
    public int ModifiedBy { get; set; }

    /// <summary>
    /// Tags for categorization
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Template category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Last validation result
    /// </summary>
    public TemplateValidationSummaryDto? LastValidation { get; set; }
}

/// <summary>
/// Response DTO for notification preference data
/// </summary>
public class NotificationPreferenceResponseDto
{
    /// <summary>
    /// Preference unique identifier
    /// </summary>
    public int id { get; set; }

    /// <summary>
    /// User ID this preference belongs to
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Notification type
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Whether notifications of this type are enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Email delivery preference
    /// </summary>
    public bool EmailEnabled { get; set; } = true;

    /// <summary>
    /// SMS delivery preference
    /// </summary>
    public bool SmsEnabled { get; set; } = false;

    /// <summary>
    /// Push notification preference
    /// </summary>
    public bool PushEnabled { get; set; } = true;

    /// <summary>
    /// In-app notification preference
    /// </summary>
    public bool InAppEnabled { get; set; } = true;

    /// <summary>
    /// Delivery frequency preference
    /// </summary>
    public NotificationFrequency Frequency { get; set; } = NotificationFrequency.Immediate;

    /// <summary>
    /// Quiet hours start time (local time)
    /// </summary>
    public string? QuietHoursStart { get; set; }

    /// <summary>
    /// Quiet hours end time (local time)
    /// </summary>
    public string? QuietHoursEnd { get; set; }

    /// <summary>
    /// User's timezone
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// When preference was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When preference was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// Additional preference metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Days of week when quiet hours apply
    /// </summary>
    public List<DayOfWeek> QuietHoursDays { get; set; } = new();

    /// <summary>
    /// Whether quiet hours are enabled
    /// </summary>
    public bool QuietHoursEnabled { get; set; }

    /// <summary>
    /// Custom delivery rules
    /// </summary>
    public List<DeliveryRuleDto> CustomRules { get; set; } = new();

    /// <summary>
    /// Preference source (user/admin/default)
    /// </summary>
    public string Source { get; set; } = "user";

    /// <summary>
    /// Whether this preference can be modified by user
    /// </summary>
    public bool IsUserModifiable { get; set; } = true;

    /// <summary>
    /// Priority threshold for this notification type
    /// </summary>
    public NotificationPriority MinimumPriority { get; set; } = NotificationPriority.Low;
}

// Supporting DTOs for response objects

/// <summary>
/// DTO for delivery attempt information
/// </summary>
public class DeliveryAttemptDto
{
    /// <summary>
    /// Attempt ID
    /// </summary>
    public int id { get; set; }

    /// <summary>
    /// Delivery channel used
    /// </summary>
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Delivery status
    /// </summary>
    public NotificationDeliveryStatus Status { get; set; }

    /// <summary>
    /// Attempt timestamp
    /// </summary>
    public DateTime AttemptedAt { get; set; }

    /// <summary>
    /// Delivery timestamp (if successful)
    /// </summary>
    public DateTime? DeliveredAt { get; set; }

    /// <summary>
    /// Error message (if failed)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// External provider message ID
    /// </summary>
    public string? ExternalMessageId { get; set; }

    /// <summary>
    /// Delivery cost
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Response time in milliseconds
    /// </summary>
    public double? ResponseTimeMs { get; set; }

    /// <summary>
    /// Recipient information
    /// </summary>
    public string? Recipient { get; set; }

    /// <summary>
    /// Provider response metadata
    /// </summary>
    public Dictionary<string, object> ProviderResponse { get; set; } = new();
}

/// <summary>
/// DTO for template usage information
/// </summary>
public class TemplateUsageDto
{
    /// <summary>
    /// Total times used
    /// </summary>
    public int TotalUsage { get; set; }

    /// <summary>
    /// Usage in last 30 days
    /// </summary>
    public int Usage30Days { get; set; }

    /// <summary>
    /// Usage in last 7 days
    /// </summary>
    public int Usage7Days { get; set; }

    /// <summary>
    /// Last used timestamp
    /// </summary>
    public DateTime? LastUsed { get; set; }

    /// <summary>
    /// Success rate percentage
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Average processing time
    /// </summary>
    public double AverageProcessingTimeMs { get; set; }
}

/// <summary>
/// DTO for template validation summary
/// </summary>
public class TemplateValidationSummaryDto
{
    /// <summary>
    /// Whether template is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Number of errors found
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Number of warnings found
    /// </summary>
    public int WarningCount { get; set; }

    /// <summary>
    /// Quality score (0-100)
    /// </summary>
    public int QualityScore { get; set; }

    /// <summary>
    /// Last validation timestamp
    /// </summary>
    public DateTime LastValidated { get; set; }

    /// <summary>
    /// Variables found in template
    /// </summary>
    public List<string> Variables { get; set; } = new();
}

/// <summary>
/// DTO for custom delivery rules
/// </summary>
public class DeliveryRuleDto
{
    /// <summary>
    /// Rule ID
    /// </summary>
    public int id { get; set; }

    /// <summary>
    /// Rule name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Rule condition (JSON)
    /// </summary>
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// Rule action (JSON)
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Rule priority
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Whether rule is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Rule description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Created timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
