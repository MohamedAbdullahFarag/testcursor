namespace Ikhtibar.Shared.DTOs
{
    public class QuestionUsageStatisticsDto
    {
        public int QuestionId { get; set; }
        
        public int TotalUsageCount { get; set; } = 0;
        
        public int ViewCount { get; set; } = 0;
        
        public int AnswerCount { get; set; } = 0;
        
        public int ReviewCount { get; set; } = 0;
        
        public decimal AverageScore { get; set; } = 0;
        
        public TimeSpan AverageTimeSpent { get; set; } = TimeSpan.Zero;
        
        public DateTime? FirstUsedAt { get; set; }
        
        public DateTime? LastUsedAt { get; set; }
        
        public int UniqueUserCount { get; set; } = 0;
        
        public int UniqueExamCount { get; set; } = 0;
        
        public IEnumerable<UsageTrendDto>? UsageTrends { get; set; }
        
        public IEnumerable<PerformanceTrendDto>? PerformanceTrends { get; set; }
    }
}
