using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services;

/// <summary>
/// Service interface for audit logging functionality
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Logs an audit entry
    /// </summary>
    /// <param name="entry">Audit log entry details</param>
    /// <returns>Created audit log ID</returns>
    Task<int> LogAsync(AuditLogEntry entry);
    
    /// <summary>
    /// Logs a user action with optional before/after state
    /// </summary>
    /// <param name="userId">User ID who performed the action</param>
    /// <param name="action">Action performed</param>
    /// <param name="entityType">Type of entity affected</param>
    /// <param name="entityId">ID of affected entity</param>
    /// <param name="oldValues">Optional previous state</param>
    /// <param name="newValues">Optional new state</param>
    /// <returns>Created audit log ID</returns>
    Task<int> LogUserActionAsync(int? userId, string action, string entityType, string entityId, 
        object? oldValues = null, object? newValues = null);
    
    /// <summary>
    /// Logs a security-related event
    /// </summary>
    /// <param name="userIdentifier">User identifier (email/username)</param>
    /// <param name="action">Security action</param>
    /// <param name="details">Event details</param>
    /// <param name="severity">Event severity</param>
    /// <returns>Created audit log ID</returns>
    Task<int> LogSecurityEventAsync(string userIdentifier, string action, string details, 
        AuditSeverity severity = AuditSeverity.High);
    
    /// <summary>
    /// Logs a system-generated action
    /// </summary>
    /// <param name="action">System action</param>
    /// <param name="details">Action details</param>
    /// <param name="entityType">Optional entity type</param>
    /// <param name="entityId">Optional entity ID</param>
    /// <returns>Created audit log ID</returns>
    Task<int> LogSystemActionAsync(string action, string details, 
        string entityType = "System", string? entityId = null);
    
    /// <summary>
    /// Retrieves audit logs with filtering and pagination
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <returns>Paginated audit logs</returns>
    Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(AuditLogFilter filter);
    
    /// <summary>
    /// Retrieves audit logs for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="filter">Filter criteria</param>
    /// <returns>Paginated user-specific audit logs</returns>
    Task<PagedResult<AuditLogDto>> GetUserAuditLogsAsync(int userId, AuditLogFilter filter);
    
    /// <summary>
    /// Retrieves security events within a date range
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Collection of security events</returns>
    Task<IEnumerable<AuditLogDto>> GetSecurityEventsAsync(DateTime fromDate, DateTime toDate);
    
    /// <summary>
    /// Exports audit logs based on filter criteria
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <param name="format">Export format</param>
    /// <returns>Exported file as byte array</returns>
    Task<byte[]> ExportAuditLogsAsync(AuditLogFilter filter, AuditLogExportFormat format);
    
    /// <summary>
    /// Archives audit logs older than the retention period
    /// </summary>
    /// <param name="retentionPeriod">Retention period</param>
    /// <returns>Number of archived logs</returns>
    Task<int> ArchiveOldLogsAsync(TimeSpan retentionPeriod);
    
    /// <summary>
    /// Verifies the integrity of audit logs
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Integrity verification results</returns>
    Task<Dictionary<int, bool>> VerifyLogsIntegrityAsync(DateTime fromDate, DateTime toDate);
}
