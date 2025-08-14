namespace Ikhtibar.Shared.Models;

/// <summary>
/// General authentication configuration settings
/// </summary>
public class AuthSettings
{
    /// <summary>
    /// Whether to enable two-factor authentication
    /// </summary>
    public bool EnableTwoFactor { get; set; } = false;

    /// <summary>
    /// Number of failed login attempts before account lockout
    /// </summary>
    public int LockoutThreshold { get; set; } = 5;

    /// <summary>
    /// Duration of account lockout in minutes
    /// </summary>
    public int LockoutDurationMinutes { get; set; } = 15;

    /// <summary>
    /// Whether to enable account lockout
    /// </summary>
    public bool EnableAccountLockout { get; set; } = true;

    /// <summary>
    /// Password minimum length requirement
    /// </summary>
    public int PasswordMinimumLength { get; set; } = 8;

    /// <summary>
    /// Whether to require digits in password
    /// </summary>
    public bool PasswordRequireDigit { get; set; } = true;

    /// <summary>
    /// Whether to require lowercase letters in password
    /// </summary>
    public bool PasswordRequireLowercase { get; set; } = true;

    /// <summary>
    /// Whether to require uppercase letters in password
    /// </summary>
    public bool PasswordRequireUppercase { get; set; } = true;

    /// <summary>
    /// Whether to require special characters in password
    /// </summary>
    public bool PasswordRequireSpecialCharacter { get; set; } = true;

    /// <summary>
    /// Maximum number of refresh tokens per user
    /// </summary>
    public int MaxRefreshTokensPerUser { get; set; } = 5;

    /// <summary>
    /// Whether to enable refresh token rotation
    /// </summary>
    public bool EnableRefreshTokenRotation { get; set; } = true;

    /// <summary>
    /// Default session timeout in minutes
    /// </summary>
    public int DefaultSessionTimeoutMinutes { get; set; } = 30;

    /// <summary>
    /// Whether to require email verification
    /// </summary>
    public bool RequireEmailVerification { get; set; } = true;

    /// <summary>
    /// Whether to allow multiple concurrent sessions
    /// </summary>
    public bool AllowMultipleSessions { get; set; } = true;
}
