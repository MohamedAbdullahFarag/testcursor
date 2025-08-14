using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Background job service interface for notification scheduling and processing
/// </summary>
public interface INotificationJobService
{
    /// <summary>
    /// Schedules a notification for future delivery
    /// </summary>
    /// <param name="notificationId">ID of notification to schedule</param>
    /// <param name="scheduleTime">When to deliver the notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Job ID for tracking</returns>
    Task<string> ScheduleNotificationAsync(Guid notificationId, DateTime scheduleTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules a recurring notification
    /// </summary>
    /// <param name="notificationId">ID of notification to schedule</param>
    /// <param name="cronExpression">Cron expression for recurrence pattern</param>
    /// <param name="endTime">Optional end time for recurring notifications</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Job ID for tracking</returns>
    Task<string> ScheduleRecurringNotificationAsync(Guid notificationId, string cronExpression, DateTime? endTime = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a scheduled notification job
    /// </summary>
    /// <param name="jobId">Job ID to cancel</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successfully cancelled</returns>
    Task<bool> CancelScheduledNotificationAsync(string jobId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cancels all scheduled notifications for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of jobs cancelled</returns>
    Task<int> CancelAllScheduledNotificationsForUserAsync(int userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Schedules cleanup of old notifications
    /// </summary>
    /// <param name="retentionDays">Number of days to retain notifications</param>
    /// <param name="notificationTypes">Optional specific types to clean up</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Job ID for tracking</returns>
    Task<string> ScheduleCleanupJobAsync(int retentionDays, IEnumerable<NotificationType>? notificationTypes = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all scheduled jobs for a notification
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of scheduled jobs</returns>
    Task<List<ScheduledJobDto>> GetScheduledJobsForNotificationAsync(Guid notificationId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all scheduled jobs for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of scheduled jobs</returns>
    Task<List<ScheduledJobDto>> GetScheduledJobsForUserAsync(int userId, CancellationToken cancellationToken = default);
}
