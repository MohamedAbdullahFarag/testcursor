using Dapper;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;
using Ikhtibar.Infrastructure.Data;
using Ikhtibar.Infrastructure.Repositories;

namespace Ikhtibar.Infrastructure.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshTokens>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IDbConnectionFactory connectionFactory, ILogger<RefreshTokenRepository> logger) 
            : base(connectionFactory, logger, "RefreshTokens", "Id")
        {
        }

        public async Task<RefreshTokens?> GetByTokenHashAsync(string tokenHash)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var query = @"
                    SELECT * FROM RefreshTokens 
                    WHERE TokenHash = @TokenHash 
                    AND IsDeleted = 0 
                    AND ExpiresAt > GETUTCDATE()";

                return await connection.QueryFirstOrDefaultAsync<RefreshTokens>(
                    query,
                    new { TokenHash = tokenHash }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving refresh token by hash");
                throw;
            }
        }

        public async Task<bool> RevokeByUserIdAsync(int userId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var query = @"
                    UPDATE RefreshTokens 
                    SET IsDeleted = 1, 
                        DeletedAt = GETUTCDATE(), 
                        DeletedBy = @UserId 
                    WHERE UserId = @UserId 
                    AND IsDeleted = 0";

                var result = await connection.ExecuteAsync(
                    query,
                    new { UserId = userId }
                );

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh tokens for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> RevokeByTokenHashAsync(string tokenHash)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var query = @"
                    UPDATE RefreshTokens 
                    SET IsDeleted = 1, 
                        DeletedAt = GETUTCDATE() 
                    WHERE TokenHash = @TokenHash 
                    AND IsDeleted = 0";

                var result = await connection.ExecuteAsync(
                    query,
                    new { TokenHash = tokenHash }
                );

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token by hash");
                throw;
            }
        }

        public async Task<bool> CleanupExpiredTokensAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var query = @"
                    UPDATE RefreshTokens 
                    SET IsDeleted = 1, 
                        DeletedAt = GETUTCDATE() 
                    WHERE ExpiresAt <= GETUTCDATE() 
                    AND IsDeleted = 0";

                var result = await connection.ExecuteAsync(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired refresh tokens");
                throw;
            }
        }

        public async Task<IEnumerable<RefreshTokens>> GetActiveByUserIdAsync(int userId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var query = @"
                    SELECT * FROM RefreshTokens 
                    WHERE UserId = @UserId 
                    AND IsDeleted = 0 
                    AND ExpiresAt > GETUTCDATE()
                    ORDER BY CreatedAt DESC";

                return await connection.QueryAsync<RefreshTokens>(
                    query,
                    new { UserId = userId }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active refresh tokens for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> RevokeAllByUserIdAsync(int userId, string reason = "User logout")
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var query = @"
                    UPDATE RefreshTokens 
                    SET IsDeleted = 1, 
                        DeletedAt = GETUTCDATE(), 
                        DeletedBy = @UserId 
                    WHERE UserId = @UserId 
                    AND IsDeleted = 0";

                var result = await connection.ExecuteAsync(
                    query,
                    new { UserId = userId }
                );

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking all refresh tokens for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> RevokeByTokenHashAsync(string tokenHash, string reason = "Token revoked")
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var query = @"
                    UPDATE RefreshTokens 
                    SET IsDeleted = 1, 
                        DeletedAt = GETUTCDATE() 
                    WHERE TokenHash = @TokenHash 
                    AND IsDeleted = 0";

                var result = await connection.ExecuteAsync(
                    query,
                    new { TokenHash = tokenHash }
                );

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token by hash");
                throw;
            }
        }

        public async Task<int> CleanupExpiredTokensAsync(int batchSize = 100)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var query = @"
                    UPDATE TOP(@BatchSize) RefreshTokens 
                    SET IsDeleted = 1, 
                        DeletedAt = GETUTCDATE() 
                    WHERE ExpiresAt <= GETUTCDATE() 
                    AND IsDeleted = 0";

                var result = await connection.ExecuteAsync(query, new { BatchSize = batchSize });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired refresh tokens");
                throw;
            }
        }

        public async Task<RefreshTokenStats> GetStatsByUserIdAsync(int userId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                var query = @"
                    SELECT 
                        COUNT(*) as TotalTokens,
                        SUM(CASE WHEN ExpiresAt > GETUTCDATE() AND IsDeleted = 0 THEN 1 ELSE 0 END) as ActiveTokens,
                        SUM(CASE WHEN ExpiresAt <= GETUTCDATE() AND IsDeleted = 0 THEN 1 ELSE 0 END) as ExpiredTokens,
                        SUM(CASE WHEN IsDeleted = 1 THEN 1 ELSE 0 END) as RevokedTokens,
                        MAX(CASE WHEN IsDeleted = 0 THEN CreatedAt END) as LastIssuedAt,
                        MAX(CASE WHEN IsDeleted = 0 THEN ModifiedAt END) as LastUsedAt
                    FROM RefreshTokens 
                    WHERE UserId = @UserId";

                var stats = await connection.QueryFirstOrDefaultAsync<RefreshTokenStats>(
                    query,
                    new { UserId = userId }
                );

                return stats ?? new RefreshTokenStats();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving refresh token stats for user {UserId}", userId);
                throw;
            }
        }
    }
}
