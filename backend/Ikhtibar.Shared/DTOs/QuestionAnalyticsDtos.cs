using System;
using System.Collections.Generic;

namespace Ikhtibar.Shared.DTOs
{
    // Tag related DTOs
    public class TagUsageStatisticsDto
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
        public int TotalUses { get; set; }
        public int UniqueUsers { get; set; }
        public DateTime? FirstUsed { get; set; }
        public DateTime? LastUsed { get; set; }
        public Dictionary<string, int> UsageByMonth { get; set; } = new Dictionary<string, int>();
    }

    public class TagTrendDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int Count { get; set; }
    }

    public class TagAnalyticsDto
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int QuestionsThisMonth { get; set; }
        public int QuestionsThisYear { get; set; }
        public double GrowthRate { get; set; }
        public Dictionary<string, int> UsageBySubject { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> UsageByGrade { get; set; } = new Dictionary<string, int>();
        public List<string> TopQuestions { get; set; } = new List<string>();
    }

    // Difficulty related DTOs
    public class SearchQuestionDifficultiesDto
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public int? MinLevel { get; set; }
        public int? MaxLevel { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsPublic { get; set; }
        public int? Limit { get; set; } = 50;
    }

    public class DifficultyStatisticsDto
    {
        public int DifficultyId { get; set; }
        public string DifficultyName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int ActiveQuestions { get; set; }
        public int ArchivedQuestions { get; set; }
        public DateTime LastUsed { get; set; }
        public double AverageQualityScore { get; set; }
        public Dictionary<string, int> UsageByMonth { get; set; } = new Dictionary<string, int>();
    }

    public class DifficultyAnalyticsDto
    {
        public int DifficultyId { get; set; }
        public string DifficultyName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public double ChangeRate { get; set; }
        public Dictionary<string, int> UsageBySubject { get; set; } = new Dictionary<string, int>();
    }

    public class CloneDifficultyDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsPublic { get; set; } = false;
        public int CreatedBy { get; set; }
    }

    public class DifficultyDistributionDto
    {
        public int Level { get; set; }
        public int Count { get; set; }
    }

    public class DifficultyPerformanceDto
    {
        public int DifficultyId { get; set; }
        public double AverageScore { get; set; }
        public double PassRate { get; set; }
    }

    // Question type & validation DTOs
    public class SearchQuestionTypesDto
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public int? Limit { get; set; } = 50;
    }

    public class AddValidationRuleDto
    {
        public string RuleName { get; set; } = string.Empty;
        public string? RuleConfigJson { get; set; }
        public int CreatedBy { get; set; }
    }

    public class QuestionTypeStatisticsDto
    {
        public int QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public Dictionary<string, int> UsageByMonth { get; set; } = new Dictionary<string, int>();
    }

    public class QuestionTypeAnalyticsDto
    {
        public int QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public double GrowthRate { get; set; }
    }

    public class CloneQuestionTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsPublic { get; set; } = false;
        public int CreatedBy { get; set; }
    }
}
