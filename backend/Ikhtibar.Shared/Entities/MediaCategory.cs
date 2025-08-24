using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Media categorization entity for organizing media files
/// Supports hierarchical organization with parent-child relationships
/// </summary>
[Table("MediaCategories")]
public class MediaCategory : BaseEntity
{
    /// <summary>
    /// Category name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Category slug for URLs
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Parent category ID for hierarchical organization
    /// </summary>
    public int? ParentCategoryId { get; set; }

    /// <summary>
    /// Display order within parent category
    /// </summary>
    [Required]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Whether this category is active/visible
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Icon class or identifier for UI display
    /// </summary>
    [StringLength(50)]
    public string? IconClass { get; set; }

    /// <summary>
    /// Color code for UI theming
    /// </summary>
    [StringLength(7)]
    public string? ColorCode { get; set; }

    /// <summary>
    /// JSON metadata for category-specific properties
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Metadata { get; set; }

    // Navigation Properties

    /// <summary>
    /// Parent category
    /// </summary>
    [ForeignKey(nameof(ParentCategoryId))]
    public virtual MediaCategory? ParentCategory { get; set; }

    /// <summary>
    /// Child categories
    /// </summary>
    public virtual ICollection<MediaCategory> ChildCategories { get; set; } = new List<MediaCategory>();

    /// <summary>
    /// Media files in this category
    /// </summary>
    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
}
