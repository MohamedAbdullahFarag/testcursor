namespace Ikhtibar.Shared.DTOs
{
    public class QuestionPerformanceDto
    {
        public int QuestionId { get; set; }
        
        public int TotalAttempts { get; set; } = 0;
        
        public int CorrectAttempts { get; set; } = 0;
        
        public int IncorrectAttempts { get; set; } = 0;
        
        public decimal SuccessRate { get; set; } = 0; // Percentage
        
        public decimal AverageScore { get; set; } = 0;
        
        public TimeSpan AverageTimeSpent { get; set; } = TimeSpan.Zero;
        
        public TimeSpan FastestAnswer { get; set; } = TimeSpan.Zero;
        
        public TimeSpan SlowestAnswer { get; set; } = TimeSpan.Zero;
        
        public decimal HighestScore { get; set; } = 0;
        
        public decimal LowestScore { get; set; } = 0;
        
        public DateTime? FirstAttemptAt { get; set; }
        
        public DateTime? LastAttemptAt { get; set; }
        
        public IEnumerable<PerformanceTrendDto>? PerformanceTrends { get; set; }
        
        public IEnumerable<DifficultyAnalysisDto>? DifficultyAnalysis { get; set; }
    }
}
