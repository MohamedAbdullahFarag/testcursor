using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class UpdateAnswerDto
    {
        public int Id { get; set; }
        
        [MaxLength(2000)]
        public string? Text { get; set; }
        
        public bool? IsCorrect { get; set; }
        
        [MaxLength(2000)]
        public string? Explanation { get; set; }
        
        public int? SortOrder { get; set; }
        
        public bool? IsActive { get; set; }
    }
}
