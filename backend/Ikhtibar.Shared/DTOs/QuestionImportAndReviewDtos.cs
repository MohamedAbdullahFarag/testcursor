using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    // Import/export DTOs
    public class QuestionBankImportDto
    {
        public int QuestionBankId { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public int CreatedBy { get; set; }
    }

    public class QuestionBankImportResultDto
    {
        public int BatchId { get; set; }
        public int Total { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<QuestionImportResultDto> Results { get; set; } = new List<QuestionImportResultDto>();
    }

    public class QuestionImportDto
    {
        public int BatchId { get; set; }
        public string Source { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
    public int ImportedBy { get; set; }
    }

    public class QuestionImportResultDto
    {
        public int RowNumber { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? CreatedQuestionId { get; set; }
    }

    public class QuestionExportDto
    {
        public IEnumerable<int> QuestionIds { get; set; } = new List<int>();
        public string Format { get; set; } = "json";
        public int RequestedBy { get; set; }
    }

    public class QuestionExportResultDto
    {
        public string FilePath { get; set; } = string.Empty;
        public int TotalExported { get; set; }
        public DateTime ExportedAt { get; set; }
    }

    public class QuestionImportLogDto
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int RowNumber { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime LoggedAt { get; set; }
    }

    public class SupportedImportFormatDto
    {
        public string Format { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
    }

    public class QuestionImportTemplateDto
    {
        public string Format { get; set; } = string.Empty;
        public string TemplateContent { get; set; } = string.Empty; // could be CSV header or JSON schema
    }

    // Review DTOs
    public class QuestionReviewDto
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int ReviewerId { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected
        public string? Comments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }

    public class CreateQuestionReviewDto
    {
        [Required]
        public int QuestionId { get; set; }
        [Required]
        public int ReviewerId { get; set; }
        public string? InitialComments { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdateQuestionReviewDto
    {
        public int Id { get; set; }
        public string? Comments { get; set; }
        public string? Status { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class RejectQuestionReviewDto
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int RejectedBy { get; set; }
    }

    public class RequestRevisionDto
    {
        public int Id { get; set; }
        public string RevisionNotes { get; set; } = string.Empty;
        public int RequestedBy { get; set; }
    }

    public class AssignReviewerDto
    {
        public int Id { get; set; } // review id or question id depending on caller
        public int ReviewerId { get; set; }
        public int AssignedBy { get; set; }
    }

    public class AddReviewCommentDto
    {
        public int ReviewId { get; set; }
        public int CommentedBy { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;
    }

    // Question creation workflow DTOs
    public class QuestionCreationWorkflowDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class CreateQuestionCreationWorkflowDto
    {
        public string Name { get; set; } = string.Empty;
        public int CreatorId { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class UpdateQuestionCreationWorkflowDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class CancelWorkflowDto
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int CancelledBy { get; set; }
    }

    // Analytics helper DTOs referenced by API
    public class QuestionQualityDto
    {
        public int QuestionId { get; set; }
        public decimal QualityScore { get; set; }
        public string? Summary { get; set; }
        public Dictionary<string, object>? Metrics { get; set; }
    }

    public class ReportingDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object>? Data { get; set; }
    }

    // (analytics/search/clone DTOs are defined in their canonical Shared DTO files)
}
