namespace Ikhtibar.Shared.DTOs
{
    public class QuestionCriteriaDto
    {
        public string? Text { get; set; }
        
        public IEnumerable<string>? Tags { get; set; }
        
        public int? QuestionTypeId { get; set; }
        
        public int? DifficultyLevelId { get; set; }
        
        public int? StatusId { get; set; }
        
        public int? TreeNodeId { get; set; }
        
        public int? QuestionBankId { get; set; }
        
        public int? CreatedBy { get; set; }
        
        public decimal? MinPoints { get; set; }
        
        public decimal? MaxPoints { get; set; }
        
        public int? MinEstimatedTime { get; set; }
        
        public int? MaxEstimatedTime { get; set; }
        
        public DateTime? CreatedFrom { get; set; }
        
        public DateTime? CreatedTo { get; set; }
        
        public DateTime? ModifiedFrom { get; set; }
        
        public DateTime? ModifiedTo { get; set; }
        
        public bool? HasMedia { get; set; }
        
        public bool? HasAnswers { get; set; }
        
        public bool? HasSolution { get; set; }
        
        public string? Language { get; set; }
        
        public string? Subject { get; set; }
        
        public string? GradeLevel { get; set; }
    }
}
