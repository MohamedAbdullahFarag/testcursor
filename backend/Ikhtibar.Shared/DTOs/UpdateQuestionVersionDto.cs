using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for updating an existing question version
/// </summary>
public class UpdateQuestionVersionDto
{
    /// <summary>
    /// Title of the question version
    /// </summary>
    [StringLength(500)]
    public string? Title { get; set; }

    /// <summary>
    /// Content of the question version
    /// </summary>
    public string? Content { get; set; }

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
    public bool? IsActive { get; set; }

    /// <summary>
    /// Whether this version is published
    /// </summary>
    public bool? IsPublished { get; set; }

    /// <summary>
    /// Whether this version is archived
    /// </summary>
    public bool? IsArchived { get; set; }

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
