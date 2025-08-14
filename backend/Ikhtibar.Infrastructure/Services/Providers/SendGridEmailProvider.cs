using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text.RegularExpressions;
using Ikhtibar.Core.Services.Providers;
using EmailDeliveryStatusCore = Ikhtibar.Core.Services.Providers.EmailDeliveryStatus;

namespace Ikhtibar.Infrastructure.Services.Providers;

/// <summary>
/// SendGrid implementation of email provider for sending email notifications
/// </summary>
public class SendGridEmailProvider : IEmailProvider
{
    private readonly ISendGridClient _sendGridClient;
    private readonly SendGridOptions _options;
    private readonly ILogger<SendGridEmailProvider> _logger;
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public SendGridEmailProvider(
        ISendGridClient sendGridClient,
        IOptions<SendGridOptions> options,
        ILogger<SendGridEmailProvider> logger)
    {
        _sendGridClient = sendGridClient ?? throw new ArgumentNullException(nameof(sendGridClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<bool> SendEmailAsync(
        string to, 
        string subject, 
        string body, 
        bool isHtml = true,
        string? from = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!IsValidEmailAddress(to))
            {
                _logger.LogWarning("Invalid email address provided: {Email}", to);
                return false;
            }

            var fromEmail = new EmailAddress(from ?? _options.FromEmail, _options.FromName);
            var toEmail = new EmailAddress(to);
            
            var msg = MailHelper.CreateSingleEmail(
                fromEmail, 
                toEmail, 
                subject, 
                isHtml ? null : body, 
                isHtml ? body : null);

            // Add tracking settings
            msg.SetClickTracking(_options.EnableClickTracking, _options.EnableClickTracking);
            msg.SetOpenTracking(_options.EnableOpenTracking);

            // Add custom headers for identification
            msg.AddHeader("X-Ikhtibar-Source", "NotificationSystem");
            msg.AddHeader("X-Ikhtibar-Timestamp", DateTime.UtcNow.ToString("O"));

            using var scope = _logger.BeginScope("Sending email to {To} with subject {Subject}", to, subject);
            
            var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully to {To}", to);
                return true;
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send email to {To}. StatusCode: {StatusCode}, Response: {Response}", 
                    to, response.StatusCode, responseBody);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending email to {To}", to);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, bool>> SendBulkEmailAsync(
        IEnumerable<string> recipients,
        string subject, 
        string body, 
        bool isHtml = true,
        string? from = null,
        CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, bool>();
        var validRecipients = recipients.Where(IsValidEmailAddress).ToList();
        
        if (!validRecipients.Any())
        {
            _logger.LogWarning("No valid email addresses provided for bulk send");
            return results;
        }

        try
        {
            var fromEmail = new EmailAddress(from ?? _options.FromEmail, _options.FromName);
            var tos = validRecipients.Select(email => new EmailAddress(email)).ToList();

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(
                fromEmail,
                tos,
                subject,
                isHtml ? null : body,
                isHtml ? body : null);

            // Add tracking settings
            msg.SetClickTracking(_options.EnableClickTracking, _options.EnableClickTracking);
            msg.SetOpenTracking(_options.EnableOpenTracking);

            // Add custom headers
            msg.AddHeader("X-Ikhtibar-Source", "NotificationSystem");
            msg.AddHeader("X-Ikhtibar-Timestamp", DateTime.UtcNow.ToString("O"));
            msg.AddHeader("X-Ikhtibar-BulkSend", "true");

            using var scope = _logger.BeginScope("Sending bulk email to {Count} recipients", validRecipients.Count);

            var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);

            var success = response.IsSuccessStatusCode;
            if (!success)
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send bulk email. StatusCode: {StatusCode}, Response: {Response}", 
                    response.StatusCode, responseBody);
            }
            else
            {
                _logger.LogInformation("Bulk email sent successfully to {Count} recipients", validRecipients.Count);
            }

            // Since SendGrid bulk send either succeeds or fails for all recipients,
            // we set the same result for all valid email addresses
            foreach (var email in validRecipients)
            {
                results[email] = success;
            }

            // Add failed results for invalid email addresses
            foreach (var email in recipients.Where(e => !IsValidEmailAddress(e)))
            {
                results[email] = false;
                _logger.LogWarning("Invalid email address in bulk send: {Email}", email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during bulk email send");
            
            // Mark all as failed
            foreach (var email in recipients)
            {
                results[email] = false;
            }
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<bool> SendTemplatedEmailAsync(
        string to,
        string templateId,
        Dictionary<string, object> templateData,
        string? from = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!IsValidEmailAddress(to))
            {
                _logger.LogWarning("Invalid email address provided for templated email: {Email}", to);
                return false;
            }

            var fromEmail = new EmailAddress(from ?? _options.FromEmail, _options.FromName);
            var toEmail = new EmailAddress(to);

            var msg = new SendGridMessage
            {
                From = fromEmail,
                TemplateId = templateId
            };

            msg.AddTo(toEmail);

            // Add template data
            if (templateData.Any())
            {
                msg.SetTemplateData(templateData);
            }

            // Add tracking settings
            msg.SetClickTracking(_options.EnableClickTracking, _options.EnableClickTracking);
            msg.SetOpenTracking(_options.EnableOpenTracking);

            // Add custom headers
            msg.AddHeader("X-Ikhtibar-Source", "NotificationSystem");
            msg.AddHeader("X-Ikhtibar-Template", templateId);
            msg.AddHeader("X-Ikhtibar-Timestamp", DateTime.UtcNow.ToString("O"));

            using var scope = _logger.BeginScope("Sending templated email to {To} using template {TemplateId}", to, templateId);

            var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Templated email sent successfully to {To} using template {TemplateId}", to, templateId);
                return true;
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send templated email to {To}. StatusCode: {StatusCode}, Response: {Response}", 
                    to, response.StatusCode, responseBody);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending templated email to {To}", to);
            return false;
        }
    }

    /// <inheritdoc />
    public bool IsValidEmailAddress(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }

    /// <inheritdoc />
    public async Task<bool> IsConfiguredAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                _logger.LogWarning("SendGrid API key is not configured");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_options.FromEmail))
            {
                _logger.LogWarning("SendGrid from email is not configured");
                return false;
            }

            // Test API key validity by making a simple API call
            var response = await _sendGridClient.RequestAsync(method: SendGridClient.Method.GET, urlPath: "user/profile");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking SendGrid configuration");
            return false;
        }
    }

    /// <inheritdoc />
    public Task<EmailDeliveryStatusCore?> GetDeliveryStatusAsync(string messageId, CancellationToken cancellationToken = default)
    {
        try
        {
            // SendGrid uses event webhooks for delivery status, not direct API queries
            // This would typically be implemented through webhook event processing
            // For now, we return null as this feature requires webhook setup
            _logger.LogInformation("Delivery status check requested for message {MessageId}, but webhook processing is required", messageId);
            return Task.FromResult<EmailDeliveryStatusCore?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving delivery status for message {MessageId}", messageId);
            return Task.FromResult<EmailDeliveryStatusCore?>(null);
        }
    }
}

/// <summary>
/// SendGrid provider configuration options
/// </summary>
public class SendGridOptions
{
    public const string SectionName = "SendGrid";

    /// <summary>
    /// SendGrid API key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Default sender email address
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Default sender name
    /// </summary>
    public string FromName { get; set; } = "Ikhtibar System";

    /// <summary>
    /// Enable click tracking for emails
    /// </summary>
    public bool EnableClickTracking { get; set; } = true;

    /// <summary>
    /// Enable open tracking for emails
    /// </summary>
    public bool EnableOpenTracking { get; set; } = true;

    /// <summary>
    /// Webhook endpoint URL for delivery events (optional)
    /// </summary>
    public string? WebhookUrl { get; set; }

    /// <summary>
    /// Webhook signing key for verifying webhook authenticity (optional)
    /// </summary>
    public string? WebhookSigningKey { get; set; }
}
