using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Question status lookup entity (e.g., Draft, Published, Archived)
/// </summary>
[Table("QuestionStatuses")]
public class QuestionStatus : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int QuestionStatusId { get; set; }

    /// <summary>
    /// Question status name
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Questions that belong to this status
    /// </summary>
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
