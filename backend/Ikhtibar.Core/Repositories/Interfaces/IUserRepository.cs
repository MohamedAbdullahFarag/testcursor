using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for User entity operations
/// Following SRP: ONLY User data operations
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>
    /// Get user by email address
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>User entity if found, null otherwise</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Check if user exists by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>True if user exists, false otherwise</returns>
    Task<bool> UserExistsAsync(int id);

    /// <summary>
    /// Get users by role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>Collection of users with the specified role</returns>
    Task<IEnumerable<User>> GetByRoleAsync(int roleId);

    /// <summary>
    /// Get active users only
    /// </summary>
    /// <returns>Collection of active users</returns>
    Task<IEnumerable<User>> GetActiveUsersAsync();

    /// <summary>
    /// Search users by name or email
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>Collection of matching users</returns>
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);

    /// <summary>
    /// Check if email exists
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="excludeUserId">User ID to exclude from check</param>
    /// <returns>True if email exists, false otherwise</returns>
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
}
