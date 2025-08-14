using System.ComponentModel.DataAnnotations;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Request DTO for sending bulk SMS messages
/// </summary>
public class BulkSmsRequest
{
    /// <summary>
    /// List of recipients with personalized data
    /// </summary>
    [Required]
    public List<BulkSmsRecipient> Recipients { get; set; } = new();

    /// <summary>
    /// SMS message content
    /// </summary>
    [Required]
    [StringLength(1600)] // Max SMS length with concatenation
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Template ID to use (optional)
    /// </summary>
    public Guid? TemplateId { get; set; }

    /// <summary>
    /// Global template variables
    /// </summary>
    public Dictionary<string, object> GlobalVariables { get; set; } = new();

    /// <summary>
    /// SMS sender ID or phone number
    /// </summary>
    public string? SenderId { get; set; }

    /// <summary>
    /// SMS priority
    /// </summary>
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Whether to send immediately or schedule
    /// </summary>
    public bool SendImmediately { get; set; } = true;

    /// <summary>
    /// Schedule time (if not sending immediately)
    /// </summary>
    public DateTime? ScheduledFor { get; set; }

    /// <summary>
    /// Maximum batch size for processing
    /// </summary>
    public int BatchSize { get; set; } = 50;

    /// <summary>
    /// Delay between batches (milliseconds)
    /// </summary>
    public int BatchDelayMs { get; set; } = 2000;

    /// <summary>
    /// SMS type (promotional, transactional, etc.)
    /// </summary>
    public SmsType Type { get; set; } = SmsType.Transactional;

    /// <summary>
    /// Tags for categorization
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Delivery options
    /// </summary>
    public SmsDeliveryOptions? DeliveryOptions { get; set; }

    /// <summary>
    /// Compliance settings
    /// </summary>
    public SmsComplianceSettings? ComplianceSettings { get; set; }
}

/// <summary>
/// SMS type enumeration
/// </summary>
public enum SmsType
{
    /// <summary>
    /// Transactional SMS (order confirmations, alerts, etc.)
    /// </summary>
    Transactional = 0,

    /// <summary>
    /// Promotional SMS (marketing messages)
    /// </summary>
    Promotional = 1,

    /// <summary>
    /// OTP/Verification SMS
    /// </summary>
    Verification = 2,

    /// <summary>
    /// Alert/Notification SMS
    /// </summary>
    Alert = 3,

    /// <summary>
    /// Reminder SMS
    /// </summary>
    Reminder = 4
}

/// <summary>
/// Bulk SMS recipient information
/// </summary>
public class BulkSmsRecipient
{
    /// <summary>
    /// Recipient phone number (E.164 format)
    /// </summary>
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Recipient name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// User ID (if known)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Personalized variables for this recipient
    /// </summary>
    public Dictionary<string, object> Variables { get; set; } = new();

    /// <summary>
    /// Custom metadata for this recipient
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Language preference
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Timezone for scheduling
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// Custom message override
    /// </summary>
    public string? CustomMessage { get; set; }

    /// <summary>
    /// Recipient country code
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Opt-in status
    /// </summary>
    public bool OptedIn { get; set; } = true;

    /// <summary>
    /// Opt-in timestamp
    /// </summary>
    public DateTime? OptedInAt { get; set; }
}

/// <summary>
/// SMS delivery options
/// </summary>
public class SmsDeliveryOptions
{
    /// <summary>
    /// Maximum retry attempts
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Retry delay in minutes
    /// </summary>
    public int RetryDelayMinutes { get; set; } = 5;

    /// <summary>
    /// Send timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Whether to use backup provider on failure
    /// </summary>
    public bool UseBackupProvider { get; set; } = true;

    /// <summary>
    /// Test mode (don't actually send)
    /// </summary>
    public bool TestMode { get; set; } = false;

    /// <summary>
    /// Delivery receipt tracking
    /// </summary>
    public bool TrackDelivery { get; set; } = true;

    /// <summary>
    /// Message validity period (hours)
    /// </summary>
    public int ValidityPeriodHours { get; set; } = 24;

    /// <summary>
    /// Flash SMS (immediate display)
    /// </summary>
    public bool FlashSms { get; set; } = false;

    /// <summary>
    /// Unicode encoding for special characters
    /// </summary>
    public bool UseUnicode { get; set; } = false;

    /// <summary>
    /// Concatenated message handling
    /// </summary>
    public bool AllowConcatenation { get; set; } = true;
}

/// <summary>
/// SMS compliance settings
/// </summary>
public class SmsComplianceSettings
{
    /// <summary>
    /// Require opt-in verification
    /// </summary>
    public bool RequireOptIn { get; set; } = true;

    /// <summary>
    /// Include unsubscribe instructions
    /// </summary>
    public bool IncludeUnsubscribe { get; set; } = true;

    /// <summary>
    /// Unsubscribe keyword
    /// </summary>
    public string UnsubscribeKeyword { get; set; } = "STOP";

    /// <summary>
    /// Respect quiet hours
    /// </summary>
    public bool RespectQuietHours { get; set; } = true;

    /// <summary>
    /// TCPA compliance mode (US)
    /// </summary>
    public bool TcpaCompliance { get; set; } = false;

    /// <summary>
    /// GDPR compliance mode (EU)
    /// </summary>
    public bool GdprCompliance { get; set; } = false;

    /// <summary>
    /// Custom compliance rules
    /// </summary>
    public List<string> CustomRules { get; set; } = new();
}

/// <summary>
/// Device registration request
/// </summary>
public class DeviceRegistrationRequest
{
    /// <summary>
    /// User ID owning the device
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Device platform (iOS, Android, Web, etc.)
    /// </summary>
    [Required]
    public DevicePlatform Platform { get; set; }

    /// <summary>
    /// Device token for push notifications
    /// </summary>
    [Required]
    public string DeviceToken { get; set; } = string.Empty;

    /// <summary>
    /// Device identifier (optional)
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Device name/model
    /// </summary>
    public string? DeviceName { get; set; }

    /// <summary>
    /// Operating system version
    /// </summary>
    public string? OsVersion { get; set; }

    /// <summary>
    /// Application version
    /// </summary>
    public string? AppVersion { get; set; }

    /// <summary>
    /// Device language preference
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Device timezone
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// Whether device allows push notifications
    /// </summary>
    public bool PushEnabled { get; set; } = true;

    /// <summary>
    /// Notification categories the device subscribes to
    /// </summary>
    public List<NotificationType> SubscribedTypes { get; set; } = new();

    /// <summary>
    /// Device metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Device registration source
    /// </summary>
    public string Source { get; set; } = "app";

    /// <summary>
    /// User agent string (for web devices)
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// IP address at registration
    /// </summary>
    public string? IpAddress { get; set; }
}

/// <summary>
/// Device platform enumeration
/// </summary>
public enum DevicePlatform
{
    /// <summary>
    /// iOS devices (iPhone, iPad)
    /// </summary>
    iOS = 0,

    /// <summary>
    /// Android devices
    /// </summary>
    Android = 1,

    /// <summary>
    /// Web browsers
    /// </summary>
    Web = 2,

    /// <summary>
    /// Windows devices
    /// </summary>
    Windows = 3,

    /// <summary>
    /// macOS devices
    /// </summary>
    macOS = 4,

    /// <summary>
    /// Unknown platform
    /// </summary>
    Unknown = 5
}

/// <summary>
/// Device registration result
/// </summary>
public class DeviceRegistrationResult
{
    /// <summary>
    /// Whether registration was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Device registration ID
    /// </summary>
    public Guid? DeviceId { get; set; }

    /// <summary>
    /// Registration token (if different from device token)
    /// </summary>
    public string? RegistrationToken { get; set; }

    /// <summary>
    /// Registration timestamp
    /// </summary>
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether this was a new registration or update
    /// </summary>
    public bool IsNewRegistration { get; set; }

    /// <summary>
    /// Previous registration info (if update)
    /// </summary>
    public DeviceRegistrationInfo? PreviousRegistration { get; set; }

    /// <summary>
    /// Error message (if registration failed)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Error code (if registration failed)
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Registration warnings
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Additional result metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Push notification capabilities detected
    /// </summary>
    public PushCapabilities? Capabilities { get; set; }

    /// <summary>
    /// Recommended notification types for this device
    /// </summary>
    public List<NotificationType> RecommendedTypes { get; set; } = new();
}

/// <summary>
/// Device registration information
/// </summary>
public class DeviceRegistrationInfo
{
    /// <summary>
    /// Registration ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Device platform
    /// </summary>
    public DevicePlatform Platform { get; set; }

    /// <summary>
    /// Device token
    /// </summary>
    public string DeviceToken { get; set; } = string.Empty;

    /// <summary>
    /// Device name
    /// </summary>
    public string? DeviceName { get; set; }

    /// <summary>
    /// Registration timestamp
    /// </summary>
    public DateTime RegisteredAt { get; set; }

    /// <summary>
    /// Last active timestamp
    /// </summary>
    public DateTime? LastActiveAt { get; set; }

    /// <summary>
    /// Whether device is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Push notification capabilities
/// </summary>
public class PushCapabilities
{
    /// <summary>
    /// Supports rich notifications
    /// </summary>
    public bool SupportsRichContent { get; set; }

    /// <summary>
    /// Supports action buttons
    /// </summary>
    public bool SupportsActions { get; set; }

    /// <summary>
    /// Supports images in notifications
    /// </summary>
    public bool SupportsImages { get; set; }

    /// <summary>
    /// Supports custom sounds
    /// </summary>
    public bool SupportsCustomSounds { get; set; }

    /// <summary>
    /// Supports badge updates
    /// </summary>
    public bool SupportsBadges { get; set; }

    /// <summary>
    /// Maximum payload size (bytes)
    /// </summary>
    public int MaxPayloadSize { get; set; }

    /// <summary>
    /// Supported notification categories
    /// </summary>
    public List<string> SupportedCategories { get; set; } = new();
}
