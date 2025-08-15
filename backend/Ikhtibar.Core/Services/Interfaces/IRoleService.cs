using Ikhtibar.Core.DTOs;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for role management operations
/// Following SRP: ONLY role business logic
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="createRoleDto">Role creation data</param>
    /// <returns>Created role DTO</returns>
    Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto);

    /// <summary>
    /// Get role by ID
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>Role DTO if found, null otherwise</returns>
    Task<RoleDto?> GetRoleAsync(int roleId);

    /// <summary>
    /// Get role by code
    /// </summary>
    /// <param name="code">Role code</param>
    /// <returns>Role DTO if found, null otherwise</returns>
    Task<RoleDto?> GetRoleByCodeAsync(string code);

    /// <summary>
    /// Update an existing role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="updateRoleDto">Role update data</param>
    /// <returns>Updated role DTO</returns>
    Task<RoleDto> UpdateRoleAsync(int roleId, UpdateRoleDto updateRoleDto);

    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteRoleAsync(int roleId);

    /// <summary>
    /// Get all roles
    /// </summary>
    /// <returns>Collection of role DTOs</returns>
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();

    /// <summary>
    /// Get active roles only
    /// </summary>
    /// <returns>Collection of active role DTOs</returns>
    Task<IEnumerable<RoleDto>> GetActiveRolesAsync();

    /// <summary>
    /// Seed default system roles
    /// </summary>
    /// <returns>Task</returns>
    Task SeedDefaultRolesAsync();

    /// <summary>
    /// Check if role exists
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>True if role exists, false otherwise</returns>
    Task<bool> RoleExistsAsync(int roleId);

    /// <summary>
    /// Check if role code exists
    /// </summary>
    /// <param name="code">Role code to check</param>
    /// <param name="excludeRoleId">Role ID to exclude from check</param>
    /// <returns>True if role code exists, false otherwise</returns>
    Task<bool> CodeExistsAsync(string code, int? excludeRoleId = null);

    /// <summary>
    /// Get system roles
    /// </summary>
    /// <returns>Collection of system role DTOs</returns>
    Task<IEnumerable<RoleDto>> GetSystemRolesAsync();
}
