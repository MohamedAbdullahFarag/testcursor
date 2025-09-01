using AutoMapper;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Core.Services.Implementations;

/// <summary>
/// Service implementation for role management operations
/// Following SRP: ONLY role business logic
/// </summary>
public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        IRoleRepository roleRepository,
        IMapper mapper,
        ILogger<RoleService> logger)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
        try
        {
            _logger.LogInformation("Creating new role with code: {Code}", createRoleDto.Code);

            // Check if role code already exists
            if (await _roleRepository.CodeExistsAsync(createRoleDto.Code))
            {
                throw new InvalidOperationException($"Role code '{createRoleDto.Code}' is already in use");
            }

            // Create role entity
            var role = new Role
            {
                Code = createRoleDto.Code,
                Name = createRoleDto.Name,
                Description = createRoleDto.Description,
                IsActive = createRoleDto.IsActive,
                IsSystemRole = createRoleDto.IsSystemRole,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            // Save role
            var createdRole = await _roleRepository.AddAsync(role);

            _logger.LogInformation("Role created successfully with ID: {RoleId}", createdRole.RoleId);
            return _mapper.Map<RoleDto>(createdRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role with code: {Code}", createRoleDto.Code);
            throw;
        }
    }

    /// <summary>
    /// Get role by ID
    /// </summary>
    public async Task<RoleDto?> GetRoleAsync(int roleId)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null) return null;

            return _mapper.Map<RoleDto>(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role by ID: {RoleId}", roleId);
            throw;
        }
    }

    /// <summary>
    /// Get role by code
    /// </summary>
    public async Task<RoleDto?> GetRoleByCodeAsync(string code)
    {
        try
        {
            var role = await _roleRepository.GetByCodeAsync(code);
            if (role == null) return null;

            return _mapper.Map<RoleDto>(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role by code: {Code}", code);
            throw;
        }
    }

    /// <summary>
    /// Update an existing role
    /// </summary>
    public async Task<RoleDto> UpdateRoleAsync(int roleId, UpdateRoleDto updateRoleDto)
    {
        try
        {
            _logger.LogInformation("Updating role with ID: {RoleId}", roleId);

            var existingRole = await _roleRepository.GetByIdAsync(roleId);
            if (existingRole == null)
            {
                throw new InvalidOperationException($"Role with ID {roleId} not found");
            }

            // Prevent updating system roles
            if (existingRole.IsSystemRole)
            {
                throw new InvalidOperationException($"Cannot update system role '{existingRole.Code}'");
            }

            // Update properties (only if provided)
            if (!string.IsNullOrEmpty(updateRoleDto.Name))
            {
                existingRole.Name = updateRoleDto.Name;
            }
            
            if (updateRoleDto.Description != null)
            {
                existingRole.Description = updateRoleDto.Description;
            }
            
            if (updateRoleDto.IsActive.HasValue)
            {
                existingRole.IsActive = updateRoleDto.IsActive.Value;
            }

            existingRole.ModifiedAt = DateTime.UtcNow;

            // Update role
            var updatedRole = await _roleRepository.UpdateAsync(existingRole);

            _logger.LogInformation("Role updated successfully with ID: {RoleId}", roleId);
            return _mapper.Map<RoleDto>(updatedRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role with ID: {RoleId}", roleId);
            throw;
        }
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    public async Task<bool> DeleteRoleAsync(int roleId)
    {
        try
        {
            _logger.LogInformation("Deleting role with ID: {RoleId}", roleId);

            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found for deletion", roleId);
                return false;
            }

            // Prevent deletion of system roles
            if (role.IsSystemRole)
            {
                throw new InvalidOperationException($"Cannot delete system role '{role.Code}'");
            }

            // Delete role (soft delete)
            var result = await _roleRepository.DeleteAsync(roleId);

            if (result)
            {
                _logger.LogInformation("Role deleted successfully with ID: {RoleId}", roleId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role with ID: {RoleId}", roleId);
            throw;
        }
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        try
        {
            var roles = await _roleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles");
            throw;
        }
    }

    /// <summary>
    /// Get active roles only
    /// </summary>
    public async Task<IEnumerable<RoleDto>> GetActiveRolesAsync()
    {
        try
        {
            var roles = await _roleRepository.GetActiveRolesAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active roles");
            throw;
        }
    }

    /// <summary>
    /// Seed default system roles
    /// </summary>
    public async Task SeedDefaultRolesAsync()
    {
        try
        {
            _logger.LogInformation("Seeding default system roles");

            var defaultRoles = new[]
            {
                new Role
                {
                    Code = "system-admin",
                    Name = "System Administrator",
                    Description = "System administrator with full access",
                    IsActive = true,
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                },
                new Role
                {
                    Code = "ADMIN",
                    Name = "Administrator",
                    Description = "System administrator with full access",
                    IsActive = true,
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                },
                new Role
                {
                    Code = "TEACHER",
                    Name = "Teacher",
                    Description = "Teacher with exam management access",
                    IsActive = true,
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                },
                new Role
                {
                    Code = "STUDENT",
                    Name = "Student",
                    Description = "Student with exam access",
                    IsActive = true,
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                },
                new Role
                {
                    Code = "USER",
                    Name = "User",
                    Description = "Regular system user",
                    IsActive = true,
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                }
            };

            foreach (var role in defaultRoles)
            {
                // Check if role already exists
                var existingRole = await _roleRepository.GetByCodeAsync(role.Code);
                if (existingRole == null)
                {
                    await _roleRepository.AddAsync(role);
                    _logger.LogInformation("Created default role: {Code}", role.Code);
                }
                else
                {
                    _logger.LogDebug("Default role already exists: {Code}", role.Code);
                }
            }

            _logger.LogInformation("Default system roles seeding completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding default system roles");
            throw;
        }
    }

    /// <summary>
    /// Check if role exists
    /// </summary>
    public async Task<bool> RoleExistsAsync(int roleId)
    {
        try
        {
            return await _roleRepository.GetByIdAsync(roleId) != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if role exists: {RoleId}", roleId);
            throw;
        }
    }

    /// <summary>
    /// Check if role code exists
    /// </summary>
    public async Task<bool> CodeExistsAsync(string code, int? excludeRoleId = null)
    {
        try
        {
            return await _roleRepository.CodeExistsAsync(code, excludeRoleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if role code exists: {Code}", code);
            throw;
        }
    }

    public async Task<IEnumerable<RoleDto>> GetSystemRolesAsync()
    {
        try
        {
            var systemRoles = await _roleRepository.GetAllAsync("IsSystemRole = 1");
            return _mapper.Map<IEnumerable<RoleDto>>(systemRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system roles");
            throw;
        }
    }
}
