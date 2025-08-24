using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class QuestionValidationDto
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int ValidatedBy { get; set; }
        public DateTime ValidatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public double? Score { get; set; }
        public string? Comments { get; set; }
        public string? ValidationData { get; set; }
        public bool IsActive { get; set; }
    }

    public class QuestionQualityScoreDto
    {
        public int QuestionId { get; set; }
        public double OverallScore { get; set; }
        public double ContentScore { get; set; }
        public double FormatScore { get; set; }
        public double DifficultyScore { get; set; }
        public int ValidationCount { get; set; }
        public DateTime LastValidated { get; set; }
        public string QualityLevel { get; set; } = string.Empty; // Poor, Fair, Good, Excellent
    }

    public class QuestionQualityIssueDto
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string IssueType { get; set; } = string.Empty; // Content, Format, Business, Technical
        public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
        public string Description { get; set; } = string.Empty;
        public string? SuggestedFix { get; set; }
        public DateTime ReportedAt { get; set; }
        public int ReportedBy { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int? ResolvedBy { get; set; }
    }
}
