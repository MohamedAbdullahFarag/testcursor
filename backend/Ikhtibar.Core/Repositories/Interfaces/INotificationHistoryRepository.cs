using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for notification history entities
/// Tracks delivery attempts and provides analytics data
/// </summary>
public interface INotificationHistoryRepository : IRepository<NotificationHistory>
{
    /// <summary>
    /// Gets delivery history for a specific notification
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <returns>List of delivery attempts for the notification</returns>
    Task<IEnumerable<NotificationHistory>> GetByNotificationIdAsync(Guid notificationId);

    /// <summary>
    /// Records a delivery attempt
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <param name="channel">Delivery channel used</param>
    /// <param name="status">Delivery status</param>
    /// <param name="errorMessage">Error message if failed</param>
    /// <param name="externalId">External provider ID</param>
    /// <param name="responseData">Raw response from provider</param>
    /// <param name="cost">Cost of delivery</param>
    /// <param name="costCurrency">Currency of cost</param>
    /// <returns>Created history record</returns>
    Task<NotificationHistory> RecordDeliveryAttemptAsync(
        Guid notificationId,
        NotificationChannel channel,
        NotificationDeliveryStatus status,
        string? errorMessage = null,
        string? externalId = null,
        string? responseData = null,
        decimal? cost = null,
        string? costCurrency = null);

    /// <summary>
    /// Updates delivery status for an existing history record
    /// </summary>
    /// <param name="historyId">History record ID</param>
    /// <param name="status">New delivery status</param>
    /// <param name="deliveredAt">Delivery timestamp</param>
    /// <param name="responseData">Updated response data</param>
    /// <returns>True if successfully updated</returns>
    Task<bool> UpdateDeliveryStatusAsync(
        Guid historyId,
        NotificationDeliveryStatus status,
        DateTime? deliveredAt = null,
        string? responseData = null);

    /// <summary>
    /// Records notification opened event
    /// </summary>
    /// <param name="externalId">External provider ID</param>
    /// <param name="openedAt">When notification was opened</param>
    /// <returns>True if successfully recorded</returns>
    Task<bool> RecordOpenedAsync(string externalId, DateTime? openedAt = null);

    /// <summary>
    /// Records notification clicked event
    /// </summary>
    /// <param name="externalId">External provider ID</param>
    /// <param name="clickedAt">When notification was clicked</param>
    /// <returns>True if successfully recorded</returns>
    Task<bool> RecordClickedAsync(string externalId, DateTime? clickedAt = null);

    /// <summary>
    /// Gets delivery statistics for a date range
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <param name="channel">Optional channel filter</param>
    /// <returns>Delivery statistics</returns>
    Task<DeliveryStats> GetDeliveryStatsAsync(
        DateTime fromDate,
        DateTime toDate,
        NotificationChannel? channel = null);

    /// <summary>
    /// Gets failed deliveries for retry processing
    /// </summary>
    /// <param name="maxRetryCount">Maximum retry attempts</param>
    /// <param name="retryAfterMinutes">Minimum minutes since last attempt</param>
    /// <param name="batchSize">Maximum number of records to return</param>
    /// <returns>List of failed deliveries eligible for retry</returns>
    Task<IEnumerable<NotificationHistory>> GetFailedDeliveriesForRetryAsync(
        int maxRetryCount = 3,
        int retryAfterMinutes = 30,
        int batchSize = 100);

    /// <summary>
    /// Gets delivery history with pagination and filtering
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of records per page</param>
    /// <param name="channel">Optional channel filter</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="fromDate">Optional start date filter</param>
    /// <param name="toDate">Optional end date filter</param>
    /// <returns>Paginated delivery history</returns>
    Task<(IEnumerable<NotificationHistory> History, int TotalCount)> GetDeliveryHistoryAsync(
        int page = 1,
        int pageSize = 20,
        NotificationChannel? channel = null,
        NotificationDeliveryStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);

    /// <summary>
    /// Gets cost analysis for delivered notifications
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <param name="groupBy">Group results by channel, day, or month</param>
    /// <returns>Cost analysis data</returns>
    Task<IEnumerable<CostAnalysis>> GetCostAnalysisAsync(
        DateTime fromDate,
        DateTime toDate,
        CostGrouping groupBy = CostGrouping.Channel);

    /// <summary>
    /// Cleans up old delivery history records beyond retention period
    /// </summary>
    /// <param name="retentionDays">Number of days to retain history</param>
    /// <returns>Number of records deleted</returns>
    Task<int> CleanupOldHistoryAsync(int retentionDays = 180);
}

/// <summary>
/// Delivery statistics model
/// </summary>
public class DeliveryStats
{
    public int TotalAttempts { get; set; }
    public int SuccessfulDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    public int PendingDeliveries { get; set; }
    public decimal SuccessRate { get; set; }
    public Dictionary<NotificationChannel, int> DeliveriesByChannel { get; set; } = new();
    public Dictionary<NotificationDeliveryStatus, int> DeliveriesByStatus { get; set; } = new();
}

/// <summary>
/// Cost analysis model
/// </summary>
public class CostAnalysis
{
    public string GroupKey { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int DeliveryCount { get; set; }
    public decimal AverageCost { get; set; }
}

/// <summary>
/// Cost grouping options
/// </summary>
public enum CostGrouping
{
    Channel,
    Day,
    Month,
    NotificationType
}
