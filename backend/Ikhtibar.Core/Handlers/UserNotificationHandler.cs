// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Events;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.Handlers;

/// <summary>
/// User notification event handler
/// Processes user-related events and triggers appropriate notifications
/// </summary>
public class UserNotificationHandler
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<UserNotificationHandler> _logger;

    public UserNotificationHandler(
        INotificationService notificationService,
        ILogger<UserNotificationHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Handles user welcome events
    /// </summary>
    public async Task HandleUserWelcomeAsync(UserEventNotifications.UserWelcomeEvent @event)
    {
        try
        {
            _logger.LogInformation("Handling user welcome event for user {UserId}", @event.UserId);
            await _notificationService.SendWelcomeNotificationAsync(@event.UserId, @event.WelcomeMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling user welcome event for user {UserId}", @event.UserId);
        }
    }

    /// <summary>
    /// Handles password reset events
    /// </summary>
    public async Task HandlePasswordResetAsync(UserEventNotifications.PasswordResetEvent @event)
    {
        try
        {
            _logger.LogInformation("Handling password reset event for user {UserId}", @event.UserId);
            await _notificationService.SendPasswordResetNotificationAsync(@event.UserId, @event.ResetToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling password reset event for user {UserId}", @event.UserId);
        }
    }

    /// <summary>
    /// Handles role assignment events
    /// </summary>
    public async Task HandleRoleAssignmentAsync(UserEventNotifications.RoleAssignmentEvent @event)
    {
        try
        {
            _logger.LogInformation("Handling role assignment event for user {UserId}, role {RoleName}", 
                @event.UserId, @event.RoleName);
            await _notificationService.SendRoleAssignmentNotificationAsync(@event.UserId, @event.RoleName, @event.AssignedBy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling role assignment event for user {UserId}, role {RoleName}", 
                @event.UserId, @event.RoleName);
        }
    }
}
*/
