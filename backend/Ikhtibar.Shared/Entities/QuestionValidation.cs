using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Shared.Entities
{
    [Table("QuestionValidations")]
    public class QuestionValidation : BaseEntity
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int ValidatedBy { get; set; }

        [Required]
        public bool IsValid { get; set; } = false;

        [Required]
        public int Score { get; set; } = 0; // 0-100

        [MaxLength(2000)]
        public string? ValidationErrors { get; set; } // JSON array of errors

        [MaxLength(2000)]
        public string? ValidationWarnings { get; set; } // JSON array of warnings

        [MaxLength(2000)]
        public string? ValidationNotes { get; set; }

        [Required]
        public ValidationStatus Status { get; set; } = ValidationStatus.Pending;

        public DateTime? ApprovedAt { get; set; }

        public int? ApprovedBy { get; set; }

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        // Navigation properties
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        [ForeignKey("ValidatedBy")]
        public virtual User ValidatedByUser { get; set; } = null!;

        [ForeignKey("ApprovedBy")]
        public virtual User? ApprovedByUser { get; set; }
    }

    public enum ValidationStatus
    {
        Pending = 1,
        InProgress = 2,
        Validated = 3,
        Approved = 4,
        Rejected = 5,
        NeedsRevision = 6
    }
}
