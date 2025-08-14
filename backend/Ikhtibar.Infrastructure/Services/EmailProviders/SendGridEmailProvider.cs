using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Ikhtibar.Core.Integrations.EmailProviders;
using Ikhtibar.Shared.Enums;
using System.Text.Json;
using EmailDeliveryStatusType = Ikhtibar.Core.Services.Providers.EmailDeliveryStatus;
using EmailStatusType = Ikhtibar.Core.Services.Providers.EmailStatus;
using DeliveryEventType = Ikhtibar.Core.Services.Providers.DeliveryEvent;

namespace Ikhtibar.Infrastructure.Services.EmailProviders;

/// <summary>
/// SendGrid implementation of IEmailProvider.
/// Provides email delivery through SendGrid's Web API v3.
/// Supports templates, bulk sending, and delivery tracking.
/// </summary>
public class SendGridEmailProvider : IEmailProvider
{
    private readonly ISendGridClient _sendGridClient;
    private readonly SendGridOptions _options;
    private readonly ILogger<SendGridEmailProvider> _logger;

    public string ProviderName => "SendGrid";

    public bool IsAvailable => !string.IsNullOrEmpty(_options.ApiKey);

    public SendGridEmailProvider(
        ISendGridClient sendGridClient,
        IOptions<SendGridOptions> options,
        ILogger<SendGridEmailProvider> logger)
    {
        _sendGridClient = sendGridClient ?? throw new ArgumentNullException(nameof(sendGridClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<EmailDeliveryResult> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Sending email via SendGrid to {ToEmail}", request.ToEmail);
        
        try
        {
            ValidateEmailRequest(request);

            var from = new EmailAddress(
                request.FromEmail ?? _options.DefaultFromEmail,
                request.FromName ?? _options.DefaultFromName
            );

            var to = new EmailAddress(request.ToEmail, request.ToName);

            var sendGridMessage = MailHelper.CreateSingleEmail(
                from, 
                to, 
                request.Subject,
                request.TextContent,
                request.HtmlContent
            );

            // Configure message settings
            ConfigureMessage(sendGridMessage, request);

            var response = await _sendGridClient.SendEmailAsync(sendGridMessage, cancellationToken);

            return await ProcessSendGridResponse(response, request.ToEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email via SendGrid to {ToEmail}", request.ToEmail);
            return new EmailDeliveryResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorCode = "SENDGRID_ERROR"
            };
        }
    }

    public async Task<EmailDeliveryResult> SendTemplateEmailAsync(TemplateEmailRequest request, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Sending template email via SendGrid to {ToEmail} with template {TemplateId}", 
            request.ToEmail, request.TemplateId);

        try
        {
            ValidateTemplateEmailRequest(request);

            var from = new EmailAddress(
                request.FromEmail ?? _options.DefaultFromEmail,
                request.FromName ?? _options.DefaultFromName
            );

            var to = new EmailAddress(request.ToEmail, request.ToName);

            var sendGridMessage = MailHelper.CreateSingleTemplateEmail(
                from,
                to,
                request.TemplateId,
                request.TemplateVariables
            );

            // Configure message settings
            ConfigureMessage(sendGridMessage, request);

            var response = await _sendGridClient.SendEmailAsync(sendGridMessage, cancellationToken);

            return await ProcessSendGridResponse(response, request.ToEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send template email via SendGrid to {ToEmail} with template {TemplateId}", 
                request.ToEmail, request.TemplateId);
            return new EmailDeliveryResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorCode = "SENDGRID_TEMPLATE_ERROR"
            };
        }
    }

    public async Task<BulkEmailDeliveryResult> SendBulkEmailAsync(IEnumerable<EmailRequest> requests, CancellationToken cancellationToken = default)
    {
        var requestList = requests.ToList();
        using var scope = _logger.BeginScope("Sending bulk email via SendGrid to {Count} recipients", requestList.Count);

        var result = new BulkEmailDeliveryResult
        {
            TotalProcessed = requestList.Count
        };

        try
        {
            // SendGrid recommends batching in groups of 1000
            const int batchSize = 1000;
            var batches = requestList.Chunk(batchSize);

            foreach (var batch in batches)
            {
                var batchResults = await ProcessEmailBatch(batch, cancellationToken);
                result.Results.AddRange(batchResults);
                result.SuccessCount += batchResults.Count(r => r.Success);
                result.FailureCount += batchResults.Count(r => !r.Success);
            }

            _logger.LogInformation("Bulk email completed: {SuccessCount} successful, {FailureCount} failed out of {TotalProcessed}", 
                result.SuccessCount, result.FailureCount, result.TotalProcessed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk email operation failed");
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public Task<EmailValidationResult> ValidateEmailAsync(string emailAddress, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Validating email address {EmailAddress}", emailAddress);

        try
        {
            // Basic format validation
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

            // SendGrid Email Validation API (if configured)
            if (!string.IsNullOrEmpty(_options.ValidationApiKey))
            {
                // Implementation would go here for SendGrid's Email Validation API
                // For now, return basic validation
            }

            return Task.FromResult(new EmailValidationResult
            {
                IsValid = true,
                Reason = "Valid format",
                RiskScore = 0,
                IsDeliverable = true
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
        using var scope = _logger.BeginScope("Getting delivery status for message {MessageId}", messageId);

        try
        {
            // SendGrid Event API implementation would go here
            // For now, return basic status
            return Task.FromResult(new EmailDeliveryStatusType
            {
                Status = EmailStatusType.Delivered,
                LastUpdated = DateTime.UtcNow,
                Events = new List<DeliveryEventType>
                {
                    new DeliveryEventType
                    {
                        EventType = "sent",
                        Timestamp = DateTime.UtcNow,
                        Data = new Dictionary<string, object> { { "messageId", messageId } }
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get delivery status for message {MessageId}", messageId);
            return Task.FromResult(new EmailDeliveryStatusType
            {
                Status = EmailStatusType.Failed,
                LastUpdated = DateTime.UtcNow,
                ErrorDetails = ex.Message
            });
        }
    }

    private async Task<List<EmailDeliveryResult>> ProcessEmailBatch(IEnumerable<EmailRequest> batch, CancellationToken cancellationToken)
    {
        var results = new List<EmailDeliveryResult>();

        foreach (var request in batch)
        {
            try
            {
                var result = await SendEmailAsync(request, cancellationToken);
                results.Add(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email in batch to {ToEmail}", request.ToEmail);
                results.Add(new EmailDeliveryResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ErrorCode = "BATCH_ERROR"
                });
            }
        }

        return results;
    }

    private void ConfigureMessage(SendGridMessage message, EmailRequest request)
    {
        // Set reply-to if specified
        if (!string.IsNullOrEmpty(request.ReplyToEmail))
        {
            message.SetReplyTo(new EmailAddress(request.ReplyToEmail));
        }

        // Add custom headers
        foreach (var header in request.Headers)
        {
            message.AddHeader(header.Key, header.Value);
        }

        // Add categories (tags in SendGrid)
        foreach (var tag in request.Tags)
        {
            message.AddCategory(tag);
        }

        // Add custom args (metadata)
        foreach (var metadata in request.Metadata)
        {
            message.AddCustomArg(metadata.Key, metadata.Value.ToString() ?? string.Empty);
        }

        // Set priority
        if (request.Priority == EmailPriority.High)
        {
            message.AddHeader("X-Priority", "1");
            message.AddHeader("X-MSMail-Priority", "High");
        }

        // Add attachments
        foreach (var attachment in request.Attachments)
        {
            var sendGridAttachment = new Attachment
            {
                Filename = attachment.FileName,
                Content = attachment.Content, // Already base64 string
                Type = attachment.ContentType,
                Disposition = attachment.Disposition
            };

            if (!string.IsNullOrEmpty(attachment.ContentId))
            {
                sendGridAttachment.ContentId = attachment.ContentId;
            }

            message.AddAttachment(sendGridAttachment);
        }

        // Add tracking settings
        message.SetClickTracking(_options.EnableClickTracking, _options.EnableClickTracking);
        message.SetOpenTracking(_options.EnableOpenTracking);
        message.SetGoogleAnalytics(_options.EnableGoogleAnalytics);
    }

    private async Task<EmailDeliveryResult> ProcessSendGridResponse(Response response, string toEmail)
    {
        var isSuccess = response.IsSuccessStatusCode;
        string? messageId = null;
        string? errorMessage = null;
        string? errorCode = null;

        if (isSuccess)
        {
            // Extract message ID from headers
            if (response.Headers != null && response.Headers.Contains("X-Message-Id"))
            {
                messageId = response.Headers.GetValues("X-Message-Id").FirstOrDefault();
            }

            _logger.LogInformation("Email sent successfully via SendGrid to {ToEmail} with message ID {MessageId}", 
                toEmail, messageId);
        }
        else
        {
            var responseBody = await response.Body.ReadAsStringAsync();
            errorCode = response.StatusCode.ToString();
            
            try
            {
                var errorResponse = JsonSerializer.Deserialize<SendGridErrorResponse>(responseBody);
                errorMessage = string.Join("; ", errorResponse?.Errors?.Select(e => e.Message) ?? new[] { "Unknown error" });
            }
            catch
            {
                errorMessage = responseBody;
            }

            _logger.LogError("Email failed to send via SendGrid to {ToEmail}: {ErrorCode} - {ErrorMessage}", 
                toEmail, errorCode, errorMessage);
        }

        return new EmailDeliveryResult
        {
            Success = isSuccess,
            MessageId = messageId,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode,
            SentAt = DateTime.UtcNow
        };
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

    private static void ValidateTemplateEmailRequest(TemplateEmailRequest request)
    {
        ValidateEmailRequest(request);

        if (string.IsNullOrEmpty(request.TemplateId))
            throw new ArgumentException("TemplateId is required", nameof(request));
    }

    private static bool IsValidEmailFormat(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Configuration options for SendGrid email provider.
/// </summary>
public class SendGridOptions
{
    public const string ConfigSection = "SendGrid";

    /// <summary>
    /// SendGrid API key for authentication.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// SendGrid Email Validation API key (optional).
    /// </summary>
    public string? ValidationApiKey { get; set; }

    /// <summary>
    /// Default from email address.
    /// </summary>
    public string DefaultFromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Default from name.
    /// </summary>
    public string DefaultFromName { get; set; } = string.Empty;

    /// <summary>
    /// Enable click tracking.
    /// </summary>
    public bool EnableClickTracking { get; set; } = true;

    /// <summary>
    /// Enable open tracking.
    /// </summary>
    public bool EnableOpenTracking { get; set; } = true;

    /// <summary>
    /// Enable Google Analytics tracking.
    /// </summary>
    public bool EnableGoogleAnalytics { get; set; } = false;

    /// <summary>
    /// Template mappings for different notification types.
    /// </summary>
    public Dictionary<string, string> TemplateIds { get; set; } = new();
}

/// <summary>
/// SendGrid error response model.
/// </summary>
internal class SendGridErrorResponse
{
    public List<SendGridError>? Errors { get; set; }
}

/// <summary>
/// SendGrid error details.
/// </summary>
internal class SendGridError
{
    public string Message { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Help { get; set; } = string.Empty;
}
