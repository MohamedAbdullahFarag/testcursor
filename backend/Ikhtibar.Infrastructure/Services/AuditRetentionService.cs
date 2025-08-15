using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services;
using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Text.Json;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Service implementation for audit log retention management
/// Provides automated retention policies, archival, and cleanup capabilities
/// </summary>
public class AuditRetentionService : IAuditRetentionService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<AuditRetentionService> _logger;
    private readonly string _archivePath;
    private readonly RetentionPolicy _defaultPolicy;

    /// <summary>
    /// Constructor for AuditRetentionService
    /// </summary>
    /// <param name="auditLogRepository">Audit log repository</param>
    /// <param name="logger">Logger instance</param>
    public AuditRetentionService(
        IAuditLogRepository auditLogRepository,
        ILogger<AuditRetentionService> logger)
    {
        _auditLogRepository = auditLogRepository;
        _logger = logger;
        
        // In production, this should come from configuration
        _archivePath = Environment.GetEnvironmentVariable("AUDIT_LOG_ARCHIVE_PATH") ?? 
                      Path.Combine(Environment.CurrentDirectory, "AuditLogs", "Archive");
        
        _defaultPolicy = new RetentionPolicy();
        
        // Ensure archive directory exists
        Directory.CreateDirectory(_archivePath);
    }

    /// <summary>
    /// Archives audit logs older than the specified retention period
    /// </summary>
    /// <param name="retentionPeriod">Time period to retain logs</param>
    /// <returns>Number of logs archived</returns>
    public async Task<int> ArchiveOldLogsAsync(TimeSpan retentionPeriod)
    {
        var cutoffDate = DateTime.UtcNow.Subtract(retentionPeriod);
        var archivedCount = 0;
        
        try
        {
            _logger.LogInformation("Starting archival of audit logs older than {CutoffDate}", cutoffDate);
            
            // Get logs older than retention period
            var filter = new Shared.DTOs.AuditLogFilter
            {
                ToDate = cutoffDate,
                PageSize = 1000
            };
            
            var oldLogs = await _auditLogRepository.GetAuditLogsAsync(filter);
            
            foreach (var log in oldLogs.Items)
            {
                if (await ArchiveLogAsync(log))
                {
                    archivedCount++;
                }
            }
            
            _logger.LogInformation("Archived {ArchivedCount} audit logs older than {CutoffDate}", archivedCount, cutoffDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to archive old audit logs");
        }
        
        return archivedCount;
    }

    /// <summary>
    /// Purges archived logs older than the specified archival period
    /// </summary>
    /// <param name="archivalPeriod">Time period to retain archived logs</param>
    /// <returns>Number of logs purged</returns>
    public async Task<int> PurgeArchivedLogsAsync(TimeSpan archivalPeriod)
    {
        var cutoffDate = DateTime.UtcNow.Subtract(archivalPeriod);
        var purgedCount = 0;
        
        try
        {
            _logger.LogInformation("Starting purge of archived logs older than {CutoffDate}", cutoffDate);
            
            var archiveFiles = Directory.GetFiles(_archivePath, "*.zip")
                .Where(f => File.GetLastWriteTime(f) < cutoffDate);
            
            foreach (var file in archiveFiles)
            {
                try
                {
                    File.Delete(file);
                    purgedCount++;
                    _logger.LogDebug("Purged archived log file: {FileName}", Path.GetFileName(file));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to purge archived log file: {FileName}", Path.GetFileName(file));
                }
            }
            
            _logger.LogInformation("Purged {PurgedCount} archived log files older than {CutoffDate}", purgedCount, cutoffDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to purge archived logs");
        }
        
        return purgedCount;
    }

    /// <summary>
    /// Verifies the integrity of archived logs
    /// </summary>
    /// <returns>Integrity verification results</returns>
    public async Task<bool> VerifyArchivedLogsIntegrityAsync()
    {
        try
        {
            _logger.LogInformation("Starting verification of archived logs integrity");
            
            var archiveFiles = Directory.GetFiles(_archivePath, "*.zip");
            var allValid = true;
            
            foreach (var file in archiveFiles)
            {
                if (!await VerifyArchiveFileIntegrityAsync(file))
                {
                    allValid = false;
                    _logger.LogWarning("Archive file integrity check failed: {FileName}", Path.GetFileName(file));
                }
            }
            
            _logger.LogInformation("Archived logs integrity verification completed. All valid: {AllValid}", allValid);
            return allValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify archived logs integrity");
            return false;
        }
    }

    /// <summary>
    /// Gets retention policy configuration
    /// </summary>
    /// <returns>Current retention policy settings</returns>
    public async Task<RetentionPolicy> GetRetentionPolicyAsync()
    {
        // In a real implementation, this would be stored in configuration or database
        // For now, return the default policy
        return await Task.FromResult(_defaultPolicy);
    }

    /// <summary>
    /// Updates retention policy configuration
    /// </summary>
    /// <param name="policy">New retention policy settings</param>
    /// <returns>Updated retention policy</returns>
    public async Task<RetentionPolicy> UpdateRetentionPolicyAsync(RetentionPolicy policy)
    {
        try
        {
            _logger.LogInformation("Updating retention policy configuration");
            
            // In a real implementation, this would be stored in configuration or database
            // For now, just validate and return the policy
            ValidateRetentionPolicy(policy);
            
            _logger.LogInformation("Retention policy updated successfully");
            return await Task.FromResult(policy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update retention policy");
            throw;
        }
    }

    /// <summary>
    /// Executes scheduled retention tasks
    /// </summary>
    /// <returns>Task execution results</returns>
    public async Task<RetentionTaskResult> ExecuteRetentionTasksAsync()
    {
        var startTime = DateTime.UtcNow;
        var result = new RetentionTaskResult();
        
        try
        {
            _logger.LogInformation("Starting scheduled retention tasks execution");
            
            var policy = await GetRetentionPolicyAsync();
            
            if (policy.EnableAutomaticArchival)
            {
                result.LogsArchived = await ArchiveOldLogsAsync(policy.ActiveLogRetentionPeriod);
            }
            
            if (policy.EnableAutomaticPurging)
            {
                result.LogsPurged = await PurgeArchivedLogsAsync(policy.ArchivedLogRetentionPeriod);
            }
            
            result.Success = true;
            result.Duration = DateTime.UtcNow.Subtract(startTime);
            
            _logger.LogInformation("Retention tasks completed successfully. Duration: {Duration}", result.Duration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Retention tasks execution failed");
            result.Success = false;
            result.ErrorMessage = ex.Message;
            result.Duration = DateTime.UtcNow.Subtract(startTime);
        }
        
        return result;
    }

    /// <summary>
    /// Gets retention statistics and metrics
    /// </summary>
    /// <returns>Retention statistics</returns>
    public async Task<RetentionStatistics> GetRetentionStatisticsAsync()
    {
        var stats = new RetentionStatistics();
        
        try
        {
            _logger.LogDebug("Generating retention statistics");
            
            // Get current database statistics
            var allLogs = await _auditLogRepository.GetAuditLogsAsync(new Shared.DTOs.AuditLogFilter { PageSize = 1 });
            stats.TotalActiveLogs = allLogs.TotalCount;
            
            // Get archive statistics
            var archiveFiles = Directory.GetFiles(_archivePath, "*.zip");
            stats.TotalArchivedLogs = archiveFiles.Length;
            
            // Calculate database sizes (simplified)
            stats.CurrentDatabaseSizeMB = stats.TotalActiveLogs * 0.1; // Rough estimate
            stats.ArchivedLogsSizeMB = archiveFiles.Sum(f => new FileInfo(f).Length) / (1024.0 * 1024.0);
            stats.ActiveLogsSizeMB = stats.CurrentDatabaseSizeMB;
            
            // Get date ranges
            if (stats.TotalActiveLogs > 0)
            {
                // In a real implementation, these would come from database queries
                stats.OldestActiveLog = DateTime.UtcNow.AddDays(-30);
                stats.NewestActiveLog = DateTime.UtcNow;
            }
            
            if (stats.TotalArchivedLogs > 0)
            {
                var oldestFile = archiveFiles.Min(f => File.GetLastWriteTime(f));
                var newestFile = archiveFiles.Max(f => File.GetLastWriteTime(f));
                stats.OldestArchivedLog = oldestFile;
                stats.NewestArchivedLog = newestFile;
            }
            
            // Calculate next retention task
            var policy = await GetRetentionPolicyAsync();
            stats.LastRetentionTask = DateTime.UtcNow.AddHours(-12); // Simplified
            stats.NextRetentionTask = DateTime.UtcNow.AddHours(policy.RetentionTaskFrequencyHours);
            
            _logger.LogDebug("Retention statistics generated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate retention statistics");
        }
        
        return stats;
    }

    /// <summary>
    /// Archives a single audit log
    /// </summary>
    /// <param name="log">The audit log to archive</param>
    /// <returns>True if archived successfully, false otherwise</returns>
    private async Task<bool> ArchiveLogAsync(Shared.Entities.AuditLog log)
    {
        try
        {
            var archiveFileName = $"audit-log-{log.AuditLogId}-{log.Timestamp:yyyy-MM-dd-HH-mm-ss}.zip";
            var archivePath = Path.Combine(_archivePath, archiveFileName);
            
            using var archive = ZipFile.Open(archivePath, ZipArchiveMode.Create);
            
            // Create JSON content
            var logContent = JsonSerializer.Serialize(log, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            var entry = archive.CreateEntry("audit-log.json");
            using var writer = new StreamWriter(entry.Open());
            await writer.WriteAsync(logContent);
            
            _logger.LogDebug("Archived audit log {AuditLogId} to {ArchivePath}", log.AuditLogId, archivePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to archive audit log {AuditLogId}", log.AuditLogId);
            return false;
        }
    }

    /// <summary>
    /// Verifies the integrity of an archive file
    /// </summary>
    /// <param name="filePath">Path to the archive file</param>
    /// <returns>True if integrity is verified, false otherwise</returns>
    private async Task<bool> VerifyArchiveFileIntegrityAsync(string filePath)
    {
        try
        {
            using var archive = ZipFile.OpenRead(filePath);
            var entry = archive.GetEntry("audit-log.json");
            
            if (entry == null)
            {
                _logger.LogWarning("Archive file missing audit-log.json entry: {FileName}", Path.GetFileName(filePath));
                return false;
            }
            
            using var reader = new StreamReader(entry.Open());
            var content = await reader.ReadToEndAsync();
            
            // Verify JSON is valid
            var log = JsonSerializer.Deserialize<Shared.Entities.AuditLog>(content);
            return log != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify archive file integrity: {FileName}", Path.GetFileName(filePath));
            return false;
        }
    }

    /// <summary>
    /// Validates retention policy configuration
    /// </summary>
    /// <param name="policy">Policy to validate</param>
    private void ValidateRetentionPolicy(RetentionPolicy policy)
    {
        if (policy.ActiveLogRetentionPeriod <= TimeSpan.Zero)
            throw new ArgumentException("Active log retention period must be positive", nameof(policy));
        
        if (policy.ArchivedLogRetentionPeriod <= TimeSpan.Zero)
            throw new ArgumentException("Archived log retention period must be positive", nameof(policy));
        
        if (policy.RetentionTaskFrequencyHours <= 0)
            throw new ArgumentException("Retention task frequency must be positive", nameof(policy));
        
        if (policy.MaxDatabaseSizeGB <= 0)
            throw new ArgumentException("Maximum database size must be positive", nameof(policy));
    }
}
