using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Question categorization entity linking questions to question bank categories
/// Supports many-to-many relationship between questions and categories
/// Allows primary category designation and categorization metadata
/// </summary>
[Table("QuestionCategorizations")]
public class QuestionCategorization : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int QuestionCategorizationId { get; set; }

    /// <summary>
    /// Foreign key to Question entity
    /// </summary>
    [Required]
    public int QuestionId { get; set; }

    /// <summary>
    /// Foreign key to QuestionBankCategory entity
    /// </summary>
    [Required]
    public int CategoryId { get; set; }

    /// <summary>
    /// Indicates if this is the primary category for the question
    /// Only one primary category per question allowed
    /// </summary>
    [Required]
    public bool IsPrimary { get; set; } = false;

    /// <summary>
    /// When the question was assigned to this category
    /// </summary>
    [Required]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User who assigned the question to this category
    /// </summary>
    [Required]
    public int AssignedBy { get; set; }

    /// <summary>
    /// Optional notes about the categorization
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Weight or importance of this categorization (1-10 scale)
    /// </summary>
    public int? Weight { get; set; }

    /// <summary>
    /// Whether this categorization was done automatically or manually
    /// </summary>
    [Required]
    public bool IsAutomatic { get; set; } = false;

    /// <summary>
    /// Confidence score for automatic categorization (0.0-1.0)
    /// </summary>
    public decimal? ConfidenceScore { get; set; }

    // Navigation properties
    /// <summary>
    /// Reference to the associated question
    /// </summary>
    [ForeignKey("QuestionId")]
    public virtual Question Question { get; set; } = null!;

    /// <summary>
    /// Reference to the associated category
    /// </summary>
    [ForeignKey("CategoryId")]
    public virtual QuestionBankCategory Category { get; set; } = null!;

    /// <summary>
    /// Reference to the user who assigned this categorization
    /// </summary>
    [ForeignKey("AssignedBy")]
    public virtual User AssignedByUser { get; set; } = null!;
}
