using System.ComponentModel.DataAnnotations;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for creating a new notification
/// </summary>
public class CreateNotificationDto
{
    /// <summary>
    /// Target user ID for the notification
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Type of notification
    /// </summary>
    [Required]
    [EnumDataType(typeof(NotificationType))]
    public NotificationType NotificationType { get; set; }
    
    /// <summary>
    /// Type of notification (alternative property name for compatibility)
    /// </summary>
    [EnumDataType(typeof(NotificationType))]
    public NotificationType Type 
    { 
        get => NotificationType; 
        set => NotificationType = value; 
    }

    /// <summary>
    /// Priority level of the notification
    /// </summary>
    [EnumDataType(typeof(NotificationPriority))]
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Subject/title of the notification
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Subject { get; set; } = string.Empty;
    
    /// <summary>
    /// Title of the notification (alternative property name for compatibility)
    /// </summary>
    [StringLength(500)]
    public string Title 
    { 
        get => Subject; 
        set => Subject = value; 
    }

    /// <summary>
    /// Main message content
    /// </summary>
    [Required]
    [StringLength(5000)]
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Content of the notification (alternative property name for compatibility)
    /// </summary>
    [StringLength(5000)]
    public string Content 
    { 
        get => Message; 
        set => Message = value; 
    }

    /// <summary>
    /// Optional scheduled delivery time (null for immediate delivery)
    /// </summary>
    public DateTime? ScheduledAt { get; set; }
    
    /// <summary>
    /// Scheduled delivery time (alternative property name for compatibility)
    /// </summary>
    public DateTime? ScheduledFor 
    { 
        get => ScheduledAt; 
        set => ScheduledAt = value; 
    }
    
    /// <summary>
    /// Notification expiration time
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Delivery channels for this notification
    /// </summary>
    public List<NotificationChannel> Channels { get; set; } = new();

    /// <summary>
    /// Entity type this notification relates to (e.g., "Exam", "User")
    /// </summary>
    [StringLength(100)]
    public string? EntityType { get; set; }

    /// <summary>
    /// Entity ID this notification relates to
    /// </summary>
    public int? EntityId { get; set; }

    /// <summary>
    /// Template ID to use for rendering (optional)
    /// </summary>
    public int? TemplateId { get; set; }

    /// <summary>
    /// Variables for template substitution
    /// </summary>
    public Dictionary<string, object> Variables { get; set; } = new();

    /// <summary>
    /// Channel-specific delivery data
    /// </summary>
    public Dictionary<string, object> ChannelData { get; set; } = new();

    /// <summary>
    /// Metadata for additional context
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for updating notification properties
/// </summary>
public class UpdateNotificationDto
{
    /// <summary>
    /// Priority level of the notification
    /// </summary>
    [EnumDataType(typeof(NotificationPriority))]
    public NotificationPriority? Priority { get; set; }

    /// <summary>
    /// Subject/title of the notification
    /// </summary>
    [StringLength(500)]
    public string? Subject { get; set; }

    /// <summary>
    /// Main message content
    /// </summary>
    [StringLength(5000)]
    public string? Message { get; set; }

    /// <summary>
    /// Scheduled delivery time
    /// </summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// Channel-specific delivery data
    /// </summary>
    public Dictionary<string, object>? ChannelData { get; set; }

    /// <summary>
    /// Metadata for additional context
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// DTO for notification response data
/// </summary>
public class NotificationDto
{
    /// <summary>
    /// Unique identifier for the notification
    /// </summary>
    public int id { get; set; }

    /// <summary>
    /// Target user ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Type of notification
    /// </summary>
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Priority level
    /// </summary>
    public NotificationPriority Priority { get; set; }

    /// <summary>
    /// Current status
    /// </summary>
    public NotificationStatus Status { get; set; }

    /// <summary>
    /// Subject/title
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Main message content
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Whether the notification has been read
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// When the notification was read (if applicable)
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// When the notification was sent
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Scheduled delivery time
    /// </summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// Entity type this notification relates to
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// Entity ID this notification relates to
    /// </summary>
    public int? EntityId { get; set; }

    /// <summary>
    /// Template ID used for rendering
    /// </summary>
    public int? TemplateId { get; set; }

    /// <summary>
    /// Variables used for template substitution
    /// </summary>
    public Dictionary<string, object> Variables { get; set; } = new();

    /// <summary>
    /// Channel-specific delivery data
    /// </summary>
    public Dictionary<string, object> ChannelData { get; set; } = new();

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// When the notification was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the notification was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; }
}

/// <summary>
/// DTO for bulk notification operations
/// </summary>
public class BulkNotificationDto
{
    /// <summary>
    /// List of user IDs to send notifications to
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<int> UserIds { get; set; } = new();

    /// <summary>
    /// Type of notification
    /// </summary>
    [Required]
    [EnumDataType(typeof(NotificationType))]
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Priority level of the notification
    /// </summary>
    [EnumDataType(typeof(NotificationPriority))]
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Subject/title of the notification
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Main message content
    /// </summary>
    [Required]
    [StringLength(5000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional scheduled delivery time
    /// </summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// Template ID to use for rendering (optional)
    /// </summary>
    public int? TemplateId { get; set; }

    /// <summary>
    /// Variables for template substitution
    /// </summary>
    public Dictionary<string, object> Variables { get; set; } = new();

    /// <summary>
    /// Channel-specific delivery data
    /// </summary>
    public Dictionary<string, object> ChannelData { get; set; } = new();

    /// <summary>
    /// Metadata for additional context
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for marking notifications as read
/// </summary>
public class MarkAsReadDto
{
    /// <summary>
    /// List of notification IDs to mark as read
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<int> NotificationIds { get; set; } = new();
}

/// <summary>
/// DTO for notification delivery tracking
/// </summary>
public class NotificationDeliveryDto
{
    /// <summary>
    /// Notification ID
    /// </summary>
    public int NotificationId { get; set; }

    /// <summary>
    /// Delivery channel
    /// </summary>
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Delivery status
    /// </summary>
    public NotificationDeliveryStatus Status { get; set; }

    /// <summary>
    /// When delivery was attempted
    /// </summary>
    public DateTime AttemptedAt { get; set; }

    /// <summary>
    /// When delivery was completed (if successful)
    /// </summary>
    public DateTime? DeliveredAt { get; set; }

    /// <summary>
    /// Error message if delivery failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Delivery cost (if applicable)
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Additional delivery metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for paginated notification results
/// </summary>
public class PagedNotificationResultDto
{
    /// <summary>
    /// List of notifications for the current page
    /// </summary>
    public List<NotificationDto> Notifications { get; set; } = new();

    /// <summary>
    /// Total number of notifications matching the criteria
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;
}

/// <summary>
/// DTO for bulk operation results
/// </summary>
public class BulkOperationResultDto
{
    /// <summary>
    /// Number of successful operations
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed operations
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// List of error messages for failed operations
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Total number of operations attempted
    /// </summary>
    public int TotalCount => SuccessCount + FailureCount;

    /// <summary>
    /// Success rate as a percentage
    /// </summary>
    public double SuccessRate => TotalCount > 0 ? (double)SuccessCount / TotalCount * 100 : 0;
}
