using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Core.DTOs;

/// <summary>
/// Login request DTO
/// </summary>
public class LoginDto
{
    /// <summary>
    /// User email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User password
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Client IP address for security logging
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// User agent for security logging
    /// </summary>
    public string? UserAgent { get; set; }
}

/// <summary>
/// Authentication result DTO
/// </summary>
public class AuthResultDto
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token for getting new access tokens
    /// </summary>
    public string RefreshTokens { get; set; } = string.Empty;

    /// <summary>
    /// Token type (usually "Bearer")
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Token expiration time in seconds
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// When the token was issued
    /// </summary>
    public DateTime IssuedAt { get; set; }

    /// <summary>
    /// When the token expires
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// User information
    /// </summary>
    public UserDto User { get; set; } = new();
}

/// <summary>
/// Refresh token request DTO
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// Refresh token to exchange for new access token
    /// </summary>
    [Required]
    public string RefreshTokens { get; set; } = string.Empty;

    /// <summary>
    /// Client IP address for security logging
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// User agent for security logging
    /// </summary>
    public string? UserAgent { get; set; }
}

/// <summary>
/// SSO callback DTO
/// </summary>
public class SsoCallbackDto
{
    /// <summary>
    /// Authorization code from OIDC provider
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Error from OIDC provider
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Error description from OIDC provider
    /// </summary>
    public string? ErrorDescription { get; set; }

    /// <summary>
    /// State parameter for CSRF protection
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Client IP address for security logging
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// User agent for security logging
    /// </summary>
    public string? UserAgent { get; set; }
}
