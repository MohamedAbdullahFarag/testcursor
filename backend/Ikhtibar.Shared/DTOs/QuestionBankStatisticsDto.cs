namespace Ikhtibar.Shared.DTOs
{
    public class QuestionBankStatisticsDto
    {
        public int TotalQuestions { get; set; } = 0;
        
        public int ActiveQuestions { get; set; } = 0;
        
        public int PublishedQuestions { get; set; } = 0;
        
        public int DraftQuestions { get; set; } = 0;
        
        public int ArchivedQuestions { get; set; } = 0;
        
        public decimal AverageDifficulty { get; set; } = 0;
        
        public decimal AveragePoints { get; set; } = 0;
        
        public int TotalUsageCount { get; set; } = 0;
        
        public DateTime? LastQuestionAdded { get; set; }
        
        public DateTime? LastQuestionModified { get; set; }
    }
}
