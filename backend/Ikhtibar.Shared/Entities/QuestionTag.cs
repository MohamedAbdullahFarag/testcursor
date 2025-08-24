using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Shared.Entities
{
    [Table("QuestionTags")]
    public class QuestionTag : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public int UsageCount { get; set; } = 0;

        [MaxLength(7)]
        public string? Color { get; set; }

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;

        public virtual ICollection<QuestionTagAssignment> QuestionAssignments { get; set; } = new List<QuestionTagAssignment>();
    }

    [Table("QuestionTagAssignments")]
    public class QuestionTagAssignment : BaseEntity
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int TagId { get; set; }

        [Required]
        public int AssignedBy { get; set; }

        [Required]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        [ForeignKey("TagId")]
        public virtual QuestionTag Tag { get; set; } = null!;

        [ForeignKey("AssignedBy")]
        public virtual User AssignedByUser { get; set; } = null!;
    }
}
