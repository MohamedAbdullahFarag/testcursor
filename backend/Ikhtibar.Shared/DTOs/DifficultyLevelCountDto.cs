namespace Ikhtibar.Shared.DTOs
{
    public class DifficultyLevelCountDto
    {
        public int DifficultyLevelId { get; set; }
        
        public string DifficultyLevelName { get; set; } = string.Empty;
        
        public int Count { get; set; } = 0;
        
        public decimal Percentage { get; set; } = 0; // Percentage of total
    }
}
