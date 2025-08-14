using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Core.Hubs;

/// <summary>
/// Interface for real-time notification hub operations.
/// Defines the contract for notification delivery via SignalR.
/// </summary>
public interface INotificationHub
{
    /// <summary>
    /// Sends a notification to a specific user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="notification">Notification to send</param>
    Task SendToUserAsync(int userId, NotificationDto notification);

    /// <summary>
    /// Sends notification to multiple users
    /// </summary>
    /// <param name="userIds">List of target user IDs</param>
    /// <param name="notification">Notification to send</param>
    Task SendToUsersAsync(IEnumerable<int> userIds, NotificationDto notification);

    /// <summary>
    /// Updates the unread notification count for a user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="count">New unread count</param>
    Task UpdateUnreadCountAsync(int userId, int count);
    
    /// <summary>
    /// Broadcasts a notification to all connected users
    /// </summary>
    /// <param name="notification">Notification to broadcast</param>
    /// <param name="excludeUserIds">Optional user IDs to exclude</param>
    Task BroadcastAsync(NotificationDto notification, IEnumerable<int>? excludeUserIds = null);
    
    /// <summary>
    /// Sends a notification to a specific topic/group
    /// </summary>
    /// <param name="topic">Target topic name</param>
    /// <param name="notification">Notification to send</param>
    Task SendToTopicAsync(string topic, NotificationDto notification);
    
    /// <summary>
    /// Updates notification status (read, deleted) for a user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="notificationId">Notification ID</param>
    /// <param name="status">New status</param>
    Task UpdateNotificationStatusAsync(int userId, Guid notificationId, string status);
}
