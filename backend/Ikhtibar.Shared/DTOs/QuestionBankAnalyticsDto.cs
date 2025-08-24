namespace Ikhtibar.Shared.DTOs
{
    public class QuestionBankAnalyticsDto
    {
        public int TotalQuestions { get; set; } = 0;
        
        public int ActiveQuestions { get; set; } = 0;
        
        public int PublishedQuestions { get; set; } = 0;
        
        public decimal AverageDifficulty { get; set; } = 0;
        
        public decimal AveragePoints { get; set; } = 0;
        
        public int TotalUsageCount { get; set; } = 0;
        
        public decimal AverageScore { get; set; } = 0;
        
        public TimeSpan AverageTimeSpent { get; set; } = TimeSpan.Zero;
        
        public DateTime? LastQuestionAdded { get; set; }
        
        public DateTime? LastQuestionModified { get; set; }
        
        public DateTime? LastQuestionUsed { get; set; }
        
        public IEnumerable<QuestionTypeCountDto>? QuestionTypeDistribution { get; set; }
        
        public IEnumerable<DifficultyLevelCountDto>? DifficultyDistribution { get; set; }
        
        public IEnumerable<UsageTrendDto>? UsageTrends { get; set; }
        
        public IEnumerable<PerformanceTrendDto>? PerformanceTrends { get; set; }
    }
}
