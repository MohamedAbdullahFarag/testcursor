using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ikhtibar.Core.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Temporary test controller for development purposes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // Allow anonymous access for testing
public class TestController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<TestController> _logger;

    public TestController(
        IUserService userService,
        ITokenService tokenService,
        ILogger<TestController> logger)
    {
        _userService = userService;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users for testing (no auth required)
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult> GetUsersTestAsync()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new { 
                success = true, 
                count = users?.Items?.Count() ?? 0,
                totalCount = users?.TotalCount ?? 0,
                users = users?.Items 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in test endpoint");
            return Ok(new { 
                success = false, 
                error = ex.Message,
                stackTrace = ex.StackTrace 
            });
        }
    }

    /// <summary>
    /// Create a test user for development
    /// </summary>
    [HttpPost("create-test-user")]
    public async Task<ActionResult> CreateTestUserAsync()
    {
        try
        {
            // Hash password "password123"
            var password = "password123";
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashedPassword = Convert.ToBase64String(hashedBytes);

            var createUserDto = new CreateUserDto
            {
                Username = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Password = password, // Let the service handle hashing
                PhoneNumber = "+1234567890",
                PreferredLanguage = "en"
            };

            var user = await _userService.CreateUserAsync(createUserDto);
            return Ok(new { 
                success = true, 
                message = "Test user created successfully",
                user = user 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating test user");
            return Ok(new { 
                success = false, 
                error = ex.Message,
                stackTrace = ex.StackTrace 
            });
        }
    }

    /// <summary>
    /// Generate JWT token for test user (development only)
    /// </summary>
    [HttpGet("get-test-token")]
    public async Task<ActionResult> GetTestTokenAsync()
    {
        try
        {
            // Create a simple User entity for token generation
            var testUser = new Ikhtibar.Shared.Entities.User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                EmailVerified = true
            };

            // Generate JWT token
            var token = await _tokenService.GenerateJwtAsync(testUser);
            
            return Ok(new { 
                success = true,
                message = "JWT token generated successfully",
                token = token,
                user = new {
                    userId = testUser.UserId,
                    email = testUser.Email,
                    username = testUser.Username
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating test token");
            return Ok(new { 
                success = false, 
                error = ex.Message,
                stackTrace = ex.StackTrace 
            });
        }
    }
}
