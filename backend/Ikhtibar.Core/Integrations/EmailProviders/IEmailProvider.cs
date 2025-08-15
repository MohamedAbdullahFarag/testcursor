using Ikhtibar.Core.DTOs;
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




