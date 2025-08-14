using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Exam entity to support exam creation and management
/// </summary>
[Table("Exams")]
public class Exam : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int ExamId { get; set; }

    /// <summary>
    /// Exam title
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Exam description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Exam instructions
    /// </summary>
    [StringLength(1000)]
    public string? Instructions { get; set; }

    /// <summary>
    /// Duration in minutes
    /// </summary>
    [Required]
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Passing score
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? PassingScore { get; set; }

    /// <summary>
    /// Maximum score
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal MaxScore { get; set; }

    /// <summary>
    /// Is question order randomized
    /// </summary>
    [Required]
    public bool IsRandomized { get; set; } = false;

    /// <summary>
    /// Allow review of answers
    /// </summary>
    [Required]
    public bool AllowReview { get; set; } = true;

    /// <summary>
    /// Show results to student
    /// </summary>
    [Required]
    public bool ShowResults { get; set; } = true;

    /// <summary>
    /// Exam start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Exam end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Exam status
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Draft";

    /// <summary>
    /// Navigation property to ExamQuestions
    /// </summary>
    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
}
