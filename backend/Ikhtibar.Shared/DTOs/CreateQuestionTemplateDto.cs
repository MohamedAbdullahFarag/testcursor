using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for creating a new question template
/// </summary>
public class CreateQuestionTemplateDto
{
    /// <summary>
    /// Name of the question template
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the question template
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Code identifier for the question template
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Category of the question template
    /// </summary>
    [StringLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// ID of the question type this template is based on
    /// </summary>
    [Required]
    public int QuestionTypeId { get; set; }

    /// <summary>
    /// ID of the question difficulty level
    /// </summary>
    public int? DifficultyLevelId { get; set; }

    /// <summary>
    /// ID of the question bank this template belongs to
    /// </summary>
    public int? QuestionBankId { get; set; }

    /// <summary>
    /// ID of the curriculum this template aligns with
    /// </summary>
    public int? CurriculumId { get; set; }

    /// <summary>
    /// ID of the subject this template belongs to
    /// </summary>
    public int? SubjectId { get; set; }

    /// <summary>
    /// ID of the grade level this template is designed for
    /// </summary>
    public int? GradeLevelId { get; set; }

    /// <summary>
    /// Template content in JSON format
    /// </summary>
    [Required]
    public string TemplateContent { get; set; } = string.Empty;

    /// <summary>
    /// Template structure definition
    /// </summary>
    public string? StructureDefinition { get; set; }

    /// <summary>
    /// Validation rules for the template
    /// </summary>
    public string? ValidationRules { get; set; }

    /// <summary>
    /// Default points for questions created from this template
    /// </summary>
    [Range(0, 100)]
    public int DefaultPoints { get; set; } = 1;

    /// <summary>
    /// Estimated time in seconds to answer questions from this template
    /// </summary>
    [Range(1, 3600)]
    public int? EstimatedTimeSec { get; set; }

    /// <summary>
    /// Whether the template is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the template is publicly available
    /// </summary>
    public bool IsPublic { get; set; } = true;

    /// <summary>
    /// Whether the template is a system template
    /// </summary>
    public bool IsSystemTemplate { get; set; } = false;

    /// <summary>
    /// Version number of the template
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Tags associated with this template
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();
}
