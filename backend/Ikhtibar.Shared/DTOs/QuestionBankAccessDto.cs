using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class QuestionBankAccessDto
    {
        public int Id { get; set; }
        
        [Required]
        public int QuestionBankId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        public string? UserName { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Permission { get; set; } = string.Empty;
        
        [Required]
        public DateTime GrantedAt { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
