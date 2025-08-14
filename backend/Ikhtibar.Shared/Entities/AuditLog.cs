using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Comprehensive audit log entity for tracking user and system actions
/// Provides detailed audit trail with entity tracking and security monitoring capabilities
/// </summary>
[Table("AuditLogs")]
public class AuditLog : BaseEntity
{
    /// <summary>
    /// Primary key for audit log - overrides the base Id property to use AuditLogId
    /// </summary>
    [Key]
    [NotMapped]
    public override int Id { get; set; }
    
    /// <summary>
    /// Database primary key for audit log
    /// </summary>
    public int AuditLogId { get; set; }
    
    /// <summary>
    /// Foreign key to Users table (nullable for system actions)
    /// </summary>
    public int? UserId { get; set; }
    
    /// <summary>
    /// User identifier (email/username) for auditing
    /// Stored separately to maintain audit trail even if user is deleted
    /// </summary>
    [Required]
    [StringLength(255)]
    public string UserIdentifier { get; set; } = string.Empty;
    
    /// <summary>
    /// The action that was performed
    /// Examples: CREATE_USER, UPDATE_PROFILE, DELETE_RECORD
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of entity that was affected
    /// Examples: User, Role, Question
    /// </summary>
    [Required]
    [StringLength(50)]
    public string EntityType { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the entity that was affected
    /// Can be nullable for system-wide actions
    /// </summary>
    [StringLength(50)]
    public string? EntityId { get; set; }
    
    /// <summary>
    /// JSON details of the action for additional context
    /// </summary>
    public string? Details { get; set; }
    
    /// <summary>
    /// JSON representation of previous values before changes
    /// For tracking what was changed in update operations
    /// </summary>
    public string? OldValues { get; set; }
    
    /// <summary>
    /// JSON representation of new values after changes
    /// For tracking what values were applied in update operations
    /// </summary>
    public string? NewValues { get; set; }
    
    /// <summary>
    /// Client IP address (IPv4 or IPv6)
    /// </summary>
    [StringLength(45)]
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// Browser/client user agent
    /// </summary>
    [StringLength(500)]
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Session identifier for tracking related operations
    /// </summary>
    [StringLength(100)]
    public string? SessionId { get; set; }
    
    /// <summary>
    /// Severity level for the audit event
    /// </summary>
    [Required]
    public AuditSeverity Severity { get; set; } = AuditSeverity.Medium;
    
    /// <summary>
    /// Category of the audit event for organization and filtering
    /// </summary>
    [Required]
    public AuditCategory Category { get; set; } = AuditCategory.System;
    
    /// <summary>
    /// When the action occurred (may be different from CreatedAt)
    /// </summary>
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Whether this was a system-generated action vs. user action
    /// </summary>
    [Required]
    public bool IsSystemAction { get; set; } = false;
    
    /// <summary>
    /// Navigation property to User
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}
