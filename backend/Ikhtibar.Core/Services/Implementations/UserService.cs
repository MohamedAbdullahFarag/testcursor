using AutoMapper;
using Microsoft.Extensions.Logging;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.Core.Services.Implementations;

/// <summary>
/// Service implementation for User management operations
/// Following SRP: ONLY user business logic
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        using var scope = _logger.BeginScope("Creating user with email {Email}", createUserDto.Email);

        _logger.LogInformation("Starting user creation process");

        // Business validation: Check if email is already in use
        var emailExists = await _userRepository.EmailExistsAsync(createUserDto.Email);
        if (emailExists)
        {
            _logger.LogWarning("Attempted to create user with existing email: {Email}", createUserDto.Email);
            throw new InvalidOperationException($"Email '{createUserDto.Email}' is already in use");
        }

        // Business validation: Check if username is already in use
        var usernameExists = await _userRepository.UsernameExistsAsync(createUserDto.Username);
        if (usernameExists)
        {
            _logger.LogWarning("Attempted to create user with existing username: {Username}", createUserDto.Username);
            throw new InvalidOperationException($"Username '{createUserDto.Username}' is already in use");
        }

        // Map DTO to entity
        var user = _mapper.Map<User>(createUserDto);
        user.CreatedAt = DateTime.UtcNow;

        // Create user through repository
        var createdUser = await _userRepository.CreateAsync(user);

        _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.UserId);

        // Map entity back to DTO
        return _mapper.Map<UserDto>(createdUser);
    }

    public async Task<UserDto?> GetUserAsync(int userId)
    {
        _logger.LogDebug("Retrieving user with ID: {UserId}", userId);

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            _logger.LogDebug("User not found with ID: {UserId}", userId);
            return null;
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
    {
        using var scope = _logger.BeginScope("Updating user {UserId}", userId);

        _logger.LogInformation("Starting user update process");

        // Check if user exists
        var existingUser = await _userRepository.GetByIdAsync(userId);
        if (existingUser == null)
        {
            _logger.LogWarning("Attempted to update non-existent user: {UserId}", userId);
            return null;
        }

        // Business validation: Check if email is already in use (excluding current user)
        if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != existingUser.Email)
        {
            var emailExists = await _userRepository.EmailExistsAsync(updateUserDto.Email, userId);
            if (emailExists)
            {
                _logger.LogWarning("Attempted to update user {UserId} with existing email: {Email}", userId, updateUserDto.Email);
                throw new InvalidOperationException($"Email '{updateUserDto.Email}' is already in use");
            }
        }

        // Business validation: Check if username is already in use (excluding current user)
        if (!string.IsNullOrEmpty(updateUserDto.Username) && updateUserDto.Username != existingUser.Username)
        {
            var usernameExists = await _userRepository.UsernameExistsAsync(updateUserDto.Username, userId);
            if (usernameExists)
            {
                _logger.LogWarning("Attempted to update user {UserId} with existing username: {Username}", userId, updateUserDto.Username);
                throw new InvalidOperationException($"Username '{updateUserDto.Username}' is already in use");
            }
        }

        // Map updates to existing entity
        _mapper.Map(updateUserDto, existingUser);
        existingUser.ModifiedAt = DateTime.UtcNow;

        // Update through repository
        var updatedUser = await _userRepository.UpdateAsync(existingUser);

        _logger.LogInformation("User updated successfully: {UserId}", userId);

        return _mapper.Map<UserDto>(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        _logger.LogInformation("Deleting user: {UserId}", userId);

        var result = await _userRepository.DeleteAsync(userId);

        if (result)
        {
            _logger.LogInformation("User deleted successfully: {UserId}", userId);
        }
        else
        {
            _logger.LogWarning("Failed to delete user or user not found: {UserId}", userId);
        }

        return result;
    }

    public async Task<PaginatedResult<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 10)
    {
        _logger.LogDebug("Retrieving users with details - Page: {Page}, PageSize: {PageSize}", page, pageSize);

        var (users, totalCount) = await _userRepository.GetAllWithDetailsAsync(page, pageSize);
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

        return new PaginatedResult<UserDto>(userDtos, totalCount, page, pageSize);
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        _logger.LogDebug("Retrieving user by username: {Username}", username);

        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            _logger.LogDebug("User not found with username: {Username}", username);
            return null;
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        _logger.LogDebug("Retrieving user by email: {Email}", email);

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            _logger.LogDebug("User not found with email: {Email}", email);
            return null;
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> UserExistsAsync(int userId)
    {
        _logger.LogDebug("Checking if user exists: {UserId}", userId);
        return await _userRepository.UserExistsAsync(userId);
    }

    public async Task<bool> IsEmailInUseAsync(string email, int? excludeUserId = null)
    {
        _logger.LogDebug("Checking if email is in use: {Email}", email);
        return await _userRepository.EmailExistsAsync(email, excludeUserId);
    }

    public async Task<bool> IsUsernameInUseAsync(string username, int? excludeUserId = null)
    {
        _logger.LogDebug("Checking if username is in use: {Username}", username);
        return await _userRepository.UsernameExistsAsync(username, excludeUserId);
    }

    public async Task<UserDto?> AuthenticateAsync(string email, string password)
    {
        _logger.LogDebug("Authenticating user with email: {Email}", email);

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("Authentication failed: User not found with email: {Email}", email);
            return null;
        }

        // Verify password using BCrypt
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            _logger.LogWarning("Authentication failed: Invalid password for email: {Email}", email);
            return null;
        }

        _logger.LogInformation("User authenticated successfully: {Email}", email);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        // This is an alias for GetUserAsync for compatibility
        return await GetUserAsync(userId);
    }

    public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId)
    {
        _logger.LogDebug("Retrieving roles for user: {UserId}", userId);

        var userRoles = await _userRepository.GetUserRoleEntitiesAsync(userId);
        return _mapper.Map<IEnumerable<RoleDto>>(userRoles);
    }

    public async Task<string> HashPasswordAsync(string password)
    {
        _logger.LogDebug("Hashing password");

        // Use BCrypt to hash the password
        return await Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password));
    }

    public async Task<bool> UpdateLastLoginAsync(int userId)
    {
        _logger.LogDebug("Updating last login for user: {UserId}", userId);

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("Cannot update last login: User not found: {UserId}", userId);
            return false;
        }

        user.ModifiedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("Last login updated for user: {UserId}", userId);
        return true;
    }
}
