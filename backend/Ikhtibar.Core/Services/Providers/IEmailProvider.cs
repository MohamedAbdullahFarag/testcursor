namespace Ikhtibar.Core.Services.Providers;

/// <summary>
/// Email provider interface for sending email notifications through various providers
/// </summary>
public interface IEmailProvider
{
    /// <summary>
    /// Sends a single email notification
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject line</param>
    /// <param name="body">Email body content (HTML or plain text)</param>
    /// <param name="isHtml">Whether the body content is HTML formatted</param>
    /// <param name="from">Optional sender email address (uses default if not provided)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if email was sent successfully, false otherwise</returns>
    Task<bool> SendEmailAsync(
        string to, 
        string subject, 
        string body, 
        bool isHtml = true,
        string? from = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends email to multiple recipients
    /// </summary>
    /// <param name="recipients">List of recipient email addresses</param>
    /// <param name="subject">Email subject line</param>
    /// <param name="body">Email body content (HTML or plain text)</param>
    /// <param name="isHtml">Whether the body content is HTML formatted</param>
    /// <param name="from">Optional sender email address (uses default if not provided)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary with email addresses as keys and success status as values</returns>
    Task<Dictionary<string, bool>> SendBulkEmailAsync(
        IEnumerable<string> recipients,
        string subject, 
        string body, 
        bool isHtml = true,
        string? from = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends email using a template
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="templateId">Email template identifier</param>
    /// <param name="templateData">Data to populate template placeholders</param>
    /// <param name="from">Optional sender email address (uses default if not provided)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if email was sent successfully, false otherwise</returns>
    Task<bool> SendTemplatedEmailAsync(
        string to,
        string templateId,
        Dictionary<string, object> templateData,
        string? from = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that an email address format is correct
    /// </summary>
    /// <param name="email">Email address to validate</param>
    /// <returns>True if email format is valid, false otherwise</returns>
    bool IsValidEmailAddress(string email);

    /// <summary>
    /// Gets the provider configuration status
    /// </summary>
    /// <returns>True if provider is properly configured and ready to send emails</returns>
    Task<bool> IsConfiguredAsync();

    /// <summary>
    /// Gets delivery status for sent emails (if supported by provider)
    /// </summary>
    /// <param name="messageId">Message ID returned from send operation</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Delivery status information or null if not supported</returns>
    Task<EmailDeliveryStatus?> GetDeliveryStatusAsync(string messageId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Email delivery status information
/// </summary>
public class EmailDeliveryStatus
{
    public string MessageId { get; set; } = string.Empty;
    public EmailStatus Status { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? BouncedAt { get; set; }
    public DateTime? OpenedAt { get; set; }
    public DateTime? ClickedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    /// <summary>
    /// Last status update timestamp
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Delivery events timeline
    /// </summary>
    public List<DeliveryEvent> Events { get; set; } = new();
    
    /// <summary>
    /// Error details if delivery failed
    /// </summary>
    public string? ErrorDetails { get; set; }
}

/// <summary>
/// Individual delivery event in the timeline
/// </summary>
public class DeliveryEvent
{
    /// <summary>
    /// Event type (sent, delivered, opened, clicked, etc.)
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when event occurred
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Additional event data
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();
}

/// <summary>
/// Email delivery status enumeration
/// </summary>
public enum EmailStatus
{
    Pending = 0,
    Sent = 1,
    Delivered = 2,
    Opened = 3,
    Clicked = 4,
    Bounced = 5,
    Failed = 6,
    Unsubscribed = 7
}
