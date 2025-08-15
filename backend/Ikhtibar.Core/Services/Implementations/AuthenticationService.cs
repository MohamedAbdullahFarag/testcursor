using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Implementations;

/// <summary>
/// Authentication service implementation
/// Following SRP: ONLY authentication business logic
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IOidcService _oidcService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly AuthSettings _authSettings;

    public AuthenticationService(
        IUserService userService,
        ITokenService tokenService,
        IOidcService oidcService,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<AuthSettings> authSettings,
        ILogger<AuthenticationService> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _oidcService = oidcService ?? throw new ArgumentNullException(nameof(oidcService));
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        _authSettings = authSettings?.Value ?? throw new ArgumentNullException(nameof(authSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    public async Task<AuthResult?> AuthenticateAsync(LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Authentication attempt for email: {Email}", request.Email);

            // Validate request
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Invalid authentication request: missing email or password");
                return null;
            }

            // Authenticate user via user service
            var user = await _userService.AuthenticateAsync(request.Email, request.Password);
            if (user == null)
            {
                _logger.LogWarning("Authentication failed for email: {Email}", request.Email);
                return null;
            }

            // Check if user is active
            if (!user.IsActive)
            {
                _logger.LogWarning("Authentication failed for inactive user: {Email}", request.Email);
                return null;
            }

            // Generate tokens
            var accessToken = await _tokenService.GenerateJwtAsync(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

            // Store refresh token
            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = HashToken(refreshToken),
                UserId = user.UserId,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_authSettings.RefreshTokenExpirationDays),
                ClientIpAddress = request.ClientIpAddress,
                UserAgent = request.UserAgent
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            // Log successful authentication
            _logger.LogInformation("Authentication successful for user: {Email} (ID: {UserId})", 
                request.Email, user.UserId);

            return new AuthResult
            {
                User = user,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_authSettings.AccessTokenExpirationMinutes),
                TokenType = "Bearer"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication for email: {Email}", request.Email);
            throw;
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    public async Task<AuthResult?> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("Refresh token is null or empty");
                return null;
            }

            // Hash the provided token for comparison
            var tokenHash = HashToken(refreshToken);

            // Get stored refresh token
            var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);
            if (storedToken == null)
            {
                _logger.LogWarning("Refresh token not found in database");
                return null;
            }

            // Validate token
            if (storedToken.IsExpired || storedToken.IsRevoked)
            {
                _logger.LogWarning("Refresh token is expired or revoked for user: {UserId}", storedToken.UserId);
                return null;
            }

            // Get user information
            var user = await _userService.GetUserAsync(storedToken.UserId);
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("User not found or inactive for refresh token: {UserId}", storedToken.UserId);
                return null;
            }

            // Revoke old refresh token
            await _refreshTokenRepository.RevokeByTokenHashAsync(tokenHash, "Token refreshed");

            // Generate new tokens
            var newAccessToken = await _tokenService.GenerateJwtAsync(user);
            var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();

            // Store new refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                TokenHash = HashToken(newRefreshToken),
                UserId = user.UserId,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_authSettings.RefreshTokenExpirationDays),
                ClientIpAddress = storedToken.ClientIpAddress,
                UserAgent = storedToken.UserAgent
            };

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

            _logger.LogInformation("Token refreshed successfully for user: {UserId}", user.UserId);

            return new AuthResult
            {
                User = user,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_authSettings.AccessTokenExpirationMinutes),
                TokenType = "Bearer"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            throw;
        }
    }

    /// <summary>
    /// Process SSO callback and authenticate user
    /// </summary>
    public async Task<AuthResult?> ProcessSsoCallbackAsync(SsoCallbackData callbackData)
    {
        try
        {
            _logger.LogInformation("Processing SSO callback for state: {State}", callbackData.State);

            // Check for OIDC errors
            if (!string.IsNullOrEmpty(callbackData.Error))
            {
                _logger.LogWarning("OIDC callback error: {Error} - {Description}", 
                    callbackData.Error, callbackData.ErrorDescription);
                return null;
            }

            // Exchange authorization code for tokens
            var redirectUri = callbackData.RedirectUri ?? "http://localhost:3000/callback";
            var tokenResponse = await _oidcService.ExchangeCodeAsync(callbackData.Code, redirectUri);
            if (tokenResponse == null)
            {
                _logger.LogWarning("Failed to exchange authorization code for tokens");
                return null;
            }

            // Get user information from OIDC provider
            var oidcUserInfo = await _oidcService.GetUserInfoAsync(tokenResponse.AccessToken);
            if (oidcUserInfo == null)
            {
                _logger.LogWarning("Failed to get user info from OIDC provider");
                return null;
            }

            // Find or create local user
            if (string.IsNullOrWhiteSpace(oidcUserInfo.Email))
            {
                _logger.LogWarning("OIDC user info missing email address");
                return null;
            }
            
            var user = await _userService.GetUserByEmailAsync(oidcUserInfo.Email);
            if (user == null)
            {
                // Create new user from OIDC info
                user = await CreateUserFromOidcAsync(oidcUserInfo);
                if (user == null)
                {
                    _logger.LogWarning("Failed to create user from OIDC info for email: {Email}", oidcUserInfo.Email);
                    return null;
                }
            }

            // Check if user is active
            if (!user.IsActive)
            {
                _logger.LogWarning("OIDC user is inactive: {Email}", user.Email);
                return null;
            }

            // Generate local tokens
            var accessToken = await _tokenService.GenerateJwtAsync(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

            // Store refresh token
            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = HashToken(refreshToken),
                UserId = user.UserId,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_authSettings.RefreshTokenExpirationDays),
                ClientIpAddress = null, // OIDC flow doesn't provide this
                UserAgent = null // OIDC flow doesn't provide this
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            _logger.LogInformation("SSO authentication successful for user: {Email} (ID: {UserId})", 
                user.Email, user.UserId);

            return new AuthResult
            {
                User = user,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_authSettings.AccessTokenExpirationMinutes),
                TokenType = "Bearer"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SSO callback");
            throw;
        }
    }

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            var tokenHash = HashToken(refreshToken);
            var result = await _refreshTokenRepository.RevokeByTokenHashAsync(tokenHash, "Token revoked by user");

            if (result)
            {
                _logger.LogInformation("Refresh token revoked successfully");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token");
            throw;
        }
    }

    /// <summary>
    /// Validate access token
    /// </summary>
    public async Task<bool> ValidateTokenAsync(string accessToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return false;
            }

            return await _tokenService.ValidateTokenAsync(accessToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating access token");
            return false;
        }
    }

    /// <summary>
    /// Logout user and revoke all tokens
    /// </summary>
    public async Task<bool> LogoutAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Logging out user: {UserId}", userId);

            // Revoke all refresh tokens for the user
            var result = await _refreshTokenRepository.RevokeAllByUserIdAsync(userId, "User logout");

            if (result)
            {
                _logger.LogInformation("Successfully logged out user: {UserId}", userId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
            throw;
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Hash refresh token for secure storage
    /// </summary>
    private static string HashToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }



    /// <summary>
    /// Create new user from OIDC information
    /// </summary>
    private Task<UserDto?> CreateUserFromOidcAsync(OidcUserInfo oidcUserInfo)
    {
        try
        {
            // This would typically involve creating a new user with default settings
            // For now, we'll return null to indicate user creation is not implemented
            _logger.LogWarning("User creation from OIDC not implemented for email: {Email}", oidcUserInfo.Email);
            return Task.FromResult<UserDto?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user from OIDC info for email: {Email}", oidcUserInfo.Email);
            return Task.FromResult<UserDto?>(null);
        }
    }

    #endregion
}
