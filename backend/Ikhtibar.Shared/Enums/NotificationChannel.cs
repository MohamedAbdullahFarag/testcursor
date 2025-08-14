namespace Ikhtibar.Shared.Enums;

/// <summary>
/// Notification delivery channel
/// </summary>
public enum NotificationChannel
{
    /// <summary>
    /// Email notification
    /// </summary>
    Email = 1,
    
    /// <summary>
    /// SMS text message
    /// </summary>
    Sms = 2,
    
    /// <summary>
    /// SMS text message (alternative reference)
    /// </summary>
    SMS = 2,
    
    /// <summary>
    /// Mobile push notification
    /// </summary>
    Push = 3,
    
    /// <summary>
    /// In-app notification
    /// </summary>
    InApp = 4,
    
    /// <summary>
    /// WhatsApp message
    /// </summary>
    WhatsApp = 5,
    
    /// <summary>
    /// Slack message
    /// </summary>
    Slack = 6,
    
    /// <summary>
    /// Microsoft Teams message
    /// </summary>
    Teams = 7
}
