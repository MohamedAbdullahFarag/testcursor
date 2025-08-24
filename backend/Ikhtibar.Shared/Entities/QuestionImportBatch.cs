using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Shared.Entities
{
    [Table("QuestionImportBatches")]
    public class QuestionImportBatch : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public ImportStatus Status { get; set; } = ImportStatus.Pending;

        [Required]
        public int TotalQuestions { get; set; } = 0;

        [Required]
        public int ProcessedQuestions { get; set; } = 0;

        [Required]
        public int SuccessfulQuestions { get; set; } = 0;

        [Required]
        public int FailedQuestions { get; set; } = 0;

        [MaxLength(50)]
        public string? FileFormat { get; set; } // CSV, Excel, JSON, XML

        [MaxLength(500)]
        public string? FilePath { get; set; }

        [MaxLength(2000)]
        public string? ImportErrors { get; set; } // JSON array of errors

        [MaxLength(2000)]
        public string? ImportWarnings { get; set; } // JSON array of warnings

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;

        public virtual ICollection<QuestionImportLog> ImportLogs { get; set; } = new List<QuestionImportLog>();
    }

    [Table("QuestionImportLogs")]
    public class QuestionImportLog : BaseEntity
    {
        [Required]
        public int ImportBatchId { get; set; }

        [Required]
        public int RowNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // Success, Error, Warning

        [MaxLength(2000)]
        public string? QuestionText { get; set; }

        [MaxLength(2000)]
        public string? ErrorMessage { get; set; }

        [MaxLength(2000)]
        public string? WarningMessage { get; set; }

        [MaxLength(2000)]
        public string? RawData { get; set; } // Original import data

        [Required]
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ImportBatchId")]
        public virtual QuestionImportBatch ImportBatch { get; set; } = null!;
    }

    public enum ImportStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Failed = 4,
        Cancelled = 5
    }
}
