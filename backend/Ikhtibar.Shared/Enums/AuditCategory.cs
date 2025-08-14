namespace Ikhtibar.Shared.Enums;

/// <summary>
/// Defines categories for audit log entries
/// Used for organization and filtering of audit events
/// </summary>
public enum AuditCategory
{
    /// <summary>
    /// Authentication events related to user identity verification
    /// Examples: Login attempts, password resets, multi-factor authentication
    /// </summary>
    Authentication = 0,
    
    /// <summary>
    /// Authorization events related to access control
    /// Examples: Permission changes, access denials, role assignments
    /// </summary>
    Authorization = 1,
    
    /// <summary>
    /// User management operations
    /// Examples: User creation, profile updates, account status changes
    /// </summary>
    UserManagement = 2,
    
    /// <summary>
    /// Data access and modification events
    /// Examples: CRUD operations on entities, exports, imports
    /// </summary>
    DataAccess = 3,
    
    /// <summary>
    /// System administration events
    /// Examples: Configuration changes, system maintenance, service operations
    /// </summary>
    System = 4,
    
    /// <summary>
    /// Security-related events
    /// Examples: Suspicious activities, integrity violations, security policy changes
    /// </summary>
    Security = 5
}
