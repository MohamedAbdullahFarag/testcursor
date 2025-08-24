using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for RolePermission relationship operations
/// Following SRP: ONLY RolePermission relationship data operations
/// </summary>
public interface IRolePermissionRepository : IBaseRepository<RolePermission>
{
    /// <summary>
    /// Get permissions by role ID
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>Collection of permissions for the role</returns>
    Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(int roleId);
    
    /// <summary>
    /// Get roles by permission ID
    /// </summary>
    /// <param name="permissionId">Permission ID</param>
    /// <returns>Collection of roles that have the permission</returns>
    Task<IEnumerable<Role>> GetRolesByPermissionAsync(int permissionId);
    
    /// <summary>
    /// Assign permissions to a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionIds">Collection of permission IDs to assign</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds);
    
    /// <summary>
    /// Remove permissions from a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionIds">Collection of permission IDs to remove</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> RemovePermissionsAsync(int roleId, IEnumerable<int> permissionIds);
    
    /// <summary>
    /// Check if a role has a specific permission
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionId">Permission ID</param>
    /// <returns>True if role has permission, false otherwise</returns>
    Task<bool> RoleHasPermissionAsync(int roleId, int permissionId);
    
    /// <summary>
    /// Get all roles with their permissions
    /// </summary>
    /// <returns>Collection of roles with permission information</returns>
    Task<IEnumerable<Role>> GetAllRolesWithPermissionsAsync();
    
    /// <summary>
    /// Get role permissions by multiple role IDs
    /// </summary>
    /// <param name="roleIds">Collection of role IDs</param>
    /// <returns>Dictionary mapping role IDs to their permissions</returns>
    Task<Dictionary<int, IEnumerable<Permission>>> GetPermissionsByRolesAsync(IEnumerable<int> roleIds);
}
