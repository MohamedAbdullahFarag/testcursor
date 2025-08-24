using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Represents a type/category for tree nodes.
/// Provides classification and validation for different node types.
/// </summary>
[Table("TreeNodeTypes")]
public class TreeNodeType
{
    /// <summary>
    /// Unique identifier for the tree node type.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Name of the tree node type.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for the tree node type.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Description of the tree node type.
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Icon or visual representation for the type.
    /// </summary>
    [StringLength(100)]
    public string? Icon { get; set; }

    /// <summary>
    /// Color theme for the type.
    /// </summary>
    [StringLength(50)]
    public string? Color { get; set; }

    /// <summary>
    /// Whether this type allows children.
    /// </summary>
    [Required]
    public bool AllowsChildren { get; set; } = true;

    /// <summary>
    /// Maximum number of children allowed.
    /// </summary>
    public int? MaxChildren { get; set; }

    /// <summary>
    /// Maximum depth allowed for this type.
    /// </summary>
    public int? MaxDepth { get; set; }

    /// <summary>
    /// Whether this type is system-defined (cannot be deleted).
    /// </summary>
    [Required]
    public bool IsSystem { get; set; } = false;

    /// <summary>
    /// Whether this type is active/enabled.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this type is visible to users.
    /// </summary>
    [Required]
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Metadata { get; set; }

    /// <summary>
    /// When the type was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Who created the type.
    /// </summary>
    [StringLength(100)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// When the type was last modified.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Who last modified the type.
    /// </summary>
    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Version number for optimistic concurrency.
    /// </summary>
    [Required]
    public int Version { get; set; } = 1;

    /// <summary>
    /// Soft delete flag.
    /// </summary>
    [Required]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// When the type was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Who soft deleted the type.
    /// </summary>
    [StringLength(100)]
    public string? DeletedBy { get; set; }

    /// <summary>
    /// Collection of tree nodes of this type.
    /// </summary>
    public virtual ICollection<TreeNode> TreeNodes { get; set; } = new List<TreeNode>();

    /// <summary>
    /// Gets a display name for the type.
    /// </summary>
    /// <returns>Display name</returns>
    public string GetDisplayName() => string.IsNullOrEmpty(Description) ? Name : $"{Name} - {Description}";

    /// <summary>
    /// Checks if this type can have children.
    /// </summary>
    /// <returns>True if children are allowed, false otherwise</returns>
    public bool CanHaveChildren() => AllowsChildren && (!MaxChildren.HasValue || MaxChildren.Value > 0);

    /// <summary>
    /// Checks if this type can be deleted.
    /// </summary>
    /// <returns>True if deletable, false otherwise</returns>
    public bool CanBeDeleted() => !IsSystem && !TreeNodes.Any();
}
