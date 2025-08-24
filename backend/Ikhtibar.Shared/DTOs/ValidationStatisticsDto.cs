using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class ValidationStatisticsDto
    {
        public int TotalValidations { get; set; }
        public int PendingValidations { get; set; }
        public int ApprovedValidations { get; set; }
        public int RejectedValidations { get; set; }
        public int InProgressValidations { get; set; }
        public double AverageValidationTime { get; set; } // in hours
        public double AverageQualityScore { get; set; }
        public int TotalValidators { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class ValidationStatisticsFilterDto
    {
        public int? QuestionId { get; set; }
        public int? ValidatorId { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? QuestionBankId { get; set; }
        public double? MinQualityScore { get; set; }
        public double? MaxQualityScore { get; set; }
    }

    public class ValidationTrendDto
    {
        public DateTime Date { get; set; }
        public int TotalValidations { get; set; }
        public int ApprovedValidations { get; set; }
        public int RejectedValidations { get; set; }
        public double AverageQualityScore { get; set; }
        public double AverageValidationTime { get; set; }
        public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly
    }

    public class ValidationAnalyticsDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int TotalValidations { get; set; }
        public double AverageQualityScore { get; set; }
        public double AverageValidationTime { get; set; }
        public int TotalValidators { get; set; }
        public DateTime FirstValidation { get; set; }
        public DateTime LastValidation { get; set; }
        public string MostCommonIssues { get; set; } = string.Empty;
        public string QualityTrend { get; set; } = string.Empty; // Improving, Declining, Stable
    }

    public class ValidatorPerformanceDto
    {
        public int ValidatorId { get; set; }
        public string ValidatorName { get; set; } = string.Empty;
        public int TotalValidations { get; set; }
        public int ApprovedValidations { get; set; }
        public int RejectedValidations { get; set; }
        public double ApprovalRate { get; set; }
        public double AverageQualityScore { get; set; }
        public double AverageValidationTime { get; set; }
        public DateTime LastValidation { get; set; }
        public string PerformanceLevel { get; set; } = string.Empty; // Beginner, Intermediate, Expert
    }

    public class ValidationIssueSummaryDto
    {
        public string IssueType { get; set; } = string.Empty;
        public int OccurrenceCount { get; set; }
        public double PercentageOfTotal { get; set; }
        public string MostCommonDescription { get; set; } = string.Empty;
        public string AverageSeverity { get; set; } = string.Empty;
        public DateTime LastOccurrence { get; set; }
    }
}
