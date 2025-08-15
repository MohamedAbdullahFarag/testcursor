using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Ikhtibar.API.Controllers;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Core.DTOs;
using Ikhtibar.API.Models;

namespace Ikhtibar.Tests.Auth;

/// <summary>
/// Tests for AuthController
/// </summary>
public class AuthControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IOidcService> _mockOidcService;
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockTokenService = new Mock<ITokenService>();
        _mockOidcService = new Mock<IOidcService>();
        _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();
        _mockLogger = new Mock<ILogger<AuthController>>();

        _controller = new AuthController(
            _mockUserService.Object,
            _mockTokenService.Object,
            _mockOidcService.Object,
            _mockRefreshTokenRepo.Object,
            _mockLogger.Object
        );

        // Setup HTTP context
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 });
        httpContext.Request.Headers["User-Agent"] = "Test Agent";

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
        var userDto = new UserDto { 
            UserId = 1, 
            Email = "test@example.com", 
            Username = "testuser",
            FirstName = "Test",
            LastName = "User",
            IsActive = true
        };
        var user = new UserDto { 
            UserId = 1, 
            Email = "test@example.com", 
            Username = "testuser",
            FirstName = "Test",
            LastName = "User",
            IsActive = true,
            Roles = new List<string> { "User" }
        };

        _mockUserService.Setup(x => x.AuthenticateAsync(loginDto.Email, loginDto.Password))
            .ReturnsAsync(userDto);
        _mockTokenService.Setup(x => x.GenerateJwtAsync(It.IsAny<UserDto>()))
            .ReturnsAsync("test-jwt-token");
        _mockTokenService.Setup(x => x.GenerateRefreshTokenAsync())
            .ReturnsAsync("test-refresh-token");
        _mockRefreshTokenRepo.Setup(x => x.CreateAsync(It.IsAny<RefreshToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _controller.LoginAsync(loginDto);

        // Assert
        var okResult = Assert.IsType<ActionResult<AuthResultDto>>(result);
        var authResult = Assert.IsType<AuthResultDto>(((OkObjectResult)okResult.Result).Value);
        Assert.True(authResult.Success);
        Assert.Equal("test-jwt-token", authResult.AccessToken);
        Assert.Equal("test-refresh-token", authResult.RefreshToken);
        Assert.NotNull(authResult.User);
        Assert.Equal(loginDto.Email, authResult.User.Email);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "WrongPassword" };
        _mockUserService.Setup(x => x.AuthenticateAsync(loginDto.Email, loginDto.Password))
            .ReturnsAsync((UserDto)null);

        // Act
        var result = await _controller.LoginAsync(loginDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<AuthResultDto>>(result);
        Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidToken_ReturnsNewTokens()
    {
        // Arrange
        var refreshTokenDto = new RefreshTokenDto { RefreshToken = "valid-refresh-token" };
        var storedRefreshToken = new RefreshToken { 
            TokenId = 1, 
            TokenHash = HashToken("valid-refresh-token"), 
            UserId = 1,
            IsRevoked = false,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        var user = new UserDto { 
            UserId = 1, 
            Email = "test@example.com",
            Username = "testuser",
            Roles = new List<string> { "User" }
        };
        var userDto = new UserDto { 
            UserId = 1, 
            Email = "test@example.com",
            Username = "testuser"
        };

        _mockRefreshTokenRepo.Setup(x => x.GetByTokenHashAsync(It.IsAny<string>()))
            .ReturnsAsync(storedRefreshToken);
        _mockRefreshTokenRepo.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(user);
        _mockUserService.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(userDto);
        _mockTokenService.Setup(x => x.GenerateJwtAsync(It.IsAny<UserDto>()))
            .ReturnsAsync("new-jwt-token");
        _mockTokenService.Setup(x => x.GenerateRefreshTokenAsync())
            .ReturnsAsync("new-refresh-token");

        // Act
        var result = await _controller.RefreshTokenAsync(refreshTokenDto);

        // Assert
        var okResult = Assert.IsType<ActionResult<AuthResultDto>>(result);
        var authResult = Assert.IsType<AuthResultDto>(((OkObjectResult)okResult.Result).Value);
        Assert.True(authResult.Success);
        Assert.Equal("new-jwt-token", authResult.AccessToken);
        Assert.Equal("new-refresh-token", authResult.RefreshToken);
    }

    [Fact]
    public async Task SsoCallbackAsync_WithValidCode_ReturnsTokens()
    {
        // Arrange
        var callbackDto = new SsoCallbackDto { 
            Code = "valid-auth-code",
            State = "state-value",
            RedirectUri = "https://localhost:5000/callback"
        };
        var tokenResponse = new OidcTokenResponse {
            AccessToken = "oidc-access-token",
            IdToken = "oidc-id-token",
            RefreshToken = "oidc-refresh-token",
            ExpiresIn = 3600
        };
        var userInfo = new OidcUserInfo {
            Sub = "oidc-subject-id",
            Email = "test@example.com",
            EmailVerified = true,
            GivenName = "Test",
            FamilyName = "User"
        };
        var user = new UserDto {
            UserId = 1,
            Email = "test@example.com",
            Username = "testuser",
            FirstName = "Test",
            LastName = "User",
            IsActive = true,
            Roles = new List<string> { "User" }
        };
        var userDto = new UserDto {
            UserId = 1,
            Email = "test@example.com",
            Username = "testuser",
            FirstName = "Test",
            LastName = "User",
            IsActive = true
        };

        _mockOidcService.Setup(x => x.ExchangeCodeAsync(callbackDto.Code, callbackDto.RedirectUri, callbackDto.CodeVerifier))
            .ReturnsAsync(tokenResponse);
        _mockOidcService.Setup(x => x.GetUserInfoAsync(tokenResponse.AccessToken))
            .ReturnsAsync(userInfo);
        _mockUserService.Setup(x => x.FindOrCreateExternalUserAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(userDto);
        _mockRefreshTokenRepo.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(user);
        _mockTokenService.Setup(x => x.GenerateJwtAsync(It.IsAny<UserDto>()))
            .ReturnsAsync("jwt-token");
        _mockTokenService.Setup(x => x.GenerateRefreshTokenAsync())
            .ReturnsAsync("refresh-token");

        // Act
        var result = await _controller.SsoCallbackAsync(callbackDto);

        // Assert
        var okResult = Assert.IsType<ActionResult<AuthResultDto>>(result);
        var authResult = Assert.IsType<AuthResultDto>(((OkObjectResult)okResult.Result).Value);
        Assert.True(authResult.Success);
        Assert.Equal("jwt-token", authResult.AccessToken);
        Assert.Equal("refresh-token", authResult.RefreshToken);
    }

    [Fact]
    public async Task SsoCallbackAsync_WithError_ReturnsBadRequest()
    {
        // Arrange
        var callbackDto = new SsoCallbackDto {
            Error = "access_denied",
            ErrorDescription = "User denied access"
        };

        // Act
        var result = await _controller.SsoCallbackAsync(callbackDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<AuthResultDto>>(result);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
    
    // Helpers
    private static string HashToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
