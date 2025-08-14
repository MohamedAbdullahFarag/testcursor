using System.ComponentModel.DataAnnotations;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Data transfer object for CurriculumAlignment entity
/// </summary>
public class CurriculumAlignmentDto
{
    /// <summary>
    /// Curriculum alignment identifier
    /// </summary>
    public int CurriculumAlignmentId { get; set; }

    /// <summary>
    /// Tree node identifier
    /// </summary>
    public int TreeNodeId { get; set; }

    /// <summary>
    /// Standard code
    /// </summary>
    [Required]
    [StringLength(100)]
    public string StandardCode { get; set; } = string.Empty;

    /// <summary>
    /// Curriculum version
    /// </summary>
    [StringLength(20)]
    public string? CurriculumVersion { get; set; }

    /// <summary>
    /// Education level
    /// </summary>
    [StringLength(50)]
    public string? EducationLevel { get; set; }

    /// <summary>
    /// Grade level
    /// </summary>
    public int? GradeLevel { get; set; }

    /// <summary>
    /// Subject area
    /// </summary>
    [StringLength(100)]
    public string? SubjectArea { get; set; }

    /// <summary>
    /// Strand
    /// </summary>
    [StringLength(200)]
    public string? Strand { get; set; }

    /// <summary>
    /// Standard URL
    /// </summary>
    [StringLength(500)]
    public string? StandardUrl { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last modification timestamp
    /// </summary>
    public DateTime? ModifiedAt { get; set; }
}

/// <summary>
/// DTO for creating a new curriculum alignment
/// </summary>
public class CreateCurriculumAlignmentDto
{
    /// <summary>
    /// Tree node identifier
    /// </summary>
    [Required]
    public int TreeNodeId { get; set; }

    /// <summary>
    /// Standard code
    /// </summary>
    [Required]
    [StringLength(100)]
    public string StandardCode { get; set; } = string.Empty;

    /// <summary>
    /// Curriculum version
    /// </summary>
    [StringLength(20)]
    public string? CurriculumVersion { get; set; }

    /// <summary>
    /// Education level
    /// </summary>
    [StringLength(50)]
    public string? EducationLevel { get; set; }

    /// <summary>
    /// Grade level
    /// </summary>
    public int? GradeLevel { get; set; }

    /// <summary>
    /// Subject area
    /// </summary>
    [StringLength(100)]
    public string? SubjectArea { get; set; }

    /// <summary>
    /// Strand
    /// </summary>
    [StringLength(200)]
    public string? Strand { get; set; }

    /// <summary>
    /// Standard URL
    /// </summary>
    [StringLength(500)]
    public string? StandardUrl { get; set; }
}

/// <summary>
/// DTO for updating an existing curriculum alignment
/// </summary>
public class UpdateCurriculumAlignmentDto
{
    /// <summary>
    /// Standard code
    /// </summary>
    [Required]
    [StringLength(100)]
    public string StandardCode { get; set; } = string.Empty;

    /// <summary>
    /// Curriculum version
    /// </summary>
    [StringLength(20)]
    public string? CurriculumVersion { get; set; }

    /// <summary>
    /// Education level
    /// </summary>
    [StringLength(50)]
    public string? EducationLevel { get; set; }

    /// <summary>
    /// Grade level
    /// </summary>
    public int? GradeLevel { get; set; }

    /// <summary>
    /// Subject area
    /// </summary>
    [StringLength(100)]
    public string? SubjectArea { get; set; }

    /// <summary>
    /// Strand
    /// </summary>
    [StringLength(200)]
    public string? Strand { get; set; }

    /// <summary>
    /// Standard URL
    /// </summary>
    [StringLength(500)]
    public string? StandardUrl { get; set; }
}
