using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Implementation of OpenID Connect authentication services
/// </summary>
public class OidcService : IOidcService
{
    private readonly HttpClient _httpClient;
    private readonly OidcSettings _oidcSettings;
    // private readonly ILogger<OidcService> _logger;

    public OidcService(
        HttpClient httpClient,
        IOptions<OidcSettings> oidcSettings)
    // ILogger<OidcService> logger)
    {
        _httpClient = httpClient;
        _oidcSettings = oidcSettings.Value;
        // _logger = logger;
    }

    /// <summary>
    /// Exchange authorization code for access token
    /// </summary>
    /// <param name="code">Authorization code from OIDC provider</param>
    /// <param name="redirectUri">Redirect URI used in authorization request</param>
    /// <param name="codeVerifier">PKCE code verifier (optional)</param>
    /// <returns>Token response from OIDC provider</returns>
    public async Task<OidcTokenResponse> ExchangeCodeAsync(string code, string redirectUri, string? codeVerifier = null)
    {
        try
        {
            // //             // _logger.LogInformation("Exchanging authorization code for access token");

            var tokenEndpoint = $"{_oidcSettings.Authority.TrimEnd('/')}/protocol/openid-connect/token";

            var parameters = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"client_id", _oidcSettings.ClientId},
                {"client_secret", _oidcSettings.ClientSecret},
                {"code", code},
                {"redirect_uri", redirectUri}
            };

            // Add PKCE code verifier if provided
            if (!string.IsNullOrEmpty(codeVerifier))
            {
                parameters.Add("code_verifier", codeVerifier);
            }

            var content = new FormUrlEncodedContent(parameters);

            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // _logger.LogError("Token exchange failed. Status: {StatusCode}, Response: {Response}", 
                //     response.StatusCode, responseContent);

                var errorResponse = JsonSerializer.Deserialize<OidcTokenResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return errorResponse ?? new OidcTokenResponse
                {
                    Error = "token_exchange_failed",
                    ErrorDescription = $"HTTP {response.StatusCode}: {response.ReasonPhrase}"
                };
            }

            var tokenResponse = JsonSerializer.Deserialize<OidcTokenResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //             _logger.LogInformation("Token exchange successful");
            return tokenResponse ?? throw new InvalidOperationException("Failed to deserialize token response");
        }
        catch (Exception)
        {
            //             _logger.LogError(ex, "Error during token exchange");
            return new OidcTokenResponse
            {
                Error = "internal_error",
                ErrorDescription = "An internal error occurred during token exchange"
            };
        }
    }

    /// <summary>
    /// Get user information from OIDC provider
    /// </summary>
    /// <param name="accessToken">Access token from OIDC provider</param>
    /// <returns>User information from OIDC provider</returns>
    public async Task<OidcUserInfo> GetUserInfoAsync(string accessToken)
    {
        try
        {
            //             _logger.LogInformation("Retrieving user information from OIDC provider");

            var userInfoEndpoint = $"{_oidcSettings.Authority.TrimEnd('/')}/protocol/openid-connect/userinfo";

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync(userInfoEndpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // _logger.LogError("UserInfo request failed. Status: {StatusCode}, Response: {Response}", 
                //     response.StatusCode, responseContent);
                throw new HttpRequestException($"UserInfo request failed: {response.StatusCode}");
            }

            var userInfo = JsonSerializer.Deserialize<OidcUserInfo>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //             _logger.LogInformation("User information retrieved successfully for subject: {Subject}", userInfo?.Sub);
            return userInfo ?? throw new InvalidOperationException("Failed to deserialize user info response");
        }
        catch (Exception)
        {
            //             _logger.LogError(ex, "Error retrieving user information");
            throw;
        }
    }

    /// <summary>
    /// Validate access token with OIDC provider
    /// </summary>
    /// <param name="accessToken">Access token to validate</param>
    /// <returns>True if token is valid, false otherwise</returns>
    public async Task<bool> ValidateTokenAsync(string accessToken)
    {
        try
        {
            //             _logger.LogDebug("Validating access token with OIDC provider");

            var introspectionEndpoint = $"{_oidcSettings.Authority.TrimEnd('/')}/protocol/openid-connect/token/introspect";

            var parameters = new Dictionary<string, string>
            {
                {"token", accessToken},
                {"client_id", _oidcSettings.ClientId},
                {"client_secret", _oidcSettings.ClientSecret}
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(introspectionEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                //                 _logger.LogWarning("Token introspection failed. Status: {StatusCode}", response.StatusCode);
                return false;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var introspectionResult = JsonSerializer.Deserialize<JsonElement>(responseContent);

            if (introspectionResult.TryGetProperty("active", out var activeProperty))
            {
                var isActive = activeProperty.GetBoolean();
                //                 _logger.LogDebug("Token validation result: {IsActive}", isActive);
                return isActive;
            }

            return false;
        }
        catch (Exception)
        {
            //             _logger.LogError(ex, "Error validating access token");
            return false;
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>New token response</returns>
    public async Task<OidcTokenResponse> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            //             _logger.LogInformation("Refreshing access token using refresh token");

            var tokenEndpoint = $"{_oidcSettings.Authority.TrimEnd('/')}/protocol/openid-connect/token";

            var parameters = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"client_id", _oidcSettings.ClientId},
                {"client_secret", _oidcSettings.ClientSecret},
                {"refresh_token", refreshToken}
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // _logger.LogError("Token refresh failed. Status: {StatusCode}, Response: {Response}", 
                //     response.StatusCode, responseContent);

                var errorResponse = JsonSerializer.Deserialize<OidcTokenResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return errorResponse ?? new OidcTokenResponse
                {
                    Error = "token_refresh_failed",
                    ErrorDescription = $"HTTP {response.StatusCode}: {response.ReasonPhrase}"
                };
            }

            var tokenResponse = JsonSerializer.Deserialize<OidcTokenResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //             _logger.LogInformation("Token refresh successful");
            return tokenResponse ?? throw new InvalidOperationException("Failed to deserialize token response");
        }
        catch (Exception)
        {
            //             _logger.LogError(ex, "Error during token refresh");
            return new OidcTokenResponse
            {
                Error = "internal_error",
                ErrorDescription = "An internal error occurred during token refresh"
            };
        }
    }

    /// <summary>
    /// Revoke token at OIDC provider
    /// </summary>
    /// <param name="token">Token to revoke</param>
    /// <param name="tokenType">Type of token (access_token or refresh_token)</param>
    /// <returns>True if revocation successful, false otherwise</returns>
    public async Task<bool> RevokeTokenAsync(string token, string tokenType = "access_token")
    {
        try
        {
            //             _logger.LogInformation("Revoking {TokenType} at OIDC provider", tokenType);

            var revocationEndpoint = $"{_oidcSettings.Authority.TrimEnd('/')}/protocol/openid-connect/revoke";

            var parameters = new Dictionary<string, string>
            {
                {"token", token},
                {"token_type_hint", tokenType},
                {"client_id", _oidcSettings.ClientId},
                {"client_secret", _oidcSettings.ClientSecret}
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(revocationEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                //                 _logger.LogInformation("Token revocation successful");
                return true;
            }
            else
            {
                //                 _logger.LogWarning("Token revocation failed. Status: {StatusCode}", response.StatusCode);
                return false;
            }
        }
        catch (Exception)
        {
            //             _logger.LogError(ex, "Error during token revocation");
            return false;
        }
    }

    /// <summary>
    /// Get authorization URL for OIDC flow
    /// </summary>
    /// <param name="state">State parameter for CSRF protection</param>
    /// <param name="codeChallenge">PKCE code challenge (optional)</param>
    /// <param name="codeChallengeMethod">PKCE code challenge method (optional)</param>
    /// <returns>Authorization URL</returns>
    public Task<string> GetAuthorizationUrlAsync(string state, string? codeChallenge = null, string? codeChallengeMethod = null)
    {
        try
        {
            var authEndpoint = $"{_oidcSettings.Authority.TrimEnd('/')}/protocol/openid-connect/auth";

            var parameters = new Dictionary<string, string>
            {
                {"response_type", _oidcSettings.ResponseType},
                {"client_id", _oidcSettings.ClientId},
                {"redirect_uri", _oidcSettings.RedirectUri},
                {"scope", string.Join(" ", _oidcSettings.Scopes)},
                {"state", state},
                {"response_mode", _oidcSettings.ResponseMode}
            };

            // Add PKCE parameters if provided
            if (!string.IsNullOrEmpty(codeChallenge))
            {
                parameters.Add("code_challenge", codeChallenge);
                parameters.Add("code_challenge_method", codeChallengeMethod ?? "S256");
            }

            var queryString = string.Join("&", parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            var authorizationUrl = $"{authEndpoint}?{queryString}";

            //             _logger.LogDebug("Generated authorization URL");
            return Task.FromResult(authorizationUrl);
        }
        catch (Exception)
        {
            //             _logger.LogError(ex, "Error generating authorization URL");
            throw;
        }
    }

    /// <summary>
    /// Generate PKCE code verifier and challenge
    /// </summary>
    /// <returns>Tuple of (code verifier, code challenge)</returns>
    public Task<(string codeVerifier, string codeChallenge)> GeneratePkceParametersAsync()
    {
        try
        {
            // Generate code verifier (43-128 characters, URL-safe)
            var codeVerifierBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(codeVerifierBytes);
            var codeVerifier = Convert.ToBase64String(codeVerifierBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            // Generate code challenge (SHA256 hash of code verifier)
            using var sha256 = SHA256.Create();
            var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            var codeChallenge = Convert.ToBase64String(challengeBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            //             _logger.LogDebug("Generated PKCE parameters");
            return Task.FromResult((codeVerifier, codeChallenge));
        }
        catch (Exception)
        {
            //             _logger.LogError(ex, "Error generating PKCE parameters");
            throw;
        }
    }
}
