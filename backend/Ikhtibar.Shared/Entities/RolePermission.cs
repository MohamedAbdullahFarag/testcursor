using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Junction entity for Roles and Permissions many-to-many relationship
/// </summary>
[Table("RolePermissions")]
public class RolePermission
{
    /// <summary>
    /// Foreign key to Roles
    /// </summary>
    [Key]
    [Column(Order = 0)]
    public int RoleId { get; set; }

    /// <summary>
    /// Foreign key to Permissions
    /// </summary>
    [Key]
    [Column(Order = 1)]
    public int PermissionId { get; set; }

    /// <summary>
    /// When the permission was assigned
    /// </summary>
    [Required]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User ID who assigned the permission
    /// </summary>
    public int? AssignedBy { get; set; }

    /// <summary>
    /// Navigation property to Role
    /// </summary>
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;

    /// <summary>
    /// Navigation property to Permission
    /// </summary>
    [ForeignKey("PermissionId")]
    public virtual Permission Permission { get; set; } = null!;

    /// <summary>
    /// Navigation property to User who assigned the permission
    /// </summary>
    [ForeignKey("AssignedBy")]
    public virtual User? AssignedByUser { get; set; }
}
