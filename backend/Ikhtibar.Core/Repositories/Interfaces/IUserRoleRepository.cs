using Ikhtibar.Shared.Entities;

using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for UserRole junction entity operations
/// Following SRP: ONLY user-role relationship operations
/// </summary>
public interface IUserRoleRepository
{
    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="roleId">The role identifier</param>
    /// <returns>Task representing the async operation</returns>
    Task AssignRoleAsync(int userId, int roleId);

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="roleId">The role identifier</param>
    /// <returns>Task representing the async operation</returns>
    Task RemoveRoleAsync(int userId, int roleId);

    /// <summary>
    /// Gets all roles assigned to a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>Collection of roles assigned to the user</returns>
    Task<IEnumerable<Role>> GetUserRolesAsync(int userId);

    /// <summary>
    /// Gets all users assigned to a role
    /// </summary>
    /// <param name="roleId">The role identifier</param>
    /// <returns>Collection of users assigned to the role</returns>
    Task<IEnumerable<User>> GetRoleUsersAsync(int roleId);

    /// <summary>
    /// Checks if a user has a specific role
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="roleId">The role identifier</param>
    /// <returns>True if user has the role, false otherwise</returns>
    Task<bool> UserHasRoleAsync(int userId, int roleId);

    /// <summary>
    /// Checks if a user has a specific role by role code
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="roleCode">The role code</param>
    /// <returns>True if user has the role, false otherwise</returns>
    Task<bool> UserHasRoleAsync(int userId, string roleCode);

    /// <summary>
    /// Removes all roles from a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>Task representing the async operation</returns>
    Task RemoveAllUserRolesAsync(int userId);

    /// <summary>
    /// Gets the user-role assignment record
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="roleId">The role identifier</param>
    /// <returns>The user-role assignment if found, null otherwise</returns>
    Task<UserRole?> GetUserRoleAsync(int userId, int roleId);
}
