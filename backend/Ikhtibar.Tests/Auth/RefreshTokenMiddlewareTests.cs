using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Ikhtibar.API.Middleware;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Tests.Auth;

/// <summary>
/// Tests for RefreshTokenMiddleware
/// </summary>
public class RefreshTokenMiddlewareTests
{
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;
    private readonly Mock<ILogger<RefreshTokenMiddleware>> _mockLogger;
    private readonly RefreshTokenMiddleware _middleware;
    private readonly RequestDelegate _next;

    public RefreshTokenMiddlewareTests()
    {
        _mockTokenService = new Mock<ITokenService>();
        _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();
        _mockLogger = new Mock<ILogger<RefreshTokenMiddleware>>();
        
        _next = (ctx) => Task.CompletedTask;
        _middleware = new RefreshTokenMiddleware(_next, _mockLogger.Object);
    }

    [Fact]
    public async Task InvokeAsync_WithNoAuthHeader_CallsNextMiddleware()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextCalled = false;
        
        RequestDelegate next = (ctx) => {
            nextCalled = true;
            return Task.CompletedTask;
        };
        
        var middleware = new RefreshTokenMiddleware(next, _mockLogger.Object);
        
        // Act
        await middleware.InvokeAsync(context, _mockTokenService.Object, _mockRefreshTokenRepo.Object);
        
        // Assert
        Assert.True(nextCalled);
        _mockTokenService.Verify(x => x.IsTokenExpiredAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WithNonExpiredToken_CallsNextMiddleware()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Bearer valid-token";
        var nextCalled = false;
        
        RequestDelegate next = (ctx) => {
            nextCalled = true;
            return Task.CompletedTask;
        };
        
        var middleware = new RefreshTokenMiddleware(next, _mockLogger.Object);
        
        _mockTokenService.Setup(x => x.IsTokenExpiredAsync("valid-token"))
            .ReturnsAsync(false);
        
        // Act
        await middleware.InvokeAsync(context, _mockTokenService.Object, _mockRefreshTokenRepo.Object);
        
        // Assert
        Assert.True(nextCalled);
        _mockTokenService.Verify(x => x.IsTokenExpiredAsync("valid-token"), Times.Once);
        _mockRefreshTokenRepo.Verify(x => x.GetLatestByUserIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WithExpiredTokenAndValidRefreshToken_RefreshesToken()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Bearer expired-token";
        
        _mockTokenService.Setup(x => x.IsTokenExpiredAsync("expired-token"))
            .ReturnsAsync(true);
        _mockTokenService.Setup(x => x.GetUserIdFromTokenAsync("expired-token"))
            .ReturnsAsync(1);
        
        var refreshToken = new RefreshToken
        {
            TokenId = 1,
            UserId = 1,
            TokenHash = "hash",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false
        };
        
        _mockRefreshTokenRepo.Setup(x => x.GetLatestByUserIdAsync(1))
            .ReturnsAsync(refreshToken);
        
        var user = new User
        {
            UserId = 1,
            Email = "test@example.com",
            Username = "testuser"
        };
        
        _mockRefreshTokenRepo.Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(user);
        
        _mockTokenService.Setup(x => x.GenerateJwtAsync(It.IsAny<User>()))
            .ReturnsAsync("new-token");
        _mockTokenService.Setup(x => x.GenerateRefreshTokenAsync())
            .ReturnsAsync("new-refresh-token");
        
        // Act
        await _middleware.InvokeAsync(context, _mockTokenService.Object, _mockRefreshTokenRepo.Object);
        
        // Assert
        Assert.True(context.Response.Headers.ContainsKey("X-Token-Refreshed"));
        Assert.Equal("true", context.Response.Headers["X-Token-Refreshed"]);
        Assert.True(context.Response.Headers.ContainsKey("X-New-Token"));
        Assert.Equal("new-token", context.Response.Headers["X-New-Token"]);
        
        _mockRefreshTokenRepo.Verify(x => x.RevokeAsync(1), Times.Once);
        _mockRefreshTokenRepo.Verify(x => x.CreateAsync(It.IsAny<RefreshToken>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithExpiredTokenAndExpiredRefreshToken_DoesNotRefreshToken()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Bearer expired-token";
        
        _mockTokenService.Setup(x => x.IsTokenExpiredAsync("expired-token"))
            .ReturnsAsync(true);
        _mockTokenService.Setup(x => x.GetUserIdFromTokenAsync("expired-token"))
            .ReturnsAsync(1);
        
        var refreshToken = new RefreshToken
        {
            TokenId = 1,
            UserId = 1,
            TokenHash = "hash",
            ExpiresAt = DateTime.UtcNow.AddDays(-1), // Expired refresh token
            IsRevoked = false
        };
        
        _mockRefreshTokenRepo.Setup(x => x.GetLatestByUserIdAsync(1))
            .ReturnsAsync(refreshToken);
        
        // Act
        await _middleware.InvokeAsync(context, _mockTokenService.Object, _mockRefreshTokenRepo.Object);
        
        // Assert
        Assert.False(context.Response.Headers.ContainsKey("X-Token-Refreshed"));
        
        _mockRefreshTokenRepo.Verify(x => x.RevokeAsync(It.IsAny<int>()), Times.Never);
        _mockRefreshTokenRepo.Verify(x => x.CreateAsync(It.IsAny<RefreshToken>()), Times.Never);
    }
}
