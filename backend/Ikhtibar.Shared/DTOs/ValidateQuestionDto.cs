using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class ValidateQuestionDto
    {
        [Required]
        public int QuestionId { get; set; }
        
        public string? Content { get; set; }
        
        public string? Answers { get; set; }
        
        public string? Media { get; set; }
        
        public string? BusinessRules { get; set; }
        
        [Required]
        public int ValidatedBy { get; set; }
        
        public string? Notes { get; set; }
    }
}
