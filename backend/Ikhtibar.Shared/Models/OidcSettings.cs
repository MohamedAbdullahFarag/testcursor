namespace Ikhtibar.Shared.Models;

/// <summary>
/// Configuration settings for OpenID Connect authentication
/// </summary>
public class OidcSettings
{
    /// <summary>
    /// OIDC authority URL (identity provider)
    /// </summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    /// Client ID registered with the OIDC provider
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret for confidential clients
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Redirect URI for OIDC callbacks
    /// </summary>
    public string RedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// Post-logout redirect URI
    /// </summary>
    public string PostLogoutRedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// Scopes to request during authentication
    /// </summary>
    public string[] Scopes { get; set; } = { "openid", "profile", "email" };

    /// <summary>
    /// Response type for OIDC flow
    /// </summary>
    public string ResponseType { get; set; } = "code";

    /// <summary>
    /// Response mode for OIDC flow
    /// </summary>
    public string ResponseMode { get; set; } = "query";

    /// <summary>
    /// Whether to use PKCE (Proof Key for Code Exchange)
    /// </summary>
    public bool UsePkce { get; set; } = true;

    /// <summary>
    /// Whether to save tokens in authentication properties
    /// </summary>
    public bool SaveTokens { get; set; } = true;

    /// <summary>
    /// Whether to get claims from user info endpoint
    /// </summary>
    public bool GetClaimsFromUserInfoEndpoint { get; set; } = true;

    /// <summary>
    /// Metadata address for OIDC discovery
    /// </summary>
    public string? MetadataAddress { get; set; }

    /// <summary>
    /// Whether to require HTTPS for metadata address
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;
}
