using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Roles;

/// <summary>
/// DTO for assigning a permission to a role
/// </summary>
public class RolePermissionAssignmentDto
{
    /// <summary>
    /// The role ID to assign the permission to
    /// </summary>
    [Required]
    public int RoleId { get; set; }

    /// <summary>
    /// The permission ID to assign
    /// </summary>
    [Required]
    public int PermissionId { get; set; }

    /// <summary>
    /// Whether the permission is granted (true) or denied (false)
    /// </summary>
    public bool IsGranted { get; set; } = true;

    /// <summary>
    /// Optional scope for the permission (e.g., "own", "department", "all")
    /// </summary>
    [StringLength(100)]
    public string? Scope { get; set; }

    /// <summary>
    /// Optional conditions for when this permission applies
    /// </summary>
    [StringLength(500)]
    public string? Conditions { get; set; }

    /// <summary>
    /// User who made the assignment
    /// </summary>
    [StringLength(100)]
    public string? AssignedBy { get; set; }

    /// <summary>
    /// When the assignment was made
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// DTO for bulk role permission assignments
/// </summary>
public class BulkRolePermissionAssignmentDto
{
    /// <summary>
    /// Collection of role-permission assignments
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "At least one assignment is required")]
    public IEnumerable<RolePermissionAssignmentDto> Assignments { get; set; } = new List<RolePermissionAssignmentDto>();

    /// <summary>
    /// Whether to remove existing permission assignments before adding new ones
    /// </summary>
    public bool ReplaceExisting { get; set; } = false;

    /// <summary>
    /// Optional reason for the bulk assignment
    /// </summary>
    [StringLength(500)]
    public string? BulkAssignmentReason { get; set; }
}
