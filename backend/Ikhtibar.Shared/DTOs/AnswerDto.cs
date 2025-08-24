using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class AnswerDto
    {
        public int Id { get; set; }
        
        [Required]
        public int QuestionId { get; set; }
        
        [Required]
        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;
        
        [Required]
        public bool IsCorrect { get; set; } = false;
        
        [MaxLength(2000)]
        public string? Explanation { get; set; }
        
        [Required]
        public int SortOrder { get; set; } = 0;
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
