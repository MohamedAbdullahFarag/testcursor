namespace Ikhtibar.Shared.DTOs
{
    public class VersionAnalyticsDto
    {
        public int QuestionId { get; set; }
        
        public int TotalVersions { get; set; } = 0;
        
        public string CurrentVersion { get; set; } = string.Empty;
        
        public DateTime? CurrentVersionCreatedAt { get; set; }
        
        public int? CurrentVersionCreatedBy { get; set; }
        
        public string? CurrentVersionCreatedByUserName { get; set; }
        
        public DateTime? FirstVersionCreatedAt { get; set; }
        
        public DateTime? LastVersionCreatedAt { get; set; }
        
        public int TotalEdits { get; set; } = 0;
        
        public decimal AverageEditsPerVersion { get; set; } = 0;
        
        public TimeSpan AverageTimeBetweenVersions { get; set; } = TimeSpan.Zero;
        
        public IEnumerable<VersionUsageDto>? VersionUsage { get; set; }
        
        public IEnumerable<VersionTrendDto>? VersionTrends { get; set; }
        
        public IEnumerable<EditorActivityDto>? EditorActivity { get; set; }
    }
    
    public class VersionTrendDto
    {
        public DateTime Date { get; set; }
        
        public int VersionsCreated { get; set; } = 0;
        
        public int EditsMade { get; set; } = 0;
        
        public int UniqueEditors { get; set; } = 0;
    }
    
    public class EditorActivityDto
    {
        public int UserId { get; set; }
        
        public string UserName { get; set; } = string.Empty;
        
        public int VersionsCreated { get; set; } = 0;
        
        public int EditsMade { get; set; } = 0;
        
        public DateTime? FirstEditAt { get; set; }
        
        public DateTime? LastEditAt { get; set; }
        
        public TimeSpan TotalEditingTime { get; set; } = TimeSpan.Zero;
    }
}
