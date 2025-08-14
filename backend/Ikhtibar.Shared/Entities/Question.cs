using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Question entity
/// </summary>
[Table("Questions")]
public class Question : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int QuestionId { get; set; }

    /// <summary>
    /// Full question text
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to QuestionTypes
    /// </summary>
    [Required]
    public int QuestionTypeId { get; set; }

    /// <summary>
    /// Foreign key to DifficultyLevels
    /// </summary>
    [Required]
    public int DifficultyLevelId { get; set; }

    /// <summary>
    /// Solution explanation
    /// </summary>
    [StringLength(500)]
    public string? Solution { get; set; }

    /// <summary>
    /// Estimated solve time in seconds
    /// </summary>
    public int? EstimatedTimeSec { get; set; }

    /// <summary>
    /// Score points
    /// </summary>
    public int? Points { get; set; }

    /// <summary>
    /// Foreign key to QuestionStatuses
    /// </summary>
    [Required]
    public int QuestionStatusId { get; set; }

    /// <summary>
    /// Foreign key to primary TreeNodeId
    /// </summary>
    [Required]
    public int PrimaryTreeNodeId { get; set; }

    /// <summary>
    /// Version string
    /// </summary>
    [StringLength(50)]
    public string? Version { get; set; }

    /// <summary>
    /// Comma-separated tags
    /// </summary>
    [StringLength(200)]
    public string? Tags { get; set; }

    /// <summary>
    /// JSON metadata
    /// </summary>
    [StringLength(500)]
    public string? Metadata { get; set; }

    /// <summary>
    /// Foreign key to original question for versions
    /// </summary>
    public int? OriginalQuestionId { get; set; }

    /// <summary>
    /// Navigation property to QuestionType
    /// </summary>
    [ForeignKey("QuestionTypeId")]
    public virtual QuestionType QuestionType { get; set; } = null!;

    /// <summary>
    /// Navigation property to DifficultyLevel
    /// </summary>
    [ForeignKey("DifficultyLevelId")]
    public virtual DifficultyLevel DifficultyLevel { get; set; } = null!;

    /// <summary>
    /// Navigation property to QuestionStatus
    /// </summary>
    [ForeignKey("QuestionStatusId")]
    public virtual QuestionStatus QuestionStatus { get; set; } = null!;

    /// <summary>
    /// Navigation property to PrimaryTreeNode
    /// </summary>
    [ForeignKey("PrimaryTreeNodeId")]
    public virtual TreeNode PrimaryTreeNode { get; set; } = null!;

    /// <summary>
    /// Navigation property to original question
    /// </summary>
    [ForeignKey("OriginalQuestionId")]
    public virtual Question? OriginalQuestion { get; set; }

    /// <summary>
    /// Navigation property to Answers
    /// </summary>
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    /// <summary>
    /// Navigation property to QuestionMedia
    /// </summary>
    public virtual ICollection<QuestionMedia> QuestionMedia { get; set; } = new List<QuestionMedia>();

    /// <summary>
    /// Navigation property to ExamQuestions
    /// </summary>
    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    /// <summary>
    /// Navigation property to question versions
    /// </summary>
    public virtual ICollection<Question> QuestionVersions { get; set; } = new List<Question>();
}
