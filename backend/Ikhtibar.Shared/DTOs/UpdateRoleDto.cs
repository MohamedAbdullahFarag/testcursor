using System.ComponentModel.DataAnnotations;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Data transfer object for updating an existing role
/// </summary>
public class UpdateRoleDto
{
    /// <summary>
    /// Human-readable role name
    /// </summary>
    [StringLength(100, MinimumLength = 2)]
    public string? Name { get; set; }

    /// <summary>
    /// Optional role description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Flag if role should be active
    /// </summary>
    public bool? IsActive { get; set; }
}
