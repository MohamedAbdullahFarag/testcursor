using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Junction entity for Questions and Media many-to-many relationship
/// </summary>
[Table("QuestionMedia")]
public class QuestionMedia
{
    /// <summary>
    /// Foreign key to Questions
    /// </summary>
    [Key]
    [Column(Order = 0)]
    public int QuestionId { get; set; }

    /// <summary>
    /// Foreign key to Media
    /// </summary>
    [Key]
    [Column(Order = 1)]
    public int MediaId { get; set; }

    /// <summary>
    /// Navigation property to Question
    /// </summary>
    [ForeignKey("QuestionId")]
    public virtual Question Question { get; set; } = null!;

    /// <summary>
    /// Navigation property to Media
    /// </summary>
    [ForeignKey("MediaId")]
    public virtual Media Media { get; set; } = null!;
}
