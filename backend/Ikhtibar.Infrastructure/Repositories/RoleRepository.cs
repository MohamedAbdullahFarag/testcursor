using System.Data;
using Dapper;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Role entity operations using Dapper
/// Following SRP: ONLY role-specific data access operations
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RoleRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc />
    public async Task<Role?> GetByCodeAsync(string code)
    {
        const string sql = @"
            SELECT RoleId, Code, Name, Description, IsSystemRole, CreatedAt, ModifiedAt, IsDeleted
            FROM Roles 
            WHERE Code = @code AND IsDeleted = 0";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { code });
    }

    /// <inheritdoc />
    public async Task<bool> RoleExistsAsync(int id)
    {
        const string sql = @"
            SELECT COUNT(1) 
            FROM Roles 
            WHERE RoleId = @id AND IsDeleted = 0";

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.QueryFirstOrDefaultAsync<int>(sql, new { id });
        return count > 0;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> GetSystemRolesAsync()
    {
        const string sql = @"
            SELECT RoleId, Code, Name, Description, IsSystemRole, CreatedAt, ModifiedAt, IsDeleted
            FROM Roles 
            WHERE IsSystemRole = 1 AND IsDeleted = 0
            ORDER BY Name";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Role>(sql);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> GetCustomRolesAsync()
    {
        const string sql = @"
            SELECT RoleId, Code, Name, Description, IsSystemRole, CreatedAt, ModifiedAt, IsDeleted
            FROM Roles 
            WHERE IsSystemRole = 0 AND IsDeleted = 0
            ORDER BY Name";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Role>(sql);
    }

    // IRepository<Role> implementation

    /// <inheritdoc />
    public async Task<Role?> GetByIdAsync(Guid id)
    {
        // Note: Role entity uses int RoleId, not Guid
        // This method is here for interface compliance but should use GetByIdAsync(int)
        await Task.CompletedTask;
        throw new NotSupportedException("Use GetByIdAsync(int) for Role entity");
    }

    /// <summary>
    /// Gets a role by its integer ID
    /// </summary>
    public async Task<Role?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT RoleId, Code, Name, Description, IsSystemRole, CreatedAt, ModifiedAt, IsDeleted
            FROM Roles 
            WHERE RoleId = @id AND IsDeleted = 0";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { id });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> GetAllAsync(string? where = null, object? parameters = null)
    {
        var sql = @"
            SELECT RoleId, Code, Name, Description, IsSystemRole, CreatedAt, ModifiedAt, IsDeleted
            FROM Roles 
            WHERE IsDeleted = 0";

        if (!string.IsNullOrEmpty(where))
        {
            sql += $" AND {where}";
        }

        sql += " ORDER BY Name";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Role>(sql, parameters);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> GetPagedAsync(int offset, int limit, string? where = null, string? orderBy = null, object? parameters = null)
    {
        var sql = @"
            SELECT RoleId, Code, Name, Description, IsSystemRole, CreatedAt, ModifiedAt, IsDeleted
            FROM Roles 
            WHERE IsDeleted = 0";

        if (!string.IsNullOrEmpty(where))
        {
            sql += $" AND ({where})";
        }

        if (!string.IsNullOrEmpty(orderBy))
        {
            sql += $" ORDER BY {orderBy}";
        }
        else
        {
            sql += " ORDER BY Name";
        }

        sql += $" LIMIT {limit} OFFSET {offset}";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Role>(sql, parameters);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(string? where = null, object? parameters = null)
    {
        var sql = "SELECT COUNT(*) FROM Roles WHERE IsDeleted = 0";

        if (!string.IsNullOrEmpty(where))
        {
            sql += $" AND ({where})";
        }

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, parameters);
    }

    /// <inheritdoc />
    public async Task<Role> AddAsync(Role entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.ModifiedAt = DateTime.UtcNow;
        entity.IsDeleted = false;

        const string sql = @"
            INSERT INTO Roles (Code, Name, Description, IsSystemRole, CreatedAt, ModifiedAt, IsDeleted)
            VALUES (@Code, @Name, @Description, @IsSystemRole, @CreatedAt, @ModifiedAt, @IsDeleted);
            SELECT last_insert_rowid();";

        using var connection = _connectionFactory.CreateConnection();
        var id = await connection.QuerySingleAsync<int>(sql, entity);
        entity.RoleId = id;
        return entity;
    }

    /// <inheritdoc />
    public async Task<Role> UpdateAsync(Role entity)
    {
        entity.ModifiedAt = DateTime.UtcNow;

        const string sql = @"
            UPDATE Roles 
            SET Code = @Code, 
                Name = @Name, 
                Description = @Description, 
                IsSystemRole = @IsSystemRole, 
                ModifiedAt = @ModifiedAt
            WHERE RoleId = @RoleId AND IsDeleted = 0";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, entity);
        return entity;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id)
    {
        // Note: Role entity uses int RoleId, not Guid
        await Task.CompletedTask;
        throw new NotSupportedException("Use DeleteAsync(int) for Role entity");
    }

    /// <summary>
    /// Soft deletes a role by its integer ID
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = @"
            UPDATE Roles 
            SET IsDeleted = 1, ModifiedAt = @ModifiedAt 
            WHERE RoleId = @id AND IsDeleted = 0";

        using var connection = _connectionFactory.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(sql, new { id, ModifiedAt = DateTime.UtcNow });
        return rowsAffected > 0;
    }

    /// <inheritdoc />
    public async Task<bool> HardDeleteAsync(Guid id)
    {
        // Note: Role entity uses int RoleId, not Guid
        await Task.CompletedTask;
        throw new NotSupportedException("Use HardDeleteAsync(int) for Role entity");
    }

    /// <summary>
    /// Permanently deletes a role by its integer ID
    /// </summary>
    public async Task<bool> HardDeleteAsync(int id)
    {
        const string sql = "DELETE FROM Roles WHERE RoleId = @id";

        using var connection = _connectionFactory.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(sql, new { id });
        return rowsAffected > 0;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Guid id)
    {
        // Note: Role entity uses int RoleId, not Guid
        await Task.CompletedTask;
        throw new NotSupportedException("Use RoleExistsAsync(int) for Role entity");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> QueryAsync(string sql, object? parameters = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Role>(sql, parameters);
    }

    /// <inheritdoc />
    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, parameters);
    }

    /// <inheritdoc />
    public async Task<Role> CreateAsync(Role role)
    {
        const string sql = @"
            INSERT INTO Roles (Code, Name, Description, IsSystemRole, CreatedAt, ModifiedAt, IsDeleted)
            VALUES (@Code, @Name, @Description, @IsSystemRole, @CreatedAt, @ModifiedAt, @IsDeleted);
            SELECT RoleId, Code, Name, Description, IsSystemRole, CreatedAt, ModifiedAt, IsDeleted
            FROM Roles WHERE RoleId = last_insert_rowid();";

        using var connection = _connectionFactory.CreateConnection();
        role.CreatedAt = DateTime.UtcNow;
        role.ModifiedAt = null;
        role.IsDeleted = false;

        var createdRole = await connection.QuerySingleAsync<Role>(sql, role);
        return createdRole;
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Role> Roles, int TotalCount)> GetAllAsync(int page = 1, int pageSize = 10, bool includeSystemRoles = true)
    {
        var whereClause = includeSystemRoles ? "WHERE IsDeleted = 0" : "WHERE IsDeleted = 0 AND IsSystemRole = 0";

        var countSql = $"SELECT COUNT(*) FROM Roles {whereClause}";
        var dataSql = $@"
            SELECT RoleId, Code, Name, Description, IsSystemRole, CreatedAt, ModifiedAt, IsDeleted
            FROM Roles {whereClause}
            ORDER BY Name
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        using var connection = _connectionFactory.CreateConnection();
        var offset = (page - 1) * pageSize;

        var totalCount = await connection.QuerySingleAsync<int>(countSql);
        var roles = await connection.QueryAsync<Role>(dataSql, new { pageSize, offset });

        return (roles, totalCount);
    }

    /// <inheritdoc />
    public async Task<bool> IsRoleCodeInUseAsync(string code, int? excludeRoleId = null)
    {
        var sql = "SELECT COUNT(*) FROM Roles WHERE Code = @code AND IsDeleted = 0";
        object parameters = new { code };

        if (excludeRoleId.HasValue)
        {
            sql += " AND RoleId != @excludeRoleId";
            parameters = new { code, excludeRoleId = excludeRoleId.Value };
        }

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.QuerySingleAsync<int>(sql, parameters);
        return count > 0;
    }

    /// <inheritdoc />
    public async Task SeedDefaultRolesAsync()
    {
        const string checkSql = "SELECT COUNT(*) FROM Roles WHERE IsSystemRole = 1 AND IsDeleted = 0";
        const string insertSql = @"
            INSERT OR IGNORE INTO Roles (Code, Name, Description, IsSystemRole, CreatedAt, ModifiedAt, IsDeleted)
            VALUES (@Code, @Name, @Description, @IsSystemRole, @CreatedAt, @ModifiedAt, @IsDeleted)";

        using var connection = _connectionFactory.CreateConnection();

        var existingSystemRoles = await connection.QuerySingleAsync<int>(checkSql);
        if (existingSystemRoles > 0)
        {
            return; // Default roles already exist
        }

        var defaultRoles = new[]
        {
            new Role
            {
                Code = "ADMIN",
                Name = "Administrator",
                Description = "System administrator with full access",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Role
            {
                Code = "USER",
                Name = "User",
                Description = "Regular system user",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        };

        foreach (var role in defaultRoles)
        {
            await connection.ExecuteAsync(insertSql, role);
        }
    }
}
