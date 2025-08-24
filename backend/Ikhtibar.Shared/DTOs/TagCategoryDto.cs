namespace Ikhtibar.Shared.DTOs
{
    public class TagCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public int TagCount { get; set; } = 0;
        
        public int ActiveTagCount { get; set; } = 0;
        
        public DateTime? FirstTagCreatedAt { get; set; }
        
        public DateTime? LastTagCreatedAt { get; set; }
        
        public int TotalUsageCount { get; set; } = 0;
    }
}
