using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.DTOs;
using Microsoft.AspNetCore.Builder;
using System.Linq;

namespace Ikhtibar.API.Middleware;

/// <summary>
/// Middleware for automatic refresh token rotation
/// Inspects expired tokens and attempts automatic refresh
/// </summary>
public class RefreshTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RefreshTokenMiddleware> _logger;
    private readonly AuthSettings _authSettings;

    public RefreshTokenMiddleware(
        RequestDelegate next,
        ILogger<RefreshTokenMiddleware> logger,
        IOptions<AuthSettings> authSettings)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _authSettings = authSettings?.Value ?? throw new ArgumentNullException(nameof(authSettings));
    }

    /// <summary>
    /// Process the HTTP request
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Check if request has expired token header
            if (context.Request.Headers.ContainsKey("Token-Expired"))
            {
                await HandleExpiredTokenAsync(context);
                return;
            }

            // Continue with the request pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RefreshTokenMiddleware");
            throw;
        }
    }

    /// <summary>
    /// Handle expired token by attempting automatic refresh
    /// </summary>
    private async Task HandleExpiredTokenAsync(HttpContext context)
    {
        try
        {
            _logger.LogDebug("Handling expired token for request: {Path}", context.Request.Path);

            // Get refresh token from cookie or header
            var refreshToken = GetRefreshTokenFromRequest(context);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("No refresh token found for expired token request");
                await ReturnUnauthorizedResponse(context, "No refresh token available");
                return;
            }

            // Resolve IAuthenticationService from request services
            var authenticationService = context.RequestServices.GetRequiredService<IAuthenticationService>();

            // Attempt to refresh the token
            var authResult = await authenticationService.RefreshTokenAsync(refreshToken);
            if (authResult == null || !authResult.Success)
            {
                _logger.LogWarning("Token refresh failed for request: {Path}", context.Request.Path);
                await ReturnUnauthorizedResponse(context, "Token refresh failed");
                return;
            }

            // Set new tokens in response headers
            context.Response.Headers["New-Access-Token"] = authResult.AccessToken;
            context.Response.Headers["New-Refresh-Token"] = authResult.RefreshTokens;
            context.Response.Headers["Token-Expires-At"] = authResult.ExpiresAt?.ToString("O");

            // Set new refresh token in cookie if using cookie-based storage
            if (_authSettings.UseHttpOnlyCookies)
            {
                SetRefreshTokenCookie(context, authResult.RefreshTokens);
            }

            _logger.LogInformation("Token refreshed successfully for request: {Path}", context.Request.Path);

            // Continue with the original request using new token
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling expired token for request: {Path}", context.Request.Path);
            await ReturnUnauthorizedResponse(context, "Token refresh error");
        }
    }

    /// <summary>
    /// Extract refresh token from request (cookie or header)
    /// </summary>
    private string? GetRefreshTokenFromRequest(HttpContext context)
    {
        // Try to get from cookie first (more secure)
        if (_authSettings.UseHttpOnlyCookies)
        {
            var cookieToken = context.Request.Cookies[_authSettings.RefreshTokenCookieName];
            if (!string.IsNullOrWhiteSpace(cookieToken))
            {
                return cookieToken;
            }
        }

        // Fallback to header
        var headerToken = context.Request.Headers["X-Refresh-Token"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(headerToken))
        {
            return headerToken;
        }

        // Try Authorization header with Bearer token (for backward compatibility)
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length);
            // Check if this is a refresh token (you might want to add validation here)
            return token;
        }

        return null;
    }

    /// <summary>
    /// Set refresh token in HTTP-only cookie
    /// </summary>
    private void SetRefreshTokenCookie(HttpContext context, string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = context.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            MaxAge = TimeSpan.FromDays(_authSettings.RefreshTokenExpirationDays),
            Path = "/"
        };

        context.Response.Cookies.Append(_authSettings.RefreshTokenCookieName, refreshToken, cookieOptions);
    }

    /// <summary>
    /// Return unauthorized response with appropriate headers
    /// </summary>
    private async Task ReturnUnauthorizedResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers["Token-Refresh-Required"] = "true";
        context.Response.Headers["Token-Refresh-Error"] = message;

        var response = new
        {
            error = "Token expired",
            message = message,
            requiresRefresh = true
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}

/// <summary>
/// Extension method for registering the middleware
/// </summary>
public static class RefreshTokenMiddlewareExtensions
{
    /// <summary>
    /// Add refresh token middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseRefreshTokenMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RefreshTokenMiddleware>();
    }
}
