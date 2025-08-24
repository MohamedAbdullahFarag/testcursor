namespace Ikhtibar.Shared.DTOs
{
    public class DifficultyAnalysisDto
    {
        public int DifficultyLevelId { get; set; }
        
        public string DifficultyLevelName { get; set; } = string.Empty;
        
        public int AttemptCount { get; set; } = 0;
        
        public decimal SuccessRate { get; set; } = 0; // Percentage
        
        public decimal AverageScore { get; set; } = 0;
        
        public TimeSpan AverageTimeSpent { get; set; } = TimeSpan.Zero;
        
        public decimal DifficultyScore { get; set; } = 0; // Calculated difficulty based on performance
    }
}
