using System.ComponentModel.DataAnnotations;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for creating notification history entry
/// </summary>
public class CreateNotificationHistoryDto
{
    /// <summary>
    /// Associated notification ID
    /// </summary>
    [Required]
    public Guid NotificationId { get; set; }

    /// <summary>
    /// Delivery channel used
    /// </summary>
    [Required]
    [EnumDataType(typeof(NotificationChannel))]
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Delivery status
    /// </summary>
    [Required]
    [EnumDataType(typeof(DeliveryStatus))]
    public DeliveryStatus Status { get; set; }

    /// <summary>
    /// Third-party provider used for delivery
    /// </summary>
    [StringLength(100)]
    public string? Provider { get; set; }

    /// <summary>
    /// External tracking ID from provider
    /// </summary>
    [StringLength(200)]
    public string? ExternalId { get; set; }

    /// <summary>
    /// Error message if delivery failed
    /// </summary>
    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Number of delivery attempts
    /// </summary>
    [Range(1, 10)]
    public int AttemptCount { get; set; } = 1;

    /// <summary>
    /// Cost of delivery (if applicable)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? Cost { get; set; }

    /// <summary>
    /// Currency for cost calculation
    /// </summary>
    [StringLength(3)]
    public string? Currency { get; set; }

    /// <summary>
    /// Delivery-specific metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// When delivery was attempted
    /// </summary>
    public DateTime DeliveredAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// DTO for updating notification history
/// </summary>
public class UpdateNotificationHistoryDto
{
    /// <summary>
    /// Updated delivery status
    /// </summary>
    [EnumDataType(typeof(DeliveryStatus))]
    public DeliveryStatus? Status { get; set; }

    /// <summary>
    /// External tracking ID from provider
    /// </summary>
    [StringLength(200)]
    public string? ExternalId { get; set; }

    /// <summary>
    /// Error message if delivery failed
    /// </summary>
    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Updated attempt count
    /// </summary>
    [Range(1, 10)]
    public int? AttemptCount { get; set; }

    /// <summary>
    /// Cost of delivery (if applicable)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? Cost { get; set; }

    /// <summary>
    /// Currency for cost calculation
    /// </summary>
    [StringLength(3)]
    public string? Currency { get; set; }

    /// <summary>
    /// Additional delivery metadata
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// When delivery was completed/updated
    /// </summary>
    public DateTime? DeliveredAt { get; set; }
}

/// <summary>
/// DTO for notification history response data
/// </summary>
public class NotificationHistoryDto
{
    /// <summary>
    /// Unique identifier for the history entry
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Associated notification ID
    /// </summary>
    public Guid NotificationId { get; set; }

    /// <summary>
    /// Delivery channel used
    /// </summary>
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Delivery status
    /// </summary>
    public DeliveryStatus Status { get; set; }

    /// <summary>
    /// Third-party provider used for delivery
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// External tracking ID from provider
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Error message if delivery failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Number of delivery attempts
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Cost of delivery
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Currency for cost calculation
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Delivery-specific metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// When delivery was attempted
    /// </summary>
    public DateTime DeliveredAt { get; set; }

    /// <summary>
    /// When the history entry was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the history entry was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// Associated notification details (if included)
    /// </summary>
    public NotificationDto? Notification { get; set; }
}

/// <summary>
/// DTO for filtering notification history
/// </summary>
public class HistoryFilterDto
{
    /// <summary>
    /// Filter by notification ID
    /// </summary>
    public Guid? NotificationId { get; set; }

    /// <summary>
    /// Filter by user ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Filter by delivery channel
    /// </summary>
    [EnumDataType(typeof(NotificationChannel))]
    public NotificationChannel? Channel { get; set; }

    /// <summary>
    /// Filter by delivery status
    /// </summary>
    [EnumDataType(typeof(DeliveryStatus))]
    public DeliveryStatus? Status { get; set; }

    /// <summary>
    /// Filter by notification type
    /// </summary>
    [EnumDataType(typeof(NotificationType))]
    public NotificationType? NotificationType { get; set; }

    /// <summary>
    /// Filter by provider
    /// </summary>
    [StringLength(100)]
    public string? Provider { get; set; }

    /// <summary>
    /// Start date for filtering
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for filtering
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Filter by minimum cost
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? MinCost { get; set; }

    /// <summary>
    /// Filter by maximum cost
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? MaxCost { get; set; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Sort field
    /// </summary>
    [StringLength(50)]
    public string SortBy { get; set; } = "DeliveredAt";

    /// <summary>
    /// Sort direction
    /// </summary>
    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'")]
    public string SortDirection { get; set; } = "desc";
}

/// <summary>
/// DTO for delivery statistics
/// </summary>
public class DeliveryStatsDto
{
    /// <summary>
    /// Total number of deliveries
    /// </summary>
    public int TotalDeliveries { get; set; }

    /// <summary>
    /// Number of successful deliveries
    /// </summary>
    public int SuccessfulDeliveries { get; set; }

    /// <summary>
    /// Number of failed deliveries
    /// </summary>
    public int FailedDeliveries { get; set; }

    /// <summary>
    /// Number of pending deliveries
    /// </summary>
    public int PendingDeliveries { get; set; }

    /// <summary>
    /// Overall delivery success rate as percentage
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Statistics by channel
    /// </summary>
    public Dictionary<NotificationChannel, ChannelStatsDto> StatsByChannel { get; set; } = new();

    /// <summary>
    /// Statistics by notification type
    /// </summary>
    public Dictionary<NotificationType, TypeStatsDto> StatsByType { get; set; } = new();

    /// <summary>
    /// Statistics by provider
    /// </summary>
    public Dictionary<string, ProviderStatsDto> StatsByProvider { get; set; } = new();

    /// <summary>
    /// Total cost across all deliveries
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Average cost per delivery
    /// </summary>
    public decimal AverageCost { get; set; }

    /// <summary>
    /// Statistics period start date
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Statistics period end date
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// When statistics were generated
    /// </summary>
    public DateTime GeneratedAt { get; set; }
}

/// <summary>
/// DTO for channel-specific delivery statistics
/// </summary>
public class ChannelStatsDto
{
    /// <summary>
    /// Delivery channel
    /// </summary>
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Total deliveries for this channel
    /// </summary>
    public int TotalDeliveries { get; set; }

    /// <summary>
    /// Successful deliveries for this channel
    /// </summary>
    public int SuccessfulDeliveries { get; set; }

    /// <summary>
    /// Failed deliveries for this channel
    /// </summary>
    public int FailedDeliveries { get; set; }

    /// <summary>
    /// Success rate for this channel
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Average delivery time in milliseconds
    /// </summary>
    public double AverageDeliveryTime { get; set; }

    /// <summary>
    /// Total cost for this channel
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Average cost per delivery for this channel
    /// </summary>
    public decimal AverageCost { get; set; }
}

/// <summary>
/// DTO for notification type-specific delivery statistics
/// </summary>
public class TypeStatsDto
{
    /// <summary>
    /// Notification type
    /// </summary>
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Total deliveries for this type
    /// </summary>
    public int TotalDeliveries { get; set; }

    /// <summary>
    /// Successful deliveries for this type
    /// </summary>
    public int SuccessfulDeliveries { get; set; }

    /// <summary>
    /// Failed deliveries for this type
    /// </summary>
    public int FailedDeliveries { get; set; }

    /// <summary>
    /// Success rate for this type
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Most used channel for this type
    /// </summary>
    public NotificationChannel MostUsedChannel { get; set; }

    /// <summary>
    /// Total cost for this type
    /// </summary>
    public decimal TotalCost { get; set; }
}

/// <summary>
/// DTO for provider-specific delivery statistics
/// </summary>
public class ProviderStatsDto
{
    /// <summary>
    /// Provider name
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Total deliveries for this provider
    /// </summary>
    public int TotalDeliveries { get; set; }

    /// <summary>
    /// Successful deliveries for this provider
    /// </summary>
    public int SuccessfulDeliveries { get; set; }

    /// <summary>
    /// Failed deliveries for this provider
    /// </summary>
    public int FailedDeliveries { get; set; }

    /// <summary>
    /// Success rate for this provider
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Average response time in milliseconds
    /// </summary>
    public double AverageResponseTime { get; set; }

    /// <summary>
    /// Total cost for this provider
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Average cost per delivery for this provider
    /// </summary>
    public decimal AverageCost { get; set; }

    /// <summary>
    /// Reliability score (0-100)
    /// </summary>
    public double ReliabilityScore { get; set; }
}

/// <summary>
/// DTO for cost analytics
/// </summary>
public class CostAnalyticsDto
{
    /// <summary>
    /// Total cost across all deliveries
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Currency for cost calculations
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Cost breakdown by channel
    /// </summary>
    public Dictionary<NotificationChannel, decimal> CostByChannel { get; set; } = new();

    /// <summary>
    /// Cost breakdown by notification type
    /// </summary>
    public Dictionary<NotificationType, decimal> CostByType { get; set; } = new();

    /// <summary>
    /// Cost breakdown by provider
    /// </summary>
    public Dictionary<string, decimal> CostByProvider { get; set; } = new();

    /// <summary>
    /// Daily cost breakdown
    /// </summary>
    public Dictionary<DateTime, decimal> DailyCosts { get; set; } = new();

    /// <summary>
    /// Cost trends and predictions
    /// </summary>
    public CostTrendDto Trends { get; set; } = new();

    /// <summary>
    /// Period start date
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Period end date
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// When analytics were generated
    /// </summary>
    public DateTime GeneratedAt { get; set; }
}

/// <summary>
/// DTO for cost trend analysis
/// </summary>
public class CostTrendDto
{
    /// <summary>
    /// Average daily cost
    /// </summary>
    public decimal AverageDailyCost { get; set; }

    /// <summary>
    /// Projected monthly cost based on current trend
    /// </summary>
    public decimal ProjectedMonthlyCost { get; set; }

    /// <summary>
    /// Cost change percentage compared to previous period
    /// </summary>
    public double CostChangePercent { get; set; }

    /// <summary>
    /// Whether costs are trending up or down
    /// </summary>
    public string Trend { get; set; } = "stable";

    /// <summary>
    /// Most expensive day in the period
    /// </summary>
    public DateTime MostExpensiveDay { get; set; }

    /// <summary>
    /// Cost on the most expensive day
    /// </summary>
    public decimal MostExpensiveDayCost { get; set; }
}

/// <summary>
/// DTO for failed delivery analysis
/// </summary>
public class FailedDeliveryAnalysisDto
{
    /// <summary>
    /// Total number of failed deliveries
    /// </summary>
    public int TotalFailures { get; set; }

    /// <summary>
    /// Failed deliveries by channel
    /// </summary>
    public Dictionary<NotificationChannel, int> FailuresByChannel { get; set; } = new();

    /// <summary>
    /// Failed deliveries by provider
    /// </summary>
    public Dictionary<string, int> FailuresByProvider { get; set; } = new();

    /// <summary>
    /// Common error messages and their frequency
    /// </summary>
    public Dictionary<string, int> CommonErrors { get; set; } = new();

    /// <summary>
    /// Failed deliveries that need retry
    /// </summary>
    public List<RetryableFailureDto> RetryableFailures { get; set; } = new();

    /// <summary>
    /// Failed deliveries by hour of day
    /// </summary>
    public Dictionary<int, int> FailuresByHour { get; set; } = new();

    /// <summary>
    /// Analysis period start
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Analysis period end
    /// </summary>
    public DateTime PeriodEnd { get; set; }
}

/// <summary>
/// DTO for retryable failed delivery
/// </summary>
public class RetryableFailureDto
{
    /// <summary>
    /// History entry ID
    /// </summary>
    public Guid HistoryId { get; set; }

    /// <summary>
    /// Notification ID
    /// </summary>
    public Guid NotificationId { get; set; }

    /// <summary>
    /// Delivery channel
    /// </summary>
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Number of attempts made
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// When the failure occurred
    /// </summary>
    public DateTime FailedAt { get; set; }

    /// <summary>
    /// Whether this failure is retryable
    /// </summary>
    public bool IsRetryable { get; set; }

    /// <summary>
    /// Suggested retry time
    /// </summary>
    public DateTime? SuggestedRetryAt { get; set; }
}

/// <summary>
/// DTO for paginated history results
/// </summary>
public class PagedHistoryResultDto
{
    /// <summary>
    /// List of history entries for the current page
    /// </summary>
    public List<NotificationHistoryDto> History { get; set; } = new();

    /// <summary>
    /// Total number of history entries matching the criteria
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;
}

/// <summary>
/// DTO for bulk retry request
/// </summary>
public class BulkRetryDto
{
    /// <summary>
    /// List of history IDs to retry
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<Guid> HistoryIds { get; set; } = new();

    /// <summary>
    /// Whether to force retry even if not typically retryable
    /// </summary>
    public bool ForceRetry { get; set; } = false;

    /// <summary>
    /// Maximum number of retries to attempt
    /// </summary>
    [Range(1, 5)]
    public int MaxRetries { get; set; } = 3;
}

/// <summary>
/// DTO for bulk retry result
/// </summary>
public class BulkRetryResultDto
{
    /// <summary>
    /// Number of deliveries successfully queued for retry
    /// </summary>
    public int SuccessfulRetries { get; set; }

    /// <summary>
    /// Number of deliveries that could not be retried
    /// </summary>
    public int FailedRetries { get; set; }

    /// <summary>
    /// List of errors that occurred during retry
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Details of each retry attempt
    /// </summary>
    public List<RetryResultDto> RetryResults { get; set; } = new();
}

/// <summary>
/// DTO for individual retry result
/// </summary>
public class RetryResultDto
{
    /// <summary>
    /// History ID that was retried
    /// </summary>
    public Guid HistoryId { get; set; }

    /// <summary>
    /// Whether the retry was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if retry failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// New history ID created for the retry
    /// </summary>
    public Guid? NewHistoryId { get; set; }
}

/// <summary>
/// Delivery status enumeration
/// </summary>
public enum DeliveryStatus
{
    Pending,
    Sent,
    Delivered,
    Failed,
    Bounced,
    Clicked,
    Opened,
    Unsubscribed
}
