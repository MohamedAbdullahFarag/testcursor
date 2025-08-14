using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.Integrations.SmsProviders;

/// <summary>
/// Interface for SMS providers in the notification system.
/// Provides abstraction layer for different SMS delivery services (Twilio, AWS SNS, etc.).
/// Supports international messaging, delivery tracking, and message templates.
/// </summary>
public interface ISmsProvider
{
    /// <summary>
    /// Gets the provider name for logging and configuration.
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Indicates whether the provider is currently available and configured.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Sends a simple SMS message.
    /// </summary>
    /// <param name="request">SMS request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>SMS delivery result</returns>
    Task<SmsDeliveryResult> SendSmsAsync(SmsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends bulk SMS messages efficiently using provider-specific batching.
    /// </summary>
    /// <param name="requests">Collection of SMS requests</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bulk delivery results</returns>
    Task<BulkSmsDeliveryResult> SendBulkSmsAsync(IEnumerable<SmsRequest> requests, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a phone number format and carrier information (if supported).
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<PhoneValidationResult> ValidatePhoneAsync(string phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets delivery status for a previously sent SMS.
    /// </summary>
    /// <param name="messageId">Provider-specific message identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Delivery status</returns>
    Task<SmsDeliveryStatus> GetDeliveryStatusAsync(string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current account balance or credit information (if supported).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account balance information</returns>
    Task<SmsAccountBalance> GetAccountBalanceAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Request model for sending SMS messages.
/// </summary>
public class SmsRequest
{
    /// <summary>
    /// Recipient phone number in E.164 format (e.g., +1234567890).
    /// </summary>
    public string ToPhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Sender phone number or sender ID (optional, uses default if not specified).
    /// </summary>
    public string? FromPhoneNumber { get; set; }

    /// <summary>
    /// SMS message content.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Priority level for delivery.
    /// </summary>
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Whether to request delivery receipt (if supported).
    /// </summary>
    public bool RequestDeliveryReceipt { get; set; } = true;

    /// <summary>
    /// Message validity period in minutes (optional).
    /// </summary>
    public int? ValidityPeriodMinutes { get; set; }

    /// <summary>
    /// Tags for categorizing messages in provider dashboards.
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Metadata for tracking and analytics.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Scheduled delivery time (optional, for future sending).
    /// </summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// Message type (e.g., promotional, transactional).
    /// </summary>
    public SmsMessageType MessageType { get; set; } = SmsMessageType.Transactional;
}

/// <summary>
/// Result of SMS delivery operation.
/// </summary>
public class SmsDeliveryResult
{
    /// <summary>
    /// Indicates whether the SMS was successfully sent.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Provider-specific message identifier for tracking.
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Error message if delivery failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Error code if delivery failed.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Timestamp when the SMS was sent.
    /// </summary>
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Cost of sending the message (if available).
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Currency of the cost (if available).
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Number of message segments (for long messages).
    /// </summary>
    public int MessageSegments { get; set; } = 1;

    /// <summary>
    /// Additional metadata from the provider.
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
