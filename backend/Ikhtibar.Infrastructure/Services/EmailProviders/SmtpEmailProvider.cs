using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Ikhtibar.Core.Integrations.EmailProviders;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.DTOs;
using EmailDeliveryStatusType = Ikhtibar.Core.Services.Providers.EmailDeliveryStatus;
using DeliveryEventType = Ikhtibar.Core.Services.Providers.DeliveryEvent;
using EmailStatusType = Ikhtibar.Core.Services.Providers.EmailStatus;

namespace Ikhtibar.Infrastructure.Services.EmailProviders;

/// <summary>
/// SMTP implementation of IEmailProvider.
/// Provides email delivery through SMTP servers as a fallback option.
/// Supports basic email sending with attachments.
/// </summary>
public class SmtpEmailProvider : IEmailProvider
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailProvider> _logger;

    public string ProviderName => "SMTP";

    public bool IsAvailable => !string.IsNullOrEmpty(_options.Host) && _options.Port > 0;

    public SmtpEmailProvider(
        IOptions<SmtpOptions> options,
        ILogger<SmtpEmailProvider> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<EmailDeliveryResult> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Sending email via SMTP to {ToEmail}", request.ToEmail);

        try
        {
            ValidateEmailRequest(request);

            using var smtpClient = CreateSmtpClient();
            using var mailMessage = CreateMailMessage(request);

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInformation("Email sent successfully via SMTP to {ToEmail}", request.ToEmail);

            return new EmailDeliveryResult
            {
                Success = true,
                MessageId = Guid.NewGuid().ToString(),
                SentAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email via SMTP to {ToEmail}", request.ToEmail);
            return new EmailDeliveryResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorCode = "SMTP_ERROR"
            };
        }
    }

    public async Task<EmailDeliveryResult> SendTemplateEmailAsync(TemplateEmailRequest request, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Sending template email via SMTP to {ToEmail}", request.ToEmail);

        try
        {
            // SMTP doesn't support templates natively, so we need to process them manually
            var processedRequest = ProcessTemplate(request);
            return await SendEmailAsync(processedRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send template email via SMTP to {ToEmail}", request.ToEmail);
            return new EmailDeliveryResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorCode = "SMTP_TEMPLATE_ERROR"
            };
        }
    }

    public async Task<BulkEmailDeliveryResult> SendBulkEmailAsync(IEnumerable<EmailRequest> requests, CancellationToken cancellationToken = default)
    {
        var requestList = requests.ToList();
        using var scope = _logger.BeginScope("Sending bulk email via SMTP to {Count} recipients", requestList.Count);

        var result = new BulkEmailDeliveryResult
        {
            TotalProcessed = requestList.Count
        };

        try
        {
            // SMTP doesn't support bulk sending, so we send individually
            foreach (var request in requestList)
            {
                var emailResult = await SendEmailAsync(request, cancellationToken);
                result.Results.Add(emailResult);

                if (emailResult.Success)
                    result.SuccessCount++;
                else
                    result.FailureCount++;

                // Add small delay to avoid overwhelming SMTP server
                if (_options.DelayBetweenEmails > 0)
                {
                    await Task.Delay(_options.DelayBetweenEmails, cancellationToken);
                }
            }

            _logger.LogInformation("Bulk email completed via SMTP: {SuccessCount} successful, {FailureCount} failed out of {TotalProcessed}",
                result.SuccessCount, result.FailureCount, result.TotalProcessed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk email operation failed via SMTP");
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public Task<EmailValidationResult> ValidateEmailAsync(string emailAddress, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Validating email address {EmailAddress}", emailAddress);

        try
        {
            // Basic format validation only for SMTP
            if (string.IsNullOrEmpty(emailAddress) || !IsValidEmailFormat(emailAddress))
            {
                return Task.FromResult(new EmailValidationResult
                {
                    IsValid = false,
                    Reason = "Invalid email format",
                    RiskScore = 100,
                    IsDeliverable = false
                });
            }

            return Task.FromResult(new EmailValidationResult
            {
                IsValid = true,
                Reason = "Valid format",
                RiskScore = 0,
                IsDeliverable = true // Can't verify deliverability with basic SMTP
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email validation failed for {EmailAddress}", emailAddress);
            return Task.FromResult(new EmailValidationResult
            {
                IsValid = false,
                Reason = ex.Message,
                RiskScore = 50,
                IsDeliverable = false
            });
        }
    }

    public Task<EmailDeliveryStatusType> GetDeliveryStatusAsync(string messageId, CancellationToken cancellationToken = default)
    {
        // SMTP doesn't provide delivery tracking
        return Task.FromResult(new EmailDeliveryStatusType
        {
            MessageId = messageId,
            Status = EmailStatusType.Sent, // Best we can do is "sent"
            LastUpdated = DateTime.UtcNow,
            Events = new List<DeliveryEventType>
            {
                new()
                {
                    EventType = "sent",
                    Timestamp = DateTime.UtcNow,
                    Data = new Dictionary<string, object> { { "messageId", messageId } }
                }
            }
        });
    }

    private SmtpClient CreateSmtpClient()
    {
        var smtpClient = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Timeout = _options.TimeoutMs
        };

        if (!string.IsNullOrEmpty(_options.Username) && !string.IsNullOrEmpty(_options.Password))
        {
            smtpClient.Credentials = new NetworkCredential(_options.Username, _options.Password);
        }

        return smtpClient;
    }

    private MailMessage CreateMailMessage(EmailRequest request)
    {
        var mailMessage = new MailMessage();

        // From address
        var fromEmail = request.FromEmail ?? _options.DefaultFromEmail;
        var fromName = request.FromName ?? _options.DefaultFromName;
        mailMessage.From = new MailAddress(fromEmail, fromName);

        // To address
        mailMessage.To.Add(new MailAddress(request.ToEmail, request.ToName ?? string.Empty));

        // Subject
        mailMessage.Subject = request.Subject;

        // Body content
        if (!string.IsNullOrEmpty(request.HtmlContent))
        {
            mailMessage.Body = request.HtmlContent;
            mailMessage.IsBodyHtml = true;

            // Add text alternative if available
            if (!string.IsNullOrEmpty(request.TextContent))
            {
                var textView = AlternateView.CreateAlternateViewFromString(request.TextContent, null, "text/plain");
                mailMessage.AlternateViews.Add(textView);
            }
        }
        else if (!string.IsNullOrEmpty(request.TextContent))
        {
            mailMessage.Body = request.TextContent;
            mailMessage.IsBodyHtml = false;
        }

        // Reply-to
        if (!string.IsNullOrEmpty(request.ReplyToEmail))
        {
            mailMessage.ReplyToList.Add(new MailAddress(request.ReplyToEmail));
        }

        // Priority
        mailMessage.Priority = request.Priority switch
        {
            EmailPriority.Low => MailPriority.Low,
            EmailPriority.High => MailPriority.High,
            _ => MailPriority.Normal
        };

        // Custom headers
        foreach (var header in request.Headers)
        {
            mailMessage.Headers.Add(header.Key, header.Value);
        }

        // Attachments
        foreach (var attachment in request.Attachments)
        {
            var contentBytes = Convert.FromBase64String(attachment.Content);
            var stream = new MemoryStream(contentBytes);
            var mailAttachment = new Attachment(stream, attachment.FileName, attachment.ContentType);
            
            if (!string.IsNullOrEmpty(attachment.ContentId))
            {
                mailAttachment.ContentId = attachment.ContentId;
            }

            if (attachment.Disposition == "inline" && mailAttachment.ContentDisposition != null)
            {
                mailAttachment.ContentDisposition.Inline = true;
            }

            mailMessage.Attachments.Add(mailAttachment);
        }

        return mailMessage;
    }

    private EmailRequest ProcessTemplate(TemplateEmailRequest request)
    {
        // Simple template processing - replace variables in subject and content
        var processedSubject = ProcessTemplateVariables(request.Subject, request.TemplateVariables);
        var processedTextContent = !string.IsNullOrEmpty(request.TextContent) 
            ? ProcessTemplateVariables(request.TextContent, request.TemplateVariables) 
            : null;
        var processedHtmlContent = !string.IsNullOrEmpty(request.HtmlContent) 
            ? ProcessTemplateVariables(request.HtmlContent, request.TemplateVariables) 
            : null;

        return new EmailRequest
        {
            ToEmail = request.ToEmail,
            ToName = request.ToName,
            FromEmail = request.FromEmail,
            FromName = request.FromName,
            Subject = processedSubject,
            TextContent = processedTextContent,
            HtmlContent = processedHtmlContent,
            Attachments = request.Attachments,
            ReplyToEmail = request.ReplyToEmail,
            Priority = request.Priority,
            Headers = request.Headers,
            Tags = request.Tags,
            Metadata = request.Metadata
        };
    }

    private static string ProcessTemplateVariables(string template, Dictionary<string, object> variables)
    {
        if (string.IsNullOrEmpty(template) || variables.Count == 0)
            return template;

        var result = template;
        foreach (var variable in variables)
        {
            var placeholder = $"{{{variable.Key}}}";
            result = result.Replace(placeholder, variable.Value?.ToString() ?? string.Empty);
        }

        return result;
    }

    private static void ValidateEmailRequest(EmailRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrEmpty(request.ToEmail))
            throw new ArgumentException("ToEmail is required", nameof(request));

        if (string.IsNullOrEmpty(request.Subject))
            throw new ArgumentException("Subject is required", nameof(request));

        if (string.IsNullOrEmpty(request.TextContent) && string.IsNullOrEmpty(request.HtmlContent))
            throw new ArgumentException("Either TextContent or HtmlContent is required", nameof(request));

        if (!IsValidEmailFormat(request.ToEmail))
            throw new ArgumentException("Invalid ToEmail format", nameof(request));
    }

    private static bool IsValidEmailFormat(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Configuration options for SMTP email provider.
/// </summary>
public class SmtpOptions
{
    public const string ConfigSection = "Smtp";

    /// <summary>
    /// SMTP server hostname.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port.
    /// </summary>
    public int Port { get; set; } = 587;

    /// <summary>
    /// Username for SMTP authentication.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password for SMTP authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Enable SSL/TLS encryption.
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Default from email address.
    /// </summary>
    public string DefaultFromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Default from name.
    /// </summary>
    public string DefaultFromName { get; set; } = string.Empty;

    /// <summary>
    /// Timeout in milliseconds for SMTP operations.
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Delay between emails in bulk sending (milliseconds).
    /// </summary>
    public int DelayBetweenEmails { get; set; } = 100;
}
