using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for login request
/// </summary>
public class LoginDto
{
    /// <summary>
    /// User email
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
    /// Remember me flag
    /// </summary>
    public bool RememberMe { get; set; } = false;
    
    /// <summary>
    /// Client IP address (populated by the server)
    /// </summary>
    public string? ClientIpAddress { get; set; }
    
    /// <summary>
    /// User agent (populated by the server)
    /// </summary>
    public string? UserAgent { get; set; }
}
