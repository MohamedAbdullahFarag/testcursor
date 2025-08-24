namespace Ikhtibar.Shared.DTOs
{
    public class SearchOptionsDto
    {
        public int MaxResults { get; set; } = 100;
        
        public bool IncludeInactive { get; set; } = false;
        
        public bool IncludeArchived { get; set; } = false;
        
        public string? SortBy { get; set; } = "Relevance";
        
        public string? SortDirection { get; set; } = "DESC";
        
        public bool UseFuzzySearch { get; set; } = true;
        
        public decimal MinRelevanceScore { get; set; } = 0.1m;
        
        public IEnumerable<string>? ExcludeTags { get; set; }
        
        public IEnumerable<int>? ExcludeQuestionIds { get; set; }
        
        public DateTime? FromDate { get; set; }
        
        public DateTime? ToDate { get; set; }
    }
}
