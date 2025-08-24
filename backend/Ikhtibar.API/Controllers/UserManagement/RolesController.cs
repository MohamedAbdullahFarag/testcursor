using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ikhtibar.API.Controllers.Base;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.API.Controllers.UserManagement;

/// <summary>
/// API Controller for Role management operations
/// Provides CRUD operations for roles following REST conventions
/// </summary>
[Authorize]
[ApiController]
[Route("api/user-management/roles")]
[Produces("application/json")]
public class UserManagementRolesController : ApiControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ILogger<UserManagementRolesController> _logger;

    public UserManagementRolesController(
        IRoleService roleService,
        ILogger<UserManagementRolesController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all roles
    /// </summary>
    /// <returns>List of all roles</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all roles");
            var roles = await _roleService.GetAllRolesAsync();
            return SuccessResponse(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all roles");
            return ErrorResponse("Failed to retrieve roles", StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Retrieves a specific role by ID
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Role details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRoleAsync(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving role with ID: {RoleId}", id);
            var role = await _roleService.GetRoleAsync(id);

            if (role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found", id);
                return NotFound($"Role with ID {id} not found");
            }

            return SuccessResponse(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role with ID: {RoleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new {
                success = false,
                message = "Failed to retrieve role",
                data = (object?)null,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Retrieves a role by its code
    /// </summary>
    /// <param name="code">Role code</param>
    /// <returns>Role details</returns>
    [HttpGet("by-code/{code}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRoleByCodeAsync(string code)
    {
        try
        {
            _logger.LogInformation("Retrieving role with code: {RoleCode}", code);
            var role = await _roleService.GetRoleByCodeAsync(code);

            if (role == null)
            {
                _logger.LogWarning("Role with code {RoleCode} not found", code);
                return NotFound($"Role with code {code} not found");
            }

            return SuccessResponse(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role with code: {RoleCode}", code);
            return StatusCode(StatusCodes.Status500InternalServerError, new {
                success = false,
                message = "Failed to retrieve role",
                data = (object?)null,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Creates a new role
    /// </summary>
    /// <param name="createRoleDto">Role creation data</param>
    /// <returns>Created role details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoleDto>> CreateRoleAsync([FromBody] CreateRoleDto createRoleDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for role creation: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new role with code: {RoleCode}", createRoleDto.Code);
            var createdRole = await _roleService.CreateRoleAsync(createRoleDto);

            return CreatedAtAction(
                nameof(GetRoleAsync),
                new { id = createdRole.RoleId },
                createdRole);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Role creation failed - business rule violation");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role with code: {RoleCode}", createRoleDto.Code);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create role");
        }
    }

    /// <summary>
    /// Updates an existing role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="updateRoleDto">Role update data</param>
    /// <returns>Updated role details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoleDto>> UpdateRoleAsync(int id, [FromBody] UpdateRoleDto updateRoleDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for role update: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating role with ID: {RoleId}", id);
            var updatedRole = await _roleService.UpdateRoleAsync(id, updateRoleDto);

            if (updatedRole == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found for update", id);
                return NotFound($"Role with ID {id} not found");
            }

            return Ok(updatedRole);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Role update failed - business rule violation for ID: {RoleId}", id);
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role with ID: {RoleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update role");
        }
    }

    /// <summary>
    /// Deletes a role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Deletion confirmation</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRoleAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting role with ID: {RoleId}", id);
            var deleted = await _roleService.DeleteRoleAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Role with ID {RoleId} not found for deletion", id);
                return NotFound($"Role with ID {id} not found");
            }

            _logger.LogInformation("Role with ID {RoleId} successfully deleted", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Role deletion failed - business rule violation for ID: {RoleId}", id);
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role with ID: {RoleId}", id);
            return ErrorResponse("Failed to delete role", StatusCodes.Status500InternalServerError);
        }
    }
}
