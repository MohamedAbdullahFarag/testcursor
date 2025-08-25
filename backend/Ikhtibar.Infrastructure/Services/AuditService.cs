using System.Text.Json;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Globalization;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Service implementation for audit logging functionality
/// </summary>
public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly ILogger<AuditService> _logger;
    
    /// <summary>
    /// Constructor for AuditService
    /// </summary>
    /// <param name="auditLogRepository">Audit log repository</param>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    /// <param name="mapper">AutoMapper instance</param>
    /// <param name="logger">Logger instance</param>
    public AuditService(
        IAuditLogRepository auditLogRepository,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        ILogger<AuditService> logger)
    {
        _auditLogRepository = auditLogRepository;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _logger = logger;
        
    // NOTE: EPPlus 8+ changed license configuration API and the old
    // ExcelPackage.LicenseContext setter may throw at runtime.
    // The license should be configured once at application startup
    // (for example in Program.cs) using the EPPlus 8+ recommended API.
    // Removing license configuration here avoids throwing during DI/constructor.
    }
    
    /// <summary>
    /// Logs an audit entry
    /// </summary>
    /// <param name="entry">Audit log entry details</param>
    /// <returns>Created audit log ID</returns>
    public async Task<int> LogAsync(AuditLogEntry entry)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        var auditLog = new AuditLog
        {
            UserId = entry.UserId,
            UserIdentifier = entry.UserIdentifier,
            Action = entry.Action,
            EntityType = entry.EntityType,
            EntityId = entry.EntityId,
            Details = entry.Details,
            OldValues = entry.OldValues != null ? JsonSerializer.Serialize(entry.OldValues) : null,
            NewValues = entry.NewValues != null ? JsonSerializer.Serialize(entry.NewValues) : null,
            IpAddress = GetClientIpAddress(httpContext),
            UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
            SessionId = GetSessionIdSafely(httpContext),
            Severity = entry.Severity,
            Category = entry.Category,
            Timestamp = DateTime.UtcNow,
            IsSystemAction = entry.IsSystemAction
        };
        
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = entry.UserId ?? 0,
            ["UserIdentifier"] = entry.UserIdentifier,
            ["Action"] = entry.Action,
            ["EntityType"] = entry.EntityType,
            ["EntityId"] = entry.EntityId ?? "null",
            ["Severity"] = entry.Severity,
            ["Category"] = entry.Category
        });
        
        _logger.LogInformation("Creating audit log for action {Action} on {EntityType} {EntityId}", 
            entry.Action, entry.EntityType, entry.EntityId);
        
        var createdLog = await _auditLogRepository.AddAsync(auditLog);
        return createdLog.AuditLogId;
    }
    
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
    public async Task<int> LogUserActionAsync(int? userId, string action, string entityType, string entityId, 
        object? oldValues = null, object? newValues = null)
    {
        var userIdentifier = userId.HasValue ? userId.Value.ToString() : "Anonymous";
        
        var entry = new AuditLogEntry
        {
            UserId = userId,
            UserIdentifier = userIdentifier,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            Severity = DetermineSeverity(action),
            Category = DetermineCategory(action, entityType),
            IsSystemAction = false
        };
        
        return await LogAsync(entry);
    }
    
    /// <summary>
    /// Logs a security-related event
    /// </summary>
    /// <param name="userIdentifier">User identifier (email/username)</param>
    /// <param name="action">Security action</param>
    /// <param name="details">Event details</param>
    /// <param name="severity">Event severity</param>
    /// <returns>Created audit log ID</returns>
    public async Task<int> LogSecurityEventAsync(string userIdentifier, string action, string details, 
        AuditSeverity severity = AuditSeverity.High)
    {
        var entry = new AuditLogEntry
        {
            UserId = null, // May not have a valid user ID for security events
            UserIdentifier = userIdentifier,
            Action = action,
            EntityType = "Security",
            Details = details,
            Severity = severity,
            Category = AuditCategory.Security,
            IsSystemAction = false
        };
        
        _logger.LogWarning("Security event {Action} for user {UserIdentifier}: {Details}",
            action, userIdentifier, details);
            
        return await LogAsync(entry);
    }
    
    /// <summary>
    /// Logs a system-generated action
    /// </summary>
    /// <param name="action">System action</param>
    /// <param name="details">Action details</param>
    /// <param name="entityType">Optional entity type</param>
    /// <param name="entityId">Optional entity ID</param>
    /// <returns>Created audit log ID</returns>
    public async Task<int> LogSystemActionAsync(string action, string details, 
        string entityType = "System", string? entityId = null)
    {
        var entry = new AuditLogEntry
        {
            UserId = null,
            UserIdentifier = "System",
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            Severity = DetermineSeverity(action),
            Category = AuditCategory.System,
            IsSystemAction = true
        };
        
        return await LogAsync(entry);
    }
    
    /// <summary>
    /// Retrieves audit logs with filtering and pagination
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <returns>Paginated audit logs</returns>
    public async Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(AuditLogFilter filter)
    {
        var result = await _auditLogRepository.GetAuditLogsAsync(filter);
        
        return new PagedResult<AuditLogDto>
        {
            Items = _mapper.Map<List<AuditLogDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }
    
    /// <summary>
    /// Retrieves audit logs for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="filter">Filter criteria</param>
    /// <returns>Paginated user-specific audit logs</returns>
    public async Task<PagedResult<AuditLogDto>> GetUserAuditLogsAsync(int userId, AuditLogFilter filter)
    {
        filter.UserId = userId;
        return await GetAuditLogsAsync(filter);
    }
    
    /// <summary>
    /// Retrieves security events within a date range
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Collection of security events</returns>
    public async Task<IEnumerable<AuditLogDto>> GetSecurityEventsAsync(DateTime fromDate, DateTime toDate)
    {
        var securityEvents = await _auditLogRepository.GetSecurityEventsAsync(fromDate, toDate);
        return _mapper.Map<IEnumerable<AuditLogDto>>(securityEvents);
    }
    
    /// <summary>
    /// Exports audit logs based on filter criteria
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <param name="format">Export format</param>
    /// <returns>Exported file as byte array</returns>
    public async Task<byte[]> ExportAuditLogsAsync(AuditLogFilter filter, AuditLogExportFormat format)
    {
        // Temporarily increase page size for export
        var exportFilter = new AuditLogFilter
        {
            UserId = filter.UserId,
            UserIdentifier = filter.UserIdentifier,
            Action = filter.Action,
            EntityType = filter.EntityType,
            EntityId = filter.EntityId,
            Severity = filter.Severity,
            Category = filter.Category,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            IpAddress = filter.IpAddress,
            IncludeSystemActions = filter.IncludeSystemActions,
            SearchText = filter.SearchText,
            Page = 1,
            PageSize = 10000, // Use a larger page size for exports
            SortBy = filter.SortBy,
            SortDirection = filter.SortDirection
        };
        
        var logs = await GetAuditLogsAsync(exportFilter);
        
        using var stream = new MemoryStream();
        
        switch (format)
        {
            case AuditLogExportFormat.CSV:
                await ExportToCsvAsync(stream, logs.Items);
                break;
                
            case AuditLogExportFormat.Excel:
                await ExportToExcelAsync(stream, logs.Items);
                break;
                
            case AuditLogExportFormat.Json:
                await ExportToJsonAsync(stream, logs.Items);
                break;
                
            default:
                throw new ArgumentException("Unsupported export format", nameof(format));
        }
        
        return stream.ToArray();
    }
    
    /// <summary>
    /// Archives audit logs older than the retention period
    /// </summary>
    /// <param name="retentionPeriod">Retention period</param>
    /// <returns>Number of archived logs</returns>
    public async Task<int> ArchiveOldLogsAsync(TimeSpan retentionPeriod)
    {
        var cutoffDate = DateTime.UtcNow.Subtract(retentionPeriod);
        _logger.LogInformation("Archiving audit logs older than {CutoffDate}", cutoffDate);
        
        return await _auditLogRepository.ArchiveLogsAsync(cutoffDate);
    }
    
    /// <summary>
    /// Verifies the integrity of audit logs
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Integrity verification results</returns>
    public async Task<Dictionary<int, bool>> VerifyLogsIntegrityAsync(DateTime fromDate, DateTime toDate)
    {
        var filter = new AuditLogFilter
        {
            FromDate = fromDate,
            ToDate = toDate,
            Page = 1,
            PageSize = 1000 // Use a larger page size for verification
        };
        
        var logs = await _auditLogRepository.GetAuditLogsAsync(filter);
        var results = new Dictionary<int, bool>();
        
        foreach (var log in logs.Items)
        {
            bool isValid = await _auditLogRepository.VerifyLogIntegrityAsync(log.AuditLogId);
            results.Add(log.AuditLogId, isValid);
            
            if (!isValid)
            {
                _logger.LogWarning("Audit log integrity check failed for log ID {LogId}", log.AuditLogId);
            }
        }
        
        return results;
    }
    
    #region Helper Methods
    
    /// <summary>
    /// Gets the client IP address from the HTTP context
    /// </summary>
    private string? GetClientIpAddress(HttpContext? httpContext)
    {
        if (httpContext == null) return null;
        
        // Try to get IP from forwarded header
        var forwardedHeader = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            return forwardedHeader.Split(',')[0].Trim();
        }
        
        // Fall back to connection remote IP
        return httpContext.Connection.RemoteIpAddress?.ToString();
    }
    
    /// <summary>
    /// Safely gets the session ID from the HTTP context
    /// </summary>
    private string? GetSessionIdSafely(HttpContext? httpContext)
    {
        if (httpContext == null) return null;
        
        try
        {
            // Only access session if it's configured
            return httpContext.Session?.Id;
        }
        catch (InvalidOperationException)
        {
            // Session is not configured, return null
            return null;
        }
    }
    
    /// <summary>
    /// Determines the severity based on action type
    /// </summary>
    private AuditSeverity DetermineSeverity(string action)
    {
        // Critical actions (security-sensitive operations)
        if (action.StartsWith("DELETE_") ||
            action.Contains("ADMIN") || 
            action.Contains("PERMISSION") ||
            action.Contains("ROLE") ||
            action.Contains("PASSWORD") ||
            action.Contains("CREDENTIAL") ||
            action.Contains("CONFIG") ||
            action.Contains("SECURITY"))
        {
            return AuditSeverity.Critical;
        }
        
        // High severity actions (data modification)
        if (action.StartsWith("CREATE_") ||
            action.StartsWith("UPDATE_") ||
            action.StartsWith("MODIFY_") ||
            action.StartsWith("IMPORT_") ||
            action.StartsWith("EXPORT_"))
        {
            return AuditSeverity.High;
        }
        
        // Medium severity actions (status changes)
        if (action.Contains("STATUS") ||
            action.Contains("STATE") ||
            action.Contains("LOGIN") ||
            action.Contains("LOGOUT"))
        {
            return AuditSeverity.Medium;
        }
        
        // Default to low severity for read operations
        return AuditSeverity.Low;
    }
    
    /// <summary>
    /// Determines the category based on action and entity type
    /// </summary>
    private AuditCategory DetermineCategory(string action, string entityType)
    {
        if (action.Contains("LOGIN") || 
            action.Contains("LOGOUT") ||
            action.Contains("PASSWORD") ||
            action.Contains("CREDENTIAL") ||
            action.Contains("MFA") ||
            entityType == "Authentication")
        {
            return AuditCategory.Authentication;
        }
        
        if (action.Contains("PERMISSION") ||
            action.Contains("ROLE") ||
            action.Contains("ACCESS") ||
            entityType == "Authorization" ||
            entityType == "Permission" ||
            entityType == "Role")
        {
            return AuditCategory.Authorization;
        }
        
        if (entityType == "User" ||
            entityType == "Profile" ||
            action.Contains("USER"))
        {
            return AuditCategory.UserManagement;
        }
        
        if (action.StartsWith("CREATE_") ||
            action.StartsWith("UPDATE_") ||
            action.StartsWith("DELETE_") ||
            action.StartsWith("READ_"))
        {
            return AuditCategory.DataAccess;
        }
        
        if (entityType == "System" ||
            action.Contains("CONFIG") ||
            action.Contains("SETTING") ||
            action.Contains("SYSTEM"))
        {
            return AuditCategory.System;
        }
        
        if (action.Contains("SECURITY") ||
            action.Contains("BREACH") ||
            action.Contains("VIOLATION"))
        {
            return AuditCategory.Security;
        }
        
        // Default to System
        return AuditCategory.System;
    }
    
    /// <summary>
    /// Exports audit logs to CSV format
    /// </summary>
    private async Task ExportToCsvAsync(Stream stream, IEnumerable<AuditLogDto> logs)
    {
        using var writer = new StreamWriter(stream, leaveOpen: true);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
        
        await csv.WriteRecordsAsync(logs);
        await writer.FlushAsync();
        stream.Position = 0;
    }
    
    /// <summary>
    /// Exports audit logs to Excel format
    /// </summary>
    private async Task ExportToExcelAsync(Stream stream, IEnumerable<AuditLogDto> logs)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Audit Logs");
        
        // Add headers
        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[1, 2].Value = "Timestamp";
        worksheet.Cells[1, 3].Value = "User";
        worksheet.Cells[1, 4].Value = "Action";
        worksheet.Cells[1, 5].Value = "Entity Type";
        worksheet.Cells[1, 6].Value = "Entity ID";
        worksheet.Cells[1, 7].Value = "Details";
        worksheet.Cells[1, 8].Value = "Severity";
        worksheet.Cells[1, 9].Value = "Category";
        worksheet.Cells[1, 10].Value = "IP Address";
        
        // Style header row
        using (var range = worksheet.Cells[1, 1, 1, 10])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }
        
        // Add data
        int row = 2;
        foreach (var log in logs)
        {
            worksheet.Cells[row, 1].Value = log.AuditLogId;
            worksheet.Cells[row, 2].Value = log.Timestamp;
            worksheet.Cells[row, 3].Value = log.UserIdentifier;
            worksheet.Cells[row, 4].Value = log.Action;
            worksheet.Cells[row, 5].Value = log.EntityType;
            worksheet.Cells[row, 6].Value = log.EntityId;
            worksheet.Cells[row, 7].Value = log.Details;
            worksheet.Cells[row, 8].Value = log.SeverityText;
            worksheet.Cells[row, 9].Value = log.CategoryText;
            worksheet.Cells[row, 10].Value = log.IpAddress;
            
            // Colorize severity
            var severityCell = worksheet.Cells[row, 8];
            switch (log.Severity)
            {
                case AuditSeverity.Critical:
                    severityCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    severityCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                    severityCell.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    break;
                case AuditSeverity.High:
                    severityCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    severityCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    break;
                case AuditSeverity.Medium:
                    severityCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    severityCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    break;
            }
            
            row++;
        }
        
        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        // Save to stream
        await package.SaveAsAsync(stream);
        stream.Position = 0;
    }
    
    /// <summary>
    /// Exports audit logs to JSON format
    /// </summary>
    private async Task ExportToJsonAsync(Stream stream, IEnumerable<AuditLogDto> logs)
    {
        await JsonSerializer.SerializeAsync(stream, logs, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        stream.Position = 0;
    }
    
    #endregion
}
