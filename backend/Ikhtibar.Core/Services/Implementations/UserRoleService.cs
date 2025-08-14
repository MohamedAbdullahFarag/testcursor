using AutoMapper;
using Microsoft.Extensions.Logging;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.Core.Services.Implementations;

/// <summary>
/// Service implementation for User-Role assignment operations
/// Following SRP: ONLY user-role relationship business logic
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
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> AssignRoleToUserAsync(AssignRoleDto assignRoleDto)
    {
        using var scope = _logger.BeginScope("Assigning role {RoleId} to user {UserId}", assignRoleDto.RoleId, assignRoleDto.UserId);

        _logger.LogInformation("Starting role assignment process");

        try
        {
            // Business validation: Check if user exists
            var userExists = await _userRepository.UserExistsAsync(assignRoleDto.UserId);
            if (!userExists)
            {
                _logger.LogWarning("Attempted to assign role to non-existent user: {UserId}", assignRoleDto.UserId);
                throw new InvalidOperationException($"User with ID '{assignRoleDto.UserId}' does not exist");
            }

            // Business validation: Check if role exists
            var roleExists = await _roleRepository.RoleExistsAsync(assignRoleDto.RoleId);
            if (!roleExists)
            {
                _logger.LogWarning("Attempted to assign non-existent role: {RoleId}", assignRoleDto.RoleId);
                throw new InvalidOperationException($"Role with ID '{assignRoleDto.RoleId}' does not exist");
            }

            // Business validation: Check if assignment already exists
            var alreadyAssigned = await _userRoleRepository.UserHasRoleAsync(assignRoleDto.UserId, assignRoleDto.RoleId);
            if (alreadyAssigned)
            {
                _logger.LogInformation("User {UserId} already has role {RoleId}", assignRoleDto.UserId, assignRoleDto.RoleId);
                return true; // Consider this a success (idempotent operation)
            }

            // Perform assignment
            await _userRoleRepository.AssignRoleAsync(assignRoleDto.UserId, assignRoleDto.RoleId);

            _logger.LogInformation("Successfully assigned role {RoleId} to user {UserId}", assignRoleDto.RoleId, assignRoleDto.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign role {RoleId} to user {UserId}", assignRoleDto.RoleId, assignRoleDto.UserId);
            throw;
        }
    }

    public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
    {
        using var scope = _logger.BeginScope("Removing role {RoleId} from user {UserId}", roleId, userId);

        _logger.LogInformation("Starting role removal process");

        try
        {
            // Business validation: Check if assignment exists
            var hasRole = await _userRoleRepository.UserHasRoleAsync(userId, roleId);
            if (!hasRole)
            {
                _logger.LogInformation("User {UserId} does not have role {RoleId}", userId, roleId);
                return true; // Consider this a success (idempotent operation)
            }

            // Perform removal
            await _userRoleRepository.RemoveRoleAsync(userId, roleId);

            _logger.LogInformation("Successfully removed role {RoleId} from user {UserId}", roleId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove role {RoleId} from user {UserId}", roleId, userId);
            throw;
        }
    }

    public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId)
    {
        using var scope = _logger.BeginScope("Getting roles for user {UserId}", userId);

        _logger.LogInformation("Retrieving user roles");

        try
        {
            // Business validation: Check if user exists
            var userExists = await _userRepository.UserExistsAsync(userId);
            if (!userExists)
            {
                _logger.LogWarning("Attempted to get roles for non-existent user: {UserId}", userId);
                throw new InvalidOperationException($"User with ID '{userId}' does not exist");
            }

            // Get user roles
            var roles = await _userRoleRepository.GetUserRolesAsync(userId);

            // Map to DTOs
            var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);

            _logger.LogInformation("Successfully retrieved {Count} roles for user {UserId}", roleDtos.Count(), userId);
            return roleDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get roles for user {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<UserDto>> GetRoleUsersAsync(int roleId)
    {
        using var scope = _logger.BeginScope("Getting users for role {RoleId}", roleId);

        _logger.LogInformation("Retrieving role users");

        try
        {
            // Business validation: Check if role exists
            var roleExists = await _roleRepository.RoleExistsAsync(roleId);
            if (!roleExists)
            {
                _logger.LogWarning("Attempted to get users for non-existent role: {RoleId}", roleId);
                throw new InvalidOperationException($"Role with ID '{roleId}' does not exist");
            }

            // Get role users
            var users = await _userRoleRepository.GetRoleUsersAsync(roleId);

            // Map to DTOs
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            _logger.LogInformation("Successfully retrieved {Count} users for role {RoleId}", userDtos.Count(), roleId);
            return userDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get users for role {RoleId}", roleId);
            throw;
        }
    }

    public async Task<bool> UserHasRoleAsync(int userId, int roleId)
    {
        using var scope = _logger.BeginScope("Checking if user {UserId} has role {RoleId}", userId, roleId);

        try
        {
            var hasRole = await _userRoleRepository.UserHasRoleAsync(userId, roleId);

            _logger.LogInformation("User {UserId} {HasRole} role {RoleId}", userId, hasRole ? "has" : "does not have", roleId);
            return hasRole;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if user {UserId} has role {RoleId}", userId, roleId);
            throw;
        }
    }

    public async Task<bool> UserHasRoleAsync(int userId, string roleCode)
    {
        using var scope = _logger.BeginScope("Checking if user {UserId} has role with code {RoleCode}", userId, roleCode);

        try
        {
            var hasRole = await _userRoleRepository.UserHasRoleAsync(userId, roleCode);

            _logger.LogInformation("User {UserId} {HasRole} role {RoleCode}", userId, hasRole ? "has" : "does not have", roleCode);
            return hasRole;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if user {UserId} has role {RoleCode}", userId, roleCode);
            throw;
        }
    }

    public async Task<int> RemoveAllUserRolesAsync(int userId)
    {
        using var scope = _logger.BeginScope("Removing all roles from user {UserId}", userId);

        _logger.LogInformation("Starting removal of all user roles");

        try
        {
            // Business validation: Check if user exists
            var userExists = await _userRepository.UserExistsAsync(userId);
            if (!userExists)
            {
                _logger.LogWarning("Attempted to remove roles from non-existent user: {UserId}", userId);
                throw new InvalidOperationException($"User with ID '{userId}' does not exist");
            }

            // Get current roles count for logging
            var currentRoles = await _userRoleRepository.GetUserRolesAsync(userId);
            var roleCount = currentRoles.Count();

            // Remove all roles
            await _userRoleRepository.RemoveAllUserRolesAsync(userId);

            _logger.LogInformation("Successfully removed {Count} roles from user {UserId}", roleCount, userId);
            return roleCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove all roles from user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds)
    {
        using var scope = _logger.BeginScope("Updating roles for user {UserId}", userId);

        _logger.LogInformation("Starting user roles update with {Count} roles", roleIds.Count());

        try
        {
            // Business validation: Check if user exists
            var userExists = await _userRepository.UserExistsAsync(userId);
            if (!userExists)
            {
                _logger.LogWarning("Attempted to update roles for non-existent user: {UserId}", userId);
                throw new InvalidOperationException($"User with ID '{userId}' does not exist");
            }

            // Business validation: Check if all roles exist
            foreach (var roleId in roleIds)
            {
                var roleExists = await _roleRepository.RoleExistsAsync(roleId);
                if (!roleExists)
                {
                    _logger.LogWarning("Attempted to assign non-existent role: {RoleId}", roleId);
                    throw new InvalidOperationException($"Role with ID '{roleId}' does not exist");
                }
            }

            // Remove all current roles
            await _userRoleRepository.RemoveAllUserRolesAsync(userId);

            // Assign new roles
            foreach (var roleId in roleIds)
            {
                await _userRoleRepository.AssignRoleAsync(userId, roleId);
            }

            _logger.LogInformation("Successfully updated user {UserId} with {Count} roles", userId, roleIds.Count());
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update roles for user {UserId}", userId);
            throw;
        }
    }

    public async Task<PaginatedResult<UserRoleSummaryDto>> GetUsersWithRolesAsync(int page = 1, int pageSize = 10)
    {
        using var scope = _logger.BeginScope("Getting users with roles - page {Page}, size {PageSize}", page, pageSize);

        _logger.LogInformation("Retrieving users with role summary");

        try
        {
            // Get paginated users
            var (users, totalCount) = await _userRepository.GetAllAsync(page, pageSize);

            // Build summary for each user
            var userSummaries = new List<UserRoleSummaryDto>();

            foreach (var user in users)
            {
                var userRoles = await _userRoleRepository.GetUserRolesAsync(user.UserId);

                var summary = new UserRoleSummaryDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    RoleCount = userRoles.Count(),
                    RoleNames = userRoles.Select(r => r.Name)
                };

                userSummaries.Add(summary);
            }

            var result = new PaginatedResult<UserRoleSummaryDto>
            {
                Items = userSummaries,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            _logger.LogInformation("Successfully retrieved {Count} users with role summaries", userSummaries.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get users with roles summary");
            throw;
        }
    }
}
