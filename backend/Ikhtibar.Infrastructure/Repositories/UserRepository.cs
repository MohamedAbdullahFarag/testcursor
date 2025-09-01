using Dapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User entity operations
/// Following SRP: ONLY User data operations
/// </summary>
public class UserRepository : BaseRepository<User>, IUserRepository
{
    private new readonly ILogger<UserRepository> _logger;

    public UserRepository(IDbConnectionFactory connectionFactory, ILogger<UserRepository> logger)
        : base(connectionFactory, logger, "Users", "UserId")
    {
        _logger = logger;
    }

    /// <summary>
    /// Get user by email address with roles
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT u.UserId, u.Username, u.Email, u.FirstName, u.LastName, u.PhoneNumber, 
                       u.PreferredLanguage, u.IsActive, u.EmailVerified, u.PhoneVerified, 
                       u.PasswordHash, u.CreatedAt, u.ModifiedAt,
                       ur.UserId, ur.RoleId, ur.AssignedAt, ur.AssignedBy,
                       r.RoleId, r.Code, r.Name, r.Description, r.IsSystemRole, r.CreatedAt
                FROM Users u
                LEFT JOIN UserRoles ur ON u.UserId = ur.UserId
                LEFT JOIN Roles r ON ur.RoleId = r.RoleId
                WHERE u.Email = @Email AND u.IsDeleted = 0";

            var userDictionary = new Dictionary<int, User>();
            
            await connection.QueryAsync<User, UserRole, Role, User>(
                sql,
                (user, userRole, role) =>
                {
                    if (!userDictionary.TryGetValue(user.UserId, out var existingUser))
                    {
                        existingUser = user;
                        existingUser.UserRoles = new List<UserRole>();
                        userDictionary.Add(user.UserId, existingUser);
                    }

                    if (userRole != null && userRole.UserId > 0 && role != null)
                    {
                        userRole.Role = role;
                        existingUser.UserRoles.Add(userRole);
                    }

                    return existingUser;
                },
                new { Email = email },
                splitOn: "UserId,RoleId"
            );

            return userDictionary.Values.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Check if user exists by ID
    /// </summary>
    public async Task<bool> UserExistsAsync(int id)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM Users WHERE UserId = @Id AND IsDeleted = 0";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user exists: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Get users by role
    /// </summary>
    public async Task<IEnumerable<User>> GetByRoleAsync(int roleId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT u.UserId, u.Username, u.Email, u.FirstName, u.LastName, 
                       u.PhoneNumber, u.PreferredLanguage, u.IsActive, u.EmailVerified, 
                       u.PhoneVerified, u.CreatedAt, u.ModifiedAt
                FROM Users u
                INNER JOIN UserRoles ur ON u.UserId = ur.UserId
                WHERE ur.RoleId = @RoleId AND u.IsDeleted = 0 AND u.IsActive = 1";

            var users = await connection.QueryAsync<User>(sql, new { RoleId = roleId });
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by role: {RoleId}", roleId);
            throw;
        }
    }

    /// <summary>
    /// Get active users only
    /// </summary>
    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT UserId, Username, Email, FirstName, LastName, PhoneNumber, 
                       PreferredLanguage, IsActive, EmailVerified, PhoneVerified, 
                       CreatedAt, ModifiedAt
                FROM Users 
                WHERE IsActive = 1 AND IsDeleted = 0
                ORDER BY FirstName, LastName";

            var users = await connection.QueryAsync<User>(sql);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users");
            throw;
        }
    }

    /// <summary>
    /// Search users by name or email
    /// </summary>
    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT UserId, Username, Email, FirstName, LastName, PhoneNumber, 
                       PreferredLanguage, IsActive, EmailVerified, PhoneVerified, 
                       CreatedAt, ModifiedAt
                FROM Users 
                WHERE (FirstName LIKE @SearchTerm OR LastName LIKE @SearchTerm OR Email LIKE @SearchTerm)
                  AND IsDeleted = 0
                ORDER BY FirstName, LastName";

            var searchPattern = $"%{searchTerm}%";
            var users = await connection.QueryAsync<User>(sql, new { SearchTerm = searchPattern });
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users: {SearchTerm}", searchTerm);
            throw;
        }
    }

    /// <summary>
    /// Override base GetByIdAsync to include soft delete check
    /// </summary>
    public override async Task<User?> GetByIdAsync(int id)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT UserId, Username, Email, FirstName, LastName, PhoneNumber, 
                       PreferredLanguage, IsActive, EmailVerified, PhoneVerified, 
                       CreatedAt, ModifiedAt
                FROM Users 
                WHERE UserId = @Id AND IsDeleted = 0";

            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Get user by ID with roles included
    /// </summary>
    public async Task<User?> GetByIdWithRolesAsync(int id)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT u.UserId, u.Username, u.Email, u.FirstName, u.LastName, u.PhoneNumber, 
                       u.PreferredLanguage, u.IsActive, u.EmailVerified, u.PhoneVerified, 
                       u.PasswordHash, u.CreatedAt, u.ModifiedAt,
                       ur.UserId, ur.RoleId, ur.AssignedAt, ur.AssignedBy,
                       r.RoleId, r.Code, r.Name, r.Description, r.IsSystemRole, r.CreatedAt
                FROM Users u
                LEFT JOIN UserRoles ur ON u.UserId = ur.UserId
                LEFT JOIN Roles r ON ur.RoleId = r.RoleId
                WHERE u.UserId = @Id AND u.IsDeleted = 0";

            var userDictionary = new Dictionary<int, User>();
            
            await connection.QueryAsync<User, UserRole, Role, User>(
                sql,
                (user, userRole, role) =>
                {
                    if (!userDictionary.TryGetValue(user.UserId, out var existingUser))
                    {
                        existingUser = user;
                        existingUser.UserRoles = new List<UserRole>();
                        userDictionary.Add(user.UserId, existingUser);
                    }

                    if (userRole != null && userRole.UserId > 0 && role != null)
                    {
                        userRole.Role = role;
                        existingUser.UserRoles.Add(userRole);
                    }

                    return existingUser;
                },
                new { Id = id },
                splitOn: "UserId,RoleId"
            );

            return userDictionary.Values.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID with roles: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Check if email exists
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = excludeUserId.HasValue
                ? "SELECT COUNT(1) FROM Users WHERE Email = @Email AND UserId != @ExcludeUserId AND IsDeleted = 0"
                : "SELECT COUNT(1) FROM Users WHERE Email = @Email AND IsDeleted = 0";

            var parameters = excludeUserId.HasValue
                ? new { Email = email, ExcludeUserId = excludeUserId.Value }
                : new { Email = email, ExcludeUserId = 0 };

            var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if email exists: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Override base GetAllAsync to include soft delete check
    /// </summary>
    public override async Task<IEnumerable<User>> GetAllAsync(string? where = null, object? parameters = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            // Build base SQL with soft delete check
            var baseWhere = "IsDeleted = 0";
            if (!string.IsNullOrEmpty(where))
            {
                baseWhere += $" AND {where}";
            }
            
            var sql = $@"
                SELECT UserId, Username, Email, FirstName, LastName, PhoneNumber, 
                       PreferredLanguage, IsActive, EmailVerified, PhoneVerified, 
                       CreatedAt, ModifiedAt
                FROM Users 
                WHERE {baseWhere}
                ORDER BY FirstName, LastName";

            var users = await connection.QueryAsync<User>(sql, parameters);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            throw;
        }
    }
}
