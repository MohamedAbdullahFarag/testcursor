using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class QuestionFilterDto
    {
        public int? QuestionBankId { get; set; }
        
        public int? QuestionTypeId { get; set; }
        
        public int? DifficultyLevelId { get; set; }
        
        public int? StatusId { get; set; }
        
        public int? TreeNodeId { get; set; }
        
        public int? CreatedBy { get; set; }
        
        public string? SearchText { get; set; }
        
        public IEnumerable<string>? Tags { get; set; }
        
        public DateTime? FromDate { get; set; }
        
        public DateTime? ToDate { get; set; }
        
        public string? SortBy { get; set; } = "CreatedAt";
        
        public string? SortDirection { get; set; } = "DESC";
        
        [Required]
        [Range(1, 1000)]
        public int Page { get; set; } = 1;
        
        [Required]
        [Range(1, 100)]
        public int PageSize { get; set; } = 20;
        
        public bool IncludeAnswers { get; set; } = false;
        
        public bool IncludeMedia { get; set; } = false;
        
        public bool IncludeTags { get; set; } = false;
        
        public QuestionFilterDto Clone()
        {
            return new QuestionFilterDto
            {
                QuestionBankId = this.QuestionBankId,
                QuestionTypeId = this.QuestionTypeId,
                DifficultyLevelId = this.DifficultyLevelId,
                StatusId = this.StatusId,
                TreeNodeId = this.TreeNodeId,
                CreatedBy = this.CreatedBy,
                SearchText = this.SearchText,
                Tags = this.Tags?.ToList(),
                FromDate = this.FromDate,
                ToDate = this.ToDate,
                SortBy = this.SortBy,
                SortDirection = this.SortDirection,
                Page = this.Page,
                PageSize = this.PageSize,
                IncludeAnswers = this.IncludeAnswers,
                IncludeMedia = this.IncludeMedia,
                IncludeTags = this.IncludeTags
            };
        }
    }
}
