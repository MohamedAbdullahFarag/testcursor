using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class DuplicateQuestionDto
    {
        public string? NewText { get; set; }
        
        public int? NewTreeNodeId { get; set; }
        
        [Required]
        public int CreatedBy { get; set; }
        
        public string? Notes { get; set; }
    }
}
