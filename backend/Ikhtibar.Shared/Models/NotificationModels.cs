namespace Ikhtibar.Shared.Models;

/// <summary>
/// System-wide notification statistics
/// </summary>
public class NotificationSystemStats
{
    public int TotalNotifications { get; set; }
    public int SentNotifications { get; set; }
    public int FailedNotifications { get; set; }
    public int PendingNotifications { get; set; }
    public decimal OverallSuccessRate { get; set; }
    public Dictionary<string, int> NotificationsByType { get; set; } = new();
    public Dictionary<string, int> NotificationsByChannel { get; set; } = new();
    public Dictionary<string, decimal> SuccessRateByChannel { get; set; } = new();
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public TimeSpan ReportPeriod => ToDate - FromDate;
}

/// <summary>
/// Result of cleanup operations
/// </summary>
public class CleanupResult
{
    public int NotificationsDeleted { get; set; }
    public int HistoryRecordsDeleted { get; set; }
    public int TotalRecordsDeleted => NotificationsDeleted + HistoryRecordsDeleted;
    public DateTime CleanupDate { get; set; }
    public TimeSpan CleanupDuration { get; set; }
    public bool IsSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
}
