// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.Hubs;

/// <summary>
/// Notification hub interface for real-time communication
/// </summary>
public interface INotificationHub
{
    /// <summary>
    /// Sends notification to a specific user
    /// </summary>
    Task SendToUserAsync(int userId, NotificationDto notification);

    /// <summary>
    /// Sends notification to multiple users
    /// </summary>
    Task SendToUsersAsync(IEnumerable<int> userIds, NotificationDto notification);

    /// <summary>
    /// Sends notification to all users in a specific role
    /// </summary>
    Task SendToRoleAsync(string roleName, NotificationDto notification);

    /// <summary>
    /// Sends notification to all users in a specific department
    /// </summary>
    Task SendToDepartmentAsync(int departmentId, NotificationDto notification);

    /// <summary>
    /// Broadcasts notification to all connected users
    /// </summary>
    Task BroadcastAsync(NotificationDto notification, IEnumerable<int>? excludeUserIds = null);

    /// <summary>
    /// Sends notification to users subscribed to a specific topic
    /// </summary>
    Task SendToTopicAsync(string topic, NotificationDto notification);

    /// <summary>
    /// Subscribes user to a topic
    /// </summary>
    Task SubscribeToTopicAsync(string topic);

    /// <summary>
    /// Unsubscribes user from a topic
    /// </summary>
    Task UnsubscribeFromTopicAsync(string topic);

    /// <summary>
    /// Gets user's active subscriptions
    /// </summary>
    Task<IEnumerable<string>> GetUserSubscriptionsAsync();
}
*/
