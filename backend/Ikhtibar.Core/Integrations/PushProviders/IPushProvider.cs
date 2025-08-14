using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.Integrations.PushProviders;

/// <summary>
/// Interface for push notification providers in the notification system.
/// Provides abstraction layer for different push notification services (Firebase, APNS, etc.).
/// Supports multi-platform delivery, topic subscriptions, and delivery tracking.
/// </summary>
public interface IPushProvider
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
    /// Sends a push notification to a specific device.
    /// </summary>
    /// <param name="request">Push notification request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Push delivery result</returns>
    Task<PushDeliveryResult> SendPushAsync(PushRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends push notifications to multiple devices efficiently.
    /// </summary>
    /// <param name="requests">Collection of push notification requests</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bulk delivery results</returns>
    Task<BulkPushDeliveryResult> SendBulkPushAsync(IEnumerable<PushRequest> requests, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a push notification to a topic/channel subscription.
    /// </summary>
    /// <param name="request">Topic push notification request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Push delivery result</returns>
    Task<PushDeliveryResult> SendTopicPushAsync(TopicPushRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes a device token to a topic/channel.
    /// </summary>
    /// <param name="deviceToken">Device token to subscribe</param>
    /// <param name="topic">Topic name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Subscription result</returns>
    Task<TopicSubscriptionResult> SubscribeToTopicAsync(string deviceToken, string topic, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unsubscribes a device token from a topic/channel.
    /// </summary>
    /// <param name="deviceToken">Device token to unsubscribe</param>
    /// <param name="topic">Topic name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Subscription result</returns>
    Task<TopicSubscriptionResult> UnsubscribeFromTopicAsync(string deviceToken, string topic, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a device token format and registration status.
    /// </summary>
    /// <param name="deviceToken">Device token to validate</param>
    /// <param name="platform">Target platform</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<DeviceTokenValidationResult> ValidateDeviceTokenAsync(string deviceToken, PushPlatform platform, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets delivery status for a previously sent push notification.
    /// </summary>
    /// <param name="messageId">Provider-specific message identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Delivery status</returns>
    Task<PushDeliveryStatus> GetDeliveryStatusAsync(string messageId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Request model for sending push notifications to specific devices.
/// </summary>
public class PushRequest
{
    /// <summary>
    /// Device token for the target device.
    /// </summary>
    public string DeviceToken { get; set; } = string.Empty;

    /// <summary>
    /// Target platform (iOS, Android, Web).
    /// </summary>
    public PushPlatform Platform { get; set; }

    /// <summary>
    /// Notification title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Notification body/message.
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Custom data payload (optional).
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();

    /// <summary>
    /// Notification icon (optional).
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Notification image URL (optional).
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Click action or deep link URL (optional).
    /// </summary>
    public string? ClickAction { get; set; }

    /// <summary>
    /// Notification sound (optional, uses default if not specified).
    /// </summary>
    public string? Sound { get; set; }

    /// <summary>
    /// Badge count for iOS (optional).
    /// </summary>
    public int? Badge { get; set; }

    /// <summary>
    /// Notification tag/channel for Android (optional).
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// Priority level for delivery.
    /// </summary>
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Time to live (TTL) in seconds for the notification.
    /// </summary>
    public int? TimeToLiveSeconds { get; set; }

    /// <summary>
    /// Whether to collapse/replace previous notifications with same collapse key.
    /// </summary>
    public string? CollapseKey { get; set; }

    /// <summary>
    /// Platform-specific options.
    /// </summary>
    public PlatformOptions? PlatformOptions { get; set; }

    /// <summary>
    /// Metadata for tracking and analytics.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Scheduled delivery time (optional, for future sending).
    /// </summary>
    public DateTime? ScheduledAt { get; set; }
}

/// <summary>
/// Request model for sending push notifications to topics.
/// </summary>
public class TopicPushRequest : PushRequest
{
    /// <summary>
    /// Topic name to send notification to.
    /// </summary>
    public string Topic { get; set; } = string.Empty;

    /// <summary>
    /// Condition expression for targeting specific subscribers (optional).
    /// Example: "'TopicA' in topics && ('TopicB' in topics || 'TopicC' in topics)"
    /// </summary>
    public string? Condition { get; set; }
}

/// <summary>
/// Platform-specific notification options.
/// </summary>
public class PlatformOptions
{
    /// <summary>
    /// iOS-specific options.
    /// </summary>
    public IosOptions? Ios { get; set; }

    /// <summary>
    /// Android-specific options.
    /// </summary>
    public AndroidOptions? Android { get; set; }

    /// <summary>
    /// Web push-specific options.
    /// </summary>
    public WebPushOptions? WebPush { get; set; }
}

/// <summary>
/// iOS-specific notification options.
/// </summary>
public class IosOptions
{
    /// <summary>
    /// Custom sound file name.
    /// </summary>
    public string? Sound { get; set; }

    /// <summary>
    /// Badge count to display on app icon.
    /// </summary>
    public int? Badge { get; set; }

    /// <summary>
    /// Whether notification is content-available (silent).
    /// </summary>
    public bool? ContentAvailable { get; set; }

    /// <summary>
    /// Whether notification is mutable-content (modifiable).
    /// </summary>
    public bool? MutableContent { get; set; }

    /// <summary>
    /// Category identifier for action buttons.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Thread identifier for grouping notifications.
    /// </summary>
    public string? ThreadId { get; set; }
}

/// <summary>
/// Android-specific notification options.
/// </summary>
public class AndroidOptions
{
    /// <summary>
    /// Notification channel ID.
    /// </summary>
    public string? ChannelId { get; set; }

    /// <summary>
    /// Notification icon resource name.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Notification color in #RRGGBB format.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Notification tag for replacement.
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// Sound file name or default/none.
    /// </summary>
    public string? Sound { get; set; }

    /// <summary>
    /// Click action intent filter.
    /// </summary>
    public string? ClickAction { get; set; }

    /// <summary>
    /// Message priority (high/normal).
    /// </summary>
    public string? Priority { get; set; }

    /// <summary>
    /// Restricted package name for targeting.
    /// </summary>
    public string? RestrictedPackageName { get; set; }
}

/// <summary>
/// Web push-specific notification options.
/// </summary>
public class WebPushOptions
{
    /// <summary>
    /// Custom icon URL.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Custom badge URL.
    /// </summary>
    public string? Badge { get; set; }

    /// <summary>
    /// Large image URL.
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// Notification tag for replacement.
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// Whether notification should be persistent.
    /// </summary>
    public bool? RequireInteraction { get; set; }

    /// <summary>
    /// Notification language.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Vibration pattern.
    /// </summary>
    public int[]? Vibrate { get; set; }

    /// <summary>
    /// Time to live in seconds.
    /// </summary>
    public int? Ttl { get; set; }

    /// <summary>
    /// Urgency level (very-low, low, normal, high).
    /// </summary>
    public string? Urgency { get; set; }
}

/// <summary>
/// Result of push notification delivery operation.
/// </summary>
public class PushDeliveryResult
{
    /// <summary>
    /// Indicates whether the push notification was successfully sent.
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
    /// Timestamp when the notification was sent.
    /// </summary>
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Number of devices reached (for topic notifications).
    /// </summary>
    public int? DevicesReached { get; set; }

    /// <summary>
    /// Additional metadata from the provider.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Result of bulk push notification delivery operation.
/// </summary>
public class BulkPushDeliveryResult
{
    /// <summary>
    /// Total number of push notifications processed.
    /// </summary>
    public int TotalProcessed { get; set; }

    /// <summary>
    /// Number of notifications successfully sent.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of notifications that failed to send.
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Individual results for each push notification.
    /// </summary>
    public List<PushDeliveryResult> Results { get; set; } = new();

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
/// Result of topic subscription operation.
/// </summary>
public class TopicSubscriptionResult
{
    /// <summary>
    /// Indicates whether the subscription operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Error code if operation failed.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Number of tokens successfully processed (for batch operations).
    /// </summary>
    public int? ProcessedTokens { get; set; }

    /// <summary>
    /// Additional metadata from the provider.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Result of device token validation operation.
/// </summary>
public class DeviceTokenValidationResult
{
    /// <summary>
    /// Indicates whether the device token is valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Validation reason or error message.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Whether the token is currently registered and active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Application package/bundle identifier.
    /// </summary>
    public string? ApplicationId { get; set; }

    /// <summary>
    /// Platform detected from token.
    /// </summary>
    public PushPlatform? DetectedPlatform { get; set; }
}

/// <summary>
/// Push notification delivery status information.
/// </summary>
public class PushDeliveryStatus
{
    /// <summary>
    /// Current delivery status.
    /// </summary>
    public NotificationDeliveryStatus Status { get; set; }

    /// <summary>
    /// Timestamp when status was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Delivery events timeline.
    /// </summary>
    public List<PushDeliveryEvent> Events { get; set; } = new();

    /// <summary>
    /// Error details if delivery failed.
    /// </summary>
    public string? ErrorDetails { get; set; }

    /// <summary>
    /// Number of devices that received the notification.
    /// </summary>
    public int? DevicesReached { get; set; }
}

/// <summary>
/// Individual push notification delivery event in the timeline.
/// </summary>
public class PushDeliveryEvent
{
    /// <summary>
    /// Event type (sent, delivered, opened, etc.).
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when event occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Event description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Additional event data.
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();
}

/// <summary>
/// Supported push notification platforms.
/// </summary>
public enum PushPlatform
{
    /// <summary>
    /// iOS devices (iPhone, iPad).
    /// </summary>
    Ios = 1,

    /// <summary>
    /// Android devices.
    /// </summary>
    Android = 2,

    /// <summary>
    /// Web browsers (PWA).
    /// </summary>
    Web = 3
}
