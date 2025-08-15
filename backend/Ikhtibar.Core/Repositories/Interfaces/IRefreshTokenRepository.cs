using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for refresh token operations
/// Following SRP: ONLY RefreshToken data operations
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    /// <summary>
    /// Get refresh token by its hash
    /// </summary>
    /// <param name="tokenHash">Hashed token value</param>
    /// <returns>Refresh token if found, null otherwise</returns>
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);

    /// <summary>
    /// Get all active refresh tokens for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of active refresh tokens</returns>
    Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(int userId);

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

/// <summary>
/// Refresh token statistics
/// </summary>
public class RefreshTokenStats
{
    public int TotalTokens { get; set; }
    public int ActiveTokens { get; set; }
    public int ExpiredTokens { get; set; }
    public int RevokedTokens { get; set; }
    public DateTime? LastIssuedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
