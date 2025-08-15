using Ikhtibar.Core.DTOs;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for user management operations
/// Following SRP: ONLY user business logic
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="createUserDto">User creation data</param>
    /// <returns>Created user DTO</returns>
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User DTO if found, null otherwise</returns>
    Task<UserDto?> GetUserAsync(int userId);

    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>User DTO if found, null otherwise</returns>
    Task<UserDto?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="updateUserDto">User update data</param>
    /// <returns>Updated user DTO</returns>
    Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteUserAsync(int userId);

    /// <summary>
    /// Get all users with pagination
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>Collection of user DTOs</returns>
    Task<IEnumerable<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// Get user roles
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of role names</returns>
    Task<List<string>> GetUserRolesAsync(int userId);

    /// <summary>
    /// Update user's last login timestamp
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    Task UpdateLastLoginAsync(int userId);

    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <returns>User DTO if authenticated, null otherwise</returns>
    Task<UserDto?> AuthenticateAsync(string email, string password);

    /// <summary>
    /// Check if user exists
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>True if user exists, false otherwise</returns>
    Task<bool> UserExistsAsync(int userId);

    /// <summary>
    /// Check if email exists
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="excludeUserId">User ID to exclude from check</param>
    /// <returns>True if email exists, false otherwise</returns>
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
}
