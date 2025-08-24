using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class CreateVersionDto
    {
        [Required]
        [MaxLength(50)]
        public string Version { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;
        
        [MaxLength(2000)]
        public string? Solution { get; set; }
        
        public string? AnswersJson { get; set; }
        
        public string? MediaJson { get; set; }
        
        [Required]
        public int CreatedBy { get; set; }
        
        public string? Notes { get; set; }
    }
}
