using System.Security.Cryptography;
using System.Text;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.DTOs;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace Ikhtibar.Core.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IOidcService _oidcService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IOidcService oidcService,
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<AuthenticationService> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _oidcService = oidcService;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<AuthResultDto> AuthenticateAsync(LoginDto request)
        {
            try
            {
                _logger.LogInformation("Authentication attempt for user: {Email}", request.Email);

                // Validate input
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "Email and password are required"
                    };
                }

                // Get user by email
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("Authentication failed: User not found for email: {Email}", request.Email);
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "Invalid email or password"
                    };
                }

                // Debug: Log user roles for troubleshooting
                _logger.LogInformation("Authentication Debug - User ID: {UserId}, UserRoles count: {RoleCount}", 
                    user.UserId, user.UserRoles?.Count ?? 0);
                if (user.UserRoles != null)
                {
                    foreach (var userRole in user.UserRoles)
                    {
                        _logger.LogInformation("Authentication Debug - UserRole: UserId={UserId}, RoleId={RoleId}, Role.Code={RoleCode}, Role.Name={RoleName}", 
                            userRole.UserId, userRole.RoleId, userRole.Role?.Code, userRole.Role?.Name);
                    }
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Authentication failed: Inactive user for email: {Email}", request.Email);
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "Account is deactivated"
                    };
                }

                // Verify password
                if (request.Password != user.PasswordHash)//!VerifyPassword(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Authentication failed: Invalid password for user: {Email}", request.Email);
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "Invalid email or password"
                    };
                }

                // Check if email is verified (if required)
                if (!user.EmailVerified)
                {
                    _logger.LogWarning("Authentication failed: Unverified email for user: {Email}", request.Email);
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "Please verify your email address before logging in"
                    };
                }

                // Generate JWT token
                var accessToken = await _tokenService.GenerateJwtAsync(user);
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

                // Store refresh token
                var refreshTokenEntity = new RefreshTokens
                {
                    UserId = user.UserId,
                    TokenHash = HashToken(refreshToken),
                    ExpiresAt = DateTime.UtcNow.AddDays(30), // 30 days
                    IssuedAt = DateTime.UtcNow,
                    ClientIpAddress = request.ClientIpAddress,
                    UserAgent = request.UserAgent
                };

                await _refreshTokenRepository.AddAsync(refreshTokenEntity);

                // Get user roles
                var roles = user.UserRoles?
                    .Where(ur => ur.Role != null && !string.IsNullOrEmpty(ur.Role.Name))
                    .Select(ur => ur.Role!.Name)
                    .ToList() ?? new List<string>();

                _logger.LogInformation("Authentication successful for user: {Email}", request.Email);

                return new AuthResultDto
                {
                    Success = true,
                    User = new UserDto
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Username = user.Username,
                        IsActive = user.IsActive,
                        EmailVerified = user.EmailVerified,
                        CreatedAt = user.CreatedAt,
                        ModifiedAt = user.ModifiedAt ?? user.CreatedAt
                    },
                    AccessToken = accessToken,
                    RefreshTokens = refreshToken,
                    TokenType = "Bearer",
                    ExpiresIn = 3600, // 1 hour
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IssuedAt = DateTime.UtcNow,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for user: {Email}", request.Email);
                return new AuthResultDto
                {
                    Success = false,
                    ErrorMessage = "Authentication failed due to a system error"
                };
            }
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var tokenPrefix = refreshToken != null && refreshToken.Length >= 8 ? refreshToken[..8] : refreshToken;
                _logger.LogInformation("Token refresh attempt for token: {TokenPrefix}", tokenPrefix);

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "Refresh token is required"
                    };
                }

                // Hash the refresh token
                var tokenHash = HashToken(refreshToken);

                // Find the refresh token in database
                var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);
                if (storedToken == null)
                {
                    _logger.LogWarning("Token refresh failed: Invalid refresh token");
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "Invalid refresh token"
                    };
                }

                // Check if token is revoked
                if (storedToken.IsRevoked)
                {
                    _logger.LogWarning("Token refresh failed: Token is revoked");
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "Refresh token has been revoked"
                    };
                }

                // Check if token is expired
                if (storedToken.ExpiresAt <= DateTime.UtcNow)
                {
                    _logger.LogWarning("Token refresh failed: Token is expired");
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "Refresh token has expired"
                    };
                }

                // Get user
                var user = await _userRepository.GetByIdWithRolesAsync(storedToken.UserId);
                if (user == null || !user.IsActive)
                {
                    _logger.LogWarning("Token refresh failed: User not found or inactive");
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "User account is not available"
                    };
                }

                // Generate new tokens
                var newAccessToken = await _tokenService.GenerateJwtAsync(user);
                var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();

                // Revoke old refresh token
                await _refreshTokenRepository.RevokeByTokenHashAsync(tokenHash);

                // Store new refresh token
                var newRefreshTokenEntity = new RefreshTokens
                {
                    UserId = user.UserId,
                    TokenHash = HashToken(newRefreshToken),
                    ExpiresAt = DateTime.UtcNow.AddDays(30),
                    IssuedAt = DateTime.UtcNow,
                    ClientIpAddress = storedToken.ClientIpAddress,
                    UserAgent = storedToken.UserAgent
                };

                await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

                // Get user roles
                var roles = user.UserRoles?
                    .Where(ur => ur.Role != null && !string.IsNullOrEmpty(ur.Role.Name))
                    .Select(ur => ur.Role!.Name)
                    .ToList() ?? new List<string>();

                _logger.LogInformation("Token refresh successful for user: {Email}", user.Email);

                return new AuthResultDto
                {
                    Success = true,
                    User = new UserDto
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Username = user.Username,
                        IsActive = user.IsActive,
                        EmailVerified = user.EmailVerified,
                        CreatedAt = user.CreatedAt,
                        ModifiedAt = user.ModifiedAt ?? user.CreatedAt
                    },
                    AccessToken = newAccessToken,
                    RefreshTokens = newRefreshToken,
                    TokenType = "Bearer",
                    ExpiresIn = 3600,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IssuedAt = DateTime.UtcNow,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return new AuthResultDto
                {
                    Success = false,
                    ErrorMessage = "Token refresh failed due to a system error"
                };
            }
        }

        public async Task<AuthResultDto> ProcessSsoCallbackAsync(SsoCallbackDto callbackData)
        {
            try
            {
                _logger.LogInformation("Processing SSO callback for user");

                // Validate callback data
                if (string.IsNullOrWhiteSpace(callbackData.Code))
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "Authorization code is required"
                    };
                }

                // Exchange authorization code for tokens
                var tokenResponse = await _oidcService.ExchangeCodeAsync(callbackData.Code);
                if (tokenResponse == null)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "Failed to exchange authorization code for tokens"
                    };
                }

                // Get user info from OIDC provider
                var userInfo = await _oidcService.GetUserInfoAsync(tokenResponse.AccessToken);
                if (userInfo == null)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        ErrorMessage = "Failed to retrieve user information"
                    };
                }

                // Find or create user
                var user = await _userRepository.GetByEmailAsync(userInfo.Email ?? string.Empty);
                if (user == null)
                {
                    // Create new user from SSO
                    var normalizedEmail = userInfo.Email ?? string.Empty;
                    user = new User
                    {
                        Email = normalizedEmail,
                        FirstName = userInfo.GivenName ?? userInfo.Name?.Split(' ').FirstOrDefault() ?? "",
                        LastName = userInfo.FamilyName ?? userInfo.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "",
                        Username = normalizedEmail.Split('@')[0], // Use email prefix as username
                        IsActive = true,
                        EmailVerified = userInfo.EmailVerified,
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow
                    };

                    await _userRepository.AddAsync(user);
                    _logger.LogInformation("Created new user from SSO: {Email}", user.Email);
                }

                // Generate JWT token
                var accessToken = await _tokenService.GenerateJwtAsync(user);
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

                // Store refresh token
                var refreshTokenEntity = new RefreshTokens
                {
                    UserId = user.UserId,
                    TokenHash = HashToken(refreshToken),
                    ExpiresAt = DateTime.UtcNow.AddDays(30),
                    IssuedAt = DateTime.UtcNow,
                    ClientIpAddress = callbackData.ClientIpAddress,
                    UserAgent = callbackData.UserAgent
                };

                await _refreshTokenRepository.AddAsync(refreshTokenEntity);

                // Get user roles
                var roles = user.UserRoles?
                    .Where(ur => ur.Role != null && !string.IsNullOrEmpty(ur.Role.Name))
                    .Select(ur => ur.Role!.Name)
                    .ToList() ?? new List<string>();

                _logger.LogInformation("SSO authentication successful for user: {Email}", user.Email);

                return new AuthResultDto
                {
                    Success = true,
                    User = new UserDto
                    {
                        UserId = user.UserId,
                        Email = user.Email ?? string.Empty,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Username = user.Username,
                        IsActive = user.IsActive,
                        EmailVerified = user.EmailVerified,
                        CreatedAt = user.CreatedAt,
                        ModifiedAt = user.ModifiedAt ?? user.CreatedAt
                    },
                    AccessToken = accessToken,
                    RefreshTokens = refreshToken,
                    TokenType = "Bearer",
                    ExpiresIn = 3600,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IssuedAt = DateTime.UtcNow,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing SSO callback");
                return new AuthResultDto
                {
                    Success = false,
                    ErrorMessage = "SSO authentication failed due to a system error"
                };
            }
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            try
            {
                var tokenHash = HashToken(refreshToken);
                var result = await _refreshTokenRepository.RevokeByTokenHashAsync(tokenHash);

                if (result)
                {
                    _logger.LogInformation("Refresh token revoked successfully");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token");
                return false;
            }
        }

        public async Task<bool> ValidateTokenAsync(string accessToken)
        {
            try
            {
                return await _tokenService.IsTokenValidAsync(accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating access token");
                return false;
            }
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            try
            {
                // Use BCrypt for password verification
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying password");
                return false;
            }
        }

        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
