using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class ValidationRuleDto
    {
        public int Id { get; set; }
        public int QuestionTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RuleType { get; set; } = string.Empty; // Content, Format, Business
        public string RuleExpression { get; set; } = string.Empty;
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModified { get; set; }
    }

    public class CreateValidationRuleDto
    {
        [Required]
        public int QuestionTypeId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string RuleType { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string RuleExpression { get; set; } = string.Empty;
        
        public int Priority { get; set; } = 1;
        
        public bool IsActive { get; set; } = true;
    }

    public class UpdateValidationRuleDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(50)]
        public string? RuleType { get; set; }
        
        [MaxLength(1000)]
        public string? RuleExpression { get; set; }
        
        public int? Priority { get; set; }
        
        public bool? IsActive { get; set; }
    }
}
