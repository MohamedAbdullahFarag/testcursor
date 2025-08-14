namespace Ikhtibar.Shared.Enums;

/// <summary>
/// Types of notifications that can be sent to users
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Welcome notification for new users
    /// </summary>
    Welcome = 1,
    
    /// <summary>
    /// Password reset notification
    /// </summary>
    PasswordReset = 2,
    
    /// <summary>
    /// Exam reminder notification
    /// </summary>
    ExamReminder = 3,
    
    /// <summary>
    /// Exam start notification
    /// </summary>
    ExamStart = 4,
    
    /// <summary>
    /// Exam end notification
    /// </summary>
    ExamEnd = 5,
    
    /// <summary>
    /// Exam grading completion notification
    /// </summary>
    GradingComplete = 6,
    
    /// <summary>
    /// Assignment deadline reminder
    /// </summary>
    AssignmentReminder = 7,
    
    /// <summary>
    /// General announcement
    /// </summary>
    Announcement = 8,
    
    /// <summary>
    /// Account security notification
    /// </summary>
    SecurityAlert = 9,
    
    /// <summary>
    /// System maintenance notification
    /// </summary>
    SystemMaintenance = 10,
    
    /// <summary>
    /// Account activation notification
    /// </summary>
    AccountActivation = 11,
    
    /// <summary>
    /// General system announcement
    /// </summary>
    SystemAnnouncement = 12,
    
    /// <summary>
    /// Grade published notification
    /// </summary>
    GradePublished = 13,
    
    /// <summary>
    /// Account locked notification
    /// </summary>
    AccountLocked = 14,
    
    /// <summary>
    /// Role assignment notification
    /// </summary>
    RoleAssignment = 15,
    
    /// <summary>
    /// Exam created notification
    /// </summary>
    ExamCreated = 16,
    
    /// <summary>
    /// Exam deadline approaching notification
    /// </summary>
    ExamDeadline = 17,
    
    /// <summary>
    /// Exam completed notification
    /// </summary>
    ExamCompleted = 18,
    
    /// <summary>
    /// Exam results published notification
    /// </summary>
    ExamResultsPublished = 19,
    
    /// <summary>
    /// Custom notification type
    /// </summary>
    Custom = 99
}
