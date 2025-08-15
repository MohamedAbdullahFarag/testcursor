using Ikhtibar.Core.DTOs;

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




