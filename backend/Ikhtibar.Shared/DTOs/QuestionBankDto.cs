using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for question bank information
/// </summary>
public class QuestionBankDto
{
    /// <summary>
    /// Unique identifier for the question bank
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the question bank
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the question bank
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Code identifier for the question bank
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Category of the question bank
    /// </summary>
    [StringLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// ID of the parent question bank (for hierarchical structure)
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Name of the parent question bank
    /// </summary>
    public string? ParentName { get; set; }

    /// <summary>
    /// ID of the curriculum this question bank aligns with
    /// </summary>
    public int? CurriculumId { get; set; }

    /// <summary>
    /// Name of the curriculum
    /// </summary>
    public string? CurriculumName { get; set; }

    /// <summary>
    /// ID of the subject this question bank belongs to
    /// </summary>
    public int? SubjectId { get; set; }

    /// <summary>
    /// Name of the subject
    /// </summary>
    public string? SubjectName { get; set; }

    /// <summary>
    /// ID of the grade level this question bank is designed for
    /// </summary>
    public int? GradeLevelId { get; set; }

    /// <summary>
    /// Name of the grade level
    /// </summary>
    public string? GradeLevelName { get; set; }

    /// <summary>
    /// ID of the academic year this question bank is for
    /// </summary>
    public int? AcademicYearId { get; set; }

    /// <summary>
    /// Name of the academic year
    /// </summary>
    public string? AcademicYearName { get; set; }

    /// <summary>
    /// ID of the semester this question bank is for
    /// </summary>
    public int? SemesterId { get; set; }

    /// <summary>
    /// Name of the semester
    /// </summary>
    public string? SemesterName { get; set; }

    /// <summary>
    /// ID of the term this question bank is for
    /// </summary>
    public int? TermId { get; set; }

    /// <summary>
    /// Name of the term
    /// </summary>
    public string? TermName { get; set; }

    /// <summary>
    /// Whether the question bank is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the question bank is publicly available
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Whether the question bank is a system question bank
    /// </summary>
    public bool IsSystemQuestionBank { get; set; }

    /// <summary>
    /// Whether the question bank allows question creation
    /// </summary>
    public bool AllowsQuestionCreation { get; set; }

    /// <summary>
    /// Whether the question bank allows question editing
    /// </summary>
    public bool AllowsQuestionEditing { get; set; }

    /// <summary>
    /// Whether the question bank allows question deletion
    /// </summary>
    public bool AllowsQuestionDeletion { get; set; }

    /// <summary>
    /// Whether the question bank allows question import
    /// </summary>
    public bool AllowsQuestionImport { get; set; }

    /// <summary>
    /// Whether the question bank allows question export
    /// </summary>
    public bool AllowsQuestionExport { get; set; }

    /// <summary>
    /// Maximum number of questions allowed in this bank
    /// </summary>
    [Range(1, 100000)]
    public int? MaxQuestions { get; set; }

    /// <summary>
    /// ID of the user who created this question bank
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// Name of the user who created this question bank
    /// </summary>
    public string? CreatedByName { get; set; }

    /// <summary>
    /// When the question bank was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// ID of the user who last updated this question bank
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Name of the user who last updated this question bank
    /// </summary>
    public string? UpdatedByName { get; set; }

    /// <summary>
    /// When the question bank was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Number of questions in this bank
    /// </summary>
    public int QuestionCount { get; set; }

    /// <summary>
    /// Number of sub-question banks
    /// </summary>
    public int SubQuestionBankCount { get; set; }

    /// <summary>
    /// Average score for questions in this bank
    /// </summary>
    public double AverageScore { get; set; }

    /// <summary>
    /// Pass rate for questions in this bank
    /// </summary>
    public double PassRate { get; set; }

    /// <summary>
    /// Tags associated with this question bank
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();
}
