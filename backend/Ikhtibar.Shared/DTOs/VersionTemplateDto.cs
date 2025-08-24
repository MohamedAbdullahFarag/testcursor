using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class VersionFilterDto
    {
        public int? QuestionId { get; set; }
        public string? Version { get; set; }
        public string? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsArchived { get; set; }
    }

    public class VersionHistoryDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int TotalVersions { get; set; }
        public string CurrentVersion { get; set; } = string.Empty;
        public string LatestVersion { get; set; } = string.Empty;
        public DateTime FirstVersionCreated { get; set; }
        public DateTime LastVersionCreated { get; set; }
        public List<QuestionVersionDto> Versions { get; set; } = new List<QuestionVersionDto>();
        public Dictionary<string, int> VersionCountsByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, DateTime> VersionTimeline { get; set; } = new Dictionary<string, DateTime>();
    }

    public class QuestionTemplateUsageDto
    {
        public bool Success { get; set; }
        public string Format { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public DateTime ExportedAt { get; set; }
        public int ExportedBy { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class ScheduleAnalyticsReportDto
    {
        public int Id { get; set; }
        public string ReportName { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty; // Performance, Usage, Trends, Quality
        public string Schedule { get; set; } = string.Empty; // Daily, Weekly, Monthly, Custom
        public string CronExpression { get; set; } = string.Empty;
        public DateTime NextRun { get; set; }
        public DateTime? LastRun { get; set; }
        public bool IsActive { get; set; }
        public string Recipients { get; set; } = string.Empty; // Comma-separated emails
        public string Format { get; set; } = "Excel"; // Excel, CSV, PDF
        public Dictionary<string, object>? Parameters { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
    }

    public class AnalyticsAlertDto
    {
        public int Id { get; set; }
        public string AlertName { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty; // Performance, Usage, Quality, System
        public string Condition { get; set; } = string.Empty; // Threshold, Trend, Anomaly
        public string Metric { get; set; } = string.Empty; // SuccessRate, UsageCount, QualityScore
        public string Operator { get; set; } = string.Empty; // GreaterThan, LessThan, Equals, NotEquals
        public double Threshold { get; set; }
        public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
        public bool IsActive { get; set; }
        public string Recipients { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? LastTriggered { get; set; }
        public int TriggerCount { get; set; }
    }
}
