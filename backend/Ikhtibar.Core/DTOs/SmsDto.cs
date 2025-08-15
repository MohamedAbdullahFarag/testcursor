using System.ComponentModel.DataAnnotations;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.DTOs;

/// <summary>
/// Phone validation result DTO
/// </summary>
public class PhoneValidationResult
{
    /// <summary>
    /// Indicates whether the phone number is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// The formatted phone number in E.164 format
    /// </summary>
    public string? FormattedPhoneNumber { get; set; }

    /// <summary>
    /// The formatted phone number (alias for FormattedPhoneNumber)
    /// </summary>
    public string? FormattedNumber { get; set; }

    /// <summary>
    /// The carrier information if available
    /// </summary>
    public string? Carrier { get; set; }

    /// <summary>
    /// Any error message if validation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Reason for validation result
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Whether this phone number can receive SMS
    /// </summary>
    public bool CanReceiveSms { get; set; } = true;
}

/// <summary>
/// SMS request DTO
/// </summary>
public class SmsRequest
{
    /// <summary>
    /// Recipient phone number (E.164 format)
    /// </summary>
    [Required]
    [Phone]
    public string ToPhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Sender phone number (optional, uses configured default if not provided)
    /// </summary>
    [Phone]
    public string? FromPhoneNumber { get; set; }

    /// <summary>
    /// SMS message content
    /// </summary>
    [Required]
    [StringLength(1600)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Message validity period in minutes
    /// </summary>
    [Range(1, 4320)]
    public int? ValidityPeriodMinutes { get; set; }

    /// <summary>
    /// Scheduled delivery time
    /// </summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// Request delivery receipt
    /// </summary>
    public bool RequestDeliveryReceipt { get; set; } = true;

    /// <summary>
    /// Custom metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Templated SMS request DTO
/// </summary>
public class TemplatedSmsRequest : SmsRequest
{
    /// <summary>
    /// Template ID to use
    /// </summary>
    [Required]
    public Guid TemplateId { get; set; }

    /// <summary>
    /// Template variables for substitution
    /// </summary>
    public Dictionary<string, object> TemplateVariables { get; set; } = new();
}

/// <summary>
/// SMS result DTO
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
}

/// <summary>
/// SMS delivery result DTO
/// </summary>
public class SmsDeliveryResult
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
    /// Error code if sending failed
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Provider used for sending
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Cost of sending
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Currency of the cost
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Number of SMS segments used
    /// </summary>
    public int MessageSegments { get; set; } = 1;

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
/// Result of bulk SMS delivery operation.
/// </summary>
public class BulkSmsDeliveryResult
{
    /// <summary>
    /// Total number of SMS messages processed.
    /// </summary>
    public int TotalProcessed { get; set; }

    /// <summary>
    /// Number of messages successfully sent.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of messages that failed to send.
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Individual results for each SMS.
    /// </summary>
    public List<SmsDeliveryResult> Results { get; set; } = new();

    /// <summary>
    /// Overall error message if the bulk operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Total cost of the bulk operation (if available).
    /// </summary>
    public decimal? TotalCost { get; set; }

    /// <summary>
    /// Currency of the cost (if available).
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Success rate as a percentage.
    /// </summary>
    public double SuccessRate => TotalProcessed > 0 ? (double)SuccessCount / TotalProcessed * 100 : 0;
}

/// <summary>
/// SMS account balance information.
/// </summary>
public class SmsAccountBalance
{
    /// <summary>
    /// Current account balance.
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Currency of the balance.
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Account status (active, suspended, etc.).
    /// </summary>
    public string AccountStatus { get; set; } = string.Empty;

    /// <summary>
    /// Estimated number of SMS messages that can be sent with current balance.
    /// </summary>
    public int? EstimatedSmsCount { get; set; }

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
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
/// SMS message types for categorization and routing.
/// </summary>
public enum SmsMessageType
{
    /// <summary>
    /// Transactional messages (OTP, alerts, confirmations).
    /// </summary>
    Transactional = 1,

    /// <summary>
    /// Promotional messages (marketing, announcements).
    /// </summary>
    Promotional = 2,

    /// <summary>
    /// Informational messages (updates, notifications).
    /// </summary>
    Informational = 3
}
