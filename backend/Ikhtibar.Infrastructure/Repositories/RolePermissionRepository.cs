using Dapper;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Ikhtibar.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for RolePermission relationship operations
/// Following SRP: ONLY RolePermission relationship data operations
/// </summary>
public class RolePermissionRepository : BaseRepository<RolePermission>, IRolePermissionRepository
{
    private new readonly ILogger<RolePermissionRepository> _logger;

    public RolePermissionRepository(IDbConnectionFactory connectionFactory, ILogger<RolePermissionRepository> logger)
        : base(connectionFactory, logger, "RolePermissions")
    {
        _logger = logger;
    }

    /// <summary>
    /// Get permissions by role ID
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>Collection of permissions for the role</returns>
    public async Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(int roleId)
    {
        try
        {
            const string sql = @"
                SELECT p.* FROM Permissions p
                INNER JOIN RolePermissions rp ON p.PermissionId = rp.PermissionId
                WHERE rp.RoleId = @RoleId AND p.IsDeleted = 0
                ORDER BY p.Category, p.Name";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Permission>(sql, new { RoleId = roleId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions by role ID: {RoleId}", roleId);
            throw;
        }
    }

    /// <summary>
    /// Get roles by permission ID
    /// </summary>
    /// <param name="permissionId">Permission ID</param>
    /// <returns>Collection of roles that have the permission</returns>
    public async Task<IEnumerable<Role>> GetRolesByPermissionAsync(int permissionId)
    {
        try
        {
            const string sql = @"
                SELECT r.* FROM Roles r
                INNER JOIN RolePermissions rp ON r.RoleId = rp.RoleId
                WHERE rp.PermissionId = @PermissionId AND r.IsDeleted = 0
                ORDER BY r.Name";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Role>(sql, new { PermissionId = permissionId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles by permission ID: {PermissionId}", permissionId);
            throw;
        }
    }

    /// <summary>
    /// Assign permissions to a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionIds">Collection of permission IDs to assign</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds)
    {
        try
        {
            if (!permissionIds.Any())
                return true;

            const string sql = @"
                INSERT INTO RolePermissions (RoleId, PermissionId, AssignedAt, AssignedBy)
                VALUES (@RoleId, @PermissionId, @AssignedAt, @AssignedBy)";

            var rolePermissions = permissionIds.Select(permissionId => new
            {
                RoleId = roleId,
                PermissionId = permissionId,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = 1 // TODO: Get from current user context
            });

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, rolePermissions);
            return rowsAffected == permissionIds.Count();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permissions to role: {RoleId}, Permissions: {PermissionIds}", 
                roleId, string.Join(",", permissionIds));
            throw;
        }
    }

    /// <summary>
    /// Remove permissions from a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionIds">Collection of permission IDs to remove</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> RemovePermissionsAsync(int roleId, IEnumerable<int> permissionIds)
    {
        try
        {
            if (!permissionIds.Any())
                return true;

            const string sql = "DELETE FROM RolePermissions WHERE RoleId = @RoleId AND PermissionId IN @PermissionIds";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new { RoleId = roleId, PermissionIds = permissionIds });
            return rowsAffected == permissionIds.Count();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing permissions from role: {RoleId}, Permissions: {PermissionIds}", 
                roleId, string.Join(",", permissionIds));
            throw;
        }
    }

    /// <summary>
    /// Check if a role has a specific permission
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionId">Permission ID</param>
    /// <returns>True if role has permission, false otherwise</returns>
    public async Task<bool> RoleHasPermissionAsync(int roleId, int permissionId)
    {
        try
        {
            const string sql = "SELECT COUNT(1) FROM RolePermissions WHERE RoleId = @RoleId AND PermissionId = @PermissionId";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { RoleId = roleId, PermissionId = permissionId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if role has permission: {RoleId}, {PermissionId}", roleId, permissionId);
            throw;
        }
    }

    /// <summary>
    /// Get all roles with their permissions
    /// </summary>
    /// <returns>Collection of roles with permission information</returns>
    public async Task<IEnumerable<Role>> GetAllRolesWithPermissionsAsync()
    {
        try
        {
            const string sql = @"
                SELECT DISTINCT r.* FROM Roles r
                INNER JOIN RolePermissions rp ON r.RoleId = rp.RoleId
                WHERE r.IsDeleted = 0
                ORDER BY r.Name";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Role>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles with permissions");
            throw;
        }
    }

    /// <summary>
    /// Get role permissions by multiple role IDs
    /// </summary>
    /// <param name="roleIds">Collection of role IDs</param>
    /// <returns>Dictionary mapping role IDs to their permissions</returns>
    public async Task<Dictionary<int, IEnumerable<Permission>>> GetPermissionsByRolesAsync(IEnumerable<int> roleIds)
    {
        try
        {
            if (!roleIds.Any())
                return new Dictionary<int, IEnumerable<Permission>>();

            const string sql = @"
                SELECT rp.RoleId, p.* FROM Permissions p
                INNER JOIN RolePermissions rp ON p.PermissionId = rp.PermissionId
                WHERE rp.RoleId IN @RoleIds AND p.IsDeleted = 0
                ORDER BY p.Category, p.Name";

            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<dynamic>(sql, new { RoleIds = roleIds });

            var rolePermissions = new Dictionary<int, IEnumerable<Permission>>();
            foreach (var roleId in roleIds)
            {
                var permissions = results
                    .Where(r => r.RoleId == roleId)
                    .Select(r => new Permission
                    {
                        PermissionId = r.PermissionId,
                        Code = r.Code,
                        Name = r.Name,
                        Description = r.Description,
                        Category = r.Category,
                        CreatedAt = r.CreatedAt,
                        CreatedBy = r.CreatedBy,
                        ModifiedAt = r.UpdatedAt,
                        ModifiedBy = r.UpdatedBy,
                        IsDeleted = r.IsDeleted
                    });
                rolePermissions[roleId] = permissions;
            }

            return rolePermissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions by roles: {RoleIds}", string.Join(",", roleIds));
            throw;
        }
    }
}
