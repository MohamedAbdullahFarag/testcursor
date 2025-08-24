using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class QuestionImportBatchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Pending, Processing, Completed, Failed
        public int TotalQuestions { get; set; }
        public int ProcessedQuestions { get; set; }
        public int SuccessfulQuestions { get; set; }
        public int FailedQuestions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int CreatedBy { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ImportFile { get; set; }
    }

    public class ImportQuestionsDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        public int QuestionBankId { get; set; }
        
        [Required]
        public int CreatedBy { get; set; }
        
        public string? Tags { get; set; }
        
        public string? Metadata { get; set; }
    }

    public class ImportBatchFilterDto
    {
        public string? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class ImportProgressDto
    {
        public int BatchId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int ProcessedQuestions { get; set; }
        public int SuccessfulQuestions { get; set; }
        public int FailedQuestions { get; set; }
        public double ProgressPercentage { get; set; }
        public string? CurrentOperation { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ImportFileDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FileType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public int UploadedBy { get; set; }
    }

    public class ImportValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public int TotalQuestions { get; set; }
        public int ValidQuestions { get; set; }
        public int InvalidQuestions { get; set; }
    }
}
