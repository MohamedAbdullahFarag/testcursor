// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using System.Security.Claims;

namespace Ikhtibar.API.Hubs;

/// <summary>
/// Real-time notification hub using SignalR for immediate notification delivery.
/// Provides functionality for user-specific and broadcast notifications with various scopes.
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;
    private readonly INotificationService _notificationService;
    
    public NotificationHub(
        ILogger<NotificationHub> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }
    
    /// <summary>
    /// Called when a connection with the hub is established.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
        {
            // Add the connection to the user-specific group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            
            _logger.LogInformation("User {UserId} connected to NotificationHub with connection {ConnectionId}",
                userId, Context.ConnectionId);
            
            // Send initial unread count - replace with the appropriate method from your NotificationService
            var notifications = await _notificationService.GetUserNotificationsAsync(userId,
                new NotificationFilterDto { IsRead = false });
            int unreadCount = notifications.TotalCount;
            await Clients.Caller.SendAsync("UnreadCountUpdated", unreadCount);
        }
        
        await base.OnConnectedAsync();
    }
    
    /// <summary>
    /// Called when a connection with the hub is terminated.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            
            _logger.LogInformation("User {UserId} disconnected from NotificationHub with connection {ConnectionId}",
                userId, Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
    
    /// <summary>
    /// Marks a notification as read from the client side.
    /// </summary>
    public async Task MarkAsRead(int notificationId)
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogInformation("User {UserId} marking notification {NotificationId} as read", 
                userId, notificationId);
            
            // Update notification status in the database
            await _notificationService.MarkAsReadAsync(notificationId, userId);
            
            // Update the status for this user's connections
            await Clients.Group($"User_{userId}").SendAsync("NotificationStatusChanged", notificationId.ToString(), "Read");
            
            // Send updated unread count
            var notifications = await _notificationService.GetUserNotificationsAsync(userId,
                new NotificationFilterDto { IsRead = false });
            int unreadCount = notifications.TotalCount;
            await Clients.Group($"User_{userId}").SendAsync("UnreadCountUpdated", unreadCount);
        }
    }
    
    /// <summary>
    /// Client-initiated subscription to a specific topic (e.g., an exam or course).
    /// </summary>
    public async Task SubscribeToTopic(string topic)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Topic_{topic}");
        _logger.LogInformation("Connection {ConnectionId} subscribed to topic {Topic}", 
            Context.ConnectionId, topic);
    }
    
    /// <summary>
    /// Client-initiated unsubscription from a specific topic.
    /// </summary>
    public async Task UnsubscribeFromTopic(string topic)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Topic_{topic}");
        _logger.LogInformation("Connection {ConnectionId} unsubscribed from topic {Topic}", 
            Context.ConnectionId, topic);
    }
    
    /// <summary>
    /// Updates the client's connection status to manage presence information.
    /// </summary>
    public async Task UpdatePresence(string status)
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
        {
            // Store the user's presence status (could use a distributed cache for scalability)
            _logger.LogInformation("User {UserId} presence status changed to {Status}", 
                userId, status);
                
            // Implementation to store presence information would go here
            await Task.CompletedTask; // Placeholder to satisfy async method requirement
        }
    }
}
*/
