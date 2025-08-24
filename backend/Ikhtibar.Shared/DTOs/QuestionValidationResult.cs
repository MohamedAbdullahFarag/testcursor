namespace Ikhtibar.Shared.DTOs
{
    public class QuestionValidationResult
    {
        public int QuestionId { get; set; }
        
        public bool IsValid { get; set; } = false;
        
        public int Score { get; set; } = 0; // 0-100
        
        public IEnumerable<ValidationError> ValidationErrors { get; set; } = new List<ValidationError>();
        
        public IEnumerable<ValidationWarning> ValidationWarnings { get; set; } = new List<ValidationWarning>();
        
        public string? Notes { get; set; }
        
        public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;
        
        public int ValidatedBy { get; set; }
    }
    
    public class ValidationError
    {
        public string Code { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        
        public string? Field { get; set; }
        
        public string? Severity { get; set; }
    }
    
    public class ValidationWarning
    {
        public string Code { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        
        public string? Field { get; set; }
        
        public string? Suggestion { get; set; }
    }
}
