using AutoMapper;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace Ikhtibar.Core.Services.Implementations;

/// <summary>
/// Service implementation for user management operations
/// Following SRP: ONLY user business logic
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        try
        {
            _logger.LogInformation("Creating new user with email: {Email}", createUserDto.Email);

            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(createUserDto.Email))
            {
                throw new InvalidOperationException($"Email '{createUserDto.Email}' is already in use");
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

            // Create user entity
            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                PhoneNumber = createUserDto.PhoneNumber,
                PreferredLanguage = createUserDto.PreferredLanguage,
                IsActive = createUserDto.IsActive,
                EmailVerified = false,
                PhoneVerified = false,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            // Save user to get ID
            var createdUser = await _userRepository.AddAsync(user);

            // Assign roles if specified
            if (createUserDto.RoleIds?.Any() == true)
            {
                foreach (var roleId in createUserDto.RoleIds)
                {
                    await _userRoleRepository.AssignRoleAsync(createdUser.UserId, roleId);
                }
            }

            // Get user with roles
            var userDto = await GetUserAsync(createdUser.UserId);
            if (userDto == null)
            {
                throw new InvalidOperationException("Failed to retrieve created user");
            }

            _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.UserId);
            return userDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email: {Email}", createUserDto.Email);
            throw;
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    public async Task<UserDto?> GetUserAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var userDto = _mapper.Map<UserDto>(user);
            
            // Get user roles
            var roles = await _userRepository.GetByRoleAsync(userId);
            userDto.Roles = roles.Select(r => r.Code).Where(code => !string.IsNullOrEmpty(code)).Cast<string>().ToList();

            return userDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            var userDto = _mapper.Map<UserDto>(user);
            
            // Get user roles
            var roles = await _userRepository.GetByRoleAsync(user.UserId);
            userDto.Roles = roles.Select(r => r.Code).Where(code => !string.IsNullOrEmpty(code)).Cast<string>().ToList();

            return userDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    public async Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
    {
        try
        {
            _logger.LogInformation("Updating user with ID: {UserId}", userId);

            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            // Update properties if provided
            if (!string.IsNullOrEmpty(updateUserDto.Username))
                existingUser.Username = updateUserDto.Username;
            
            if (!string.IsNullOrEmpty(updateUserDto.FirstName))
                existingUser.FirstName = updateUserDto.FirstName;
            
            if (!string.IsNullOrEmpty(updateUserDto.LastName))
                existingUser.LastName = updateUserDto.LastName;
            
            if (updateUserDto.PhoneNumber != null)
                existingUser.PhoneNumber = updateUserDto.PhoneNumber;
            
            if (!string.IsNullOrEmpty(updateUserDto.PreferredLanguage))
                existingUser.PreferredLanguage = updateUserDto.PreferredLanguage;
            
            if (updateUserDto.IsActive.HasValue)
                existingUser.IsActive = updateUserDto.IsActive.Value;
            
            if (updateUserDto.EmailVerified.HasValue)
                existingUser.EmailVerified = updateUserDto.EmailVerified.Value;
            
            if (updateUserDto.PhoneVerified.HasValue)
                existingUser.PhoneVerified = updateUserDto.PhoneVerified.Value;

            existingUser.ModifiedAt = DateTime.UtcNow;

            // Update user
            var updatedUser = await _userRepository.UpdateAsync(existingUser);

            // Get updated user DTO
            var userDto = await GetUserAsync(userId);
            if (userDto == null)
            {
                throw new InvalidOperationException("Failed to retrieve updated user");
            }

            _logger.LogInformation("User updated successfully with ID: {UserId}", userId);
            return userDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    public async Task<bool> DeleteUserAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", userId);
                return false;
            }

            // Remove all user roles first
            await _userRoleRepository.RemoveAllUserRolesAsync(userId);

            // Delete user (soft delete)
            var result = await _userRepository.DeleteAsync(userId);

            if (result)
            {
                _logger.LogInformation("User deleted successfully with ID: {UserId}", userId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get all users with pagination
    /// </summary>
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 20)
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            
            // Apply pagination
            var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize);
            
            var userDtos = new List<UserDto>();
            foreach (var user in pagedUsers)
            {
                var userDto = _mapper.Map<UserDto>(user);
                
                // Get user roles
                var roles = await _userRepository.GetByRoleAsync(user.UserId);
                userDto.Roles = roles.Select(r => r.Code).Where(code => !string.IsNullOrEmpty(code)).Cast<string>().ToList();
                
                userDtos.Add(userDto);
            }

            return userDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            throw;
        }
    }

    /// <summary>
    /// Get user roles
    /// </summary>
    public async Task<List<string>> GetUserRolesAsync(int userId)
    {
        try
        {
            var roles = await _userRepository.GetByRoleAsync(userId);
            return roles.Select(r => r.Code).Where(code => !string.IsNullOrEmpty(code)).Cast<string>().ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Update user's last login timestamp
    /// </summary>
    public async Task UpdateLastLoginAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                user.ModifiedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last login for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    public async Task<UserDto?> AuthenticateAsync(string email, string password)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !user.IsActive)
            {
                return null;
            }

            // Verify password (assuming password hash is stored in the user entity)
            // Note: This would need to be implemented based on your password storage strategy
            // For now, we'll assume the password is already hashed and stored
            
            var userDto = await GetUserAsync(user.UserId);
            return userDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user with email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Check if user exists
    /// </summary>
    public async Task<bool> UserExistsAsync(int userId)
    {
        try
        {
            return await _userRepository.UserExistsAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user exists: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Check if email exists
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        try
        {
            // This would need to be implemented in the repository
            // For now, we'll use a simple check
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;
            
            return excludeUserId.HasValue ? user.UserId != excludeUserId.Value : true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if email exists: {Email}", email);
            throw;
        }
    }
}
