// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Core.Entities;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Notification controller for managing notifications
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        INotificationService notificationService,
        ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Get notifications for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<NotificationDto>>> GetNotifications(
        [FromQuery] NotificationFilterDto filter)
    {
        try
        {
            // Get current user ID from claims
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId.Value, filter);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for user");
            return StatusCode(500, "An error occurred while retrieving notifications");
        }
    }

    /// <summary>
    /// Get unread notification count for the current user
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var count = await _notificationService.GetUnreadCountAsync(userId.Value);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count for user");
            return StatusCode(500, "An error occurred while retrieving unread count");
        }
    }

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    [HttpPost("{notificationId}/mark-read")]
    public async Task<ActionResult<bool>> MarkAsRead(int notificationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _notificationService.MarkAsReadAsync(notificationId, userId.Value);
            
            if (result)
            {
                return Ok(true);
            }
            else
            {
                return NotFound("Notification not found or already marked as read");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
            return StatusCode(500, "An error occurred while marking notification as read");
        }
    }

    /// <summary>
    /// Mark all notifications as read for the current user
    /// </summary>
    [HttpPost("mark-all-read")]
    public async Task<ActionResult<bool>> MarkAllAsRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _notificationService.MarkAllAsReadAsync(userId.Value);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user");
            return StatusCode(500, "An error occurred while marking all notifications as read");
        }
    }

    /// <summary>
    /// Create a new notification (admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<NotificationDto>> CreateNotification([FromBody] CreateNotificationDto dto)
    {
        try
        {
            var notification = await _notificationService.CreateNotificationAsync(dto);
            return CreatedAtAction(nameof(GetNotifications), new { id = notification.Id }, notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification");
            return StatusCode(500, "An error occurred while creating the notification");
        }
    }

    /// <summary>
    /// Send an immediate notification (admin only)
    /// </summary>
    [HttpPost("send-immediate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> SendImmediateNotification([FromBody] CreateNotificationDto dto)
    {
        try
        {
            var result = await _notificationService.SendImmediateNotificationAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending immediate notification");
            return StatusCode(500, "An error occurred while sending the notification");
        }
    }

    /// <summary>
    /// Send bulk notifications (admin only)
    /// </summary>
    [HttpPost("send-bulk")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> SendBulkNotifications([FromBody] List<CreateNotificationDto> notifications)
    {
        try
        {
            if (notifications == null || !notifications.Any())
            {
                return BadRequest("No notifications provided");
            }

            var result = await _notificationService.SendBulkNotificationAsync(notifications);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending bulk notifications");
            return StatusCode(500, "An error occurred while sending bulk notifications");
        }
    }

    /// <summary>
    /// Schedule a notification (admin only)
    /// </summary>
    [HttpPost("schedule")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> ScheduleNotification(
        [FromBody] CreateNotificationDto dto,
        [FromQuery] DateTime scheduleTime)
    {
        try
        {
            if (scheduleTime <= DateTime.UtcNow)
            {
                return BadRequest("Schedule time must be in the future");
            }

            var result = await _notificationService.ScheduleNotificationAsync(dto, scheduleTime);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling notification");
            return StatusCode(500, "An error occurred while scheduling the notification");
        }
    }

    /// <summary>
    /// Cancel a scheduled notification (admin only)
    /// </summary>
    [HttpPost("{notificationId}/cancel")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> CancelNotification(int notificationId)
    {
        try
        {
            var result = await _notificationService.CancelNotificationAsync(notificationId);
            
            if (result)
            {
                return Ok(true);
            }
            else
            {
                return NotFound("Notification not found or cannot be cancelled");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling notification {NotificationId}", notificationId);
            return StatusCode(500, "An error occurred while cancelling the notification");
        }
    }

    /// <summary>
    /// Send exam reminder notification (admin only)
    /// </summary>
    [HttpPost("exam-reminder")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SendExamReminder(
        [FromQuery] int examId,
        [FromQuery] int reminderMinutes)
    {
        try
        {
            await _notificationService.SendExamReminderAsync(examId, reminderMinutes);
            return Ok("Exam reminder notification sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending exam reminder for exam {ExamId}", examId);
            return StatusCode(500, "An error occurred while sending exam reminder");
        }
    }

    /// <summary>
    /// Send exam start notification (admin only)
    /// </summary>
    [HttpPost("exam-start")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SendExamStart([FromQuery] int examId)
    {
        try
        {
            await _notificationService.SendExamStartNotificationAsync(examId);
            return Ok("Exam start notification sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending exam start notification for exam {ExamId}", examId);
            return StatusCode(500, "An error occurred while sending exam start notification");
        }
    }

    /// <summary>
    /// Send exam end notification (admin only)
    /// </summary>
    [HttpPost("exam-end")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SendExamEnd([FromQuery] int examId)
    {
        try
        {
            await _notificationService.SendExamEndNotificationAsync(examId);
            return Ok("Exam end notification sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending exam end notification for exam {ExamId}", examId);
            return StatusCode(500, "An error occurred while sending exam end notification");
        }
    }

    /// <summary>
    /// Send grading complete notification (admin only)
    /// </summary>
    [HttpPost("grading-complete")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SendGradingComplete(
        [FromQuery] int examId,
        [FromQuery] int studentId)
    {
        try
        {
            await _notificationService.SendGradingCompleteNotificationAsync(examId, studentId);
            return Ok("Grading complete notification sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending grading complete notification for exam {ExamId}, student {StudentId}", examId, studentId);
            return StatusCode(500, "An error occurred while sending grading complete notification");
        }
    }

    /// <summary>
    /// Send deadline reminder notification (admin only)
    /// </summary>
    [HttpPost("deadline-reminder")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SendDeadlineReminder(
        [FromQuery] string entityType,
        [FromQuery] int entityId,
        [FromQuery] DateTime deadline)
    {
        try
        {
            await _notificationService.SendDeadlineReminderAsync(entityType, entityId, deadline);
            return Ok("Deadline reminder notification sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending deadline reminder for {EntityType}: {EntityId}", entityType, entityId);
            return StatusCode(500, "An error occurred while sending deadline reminder");
        }
    }

    /// <summary>
    /// Send welcome notification (admin only)
    /// </summary>
    [HttpPost("welcome")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SendWelcomeNotification([FromQuery] int userId)
    {
        try
        {
            await _notificationService.SendWelcomeNotificationAsync(userId);
            return Ok("Welcome notification sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending welcome notification for user {UserId}", userId);
            return StatusCode(500, "An error occurred while sending welcome notification");
        }
    }

    /// <summary>
    /// Send password reset notification (admin only)
    /// </summary>
    [HttpPost("password-reset")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SendPasswordResetNotification(
        [FromQuery] int userId,
        [FromQuery] string resetToken)
    {
        try
        {
            await _notificationService.SendPasswordResetNotificationAsync(userId, resetToken);
            return Ok("Password reset notification sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password reset notification for user {UserId}", userId);
            return StatusCode(500, "An error occurred while sending password reset notification");
        }
    }

    /// <summary>
    /// Send role assignment notification (admin only)
    /// </summary>
    [HttpPost("role-assignment")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SendRoleAssignmentNotification(
        [FromQuery] int userId,
        [FromQuery] string roleName)
    {
        try
        {
            await _notificationService.SendRoleAssignmentNotificationAsync(userId, roleName);
            return Ok("Role assignment notification sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending role assignment notification for user {UserId}", userId);
            return StatusCode(500, "An error occurred while sending role assignment notification");
        }
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }
}
*/
