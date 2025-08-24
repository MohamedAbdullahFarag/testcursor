using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for creating a new question type
/// </summary>
public class CreateQuestionTypeDto
{
    /// <summary>
    /// Name of the question type
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the question type
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Code identifier for the question type
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Category of the question type
    /// </summary>
    [StringLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Whether the question type supports multiple choice
    /// </summary>
    public bool SupportsMultipleChoice { get; set; }

    /// <summary>
    /// Whether the question type supports true/false answers
    /// </summary>
    public bool SupportsTrueFalse { get; set; }

    /// <summary>
    /// Whether the question type supports essay answers
    /// </summary>
    public bool SupportsEssay { get; set; }

    /// <summary>
    /// Whether the question type supports numeric answers
    /// </summary>
    public bool SupportsNumeric { get; set; }

    /// <summary>
    /// Whether the question type supports file uploads
    /// </summary>
    public bool SupportsFileUpload { get; set; }

    /// <summary>
    /// Maximum number of answers allowed
    /// </summary>
    [Range(1, 20)]
    public int MaxAnswers { get; set; } = 4;

    /// <summary>
    /// Minimum number of answers required
    /// </summary>
    [Range(1, 20)]
    public int MinAnswers { get; set; } = 2;

    /// <summary>
    /// Whether partial credit is allowed
    /// </summary>
    public bool AllowsPartialCredit { get; set; }

    /// <summary>
    /// Whether negative marking is allowed
    /// </summary>
    public bool AllowsNegativeMarking { get; set; }

    /// <summary>
    /// Default points for this question type
    /// </summary>
    [Range(0, 100)]
    public int DefaultPoints { get; set; } = 1;

    /// <summary>
    /// Whether the question type is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the question type is publicly available
    /// </summary>
    public bool IsPublic { get; set; } = true;

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? MetadataJson { get; set; }
}
