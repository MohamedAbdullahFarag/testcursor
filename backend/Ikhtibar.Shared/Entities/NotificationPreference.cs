using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Enums;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// User notification preferences for controlling delivery channels
/// Allows users to configure how they receive different types of notifications
/// </summary>
[Table("NotificationPreferences")]
public class NotificationPreference : BaseEntity
{
    /// <summary>
    /// User ID these preferences belong to
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Type of notification these preferences apply to
    /// </summary>
    [Required]
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Whether user wants to receive email notifications for this type
    /// </summary>
    [Required]
    public bool EmailEnabled { get; set; } = true;

    /// <summary>
    /// Whether user wants to receive SMS notifications for this type
    /// </summary>
    [Required]
    public bool SmsEnabled { get; set; } = false;

    /// <summary>
    /// Whether user wants to receive in-app notifications for this type
    /// </summary>
    [Required]
    public bool InAppEnabled { get; set; } = true;

    /// <summary>
    /// Whether user wants to receive push notifications for this type
    /// </summary>
    [Required]
    public bool PushEnabled { get; set; } = true;

    /// <summary>
    /// Whether quiet hours are enabled for this user
    /// </summary>
    [Required]
    public bool QuietHoursEnabled { get; set; } = false;

    /// <summary>
    /// Quiet hours start time (hour of day, 0-23)
    /// </summary>
    public int QuietHoursStart { get; set; } = 22;

    /// <summary>
    /// Quiet hours end time (hour of day, 0-23)
    /// </summary>
    public int QuietHoursEnd { get; set; } = 7;

    /// <summary>
    /// Time zone for quiet hours calculation
    /// </summary>
    [MaxLength(50)]
    public string? TimeZone { get; set; }

    /// <summary>
    /// Frequency limit for this notification type (e.g., max 5 per hour)
    /// 0 means no limit
    /// </summary>
    public int FrequencyLimit { get; set; } = 0;

    /// <summary>
    /// Frequency window in minutes for the frequency limit
    /// </summary>
    public int FrequencyWindowMinutes { get; set; } = 60;

    // Navigation properties
    /// <summary>
    /// User these preferences belong to
    /// </summary>
    public virtual User User { get; set; } = null!;
}
