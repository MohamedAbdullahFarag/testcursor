using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for audit log data operations
/// Specialized interface for audit logs with int primary key (not inheriting from IRepository)
/// </summary>
public interface IAuditLogRepository
{
    /// <summary>
    /// Creates a new audit log entry
    /// </summary>
    /// <param name="auditLog">The audit log to create</param>
    /// <returns>The created audit log with generated ID</returns>
    Task<AuditLog> AddAsync(AuditLog auditLog);

    /// <summary>
    /// Gets an audit log by ID
    /// </summary>
    /// <param name="id">The audit log ID</param>
    /// <returns>The audit log or null if not found</returns>
    Task<AuditLog?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves audit logs based on filter criteria with pagination
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <returns>Paginated result of audit logs</returns>
    Task<PagedResult<AuditLog>> GetAuditLogsAsync(AuditLogFilter filter);
    
    /// <summary>
    /// Retrieves audit logs for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="fromDate">Start date/time</param>
    /// <param name="toDate">End date/time</param>
    /// <returns>Collection of audit logs</returns>
    Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(int userId, DateTime fromDate, DateTime toDate);
    
    /// <summary>
    /// Retrieves security-related audit events
    /// </summary>
    /// <param name="fromDate">Start date/time</param>
    /// <param name="toDate">End date/time</param>
    /// <returns>Collection of security audit logs</returns>
    Task<IEnumerable<AuditLog>> GetSecurityEventsAsync(DateTime fromDate, DateTime toDate);
    
    /// <summary>
    /// Retrieves audit logs for a specific entity
    /// </summary>
    /// <param name="entityType">Entity type</param>
    /// <param name="entityId">Entity ID</param>
    /// <returns>Collection of entity audit logs</returns>
    Task<IEnumerable<AuditLog>> GetEntityAuditLogsAsync(string entityType, string entityId);
    
    /// <summary>
    /// Archives audit logs older than specified retention period
    /// </summary>
    /// <param name="cutoffDate">Date threshold for archiving</param>
    /// <returns>Number of archived logs</returns>
    Task<int> ArchiveLogsAsync(DateTime cutoffDate);
    
    /// <summary>
    /// Verifies the integrity of an audit log entry
    /// </summary>
    /// <param name="auditLogId">Audit log ID</param>
    /// <returns>True if log integrity is verified</returns>
    Task<bool> VerifyLogIntegrityAsync(int auditLogId);
}
