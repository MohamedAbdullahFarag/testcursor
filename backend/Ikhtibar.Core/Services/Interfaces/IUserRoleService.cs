using Ikhtibar.Core.DTOs;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for user-role relationship operations
/// Following SRP: ONLY user-role business logic
/// </summary>
public interface IUserRoleService
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
    /// <returns>Collection of role DTOs</returns>
    Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId);

    /// <summary>
    /// Get all users for a specific role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>Collection of user DTOs</returns>
    Task<IEnumerable<UserDto>> GetRoleUsersAsync(int roleId);

    /// <summary>
    /// Check if a user has a specific role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <returns>True if user has the role, false otherwise</returns>
    Task<bool> UserHasRoleAsync(int userId, int roleId);

    /// <summary>
    /// Check if a user has a specific role by code
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleCode">Role code</param>
    /// <returns>True if user has the role, false otherwise</returns>
    Task<bool> UserHasRoleAsync(int userId, string roleCode);

    /// <summary>
    /// Remove all roles from a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    Task RemoveAllUserRolesAsync(int userId);

    /// <summary>
    /// Bulk assign roles to a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleIds">Collection of role IDs</param>
    /// <returns>Task</returns>
    Task AssignRolesAsync(int userId, IEnumerable<int> roleIds);

    /// <summary>
    /// Replace all user roles with new ones
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleIds">Collection of new role IDs</param>
    /// <returns>Task</returns>
    Task ReplaceUserRolesAsync(int userId, IEnumerable<int> roleIds);
}
