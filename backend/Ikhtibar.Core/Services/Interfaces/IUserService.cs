using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for User management operations
/// Following SRP: ONLY user business logic operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="createUserDto">User creation data</param>
    /// <returns>Created user data</returns>
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>User data if found, null otherwise</returns>
    Task<UserDto?> GetUserAsync(int userId);

    /// <summary>
    /// Updates an existing user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="updateUserDto">User update data</param>
    /// <returns>Updated user data if successful, null if user not found</returns>
    Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);

    /// <summary>
    /// Deletes a user (soft delete)
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>True if user was deleted, false if not found</returns>
    Task<bool> DeleteUserAsync(int userId);

    /// <summary>
    /// Gets all users with pagination
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated list of users</returns>
    Task<PaginatedResult<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Gets a user by username
    /// </summary>
    /// <param name="username">Username to search for</param>
    /// <returns>User data if found, null otherwise</returns>
    Task<UserDto?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Gets a user by email
    /// </summary>
    /// <param name="email">Email to search for</param>
    /// <returns>User data if found, null otherwise</returns>
    Task<UserDto?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Checks if a user exists
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>True if user exists, false otherwise</returns>
    Task<bool> UserExistsAsync(int userId);

    /// <summary>
    /// Checks if an email is already in use
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="excludeUserId">User ID to exclude from check (for updates)</param>
    /// <returns>True if email is in use, false otherwise</returns>
    Task<bool> IsEmailInUseAsync(string email, int? excludeUserId = null);

    /// <summary>
    /// Checks if a username is already in use
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <param name="excludeUserId">User ID to exclude from check (for updates)</param>
    /// <returns>True if username is in use, false otherwise</returns>
    Task<bool> IsUsernameInUseAsync(string username, int? excludeUserId = null);

    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <returns>User data if authentication successful, null otherwise</returns>
    Task<UserDto?> AuthenticateAsync(string email, string password);

    /// <summary>
    /// Gets a user by ID (alternative method name)
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>User data if found, null otherwise</returns>
    Task<UserDto?> GetUserByIdAsync(int userId);

    /// <summary>
    /// Gets user roles by user ID
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of user roles</returns>
    Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId);

    /// <summary>
    /// Hashes a password
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    Task<string> HashPasswordAsync(string password);

    /// <summary>
    /// Updates the last login time for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>True if update successful, false otherwise</returns>
    Task<bool> UpdateLastLoginAsync(int userId);
}
