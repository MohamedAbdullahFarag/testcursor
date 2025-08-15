using Dapper;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Infrastructure.Data;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Dapper implementation of refresh token repository
/// Following SRP: ONLY RefreshToken data operations
/// </summary>
public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(IDbConnectionFactory connectionFactory, ILogger<RefreshTokenRepository> logger)
        : base(connectionFactory, logger, "RefreshTokens", "Id")
    {
    }

    /// <summary>
    /// Get refresh token by its hash
    /// </summary>
    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT Id, TokenHash, UserId, IssuedAt, ExpiresAt, RevokedAt, 
                       RevocationReason, ClientIpAddress, UserAgent, IsDeleted, 
                       CreatedAt, ModifiedAt, DeletedAt
                FROM RefreshTokens 
                WHERE TokenHash = @TokenHash AND IsDeleted = 0";

            var result = await connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { TokenHash = tokenHash });
            
            if (result != null)
            {
                _logger.LogDebug("Found refresh token for hash: {TokenHashPrefix}", tokenHash[..8]);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving refresh token by hash: {TokenHashPrefix}", tokenHash[..8]);
            throw;
        }
    }

    /// <summary>
    /// Get all active refresh tokens for a user
    /// </summary>
    public async Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(int userId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT Id, TokenHash, UserId, IssuedAt, ExpiresAt, RevokedAt, 
                       RevocationReason, ClientIpAddress, UserAgent, IsDeleted, 
                       CreatedAt, ModifiedAt, DeletedAt
                FROM RefreshTokens 
                WHERE UserId = @UserId 
                  AND IsDeleted = 0 
                  AND RevokedAt IS NULL 
                  AND ExpiresAt > @Now
                ORDER BY IssuedAt DESC";

            var result = await connection.QueryAsync<RefreshToken>(sql, new { UserId = userId, Now = DateTime.UtcNow });
            
            _logger.LogDebug("Retrieved {Count} active refresh tokens for user {UserId}", result.Count(), userId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active refresh tokens for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Revoke all refresh tokens for a user
    /// </summary>
    public async Task<bool> RevokeAllByUserIdAsync(int userId, string reason = "User logout")
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                UPDATE RefreshTokens 
                SET RevokedAt = @RevokedAt, 
                    RevocationReason = @Reason,
                    ModifiedAt = @ModifiedAt
                WHERE UserId = @UserId 
                  AND IsDeleted = 0 
                  AND RevokedAt IS NULL";

            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                RevokedAt = DateTime.UtcNow,
                Reason = reason,
                ModifiedAt = DateTime.UtcNow
            });

            _logger.LogInformation("Revoked {Count} refresh tokens for user {UserId} with reason: {Reason}", 
                affectedRows, userId, reason);
            
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all refresh tokens for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Revoke a specific refresh token
    /// </summary>
    public async Task<bool> RevokeByTokenHashAsync(string tokenHash, string reason = "Token revoked")
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                UPDATE RefreshTokens 
                SET RevokedAt = @RevokedAt, 
                    RevocationReason = @Reason,
                    ModifiedAt = @ModifiedAt
                WHERE TokenHash = @TokenHash 
                  AND IsDeleted = 0 
                  AND RevokedAt IS NULL";

            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                TokenHash = tokenHash,
                RevokedAt = DateTime.UtcNow,
                Reason = reason,
                ModifiedAt = DateTime.UtcNow
            });

            if (affectedRows > 0)
            {
                _logger.LogInformation("Revoked refresh token {TokenHashPrefix} with reason: {Reason}", 
                    tokenHash[..8], reason);
            }
            
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token {TokenHashPrefix}", tokenHash[..8]);
            throw;
        }
    }

    /// <summary>
    /// Clean up expired refresh tokens
    /// </summary>
    public async Task<int> CleanupExpiredTokensAsync(int batchSize = 100)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                UPDATE RefreshTokens 
                SET IsDeleted = 1, 
                    DeletedAt = @DeletedAt,
                    ModifiedAt = @ModifiedAt
                WHERE Id IN (
                    SELECT TOP (@BatchSize) Id 
                    FROM RefreshTokens 
                    WHERE IsDeleted = 0 
                      AND ExpiresAt < @Now
                      AND RevokedAt IS NULL
                    ORDER BY ExpiresAt ASC
                )";

            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                BatchSize = batchSize,
                DeletedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                Now = DateTime.UtcNow
            });

            if (affectedRows > 0)
            {
                _logger.LogInformation("Cleaned up {Count} expired refresh tokens", affectedRows);
            }
            
            return affectedRows;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired refresh tokens");
            throw;
        }
    }

    /// <summary>
    /// Get refresh token statistics for a user
    /// </summary>
    public async Task<RefreshTokenStats> GetStatsByUserIdAsync(int userId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = @"
                SELECT 
                    COUNT(*) as TotalTokens,
                    SUM(CASE WHEN RevokedAt IS NULL AND ExpiresAt > @Now THEN 1 ELSE 0 END) as ActiveTokens,
                    SUM(CASE WHEN ExpiresAt <= @Now THEN 1 ELSE 0 END) as ExpiredTokens,
                    SUM(CASE WHEN RevokedAt IS NOT NULL THEN 1 ELSE 0 END) as RevokedTokens,
                    MAX(IssuedAt) as LastIssuedAt,
                    MAX(CASE WHEN RevokedAt IS NOT NULL THEN RevokedAt ELSE IssuedAt END) as LastUsedAt
                FROM RefreshTokens 
                WHERE UserId = @UserId AND IsDeleted = 0";

            var result = await connection.QueryFirstOrDefaultAsync<RefreshTokenStats>(sql, new
            {
                UserId = userId,
                Now = DateTime.UtcNow
            });

            return result ?? new RefreshTokenStats();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving refresh token stats for user {UserId}", userId);
            throw;
        }
    }
}
