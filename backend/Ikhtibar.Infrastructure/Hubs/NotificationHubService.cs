// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Hubs;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Enums;
using System.Collections.Concurrent;

namespace Ikhtibar.Infrastructure.Hubs;

/// <summary>
/// SignalR notification hub service implementation
/// Handles real-time notification delivery via WebSocket connections
/// </summary>
public class NotificationHubService : INotificationHub
{
    private readonly ILogger<NotificationHubService> _logger;
    private readonly ConcurrentDictionary<string, HashSet<string>> _userConnections;
    private readonly ConcurrentDictionary<string, HashSet<string>> _topicSubscriptions;
    private readonly ConcurrentDictionary<string, HashSet<string>> _roleConnections;
    private readonly ConcurrentDictionary<string, HashSet<string>> _departmentConnections;

    public NotificationHubService(ILogger<NotificationHubService> logger)
    {
        _logger = logger;
        _userConnections = new ConcurrentDictionary<string, HashSet<string>>();
        _topicSubscriptions = new ConcurrentDictionary<string, HashSet<string>>();
        _roleConnections = new ConcurrentDictionary<string, HashSet<string>>();
        _departmentConnections = new ConcurrentDictionary<string, HashSet<string>>();
    }

    /// <summary>
    /// Sends notification to a specific user
    /// </summary>
    public async Task SendToUserAsync(int userId, NotificationDto notification)
    {
        try
        {
            var userKey = userId.ToString();
            if (_userConnections.TryGetValue(userKey, out var connections))
            {
                var tasks = connections.Select(connectionId => 
                    SendToConnectionAsync(connectionId, "ReceiveNotification", notification));
                
                await Task.WhenAll(tasks);
                _logger.LogInformation("Notification sent to user {UserId} via {ConnectionCount} connections", 
                    userId, connections.Count);
            }
            else
            {
                _logger.LogDebug("User {UserId} has no active connections", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
        }
    }

    /// <summary>
    /// Sends notification to multiple users
    /// </summary>
    public async Task SendToUsersAsync(IEnumerable<int> userIds, NotificationDto notification)
    {
        try
        {
            var tasks = userIds.Select(userId => SendToUserAsync(userId, notification));
            await Task.WhenAll(tasks);
            
            _logger.LogInformation("Notification sent to {UserCount} users", userIds.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to multiple users");
        }
    }

    /// <summary>
    /// Sends notification to all users in a specific role
    /// </summary>
    public async Task SendToRoleAsync(string roleName, NotificationDto notification)
    {
        try
        {
            if (_roleConnections.TryGetValue(roleName, out var connections))
            {
                var tasks = connections.Select(connectionId => 
                    SendToConnectionAsync(connectionId, "ReceiveNotification", notification));
                
                await Task.WhenAll(tasks);
                _logger.LogInformation("Notification sent to role {RoleName} via {ConnectionCount} connections", 
                    roleName, connections.Count);
            }
            else
            {
                _logger.LogDebug("Role {RoleName} has no active connections", roleName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to role {RoleName}", roleName);
        }
    }

    /// <summary>
    /// Sends notification to all users in a specific department
    /// </summary>
    public async Task SendToDepartmentAsync(int departmentId, NotificationDto notification)
    {
        try
        {
            var deptKey = departmentId.ToString();
            if (_departmentConnections.TryGetValue(deptKey, out var connections))
            {
                var tasks = connections.Select(connectionId => 
                    SendToConnectionAsync(connectionId, "ReceiveNotification", notification));
                
                await Task.WhenAll(tasks);
                _logger.LogInformation("Notification sent to department {DepartmentId} via {ConnectionCount} connections", 
                    departmentId, connections.Count);
            }
            else
            {
                _logger.LogDebug("Department {DepartmentId} has no active connections", departmentId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to department {DepartmentId}", departmentId);
        }
    }

    /// <summary>
    /// Broadcasts notification to all connected users
    /// </summary>
    public async Task BroadcastAsync(NotificationDto notification, IEnumerable<int>? excludeUserIds = null)
    {
        try
        {
            var excludeSet = excludeUserIds?.ToHashSet() ?? new HashSet<int>();
            var allConnections = _userConnections
                .Where(kvp => !excludeSet.Contains(int.Parse(kvp.Key)))
                .SelectMany(kvp => kvp.Value)
                .Distinct()
                .ToList();

            if (allConnections.Any())
            {
                var tasks = allConnections.Select(connectionId => 
                    SendToConnectionAsync(connectionId, "ReceiveNotification", notification));
                
                await Task.WhenAll(tasks);
                _logger.LogInformation("Notification broadcasted to {ConnectionCount} connections", allConnections.Count);
            }
            else
            {
                _logger.LogDebug("No active connections for broadcast");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error broadcasting notification");
        }
    }

    /// <summary>
    /// Sends notification to users subscribed to a specific topic
    /// </summary>
    public async Task SendToTopicAsync(string topic, NotificationDto notification)
    {
        try
        {
            if (_topicSubscriptions.TryGetValue(topic, out var connections))
            {
                var tasks = connections.Select(connectionId => 
                    SendToConnectionAsync(connectionId, "ReceiveNotification", notification));
                
                await Task.WhenAll(tasks);
                _logger.LogInformation("Notification sent to topic {Topic} via {ConnectionCount} connections", 
                    topic, connections.Count);
            }
            else
            {
                _logger.LogDebug("Topic {Topic} has no active subscriptions", topic);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to topic {Topic}", topic);
        }
    }

    /// <summary>
    /// Subscribes user to a topic
    /// </summary>
    public async Task SubscribeToTopicAsync(string topic)
    {
        // This would be implemented with actual SignalR hub context
        _logger.LogDebug("User subscribed to topic {Topic}", topic);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Unsubscribes user from a topic
    /// </summary>
    public async Task UnsubscribeFromTopicAsync(string topic)
    {
        // This would be implemented with actual SignalR hub context
        _logger.LogDebug("User unsubscribed from topic {Topic}", topic);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Gets user's active subscriptions
    /// </summary>
    public async Task<IEnumerable<string>> GetUserSubscriptionsAsync()
    {
        // This would be implemented with actual SignalR hub context
        _logger.LogDebug("Getting user subscriptions");
        await Task.CompletedTask;
        return Enumerable.Empty<string>();
    }

    #region Connection Management

    /// <summary>
    /// Registers a user connection
    /// </summary>
    public void RegisterUserConnection(int userId, string connectionId)
    {
        var userKey = userId.ToString();
        _userConnections.AddOrUpdate(userKey, 
            new HashSet<string> { connectionId },
            (key, existing) =>
            {
                existing.Add(connectionId);
                return existing;
            });
        
        _logger.LogDebug("User {UserId} connection {ConnectionId} registered", userId, connectionId);
    }

    /// <summary>
    /// Unregisters a user connection
    /// </summary>
    public void UnregisterUserConnection(int userId, string connectionId)
    {
        var userKey = userId.ToString();
        if (_userConnections.TryGetValue(userKey, out var connections))
        {
            connections.Remove(connectionId);
            if (!connections.Any())
            {
                _userConnections.TryRemove(userKey, out _);
            }
        }
        
        _logger.LogDebug("User {UserId} connection {ConnectionId} unregistered", userId, connectionId);
    }

    /// <summary>
    /// Registers a role connection
    /// </summary>
    public void RegisterRoleConnection(string roleName, string connectionId)
    {
        _roleConnections.AddOrUpdate(roleName,
            new HashSet<string> { connectionId },
            (key, existing) =>
            {
                existing.Add(connectionId);
                return existing;
            });
    }

    /// <summary>
    /// Registers a department connection
    /// </summary>
    public void RegisterDepartmentConnection(int departmentId, string connectionId)
    {
        var deptKey = departmentId.ToString();
        _departmentConnections.AddOrUpdate(deptKey,
            new HashSet<string> { connectionId },
            (key, existing) =>
            {
                existing.Add(connectionId);
                return existing;
            });
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Sends message to a specific connection
    /// </summary>
    private async Task SendToConnectionAsync(string connectionId, string method, object message)
    {
        try
        {
            // This would be implemented with actual SignalR hub context
            // For now, we'll just log the attempt
            _logger.LogDebug("Sending {Method} to connection {ConnectionId}", method, connectionId);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending {Method} to connection {ConnectionId}", method, connectionId);
        }
    }

    #endregion
}
*/
