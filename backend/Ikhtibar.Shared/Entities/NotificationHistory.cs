using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Records the delivery history of notifications across different channels
/// Tracks delivery attempts, status, and performance metrics
/// </summary>
[Table("NotificationHistory")]
public class NotificationHistory : BaseEntity
{
    /// <summary>
    /// Associated notification ID
    /// </summary>
    [Required]
    public Guid NotificationId { get; set; }

    /// <summary>
    /// Delivery channel (Email, SMS, Push, etc.)
    /// </summary>
    [Required]
    public NotificationChannel Channel { get; set; }
    
    /// <summary>
    /// Service provider used for delivery (SendGrid, Twilio, etc.)
    /// </summary>
    [MaxLength(100)]
    public string? Provider { get; set; }
    
    /// <summary>
    /// Delivery status of the notification
    /// </summary>
    [Required]
    public NotificationDeliveryStatus DeliveryStatus { get; set; }
    
    /// <summary>
    /// Status property for backward compatibility
    /// </summary>
    [NotMapped]
    public NotificationDeliveryStatus Status 
    { 
        get => DeliveryStatus; 
        set => DeliveryStatus = value; 
    }

    /// <summary>
    /// When the delivery was attempted
    /// </summary>
    [Required]
    public DateTime AttemptedAt { get; set; }

    /// <summary>
    /// When the notification was successfully delivered
    /// </summary>
    public DateTime? DeliveredAt { get; set; }
    
    /// <summary>
    /// When the notification was opened/read
    /// </summary>
    public DateTime? OpenedAt { get; set; }
    
    /// <summary>
    /// When the notification was clicked
    /// </summary>
    public DateTime? ClickedAt { get; set; }

    /// <summary>
    /// Error message if delivery failed
    /// </summary>
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// External ID from delivery provider
    /// </summary>
    [MaxLength(500)]
    public string? ExternalId { get; set; }

    /// <summary>
    /// Response data from delivery provider
    /// </summary>
    [MaxLength(2000)]
    public string? ResponseData { get; set; }
    
    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Delivery cost (if available)
    /// </summary>
    [Column(TypeName = "decimal(18, 6)")]
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Currency of the cost
    /// </summary>
    [MaxLength(10)]
    public string? CostCurrency { get; set; }
    
    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    [MaxLength(2000)]
    public string? Metadata { get; set; }
    
    /// <summary>
    /// Priority for delivery ordering
    /// </summary>
    public int Priority { get; set; }

    // Navigation property
    public virtual Notification? Notification { get; set; }
}