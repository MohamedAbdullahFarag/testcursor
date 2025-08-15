namespace Ikhtibar.Core.Services;

/// <summary>
/// Service interface for audit log retention management
/// Provides automated retention policies, archival, and cleanup capabilities
/// </summary>
public interface IAuditRetentionService
{
    /// <summary>
    /// Archives audit logs older than the specified retention period
    /// </summary>
    /// <param name="retentionPeriod">Time period to retain logs</param>
    /// <returns>Number of logs archived</returns>
    Task<int> ArchiveOldLogsAsync(TimeSpan retentionPeriod);
    
    /// <summary>
    /// Purges archived logs older than the specified archival period
    /// </summary>
    /// <param name="archivalPeriod">Time period to retain archived logs</param>
    /// <returns>Number of logs purged</returns>
    Task<int> PurgeArchivedLogsAsync(TimeSpan archivalPeriod);
    
    /// <summary>
    /// Verifies the integrity of archived logs
    /// </summary>
    /// <returns>Integrity verification results</returns>
    Task<bool> VerifyArchivedLogsIntegrityAsync();
    
    /// <summary>
    /// Gets retention policy configuration
    /// </summary>
    /// <returns>Current retention policy settings</returns>
    Task<RetentionPolicy> GetRetentionPolicyAsync();
    
    /// <summary>
    /// Updates retention policy configuration
    /// </summary>
    /// <param name="policy">New retention policy settings</param>
    /// <returns>Updated retention policy</returns>
    Task<RetentionPolicy> UpdateRetentionPolicyAsync(RetentionPolicy policy);
    
    /// <summary>
    /// Executes scheduled retention tasks
    /// </summary>
    /// <returns>Task execution results</returns>
    Task<RetentionTaskResult> ExecuteRetentionTasksAsync();
    
    /// <summary>
    /// Gets retention statistics and metrics
    /// </summary>
    /// <returns>Retention statistics</returns>
    Task<RetentionStatistics> GetRetentionStatisticsAsync();
}

/// <summary>
/// Configuration for audit log retention policies
/// </summary>
public class RetentionPolicy
{
    /// <summary>
    /// How long to retain active audit logs
    /// </summary>
    public TimeSpan ActiveLogRetentionPeriod { get; set; } = TimeSpan.FromDays(365);
    
    /// <summary>
    /// How long to retain archived audit logs
    /// </summary>
    public TimeSpan ArchivedLogRetentionPeriod { get; set; } = TimeSpan.FromDays(2555); // 7 years
    
    /// <summary>
    /// How often to run retention tasks (in hours)
    /// </summary>
    public int RetentionTaskFrequencyHours { get; set; } = 24;
    
    /// <summary>
    /// Whether to enable automatic archival
    /// </summary>
    public bool EnableAutomaticArchival { get; set; } = true;
    
    /// <summary>
    /// Whether to enable automatic purging
    /// </summary>
    public bool EnableAutomaticPurging { get; set; } = true;
    
    /// <summary>
    /// Maximum size of audit log database in GB
    /// </summary>
    public int MaxDatabaseSizeGB { get; set; } = 100;
    
    /// <summary>
    /// Whether to compress archived logs
    /// </summary>
    public bool CompressArchivedLogs { get; set; } = true;
    
    /// <summary>
    /// Whether to encrypt archived logs
    /// </summary>
    public bool EncryptArchivedLogs { get; set; } = true;
    
    /// <summary>
    /// Notification email for retention policy violations
    /// </summary>
    public string? NotificationEmail { get; set; }
}

/// <summary>
/// Results of retention task execution
/// </summary>
public class RetentionTaskResult
{
    /// <summary>
    /// When the task was executed
    /// </summary>
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Number of logs archived
    /// </summary>
    public int LogsArchived { get; set; }
    
    /// <summary>
    /// Number of logs purged
    /// </summary>
    public int LogsPurged { get; set; }
    
    /// <summary>
    /// Whether the task completed successfully
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Error message if task failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Duration of the task execution
    /// </summary>
    public TimeSpan Duration { get; set; }
    
    /// <summary>
    /// Space freed in MB
    /// </summary>
    public double SpaceFreedMB { get; set; }
}

/// <summary>
/// Statistics about audit log retention
/// </summary>
public class RetentionStatistics
{
    /// <summary>
    /// Total number of active audit logs
    /// </summary>
    public int TotalActiveLogs { get; set; }
    
    /// <summary>
    /// Total number of archived audit logs
    /// </summary>
    public int TotalArchivedLogs { get; set; }
    
    /// <summary>
    /// Current database size in MB
    /// </summary>
    public double CurrentDatabaseSizeMB { get; set; }
    
    /// <summary>
    /// Space used by active logs in MB
    /// </summary>
    public double ActiveLogsSizeMB { get; set; }
    
    /// <summary>
    /// Space used by archived logs in MB
    /// </summary>
    public double ArchivedLogsSizeMB { get; set; }
    
    /// <summary>
    /// Oldest active log timestamp
    /// </summary>
    public DateTime? OldestActiveLog { get; set; }
    
    /// <summary>
    /// Newest active log timestamp
    /// </summary>
    public DateTime? NewestActiveLog { get; set; }
    
    /// <summary>
    /// Oldest archived log timestamp
    /// </summary>
    public DateTime? OldestArchivedLog { get; set; }
    
    /// <summary>
    /// Newest archived log timestamp
    /// </summary>
    public DateTime? NewestArchivedLog { get; set; }
    
    /// <summary>
    /// Last retention task execution
    /// </summary>
    public DateTime? LastRetentionTask { get; set; }
    
    /// <summary>
    /// Next scheduled retention task
    /// </summary>
    public DateTime? NextRetentionTask { get; set; }
}
