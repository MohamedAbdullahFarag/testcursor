using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Question Bank Hierarchy entity for efficient tree traversal operations
/// Implements closure table pattern to support complex hierarchy queries
/// Complements materialized path pattern for optimal performance
/// </summary>
[Table("QuestionBankHierarchies")]
public class QuestionBankHierarchy : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int HierarchyId { get; set; }

    /// <summary>
    /// Foreign key to ancestor category in the hierarchy
    /// </summary>
    [Required]
    public int AncestorId { get; set; }

    /// <summary>
    /// Foreign key to descendant category in the hierarchy
    /// </summary>
    [Required]
    public int DescendantId { get; set; }

    /// <summary>
    /// Depth level between ancestor and descendant
    /// 0 = same node, 1 = direct parent-child, 2+ = indirect relationship
    /// </summary>
    [Required]
    public int Depth { get; set; }

    /// <summary>
    /// Path from ancestor to descendant as comma-separated category IDs
    /// Enables path reconstruction for breadcrumb navigation
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Whether this hierarchy relationship is direct (parent-child)
    /// </summary>
    [Required]
    public bool IsDirect { get; set; } = false;

    /// <summary>
    /// Sort order for consistent hierarchy traversal
    /// </summary>
    public int? SortOrder { get; set; }

    /// <summary>
    /// When this hierarchy relationship was established
    /// </summary>
    [Required]
    public DateTime EstablishedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Reference to the ancestor category
    /// </summary>
    [ForeignKey("AncestorId")]
    public virtual QuestionBankCategory Ancestor { get; set; } = null!;

    /// <summary>
    /// Reference to the descendant category
    /// </summary>
    [ForeignKey("DescendantId")]
    public virtual QuestionBankCategory Descendant { get; set; } = null!;
}
