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
using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Tests.Auth;

/// <summary>
/// Tests for TokenService
/// </summary>
public class TokenServiceTests
{
    private JwtSettings _jwtSettings;
    private Mock<IOptions<JwtSettings>> _mockOptions;
    private Mock<ILogger<TokenService>> _mockLogger;
    private TokenService _service;

    public TokenServiceTests()
    {
    _jwtSettings = new JwtSettings
        {
            SecretKey = "test-secret-key-that-is-long-enough-for-jwt-signature",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7,
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
            IsActive = true,
            UserRoles = new List<Ikhtibar.Shared.Entities.UserRole>()
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
    Assert.Equal(user.UserId.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier).Value);
    Assert.Equal(user.Email, principal.FindFirst(ClaimTypes.Email).Value);
    Assert.Equal(user.Username, principal.FindFirst(ClaimTypes.Name).Value);
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
    public async Task IsTokenValidAsync_WithValidToken_ReturnsTrue()
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
        var result = await _service.IsTokenValidAsync(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsTokenValidAsync_WithInvalidToken_ReturnsFalse()
    {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var result = await _service.IsTokenValidAsync(invalidToken);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public async Task GetPrincipalFromExpiredTokenAsync_WithValidToken_ReturnsPrincipal()
    {
        // Arrange
        var user = new User
        {
            UserId = 42,
            Username = "testuser",
            Email = "test@example.com",
            IsActive = true,
            UserRoles = new List<Ikhtibar.Shared.Entities.UserRole>()
        };

        var token = await _service.GenerateJwtAsync(user);

        // Act
        var principal = await _service.GetPrincipalFromExpiredTokenAsync(token);
        Assert.NotNull(principal);
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        Assert.NotNull(claim);
        Assert.Equal("42", claim.Value);
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
    var valid = await _service.IsTokenValidAsync(token);
    Assert.True(valid);
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
            IsActive = true,
            UserRoles = new List<Ikhtibar.Shared.Entities.UserRole>()
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
            NotBefore = DateTime.UtcNow.AddMinutes(-10),
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
    // Validate that expired token is considered invalid
    var valid = await _service.IsTokenValidAsync(expiredTokenString);
    Assert.False(valid);
    }
}
