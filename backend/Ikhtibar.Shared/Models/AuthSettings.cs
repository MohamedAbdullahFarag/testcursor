namespace Ikhtibar.Shared.Models;

/// <summary>
/// Authentication configuration settings
/// </summary>
public class AuthSettings
{
    /// <summary>
    /// JWT secret key for signing tokens
    /// </summary>
    public string JwtSecretKey { get; set; } = string.Empty;

    /// <summary>
    /// JWT issuer for token validation
    /// </summary>
    public string JwtIssuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT audience for token validation
    /// </summary>
    public string JwtAudience { get; set; } = string.Empty;

    /// <summary>
    /// Access token expiration time in minutes
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh token expiration time in days
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;

    /// <summary>
    /// Maximum failed login attempts before lockout
    /// </summary>
    public int MaxFailedLoginAttempts { get; set; } = 5;

    /// <summary>
    /// Lockout duration in minutes
    /// </summary>
    public int LockoutDurationMinutes { get; set; } = 15;

    /// <summary>
    /// Whether to use HTTP-only cookies for refresh tokens
    /// </summary>
    public bool UseHttpOnlyCookies { get; set; } = true;

    /// <summary>
    /// Name of the refresh token cookie
    /// </summary>
    public string RefreshTokenCookieName { get; set; } = "refresh_token";

    /// <summary>
    /// Whether to require HTTPS for cookies
    /// </summary>
    public bool RequireHttps { get; set; } = true;

    /// <summary>
    /// Cookie domain for refresh tokens
    /// </summary>
    public string? CookieDomain { get; set; }

    /// <summary>
    /// Cookie path for refresh tokens
    /// </summary>
    public string CookiePath { get; set; } = "/";

    /// <summary>
    /// Whether to enable refresh token rotation
    /// </summary>
    public bool EnableTokenRotation { get; set; } = true;

    /// <summary>
    /// Maximum concurrent refresh tokens per user
    /// </summary>
    public int MaxConcurrentTokens { get; set; } = 5;
}
