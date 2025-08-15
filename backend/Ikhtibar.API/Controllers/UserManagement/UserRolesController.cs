using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ikhtibar.API.Controllers.Base;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.API.Controllers.UserManagement;

/// <summary>
/// API Controller for User-Role assignment operations
/// Provides operations to manage user-role relationships
/// </summary>
[Authorize]
[ApiController]
[Route("api/user-roles")]
[Produces("application/json")]
public class UserRolesController : ApiControllerBase
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
    /// Retrieves all roles assigned to a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of roles assigned to the user</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetUserRolesAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Retrieving roles for user ID: {UserId}", userId);
            var roles = await _userRoleService.GetUserRolesAsync(userId);
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user ID: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve user roles");
        }
    }

    /// <summary>
    /// Retrieves all users assigned to a specific role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>List of users assigned to the role</returns>
    [HttpGet("role/{roleId}")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetRoleUsersAsync(int roleId)
    {
        try
        {
            _logger.LogInformation("Retrieving users for role ID: {RoleId}", roleId);
            var users = await _userRoleService.GetRoleUsersAsync(roleId);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for role ID: {RoleId}", roleId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve role users");
        }
    }

    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    /// <param name="assignRoleDto">Role assignment data</param>
    /// <returns>Assignment confirmation</returns>
    [HttpPost("assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignRoleToUserAsync([FromBody] AssignRoleDto assignRoleDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for role assignment: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Assigning role {RoleId} to user {UserId}", assignRoleDto.RoleId, assignRoleDto.UserId);

            // Map API DTO to Core DTO
            var coreAssignRoleDto = new AssignRoleDto
            {
                UserId = assignRoleDto.UserId,
                RoleId = assignRoleDto.RoleId
            };

            await _userRoleService.AssignRoleAsync(coreAssignRoleDto.UserId, coreAssignRoleDto.RoleId);

            return Ok(new { Message = "Role assigned successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Role assignment failed - business rule violation");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", assignRoleDto.RoleId, assignRoleDto.UserId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to assign role");
        }
    }

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <returns>Removal confirmation</returns>
    [HttpDelete("user/{userId}/role/{roleId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveRoleFromUserAsync(int userId, int roleId)
    {
        try
        {
            _logger.LogInformation("Removing role {RoleId} from user {UserId}", roleId, userId);
            await _userRoleService.RemoveRoleAsync(userId, roleId);

            return Ok(new { Message = "Role removed successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Role removal failed - business rule violation");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to remove role");
        }
    }

    /// <summary>
    /// Checks if a user has a specific role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <returns>True if user has the role, false otherwise</returns>
    [HttpGet("user/{userId}/role/{roleId}/exists")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> UserHasRoleAsync(int userId, int roleId)
    {
        try
        {
            _logger.LogInformation("Checking if user {UserId} has role {RoleId}", userId, roleId);
            var hasRole = await _userRoleService.UserHasRoleAsync(userId, roleId);
            return Ok(hasRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} has role {RoleId}", userId, roleId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to check user role");
        }
    }
}
