using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Events;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Core.Handlers;

/// <summary>
/// Event handler for user-related domain events that trigger automated notifications.
/// Processes user lifecycle events and creates appropriate notifications for security, 
/// account management, and compliance purposes.
/// </summary>
public class UserNotificationHandler : 
    IEventHandler<UserAccountCreatedEvent>,
    IEventHandler<UserRoleChangedEvent>,
    IEventHandler<UserAccountDeactivatedEvent>,
    IEventHandler<UserLoginEvent>,
    IEventHandler<UserPasswordResetEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<UserNotificationHandler> _logger;

    public UserNotificationHandler(
        INotificationService notificationService,
        ILogger<UserNotificationHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int Priority => 1; // High priority for security notifications
    public bool ContinueOnFailure => true; // Continue processing other handlers on failure

    /// <summary>
    /// Handles user account creation events by sending welcome notifications.
    /// Creates welcome messages with account setup information and security guidelines.
    /// </summary>
    public async Task HandleAsync(UserAccountCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Handling UserAccountCreatedEvent {EventId} for user {UserId}", 
            domainEvent.EventId, domainEvent.TargetUserId);

        try
        {
            _logger.LogInformation("Processing account creation notification for user {Email} with role {Role}", 
                domainEvent.UserEmail, domainEvent.AssignedRole);

            // Create welcome notification for the new user
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = domainEvent.TargetUserId,
                NotificationType = NotificationType.Welcome,
                Priority = NotificationPriority.Normal,
                Subject = $"Welcome to Ikhtibar - Account Created Successfully",
                Message = $"Welcome to Ikhtibar, {domainEvent.UserFullName}! " +
                         $"Your account has been created successfully with {domainEvent.AssignedRole} privileges. " +
                         $"Created by: {domainEvent.CreatedByAdminName}. " +
                         $"Activation Info: {domainEvent.ActivationInfo}. " +
                         $"Access the platform at: {domainEvent.PlatformUrl}",
                EntityType = "User",
                EntityId = domainEvent.TargetUserId,
                Metadata = new Dictionary<string, object>
                {
                    ["TargetUserId"] = domainEvent.TargetUserId,
                    ["UserEmail"] = domainEvent.UserEmail,
                    ["UserFullName"] = domainEvent.UserFullName,
                    ["AssignedRole"] = domainEvent.AssignedRole,
                    ["CreatedByAdminId"] = domainEvent.CreatedByAdminId,
                    ["CreatedByAdminName"] = domainEvent.CreatedByAdminName,
                    ["ActivationInfo"] = domainEvent.ActivationInfo,
                    ["PlatformUrl"] = domainEvent.PlatformUrl,
                    ["EventId"] = domainEvent.EventId.ToString(),
                    ["CorrelationId"] = domainEvent.CorrelationId ?? string.Empty
                }
            });

            // Create notification for the admin who created the account (if different user)
            if (domainEvent.CreatedByAdminId != domainEvent.TargetUserId)
            {
                await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                {
                    UserId = domainEvent.CreatedByAdminId,
                    NotificationType = NotificationType.AccountActivation,
                    Priority = NotificationPriority.Low,
                    Subject = $"User Account Created: {domainEvent.UserFullName}",
                    Message = $"Successfully created account for {domainEvent.UserFullName} " +
                             $"({domainEvent.UserEmail}) with {domainEvent.AssignedRole} role.",
                    EntityType = "User",
                    EntityId = domainEvent.TargetUserId,
                    Metadata = new Dictionary<string, object>
                    {
                        ["NewUserId"] = domainEvent.TargetUserId,
                        ["NewUserEmail"] = domainEvent.UserEmail,
                        ["NewUserRole"] = domainEvent.AssignedRole,
                        ["EventId"] = domainEvent.EventId.ToString()
                    }
                });
            }

            _logger.LogInformation("Successfully created user account creation notifications");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling user account creation event {EventId}", domainEvent.EventId);
            throw;
        }
    }

    /// <summary>
    /// Handles user role change events by sending security notifications.
    /// Creates notifications for privilege changes with audit information.
    /// </summary>
    public async Task HandleAsync(UserRoleChangedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Handling UserRoleChangedEvent {EventId} for user {UserId}", 
            domainEvent.EventId, domainEvent.TargetUserId);

        try
        {
            _logger.LogInformation("Processing role change notification from {PreviousRole} to {NewRole} for user {Email}", 
                domainEvent.PreviousRole, domainEvent.NewRole, domainEvent.UserEmail);

            // Determine notification priority based on role change significance
            var priority = IsSignificantRoleChange(domainEvent.PreviousRole, domainEvent.NewRole) 
                ? NotificationPriority.High 
                : NotificationPriority.Normal;

            // Create role change notification for the affected user
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = domainEvent.TargetUserId,
                NotificationType = NotificationType.RoleAssignment,
                Priority = priority,
                Subject = $"Account Role Updated - {domainEvent.NewRole}",
                Message = $"Your account role has been changed from {domainEvent.PreviousRole} to {domainEvent.NewRole} " +
                         $"by {domainEvent.ChangedByAdminName}. " +
                         $"This change affects your platform access and permissions. " +
                         $"If you have questions, please contact support.",
                EntityType = "User",
                EntityId = domainEvent.TargetUserId,
                Metadata = new Dictionary<string, object>
                {
                    ["TargetUserId"] = domainEvent.TargetUserId,
                    ["UserEmail"] = domainEvent.UserEmail,
                    ["PreviousRole"] = domainEvent.PreviousRole,
                    ["NewRole"] = domainEvent.NewRole,
                    ["ChangedByAdminId"] = domainEvent.ChangedByAdminId,
                    ["ChangedByAdminName"] = domainEvent.ChangedByAdminName,
                    ["PermissionChanges"] = string.Join(", ", domainEvent.PermissionChanges),
                    ["IsElevation"] = IsRoleElevation(domainEvent.PreviousRole, domainEvent.NewRole),
                    ["EventId"] = domainEvent.EventId.ToString(),
                    ["CorrelationId"] = domainEvent.CorrelationId ?? string.Empty
                }
            });

            // Create audit notification for the admin who made the change
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = domainEvent.ChangedByAdminId,
                NotificationType = NotificationType.RoleAssignment,
                Priority = NotificationPriority.Low,
                Subject = $"Role Change Completed: {domainEvent.UserEmail}",
                Message = $"Successfully changed role for {domainEvent.UserEmail} " +
                         $"from {domainEvent.PreviousRole} to {domainEvent.NewRole}. " +
                         $"Permission changes: {string.Join(", ", domainEvent.PermissionChanges)}",
                EntityType = "User",
                EntityId = domainEvent.TargetUserId,
                Metadata = new Dictionary<string, object>
                {
                    ["TargetUserId"] = domainEvent.TargetUserId,
                    ["TargetUserEmail"] = domainEvent.UserEmail,
                    ["RoleChangeType"] = IsRoleElevation(domainEvent.PreviousRole, domainEvent.NewRole) ? "Elevation" : "Reduction",
                    ["EventId"] = domainEvent.EventId.ToString()
                }
            });

            _logger.LogInformation("Successfully created user role change notifications");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling user role change event {EventId}", domainEvent.EventId);
            throw;
        }
    }

    /// <summary>
    /// Handles user account deactivation events by sending security notifications.
    /// Creates notifications for account suspension with compliance information.
    /// </summary>
    public async Task HandleAsync(UserAccountDeactivatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Handling UserAccountDeactivatedEvent {EventId} for user {UserId}", 
            domainEvent.EventId, domainEvent.TargetUserId);

        try
        {
            _logger.LogInformation("Processing account deactivation notification for user {Email}, reason: {Reason}", 
                domainEvent.UserEmail, domainEvent.DeactivationReason);

            // Create deactivation notification for the affected user
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = domainEvent.TargetUserId,
                NotificationType = NotificationType.AccountLocked,
                Priority = NotificationPriority.High,
                Subject = $"Account Deactivated - Access Suspended",
                Message = $"Your Ikhtibar account has been deactivated. " +
                         $"Reason: {domainEvent.DeactivationReason}. " +
                         $"Deactivated by: {domainEvent.DeactivatedByAdminName}. " +
                         (domainEvent.IsTemporary 
                            ? $"This is temporary until {domainEvent.ReactivationDate:yyyy-MM-dd}. "
                            : "This deactivation is permanent. ") +
                         $"Contact support if you need assistance.",
                EntityType = "User",
                EntityId = domainEvent.TargetUserId,
                Metadata = new Dictionary<string, object>
                {
                    ["TargetUserId"] = domainEvent.TargetUserId,
                    ["UserEmail"] = domainEvent.UserEmail,
                    ["DeactivationReason"] = domainEvent.DeactivationReason,
                    ["DeactivatedByAdminId"] = domainEvent.DeactivatedByAdminId,
                    ["DeactivatedByAdminName"] = domainEvent.DeactivatedByAdminName,
                    ["IsTemporary"] = domainEvent.IsTemporary,
                    ["ReactivationDate"] = domainEvent.ReactivationDate?.ToString("yyyy-MM-dd") ?? "",
                    ["ComplianceNotes"] = domainEvent.ComplianceNotes ?? "",
                    ["EventId"] = domainEvent.EventId.ToString(),
                    ["CorrelationId"] = domainEvent.CorrelationId ?? string.Empty
                }
            });

            // Create compliance notification for the admin who deactivated the account
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = domainEvent.DeactivatedByAdminId,
                NotificationType = NotificationType.AccountLocked,
                Priority = NotificationPriority.Normal,
                Subject = $"Account Deactivation Completed: {domainEvent.UserEmail}",
                Message = $"Successfully deactivated account for {domainEvent.UserEmail}. " +
                         $"Reason: {domainEvent.DeactivationReason}. " +
                         (domainEvent.IsTemporary ? "Temporary deactivation." : "Permanent deactivation.") +
                         (!string.IsNullOrEmpty(domainEvent.ComplianceNotes) 
                            ? $" Compliance notes: {domainEvent.ComplianceNotes}" : ""),
                EntityType = "User",
                EntityId = domainEvent.TargetUserId,
                Metadata = new Dictionary<string, object>
                {
                    ["DeactivatedUserId"] = domainEvent.TargetUserId,
                    ["DeactivatedUserEmail"] = domainEvent.UserEmail,
                    ["DeactivationType"] = domainEvent.IsTemporary ? "Temporary" : "Permanent",
                    ["EventId"] = domainEvent.EventId.ToString()
                }
            });

            _logger.LogInformation("Successfully created user account deactivation notifications");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling user account deactivation event {EventId}", domainEvent.EventId);
            throw;
        }
    }

    /// <summary>
    /// Handles user login events by sending security notifications for suspicious activity.
    /// Creates notifications for unusual login patterns or security-relevant sessions.
    /// </summary>
    public async Task HandleAsync(UserLoginEvent domainEvent, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Handling UserLoginEvent {EventId} for user {UserId}", 
            domainEvent.EventId, domainEvent.TargetUserId);

        try
        {
            // Only create notifications for suspicious or notable login events
            if (!domainEvent.IsSuspiciousActivity && !domainEvent.IsFirstTimeLogin && !domainEvent.IsNewDevice)
            {
                _logger.LogDebug("Skipping notification for routine login event");
                return;
            }

            _logger.LogInformation("Processing login notification for user {Email} from {Location}, " +
                                 "suspicious: {IsSuspicious}, new device: {IsNewDevice}", 
                                 domainEvent.UserEmail, domainEvent.Location, 
                                 domainEvent.IsSuspiciousActivity, domainEvent.IsNewDevice);

            // Determine notification priority and type based on login characteristics
            var priority = domainEvent.IsSuspiciousActivity ? NotificationPriority.Critical : NotificationPriority.Normal;
            var notificationType = domainEvent.IsSuspiciousActivity 
                ? NotificationType.SecurityAlert 
                : NotificationType.AccountActivation;

            string subject, message;

            if (domainEvent.IsSuspiciousActivity)
            {
                subject = "⚠️ Suspicious Login Activity Detected";
                message = $"A suspicious login to your Ikhtibar account was detected. " +
                         $"Location: {domainEvent.Location}, Device: {domainEvent.DeviceType}, " +
                         $"Time: {domainEvent.LoginTimestamp:yyyy-MM-dd HH:mm} UTC. " +
                         $"If this wasn't you, please change your password immediately and contact support.";
            }
            else if (domainEvent.IsFirstTimeLogin)
            {
                subject = "Welcome! First Login Completed";
                message = $"Welcome to Ikhtibar! You've successfully completed your first login. " +
                         $"Login details: {domainEvent.Location}, {domainEvent.DeviceType}, " +
                         $"{domainEvent.LoginTimestamp:yyyy-MM-dd HH:mm} UTC. " +
                         $"Please review your account security settings.";
            }
            else // New device
            {
                subject = "New Device Login Detected";
                message = $"A login from a new device was detected on your Ikhtibar account. " +
                         $"Location: {domainEvent.Location}, Device: {domainEvent.DeviceType}, " +
                         $"Time: {domainEvent.LoginTimestamp:yyyy-MM-dd HH:mm} UTC. " +
                         $"If this wasn't you, please secure your account immediately.";
            }

            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = domainEvent.TargetUserId,
                NotificationType = notificationType,
                Priority = priority,
                Subject = subject,
                Message = message,
                ScheduledAt = DateTime.UtcNow, // Immediate delivery for security notifications
                EntityType = "User",
                EntityId = domainEvent.TargetUserId,
                Metadata = new Dictionary<string, object>
                {
                    ["TargetUserId"] = domainEvent.TargetUserId,
                    ["UserEmail"] = domainEvent.UserEmail ?? string.Empty,
                    ["LoginTimestamp"] = domainEvent.LoginTimestamp,
                    ["IpAddress"] = domainEvent.IpAddress ?? string.Empty,
                    ["UserAgent"] = domainEvent.UserAgent ?? string.Empty,
                    ["DeviceType"] = domainEvent.DeviceType ?? string.Empty,
                    ["Location"] = domainEvent.Location ?? string.Empty,
                    ["IsSuspiciousActivity"] = domainEvent.IsSuspiciousActivity,
                    ["IsFirstTimeLogin"] = domainEvent.IsFirstTimeLogin,
                    ["IsNewDevice"] = domainEvent.IsNewDevice,
                    ["SessionDuration"] = domainEvent.SessionDuration?.ToString() ?? "",
                    ["EventId"] = domainEvent.EventId.ToString(),
                    ["CorrelationId"] = domainEvent.CorrelationId ?? string.Empty
                }
            });

            _logger.LogInformation("Successfully created user login notification");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling user login event {EventId}", domainEvent.EventId);
            throw;
        }
    }

    /// <summary>
    /// Handles user password reset events by sending confirmation notifications.
    /// Creates notifications for password reset confirmations and security alerts.
    /// </summary>
    public async Task HandleAsync(UserPasswordResetEvent domainEvent, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Handling UserPasswordResetEvent {EventId} for user {UserId}", 
            domainEvent.EventId, domainEvent.TargetUserId);

        try
        {
            _logger.LogInformation("Processing password reset notification for user {Email}, " +
                                 "initiated by: {InitiatedBy}, method: {ResetMethod}", 
                                 domainEvent.UserEmail, domainEvent.InitiatedByUser, domainEvent.ResetMethod);

            // Create password reset notification for the user
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = domainEvent.TargetUserId,
                NotificationType = NotificationType.PasswordReset,
                Priority = NotificationPriority.High,
                Subject = $"Password Reset Completed",
                Message = $"Your Ikhtibar account password has been successfully reset. " +
                         $"Reset method: {domainEvent.ResetMethod}, " +
                         $"Initiated by: {domainEvent.InitiatedByUser}, " +
                         $"Time: {domainEvent.ResetTimestamp:yyyy-MM-dd HH:mm} UTC. " +
                         $"Token expires: {domainEvent.TokenExpiryTime:yyyy-MM-dd HH:mm} UTC. " +
                         $"If you didn't initiate this reset, please contact support immediately.",
                ScheduledAt = DateTime.UtcNow, // Immediate delivery for security notifications
                EntityType = "User",
                EntityId = domainEvent.TargetUserId,
                Metadata = new Dictionary<string, object>
                {
                    ["TargetUserId"] = domainEvent.TargetUserId,
                    ["UserEmail"] = domainEvent.UserEmail,
                    ["ResetTimestamp"] = domainEvent.ResetTimestamp,
                    ["ResetMethod"] = domainEvent.ResetMethod,
                    ["InitiatedByUser"] = domainEvent.InitiatedByUser,
                    ["ResetToken"] = domainEvent.ResetToken ?? "",
                    ["TokenExpiryTime"] = domainEvent.TokenExpiryTime,
                    ["RequestIpAddress"] = domainEvent.RequestIpAddress ?? "",
                    ["RequestUserAgent"] = domainEvent.RequestUserAgent ?? "",
                    ["EventId"] = domainEvent.EventId.ToString(),
                    ["CorrelationId"] = domainEvent.CorrelationId ?? string.Empty
                }
            });

            _logger.LogInformation("Successfully created password reset notification");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling password reset event {EventId}", domainEvent.EventId);
            throw;
        }
    }

    /// <summary>
    /// Determines if a role change represents a significant security elevation.
    /// </summary>
    private static bool IsSignificantRoleChange(string oldRole, string newRole)
    {
        // Define role hierarchy levels for comparison
        var roleHierarchy = new Dictionary<string, int>
        {
            ["Student"] = 1,
            ["Teacher"] = 2,
            ["Admin"] = 3,
            ["SuperAdmin"] = 4
        };

        if (!roleHierarchy.TryGetValue(oldRole, out int oldLevel) || 
            !roleHierarchy.TryGetValue(newRole, out int newLevel))
        {
            return true; // Unknown roles are considered significant
        }

        return Math.Abs(newLevel - oldLevel) > 1; // More than one level change
    }

    /// <summary>
    /// Determines if a role change represents an elevation in privileges.
    /// </summary>
    private static bool IsRoleElevation(string oldRole, string newRole)
    {
        var roleHierarchy = new Dictionary<string, int>
        {
            ["Student"] = 1,
            ["Teacher"] = 2,
            ["Admin"] = 3,
            ["SuperAdmin"] = 4
        };

        if (!roleHierarchy.TryGetValue(oldRole, out int oldLevel) || 
            !roleHierarchy.TryGetValue(newRole, out int newLevel))
        {
            return false;
        }

        return newLevel > oldLevel;
    }
}
