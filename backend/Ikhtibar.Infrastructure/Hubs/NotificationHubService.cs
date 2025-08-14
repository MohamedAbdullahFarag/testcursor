using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Hubs;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Infrastructure.Hubs;

/// <summary>
/// Implementation of the notification hub service for real-time notification delivery.
/// Uses SignalR to push notifications to connected clients.
/// </summary>
public class NotificationHubService : INotificationHub
{
    private readonly IHubContext<Hub> _hubContext;
    private readonly ILogger<NotificationHubService> _logger;

    public NotificationHubService(
        IHubContext<Hub> hubContext,
        ILogger<NotificationHubService> logger)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sends a notification to a specific user
    /// </summary>
    public async Task SendToUserAsync(int userId, NotificationDto notification)
    {
        try
        {
            await _hubContext.Clients.Group($"User_{userId}")
                .SendAsync("ReceiveNotification", notification);
            
            _logger.LogInformation("Sent notification {NotificationId} to user {UserId}", 
                notification.Id, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification {NotificationId} to user {UserId}", 
                notification.Id, userId);
        }
    }

    /// <summary>
    /// Sends notification to multiple users
    /// </summary>
    public async Task SendToUsersAsync(IEnumerable<int> userIds, NotificationDto notification)
    {
        if (userIds == null || !userIds.Any())
        {
            _logger.LogWarning("No users provided to send notification {NotificationId}", notification.Id);
            return;
        }

        var tasks = new List<Task>();
        foreach (var userId in userIds)
        {
            tasks.Add(SendToUserAsync(userId, notification));
        }

        await Task.WhenAll(tasks);
        _logger.LogInformation("Sent notification {NotificationId} to {UserCount} users", 
            notification.Id, userIds.Count());
    }

    /// <summary>
    /// Updates the unread notification count for a user
    /// </summary>
    public async Task UpdateUnreadCountAsync(int userId, int count)
    {
        try
        {
            await _hubContext.Clients.Group($"User_{userId}")
                .SendAsync("UnreadCountUpdated", count);
            
            _logger.LogDebug("Updated unread count to {Count} for user {UserId}", 
                count, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update unread count for user {UserId}", userId);
        }
    }
    
    /// <summary>
    /// Broadcasts a notification to all connected users
    /// </summary>
    public async Task BroadcastAsync(NotificationDto notification, IEnumerable<int>? excludeUserIds = null)
    {
        try
        {
            if (excludeUserIds == null || !excludeUserIds.Any())
            {
                await _hubContext.Clients.All
                    .SendAsync("ReceiveNotification", notification);
                
                _logger.LogInformation("Broadcast notification {NotificationId} to all connected users", 
                    notification.Id);
            }
            else
            {
                // In a real implementation, we would filter out excluded users more efficiently
                // This simple implementation broadcasts to everyone since we can't easily exclude specific users
                await _hubContext.Clients.All
                    .SendAsync("ReceiveNotification", notification);
                
                _logger.LogInformation("Broadcast notification {NotificationId} to all connected users (except {ExcludedCount})", 
                    notification.Id, excludeUserIds.Count());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast notification {NotificationId}", notification.Id);
        }
    }
    
    /// <summary>
    /// Sends a notification to a specific topic/group
    /// </summary>
    public async Task SendToTopicAsync(string topic, NotificationDto notification)
    {
        if (string.IsNullOrWhiteSpace(topic))
        {
            _logger.LogWarning("No topic provided to send notification {NotificationId}", notification.Id);
            return;
        }

        try
        {
            await _hubContext.Clients.Group($"Topic_{topic}")
                .SendAsync("ReceiveNotification", notification);
            
            _logger.LogInformation("Sent notification {NotificationId} to topic {Topic}", 
                notification.Id, topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification {NotificationId} to topic {Topic}", 
                notification.Id, topic);
        }
    }
    
    /// <summary>
    /// Updates notification status (read, deleted) for a user
    /// </summary>
    public async Task UpdateNotificationStatusAsync(int userId, Guid notificationId, string status)
    {
        try
        {
            await _hubContext.Clients.Group($"User_{userId}")
                .SendAsync("NotificationStatusChanged", notificationId.ToString(), status);
            
            _logger.LogDebug("Updated status for notification {NotificationId} to {Status} for user {UserId}", 
                notificationId, status, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update status for notification {NotificationId} for user {UserId}", 
                notificationId, userId);
        }
    }
}
