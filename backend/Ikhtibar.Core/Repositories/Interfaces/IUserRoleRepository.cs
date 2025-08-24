using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for UserRole entity operations
/// Following SRP: ONLY UserRole data operations
/// </summary>
public interface IUserRoleRepository : IBaseRepository<UserRole>
{
    /// <summary>
    /// Assign a role to a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <returns>Task</returns>
    Task AssignRoleAsync(int userId, int roleId);

    /// <summary>
    /// Remove a role from a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <returns>Task</returns>
    Task RemoveRoleAsync(int userId, int roleId);

    /// <summary>
    /// Get all roles for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of user roles</returns>
    Task<IEnumerable<UserRole>> GetUserRolesAsync(int userId);

    /// <summary>
    /// Get all users for a specific role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>Collection of user roles</returns>
    Task<IEnumerable<UserRole>> GetRoleUsersAsync(int roleId);

    /// <summary>
    /// Check if a user has a specific role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <returns>True if user has the role, false otherwise</returns>
    Task<bool> UserHasRoleAsync(int userId, int roleId);

    /// <summary>
    /// Remove all roles from a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    Task RemoveAllUserRolesAsync(int userId);

    /// <summary>
    /// Get user role by user ID and role ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <returns>UserRole entity if found, null otherwise</returns>
    Task<UserRole?> GetUserRoleAsync(int userId, int roleId);
}
