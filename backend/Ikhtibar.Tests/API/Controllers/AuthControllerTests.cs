using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Ikhtibar.API.Controllers;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.DTOs.Authentication;

namespace Ikhtibar.Tests.API.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthenticationService> _mockAuthenticationService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _authController = new AuthController(_mockAuthenticationService.Object, _mockLogger.Object);
        }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginRequest = new LoginDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var authResult = new AuthResultDto
            {
                Success = true,
                User = new UserDto
                {
                    UserId = 1,
                    Email = "test@example.com",
                    FirstName = "Test",
                    LastName = "User",
                    Username = "testuser",
                    IsActive = true,
                    EmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                },
                AccessToken = "access_token_123",
                RefreshTokens = "refresh_token_123",
                TokenType = "Bearer",
                ExpiresIn = 3600,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IssuedAt = DateTime.UtcNow
            };

            _mockAuthenticationService.Setup(x => x.AuthenticateAsync(loginRequest))
                .ReturnsAsync(authResult);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAuthResult = Assert.IsType<AuthResultDto>(okResult.Value);
            Assert.True(returnedAuthResult.Success);
            Assert.Equal(authResult.AccessToken, returnedAuthResult.AccessToken);
        }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorizedResult()
        {
            // Arrange
            var loginRequest = new LoginDto
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            _mockAuthenticationService.Setup(x => x.AuthenticateAsync(loginRequest))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

    [Fact]
    public async Task Login_WithServiceException_ReturnsInternalServerError()
        {
            // Arrange
            var loginRequest = new LoginDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            _mockAuthenticationService.Setup(x => x.AuthenticateAsync(loginRequest))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsOkResult()
        {
            // Arrange
            var refreshToken = "valid_refresh_token";
            var authResult = new AuthResultDto
            {
                Success = true,
                User = new UserDto
                {
                    UserId = 1,
                    Email = "test@example.com",
                    FirstName = "Test",
                    LastName = "User",
                    Username = "testuser",
                    IsActive = true,
                    EmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                },
                AccessToken = "new_access_token",
                RefreshTokens = "new_refresh_token",
                TokenType = "Bearer",
                ExpiresIn = 3600,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IssuedAt = DateTime.UtcNow
            };

            _mockAuthenticationService.Setup(x => x.RefreshTokenAsync(refreshToken))
                .ReturnsAsync(authResult);

            // Act
            var result = await _authController.RefreshToken(refreshToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAuthResult = Assert.IsType<AuthResultDto>(okResult.Value);
            Assert.True(returnedAuthResult.Success);
            Assert.Equal(authResult.AccessToken, returnedAuthResult.AccessToken);
        }

    [Fact]
    public async Task RefreshToken_WithEmptyToken_ReturnsBadRequest()
        {
            // Arrange
            var refreshToken = "";

            // Act
            var result = await _authController.RefreshToken(refreshToken);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorValue = badRequestResult.Value;
            var messageProperty = errorValue.GetType().GetProperty("message");
            Assert.Equal("Refresh token is required", messageProperty?.GetValue(errorValue));
        }

    [Fact]
    public async Task RefreshToken_WithServiceException_ReturnsInternalServerError()
        {
            // Arrange
            var refreshToken = "valid_refresh_token";
            _mockAuthenticationService.Setup(x => x.RefreshTokenAsync(refreshToken))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _authController.RefreshToken(refreshToken);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

    [Fact]
    public async Task SsoCallback_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var ssoCallbackRequest = new SsoCallbackDto
            {
                Code = "authorization_code",
                State = "state_value",
                RedirectUri = "https://example.com/callback"
            };

            var authResult = new AuthResultDto
            {
                Success = true,
                User = new UserDto
                {
                    UserId = 1,
                    Email = "sso@example.com",
                    FirstName = "SSO",
                    LastName = "User",
                    Username = "ssouser",
                    IsActive = true,
                    EmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                },
                AccessToken = "sso_access_token",
                RefreshTokens = "sso_refresh_token",
                TokenType = "Bearer",
                ExpiresIn = 3600,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IssuedAt = DateTime.UtcNow
            };

            _mockAuthenticationService.Setup(x => x.ProcessSsoCallbackAsync(ssoCallbackRequest))
                .ReturnsAsync(authResult);

            // Act
            var result = await _authController.SsoCallback(ssoCallbackRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAuthResult = Assert.IsType<AuthResultDto>(okResult.Value);
            Assert.True(returnedAuthResult.Success);
            Assert.Equal(authResult.AccessToken, returnedAuthResult.AccessToken);
        }

    [Fact]
    public async Task SsoCallback_WithServiceException_ReturnsInternalServerError()
        {
            // Arrange
            var ssoCallbackRequest = new SsoCallbackDto
            {
                Code = "authorization_code",
                State = "state_value",
                RedirectUri = "https://example.com/callback"
            };

            _mockAuthenticationService.Setup(x => x.ProcessSsoCallbackAsync(ssoCallbackRequest))
                .ThrowsAsync(new Exception("SSO service error"));

            // Act
            var result = await _authController.SsoCallback(ssoCallbackRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

    [Fact]
    public async Task Logout_WithValidToken_ReturnsOkResult()
        {
            // Arrange
            var refreshToken = "valid_refresh_token";
            _mockAuthenticationService.Setup(x => x.RevokeTokenAsync(refreshToken))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.Logout(refreshToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var messageValue = okResult.Value;
            var messageProperty = messageValue.GetType().GetProperty("message");
            Assert.Equal("Successfully logged out", messageProperty?.GetValue(messageValue));
        }

    [Fact]
    public async Task Logout_WithEmptyToken_ReturnsBadRequest()
        {
            // Arrange
            var refreshToken = "";

            // Act
            var result = await _authController.Logout(refreshToken);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorValue = badRequestResult.Value;
            var messageProperty = errorValue.GetType().GetProperty("message");
            Assert.Equal("Refresh token is required", messageProperty?.GetValue(errorValue));
        }

    [Fact]
    public async Task Logout_WithFailedRevocation_ReturnsBadRequest()
        {
            // Arrange
            var refreshToken = "valid_refresh_token";
            _mockAuthenticationService.Setup(x => x.RevokeTokenAsync(refreshToken))
                .ReturnsAsync(false);

            // Act
            var result = await _authController.Logout(refreshToken);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorValue = badRequestResult.Value;
            var messageProperty = errorValue.GetType().GetProperty("message");
            Assert.Equal("Failed to logout", messageProperty?.GetValue(errorValue));
        }

    [Fact]
    public async Task ValidateToken_WithValidToken_ReturnsOkResult()
        {
            // Arrange
            var token = "valid_token";
            _mockAuthenticationService.Setup(x => x.ValidateTokenAsync(token))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.ValidateToken(token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var validationValue = okResult.Value;
            var isValidProperty = validationValue.GetType().GetProperty("isValid");
            Assert.True((bool)isValidProperty?.GetValue(validationValue)!);
        }

    [Fact]
    public async Task ValidateToken_WithEmptyToken_ReturnsBadRequest()
        {
            // Arrange
            var token = "";

            // Act
            var result = await _authController.ValidateToken(token);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorValue = badRequestResult.Value;
            var messageProperty = errorValue.GetType().GetProperty("message");
            Assert.Equal("Token is required", messageProperty?.GetValue(errorValue));
        }

    [Fact]
    public async Task ValidateToken_WithServiceException_ReturnsInternalServerError()
        {
            // Arrange
            var token = "valid_token";
            _mockAuthenticationService.Setup(x => x.ValidateTokenAsync(token))
                .ThrowsAsync(new Exception("Validation service error"));

            // Act
            var result = await _authController.ValidateToken(token);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
