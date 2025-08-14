using System.ComponentModel.DataAnnotations;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for quiet hours configuration
/// </summary>
public class QuietHoursDto
{
    /// <summary>
    /// Start time for quiet hours (local time)
    /// </summary>
    [Required]
    public TimeSpan Start { get; set; }

    /// <summary>
    /// End time for quiet hours (local time)
    /// </summary>
    [Required]
    public TimeSpan End { get; set; }

    /// <summary>
    /// User's timezone identifier
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Timezone { get; set; } = string.Empty;

    /// <summary>
    /// Days of the week when quiet hours apply
    /// </summary>
    public List<DayOfWeek> ApplicableDays { get; set; } = new();

    /// <summary>
    /// Whether quiet hours are enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Description of the quiet hours configuration
    /// </summary>
    [StringLength(200)]
    public string? Description { get; set; }
}

/// <summary>
/// DTO for template usage statistics
/// </summary>
public class TemplateUsageStats
{
    /// <summary>
    /// Template ID
    /// </summary>
    public Guid TemplateId { get; set; }

    /// <summary>
    /// Template name or key
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// Total number of times used
    /// </summary>
    public int TotalUsage { get; set; }

    /// <summary>
    /// Successful deliveries using this template
    /// </summary>
    public int SuccessfulDeliveries { get; set; }

    /// <summary>
    /// Failed deliveries using this template
    /// </summary>
    public int FailedDeliveries { get; set; }

    /// <summary>
    /// Success rate as percentage
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Most recent usage date
    /// </summary>
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// Usage breakdown by channel
    /// </summary>
    public Dictionary<string, int> UsageByChannel { get; set; } = new();

    /// <summary>
    /// Statistics period start
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Statistics period end
    /// </summary>
    public DateTime PeriodEnd { get; set; }
}

/// <summary>
/// DTO for user notification preference summary
/// </summary>
public class PreferenceSummary
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Total number of notification types configured
    /// </summary>
    public int TotalNotificationTypes { get; set; }

    /// <summary>
    /// Number of enabled notification types
    /// </summary>
    public int EnabledNotificationTypes { get; set; }

    /// <summary>
    /// Number of disabled notification types
    /// </summary>
    public int DisabledNotificationTypes { get; set; }

    /// <summary>
    /// Preferred delivery channels
    /// </summary>
    public List<string> PreferredChannels { get; set; } = new();

    /// <summary>
    /// Whether user has quiet hours configured
    /// </summary>
    public bool HasQuietHours { get; set; }

    /// <summary>
    /// User's timezone setting
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// Average notifications per day
    /// </summary>
    public double AverageNotificationsPerDay { get; set; }

    /// <summary>
    /// Most active notification type
    /// </summary>
    public string? MostActiveNotificationType { get; set; }

    /// <summary>
    /// Least active notification type
    /// </summary>
    public string? LeastActiveNotificationType { get; set; }

    /// <summary>
    /// When preferences were last updated
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Breakdown of preferences by type and channel
    /// </summary>
    public Dictionary<string, Dictionary<string, bool>> PreferenceBreakdown { get; set; } = new();
}

/// <summary>
/// DTO for common filter parameters
/// </summary>
public class BaseFilterDto
{
    /// <summary>
    /// Search term for text-based filtering
    /// </summary>
    [StringLength(100)]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Start date for date range filtering
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for date range filtering
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    [Range(1, 1000)]
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Sort field name
    /// </summary>
    [StringLength(50)]
    public string SortBy { get; set; } = "CreatedAt";

    /// <summary>
    /// Sort direction (asc or desc)
    /// </summary>
    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'")]
    public string SortDirection { get; set; } = "desc";

    /// <summary>
    /// Skip count for offset-based pagination
    /// </summary>
    public int Skip => (Page - 1) * PageSize;

    /// <summary>
    /// Take count for limit-based pagination
    /// </summary>
    public int Take => PageSize;

    /// <summary>
    /// Whether sorting is ascending
    /// </summary>
    public bool IsAscending => SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);
}

/// <summary>
/// DTO for API operation result
/// </summary>
/// <typeparam name="T">Type of the result data</typeparam>
public class OperationResult<T>
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Result data (null if operation failed)
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<string> ValidationErrors { get; set; } = new();

    /// <summary>
    /// Error code for programmatic handling
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Additional metadata about the operation
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// When the operation was executed
    /// </summary>
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <param name="data">The result data</param>
    /// <returns>Successful operation result</returns>
    public static OperationResult<T> Success(T data)
    {
        return new OperationResult<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    /// <summary>
    /// Creates a failed result
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <param name="errorCode">Optional error code</param>
    /// <returns>Failed operation result</returns>
    public static OperationResult<T> Failure(string errorMessage, string? errorCode = null)
    {
        return new OperationResult<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }

    /// <summary>
    /// Creates a validation failure result
    /// </summary>
    /// <param name="validationErrors">List of validation errors</param>
    /// <returns>Validation failure result</returns>
    public static OperationResult<T> ValidationFailure(List<string> validationErrors)
    {
        return new OperationResult<T>
        {
            IsSuccess = false,
            ErrorMessage = "Validation failed",
            ValidationErrors = validationErrors,
            ErrorCode = "VALIDATION_FAILED"
        };
    }
}

/// <summary>
/// DTO for operation result without data
/// </summary>
public class OperationResult : OperationResult<object>
{
    /// <summary>
    /// Creates a successful result without data
    /// </summary>
    /// <returns>Successful operation result</returns>
    public static OperationResult Success()
    {
        return new OperationResult
        {
            IsSuccess = true
        };
    }

    /// <summary>
    /// Creates a failed result without data
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <param name="errorCode">Optional error code</param>
    /// <returns>Failed operation result</returns>
    public static new OperationResult Failure(string errorMessage, string? errorCode = null)
    {
        return new OperationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }
}

/// <summary>
/// DTO for batch operation result
/// </summary>
public class BatchOperationResult
{
    /// <summary>
    /// Total number of items processed
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Number of items processed successfully
    /// </summary>
    public int SuccessfulItems { get; set; }

    /// <summary>
    /// Number of items that failed processing
    /// </summary>
    public int FailedItems { get; set; }

    /// <summary>
    /// List of errors that occurred during batch processing
    /// </summary>
    public List<BatchItemError> Errors { get; set; } = new();

    /// <summary>
    /// Success rate as percentage
    /// </summary>
    public double SuccessRate => TotalItems > 0 ? (double)SuccessfulItems / TotalItems * 100 : 0;

    /// <summary>
    /// Whether the entire batch operation was successful
    /// </summary>
    public bool IsFullySuccessful => FailedItems == 0;

    /// <summary>
    /// When the batch operation was executed
    /// </summary>
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Duration of the batch operation
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Additional metadata about the batch operation
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// DTO for individual batch item error
/// </summary>
public class BatchItemError
{
    /// <summary>
    /// Index of the failed item in the batch
    /// </summary>
    public int ItemIndex { get; set; }

    /// <summary>
    /// Identifier of the failed item (if available)
    /// </summary>
    public string? ItemId { get; set; }

    /// <summary>
    /// Error message for this item
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Error code for programmatic handling
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Additional details about the error
    /// </summary>
    public Dictionary<string, object> ErrorDetails { get; set; } = new();
}

/// <summary>
/// DTO for health check result
/// </summary>
public class HealthCheckResult
{
    /// <summary>
    /// Service or component name
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Health status
    /// </summary>
    public HealthStatus Status { get; set; }

    /// <summary>
    /// Response time in milliseconds
    /// </summary>
    public double ResponseTimeMs { get; set; }

    /// <summary>
    /// Additional health information
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();

    /// <summary>
    /// When the health check was performed
    /// </summary>
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Error message if health check failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Health status enumeration
/// </summary>
public enum HealthStatus
{
    Healthy,
    Degraded,
    Unhealthy
}
