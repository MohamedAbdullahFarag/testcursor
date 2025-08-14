using Ikhtibar.Shared.Entities;

using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for User entity operations
/// Following schema with int primary keys
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetByIdAsync(int userId);

    /// <summary>
    /// Get user by email address
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Get user by username
    /// </summary>
    /// <param name="username">User's username</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="user">User entity to create</param>
    /// <returns>Created user with ID populated</returns>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="user">User entity to update</param>
    /// <returns>Updated user</returns>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// Delete a user (soft delete)
    /// </summary>
    /// <param name="userId">User ID to delete</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// Get all users with pagination
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>Tuple of users and total count</returns>
    Task<(IEnumerable<User> Users, int TotalCount)> GetAllAsync(int page, int pageSize);

    /// <summary>
    /// Get all users with complete information including roles and last login
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>Tuple of users with roles and last login, and total count</returns>
    Task<(IEnumerable<User> Users, int TotalCount)> GetAllWithDetailsAsync(int page, int pageSize);

    /// <summary>
    /// Check if user exists by ID
    /// </summary>
    /// <param name="userId">User ID to check</param>
    /// <returns>True if user exists, false otherwise</returns>
    Task<bool> UserExistsAsync(int userId);

    /// <summary>
    /// Check if email exists in the system
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="excludeUserId">User ID to exclude from check (for updates)</param>
    /// <returns>True if email exists, false otherwise</returns>
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);

    /// <summary>
    /// Check if username exists in the system
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <param name="excludeUserId">User ID to exclude from check (for updates)</param>
    /// <returns>True if username exists, false otherwise</returns>
    Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null);

    /// <summary>
    /// Validate user credentials
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="passwordHash">Hashed password</param>
    /// <returns>True if credentials are valid, false otherwise</returns>
    Task<bool> ValidateCredentialsAsync(string email, string passwordHash);

    /// <summary>
    /// Update user's last login timestamp
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    Task UpdateLastLoginAsync(int userId);

    /// <summary>
    /// Get user roles
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user role names</returns>
    Task<List<string>> GetUserRolesAsync(int userId);

    /// <summary>
    /// Get user role entities
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of role entities for the user</returns>
    Task<IEnumerable<Role>> GetUserRoleEntitiesAsync(int userId);

    /// <summary>
    /// Check if user is locked out
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>True if user is locked out, false otherwise</returns>
    Task<bool> IsUserLockedOutAsync(int userId);

    /// <summary>
    /// Update user's failed login count
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="increment">Whether to increment (true) or reset (false) the count</param>
    /// <returns>Task</returns>
    Task UpdateFailedLoginCountAsync(int userId, bool increment = true);

    /// <summary>
    /// Set user lockout
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="lockoutEnd">Lockout end time (null to remove lockout)</param>
    /// <returns>Task</returns>
    Task SetUserLockoutAsync(int userId, DateTime? lockoutEnd = null);

    /// <summary>
    /// Get user with roles by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User with roles if found, null otherwise</returns>
    Task<User?> GetUserWithRolesAsync(int userId);
}
