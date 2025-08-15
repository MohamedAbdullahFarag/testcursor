using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Core.Services;

/// <summary>
/// Service interface for audit log integrity verification
/// Provides cryptographic integrity checking and tamper detection capabilities
/// </summary>
public interface ILogIntegrityService
{
    /// <summary>
    /// Generates a cryptographic hash for an audit log entry
    /// </summary>
    /// <param name="auditLogId">The audit log ID to hash</param>
    /// <returns>Base64 encoded hash value</returns>
    Task<string> GenerateLogHashAsync(int auditLogId);
    
    /// <summary>
    /// Verifies the integrity of a specific audit log entry
    /// </summary>
    /// <param name="auditLogId">The audit log ID to verify</param>
    /// <returns>True if integrity is verified, false otherwise</returns>
    Task<bool> VerifyLogIntegrityAsync(int auditLogId);
    
    /// <summary>
    /// Verifies the integrity of all audit logs within a date range
    /// </summary>
    /// <param name="fromDate">Start date for verification</param>
    /// <param name="toDate">End date for verification</param>
    /// <returns>Dictionary mapping audit log IDs to integrity status</returns>
    Task<Dictionary<int, bool>> VerifyAllLogsIntegrityAsync(DateTime fromDate, DateTime toDate);
    
    /// <summary>
    /// Generates an integrity report for audit logs
    /// </summary>
    /// <param name="fromDate">Start date for report</param>
    /// <param name="toDate">End date for report</param>
    /// <returns>Comprehensive integrity report</returns>
    Task<IntegrityReport> GenerateIntegrityReportAsync(DateTime fromDate, DateTime toDate);
    
    /// <summary>
    /// Signs an audit log entry with a digital signature
    /// </summary>
    /// <param name="auditLogId">The audit log ID to sign</param>
    /// <returns>Digital signature as base64 string</returns>
    Task<string> SignAuditLogAsync(int auditLogId);
    
    /// <summary>
    /// Verifies a digital signature for an audit log entry
    /// </summary>
    /// <param name="auditLogId">The audit log ID</param>
    /// <param name="signature">The digital signature to verify</param>
    /// <returns>True if signature is valid, false otherwise</returns>
    Task<bool> VerifySignatureAsync(int auditLogId, string signature);
}

/// <summary>
/// Comprehensive integrity report for audit logs
/// </summary>
public class IntegrityReport
{
    /// <summary>
    /// Total number of audit logs checked
    /// </summary>
    public int TotalLogsChecked { get; set; }
    
    /// <summary>
    /// Number of logs with verified integrity
    /// </summary>
    public int LogsWithVerifiedIntegrity { get; set; }
    
    /// <summary>
    /// Number of logs with integrity issues
    /// </summary>
    public int LogsWithIntegrityIssues { get; set; }
    
    /// <summary>
    /// List of audit log IDs with integrity issues
    /// </summary>
    public List<int> LogsWithIssues { get; set; } = new();
    
    /// <summary>
    /// Overall integrity score (0-100)
    /// </summary>
    public double IntegrityScore => TotalLogsChecked > 0 ? (LogsWithVerifiedIntegrity * 100.0) / TotalLogsChecked : 0;
    
    /// <summary>
    /// Whether all logs passed integrity verification
    /// </summary>
    public bool AllLogsIntegrityVerified => LogsWithIntegrityIssues == 0;
    
    /// <summary>
    /// Timestamp when the report was generated
    /// </summary>
    public DateTime ReportGeneratedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date range covered by the report
    /// </summary>
    public DateTime FromDate { get; set; }
    
    /// <summary>
    /// Date range covered by the report
    /// </summary>
    public DateTime ToDate { get; set; }
}
