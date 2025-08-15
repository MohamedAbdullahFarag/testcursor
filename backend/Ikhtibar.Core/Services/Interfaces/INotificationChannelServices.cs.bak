using Ikhtibar.Core.Integrations.SmsProviders;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Email service interface for email notification delivery
/// Handles email-specific operations including templating and delivery tracking
/// Following SRP: ONLY email delivery operations
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a single email
    /// </summary>
    /// <param name="request">Email request data</param>
    /// <returns>Email delivery result</returns>
    Task<EmailResult> SendEmailAsync(EmailRequest request);

    /// <summary>
    /// Sends email using a template
    /// </summary>
    /// <param name="request">Templated email request data</param>
    /// <returns>Email delivery result</returns>
    Task<EmailResult> SendTemplatedEmailAsync(TemplatedEmailRequest request);

    /// <summary>
    /// Sends bulk emails (up to provider limits)
    /// </summary>
    /// <param name="requests">List of email requests</param>
    /// <returns>Bulk email delivery results</returns>
    Task<BulkEmailResult> SendBulkEmailAsync(List<EmailRequest> requests);

    /// <summary>
    /// Validates email address format and deliverability
    /// </summary>
    /// <param name="email">Email address to validate</param>
    /// <returns>True if email is valid and deliverable</returns>
    Task<bool> ValidateEmailAsync(string email);

    /// <summary>
    /// Gets email delivery status by external ID
    /// </summary>
    /// <param name="externalId">External provider message ID</param>
    /// <returns>Current delivery status</returns>
    Task<EmailDeliveryStatus> GetDeliveryStatusAsync(string externalId);

    /// <summary>
    /// Processes email webhooks from providers (bounce, open, click events)
    /// </summary>
    /// <param name="webhookData">Raw webhook data from provider</param>
    /// <param name="providerType">Email provider type</param>
    /// <returns>True if webhook was processed successfully</returns>
    Task<bool> ProcessWebhookAsync(string webhookData, EmailProviderType providerType);

    /// <summary>
    /// Gets email sending statistics
    /// </summary>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Email delivery statistics</returns>
    Task<EmailStats> GetEmailStatsAsync(DateTime fromDate, DateTime toDate);
}

/// <summary>
/// SMS service interface for SMS notification delivery
/// Handles SMS-specific operations including international delivery
/// Following SRP: ONLY SMS delivery operations
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Sends a single SMS
    /// </summary>
    /// <param name="request">SMS request data</param>
    /// <returns>SMS delivery result</returns>
    Task<SmsResult> SendSmsAsync(SmsRequest request);

    /// <summary>
    /// Sends SMS using a template
    /// </summary>
    /// <param name="request">Templated SMS request data</param>
    /// <returns>SMS delivery result</returns>
    Task<SmsResult> SendTemplatedSmsAsync(TemplatedSmsRequest request);

    /// <summary>
    /// Sends bulk SMS messages
    /// </summary>
    /// <param name="requests">List of SMS requests</param>
    /// <returns>Bulk SMS delivery results</returns>
    Task<BulkSmsResult> SendBulkSmsAsync(List<SmsRequest> requests);

    /// <summary>
    /// Validates phone number format and carrier availability
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate (international format)</param>
    /// <returns>Phone number validation result</returns>
    Task<PhoneValidationResult> ValidatePhoneAsync(string phoneNumber);

    /// <summary>
    /// Gets SMS delivery status by external ID
    /// </summary>
    /// <param name="externalId">External provider message SID</param>
    /// <returns>Current delivery status</returns>
    Task<SmsDeliveryStatus> GetDeliveryStatusAsync(string externalId);

    /// <summary>
    /// Processes SMS webhooks from providers (delivery, failure events)
    /// </summary>
    /// <param name="webhookData">Raw webhook data from provider</param>
    /// <param name="providerType">SMS provider type</param>
    /// <returns>True if webhook was processed successfully</returns>
    Task<bool> ProcessWebhookAsync(string webhookData, SmsProviderType providerType);

    /// <summary>
    /// Gets SMS sending statistics
    /// </summary>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>SMS delivery statistics</returns>
    Task<SmsStats> GetSmsStatsAsync(DateTime fromDate, DateTime toDate);
}

/// <summary>
/// Push notification service interface for mobile device notifications
/// Handles push notification delivery via Firebase and other providers
/// Following SRP: ONLY push notification operations
/// </summary>
public interface IPushNotificationService
{
    /// <summary>
    /// Sends a push notification to a single device
    /// </summary>
    /// <param name="request">Push notification request data</param>
    /// <returns>Push notification delivery result</returns>
    Task<PushNotificationResult> SendPushNotificationAsync(PushNotificationRequest request);

    /// <summary>
    /// Sends push notification to multiple devices
    /// </summary>
    /// <param name="request">Multi-device push notification request</param>
    /// <returns>Push notification delivery results</returns>
    Task<BulkPushNotificationResult> SendBulkPushNotificationAsync(BulkPushNotificationRequest request);

    /// <summary>
    /// Sends push notification to a topic/channel
    /// </summary>
    /// <param name="request">Topic-based push notification request</param>
    /// <returns>Push notification delivery result</returns>
    Task<PushNotificationResult> SendTopicNotificationAsync(TopicPushNotificationRequest request);

    /// <summary>
    /// Registers a device token for push notifications
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="deviceToken">Device registration token</param>
    /// <param name="platform">Device platform (iOS, Android)</param>
    /// <returns>True if successfully registered</returns>
    Task<bool> RegisterDeviceAsync(int userId, string deviceToken, DevicePlatform platform);

    /// <summary>
    /// Unregisters a device token
    /// </summary>
    /// <param name="deviceToken">Device token to unregister</param>
    /// <returns>True if successfully unregistered</returns>
    Task<bool> UnregisterDeviceAsync(string deviceToken);

    /// <summary>
    /// Gets active device tokens for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of active device tokens</returns>
    Task<List<string>> GetUserDeviceTokensAsync(int userId);

    /// <summary>
    /// Subscribes user devices to a topic
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="topic">Topic name to subscribe to</param>
    /// <returns>True if successfully subscribed</returns>
    Task<bool> SubscribeToTopicAsync(int userId, string topic);

    /// <summary>
    /// Gets push notification statistics
    /// </summary>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Push notification statistics</returns>
    Task<PushNotificationStats> GetPushStatsAsync(DateTime fromDate, DateTime toDate);
}

/// <summary>
/// Email provider types
/// </summary>
public enum EmailProviderType
{
    SendGrid,
    SMTP,
    AmazonSES,
    Mailgun
}

/// <summary>
/// SMS provider types
/// </summary>
public enum SmsProviderType
{
    Twilio,
    Nafath,
    AmazonSNS,
    Vonage
}

/// <summary>
/// Device platforms for push notifications
/// </summary>
public enum DevicePlatform
{
    iOS,
    Android,
    Web
}
