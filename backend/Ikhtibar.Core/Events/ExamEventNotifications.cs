using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.Core.Events;

/// <summary>
/// Domain events related to exam lifecycle that trigger automated notifications.
/// These events are published when significant exam activities occur and need user notification.
/// Integrates with the notification system to send timely updates to users.
/// </summary>
public abstract class ExamEventBase : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime CreatedAt { get; }
    public int? UserId { get; }
    public string? CorrelationId { get; }
    public Dictionary<string, object> Metadata { get; }
    
    /// <summary>
    /// The exam identifier associated with this event.
    /// </summary>
    public int ExamId { get; }
    
    /// <summary>
    /// The title of the exam for display in notifications.
    /// </summary>
    public string ExamTitle { get; }

    protected ExamEventBase(int examId, string examTitle, int? userId = null, string? correlationId = null)
    {
        EventId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        ExamId = examId;
        ExamTitle = examTitle ?? throw new ArgumentNullException(nameof(examTitle));
        UserId = userId;
        CorrelationId = correlationId;
        Metadata = new Dictionary<string, object>();
    }
}

/// <summary>
/// Published when a new exam is created and students need to be notified.
/// Triggers welcome notifications to enrolled students with exam details.
/// </summary>
public class ExamCreatedEvent : ExamEventBase
{
    /// <summary>
    /// The instructor who created the exam.
    /// </summary>
    public int InstructorId { get; }
    
    /// <summary>
    /// The instructor's name for personalized notifications.
    /// </summary>
    public string InstructorName { get; }
    
    /// <summary>
    /// List of student IDs who should be notified about the new exam.
    /// </summary>
    public List<int> StudentIds { get; }
    
    /// <summary>
    /// The scheduled start date and time for the exam.
    /// </summary>
    public DateTime ScheduledStartDate { get; }
    
    /// <summary>
    /// The duration of the exam in minutes.
    /// </summary>
    public int DurationMinutes { get; }

    public ExamCreatedEvent(
        int examId, 
        string examTitle, 
        int instructorId, 
        string instructorName,
        List<int> studentIds,
        DateTime scheduledStartDate,
        int durationMinutes,
        string? correlationId = null) 
        : base(examId, examTitle, instructorId, correlationId)
    {
        InstructorId = instructorId;
        InstructorName = instructorName ?? throw new ArgumentNullException(nameof(instructorName));
        StudentIds = studentIds ?? throw new ArgumentNullException(nameof(studentIds));
        ScheduledStartDate = scheduledStartDate;
        DurationMinutes = durationMinutes;
        
        // Add metadata for template rendering
        Metadata["InstructorName"] = instructorName;
        Metadata["StudentCount"] = studentIds.Count;
        Metadata["ScheduledStartDate"] = scheduledStartDate;
        Metadata["DurationMinutes"] = durationMinutes;
    }
}

/// <summary>
/// Published when an exam is about to start (configurable time before start).
/// Triggers reminder notifications to students with exam details and preparation tips.
/// </summary>
public class ExamStartingEvent : ExamEventBase
{
    /// <summary>
    /// The exact start time of the exam.
    /// </summary>
    public DateTime StartTime { get; }
    
    /// <summary>
    /// List of student IDs who should receive the starting reminder.
    /// </summary>
    public List<int> StudentIds { get; }
    
    /// <summary>
    /// The access URL for students to join the exam.
    /// </summary>
    public string ExamUrl { get; }
    
    /// <summary>
    /// Time remaining until exam start (for display in notification).
    /// </summary>
    public TimeSpan TimeUntilStart { get; }

    public ExamStartingEvent(
        int examId,
        string examTitle,
        DateTime startTime,
        List<int> studentIds,
        string examUrl,
        string? correlationId = null)
        : base(examId, examTitle, null, correlationId)
    {
        StartTime = startTime;
        StudentIds = studentIds ?? throw new ArgumentNullException(nameof(studentIds));
        ExamUrl = examUrl ?? throw new ArgumentNullException(nameof(examUrl));
        TimeUntilStart = startTime - DateTime.UtcNow;
        
        // Add metadata for template rendering
        Metadata["StartTime"] = startTime;
        Metadata["ExamUrl"] = examUrl;
        Metadata["TimeUntilStart"] = TimeUntilStart;
        Metadata["StudentCount"] = studentIds.Count;
    }
}

/// <summary>
/// Published when an exam submission deadline is approaching.
/// Triggers deadline reminder notifications to students who haven't submitted yet.
/// </summary>
public class ExamDeadlineApproachingEvent : ExamEventBase
{
    /// <summary>
    /// The submission deadline for the exam.
    /// </summary>
    public DateTime Deadline { get; }
    
    /// <summary>
    /// List of student IDs who haven't submitted and should be reminded.
    /// </summary>
    public List<int> PendingStudentIds { get; }
    
    /// <summary>
    /// Time remaining until deadline (for urgency in notification).
    /// </summary>
    public TimeSpan TimeUntilDeadline { get; }
    
    /// <summary>
    /// The submission URL for students to access the exam.
    /// </summary>
    public string SubmissionUrl { get; }

    public ExamDeadlineApproachingEvent(
        int examId,
        string examTitle,
        DateTime deadline,
        List<int> pendingStudentIds,
        string submissionUrl,
        string? correlationId = null)
        : base(examId, examTitle, null, correlationId)
    {
        Deadline = deadline;
        PendingStudentIds = pendingStudentIds ?? throw new ArgumentNullException(nameof(pendingStudentIds));
        TimeUntilDeadline = deadline - DateTime.UtcNow;
        SubmissionUrl = submissionUrl ?? throw new ArgumentNullException(nameof(submissionUrl));
        
        // Add metadata for template rendering
        Metadata["Deadline"] = deadline;
        Metadata["TimeUntilDeadline"] = TimeUntilDeadline;
        Metadata["SubmissionUrl"] = submissionUrl;
        Metadata["PendingStudentCount"] = pendingStudentIds.Count;
        Metadata["IsUrgent"] = TimeUntilDeadline.TotalHours < 24; // Flag for urgent styling
    }
}

/// <summary>
/// Published when an exam session has ended (time limit reached or manual end).
/// Triggers completion notifications to students and instructors.
/// </summary>
public class ExamCompletedEvent : ExamEventBase
{
    /// <summary>
    /// The actual end time of the exam session.
    /// </summary>
    public DateTime EndTime { get; }
    
    /// <summary>
    /// List of student IDs who participated in the exam.
    /// </summary>
    public List<int> ParticipantIds { get; }
    
    /// <summary>
    /// Number of students who completed the exam.
    /// </summary>
    public int CompletedCount { get; }
    
    /// <summary>
    /// Number of students who started but didn't complete.
    /// </summary>
    public int IncompleteCount { get; }
    
    /// <summary>
    /// The instructor who should be notified of completion stats.
    /// </summary>
    public int InstructorId { get; }

    public ExamCompletedEvent(
        int examId,
        string examTitle,
        DateTime endTime,
        List<int> participantIds,
        int completedCount,
        int incompleteCount,
        int instructorId,
        string? correlationId = null)
        : base(examId, examTitle, instructorId, correlationId)
    {
        EndTime = endTime;
        ParticipantIds = participantIds ?? throw new ArgumentNullException(nameof(participantIds));
        CompletedCount = completedCount;
        IncompleteCount = incompleteCount;
        InstructorId = instructorId;
        
        // Add metadata for template rendering
        Metadata["EndTime"] = endTime;
        Metadata["ParticipantCount"] = participantIds.Count;
        Metadata["CompletedCount"] = completedCount;
        Metadata["IncompleteCount"] = incompleteCount;
        Metadata["CompletionRate"] = participantIds.Count > 0 ? (double)completedCount / participantIds.Count * 100 : 0;
    }
}

/// <summary>
/// Published when exam results are published and students need to be notified.
/// Triggers result notification emails with grades and feedback access.
/// </summary>
public class ExamResultsPublishedEvent : ExamEventBase
{
    /// <summary>
    /// The publication timestamp for results.
    /// </summary>
    public DateTime PublishedAt { get; }
    
    /// <summary>
    /// List of student IDs who should be notified of their results.
    /// </summary>
    public List<int> StudentIds { get; }
    
    /// <summary>
    /// The URL where students can view their detailed results.
    /// </summary>
    public string ResultsUrl { get; }
    
    /// <summary>
    /// The instructor who published the results.
    /// </summary>
    public int InstructorId { get; }
    
    /// <summary>
    /// The instructor's name for personalized notifications.
    /// </summary>
    public string InstructorName { get; }
    
    /// <summary>
    /// Whether individual feedback is available for students.
    /// </summary>
    public bool HasFeedback { get; }

    public ExamResultsPublishedEvent(
        int examId,
        string examTitle,
        DateTime publishedAt,
        List<int> studentIds,
        string resultsUrl,
        int instructorId,
        string instructorName,
        bool hasFeedback = false,
        string? correlationId = null)
        : base(examId, examTitle, instructorId, correlationId)
    {
        PublishedAt = publishedAt;
        StudentIds = studentIds ?? throw new ArgumentNullException(nameof(studentIds));
        ResultsUrl = resultsUrl ?? throw new ArgumentNullException(nameof(resultsUrl));
        InstructorId = instructorId;
        InstructorName = instructorName ?? throw new ArgumentNullException(nameof(instructorName));
        HasFeedback = hasFeedback;
        
        // Add metadata for template rendering
        Metadata["PublishedAt"] = publishedAt;
        Metadata["ResultsUrl"] = resultsUrl;
        Metadata["InstructorName"] = instructorName;
        Metadata["StudentCount"] = studentIds.Count;
        Metadata["HasFeedback"] = hasFeedback;
    }
}
