using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for question tag information
/// </summary>
public class QuestionTagDto
{
    /// <summary>
    /// Unique identifier for the question tag
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the question tag
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the question tag
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Code identifier for the question tag
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Category of the question tag
    /// </summary>
    [StringLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Color representation for the question tag
    /// </summary>
    [StringLength(7)] // Hex color code
    public string? Color { get; set; }

    /// <summary>
    /// Icon representation for the question tag
    /// </summary>
    [StringLength(50)]
    public string? Icon { get; set; }

    /// <summary>
    /// Whether the question tag is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the question tag is publicly available
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Whether the question tag is a system tag
    /// </summary>
    public bool IsSystemTag { get; set; }

    /// <summary>
    /// ID of the user who created this question tag
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// Name of the user who created this question tag
    /// </summary>
    public string? CreatedByName { get; set; }

    /// <summary>
    /// When the question tag was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// ID of the user who last updated this question tag
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Name of the user who last updated this question tag
    /// </summary>
    public string? UpdatedByName { get; set; }

    /// <summary>
    /// When the question tag was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Number of questions using this tag
    /// </summary>
    public int QuestionCount { get; set; }

    /// <summary>
    /// Number of question banks using this tag
    /// </summary>
    public int QuestionBankCount { get; set; }

    /// <summary>
    /// Average score for questions with this tag
    /// </summary>
    public double AverageScore { get; set; }

    /// <summary>
    /// Pass rate for questions with this tag
    /// </summary>
    public double PassRate { get; set; }
}
