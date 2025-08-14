namespace Ikhtibar.Shared.Enums;

/// <summary>
/// Detailed delivery status for tracking notification lifecycle
/// </summary>
public enum NotificationDeliveryStatus
{
    /// <summary>
    /// Delivery attempt is pending
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Successfully sent to provider
    /// </summary>
    Sent = 2,

    /// <summary>
    /// Confirmed delivered to recipient
    /// </summary>
    Delivered = 3,

    /// <summary>
    /// Delivery failed
    /// </summary>
    Failed = 4,

    /// <summary>
    /// Email bounced back
    /// </summary>
    Bounced = 5,

    /// <summary>
    /// Notification was opened by user
    /// </summary>
    Opened = 6,

    /// <summary>
    /// User clicked on notification content
    /// </summary>
    Clicked = 7,

    /// <summary>
    /// User unsubscribed from this type
    /// </summary>
    Unsubscribed = 8,

    /// <summary>
    /// Marked as spam by recipient
    /// </summary>
    Spam = 9,

    /// <summary>
    /// Delivery was rate limited
    /// </summary>
    RateLimited = 10,

    /// <summary>
    /// Delivery was blocked by provider
    /// </summary>
    Blocked = 11,

    /// <summary>
    /// Status is unknown or not available
    /// </summary>
    Unknown = 12,

    /// <summary>
    /// Message was read by the recipient
    /// </summary>
    Read = 13
}
