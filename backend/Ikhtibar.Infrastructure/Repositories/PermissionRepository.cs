using Dapper;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Ikhtibar.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Permission entity operations
/// Following SRP: ONLY Permission data operations
/// </summary>
public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    private new readonly ILogger<PermissionRepository> _logger;

    public PermissionRepository(IDbConnectionFactory connectionFactory, ILogger<PermissionRepository> logger)
        : base(connectionFactory, logger, "Permissions", "PermissionId")
    {
        _logger = logger;
    }

    /// <summary>
    /// Get permission by code
    /// </summary>
    /// <param name="code">Permission code</param>
    /// <returns>Permission if found, null otherwise</returns>
    public async Task<Permission?> GetByCodeAsync(string code)
    {
        try
        {
            const string sql = "SELECT * FROM Permissions WHERE Code = @Code AND IsDeleted = 0";
            
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Permission>(sql, new { Code = code });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permission by code: {Code}", code);
            throw;
        }
    }

    /// <summary>
    /// Get permissions by category
    /// </summary>
    /// <param name="category">Permission category</param>
    /// <returns>Collection of permissions in the category</returns>
    public async Task<IEnumerable<Permission>> GetByCategoryAsync(string category)
    {
        try
        {
            const string sql = "SELECT * FROM Permissions WHERE Category = @Category AND IsDeleted = 0 ORDER BY Name";
            
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Permission>(sql, new { Category = category });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions by category: {Category}", category);
            throw;
        }
    }

    /// <summary>
    /// Get active permissions only
    /// </summary>
    /// <returns>Collection of active permissions</returns>
    public async Task<IEnumerable<Permission>> GetActivePermissionsAsync()
    {
        try
        {
            const string sql = "SELECT * FROM Permissions WHERE IsActive = 1 AND IsDeleted = 0 ORDER BY Category, Name";
            
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Permission>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active permissions");
            throw;
        }
    }

    /// <summary>
    /// Get system permissions only
    /// </summary>
    /// <returns>Collection of system permissions</returns>
    public async Task<IEnumerable<Permission>> GetSystemPermissionsAsync()
    {
        try
        {
            const string sql = "SELECT * FROM Permissions WHERE Category = 'System' AND IsDeleted = 0 ORDER BY Name";
            
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Permission>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system permissions");
            throw;
        }
    }

    /// <summary>
    /// Check if permission code exists
    /// </summary>
    /// <param name="code">Permission code to check</param>
    /// <param name="excludePermissionId">Permission ID to exclude from check</param>
    /// <returns>True if permission code exists, false otherwise</returns>
    public async Task<bool> CodeExistsAsync(string code, int? excludePermissionId = null)
    {
        try
        {
            string sql = excludePermissionId.HasValue
                ? "SELECT COUNT(1) FROM Permissions WHERE Code = @Code AND PermissionId != @ExcludePermissionId AND IsDeleted = 0"
                : "SELECT COUNT(1) FROM Permissions WHERE Code = @Code AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            object parameters = excludePermissionId.HasValue 
                ? new { Code = code, ExcludePermissionId = excludePermissionId.Value }
                : (object)new { Code = code };
            var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if permission code exists: {Code}", code);
            throw;
        }
    }

    /// <summary>
    /// Get permissions by multiple IDs
    /// </summary>
    /// <param name="permissionIds">Collection of permission IDs</param>
    /// <returns>Collection of permissions</returns>
    public async Task<IEnumerable<Permission>> GetByIdsAsync(IEnumerable<int> permissionIds)
    {
        try
        {
            if (!permissionIds.Any())
                return Enumerable.Empty<Permission>();

            const string sql = "SELECT * FROM Permissions WHERE PermissionId IN @PermissionIds AND IsDeleted = 0 ORDER BY Category, Name";
            
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Permission>(sql, new { PermissionIds = permissionIds });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions by IDs: {PermissionIds}", string.Join(",", permissionIds));
            throw;
        }
    }
}
