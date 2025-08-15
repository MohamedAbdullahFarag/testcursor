using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Core.DTOs;

/// <summary>
/// DTO for creating a new role
/// Following SRP: ONLY role creation data
/// </summary>
public class CreateRoleDto
{
    /// <summary>
    /// Role's unique code
    /// </summary>
    [Required(ErrorMessage = "Role code is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Role code must be between 2 and 50 characters")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Role's display name
    /// </summary>
    [Required(ErrorMessage = "Role name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Role's description
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Whether the role is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this is a system role
    /// </summary>
    public bool IsSystemRole { get; set; } = false;
}
