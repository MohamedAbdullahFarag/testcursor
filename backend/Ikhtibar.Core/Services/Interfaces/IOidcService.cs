using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.DTOs.Authentication;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for OpenID Connect authentication operations
/// </summary>
public interface IOidcService
{
    Task<OidcTokenResponse> ExchangeCodeAsync(string code);
    Task<OidcUserInfo> GetUserInfoAsync(string accessToken);
    Task<string> GetAuthorizationUrlAsync(string state, string redirectUri);
}
