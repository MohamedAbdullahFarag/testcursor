using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Core.DTOs;

/// <summary>
/// DTO for assigning roles to users
/// Following SRP: ONLY role assignment data
/// </summary>
public class AssignRoleDto
{
    /// <summary>
    /// User ID to assign the role to
    /// </summary>
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    /// <summary>
    /// Role ID to assign
    /// </summary>
    [Required(ErrorMessage = "Role ID is required")]
    public int RoleId { get; set; }
}
