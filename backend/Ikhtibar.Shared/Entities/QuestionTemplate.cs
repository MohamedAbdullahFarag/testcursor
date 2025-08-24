using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Shared.Entities
{
    [Table("QuestionTemplates")]
    public class QuestionTemplate : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int QuestionTypeId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Template { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? AnswerTemplate { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public bool IsPublic { get; set; } = false;

        [MaxLength(100)]
        public string? Subject { get; set; }

        [MaxLength(1000)]
        public string? Tags { get; set; }

        [MaxLength(2000)]
        public string? Instructions { get; set; }

        [MaxLength(2000)]
        public string? ValidationRules { get; set; }

        [MaxLength(2000)]
        public string? MetadataJson { get; set; }

        // Navigation properties
        [ForeignKey("QuestionTypeId")]
        public virtual QuestionType QuestionType { get; set; } = null!;

        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;

        public virtual ICollection<Question> QuestionsCreated { get; set; } = new List<Question>();
    }
}
