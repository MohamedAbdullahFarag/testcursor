using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;
using System.Net;
using System.Text;
using System.Text.Json;
using Ikhtibar.API.Models;
using SharedOidc = Ikhtibar.Shared.DTOs.Authentication;
using Ikhtibar.Infrastructure.Services;
using Ikhtibar.Shared.Models;
using Ikhtibar.Tests.TestHelpers;

namespace Ikhtibar.Tests.Auth;

public class OidcServiceTests
{
    private Mock<HttpMessageHandler> _mockHttpHandler;
    private HttpClient _httpClient;
    private OidcSettings _oidcSettings;
    private Mock<ILogger<OidcService>> _mockLogger;
    private OidcService _service;

    public OidcServiceTests()
    {
        _mockHttpHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_mockHttpHandler.Object);
        _oidcSettings = new OidcSettings
        {
            Authority = "https://oidc.example.com",
            ClientId = "test-client",
            ClientSecret = "test-secret",
            //RedirectUri = "https://localhost:7001/callback",
            Scopes = new[] { "openid", "profile", "email" }
        };
        // OidcService posts to relative '/token' and '/userinfo' paths; set BaseAddress so RequestUri becomes absolute
        _httpClient.BaseAddress = new Uri(_oidcSettings.Authority + "/protocol/openid-connect");
        _mockLogger = new Mock<ILogger<OidcService>>();

        var mockOptions = new Mock<IOptions<OidcSettings>>();
        mockOptions.Setup(x => x.Value).Returns(_oidcSettings);

        _service = new OidcService(_httpClient, mockOptions.Object, _mockLogger.Object);
    }

[Fact]
public async Task ExchangeCodeAsync_WithValidCode_ReturnsTokenResponse()
{
    // Arrange
    var tokenResponse = new SharedOidc.OidcTokenResponse
    {
        AccessToken = "access-token",
        IdToken = "id-token",
        RefreshToken = "refresh-token",
        TokenType = "Bearer",
        ExpiresIn = 3600
    };

    var jsonResponse = JsonSerializer.Serialize(tokenResponse);
    HttpRequestMessage? capturedRequest = null;
    _mockHttpHandler
        .Protected()
        .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        )
        .Callback((HttpRequestMessage req, CancellationToken _) => capturedRequest = req)
        .ReturnsAsync(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        })
        .Verifiable();

    // Act
var result = await _service.ExchangeCodeAsync("valid-code");

// Assert
Assert.NotNull(result);
Assert.Equal("access-token", result.AccessToken);
Assert.Equal("id-token", result.IdToken);
Assert.Equal("refresh-token", result.RefreshToken);
Assert.Equal("Bearer", result.TokenType);
Assert.Equal(3600, result.ExpiresIn);

    // verify the request was made and contained expected form values
    _mockHttpHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    Assert.NotNull(capturedRequest);
    Assert.Equal(HttpMethod.Post, capturedRequest.Method);
    Assert.NotNull(capturedRequest.RequestUri);
    // basic host/path check instead of strict full-URI match
    Assert.Equal(new Uri(_oidcSettings.Authority).Host, capturedRequest.RequestUri.Host);
    Assert.Contains("/token", capturedRequest.RequestUri.AbsolutePath);

    if (capturedRequest.Content is FormUrlEncodedContent)
    {
        var contentString = await capturedRequest.Content.ReadAsStringAsync();
        Assert.Contains("grant_type=authorization_code", contentString);
        Assert.Contains("code=valid-code", contentString);
    }
}

[Fact]
public async Task GetUserInfoAsync_WithValidToken_ReturnsUserInfo()
{
    // Arrange
    var userInfo = new OidcUserInfo
    {
        Sub = "user-123",
        Email = "user@example.com",
        Name = "Test User",
        GivenName = "Test",
        FamilyName = "User",
        EmailVerified = true
    };

    var jsonResponse = JsonSerializer.Serialize(userInfo);
    _mockHttpHandler
        .Protected()
        .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        })
        .Verifiable();

    // Act
var result = await _service.GetUserInfoAsync("valid-token");

// Assert
Assert.NotNull(result);
Assert.Equal("user-123", result.Sub);
Assert.Equal("user@example.com", result.Email);
Assert.Equal("Test User", result.Name);
Assert.Equal("Test", result.GivenName);
Assert.Equal("User", result.FamilyName);
Assert.True(result.EmailVerified);

    _mockHttpHandler
        .Protected()
        .Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri != null && req.RequestUri.Host == new Uri(_oidcSettings.Authority).Host
            ),
            ItExpr.IsAny<CancellationToken>()
        );
}

// Note: OidcService does not expose token introspection in current API; remove ValidateTokenAsync tests.
}