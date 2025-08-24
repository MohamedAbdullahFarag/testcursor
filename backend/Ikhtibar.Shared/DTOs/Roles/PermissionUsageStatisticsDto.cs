using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Roles;

/// <summary>
/// DTO representing permission usage statistics
/// </summary>
public class PermissionUsageStatisticsDto
{
    /// <summary>
    /// The permission ID
    /// </summary>
    [Required]
    public int PermissionId { get; set; }

    /// <summary>
    /// The permission name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string PermissionName { get; set; } = string.Empty;

    /// <summary>
    /// The permission code
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PermissionCode { get; set; } = string.Empty;

    /// <summary>
    /// The permission category
    /// </summary>
    [StringLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Total number of roles that have this permission
    /// </summary>
    [Required]
    public int TotalRoles { get; set; }

    /// <summary>
    /// Number of active roles with this permission
    /// </summary>
    [Required]
    public int ActiveRoles { get; set; }

    /// <summary>
    /// Number of inactive roles with this permission
    /// </summary>
    [Required]
    public int InactiveRoles { get; set; }

    /// <summary>
    /// Total number of users that have this permission through their roles
    /// </summary>
    [Required]
    public int TotalUsers { get; set; }

    /// <summary>
    /// Number of active users with this permission
    /// </summary>
    [Required]
    public int ActiveUsers { get; set; }

    /// <summary>
    /// Number of inactive users with this permission
    /// </summary>
    [Required]
    public int InactiveUsers { get; set; }

    /// <summary>
    /// When the permission was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the permission was last modified
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }

    /// <summary>
    /// When the permission was last accessed
    /// </summary>
    public DateTime? LastAccessedAt { get; set; }

    /// <summary>
    /// Number of times the permission was accessed in the last 30 days
    /// </summary>
    [Required]
    public int AccessCountLast30Days { get; set; }

    /// <summary>
    /// Number of times the permission was accessed in the last 7 days
    /// </summary>
    [Required]
    public int AccessCountLast7Days { get; set; }

    /// <summary>
    /// Number of times the permission was accessed today
    /// </summary>
    [Required]
    public int AccessCountToday { get; set; }

    /// <summary>
    /// Whether the permission is system-defined
    /// </summary>
    [Required]
    public bool IsSystemPermission { get; set; }

    /// <summary>
    /// Whether the permission is currently active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    /// <summary>
    /// Risk level associated with this permission
    /// </summary>
    [Required]
    public PermissionRiskLevel RiskLevel { get; set; } = PermissionRiskLevel.Low;

    /// <summary>
    /// Whether this permission requires approval
    /// </summary>
    [Required]
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// Whether this permission is audited
    /// </summary>
    [Required]
    public bool IsAudited { get; set; }

    /// <summary>
    /// Last audit date for this permission
    /// </summary>
    public DateTime? LastAuditDate { get; set; }

    /// <summary>
    /// Next scheduled audit date
    /// </summary>
    public DateTime? NextAuditDate { get; set; }

    /// <summary>
    /// Additional metadata about the permission usage
    /// </summary>
    [StringLength(2000)]
    public string? Metadata { get; set; }
}

/// <summary>
/// Risk levels for permissions
/// </summary>
public enum PermissionRiskLevel
{
    /// <summary>
    /// Low risk permission
    /// </summary>
    Low = 1,

    /// <summary>
    /// Medium risk permission
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High risk permission
    /// </summary>
    High = 3,

    /// <summary>
    /// Critical risk permission
    /// </summary>
    Critical = 4
}
