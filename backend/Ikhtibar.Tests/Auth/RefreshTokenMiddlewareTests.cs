using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Ikhtibar.API.Middleware;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Tests.Auth;

public class RefreshTokenMiddlewareTests
{
    private readonly Mock<ILogger<RefreshTokenMiddleware>> _mockLogger = new();
    private readonly Mock<IOptions<AuthSettings>> _mockAuthSettings = new();
    private readonly Mock<IAuthenticationService> _mockAuthenticationService = new();
    private readonly AuthSettings _authSettings;
    private readonly RequestDelegate _next = (ctx) => Task.CompletedTask;
    private readonly RefreshTokenMiddleware _middleware;

    public RefreshTokenMiddlewareTests()
    {
        _authSettings = new AuthSettings
        {
            JwtSecretKey = "test-secret-key-must-be-at-least-32-characters-long",
            JwtIssuer = "test-issuer",
            JwtAudience = "test-audience",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 30,
            UseHttpOnlyCookies = false,
            RefreshTokenCookieName = "refresh_token",
            RequireHttps = false,
            EnableTokenRotation = true
        };

        _mockAuthSettings.Setup(x => x.Value).Returns(_authSettings);
        _middleware = new RefreshTokenMiddleware(_next, _mockLogger.Object, _mockAuthSettings.Object);
    }

    [Fact]
    public async Task InvokeAsync_NoTokenExpiredHeader_CallsNext()
    {
        var context = new DefaultHttpContext();
        var called = false;
        RequestDelegate next = (ctx) => { called = true; return Task.CompletedTask; };

        var mw = new RefreshTokenMiddleware(next, _mockLogger.Object, _mockAuthSettings.Object);
        await mw.InvokeAsync(context);

        Assert.True(called);
    }

    [Fact]
    public async Task InvokeAsync_TokenExpired_NoRefreshToken_ReturnsUnauthorized()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["Token-Expired"] = "true";

        await _middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.True(context.Response.Headers.ContainsKey("Token-Refresh-Required"));
    }
}
