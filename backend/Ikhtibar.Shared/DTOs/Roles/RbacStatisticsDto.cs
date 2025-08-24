using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Roles;

/// <summary>
/// DTO representing comprehensive RBAC system statistics
/// </summary>
public class RbacStatisticsDto
{
    /// <summary>
    /// When these statistics were generated
    /// </summary>
    [Required]
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Time period these statistics cover
    /// </summary>
    [Required]
    public StatisticsTimePeriod TimePeriod { get; set; } = StatisticsTimePeriod.Last30Days;

    #region User Statistics

    /// <summary>
    /// Total number of users in the system
    /// </summary>
    [Required]
    public int TotalUsers { get; set; }

    /// <summary>
    /// Number of active users
    /// </summary>
    [Required]
    public int ActiveUsers { get; set; }

    /// <summary>
    /// Number of inactive users
    /// </summary>
    [Required]
    public int InactiveUsers { get; set; }

    /// <summary>
    /// Number of users with no roles assigned
    /// </summary>
    [Required]
    public int UsersWithNoRoles { get; set; }

    /// <summary>
    /// Number of users with multiple roles
    /// </summary>
    [Required]
    public int UsersWithMultipleRoles { get; set; }

    /// <summary>
    /// Average number of roles per user
    /// </summary>
    [Required]
    public double AverageRolesPerUser { get; set; }

    /// <summary>
    /// Maximum number of roles assigned to a single user
    /// </summary>
    [Required]
    public int MaxRolesPerUser { get; set; }

    #endregion

    #region Role Statistics

    /// <summary>
    /// Total number of roles in the system
    /// </summary>
    [Required]
    public int TotalRoles { get; set; }

    /// <summary>
    /// Number of active roles
    /// </summary>
    [Required]
    public int ActiveRoles { get; set; }

    /// <summary>
    /// Number of inactive roles
    /// </summary>
    [Required]
    public int InactiveRoles { get; set; }

    /// <summary>
    /// Number of system-defined roles
    /// </summary>
    [Required]
    public int SystemRoles { get; set; }

    /// <summary>
    /// Number of user-defined roles
    /// </summary>
    [Required]
    public int UserDefinedRoles { get; set; }

    /// <summary>
    /// Number of roles with no users assigned
    /// </summary>
    [Required]
    public int UnusedRoles { get; set; }

    /// <summary>
    /// Number of roles with no permissions assigned
    /// </summary>
    [Required]
    public int RolesWithNoPermissions { get; set; }

    /// <summary>
    /// Average number of permissions per role
    /// </summary>
    [Required]
    public double AveragePermissionsPerRole { get; set; }

    /// <summary>
    /// Maximum number of permissions assigned to a single role
    /// </summary>
    [Required]
    public int MaxPermissionsPerRole { get; set; }

    #endregion

    #region Permission Statistics

    /// <summary>
    /// Total number of permissions in the system
    /// </summary>
    [Required]
    public int TotalPermissions { get; set; }

    /// <summary>
    /// Number of active permissions
    /// </summary>
    [Required]
    public int ActivePermissions { get; set; }

    /// <summary>
    /// Number of inactive permissions
    /// </summary>
    [Required]
    public int InactivePermissions { get; set; }

    /// <summary>
    /// Number of system-defined permissions
    /// </summary>
    [Required]
    public int SystemPermissions { get; set; }

    /// <summary>
    /// Number of user-defined permissions
    /// </summary>
    [Required]
    public int UserDefinedPermissions { get; set; }

    /// <summary>
    /// Number of permissions not assigned to any role
    /// </summary>
    [Required]
    public int UnusedPermissions { get; set; }

    /// <summary>
    /// Number of high-risk permissions
    /// </summary>
    [Required]
    public int HighRiskPermissions { get; set; }

    /// <summary>
    /// Number of critical-risk permissions
    /// </summary>
    [Required]
    public int CriticalRiskPermissions { get; set; }

    #endregion

    #region Assignment Statistics

    /// <summary>
    /// Total number of user-role assignments
    /// </summary>
    [Required]
    public int TotalUserRoleAssignments { get; set; }

    /// <summary>
    /// Total number of role-permission assignments
    /// </summary>
    [Required]
    public int TotalRolePermissionAssignments { get; set; }

    /// <summary>
    /// Number of active user-role assignments
    /// </summary>
    [Required]
    public int ActiveUserRoleAssignments { get; set; }

    /// <summary>
    /// Number of active role-permission assignments
    /// </summary>
    [Required]
    public int ActiveRolePermissionAssignments { get; set; }

    /// <summary>
    /// Number of expired user-role assignments
    /// </summary>
    [Required]
    public int ExpiredUserRoleAssignments { get; set; }

    #endregion

    #region Security Statistics

    /// <summary>
    /// Number of permission conflicts detected
    /// </summary>
    [Required]
    public int PermissionConflicts { get; set; }

    /// <summary>
    /// Number of high-risk role assignments
    /// </summary>
    [Required]
    public int HighRiskRoleAssignments { get; set; }

    /// <summary>
    /// Number of critical-risk role assignments
    /// </summary>
    [Required]
    public int CriticalRiskRoleAssignments { get; set; }

    /// <summary>
    /// Number of roles requiring approval
    /// </summary>
    [Required]
    public int RolesRequiringApproval { get; set; }

    /// <summary>
    /// Number of permissions requiring approval
    /// </summary>
    [Required]
    public int PermissionsRequiringApproval { get; set; }

    /// <summary>
    /// Number of audited roles
    /// </summary>
    [Required]
    public int AuditedRoles { get; set; }

    /// <summary>
    /// Number of audited permissions
    /// </summary>
    [Required]
    public int AuditedPermissions { get; set; }

    #endregion

    #region Usage Statistics

    /// <summary>
    /// Total number of role accesses in the time period
    /// </summary>
    [Required]
    public int TotalRoleAccesses { get; set; }

    /// <summary>
    /// Total number of permission accesses in the time period
    /// </summary>
    [Required]
    public int TotalPermissionAccesses { get; set; }

    /// <summary>
    /// Most frequently accessed role
    /// </summary>
    [StringLength(100)]
    public string? MostAccessedRole { get; set; }

    /// <summary>
    /// Most frequently accessed permission
    /// </summary>
    [StringLength(100)]
    public string? MostAccessedPermission { get; set; }

    /// <summary>
    /// Least frequently accessed role
    /// </summary>
    [StringLength(100)]
    public string? LeastAccessedRole { get; set; }

    /// <summary>
    /// Least frequently accessed permission
    /// </summary>
    [StringLength(100)]
    public string? LeastAccessedPermission { get; set; }

    #endregion

    #region Compliance Statistics

    /// <summary>
    /// Number of compliant roles
    /// </summary>
    [Required]
    public int CompliantRoles { get; set; }

    /// <summary>
    /// Number of non-compliant roles
    /// </summary>
    [Required]
    public int NonCompliantRoles { get; set; }

    /// <summary>
    /// Number of roles with minor compliance issues
    /// </summary>
    [Required]
    public int RolesWithMinorIssues { get; set; }

    /// <summary>
    /// Number of roles with major compliance issues
    /// </summary>
    [Required]
    public int RolesWithMajorIssues { get; set; }

    /// <summary>
    /// Overall compliance score (0-100)
    /// </summary>
    [Required]
    [Range(0, 100)]
    public double OverallComplianceScore { get; set; }

    #endregion

    /// <summary>
    /// Additional metadata about the statistics
    /// </summary>
    [StringLength(2000)]
    public string? Metadata { get; set; }
}

/// <summary>
/// Time periods for statistics
/// </summary>
public enum StatisticsTimePeriod
{
    /// <summary>
    /// Last 24 hours
    /// </summary>
    Last24Hours = 1,

    /// <summary>
    /// Last 7 days
    /// </summary>
    Last7Days = 2,

    /// <summary>
    /// Last 30 days
    /// </summary>
    Last30Days = 3,

    /// <summary>
    /// Last 90 days
    /// </summary>
    Last90Days = 4,

    /// <summary>
    /// Last 365 days
    /// </summary>
    Last365Days = 5,

    /// <summary>
    /// All time
    /// </summary>
    AllTime = 6
}
