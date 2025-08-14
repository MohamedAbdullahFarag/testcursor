# Notification System - Comprehensive Implementation PRP

## 🎯 Executive Summary
Generate a comprehensive notification system for the Ikhtibar educational assessment platform that handles email, SMS notifications for deadlines, exam events, and grading completion. This system will provide event-driven messaging, user communication preferences, multi-channel delivery, and integration with all existing system components including authentication, role management, and audit logging.

## 📋 What to Generate

### 1. Backend Notification System
```
backend/Ikhtibar.Core/Entities/
├── Notification.cs                    # Core notification entity
├── NotificationTemplate.cs            # Template management entity  
├── NotificationPreference.cs          # User preference entity
├── NotificationHistory.cs             # Delivery tracking entity
└── NotificationChannel.cs             # Channel management entity

backend/Ikhtibar.Core/Services/Interfaces/
├── INotificationService.cs            # Core notification operations
├── IEmailService.cs                   # Email-specific operations  
├── ISmsService.cs                     # SMS-specific operations
├── INotificationTemplateService.cs    # Template management
└── INotificationPreferenceService.cs  # User preference management

backend/Ikhtibar.Core/Services/Implementations/
├── NotificationService.cs             # Main notification logic
├── EmailService.cs                    # Email delivery implementation
├── SmsService.cs                      # SMS delivery implementation  
├── NotificationTemplateService.cs     # Template operations
└── NotificationPreferenceService.cs   # Preference management

backend/Ikhtibar.Core/Repositories/Interfaces/
├── INotificationRepository.cs         # Notification data access
├── INotificationTemplateRepository.cs # Template data access
├── INotificationPreferenceRepository.cs # Preference data access
└── INotificationHistoryRepository.cs  # History data access

backend/Ikhtibar.Infrastructure/Repositories/
├── NotificationRepository.cs          # Notification repository implementation
├── NotificationTemplateRepository.cs  # Template repository implementation
├── NotificationPreferenceRepository.cs # Preference repository implementation
└── NotificationHistoryRepository.cs   # History repository implementation

backend/Ikhtibar.API/Controllers/
├── NotificationController.cs          # Notification management endpoints
├── NotificationPreferenceController.cs # User preference endpoints
└── NotificationTemplateController.cs  # Template management endpoints

backend/Ikhtibar.API/DTOs/
├── NotificationDto.cs                 # Notification data transfer objects
├── CreateNotificationDto.cs           # Notification creation objects
├── NotificationPreferenceDto.cs       # Preference data objects
├── NotificationTemplateDto.cs         # Template data objects
└── NotificationHistoryDto.cs          # History data objects
```

### 2. Event-Driven Notification System
```
backend/Ikhtibar.Core/Events/
├── ExamEventNotifications.cs          # Exam-related events
├── GradingEventNotifications.cs       # Grading completion events
├── UserEventNotifications.cs          # User management events
├── SystemEventNotifications.cs        # System-wide notifications
└── DeadlineEventNotifications.cs      # Deadline reminder events

backend/Ikhtibar.Core/Handlers/
├── ExamNotificationHandler.cs         # Exam event handlers
├── GradingNotificationHandler.cs      # Grading event handlers
├── UserNotificationHandler.cs         # User event handlers
└── SystemNotificationHandler.cs       # System event handlers

backend/Ikhtibar.Infrastructure/Messaging/
├── EventBusService.cs                 # Event bus implementation
├── NotificationScheduler.cs           # Scheduled notification service
└── NotificationQueue.cs               # Queue management service
```

### 3. Frontend Notification Interface
```
frontend/src/modules/notifications/
├── components/
│   ├── NotificationCenter.tsx         # Main notification center
│   ├── NotificationItem.tsx           # Individual notification display
│   ├── NotificationList.tsx           # Notification list component
│   ├── NotificationPreferences.tsx    # User preference settings
│   ├── NotificationBell.tsx           # Header notification bell
│   ├── NotificationModal.tsx          # Modal for detailed notifications
│   ├── NotificationToast.tsx          # Toast notification component
│   └── NotificationTemplateManager.tsx # Admin template management
├── hooks/
│   ├── useNotifications.tsx           # Main notification hook
│   ├── useNotificationPreferences.tsx # Preference management hook
│   ├── useNotificationTemplates.tsx   # Template management hook
│   └── useRealTimeNotifications.tsx   # Real-time notification hook
├── services/
│   ├── notificationService.ts         # Frontend notification service
│   ├── notificationPreferenceService.ts # Preference service
│   └── notificationTemplateService.ts # Template service
├── types/
│   ├── notification.types.ts          # TypeScript interfaces
│   ├── notificationPreference.types.ts # Preference types
│   └── notificationTemplate.types.ts  # Template types
├── locales/
│   ├── en.json                        # English translations
│   └── ar.json                        # Arabic translations
└── constants/
    ├── notificationTypes.ts           # Notification type constants
    ├── notificationChannels.ts        # Channel type constants
    └── notificationTemplates.ts       # Template constants
```

### 4. Integration Components
```
backend/Ikhtibar.Core/Integrations/
├── EmailProviders/
│   ├── SendGridProvider.cs            # SendGrid email provider
│   ├── SmtpProvider.cs                # SMTP email provider
│   └── IEmailProvider.cs              # Email provider interface
├── SmsProviders/
│   ├── TwilioProvider.cs              # Twilio SMS provider
│   ├── NafathProvider.cs              # Saudi Nafath SMS provider
│   └── ISmsProvider.cs                # SMS provider interface
└── PushNotificationProviders/
    ├── FirebaseProvider.cs            # Firebase push notifications
    └── IPushNotificationProvider.cs   # Push notification interface
```

## 🏗 Implementation Architecture

### Entity Design Patterns

#### Core Notification Entity
```csharp
[Table("Notifications")]
public class Notification : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    [Required]
    public NotificationType Type { get; set; }

    [Required]
    public NotificationPriority Priority { get; set; }

    [Required]
    public NotificationStatus Status { get; set; }

    [Required]
    public int UserId { get; set; }

    [MaxLength(100)]
    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    [Required]
    public DateTime ScheduledAt { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? ReadAt { get; set; }

    [MaxLength(2000)]
    public string? MetadataJson { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<NotificationHistory> History { get; set; } = new List<NotificationHistory>();
}

public enum NotificationType
{
    ExamReminder = 1,
    ExamStart = 2,
    ExamEnd = 3,
    GradingComplete = 4,
    DeadlineReminder = 5,
    SystemAlert = 6,
    UserWelcome = 7,
    PasswordReset = 8,
    AccountActivation = 9,
    RoleAssignment = 10
}

public enum NotificationPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum NotificationStatus
{
    Pending = 1,
    Sent = 2,
    Delivered = 3,
    Read = 4,
    Failed = 5,
    Cancelled = 6
}
```

#### Notification Template Entity
```csharp
[Table("NotificationTemplates")]
public class NotificationTemplate : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public NotificationType Type { get; set; }

    [Required]
    [MaxLength(200)]
    public string SubjectTemplate { get; set; } = string.Empty;

    [Required]
    [MaxLength(5000)]
    public string MessageTemplate { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string Language { get; set; } = string.Empty;

    [Required]
    public bool IsActive { get; set; } = true;

    [MaxLength(1000)]
    public string? VariablesJson { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
```

#### Notification Preference Entity
```csharp
[Table("NotificationPreferences")]
public class NotificationPreference : BaseEntity
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public NotificationType NotificationType { get; set; }

    [Required]
    public bool EmailEnabled { get; set; } = true;

    [Required]
    public bool SmsEnabled { get; set; } = false;

    [Required]
    public bool InAppEnabled { get; set; } = true;

    [Required]
    public bool PushEnabled { get; set; } = true;

    // Navigation properties
    public virtual User User { get; set; } = null!;
}
```

#### Notification History Entity
```csharp
[Table("NotificationHistory")]
public class NotificationHistory : BaseEntity
{
    [Required]
    public int NotificationId { get; set; }

    [Required]
    public NotificationChannel Channel { get; set; }

    [Required]
    public NotificationDeliveryStatus Status { get; set; }

    [Required]
    public DateTime AttemptedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    [MaxLength(500)]
    public string? ExternalId { get; set; }

    [MaxLength(2000)]
    public string? ResponseData { get; set; }

    // Navigation properties
    public virtual Notification Notification { get; set; } = null!;
}

public enum NotificationChannel
{
    Email = 1,
    SMS = 2,
    InApp = 3,
    Push = 4,
    WhatsApp = 5
}

public enum NotificationDeliveryStatus
{
    Pending = 1,
    Sent = 2,
    Delivered = 3,
    Failed = 4,
    Bounced = 5,
    Opened = 6,
    Clicked = 7
}
```

### Service Implementation Patterns

#### Core Notification Service
```csharp
public interface INotificationService
{
    // Core notification operations
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto);
    Task<bool> SendNotificationAsync(int notificationId);
    Task<bool> SendImmediateNotificationAsync(CreateNotificationDto dto);
    Task<PagedResult<NotificationDto>> GetUserNotificationsAsync(int userId, NotificationFilterDto filter);
    Task<bool> MarkAsReadAsync(int notificationId, int userId);
    Task<bool> MarkAllAsReadAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    
    // Bulk operations
    Task<bool> SendBulkNotificationAsync(List<CreateNotificationDto> notifications);
    Task<bool> ScheduleNotificationAsync(CreateNotificationDto dto, DateTime scheduleTime);
    Task<bool> CancelNotificationAsync(int notificationId);
    
    // Event-driven notifications
    Task SendExamReminderAsync(int examId, int reminderMinutes);
    Task SendExamStartNotificationAsync(int examId);
    Task SendExamEndNotificationAsync(int examId);
    Task SendGradingCompleteNotificationAsync(int examId, int studentId);
    Task SendDeadlineReminderAsync(string entityType, int entityId, DateTime deadline);
    Task SendWelcomeNotificationAsync(int userId);
    Task SendPasswordResetNotificationAsync(int userId, string resetToken);
    Task SendRoleAssignmentNotificationAsync(int userId, string roleName);
}

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationTemplateService _templateService;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly INotificationPreferenceService _preferenceService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<NotificationService> _logger;
    private readonly IEventBusService _eventBus;

    public NotificationService(
        INotificationRepository notificationRepository,
        INotificationTemplateService templateService,
        IEmailService emailService,
        ISmsService smsService,
        INotificationPreferenceService preferenceService,
        IUserRepository userRepository,
        ILogger<NotificationService> logger,
        IEventBusService eventBus)
    {
        _notificationRepository = notificationRepository;
        _templateService = templateService;
        _emailService = emailService;
        _smsService = smsService;
        _preferenceService = preferenceService;
        _userRepository = userRepository;
        _logger = logger;
        _eventBus = eventBus;
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
    {
        try
        {
            _logger.LogInformation("Creating notification for user: {UserId}, Type: {Type}", dto.UserId, dto.Type);

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {dto.UserId} not found");
            }

            // Create notification entity
            var notification = new Notification
            {
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                Priority = dto.Priority,
                Status = NotificationStatus.Pending,
                UserId = dto.UserId,
                EntityType = dto.EntityType,
                EntityId = dto.EntityId,
                ScheduledAt = dto.ScheduledAt ?? DateTime.UtcNow,
                MetadataJson = dto.MetadataJson
            };

            var savedNotification = await _notificationRepository.AddAsync(notification);
            
            _logger.LogInformation("Notification created successfully with ID: {NotificationId}", savedNotification.Id);

            // Publish notification created event
            await _eventBus.PublishAsync(new NotificationCreatedEvent(savedNotification.Id, savedNotification.UserId, savedNotification.Type));

            return MapToDto(savedNotification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification for user: {UserId}", dto.UserId);
            throw;
        }
    }

    public async Task<bool> SendNotificationAsync(int notificationId)
    {
        try
        {
            _logger.LogInformation("Sending notification: {NotificationId}", notificationId);

            var notification = await _notificationRepository.GetByIdWithUserAsync(notificationId);
            if (notification == null)
            {
                _logger.LogWarning("Notification not found: {NotificationId}", notificationId);
                return false;
            }

            if (notification.Status != NotificationStatus.Pending)
            {
                _logger.LogWarning("Notification {NotificationId} is not in pending status", notificationId);
                return false;
            }

            // Get user preferences
            var preferences = await _preferenceService.GetUserPreferencesAsync(notification.UserId);
            var typePreference = preferences.FirstOrDefault(p => p.NotificationType == notification.Type);

            var deliveryTasks = new List<Task<bool>>();

            // Send via enabled channels
            if (typePreference?.EmailEnabled == true && !string.IsNullOrEmpty(notification.User.Email))
            {
                deliveryTasks.Add(SendEmailNotificationAsync(notification));
            }

            if (typePreference?.SmsEnabled == true && !string.IsNullOrEmpty(notification.User.PhoneNumber))
            {
                deliveryTasks.Add(SendSmsNotificationAsync(notification));
            }

            if (typePreference?.InAppEnabled == true)
            {
                // In-app notifications are already stored, just mark as sent
                deliveryTasks.Add(Task.FromResult(true));
            }

            // Execute all delivery attempts
            var results = await Task.WhenAll(deliveryTasks);
            var success = results.Any(r => r);

            // Update notification status
            notification.Status = success ? NotificationStatus.Sent : NotificationStatus.Failed;
            notification.SentAt = success ? DateTime.UtcNow : null;
            await _notificationRepository.UpdateAsync(notification);

            _logger.LogInformation("Notification {NotificationId} sending completed. Success: {Success}", notificationId, success);

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification: {NotificationId}", notificationId);
            return false;
        }
    }

    public async Task<bool> SendImmediateNotificationAsync(CreateNotificationDto dto)
    {
        try
        {
            // Create and immediately send notification
            var notificationDto = await CreateNotificationAsync(dto);
            return await SendNotificationAsync(notificationDto.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending immediate notification for user: {UserId}", dto.UserId);
            return false;
        }
    }

    public async Task SendExamReminderAsync(int examId, int reminderMinutes)
    {
        try
        {
            _logger.LogInformation("Sending exam reminder for exam: {ExamId}, Minutes: {Minutes}", examId, reminderMinutes);

            // Get exam details and enrolled students
            var exam = await _examRepository.GetByIdWithStudentsAsync(examId);
            if (exam == null) return;

            var template = await _templateService.GetTemplateAsync(NotificationType.ExamReminder, "en");
            if (template == null)
            {
                _logger.LogWarning("No template found for exam reminder");
                return;
            }

            var tasks = exam.EnrolledStudents.Select(async student =>
            {
                var variables = new Dictionary<string, object>
                {
                    ["ExamTitle"] = exam.Title,
                    ["ReminderMinutes"] = reminderMinutes,
                    ["ExamStartTime"] = exam.StartTime.ToString("yyyy-MM-dd HH:mm"),
                    ["StudentName"] = student.User.FullName
                };

                var message = await _templateService.ProcessTemplateAsync(template, variables);

                var notificationDto = new CreateNotificationDto
                {
                    UserId = student.UserId,
                    Type = NotificationType.ExamReminder,
                    Priority = NotificationPriority.High,
                    Title = $"Exam Reminder: {exam.Title}",
                    Message = message,
                    EntityType = "Exam",
                    EntityId = examId,
                    MetadataJson = JsonSerializer.Serialize(variables)
                };

                return await SendImmediateNotificationAsync(notificationDto);
            });

            await Task.WhenAll(tasks);
            _logger.LogInformation("Exam reminder sent for exam: {ExamId}", examId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending exam reminder for exam: {ExamId}", examId);
        }
    }

    // Additional notification methods implementation...
    
    private async Task<bool> SendEmailNotificationAsync(Notification notification)
    {
        try
        {
            var emailRequest = new EmailRequest
            {
                To = notification.User.Email,
                Subject = notification.Title,
                Body = notification.Message,
                IsHtml = true
            };

            var emailResult = await _emailService.SendEmailAsync(emailRequest);
            
            // Record delivery attempt
            await RecordDeliveryAttemptAsync(notification.Id, NotificationChannel.Email, emailResult.Success, emailResult.ErrorMessage);
            
            return emailResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email notification: {NotificationId}", notification.Id);
            await RecordDeliveryAttemptAsync(notification.Id, NotificationChannel.Email, false, ex.Message);
            return false;
        }
    }

    private async Task<bool> SendSmsNotificationAsync(Notification notification)
    {
        try
        {
            var smsRequest = new SmsRequest
            {
                To = notification.User.PhoneNumber,
                Message = notification.Message
            };

            var smsResult = await _smsService.SendSmsAsync(smsRequest);
            
            // Record delivery attempt
            await RecordDeliveryAttemptAsync(notification.Id, NotificationChannel.SMS, smsResult.Success, smsResult.ErrorMessage);
            
            return smsResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS notification: {NotificationId}", notification.Id);
            await RecordDeliveryAttemptAsync(notification.Id, NotificationChannel.SMS, false, ex.Message);
            return false;
        }
    }

    private async Task RecordDeliveryAttemptAsync(int notificationId, NotificationChannel channel, bool success, string? errorMessage)
    {
        var history = new NotificationHistory
        {
            NotificationId = notificationId,
            Channel = channel,
            Status = success ? NotificationDeliveryStatus.Sent : NotificationDeliveryStatus.Failed,
            AttemptedAt = DateTime.UtcNow,
            DeliveredAt = success ? DateTime.UtcNow : null,
            ErrorMessage = errorMessage
        };

        await _notificationHistoryRepository.AddAsync(history);
    }

    private NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            Priority = notification.Priority,
            Status = notification.Status,
            UserId = notification.UserId,
            EntityType = notification.EntityType,
            EntityId = notification.EntityId,
            ScheduledAt = notification.ScheduledAt,
            SentAt = notification.SentAt,
            ReadAt = notification.ReadAt,
            CreatedAt = notification.CreatedAt
        };
    }
}
```

#### Email Service Implementation
```csharp
public interface IEmailService
{
    Task<EmailResult> SendEmailAsync(EmailRequest request);
    Task<EmailResult> SendTemplatedEmailAsync(TemplatedEmailRequest request);
    Task<bool> ValidateEmailAsync(string email);
}

public class EmailService : IEmailService
{
    private readonly IEmailProvider _emailProvider;
    private readonly ILogger<EmailService> _logger;
    private readonly EmailSettings _emailSettings;

    public EmailService(
        IEmailProvider emailProvider,
        ILogger<EmailService> logger,
        IOptions<EmailSettings> emailSettings)
    {
        _emailProvider = emailProvider;
        _logger = logger;
        _emailSettings = emailSettings.Value;
    }

    public async Task<EmailResult> SendEmailAsync(EmailRequest request)
    {
        try
        {
            _logger.LogInformation("Sending email to: {Recipient}", request.To);

            // Validate email request
            if (!ValidateEmailRequest(request))
            {
                return new EmailResult { Success = false, ErrorMessage = "Invalid email request" };
            }

            // Enhanced request with sender info
            var enhancedRequest = new EmailRequest
            {
                From = _emailSettings.FromAddress,
                FromName = _emailSettings.FromName,
                To = request.To,
                Subject = request.Subject,
                Body = request.Body,
                IsHtml = request.IsHtml,
                ReplyTo = _emailSettings.ReplyToAddress
            };

            var result = await _emailProvider.SendEmailAsync(enhancedRequest);
            
            _logger.LogInformation("Email send result for {Recipient}: Success={Success}", 
                request.To, result.Success);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to: {Recipient}", request.To);
            return new EmailResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    private bool ValidateEmailRequest(EmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.To) || 
            string.IsNullOrWhiteSpace(request.Subject) || 
            string.IsNullOrWhiteSpace(request.Body))
        {
            return false;
        }

        return ValidateEmailAsync(request.To).Result;
    }

    public async Task<bool> ValidateEmailAsync(string email)
    {
        try
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }
}

public class EmailRequest
{
    public string From { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public string? ReplyTo { get; set; }
    public List<string>? Attachments { get; set; }
}

public class EmailResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? MessageId { get; set; }
}
```

### Frontend Implementation Patterns

#### Notification Center Component
```typescript
// frontend/src/modules/notifications/components/NotificationCenter.tsx
import React, { useState, useEffect } from 'react';
import { useNotifications } from '../hooks/useNotifications';
import { NotificationList } from './NotificationList';
import { NotificationPreferences } from './NotificationPreferences';
import { Button } from '@/shared/components/ui/button';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/shared/components/ui/tabs';
import { Badge } from '@/shared/components/ui/badge';
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger } from '@/shared/components/ui/sheet';
import { Bell, Settings, MarkEmailRead } from '@mui/icons-material';

interface NotificationCenterProps {
  userId: number;
  className?: string;
}

export const NotificationCenter: React.FC<NotificationCenterProps> = ({ 
  userId, 
  className 
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [activeTab, setActiveTab] = useState('notifications');
  
  const {
    notifications,
    unreadCount,
    isLoading,
    error,
    markAsRead,
    markAllAsRead,
    refreshNotifications
  } = useNotifications(userId);

  useEffect(() => {
    refreshNotifications();
  }, [userId]);

  const handleMarkAllAsRead = async () => {
    try {
      await markAllAsRead();
      await refreshNotifications();
    } catch (error) {
      console.error('Error marking all notifications as read:', error);
    }
  };

  const handleNotificationRead = async (notificationId: number) => {
    try {
      await markAsRead(notificationId);
      await refreshNotifications();
    } catch (error) {
      console.error('Error marking notification as read:', error);
    }
  };

  return (
    <div className={className}>
      <Sheet open={isOpen} onOpenChange={setIsOpen}>
        <SheetTrigger asChild>
          <Button variant="ghost" size="icon" className="relative">
            <Bell className="h-5 w-5" />
            {unreadCount > 0 && (
              <Badge 
                variant="destructive" 
                className="absolute -top-1 -right-1 h-5 w-5 rounded-full p-0 text-xs"
              >
                {unreadCount > 99 ? '99+' : unreadCount}
              </Badge>
            )}
          </Button>
        </SheetTrigger>
        
        <SheetContent className="w-96 sm:w-[540px]">
          <SheetHeader>
            <SheetTitle className="flex items-center justify-between">
              <span>Notifications</span>
              {unreadCount > 0 && (
                <Button 
                  variant="outline" 
                  size="sm" 
                  onClick={handleMarkAllAsRead}
                  className="flex items-center gap-2"
                >
                  <MarkEmailRead className="h-4 w-4" />
                  Mark All Read
                </Button>
              )}
            </SheetTitle>
          </SheetHeader>

          <Tabs value={activeTab} onValueChange={setActiveTab} className="mt-4">
            <TabsList className="grid w-full grid-cols-2">
              <TabsTrigger value="notifications" className="flex items-center gap-2">
                <Bell className="h-4 w-4" />
                Notifications
                {unreadCount > 0 && (
                  <Badge variant="secondary" className="ml-1">
                    {unreadCount}
                  </Badge>
                )}
              </TabsTrigger>
              <TabsTrigger value="preferences" className="flex items-center gap-2">
                <Settings className="h-4 w-4" />
                Preferences
              </TabsTrigger>
            </TabsList>

            <TabsContent value="notifications" className="mt-4">
              <NotificationList
                notifications={notifications}
                isLoading={isLoading}
                error={error}
                onNotificationRead={handleNotificationRead}
                onRefresh={refreshNotifications}
              />
            </TabsContent>

            <TabsContent value="preferences" className="mt-4">
              <NotificationPreferences
                userId={userId}
                onPreferencesUpdated={refreshNotifications}
              />
            </TabsContent>
          </Tabs>
        </SheetContent>
      </Sheet>
    </div>
  );
};
```

#### Notification Hook Implementation
```typescript
// frontend/src/modules/notifications/hooks/useNotifications.tsx
import { useState, useEffect, useCallback } from 'react';
import { notificationService } from '../services/notificationService';
import { 
  NotificationDto, 
  NotificationFilterDto, 
  NotificationStatus,
  NotificationType 
} from '../types/notification.types';

interface UseNotificationsResult {
  notifications: NotificationDto[];
  unreadCount: number;
  isLoading: boolean;
  error: string | null;
  totalCount: number;
  currentPage: number;
  pageSize: number;
  hasNextPage: boolean;
  
  // Actions
  refreshNotifications: () => Promise<void>;
  loadMoreNotifications: () => Promise<void>;
  markAsRead: (notificationId: number) => Promise<boolean>;
  markAllAsRead: () => Promise<boolean>;
  filterNotifications: (filter: NotificationFilterDto) => void;
  setPage: (page: number) => void;
}

export const useNotifications = (
  userId: number,
  initialPageSize: number = 20
): UseNotificationsResult => {
  const [notifications, setNotifications] = useState<NotificationDto[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(initialPageSize);
  const [currentFilter, setCurrentFilter] = useState<NotificationFilterDto>({});

  const fetchNotifications = useCallback(async (
    page: number = 1, 
    append: boolean = false
  ) => {
    try {
      setIsLoading(true);
      setError(null);

      const filter: NotificationFilterDto = {
        ...currentFilter,
        page,
        pageSize
      };

      const result = await notificationService.getUserNotifications(userId, filter);
      
      if (append && page > 1) {
        setNotifications(prev => [...prev, ...result.data]);
      } else {
        setNotifications(result.data);
      }
      
      setTotalCount(result.totalCount);
      setCurrentPage(page);

      // Fetch unread count
      const unread = await notificationService.getUnreadCount(userId);
      setUnreadCount(unread);

    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to fetch notifications';
      setError(errorMessage);
      console.error('Error fetching notifications:', err);
    } finally {
      setIsLoading(false);
    }
  }, [userId, pageSize, currentFilter]);

  const refreshNotifications = useCallback(async () => {
    await fetchNotifications(1, false);
  }, [fetchNotifications]);

  const loadMoreNotifications = useCallback(async () => {
    if (hasNextPage && !isLoading) {
      await fetchNotifications(currentPage + 1, true);
    }
  }, [fetchNotifications, currentPage, isLoading]);

  const markAsRead = useCallback(async (notificationId: number): Promise<boolean> => {
    try {
      const success = await notificationService.markAsRead(notificationId, userId);
      
      if (success) {
        // Update local state
        setNotifications(prev => 
          prev.map(notification => 
            notification.id === notificationId 
              ? { ...notification, readAt: new Date().toISOString(), status: NotificationStatus.Read }
              : notification
          )
        );
        
        // Update unread count
        setUnreadCount(prev => Math.max(0, prev - 1));
      }
      
      return success;
    } catch (err) {
      console.error('Error marking notification as read:', err);
      return false;
    }
  }, [userId]);

  const markAllAsRead = useCallback(async (): Promise<boolean> => {
    try {
      const success = await notificationService.markAllAsRead(userId);
      
      if (success) {
        // Update local state
        const now = new Date().toISOString();
        setNotifications(prev => 
          prev.map(notification => ({
            ...notification,
            readAt: notification.readAt || now,
            status: NotificationStatus.Read
          }))
        );
        
        setUnreadCount(0);
      }
      
      return success;
    } catch (err) {
      console.error('Error marking all notifications as read:', err);
      return false;
    }
  }, [userId]);

  const filterNotifications = useCallback((filter: NotificationFilterDto) => {
    setCurrentFilter(filter);
    setCurrentPage(1);
    fetchNotifications(1, false);
  }, [fetchNotifications]);

  const setPage = useCallback((page: number) => {
    fetchNotifications(page, false);
  }, [fetchNotifications]);

  // Calculate if there's a next page
  const hasNextPage = currentPage * pageSize < totalCount;

  // Initial load
  useEffect(() => {
    if (userId) {
      fetchNotifications();
    }
  }, [userId, fetchNotifications]);

  return {
    notifications,
    unreadCount,
    isLoading,
    error,
    totalCount,
    currentPage,
    pageSize,
    hasNextPage,
    
    refreshNotifications,
    loadMoreNotifications,
    markAsRead,
    markAllAsRead,
    filterNotifications,
    setPage
  };
};
```

## 🔄 Integration Validation Loops

### Backend Validation Loop
```bash
# 1. Entity Validation
dotnet ef migrations add AddNotificationEntities
dotnet ef database update

# 2. Service Layer Testing
dotnet test --filter Category=NotificationService

# 3. Repository Testing  
dotnet test --filter Category=NotificationRepository

# 4. Controller Testing
dotnet test --filter Category=NotificationController

# 5. Integration Testing
dotnet test --filter Category=NotificationIntegration

# 6. Email Provider Testing
dotnet test --filter Category=EmailProvider

# 7. SMS Provider Testing
dotnet test --filter Category=SmsProvider
```

### Frontend Validation Loop
```bash
# 1. Component Testing
npm run test -- --testPathPattern=notifications

# 2. Hook Testing
npm run test -- --testPathPattern=useNotifications

# 3. Service Testing
npm run test -- --testPathPattern=notificationService

# 4. Type Checking
npx tsc --noEmit --project tsconfig.json

# 5. Integration Testing
npm run test:integration -- --testPathPattern=notifications

# 6. E2E Testing
npm run test:e2e -- --testPathPattern=notifications
```

### Event System Validation Loop
```bash
# 1. Event Handler Testing
dotnet test --filter Category=NotificationEventHandlers

# 2. Event Bus Testing
dotnet test --filter Category=EventBus

# 3. Scheduler Testing
dotnet test --filter Category=NotificationScheduler

# 4. Queue Testing
dotnet test --filter Category=NotificationQueue
```

## 🎯 Success Criteria

### Functional Requirements ✅
- [ ] Email notifications sent successfully with templates
- [ ] SMS notifications delivered through configured providers
- [ ] In-app notifications displayed in real-time
- [ ] User preferences respected for all notification types
- [ ] Event-driven notifications triggered automatically
- [ ] Scheduled notifications sent at specified times
- [ ] Bulk notifications processed efficiently
- [ ] Notification history tracked and auditable
- [ ] Template management system operational
- [ ] Multi-language support functional
- [ ] Delivery status tracking working
- [ ] Failed notification retry mechanism active

### Performance Requirements ✅
- [ ] Notification creation < 100ms response time
- [ ] Email delivery < 5 seconds processing time
- [ ] SMS delivery < 10 seconds processing time
- [ ] Real-time notifications < 1 second latency
- [ ] Bulk notifications process 1000+ per minute
- [ ] Database queries optimized with proper indexing
- [ ] Memory usage stays within acceptable limits
- [ ] Queue processing handles peak loads

### Security Requirements ✅
- [ ] User authorization for notification access
- [ ] PII protection in notification content
- [ ] Secure email/SMS provider connections
- [ ] Audit logging for all notification activities
- [ ] Rate limiting for notification requests
- [ ] Input validation and sanitization
- [ ] Encryption for sensitive notification data

### Integration Requirements ✅
- [ ] Authentication system integration complete
- [ ] Role management system integration functional
- [ ] Audit logging system integration active
- [ ] Exam management system events connected
- [ ] User management system events connected
- [ ] Grading system events connected
- [ ] Frontend notification center operational
- [ ] Real-time updates via WebSocket/SignalR

## ⚠ Anti-Patterns to Avoid

### Backend Anti-Patterns ❌
```csharp
// ❌ DON'T: Mixed responsibilities in notification service
public class NotificationService
{
    public async Task SendNotificationAndUpdateUserProfile(int userId) { } // Mixed concerns
    public async Task ProcessPaymentAndSendReceipt(int paymentId) { } // Unrelated functionality
}

// ❌ DON'T: Direct database access in controllers
public class NotificationController
{
    public async Task<IActionResult> GetNotifications()
    {
        var notifications = await _context.Notifications.ToListAsync(); // Direct DB access
    }
}

// ❌ DON'T: Blocking async calls
public bool SendEmailSync(EmailRequest request)
{
    return SendEmailAsync(request).Result; // Blocking async
}

// ❌ DON'T: Unhandled exceptions in background tasks
public async Task ProcessNotificationQueue()
{
    var notification = await GetNextNotification(); // No try-catch
    await SendNotification(notification); // Unhandled exceptions
}
```

### Frontend Anti-Patterns ❌
```typescript
// ❌ DON'T: Direct API calls in components
const NotificationCenter = () => {
  const [notifications, setNotifications] = useState([]);
  
  useEffect(() => {
    // Direct API call without service layer
    fetch('/api/notifications').then(/* ... */);
  }, []);
};

// ❌ DON'T: Missing error boundaries
const NotificationApp = () => {
  return (
    <div>
      {/* No error boundary around notification components */}
      <NotificationCenter />
    </div>
  );
};

// ❌ DON'T: Infinite re-renders
const useNotifications = (userId) => {
  const [notifications, setNotifications] = useState([]);
  
  useEffect(() => {
    fetchNotifications(); // Missing dependency array
  });
};

// ❌ DON'T: Memory leaks in subscriptions
const useRealTimeNotifications = () => {
  useEffect(() => {
    const subscription = notificationHub.subscribe();
    // Missing cleanup function
  }, []);
};
```

### Architecture Anti-Patterns ❌
```csharp
// ❌ DON'T: Circular dependencies
public class NotificationService
{
    private readonly UserService _userService; // Creates circular dependency
}

public class UserService  
{
    private readonly NotificationService _notificationService; // Circular reference
}

// ❌ DON'T: Tight coupling to specific providers
public class NotificationService
{
    private readonly SendGridService _sendGrid; // Tightly coupled to SendGrid
    
    public async Task SendEmail()
    {
        await _sendGrid.SendAsync(); // Can't switch providers easily
    }
}

// ❌ DON'T: Missing abstraction layers
public class NotificationController
{
    public async Task<IActionResult> SendNotification()
    {
        // Direct business logic in controller
        var user = await _context.Users.FindAsync(userId);
        var template = await _context.Templates.FindAsync(templateId);
        // ... complex notification logic
    }
}
```

## 📚 Implementation Guide

### Phase 1: Core Backend Implementation (Week 1)
1. **Entity Setup**
   - Create all notification entities with proper relationships
   - Set up database migrations
   - Configure Dapper relationships

2. **Repository Layer**
   - Implement all repository interfaces and implementations
   - Add proper indexing for performance
   - Include repository unit tests

3. **Service Layer Foundation**
   - Create core notification service interfaces
   - Implement basic CRUD operations
   - Add comprehensive validation logic

### Phase 2: Communication Providers (Week 2)
1. **Email Integration**
   - Set up SendGrid/SMTP providers
   - Implement email templates
   - Add email validation and delivery tracking

2. **SMS Integration**
   - Configure SMS providers (Twilio, etc.)
   - Implement SMS templates
   - Add SMS delivery tracking

3. **Push Notifications**
   - Set up Firebase integration
   - Implement push notification logic
   - Add device token management

### Phase 3: Event-Driven System (Week 3)
1. **Event System**
   - Implement event bus service
   - Create notification event handlers
   - Set up event publishing from other services

2. **Scheduling System**
   - Implement notification scheduler
   - Create background job processing
   - Add retry mechanisms for failed notifications

3. **Template Management**
   - Create template service
   - Implement variable substitution
   - Add multi-language template support

### Phase 4: Frontend Implementation (Week 4)
1. **React Components**
   - Build notification center component
   - Create notification list and item components
   - Implement notification preferences UI

2. **State Management**
   - Create notification hooks
   - Implement real-time updates
   - Add optimistic UI updates

3. **Integration**
   - Connect to backend APIs
   - Implement WebSocket connections
   - Add proper error handling

### Phase 5: Advanced Features (Week 5)
1. **Analytics and Reporting**
   - Add notification metrics tracking
   - Create delivery rate reports
   - Implement user engagement analytics

2. **Advanced Filtering**
   - Add complex notification filters
   - Implement search functionality
   - Create notification categorization

3. **Performance Optimization**
   - Optimize database queries
   - Implement caching strategies
   - Add rate limiting

### Phase 6: Testing and Quality Assurance (Week 6)
1. **Comprehensive Testing**
   - Unit tests for all services
   - Integration tests for API endpoints
   - E2E tests for notification flows

2. **Performance Testing**
   - Load testing for bulk notifications
   - Stress testing for high concurrency
   - Memory and CPU usage optimization

3. **Security Testing**
   - Penetration testing for API endpoints
   - Security audit for PII handling
   - Access control validation

## 🔧 Configuration Examples

### Backend Configuration
```json
// appsettings.json
{
  "NotificationSettings": {
    "BatchSize": 100,
    "RetryAttempts": 3,
    "RetryDelayMinutes": 5,
    "MaxQueueSize": 10000,
    "EnableRealTimeNotifications": true,
    "DefaultLanguage": "en"
  },
  "EmailSettings": {
    "Provider": "SendGrid",
    "FromAddress": "noreply@ikhtibar.edu.sa",
    "FromName": "Ikhtibar System",
    "ReplyToAddress": "support@ikhtibar.edu.sa",
    "SendGridApiKey": "{{SendGrid_API_Key}}",
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUsername": "{{SMTP_Username}}",
    "SmtpPassword": "{{SMTP_Password}}"
  },
  "SmsSettings": {
    "Provider": "Twilio",
    "TwilioAccountSid": "{{Twilio_Account_SID}}",
    "TwilioAuthToken": "{{Twilio_Auth_Token}}",
    "TwilioPhoneNumber": "+1234567890",
    "NafathApiKey": "{{Nafath_API_Key}}",
    "NafathEndpoint": "https://api.nafath.sa"
  },
  "PushNotificationSettings": {
    "FirebaseServerKey": "{{Firebase_Server_Key}}",
    "FirebaseProjectId": "ikhtibar-notifications"
  }
}
```

### Frontend Configuration
```typescript
// src/config/notifications.config.ts
export const notificationConfig = {
  realTimeConnection: {
    hubUrl: '/hubs/notifications',
    reconnectAttempts: 5,
    reconnectInterval: 5000
  },
  pagination: {
    defaultPageSize: 20,
    maxPageSize: 100
  },
  polling: {
    enabled: true,
    interval: 30000, // 30 seconds
    unreadCountInterval: 10000 // 10 seconds
  },
  toast: {
    duration: 5000,
    position: 'top-right',
    maxVisible: 3
  },
  sounds: {
    enabled: true,
    newNotification: '/sounds/notification.mp3'
  }
};
```

This comprehensive notification system PRP provides a complete implementation guide following the established patterns and architectural principles of the Ikhtibar system. The implementation includes proper separation of concerns, comprehensive error handling, security considerations, and integration with all existing system components.

The system supports multiple communication channels (email, SMS, in-app, push), event-driven architecture, user preferences, template management, and comprehensive audit logging - making it a production-ready notification system for the educational assessment platform.
