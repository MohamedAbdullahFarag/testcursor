// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class NotificationTemplateController : ControllerBase
{
    private readonly INotificationTemplateService _templateService;
    private readonly ILogger<NotificationTemplateController> _logger;

    public NotificationTemplateController(
        INotificationTemplateService templateService,
        ILogger<NotificationTemplateController> logger)
    {
        _templateService = templateService;
        _logger = logger;
    }

    /// <summary>
    /// Get all notification templates
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificationTemplate>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationTemplate>>> GetAllTemplates()
    {
        try
        {
            var templates = await _templateService.GetAllAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all notification templates");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get templates by type
    /// </summary>
    [HttpGet("by-type/{type}")]
    [ProducesResponseType(typeof(IEnumerable<NotificationTemplate>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationTemplate>>> GetTemplatesByType(NotificationType type)
    {
        try
        {
            var templates = await _templateService.GetByTypeAsync(type);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting templates by type {Type}", type);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get template by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NotificationTemplate), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationTemplate>> GetTemplate(int id)
    {
        try
        {
            var template = await _templateService.GetByIdAsync(id);
            if (template == null)
            {
                return NotFound();
            }
            
            return Ok(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create new template
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NotificationTemplate), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NotificationTemplate>> CreateTemplate([FromBody] NotificationTemplate template)
    {
        try
        {
            var createdTemplate = await _templateService.CreateAsync(template);
            return CreatedAtAction(nameof(GetTemplate), new { id = createdTemplate.Id }, createdTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification template");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update template
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(NotificationTemplate), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationTemplate>> UpdateTemplate(int id, [FromBody] NotificationTemplate template)
    {
        try
        {
            if (id != template.Id)
            {
                return BadRequest("ID mismatch");
            }
            
            var updatedTemplate = await _templateService.UpdateAsync(template);
            if (updatedTemplate == null)
            {
                return NotFound();
            }
            
            return Ok(updatedTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete template
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> DeleteTemplate(int id)
    {
        try
        {
            var result = await _templateService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Set template active status
    /// </summary>
    [HttpPut("{id}/active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> SetTemplateActive(int id, [FromBody] bool isActive)
    {
        try
        {
            var result = await _templateService.SetActiveStatusAsync(id, isActive);
            if (!result)
            {
                return NotFound();
            }
            
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting template {Id} active status to {IsActive}", id, isActive);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Clone template
    /// </summary>
    [HttpPost("{id}/clone")]
    [ProducesResponseType(typeof(NotificationTemplate), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationTemplate>> CloneTemplate(int id, [FromBody] string newName)
    {
        try
        {
            var clonedTemplate = await _templateService.CloneTemplateAsync(id, newName);
            if (clonedTemplate == null)
            {
                return NotFound();
            }
            
            return CreatedAtAction(nameof(GetTemplate), new { id = clonedTemplate.Id }, clonedTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cloning template {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
*/
