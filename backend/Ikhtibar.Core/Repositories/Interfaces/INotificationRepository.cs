using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for notification entities with specialized operations
/// Extends base repository with notification-specific query methods
/// </summary>
public interface INotificationRepository : IRepository<Notification>
{
    /// <summary>
    /// Gets notifications for a specific user with pagination
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of notifications per page</param>
    /// <param name="unreadOnly">If true, returns only unread notifications</param>
    /// <param name="notificationType">Optional filter by notification type</param>
    /// <returns>Paginated list of notifications for the user</returns>
    Task<(IEnumerable<Notification> Notifications, int TotalCount)> GetUserNotificationsAsync(
        int userId, 
        int page = 1, 
        int pageSize = 20, 
        bool unreadOnly = false,
        NotificationType? notificationType = null);

    /// <summary>
    /// Gets count of unread notifications for a user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <returns>Number of unread notifications</returns>
    Task<int> GetUnreadCountAsync(int userId);

    /// <summary>
    /// Gets notification by ID including user information
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <returns>Notification with user details if found</returns>
    Task<Notification?> GetByIdWithUserAsync(Guid notificationId);

    /// <summary>
    /// Marks a notification as read
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <param name="userId">User ID for authorization</param>
    /// <returns>True if successfully marked as read</returns>
    Task<bool> MarkAsReadAsync(Guid notificationId, int userId);

    /// <summary>
    /// Marks all notifications as read for a user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <returns>Number of notifications marked as read</returns>
    Task<int> MarkAllAsReadAsync(int userId);

    /// <summary>
    /// Gets pending notifications that are ready to be sent
    /// </summary>
    /// <param name="batchSize">Maximum number of notifications to return</param>
    /// <returns>List of notifications ready for processing</returns>
    Task<IEnumerable<Notification>> GetPendingNotificationsAsync(int batchSize = 100);

    /// <summary>
    /// Gets notifications scheduled for future delivery that are now due
    /// </summary>
    /// <param name="batchSize">Maximum number of notifications to return</param>
    /// <returns>List of scheduled notifications that are due</returns>
    Task<IEnumerable<Notification>> GetDueScheduledNotificationsAsync(int batchSize = 100);

    /// <summary>
    /// Updates notification status
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <param name="status">New status</param>
    /// <param name="sentAt">Optional sent timestamp</param>
    /// <returns>True if successfully updated</returns>
    Task<bool> UpdateStatusAsync(Guid notificationId, NotificationStatus status, DateTime? sentAt = null);

    /// <summary>
    /// Gets notifications by entity reference (e.g., all notifications for an exam)
    /// </summary>
    /// <param name="entityType">Entity type (e.g., "Exam", "User")</param>
    /// <param name="entityId">Entity ID</param>
    /// <returns>List of notifications related to the entity</returns>
    Task<IEnumerable<Notification>> GetByEntityAsync(string entityType, int entityId);

    /// <summary>
    /// Gets notification statistics for a user within a date range
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Statistics including total, read, unread counts by type</returns>
    Task<NotificationStats> GetUserStatsAsync(int userId, DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Deletes old read notifications beyond retention period
    /// </summary>
    /// <param name="retentionDays">Number of days to retain read notifications</param>
    /// <returns>Number of notifications deleted</returns>
    Task<int> CleanupOldNotificationsAsync(int retentionDays = 90);
}

/// <summary>
/// Statistics model for notification analytics
/// </summary>
public class NotificationStats
{
    public int TotalNotifications { get; set; }
    public int ReadNotifications { get; set; }
    public int UnreadNotifications { get; set; }
    public Dictionary<NotificationType, int> NotificationsByType { get; set; } = new();
    public Dictionary<NotificationChannel, int> NotificationsByChannel { get; set; } = new();
}
