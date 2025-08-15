// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly INotificationTemplateService _templateService;
    private readonly INotificationPreferenceService _preferenceService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        INotificationTemplateService templateService,
        INotificationPreferenceService preferenceService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _templateService = templateService;
        _preferenceService = preferenceService;
        _logger = logger;
    }

    /// <summary>
    /// Get all notifications for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAllNotifications()
    {
        try
        {
            // Get current user ID from claims
            var userId = GetCurrentUserId();
            
            // This would typically call the notification service
            // For now, we'll return placeholder data
            var notifications = new List<NotificationDto>();
            
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all notifications");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationDto>> GetNotification(Guid id)
    {
        try
        {
            // This would typically call the notification service
            // For now, we'll return a placeholder response
            _logger.LogInformation("Getting notification {Id}", id);
            
            // Return 404 for now since we don't have actual notifications
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new notification
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NotificationDto>> CreateNotification([FromBody] CreateNotificationDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // This would typically call the notification service
            // For now, we'll return a placeholder response
            _logger.LogInformation("Creating notification with title: {Title}", createDto.Title);
            
            // Return a placeholder notification
            var notification = new NotificationDto
            {
                Id = 1,
                Title = createDto.Title,
                Message = createDto.Message,
                UserId = GetCurrentUserId(),
                CreatedAt = DateTime.UtcNow
            };
            
            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update notification status
    /// </summary>
    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> UpdateNotificationStatus(Guid id, [FromBody] string status)
    {
        try
        {
            // This would typically call the notification service
            // For now, we'll return a placeholder response
            _logger.LogInformation("Updating notification {Id} status to {Status}", id, status);
            
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification {Id} status", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete notification
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> DeleteNotification(Guid id)
    {
        try
        {
            // This would typically call the notification service
            // For now, we'll return a placeholder response
            _logger.LogInformation("Deleting notification {Id}", id);
            
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPut("{id}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> MarkAsRead(Guid id)
    {
        try
        {
            // This would typically call the notification service
            // For now, we'll return a placeholder response
            _logger.LogInformation("Marking notification {Id} as read", id);
            
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {Id} as read", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Mark all notifications as read for the current user
    /// </summary>
    [HttpPut("mark-all-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> MarkAllAsRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            
            // This would typically call the notification service
            // For now, we'll return a placeholder response
            _logger.LogInformation("Marking all notifications as read for user {UserId}", userId);
            
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get notification statistics for the current user
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetNotificationStats()
    {
        try
        {
            var userId = GetCurrentUserId();
            
            // This would typically call the notification service
            // For now, we'll return placeholder data
            var stats = new
            {
                TotalCount = 0,
                UnreadCount = 0,
                ReadCount = 0,
                TodayCount = 0,
                ThisWeekCount = 0
            };
            
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification statistics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get notification preferences for the current user
    /// </summary>
    [HttpGet("preferences")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetNotificationPreferences()
    {
        try
        {
            var userId = GetCurrentUserId();
            
            // This would typically call the preference service
            // For now, we'll return placeholder data
            var preferences = new
            {
                EmailEnabled = true,
                PushEnabled = true,
                SmsEnabled = false,
                QuietHoursEnabled = false,
                QuietHoursStart = "22:00",
                QuietHoursEnd = "08:00"
            };
            
            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification preferences");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update notification preferences for the current user
    /// </summary>
    [HttpPut("preferences")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> UpdateNotificationPreferences([FromBody] object preferences)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            // This would typically call the preference service
            // For now, we'll return a placeholder response
            _logger.LogInformation("Updating notification preferences for user {UserId}", userId);
            
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification preferences");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Send test notification to the current user
    /// </summary>
    [HttpPost("test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> SendTestNotification()
    {
        try
        {
            var userId = GetCurrentUserId();
            
            // This would typically call the notification service
            // For now, we'll return a placeholder response
            _logger.LogInformation("Sending test notification to user {UserId}", userId);
            
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending test notification");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get notification templates
    /// </summary>
    [HttpGet("templates")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetNotificationTemplates()
    {
        try
        {
            // This would typically call the template service
            // For now, we'll return placeholder data
            var templates = new List<object>
            {
                new { Id = 1, Name = "Welcome", Type = "User", Language = "en" },
                new { Id = 2, Name = "Password Reset", Type = "Security", Language = "en" },
                new { Id = 3, Name = "Exam Reminder", Type = "Exam", Language = "en" }
            };
            
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification templates");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Process notification template with variables
    /// </summary>
    [HttpPost("templates/{templateId}/process")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> ProcessTemplate(int templateId, [FromBody] Dictionary<string, object> variables)
    {
        try
        {
            // This would typically call the template service
            // For now, we'll return a placeholder response
            _logger.LogInformation("Processing template {TemplateId} with variables", templateId);
            
            var processedMessage = $"Template {templateId} processed with {variables.Count} variables";
            return Ok(processedMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing template {TemplateId}", templateId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get notification history for the current user
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetNotificationHistory()
    {
        try
        {
            var userId = GetCurrentUserId();
            
            // This would typically call the notification service
            // For now, we'll return placeholder data
            var history = new List<object>
            {
                new { Id = 1, Type = "Welcome", Status = "Delivered", DeliveredAt = DateTime.UtcNow.AddDays(-1) },
                new { Id = 2, Type = "Password Reset", Status = "Delivered", DeliveredAt = DateTime.UtcNow.AddDays(-2) }
            };
            
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification history");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get notification analytics for the current user
    /// </summary>
    [HttpGet("analytics")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetNotificationAnalytics()
    {
        try
        {
            var userId = GetCurrentUserId();
            
            // This would typically call the notification service
            // For now, we'll return placeholder data
            var analytics = new
            {
                TotalSent = 0,
                TotalDelivered = 0,
                TotalFailed = 0,
                DeliveryRate = 0.0,
                AverageDeliveryTime = 0,
                TopNotificationTypes = new string[0]
            };
            
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification analytics");
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
