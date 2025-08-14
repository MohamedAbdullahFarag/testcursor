using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Permission entity representing system permissions
/// </summary>
[Table("Permissions")]
public class Permission : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int PermissionId { get; set; }

    /// <summary>
    /// Permission code
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Permission name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Permission description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Permission category
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property for role permissions
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
