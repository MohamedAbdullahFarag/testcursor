namespace Ikhtibar.Shared.DTOs
{
    public class UsageTrendDto
    {
        public DateTime Date { get; set; }
        
        public int UsageCount { get; set; } = 0;
        
        public int ViewCount { get; set; } = 0;
        
        public int AnswerCount { get; set; } = 0;
        
        public int ReviewCount { get; set; } = 0;
        
        public int UniqueUserCount { get; set; } = 0;
        
        public int UniqueExamCount { get; set; } = 0;
        
        public decimal AverageScore { get; set; } = 0;
        
        public TimeSpan AverageTimeSpent { get; set; } = TimeSpan.Zero;
    }
}
