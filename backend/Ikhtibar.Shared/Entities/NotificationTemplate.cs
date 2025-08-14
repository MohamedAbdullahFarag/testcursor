using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Enums;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Template entity for managing reusable notification templates
/// Supports multi-language templates with variable substitution
/// </summary>
[Table("NotificationTemplates")]
public class NotificationTemplate : BaseEntity
{
    /// <summary>
    /// Unique template name for identification
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Notification type this template applies to
    /// </summary>
    [Required]
    public NotificationType Type { get; set; }

    /// <summary>
    /// Template for notification subject/title with variable placeholders
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string SubjectTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Template for notification message body with variable placeholders
    /// Supports HTML content for email notifications
    /// </summary>
    [Required]
    [MaxLength(5000)]
    public string MessageTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Language code for this template (e.g., "en", "ar")
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Language { get; set; } = "en";

    /// <summary>
    /// Whether this template is currently active
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// JSON array of available variables for this template
    /// Example: ["UserName", "ExamTitle", "StartTime"]
    /// </summary>
    [MaxLength(1000)]
    public string? VariablesJson { get; set; }

    /// <summary>
    /// Optional description of the template purpose
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Email-specific HTML template (optional)
    /// </summary>
    [Column(TypeName = "ntext")]
    public string? EmailHtmlTemplate { get; set; }

    /// <summary>
    /// SMS-specific template (shorter version)
    /// </summary>
    [MaxLength(500)]
    public string? SmsTemplate { get; set; }

    /// <summary>
    /// Push notification title template
    /// </summary>
    [MaxLength(100)]
    public string? PushTitleTemplate { get; set; }

    /// <summary>
    /// Push notification body template
    /// </summary>
    [MaxLength(300)]
    public string? PushBodyTemplate { get; set; }

    // Navigation properties
    /// <summary>
    /// Notifications that used this template
    /// </summary>
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
