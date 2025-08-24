using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class QuestionStatisticsDto
    {
        public int TotalQuestions { get; set; }
        public int ActiveQuestions { get; set; }
        public int DraftQuestions { get; set; }
        public int PublishedQuestions { get; set; }
        public int ArchivedQuestions { get; set; }
        public int QuestionsWithMedia { get; set; }
        public int QuestionsWithTags { get; set; }
        public double AverageDifficulty { get; set; }
        public double AverageEstimatedTime { get; set; }
        public double AveragePoints { get; set; }
        public int TotalAnswers { get; set; }
        public int TotalMediaFiles { get; set; }
        public int TotalTags { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class QuestionStatisticsFilterDto
    {
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? DifficultyLevelId { get; set; }
        public int? QuestionStatusId { get; set; }
        public int? PrimaryTreeNodeId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public DateTime? UpdatedFrom { get; set; }
        public DateTime? UpdatedTo { get; set; }
        public bool? IsActive { get; set; }
        public bool? HasMedia { get; set; }
        public bool? HasTags { get; set; }
        public bool? HasAnswers { get; set; }
    }

    public class QuestionBankValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public double QualityScore { get; set; }
        public string QualityLevel { get; set; } = string.Empty; // Poor, Fair, Good, Excellent
        public List<string> Recommendations { get; set; } = new List<string>();
        public DateTime ValidatedAt { get; set; }
        public int ValidatedBy { get; set; }
    }

    public class BankValidationIssueDto
    {
        public int Id { get; set; }
        public int QuestionBankId { get; set; }
        public string IssueType { get; set; } = string.Empty; // Structure, Content, Access, Performance
        public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
        public string Description { get; set; } = string.Empty;
        public string? SuggestedFix { get; set; }
        public DateTime ReportedAt { get; set; }
        public int ReportedBy { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int? ResolvedBy { get; set; }
        public string? Resolution { get; set; }
    }

    public class QuestionBankCriteriaDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Subject { get; set; }
        public string? GradeLevel { get; set; }
        public string? Language { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public int? MinQuestionCount { get; set; }
        public int? MaxQuestionCount { get; set; }
        public double? MinQualityScore { get; set; }
        public double? MaxQualityScore { get; set; }
    }
}
