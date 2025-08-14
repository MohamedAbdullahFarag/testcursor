using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.API.Middleware;

/// <summary>
/// Middleware to handle JWT token refresh automatically for expired tokens
/// </summary>
public class RefreshTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RefreshTokenMiddleware> _logger;

    public RefreshTokenMiddleware(
        RequestDelegate next,
        ILogger<RefreshTokenMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Middleware invoke method
    /// </summary>
    public async Task InvokeAsync(
        HttpContext context,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        try
        {
            // Skip if no Authorization header or it's not a Bearer token
            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader) || 
                !authHeader.ToString().StartsWith("Bearer "))
            {
                await _next(context);
                return;
            }

            var token = authHeader.ToString().Substring("Bearer ".Length).Trim();
            
            // Check if token is expired
            bool isExpired = await tokenService.IsTokenExpiredAsync(token);
            
            if (isExpired)
            {
                _logger.LogInformation("Token is expired, attempting to refresh");
                
                // Get user ID from token
                var userId = await tokenService.GetUserIdFromTokenAsync(token);
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Failed to extract user ID from expired token");
                    await _next(context);
                    return;
                }
                
                // Find valid refresh token for user
                var refreshToken = await refreshTokenRepository.GetLatestByUserIdAsync(userId.Value);
                if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
                {
                    _logger.LogInformation("No valid refresh token found for user {UserId}", userId.Value);
                    await _next(context);
                    return;
                }
                
                try
                {
                    // Generate new JWT token
                    var user = await refreshTokenRepository.GetUserByIdAsync(userId.Value);
                    if (user == null)
                    {
                        _logger.LogWarning("User {UserId} not found for token refresh", userId.Value);
                        await _next(context);
                        return;
                    }
                    
                    var newAccessToken = await tokenService.GenerateJwtAsync(user);
                    var newRefreshToken = await tokenService.GenerateRefreshTokenAsync();
                    
                    // Update refresh token in repository
                    await refreshTokenRepository.RevokeAsync(refreshToken.RefreshTokenId);
                    
                    // Hash and store new refresh token
                    var tokenHash = HashToken(newRefreshToken);
                    var newRefreshTokenEntity = new Ikhtibar.Shared.Entities.RefreshToken
                    {
                        TokenHash = tokenHash,
                        UserId = userId.Value,
                        IssuedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddDays(7), // Should come from configuration
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    await refreshTokenRepository.AddAsync(newRefreshTokenEntity);
                    
                    // Add refresh info to response headers
                    context.Response.Headers.Append("X-Token-Refreshed", "true");
                    context.Response.Headers.Append("X-New-Token", newAccessToken);
                    context.Response.Headers.Append("X-New-Refresh-Token", newRefreshToken);
                    
                    _logger.LogInformation("Successfully refreshed token for user {UserId}", userId.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error refreshing token for user {UserId}", userId.Value);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RefreshTokenMiddleware");
        }
        
        // Continue processing the request
        await _next(context);
    }
    
    /// <summary>
    /// Hash refresh token for secure storage
    /// </summary>
    private string HashToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
