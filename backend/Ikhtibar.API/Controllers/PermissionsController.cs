using Ikhtibar.Core.DTOs;
using Ikhtibar.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Permissions API - exposes permission catalog for the frontend
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(IPermissionService permissionService, ILogger<PermissionsController> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all available permissions with pagination support
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PermissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? searchTerm = null)
    {
        try
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            
            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                permissions = permissions.Where(p =>
                    (p.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (p.Code?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (p.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            var total = permissions.Count();
            var items = permissions.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
            _logger.LogError(ex, "Failed to retrieve permissions");
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve permissions");
        }
    }

    /// <summary>
    /// Seed default permissions for development
    /// </summary>
    [HttpPost("seed-defaults")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SeedDefaultPermissions()
    {
        try
        {
            await _permissionService.SeedDefaultPermissionsAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed default permissions");
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to seed default permissions");
        }
    }
}
