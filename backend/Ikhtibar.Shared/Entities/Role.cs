using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Role entity representing system roles
/// </summary>
[Table("Roles")]
public class Role : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int RoleId { get; set; }

    /// <summary>
    /// System role code
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Role display name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Role description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Built-in role flag
    /// </summary>
    [Required]
    public bool IsSystemRole { get; set; } = false;

    /// <summary>
    /// Whether the role is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property for role permissions
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    /// <summary>
    /// Navigation property for user roles
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
