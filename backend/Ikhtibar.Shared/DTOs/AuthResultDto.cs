using System;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for authentication result
/// </summary>
public class AuthResultDto
{
    /// <summary>
    /// Whether authentication was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Error message if authentication failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// JWT access token
    /// </summary>
    public string? AccessToken { get; set; }
    
    /// <summary>
    /// Refresh token
    /// </summary>
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// Token expiration time
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// Authenticated user information
    /// </summary>
    public UserDto? User { get; set; }
    
    /// <summary>
    /// List of user roles
    /// </summary>
    public List<string>? Roles { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
    public DateTime IssuedAt { get; set; }
}
