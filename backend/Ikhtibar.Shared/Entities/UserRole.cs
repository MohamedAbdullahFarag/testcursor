using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Junction entity for Users and Roles many-to-many relationship
/// </summary>
[Table("UserRoles")]
public class UserRole
{
    /// <summary>
    /// Foreign key to Users
    /// </summary>
    [Key]
    [Column(Order = 0)]
    public int UserId { get; set; }

    /// <summary>
    /// Foreign key to Roles
    /// </summary>
    [Key]
    [Column(Order = 1)]
    public int RoleId { get; set; }

    /// <summary>
    /// When the role was assigned
    /// </summary>
    [Required]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User ID who assigned the role
    /// </summary>
    public int? AssignedBy { get; set; }

    /// <summary>
    /// Navigation property to User
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Navigation property to Role
    /// </summary>
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;

    /// <summary>
    /// Navigation property to User who assigned the role
    /// </summary>
    [ForeignKey("AssignedBy")]
    public virtual User? AssignedByUser { get; set; }
}
