namespace Ikhtibar.Shared.DTOs
{
    public class VersionMetadataDto
    {
        public int QuestionId { get; set; }
        
        public string Version { get; set; } = string.Empty;
        
        public int CreatedBy { get; set; }
        
        public string? CreatedByUserName { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string? Notes { get; set; }
        
        public bool IsCurrentVersion { get; set; } = false;
        
        public string? ChangeSummary { get; set; }
        
        public string? ApprovalStatus { get; set; }
        
        public int? ApprovedBy { get; set; }
        
        public string? ApprovedByUserName { get; set; }
        
        public DateTime? ApprovedAt { get; set; }
    }
}
