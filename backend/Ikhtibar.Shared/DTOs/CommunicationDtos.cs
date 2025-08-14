using System.ComponentModel.DataAnnotations;
using Ikhtibar.Shared.Enums;


namespace Ikhtibar.Shared.DTOs;

#region Email DTOs

/// <summary>
/// DTO for email sending request
/// </summary>
public class EmailRequest
{
    /// <summary>
    /// Recipient email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Sender email address (optional, uses configured default if not provided)
    /// </summary>
    [EmailAddress]
    public string? From { get; set; }

    /// <summary>
    /// Email subject
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Plain text email content
    /// </summary>
    [StringLength(10000)]
    public string? TextContent { get; set; }

    /// <summary>
    /// HTML email content
    /// </summary>
    [StringLength(50000)]
    public string? HtmlContent { get; set; }

    /// <summary>
    /// Reply-to email address
    /// </summary>
    [EmailAddress]
    public string? ReplyTo { get; set; }

    /// <summary>
    /// CC email addresses
    /// </summary>
    public List<string> Cc { get; set; } = new();

    /// <summary>
    /// BCC email addresses
    /// </summary>
    public List<string> Bcc { get; set; } = new();

    /// <summary>
    /// Email attachments
    /// </summary>
    public List<EmailAttachment> Attachments { get; set; } = new();

    /// <summary>
    /// Custom headers
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Email priority
    /// </summary>
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;

    /// <summary>
    /// Track email opens
    /// </summary>
    public bool TrackOpens { get; set; } = true;

    /// <summary>
    /// Track email clicks
    /// </summary>
    public bool TrackClicks { get; set; } = true;

    /// <summary>
    /// Custom metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    // Alias properties for backward compatibility
    /// <summary>
    /// Alias for To property
    /// </summary>
    public string ToEmail 
    { 
        get => To; 
        set => To = value; 
    }

    /// <summary>
    /// Alias for From property
    /// </summary>
    public string? FromEmail 
    { 
        get => From; 
        set => From = value; 
    }

    /// <summary>
    /// Recipient display name
    /// </summary>
    public string? ToName { get; set; }

    /// <summary>
    /// Sender display name
    /// </summary>
    public string? FromName { get; set; }

    /// <summary>
    /// Alias for ReplyTo property
    /// </summary>
    public string? ReplyToEmail 
    { 
        get => ReplyTo; 
        set => ReplyTo = value; 
    }

    /// <summary>
    /// Email tags for categorization
    /// </summary>
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// DTO for templated email request
/// </summary>
public class TemplatedEmailRequest
{
    /// <summary>
    /// Recipient email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Template ID to use
    /// </summary>
    [Required]
    public Guid TemplateId { get; set; }

    /// <summary>
    /// Template variables for substitution
    /// </summary>
    public Dictionary<string, object> TemplateData { get; set; } = new();

    /// <summary>
    /// Sender email address (optional)
    /// </summary>
    [EmailAddress]
    public string? From { get; set; }

    /// <summary>
    /// Reply-to email address
    /// </summary>
    [EmailAddress]
    public string? ReplyTo { get; set; }

    /// <summary>
    /// CC email addresses
    /// </summary>
    public List<string> Cc { get; set; } = new();

    /// <summary>
    /// BCC email addresses
    /// </summary>
    public List<string> Bcc { get; set; } = new();

    /// <summary>
    /// Email attachments
    /// </summary>
    public List<EmailAttachment> Attachments { get; set; } = new();

    /// <summary>
    /// Email priority
    /// </summary>
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;

    /// <summary>
    /// Track email opens
    /// </summary>
    public bool TrackOpens { get; set; } = true;

    /// <summary>
    /// Track email clicks
    /// </summary>
    public bool TrackClicks { get; set; } = true;

    /// <summary>
    /// Custom metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    // Alias properties for backward compatibility
    /// <summary>
    /// Alias for To property
    /// </summary>
    public string ToEmail 
    { 
        get => To; 
        set => To = value; 
    }

    /// <summary>
    /// Alias for From property
    /// </summary>
    public string? FromEmail 
    { 
        get => From; 
        set => From = value; 
    }

    /// <summary>
    /// Recipient display name
    /// </summary>
    public string? ToName { get; set; }

    /// <summary>
    /// Sender display name
    /// </summary>
    public string? FromName { get; set; }

    /// <summary>
    /// Alias for ReplyTo property
    /// </summary>
    public string? ReplyToEmail 
    { 
        get => ReplyTo; 
        set => ReplyTo = value; 
    }

    /// <summary>
    /// Email tags for categorization
    /// </summary>
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// DTO for email attachment
/// </summary>
public class EmailAttachment
{
    /// <summary>
    /// Attachment filename
    /// </summary>
    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File content as base64 string
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// MIME type of the attachment
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Content disposition (attachment or inline)
    /// </summary>
    public string Disposition { get; set; } = "attachment";

    /// <summary>
    /// Content ID for inline attachments
    /// </summary>
    public string? ContentId { get; set; }
}

/// <summary>
/// DTO for email delivery result
/// </summary>
public class EmailResult
{
    /// <summary>
    /// Whether the email was successfully sent
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// External message ID from provider
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Error message if sending failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Provider used for sending
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Cost of sending (if available)
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Delivery status
    /// </summary>
    public EmailDeliveryStatus Status { get; set; }

    /// <summary>
    /// When the email was sent
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Additional metadata from provider
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for bulk email result
/// </summary>
public class BulkEmailResult
{
    /// <summary>
    /// Number of emails successfully sent
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of emails that failed to send
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Individual email results
    /// </summary>
    public List<EmailResult> Results { get; set; } = new();

    /// <summary>
    /// Total cost of bulk sending
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Bulk operation ID
    /// </summary>
    public string? BulkId { get; set; }

    /// <summary>
    /// When the bulk operation was executed
    /// </summary>
    public DateTime ExecutedAt { get; set; }
}

/// <summary>
/// DTO for email statistics
/// </summary>
public class EmailStats
{
    /// <summary>
    /// Total emails sent
    /// </summary>
    public int TotalSent { get; set; }

    /// <summary>
    /// Total emails delivered
    /// </summary>
    public int TotalDelivered { get; set; }

    /// <summary>
    /// Total emails opened
    /// </summary>
    public int TotalOpened { get; set; }

    /// <summary>
    /// Total emails clicked
    /// </summary>
    public int TotalClicked { get; set; }

    /// <summary>
    /// Total emails bounced
    /// </summary>
    public int TotalBounced { get; set; }

    /// <summary>
    /// Total emails that resulted in complaints
    /// </summary>
    public int TotalComplaints { get; set; }

    /// <summary>
    /// Total emails that resulted in unsubscribes
    /// </summary>
    public int TotalUnsubscribes { get; set; }

    /// <summary>
    /// Delivery rate percentage
    /// </summary>
    public double DeliveryRate { get; set; }

    /// <summary>
    /// Open rate percentage
    /// </summary>
    public double OpenRate { get; set; }

    /// <summary>
    /// Click rate percentage
    /// </summary>
    public double ClickRate { get; set; }

    /// <summary>
    /// Bounce rate percentage
    /// </summary>
    public double BounceRate { get; set; }

    /// <summary>
    /// Statistics period start
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Statistics period end
    /// </summary>
    public DateTime PeriodEnd { get; set; }
}

#endregion

#region SMS DTOs

/// <summary>
/// DTO for templated SMS request
/// </summary>
public class TemplatedSmsRequest
{
    /// <summary>
    /// Recipient phone number (E.164 format)
    /// </summary>
    [Required]
    [Phone]
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Template ID to use
    /// </summary>
    [Required]
    public Guid TemplateId { get; set; }

    /// <summary>
    /// Template variables for substitution
    /// </summary>
    public Dictionary<string, object> TemplateData { get; set; } = new();

    /// <summary>
    /// Sender ID (optional)
    /// </summary>
    [StringLength(11)]
    public string? From { get; set; }

    /// <summary>
    /// Message validity period in minutes
    /// </summary>
    [Range(1, 4320)]
    public int? ValidityPeriod { get; set; }

    /// <summary>
    /// Scheduled delivery time
    /// </summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// Custom metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for SMS delivery result
/// </summary>
public class SmsResult
{
    /// <summary>
    /// Whether the SMS was successfully sent
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// External message ID from provider
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Error message if sending failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Provider used for sending
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Cost of sending
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Number of SMS segments used
    /// </summary>
    public int SegmentCount { get; set; } = 1;

    /// <summary>
    /// Delivery status
    /// </summary>
    public SmsDeliveryStatus Status { get; set; } = new();

    /// <summary>
    /// When the SMS was sent
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Additional metadata from provider
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// SMS delivery status information.
/// </summary>
public class SmsDeliveryStatus
{
    /// <summary>
    /// Current delivery status.
    /// </summary>
    public NotificationDeliveryStatus Status { get; set; } = NotificationDeliveryStatus.Unknown;

    /// <summary>
    /// Timestamp when status was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Delivery events timeline.
    /// </summary>
    public List<SmsDeliveryEvent> Events { get; set; } = new();

    /// <summary>
    /// Error details if delivery failed.
    /// </summary>
    public string? ErrorDetails { get; set; }

    /// <summary>
    /// Final cost of the message (if available).
    /// </summary>
    public decimal? FinalCost { get; set; }

    /// <summary>
    /// Currency of the cost.
    /// </summary>
    public string? Currency { get; set; }
}
/// <summary>
/// Individual SMS delivery event in the timeline.
/// </summary>
public class SmsDeliveryEvent
{
    /// <summary>
    /// Event type (sent, delivered, failed, etc.).
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when event occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Event description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Additional event data.
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();
}

/// <summary>
/// DTO for bulk SMS result
/// </summary>
public class BulkSmsResult
{
    /// <summary>
    /// Number of SMS successfully sent
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of SMS that failed to send
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Individual SMS results
    /// </summary>
    public List<SmsResult> Results { get; set; } = new();

    /// <summary>
    /// Total cost of bulk sending
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Total segments used
    /// </summary>
    public int TotalSegments { get; set; }

    /// <summary>
    /// Bulk operation ID
    /// </summary>
    public string? BulkId { get; set; }

    /// <summary>
    /// When the bulk operation was executed
    /// </summary>
    public DateTime ExecutedAt { get; set; }
}

/// <summary>
/// DTO for SMS statistics
/// </summary>
public class SmsStats
{
    /// <summary>
    /// Total SMS sent
    /// </summary>
    public int TotalSent { get; set; }

    /// <summary>
    /// Total SMS delivered
    /// </summary>
    public int TotalDelivered { get; set; }

    /// <summary>
    /// Total SMS failed
    /// </summary>
    public int TotalFailed { get; set; }

    /// <summary>
    /// Total segments used
    /// </summary>
    public int TotalSegments { get; set; }

    /// <summary>
    /// Total cost
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Delivery rate percentage
    /// </summary>
    public double DeliveryRate { get; set; }

    /// <summary>
    /// Average cost per SMS
    /// </summary>
    public decimal AverageCost { get; set; }

    /// <summary>
    /// Statistics period start
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Statistics period end
    /// </summary>
    public DateTime PeriodEnd { get; set; }
}

#endregion

#region Push Notification DTOs

/// <summary>
/// DTO for push notification request
/// </summary>
public class PushNotificationRequest
{
    /// <summary>
    /// Device token or registration ID
    /// </summary>
    [Required]
    public string DeviceToken { get; set; } = string.Empty;

    /// <summary>
    /// Notification title
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Notification body
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Platform (iOS, Android, Web)
    /// </summary>
    [Required]
    public PushPlatform Platform { get; set; }

    /// <summary>
    /// Notification icon
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Notification image
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// Click action URL
    /// </summary>
    public string? ClickAction { get; set; }

    /// <summary>
    /// Notification badge count
    /// </summary>
    public int? Badge { get; set; }

    /// <summary>
    /// Notification sound
    /// </summary>
    public string? Sound { get; set; }

    /// <summary>
    /// Time to live in seconds
    /// </summary>
    [Range(0, 2419200)] // Up to 28 days
    public int? TimeToLive { get; set; }

    /// <summary>
    /// Notification priority
    /// </summary>
    public PushPriority Priority { get; set; } = PushPriority.Normal;

    /// <summary>
    /// Custom data payload
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();

    /// <summary>
    /// Scheduled delivery time
    /// </summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// Custom metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for topic-based push notification request
/// </summary>
public class TopicPushNotificationRequest
{
    /// <summary>
    /// Topic name to send to
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Topic { get; set; } = string.Empty;

    /// <summary>
    /// Notification title
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Notification body
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Notification icon
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Notification image
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// Click action URL
    /// </summary>
    public string? ClickAction { get; set; }

    /// <summary>
    /// Time to live in seconds
    /// </summary>
    [Range(0, 2419200)]
    public int? TimeToLive { get; set; }

    /// <summary>
    /// Notification priority
    /// </summary>
    public PushPriority Priority { get; set; } = PushPriority.Normal;

    /// <summary>
    /// Custom data payload
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();

    /// <summary>
    /// Platform-specific configuration
    /// </summary>
    public Dictionary<PushPlatform, object> PlatformConfig { get; set; } = new();

    /// <summary>
    /// Scheduled delivery time
    /// </summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// Custom metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for bulk push notification request
/// </summary>
public class BulkPushNotificationRequest
{
    /// <summary>
    /// List of device tokens
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<string> DeviceTokens { get; set; } = new();

    /// <summary>
    /// Notification title
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Notification body
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Target platform
    /// </summary>
    [Required]
    public PushPlatform Platform { get; set; }

    /// <summary>
    /// Notification icon
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Notification image
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// Click action URL
    /// </summary>
    public string? ClickAction { get; set; }

    /// <summary>
    /// Time to live in seconds
    /// </summary>
    [Range(0, 2419200)]
    public int? TimeToLive { get; set; }

    /// <summary>
    /// Notification priority
    /// </summary>
    public PushPriority Priority { get; set; } = PushPriority.Normal;

    /// <summary>
    /// Custom data payload
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();

    /// <summary>
    /// Scheduled delivery time
    /// </summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// Custom metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for push notification delivery result
/// </summary>
public class PushNotificationResult
{
    /// <summary>
    /// Whether the notification was successfully sent
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// External message ID from provider
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Error message if sending failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Provider used for sending
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Device token used
    /// </summary>
    public string? DeviceToken { get; set; }

    /// <summary>
    /// Platform
    /// </summary>
    public PushPlatform Platform { get; set; }

    /// <summary>
    /// When the notification was sent
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Additional metadata from provider
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for bulk push notification result
/// </summary>
public class BulkPushNotificationResult
{
    /// <summary>
    /// Number of notifications successfully sent
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of notifications that failed to send
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Individual notification results
    /// </summary>
    public List<PushNotificationResult> Results { get; set; } = new();

    /// <summary>
    /// Bulk operation ID
    /// </summary>
    public string? BulkId { get; set; }

    /// <summary>
    /// When the bulk operation was executed
    /// </summary>
    public DateTime ExecutedAt { get; set; }
}

/// <summary>
/// DTO for push notification statistics
/// </summary>
public class PushNotificationStats
{
    /// <summary>
    /// Total notifications sent
    /// </summary>
    public int TotalSent { get; set; }

    /// <summary>
    /// Total notifications delivered
    /// </summary>
    public int TotalDelivered { get; set; }

    /// <summary>
    /// Total notifications failed
    /// </summary>
    public int TotalFailed { get; set; }

    /// <summary>
    /// Total notifications clicked
    /// </summary>
    public int TotalClicked { get; set; }

    /// <summary>
    /// Delivery rate percentage
    /// </summary>
    public double DeliveryRate { get; set; }

    /// <summary>
    /// Click rate percentage
    /// </summary>
    public double ClickRate { get; set; }

    /// <summary>
    /// Statistics by platform
    /// </summary>
    public Dictionary<PushPlatform, int> StatsByPlatform { get; set; } = new();

    /// <summary>
    /// Statistics period start
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Statistics period end
    /// </summary>
    public DateTime PeriodEnd { get; set; }
}

#endregion

#region Enums

/// <summary>
/// Email priority levels
/// </summary>
public enum EmailPriority
{
    Low,
    Normal,
    High
}

/// <summary>
/// Email delivery status
/// </summary>
public enum EmailDeliveryStatus
{
    Sent,
    Delivered,
    Opened,
    Clicked,
    Bounced,
    Complained,
    Unsubscribed,
    Failed
}

/// <summary>
/// Push notification platforms
/// </summary>
public enum PushPlatform
{
    iOS,
    Android,
    Web
}

/// <summary>
/// Push notification priority levels
/// </summary>
public enum PushPriority
{
    Low,
    Normal,
    High
}

#endregion
