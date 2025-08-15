using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for authentication operations
/// Following SRP: ONLY authentication business logic
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    /// <param name="request">Login request with credentials</param>
    /// <returns>Authentication result with tokens and user data, or null if authentication fails</returns>
    Task<AuthResult?> AuthenticateAsync(LoginRequest request);

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to use</param>
    /// <returns>New authentication result with refreshed tokens, or null if refresh fails</returns>
    Task<AuthResult?> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Process SSO callback and authenticate user
    /// </summary>
    /// <param name="callbackData">SSO callback data</param>
    /// <returns>Authentication result with tokens and user data, or null if processing fails</returns>
    Task<AuthResult?> ProcessSsoCallbackAsync(SsoCallbackData callbackData);

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <returns>True if token was successfully revoked, false otherwise</returns>
    Task<bool> RevokeTokenAsync(string refreshToken);

    /// <summary>
    /// Validate access token
    /// </summary>
    /// <param name="accessToken">Access token to validate</param>
    /// <returns>True if token is valid, false otherwise</returns>
    Task<bool> ValidateTokenAsync(string accessToken);

    /// <summary>
    /// Logout user and revoke all tokens
    /// </summary>
    /// <param name="userId">User ID to logout</param>
    /// <returns>True if logout was successful, false otherwise</returns>
    Task<bool> LogoutAsync(int userId);
}

/// <summary>
/// Authentication request model
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ClientIpAddress { get; set; }
    public string? UserAgent { get; set; }
}

/// <summary>
/// SSO callback data model
/// </summary>
public class SsoCallbackData
{
    public string Code { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string? Error { get; set; }
    public string? ErrorDescription { get; set; }
    public string? RedirectUri { get; set; }
}

/// <summary>
/// Authentication result model
/// </summary>
public class AuthResult
{
    public UserDto User { get; set; } = null!;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}
