using Ikhtibar.Shared.Entities;

using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for JWT token generation and validation
/// Following SRP: ONLY token-related operations
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate JWT access token for authenticated user
    /// </summary>
    /// <param name="user">User to generate token for</param>
    /// <returns>JWT access token</returns>
    Task<string> GenerateJwtAsync(User user);

    /// <summary>
    /// Generate refresh token for token rotation
    /// </summary>
    /// <returns>Refresh token</returns>
    Task<string> GenerateRefreshTokenAsync();

    /// <summary>
    /// Validate JWT token and extract claims
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>True if token is valid, false otherwise</returns>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// Extract user ID from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID if token is valid, null otherwise</returns>
    Task<int?> GetUserIdFromTokenAsync(string token);

    /// <summary>
    /// Check if token is expired
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>True if token is expired, false otherwise</returns>
    Task<bool> IsTokenExpiredAsync(string token);

    /// <summary>
    /// Get token expiration time
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Expiration time if token is valid, null otherwise</returns>
    Task<DateTime?> GetTokenExpirationAsync(string token);
}
