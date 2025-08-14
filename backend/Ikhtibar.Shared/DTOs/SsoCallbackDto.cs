using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for SSO callback request
/// </summary>
public class SsoCallbackDto
{
    /// <summary>
    /// Authorization code from OIDC provider
    /// </summary>
    [Required]
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// State value used to prevent CSRF
    /// </summary>
    [Required]
    public string State { get; set; } = string.Empty;
    
    /// <summary>
    /// Redirect URI used in authorization request
    /// </summary>
    [Required]
    public string RedirectUri { get; set; } = string.Empty;
    
    /// <summary>
    /// PKCE code verifier (if PKCE was used)
    /// </summary>
    public string? CodeVerifier { get; set; }
    
    /// <summary>
    /// Error code (if authorization failed)
    /// </summary>
    public string? Error { get; set; }
    
    /// <summary>
    /// Error description (if authorization failed)
    /// </summary>
    public string? ErrorDescription { get; set; }
    public string UserAgent { get; set; } = string.Empty;
    public string? ClientIpAddress { get; set; }
}
