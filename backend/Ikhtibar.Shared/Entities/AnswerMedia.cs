using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Junction entity for Answers and Media many-to-many relationship
/// </summary>
[Table("AnswerMedia")]
public class AnswerMedia
{
    /// <summary>
    /// Foreign key to Answers
    /// </summary>
    [Key]
    [Column(Order = 0)]
    public int AnswerId { get; set; }

    /// <summary>
    /// Foreign key to Media
    /// </summary>
    [Key]
    [Column(Order = 1)]
    public int MediaId { get; set; }

    /// <summary>
    /// Navigation property to Answer
    /// </summary>
    [ForeignKey("AnswerId")]
    public virtual Answer Answer { get; set; } = null!;

    /// <summary>
    /// Navigation property to Media
    /// </summary>
    [ForeignKey("MediaId")]
    public virtual Media Media { get; set; } = null!;
}
