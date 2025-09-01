using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Controller for role management operations
/// Following SRP: ONLY role HTTP operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;
    private readonly IUserRoleService _userRoleService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(
        IRoleService roleService,
    IPermissionService permissionService,
    IUserRoleService userRoleService,
        ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _permissionService = permissionService;
    _userRoleService = userRoleService;
        _logger = logger;
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    /// <returns>Collection of roles</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        try
        {
            // For now, return full list and emulate simple pagination on server side
            var roles = await _roleService.GetAllRolesAsync();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                roles = roles.Where(r =>
                    (r.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Code?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            var total = roles.Count();
            var items = roles.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var payload = new
            {
                items,
                totalCount = total,
                page,
                pageSize
            };
            return Ok(payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles");
            return StatusCode(500, "An error occurred while retrieving roles");
        }
    }

    /// <summary>
    /// Get users for a role (simple, non-paged for now)
    /// </summary>
    [HttpGet("{roleId}/users")]
    public async Task<IActionResult> GetRoleUsers(int roleId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var users = await _userRoleService.GetRoleUsersAsync(roleId);
            var total = users.Count();
            var items = users.Skip((page - 1) * pageSize).Take(pageSize);
            return Ok(new { items, totalCount = total, page, pageSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users for role {RoleId}", roleId);
            return StatusCode(500, "An error occurred while retrieving role users");
        }
    }

    /// <summary>
    /// Get role permission matrix (permissions granted to role)
    /// </summary>
    [HttpGet("{roleId}/permissions")]
    public async Task<IActionResult> GetRolePermissions(int roleId)
    {
        try
        {
            // Use permission service to build a matrix; adapt to frontend shape
            var matrix = await _permissionService.GetPermissionMatrixAsync();
            var roleInfo = matrix.Roles.FirstOrDefault(r => r.RoleId == roleId);
            if (roleInfo == null)
                return NotFound($"Role {roleId} not found in permission matrix");

            // Map to code->bool
            var grantedIds = new HashSet<int>(roleInfo.PermissionIds);
            var permsDict = matrix.Permissions.ToDictionary(p => p.Code, p => grantedIds.Contains(p.PermissionId));

            return Ok(new { roleId, roleName = roleInfo.Name, permissions = permsDict });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permission matrix for role {RoleId}", roleId);
            return StatusCode(500, "An error occurred while retrieving role permissions");
        }
    }

    /// <summary>
    /// Update role permissions with a list of permission codes
    /// </summary>
    [HttpPut("{roleId}/permissions")]
    public async Task<IActionResult> UpdateRolePermissions(int roleId, [FromBody] PermissionCodesRequest request)
    {
        try
        {
            if (request?.PermissionCodes == null)
            {
                return BadRequest("permissionCodes is required");
            }

            // Translate codes -> ids using permission service catalog
            var all = await _permissionService.GetAllPermissionsAsync();
            var map = all.ToDictionary(p => p.Code, p => p.PermissionId, StringComparer.OrdinalIgnoreCase);
            var resolvedIds = request.PermissionCodes.Where(c => map.ContainsKey(c)).Select(c => map[c]).ToList();

            // For simplicity replace set by assigning selected and removing missing
            // Here we call AssignPermissionsToRoleAsync; in a richer impl we'd compute deltas
            await _permissionService.RemovePermissionsFromRoleAsync(roleId, map.Values); // remove all
            var ok = await _permissionService.AssignPermissionsToRoleAsync(roleId, resolvedIds);
            if (!ok) return StatusCode(500, "Failed to update permissions");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permissions for role {RoleId}", roleId);
            return StatusCode(500, "An error occurred while updating role permissions");
        }
    }

    public class PermissionCodesRequest
    {
        public IEnumerable<string> PermissionCodes { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// Get active roles only
    /// </summary>
    /// <returns>Collection of active roles</returns>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetActiveRoles()
    {
        try
        {
            var roles = await _roleService.GetActiveRolesAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active roles");
            return StatusCode(500, "An error occurred while retrieving active roles");
        }
    }

    /// <summary>
    /// Get role by ID
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Role details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetRole(int id)
    {
        try
        {
            var role = await _roleService.GetRoleAsync(id);
            if (role == null)
            {
                return NotFound($"Role with ID {id} not found");
            }

            return Ok(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role {RoleId}", id);
            return StatusCode(500, "An error occurred while retrieving the role");
        }
    }

    /// <summary>
    /// Get role by code
    /// </summary>
    /// <param name="code">Role code</param>
    /// <returns>Role details</returns>
    [HttpGet("code/{code}")]
    public async Task<ActionResult<RoleDto>> GetRoleByCode(string code)
    {
        try
        {
            var role = await _roleService.GetRoleByCodeAsync(code);
            if (role == null)
            {
                return NotFound($"Role with code {code} not found");
            }

            return Ok(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role by code {Code}", code);
            return StatusCode(500, "An error occurred while retrieving the role");
        }
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="createRoleDto">Role creation data</param>
    /// <returns>Created role</returns>
    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto createRoleDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = await _roleService.CreateRoleAsync(createRoleDto);
            return CreatedAtAction(nameof(GetRole), new { id = role.RoleId }, role);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return StatusCode(500, "An error occurred while creating the role");
        }
    }

    /// <summary>
    /// Update an existing role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="updateRoleDto">Role update data</param>
    /// <returns>Updated role</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = await _roleService.UpdateRoleAsync(id, updateRoleDto);
            return Ok(role);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            return StatusCode(500, "An error occurred while updating the role");
        }
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        try
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (!result)
            {
                return NotFound($"Role with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            return StatusCode(500, "An error occurred while deleting the role");
        }
    }

    /// <summary>
    /// Seed default system roles
    /// </summary>
    /// <returns>No content</returns>
    [HttpPost("seed-defaults")]
    public async Task<IActionResult> SeedDefaultRoles()
    {
        try
        {
            await _roleService.SeedDefaultRolesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding default roles");
            return StatusCode(500, "An error occurred while seeding default roles");
        }
    }
}

