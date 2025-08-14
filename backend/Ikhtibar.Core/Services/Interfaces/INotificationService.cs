using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Core notification service interface for comprehensive notification management
/// Handles notification creation, delivery, scheduling, and event-driven notifications
/// Following SRP: ONLY notification business logic operations
/// </summary>
public interface INotificationService
{
    // Core notification operations
    /// <summary>
    /// Creates a new notification
    /// </summary>
    /// <param name="dto">Notification creation data</param>
    /// <returns>Created notification data</returns>
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto);

    /// <summary>
    /// Sends a notification through appropriate channels based on user preferences
    /// </summary>
    /// <param name="notificationId">Notification ID to send</param>
    /// <returns>True if sent successfully through at least one channel</returns>
    Task<bool> SendNotificationAsync(Guid notificationId);

    /// <summary>
    /// Creates and immediately sends a notification
    /// </summary>
    /// <param name="dto">Notification creation data</param>
    /// <returns>True if created and sent successfully</returns>
    Task<bool> SendImmediateNotificationAsync(CreateNotificationDto dto);

    /// <summary>
    /// Gets paginated notifications for a user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="filter">Filtering and pagination options</param>
    /// <returns>Paginated notification results</returns>
    Task<PagedResult<NotificationDto>> GetUserNotificationsAsync(int userId, NotificationFilterDto filter);

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
    /// Gets count of unread notifications for a user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <returns>Number of unread notifications</returns>
    Task<int> GetUnreadCountAsync(int userId);

    // Bulk operations
    /// <summary>
    /// Sends notifications to multiple users
    /// </summary>
    /// <param name="notifications">List of notification creation data</param>
    /// <returns>Number of notifications successfully sent</returns>
    Task<int> SendBulkNotificationAsync(List<CreateNotificationDto> notifications);

    /// <summary>
    /// Schedules a notification for future delivery
    /// </summary>
    /// <param name="dto">Notification creation data</param>
    /// <param name="scheduleTime">When to send the notification</param>
    /// <returns>True if successfully scheduled</returns>
    Task<bool> ScheduleNotificationAsync(CreateNotificationDto dto, DateTime scheduleTime);

    /// <summary>
    /// Cancels a pending or scheduled notification
    /// </summary>
    /// <param name="notificationId">Notification ID to cancel</param>
    /// <returns>True if successfully cancelled</returns>
    Task<bool> CancelNotificationAsync(Guid notificationId);

    /// <summary>
    /// Gets a notification by ID for a specific user
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <param name="userId">User ID for authorization</param>
    /// <returns>Notification data if found and user has access</returns>
    Task<NotificationDto?> GetNotificationByIdAsync(Guid notificationId, int userId);

    /// <summary>
    /// Deletes a notification for a specific user
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <param name="userId">User ID for authorization</param>
    /// <returns>True if successfully deleted</returns>
    Task<bool> DeleteNotificationAsync(Guid notificationId, int userId);

    // Event-driven notifications
    /// <summary>
    /// Sends exam reminder notification to enrolled students
    /// </summary>
    /// <param name="examId">Exam ID</param>
    /// <param name="reminderMinutes">Minutes before exam start</param>
    Task SendExamReminderAsync(int examId, int reminderMinutes);

    /// <summary>
    /// Sends exam start notification to enrolled students
    /// </summary>
    /// <param name="examId">Exam ID</param>
    Task SendExamStartNotificationAsync(int examId);

    /// <summary>
    /// Sends exam end notification to enrolled students
    /// </summary>
    /// <param name="examId">Exam ID</param>
    Task SendExamEndNotificationAsync(int examId);

    /// <summary>
    /// Sends grading complete notification to student
    /// </summary>
    /// <param name="examId">Exam ID</param>
    /// <param name="studentId">Student user ID</param>
    /// <param name="score">Final score</param>
    /// <param name="grade">Letter grade</param>
    Task SendGradingCompleteNotificationAsync(int examId, int studentId, decimal score, string grade);

    /// <summary>
    /// Sends deadline reminder notification
    /// </summary>
    /// <param name="entityType">Type of entity (e.g., "Exam", "Assignment")</param>
    /// <param name="entityId">Entity ID</param>
    /// <param name="deadline">Deadline date/time</param>
    /// <param name="userIds">List of users to notify</param>
    Task SendDeadlineReminderAsync(string entityType, int entityId, DateTime deadline, List<int> userIds);

    /// <summary>
    /// Sends welcome notification to new user
    /// </summary>
    /// <param name="userId">New user ID</param>
    Task SendWelcomeNotificationAsync(int userId);

    /// <summary>
    /// Sends password reset notification
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="resetToken">Password reset token</param>
    /// <param name="resetUrl">Password reset URL</param>
    Task SendPasswordResetNotificationAsync(int userId, string resetToken, string resetUrl);

    /// <summary>
    /// Sends role assignment notification
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleName">Assigned role name</param>
    /// <param name="assignedBy">User who assigned the role</param>
    Task SendRoleAssignmentNotificationAsync(int userId, string roleName, string assignedBy);

    /// <summary>
    /// Sends system maintenance notification to all users
    /// </summary>
    /// <param name="maintenanceStart">Maintenance start time</param>
    /// <param name="maintenanceEnd">Maintenance end time</param>
    /// <param name="description">Maintenance description</param>
    Task SendSystemMaintenanceNotificationAsync(DateTime maintenanceStart, DateTime maintenanceEnd, string description);

    // Administrative operations
    /// <summary>
    /// Processes pending scheduled notifications
    /// Called by background service to send due notifications
    /// </summary>
    /// <param name="batchSize">Maximum number of notifications to process</param>
    /// <returns>Number of notifications processed</returns>
    Task<int> ProcessScheduledNotificationsAsync(int batchSize = 100);

    /// <summary>
    /// Retries failed notification deliveries
    /// </summary>
    /// <param name="maxRetryCount">Maximum retry attempts</param>
    /// <param name="batchSize">Maximum number of notifications to retry</param>
    /// <returns>Number of notifications retried</returns>
    Task<int> RetryFailedNotificationsAsync(int maxRetryCount = 3, int batchSize = 50);

    /// <summary>
    /// Gets notification statistics for admin dashboard
    /// </summary>
    /// <param name="fromDate">Start date for statistics</param>
    /// <param name="toDate">End date for statistics</param>
    /// <returns>Comprehensive notification statistics</returns>
    Task<NotificationSystemStats> GetSystemStatsAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Cleans up old notifications and history records
    /// </summary>
    /// <param name="notificationRetentionDays">Days to retain notifications</param>
    /// <param name="historyRetentionDays">Days to retain delivery history</param>
    /// <returns>Number of records cleaned up</returns>
    Task<CleanupResult> CleanupOldDataAsync(int notificationRetentionDays = 90, int historyRetentionDays = 180);
}
