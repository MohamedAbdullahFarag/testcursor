using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Linq;

namespace Ikhtibar.API.Controllers
{
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

        // === Notification Endpoints ===

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAllNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                // Create default filter for getting all user notifications
                var filter = new NotificationFilterDto
                {
                    Page = 1,
                    PageSize = 100 // Default page size, can be made configurable
                };
                var notifications = await _notificationService.GetUserNotificationsAsync(userId, filter);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications");
                return StatusCode(500, "An error occurred while retrieving notifications");
            }
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationDto>> GetNotification(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var notification = await _notificationService.GetNotificationByIdAsync(id, userId);
                
                if (notification == null)
                    return NotFound();
                
                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notification {NotificationId}", id);
                return StatusCode(500, "An error occurred while retrieving the notification");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationDto createDto)
        {
            try
            {
                var notification = await _notificationService.CreateNotificationAsync(createDto);
                return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                return StatusCode(500, "An error occurred while creating the notification");
            }
        }

        [HttpPost("send/{id:guid}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SendNotification(Guid id)
        {
            try
            {
                var success = await _notificationService.SendNotificationAsync(id);
                
                if (!success)
                    return NotFound();
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification {NotificationId}", id);
                return StatusCode(500, "An error occurred while sending the notification");
            }
        }

        [HttpPut("{id:guid}/read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> MarkAsRead(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _notificationService.MarkAsReadAsync(id, userId);
                
                if (!success)
                    return NotFound();
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read", id);
                return StatusCode(500, "An error occurred while updating the notification");
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteNotification(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _notificationService.DeleteNotificationAsync(id, userId);
                
                if (!success)
                    return NotFound();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
                return StatusCode(500, "An error occurred while deleting the notification");
            }
        }

        // === Template Endpoints ===

        [HttpGet("templates")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(IEnumerable<NotificationTemplateDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NotificationTemplateDto>>> GetAllTemplates()
        {
            try
            {
                // Create default filter to get all templates
                var filter = new TemplateFilterDto
                {
                    Page = 1,
                    PageSize = 100 // Default page size, can be made configurable
                    // IsActive = null to include both active and inactive templates for admin view
                };
                var templates = await _templateService.GetTemplatesAsync(filter);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notification templates");
                return StatusCode(500, "An error occurred while retrieving notification templates");
            }
        }

        [HttpGet("templates/{id:guid}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationTemplateDto>> GetTemplate(Guid id)
        {
            try
            {
                var template = await _templateService.GetTemplateAsync(id);
                
                if (template == null)
                    return NotFound();
                
                return Ok(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notification template {TemplateId}", id);
                return StatusCode(500, "An error occurred while retrieving the notification template");
            }
        }

        [HttpPost("templates")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NotificationTemplateDto>> CreateTemplate(CreateNotificationTemplateDto templateDto)
        {
            try
            {
                var template = await _templateService.CreateTemplateAsync(templateDto);
                return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification template");
                return StatusCode(500, "An error occurred while creating the notification template");
            }
        }

        [HttpPut("templates/{id:guid}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationTemplateDto>> UpdateTemplate(Guid id, UpdateNotificationTemplateDto templateDto)
        {
            try
            {
                var template = await _templateService.UpdateTemplateAsync(id, templateDto);
                
                if (template == null)
                    return NotFound();
                
                return Ok(template);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification template {TemplateId}", id);
                return StatusCode(500, "An error occurred while updating the notification template");
            }
        }

        [HttpDelete("templates/{id:guid}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteTemplate(Guid id)
        {
            try
            {
                var success = await _templateService.DeleteTemplateAsync(id);
                
                if (!success)
                    return NotFound();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification template {TemplateId}", id);
                return StatusCode(500, "An error occurred while deleting the notification template");
            }
        }

        // === Preference Endpoints ===

        [HttpGet("preferences")]
        [ProducesResponseType(typeof(IEnumerable<NotificationPreferenceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NotificationPreferenceDto>>> GetUserPreferences()
        {
            try
            {
                var userId = GetCurrentUserId();
                var preferences = await _preferenceService.GetUserPreferencesAsync(userId);
                return Ok(preferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notification preferences");
                return StatusCode(500, "An error occurred while retrieving notification preferences");
            }
        }

        [HttpPut("preferences")]
        [ProducesResponseType(typeof(NotificationPreferenceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NotificationPreferenceDto>> UpdatePreference(UpdateNotificationPreferenceDto preferenceDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var preference = await _preferenceService.UpdateUserPreferenceAsync(userId, preferenceDto);
                return Ok(preference);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification preferences");
                return StatusCode(500, "An error occurred while updating notification preferences");
            }
        }

        [HttpPut("preferences/quiet-hours")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateQuietHours(QuietHoursDto quietHoursDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _preferenceService.UpdateQuietHoursAsync(userId, quietHoursDto);
                return Ok();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quiet hours");
                return StatusCode(500, "An error occurred while updating quiet hours");
            }
        }

        // DTO Conversion Methods
        private CreateNotificationDto MapToCore(CreateNotificationDto dto)
        {
            return new CreateNotificationDto
            {
                UserId = dto.UserId,
                NotificationType = dto.NotificationType,
                Subject = dto.Subject,
                Message = dto.Message,
                Priority = dto.Priority,
                ScheduledAt = dto.ScheduledAt,
                EntityType = dto.EntityType,
                EntityId = dto.EntityId,
                TemplateId = dto.TemplateId,
                Variables = dto.Variables ?? new Dictionary<string, object>(),
                ChannelData = dto.ChannelData ?? new Dictionary<string, object>(),
                Metadata = dto.Metadata ?? new Dictionary<string, object>()
            };
        }

        private NotificationDto MapToApi(NotificationDto dto)
        {
            return new NotificationDto
            {
                Id = dto.Id,
                UserId = dto.UserId,
                NotificationType = dto.NotificationType,
                Priority = dto.Priority,
                Status = dto.Status,
                Subject = dto.Subject,
                Message = dto.Message,
                IsRead = dto.IsRead,
                ReadAt = dto.ReadAt,
                SentAt = dto.SentAt,
                ScheduledAt = dto.ScheduledAt,
                EntityType = dto.EntityType,
                EntityId = dto.EntityId,
                Variables = dto.Variables,
                ChannelData = dto.ChannelData,
                Metadata = dto.Metadata,
                CreatedAt = dto.CreatedAt,
                ModifiedAt = dto.ModifiedAt // Core DTO uses ModifiedAt, API DTO uses ModifiedAt
            };
        }

        private NotificationFilterDto MapToCore(NotificationFilterDto dto)
        {
            return new NotificationFilterDto
            {
                Page = dto.Page,
                PageSize = dto.PageSize,
                IsRead = dto.IsRead,
                NotificationType = dto.NotificationType,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                SearchTerm = dto.SearchTerm,
                EntityType = dto.EntityType,
                EntityId = dto.EntityId,
                Priority = dto.Priority,
                Status = dto.Status
            };
        }

        // Helper method to get the current user's ID from the claims
        private int GetCurrentUserId()
        {
            // In a real application, this would use the JWT claims to get the user ID
            // For simplicity in this exercise, we're returning a placeholder value
            var userIdClaim = User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                // Fallback for development
                return 1;
            }
            
            return userId;
        }
    }
}
