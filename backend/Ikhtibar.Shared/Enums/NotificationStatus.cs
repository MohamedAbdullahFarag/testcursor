namespace Ikhtibar.Shared.Enums;

/// <summary>
/// Current status of a notification
/// </summary>
public enum NotificationStatus
{
    /// <summary>
    /// Created but not yet sent
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Successfully sent to at least one channel
    /// </summary>
    Sent = 2,

    /// <summary>
    /// Failed to send through any channel
    /// </summary>
    Failed = 3,

    /// <summary>
    /// User has read the notification
    /// </summary>
    Read = 4,

    /// <summary>
    /// Notification was cancelled before sending
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// Scheduled for future delivery
    /// </summary>
    Scheduled = 6
}
