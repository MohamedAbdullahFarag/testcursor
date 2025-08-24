using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Roles;

/// <summary>
/// DTO representing a permission conflict in role assignments
/// </summary>
public class PermissionConflictDto
{
    /// <summary>
    /// The permission ID that has a conflict
    /// </summary>
    [Required]
    public int PermissionId { get; set; }

    /// <summary>
    /// The name of the permission
    /// </summary>
    [Required]
    [StringLength(100)]
    public string PermissionName { get; set; } = string.Empty;

    /// <summary>
    /// The code of the permission
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PermissionCode { get; set; } = string.Empty;

    /// <summary>
    /// The type of conflict
    /// </summary>
    [Required]
    public PermissionConflictType ConflictType { get; set; }

    /// <summary>
    /// Description of the conflict
    /// </summary>
    [Required]
    [StringLength(500)]
    public string ConflictDescription { get; set; } = string.Empty;

    /// <summary>
    /// The role ID where the conflict occurs
    /// </summary>
    [Required]
    public int RoleId { get; set; }

    /// <summary>
    /// The name of the role
    /// </summary>
    [Required]
    [StringLength(100)]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// Additional details about the conflict
    /// </summary>
    [StringLength(1000)]
    public string? AdditionalDetails { get; set; }

    /// <summary>
    /// Suggested resolution for the conflict
    /// </summary>
    [StringLength(500)]
    public string? SuggestedResolution { get; set; }

    /// <summary>
    /// Severity level of the conflict
    /// </summary>
    [Required]
    public ConflictSeverity Severity { get; set; } = ConflictSeverity.Warning;

    /// <summary>
    /// When the conflict was detected
    /// </summary>
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Types of permission conflicts
/// </summary>
public enum PermissionConflictType
{
    /// <summary>
    /// Permission is both granted and denied
    /// </summary>
    GrantDenyConflict,

    /// <summary>
    /// Permission conflicts with another permission
    /// </summary>
    PermissionConflict,

    /// <summary>
    /// Permission conflicts with role hierarchy
    /// </summary>
    HierarchyConflict,

    /// <summary>
    /// Permission conflicts with scope restrictions
    /// </summary>
    ScopeConflict,

    /// <summary>
    /// Permission conflicts with time-based restrictions
    /// </summary>
    TimeConflict,

    /// <summary>
    /// Permission conflicts with conditional logic
    /// </summary>
    ConditionalConflict,

    /// <summary>
    /// Unknown conflict type
    /// </summary>
    Unknown
}

/// <summary>
/// Severity levels for permission conflicts
/// </summary>
public enum ConflictSeverity
{
    /// <summary>
    /// Low severity - informational only
    /// </summary>
    Info = 0,

    /// <summary>
    /// Warning - should be reviewed
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error - must be resolved
    /// </summary>
    Error = 2,

    /// <summary>
    /// Critical - system security issue
    /// </summary>
    Critical = 3
}
