using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class TemplateQualityDto
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public double OverallScore { get; set; }
        public double ContentScore { get; set; }
        public double FormatScore { get; set; }
        public double UsabilityScore { get; set; }
        public int ValidationCount { get; set; }
        public DateTime LastAssessment { get; set; }
        public string QualityLevel { get; set; } = string.Empty; // Poor, Fair, Good, Excellent
        public List<string> Strengths { get; set; } = new List<string>();
        public List<string> Weaknesses { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public Dictionary<string, double> DetailedScores { get; set; } = new Dictionary<string, double>();
    }

    public class TemplateQualityIssueDto
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
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

    public class TemplateQualityScoreDto
    {
        public int TemplateId { get; set; }
        public double OverallScore { get; set; }
        public double ContentScore { get; set; }
        public double FormatScore { get; set; }
        public double UsabilityScore { get; set; }
        public int ValidationCount { get; set; }
        public DateTime LastAssessment { get; set; }
        public string QualityLevel { get; set; } = string.Empty; // Poor, Fair, Good, Excellent
    }

    public class TemplateValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public double QualityScore { get; set; }
        public string QualityLevel { get; set; } = string.Empty;
        public List<string> Recommendations { get; set; } = new List<string>();
    }

    public class TemplateUsageStatisticsDto
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public int TotalUsage { get; set; }
        public int UniqueUsers { get; set; }
        public DateTime LastUsed { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
        public string MostCommonSubject { get; set; } = string.Empty;
        public string MostCommonGradeLevel { get; set; } = string.Empty;
    }

    public class TemplateCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TemplateCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TemplateAnalyticsDto
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public int TotalUsage { get; set; }
        public int UniqueUsers { get; set; }
        public double AverageQualityScore { get; set; }
        public DateTime FirstUsed { get; set; }
        public DateTime LastUsed { get; set; }
        public string GrowthTrend { get; set; } = string.Empty; // Increasing, Decreasing, Stable
        public Dictionary<string, int> UsageBySubject { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> UsageByGradeLevel { get; set; } = new Dictionary<string, int>();
    }

    public class TemplateTrendDto
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int UsageCount { get; set; }
        public int UniqueUsers { get; set; }
        public double AverageRating { get; set; }
        public double QualityScore { get; set; }
        public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly
    }

    public class TemplatePerformanceDto
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public double SuccessRate { get; set; }
        public double AverageCompletionTime { get; set; }
        public double UserSatisfactionScore { get; set; }
        public int TotalQuestionsCreated { get; set; }
        public double AverageQuestionQuality { get; set; }
        public DateTime LastPerformanceAssessment { get; set; }
    }

    public class ImportTemplateDto
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string SourceFormat { get; set; } = string.Empty;
        public string? SourcePath { get; set; }
        public string? ValidationRules { get; set; }
        public bool OverwriteExisting { get; set; }
        public string? ImportNotes { get; set; }
    }
}
