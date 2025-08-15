using AutoMapper;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Service implementation for permission management operations
/// Following SRP: ONLY permission business logic
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IUserRoleRepository userRoleRepository,
        IMapper mapper,
        ILogger<PermissionService> logger)
    {
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _userRoleRepository = userRoleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all permissions
    /// </summary>
    public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
    {
        try
        {
            _logger.LogDebug("Retrieving all permissions");
            var permissions = await _permissionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all permissions");
            throw;
        }
    }

    /// <summary>
    /// Get permissions by role ID
    /// </summary>
    public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(int roleId)
    {
        try
        {
            _logger.LogDebug("Retrieving permissions for role ID: {RoleId}", roleId);
            var permissions = await _rolePermissionRepository.GetPermissionsByRoleAsync(roleId);
            return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for role ID: {RoleId}", roleId);
            throw;
        }
    }

    /// <summary>
    /// Get permissions by user ID
    /// </summary>
    public async Task<IEnumerable<PermissionDto>> GetPermissionsByUserAsync(int userId)
    {
        try
        {
            _logger.LogDebug("Retrieving permissions for user ID: {UserId}", userId);
            
            // Get user roles first
            var userRoles = await _userRoleRepository.GetUserRolesAsync(userId);
            var allPermissions = new HashSet<PermissionDto>();
            
            // Collect permissions from all user roles
            foreach (var role in userRoles)
            {
                var rolePermissions = await GetPermissionsByRoleAsync(role.RoleId);
                foreach (var permission in rolePermissions)
                {
                    allPermissions.Add(permission);
                }
            }
            
            return allPermissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for user ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Assign permissions to a role
    /// </summary>
    public async Task<bool> AssignPermissionsToRoleAsync(int roleId, IEnumerable<int> permissionIds)
    {
        try
        {
            _logger.LogInformation("Assigning {PermissionCount} permissions to role ID: {RoleId}", 
                permissionIds.Count(), roleId);
            
            var result = await _rolePermissionRepository.AssignPermissionsAsync(roleId, permissionIds);
            
            if (result)
            {
                _logger.LogInformation("Successfully assigned permissions to role ID: {RoleId}", roleId);
            }
            else
            {
                _logger.LogWarning("Failed to assign permissions to role ID: {RoleId}", roleId);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permissions to role ID: {RoleId}", roleId);
            throw;
        }
    }

    /// <summary>
    /// Remove permissions from a role
    /// </summary>
    public async Task<bool> RemovePermissionsFromRoleAsync(int roleId, IEnumerable<int> permissionIds)
    {
        try
        {
            _logger.LogInformation("Removing {PermissionCount} permissions from role ID: {RoleId}", 
                permissionIds.Count(), roleId);
            
            var result = await _rolePermissionRepository.RemovePermissionsAsync(roleId, permissionIds);
            
            if (result)
            {
                _logger.LogInformation("Successfully removed permissions from role ID: {RoleId}", roleId);
            }
            else
            {
                _logger.LogWarning("Failed to remove permissions from role ID: {RoleId}", roleId);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing permissions from role ID: {RoleId}", roleId);
            throw;
        }
    }

    /// <summary>
    /// Check if a user has a specific permission
    /// </summary>
    public async Task<bool> UserHasPermissionAsync(int userId, string permissionCode)
    {
        try
        {
            _logger.LogDebug("Checking if user ID: {UserId} has permission: {PermissionCode}", 
                userId, permissionCode);
            
            // Get user permissions
            var userPermissions = await GetPermissionsByUserAsync(userId);
            
            // Check if the specific permission exists
            var hasPermission = userPermissions.Any(p => p.Code == permissionCode);
            
            _logger.LogDebug("User ID: {UserId} has permission {PermissionCode}: {HasPermission}", 
                userId, permissionCode, hasPermission);
            
            return hasPermission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission {PermissionCode} for user ID: {UserId}", 
                permissionCode, userId);
            return false;
        }
    }

    /// <summary>
    /// Check if a role has a specific permission
    /// </summary>
    public async Task<bool> RoleHasPermissionAsync(int roleId, string permissionCode)
    {
        try
        {
            _logger.LogDebug("Checking if role ID: {RoleId} has permission: {PermissionCode}", 
                roleId, permissionCode);
            
            // Get role permissions
            var rolePermissions = await GetPermissionsByRoleAsync(roleId);
            
            // Check if the specific permission exists
            var hasPermission = rolePermissions.Any(p => p.Code == permissionCode);
            
            _logger.LogDebug("Role ID: {RoleId} has permission {PermissionCode}: {HasPermission}", 
                roleId, permissionCode, hasPermission);
            
            return hasPermission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission {PermissionCode} for role ID: {RoleId}", 
                permissionCode, roleId);
            return false;
        }
    }

    /// <summary>
    /// Get permission matrix for all roles
    /// </summary>
    public async Task<PermissionMatrix> GetPermissionMatrixAsync()
    {
        try
        {
            _logger.LogDebug("Retrieving permission matrix for all roles");
            
            var matrix = new PermissionMatrix();
            
            // Get all permissions
            matrix.Permissions = await GetAllPermissionsAsync();
            
            // Get all roles with their permissions
            var roles = await _rolePermissionRepository.GetAllRolesWithPermissionsAsync();
            var rolePermissionInfos = new List<RolePermissionInfo>();
            
            foreach (var role in roles)
            {
                var rolePermissions = await GetPermissionsByRoleAsync(role.RoleId);
                var permissionIds = rolePermissions.Select(p => p.PermissionId).ToHashSet();
                
                rolePermissionInfos.Add(new RolePermissionInfo
                {
                    RoleId = role.RoleId,
                    Code = role.Code,
                    Name = role.Name,
                    IsSystemRole = role.IsSystemRole,
                    PermissionIds = permissionIds
                });
                
                // Add to matrix
                matrix.Matrix[role.RoleId] = permissionIds;
            }
            
            matrix.Roles = rolePermissionInfos;
            
            _logger.LogDebug("Successfully retrieved permission matrix with {RoleCount} roles and {PermissionCount} permissions", 
                matrix.Roles.Count(), matrix.Permissions.Count());
            
            return matrix;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permission matrix");
            throw;
        }
    }

    /// <summary>
    /// Seed default permissions
    /// </summary>
    public async Task SeedDefaultPermissionsAsync()
    {
        try
        {
            _logger.LogInformation("Seeding default permissions");
            
            // Define default permissions for the educational assessment platform
            var defaultPermissions = new[]
            {
                // User Management
                new Permission { Code = "user.create", Name = "Create User", Description = "Create new users", Category = "User Management" },
                new Permission { Code = "user.read", Name = "Read User", Description = "View user information", Category = "User Management" },
                new Permission { Code = "user.update", Name = "Update User", Description = "Modify user information", Category = "User Management" },
                new Permission { Code = "user.delete", Name = "Delete User", Description = "Remove users", Category = "User Management" },
                
                // Role Management
                new Permission { Code = "role.create", Name = "Create Role", Description = "Create new roles", Category = "Role Management" },
                new Permission { Code = "role.read", Name = "Read Role", Description = "View role information", Category = "Role Management" },
                new Permission { Code = "role.update", Name = "Update Role", Description = "Modify role information", Category = "Role Management" },
                new Permission { Code = "role.delete", Name = "Delete Role", Description = "Remove roles", Category = "Role Management" },
                
                // Question Management
                new Permission { Code = "question.create", Name = "Create Question", Description = "Create new questions", Category = "Question Management" },
                new Permission { Code = "question.read", Name = "Read Question", Description = "View questions", Category = "Question Management" },
                new Permission { Code = "question.update", Name = "Update Question", Description = "Modify questions", Category = "Question Management" },
                new Permission { Code = "question.delete", Name = "Delete Question", Description = "Remove questions", Category = "Question Management" },
                new Permission { Code = "question.review", Name = "Review Question", Description = "Review and approve questions", Category = "Question Management" },
                
                // Exam Management
                new Permission { Code = "exam.create", Name = "Create Exam", Description = "Create new exams", Category = "Exam Management" },
                new Permission { Code = "exam.read", Name = "Read Exam", Description = "View exam information", Category = "Exam Management" },
                new Permission { Code = "exam.update", Name = "Update Exam", Description = "Modify exam information", Category = "Exam Management" },
                new Permission { Code = "exam.delete", Name = "Delete Exam", Description = "Remove exams", Category = "Exam Management" },
                new Permission { Code = "exam.schedule", Name = "Schedule Exam", Description = "Schedule exam sessions", Category = "Exam Management" },
                new Permission { Code = "exam.supervise", Name = "Supervise Exam", Description = "Supervise exam sessions", Category = "Exam Management" },
                
                // Grading
                new Permission { Code = "grade.read", Name = "Read Grades", Description = "View exam grades", Category = "Grading" },
                new Permission { Code = "grade.update", Name = "Update Grades", Description = "Modify exam grades", Category = "Grading" },
                new Permission { Code = "grade.approve", Name = "Approve Grades", Description = "Approve final grades", Category = "Grading" },
                
                // Reports and Analytics
                new Permission { Code = "report.read", Name = "Read Reports", Description = "View system reports", Category = "Reports" },
                new Permission { Code = "report.create", Name = "Create Reports", Description = "Generate custom reports", Category = "Reports" },
                new Permission { Code = "analytics.read", Name = "Read Analytics", Description = "View analytics data", Category = "Analytics" },
                
                // System Administration
                new Permission { Code = "system.config", Name = "System Configuration", Description = "Configure system settings", Category = "System Administration" },
                new Permission { Code = "system.audit", Name = "System Audit", Description = "Access audit logs", Category = "System Administration" },
                new Permission { Code = "system.backup", Name = "System Backup", Description = "Perform system backups", Category = "System Administration" }
            };
            
            foreach (var permission in defaultPermissions)
            {
                if (!await _permissionRepository.CodeExistsAsync(permission.Code))
                {
                    await _permissionRepository.AddAsync(permission);
                    _logger.LogDebug("Created default permission: {PermissionCode}", permission.Code);
                }
            }
            
            _logger.LogInformation("Default permissions seeding completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding default permissions");
            throw;
        }
    }
}
