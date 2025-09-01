using System.Collections.Concurrent;
using System.Net;

namespace Ikhtibar.API.Middleware;

/// <summary>
/// Rate limiting middleware to prevent brute force attacks
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimitStore = new();
    private readonly int _maxAttempts;
    private readonly int _windowMinutes;
    private readonly int _lockoutMinutes;

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _maxAttempts = configuration.GetValue<int>("AuthSettings:MaxLoginAttempts", 5);
        _windowMinutes = configuration.GetValue<int>("AuthSettings:RateLimitWindowMinutes", 15);
        _lockoutMinutes = configuration.GetValue<int>("AuthSettings:LockoutDurationMinutes", 30);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only apply rate limiting to authentication endpoints
        if (IsAuthenticationEndpoint(context.Request.Path))
        {
            var clientIp = GetClientIpAddress(context);
            var endpoint = context.Request.Path.Value;

            if (IsRateLimited(clientIp, endpoint))
            {
                _logger.LogWarning("Rate limit exceeded for IP: {ClientIp} on endpoint: {Endpoint}", clientIp, endpoint);
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests",
                    message = $"Rate limit exceeded. Please try again in {_lockoutMinutes} minutes.",
                    retryAfter = _lockoutMinutes * 60
                });
                return;
            }
        }

        await _next(context);
    }

    private bool IsAuthenticationEndpoint(PathString path)
    {
        return path.StartsWithSegments("/api/auth/login", StringComparison.OrdinalIgnoreCase);
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Get IP from various headers (for proxy scenarios)
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
                 context.Request.Headers["X-Real-IP"].FirstOrDefault() ??
                 context.Connection.RemoteIpAddress?.ToString() ??
                 "unknown";

        return ip.Split(',')[0].Trim(); // Handle multiple IPs in X-Forwarded-For
    }

    private bool IsRateLimited(string clientIp, string? endpoint)
    {
        var key = $"{clientIp}:{endpoint}";
        
        if (_rateLimitStore.TryGetValue(key, out var rateLimitInfo))
        {
            // Check if still in lockout period
            if (rateLimitInfo.IsLockedOut && DateTime.UtcNow < rateLimitInfo.LockoutUntil)
            {
                return true;
            }

            // Check if lockout period has expired
            if (rateLimitInfo.IsLockedOut && DateTime.UtcNow >= rateLimitInfo.LockoutUntil)
            {
                // Reset lockout
                rateLimitInfo.IsLockedOut = false;
                rateLimitInfo.Attempts = 0;
                rateLimitInfo.FirstAttempt = DateTime.UtcNow;
            }

            // Check if within rate limit window
            if (DateTime.UtcNow < rateLimitInfo.FirstAttempt.AddMinutes(_windowMinutes))
            {
                if (rateLimitInfo.Attempts >= _maxAttempts)
                {
                    // Apply lockout
                    rateLimitInfo.IsLockedOut = true;
                    rateLimitInfo.LockoutUntil = DateTime.UtcNow.AddMinutes(_lockoutMinutes);
                    _logger.LogWarning("IP {ClientIp} locked out due to rate limit on {Endpoint}", clientIp, endpoint);
                    return true;
                }
            }
            else
            {
                // Reset window
                rateLimitInfo.Attempts = 0;
                rateLimitInfo.FirstAttempt = DateTime.UtcNow;
            }
        }
        else
        {
            // First attempt
            rateLimitInfo = new RateLimitInfo
            {
                FirstAttempt = DateTime.UtcNow,
                Attempts = 0,
                IsLockedOut = false
            };
        }

        // Increment attempt counter
        rateLimitInfo.Attempts++;
        _rateLimitStore.AddOrUpdate(key, rateLimitInfo, (_, _) => rateLimitInfo);

        return false;
    }

    private class RateLimitInfo
    {
        public DateTime FirstAttempt { get; set; }
        public int Attempts { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime LockoutUntil { get; set; }
    }
}

/// <summary>
/// Extension method to register rate limiting middleware
/// </summary>
public static class RateLimitingMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>();
    }
}
