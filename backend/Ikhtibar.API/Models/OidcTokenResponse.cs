namespace Ikhtibar.API.Models;

/// <summary>
/// Response model for OIDC token exchange
/// </summary>
public class OidcTokenResponse
{
    /// <summary>
    /// Access token received from OIDC provider
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token received from OIDC provider
    /// </summary>
    public string? RefreshTokens { get; set; }

    /// <summary>
    /// ID token received from OIDC provider
    /// </summary>
    public string? IdToken { get; set; }

    /// <summary>
    /// Token type (typically "Bearer")
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Access token expiration time in seconds
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Scopes granted by the OIDC provider
    /// </summary>
    public string? Scope { get; set; }

    /// <summary>
    /// Error code if token exchange failed
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Error description if token exchange failed
    /// </summary>
    public string? ErrorDescription { get; set; }
}
