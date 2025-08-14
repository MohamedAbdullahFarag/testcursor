namespace Ikhtibar.Core.Services.Providers;

/// <summary>
/// SMS provider interface for sending SMS notifications through various providers
/// </summary>
public interface ISmsProvider
{
    /// <summary>
    /// Sends a single SMS notification
    /// </summary>
    /// <param name="to">Recipient phone number in international format (e.g., +1234567890)</param>
    /// <param name="message">SMS message content (max 160 characters for single SMS)</param>
    /// <param name="from">Optional sender phone number or sender ID</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>SMS delivery result with message ID and status</returns>
    Task<SmsDeliveryResult> SendSmsAsync(
        string to, 
        string message, 
        string? from = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends SMS to multiple recipients
    /// </summary>
    /// <param name="recipients">List of recipient phone numbers in international format</param>
    /// <param name="message">SMS message content</param>
    /// <param name="from">Optional sender phone number or sender ID</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary with phone numbers as keys and delivery results as values</returns>
    Task<Dictionary<string, SmsDeliveryResult>> SendBulkSmsAsync(
        IEnumerable<string> recipients,
        string message, 
        string? from = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends SMS using a template
    /// </summary>
    /// <param name="to">Recipient phone number in international format</param>
    /// <param name="templateId">SMS template identifier</param>
    /// <param name="templateData">Data to populate template placeholders</param>
    /// <param name="from">Optional sender phone number or sender ID</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>SMS delivery result with message ID and status</returns>
    Task<SmsDeliveryResult> SendTemplatedSmsAsync(
        string to,
        string templateId,
        Dictionary<string, object> templateData,
        string? from = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that a phone number format is correct
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate (should be in international format)</param>
    /// <returns>True if phone number format is valid, false otherwise</returns>
    bool IsValidPhoneNumber(string phoneNumber);

    /// <summary>
    /// Formats a phone number to international format
    /// </summary>
    /// <param name="phoneNumber">Phone number to format</param>
    /// <param name="defaultCountryCode">Default country code if not provided in number</param>
    /// <returns>Formatted phone number or null if invalid</returns>
    string? FormatPhoneNumber(string phoneNumber, string defaultCountryCode = "+1");

    /// <summary>
    /// Gets the provider configuration status
    /// </summary>
    /// <returns>True if provider is properly configured and ready to send SMS</returns>
    Task<bool> IsConfiguredAsync();

    /// <summary>
    /// Gets delivery status for sent SMS messages (if supported by provider)
    /// </summary>
    /// <param name="messageId">Message ID returned from send operation</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Delivery status information or null if not supported</returns>
    Task<SmsDeliveryStatus?> GetDeliveryStatusAsync(string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets estimated cost for sending SMS to specific destination
    /// </summary>
    /// <param name="to">Destination phone number</param>
    /// <param name="messageLength">Length of message content</param>
    /// <returns>Estimated cost in USD or null if not available</returns>
    Task<decimal?> GetEstimatedCostAsync(string to, int messageLength);
}

/// <summary>
/// SMS delivery result information
/// </summary>
public class SmsDeliveryResult
{
    public bool IsSuccess { get; set; }
    public string? MessageId { get; set; }
    public SmsStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public decimal? Cost { get; set; }
    public int SegmentCount { get; set; } = 1;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// SMS delivery status information
/// </summary>
public class SmsDeliveryStatus
{
    public string MessageId { get; set; } = string.Empty;
    public SmsStatus Status { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CarrierName { get; set; }
    public decimal? Cost { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// SMS delivery status enumeration
/// </summary>
public enum SmsStatus
{
    Pending = 0,
    Queued = 1,
    Sent = 2,
    Delivered = 3,
    Failed = 4,
    Undelivered = 5,
    Unknown = 6
}
