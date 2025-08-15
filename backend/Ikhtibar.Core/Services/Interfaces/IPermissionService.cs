using Ikhtibar.Core.DTOs;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for permission management operations
/// Following SRP: ONLY permission business logic
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Get all permissions
    /// </summary>
    /// <returns>Collection of permission DTOs</returns>
    Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
    
    /// <summary>
    /// Get permissions by role ID
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>Collection of permission DTOs for the role</returns>
    Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(int roleId);
    
    /// <summary>
    /// Get permissions by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of permission DTOs for the user</returns>
    Task<IEnumerable<PermissionDto>> GetPermissionsByUserAsync(int userId);
    
    /// <summary>
    /// Assign permissions to a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionIds">Collection of permission IDs to assign</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> AssignPermissionsToRoleAsync(int roleId, IEnumerable<int> permissionIds);
    
    /// <summary>
    /// Remove permissions from a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionIds">Collection of permission IDs to remove</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> RemovePermissionsFromRoleAsync(int roleId, IEnumerable<int> permissionIds);
    
    /// <summary>
    /// Check if a user has a specific permission
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="permissionCode">Permission code to check</param>
    /// <returns>True if user has permission, false otherwise</returns>
    Task<bool> UserHasPermissionAsync(int userId, string permissionCode);
    
    /// <summary>
    /// Check if a role has a specific permission
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionCode">Permission code to check</param>
    /// <returns>True if role has permission, false otherwise</returns>
    Task<bool> RoleHasPermissionAsync(int roleId, string permissionCode);
    
    /// <summary>
    /// Get permission matrix for all roles
    /// </summary>
    /// <returns>Permission matrix showing which roles have which permissions</returns>
    Task<PermissionMatrix> GetPermissionMatrixAsync();
    
    /// <summary>
    /// Seed default permissions
    /// </summary>
    /// <returns>Task</returns>
    Task SeedDefaultPermissionsAsync();
}

/// <summary>
/// Permission matrix showing role-permission relationships
/// </summary>
public class PermissionMatrix
{
    /// <summary>
    /// Collection of roles with their permissions
    /// </summary>
    public IEnumerable<RolePermissionInfo> Roles { get; set; } = new List<RolePermissionInfo>();
    
    /// <summary>
    /// Collection of all available permissions
    /// </summary>
    public IEnumerable<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    
    /// <summary>
    /// Matrix data showing role-permission assignments
    /// </summary>
    public Dictionary<int, HashSet<int>> Matrix { get; set; } = new();
}

/// <summary>
/// Role information with permission details
/// </summary>
public class RolePermissionInfo
{
    /// <summary>
    /// Role ID
    /// </summary>
    public int RoleId { get; set; }
    
    /// <summary>
    /// Role code
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Role name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the role is a system role
    /// </summary>
    public bool IsSystemRole { get; set; }
    
    /// <summary>
    /// Collection of permission IDs assigned to this role
    /// </summary>
    public IEnumerable<int> PermissionIds { get; set; } = new List<int>();
}
