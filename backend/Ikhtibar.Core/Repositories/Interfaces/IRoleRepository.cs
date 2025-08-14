using Ikhtibar.Shared.Entities;

using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for Role entity operations
/// Following SRP: ONLY role-specific data access operations
/// Following schema with int primary keys
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Get role by ID
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>Role if found, null otherwise</returns>
    Task<Role?> GetByIdAsync(int roleId);

    /// <summary>
    /// Gets a role by its unique code
    /// </summary>
    /// <param name="code">The role code (e.g., "ADMIN", "USER")</param>
    /// <returns>The role if found, null otherwise</returns>
    Task<Role?> GetByCodeAsync(string code);

    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="role">Role entity to create</param>
    /// <returns>Created role with ID populated</returns>
    Task<Role> CreateAsync(Role role);

    /// <summary>
    /// Update an existing role
    /// </summary>
    /// <param name="role">Role entity to update</param>
    /// <returns>Updated role</returns>
    Task<Role> UpdateAsync(Role role);

    /// <summary>
    /// Delete a role (soft delete)
    /// </summary>
    /// <param name="roleId">Role ID to delete</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteAsync(int roleId);

    /// <summary>
    /// Get all roles with pagination
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="includeSystemRoles">Whether to include system roles</param>
    /// <returns>Tuple of roles and total count</returns>
    Task<(IEnumerable<Role> Roles, int TotalCount)> GetAllAsync(int page, int pageSize, bool includeSystemRoles = true);

    /// <summary>
    /// Checks if a role exists by its unique identifier
    /// </summary>
    /// <param name="roleId">The role identifier</param>
    /// <returns>True if role exists, false otherwise</returns>
    Task<bool> RoleExistsAsync(int roleId);

    /// <summary>
    /// Check if role code is already in use
    /// </summary>
    /// <param name="code">Role code to check</param>
    /// <param name="excludeRoleId">Role ID to exclude from check (for updates)</param>
    /// <returns>True if code is in use, false otherwise</returns>
    Task<bool> IsRoleCodeInUseAsync(string code, int? excludeRoleId = null);

    /// <summary>
    /// Gets all system roles (built-in roles)
    /// </summary>
    /// <returns>Collection of system roles</returns>
    Task<IEnumerable<Role>> GetSystemRolesAsync();

    /// <summary>
    /// Gets all custom (non-system) roles
    /// </summary>
    /// <returns>Collection of custom roles</returns>
    Task<IEnumerable<Role>> GetCustomRolesAsync();

    /// <summary>
    /// Seeds default system roles if they don't exist
    /// </summary>
    /// <returns>Task</returns>
    Task SeedDefaultRolesAsync();
}
