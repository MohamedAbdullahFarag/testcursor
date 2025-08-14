using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Junction entity for Exams and Questions many-to-many relationship
/// </summary>
[Table("ExamQuestions")]
public class ExamQuestion
{
    /// <summary>
    /// Foreign key to Exams
    /// </summary>
    [Key]
    [Column(Order = 0)]
    public int ExamId { get; set; }

    /// <summary>
    /// Foreign key to Questions
    /// </summary>
    [Key]
    [Column(Order = 1)]
    public int QuestionId { get; set; }

    /// <summary>
    /// Order index in exam
    /// </summary>
    [Required]
    public int OrderIndex { get; set; } = 0;

    /// <summary>
    /// Points for this question in this exam
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Points { get; set; } = 1.00m;

    /// <summary>
    /// Navigation property to Exam
    /// </summary>
    [ForeignKey("ExamId")]
    public virtual Exam Exam { get; set; } = null!;

    /// <summary>
    /// Navigation property to Question
    /// </summary>
    [ForeignKey("QuestionId")]
    public virtual Question Question { get; set; } = null!;
}
