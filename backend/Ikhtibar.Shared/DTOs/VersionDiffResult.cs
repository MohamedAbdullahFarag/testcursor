namespace Ikhtibar.Shared.DTOs
{
    public class VersionDiffResult
    {
        public int QuestionId { get; set; }
        
        public string Version1 { get; set; } = string.Empty;
        
        public string Version2 { get; set; } = string.Empty;
        
        public bool HasChanges { get; set; } = false;
        
        public IEnumerable<VersionChangeDto> Changes { get; set; } = new List<VersionChangeDto>();
        
        public string? Summary { get; set; }
        
        public DateTime ComparedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class VersionChangeDto
    {
        public string Field { get; set; } = string.Empty;
        
        public string ChangeType { get; set; } = string.Empty; // Added, Modified, Deleted
        
        public string? OldValue { get; set; }
        
        public string? NewValue { get; set; }
        
        public string? Description { get; set; }
        
        public string? Severity { get; set; } // Low, Medium, High, Critical
    }
}
