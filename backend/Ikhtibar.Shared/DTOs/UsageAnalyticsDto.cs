using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class UsageFilterDto
    {
        public int? QuestionId { get; set; }
        public int? UserId { get; set; }
        public int? ExamId { get; set; }
        public string? UsageType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? MinScore { get; set; }
        public int? MaxScore { get; set; }
        public int? MinTimeSpent { get; set; }
        public int? MaxTimeSpent { get; set; }
    }

    public class UserUsageAnalyticsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TotalQuestionsAttempted { get; set; }
        public int TotalExamsTaken { get; set; }
        public double AverageScore { get; set; }
        public double AverageTimeSpent { get; set; }
        public DateTime FirstUsage { get; set; }
        public DateTime LastUsage { get; set; }
        public string MostCommonSubject { get; set; } = string.Empty;
        public string MostCommonQuestionType { get; set; } = string.Empty;
        public string PerformanceTrend { get; set; } = string.Empty; // Improving, Declining, Stable
    }

    public class UserPerformanceDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public double Score { get; set; }
        public int TimeSpent { get; set; }
        public DateTime AttemptedAt { get; set; }
        public string QuestionType { get; set; } = string.Empty;
        public string DifficultyLevel { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class UserTrendDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int QuestionsAttempted { get; set; }
        public double AverageScore { get; set; }
        public double AverageTimeSpent { get; set; }
        public int ExamsTaken { get; set; }
        public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly
    }

    public class ExamUsageAnalyticsDto
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int TotalParticipants { get; set; }
        public int TotalQuestions { get; set; }
        public double AverageScore { get; set; }
        public double AverageCompletionTime { get; set; }
        public DateTime FirstAttempt { get; set; }
        public DateTime LastAttempt { get; set; }
        public string DifficultyDistribution { get; set; } = string.Empty;
        public string QuestionTypeDistribution { get; set; } = string.Empty;
    }

    public class ExamPerformanceDto
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public double AverageScore { get; set; }
        public double AverageTimeSpent { get; set; }
        public int TotalAttempts { get; set; }
        public int CorrectAnswers { get; set; }
        public double SuccessRate { get; set; }
        public string QuestionType { get; set; } = string.Empty;
        public string DifficultyLevel { get; set; } = string.Empty;
    }

    public class ExamTrendDto
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int Participants { get; set; }
        public double AverageScore { get; set; }
        public double AverageCompletionTime { get; set; }
        public int TotalAttempts { get; set; }
        public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly
    }

    public class PerformanceFilterDto
    {
        public int? QuestionId { get; set; }
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? DifficultyLevelId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? MinAttempts { get; set; }
        public double? MinSuccessRate { get; set; }
        public double? MaxSuccessRate { get; set; }
        public int? MinTimeSpent { get; set; }
        public int? MaxTimeSpent { get; set; }
    }

    public class AdvancedAnalyticsDto
    {
        public int TotalQuestions { get; set; }
        public int TotalUsers { get; set; }
        public int TotalExams { get; set; }
        public double OverallSuccessRate { get; set; }
        public double OverallAverageTime { get; set; }
        public Dictionary<string, double> SuccessRateByType { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> SuccessRateByDifficulty { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, int> UsageBySubject { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> UsageByGradeLevel { get; set; } = new Dictionary<string, int>();
        public List<QuestionPerformanceDto> TopPerformingQuestions { get; set; } = new List<QuestionPerformanceDto>();
        public List<QuestionPerformanceDto> LowestPerformingQuestions { get; set; } = new List<QuestionPerformanceDto>();
    }

    public class AdvancedAnalyticsFilterDto
    {
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? DifficultyLevelId { get; set; }
        public string? Subject { get; set; }
        public string? GradeLevel { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? MinQuestionCount { get; set; }
        public int? MinUserCount { get; set; }
        public double? MinSuccessRate { get; set; }
        public string? AnalysisType { get; set; } // Performance, Usage, Trends, Quality
    }
}
