using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for updating an existing question difficulty level
/// </summary>
public class UpdateQuestionDifficultyDto
{
    /// <summary>
    /// Name of the difficulty level
    /// </summary>
    [StringLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Description of the difficulty level
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Numeric level of difficulty (1 = easiest, 10 = hardest)
    /// </summary>
    [Range(1, 10)]
    public int? Level { get; set; }

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
    public bool? IsActive { get; set; }

    /// <summary>
    /// Whether the difficulty level is publicly available
    /// </summary>
    public bool? IsPublic { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? MetadataJson { get; set; }
}
