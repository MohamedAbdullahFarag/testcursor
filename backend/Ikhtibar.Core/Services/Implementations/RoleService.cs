using AutoMapper;
using Microsoft.Extensions.Logging;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.Core.Services.Implementations;

/// <summary>
/// Service implementation for Role management operations
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
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
        using var scope = _logger.BeginScope("Creating role with code {Code}", createRoleDto.Code);

        _logger.LogInformation("Starting role creation process");

        // Business validation: Check if role code is already in use
        var codeExists = await _roleRepository.IsRoleCodeInUseAsync(createRoleDto.Code);
        if (codeExists)
        {
            _logger.LogWarning("Attempted to create role with existing code: {Code}", createRoleDto.Code);
            throw new InvalidOperationException($"Role code '{createRoleDto.Code}' is already in use");
        }

        try
        {
            // Map DTO to entity
            var role = _mapper.Map<Role>(createRoleDto);
            role.CreatedAt = DateTime.UtcNow;
            role.ModifiedAt = DateTime.UtcNow;

            // Create role through repository
            var createdRole = await _roleRepository.CreateAsync(role);

            _logger.LogInformation("Successfully created role with ID {RoleId} and code {Code}",
                createdRole.RoleId, createdRole.Code);

            return _mapper.Map<RoleDto>(createdRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role with code {Code}", createRoleDto.Code);
            throw;
        }
    }

    public async Task<RoleDto?> GetRoleAsync(int id)
    {
        _logger.LogDebug("Retrieving role with ID {RoleId}", id);

        try
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found", id);
                return null;
            }

            return _mapper.Map<RoleDto>(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role with ID {RoleId}", id);
            throw;
        }
    }

    public async Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto)
    {
        using var scope = _logger.BeginScope("Updating role {RoleId}", id);

        _logger.LogInformation("Starting role update process");

        try
        {
            // Check if role exists
            var existingRole = await _roleRepository.GetByIdAsync(id);
            if (existingRole == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found for update", id);
                return null;
            }

            // Business validation: Check if role name is being updated
            if (!string.IsNullOrEmpty(updateRoleDto.Name) && updateRoleDto.Name != existingRole.Name)
            {
                // For now, we only validate the name change, not the code since UpdateRoleDto doesn't have Code
                _logger.LogDebug("Updating role name from {OldName} to {NewName}", existingRole.Name, updateRoleDto.Name);
            }

            // Map updates to existing entity
            _mapper.Map(updateRoleDto, existingRole);
            existingRole.ModifiedAt = DateTime.UtcNow;

            // Update role through repository
            var updatedRole = await _roleRepository.UpdateAsync(existingRole);

            _logger.LogInformation("Successfully updated role with ID {RoleId}", id);

            return _mapper.Map<RoleDto>(updatedRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role with ID {RoleId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        using var scope = _logger.BeginScope("Deleting role {RoleId}", id);

        _logger.LogInformation("Starting role deletion process");

        try
        {
            // Check if role exists
            var roleExists = await _roleRepository.RoleExistsAsync(id);
            if (!roleExists)
            {
                _logger.LogWarning("Role with ID {RoleId} not found for deletion", id);
                return false;
            }

            // Business validation: Check if it's a system role
            var role = await _roleRepository.GetByIdAsync(id);
            if (role?.IsSystemRole == true)
            {
                _logger.LogWarning("Attempted to delete system role with ID {RoleId}", id);
                throw new InvalidOperationException("Cannot delete system roles");
            }

            // Delete role through repository (soft delete)
            var deleted = await _roleRepository.DeleteAsync(id);

            if (deleted)
            {
                _logger.LogInformation("Successfully deleted role with ID {RoleId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete role with ID {RoleId}", id);
            }

            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role with ID {RoleId}", id);
            throw;
        }
    }

    public async Task<PaginatedResult<RoleDto>> GetAllRolesAsync(int page = 1, int pageSize = 10, bool includeSystemRoles = true)
    {
        _logger.LogDebug("Retrieving paginated roles - Page: {Page}, PageSize: {PageSize}, IncludeSystem: {IncludeSystem}",
            page, pageSize, includeSystemRoles);

        try
        {
            var result = await _roleRepository.GetAllAsync(page, pageSize, includeSystemRoles);

            var rolesDtos = _mapper.Map<IEnumerable<RoleDto>>(result.Roles);

            // Calculate pagination metadata
            var totalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
            var hasNextPage = page < totalPages;
            var hasPreviousPage = page > 1;

            return new PaginatedResult<RoleDto>
            {
                Items = rolesDtos,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paginated roles");
            throw;
        }
    }

    public async Task<RoleDto?> GetRoleByCodeAsync(string code)
    {
        _logger.LogDebug("Retrieving role by code {Code}", code);

        try
        {
            var role = await _roleRepository.GetByCodeAsync(code);
            if (role == null)
            {
                _logger.LogWarning("Role with code {Code} not found", code);
                return null;
            }

            return _mapper.Map<RoleDto>(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role with code {Code}", code);
            throw;
        }
    }

    public async Task<IEnumerable<RoleDto>> GetSystemRolesAsync()
    {
        _logger.LogDebug("Retrieving system roles");

        try
        {
            var systemRoles = await _roleRepository.GetSystemRolesAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(systemRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system roles");
            throw;
        }
    }

    public async Task<IEnumerable<RoleDto>> GetCustomRolesAsync()
    {
        _logger.LogDebug("Retrieving custom roles");

        try
        {
            var customRoles = await _roleRepository.GetCustomRolesAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(customRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving custom roles");
            throw;
        }
    }

    public async Task<bool> RoleExistsAsync(int id)
    {
        _logger.LogDebug("Checking if role exists: {RoleId}", id);
        return await _roleRepository.RoleExistsAsync(id);
    }

    public async Task<bool> IsRoleCodeInUseAsync(string code, int? excludeRoleId = null)
    {
        _logger.LogDebug("Checking if role code is in use: {Code}", code);
        return await _roleRepository.IsRoleCodeInUseAsync(code, excludeRoleId);
    }

    public async Task<int> SeedDefaultRolesAsync()
    {
        _logger.LogInformation("Starting default roles seeding process");

        try
        {
            await _roleRepository.SeedDefaultRolesAsync();

            _logger.LogInformation("Successfully completed default roles seeding");

            return 0; // Since the repository method doesn't return a count, we return 0
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding default roles");
            throw;
        }
    }
}
