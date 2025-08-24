using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for question version information
/// </summary>
public class QuestionVersionDto
{
    /// <summary>
    /// Unique identifier for the question version
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the question this version belongs to
    /// </summary>
    public int QuestionId { get; set; }

    /// <summary>
    /// Version number
    /// </summary>
    public int VersionNumber { get; set; }

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
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether this version is published
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Whether this version is archived
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// ID of the user who created this version
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// Name of the user who created this version
    /// </summary>
    public string? CreatedByName { get; set; }

    /// <summary>
    /// When the version was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// ID of the user who last updated this version
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Name of the user who last updated this version
    /// </summary>
    public string? UpdatedByName { get; set; }

    /// <summary>
    /// When the version was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

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
    /// ID of the user who reviewed this version
    /// </summary>
    public int? ReviewedBy { get; set; }

    /// <summary>
    /// Name of the user who reviewed this version
    /// </summary>
    public string? ReviewedByName { get; set; }

    /// <summary>
    /// When the version was reviewed
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// Review comments for this version
    /// </summary>
    public string? ReviewComments { get; set; }

    /// <summary>
    /// Number of times this version has been used
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Average score for this version
    /// </summary>
    public double AverageScore { get; set; }

    /// <summary>
    /// Pass rate for this version
    /// </summary>
    public double PassRate { get; set; }
}
