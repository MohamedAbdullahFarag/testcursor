namespace Ikhtibar.Shared.DTOs
{
    public class VersionStatisticsDto
    {
        public int QuestionId { get; set; }
        
        public int TotalVersions { get; set; } = 0;
        
        public string CurrentVersion { get; set; } = string.Empty;
        
        public DateTime? CurrentVersionCreatedAt { get; set; }
        
        public int? CurrentVersionCreatedBy { get; set; }
        
        public string? CurrentVersionCreatedByUserName { get; set; }
        
        public DateTime? FirstVersionCreatedAt { get; set; }
        
        public DateTime? LastVersionCreatedAt { get; set; }
        
        public int? MostActiveEditor { get; set; }
        
        public string? MostActiveEditorUserName { get; set; }
        
        public int TotalEdits { get; set; } = 0;
        
        public decimal AverageEditsPerVersion { get; set; } = 0;
        
        public TimeSpan AverageTimeBetweenVersions { get; set; } = TimeSpan.Zero;
        
        public IEnumerable<VersionUsageDto>? VersionUsage { get; set; }
    }

    // Compatibility aliases/DTOs used by Core interfaces
    public class VersionCompareResultDto
    {
        public int BaseVersionId { get; set; }
        public int CompareVersionId { get; set; }
        public string? DifferencesSummary { get; set; }
        public object? DetailedDiff { get; set; }
    }

    public class QuestionVersionStatisticsDto : VersionStatisticsDto { }
}
