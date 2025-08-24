using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for question difficulty level information
/// </summary>
public class QuestionDifficultyDto
{
    /// <summary>
    /// Unique identifier for the difficulty level
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the difficulty level
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the difficulty level
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Numeric level of difficulty (1 = easiest, 10 = hardest)
    /// </summary>
    [Required]
    [Range(1, 10)]
    public int Level { get; set; }

    /// <summary>
    /// Category of the difficulty level
    /// </summary>
    [StringLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Color representation for the difficulty level
    /// </summary>
    [StringLength(7)] // Hex color code
    public string? Color { get; set; }

    /// <summary>
    /// Icon representation for the difficulty level
    /// </summary>
    [StringLength(50)]
    public string? Icon { get; set; }

    /// <summary>
    /// Whether the difficulty level is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the difficulty level is publicly available
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// ID of the user who created this difficulty level
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// Name of the user who created this difficulty level
    /// </summary>
    public string? CreatedByName { get; set; }

    /// <summary>
    /// When the difficulty level was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// ID of the user who last updated this difficulty level
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Name of the user who last updated this difficulty level
    /// </summary>
    public string? UpdatedByName { get; set; }

    /// <summary>
    /// When the difficulty level was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Number of questions using this difficulty level
    /// </summary>
    public int QuestionCount { get; set; }

    /// <summary>
    /// Average score for questions at this difficulty level
    /// </summary>
    public double AverageScore { get; set; }

    /// <summary>
    /// Pass rate for questions at this difficulty level
    /// </summary>
    public double PassRate { get; set; }
}
