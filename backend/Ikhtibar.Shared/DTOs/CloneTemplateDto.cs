using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class CloneTemplateDto
    {
        [Required]
        [MaxLength(200)]
        public string NewName { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? NewDescription { get; set; }
        
        [MaxLength(50)]
        public string? NewCode { get; set; }
        
        [Required]
        public int CreatedBy { get; set; }
        
        public bool IsPublic { get; set; } = false;
        
        public string? Notes { get; set; }
    }
}
