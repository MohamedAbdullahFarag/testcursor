using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class UsageSummaryDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int TotalUsage { get; set; }
        public int UniqueUsers { get; set; }
        public int UniqueExams { get; set; }
        public DateTime FirstUsed { get; set; }
        public DateTime LastUsed { get; set; }
        public double AverageScore { get; set; }
        public double AverageTimeSpent { get; set; }
        public string MostCommonUsageType { get; set; } = string.Empty;
    }

    public class QuestionQualityMetricsDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public double ContentQualityScore { get; set; }
        public double DifficultyAccuracyScore { get; set; }
        public double DiscriminationIndex { get; set; }
        public double ReliabilityScore { get; set; }
        public double OverallQualityScore { get; set; }
        public int ValidationCount { get; set; }
        public DateTime LastQualityAssessment { get; set; }
        public string QualityLevel { get; set; } = string.Empty; // Poor, Fair, Good, Excellent
    }

    public class QualitySummaryDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public double OverallQualityScore { get; set; }
        public string QualityLevel { get; set; } = string.Empty;
        public int ValidationCount { get; set; }
        public DateTime LastAssessment { get; set; }
        public List<string> QualityIssues { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
    }

    public class QualitySummaryFilterDto
    {
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? DifficultyLevelId { get; set; }
        public double? MinQualityScore { get; set; }
        public string? QualityLevel { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class QualityTrendDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public double QualityScore { get; set; }
        public string QualityLevel { get; set; } = string.Empty;
        public int ValidationCount { get; set; }
        public string Trend { get; set; } = string.Empty; // Improving, Declining, Stable
    }

    public class AlertFilterDto
    {
        public string? AlertType { get; set; }
        public string? Severity { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public string? Metric { get; set; }
        public double? MinThreshold { get; set; }
        public double? MaxThreshold { get; set; }
    }
}
