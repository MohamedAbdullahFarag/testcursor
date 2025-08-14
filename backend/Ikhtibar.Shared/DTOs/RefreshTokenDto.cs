using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for refresh token request
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// Refresh token
    /// </summary>
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional access token (may be expired)
    /// </summary>
    public string? AccessToken { get; set; }
    
    /// <summary>
    /// Client IP address (populated by the server)
    /// </summary>
    public string? ClientIpAddress { get; set; }
    
    /// <summary>
    /// User agent (populated by the server)
    /// </summary>
    public string? UserAgent { get; set; }
}
