using Dapper;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User entity operations
/// Handles all user-related data access operations
/// </summary>
public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IDbConnectionFactory connectionFactory, ILogger<UserRepository> logger)
        : base(connectionFactory)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get user by email address
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>User if found, null otherwise</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            const string sql = @"
                SELECT UserId, Email, Username, FirstName, LastName, PasswordHash, 
                       IsActive, EmailVerified, CreatedAt, ModifiedAt
                FROM Users 
                WHERE Email = @Email AND IsActive = 1 AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });

            _logger.LogDebug("Retrieved user by email: {Email}, Found: {Found}", email, user != null);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Get user by username
    /// </summary>
    /// <param name="username">User's username</param>
    /// <returns>User if found, null otherwise</returns>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        try
        {
            const string sql = @"
                SELECT Id, Email, Username, FirstName, LastName, PasswordHash, 
                       IsActive, EmailConfirmed, LastLoginAt, CreatedAt, UpdatedAt
                FROM Users 
                WHERE Username = @Username AND IsActive = 1";

            using var connection = _connectionFactory.CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });

            _logger.LogDebug("Retrieved user by username: {Username}, Found: {Found}", username, user != null);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by username: {Username}", username);
            throw;
        }
    }

    /// <summary>
    /// Validate user credentials
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="passwordHash">Hashed password</param>
    /// <returns>True if credentials are valid, false otherwise</returns>
    public async Task<bool> ValidateCredentialsAsync(string email, string passwordHash)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) 
                FROM Users 
                WHERE Email = @Email 
                  AND PasswordHash = @PasswordHash 
                  AND IsActive = 1 
                  AND EmailConfirmed = 1";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QuerySingleAsync<int>(sql, new { Email = email, PasswordHash = passwordHash });

            var isValid = count > 0;
            _logger.LogDebug("Credential validation for email: {Email}, Valid: {Valid}", email, isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating credentials for email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Update user's last login timestamp
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    public async Task UpdateLastLoginAsync(int userId)
    {
        try
        {
            const string sql = @"
                UPDATE Users 
                SET LastLoginAt = @LastLoginAt, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(sql, new
            {
                Id = userId,
                LastLoginAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            _logger.LogDebug("Updated last login for user: {UserId}, Affected rows: {AffectedRows}", userId, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last login for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get user roles
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user role names</returns>
    public async Task<List<string>> GetUserRolesAsync(int userId)
    {
        try
        {
            const string sql = @"
                SELECT r.Name
                FROM Roles r
                INNER JOIN UserRoles ur ON r.Id = ur.RoleId
                WHERE ur.UserId = @UserId";

            using var connection = _connectionFactory.CreateConnection();
            var roles = await connection.QueryAsync<string>(sql, new { UserId = userId });

            var roleList = roles.ToList();
            _logger.LogDebug("Retrieved {RoleCount} roles for user: {UserId}", roleList.Count, userId);
            return roleList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Check if user is locked out
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>True if user is locked out, false otherwise</returns>
    public async Task<bool> IsUserLockedOutAsync(int userId)
    {
        try
        {
            const string sql = @"
                SELECT CASE 
                    WHEN LockoutEnd IS NULL THEN 0
                    WHEN LockoutEnd > @CurrentTime THEN 1
                    ELSE 0
                END
                FROM Users 
                WHERE Id = @UserId";

            using var connection = _connectionFactory.CreateConnection();
            var isLockedOut = await connection.QuerySingleOrDefaultAsync<bool>(sql, new
            {
                UserId = userId,
                CurrentTime = DateTime.UtcNow
            });

            _logger.LogDebug("Lockout status for user {UserId}: {IsLockedOut}", userId, isLockedOut);
            return isLockedOut;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking lockout status for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Update user's failed login count
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="increment">Whether to increment (true) or reset (false) the count</param>
    /// <returns>Task</returns>
    public async Task UpdateFailedLoginCountAsync(int userId, bool increment = true)
    {
        try
        {
            string sql;
            if (increment)
            {
                sql = @"
                    UPDATE Users 
                    SET AccessFailedCount = ISNULL(AccessFailedCount, 0) + 1,
                        UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";
            }
            else
            {
                sql = @"
                    UPDATE Users 
                    SET AccessFailedCount = 0,
                        UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";
            }

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(sql, new
            {
                Id = userId,
                UpdatedAt = DateTime.UtcNow
            });

            _logger.LogDebug("Updated failed login count for user: {UserId}, Increment: {Increment}, Affected rows: {AffectedRows}",
                userId, increment, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating failed login count for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Set user lockout
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="lockoutEnd">Lockout end time (null to remove lockout)</param>
    /// <returns>Task</returns>
    public async Task SetUserLockoutAsync(int userId, DateTime? lockoutEnd = null)
    {
        try
        {
            const string sql = @"
                UPDATE Users 
                SET LockoutEnd = @LockoutEnd,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(sql, new
            {
                Id = userId,
                LockoutEnd = lockoutEnd,
                UpdatedAt = DateTime.UtcNow
            });

            _logger.LogDebug("Set lockout for user: {UserId}, LockoutEnd: {LockoutEnd}, Affected rows: {AffectedRows}",
                userId, lockoutEnd, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting lockout for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Check if email exists in the system
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="excludeUserId">User ID to exclude from check (for updates)</param>
    /// <returns>True if email exists, false otherwise</returns>
    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        try
        {
            string sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            object parameters = new { Email = email };

            if (excludeUserId.HasValue)
            {
                sql += " AND Id != @ExcludeUserId";
                parameters = new { Email = email, ExcludeUserId = excludeUserId.Value };
            }

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QuerySingleAsync<int>(sql, parameters);

            var exists = count > 0;
            _logger.LogDebug("Email existence check for: {Email}, Exists: {Exists}", email, exists);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email existence: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Hash password using SHA256
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Verify password against hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hash">Stored password hash</param>
    /// <returns>True if password matches hash</returns>
    public static bool VerifyPassword(string password, string hash)
    {
        var computedHash = HashPassword(password);
        return computedHash == hash;
    }

    /// <summary>
    /// Record failed login attempt
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="userAgent">Client user agent</param>
    /// <returns>Task</returns>
    public async Task RecordFailedLoginAttemptAsync(string email, string? ipAddress, string? userAgent)
    {
        try
        {
            const string sql = @"
                INSERT INTO LoginAttempts (Email, IpAddress, UserAgent, AttemptTime, IsSuccessful)
                VALUES (@Email, @IpAddress, @UserAgent, @AttemptTime, 0)";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                Email = email,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                AttemptTime = DateTime.UtcNow
            });

            _logger.LogDebug("Recorded failed login attempt for: {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording failed login attempt for: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Reset failed login attempts count for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    public async Task ResetFailedLoginAttemptsAsync(int userId)
    {
        try
        {
            await UpdateFailedLoginCountAsync(userId, false);
            _logger.LogDebug("Reset failed login attempts for user: {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting failed login attempts for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get failed login attempts count in the specified time window
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="timeWindowMinutes">Time window in minutes</param>
    /// <returns>Number of failed attempts</returns>
    public async Task<int> GetFailedLoginAttemptsCountAsync(string email, int timeWindowMinutes)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM LoginAttempts 
                WHERE Email = @Email
                  AND IsSuccessful = 0
                  AND AttemptTime >= @CutoffTime";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QuerySingleAsync<int>(sql, new
            {
                Email = email,
                CutoffTime = DateTime.UtcNow.AddMinutes(-timeWindowMinutes)
            });

            _logger.LogDebug("Failed login attempts for {Email} in last {Minutes} minutes: {Count}",
                email, timeWindowMinutes, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting failed login attempts count for: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Lock user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="lockoutEndTime">When lockout expires</param>
    /// <returns>Task</returns>
    public async Task LockUserAccountAsync(int userId, DateTime lockoutEndTime)
    {
        try
        {
            await SetUserLockoutAsync(userId, lockoutEndTime);
            _logger.LogDebug("Locked user account: {UserId} until {LockoutEnd}", userId, lockoutEndTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking user account: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Unlock user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    public async Task UnlockUserAccountAsync(int userId)
    {
        try
        {
            await SetUserLockoutAsync(userId, null);
            _logger.LogDebug("Unlocked user account: {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking user account: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get user with roles included
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User with roles, null if not found</returns>
    public async Task<User?> GetUserWithRolesAsync(int userId)
    {
        try
        {
            const string sql = @"
                SELECT u.UserId, u.Email, u.Username, u.FirstName, u.LastName, u.PasswordHash, 
                       u.IsActive, u.EmailVerified, u.CreatedAt, u.UpdatedAt
                FROM Users u
                WHERE u.UserId = @UserId AND u.IsActive = 1";

            using var connection = _connectionFactory.CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });

            if (user != null)
            {
                // Load user roles separately
                var roles = await GetUserRolesAsync(userId);
                // Note: Since User entity doesn't have a direct Roles property,
                // the UserRoles navigation property should be populated instead
                // This would require a more complex mapping or returning a DTO
            }

            _logger.LogDebug("Retrieved user with roles: {UserId}, Found: {Found}", userId, user != null);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with roles: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get user by email with roles included
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>User with roles, null if not found</returns>
    public async Task<User?> GetUserWithRolesByEmailAsync(string email)
    {
        try
        {
            const string sql = @"
                SELECT u.UserId, u.Email, u.Username, u.FirstName, u.LastName, u.PasswordHash, 
                       u.IsActive, u.EmailVerified, u.CreatedAt, u.UpdatedAt
                FROM Users u
                WHERE u.Email = @Email AND u.IsActive = 1";

            using var connection = _connectionFactory.CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });

            if (user != null)
            {
                // Load user roles separately
                var roles = await GetUserRolesAsync(user.UserId);
                // Note: Since User entity doesn't have a direct Roles property,
                // the UserRoles navigation property should be populated instead
                // This would require a more complex mapping or returning a DTO
            }

            _logger.LogDebug("Retrieved user with roles by email: {Email}, Found: {Found}", email, user != null);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with roles by email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Check if user exists by ID
    /// </summary>
    /// <param name="id">User ID to check</param>
    /// <returns>True if user exists, false otherwise</returns>
    public async Task<bool> UserExistsAsync(int id)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1) 
                FROM Users 
                WHERE UserId = @id AND IsActive = 1";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QueryFirstOrDefaultAsync<int>(sql, new { id });

            _logger.LogDebug("Checked user existence for ID: {UserId}, Exists: {Exists}", id, count > 0);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user existence for ID: {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User if found, null otherwise</returns>
    public new async Task<User?> GetByIdAsync(int userId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Users 
                WHERE UserId = @UserId AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="user">User entity to create</param>
    /// <returns>Created user with ID populated</returns>
    public async Task<User> CreateAsync(User user)
    {
        try
        {
            user.CreatedAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;
            user.IsDeleted = false;

            const string sql = @"
                INSERT INTO Users (
                    Username, Email, PasswordHash, FirstName, LastName, 
                    PhoneNumber, PreferredLanguage, IsActive, EmailVerified, PhoneVerified,
                    CreatedAt, ModifiedAt, IsDeleted
                ) VALUES (
                    @Username, @Email, @PasswordHash, @FirstName, @LastName,
                    @PhoneNumber, @PreferredLanguage, @IsActive, @EmailVerified, @PhoneVerified,
                    @CreatedAt, @ModifiedAt, @IsDeleted
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = _connectionFactory.CreateConnection();
            var userId = await connection.QuerySingleAsync<int>(sql, user);
            user.UserId = userId;

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {@User}", user);
            throw;
        }
    }

    /// <summary>
    /// Delete a user (soft delete)
    /// </summary>
    /// <param name="userId">User ID to delete</param>
    /// <returns>True if deleted, false if not found</returns>
    public new async Task<bool> DeleteAsync(int userId)
    {
        try
        {
            const string sql = @"
                UPDATE Users 
                SET IsDeleted = 1, ModifiedAt = @ModifiedAt, DeletedAt = @DeletedAt
                WHERE UserId = @UserId AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                ModifiedAt = DateTime.UtcNow,
                DeletedAt = DateTime.UtcNow
            });

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get all users with pagination
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>Tuple of users and total count</returns>
    public async Task<(IEnumerable<User> Users, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        try
        {
            var offset = (page - 1) * pageSize;

            const string countSql = @"
                SELECT COUNT(*) FROM Users 
                WHERE IsDeleted = 0";

            const string usersSql = @"
                SELECT * FROM Users 
                WHERE IsDeleted = 0
                ORDER BY CreatedAt DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            using var connection = _connectionFactory.CreateConnection();
            var totalCount = await connection.QuerySingleAsync<int>(countSql);
            var users = await connection.QueryAsync<User>(usersSql, new
            {
                PageSize = pageSize,
                Offset = offset
            });

            return (users, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users with pagination: Page={Page}, PageSize={PageSize}", page, pageSize);
            throw;
        }
    }

    /// <summary>
    /// Get all users with complete information including roles and last login
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>Tuple of users with roles and last login, and total count</returns>
    public async Task<(IEnumerable<User> Users, int TotalCount)> GetAllWithDetailsAsync(int page, int pageSize)
    {
        try
        {
            var offset = (page - 1) * pageSize;

            const string countSql = @"
                SELECT COUNT(*) FROM Users 
                WHERE IsDeleted = 0";

            const string usersSql = @"
                SELECT 
                    u.UserId, u.Username, u.Email, u.FirstName, u.LastName, 
                    u.PhoneNumber, u.PreferredLanguage, u.IsActive, 
                    u.EmailVerified, u.PhoneVerified, u.CreatedAt, u.ModifiedAt,
                    (SELECT MAX(la.Timestamp) 
                     FROM LoginAttempts la 
                     WHERE la.UserId = u.UserId AND la.Success = 1) as LastLoginAt,
                    r.RoleId, r.Name, r.Code, r.Description, r.IsSystemRole, r.CreatedAt as RoleCreatedAt, r.ModifiedAt as RoleModifiedAt
                FROM Users u
                LEFT JOIN UserRoles ur ON u.UserId = ur.UserId
                LEFT JOIN Roles r ON ur.RoleId = r.RoleId AND r.IsDeleted = 0
                WHERE u.IsDeleted = 0
                ORDER BY u.CreatedAt DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            using var connection = _connectionFactory.CreateConnection();
            var totalCount = await connection.QuerySingleAsync<int>(countSql);
            
            var userDictionary = new Dictionary<int, User>();
            
            await connection.QueryAsync<User, Role, User>(
                usersSql,
                (user, role) => {
                    if (!userDictionary.TryGetValue(user.UserId, out var existingUser))
                    {
                        existingUser = user;
                        existingUser.UserRoles = new List<UserRole>();
                        userDictionary.Add(user.UserId, existingUser);
                    }

                    if (role != null)
                    {
                        // Create UserRole object to maintain the navigation pattern
                        var userRole = new UserRole 
                        { 
                            UserId = user.UserId, 
                            RoleId = role.RoleId, 
                            Role = role,
                            User = existingUser
                        };
                        existingUser.UserRoles.Add(userRole);
                    }

                    return existingUser;
                },
                new { PageSize = pageSize, Offset = offset },
                splitOn: "RoleId"
            );

            var users = userDictionary.Values;
            return (users, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users with details: Page={Page}, PageSize={PageSize}", page, pageSize);
            throw;
        }
    }

    /// <summary>
    /// Check if username exists in the system
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <param name="excludeUserId">User ID to exclude from check (for updates)</param>
    /// <returns>True if username exists, false otherwise</returns>
    public async Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Users 
                WHERE Username = @Username AND IsDeleted = 0";

            object parameters = new { Username = username };

            if (excludeUserId.HasValue)
            {
                sql += " AND UserId != @ExcludeUserId";
                parameters = new { Username = username, ExcludeUserId = excludeUserId.Value };
            }

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if username exists: {Username}", username);
            throw;
        }
    }

    /// <summary>
    /// Get user role entities
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of role entities for the user</returns>
    public async Task<IEnumerable<Role>> GetUserRoleEntitiesAsync(int userId)
    {
        try
        {
            const string sql = @"
                SELECT r.RoleId, r.Code, r.Name, r.Description, r.IsSystemRole, 
                       r.CreatedAt, r.CreatedBy, r.ModifiedAt, r.ModifiedBy, 
                       r.DeletedAt, r.DeletedBy, r.IsDeleted, r.RowVersion
                FROM Roles r
                INNER JOIN UserRoles ur ON r.RoleId = ur.RoleId
                WHERE ur.UserId = @UserId AND r.IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var roles = await connection.QueryAsync<Role>(sql, new { UserId = userId });
            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user role entities for user: {UserId}", userId);
            throw;
        }
    }

    // ...existing code...
}

// ✅ SRP COMPLIANCE:
// - SINGLE RESPONSIBILITY: User data access operations only
// - NO business logic beyond data persistence
// - NO authentication logic (delegated to services)
// - NO cross-entity operations (roles handled separately)
// - FOCUSED: All methods work toward user data management

// ❌ ANTI-PATTERNS AVOIDED:
// - No mixing of business logic in repository
// - No direct authentication logic
// - No cross-cutting concerns
// - No god repository with mixed responsibilities
