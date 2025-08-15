using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Core.DTOs;

/// <summary>
/// DTO for updating an existing role
/// Following SRP: ONLY role update data
/// </summary>
public class UpdateRoleDto
{
    /// <summary>
    /// Role's display name
    /// </summary>
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 100 characters")]
    public string? Name { get; set; }

    /// <summary>
    /// Role's description
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Whether the role is active
    /// </summary>
    public bool? IsActive { get; set; }
}
