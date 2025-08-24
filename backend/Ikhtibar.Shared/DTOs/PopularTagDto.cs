namespace Ikhtibar.Shared.DTOs
{
    public class PopularTagDto
    {
        public int TagId { get; set; }
        
        public string TagName { get; set; } = string.Empty;
        
        public string Category { get; set; } = string.Empty;
        
        public int UsageCount { get; set; } = 0;
        
        public int QuestionCount { get; set; } = 0;
        
        public DateTime? LastUsedAt { get; set; }
        
        public decimal UsageTrend { get; set; } = 0; // Percentage change over time
    }
}
