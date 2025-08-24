using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Roles;

/// <summary>
/// Data transfer object for updating an existing role.
/// </summary>
public class UpdateRoleDto
{
    /// <summary>
    /// ID of the role to update.
    /// </summary>
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// Name of the role.
    /// </summary>
    [StringLength(100, ErrorMessage = "Role name cannot exceed 100 characters")]
    public string? Name { get; set; }

    /// <summary>
    /// Description of the role.
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Whether the role is active/enabled.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Whether the role is visible to users.
    /// </summary>
    public bool? IsVisible { get; set; }

    /// <summary>
    /// Priority level of the role (higher number = higher priority).
    /// </summary>
    [Range(1, 100, ErrorMessage = "Priority must be between 1 and 100")]
    public int? Priority { get; set; }

    /// <summary>
    /// Maximum number of users that can have this role.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maximum users must be non-negative")]
    public int? MaxUsers { get; set; }

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Who is updating the role.
    /// </summary>
    [StringLength(100, ErrorMessage = "Modified by cannot exceed 100 characters")]
    public string? ModifiedBy { get; set; }
}
