namespace Ikhtibar.Shared.DTOs
{
    public class QuestionTypeCountDto
    {
        public int QuestionTypeId { get; set; }
        
        public string QuestionTypeName { get; set; } = string.Empty;
        
        public int Count { get; set; } = 0;
        
        public decimal Percentage { get; set; } = 0; // Percentage of total
    }
}
