namespace Ikhtibar.Shared.Enums;

/// <summary>
/// Priority levels for notification delivery and display
/// </summary>
public enum NotificationPriority
{
    /// <summary>
    /// Low priority - sent in batch, minimal UI prominence
    /// </summary>
    Low = 1,

    /// <summary>
    /// Normal priority - standard delivery and UI
    /// </summary>
    Normal = 2,

    /// <summary>
    /// High priority - prioritized delivery, prominent UI
    /// </summary>
    High = 3,

    /// <summary>
    /// Urgent priority - immediate delivery, alert-style UI
    /// </summary>
    Urgent = 4,
    
    /// <summary>
    /// Critical priority - highest priority, immediate attention required
    /// </summary>
    Critical = 5
}
