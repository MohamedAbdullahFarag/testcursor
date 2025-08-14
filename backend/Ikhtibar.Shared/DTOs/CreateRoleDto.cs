using System.ComponentModel.DataAnnotations;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Data transfer object for creating a new role
/// </summary>
public class CreateRoleDto
{
    /// <summary>
    /// Unique role identifier
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable role name
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional role description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Flag indicating if this is a system role (non-deletable)
    /// </summary>
    public bool IsSystemRole { get; set; } = false;

    /// <summary>
    /// Flag if role should be active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
