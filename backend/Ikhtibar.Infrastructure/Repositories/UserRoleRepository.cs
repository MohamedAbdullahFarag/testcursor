using Dapper;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for UserRole junction entity operations using Dapper
/// Following SRP: ONLY user-role relationship operations
/// </summary>
public class UserRoleRepository : IUserRoleRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRoleRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc />
    public async Task AssignRoleAsync(int userId, int roleId)
    {
        const string sql = @"
            INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
            VALUES (@userId, @roleId, @assignedAt)
            ON CONFLICT(UserId, RoleId) DO NOTHING";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { userId, roleId, assignedAt = DateTime.UtcNow });
    }

    /// <inheritdoc />
    public async Task RemoveRoleAsync(int userId, int roleId)
    {
        const string sql = @"
            DELETE FROM UserRoles 
            WHERE UserId = @userId AND RoleId = @roleId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { userId, roleId });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
    {
        const string sql = @"
            SELECT r.RoleId, r.Code, r.Name, r.Description, r.IsSystemRole, r.CreatedAt, r.ModifiedAt, r.IsDeleted
            FROM Roles r
            INNER JOIN UserRoles ur ON r.RoleId = ur.RoleId
            WHERE ur.UserId = @userId AND r.IsDeleted = 0
            ORDER BY r.Name";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Role>(sql, new { userId });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetRoleUsersAsync(int roleId)
    {
        const string sql = @"
            SELECT u.UserId, u.Username, u.Email, u.FirstName, u.LastName, u.PhoneNumber, 
                   u.PreferredLanguage, u.IsActive, u.EmailVerified, u.PhoneVerified, 
                   u.CreatedAt, u.ModifiedAt, u.IsDeleted
            FROM Users u
            INNER JOIN UserRoles ur ON u.UserId = ur.UserId
            WHERE ur.RoleId = @roleId AND u.IsDeleted = 0 AND u.IsActive = 1
            ORDER BY u.FirstName, u.LastName";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<User>(sql, new { roleId });
    }

    /// <inheritdoc />
    public async Task<bool> UserHasRoleAsync(int userId, int roleId)
    {
        const string sql = @"
            SELECT COUNT(1) 
            FROM UserRoles ur
            INNER JOIN Roles r ON ur.RoleId = r.RoleId
            WHERE ur.UserId = @userId AND ur.RoleId = @roleId AND r.IsDeleted = 0";

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.QueryFirstOrDefaultAsync<int>(sql, new { userId, roleId });
        return count > 0;
    }

    /// <inheritdoc />
    public async Task<bool> UserHasRoleAsync(int userId, string roleCode)
    {
        const string sql = @"
            SELECT COUNT(1) 
            FROM UserRoles ur
            INNER JOIN Roles r ON ur.RoleId = r.RoleId
            WHERE ur.UserId = @userId AND r.Code = @roleCode AND r.IsDeleted = 0";

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.QueryFirstOrDefaultAsync<int>(sql, new { userId, roleCode });
        return count > 0;
    }

    /// <inheritdoc />
    public async Task RemoveAllUserRolesAsync(int userId)
    {
        const string sql = @"
            DELETE FROM UserRoles 
            WHERE UserId = @userId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { userId });
    }

    /// <inheritdoc />
    public async Task<UserRole?> GetUserRoleAsync(int userId, int roleId)
    {
        const string sql = @"
            SELECT UserId, RoleId, AssignedAt
            FROM UserRoles 
            WHERE UserId = @userId AND RoleId = @roleId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserRole>(sql, new { userId, roleId });
    }
}
