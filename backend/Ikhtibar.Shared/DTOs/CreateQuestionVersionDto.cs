using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for creating a new question version
/// </summary>
public class CreateQuestionVersionDto
{
    /// <summary>
    /// ID of the question this version belongs to
    /// </summary>
    [Required]
    public int QuestionId { get; set; }

    /// <summary>
    /// Title of the question version
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Content of the question version
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Explanation for the question version
    /// </summary>
    public string? Explanation { get; set; }

    /// <summary>
    /// Hints for the question version
    /// </summary>
    public string? Hints { get; set; }

    /// <summary>
    /// Answers for the question version
    /// </summary>
    public string? Answers { get; set; }

    /// <summary>
    /// Metadata for the question version
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Whether this version is the current active version
    /// </summary>
    public bool IsActive { get; set; } = false;

    /// <summary>
    /// Whether this version is published
    /// </summary>
    public bool IsPublished { get; set; } = false;

    /// <summary>
    /// Whether this version is archived
    /// </summary>
    public bool IsArchived { get; set; } = false;

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Change log for this version
    /// </summary>
    public string? ChangeLog { get; set; }

    /// <summary>
    /// Review status of this version
    /// </summary>
    public string? ReviewStatus { get; set; }

    /// <summary>
    /// Review comments for this version
    /// </summary>
    public string? ReviewComments { get; set; }
}
