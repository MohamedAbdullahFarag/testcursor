namespace Ikhtibar.Shared.DTOs
{
    public class BulkOperationResult
    {
        public int TotalCount { get; set; } = 0;
        
        public int SuccessfulCount { get; set; } = 0;
        
        public int FailedCount { get; set; } = 0;
        
        public decimal SuccessRate { get; set; } = 0; // Percentage
        
        public IEnumerable<BulkOperationItemResult> Results { get; set; } = new List<BulkOperationItemResult>();
        
        public object? Data { get; set; }
        
        public string? Message { get; set; }
        
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class BulkOperationItemResult
    {
        public int Id { get; set; }
        
        public bool Success { get; set; } = false;
        
        public string Message { get; set; } = string.Empty;
        
        public string? ErrorCode { get; set; }
        
        public string? ErrorDetails { get; set; }
    }
}
