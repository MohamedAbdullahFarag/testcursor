using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Shared.Entities
{
    [Table("QuestionVersions")]
    public class QuestionVersion : BaseEntity
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Version { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Solution { get; set; }

        [MaxLength(2000)]
        public string? AnswersJson { get; set; }

        [MaxLength(2000)]
        public string? MediaJson { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? ChangeDescription { get; set; }

        [Required]
        public bool IsCurrent { get; set; } = false;

        [Required]
        public VersionStatus Status { get; set; } = VersionStatus.Draft;

        public DateTime? PublishedAt { get; set; }

        public int? ApprovedBy { get; set; }

        [MaxLength(2000)]
        public string? MetadataJson { get; set; }

        // Navigation properties
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;

        [ForeignKey("ApprovedBy")]
        public virtual User? ApprovedByUser { get; set; }
    }

    public enum VersionStatus
    {
        Draft = 1,
        PendingReview = 2,
        Approved = 3,
        Published = 4,
        Archived = 5,
        Rejected = 6
    }
}
