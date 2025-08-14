using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Ikhtibar.Infrastructure.Services;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Tests.Auth;

/// <summary>
/// Tests for TokenService
/// </summary>
public class TokenServiceTests
{
    private readonly JwtSettings _jwtSettings;
    private readonly Mock<IOptions<JwtSettings>> _mockOptions;
    private readonly Mock<ILogger<TokenService>> _mockLogger;
    private readonly TokenService _service;

    public TokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "test-secret-key-that-is-long-enough-for-jwt-signature",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        _mockOptions = new Mock<IOptions<JwtSettings>>();
        _mockOptions.Setup(x => x.Value).Returns(_jwtSettings);

        _mockLogger = new Mock<ILogger<TokenService>>();

        _service = new TokenService(_mockOptions.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GenerateJwtAsync_WithValidUser_ReturnsValidToken()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            IsActive = true
        };

        // Act
        var token = await _service.GenerateJwtAsync(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // Validate token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
        
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
        
        Assert.NotNull(principal);
        Assert.NotNull(validatedToken);
        Assert.Equal(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[ClaimTypes.NameIdentifier], principal.FindFirst("sub").Type);
        Assert.Equal(user.UserId.ToString(), principal.FindFirst("sub").Value);
        Assert.Equal(user.Email, principal.FindFirst("email").Value);
        Assert.Equal(user.Username, principal.FindFirst("name").Value);
    }

    [Fact]
    public async Task GenerateRefreshTokenAsync_ReturnsNonEmptyString()
    {
        // Act
        var refreshToken = await _service.GenerateRefreshTokenAsync();

        // Assert
        Assert.NotNull(refreshToken);
        Assert.NotEmpty(refreshToken);
        Assert.True(refreshToken.Length >= 32); // Check that it's long enough to be secure
    }

    [Fact]
    public async Task ValidateTokenAsync_WithValidToken_ReturnsTrue()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            IsActive = true
        };

        var token = await _service.GenerateJwtAsync(user);

        // Act
        var result = await _service.ValidateTokenAsync(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateTokenAsync_WithInvalidToken_ReturnsFalse()
    {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var result = await _service.ValidateTokenAsync(invalidToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetUserIdFromTokenAsync_WithValidToken_ReturnsUserId()
    {
        // Arrange
        var user = new User
        {
            UserId = 42,
            Username = "testuser",
            Email = "test@example.com",
            IsActive = true
        };

        var token = await _service.GenerateJwtAsync(user);

        // Act
        var userId = await _service.GetUserIdFromTokenAsync(token);

        // Assert
        Assert.Equal(42, userId);
    }

    [Fact]
    public async Task IsTokenExpiredAsync_WithNonExpiredToken_ReturnsFalse()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            IsActive = true
        };

        var token = await _service.GenerateJwtAsync(user);

        // Act
        var result = await _service.IsTokenExpiredAsync(token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsTokenExpiredAsync_WithExpiredToken_ReturnsTrue()
    {
        // This test creates an expired token manually since we can't easily wait for a token to expire
        
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            IsActive = true
        };

        // Create custom expired token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", user.UserId.ToString()),
                new Claim("email", user.Email),
                new Claim("name", user.Username)
            }),
            Expires = DateTime.UtcNow.AddMinutes(-5), // Expired 5 minutes ago
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var expiredToken = tokenHandler.CreateToken(tokenDescriptor);
        var expiredTokenString = tokenHandler.WriteToken(expiredToken);

        // Act
        var result = await _service.IsTokenExpiredAsync(expiredTokenString);

        // Assert
        Assert.True(result);
    }
}
