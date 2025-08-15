using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Core.DTOs;

/// <summary>
/// Notification priority levels
/// </summary>
public enum NotificationPriority
{
    Low = 0,
    Normal = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

/// <summary>
/// Notification types
/// </summary>
public enum NotificationType
{
    Email = 0,
    SMS = 1,
    Push = 2,
    InApp = 3
}

/// <summary>
/// Notification status
/// </summary>
public enum NotificationStatus
{
    Pending = 0,
    Sent = 1,
    Delivered = 2,
    Failed = 3,
    Cancelled = 4
}

/// <summary>
/// Notification data transfer object
/// </summary>
public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; }
    public NotificationStatus Status { get; set; }
    public int UserId { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? MetadataJson { get; set; }
}

/// <summary>
/// Create notification DTO
/// </summary>
public class CreateNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;
    public int UserId { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public string? MetadataJson { get; set; }
}

/// <summary>
/// Notification filter DTO
/// </summary>
public class NotificationFilterDto
{
    public NotificationType? Type { get; set; }
    public NotificationStatus? Status { get; set; }
    public NotificationPriority? Priority { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsRead { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Paged result DTO
/// </summary>
public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
