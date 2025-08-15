using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services;
using Ikhtibar.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Service implementation for audit log integrity verification
/// Provides cryptographic integrity checking and tamper detection capabilities
/// </summary>
public class LogIntegrityService : ILogIntegrityService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<LogIntegrityService> _logger;
    private readonly string _secretKey;

    /// <summary>
    /// Constructor for LogIntegrityService
    /// </summary>
    /// <param name="auditLogRepository">Audit log repository</param>
    /// <param name="logger">Logger instance</param>
    public LogIntegrityService(
        IAuditLogRepository auditLogRepository,
        ILogger<LogIntegrityService> logger)
    {
        _auditLogRepository = auditLogRepository;
        _logger = logger;
        
        // In production, this should come from secure configuration
        _secretKey = Environment.GetEnvironmentVariable("AUDIT_LOG_SECRET_KEY") ?? 
                    "default-secret-key-for-development-only-change-in-production";
    }

    /// <summary>
    /// Generates a cryptographic hash for an audit log entry
    /// </summary>
    /// <param name="auditLogId">The audit log ID to hash</param>
    /// <returns>Base64 encoded hash value</returns>
    public async Task<string> GenerateLogHashAsync(int auditLogId)
    {
        try
        {
            var auditLog = await _auditLogRepository.GetByIdAsync(auditLogId);
            if (auditLog == null)
            {
                _logger.LogWarning("Cannot generate hash for non-existent audit log {AuditLogId}", auditLogId);
                return string.Empty;
            }

            var hashInput = CreateHashInput(auditLog);
            var hash = await ComputeHashAsync(hashInput);
            
            _logger.LogDebug("Generated hash for audit log {AuditLogId}: {Hash}", auditLogId, hash);
            return hash;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate hash for audit log {AuditLogId}", auditLogId);
            throw;
        }
    }

    /// <summary>
    /// Verifies the integrity of a specific audit log entry
    /// </summary>
    /// <param name="auditLogId">The audit log ID to verify</param>
    /// <returns>True if integrity is verified, false otherwise</returns>
    public async Task<bool> VerifyLogIntegrityAsync(int auditLogId)
    {
        try
        {
            var auditLog = await _auditLogRepository.GetByIdAsync(auditLogId);
            if (auditLog == null)
            {
                _logger.LogWarning("Cannot verify integrity for non-existent audit log {AuditLogId}", auditLogId);
                return false;
            }

            var expectedHash = await GenerateLogHashAsync(auditLogId);
            var storedHash = auditLog.Details; // In a real implementation, this would be a separate field
            
            var isValid = !string.IsNullOrEmpty(expectedHash) && expectedHash == storedHash;
            
            _logger.LogDebug("Integrity verification for audit log {AuditLogId}: {Result}", auditLogId, isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify integrity for audit log {AuditLogId}", auditLogId);
            return false;
        }
    }

    /// <summary>
    /// Verifies the integrity of all audit logs within a date range
    /// </summary>
    /// <param name="fromDate">Start date for verification</param>
    /// <param name="toDate">End date for verification</param>
    /// <returns>Dictionary mapping audit log IDs to integrity status</returns>
    public async Task<Dictionary<int, bool>> VerifyAllLogsIntegrityAsync(DateTime fromDate, DateTime toDate)
    {
        var results = new Dictionary<int, bool>();
        
        try
        {
            _logger.LogInformation("Starting integrity verification for logs from {FromDate} to {ToDate}", fromDate, toDate);
            
            // Get all audit logs in the date range
            var filter = new Shared.DTOs.AuditLogFilter
            {
                FromDate = fromDate,
                ToDate = toDate,
                PageSize = 1000 // Large page size to get all logs
            };
            
            var auditLogs = await _auditLogRepository.GetAuditLogsAsync(filter);
            
            foreach (var auditLog in auditLogs.Items)
            {
                var isValid = await VerifyLogIntegrityAsync(auditLog.AuditLogId);
                results[auditLog.AuditLogId] = isValid;
            }
            
            var validCount = results.Count(r => r.Value);
            var invalidCount = results.Count(r => !r.Value);
            
            _logger.LogInformation("Integrity verification completed. Valid: {ValidCount}, Invalid: {InvalidCount}", 
                validCount, invalidCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify integrity for logs from {FromDate} to {ToDate}", fromDate, toDate);
        }
        
        return results;
    }

    /// <summary>
    /// Generates an integrity report for audit logs
    /// </summary>
    /// <param name="fromDate">Start date for report</param>
    /// <param name="toDate">End date for report</param>
    /// <returns>Comprehensive integrity report</returns>
    public async Task<IntegrityReport> GenerateIntegrityReportAsync(DateTime fromDate, DateTime toDate)
    {
        var report = new IntegrityReport
        {
            FromDate = fromDate,
            ToDate = toDate
        };
        
        try
        {
            _logger.LogInformation("Generating integrity report for logs from {FromDate} to {ToDate}", fromDate, toDate);
            
            var integrityResults = await VerifyAllLogsIntegrityAsync(fromDate, toDate);
            
            report.TotalLogsChecked = integrityResults.Count;
            report.LogsWithVerifiedIntegrity = integrityResults.Count(r => r.Value);
            report.LogsWithIntegrityIssues = integrityResults.Count(r => !r.Value);
            report.LogsWithIssues = integrityResults.Where(r => !r.Value).Select(r => r.Key).ToList();
            
            _logger.LogInformation("Integrity report generated. Score: {Score:F1}%", report.IntegrityScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate integrity report for logs from {FromDate} to {ToDate}", fromDate, toDate);
        }
        
        return report;
    }

    /// <summary>
    /// Signs an audit log entry with a digital signature
    /// </summary>
    /// <param name="auditLogId">The audit log ID to sign</param>
    /// <returns>Digital signature as base64 string</returns>
    public async Task<string> SignAuditLogAsync(int auditLogId)
    {
        try
        {
            var auditLog = await _auditLogRepository.GetByIdAsync(auditLogId);
            if (auditLog == null)
            {
                _logger.LogWarning("Cannot sign non-existent audit log {AuditLogId}", auditLogId);
                return string.Empty;
            }

            var dataToSign = CreateHashInput(auditLog);
            var signature = await ComputeSignatureAsync(dataToSign);
            
            _logger.LogDebug("Generated signature for audit log {AuditLogId}: {Signature}", auditLogId, signature);
            return signature;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sign audit log {AuditLogId}", auditLogId);
            throw;
        }
    }

    /// <summary>
    /// Verifies a digital signature for an audit log entry
    /// </summary>
    /// <param name="auditLogId">The audit log ID</param>
    /// <param name="signature">The digital signature to verify</param>
    /// <returns>True if signature is valid, false otherwise</returns>
    public async Task<bool> VerifySignatureAsync(int auditLogId, string signature)
    {
        try
        {
            if (string.IsNullOrEmpty(signature))
            {
                _logger.LogWarning("Cannot verify empty signature for audit log {AuditLogId}", auditLogId);
                return false;
            }

            var auditLog = await _auditLogRepository.GetByIdAsync(auditLogId);
            if (auditLog == null)
            {
                _logger.LogWarning("Cannot verify signature for non-existent audit log {AuditLogId}", auditLogId);
                return false;
            }

            var expectedSignature = await SignAuditLogAsync(auditLogId);
            var isValid = expectedSignature == signature;
            
            _logger.LogDebug("Signature verification for audit log {AuditLogId}: {Result}", auditLogId, isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify signature for audit log {AuditLogId}", auditLogId);
            return false;
        }
    }

    /// <summary>
    /// Creates the input string for hashing based on audit log content
    /// </summary>
    /// <param name="auditLog">The audit log to hash</param>
    /// <returns>String representation for hashing</returns>
    private string CreateHashInput(AuditLog auditLog)
    {
        var hashData = new
        {
            auditLog.AuditLogId,
            auditLog.UserId,
            auditLog.UserIdentifier,
            auditLog.Action,
            auditLog.EntityType,
            auditLog.EntityId,
            auditLog.Details,
            auditLog.OldValues,
            auditLog.NewValues,
            auditLog.IpAddress,
            auditLog.UserAgent,
            auditLog.SessionId,
            auditLog.Severity,
            auditLog.Category,
            auditLog.Timestamp,
            auditLog.IsSystemAction,
            auditLog.CreatedAt,
            auditLog.CreatedBy
        };

        return JsonSerializer.Serialize(hashData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
    }

    /// <summary>
    /// Computes a SHA256 hash of the input data
    /// </summary>
    /// <param name="input">Input data to hash</param>
    /// <returns>Base64 encoded hash</returns>
    private async Task<string> ComputeHashAsync(string input)
    {
        using var sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input + _secretKey);
        var hashBytes = await Task.Run(() => sha256.ComputeHash(inputBytes));
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Computes a digital signature using HMAC-SHA256
    /// </summary>
    /// <param name="input">Input data to sign</param>
    /// <returns>Base64 encoded signature</returns>
    private async Task<string> ComputeSignatureAsync(string input)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var signatureBytes = await Task.Run(() => hmac.ComputeHash(inputBytes));
        return Convert.ToBase64String(signatureBytes);
    }
}
