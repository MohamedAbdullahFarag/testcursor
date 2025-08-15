-- Notification System Database Migration
-- This script creates all necessary tables for the comprehensive notification system

-- Create Notifications table
CREATE TABLE [dbo].[Notifications] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Message] NVARCHAR(2000) NOT NULL,
    [Type] INT NOT NULL,
    [Priority] INT NOT NULL,
    [Status] INT NOT NULL,
    [UserId] INT NOT NULL,
    [EntityType] NVARCHAR(100) NULL,
    [EntityId] INT NULL,
    [ScheduledAt] DATETIME2 NOT NULL,
    [SentAt] DATETIME2 NULL,
    [ReadAt] DATETIME2 NULL,
    [MetadataJson] NVARCHAR(MAX) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DeletedAt] DATETIME2 NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id])
);

-- Create NotificationTemplates table
CREATE TABLE [dbo].[NotificationTemplates] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Type] INT NOT NULL,
    [SubjectTemplate] NVARCHAR(200) NOT NULL,
    [MessageTemplate] NVARCHAR(5000) NOT NULL,
    [Language] NVARCHAR(10) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DeletedAt] DATETIME2 NULL,
    CONSTRAINT [PK_NotificationTemplates] PRIMARY KEY ([Id])
);

-- Create NotificationPreferences table
CREATE TABLE [dbo].[NotificationPreferences] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [NotificationType] INT NOT NULL,
    [EmailEnabled] BIT NOT NULL DEFAULT 1,
    [SmsEnabled] BIT NOT NULL DEFAULT 0,
    [InAppEnabled] BIT NOT NULL DEFAULT 1,
    [PushEnabled] BIT NOT NULL DEFAULT 1,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DeletedAt] DATETIME2 NULL,
    CONSTRAINT [PK_NotificationPreferences] PRIMARY KEY ([Id])
);

-- Create NotificationHistory table
CREATE TABLE [dbo].[NotificationHistory] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [NotificationId] INT NOT NULL,
    [Channel] INT NOT NULL,
    [Status] INT NOT NULL,
    [AttemptedAt] DATETIME2 NOT NULL,
    [DeliveredAt] DATETIME2 NULL,
    [ErrorMessage] NVARCHAR(1000) NULL,
    [ExternalId] NVARCHAR(100) NULL,
    [ResponseData] NVARCHAR(MAX) NULL,
    [Cost] DECIMAL(10,4) NULL,
    [CostCurrency] NVARCHAR(3) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DeletedAt] DATETIME2 NULL,
    CONSTRAINT [PK_NotificationHistory] PRIMARY KEY ([Id])
);

-- Create indexes for better performance
CREATE INDEX [IX_Notifications_UserId] ON [dbo].[Notifications] ([UserId]);
CREATE INDEX [IX_Notifications_Type] ON [dbo].[Notifications] ([Type]);
CREATE INDEX [IX_Notifications_Status] ON [dbo].[Notifications] ([Status]);
CREATE INDEX [IX_Notifications_ScheduledAt] ON [dbo].[Notifications] ([ScheduledAt]);
CREATE INDEX [IX_Notifications_EntityType_EntityId] ON [dbo].[Notifications] ([EntityType], [EntityId]);

CREATE INDEX [IX_NotificationTemplates_Type_Language] ON [dbo].[NotificationTemplates] ([Type], [Language]);
CREATE INDEX [IX_NotificationTemplates_IsActive] ON [dbo].[NotificationTemplates] ([IsActive]);

CREATE INDEX [IX_NotificationPreferences_UserId_Type] ON [dbo].[NotificationPreferences] ([UserId], [NotificationType]);

CREATE INDEX [IX_NotificationHistory_NotificationId] ON [dbo].[NotificationHistory] ([NotificationId]);
CREATE INDEX [IX_NotificationHistory_Channel] ON [dbo].[NotificationHistory] ([Channel]);
CREATE INDEX [IX_NotificationHistory_Status] ON [dbo].[NotificationHistory] ([Status]);
CREATE INDEX [IX_NotificationHistory_AttemptedAt] ON [dbo].[NotificationHistory] ([AttemptedAt]);

-- Create foreign key constraints
ALTER TABLE [dbo].[Notifications] ADD CONSTRAINT [FK_Notifications_Users] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]);

ALTER TABLE [dbo].[NotificationPreferences] ADD CONSTRAINT [FK_NotificationPreferences_Users] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]);

ALTER TABLE [dbo].[NotificationHistory] ADD CONSTRAINT [FK_NotificationHistory_Notifications] 
    FOREIGN KEY ([NotificationId]) REFERENCES [dbo].[Notifications] ([Id]);

-- Insert default notification templates
INSERT INTO [dbo].[NotificationTemplates] ([Name], [Type], [SubjectTemplate], [MessageTemplate], [Language], [IsActive])
VALUES 
    ('Exam Reminder', 1, 'Exam Reminder: {ExamTitle}', 'Your exam "{ExamTitle}" starts in {ReminderMinutes} minutes. Please be ready!', 'en', 1),
    ('Exam Reminder', 1, 'تذكير بالامتحان: {ExamTitle}', 'امتحانك "{ExamTitle}" يبدأ خلال {ReminderMinutes} دقيقة. يرجى الاستعداد!', 'ar', 1),
    ('Exam Start', 2, 'Exam Started: {ExamTitle}', 'Your exam "{ExamTitle}" has started. Good luck!', 'en', 1),
    ('Exam Start', 2, 'بدأ الامتحان: {ExamTitle}', 'امتحانك "{ExamTitle}" قد بدأ. بالتوفيق!', 'ar', 1),
    ('Exam End', 3, 'Exam Ended: {ExamTitle}', 'Your exam "{ExamTitle}" has ended. Thank you for participating!', 'en', 1),
    ('Exam End', 3, 'انتهى الامتحان: {ExamTitle}', 'امتحانك "{ExamTitle}" قد انتهى. شكراً لمشاركتك!', 'ar', 1),
    ('Grading Complete', 4, 'Grading Complete: {ExamTitle}', 'Your exam "{ExamTitle}" has been graded. Check your results!', 'en', 1),
    ('Grading Complete', 4, 'اكتمل التصحيح: {ExamTitle}', 'امتحانك "{ExamTitle}" قد تم تصحيحه. تحقق من نتائجك!', 'ar', 1),
    ('User Welcome', 5, 'Welcome to Ikhtibar!', 'Welcome to the Ikhtibar educational platform. We''re excited to have you on board!', 'en', 1),
    ('User Welcome', 5, 'مرحباً بك في اختبار!', 'مرحباً بك في منصة اختبار التعليمية. نحن متحمسون لوجودك معنا!', 'ar', 1),
    ('Password Reset', 6, 'Password Reset Request', 'Your password reset token is: {ResetToken}. Use this to reset your password.', 'en', 1),
    ('Password Reset', 6, 'طلب إعادة تعيين كلمة المرور', 'رمز إعادة تعيين كلمة المرور الخاص بك هو: {ResetToken}. استخدم هذا لإعادة تعيين كلمة المرور.', 'ar', 1),
    ('Role Assignment', 7, 'Role Assignment: {RoleName}', 'You have been assigned the role: {RoleName}. Your permissions have been updated accordingly.', 'en', 1),
    ('Role Assignment', 7, 'تعيين الدور: {RoleName}', 'لقد تم تعيينك في الدور: {RoleName}. تم تحديث صلاحياتك وفقاً لذلك.', 'ar', 1),
    ('Deadline Reminder', 8, 'Deadline Reminder: {EntityType}', 'Deadline approaching for {EntityType}. Due: {Deadline}.', 'en', 1),
    ('Deadline Reminder', 8, 'تذكير بالموعد النهائي: {EntityType}', 'الموعد النهائي يقترب لـ {EntityType}. المستحق: {Deadline}.', 'ar', 1);

-- Create stored procedure for processing scheduled notifications
CREATE PROCEDURE [dbo].[ProcessScheduledNotifications]
    @BatchSize INT = 100
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Now DATETIME2 = GETUTCDATE();
    
    -- Get pending notifications that are due
    SELECT TOP (@BatchSize)
        n.Id,
        n.Title,
        n.Message,
        n.Type,
        n.Priority,
        n.UserId,
        n.EntityType,
        n.EntityId,
        n.MetadataJson
    FROM [dbo].[Notifications] n
    WHERE n.Status = 1 -- Pending
        AND n.IsDeleted = 0
        AND n.ScheduledAt <= @Now
    ORDER BY n.Priority DESC, n.ScheduledAt ASC;
END;

-- Create stored procedure for getting notification statistics
CREATE PROCEDURE [dbo].[GetNotificationStats]
    @UserId INT = NULL,
    @FromDate DATETIME2 = NULL,
    @ToDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @FromDate IS NULL
        SET @FromDate = DATEADD(DAY, -30, GETUTCDATE());
    
    IF @ToDate IS NULL
        SET @ToDate = GETUTCDATE();
    
    -- Get notification counts by type
    SELECT 
        n.Type,
        COUNT(*) as TotalCount,
        SUM(CASE WHEN n.ReadAt IS NOT NULL THEN 1 ELSE 0 END) as ReadCount,
        SUM(CASE WHEN n.ReadAt IS NULL THEN 1 ELSE 0 END) as UnreadCount,
        SUM(CASE WHEN n.Status = 2 THEN 1 ELSE 0 END) as SentCount,
        SUM(CASE WHEN n.Status = 3 THEN 1 ELSE 0 END) as FailedCount
    FROM [dbo].[Notifications] n
    WHERE n.IsDeleted = 0
        AND (@UserId IS NULL OR n.UserId = @UserId)
        AND n.CreatedAt BETWEEN @FromDate AND @ToDate
    GROUP BY n.Type;
    
    -- Get delivery statistics
    SELECT 
        nh.Channel,
        COUNT(*) as TotalAttempts,
        SUM(CASE WHEN nh.Status IN (2, 3, 6, 7) THEN 1 ELSE 0 END) as SuccessfulDeliveries,
        SUM(CASE WHEN nh.Status = 4 THEN 1 ELSE 0 END) as FailedDeliveries,
        AVG(CASE WHEN nh.Status IN (2, 3, 6, 7) THEN CAST(DATEDIFF(MILLISECOND, nh.AttemptedAt, nh.DeliveredAt) AS FLOAT) END) as AvgDeliveryTimeMs
    FROM [dbo].[NotificationHistory] nh
    INNER JOIN [dbo].[Notifications] n ON nh.NotificationId = n.Id
    WHERE nh.IsDeleted = 0
        AND (@UserId IS NULL OR n.UserId = @UserId)
        AND nh.AttemptedAt BETWEEN @FromDate AND @ToDate
    GROUP BY nh.Channel;
END;

-- Create view for notification summary
CREATE VIEW [dbo].[v_NotificationSummary] AS
SELECT 
    n.Id,
    n.Title,
    n.Message,
    n.Type,
    n.Priority,
    n.Status,
    n.UserId,
    u.Email as UserEmail,
    u.FirstName + ' ' + u.LastName as UserFullName,
    n.EntityType,
    n.EntityId,
    n.ScheduledAt,
    n.SentAt,
    n.ReadAt,
    n.CreatedAt,
    CASE 
        WHEN n.Status = 1 THEN 'Pending'
        WHEN n.Status = 2 THEN 'Sent'
        WHEN n.Status = 3 THEN 'Failed'
        WHEN n.Status = 4 THEN 'Cancelled'
        ELSE 'Unknown'
    END as StatusText,
    CASE 
        WHEN n.Priority = 1 THEN 'Low'
        WHEN n.Priority = 2 THEN 'Normal'
        WHEN n.Priority = 3 THEN 'High'
        WHEN n.Priority = 4 THEN 'Critical'
        ELSE 'Unknown'
    END as PriorityText
FROM [dbo].[Notifications] n
INNER JOIN [dbo].[Users] u ON n.UserId = u.UserId
WHERE n.IsDeleted = 0;

-- Create view for notification delivery summary
CREATE VIEW [dbo].[v_NotificationDeliverySummary] AS
SELECT 
    n.Id as NotificationId,
    n.Title,
    n.Type,
    n.UserId,
    u.Email as UserEmail,
    nh.Channel,
    nh.Status as DeliveryStatus,
    nh.AttemptedAt,
    nh.DeliveredAt,
    nh.ErrorMessage,
    CASE 
        WHEN nh.Status = 1 THEN 'Pending'
        WHEN nh.Status = 2 THEN 'Sent'
        WHEN nh.Status = 3 THEN 'Delivered'
        WHEN nh.Status = 4 THEN 'Failed'
        WHEN nh.Status = 5 THEN 'Bounced'
        WHEN nh.Status = 6 THEN 'Opened'
        WHEN nh.Status = 7 THEN 'Clicked'
        ELSE 'Unknown'
    END as DeliveryStatusText,
    CASE 
        WHEN nh.Channel = 1 THEN 'Email'
        WHEN nh.Channel = 2 THEN 'SMS'
        WHEN nh.Channel = 3 THEN 'In-App'
        WHEN nh.Channel = 4 THEN 'Push'
        ELSE 'Unknown'
    END as ChannelText
FROM [dbo].[Notifications] n
INNER JOIN [dbo].[Users] u ON n.UserId = u.UserId
LEFT JOIN [dbo].[NotificationHistory] nh ON n.Id = nh.NotificationId
WHERE n.IsDeleted = 0 AND (nh.IsDeleted = 0 OR nh.IsDeleted IS NULL);

-- Grant permissions (adjust as needed for your security model)
-- GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Notifications] TO [YourAppRole];
-- GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[NotificationTemplates] TO [YourAppRole];
-- GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[NotificationPreferences] TO [YourAppRole];
-- GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[NotificationHistory] TO [YourAppRole];
-- GRANT EXECUTE ON [dbo].[ProcessScheduledNotifications] TO [YourAppRole];
-- GRANT EXECUTE ON [dbo].[GetNotificationStats] TO [YourAppRole];
-- GRANT SELECT ON [dbo].[v_NotificationSummary] TO [YourAppRole];
-- GRANT SELECT ON [dbo].[v_NotificationDeliverySummary] TO [YourAppRole];

PRINT 'Notification system database migration completed successfully.';
