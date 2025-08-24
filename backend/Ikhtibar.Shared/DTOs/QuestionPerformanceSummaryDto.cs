using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class QuestionPerformanceSummaryDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public double AverageScore { get; set; }
        public double AverageTimeSpent { get; set; }
        public int TotalAttempts { get; set; }
        public int CorrectAnswers { get; set; }
        public double SuccessRate { get; set; }
        public DateTime LastUsed { get; set; }
        public int UsageCount { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
    }

    public class PerformanceSummaryFilterDto
    {
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? DifficultyLevelId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MinAttempts { get; set; }
        public double? MinSuccessRate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
