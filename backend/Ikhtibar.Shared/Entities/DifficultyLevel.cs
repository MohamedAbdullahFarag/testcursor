using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Difficulty level lookup entity (e.g., Easy, Medium, Hard)
/// </summary>
[Table("DifficultyLevels")]
public class DifficultyLevel : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int DifficultyLevelId { get; set; }

    /// <summary>
    /// Difficulty level name
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Questions that belong to this difficulty level
    /// </summary>
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
