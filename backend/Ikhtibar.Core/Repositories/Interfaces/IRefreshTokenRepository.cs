using Ikhtibar.Shared.Entities;

using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for RefreshToken entity operations
/// Custom interface not inheriting from IRepository due to different schema structure
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Add a new refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token to add</param>
    /// <returns>The added refresh token</returns>
    Task<RefreshToken> AddAsync(RefreshToken refreshToken);
    /// <summary>
    /// Get refresh token by token hash
    /// </summary>
    /// <param name="tokenHash">The hash of the refresh token</param>
    /// <returns>RefreshToken if found, null otherwise</returns>
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    
    /// <summary>
    /// Get latest active refresh token for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Latest active refresh token</returns>
    Task<RefreshToken?> GetLatestByUserIdAsync(int userId);
    
    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetUserByIdAsync(int userId);
    
    /// <summary>
    /// Revoke a specific refresh token
    /// </summary>
    /// <param name="tokenId">The ID of the token to revoke</param>
    /// <returns>True if token was revoked, false if not found</returns>
    Task<bool> RevokeAsync(int tokenId);
    
    /// <summary>
    /// Get refresh token by token value
    /// </summary>
    /// <param name="tokenValue">The refresh token value</param>
    /// <returns>RefreshToken if found and valid, null otherwise</returns>
    Task<RefreshToken?> GetByTokenAsync(string tokenValue);

    /// <summary>
    /// Get all active refresh tokens for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of active refresh tokens</returns>
    Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(int userId);

    /// <summary>
    /// Revoke a specific refresh token
    /// </summary>
    /// <param name="tokenValue">The refresh token value to revoke</param>
    /// <returns>True if token was revoked, false if not found</returns>
    Task<bool> RevokeTokenAsync(string tokenValue);

    /// <summary>
    /// Revoke all refresh tokens for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Number of tokens revoked</returns>
    Task<int> RevokeAllUserTokensAsync(int userId);

    /// <summary>
    /// Clean up expired refresh tokens
    /// </summary>
    /// <returns>Number of tokens cleaned up</returns>
    Task<int> CleanupExpiredTokensAsync();

    /// <summary>
    /// Check if user has reached maximum number of active refresh tokens
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="maxTokens">Maximum allowed tokens (default: 5)</param>
    /// <returns>True if user has reached limit</returns>
    Task<bool> HasReachedTokenLimitAsync(int userId, int maxTokens = 5);

    /// <summary>
    /// Remove oldest refresh token for user when limit is reached
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>True if a token was removed</returns>
    Task<bool> RemoveOldestTokenAsync(int userId);
}
