using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for updating an existing question tag
/// </summary>
public class UpdateQuestionTagDto
{
    /// <summary>
    /// Name of the question tag
    /// </summary>
    [StringLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Description of the question tag
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Code identifier for the question tag
    /// </summary>
    [StringLength(50)]
    public string? Code { get; set; }

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
    public bool? IsActive { get; set; }

    /// <summary>
    /// Whether the question tag is publicly available
    /// </summary>
    public bool? IsPublic { get; set; }

    /// <summary>
    /// Whether the question tag is a system tag
    /// </summary>
    public bool? IsSystemTag { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? MetadataJson { get; set; }
}
