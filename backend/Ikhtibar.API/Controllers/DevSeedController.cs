using Ikhtibar.Core.DTOs;
using Ikhtibar.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Development seeding controller - only for dev/test environments
/// </summary>
[ApiController]
[Route("api/dev-seed")]
public class DevSeedController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;
    private readonly IUserService _userService;
    private readonly IUserRoleService _userRoleService;
    private readonly ILogger<DevSeedController> _logger;

    public DevSeedController(
        IRoleService roleService,
        IPermissionService permissionService,
        IUserService userService,
        IUserRoleService userRoleService,
        ILogger<DevSeedController> logger)
    {
        _roleService = roleService;
        _permissionService = permissionService;
        _userService = userService;
        _userRoleService = userRoleService;
        _logger = logger;
    }

    /// <summary>
    /// Seed all development data (roles, permissions, and test user)
    /// </summary>
    [HttpPost("all")]
    public async Task<IActionResult> SeedAllAsync()
    {
        try
        {
            _logger.LogInformation("Starting full development data seeding");

            // Seed permissions first
            await _permissionService.SeedDefaultPermissionsAsync();
            
            // Then seed roles
            await _roleService.SeedDefaultRolesAsync();
            
            // Create a test user with system-admin role
            await CreateTestUserAsync();

            _logger.LogInformation("Full development data seeding completed");
            return Ok(new { message = "Development data seeded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed development data");
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to seed development data");
        }
    }

    /// <summary>
    /// Create a test user for E2E testing
    /// </summary>
    [HttpPost("test-user")]
    public async Task<IActionResult> CreateTestUserAsync()
    {
        try
        {
            _logger.LogInformation("Creating test user for E2E testing");

            // Check if test user already exists
            var existingUsers = await _userService.GetAllUsersAsync(1, 100);
            var testUser = existingUsers.FirstOrDefault(u => u.Email == "admin@ikhtibar.com");
            
            if (testUser == null)
            {
                // Create test user
                var createUserDto = new CreateUserDto
                {
                    Username = "admin",
                    Email = "admin@ikhtibar.com",
                    FirstName = "Super",
                    LastName = "Administrator",
                    Password = "admin123", // Simple password for testing
                    IsActive = true
                };

                testUser = await _userService.CreateUserAsync(createUserDto);
                _logger.LogInformation("Created test user: {Email}", testUser.Email);
            }

            // Ensure test user has system-admin role
            var systemAdminRole = await GetRoleByCodeAsync("system-admin");
            if (systemAdminRole != null)
            {
                try
                {
                    await _userRoleService.AssignRoleAsync(testUser.UserId, systemAdminRole.RoleId);
                    _logger.LogInformation("Assigned system-admin role to test user");
                }
                catch (Exception ex)
                {
                    // Role might already be assigned
                    _logger.LogDebug("Role assignment might already exist: {Message}", ex.Message);
                }
            }

            return Ok(new { 
                message = "Test user created/updated successfully",
                userId = testUser.UserId,
                email = testUser.Email 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create test user");
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create test user");
        }
    }

    private async Task<RoleDto?> GetRoleByCodeAsync(string code)
    {
        try
        {
            return await _roleService.GetRoleByCodeAsync(code);
        }
        catch
        {
            return null;
        }
    }
}
