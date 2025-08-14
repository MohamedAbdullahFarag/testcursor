using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Core.Services.Providers;

namespace Ikhtibar.Core.Integrations.EmailProviders;

/// <summary>
/// Interface for email providers in the notification system.
/// Provides abstraction layer for different email delivery services (SendGrid, SMTP, etc.).
/// Supports template-based emails, attachments, and delivery tracking.
/// </summary>
public interface IEmailProvider
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
    /// Sends a simple email with text or HTML content.
    /// </summary>
    /// <param name="request">Email request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Email delivery result</returns>
    Task<EmailDeliveryResult> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email using a predefined template with variable substitution.
    /// </summary>
    /// <param name="request">Template-based email request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Email delivery result</returns>
    Task<EmailDeliveryResult> SendTemplateEmailAsync(TemplateEmailRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends bulk emails efficiently using provider-specific batching.
    /// </summary>
    /// <param name="requests">Collection of email requests</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bulk delivery results</returns>
    Task<BulkEmailDeliveryResult> SendBulkEmailAsync(IEnumerable<EmailRequest> requests, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates an email address format and deliverability (if supported).
    /// </summary>
    /// <param name="emailAddress">Email address to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<EmailValidationResult> ValidateEmailAsync(string emailAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets delivery status for a previously sent email.
    /// </summary>
    /// <param name="messageId">Provider-specific message identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Delivery status</returns>
    Task<Services.Providers.EmailDeliveryStatus> GetDeliveryStatusAsync(string messageId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Request model for sending template-based emails.
/// </summary>
public class TemplateEmailRequest : EmailRequest
{
    /// <summary>
    /// Template identifier in the email provider.
    /// </summary>
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>
    /// Variables for template substitution.
    /// </summary>
    public Dictionary<string, object> TemplateVariables { get; set; } = new();

    /// <summary>
    /// Language/locale for template selection.
    /// </summary>
    public string Language { get; set; } = "en";
}

/// <summary>
/// Email attachment model.
/// </summary>
public class EmailAttachment
{
    /// <summary>
    /// File name for the attachment.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME content type.
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File content as byte array.
    /// </summary>
    public byte[] Content { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Content disposition (attachment or inline).
    /// </summary>
    public string Disposition { get; set; } = "attachment";

    /// <summary>
    /// Content ID for inline attachments (optional).
    /// </summary>
    public string? ContentId { get; set; }
}

/// <summary>
/// Result of email delivery operation.
/// </summary>
public class EmailDeliveryResult
{
    /// <summary>
    /// Indicates whether the email was successfully sent.
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
    /// Timestamp when the email was sent.
    /// </summary>
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Additional metadata from the provider.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Result of bulk email delivery operation.
/// </summary>
public class BulkEmailDeliveryResult
{
    /// <summary>
    /// Total number of emails processed.
    /// </summary>
    public int TotalProcessed { get; set; }

    /// <summary>
    /// Number of emails successfully sent.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of emails that failed to send.
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Individual results for each email.
    /// </summary>
    public List<EmailDeliveryResult> Results { get; set; } = new();

    /// <summary>
    /// Overall error message if the bulk operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Success rate as a percentage.
    /// </summary>
    public double SuccessRate => TotalProcessed > 0 ? (double)SuccessCount / TotalProcessed * 100 : 0;
}

/// <summary>
/// Individual delivery event in the timeline.
/// </summary>
public class DeliveryEvent
{
    /// <summary>
    /// Event type (sent, delivered, opened, clicked, etc.).
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when event occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Additional event data.
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();
}
