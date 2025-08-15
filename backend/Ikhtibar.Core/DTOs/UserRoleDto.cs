using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Core.DTOs;

/// <summary>
/// DTO for removing a role from a user
/// </summary>
public class RemoveRoleDto
{
    /// <summary>
    /// User ID
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Role ID
    /// </summary>
    [Required]
    public int RoleId { get; set; }
}
