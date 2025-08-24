using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Represents a node in a hierarchical tree structure.
/// Supports materialized path for efficient tree traversal.
/// </summary>
[Table("TreeNodes")]
public class TreeNode
{
    /// <summary>
    /// Unique identifier for the tree node.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Name of the tree node.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the tree node.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Type/category of the tree node.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string NodeType { get; set; } = string.Empty;

    /// <summary>
    /// Materialized path from root to this node.
    /// Format: /root/parent/child (e.g., "/1/5/12")
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Level/depth of the node in the tree (0 for root).
    /// </summary>
    [Required]
    public int Level { get; set; }

    /// <summary>
    /// Order/position among siblings.
    /// </summary>
    [Required]
    public int OrderIndex { get; set; }

    /// <summary>
    /// ID of the parent node (null for root nodes).
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Navigation property for the parent node.
    /// </summary>
    [ForeignKey(nameof(ParentId))]
    public virtual TreeNode? Parent { get; set; }

    /// <summary>
    /// Collection of child nodes.
    /// </summary>
    public virtual ICollection<TreeNode> Children { get; set; } = new List<TreeNode>();

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Metadata { get; set; }

    /// <summary>
    /// Whether the node is active/enabled.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the node is visible to users.
    /// </summary>
    [Required]
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Icon or visual representation for the node.
    /// </summary>
    [StringLength(100)]
    public string? Icon { get; set; }

    /// <summary>
    /// Color theme for the node.
    /// </summary>
    [StringLength(50)]
    public string? Color { get; set; }

    /// <summary>
    /// When the node was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Who created the node.
    /// </summary>
    [StringLength(100)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// When the node was last modified.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Who last modified the node.
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
    /// When the node was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Who soft deleted the node.
    /// </summary>
    [StringLength(100)]
    public string? DeletedBy { get; set; }

    /// <summary>
    /// Gets the full path including the current node.
    /// </summary>
    /// <returns>Full path string</returns>
    public string GetFullPath() => $"{Path}/{Id}";

    /// <summary>
    /// Gets the parent path (path without current node).
    /// </summary>
    /// <returns>Parent path string</returns>
    public string GetParentPath() => Path;

    /// <summary>
    /// Checks if this node is a root node.
    /// </summary>
    /// <returns>True if root, false otherwise</returns>
    public bool IsRoot() => ParentId == null;

    /// <summary>
    /// Checks if this node is a leaf node (no children).
    /// </summary>
    /// <returns>True if leaf, false otherwise</returns>
    public bool IsLeaf() => !Children.Any();

    /// <summary>
    /// Gets the number of children.
    /// </summary>
    /// <returns>Child count</returns>
    public int GetChildCount() => Children.Count;

    /// <summary>
    /// Gets the total number of descendants.
    /// </summary>
    /// <returns>Total descendant count</returns>
    public int GetDescendantCount() => Children.Sum(c => 1 + c.GetDescendantCount());
}
