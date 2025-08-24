using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Shared.Entities
{
    [Table("QuestionBanks")]
    public class QuestionBank : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastModified { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public bool IsPublic { get; set; } = false;

        [MaxLength(100)]
        public string? Subject { get; set; }

        [MaxLength(100)]
        public string? GradeLevel { get; set; }

        [MaxLength(100)]
        public string? Language { get; set; } = "en";

        [MaxLength(2000)]
        public string? MetadataJson { get; set; }

        // Statistics
        [Required]
        public int TotalQuestions { get; set; } = 0;

        [Required]
        public int ActiveQuestions { get; set; } = 0;

        [Required]
        public int ReviewedQuestions { get; set; } = 0;

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<QuestionBankAccess> AccessPermissions { get; set; } = new List<QuestionBankAccess>();
    }

    [Table("QuestionBankAccess")]
    public class QuestionBankAccess : BaseEntity
    {
        [Required]
        public int QuestionBankId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Permission { get; set; } = string.Empty; // Read, Write, Admin

        [Required]
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiresAt { get; set; }

        [MaxLength(500)]
        public string? GrantedBy { get; set; }

        // Navigation properties
        [ForeignKey("QuestionBankId")]
        public virtual QuestionBank QuestionBank { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
