using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class BulkUpdateQuestionsDto
    {
        [Required]
        public IEnumerable<int> QuestionIds { get; set; } = new List<int>();
        
        public string? Text { get; set; }
        
        public int? QuestionTypeId { get; set; }
        
        public int? DifficultyLevelId { get; set; }
        
        public string? Solution { get; set; }
        
        public int? EstimatedTimeSec { get; set; }
        
        public decimal? Points { get; set; }
        
        public string? Tags { get; set; }
        
        public string? MetadataJson { get; set; }
        
        [Required]
        public int UpdatedBy { get; set; }
        
        public string? Notes { get; set; }
    }
}
