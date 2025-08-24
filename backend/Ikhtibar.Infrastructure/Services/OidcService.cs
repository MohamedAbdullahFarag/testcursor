using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs.Authentication;
using Ikhtibar.Shared.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Services
{
    public class OidcService : IOidcService
    {
        private readonly HttpClient _httpClient;
        private readonly OidcSettings _oidcSettings;
        private readonly ILogger<OidcService> _logger;

        public OidcService(
            HttpClient httpClient,
            IOptions<OidcSettings> oidcSettings,
            ILogger<OidcService> logger)
        {
            _httpClient = httpClient;
            _oidcSettings = oidcSettings.Value;
            _logger = logger;
        }

        public async Task<OidcTokenResponse> ExchangeCodeAsync(string code)
        {
            try
            {
                var tokenRequest = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", _oidcSettings.ClientId),
                    new KeyValuePair<string, string>("client_secret", _oidcSettings.ClientSecret),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", _oidcSettings.Authority + "/callback")
                });

                var response = await _httpClient.PostAsync("/token", tokenRequest);
                response.EnsureSuccessStatusCode();

                var tokenResponse = await response.Content.ReadFromJsonAsync<OidcTokenResponse>();
                if (tokenResponse == null)
                {
                    throw new InvalidOperationException("Failed to deserialize token response");
                }

                _logger.LogInformation("Successfully exchanged authorization code for tokens");
                return tokenResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exchanging authorization code for tokens");
                throw;
            }
        }

        public async Task<OidcUserInfo> GetUserInfoAsync(string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.GetAsync("/userinfo");
                response.EnsureSuccessStatusCode();

                var userInfo = await response.Content.ReadFromJsonAsync<OidcUserInfo>();
                if (userInfo == null)
                {
                    throw new InvalidOperationException("Failed to deserialize user info response");
                }

                _logger.LogInformation("Successfully retrieved user info from OIDC provider");
                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user info from OIDC provider");
                throw;
            }
        }

        public async Task<string> GetAuthorizationUrlAsync(string state, string redirectUri)
        {
            try
            {
                var codeChallenge = await GenerateCodeChallengeAsync();
                
                var queryParams = new List<string>
                {
                    $"client_id={Uri.EscapeDataString(_oidcSettings.ClientId)}",
                    $"response_type={Uri.EscapeDataString(_oidcSettings.ResponseType)}",
                    $"scope={Uri.EscapeDataString(string.Join(" ", _oidcSettings.Scopes))}",
                    $"redirect_uri={Uri.EscapeDataString(redirectUri)}",
                    $"state={Uri.EscapeDataString(state)}"
                };

                if (_oidcSettings.UsePkce)
                {
                    queryParams.Add($"code_challenge={Uri.EscapeDataString(codeChallenge)}");
                    queryParams.Add("code_challenge_method=S256");
                }

                var authorizationUrl = $"{_oidcSettings.Authority}/authorize?{string.Join("&", queryParams)}";
                
                _logger.LogDebug("Generated OIDC authorization URL");
                return authorizationUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating OIDC authorization URL");
                throw;
            }
        }

        private async Task<string> GenerateCodeChallengeAsync()
        {
            try
            {
                var codeVerifier = GenerateRandomString(128);
                var codeChallenge = await GenerateSha256HashAsync(codeVerifier);
                return codeChallenge;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PKCE code challenge");
                throw;
            }
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async Task<string> GenerateSha256HashAsync(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = await Task.Run(() => sha256.ComputeHash(bytes));
            return Convert.ToBase64String(hash)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }
    }
}
