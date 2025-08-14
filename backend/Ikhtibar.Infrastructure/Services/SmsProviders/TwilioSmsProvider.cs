using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Ikhtibar.Core.Integrations.SmsProviders;
using Ikhtibar.Shared.Enums;
using System.Text.Json;

namespace Ikhtibar.Infrastructure.Services.SmsProviders;

/// <summary>
/// Twilio implementation of ISmsProvider.
/// Provides SMS delivery through Twilio's REST API.
/// Supports international messaging, delivery tracking, and phone validation.
/// </summary>
public class TwilioSmsProvider : ISmsProvider
{
    private readonly TwilioOptions _options;
    private readonly ILogger<TwilioSmsProvider> _logger;

    public string ProviderName => "Twilio";

    public bool IsAvailable => !string.IsNullOrEmpty(_options.AccountSid) && 
                              !string.IsNullOrEmpty(_options.AuthToken) && 
                              !string.IsNullOrEmpty(_options.FromPhoneNumber);

    public TwilioSmsProvider(
        IOptions<TwilioOptions> options,
        ILogger<TwilioSmsProvider> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (IsAvailable)
        {
            TwilioClient.Init(_options.AccountSid, _options.AuthToken);
        }
    }

    public async Task<SmsDeliveryResult> SendSmsAsync(SmsRequest request, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Sending SMS via Twilio to {ToPhoneNumber}", request.ToPhoneNumber);

        try
        {
            ValidateSmsRequest(request);

            var from = new PhoneNumber(request.FromPhoneNumber ?? _options.FromPhoneNumber);
            var to = new PhoneNumber(request.ToPhoneNumber);

            var messageOptions = new CreateMessageOptions(to)
            {
                From = from,
                Body = request.Message
            };

            // Set optional parameters
            if (request.ValidityPeriodMinutes.HasValue)
            {
                messageOptions.ValidityPeriod = request.ValidityPeriodMinutes.Value;
            }

            if (request.RequestDeliveryReceipt && !string.IsNullOrEmpty(_options.StatusCallbackUrl))
            {
                messageOptions.StatusCallback = new Uri(_options.StatusCallbackUrl);
            }

            if (request.ScheduledAt.HasValue)
            {
                messageOptions.SendAt = request.ScheduledAt.Value;
            }

            var message = await MessageResource.CreateAsync(messageOptions);

            _logger.LogInformation("SMS sent successfully via Twilio to {ToPhoneNumber} with message SID {MessageSid}", 
                request.ToPhoneNumber, message.Sid);

            return new SmsDeliveryResult
            {
                Success = true,
                MessageId = message.Sid,
                SentAt = DateTime.UtcNow,
                Cost = message.Price != null ? decimal.Parse(message.Price) : null,
                Currency = message.PriceUnit ?? "USD",
                MessageSegments = !string.IsNullOrEmpty(message.NumSegments) && int.TryParse(message.NumSegments, out var segments) ? segments : 1,
                Metadata = new Dictionary<string, object>
                {
                    { "twilioSid", message.Sid },
                    { "status", message.Status.ToString() }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS via Twilio to {ToPhoneNumber}", request.ToPhoneNumber);
            return new SmsDeliveryResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorCode = "TWILIO_ERROR"
            };
        }
    }

    public async Task<BulkSmsDeliveryResult> SendBulkSmsAsync(IEnumerable<SmsRequest> requests, CancellationToken cancellationToken = default)
    {
        var requestList = requests.ToList();
        using var scope = _logger.BeginScope("Sending bulk SMS via Twilio to {Count} recipients", requestList.Count);

        var result = new BulkSmsDeliveryResult
        {
            TotalProcessed = requestList.Count
        };

        try
        {
            // Process in parallel with rate limiting
            var semaphore = new SemaphoreSlim(_options.MaxConcurrentRequests, _options.MaxConcurrentRequests);
            var tasks = requestList.Select(async request =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    return await SendSmsAsync(request, cancellationToken);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var results = await Task.WhenAll(tasks);
            result.Results.AddRange(results);
            result.SuccessCount = results.Count(r => r.Success);
            result.FailureCount = results.Count(r => !r.Success);
            result.TotalCost = results.Where(r => r.Cost.HasValue).Sum(r => r.Cost!.Value);
            result.Currency = results.FirstOrDefault(r => !string.IsNullOrEmpty(r.Currency))?.Currency;

            _logger.LogInformation("Bulk SMS completed via Twilio: {SuccessCount} successful, {FailureCount} failed out of {TotalProcessed}",
                result.SuccessCount, result.FailureCount, result.TotalProcessed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk SMS operation failed via Twilio");
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public Task<PhoneValidationResult> ValidatePhoneAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Validating phone number {PhoneNumber}", phoneNumber);

        try
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return Task.FromResult(new PhoneValidationResult
                {
                    IsValid = false,
                    Reason = "Phone number is required"
                });
            }

            // Use Twilio Lookup API if available
            if (!string.IsNullOrEmpty(_options.LookupApiKey))
            {
                // Implementation for Twilio Lookup API would go here
                // For now, return basic validation
            }

            // Basic E.164 format validation
            var isValidFormat = IsValidPhoneFormat(phoneNumber);
            
            return Task.FromResult(new PhoneValidationResult
            {
                IsValid = isValidFormat,
                FormattedNumber = isValidFormat ? phoneNumber : null,
                Reason = isValidFormat ? "Valid format" : "Invalid phone number format",
                CanReceiveSms = isValidFormat
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Phone validation failed for {PhoneNumber}", phoneNumber);
            return Task.FromResult(new PhoneValidationResult
            {
                IsValid = false,
                Reason = ex.Message
            });
        }
    }

    public async Task<SmsDeliveryStatus> GetDeliveryStatusAsync(string messageId, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Getting delivery status for message {MessageId}", messageId);

        try
        {
            var message = await MessageResource.FetchAsync(messageId);

            var status = ConvertTwilioStatus(message.Status);
            var events = new List<SmsDeliveryEvent>
            {
                new()
                {
                    EventType = message.Status.ToString().ToLower(),
                    Timestamp = message.DateCreated ?? DateTime.UtcNow,
                    Description = $"Message status: {message.Status}",
                    Data = new Dictionary<string, object>
                    {
                        { "twilioSid", message.Sid },
                        { "errorCode", message.ErrorCode ?? 0 },
                        { "errorMessage", message.ErrorMessage ?? "" }
                    }
                }
            };

            return new SmsDeliveryStatus
            {
                Status = status,
                LastUpdated = message.DateUpdated ?? message.DateCreated ?? DateTime.UtcNow,
                Events = events,
                ErrorDetails = message.ErrorMessage,
                FinalCost = message.Price != null ? decimal.Parse(message.Price) : null,
                Currency = message.PriceUnit
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get delivery status for message {MessageId}", messageId);
            return new SmsDeliveryStatus
            {
                Status = NotificationDeliveryStatus.Failed,
                LastUpdated = DateTime.UtcNow,
                ErrorDetails = ex.Message
            };
        }
    }

    public async Task<SmsAccountBalance> GetAccountBalanceAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Getting Twilio account balance");

        try
        {
            var account = await Twilio.Rest.Api.V2010.AccountResource.FetchAsync(_options.AccountSid);

            return new SmsAccountBalance
            {
                Balance = 0, // Twilio doesn't expose balance directly via SDK
                Currency = "USD",
                AccountStatus = account.Status.ToString(),
                LastUpdated = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Twilio account balance");
            return new SmsAccountBalance
            {
                Balance = 0,
                Currency = "USD",
                AccountStatus = "unknown",
                LastUpdated = DateTime.UtcNow
            };
        }
    }

    private static NotificationDeliveryStatus ConvertTwilioStatus(MessageResource.StatusEnum twilioStatus)
    {
        if (twilioStatus == MessageResource.StatusEnum.Queued || twilioStatus == MessageResource.StatusEnum.Sending || 
            twilioStatus == MessageResource.StatusEnum.Accepted || twilioStatus == MessageResource.StatusEnum.Scheduled)
        {
            return NotificationDeliveryStatus.Pending;
        }
        else if (twilioStatus == MessageResource.StatusEnum.Sent)
        {
            return NotificationDeliveryStatus.Sent;
        }
        else if (twilioStatus == MessageResource.StatusEnum.Delivered || twilioStatus == MessageResource.StatusEnum.Received ||
                 twilioStatus == MessageResource.StatusEnum.PartiallyDelivered)
        {
            return NotificationDeliveryStatus.Delivered;
        }
        else if (twilioStatus == MessageResource.StatusEnum.Failed || twilioStatus == MessageResource.StatusEnum.Undelivered)
        {
            return NotificationDeliveryStatus.Failed;
        }
        else if (twilioStatus == MessageResource.StatusEnum.Read)
        {
            return NotificationDeliveryStatus.Read;
        }
        else
        {
            return NotificationDeliveryStatus.Unknown;
        }
    }

    private static void ValidateSmsRequest(SmsRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrEmpty(request.ToPhoneNumber))
            throw new ArgumentException("ToPhoneNumber is required", nameof(request));

        if (string.IsNullOrEmpty(request.Message))
            throw new ArgumentException("Message is required", nameof(request));

        if (!IsValidPhoneFormat(request.ToPhoneNumber))
            throw new ArgumentException("Invalid ToPhoneNumber format", nameof(request));

        if (request.Message.Length > 1600) // Twilio's limit for concatenated SMS
            throw new ArgumentException("Message too long", nameof(request));
    }

    private static bool IsValidPhoneFormat(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return false;

        // Basic E.164 format validation: +[country code][number]
        if (!phoneNumber.StartsWith("+"))
            return false;

        var digits = phoneNumber.Substring(1);
        return digits.All(char.IsDigit) && digits.Length >= 7 && digits.Length <= 15;
    }
}

/// <summary>
/// Configuration options for Twilio SMS provider.
/// </summary>
public class TwilioOptions
{
    public const string ConfigSection = "Twilio";

    /// <summary>
    /// Twilio Account SID.
    /// </summary>
    public string AccountSid { get; set; } = string.Empty;

    /// <summary>
    /// Twilio Auth Token.
    /// </summary>
    public string AuthToken { get; set; } = string.Empty;

    /// <summary>
    /// Default from phone number (Twilio phone number).
    /// </summary>
    public string FromPhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Twilio Lookup API key for phone validation (optional).
    /// </summary>
    public string? LookupApiKey { get; set; }

    /// <summary>
    /// Webhook URL for delivery status callbacks.
    /// </summary>
    public string? StatusCallbackUrl { get; set; }

    /// <summary>
    /// Maximum concurrent requests for bulk operations.
    /// </summary>
    public int MaxConcurrentRequests { get; set; } = 10;

    /// <summary>
    /// Enable delivery receipts by default.
    /// </summary>
    public bool EnableDeliveryReceipts { get; set; } = true;
}
