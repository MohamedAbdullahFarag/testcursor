using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Roles;

/// <summary>
/// DTO for assigning a role to a user
/// </summary>
public class UserRoleAssignmentDto
{
    /// <summary>
    /// The user ID to assign the role to
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// The role ID to assign
    /// </summary>
    [Required]
    public int RoleId { get; set; }

    /// <summary>
    /// Optional expiration date for the role assignment
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Optional reason for the assignment
    /// </summary>
    [StringLength(500)]
    public string? AssignmentReason { get; set; }

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
/// DTO for bulk user role assignments
/// </summary>
public class BulkUserRoleAssignmentDto
{
    /// <summary>
    /// Collection of user-role assignments
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "At least one assignment is required")]
    public IEnumerable<UserRoleAssignmentDto> Assignments { get; set; } = new List<UserRoleAssignmentDto>();

    /// <summary>
    /// Whether to remove existing role assignments before adding new ones
    /// </summary>
    public bool ReplaceExisting { get; set; } = false;

    /// <summary>
    /// Optional reason for the bulk assignment
    /// </summary>
    [StringLength(500)]
    public string? BulkAssignmentReason { get; set; }
}
