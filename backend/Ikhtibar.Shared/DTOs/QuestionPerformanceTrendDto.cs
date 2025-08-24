using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class QuestionPerformanceTrendDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public double AverageScore { get; set; }
        public double AverageTimeSpent { get; set; }
        public int Attempts { get; set; }
        public int CorrectAnswers { get; set; }
        public double SuccessRate { get; set; }
        public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly
    }
}
