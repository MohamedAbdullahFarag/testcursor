using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class QuestionSearchDto
    {
        public string? SearchTerm { get; set; }
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? DifficultyLevelId { get; set; }
        public int? QuestionStatusId { get; set; }
        public int? PrimaryTreeNodeId { get; set; }
        public string? Tags { get; set; }
        public string? Subject { get; set; }
        public string? GradeLevel { get; set; }
        public string? Language { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public int? CreatedBy { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "Desc";
    }

    public class SearchStatisticsDto
    {
        public int TotalQuestions { get; set; }
        public int MatchedQuestions { get; set; }
        public Dictionary<string, int> QuestionTypeCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> DifficultyLevelCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> SubjectCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> GradeLevelCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> StatusCounts { get; set; } = new Dictionary<string, int>();
        public List<string> PopularTags { get; set; } = new List<string>();
        public double AverageSearchTime { get; set; }
        public DateTime LastSearchTime { get; set; }
    }

    public class SearchStatisticsFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? DifficultyLevelId { get; set; }
        public string? Subject { get; set; }
        public string? GradeLevel { get; set; }
    }

    public class PopularSearchDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int SearchCount { get; set; }
        public int UniqueUsers { get; set; }
        public DateTime LastSearched { get; set; }
        public double AverageResults { get; set; }
        public string Trend { get; set; } = string.Empty; // Increasing, Decreasing, Stable
    }

    public class SearchTrendDto
    {
        public DateTime Date { get; set; }
        public int TotalSearches { get; set; }
        public int UniqueUsers { get; set; }
        public double AverageResults { get; set; }
        public double AverageSearchTime { get; set; }
        public int SuccessfulSearches { get; set; }
        public int FailedSearches { get; set; }
    }

    public class SearchPerformanceDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int SearchCount { get; set; }
        public double AverageSearchTime { get; set; }
        public double AverageResults { get; set; }
        public double UserSatisfactionScore { get; set; }
        public int ClickThroughRate { get; set; }
        public DateTime LastSearched { get; set; }
    }

    public class SearchPerformanceFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
        public double? MinSearchTime { get; set; }
        public double? MinSatisfactionScore { get; set; }
        public int? MinSearchCount { get; set; }
    }

    public class SearchIndexStatusDto
    {
        public bool IsIndexed { get; set; }
        public DateTime LastIndexed { get; set; }
        public int TotalIndexedQuestions { get; set; }
        public int PendingIndexQuestions { get; set; }
        public string IndexStatus { get; set; } = string.Empty; // Ready, Building, Error
        public string? LastError { get; set; }
        public double IndexSizeMB { get; set; }
        public TimeSpan AverageIndexTime { get; set; }
    }
}
