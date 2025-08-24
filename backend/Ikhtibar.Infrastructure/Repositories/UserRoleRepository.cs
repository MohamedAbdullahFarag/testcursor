using Dapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for UserRole entity operations
/// Following SRP: ONLY UserRole data operations
/// </summary>
public class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository
{
    private new readonly ILogger<UserRoleRepository> _logger;

    public UserRoleRepository(IDbConnectionFactory connectionFactory, ILogger<UserRoleRepository> logger)
        : base(connectionFactory, logger, "UserRoles", "UserId")
    {
        _logger = logger;
    }

    /// <summary>
    /// Assign a role to a user
    /// </summary>
    public async Task AssignRoleAsync(int userId, int roleId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            // Check if assignment already exists
            const string checkSql = @"
                SELECT COUNT(1) FROM UserRoles 
                WHERE UserId = @UserId AND RoleId = @RoleId";
            
            var exists = await connection.ExecuteScalarAsync<int>(checkSql, new { UserId = userId, RoleId = roleId });
            
            if (exists == 0)
            {
                const string insertSql = @"
                    INSERT INTO UserRoles (UserId, RoleId, AssignedAt, AssignedBy)
                    VALUES (@UserId, @RoleId, @AssignedAt, @AssignedBy)";
                
                await connection.ExecuteAsync(insertSql, new 
                { 
                    UserId = userId, 
                    RoleId = roleId, 
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = 1 // TODO: Get from current user context
                });
                _logger.LogInformation("Role {RoleId} assigned to user {UserId}", roleId, userId);
            }
            else
            {
                _logger.LogInformation("Role {RoleId} already assigned to user {UserId}", roleId, userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
            throw;
        }
    }

    /// <summary>
    /// Remove a role from a user
    /// </summary>
    public async Task RemoveRoleAsync(int userId, int roleId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                DELETE FROM UserRoles 
                WHERE UserId = @UserId AND RoleId = @RoleId";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                UserId = userId, 
                RoleId = roleId
            });
            
            if (rowsAffected > 0)
            {
                _logger.LogInformation("Role {RoleId} removed from user {UserId}", roleId, userId);
            }
            else
            {
                _logger.LogWarning("Role {RoleId} not found for user {UserId}", roleId, userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            throw;
        }
    }

    /// <summary>
    /// Get all roles for a specific user
    /// </summary>
    public async Task<IEnumerable<UserRole>> GetUserRolesAsync(int userId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT UserId, RoleId, AssignedAt, AssignedBy
                FROM UserRoles 
                WHERE UserId = @UserId
                ORDER BY AssignedAt";
            
            var userRoles = await connection.QueryAsync<UserRole>(sql, new { UserId = userId });
            return userRoles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get all users for a specific role
    /// </summary>
    public async Task<IEnumerable<UserRole>> GetRoleUsersAsync(int roleId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT UserId, RoleId, AssignedAt, AssignedBy
                FROM UserRoles 
                WHERE RoleId = @RoleId
                ORDER BY AssignedAt";
            
            var userRoles = await connection.QueryAsync<UserRole>(sql, new { RoleId = roleId });
            return userRoles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users for role {RoleId}", roleId);
            throw;
        }
    }

    /// <summary>
    /// Check if a user has a specific role
    /// </summary>
    public async Task<bool> UserHasRoleAsync(int userId, int roleId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT COUNT(1) FROM UserRoles 
                WHERE UserId = @UserId AND RoleId = @RoleId";
            
            var count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, RoleId = roleId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} has role {RoleId}", userId, roleId);
            throw;
        }
    }

    /// <summary>
    /// Remove all roles from a user
    /// </summary>
    public async Task RemoveAllUserRolesAsync(int userId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                DELETE FROM UserRoles 
                WHERE UserId = @UserId";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new { UserId = userId });
            
            _logger.LogInformation("Removed {Count} roles from user {UserId}", rowsAffected, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing all roles from user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get user role by user ID and role ID
    /// </summary>
    public async Task<UserRole?> GetUserRoleAsync(int userId, int roleId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT UserId, RoleId, AssignedAt, AssignedBy
                FROM UserRoles 
                WHERE UserId = @UserId AND RoleId = @RoleId";
            
            var userRole = await connection.QueryFirstOrDefaultAsync<UserRole>(sql, new { UserId = userId, RoleId = roleId });
            return userRole;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user role for user {UserId} and role {RoleId}", userId, roleId);
            throw;
        }
    }

    /// <summary>
    /// Get user role by user ID and role ID
    /// Note: UserRole is a junction table, so GetByIdAsync is not applicable
    /// </summary>
    public override async Task<UserRole?> GetByIdAsync(int id)
    {
        throw new NotSupportedException("GetByIdAsync is not supported for UserRole entity. Use GetUserRoleAsync(userId, roleId) instead.");
    }

    /// <summary>
    /// Get all user roles
    /// </summary>
    public override async Task<IEnumerable<UserRole>> GetAllAsync(string? where = null, object? parameters = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            // Build base SQL
            var sql = "SELECT UserId, RoleId, AssignedAt, AssignedBy FROM UserRoles";
            if (!string.IsNullOrEmpty(where))
            {
                sql += $" WHERE {where}";
            }
            sql += " ORDER BY UserId, RoleId";
            
            var userRoles = await connection.QueryAsync<UserRole>(sql, parameters);
            return userRoles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all user roles");
            throw;
        }
    }
}
