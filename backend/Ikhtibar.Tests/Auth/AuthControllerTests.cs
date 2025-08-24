using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Ikhtibar.API.Controllers;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.DTOs.Authentication;

namespace Ikhtibar.Tests.Auth;

/// <summary>
/// Tests for AuthController
/// </summary>
public class AuthControllerTests
{
    private readonly Mock<IAuthenticationService> _mockAuthenticationService;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthenticationService = new Mock<IAuthenticationService>();
        _mockLogger = new Mock<ILogger<AuthController>>();

        _controller = new AuthController(
            _mockAuthenticationService.Object,
            _mockLogger.Object);

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
    public async Task Login_WithValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
        var authResult = new AuthResultDto { 
            Success = true,
            AccessToken = "test-jwt-token",
            RefreshTokens = "test-refresh-token",
            User = new UserDto { 
                UserId = 1, 
                Email = "test@example.com", 
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                Roles = new List<RoleDto> { new RoleDto { Name = "User" } }
            }
        };

        _mockAuthenticationService.Setup(x => x.AuthenticateAsync(loginDto))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAuthResult = Assert.IsType<AuthResultDto>(okResult.Value);
        Assert.True(returnedAuthResult.Success);
        Assert.Equal("test-jwt-token", returnedAuthResult.AccessToken);
        Assert.Equal("test-refresh-token", returnedAuthResult.RefreshTokens);
        Assert.NotNull(returnedAuthResult.User);
        Assert.Equal(loginDto.Email, returnedAuthResult.User.Email);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "WrongPassword" };
        _mockAuthenticationService.Setup(x => x.AuthenticateAsync(loginDto))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsNewTokens()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";
        var authResult = new AuthResultDto { 
            Success = true,
            AccessToken = "new-jwt-token",
            RefreshTokens = "new-refresh-token",
            User = new UserDto { 
                UserId = 1, 
                Email = "test@example.com",
                Username = "testuser",
                Roles = new List<RoleDto> { new RoleDto { Name = "User" } }
            }
        };

        _mockAuthenticationService.Setup(x => x.RefreshTokenAsync(refreshToken))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.RefreshToken(refreshToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAuthResult = Assert.IsType<AuthResultDto>(okResult.Value);
        Assert.True(returnedAuthResult.Success);
        Assert.Equal("new-jwt-token", returnedAuthResult.AccessToken);
        Assert.Equal("new-refresh-token", returnedAuthResult.RefreshTokens);
    }

    [Fact]
    public async Task SsoCallback_WithValidCode_ReturnsTokens()
    {
        // Arrange
        var callbackDto = new SsoCallbackDto { 
            Code = "valid-auth-code",
            State = "state-value",
            RedirectUri = "https://localhost:7001/callback"
        };
        var authResult = new AuthResultDto {
            Success = true,
            AccessToken = "jwt-token",
            RefreshTokens = "refresh-token",
            User = new UserDto {
                UserId = 1,
                Email = "test@example.com",
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                Roles = new List<RoleDto> { new RoleDto { Name = "User" } }
            }
        };

        _mockAuthenticationService.Setup(x => x.ProcessSsoCallbackAsync(callbackDto))
            .ReturnsAsync(authResult);

        // Act
        var result = await _controller.SsoCallback(callbackDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAuthResult = Assert.IsType<AuthResultDto>(okResult.Value);
        Assert.True(returnedAuthResult.Success);
        Assert.Equal("jwt-token", returnedAuthResult.AccessToken);
        Assert.Equal("refresh-token", returnedAuthResult.RefreshTokens);
    }

    [Fact]
    public async Task SsoCallback_WithError_ReturnsBadRequest()
    {
        // Arrange
        var callbackDto = new SsoCallbackDto {
            Error = "access_denied",
            ErrorDescription = "User denied access"
        };

        _mockAuthenticationService.Setup(x => x.ProcessSsoCallbackAsync(callbackDto))
            .ThrowsAsync(new InvalidOperationException("SSO callback error"));

        // Act
        var result = await _controller.SsoCallback(callbackDto);

        // Assert
        Assert.IsType<ObjectResult>(result.Result);
    }
}
