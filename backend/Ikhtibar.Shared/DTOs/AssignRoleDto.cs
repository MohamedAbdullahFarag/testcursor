using System.ComponentModel.DataAnnotations;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Data transfer object for role assignment operations
/// </summary>
public class AssignRoleDto
{
    /// <summary>
    /// User identifier
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Role identifier
    /// </summary>
    [Required]
    public int RoleId { get; set; }
}
