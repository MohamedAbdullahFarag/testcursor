using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class UsageReportDto
    {
        public string ReportTitle { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalUsers { get; set; }
        public int TotalExams { get; set; }
        public double OverallSuccessRate { get; set; }
        public double OverallAverageTime { get; set; }
        public List<QuestionUsageDto> TopUsedQuestions { get; set; } = new List<QuestionUsageDto>();
        public List<UserUsageAnalyticsDto> TopUsers { get; set; } = new List<UserUsageAnalyticsDto>();
        public List<ExamUsageAnalyticsDto> TopExams { get; set; } = new List<ExamUsageAnalyticsDto>();
        public Dictionary<string, int> UsageByQuestionType { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> UsageByDifficulty { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> UsageBySubject { get; set; } = new Dictionary<string, int>();
    }

    public class UsageReportFilterDto
    {
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? DifficultyLevelId { get; set; }
        public string? Subject { get; set; }
        public string? GradeLevel { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? MinUsageCount { get; set; }
        public int? MaxUsageCount { get; set; }
        public double? MinSuccessRate { get; set; }
        public double? MaxSuccessRate { get; set; }
        public string? ReportType { get; set; } // Summary, Detailed, Comparative
    }

    public class PerformanceReportDto
    {
        public string ReportTitle { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalAttempts { get; set; }
        public double OverallSuccessRate { get; set; }
        public double OverallAverageScore { get; set; }
        public double OverallAverageTime { get; set; }
        public List<QuestionPerformanceDto> TopPerformingQuestions { get; set; } = new List<QuestionPerformanceDto>();
        public List<QuestionPerformanceDto> LowestPerformingQuestions { get; set; } = new List<QuestionPerformanceDto>();
        public List<UserPerformanceDto> TopPerformingUsers { get; set; } = new List<UserPerformanceDto>();
        public Dictionary<string, double> SuccessRateByQuestionType { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> SuccessRateByDifficulty { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> SuccessRateBySubject { get; set; } = new Dictionary<string, double>();
    }

    public class PerformanceReportFilterDto
    {
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? DifficultyLevelId { get; set; }
        public string? Subject { get; set; }
        public string? GradeLevel { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? MinAttempts { get; set; }
        public double? MinSuccessRate { get; set; }
        public double? MaxSuccessRate { get; set; }
        public double? MinAverageScore { get; set; }
        public double? MaxAverageScore { get; set; }
        public string? ReportType { get; set; } // Summary, Detailed, Comparative
    }

    public class AnalyticsReportDto
    {
        public string ReportTitle { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string AnalysisType { get; set; } = string.Empty; // Performance, Usage, Trends, Quality
        public int TotalDataPoints { get; set; }
        public Dictionary<string, object> KeyMetrics { get; set; } = new Dictionary<string, object>();
        public List<object> DetailedData { get; set; } = new List<object>();
        public List<string> Insights { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public Dictionary<string, object> Charts { get; set; } = new Dictionary<string, object>();
    }

    public class AnalyticsReportFilterDto
    {
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? DifficultyLevelId { get; set; }
        public string? Subject { get; set; }
        public string? GradeLevel { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? AnalysisType { get; set; } // Performance, Usage, Trends, Quality
        public string? Granularity { get; set; } // Daily, Weekly, Monthly, Quarterly
        public bool IncludeCharts { get; set; } = true;
        public bool IncludeInsights { get; set; } = true;
        public bool IncludeRecommendations { get; set; } = true;
    }

    public class ExportOptionsDto
    {
        public string Format { get; set; } = "Excel"; // Excel, CSV, JSON, XML, PDF
        public bool IncludeHeaders { get; set; } = true;
        public bool IncludeMetadata { get; set; } = true;
        public bool IncludeCharts { get; set; } = false;
        public string? CustomFileName { get; set; }
        public Dictionary<string, object>? CustomizationOptions { get; set; }
        public string? SheetName { get; set; } // For Excel
        public string? Delimiter { get; set; } // For CSV
        public bool PrettyPrint { get; set; } = false; // For JSON
        public string? Encoding { get; set; } = "UTF-8";
    }
}
