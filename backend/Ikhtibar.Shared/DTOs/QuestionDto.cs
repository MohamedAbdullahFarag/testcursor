using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for question information
/// </summary>
public class QuestionDto
{
    /// <summary>
    /// Unique identifier for the question
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Backwards-compatible alias used by some controllers
    /// </summary>
    public int QuestionId { get => Id; set => Id = value; }

    /// <summary>
    /// Title of the question
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Content of the question
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Explanation for the question
    /// </summary>
    public string? Explanation { get; set; }

    /// <summary>
    /// Hints for the question
    /// </summary>
    public string? Hints { get; set; }

    /// <summary>
    /// ID of the question type
    /// </summary>
    public int QuestionTypeId { get; set; }

    /// <summary>
    /// Name of the question type
    /// </summary>
    public string? QuestionTypeName { get; set; }

    /// <summary>
    /// ID of the question difficulty level
    /// </summary>
    public int? DifficultyLevelId { get; set; }

    /// <summary>
    /// Name of the difficulty level
    /// </summary>
    public string? DifficultyLevelName { get; set; }

    /// <summary>
    /// ID of the question bank this question belongs to
    /// </summary>
    public int? QuestionBankId { get; set; }

    /// <summary>
    /// Name of the question bank
    /// </summary>
    public string? QuestionBankName { get; set; }

    /// <summary>
    /// ID of the question template this question is based on
    /// </summary>
    public int? QuestionTemplateId { get; set; }

    /// <summary>
    /// Name of the question template
    /// </summary>
    public string? QuestionTemplateName { get; set; }

    /// <summary>
    /// ID of the curriculum this question aligns with
    /// </summary>
    public int? CurriculumId { get; set; }

    /// <summary>
    /// Name of the curriculum
    /// </summary>
    public string? CurriculumName { get; set; }

    /// <summary>
    /// ID of the subject this question belongs to
    /// </summary>
    public int? SubjectId { get; set; }

    /// <summary>
    /// Name of the subject
    /// </summary>
    public string? SubjectName { get; set; }

    /// <summary>
    /// ID of the grade level this question is designed for
    /// </summary>
    public int? GradeLevelId { get; set; }

    /// <summary>
    /// Name of the grade level
    /// </summary>
    public string? GradeLevelName { get; set; }

    /// <summary>
    /// ID of the academic year this question is for
    /// </summary>
    public int? AcademicYearId { get; set; }

    /// <summary>
    /// Name of the academic year
    /// </summary>
    public string? AcademicYearName { get; set; }

    /// <summary>
    /// ID of the semester this question is for
    /// </summary>
    public int? SemesterId { get; set; }

    /// <summary>
    /// Name of the semester
    /// </summary>
    public string? SemesterName { get; set; }

    /// <summary>
    /// ID of the term this question is for
    /// </summary>
    public int? TermId { get; set; }

    /// <summary>
    /// Name of the term
    /// </summary>
    public string? TermName { get; set; }

    /// <summary>
    /// Points for this question
    /// </summary>
    [Range(0, 100)]
    public int Points { get; set; } = 1;

    /// <summary>
    /// Estimated time in seconds to answer this question
    /// </summary>
    [Range(1, 3600)]
    public int? EstimatedTimeSec { get; set; }

    /// <summary>
    /// Whether the question is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the question is published
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Whether the question is archived
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// Whether the question is a system question
    /// </summary>
    public bool IsSystemQuestion { get; set; }

    /// <summary>
    /// Whether the question allows partial credit
    /// </summary>
    public bool AllowsPartialCredit { get; set; }

    /// <summary>
    /// Whether the question allows negative marking
    /// </summary>
    public bool AllowsNegativeMarking { get; set; }

    /// <summary>
    /// Whether the question is randomized
    /// </summary>
    public bool IsRandomized { get; set; }

    /// <summary>
    /// Whether the question is adaptive
    /// </summary>
    public bool IsAdaptive { get; set; }

    /// <summary>
    /// ID of the user who created this question
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// Name of the user who created this question
    /// </summary>
    public string? CreatedByName { get; set; }

    /// <summary>
    /// When the question was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// ID of the user who last updated this question
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Name of the user who last updated this question
    /// </summary>
    public string? UpdatedByName { get; set; }

    /// <summary>
    /// When the question was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Version number of the question
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Number of times this question has been used
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Average score for this question
    /// </summary>
    public double AverageScore { get; set; }

    /// <summary>
    /// Pass rate for this question
    /// </summary>
    public double PassRate { get; set; }

    /// <summary>
    /// Tags associated with this question
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Media files associated with this question
    /// </summary>
    public List<string> MediaFiles { get; set; } = new List<string>();
}
