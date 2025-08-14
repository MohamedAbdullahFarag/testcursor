using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for User-Role assignment operations
/// Following SRP: ONLY user-role relationship business logic
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    /// <param name="assignRoleDto">Role assignment data</param>
    /// <returns>True if assignment was successful</returns>
    Task<bool> AssignRoleToUserAsync(AssignRoleDto assignRoleDto);

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="roleId">Role identifier</param>
    /// <returns>True if removal was successful</returns>
    Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);

    /// <summary>
    /// Gets all roles assigned to a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of roles assigned to the user</returns>
    Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId);

    /// <summary>
    /// Gets all users assigned to a role
    /// </summary>
    /// <param name="roleId">Role identifier</param>
    /// <returns>List of users assigned to the role</returns>
    Task<IEnumerable<UserDto>> GetRoleUsersAsync(int roleId);

    /// <summary>
    /// Checks if a user has a specific role
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="roleId">Role identifier</param>
    /// <returns>True if user has the role, false otherwise</returns>
    Task<bool> UserHasRoleAsync(int userId, int roleId);

    /// <summary>
    /// Checks if a user has a specific role by role code
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="roleCode">Role code</param>
    /// <returns>True if user has the role, false otherwise</returns>
    Task<bool> UserHasRoleAsync(int userId, string roleCode);

    /// <summary>
    /// Removes all roles from a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>Number of roles removed</returns>
    Task<int> RemoveAllUserRolesAsync(int userId);

    /// <summary>
    /// Updates all roles for a user (replaces current roles)
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="roleIds">New role IDs to assign</param>
    /// <returns>True if update was successful</returns>
    Task<bool> UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds);

    /// <summary>
    /// Gets users with role assignments summary
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated list of users with their role count</returns>
    Task<PaginatedResult<UserRoleSummaryDto>> GetUsersWithRolesAsync(int page = 1, int pageSize = 10);
}
