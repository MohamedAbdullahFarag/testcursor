using AutoMapper;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Core.Services.Implementations;

/// <summary>
/// Service implementation for user-role relationship operations
/// Following SRP: ONLY user-role business logic
/// </summary>
public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserRoleService> _logger;

    public UserRoleService(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IMapper mapper,
        ILogger<UserRoleService> logger)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Assign a role to a user
    /// </summary>
    public async Task AssignRoleAsync(int userId, int roleId)
    {
        try
        {
            _logger.LogInformation("Assigning role {RoleId} to user {UserId}", roleId, userId);

            // Validate user exists
            if (!await _userRepository.UserExistsAsync(userId))
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            // Validate role exists
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"Role with ID {roleId} not found");
            }

            // Check if user already has this role
            if (await _userRoleRepository.UserHasRoleAsync(userId, roleId))
            {
                _logger.LogInformation("User {UserId} already has role {RoleId}", userId, roleId);
                return;
            }

            // Assign the role
            await _userRoleRepository.AssignRoleAsync(userId, roleId);

            _logger.LogInformation("Role {RoleId} assigned to user {UserId} successfully", roleId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
            throw;
        }
    }

    /// <summary>
    /// Remove a role from a user
    /// </summary>
    public async Task RemoveRoleAsync(int userId, int roleId)
    {
        try
        {
            _logger.LogInformation("Removing role {RoleId} from user {UserId}", roleId, userId);

            // Validate user exists
            if (!await _userRepository.UserExistsAsync(userId))
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            // Validate role exists
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"Role with ID {roleId} not found");
            }

            // Check if user has this role
            if (!await _userRoleRepository.UserHasRoleAsync(userId, roleId))
            {
                _logger.LogInformation("User {UserId} does not have role {RoleId}", userId, roleId);
                return;
            }

            // Remove the role
            await _userRoleRepository.RemoveRoleAsync(userId, roleId);

            _logger.LogInformation("Role {RoleId} removed from user {UserId} successfully", roleId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            throw;
        }
    }

    /// <summary>
    /// Get all roles for a specific user
    /// </summary>
    public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId)
    {
        try
        {
            // Validate user exists
            if (!await _userRepository.UserExistsAsync(userId))
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            var userRoles = await _userRoleRepository.GetUserRolesAsync(userId);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            var roles = new List<RoleDto>();
            foreach (var roleId in roleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role != null)
                {
                    roles.Add(_mapper.Map<RoleDto>(role));
                }
            }

            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get all users for a specific role
    /// </summary>
    public async Task<IEnumerable<UserDto>> GetRoleUsersAsync(int roleId)
    {
        try
        {
            // Validate role exists
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                throw new InvalidOperationException($"Role with ID {roleId} not found");
            }

            var userRoles = await _userRoleRepository.GetRoleUsersAsync(roleId);
            var userIds = userRoles.Select(ur => ur.UserId).ToList();

            var users = new List<UserDto>();
            foreach (var userId in userIds)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    
                    // Get user roles
                    var userRoleIds = await _userRoleRepository.GetUserRolesAsync(userId);
                    var userRoleCodes = new List<string>();
                    foreach (var userRoleId in userRoleIds)
                    {
                        var userRole = await _roleRepository.GetByIdAsync(userRoleId.RoleId);
                        if (userRole != null)
                        {
                            userRoleCodes.Add(userRole.Code);
                        }
                    }
                    userDto.Roles = userRoleCodes;
                    
                    users.Add(userDto);
                }
            }

            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users for role {RoleId}", roleId);
            throw;
        }
    }

    /// <summary>
    /// Check if a user has a specific role
    /// </summary>
    public async Task<bool> UserHasRoleAsync(int userId, int roleId)
    {
        try
        {
            return await _userRoleRepository.UserHasRoleAsync(userId, roleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} has role {RoleId}", userId, roleId);
            throw;
        }
    }

    /// <summary>
    /// Check if a user has a specific role by code
    /// </summary>
    public async Task<bool> UserHasRoleAsync(int userId, string roleCode)
    {
        try
        {
            // Get role by code
            var role = await _roleRepository.GetByCodeAsync(roleCode);
            if (role == null)
            {
                return false;
            }

            return await _userRoleRepository.UserHasRoleAsync(userId, role.RoleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} has role {RoleCode}", userId, roleCode);
            throw;
        }
    }

    /// <summary>
    /// Remove all roles from a user
    /// </summary>
    public async Task RemoveAllUserRolesAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Removing all roles from user {UserId}", userId);

            // Validate user exists
            if (!await _userRepository.UserExistsAsync(userId))
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            await _userRoleRepository.RemoveAllUserRolesAsync(userId);

            _logger.LogInformation("All roles removed from user {UserId} successfully", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing all roles from user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Bulk assign roles to a user
    /// </summary>
    public async Task AssignRolesAsync(int userId, IEnumerable<int> roleIds)
    {
        try
        {
            _logger.LogInformation("Assigning {RoleCount} roles to user {UserId}", roleIds.Count(), userId);

            // Validate user exists
            if (!await _userRepository.UserExistsAsync(userId))
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            foreach (var roleId in roleIds)
            {
                await AssignRoleAsync(userId, roleId);
            }

            _logger.LogInformation("Successfully assigned {RoleCount} roles to user {UserId}", roleIds.Count(), userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning roles to user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Replace all user roles with new ones
    /// </summary>
    public async Task ReplaceUserRolesAsync(int userId, IEnumerable<int> roleIds)
    {
        try
        {
            _logger.LogInformation("Replacing all roles for user {UserId} with {RoleCount} new roles", userId, roleIds.Count());

            // Validate user exists
            if (!await _userRepository.UserExistsAsync(userId))
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            // Remove all existing roles
            await _userRoleRepository.RemoveAllUserRolesAsync(userId);

            // Assign new roles
            foreach (var roleId in roleIds)
            {
                await AssignRoleAsync(userId, roleId);
            }

            _logger.LogInformation("Successfully replaced all roles for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replacing roles for user {UserId}", userId);
            throw;
        }
    }
}
