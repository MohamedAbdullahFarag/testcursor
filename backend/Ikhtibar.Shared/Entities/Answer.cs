using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Answer entity for question answers
/// </summary>
[Table("Answers")]
public class Answer : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int AnswerId { get; set; }

    /// <summary>
    /// Foreign key to Questions
    /// </summary>
    [Required]
    public int QuestionId { get; set; }

    /// <summary>
    /// Answer text
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Correct answer flag
    /// </summary>
    [Required]
    public bool IsCorrect { get; set; } = false;

    /// <summary>
    /// Navigation property to Question
    /// </summary>
    [ForeignKey("QuestionId")]
    public virtual Question Question { get; set; } = null!;

    /// <summary>
    /// Navigation property to AnswerMedia
    /// </summary>
    public virtual ICollection<AnswerMedia> AnswerMedia { get; set; } = new List<AnswerMedia>();
}
