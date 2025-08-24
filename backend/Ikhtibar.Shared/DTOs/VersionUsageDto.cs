namespace Ikhtibar.Shared.DTOs
{
    public class VersionUsageDto
    {
        public int QuestionId { get; set; }
        
        public string Version { get; set; } = string.Empty;
        
        public int UsageCount { get; set; } = 0;
        
        public DateTime? FirstUsedAt { get; set; }
        
        public DateTime? LastUsedAt { get; set; }
        
        public int UniqueUserCount { get; set; } = 0;
        
        public int UniqueExamCount { get; set; } = 0;
        
        public decimal AverageScore { get; set; } = 0;
        
        public TimeSpan AverageTimeSpent { get; set; } = TimeSpan.Zero;
    }
}
