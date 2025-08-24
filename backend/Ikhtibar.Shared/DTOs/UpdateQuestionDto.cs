using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for updating an existing question
/// </summary>
public class UpdateQuestionDto
{
    /// <summary>
    /// Title of the question
    /// </summary>
    [StringLength(500)]
    public string? Title { get; set; }

    /// <summary>
    /// Content of the question
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Text alias expected by some services. Maps to Content.
    /// </summary>
    public string? Text
    {
        get => Content;
        set => Content = value;
    }

    /// <summary>
    /// Explanation for the question
    /// </summary>
    public string? Explanation { get; set; }

    /// <summary>
    /// Hints for the question
    /// </summary>
    public string? Hints { get; set; }

    /// <summary>
    /// Solution field expected by some services. Maps to Explanation by convention.
    /// </summary>
    public string? Solution
    {
        get => Explanation;
        set => Explanation = value;
    }

    /// <summary>
    /// ID of the question type
    /// </summary>
    public int? QuestionTypeId { get; set; }

    /// <summary>
    /// ID of the question difficulty level
    /// </summary>
    public int? DifficultyLevelId { get; set; }

    /// <summary>
    /// ID of the question bank this question belongs to
    /// </summary>
    public int? QuestionBankId { get; set; }

    /// <summary>
    /// ID of the question template this question is based on
    /// </summary>
    public int? QuestionTemplateId { get; set; }

    /// <summary>
    /// ID of the curriculum this question aligns with
    /// </summary>
    public int? CurriculumId { get; set; }

    /// <summary>
    /// ID of the subject this question belongs to
    /// </summary>
    public int? SubjectId { get; set; }

    /// <summary>
    /// ID of the grade level this question is designed for
    /// </summary>
    public int? GradeLevelId { get; set; }

    /// <summary>
    /// ID of the academic year this question is for
    /// </summary>
    public int? AcademicYearId { get; set; }

    /// <summary>
    /// ID of the semester this question is for
    /// </summary>
    public int? SemesterId { get; set; }

    /// <summary>
    /// ID of the term this question is for
    /// </summary>
    public int? TermId { get; set; }

    /// <summary>
    /// Points for this question
    /// </summary>
    [Range(0, 100)]
    public int? Points { get; set; }

    /// <summary>
    /// Estimated time in seconds to answer this question
    /// </summary>
    [Range(1, 3600)]
    public int? EstimatedTimeSec { get; set; }

    /// <summary>
    /// Whether the question is currently active
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Whether the question is published
    /// </summary>
    public bool? IsPublished { get; set; }

    /// <summary>
    /// Whether the question is archived
    /// </summary>
    public bool? IsArchived { get; set; }

    /// <summary>
    /// Whether the question is a system question
    /// </summary>
    public bool? IsSystemQuestion { get; set; }

    /// <summary>
    /// Whether the question allows partial credit
    /// </summary>
    public bool? AllowsPartialCredit { get; set; }

    /// <summary>
    /// Whether the question allows negative marking
    /// </summary>
    public bool? AllowsNegativeMarking { get; set; }

    /// <summary>
    /// Whether the question is randomized
    /// </summary>
    public bool? IsRandomized { get; set; }

    /// <summary>
    /// Whether the question is adaptive
    /// </summary>
    public bool? IsAdaptive { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Version number of the question
    /// </summary>
    public int? Version { get; set; }

    /// <summary>
    /// Tags associated with this question
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Media files associated with this question
    /// </summary>
    public List<string>? MediaFiles { get; set; }

    /// <summary>
    /// Primary tree node id
    /// </summary>
    public int? PrimaryTreeNodeId { get; set; }

    /// <summary>
    /// Id of the user who updated the question
    /// </summary>
    public int UpdatedBy { get; set; }
}
