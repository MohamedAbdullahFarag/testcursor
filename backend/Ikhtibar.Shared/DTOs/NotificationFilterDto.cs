using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for filtering notifications in queries
/// Used for pagination, searching, and filtering notifications
/// </summary>
public class NotificationFilterDto
{
    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Filter to show only unread notifications
    /// </summary>
    public bool? UnreadOnly { get; set; }

    /// <summary>
    /// Filter by notification type
    /// </summary>
    public NotificationType? NotificationType { get; set; }

    /// <summary>
    /// Filter by notification priority
    /// </summary>
    public NotificationPriority? Priority { get; set; }

    /// <summary>
    /// Filter by notification status
    /// </summary>
    public NotificationStatus? Status { get; set; }

    /// <summary>
    /// Filter notifications from this date
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Filter notifications to this date
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Search term for notification content
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by entity type the notification relates to
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// Filter by entity ID the notification relates to
    /// </summary>
    public int? EntityId { get; set; }

    /// <summary>
    /// Whether the notification has been read
    /// </summary>
    public bool? IsRead { get; set; }
}
