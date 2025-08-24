using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Roles;

/// <summary>
/// DTO representing role usage statistics
/// </summary>
public class RoleUsageStatisticsDto
{
    /// <summary>
    /// The role ID
    /// </summary>
    [Required]
    public int RoleId { get; set; }

    /// <summary>
    /// The role name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// The role code
    /// </summary>
    [Required]
    [StringLength(50)]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// Total number of users assigned to this role
    /// </summary>
    [Required]
    public int TotalUsers { get; set; }

    /// <summary>
    /// Number of active users with this role
    /// </summary>
    [Required]
    public int ActiveUsers { get; set; }

    /// <summary>
    /// Number of inactive users with this role
    /// </summary>
    [Required]
    public int InactiveUsers { get; set; }

    /// <summary>
    /// Total number of permissions assigned to this role
    /// </summary>
    [Required]
    public int TotalPermissions { get; set; }

    /// <summary>
    /// Number of granted permissions
    /// </summary>
    [Required]
    public int GrantedPermissions { get; set; }

    /// <summary>
    /// Number of denied permissions
    /// </summary>
    [Required]
    public int DeniedPermissions { get; set; }

    /// <summary>
    /// When the role was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the role was last modified
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }

    /// <summary>
    /// When the role was last accessed
    /// </summary>
    public DateTime? LastAccessedAt { get; set; }

    /// <summary>
    /// Number of times the role was accessed in the last 30 days
    /// </summary>
    [Required]
    public int AccessCountLast30Days { get; set; }

    /// <summary>
    /// Number of times the role was accessed in the last 7 days
    /// </summary>
    [Required]
    public int AccessCountLast7Days { get; set; }

    /// <summary>
    /// Number of times the role was accessed today
    /// </summary>
    [Required]
    public int AccessCountToday { get; set; }

    /// <summary>
    /// Whether the role is system-defined
    /// </summary>
    [Required]
    public bool IsSystemRole { get; set; }

    /// <summary>
    /// Whether the role is currently active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    /// <summary>
    /// Priority level of the role (higher number = higher priority)
    /// </summary>
    [Required]
    [Range(1, 100)]
    public int Priority { get; set; } = 50;

    /// <summary>
    /// Risk level associated with this role
    /// </summary>
    [Required]
    public RoleRiskLevel RiskLevel { get; set; } = RoleRiskLevel.Low;

    /// <summary>
    /// Compliance status of the role
    /// </summary>
    [Required]
    public RoleComplianceStatus ComplianceStatus { get; set; } = RoleComplianceStatus.Compliant;

    /// <summary>
    /// Last audit date for this role
    /// </summary>
    public DateTime? LastAuditDate { get; set; }

    /// <summary>
    /// Next scheduled audit date
    /// </summary>
    public DateTime? NextAuditDate { get; set; }

    /// <summary>
    /// Additional metadata about the role usage
    /// </summary>
    [StringLength(2000)]
    public string? Metadata { get; set; }
}

/// <summary>
/// Risk levels for roles
/// </summary>
public enum RoleRiskLevel
{
    /// <summary>
    /// Low risk role
    /// </summary>
    Low = 1,

    /// <summary>
    /// Medium risk role
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High risk role
    /// </summary>
    High = 3,

    /// <summary>
    /// Critical risk role
    /// </summary>
    Critical = 4
}

/// <summary>
/// Compliance status for roles
/// </summary>
public enum RoleComplianceStatus
{
    /// <summary>
    /// Role is compliant
    /// </summary>
    Compliant = 1,

    /// <summary>
    /// Role has minor compliance issues
    /// </summary>
    MinorIssues = 2,

    /// <summary>
    /// Role has major compliance issues
    /// </summary>
    MajorIssues = 3,

    /// <summary>
    /// Role is non-compliant
    /// </summary>
    NonCompliant = 4,

    /// <summary>
    /// Compliance status is unknown
    /// </summary>
    Unknown = 5
}
