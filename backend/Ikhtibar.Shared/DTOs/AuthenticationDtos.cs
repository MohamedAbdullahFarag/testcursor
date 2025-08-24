using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for logout request
/// </summary>
public class LogoutDto
{
    /// <summary>
    /// Refresh token to invalidate
    /// </summary>
    public string? RefreshTokens { get; set; }

    /// <summary>
    /// Indicates if user should be logged out from all devices
    /// </summary>
    public bool LogoutFromAllDevices { get; set; } = false;
}

/// <summary>
/// DTO for password reset request
/// </summary>
public class PasswordResetRequestDto
{
    /// <summary>
    /// Email address for password reset
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Valid email address is required")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// DTO for password reset
/// </summary>
public class PasswordResetDto
{
    /// <summary>
    /// Password reset token
    /// </summary>
    [Required(ErrorMessage = "Reset token is required")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// New password
    /// </summary>
    [Required(ErrorMessage = "New password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirm new password
    /// </summary>
    [Required(ErrorMessage = "Password confirmation is required")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// DTO for change password request
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// Current password
    /// </summary>
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password
    /// </summary>
    [Required(ErrorMessage = "New password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirm new password
    /// </summary>
    [Required(ErrorMessage = "Password confirmation is required")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
