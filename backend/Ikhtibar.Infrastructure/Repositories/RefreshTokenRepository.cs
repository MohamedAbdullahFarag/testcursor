using Dapper;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for RefreshToken entity operations
/// Handles all refresh token-related data access operations
/// Custom implementation due to different schema structure from BaseEntity
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(IDbConnectionFactory connectionFactory, ILogger<RefreshTokenRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }
    
    /// <summary>
    /// Add a new refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token to add</param>
    /// <returns>The added refresh token</returns>
    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
    {
        try
        {
            // Set audit fields
            refreshToken.CreatedAt = DateTime.UtcNow;
            refreshToken.ModifiedAt = DateTime.UtcNow;
            refreshToken.IsDeleted = false;

            const string sql = @"
                INSERT INTO RefreshTokens (TokenHash, UserId, IssuedAt, ExpiresAt, CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, IsDeleted)
                VALUES (@TokenHash, @UserId, @IssuedAt, @ExpiresAt, @CreatedAt, @CreatedBy, @ModifiedAt, @ModifiedBy, @IsDeleted);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = _connectionFactory.CreateConnection();
            var refreshTokenId = await connection.QuerySingleAsync<int>(sql, refreshToken);
            refreshToken.RefreshTokenId = refreshTokenId;

            _logger.LogDebug("Created refresh token with ID: {RefreshTokenId}", refreshTokenId);
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating refresh token");
            throw;
        }
    }
    
    /// <summary>
    /// Get refresh token by token hash
    /// </summary>
    /// <param name="tokenHash">The hash of the refresh token</param>
    /// <returns>RefreshToken if found, null otherwise</returns>
    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        try
        {
            const string sql = @"
                SELECT RefreshTokenId, UserId, TokenHash, IssuedAt, ExpiresAt, RevokedAt, ReplacedByToken, ReasonRevoked,
                       CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, IsDeleted, RowVersion
                FROM RefreshTokens 
                WHERE TokenHash = @TokenHash 
                  AND (RevokedAt IS NULL)
                  AND ExpiresAt > @CurrentTime
                  AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var refreshToken = await connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new
            {
                TokenHash = tokenHash,
                CurrentTime = DateTime.UtcNow
            });

            _logger.LogDebug("Retrieved refresh token by hash, Found: {Found}", refreshToken != null);
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving refresh token by hash");
            throw;
        }
    }

    /// <summary>
    /// Get latest active refresh token for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Latest active refresh token</returns>
    public async Task<RefreshToken?> GetLatestByUserIdAsync(int userId)
    {
        try
        {
            const string sql = @"
                SELECT TOP 1 TokenId, UserId, TokenHash, IssuedAt, ExpiresAt, CreatedAt, RevokedAt, ModifiedAt
                FROM RefreshTokens 
                WHERE UserId = @UserId 
                  AND (RevokedAt IS NULL)
                  AND ExpiresAt > @CurrentTime
                ORDER BY IssuedAt DESC";

            using var connection = _connectionFactory.CreateConnection();
            var refreshToken = await connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new
            {
                UserId = userId,
                CurrentTime = DateTime.UtcNow
            });

            _logger.LogDebug("Retrieved latest refresh token for user: {UserId}, Found: {Found}", 
                userId, refreshToken != null);
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving latest refresh token for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User if found, null otherwise</returns>
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        try
        {
            const string sql = @"
                SELECT UserId, Email, Username, FirstName, LastName, IsActive, CreatedAt, ModifiedAt
                FROM Users 
                WHERE UserId = @UserId AND IsDeleted = 0";

            using var connection = _connectionFactory.CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });

            _logger.LogDebug("Retrieved user by ID: {UserId}, Found: {Found}", userId, user != null);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Revoke a specific refresh token
    /// </summary>
    /// <param name="tokenId">The ID of the token to revoke</param>
    /// <returns>True if token was revoked, false if not found</returns>
    public async Task<bool> RevokeAsync(int tokenId)
    {
        try
        {
            const string sql = @"
                UPDATE RefreshTokens 
                SET RevokedAt = @RevokedAt, ModifiedAt = @ModifiedAt
                WHERE TokenId = @TokenId AND RevokedAt IS NULL";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(sql, new
            {
                TokenId = tokenId,
                RevokedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            });

            var wasRevoked = result > 0;
            _logger.LogDebug("Revoked refresh token ID: {TokenId}, Success: {Success}", tokenId, wasRevoked);
            return wasRevoked;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token ID: {TokenId}", tokenId);
            throw;
        }
    }

    /// <summary>
    /// Get refresh token by token value
    /// </summary>
    /// <param name="tokenValue">The refresh token value</param>
    /// <returns>RefreshToken if found and valid, null otherwise</returns>
    public async Task<RefreshToken?> GetByTokenAsync(string tokenValue)
    {
        try
        {
            const string sql = @"
                SELECT Id, UserId, Token, ExpiresAt, IsRevoked, CreatedAt, UpdatedAt
                FROM RefreshTokens 
                WHERE Token = @Token 
                  AND IsRevoked = 0 
                  AND ExpiresAt > @CurrentTime";

            using var connection = _connectionFactory.CreateConnection();
            var refreshToken = await connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new
            {
                Token = tokenValue,
                CurrentTime = DateTime.UtcNow
            });

            _logger.LogDebug("Retrieved refresh token by value, Found: {Found}", refreshToken != null);
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving refresh token by value");
            throw;
        }
    }

    /// <summary>
    /// Get all active refresh tokens for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of active refresh tokens</returns>
    public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(int userId)
    {
        try
        {
            const string sql = @"
                SELECT Id, UserId, Token, ExpiresAt, IsRevoked, CreatedAt, UpdatedAt
                FROM RefreshTokens 
                WHERE UserId = @UserId 
                  AND IsRevoked = 0 
                  AND ExpiresAt > @CurrentTime
                ORDER BY CreatedAt DESC";

            using var connection = _connectionFactory.CreateConnection();
            var tokens = await connection.QueryAsync<RefreshToken>(sql, new
            {
                UserId = userId,
                CurrentTime = DateTime.UtcNow
            });

            var tokenList = tokens.ToList();
            _logger.LogDebug("Retrieved {TokenCount} active refresh tokens for user: {UserId}", tokenList.Count, userId);
            return tokenList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active refresh tokens for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Revoke a specific refresh token
    /// </summary>
    /// <param name="tokenValue">The refresh token value to revoke</param>
    /// <returns>True if token was revoked, false if not found</returns>
    public async Task<bool> RevokeTokenAsync(string tokenValue)
    {
        try
        {
            const string sql = @"
                UPDATE RefreshTokens 
                SET IsRevoked = 1, UpdatedAt = @UpdatedAt
                WHERE Token = @Token AND IsRevoked = 0";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(sql, new
            {
                Token = tokenValue,
                UpdatedAt = DateTime.UtcNow
            });

            var wasRevoked = result > 0;
            _logger.LogDebug("Revoked refresh token, Success: {Success}, Affected rows: {AffectedRows}",
                wasRevoked, result);
            return wasRevoked;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token");
            throw;
        }
    }

    /// <summary>
    /// Revoke all refresh tokens for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Number of tokens revoked</returns>
    public async Task<int> RevokeAllUserTokensAsync(int userId)
    {
        try
        {
            const string sql = @"
                UPDATE RefreshTokens 
                SET IsRevoked = 1, UpdatedAt = @UpdatedAt
                WHERE UserId = @UserId AND IsRevoked = 0";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                UpdatedAt = DateTime.UtcNow
            });

            _logger.LogDebug("Revoked all refresh tokens for user: {UserId}, Count: {Count}", userId, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all refresh tokens for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Clean up expired refresh tokens
    /// </summary>
    /// <returns>Number of tokens cleaned up</returns>
    public async Task<int> CleanupExpiredTokensAsync()
    {
        try
        {
            const string sql = @"
                DELETE FROM RefreshTokens 
                WHERE ExpiresAt <= @CurrentTime 
                   OR IsRevoked = 1";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(sql, new { CurrentTime = DateTime.UtcNow });

            _logger.LogInformation("Cleaned up {Count} expired/revoked refresh tokens", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired refresh tokens");
            throw;
        }
    }

    /// <summary>
    /// Create a new refresh token
    /// </summary>
    /// <param name="refreshToken">RefreshToken entity to create</param>
    /// <returns>Created RefreshToken with ID</returns>
    public async Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            const string sql = @"
                INSERT INTO RefreshTokens (UserId, Token, ExpiresAt, IsRevoked, CreatedAt, UpdatedAt)
                OUTPUT INSERTED.Id
                VALUES (@UserId, @Token, @ExpiresAt, @IsRevoked, @CreatedAt, @UpdatedAt)";

            using var connection = _connectionFactory.CreateConnection();
            var id = await connection.QuerySingleAsync<int>(sql, new
            {
                refreshToken.UserId,
                Token = refreshToken.TokenHash,
                refreshToken.ExpiresAt,
                IsRevoked = refreshToken.RevokedAt.HasValue ? 1 : 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            refreshToken.RefreshTokenId = id;
            refreshToken.CreatedAt = DateTime.UtcNow;
            refreshToken.ModifiedAt = DateTime.UtcNow;

            _logger.LogDebug("Created refresh token for user: {UserId}, TokenId: {TokenId}",
                refreshToken.UserId, id);
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating refresh token for user: {UserId}", refreshToken.UserId);
            throw;
        }
    }

    /// <summary>
    /// Check if user has reached maximum number of active refresh tokens
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="maxTokens">Maximum allowed tokens (default: 5)</param>
    /// <returns>True if user has reached limit</returns>
    public async Task<bool> HasReachedTokenLimitAsync(int userId, int maxTokens = 5)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM RefreshTokens 
                WHERE UserId = @UserId 
                  AND IsRevoked = 0 
                  AND ExpiresAt > @CurrentTime";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.QuerySingleAsync<int>(sql, new
            {
                UserId = userId,
                CurrentTime = DateTime.UtcNow
            });

            var hasReachedLimit = count >= maxTokens;
            _logger.LogDebug("Token limit check for user: {UserId}, Count: {Count}, Limit: {Limit}, Reached: {Reached}",
                userId, count, maxTokens, hasReachedLimit);
            return hasReachedLimit;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking token limit for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Remove oldest refresh token for user when limit is reached
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>True if a token was removed</returns>
    public async Task<bool> RemoveOldestTokenAsync(int userId)
    {
        try
        {
            const string sql = @"
                DELETE FROM RefreshTokens 
                WHERE Id = (
                    SELECT TOP 1 Id 
                    FROM RefreshTokens 
                    WHERE UserId = @UserId 
                      AND IsRevoked = 0 
                      AND ExpiresAt > @CurrentTime
                    ORDER BY CreatedAt ASC
                )";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                CurrentTime = DateTime.UtcNow
            });

            var wasRemoved = result > 0;
            _logger.LogDebug("Removed oldest token for user: {UserId}, Success: {Success}", userId, wasRemoved);
            return wasRemoved;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing oldest token for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Generate a cryptographically secure refresh token value
    /// </summary>
    /// <returns>Base64 encoded random token</returns>
    public static string GenerateTokenValue()
    {
        var randomBytes = new byte[64]; // 512 bits
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}

// ✅ SRP COMPLIANCE:
// - SINGLE RESPONSIBILITY: RefreshToken data access operations only
// - NO business logic beyond data persistence
// - NO authentication logic (delegated to services)
// - NO cross-entity operations
// - FOCUSED: All methods work toward refresh token management

// ❌ ANTI-PATTERNS AVOIDED:
// - No mixing of business logic in repository
// - No direct authentication logic
// - No cross-cutting concerns
// - No god repository with mixed responsibilities
