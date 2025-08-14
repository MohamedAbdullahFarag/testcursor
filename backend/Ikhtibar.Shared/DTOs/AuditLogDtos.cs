using System.Text.Json.Serialization;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Data Transfer Object for audit log entries
/// Used for API responses and frontend display
/// </summary>
public class AuditLogDto
{
    /// <summary>
    /// Unique identifier for the audit log
    /// </summary>
    public int AuditLogId { get; set; }
    
    /// <summary>
    /// User ID who performed the action (null for system actions)
    /// </summary>
    public int? UserId { get; set; }
    
    /// <summary>
    /// User identifier (username or email) who performed the action
    /// </summary>
    public string UserIdentifier { get; set; } = string.Empty;
    
    /// <summary>
    /// The action that was performed
    /// </summary>
    public string Action { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of entity affected
    /// </summary>
    public string EntityType { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the affected entity
    /// </summary>
    public string? EntityId { get; set; }
    
    /// <summary>
    /// Additional context details in JSON format
    /// </summary>
    public string? Details { get; set; }
    
    /// <summary>
    /// Previous values before change in JSON format
    /// </summary>
    public string? OldValues { get; set; }
    
    /// <summary>
    /// New values after change in JSON format
    /// </summary>
    public string? NewValues { get; set; }
    
    /// <summary>
    /// IP address of client
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// Browser/application user agent
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Session identifier
    /// </summary>
    public string? SessionId { get; set; }
    
    /// <summary>
    /// Severity level of the audit event
    /// </summary>
    public AuditSeverity Severity { get; set; }
    
    /// <summary>
    /// Category of the audit event
    /// </summary>
    public AuditCategory Category { get; set; }
    
    /// <summary>
    /// When the action occurred
    /// </summary>
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// Whether this was a system-generated action
    /// </summary>
    public bool IsSystemAction { get; set; }
    
    /// <summary>
    /// Human-readable severity text
    /// </summary>
    [JsonIgnore]
    public string SeverityText => Severity.ToString();
    
    /// <summary>
    /// Human-readable category text
    /// </summary>
    [JsonIgnore]
    public string CategoryText => Category.ToString();
}

/// <summary>
/// Filter criteria for retrieving audit logs
/// </summary>
public class AuditLogFilter
{
    /// <summary>
    /// Filter by specific user ID
    /// </summary>
    public int? UserId { get; set; }
    
    /// <summary>
    /// Filter by username or email
    /// </summary>
    public string? UserIdentifier { get; set; }
    
    /// <summary>
    /// Filter by action type
    /// </summary>
    public string? Action { get; set; }
    
    /// <summary>
    /// Filter by entity type
    /// </summary>
    public string? EntityType { get; set; }
    
    /// <summary>
    /// Filter by entity ID
    /// </summary>
    public string? EntityId { get; set; }
    
    /// <summary>
    /// Filter by severity level
    /// </summary>
    public AuditSeverity? Severity { get; set; }
    
    /// <summary>
    /// Filter by category
    /// </summary>
    public AuditCategory? Category { get; set; }
    
    /// <summary>
    /// Filter from this date/time
    /// </summary>
    public DateTime? FromDate { get; set; }
    
    /// <summary>
    /// Filter to this date/time
    /// </summary>
    public DateTime? ToDate { get; set; }
    
    /// <summary>
    /// Filter by IP address
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// Include system actions
    /// </summary>
    public bool? IncludeSystemActions { get; set; }
    
    /// <summary>
    /// Text search across all fields
    /// </summary>
    public string? SearchText { get; set; }
    
    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 20;
    
    /// <summary>
    /// Sort field
    /// </summary>
    public string SortBy { get; set; } = "Timestamp";
    
    /// <summary>
    /// Sort direction
    /// </summary>
    public string SortDirection { get; set; } = "desc";
}

/// <summary>
/// Input model for creating audit log entries
/// </summary>
public class AuditLogEntry
{
    /// <summary>
    /// User ID who performed the action (null for system actions)
    /// </summary>
    public int? UserId { get; set; }
    
    /// <summary>
    /// User identifier (username or email) who performed the action
    /// </summary>
    public string UserIdentifier { get; set; } = string.Empty;
    
    /// <summary>
    /// The action that was performed
    /// </summary>
    public string Action { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of entity affected
    /// </summary>
    public string EntityType { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the affected entity
    /// </summary>
    public string? EntityId { get; set; }
    
    /// <summary>
    /// Additional context details
    /// </summary>
    public string? Details { get; set; }
    
    /// <summary>
    /// Previous state in object format
    /// </summary>
    public object? OldValues { get; set; }
    
    /// <summary>
    /// New state in object format
    /// </summary>
    public object? NewValues { get; set; }
    
    /// <summary>
    /// Severity level of the audit event
    /// </summary>
    public AuditSeverity Severity { get; set; } = AuditSeverity.Medium;
    
    /// <summary>
    /// Category of the audit event
    /// </summary>
    public AuditCategory Category { get; set; } = AuditCategory.System;
    
    /// <summary>
    /// Whether this was a system-generated action
    /// </summary>
    public bool IsSystemAction { get; set; } = false;
}

/// <summary>
/// Format options for exporting audit logs
/// </summary>
public enum AuditLogExportFormat
{
    /// <summary>
    /// Export as CSV file
    /// </summary>
    CSV,
    
    /// <summary>
    /// Export as Excel file
    /// </summary>
    Excel,
    
    /// <summary>
    /// Export as JSON file
    /// </summary>
    Json
}
