using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class TreeNodeAnalyticsDto
    {
        public int TreeNodeId { get; set; }
        public string NodeName { get; set; } = string.Empty;
        public string NodePath { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
        public double AverageDifficulty { get; set; }
        public double AverageSuccessRate { get; set; }
        public double AverageTimeSpent { get; set; }
        public int TotalUsage { get; set; }
        public int UniqueUsers { get; set; }
        public DateTime LastUsed { get; set; }
        public string PerformanceTrend { get; set; } = string.Empty; // Improving, Declining, Stable
    }

    public class TypeAnalyticsDto
    {
        public int QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
        public double AverageDifficulty { get; set; }
        public double AverageSuccessRate { get; set; }
        public double AverageTimeSpent { get; set; }
        public int TotalUsage { get; set; }
        public int UniqueUsers { get; set; }
        public DateTime LastUsed { get; set; }
        public Dictionary<string, int> UsageByDifficulty { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, double> SuccessRateByDifficulty { get; set; } = new Dictionary<string, double>();
    }

    public class UserQuestionAnalyticsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int QuestionsAttempted { get; set; }
        public int QuestionsCorrect { get; set; }
        public double SuccessRate { get; set; }
        public double AverageTimeSpent { get; set; }
        public double AverageScore { get; set; }
        public DateTime FirstAttempt { get; set; }
        public DateTime LastAttempt { get; set; }
        public string PerformanceTrend { get; set; } = string.Empty; // Improving, Declining, Stable
        public Dictionary<string, int> AttemptsByQuestionType { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, double> SuccessRateByQuestionType { get; set; } = new Dictionary<string, double>();
    }

    public class CreatorAnalyticsDto
    {
        public int CreatorId { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public int QuestionsCreated { get; set; }
        public int QuestionsPublished { get; set; }
        public double PublicationRate { get; set; }
        public double AverageQualityScore { get; set; }
        public double AverageSuccessRate { get; set; }
        public int TotalUsage { get; set; }
        public DateTime FirstQuestionCreated { get; set; }
        public DateTime LastQuestionCreated { get; set; }
        public string PerformanceTrend { get; set; } = string.Empty; // Improving, Declining, Stable
    }

    public class QuestionSimilarityDto
    {
        public int QuestionId1 { get; set; }
        public int QuestionId2 { get; set; }
        public string Question1Text { get; set; } = string.Empty;
        public string Question2Text { get; set; } = string.Empty;
        public double SimilarityScore { get; set; }
        public string SimilarityType { get; set; } = string.Empty; // Content, Structure, Difficulty, Usage
        public List<string> CommonElements { get; set; } = new List<string>();
        public List<string> Differences { get; set; } = new List<string>();
        public bool IsPotentialDuplicate { get; set; }
        public string Recommendation { get; set; } = string.Empty;
    }

    public class ClusteringOptionsDto
    {
        public string ClusteringMethod { get; set; } = "KMeans"; // KMeans, Hierarchical, DBSCAN
        public int NumberOfClusters { get; set; } = 5;
        public double MinSimilarityThreshold { get; set; } = 0.7;
        public string SimilarityMetric { get; set; } = "Cosine"; // Cosine, Euclidean, Jaccard
        public bool IncludeContentAnalysis { get; set; } = true;
        public bool IncludeUsagePatterns { get; set; } = true;
        public bool IncludePerformanceMetrics { get; set; } = true;
        public int MaxIterations { get; set; } = 100;
        public double ConvergenceThreshold { get; set; } = 0.001;
    }

    public class QuestionClusterDto
    {
        public int ClusterId { get; set; }
        public string ClusterName { get; set; } = string.Empty;
        public string ClusterDescription { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
        public double AverageDifficulty { get; set; }
        public double AverageSuccessRate { get; set; }
        public double AverageQualityScore { get; set; }
        public List<string> CommonTags { get; set; } = new List<string>();
        public List<string> CommonQuestionTypes { get; set; } = new List<string>();
        public List<int> QuestionIds { get; set; } = new List<int>();
        public Dictionary<string, object> ClusterCharacteristics { get; set; } = new Dictionary<string, object>();
    }

    public class QuestionRecommendationDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public double RecommendationScore { get; set; }
        public string RecommendationReason { get; set; } = string.Empty;
        public string RecommendationType { get; set; } = string.Empty; // Similar, Complementary, NextLevel
        public List<string> SupportingFactors { get; set; } = new List<string>();
        public Dictionary<string, double> FactorScores { get; set; } = new Dictionary<string, double>();
        public bool IsPersonalized { get; set; }
        public int? TargetUserId { get; set; }
    }

    public class PatternAnalysisOptionsDto
    {
        public string PatternType { get; set; } = "Usage"; // Usage, Performance, Content, Temporal
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Granularity { get; set; } = "Daily"; // Hourly, Daily, Weekly, Monthly
        public int MinPatternLength { get; set; } = 3;
        public double MinConfidence { get; set; } = 0.6;
        public double MinSupport { get; set; } = 0.1;
        public bool IncludeSeasonalPatterns { get; set; } = true;
        public bool IncludeTrendAnalysis { get; set; } = true;
        public int MaxPatterns { get; set; } = 100;
    }

    public class QuestionPatternDto
    {
        public int PatternId { get; set; }
        public string PatternName { get; set; } = string.Empty;
        public string PatternType { get; set; } = string.Empty; // Usage, Performance, Content, Temporal
        public string PatternDescription { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public double Support { get; set; }
        public List<string> PatternElements { get; set; } = new List<string>();
        public Dictionary<string, object> PatternData { get; set; } = new Dictionary<string, object>();
        public DateTime FirstObserved { get; set; }
        public DateTime LastObserved { get; set; }
        public int OccurrenceCount { get; set; }
        public string Trend { get; set; } = string.Empty; // Increasing, Decreasing, Stable, Cyclical
    }

    public class DashboardFilterDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public int? DifficultyLevelId { get; set; }
        public string? Subject { get; set; }
        public string? GradeLevel { get; set; }
        public string? Language { get; set; }
        public int? UserId { get; set; }
        public string? UserRole { get; set; }
        public string? Granularity { get; set; } = "Daily"; // Hourly, Daily, Weekly, Monthly
        public bool IncludeCharts { get; set; } = true;
        public bool IncludeTables { get; set; } = true;
        public bool IncludeMetrics { get; set; } = true;
    }

    public class AnalyticsDashboardDto
    {
        public string DashboardTitle { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Dictionary<string, object> KeyMetrics { get; set; } = new Dictionary<string, object>();
        public List<object> Charts { get; set; } = new List<object>();
        public List<object> Tables { get; set; } = new List<object>();
        public List<string> Insights { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();
    }
}
