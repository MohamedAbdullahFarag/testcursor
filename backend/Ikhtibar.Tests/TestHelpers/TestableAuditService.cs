using System.Text.Json;
using System.Linq;
using Ikhtibar.Core.Services;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Ikhtibar.Tests.TestHelpers;

/// <summary>
/// Minimal test-friendly implementation of IAuditService used to avoid EPPlus usage in production AuditService constructor.
/// Mirrors the behaviour needed by unit tests in the test project.
/// </summary>
public class TestableAuditService : IAuditService
{
    private readonly IAuditLogRepository? _auditLogRepository;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly IMapper? _mapper;
    private readonly ILogger? _logger;

    // In-memory fallback used when a real repository is not provided by the test
    private readonly List<AuditLog> _inMemoryLogs = new();

    public TestableAuditService(
        IAuditLogRepository? auditLogRepository = null,
        IHttpContextAccessor? httpContextAccessor = null,
        IMapper? mapper = null,
        ILogger? logger = null)
    {
        _auditLogRepository = auditLogRepository;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> LogAsync(AuditLogEntry entry)
    {
        var httpContext = _httpContextAccessor?.HttpContext;

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
            UserAgent = GetUserAgentSafely(httpContext),
            SessionId = GetSessionIdSafely(httpContext),
            Severity = entry.Severity,
            Category = entry.Category,
            Timestamp = DateTime.UtcNow,
            IsSystemAction = entry.IsSystemAction
        };

        if (_auditLogRepository != null)
        {
            var created = await _auditLogRepository.AddAsync(auditLog);
            return created.AuditLogId;
        }

        // fallback: add to in-memory store
        auditLog.AuditLogId = _inMemoryLogs.Count + 1;
        _inMemoryLogs.Add(auditLog);
        return auditLog.AuditLogId;
    }

    public Task<int> LogUserActionAsync(int? userId, string action, string entityType, string entityId, object? oldValues = null, object? newValues = null)
    {
        var entry = new AuditLogEntry
        {
            UserId = userId,
            UserIdentifier = userId.HasValue ? userId.Value.ToString() : "Anonymous",
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            Severity = DetermineSeverity(action),
            Category = DetermineCategory(action, entityType),
            IsSystemAction = false
        };

        return LogAsync(entry);
    }

    public Task<int> LogSecurityEventAsync(string userIdentifier, string action, string details, AuditSeverity severity = AuditSeverity.High)
    {
        var entry = new AuditLogEntry
        {
            UserId = null,
            UserIdentifier = userIdentifier,
            Action = action,
            EntityType = "Security",
            Details = details,
            Severity = severity,
            Category = AuditCategory.Security,
            IsSystemAction = false
        };

        return LogAsync(entry);
    }

    public Task<int> LogSystemActionAsync(string action, string details, string entityType = "System", string? entityId = null)
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

        return LogAsync(entry);
    }

    public async Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(AuditLogFilter filter)
    {
        // defensive: tests may pass null or filters with missing paging values
        if (filter == null) filter = new AuditLogFilter { Page = 1, PageSize = int.MaxValue };
        if (filter.Page <= 0) filter.Page = 1;
        if (filter.PageSize <= 0) filter.PageSize = int.MaxValue;
        if (_auditLogRepository != null)
        {
            var result = await _auditLogRepository.GetAuditLogsAsync(filter);
            if (_mapper != null)
            {
                return new PagedResult<AuditLogDto>
                {
                    Items = _mapper.Map<List<AuditLogDto>>(result.Items),
                    TotalCount = result.TotalCount,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize
                };
            }

            // Map manually if mapper is not provided
            return new PagedResult<AuditLogDto>
            {
                Items = result.Items.Select(x => new AuditLogDto
                {
                    AuditLogId = x.AuditLogId,
                    UserId = x.UserId,
                    UserIdentifier = x.UserIdentifier,
                    Action = x.Action,
                    EntityType = x.EntityType,
                    EntityId = x.EntityId,
                    Details = x.Details,
                    OldValues = x.OldValues,
                    NewValues = x.NewValues,
                    IpAddress = x.IpAddress,
                    UserAgent = x.UserAgent,
                    SessionId = x.SessionId,
                    Severity = x.Severity,
                    Category = x.Category,
                    Timestamp = x.Timestamp,
                    IsSystemAction = x.IsSystemAction
                }).ToList(),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

    // Fallback: use in-memory logs and apply simple filtering/paging
        var query = _inMemoryLogs.AsQueryable();
        if (filter.FromDate.HasValue) query = query.Where(x => x.Timestamp >= filter.FromDate.Value);
        if (filter.ToDate.HasValue) query = query.Where(x => x.Timestamp <= filter.ToDate.Value);
        if (filter.UserId.HasValue) query = query.Where(x => x.UserId == filter.UserId.Value);

        var total = query.Count();
        var items = query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize)
            .Select(x => new AuditLogDto
            {
                AuditLogId = x.AuditLogId,
                UserId = x.UserId,
                UserIdentifier = x.UserIdentifier,
                Action = x.Action,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                Details = x.Details,
                OldValues = x.OldValues,
                NewValues = x.NewValues,
                IpAddress = x.IpAddress,
                UserAgent = x.UserAgent,
                SessionId = x.SessionId,
                Severity = x.Severity,
                Category = x.Category,
                Timestamp = x.Timestamp,
                IsSystemAction = x.IsSystemAction
            }).ToList();

        return new PagedResult<AuditLogDto>
        {
            Items = items,
            TotalCount = total,
            PageNumber = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public Task<PagedResult<AuditLogDto>> GetUserAuditLogsAsync(int userId, AuditLogFilter filter)
    {
        // If repository supports optimized user queries, use it to respect test setups
        if (_auditLogRepository != null)
        {
            // production repository exposes GetUserAuditLogsAsync(userId, fromDate, toDate)
            var from = filter?.FromDate ?? DateTime.MinValue;
            var to = filter?.ToDate ?? DateTime.MaxValue;
            // call repository directly and map
            return Task.Run(async () =>
            {
                var items = await _auditLogRepository.GetUserAuditLogsAsync(userId, from, to);
                if (_mapper != null)
                {
                    var mapped = _mapper.Map<List<AuditLogDto>>(items.ToList());
                    return new PagedResult<AuditLogDto>
                    {
                        Items = mapped,
                        TotalCount = mapped.Count,
                        PageNumber = filter?.Page ?? 1,
                        PageSize = filter?.PageSize ?? mapped.Count
                    };
                }

                var manual = items.Select(x => new AuditLogDto
                {
                    AuditLogId = x.AuditLogId,
                    UserId = x.UserId,
                    UserIdentifier = x.UserIdentifier,
                    Action = x.Action,
                    EntityType = x.EntityType,
                    EntityId = x.EntityId,
                    Details = x.Details,
                    OldValues = x.OldValues,
                    NewValues = x.NewValues,
                    IpAddress = x.IpAddress,
                    UserAgent = x.UserAgent,
                    SessionId = x.SessionId,
                    Severity = x.Severity,
                    Category = x.Category,
                    Timestamp = x.Timestamp,
                    IsSystemAction = x.IsSystemAction
                }).ToList();

                return new PagedResult<AuditLogDto>
                {
                    Items = manual,
                    TotalCount = manual.Count,
                    PageNumber = filter?.Page ?? 1,
                    PageSize = filter?.PageSize ?? manual.Count
                };
            });
        }

        filter.UserId = userId;
        return GetAuditLogsAsync(filter);
    }

    public async Task<IEnumerable<AuditLogDto>> GetSecurityEventsAsync(DateTime fromDate, DateTime toDate)
    {
        if (_auditLogRepository != null)
        {
            var securityEvents = await _auditLogRepository.GetSecurityEventsAsync(fromDate, toDate);
            if (_mapper != null) return _mapper.Map<IEnumerable<AuditLogDto>>(securityEvents);

            return securityEvents.Select(x => new AuditLogDto
            {
                AuditLogId = x.AuditLogId,
                UserId = x.UserId,
                UserIdentifier = x.UserIdentifier,
                Action = x.Action,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                Details = x.Details,
                OldValues = x.OldValues,
                NewValues = x.NewValues,
                IpAddress = x.IpAddress,
                UserAgent = x.UserAgent,
                SessionId = x.SessionId,
                Severity = x.Severity,
                Category = x.Category,
                Timestamp = x.Timestamp,
                IsSystemAction = x.IsSystemAction
            });
        }

        return _inMemoryLogs.Where(x => x.Category == AuditCategory.Security && x.Timestamp >= fromDate && x.Timestamp <= toDate)
            .Select(x => new AuditLogDto
            {
                AuditLogId = x.AuditLogId,
                UserId = x.UserId,
                UserIdentifier = x.UserIdentifier,
                Action = x.Action,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                Details = x.Details,
                OldValues = x.OldValues,
                NewValues = x.NewValues,
                IpAddress = x.IpAddress,
                UserAgent = x.UserAgent,
                SessionId = x.SessionId,
                Severity = x.Severity,
                Category = x.Category,
                Timestamp = x.Timestamp,
                IsSystemAction = x.IsSystemAction
            });
    }

    public Task<byte[]> ExportAuditLogsAsync(AuditLogFilter filter, AuditLogExportFormat format)
    {
        // Tests don't exercise actual file exports; return an empty byte array.
        return Task.FromResult(Array.Empty<byte>());
    }

    public Task<int> ArchiveOldLogsAsync(TimeSpan retentionPeriod)
    {
        var cutoffDate = DateTime.UtcNow.Subtract(retentionPeriod);
        if (_auditLogRepository != null)
        {
            return _auditLogRepository.ArchiveLogsAsync(cutoffDate);
        }

        // Fallback: remove from in-memory logs and return removed count
        var before = _inMemoryLogs.Count;
        _inMemoryLogs.RemoveAll(x => x.Timestamp < cutoffDate);
        var removed = before - _inMemoryLogs.Count;
        return Task.FromResult(removed);
    }

    public async Task<Dictionary<int, bool>> VerifyLogsIntegrityAsync(DateTime fromDate, DateTime toDate)
    {
        var filter = new AuditLogFilter { FromDate = fromDate, ToDate = toDate, Page = 1, PageSize = 1000 };
        var logs = _auditLogRepository != null ? await _auditLogRepository.GetAuditLogsAsync(filter) : new PagedResult<AuditLog> { Items = _inMemoryLogs, PageNumber = 1, PageSize = _inMemoryLogs.Count, TotalCount = _inMemoryLogs.Count };
        var results = new Dictionary<int, bool>();

        foreach (var log in logs.Items)
        {
            var isValid = _auditLogRepository != null ? await _auditLogRepository.VerifyLogIntegrityAsync(log.AuditLogId) : true;
            results.Add(log.AuditLogId, isValid);
        }

        return results;
    }

    #region Helpers
    private string? GetClientIpAddress(HttpContext? httpContext)
    {
        if (httpContext == null) return null;
        try
        {
            var headers = httpContext.Request?.Headers;
            if (headers != null && headers.TryGetValue("X-Forwarded-For", out var forwarded))
            {
                var forwardedHeader = forwarded.FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedHeader)) return forwardedHeader.Split(',')[0].Trim();
            }

            return httpContext.Connection?.RemoteIpAddress?.ToString();
        }
        catch
        {
            return null;
        }
    }

    private string? GetSessionIdSafely(HttpContext? httpContext)
    {
        if (httpContext == null) return null;
        try { return httpContext.Session?.Id; } catch (InvalidOperationException) { return null; }
    }

    private string? GetUserAgentSafely(HttpContext? httpContext)
    {
        if (httpContext == null) return null;
        try
        {
            // use null-conditional access to avoid NullReferenceException
            return httpContext.Request?.Headers?["User-Agent"].ToString();
        }
        catch
        {
            return null;
        }
    }

    private AuditSeverity DetermineSeverity(string action)
    {
        if (action.StartsWith("DELETE_") || action.Contains("ADMIN") || action.Contains("PERMISSION") || action.Contains("ROLE") || action.Contains("PASSWORD") || action.Contains("CREDENTIAL") || action.Contains("CONFIG") || action.Contains("SECURITY")) return AuditSeverity.Critical;
        if (action.StartsWith("CREATE_") || action.StartsWith("UPDATE_") || action.StartsWith("MODIFY_") || action.StartsWith("IMPORT_") || action.StartsWith("EXPORT_")) return AuditSeverity.High;
        if (action.Contains("STATUS") || action.Contains("STATE") || action.Contains("LOGIN") || action.Contains("LOGOUT")) return AuditSeverity.Medium;
        return AuditSeverity.Low;
    }

    private AuditCategory DetermineCategory(string action, string entityType)
    {
        if (action.Contains("LOGIN") || action.Contains("LOGOUT") || action.Contains("PASSWORD") || action.Contains("CREDENTIAL") || action.Contains("MFA") || entityType == "Authentication") return AuditCategory.Authentication;
        if (action.Contains("PERMISSION") || action.Contains("ROLE") || action.Contains("ACCESS") || entityType == "Authorization" || entityType == "Permission" || entityType == "Role") return AuditCategory.Authorization;
        if (entityType == "User" || entityType == "Profile" || action.Contains("USER")) return AuditCategory.UserManagement;
        if (action.StartsWith("CREATE_") || action.StartsWith("UPDATE_") || action.StartsWith("DELETE_") || action.StartsWith("READ_")) return AuditCategory.DataAccess;
        if (entityType == "System" || action.Contains("CONFIG") || action.Contains("SETTING") || action.Contains("SYSTEM")) return AuditCategory.System;
        if (action.Contains("SECURITY") || action.Contains("BREACH") || action.Contains("VIOLATION")) return AuditCategory.Security;
        return AuditCategory.System;
    }
    #endregion
}
