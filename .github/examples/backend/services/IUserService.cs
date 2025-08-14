using Ikhtibar.Backend.Features.Users.Models;
using Ikhtibar.Backend.Shared.Common;
using Ikhtibar.Backend.Shared.Export;

namespace Ikhtibar.Backend.Features.Users.Services;

/// <summary>
/// Interface for user service operations
/// 
/// This interface demonstrates PRP (Product Requirements Prompt) methodology:
/// - Context is King: Clear method signatures with comprehensive documentation
/// - Validation Loops: Exception specifications for each operation
/// - Information Dense: Strongly-typed parameters and return values
/// - Progressive Success: Basic operations first, then advanced features
/// - One-Pass Implementation: Complete API surface for all user management needs
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get paginated list of users with filtering options
    /// </summary>
    /// <param name="request">Pagination and filter parameters</param>
    /// <returns>Paginated result of users</returns>
    Task<PaginatedResult<UserDto>> GetUsersAsync(GetUsersRequest request);
    
    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details or null if not found</returns>
    Task<UserDto?> GetUserByIdAsync(int id);
    
    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request">User creation data</param>
    /// <returns>Created user</returns>
    /// <exception cref="ArgumentException">Thrown when input validation fails</exception>
    /// <exception cref="DuplicateResourceException">Thrown when user email already exists</exception>
    Task<UserDto> CreateUserAsync(CreateUserRequest request);
    
    /// <summary>
    /// Create multiple users
    /// </summary>
    /// <param name="requests">List of user creation requests</param>
    /// <returns>List of created users</returns>
    /// <exception cref="ArgumentException">Thrown when input validation fails</exception>
    /// <exception cref="DuplicateResourceException">Thrown when any user email already exists</exception>
    Task<IEnumerable<UserDto>> CreateUsersAsync(IEnumerable<CreateUserRequest> requests);
    
    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">User update data</param>
    /// <returns>Updated user or null if not found</returns>
    /// <exception cref="ArgumentException">Thrown when input validation fails</exception>
    /// <exception cref="DuplicateResourceException">Thrown when updated email already exists for another user</exception>
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserRequest request);
    
    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>True if deletion was successful, false if user was not found</returns>
    Task<bool> DeleteUserAsync(int id);
    
    /// <summary>
    /// Delete multiple users
    /// </summary>
    /// <param name="ids">List of user IDs to delete</param>
    /// <returns>Result containing counts of successful and failed deletions</returns>
    Task<BulkDeleteResult> DeleteUsersAsync(IEnumerable<int> ids);
    
    /// <summary>
    /// Export users to a specified format
    /// </summary>
    /// <param name="format">Export format (csv, excel, pdf)</param>
    /// <returns>Export result containing file contents and metadata</returns>
    /// <exception cref="ArgumentException">Thrown when format is not supported</exception>
    Task<ExportResult> ExportUsersAsync(string format);
    
    /// <summary>
    /// Check if email is already in use
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="excludeUserId">Optional user ID to exclude from check (for updates)</param>
    /// <returns>True if email is already in use, otherwise false</returns>
    Task<bool> IsEmailInUseAsync(string email, int? excludeUserId = null);
    
    /// <summary>
    /// Change user status (activate/deactivate)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="isActive">New active status</param>
    /// <returns>Updated user or null if not found</returns>
    Task<UserDto?> ChangeUserStatusAsync(int id, bool isActive);
    
    /// <summary>
    /// Get user statistics
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive users in stats</param>
    /// <returns>User statistics</returns>
    Task<UserStatisticsDto> GetUserStatisticsAsync(bool includeInactive = false);
    
    /// <summary>
    /// Set user preferred language
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="languageCode">Language code (e.g., 'en', 'ar')</param>
    /// <returns>Updated user or null if not found</returns>
    /// <exception cref="ArgumentException">Thrown when language code is not supported</exception>
    Task<UserDto?> SetUserPreferredLanguageAsync(int id, string languageCode);
}
