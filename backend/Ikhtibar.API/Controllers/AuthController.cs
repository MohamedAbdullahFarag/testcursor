using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.API.Models;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Authentication controller for login, token refresh, and SSO operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IOidcService _oidcService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserService userService,
        ITokenService tokenService,
        IOidcService oidcService,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<AuthController> logger)
    {
        _userService = userService;
        _tokenService = tokenService;
        _oidcService = oidcService;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Authentication result with JWT tokens</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResultDto>> LoginAsync([FromBody] LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            // Set client info for security logging
            loginDto.ClientIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            loginDto.UserAgent = HttpContext.Request.Headers.UserAgent.ToString();

            // Authenticate user
            var user = await _userService.AuthenticateAsync(loginDto.Email, loginDto.Password);
            if (user == null)
            {
                _logger.LogWarning("Authentication failed for email: {Email}", loginDto.Email);
                return Unauthorized(new ProblemDetails
                {
                    Title = "Authentication Failed",
                    Detail = "Invalid email or password",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            // Generate JWT token - convert UserDto to User entity
            var userEntity = new User
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.UpdatedAt
            };
            var accessToken = await _tokenService.GenerateJwtAsync(userEntity);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

            // Hash and store refresh token
            var refreshTokenHash = HashToken(refreshToken);
            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = refreshTokenHash,
                UserId = user.UserId,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days from configuration
                CreatedAt = DateTime.UtcNow
            };

            var createdToken = await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            // Get user roles
            var userRoles = await _userService.GetUserRolesAsync(user.UserId);

            // Create response
            var authResult = new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = 15 * 60, // 15 minutes in seconds
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                User = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    PreferredLanguage = user.PreferredLanguage,
                    IsActive = user.IsActive,
                    EmailVerified = user.EmailVerified,
                    PhoneVerified = user.PhoneVerified,
                    Roles = userRoles.ToList(),
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                }
            };

            _logger.LogInformation("Login successful for user: {UserId}", user.UserId);
            return Ok(authResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An error occurred while processing your request",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="refreshTokenDto">Refresh token request</param>
    /// <returns>New authentication result with JWT tokens</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResultDto>> RefreshAsync([FromBody] RefreshTokenDto refreshTokenDto)
    {
        try
        {
            _logger.LogInformation("Token refresh attempt");

            // Set client info for security logging
            refreshTokenDto.ClientIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            refreshTokenDto.UserAgent = HttpContext.Request.Headers.UserAgent.ToString();

            // Hash the provided refresh token to compare with stored hash
            var refreshTokenHash = HashToken(refreshTokenDto.RefreshToken);

            // Validate refresh token
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenHash);
            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or expired refresh token");
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid Token",
                    Detail = "Refresh token is invalid or expired",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            // Get user
            var user = await _userService.GetUserByIdAsync(storedToken.UserId);
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("User not found or inactive for token refresh: {UserId}", storedToken.UserId);
                return Unauthorized(new ProblemDetails
                {
                    Title = "User Invalid",
                    Detail = "User account is not valid",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            // Revoke old token and generate new ones
            await _refreshTokenRepository.RevokeTokenAsync(storedToken.TokenHash);

            // Create User entity for token service
            var userEntity = new User
            {
                UserId = user.UserId,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                EmailVerified = user.EmailVerified,
                PhoneVerified = user.PhoneVerified,
                CreatedAt = user.CreatedAt
            };

            var newAccessToken = await _tokenService.GenerateJwtAsync(userEntity);
            var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();

            // Store new refresh token
            var newRefreshTokenHash = HashToken(newRefreshToken);
            var newRefreshTokenEntity = new RefreshToken
            {
                TokenHash = newRefreshTokenHash,
                UserId = user.UserId,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                ReplacedByToken = newRefreshTokenHash
            };

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

            // Get user roles
            var userRoles = await _userService.GetUserRolesAsync(user.UserId);

            // Create response
            var authResult = new AuthResultDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                TokenType = "Bearer",
                ExpiresIn = 15 * 60, // 15 minutes in seconds
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                User = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    PreferredLanguage = user.PreferredLanguage,
                    IsActive = user.IsActive,
                    EmailVerified = user.EmailVerified,
                    PhoneVerified = user.PhoneVerified,
                    Roles = userRoles.ToList(),
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                }
            };

            _logger.LogInformation("Token refresh successful for user: {UserId}", user.UserId);
            return Ok(authResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An error occurred while processing your request",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Handle SSO callback from OIDC provider
    /// </summary>
    /// <param name="callbackDto">SSO callback data</param>
    /// <returns>Authentication result with JWT tokens</returns>
    [HttpGet("sso/callback")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResultDto>> SsoCallbackAsync([FromQuery] SsoCallbackDto callbackDto)
    {
        try
        {
            _logger.LogInformation("SSO callback received");

            // Set client info for security logging
            callbackDto.ClientIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            callbackDto.UserAgent = HttpContext.Request.Headers.UserAgent.ToString();

            // Check for error in callback
            if (!string.IsNullOrEmpty(callbackDto.Error))
            {
                _logger.LogWarning("SSO callback error: {Error} - {ErrorDescription}", callbackDto.Error, callbackDto.ErrorDescription);
                return BadRequest(new ProblemDetails
                {
                    Title = "SSO Error",
                    Detail = callbackDto.ErrorDescription ?? callbackDto.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Validate authorization code
            if (string.IsNullOrEmpty(callbackDto.Code))
            {
                _logger.LogWarning("SSO callback missing authorization code");
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Authorization code is required",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Exchange authorization code for tokens
            var redirectUri = $"{Request.Scheme}://{Request.Host}{Request.Path}";
            var tokenResponse = await _oidcService.ExchangeCodeAsync(callbackDto.Code, redirectUri);

            if (!string.IsNullOrEmpty(tokenResponse.Error))
            {
                _logger.LogError("Token exchange failed: {Error} - {ErrorDescription}", tokenResponse.Error, tokenResponse.ErrorDescription);
                return Unauthorized(new ProblemDetails
                {
                    Title = "Token Exchange Failed",
                    Detail = tokenResponse.ErrorDescription ?? "Failed to exchange authorization code",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            // Get user info from OIDC provider
            var userInfo = await _oidcService.GetUserInfoAsync(tokenResponse.AccessToken);

            // Find or create local user
            var user = await _userService.GetUserByEmailAsync(userInfo.Email ?? "");
            if (user == null)
            {
                // Create new user from OIDC user info
                var createUserDto = new CreateUserDto
                {
                    Email = userInfo.Email ?? "",
                    Username = userInfo.PreferredUsername ?? userInfo.Email ?? "",
                    FirstName = userInfo.GivenName ?? "",
                    LastName = userInfo.FamilyName ?? "",
                    PhoneNumber = userInfo.PhoneNumber,
                    PreferredLanguage = userInfo.Locale ?? "en",
                    Password = Guid.NewGuid().ToString() // Random password for SSO users
                };

                user = await _userService.CreateUserAsync(createUserDto);
                _logger.LogInformation("New user created from SSO: {UserId}", user.UserId);
            }

            // Generate local JWT token - create User entity for token service
            var userEntity = new User
            {
                UserId = user.UserId,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                EmailVerified = user.EmailVerified,
                PhoneVerified = user.PhoneVerified,
                CreatedAt = user.CreatedAt
            };

            var accessToken = await _tokenService.GenerateJwtAsync(userEntity);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

            // Store refresh token
            var refreshTokenHash = HashToken(refreshToken);
            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = refreshTokenHash,
                UserId = user.UserId,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            // Update last login
            await _userService.UpdateLastLoginAsync(user.UserId);

            // Get user roles
            var userRoles = await _userService.GetUserRolesAsync(user.UserId);

            // Create response
            var authResult = new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = 15 * 60, // 15 minutes in seconds
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                User = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    PreferredLanguage = user.PreferredLanguage,
                    IsActive = user.IsActive,
                    EmailVerified = user.EmailVerified,
                    PhoneVerified = user.PhoneVerified,
                    Roles = userRoles.ToList(),
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                }
            };

            _logger.LogInformation("SSO authentication successful for user: {UserId}", user.UserId);
            return Ok(authResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during SSO callback processing");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An error occurred while processing your request",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Logout user and revoke refresh tokens
    /// </summary>
    /// <returns>Success response</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> LogoutAsync()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
                _logger.LogInformation("User logged out successfully: {UserId}", userId);
            }

            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An error occurred while processing your request",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Hash token using SHA256
    /// </summary>
    /// <param name="token">Token to hash</param>
    /// <returns>Hashed token</returns>
    private string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
    }
}
