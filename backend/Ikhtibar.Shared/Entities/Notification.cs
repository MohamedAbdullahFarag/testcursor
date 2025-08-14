using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Enums;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Core notification entity for managing all types of notifications in the system
/// Supports email, SMS, in-app, and push notifications with priority and scheduling
/// </summary>
[Table("Notifications")]
public class Notification : BaseEntity
{
    /// <summary>
    /// Notification title - displayed as subject line or notification headline
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Main notification message content
    /// </summary>
    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Type of notification - determines template and delivery rules
    /// </summary>
    [Required]
    public NotificationType Type { get; set; }

    /// <summary>
    /// Priority level affecting delivery order and UI presentation
    /// </summary>
    [Required]
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Current status of the notification
    /// </summary>
    [Required]
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    /// <summary>
    /// Target user ID for this notification
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Optional entity type that triggered this notification (e.g., "Exam", "User", "Grade")
    /// </summary>
    [MaxLength(100)]
    public string? EntityType { get; set; }

    /// <summary>
    /// Optional entity ID that triggered this notification
    /// </summary>
    public int? EntityId { get; set; }

    /// <summary>
    /// When the notification should be sent (supports future scheduling)
    /// </summary>
    [Required]
    public DateTime ScheduledAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when notification was actually sent
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Timestamp when user marked notification as read
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// JSON metadata for additional notification data (templates variables, etc.)
    /// </summary>
    [MaxLength(2000)]
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Template ID used for this notification (if templated)
    /// </summary>
    public int? TemplateId { get; set; }

    /// <summary>
    /// Language code for this notification (e.g., "en", "ar")
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Language { get; set; } = "en";

    // Navigation properties
    /// <summary>
    /// User who receives this notification
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Template used for this notification
    /// </summary>
    public virtual NotificationTemplate? Template { get; set; }

    /// <summary>
    /// Delivery history for this notification across all channels
    /// </summary>
    public virtual ICollection<NotificationHistory> DeliveryHistory { get; set; } = new List<NotificationHistory>();
}
