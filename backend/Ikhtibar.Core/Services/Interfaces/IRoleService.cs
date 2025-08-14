using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for Role management operations
/// Following SRP: ONLY role business logic operations
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Creates a new role
    /// </summary>
    /// <param name="createRoleDto">Role creation data</param>
    /// <returns>Created role data</returns>
    Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto);

    /// <summary>
    /// Gets a role by ID
    /// </summary>
    /// <param name="id">Role identifier</param>
    /// <returns>Role data if found, null otherwise</returns>
    Task<RoleDto?> GetRoleAsync(int id);

    /// <summary>
    /// Updates an existing role
    /// </summary>
    /// <param name="id">Role identifier</param>
    /// <param name="updateRoleDto">Role update data</param>
    /// <returns>Updated role data if successful, null if role not found</returns>
    Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto);

    /// <summary>
    /// Deletes a role (soft delete)
    /// </summary>
    /// <param name="id">Role identifier</param>
    /// <returns>True if role was deleted, false if not found</returns>
    Task<bool> DeleteRoleAsync(int id);

    /// <summary>
    /// Gets all roles with pagination
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="includeSystemRoles">Whether to include system roles</param>
    /// <returns>Paginated list of roles</returns>
    Task<PaginatedResult<RoleDto>> GetAllRolesAsync(int page = 1, int pageSize = 10, bool includeSystemRoles = true);

    /// <summary>
    /// Gets a role by its code
    /// </summary>
    /// <param name="code">Role code</param>
    /// <returns>Role data if found, null otherwise</returns>
    Task<RoleDto?> GetRoleByCodeAsync(string code);

    /// <summary>
    /// Gets all system roles
    /// </summary>
    /// <returns>List of system roles</returns>
    Task<IEnumerable<RoleDto>> GetSystemRolesAsync();

    /// <summary>
    /// Gets all custom (non-system) roles
    /// </summary>
    /// <returns>List of custom roles</returns>
    Task<IEnumerable<RoleDto>> GetCustomRolesAsync();

    /// <summary>
    /// Checks if a role exists by ID
    /// </summary>
    /// <param name="id">Role identifier</param>
    /// <returns>True if role exists, false otherwise</returns>
    Task<bool> RoleExistsAsync(int id);

    /// <summary>
    /// Checks if a role code is already in use
    /// </summary>
    /// <param name="code">Role code to check</param>
    /// <param name="excludeRoleId">Role ID to exclude from check (for updates)</param>
    /// <returns>True if code is in use, false otherwise</returns>
    Task<bool> IsRoleCodeInUseAsync(string code, int? excludeRoleId = null);

    /// <summary>
    /// Seeds default system roles if they don't exist
    /// </summary>
    /// <returns>Number of roles created</returns>
    Task<int> SeedDefaultRolesAsync();
}
