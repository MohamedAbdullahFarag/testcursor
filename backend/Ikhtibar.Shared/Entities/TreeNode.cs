using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Tree node entity for hierarchical structure with materialized path
/// </summary>
[Table("TreeNodes")]
public class TreeNode : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int TreeNodeId { get; set; }

    /// <summary>
    /// Node display name
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for node
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Optional description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Foreign key to TreeNodeTypes
    /// </summary>
    [Required]
    public int TreeNodeTypeId { get; set; }

    /// <summary>
    /// Foreign key to parent TreeNodeId
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    [Required]
    public int OrderIndex { get; set; } = 0;

    /// <summary>
    /// Materialized path (e.g., -1-4-9-)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Active flag
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to TreeNodeType
    /// </summary>
    [ForeignKey("TreeNodeTypeId")]
    public virtual TreeNodeType TreeNodeType { get; set; } = null!;

    /// <summary>
    /// Navigation property to Parent
    /// </summary>
    [ForeignKey("ParentId")]
    public virtual TreeNode? Parent { get; set; }

    /// <summary>
    /// Navigation property to Children
    /// </summary>
    public virtual ICollection<TreeNode> Children { get; set; } = new List<TreeNode>();

    /// <summary>
    /// Navigation property to Questions
    /// </summary>
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    /// <summary>
    /// Navigation property to CurriculumAlignments
    /// </summary>
    public virtual ICollection<CurriculumAlignment> CurriculumAlignments { get; set; } = new List<CurriculumAlignment>();
}
