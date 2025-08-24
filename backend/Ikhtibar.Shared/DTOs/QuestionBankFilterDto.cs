using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class QuestionBankFilterDto
    {
        public int? CreatedBy { get; set; }
        
        public bool? IsPublic { get; set; }
        
        public string? Subject { get; set; }
        
        public string? GradeLevel { get; set; }
        
        public string? Language { get; set; }
        
        public string? SearchText { get; set; }
        
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
    }
}
