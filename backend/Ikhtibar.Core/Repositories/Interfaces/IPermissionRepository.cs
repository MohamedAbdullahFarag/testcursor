using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for Permission entity operations
/// Following SRP: ONLY Permission data operations
/// </summary>
public interface IPermissionRepository : IBaseRepository<Permission>
{
    /// <summary>
    /// Get permission by code
    /// </summary>
    /// <param name="code">Permission code</param>
    /// <returns>Permission if found, null otherwise</returns>
    Task<Permission?> GetByCodeAsync(string code);
    
    /// <summary>
    /// Get permissions by category
    /// </summary>
    /// <param name="category">Permission category</param>
    /// <returns>Collection of permissions in the category</returns>
    Task<IEnumerable<Permission>> GetByCategoryAsync(string category);
    
    /// <summary>
    /// Get active permissions only
    /// </summary>
    /// <returns>Collection of active permissions</returns>
    Task<IEnumerable<Permission>> GetActivePermissionsAsync();
    
    /// <summary>
    /// Get system permissions only
    /// </summary>
    /// <returns>Collection of system permissions</returns>
    Task<IEnumerable<Permission>> GetSystemPermissionsAsync();
    
    /// <summary>
    /// Check if permission code exists
    /// </summary>
    /// <param name="code">Permission code to check</param>
    /// <param name="excludePermissionId">Permission ID to exclude from check</param>
    /// <returns>True if permission code exists, false otherwise</returns>
    Task<bool> CodeExistsAsync(string code, int? excludePermissionId = null);
    
    /// <summary>
    /// Get permissions by multiple IDs
    /// </summary>
    /// <param name="permissionIds">Collection of permission IDs</param>
    /// <returns>Collection of permissions</returns>
    Task<IEnumerable<Permission>> GetByIdsAsync(IEnumerable<int> permissionIds);
}
