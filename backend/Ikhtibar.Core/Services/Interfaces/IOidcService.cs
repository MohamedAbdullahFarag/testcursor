using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for OpenID Connect authentication operations
/// </summary>
public interface IOidcService
{
    /// <summary>
    /// Exchanges authorization code for tokens
    /// </summary>
    /// <param name="code">Authorization code</param>
    /// <param name="redirectUri">Redirect URI used in the authorization request</param>
    /// <param name="codeVerifier">PKCE code verifier (optional)</param>
    /// <returns>Token response</returns>
    Task<OidcTokenResponse> ExchangeCodeAsync(string code, string redirectUri, string? codeVerifier = null);

    /// <summary>
    /// Gets user information using access token
    /// </summary>
    /// <param name="accessToken">Access token</param>
    /// <returns>User information</returns>
    Task<OidcUserInfo> GetUserInfoAsync(string accessToken);

    /// <summary>
    /// Validates an access token
    /// </summary>
    /// <param name="accessToken">Access token to validate</param>
    /// <returns>True if token is valid, false otherwise</returns>
    Task<bool> ValidateTokenAsync(string accessToken);

    /// <summary>
    /// Refreshes an access token using refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>New token response</returns>
    Task<OidcTokenResponse> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Revokes a token
    /// </summary>
    /// <param name="token">Token to revoke</param>
    /// <param name="tokenType">Type of token (access_token or refresh_token)</param>
    /// <returns>True if revocation was successful</returns>
    Task<bool> RevokeTokenAsync(string token, string tokenType = "access_token");

    /// <summary>
    /// Generates authorization URL for OIDC flow
    /// </summary>
    /// <param name="state">State parameter for security</param>
    /// <param name="codeChallenge">PKCE code challenge (optional)</param>
    /// <param name="codeChallengeMethod">PKCE code challenge method (optional)</param>
    /// <returns>Authorization URL</returns>
    Task<string> GetAuthorizationUrlAsync(string state, string? codeChallenge = null, string? codeChallengeMethod = null);

    /// <summary>
    /// Generates PKCE parameters for secure authorization
    /// </summary>
    /// <returns>Code verifier and code challenge</returns>
    Task<(string codeVerifier, string codeChallenge)> GeneratePkceParametersAsync();
}
