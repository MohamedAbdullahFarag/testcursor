using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for refresh token operations
/// Following SRP: ONLY RefreshTokens data operations
/// </summary>
public interface IRefreshTokenRepository : IBaseRepository<RefreshTokens>
{
    /// <summary>
    /// Get refresh token by its hash
    /// </summary>
    /// <param name="tokenHash">Hashed token value</param>
    /// <returns>Refresh token if found, null otherwise</returns>
    Task<RefreshTokens?> GetByTokenHashAsync(string tokenHash);

    /// <summary>
    /// Get all active refresh tokens for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of active refresh tokens</returns>
    Task<IEnumerable<RefreshTokens>> GetActiveByUserIdAsync(int userId);

    /// <summary>
    /// Revoke all refresh tokens for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="reason">Reason for revocation</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> RevokeAllByUserIdAsync(int userId, string reason = "User logout");

    /// <summary>
    /// Revoke a specific refresh token
    /// </summary>
    /// <param name="tokenHash">Token hash to revoke</param>
    /// <param name="reason">Reason for revocation</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> RevokeByTokenHashAsync(string tokenHash, string reason = "Token revoked");

    /// <summary>
    /// Clean up expired refresh tokens
    /// </summary>
    /// <param name="batchSize">Maximum number of tokens to clean up</param>
    /// <returns>Number of tokens cleaned up</returns>
    Task<int> CleanupExpiredTokensAsync(int batchSize = 100);

    /// <summary>
    /// Get refresh token statistics for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Token statistics</returns>
    Task<RefreshTokenStats> GetStatsByUserIdAsync(int userId);
}


