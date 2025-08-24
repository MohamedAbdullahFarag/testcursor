using System.ComponentModel.DataAnnotations;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for creating notification preferences for a user
/// </summary>
public class CreateNotificationPreferenceDto
{
    /// <summary>
    /// User ID for the preferences
    /// </summary>
    [Required]
    public int userId { get; set; }

    /// <summary>
    /// Type of notification
    /// </summary>
    [Required]
    [EnumDataType(typeof(NotificationType))]
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Delivery channel for this notification type
    /// </summary>
    [Required]
    [EnumDataType(typeof(NotificationChannel))]
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Whether notifications are enabled for this type and channel
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Start of quiet hours (local time)
    /// </summary>
    public TimeSpan? QuietHoursStart { get; set; }

    /// <summary>
    /// End of quiet hours (local time)
    /// </summary>
    public TimeSpan? QuietHoursEnd { get; set; }

    /// <summary>
    /// User's timezone identifier
    /// </summary>
    [StringLength(100)]
    public string? Timezone { get; set; }

    /// <summary>
    /// Frequency preference for this notification type
    /// </summary>
    [EnumDataType(typeof(NotificationFrequency))]
    public NotificationFrequency? Frequency { get; set; }

    /// <summary>
    /// Maximum number of notifications per day for this type
    /// </summary>
    [Range(0, 1000)]
    public int? MaxNotificationsPerDay { get; set; }

    /// <summary>
    /// Custom settings for this preference
    /// </summary>
    public Dictionary<string, object> Settings { get; set; } = new();
}

/// <summary>
/// DTO for updating notification preferences
/// </summary>
public class UpdateNotificationPreferenceDto
{
    /// <summary>
    /// Type of notification
    /// </summary>
    [Required]
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Whether user wants to receive email notifications for this type
    /// </summary>
    public bool? EmailEnabled { get; set; }

    /// <summary>
    /// Whether user wants to receive SMS notifications for this type
    /// </summary>
    public bool? SmsEnabled { get; set; }

    /// <summary>
    /// Whether user wants to receive in-app notifications for this type
    /// </summary>
    public bool? InAppEnabled { get; set; }

    /// <summary>
    /// Whether user wants to receive push notifications for this type
    /// </summary>
    public bool? PushEnabled { get; set; }
}

/// <summary>
/// DTO for notification preference response data
/// </summary>
public class NotificationPreferenceDto
{
    /// <summary>
    /// Unique identifier for the preference
    /// </summary>
    public int id { get; set; }

    /// <summary>
    /// User ID for the preferences
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Type of notification
    /// </summary>
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Whether user wants to receive email notifications for this type
    /// </summary>
    public bool EmailEnabled { get; set; }

    /// <summary>
    /// Whether user wants to receive SMS notifications for this type
    /// </summary>
    public bool SmsEnabled { get; set; }

    /// <summary>
    /// Whether user wants to receive in-app notifications for this type
    /// </summary>
    public bool InAppEnabled { get; set; }

    /// <summary>
    /// Whether user wants to receive push notifications for this type
    /// </summary>
    public bool PushEnabled { get; set; }

    /// <summary>
    /// Whether quiet hours are enabled for this user
    /// </summary>
    public bool QuietHoursEnabled { get; set; }

    /// <summary>
    /// Quiet hours start time (hour of day, 0-23)
    /// </summary>
    public int QuietHoursStart { get; set; }

    /// <summary>
    /// Quiet hours end time (hour of day, 0-23)
    /// </summary>
    public int QuietHoursEnd { get; set; }

    /// <summary>
    /// When the preference was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the preference was last modified
    /// </summary>
    public DateTime? ModifiedAt { get; set; }
}

/// <summary>
/// DTO for bulk updating multiple notification preferences
/// </summary>
public class BulkUpdatePreferencesDto
{
    /// <summary>
    /// User ID for the preferences
    /// </summary>
    [Required]
    public int userId { get; set; }

    /// <summary>
    /// List of preference updates to apply
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<PreferenceUpdateDto> Updates { get; set; } = new();
}

/// <summary>
/// DTO for individual preference update within bulk operation
/// </summary>
public class PreferenceUpdateDto
{
    /// <summary>
    /// Type of notification
    /// </summary>
    [Required]
    [EnumDataType(typeof(NotificationType))]
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Delivery channel for this notification type
    /// </summary>
    [Required]
    [EnumDataType(typeof(NotificationChannel))]
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Whether notifications are enabled for this type and channel
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Frequency preference for this notification type
    /// </summary>
    [EnumDataType(typeof(NotificationFrequency))]
    public NotificationFrequency? Frequency { get; set; }

    /// <summary>
    /// Maximum number of notifications per day for this type
    /// </summary>
    [Range(0, 1000)]
    public int? MaxNotificationsPerDay { get; set; }
}

/// <summary>
/// DTO for user preference summary
/// </summary>
public class UserPreferenceSummaryDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public int userId { get; set; }

    /// <summary>
    /// User's timezone identifier
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// Global quiet hours start
    /// </summary>
    public TimeSpan? GlobalQuietHoursStart { get; set; }

    /// <summary>
    /// Global quiet hours end
    /// </summary>
    public TimeSpan? GlobalQuietHoursEnd { get; set; }

    /// <summary>
    /// Total number of enabled notification types
    /// </summary>
    public int EnabledNotificationTypes { get; set; }

    /// <summary>
    /// Total number of disabled notification types
    /// </summary>
    public int DisabledNotificationTypes { get; set; }

    /// <summary>
    /// Preferred channels for notifications
    /// </summary>
    public List<NotificationChannel> PreferredChannels { get; set; } = new();

    /// <summary>
    /// Preferences grouped by notification type
    /// </summary>
    public Dictionary<NotificationType, List<ChannelPreferenceDto>> PreferencesByType { get; set; } = new();

    /// <summary>
    /// When preferences were last updated
    /// </summary>
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// DTO for channel-specific preference information
/// </summary>
public class ChannelPreferenceDto
{
    /// <summary>
    /// Delivery channel
    /// </summary>
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Whether this channel is enabled
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Frequency setting for this channel
    /// </summary>
    public NotificationFrequency? Frequency { get; set; }

    /// <summary>
    /// Maximum notifications per day for this channel
    /// </summary>
    public int? MaxPerDay { get; set; }
}

/// <summary>
/// DTO for checking if notifications should be sent based on preferences
/// </summary>
public class CheckPreferencesDto
{
    /// <summary>
    /// User ID to check preferences for
    /// </summary>
    [Required]
    public int userId { get; set; }

    /// <summary>
    /// Type of notification to check
    /// </summary>
    [Required]
    [EnumDataType(typeof(NotificationType))]
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Delivery channel to check
    /// </summary>
    [Required]
    [EnumDataType(typeof(NotificationChannel))]
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Time to check against (defaults to current time)
    /// </summary>
    public DateTime? CheckTime { get; set; }
}

/// <summary>
/// DTO for preference check result
/// </summary>
public class PreferenceCheckResultDto
{
    /// <summary>
    /// Whether notifications are allowed based on preferences
    /// </summary>
    public bool IsAllowed { get; set; }

    /// <summary>
    /// Reason why notifications are not allowed (if applicable)
    /// </summary>
    public string? ReasonDenied { get; set; }

    /// <summary>
    /// Suggested delay before trying again (if in quiet hours)
    /// </summary>
    public TimeSpan? SuggestedDelay { get; set; }

    /// <summary>
    /// User's current notification count for the day
    /// </summary>
    public int CurrentDayCount { get; set; }

    /// <summary>
    /// Maximum notifications allowed per day
    /// </summary>
    public int MaxPerDay { get; set; }

    /// <summary>
    /// Whether user is currently in quiet hours
    /// </summary>
    public bool IsInQuietHours { get; set; }

    /// <summary>
    /// Frequency setting that applies
    /// </summary>
    public NotificationFrequency? AppliedFrequency { get; set; }
}

/// <summary>
/// DTO for setting global notification preferences
/// </summary>
public class GlobalPreferencesDto
{
    /// <summary>
    /// User ID for the global preferences
    /// </summary>
    [Required]
    public int userId { get; set; }

    /// <summary>
    /// User's timezone identifier
    /// </summary>
    [StringLength(100)]
    public string? Timezone { get; set; }

    /// <summary>
    /// Global quiet hours start (applies to all notification types)
    /// </summary>
    public TimeSpan? GlobalQuietHoursStart { get; set; }

    /// <summary>
    /// Global quiet hours end (applies to all notification types)
    /// </summary>
    public TimeSpan? GlobalQuietHoursEnd { get; set; }

    /// <summary>
    /// Whether to enable all notification types by default
    /// </summary>
    public bool EnableAllByDefault { get; set; } = true;

    /// <summary>
    /// Default channels to use for new notification types
    /// </summary>
    public List<NotificationChannel> DefaultChannels { get; set; } = new();

    /// <summary>
    /// Global maximum notifications per day across all types
    /// </summary>
    [Range(0, 1000)]
    public int? GlobalMaxPerDay { get; set; }
}

/// <summary>
/// DTO for copying preferences from one user to another
/// </summary>
public class CopyPreferencesDto
{
    /// <summary>
    /// Source user ID to copy preferences from
    /// </summary>
    [Required]
    public int SourceUserId { get; set; }

    /// <summary>
    /// Target user ID to copy preferences to
    /// </summary>
    [Required]
    public int TargetUserId { get; set; }

    /// <summary>
    /// Whether to overwrite existing preferences for the target user
    /// </summary>
    public bool OverwriteExisting { get; set; } = false;

    /// <summary>
    /// Specific notification types to copy (if empty, copies all)
    /// </summary>
    public List<NotificationType> NotificationTypesToCopy { get; set; } = new();

    /// <summary>
    /// Specific channels to copy (if empty, copies all)
    /// </summary>
    public List<NotificationChannel> ChannelsToCopy { get; set; } = new();
}

/// <summary>
/// DTO for preference export request
/// </summary>
public class ExportPreferencesDto
{
    /// <summary>
    /// User IDs to export preferences for (if empty, exports all users)
    /// </summary>
    public List<int> UserIds { get; set; } = new();

    /// <summary>
    /// Export format
    /// </summary>
    [EnumDataType(typeof(ExportFormat))]
    public ExportFormat Format { get; set; } = ExportFormat.Json;

    /// <summary>
    /// Whether to include disabled preferences in export
    /// </summary>
    public bool IncludeDisabled { get; set; } = false;

    /// <summary>
    /// Whether to include settings data
    /// </summary>
    public bool IncludeSettings { get; set; } = true;
}

/// <summary>
/// DTO for preference analytics
/// </summary>
public class PreferenceAnalyticsDto
{
    /// <summary>
    /// Total number of users with preferences configured
    /// </summary>
    public int TotalUsersWithPreferences { get; set; }

    /// <summary>
    /// Breakdown by notification type
    /// </summary>
    public Dictionary<NotificationType, int> EnabledByType { get; set; } = new();

    /// <summary>
    /// Breakdown by delivery channel
    /// </summary>
    public Dictionary<NotificationChannel, int> EnabledByChannel { get; set; } = new();

    /// <summary>
    /// Most popular frequency settings
    /// </summary>
    public Dictionary<NotificationFrequency, int> FrequencyDistribution { get; set; } = new();

    /// <summary>
    /// Users with quiet hours configured
    /// </summary>
    public int UsersWithQuietHours { get; set; }

    /// <summary>
    /// Average notifications per day setting
    /// </summary>
    public double AverageMaxPerDay { get; set; }

    /// <summary>
    /// Most common timezone settings
    /// </summary>
    public Dictionary<string, int> TimezoneDistribution { get; set; } = new();

    /// <summary>
    /// Statistics generation date
    /// </summary>
    public DateTime GeneratedAt { get; set; }
}

/// <summary>
/// Export format enumeration
/// </summary>
public enum ExportFormat
{
    Json,
    Csv,
    Excel,
    Xml
}

/// <summary>
/// Notification frequency enumeration
/// </summary>
public enum NotificationFrequency
{
    Immediate,
    Hourly,
    Daily,
    Weekly,
    Never
}
