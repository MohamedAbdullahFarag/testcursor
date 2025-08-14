namespace Ikhtibar.Shared.Enums;

/// <summary>
/// Defines the severity level for audit log entries
/// Used for filtering and prioritization of audit events
/// </summary>
public enum AuditSeverity
{
    /// <summary>
    /// Critical security or system integrity events
    /// Examples: Security breaches, authentication bypass attempts, data corruption
    /// </summary>
    Critical = 0,
    
    /// <summary>
    /// High importance events that require attention
    /// Examples: Failed login attempts, permission changes, user lockouts
    /// </summary>
    High = 1,
    
    /// <summary>
    /// Medium importance events for regular monitoring
    /// Examples: User management operations, configuration changes
    /// </summary>
    Medium = 2,
    
    /// <summary>
    /// Low importance events for routine operations
    /// Examples: Successful logins, regular user activities, read operations
    /// </summary>
    Low = 3
}
