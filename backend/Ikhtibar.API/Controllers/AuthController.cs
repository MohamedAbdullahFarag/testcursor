using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Linq;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.DTOs.Authentication;

namespace Ikhtibar.API.Controllers
{
    /// <summary>
    /// Authentication controller for login, token refresh, and SSO operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthenticationService authenticationService,
            ILogger<AuthController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticate user with email and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Authentication result with JWT tokens</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(Ikhtibar.Shared.DTOs.AuthResultDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Ikhtibar.Shared.DTOs.AuthResultDto>> Login([FromBody] Ikhtibar.Shared.DTOs.LoginDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authenticationService.AuthenticateAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during authentication" });
            }
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token string</param>
        /// <returns>New authentication result with refreshed tokens</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(Ikhtibar.Shared.DTOs.AuthResultDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Ikhtibar.Shared.DTOs.AuthResultDto>> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return BadRequest(new { message = "Refresh token is required" });
                }

                var result = await _authenticationService.RefreshTokenAsync(refreshToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new { message = "An error occurred during token refresh" });
            }
        }

        /// <summary>
        /// Process SSO callback from OIDC provider
        /// </summary>
        /// <param name="request">SSO callback data</param>
        /// <returns>Authentication result with JWT tokens</returns>
        [HttpPost("sso/callback")]
        [ProducesResponseType(typeof(Ikhtibar.Shared.DTOs.AuthResultDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Ikhtibar.Shared.DTOs.AuthResultDto>> SsoCallback([FromBody] Ikhtibar.Shared.DTOs.SsoCallbackDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authenticationService.ProcessSsoCallbackAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing SSO callback");
                return StatusCode(500, new { message = "An error occurred during SSO authentication" });
            }
        }

        /// <summary>
        /// Revoke refresh token (logout)
        /// </summary>
        /// <param name="refreshToken">Refresh token to revoke</param>
        /// <returns>Success status</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Logout([FromBody] string refreshToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return BadRequest(new { message = "Refresh token is required" });
                }

                var result = await _authenticationService.RevokeTokenAsync(refreshToken);
                if (result)
                {
                    return Ok(new { message = "Successfully logged out" });
                }

                return BadRequest(new { message = "Failed to logout" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        /// <summary>
        /// Validate access token
        /// </summary>
        /// <param name="token">Access token to validate</param>
        /// <returns>Token validation result</returns>
        [HttpPost("validate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> ValidateToken([FromBody] string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return BadRequest(new { message = "Token is required" });
                }

                var isValid = await _authenticationService.ValidateTokenAsync(token);
                return Ok(new { isValid });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return StatusCode(500, new { message = "An error occurred during token validation" });
            }
        }
    }
}
