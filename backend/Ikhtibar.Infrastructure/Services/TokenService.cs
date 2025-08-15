using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Implementation of JWT token generation and validation services
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IOptions<JwtSettings> jwtSettings, ILogger<TokenService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Generate JWT access token for authenticated user
    /// </summary>
    /// <param name="user">User to generate token for</param>
    /// <returns>JWT access token</returns>
    public Task<string> GenerateJwtAsync(UserDto user)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.GivenName, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new("preferred_language", user.PreferredLanguage ?? "en"),
                new("email_verified", user.EmailVerified.ToString().ToLower()),
                new("phone_verified", user.PhoneVerified.ToString().ToLower()),
                new("is_active", user.IsActive.ToString().ToLower()),
                new("jti", Guid.NewGuid().ToString()) // JWT ID for token tracking
            };

            // Add roles as claims
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation("JWT token generated for user {UserId}", user.UserId);
            return Task.FromResult(tokenString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating JWT token for user {UserId}", user.UserId);
            throw;
        }
    }

    /// <summary>
    /// Generate refresh token for token rotation
    /// </summary>
    /// <returns>Refresh token</returns>
    public Task<string> GenerateRefreshTokenAsync()
    {
        try
        {
            // Generate a cryptographically secure random token
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            var refreshToken = Convert.ToBase64String(randomBytes);

            _logger.LogDebug("Refresh token generated successfully");
            return Task.FromResult(refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating refresh token");
            throw;
        }
    }

    /// <summary>
    /// Validate JWT token and extract claims
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>True if token is valid, false otherwise</returns>
    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = _jwtSettings.ValidateAudience,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = _jwtSettings.ValidateLifetime,
                ClockSkew = TimeSpan.FromMinutes(_jwtSettings.ClockSkewMinutes)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            _logger.LogDebug("JWT token validated successfully");
            return Task.FromResult(true);
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "JWT token validation failed");
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating JWT token");
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Extract user ID from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID if token is valid, null otherwise</returns>
    public Task<int?> GetUserIdFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);

            var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return Task.FromResult<int?>(userId);
            }

            return Task.FromResult<int?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting user ID from JWT token");
            return Task.FromResult<int?>(null);
        }
    }

    /// <summary>
    /// Check if token is expired
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>True if token is expired, false otherwise</returns>
    public Task<bool> IsTokenExpiredAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);

            return Task.FromResult(jsonToken.ValidTo < DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking token expiration");
            return Task.FromResult(true); // Assume expired if we can't read it
        }
    }

    /// <summary>
    /// Get token expiration time
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Expiration time if token is valid, null otherwise</returns>
    public Task<DateTime?> GetTokenExpirationAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);

            return Task.FromResult<DateTime?>(jsonToken.ValidTo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting token expiration");
            return Task.FromResult<DateTime?>(null);
        }
    }
}
