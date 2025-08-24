using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for creating a new question bank
/// </summary>
public class CreateQuestionBankDto
{
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
    /// ID of the curriculum this question bank aligns with
    /// </summary>
    public int? CurriculumId { get; set; }

    /// <summary>
    /// ID of the subject this question bank belongs to
    /// </summary>
    public int? SubjectId { get; set; }

    /// <summary>
    /// ID of the grade level this question bank is designed for
    /// </summary>
    public int? GradeLevelId { get; set; }

    /// <summary>
    /// ID of the academic year this question bank is for
    /// </summary>
    public int? AcademicYearId { get; set; }

    /// <summary>
    /// ID of the semester this question bank is for
    /// </summary>
    public int? SemesterId { get; set; }

    /// <summary>
    /// ID of the term this question bank is for
    /// </summary>
    public int? TermId { get; set; }

    /// <summary>
    /// Whether the question bank is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the question bank is publicly available
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Whether the question bank is a system question bank
    /// </summary>
    public bool IsSystemQuestionBank { get; set; } = false;

    /// <summary>
    /// Whether the question bank allows question creation
    /// </summary>
    public bool AllowsQuestionCreation { get; set; } = true;

    /// <summary>
    /// Whether the question bank allows question editing
    /// </summary>
    public bool AllowsQuestionEditing { get; set; } = true;

    /// <summary>
    /// Whether the question bank allows question deletion
    /// </summary>
    public bool AllowsQuestionDeletion { get; set; } = true;

    /// <summary>
    /// Whether the question bank allows question import
    /// </summary>
    public bool AllowsQuestionImport { get; set; } = true;

    /// <summary>
    /// Whether the question bank allows question export
    /// </summary>
    public bool AllowsQuestionExport { get; set; } = true;

    /// <summary>
    /// Maximum number of questions allowed in this bank
    /// </summary>
    [Range(1, 100000)]
    public int? MaxQuestions { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Tags associated with this question bank
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// ID of the user who creates the question bank
    /// </summary>
    public int CreatedBy { get; set; }
}
