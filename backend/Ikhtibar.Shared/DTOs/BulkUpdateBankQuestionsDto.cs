using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class BulkUpdateBankQuestionsDto
    {
        [Required]
        public int QuestionBankId { get; set; }
        
        [Required]
        public IEnumerable<int> QuestionIds { get; set; } = new List<int>();
        
        public int? TargetCategoryId { get; set; }
        
        public int? TargetDifficultyLevelId { get; set; }
        
        public int? TargetQuestionTypeId { get; set; }
        
        public decimal? TargetPoints { get; set; }
        
        public string? Tags { get; set; }
        
        public string? MetadataJson { get; set; }
        
        [Required]
        public int UpdatedBy { get; set; }
        
        public string? Notes { get; set; }
    }
}
