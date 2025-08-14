using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.Core.Events;

/// <summary>
/// Domain events related to user management that trigger automated notifications.
/// These events are published when user-related activities occur and need notification.
/// Integrates with the notification system to send account and role-related updates.
/// </summary>
public abstract class UserEventBase : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime CreatedAt { get; }
    public int? UserId { get; }
    public string? CorrelationId { get; }
    public Dictionary<string, object> Metadata { get; }
    
    /// <summary>
    /// The user identifier associated with this event.
    /// </summary>
    public int TargetUserId { get; }
    
    /// <summary>
    /// The email address of the user for notification delivery.
    /// </summary>
    public string UserEmail { get; }
    
    /// <summary>
    /// The full name of the user for personalized notifications.
    /// </summary>
    public string UserFullName { get; }

    protected UserEventBase(int targetUserId, string userEmail, string userFullName, int? initiatingUserId = null, string? correlationId = null)
    {
        EventId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        TargetUserId = targetUserId;
        UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        UserFullName = userFullName ?? throw new ArgumentNullException(nameof(userFullName));
        UserId = initiatingUserId;
        CorrelationId = correlationId;
        Metadata = new Dictionary<string, object>();
    }
}

/// <summary>
/// Published when a new user account is created and welcome notifications need to be sent.
/// Triggers welcome email with account details and platform introduction.
/// </summary>
public class UserAccountCreatedEvent : UserEventBase
{
    /// <summary>
    /// The assigned role for the new user account.
    /// </summary>
    public string AssignedRole { get; }
    
    /// <summary>
    /// The temporary password or activation link for first login.
    /// </summary>
    public string ActivationInfo { get; }
    
    /// <summary>
    /// The URL for the user to access the platform.
    /// </summary>
    public string PlatformUrl { get; }
    
    /// <summary>
    /// The administrator who created the account.
    /// </summary>
    public int CreatedByAdminId { get; }
    
    /// <summary>
    /// The administrator's name for reference.
    /// </summary>
    public string CreatedByAdminName { get; }

    public UserAccountCreatedEvent(
        int targetUserId,
        string userEmail,
        string userFullName,
        string assignedRole,
        string activationInfo,
        string platformUrl,
        int createdByAdminId,
        string createdByAdminName,
        string? correlationId = null)
        : base(targetUserId, userEmail, userFullName, createdByAdminId, correlationId)
    {
        AssignedRole = assignedRole ?? throw new ArgumentNullException(nameof(assignedRole));
        ActivationInfo = activationInfo ?? throw new ArgumentNullException(nameof(activationInfo));
        PlatformUrl = platformUrl ?? throw new ArgumentNullException(nameof(platformUrl));
        CreatedByAdminId = createdByAdminId;
        CreatedByAdminName = createdByAdminName ?? throw new ArgumentNullException(nameof(createdByAdminName));
        
        // Add metadata for template rendering
        Metadata["AssignedRole"] = assignedRole;
        Metadata["ActivationInfo"] = activationInfo;
        Metadata["PlatformUrl"] = platformUrl;
        Metadata["CreatedByAdminName"] = createdByAdminName;
        Metadata["IsFirstTimeUser"] = true;
    }
}

/// <summary>
/// Published when a user's role is changed and they need to be notified.
/// Triggers role change notification with new permissions and access details.
/// </summary>
public class UserRoleChangedEvent : UserEventBase
{
    /// <summary>
    /// The previous role before the change.
    /// </summary>
    public string PreviousRole { get; }
    
    /// <summary>
    /// The new role assigned to the user.
    /// </summary>
    public string NewRole { get; }
    
    /// <summary>
    /// The administrator who made the role change.
    /// </summary>
    public int ChangedByAdminId { get; }
    
    /// <summary>
    /// The administrator's name for reference.
    /// </summary>
    public string ChangedByAdminName { get; }
    
    /// <summary>
    /// Reason for the role change (optional).
    /// </summary>
    public string? ChangeReason { get; }
    
    /// <summary>
    /// List of permission changes resulting from role change.
    /// </summary>
    public List<string> PermissionChanges { get; }

    public UserRoleChangedEvent(
        int targetUserId,
        string userEmail,
        string userFullName,
        string previousRole,
        string newRole,
        int changedByAdminId,
        string changedByAdminName,
        List<string>? permissionChanges = null,
        string? changeReason = null,
        string? correlationId = null)
        : base(targetUserId, userEmail, userFullName, changedByAdminId, correlationId)
    {
        PreviousRole = previousRole ?? throw new ArgumentNullException(nameof(previousRole));
        NewRole = newRole ?? throw new ArgumentNullException(nameof(newRole));
        ChangedByAdminId = changedByAdminId;
        ChangedByAdminName = changedByAdminName ?? throw new ArgumentNullException(nameof(changedByAdminName));
        ChangeReason = changeReason;
        PermissionChanges = permissionChanges ?? new List<string>();
        ChangeReason = changeReason;
        
        // Add metadata for template rendering
        Metadata["PreviousRole"] = previousRole;
        Metadata["NewRole"] = newRole;
        Metadata["ChangedByAdminName"] = changedByAdminName;
        Metadata["ChangeReason"] = changeReason ?? string.Empty;
        Metadata["IsPromotion"] = DetermineIfPromotion(previousRole, newRole);
    }
    
    private static bool DetermineIfPromotion(string previousRole, string newRole)
    {
        // Simple promotion logic - can be enhanced based on role hierarchy
        var roleHierarchy = new Dictionary<string, int>
        {
            ["Student"] = 1,
            ["Teacher"] = 2,
            ["Admin"] = 3,
            ["SuperAdmin"] = 4
        };
        
        var previousLevel = roleHierarchy.GetValueOrDefault(previousRole, 0);
        var newLevel = roleHierarchy.GetValueOrDefault(newRole, 0);
        
        return newLevel > previousLevel;
    }
}

/// <summary>
/// Published when a user account is deactivated and they need to be notified.
/// Triggers account deactivation notification with appeal process information.
/// </summary>
public class UserAccountDeactivatedEvent : UserEventBase
{
    /// <summary>
    /// The reason for account deactivation.
    /// </summary>
    public string DeactivationReason { get; }
    
    /// <summary>
    /// The administrator who deactivated the account.
    /// </summary>
    public int DeactivatedByAdminId { get; }
    
    /// <summary>
    /// The administrator's name for reference.
    /// </summary>
    public string DeactivatedByAdminName { get; }
    
    /// <summary>
    /// Whether the deactivation is temporary or permanent.
    /// </summary>
    public bool IsTemporary { get; }
    
    /// <summary>
    /// The reactivation date if temporary (null if permanent).
    /// </summary>
    public DateTime? ReactivationDate { get; }
    
    /// <summary>
    /// Contact information for appeals or questions.
    /// </summary>
    public string AppealContact { get; }
    
    /// <summary>
    /// Additional compliance notes for the deactivation.
    /// </summary>
    public string? ComplianceNotes { get; }

    public UserAccountDeactivatedEvent(
        int targetUserId,
        string userEmail,
        string userFullName,
        string deactivationReason,
        int deactivatedByAdminId,
        string deactivatedByAdminName,
        bool isTemporary,
        DateTime? reactivationDate,
        string appealContact,
        string? complianceNotes = null,
        string? correlationId = null)
        : base(targetUserId, userEmail, userFullName, deactivatedByAdminId, correlationId)
    {
        DeactivationReason = deactivationReason ?? throw new ArgumentNullException(nameof(deactivationReason));
        DeactivatedByAdminId = deactivatedByAdminId;
        DeactivatedByAdminName = deactivatedByAdminName ?? throw new ArgumentNullException(nameof(deactivatedByAdminName));
        IsTemporary = isTemporary;
        ReactivationDate = reactivationDate;
        AppealContact = appealContact ?? throw new ArgumentNullException(nameof(appealContact));
        ComplianceNotes = complianceNotes;
        
        // Add metadata for template rendering
        Metadata["DeactivationReason"] = deactivationReason;
        Metadata["DeactivatedByAdminName"] = deactivatedByAdminName;
        Metadata["IsTemporary"] = isTemporary;
        Metadata["ReactivationDate"] = reactivationDate?.ToString("yyyy-MM-dd") ?? "Not specified";
        Metadata["AppealContact"] = appealContact;
        Metadata["ComplianceNotes"] = complianceNotes ?? "";
    }
}

/// <summary>
/// Published when a user successfully logs in and security notifications are enabled.
/// Triggers login notification for security monitoring and access logging.
/// </summary>
public class UserLoginEvent : UserEventBase
{
    /// <summary>
    /// The IP address from which the login occurred.
    /// </summary>
    public string LoginIpAddress { get; }
    
    /// <summary>
    /// The IP address (alternative property name for compatibility).
    /// </summary>
    public string IpAddress => LoginIpAddress;
    
    /// <summary>
    /// The user agent (browser/device) used for login.
    /// </summary>
    public string UserAgent { get; }
    
    /// <summary>
    /// The geographic location of the login (if available).
    /// </summary>
    public string? LoginLocation { get; }
    
    /// <summary>
    /// The location (alternative property name for compatibility).
    /// </summary>
    public string? Location => LoginLocation;
    
    /// <summary>
    /// Whether this login appears suspicious based on patterns.
    /// </summary>
    public bool IsSuspiciousLogin { get; }
    
    /// <summary>
    /// Whether this login appears suspicious (alternative property name for compatibility).
    /// </summary>
    public bool IsSuspiciousActivity => IsSuspiciousLogin;
    
    /// <summary>
    /// Whether this is the user's first login.
    /// </summary>
    public bool IsFirstTimeLogin { get; }
    
    /// <summary>
    /// Whether this login is from a new device.
    /// </summary>
    public bool IsNewDevice { get; }
    
    /// <summary>
    /// The type of device used for login.
    /// </summary>
    public string DeviceType { get; }
    
    /// <summary>
    /// The timestamp of this login.
    /// </summary>
    public DateTime LoginTimestamp => CreatedAt;
    
    /// <summary>
    /// The duration of the login session.
    /// </summary>
    public TimeSpan? SessionDuration { get; }
    
    /// <summary>
    /// Previous login timestamp for comparison.
    /// </summary>
    public DateTime? PreviousLoginAt { get; }

    public UserLoginEvent(
        int targetUserId,
        string userEmail,
        string userFullName,
        string loginIpAddress,
        string userAgent,
        string deviceType,
        string? loginLocation = null,
        bool isSuspiciousLogin = false,
        bool isFirstTimeLogin = false,
        bool isNewDevice = false,
        TimeSpan? sessionDuration = null,
        DateTime? previousLoginAt = null,
        string? correlationId = null)
        : base(targetUserId, userEmail, userFullName, targetUserId, correlationId)
    {
        LoginIpAddress = loginIpAddress ?? throw new ArgumentNullException(nameof(loginIpAddress));
        UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
        DeviceType = deviceType ?? throw new ArgumentNullException(nameof(deviceType));
        LoginLocation = loginLocation;
        IsSuspiciousLogin = isSuspiciousLogin;
        IsFirstTimeLogin = isFirstTimeLogin;
        IsNewDevice = isNewDevice;
        SessionDuration = sessionDuration;
        PreviousLoginAt = previousLoginAt;
        
        // Add metadata for template rendering
        Metadata["LoginIpAddress"] = loginIpAddress;
        Metadata["UserAgent"] = userAgent;
        Metadata["DeviceType"] = deviceType;
        Metadata["LoginLocation"] = loginLocation ?? "Unknown";
        Metadata["IsSuspiciousLogin"] = isSuspiciousLogin;
        Metadata["IsFirstTimeLogin"] = isFirstTimeLogin;
        Metadata["IsNewDevice"] = isNewDevice;
        Metadata["SessionDuration"] = sessionDuration?.ToString() ?? "Unknown";
        Metadata["PreviousLoginAt"] = previousLoginAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "First login";
        Metadata["LoginTime"] = CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

/// <summary>
/// Published when a user's password is reset and they need confirmation.
/// Triggers password reset confirmation with security recommendations.
/// </summary>
public class UserPasswordResetEvent : UserEventBase
{
    /// <summary>
    /// Whether the password reset was initiated by the user or an administrator.
    /// </summary>
    public bool IsUserInitiated { get; }
    
    /// <summary>
    /// Whether the password reset was initiated by the user (alternative property name for compatibility).
    /// </summary>
    public bool InitiatedByUser => IsUserInitiated;
    
    /// <summary>
    /// The method used for password reset (email, SMS, admin).
    /// </summary>
    public string ResetMethod { get; }
    
    /// <summary>
    /// The reset token for password change.
    /// </summary>
    public string ResetToken { get; }
    
    /// <summary>
    /// The timestamp when the reset was initiated.
    /// </summary>
    public DateTime ResetTimestamp => CreatedAt;
    
    /// <summary>
    /// The expiry time for the reset token.
    /// </summary>
    public DateTime TokenExpiryTime { get; }
    
    /// <summary>
    /// The IP address from which the reset was initiated.
    /// </summary>
    public string RequestIpAddress { get; }
    
    /// <summary>
    /// The user agent for the reset request.
    /// </summary>
    public string RequestUserAgent { get; }
    
    /// <summary>
    /// The administrator who reset the password (if admin-initiated).
    /// </summary>
    public int? ResetByAdminId { get; }
    
    /// <summary>
    /// The administrator's name (if admin-initiated).
    /// </summary>
    public string? ResetByAdminName { get; }
    
    /// <summary>
    /// Whether the user should be forced to change password on next login.
    /// </summary>
    public bool RequirePasswordChangeOnLogin { get; }

    public UserPasswordResetEvent(
        int targetUserId,
        string userEmail,
        string userFullName,
        bool isUserInitiated,
        string resetMethod,
        string resetToken,
        DateTime tokenExpiryTime,
        string requestIpAddress,
        string requestUserAgent,
        bool requirePasswordChangeOnLogin = true,
        int? resetByAdminId = null,
        string? resetByAdminName = null,
        string? correlationId = null)
        : base(targetUserId, userEmail, userFullName, resetByAdminId ?? targetUserId, correlationId)
    {
        IsUserInitiated = isUserInitiated;
        ResetMethod = resetMethod ?? throw new ArgumentNullException(nameof(resetMethod));
        ResetToken = resetToken ?? throw new ArgumentNullException(nameof(resetToken));
        TokenExpiryTime = tokenExpiryTime;
        RequestIpAddress = requestIpAddress ?? throw new ArgumentNullException(nameof(requestIpAddress));
        RequestUserAgent = requestUserAgent ?? throw new ArgumentNullException(nameof(requestUserAgent));
        ResetByAdminId = resetByAdminId;
        ResetByAdminName = resetByAdminName;
        RequirePasswordChangeOnLogin = requirePasswordChangeOnLogin;
        
        // Add metadata for template rendering
        Metadata["IsUserInitiated"] = isUserInitiated;
        Metadata["ResetMethod"] = resetMethod;
        Metadata["ResetToken"] = resetToken;
        Metadata["TokenExpiryTime"] = tokenExpiryTime.ToString("yyyy-MM-dd HH:mm:ss");
        Metadata["RequestIpAddress"] = requestIpAddress;
        Metadata["RequestUserAgent"] = requestUserAgent;
        Metadata["ResetByAdminName"] = resetByAdminName ?? "Self-service";
        Metadata["RequirePasswordChangeOnLogin"] = requirePasswordChangeOnLogin;
        Metadata["ResetTime"] = CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
