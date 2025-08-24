using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.DTOs.Roles;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Comprehensive service interface for role-based access control (RBAC) management.
/// Provides operations for roles, permissions, and user-role assignments.
/// </summary>
public interface IRoleManagementService
{
    #region Role Management

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="role">The role to create</param>
    /// <returns>The created role with generated ID</returns>
    Task<Role> CreateRoleAsync(Role role);

    /// <summary>
    /// Gets a role by its ID.
    /// </summary>
    /// <param name="id">The role ID</param>
    /// <returns>The role if found, null otherwise</returns>
    Task<Role?> GetRoleByIdAsync(int id);

    /// <summary>
    /// Gets a role by its name.
    /// </summary>
    /// <param name="name">The role name</param>
    /// <returns>The role if found, null otherwise</returns>
    Task<Role?> GetRoleByNameAsync(string name);

    /// <summary>
    /// Gets all roles.
    /// </summary>
    /// <returns>Collection of all roles</returns>
    Task<IEnumerable<Role>> GetAllRolesAsync();

    /// <summary>
    /// Gets all active roles.
    /// </summary>
    /// <returns>Collection of active roles</returns>
    Task<IEnumerable<Role>> GetActiveRolesAsync();

    /// <summary>
    /// Updates an existing role.
    /// </summary>
    /// <param name="role">The updated role data</param>
    /// <returns>The updated role</returns>
    Task<Role> UpdateRoleAsync(Role role);

    /// <summary>
    /// Deletes a role.
    /// </summary>
    /// <param name="id">The role ID to delete</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteRoleAsync(int id);

    /// <summary>
    /// Activates a role.
    /// </summary>
    /// <param name="id">The role ID to activate</param>
    /// <returns>True if activation was successful</returns>
    Task<bool> ActivateRoleAsync(int id);

    /// <summary>
    /// Deactivates a role.
    /// </summary>
    /// <param name="id">The role ID to deactivate</param>
    /// <returns>True if deactivation was successful</returns>
    Task<bool> DeactivateRoleAsync(int id);

    #endregion

    #region Permission Management

    /// <summary>
    /// Creates a new permission.
    /// </summary>
    /// <param name="permission">The permission to create</param>
    /// <returns>The created permission with generated ID</returns>
    Task<Permission> CreatePermissionAsync(Permission permission);

    /// <summary>
    /// Gets a permission by its ID.
    /// </summary>
    /// <param name="id">The permission ID</param>
    /// <returns>The permission if found, null otherwise</returns>
    Task<Permission?> GetPermissionByIdAsync(int id);

    /// <summary>
    /// Gets a permission by its name.
    /// </summary>
    /// <param name="name">The permission name</param>
    /// <returns>The permission if found, null otherwise</returns>
    Task<Permission?> GetPermissionByNameAsync(string name);

    /// <summary>
    /// Gets all permissions.
    /// </summary>
    /// <returns>Collection of all permissions</returns>
    Task<IEnumerable<Permission>> GetAllPermissionsAsync();

    /// <summary>
    /// Gets all active permissions.
    /// </summary>
    /// <returns>Collection of active permissions</returns>
    Task<IEnumerable<Permission>> GetActivePermissionsAsync();

    /// <summary>
    /// Updates an existing permission.
    /// </summary>
    /// <param name="permission">The updated permission data</param>
    /// <returns>The updated permission</returns>
    Task<Permission> UpdatePermissionAsync(Permission permission);

    /// <summary>
    /// Deletes a permission.
    /// </summary>
    /// <param name="id">The permission ID to delete</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeletePermissionAsync(int id);

    #endregion

    #region Role-Permission Management

    /// <summary>
    /// Assigns permissions to a role.
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="permissionIds">Collection of permission IDs to assign</param>
    /// <returns>True if assignment was successful</returns>
    Task<bool> AssignPermissionsToRoleAsync(int roleId, IEnumerable<int> permissionIds);

    /// <summary>
    /// Removes permissions from a role.
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="permissionIds">Collection of permission IDs to remove</param>
    /// <returns>True if removal was successful</returns>
    Task<bool> RemovePermissionsFromRoleAsync(int roleId, IEnumerable<int> permissionIds);

    /// <summary>
    /// Gets all permissions assigned to a role.
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <returns>Collection of assigned permissions</returns>
    Task<IEnumerable<Permission>> GetPermissionsForRoleAsync(int roleId);

    /// <summary>
    /// Gets all roles that have a specific permission.
    /// </summary>
    /// <param name="permissionId">The permission ID</param>
    /// <returns>Collection of roles with the permission</returns>
    Task<IEnumerable<Role>> GetRolesWithPermissionAsync(int permissionId);

    /// <summary>
    /// Checks if a role has a specific permission.
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="permissionId">The permission ID</param>
    /// <returns>True if the role has the permission</returns>
    Task<bool> RoleHasPermissionAsync(int roleId, int permissionId);

    #endregion

    #region User-Role Management

    /// <summary>
    /// Assigns roles to a user.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="roleIds">Collection of role IDs to assign</param>
    /// <returns>True if assignment was successful</returns>
    Task<bool> AssignRolesToUserAsync(int userId, IEnumerable<int> roleIds);

    /// <summary>
    /// Removes roles from a user.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="roleIds">Collection of role IDs to remove</param>
    /// <returns>True if removal was successful</returns>
    Task<bool> RemoveRolesFromUserAsync(int userId, IEnumerable<int> roleIds);

    /// <summary>
    /// Gets all roles assigned to a user.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Collection of assigned roles</returns>
    Task<IEnumerable<Role>> GetRolesForUserAsync(int userId);

    /// <summary>
    /// Gets all users that have a specific role.
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <returns>Collection of users with the role</returns>
    Task<IEnumerable<User>> GetUsersWithRoleAsync(int roleId);

    /// <summary>
    /// Checks if a user has a specific role.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="roleId">The role ID</param>
    /// <returns>True if the user has the role</returns>
    Task<bool> UserHasRoleAsync(int userId, int roleId);

    /// <summary>
    /// Gets all permissions for a user (aggregated from all user roles).
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Collection of user permissions</returns>
    Task<IEnumerable<Permission>> GetPermissionsForUserAsync(int userId);

    /// <summary>
    /// Checks if a user has a specific permission.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="permissionId">The permission ID</param>
    /// <returns>True if the user has the permission</returns>
    Task<bool> UserHasPermissionAsync(int userId, int permissionId);

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Assigns multiple roles to multiple users.
    /// </summary>
    /// <param name="assignments">Collection of user-role assignments</param>
    /// <returns>True if all assignments were successful</returns>
    Task<bool> BulkAssignRolesAsync(IEnumerable<UserRoleAssignmentDto> assignments);

    /// <summary>
    /// Assigns multiple permissions to multiple roles.
    /// </summary>
    /// <param name="assignments">Collection of role-permission assignments</param>
    /// <returns>True if all assignments were successful</returns>
    Task<bool> BulkAssignPermissionsAsync(IEnumerable<RolePermissionAssignmentDto> assignments);

    #endregion

    #region Validation & Security

    /// <summary>
    /// Validates if a role can be deleted.
    /// </summary>
    /// <param name="id">The role ID to validate</param>
    /// <returns>Validation result with details</returns>
    Task<(bool CanDelete, string Reason)> ValidateRoleDeletionAsync(int id);

    /// <summary>
    /// Validates if a permission can be deleted.
    /// </summary>
    /// <param name="id">The permission ID to validate</param>
    /// <returns>Validation result with details</returns>
    Task<(bool CanDelete, string Reason)> ValidatePermissionDeletionAsync(int id);

    /// <summary>
    /// Checks for permission conflicts in role assignments.
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="permissionIds">Collection of permission IDs to check</param>
    /// <returns>Collection of conflicts if any</returns>
    Task<IEnumerable<PermissionConflictDto>> CheckPermissionConflictsAsync(int roleId, IEnumerable<int> permissionIds);

    #endregion

    #region Reporting & Analytics

    /// <summary>
    /// Gets role usage statistics.
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <returns>Role usage statistics</returns>
    Task<RoleUsageStatisticsDto> GetRoleUsageStatisticsAsync(int roleId);

    /// <summary>
    /// Gets permission usage statistics.
    /// </summary>
    /// <param name="permissionId">The permission ID</param>
    /// <returns>Permission usage statistics</returns>
    Task<PermissionUsageStatisticsDto> GetPermissionUsageStatisticsAsync(int permissionId);

    /// <summary>
    /// Gets comprehensive RBAC statistics.
    /// </summary>
    /// <returns>RBAC system statistics</returns>
    Task<RbacStatisticsDto> GetRbacStatisticsAsync();

    #endregion
}
