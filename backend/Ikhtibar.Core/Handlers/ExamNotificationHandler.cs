using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Events;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.Handlers;

/// <summary>
/// Event handler for exam-related domain events that trigger automated notifications.
/// Processes exam lifecycle events and creates appropriate notifications for students and instructors.
/// Integrates with the notification service to send multi-channel notifications (email, SMS, in-app).
/// </summary>
public class ExamNotificationHandler : 
    IEventHandler<ExamCreatedEvent>,
    IEventHandler<ExamStartingEvent>,
    IEventHandler<ExamDeadlineApproachingEvent>,
    IEventHandler<ExamCompletedEvent>,
    IEventHandler<ExamResultsPublishedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<ExamNotificationHandler> _logger;

    public ExamNotificationHandler(
        INotificationService notificationService,
        ILogger<ExamNotificationHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int Priority => 1; // High priority for exam notifications
    public bool ContinueOnFailure => true; // Continue processing other handlers on failure

    /// <summary>
    /// Handles exam creation events by sending welcome notifications to enrolled students.
    /// Creates notifications with exam details, schedule, and preparation information.
    /// </summary>
    public async Task HandleAsync(ExamCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Handling ExamCreatedEvent {EventId} for exam {ExamId}", 
            domainEvent.EventId, domainEvent.ExamId);

        try
        {
            _logger.LogInformation("Processing exam creation notification for {StudentCount} students", 
                domainEvent.StudentIds.Count);

            // Create notification for each enrolled student
            foreach (var studentId in domainEvent.StudentIds)
            {
                await _notificationService.CreateNotificationAsync(new Shared.DTOs.CreateNotificationDto
                {
                    UserId = studentId,
                    NotificationType = NotificationType.Announcement,
                    Priority = NotificationPriority.Normal,
                    Subject = $"New Exam Available: {domainEvent.ExamTitle}",
                    Message = $"A new exam '{domainEvent.ExamTitle}' has been created by {domainEvent.InstructorName}. " +
                             $"Scheduled for {domainEvent.ScheduledStartDate:yyyy-MM-dd HH:mm}. " +
                             $"Duration: {domainEvent.DurationMinutes} minutes.",
                    ScheduledAt = DateTime.UtcNow.AddMinutes(5), // Small delay for batching
                    EntityType = "Exam",
                    EntityId = domainEvent.ExamId,
                    Metadata = new Dictionary<string, object>
                    {
                        ["ExamId"] = domainEvent.ExamId,
                        ["InstructorName"] = domainEvent.InstructorName,
                        ["ScheduledStartDate"] = domainEvent.ScheduledStartDate,
                        ["DurationMinutes"] = domainEvent.DurationMinutes,
                        ["EventId"] = domainEvent.EventId.ToString(),
                        ["CorrelationId"] = domainEvent.CorrelationId ?? string.Empty
                    }
                });
            }

            // Create summary notification for instructor
            await _notificationService.CreateNotificationAsync(new Shared.DTOs.CreateNotificationDto
            {
                UserId = domainEvent.InstructorId,
                Type = NotificationType.ExamCreated,
                Priority = NotificationPriority.Low,
                Title = $"Exam Created Successfully: {domainEvent.ExamTitle}",
                Content = $"Your exam '{domainEvent.ExamTitle}' has been created and notifications sent to {domainEvent.StudentIds.Count} students.",
                Channels = new List<NotificationChannel> { NotificationChannel.InApp },
                Metadata = new Dictionary<string, object>
                {
                    ["ExamId"] = domainEvent.ExamId,
                    ["StudentCount"] = domainEvent.StudentIds.Count,
                    ["EventId"] = domainEvent.EventId.ToString()
                }
            });

            _logger.LogInformation("Successfully created {NotificationCount} exam creation notifications", 
                domainEvent.StudentIds.Count + 1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling exam creation event {EventId}", domainEvent.EventId);
            throw; // Re-throw to allow retry mechanisms
        }
    }

    /// <summary>
    /// Handles exam starting events by sending reminder notifications to students.
    /// Creates urgent notifications with exam access details and last-minute reminders.
    /// </summary>
    public async Task HandleAsync(ExamStartingEvent domainEvent, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Handling ExamStartingEvent {EventId} for exam {ExamId}", 
            domainEvent.EventId, domainEvent.ExamId);

        try
        {
            _logger.LogInformation("Processing exam starting notification for {StudentCount} students, " +
                                 "exam starts in {TimeUntilStart}", 
                                 domainEvent.StudentIds.Count, domainEvent.TimeUntilStart);

            // Determine priority based on time until start
            var priority = domainEvent.TimeUntilStart.TotalMinutes <= 15 
                ? NotificationPriority.Critical 
                : NotificationPriority.High;

            // Create reminder notification for each student
            foreach (var studentId in domainEvent.StudentIds)
            {
                await _notificationService.CreateNotificationAsync(new Shared.DTOs.CreateNotificationDto
                {
                    UserId = studentId,
                    Type = NotificationType.ExamReminder,
                    Priority = priority,
                    Title = $"Exam Starting Soon: {domainEvent.ExamTitle}",
                    Content = $"Your exam '{domainEvent.ExamTitle}' starts in {FormatTimeSpan(domainEvent.TimeUntilStart)}. " +
                             $"Click here to access: {domainEvent.ExamUrl}",
                    Channels = new List<NotificationChannel> 
                    { 
                        NotificationChannel.Email, 
                        NotificationChannel.InApp,
                        NotificationChannel.SMS // SMS for urgent reminders
                    },
                    ScheduledFor = DateTime.UtcNow, // Immediate delivery
                    ExpiresAt = domainEvent.StartTime.AddMinutes(15), // Expires 15 minutes after start
                    Metadata = new Dictionary<string, object>
                    {
                        ["ExamId"] = domainEvent.ExamId,
                        ["ExamUrl"] = domainEvent.ExamUrl,
                        ["StartTime"] = domainEvent.StartTime,
                        ["TimeUntilStart"] = domainEvent.TimeUntilStart.ToString(),
                        ["EventId"] = domainEvent.EventId.ToString()
                    }
                });
            }

            _logger.LogInformation("Successfully created {NotificationCount} exam starting notifications", 
                domainEvent.StudentIds.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling exam starting event {EventId}", domainEvent.EventId);
            throw;
        }
    }

    /// <summary>
    /// Handles exam deadline approaching events by sending urgent reminders to students.
    /// Creates high-priority notifications for students who haven't submitted yet.
    /// </summary>
    public async Task HandleAsync(ExamDeadlineApproachingEvent domainEvent, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Handling ExamDeadlineApproachingEvent {EventId} for exam {ExamId}", 
            domainEvent.EventId, domainEvent.ExamId);

        try
        {
            _logger.LogInformation("Processing deadline reminder for {PendingStudentCount} students, " +
                                 "deadline in {TimeUntilDeadline}", 
                                 domainEvent.PendingStudentIds.Count, domainEvent.TimeUntilDeadline);

            // Determine priority and channels based on urgency
            var isUrgent = domainEvent.TimeUntilDeadline.TotalHours < 1;
            var priority = isUrgent ? NotificationPriority.Critical : NotificationPriority.High;
            var channels = new List<NotificationChannel> 
            { 
                NotificationChannel.Email, 
                NotificationChannel.InApp 
            };
            
            if (isUrgent)
            {
                channels.Add(NotificationChannel.SMS); // Add SMS for urgent deadlines
            }

            // Create deadline reminder for each pending student
            foreach (var studentId in domainEvent.PendingStudentIds)
            {
                await _notificationService.CreateNotificationAsync(new Shared.DTOs.CreateNotificationDto
                {
                    UserId = studentId,
                    Type = NotificationType.ExamDeadline,
                    Priority = priority,
                    Title = $"⚠️ Exam Deadline Approaching: {domainEvent.ExamTitle}",
                    Content = $"URGENT: Your exam '{domainEvent.ExamTitle}' deadline is in {FormatTimeSpan(domainEvent.TimeUntilDeadline)}. " +
                             $"Submit your answers now: {domainEvent.SubmissionUrl}",
                    Channels = channels,
                    ScheduledFor = DateTime.UtcNow, // Immediate delivery
                    ExpiresAt = domainEvent.Deadline.AddMinutes(5), // Expires shortly after deadline
                    Metadata = new Dictionary<string, object>
                    {
                        ["ExamId"] = domainEvent.ExamId,
                        ["SubmissionUrl"] = domainEvent.SubmissionUrl,
                        ["Deadline"] = domainEvent.Deadline,
                        ["TimeUntilDeadline"] = domainEvent.TimeUntilDeadline.ToString(),
                        ["IsUrgent"] = isUrgent,
                        ["EventId"] = domainEvent.EventId.ToString()
                    }
                });
            }

            _logger.LogInformation("Successfully created {NotificationCount} deadline reminder notifications", 
                domainEvent.PendingStudentIds.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling exam deadline event {EventId}", domainEvent.EventId);
            throw;
        }
    }

    /// <summary>
    /// Handles exam completion events by sending completion notifications.
    /// Creates notifications for students confirming submission and for instructors with statistics.
    /// </summary>
    public async Task HandleAsync(ExamCompletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Handling ExamCompletedEvent {EventId} for exam {ExamId}", 
            domainEvent.EventId, domainEvent.ExamId);

        try
        {
            _logger.LogInformation("Processing exam completion notification for {ParticipantCount} participants", 
                domainEvent.ParticipantIds.Count);

            // Create completion confirmation for each participant
            foreach (var participantId in domainEvent.ParticipantIds)
            {
                await _notificationService.CreateNotificationAsync(new Shared.DTOs.CreateNotificationDto
                {
                    UserId = participantId,
                    Type = NotificationType.ExamCompleted,
                    Priority = NotificationPriority.Normal,
                    Title = $"Exam Completed: {domainEvent.ExamTitle}",
                    Content = $"You have successfully completed the exam '{domainEvent.ExamTitle}'. " +
                             $"Your submission was recorded at {domainEvent.EndTime:yyyy-MM-dd HH:mm}. " +
                             $"Results will be available soon.",
                    Channels = new List<NotificationChannel> 
                    { 
                        NotificationChannel.Email, 
                        NotificationChannel.InApp 
                    },
                    Metadata = new Dictionary<string, object>
                    {
                        ["ExamId"] = domainEvent.ExamId,
                        ["EndTime"] = domainEvent.EndTime,
                        ["EventId"] = domainEvent.EventId.ToString()
                    }
                });
            }

            // Create summary notification for instructor
            var completionRate = domainEvent.ParticipantIds.Count > 0 
                ? (double)domainEvent.CompletedCount / domainEvent.ParticipantIds.Count * 100 
                : 0;

            await _notificationService.CreateNotificationAsync(new Shared.DTOs.CreateNotificationDto
            {
                UserId = domainEvent.InstructorId,
                Type = NotificationType.ExamCompleted,
                Priority = NotificationPriority.Normal,
                Title = $"Exam Session Completed: {domainEvent.ExamTitle}",
                Content = $"The exam session for '{domainEvent.ExamTitle}' has ended. " +
                         $"Summary: {domainEvent.CompletedCount} completed, {domainEvent.IncompleteCount} incomplete " +
                         $"({completionRate:F1}% completion rate).",
                Channels = new List<NotificationChannel> { NotificationChannel.InApp },
                Metadata = new Dictionary<string, object>
                {
                    ["ExamId"] = domainEvent.ExamId,
                    ["ParticipantCount"] = domainEvent.ParticipantIds.Count,
                    ["CompletedCount"] = domainEvent.CompletedCount,
                    ["IncompleteCount"] = domainEvent.IncompleteCount,
                    ["CompletionRate"] = completionRate,
                    ["EndTime"] = domainEvent.EndTime,
                    ["EventId"] = domainEvent.EventId.ToString()
                }
            });

            _logger.LogInformation("Successfully created {NotificationCount} exam completion notifications", 
                domainEvent.ParticipantIds.Count + 1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling exam completion event {EventId}", domainEvent.EventId);
            throw;
        }
    }

    /// <summary>
    /// Handles exam results published events by sending result notifications to students.
    /// Creates notifications with grade information and feedback access details.
    /// </summary>
    public async Task HandleAsync(ExamResultsPublishedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Handling ExamResultsPublishedEvent {EventId} for exam {ExamId}", 
            domainEvent.EventId, domainEvent.ExamId);

        try
        {
            _logger.LogInformation("Processing results publication notification for {StudentCount} students", 
                domainEvent.StudentIds.Count);

            // Create results notification for each student
            foreach (var studentId in domainEvent.StudentIds)
            {
                await _notificationService.CreateNotificationAsync(new Shared.DTOs.CreateNotificationDto
                {
                    UserId = studentId,
                    Type = NotificationType.ExamResultsPublished,
                    Priority = NotificationPriority.Normal,
                    Title = $"Exam Results Available: {domainEvent.ExamTitle}",
                    Content = $"Your results for '{domainEvent.ExamTitle}' are now available. " +
                             $"Published by {domainEvent.InstructorName} on {domainEvent.PublishedAt:yyyy-MM-dd}. " +
                             $"View your results: {domainEvent.ResultsUrl}" +
                             (domainEvent.HasFeedback ? " (Includes detailed feedback)" : ""),
                    Channels = new List<NotificationChannel> 
                    { 
                        NotificationChannel.Email, 
                        NotificationChannel.InApp 
                    },
                    Metadata = new Dictionary<string, object>
                    {
                        ["ExamId"] = domainEvent.ExamId,
                        ["ResultsUrl"] = domainEvent.ResultsUrl,
                        ["InstructorName"] = domainEvent.InstructorName,
                        ["PublishedAt"] = domainEvent.PublishedAt,
                        ["HasFeedback"] = domainEvent.HasFeedback,
                        ["EventId"] = domainEvent.EventId.ToString()
                    }
                });
            }

            _logger.LogInformation("Successfully created {NotificationCount} exam results notifications", 
                domainEvent.StudentIds.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling exam results event {EventId}", domainEvent.EventId);
            throw;
        }
    }

    /// <summary>
    /// Formats a TimeSpan into a human-readable string for notifications.
    /// </summary>
    private static string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalDays >= 1)
            return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays != 1 ? "s" : "")}";
        
        if (timeSpan.TotalHours >= 1)
            return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours != 1 ? "s" : "")}";
        
        return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes != 1 ? "s" : "")}";
    }
}
