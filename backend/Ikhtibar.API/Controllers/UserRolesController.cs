using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for user-role relationship operations
/// Following SRP: ONLY user-role HTTP operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserRolesController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;
    private readonly ILogger<UserRolesController> _logger;

    public UserRolesController(
        IUserRoleService userRoleService,
        ILogger<UserRolesController> logger)
    {
        _userRoleService = userRoleService;
        _logger = logger;
    }

    /// <summary>
    /// Get all roles for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of roles</returns>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetUserRoles(int userId)
    {
        try
        {
            var roles = await _userRoleService.GetUserRolesAsync(userId);
            return Ok(roles);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving user roles");
        }
    }

    /// <summary>
    /// Get all users for a specific role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>Collection of users</returns>
    [HttpGet("role/{roleId}")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetRoleUsers(int roleId)
    {
        try
        {
            var users = await _userRoleService.GetRoleUsersAsync(roleId);
            return Ok(users);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users for role {RoleId}", roleId);
            return StatusCode(500, "An error occurred while retrieving role users");
        }
    }

    /// <summary>
    /// Check if a user has a specific role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <returns>True if user has the role</returns>
    [HttpGet("user/{userId}/role/{roleId}")]
    public async Task<ActionResult<bool>> UserHasRole(int userId, int roleId)
    {
        try
        {
            var hasRole = await _userRoleService.UserHasRoleAsync(userId, roleId);
            return Ok(hasRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} has role {RoleId}", userId, roleId);
            return StatusCode(500, "An error occurred while checking user role");
        }
    }

    /// <summary>
    /// Check if a user has a specific role by code
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleCode">Role code</param>
    /// <returns>True if user has the role</returns>
    [HttpGet("user/{userId}/role-code/{roleCode}")]
    public async Task<ActionResult<bool>> UserHasRoleByCode(int userId, string roleCode)
    {
        try
        {
            var hasRole = await _userRoleService.UserHasRoleAsync(userId, roleCode);
            return Ok(hasRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} has role {RoleCode}", userId, roleCode);
            return StatusCode(500, "An error occurred while checking user role");
        }
    }

    /// <summary>
    /// Assign a role to a user
    /// </summary>
    /// <param name="assignRoleDto">Role assignment data</param>
    /// <returns>No content</returns>
    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userRoleService.AssignRoleAsync(assignRoleDto.UserId, assignRoleDto.RoleId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", assignRoleDto.RoleId, assignRoleDto.UserId);
            return StatusCode(500, "An error occurred while assigning the role");
        }
    }

    /// <summary>
    /// Remove a role from a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <returns>No content</returns>
    [HttpDelete("user/{userId}/role/{roleId}")]
    public async Task<IActionResult> RemoveRole(int userId, int roleId)
    {
        try
        {
            await _userRoleService.RemoveRoleAsync(userId, roleId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            return StatusCode(500, "An error occurred while removing the role");
        }
    }

    /// <summary>
    /// Remove all roles from a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>No content</returns>
    [HttpDelete("user/{userId}/all-roles")]
    public async Task<IActionResult> RemoveAllUserRoles(int userId)
    {
        try
        {
            await _userRoleService.RemoveAllUserRolesAsync(userId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing all roles from user {UserId}", userId);
            return StatusCode(500, "An error occurred while removing all roles");
        }
    }

    /// <summary>
    /// Bulk assign roles to a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleIds">Collection of role IDs</param>
    /// <returns>No content</returns>
    [HttpPost("user/{userId}/bulk-assign")]
    public async Task<IActionResult> BulkAssignRoles(int userId, [FromBody] IEnumerable<int> roleIds)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userRoleService.AssignRolesAsync(userId, roleIds);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk assigning roles to user {UserId}", userId);
            return StatusCode(500, "An error occurred while bulk assigning roles");
        }
    }

    /// <summary>
    /// Replace all user roles with new ones
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleIds">Collection of new role IDs</param>
    /// <returns>No content</returns>
    [HttpPut("user/{userId}/replace-roles")]
    public async Task<IActionResult> ReplaceUserRoles(int userId, [FromBody] IEnumerable<int> roleIds)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userRoleService.ReplaceUserRolesAsync(userId, roleIds);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replacing roles for user {UserId}", userId);
            return StatusCode(500, "An error occurred while replacing user roles");
        }
    }
}
