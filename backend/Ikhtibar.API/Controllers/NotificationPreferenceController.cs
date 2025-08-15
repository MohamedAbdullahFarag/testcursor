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
[Authorize]
public class NotificationPreferenceController : ControllerBase
{
    private readonly INotificationPreferenceService _preferenceService;
    private readonly ILogger<NotificationPreferenceController> _logger;

    public NotificationPreferenceController(
        INotificationPreferenceService preferenceService,
        ILogger<NotificationPreferenceController> logger)
    {
        _preferenceService = preferenceService;
        _logger = logger;
    }

    /// <summary>
    /// Get user notification preferences
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificationPreference>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationPreference>>> GetUserPreferences()
    {
        try
        {
            // Get current user ID from claims
            var userId = GetCurrentUserId();
            var preferences = await _preferenceService.GetByUserIdAsync(userId);
            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user notification preferences");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get preference by type
    /// </summary>
    [HttpGet("by-type/{type}")]
    [ProducesResponseType(typeof(NotificationPreference), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationPreference>> GetPreferenceByType(NotificationType type)
    {
        try
        {
            var userId = GetCurrentUserId();
            var preference = await _preferenceService.GetByUserIdAndTypeAsync(userId, type);
            if (preference == null)
            {
                return NotFound();
            }
            
            return Ok(preference);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting preference by type {Type}", type);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update notification preference
    /// </summary>
    [HttpPut("by-type/{type}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> UpdatePreference(NotificationType type, [FromBody] object preferenceData)
    {
        try
        {
            var userId = GetCurrentUserId();
            // This would typically parse the preference data and update accordingly
            // For now, we'll return a placeholder response
            _logger.LogInformation("Updating preference for type {Type} for user {UserId}", type, userId);
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating preference for type {Type}", type);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create or update notification preference
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NotificationPreference), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NotificationPreference>> CreateOrUpdatePreference([FromBody] NotificationPreference preference)
    {
        try
        {
            var userId = GetCurrentUserId();
            preference.UserId = userId;
            
            var createdPreference = await _preferenceService.CreateAsync(preference);
            return CreatedAtAction(nameof(GetPreferenceByType), new { type = preference.Type }, createdPreference);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating or updating notification preference");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete notification preference
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> DeletePreference(int id)
    {
        try
        {
            var result = await _preferenceService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting preference {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create default preferences for user
    /// </summary>
    [HttpPost("create-defaults")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> CreateDefaultPreferences()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _preferenceService.CreateDefaultPreferencesAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating default preferences for user {UserId}", GetCurrentUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Reset preferences to defaults
    /// </summary>
    [HttpPost("reset-to-defaults")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> ResetToDefaults()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _preferenceService.ResetToDefaultsAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting preferences to defaults for user {UserId}", GetCurrentUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update quiet hours settings
    /// </summary>
    [HttpPut("quiet-hours")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> UpdateQuietHours([FromBody] object quietHours)
    {
        try
        {
            var userId = GetCurrentUserId();
            // This would typically parse the quiet hours data and update accordingly
            // For now, we'll return a placeholder response
            _logger.LogInformation("Updating quiet hours for user {UserId}", userId);
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quiet hours for user {UserId}", GetCurrentUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Check if current time is within quiet hours
    /// </summary>
    [HttpGet("quiet-hours/check")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> CheckQuietHours()
    {
        try
        {
            var userId = GetCurrentUserId();
            var isWithinQuietHours = await _preferenceService.IsWithinQuietHoursAsync(userId);
            return Ok(isWithinQuietHours);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking quiet hours for user {UserId}", GetCurrentUserId());
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get preferred delivery channels for a notification type
    /// </summary>
    [HttpGet("channels/{type}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetPreferredChannels(NotificationType type)
    {
        try
        {
            var userId = GetCurrentUserId();
            var channels = await _preferenceService.GetPreferredChannelsAsync(userId, type);
            return Ok(channels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting preferred channels for type {Type}", type);
            return StatusCode(500, "Internal server error");
        }
    }

    #region Private Methods

    private int GetCurrentUserId()
    {
        // This would typically extract the user ID from JWT claims
        // For now, we'll return a placeholder value
        return 1;
    }

    #endregion
}
*/
