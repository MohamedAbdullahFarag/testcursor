using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class QuestionUsageDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public DateTime LastUsed { get; set; }
        public string MostCommonUsageType { get; set; } = string.Empty;
        public int UniqueUsers { get; set; }
        public double AverageScore { get; set; }
        public double AverageTimeSpent { get; set; }
    }

    public class UsageSummaryFilterDto
    {
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? UsageType { get; set; }
        public int? MinUsageCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
