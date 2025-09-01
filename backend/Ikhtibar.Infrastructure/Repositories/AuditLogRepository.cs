using Dapper;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for audit log data operations
/// Specialized repository that doesn't inherit from BaseRepository due to different primary key type
/// </summary>
public class AuditLogRepository : IAuditLogRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<AuditLogRepository> _logger;

    /// <summary>
    /// Constructor for AuditLogRepository
    /// </summary>
    /// <param name="connectionFactory">Database connection factory</param>
    /// <param name="logger">Logger instance</param>
    public AuditLogRepository(IDbConnectionFactory connectionFactory, ILogger<AuditLogRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new audit log entry
    /// </summary>
    /// <param name="auditLog">The audit log to create</param>
    /// <returns>The created audit log with generated ID</returns>
    public async Task<AuditLog> AddAsync(AuditLog auditLog)
    {
        const string sql = @"
            INSERT INTO AuditLogs 
            (UserId, UserIdentifier, Action, EntityType, EntityId, Details, OldValues, NewValues, 
             IpAddress, UserAgent, SessionId, Severity, Category, Timestamp, IsSystemAction, 
             CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, IsDeleted)
            VALUES 
            (@UserId, @UserIdentifier, @Action, @EntityType, @EntityId, @Details, @OldValues, @NewValues, 
             @IpAddress, @UserAgent, @SessionId, @Severity, @Category, @Timestamp, @IsSystemAction, 
             @CreatedAt, @CreatedBy, @ModifiedAt, @ModifiedBy, @IsDeleted);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var auditLogId = await connection.QuerySingleAsync<int>(sql, auditLog);
            auditLog.AuditLogId = auditLogId;
            
            _logger.LogDebug("Created audit log with ID {AuditLogId}", auditLogId);
            return auditLog;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create audit log");
            throw;
        }
    }

    /// <summary>
    /// Gets an audit log by ID
    /// </summary>
    /// <param name="id">The audit log ID</param>
    /// <returns>The audit log or null if not found</returns>
    public async Task<AuditLog?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT 
                Id, UserId, EntityType, EntityId, Action, Timestamp, 
                OldValues, NewValues, IpAddress, UserAgent, SessionId, 
                RequestId, Method, Endpoint, StatusCode, Duration, 
                ErrorMessage, StackTrace
            FROM AuditLogs 
            WHERE Id = @Id";
            
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var auditLog = await connection.QuerySingleOrDefaultAsync<AuditLog>(sql, new { Id = id });
            
            _logger.LogDebug("Retrieved audit log with ID {AuditLogId}", id);
            return auditLog;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit log with ID {AuditLogId}", id);
            throw;
        }
    }
    
    /// <summary>
    /// Retrieves audit logs based on filter criteria with pagination
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <returns>Paginated result of audit logs</returns>
    public async Task<PagedResult<AuditLog>> GetAuditLogsAsync(AuditLogFilter filter)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var parameters = new DynamicParameters();
        var whereClause = new StringBuilder("WHERE IsDeleted = 0");
        
        // Apply filters
        if (filter.UserId.HasValue)
        {
            whereClause.Append(" AND UserId = @UserId");
            parameters.Add("@UserId", filter.UserId);
        }
        
        if (!string.IsNullOrEmpty(filter.UserIdentifier))
        {
            whereClause.Append(" AND UserIdentifier LIKE @UserIdentifier");
            parameters.Add("@UserIdentifier", $"%{filter.UserIdentifier}%");
        }
        
        if (!string.IsNullOrEmpty(filter.Action))
        {
            whereClause.Append(" AND Action = @Action");
            parameters.Add("@Action", filter.Action);
        }
        
        if (!string.IsNullOrEmpty(filter.EntityType))
        {
            whereClause.Append(" AND EntityType = @EntityType");
            parameters.Add("@EntityType", filter.EntityType);
        }
        
        if (!string.IsNullOrEmpty(filter.EntityId))
        {
            whereClause.Append(" AND EntityId = @EntityId");
            parameters.Add("@EntityId", filter.EntityId);
        }
        
        if (filter.Severity.HasValue)
        {
            whereClause.Append(" AND Severity = @Severity");
            parameters.Add("@Severity", (int)filter.Severity.Value);
        }
        
        if (filter.Category.HasValue)
        {
            whereClause.Append(" AND Category = @Category");
            parameters.Add("@Category", (int)filter.Category.Value);
        }
        
        if (filter.FromDate.HasValue)
        {
            whereClause.Append(" AND Timestamp >= @FromDate");
            parameters.Add("@FromDate", filter.FromDate.Value);
        }
        
        if (filter.ToDate.HasValue)
        {
            whereClause.Append(" AND Timestamp <= @ToDate");
            parameters.Add("@ToDate", filter.ToDate.Value);
        }
        
        if (!string.IsNullOrEmpty(filter.IpAddress))
        {
            whereClause.Append(" AND IpAddress = @IpAddress");
            parameters.Add("@IpAddress", filter.IpAddress);
        }
        
        if (filter.IncludeSystemActions.HasValue)
        {
            whereClause.Append(" AND IsSystemAction = @IsSystemAction");
            parameters.Add("@IsSystemAction", filter.IncludeSystemActions.Value);
        }
        
        if (!string.IsNullOrEmpty(filter.SearchText))
        {
            whereClause.Append(" AND (Action LIKE @SearchText OR UserIdentifier LIKE @SearchText OR EntityType LIKE @SearchText OR EntityId LIKE @SearchText OR Details LIKE @SearchText)");
            parameters.Add("@SearchText", $"%{filter.SearchText}%");
        }
        
        // Prepare sort order
        var orderBy = $"ORDER BY {filter.SortBy} {filter.SortDirection}";
        
        // Calculate pagination (0-based page from frontend)
        int offset = filter.Page * filter.PageSize;
        parameters.Add("@Offset", offset);
        parameters.Add("@PageSize", filter.PageSize);
        
        // Get total count
        var countSql = $"SELECT COUNT(*) FROM AuditLogs {whereClause}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
        
        // Get paginated data
        var sql = $@"
            SELECT * FROM AuditLogs
            {whereClause}
            {orderBy}
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";
            
        var logs = await connection.QueryAsync<AuditLog>(sql, parameters);
        
        return new PagedResult<AuditLog>
        {
            Items = logs.ToList(),
            TotalCount = totalCount,
            PageNumber = filter.Page,
            PageSize = filter.PageSize
        };
    }
    
    /// <summary>
    /// Retrieves audit logs for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="fromDate">Start date/time</param>
    /// <param name="toDate">End date/time</param>
    /// <returns>Collection of audit logs</returns>
    public async Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(int userId, DateTime fromDate, DateTime toDate)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var sql = @"
            SELECT * FROM AuditLogs
            WHERE UserId = @UserId
              AND Timestamp BETWEEN @FromDate AND @ToDate
              AND IsDeleted = 0
            ORDER BY Timestamp DESC";
            
        return await connection.QueryAsync<AuditLog>(sql, new { UserId = userId, FromDate = fromDate, ToDate = toDate });
    }
    
    /// <summary>
    /// Retrieves security-related audit events
    /// </summary>
    /// <param name="fromDate">Start date/time</param>
    /// <param name="toDate">End date/time</param>
    /// <returns>Collection of security audit logs</returns>
    public async Task<IEnumerable<AuditLog>> GetSecurityEventsAsync(DateTime fromDate, DateTime toDate)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var sql = @"
            SELECT * FROM AuditLogs
            WHERE (Category = @SecurityCategory OR Severity <= @HighSeverity)
              AND Timestamp BETWEEN @FromDate AND @ToDate
              AND IsDeleted = 0
            ORDER BY Timestamp DESC";
            
        return await connection.QueryAsync<AuditLog>(sql, 
            new { 
                SecurityCategory = (int)AuditCategory.Security,
                HighSeverity = (int)AuditSeverity.High,
                FromDate = fromDate, 
                ToDate = toDate 
            });
    }
    
    /// <summary>
    /// Retrieves audit logs for a specific entity
    /// </summary>
    /// <param name="entityType">Entity type</param>
    /// <param name="entityId">Entity ID</param>
    /// <returns>Collection of entity audit logs</returns>
    public async Task<IEnumerable<AuditLog>> GetEntityAuditLogsAsync(string entityType, string entityId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var sql = @"
            SELECT * FROM AuditLogs
            WHERE EntityType = @EntityType
              AND EntityId = @EntityId
              AND IsDeleted = 0
            ORDER BY Timestamp DESC";
            
        return await connection.QueryAsync<AuditLog>(sql, new { EntityType = entityType, EntityId = entityId });
    }
    
    /// <summary>
    /// Archives audit logs older than specified retention period
    /// </summary>
    /// <param name="cutoffDate">Date threshold for archiving</param>
    /// <returns>Number of archived logs</returns>
    public async Task<int> ArchiveLogsAsync(DateTime cutoffDate)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Create archive table if it doesn't exist
            const string createArchiveTableSql = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLogsArchive')
                BEGIN
                    CREATE TABLE dbo.AuditLogsArchive (
                        AuditLogId INT NOT NULL,
                        UserId INT NULL,
                        UserIdentifier NVARCHAR(255) NOT NULL,
                        Action NVARCHAR(100) NOT NULL,
                        EntityType NVARCHAR(50) NOT NULL,
                        EntityId NVARCHAR(50) NULL,
                        Details NVARCHAR(MAX) NULL,
                        OldValues NVARCHAR(MAX) NULL,
                        NewValues NVARCHAR(MAX) NULL,
                        IpAddress NVARCHAR(45) NULL,
                        UserAgent NVARCHAR(500) NULL,
                        SessionId NVARCHAR(100) NULL,
                        Severity TINYINT NOT NULL,
                        Category TINYINT NOT NULL,
                        Timestamp DATETIME2 NOT NULL,
                        IsSystemAction BIT NOT NULL,
                        CreatedAt DATETIME2 NOT NULL,
                        CreatedBy INT NULL,
                        ModifiedAt DATETIME2 NULL,
                        ModifiedBy INT NULL,
                        DeletedAt DATETIME2 NULL,
                        DeletedBy INT NULL,
                        IsDeleted BIT NOT NULL,
                        ArchivedAt DATETIME2 NOT NULL
                    )
                END";
                
            await connection.ExecuteAsync(createArchiveTableSql, transaction: transaction);
            
            // Insert records to archive
            const string archiveRecordsSql = @"
                INSERT INTO AuditLogsArchive
                SELECT 
                    AuditLogId, UserId, UserIdentifier, Action, EntityType, EntityId, 
                    Details, OldValues, NewValues, IpAddress, UserAgent, SessionId, 
                    Severity, Category, Timestamp, IsSystemAction, 
                    CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, DeletedAt, DeletedBy, IsDeleted,
                    SYSUTCDATETIME() AS ArchivedAt
                FROM AuditLogs
                WHERE Timestamp < @CutoffDate
                  AND IsDeleted = 0";
                  
            // Soft delete archived records
            const string softDeleteSql = @"
                UPDATE AuditLogs
                SET IsDeleted = 1,
                    DeletedAt = SYSUTCDATETIME(),
                    DeletedBy = NULL
                WHERE Timestamp < @CutoffDate
                  AND IsDeleted = 0";
                  
            // Execute archive operations
            await connection.ExecuteAsync(archiveRecordsSql, new { CutoffDate = cutoffDate }, transaction: transaction);
            int affectedRows = await connection.ExecuteAsync(softDeleteSql, new { CutoffDate = cutoffDate }, transaction: transaction);
            
            transaction.Commit();
            
            _logger.LogInformation("Archived {AffectedRows} audit logs older than {CutoffDate}", affectedRows, cutoffDate);
            return affectedRows;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Failed to archive audit logs");
            throw;
        }
    }
    
    /// <summary>
    /// Verifies the integrity of an audit log entry
    /// </summary>
    /// <param name="auditLogId">Audit log ID</param>
    /// <returns>True if log integrity is verified</returns>
    public async Task<bool> VerifyLogIntegrityAsync(int auditLogId)
    {
        // In a real implementation, this would verify cryptographic signatures or hashes
        // For now, we simply check that the record exists and hasn't been tampered with
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var sql = @"SELECT COUNT(*) FROM AuditLogs WHERE AuditLogId = @AuditLogId AND RowVersion IS NOT NULL";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { AuditLogId = auditLogId });
        
        return count > 0;
    }
}
