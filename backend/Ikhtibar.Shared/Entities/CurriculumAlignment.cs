using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Curriculum alignment entity to support curriculum-aligned tree design
/// </summary>
[Table("CurriculumAlignments")]
public class CurriculumAlignment
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int CurriculumAlignmentId { get; set; }

    /// <summary>
    /// Foreign key to TreeNodes
    /// </summary>
    [Required]
    public int TreeNodeId { get; set; }

    /// <summary>
    /// Standard code
    /// </summary>
    [Required]
    [StringLength(100)]
    public string StandardCode { get; set; } = string.Empty;

    /// <summary>
    /// Curriculum version
    /// </summary>
    [StringLength(20)]
    public string? CurriculumVersion { get; set; }

    /// <summary>
    /// Education level
    /// </summary>
    [StringLength(50)]
    public string? EducationLevel { get; set; }

    /// <summary>
    /// Grade level
    /// </summary>
    public int? GradeLevel { get; set; }

    /// <summary>
    /// Subject area
    /// </summary>
    [StringLength(100)]
    public string? SubjectArea { get; set; }

    /// <summary>
    /// Strand
    /// </summary>
    [StringLength(200)]
    public string? Strand { get; set; }

    /// <summary>
    /// Standard URL
    /// </summary>
    [StringLength(500)]
    public string? StandardUrl { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User ID who created the record
    /// </summary>
    [Required]
    public int CreatedBy { get; set; }

    /// <summary>
    /// Last modification timestamp
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// User ID who last modified the record
    /// </summary>
    public int? ModifiedBy { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    [Required]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Navigation property to TreeNode
    /// </summary>
    [ForeignKey("TreeNodeId")]
    public virtual TreeNode TreeNode { get; set; } = null!;
}
