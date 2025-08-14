using Ikhtibar.Core.Services;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for managing audit logs
/// </summary>
[ApiController]
[Route("api/audit-logs")]
[Authorize(Roles = "Admin,Supervisor")]
[ApiExplorerSettings(GroupName = "v1")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditLogsController> _logger;
    
    /// <summary>
    /// Constructor for AuditLogsController
    /// </summary>
    /// <param name="auditService">Audit service</param>
    /// <param name="logger">Logger instance</param>
    public AuditLogsController(IAuditService auditService, ILogger<AuditLogsController> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }
    
    /// <summary>
    /// Gets audit logs with filtering and pagination
    /// </summary>
    /// <param name="userId">Filter by user ID</param>
    /// <param name="action">Filter by action</param>
    /// <param name="entityType">Filter by entity type</param>
    /// <param name="entityId">Filter by entity ID</param>
    /// <param name="severity">Filter by severity</param>
    /// <param name="category">Filter by category</param>
    /// <param name="fromDate">Filter from this date</param>
    /// <param name="toDate">Filter to this date</param>
    /// <param name="searchText">Text search</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDirection">Sort direction</param>
    /// <returns>Paginated audit logs</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<AuditLogDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> GetAuditLogs(
        [FromQuery] int? userId = null,
        [FromQuery] string? action = null,
        [FromQuery] string? entityType = null,
        [FromQuery] string? entityId = null,
        [FromQuery] AuditSeverity? severity = null,
        [FromQuery] AuditCategory? category = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? searchText = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "Timestamp",
        [FromQuery] string sortDirection = "desc")
    {
        var filter = new AuditLogFilter
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Severity = severity,
            Category = category,
            FromDate = fromDate,
            ToDate = toDate,
            SearchText = searchText,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection
        };
        
        _logger.LogInformation("Getting audit logs with filter: UserId={UserId}, Action={Action}, EntityType={EntityType}",
            userId, action, entityType);
            
        var results = await _auditService.GetAuditLogsAsync(filter);
        return Ok(results);
    }
    
    /// <summary>
    /// Gets security-related events
    /// </summary>
    /// <param name="fromDate">Filter from this date</param>
    /// <param name="toDate">Filter to this date</param>
    /// <returns>Security events</returns>
    [HttpGet("security-events")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AuditLogDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetSecurityEvents(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        fromDate ??= DateTime.UtcNow.AddDays(-30);
        toDate ??= DateTime.UtcNow;
        
        var results = await _auditService.GetSecurityEventsAsync(fromDate.Value, toDate.Value);
        return Ok(results);
    }
    
    /// <summary>
    /// Gets audit logs for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="fromDate">Filter from this date</param>
    /// <param name="toDate">Filter to this date</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>User-specific audit logs</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<AuditLogDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> GetUserAuditLogs(
        [FromRoute] int userId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var filter = new AuditLogFilter
        {
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            PageSize = pageSize,
            SortBy = "Timestamp",
            SortDirection = "desc"
        };
        
        var results = await _auditService.GetUserAuditLogsAsync(userId, filter);
        
        if (results.TotalCount == 0)
        {
            return NotFound($"No audit logs found for user ID {userId}");
        }
        
        return Ok(results);
    }
    
    /// <summary>
    /// Gets audit logs for a specific entity
    /// </summary>
    /// <param name="entityType">Entity type</param>
    /// <param name="entityId">Entity ID</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Entity-specific audit logs</returns>
    [HttpGet("entity/{entityType}/{entityId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<AuditLogDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> GetEntityAuditLogs(
        [FromRoute] string entityType,
        [FromRoute] string entityId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var filter = new AuditLogFilter
        {
            EntityType = entityType,
            EntityId = entityId,
            Page = page,
            PageSize = pageSize,
            SortBy = "Timestamp",
            SortDirection = "desc"
        };
        
        var results = await _auditService.GetAuditLogsAsync(filter);
        
        if (results.TotalCount == 0)
        {
            return NotFound($"No audit logs found for {entityType} with ID {entityId}");
        }
        
        return Ok(results);
    }
    
    /// <summary>
    /// Exports audit logs based on filter criteria
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <param name="format">Export format</param>
    /// <returns>File download</returns>
    [HttpPost("export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ExportAuditLogs(
        [FromBody] AuditLogFilter filter,
        [FromQuery] AuditLogExportFormat format = AuditLogExportFormat.CSV)
    {
        if (filter == null)
        {
            return BadRequest("Filter criteria required");
        }
        
        try
        {
            var fileData = await _auditService.ExportAuditLogsAsync(filter, format);
            string fileName = $"audit_logs_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
            string contentType;
            string fileExtension;
            
            switch (format)
            {
                case AuditLogExportFormat.CSV:
                    contentType = "text/csv";
                    fileExtension = ".csv";
                    break;
                    
                case AuditLogExportFormat.Excel:
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    fileExtension = ".xlsx";
                    break;
                    
                case AuditLogExportFormat.Json:
                    contentType = "application/json";
                    fileExtension = ".json";
                    break;
                    
                default:
                    return BadRequest("Unsupported export format");
            }
            
            _logger.LogInformation("Exporting audit logs to {Format} format", format);
            
            // Log the export action
            await _auditService.LogSystemActionAsync(
                "EXPORT_AUDIT_LOGS",
                $"Exported audit logs in {format} format with filter: {filter.EntityType} {filter.Action}",
                "AuditLog",
                null);
                
            return File(fileData, contentType, $"{fileName}{fileExtension}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export audit logs");
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate export");
        }
    }
    
    /// <summary>
    /// Archives old audit logs
    /// </summary>
    /// <param name="retentionDays">Number of days to retain logs</param>
    /// <returns>Archive result</returns>
    [HttpPost("archive")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ArchiveOldLogs([FromQuery] int retentionDays = 365)
    {
        if (retentionDays < 30)
        {
            return BadRequest("Retention period must be at least 30 days");
        }
        
        try
        {
            var archivedCount = await _auditService.ArchiveOldLogsAsync(TimeSpan.FromDays(retentionDays));
            
            _logger.LogInformation("Archived {ArchivedCount} audit logs older than {RetentionDays} days", 
                archivedCount, retentionDays);
                
            // Log the archive action
            await _auditService.LogSystemActionAsync(
                "ARCHIVE_AUDIT_LOGS",
                $"Archived {archivedCount} audit logs older than {retentionDays} days",
                "AuditLog",
                null);
                
            return Ok(new { archivedCount, message = $"Successfully archived {archivedCount} logs" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to archive audit logs");
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to archive logs");
        }
    }
    
    /// <summary>
    /// Verifies integrity of audit logs
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Verification results</returns>
    [HttpGet("verify-integrity")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Dictionary<int, bool>>> VerifyLogsIntegrity(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        fromDate ??= DateTime.UtcNow.AddDays(-30);
        toDate ??= DateTime.UtcNow;
        
        var results = await _auditService.VerifyLogsIntegrityAsync(fromDate.Value, toDate.Value);
        
        var invalidCount = results.Count(r => !r.Value);
        
        _logger.LogInformation("Verified integrity of {TotalCount} audit logs. Invalid logs: {InvalidCount}", 
            results.Count, invalidCount);
            
        // Log the verification action
        await _auditService.LogSystemActionAsync(
            "VERIFY_AUDIT_LOG_INTEGRITY",
            $"Verified integrity of {results.Count} audit logs from {fromDate:d} to {toDate:d}. Found {invalidCount} invalid logs.",
            "AuditLog",
            null);
            
        return Ok(results);
    }
}
