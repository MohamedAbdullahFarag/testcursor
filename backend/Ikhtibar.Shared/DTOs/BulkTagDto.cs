using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class BulkTagDto
    {
        [Required]
        public IEnumerable<int> QuestionIds { get; set; } = new List<int>();
        
        [Required]
        public IEnumerable<int> TagIds { get; set; } = new List<int>();
        
        [Required]
        public int AppliedBy { get; set; }
        
        public string? Notes { get; set; }
    }
}
