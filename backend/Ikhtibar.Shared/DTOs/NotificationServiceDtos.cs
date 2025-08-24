using System.ComponentModel.DataAnnotations;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for delivery result information
/// </summary>
public class DeliveryResultDto
{
    /// <summary>
    /// Delivery ID for tracking
    /// </summary>
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>
    /// Whether delivery was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Delivery channel used
    /// </summary>
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Delivery status
    /// </summary>
    public NotificationDeliveryStatus Status { get; set; }

    /// <summary>
    /// Error message if delivery failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Delivery cost if applicable
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// External provider message ID
    /// </summary>
    public string? ExternalMessageId { get; set; }

    /// <summary>
    /// When delivery was attempted
    /// </summary>
    public DateTime DeliveredAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Recipient information
    /// </summary>
    public string? Recipient { get; set; }

    /// <summary>
    /// Additional delivery metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for sending notifications to specific users
/// </summary>
public class SendToUsersDto
{
    /// <summary>
    /// List of user IDs to send notification to
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "At least one user ID is required")]
    public List<int> UserIds { get; set; } = new();

    /// <summary>
    /// Notification type
    /// </summary>
    [Required]
    public NotificationType Type { get; set; }

    /// <summary>
    /// Notification title
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Notification content
    /// </summary>
    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Template ID to use (optional)
    /// </summary>
    public int? TemplateId { get; set; }

    /// <summary>
    /// Template variables (if using template)
    /// </summary>
    public Dictionary<string, object> Variables { get; set; } = new();

    /// <summary>
    /// Channels to use for delivery
    /// </summary>
    public List<NotificationChannel> Channels { get; set; } = new();

    /// <summary>
    /// Priority level
    /// </summary>
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Whether to respect user preferences
    /// </summary>
    public bool RespectUserPreferences { get; set; } = true;

    /// <summary>
    /// Whether to respect quiet hours
    /// </summary>
    public bool RespectQuietHours { get; set; } = true;

    /// <summary>
    /// Optional metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for scheduling notifications
/// </summary>
public class ScheduleNotificationDto
{
    /// <summary>
    /// Base notification data
    /// </summary>
    [Required]
    public CreateNotificationDto Notification { get; set; } = new();

    /// <summary>
    /// When to send the notification
    /// </summary>
    [Required]
    public DateTime ScheduledFor { get; set; }

    /// <summary>
    /// Timezone for scheduled time
    /// </summary>
    [StringLength(100)]
    public string? Timezone { get; set; }

    /// <summary>
    /// Whether to reschedule if user is in quiet hours
    /// </summary>
    public bool RescheduleForQuietHours { get; set; } = true;

    /// <summary>
    /// Maximum reschedule attempts
    /// </summary>
    [Range(0, 10)]
    public int MaxRescheduleAttempts { get; set; } = 3;

    /// <summary>
    /// Optional recurring pattern
    /// </summary>
    public RecurrencePattern? Recurrence { get; set; }
}

/// <summary>
/// DTO for sending bulk notifications (alternative name to avoid conflict)
/// </summary>
public class SendBulkNotificationDto
{
    /// <summary>
    /// List of notifications to send
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<CreateNotificationDto> Notifications { get; set; } = new();

    /// <summary>
    /// Whether to process asynchronously
    /// </summary>
    public bool ProcessAsync { get; set; } = true;

    /// <summary>
    /// Batch size for processing
    /// </summary>
    [Range(1, 1000)]
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Maximum concurrent operations
    /// </summary>
    [Range(1, 50)]
    public int MaxConcurrency { get; set; } = 10;

    /// <summary>
    /// Whether to stop on first error
    /// </summary>
    public bool StopOnError { get; set; } = false;

    /// <summary>
    /// Callback URL for completion notification
    /// </summary>
    [Url]
    public string? CallbackUrl { get; set; }
}

/// <summary>
/// DTO for notification analytics
/// </summary>
public class NotificationAnalyticsDto
{
    /// <summary>
    /// Total notifications sent
    /// </summary>
    public int TotalSent { get; set; }

    /// <summary>
    /// Successfully delivered notifications
    /// </summary>
    public int TotalDelivered { get; set; }

    /// <summary>
    /// Failed notifications
    /// </summary>
    public int TotalFailed { get; set; }

    /// <summary>
    /// Pending notifications
    /// </summary>
    public int TotalPending { get; set; }

    /// <summary>
    /// Delivery rate percentage
    /// </summary>
    public double DeliveryRate { get; set; }

    /// <summary>
    /// Open rate percentage
    /// </summary>
    public double OpenRate { get; set; }

    /// <summary>
    /// Click rate percentage
    /// </summary>
    public double ClickRate { get; set; }

    /// <summary>
    /// Breakdown by notification type
    /// </summary>
    public Dictionary<string, NotificationTypeStats> TypeBreakdown { get; set; } = new();

    /// <summary>
    /// Breakdown by channel
    /// </summary>
    public Dictionary<string, ChannelStats> ChannelBreakdown { get; set; } = new();

    /// <summary>
    /// Daily statistics
    /// </summary>
    public List<DailyNotificationStats> DailyStats { get; set; } = new();

    /// <summary>
    /// Analysis period
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Analysis period end
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// Total cost for the period
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Average cost per notification
    /// </summary>
    public decimal AverageCostPerNotification { get; set; }
}

/// <summary>
/// DTO for user notification statistics
/// </summary>
public class UserNotificationStatsDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Total notifications received
    /// </summary>
    public int TotalReceived { get; set; }

    /// <summary>
    /// Total notifications opened
    /// </summary>
    public int TotalOpened { get; set; }

    /// <summary>
    /// Total notifications clicked
    /// </summary>
    public int TotalClicked { get; set; }

    /// <summary>
    /// Open rate percentage
    /// </summary>
    public double OpenRate { get; set; }

    /// <summary>
    /// Click rate percentage
    /// </summary>
    public double ClickRate { get; set; }

    /// <summary>
    /// Most active notification type
    /// </summary>
    public string? MostActiveType { get; set; }

    /// <summary>
    /// Preferred notification channel
    /// </summary>
    public string? PreferredChannel { get; set; }

    /// <summary>
    /// Average notifications per day
    /// </summary>
    public double AveragePerDay { get; set; }

    /// <summary>
    /// Last notification received
    /// </summary>
    public DateTime? LastNotificationAt { get; set; }

    /// <summary>
    /// Last notification opened
    /// </summary>
    public DateTime? LastOpenedAt { get; set; }

    /// <summary>
    /// Statistics period
    /// </summary>
    public int DaysAnalyzed { get; set; }

    /// <summary>
    /// Breakdown by notification type
    /// </summary>
    public Dictionary<string, int> TypeBreakdown { get; set; } = new();

    /// <summary>
    /// Breakdown by channel
    /// </summary>
    public Dictionary<string, int> ChannelBreakdown { get; set; } = new();
}

/// <summary>
/// DTO for notification performance metrics
/// </summary>
public class NotificationPerformanceDto
{
    /// <summary>
    /// Average delivery time in seconds
    /// </summary>
    public double AverageDeliveryTime { get; set; }

    /// <summary>
    /// 95th percentile delivery time
    /// </summary>
    public double P95DeliveryTime { get; set; }

    /// <summary>
    /// 99th percentile delivery time
    /// </summary>
    public double P99DeliveryTime { get; set; }

    /// <summary>
    /// Delivery success rate
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Error rate percentage
    /// </summary>
    public double ErrorRate { get; set; }

    /// <summary>
    /// Retry rate percentage
    /// </summary>
    public double RetryRate { get; set; }

    /// <summary>
    /// Throughput (notifications per hour)
    /// </summary>
    public double ThroughputPerHour { get; set; }

    /// <summary>
    /// Peak hour analysis
    /// </summary>
    public PeakHourAnalysis PeakHours { get; set; } = new();

    /// <summary>
    /// Performance by channel
    /// </summary>
    public Dictionary<string, ChannelPerformance> ChannelPerformance { get; set; } = new();

    /// <summary>
    /// Error breakdown
    /// </summary>
    public Dictionary<string, int> ErrorBreakdown { get; set; } = new();

    /// <summary>
    /// Analysis period
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Analysis period end
    /// </summary>
    public DateTime PeriodEnd { get; set; }
}

/// <summary>
/// DTO for notification service status
/// </summary>
public class NotificationServiceStatusDto
{
    /// <summary>
    /// Overall service health status
    /// </summary>
    public HealthStatus Status { get; set; }

    /// <summary>
    /// Current queue size
    /// </summary>
    public int QueueSize { get; set; }

    /// <summary>
    /// Processing rate per minute
    /// </summary>
    public double ProcessingRate { get; set; }

    /// <summary>
    /// Number of workers active
    /// </summary>
    public int ActiveWorkers { get; set; }

    /// <summary>
    /// Service uptime
    /// </summary>
    public TimeSpan Uptime { get; set; }

    /// <summary>
    /// Memory usage in MB
    /// </summary>
    public double MemoryUsageMB { get; set; }

    /// <summary>
    /// CPU usage percentage
    /// </summary>
    public double CpuUsagePercent { get; set; }

    /// <summary>
    /// Channel status breakdown
    /// </summary>
    public Dictionary<string, ChannelStatus> ChannelStatus { get; set; } = new();

    /// <summary>
    /// Recent errors
    /// </summary>
    public List<RecentError> RecentErrors { get; set; } = new();

    /// <summary>
    /// Performance metrics
    /// </summary>
    public ServicePerformanceMetrics Performance { get; set; } = new();

    /// <summary>
    /// Last health check time
    /// </summary>
    public DateTime LastHealthCheck { get; set; }
}



/// <summary>
/// DTO for template export result
/// </summary>
public class ExportTemplatesResultDto
{
    /// <summary>
    /// Export file content (base64 encoded)
    /// </summary>
    public string FileContent { get; set; } = string.Empty;

    /// <summary>
    /// File name for download
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Content type for HTTP response
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Number of templates exported
    /// </summary>
    public int TemplateCount { get; set; }

    /// <summary>
    /// Export format used
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Export timestamp
    /// </summary>
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }
}

/// <summary>
/// DTO for export templates request
/// </summary>
public class ExportTemplatesDto
{
    /// <summary>
    /// Template IDs to export (null for all)
    /// </summary>
    public List<int>? TemplateIds { get; set; }

    /// <summary>
    /// Notification types to include
    /// </summary>
    public List<NotificationType>? NotificationTypes { get; set; }

    /// <summary>
    /// Languages to include
    /// </summary>
    public List<string>? Languages { get; set; }

    /// <summary>
    /// Export format (json, csv, xlsx)
    /// </summary>
    [Required]
    public string Format { get; set; } = "json";

    /// <summary>
    /// Include template usage statistics
    /// </summary>
    public bool IncludeStats { get; set; } = false;

    /// <summary>
    /// Include only active templates
    /// </summary>
    public bool ActiveOnly { get; set; } = true;
}

/// <summary>
/// DTO for communication service status
/// </summary>
public class CommunicationServiceStatusDto
{
    /// <summary>
    /// Overall communication health
    /// </summary>
    public HealthStatus OverallStatus { get; set; }

    /// <summary>
    /// Email service status
    /// </summary>
    public ServiceChannelStatus EmailStatus { get; set; } = new();

    /// <summary>
    /// SMS service status
    /// </summary>
    public ServiceChannelStatus SmsStatus { get; set; } = new();

    /// <summary>
    /// Push notification service status
    /// </summary>
    public ServiceChannelStatus PushStatus { get; set; } = new();

    /// <summary>
    /// Last status check time
    /// </summary>
    public DateTime LastChecked { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Recent delivery statistics
    /// </summary>
    public RecentDeliveryStats RecentStats { get; set; } = new();
}

// Supporting classes for DTOs

/// <summary>
/// Recurrence pattern for scheduled notifications
/// </summary>
public class RecurrencePattern
{
    public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Monthly
    public int Interval { get; set; } = 1;
    public List<DayOfWeek>? DaysOfWeek { get; set; }
    public int? DayOfMonth { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MaxOccurrences { get; set; }
}

/// <summary>
/// Notification type statistics
/// </summary>
public class NotificationTypeStats
{
    public int Count { get; set; }
    public double DeliveryRate { get; set; }
    public double OpenRate { get; set; }
    public double ClickRate { get; set; }
    public decimal TotalCost { get; set; }
}

/// <summary>
/// Channel statistics
/// </summary>
public class ChannelStats
{
    public int Count { get; set; }
    public double SuccessRate { get; set; }
    public double AverageDeliveryTime { get; set; }
    public decimal TotalCost { get; set; }
    public List<string> TopErrors { get; set; } = new();
}

/// <summary>
/// Daily notification statistics
/// </summary>
public class DailyNotificationStats
{
    public DateTime Date { get; set; }
    public int TotalSent { get; set; }
    public int TotalDelivered { get; set; }
    public int TotalFailed { get; set; }
    public double DeliveryRate { get; set; }
    public decimal DailyCost { get; set; }
}

/// <summary>
/// Peak hour analysis
/// </summary>
public class PeakHourAnalysis
{
    public int PeakHour { get; set; }
    public int PeakVolume { get; set; }
    public int LowestHour { get; set; }
    public int LowestVolume { get; set; }
    public Dictionary<int, int> HourlyBreakdown { get; set; } = new();
}

/// <summary>
/// Channel performance metrics
/// </summary>
public class ChannelPerformance
{
    public double AverageDeliveryTime { get; set; }
    public double SuccessRate { get; set; }
    public double ErrorRate { get; set; }
    public int TotalProcessed { get; set; }
    public decimal TotalCost { get; set; }
}

/// <summary>
/// Channel status information
/// </summary>
public class ChannelStatus
{
    public HealthStatus Status { get; set; }
    public string? LastError { get; set; }
    public DateTime? LastSuccessfulDelivery { get; set; }
    public int QueueSize { get; set; }
    public double ProcessingRate { get; set; }
}

/// <summary>
/// Recent error information
/// </summary>
public class RecentError
{
    public DateTime Timestamp { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public int Count { get; set; }
    public string? Channel { get; set; }
}

/// <summary>
/// Service performance metrics
/// </summary>
public class ServicePerformanceMetrics
{
    public double RequestsPerSecond { get; set; }
    public double AverageResponseTime { get; set; }
    public double ErrorRate { get; set; }
    public int ActiveConnections { get; set; }
    public long TotalRequests { get; set; }
    public long TotalErrors { get; set; }
}

/// <summary>
/// Channel preference statistics
/// </summary>
public class ChannelPreferenceStats
{
    public int EnabledCount { get; set; }
    public int DisabledCount { get; set; }
    public double EnabledPercentage { get; set; }
    public List<string> CommonReasons { get; set; } = new();
}

/// <summary>
/// Quiet hours distribution
/// </summary>
public class QuietHoursDistribution
{
    public Dictionary<string, int> StartTimeDistribution { get; set; } = new();
    public Dictionary<string, int> EndTimeDistribution { get; set; } = new();
    public Dictionary<string, int> TimezoneDistribution { get; set; } = new();
    public int AverageDurationHours { get; set; }
}

/// <summary>
/// Preference change statistics
/// </summary>
public class PreferenceChangeStats
{
    public DateTime Date { get; set; }
    public int TotalChanges { get; set; }
    public int OptIns { get; set; }
    public int OptOuts { get; set; }
    public Dictionary<string, int> ChangedNotificationTypes { get; set; } = new();
}

/// <summary>
/// Service channel status
/// </summary>
public class ServiceChannelStatus
{
    public HealthStatus Status { get; set; }
    public string? LastError { get; set; }
    public DateTime? LastChecked { get; set; }
    public double ResponseTimeMs { get; set; }
    public int SuccessfulDeliveries24h { get; set; }
    public int FailedDeliveries24h { get; set; }
    public double SuccessRate24h { get; set; }
}

/// <summary>
/// Recent delivery statistics
/// </summary>
public class RecentDeliveryStats
{
    public int Last24Hours { get; set; }
    public int LastHour { get; set; }
    public double SuccessRateLast24h { get; set; }
    public double AverageDeliveryTime { get; set; }
    public Dictionary<string, int> ChannelBreakdown { get; set; } = new();
}
