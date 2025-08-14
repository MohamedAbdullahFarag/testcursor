using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Question Bank Category entity for hierarchical organization of questions
/// Following materialized path pattern for efficient tree operations
/// Extends existing TreeNode concepts for question bank specific categorization
/// </summary>
[Table("QuestionBankCategories")]
public class QuestionBankCategory : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int CategoryId { get; set; }

    /// <summary>
    /// Category display name
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for category identification
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Optional category description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Category type defining the level in hierarchy
    /// </summary>
    [Required]
    public CategoryType Type { get; set; }

    /// <summary>
    /// Hierarchical level (1-6 supported)
    /// </summary>
    [Required]
    public CategoryLevel Level { get; set; }

    /// <summary>
    /// Foreign key to parent category
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Sort order within parent level
    /// </summary>
    [Required]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Materialized path for efficient tree traversal
    /// Format: -1-4-9- (ancestor ids separated by dashes)
    /// </summary>
    [Required]
    [StringLength(500)]
    public string TreePath { get; set; } = string.Empty;

    /// <summary>
    /// Whether category is active and visible
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether questions can be directly assigned to this category
    /// </summary>
    [Required]
    public bool AllowQuestions { get; set; } = true;

    /// <summary>
    /// Additional metadata as JSON string
    /// </summary>
    [StringLength(2000)]
    public string? MetadataJson { get; set; }

    // Curriculum alignment properties
    /// <summary>
    /// Curriculum standard code reference
    /// </summary>
    [StringLength(100)]
    public string? CurriculumCode { get; set; }

    /// <summary>
    /// Associated grade level (e.g., "Grade 5", "High School")
    /// </summary>
    [StringLength(100)]
    public string? GradeLevel { get; set; }

    /// <summary>
    /// Subject area (e.g., "Mathematics", "Science")
    /// </summary>
    [StringLength(100)]
    public string? Subject { get; set; }

    // Navigation properties
    /// <summary>
    /// Parent category reference
    /// </summary>
    [ForeignKey("ParentId")]
    public virtual QuestionBankCategory? Parent { get; set; }

    /// <summary>
    /// Child categories collection
    /// </summary>
    public virtual ICollection<QuestionBankCategory> Children { get; set; } = new List<QuestionBankCategory>();

    /// <summary>
    /// Questions associated with this category
    /// </summary>
    public virtual ICollection<QuestionCategorization> Questions { get; set; } = new List<QuestionCategorization>();

    /// <summary>
    /// Hierarchy relationships for efficient querying
    /// </summary>
    public virtual ICollection<QuestionBankHierarchy> HierarchyRelations { get; set; } = new List<QuestionBankHierarchy>();
}
