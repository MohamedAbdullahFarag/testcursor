using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class CreateAnswerDto
    {
        [Required]
        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;
        
        [Required]
        public bool IsCorrect { get; set; } = false;
        
        [MaxLength(2000)]
        public string? Explanation { get; set; }
        
        [Required]
        public int SortOrder { get; set; } = 0;
    }
}
