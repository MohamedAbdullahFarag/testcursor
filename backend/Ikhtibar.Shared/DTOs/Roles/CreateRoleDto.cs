using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Roles;

/// <summary>
/// Data transfer object for creating a new role.
/// </summary>
public class CreateRoleDto
{
    /// <summary>
    /// Name of the role.
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "Role name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the role.
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Unique code for the role.
    /// </summary>
    [Required]
    [StringLength(50, ErrorMessage = "Role code cannot exceed 50 characters")]
    [RegularExpression(@"^[A-Z_]+$", ErrorMessage = "Role code can only contain uppercase letters and underscores")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Whether the role is active/enabled.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the role is visible to users.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Whether this role is system-defined (cannot be deleted).
    /// </summary>
    public bool IsSystem { get; set; } = false;

    /// <summary>
    /// Priority level of the role (higher number = higher priority).
    /// </summary>
    [Range(1, 100, ErrorMessage = "Priority must be between 1 and 100")]
    public int Priority { get; set; } = 50;

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
    /// Who is creating the role.
    /// </summary>
    [StringLength(100, ErrorMessage = "Created by cannot exceed 100 characters")]
    public string? CreatedBy { get; set; }
}
