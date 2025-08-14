using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Question type lookup entity (e.g., Multiple Choice, True/False, Essay)
/// </summary>
[Table("QuestionTypes")]
public class QuestionType : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int QuestionTypeId { get; set; }

    /// <summary>
    /// Question type name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Questions that belong to this type
    /// </summary>
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
