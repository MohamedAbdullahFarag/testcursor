using System.ComponentModel.DataAnnotations;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Request DTO for sending bulk emails
/// </summary>
public class BulkEmailRequest
{
    /// <summary>
    /// List of recipients with personalized data
    /// </summary>
    [Required]
    public List<BulkEmailRecipient> Recipients { get; set; } = new();

    /// <summary>
    /// Email subject
    /// </summary>
    [Required]
    [StringLength(300)]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Email body content
    /// </summary>
    [Required]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Content type (text/html)
    /// </summary>
    public string ContentType { get; set; } = "text/html";

    /// <summary>
    /// Template ID to use (optional)
    /// </summary>
    public int? TemplateId { get; set; }

    /// <summary>
    /// Global template variables
    /// </summary>
    public Dictionary<string, object> GlobalVariables { get; set; } = new();

    /// <summary>
    /// Email sender information
    /// </summary>
    public EmailSenderInfo? Sender { get; set; }

    /// <summary>
    /// Reply-to address
    /// </summary>
    [EmailAddress]
    public string? ReplyTo { get; set; }

    /// <summary>
    /// Email priority
    /// </summary>
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Attachments to include
    /// </summary>
    public List<EmailAttachment> Attachments { get; set; } = new();

    /// <summary>
    /// Tracking options
    /// </summary>
    public EmailTrackingOptions? Tracking { get; set; }

    /// <summary>
    /// Delivery options
    /// </summary>
    public EmailDeliveryOptions? DeliveryOptions { get; set; }

    /// <summary>
    /// Tags for categorization
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Whether to send immediately or schedule
    /// </summary>
    public bool SendImmediately { get; set; } = true;

    /// <summary>
    /// Schedule time (if not sending immediately)
    /// </summary>
    public DateTime? ScheduledFor { get; set; }

    /// <summary>
    /// Maximum batch size for processing
    /// </summary>
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Delay between batches (milliseconds)
    /// </summary>
    public int BatchDelayMs { get; set; } = 1000;
}

/// <summary>
/// Bulk email recipient information
/// </summary>
public class BulkEmailRecipient
{
    /// <summary>
    /// Recipient email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Recipient name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// User ID (if known)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Personalized variables for this recipient
    /// </summary>
    public Dictionary<string, object> Variables { get; set; } = new();

    /// <summary>
    /// Custom metadata for this recipient
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Language preference
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Timezone for scheduling
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// Custom subject override
    /// </summary>
    public string? CustomSubject { get; set; }

    /// <summary>
    /// Custom body override
    /// </summary>
    public string? CustomBody { get; set; }
}

/// <summary>
/// Email validation result
/// </summary>
public class EmailValidationResult
{
    /// <summary>
    /// Email address that was validated
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Whether email is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Validation score (0-100)
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Validation status
    /// </summary>
    public EmailValidationStatus Status { get; set; }

    /// <summary>
    /// Validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Validation warnings
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Whether email domain exists
    /// </summary>
    public bool DomainExists { get; set; }

    /// <summary>
    /// Whether mailbox exists (if checked)
    /// </summary>
    public bool? MailboxExists { get; set; }

    /// <summary>
    /// Whether email is disposable
    /// </summary>
    public bool IsDisposable { get; set; }

    /// <summary>
    /// Whether email is a role account
    /// </summary>
    public bool IsRole { get; set; }

    /// <summary>
    /// Suggested correction (if applicable)
    /// </summary>
    public string? SuggestedCorrection { get; set; }

    /// <summary>
    /// Validation timestamp
    /// </summary>
    public DateTime ValidatedAt { get; set; }

    /// <summary>
    /// Validation provider used
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Time taken for validation (ms)
    /// </summary>
    public double ValidationTimeMs { get; set; }

    /// <summary>
    /// Additional validation metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Validation failure reason
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Risk score for email deliverability (0-100)
    /// </summary>
    public int RiskScore { get; set; }

    /// <summary>
    /// Whether email is deliverable
    /// </summary>
    public bool IsDeliverable { get; set; }
}

/// <summary>
/// Email validation status enumeration
/// </summary>
public enum EmailValidationStatus
{
    /// <summary>
    /// Valid email address
    /// </summary>
    Valid = 0,

    /// <summary>
    /// Invalid email format
    /// </summary>
    InvalidFormat = 1,

    /// <summary>
    /// Domain does not exist
    /// </summary>
    InvalidDomain = 2,

    /// <summary>
    /// Mailbox does not exist
    /// </summary>
    InvalidMailbox = 3,

    /// <summary>
    /// Disposable email address
    /// </summary>
    Disposable = 4,

    /// <summary>
    /// Role-based email address
    /// </summary>
    Role = 5,

    /// <summary>
    /// Validation failed due to error
    /// </summary>
    ValidationError = 6,

    /// <summary>
    /// Validation timed out
    /// </summary>
    Timeout = 7,

    /// <summary>
    /// Unknown validation status
    /// </summary>
    Unknown = 8
}

/// <summary>
/// Email sender information
/// </summary>
public class EmailSenderInfo
{
    /// <summary>
    /// Sender email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Sender display name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Organization name
    /// </summary>
    public string? Organization { get; set; }
}

/// <summary>
/// Email tracking options
/// </summary>
public class EmailTrackingOptions
{
    /// <summary>
    /// Track email opens
    /// </summary>
    public bool TrackOpens { get; set; } = true;

    /// <summary>
    /// Track link clicks
    /// </summary>
    public bool TrackClicks { get; set; } = true;

    /// <summary>
    /// Track unsubscribes
    /// </summary>
    public bool TrackUnsubscribes { get; set; } = true;

    /// <summary>
    /// Custom tracking parameters
    /// </summary>
    public Dictionary<string, string> CustomParameters { get; set; } = new();
}

/// <summary>
/// Email delivery options
/// </summary>
public class EmailDeliveryOptions
{
    /// <summary>
    /// Maximum retry attempts
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Retry delay in minutes
    /// </summary>
    public int RetryDelayMinutes { get; set; } = 5;

    /// <summary>
    /// Send timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Whether to use backup provider on failure
    /// </summary>
    public bool UseBackupProvider { get; set; } = true;

    /// <summary>
    /// Test mode (don't actually send)
    /// </summary>
    public bool TestMode { get; set; } = false;

    /// <summary>
    /// Suppress duplicate sends within timeframe
    /// </summary>
    public bool SuppressDuplicates { get; set; } = true;

    /// <summary>
    /// Duplicate suppression window in minutes
    /// </summary>
    public int DuplicateWindowMinutes { get; set; } = 60;
}

/// <summary>
/// Request for validating multiple emails
/// </summary>
public class BulkEmailValidationRequest
{
    /// <summary>
    /// List of email addresses to validate
    /// </summary>
    [Required]
    public List<string> Emails { get; set; } = new();

    /// <summary>
    /// Validation options
    /// </summary>
    public EmailValidationOptions? Options { get; set; }

    /// <summary>
    /// Whether to check mailbox existence
    /// </summary>
    public bool CheckMailbox { get; set; } = false;

    /// <summary>
    /// Maximum validation time per email (seconds)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 10;

    /// <summary>
    /// Batch size for processing
    /// </summary>
    public int BatchSize { get; set; } = 50;
}

/// <summary>
/// Email validation options
/// </summary>
public class EmailValidationOptions
{
    /// <summary>
    /// Check domain MX records
    /// </summary>
    public bool CheckMX { get; set; } = true;

    /// <summary>
    /// Check for disposable domains
    /// </summary>
    public bool CheckDisposable { get; set; } = true;

    /// <summary>
    /// Check for role accounts
    /// </summary>
    public bool CheckRole { get; set; } = true;

    /// <summary>
    /// Provide typo suggestions
    /// </summary>
    public bool ProvideSuggestions { get; set; } = true;

    /// <summary>
    /// Strict validation mode
    /// </summary>
    public bool StrictMode { get; set; } = false;
}

/// <summary>
/// Bulk email validation response
/// </summary>
public class BulkEmailValidationResponse
{
    /// <summary>
    /// Individual validation results
    /// </summary>
    public List<EmailValidationResult> Results { get; set; } = new();

    /// <summary>
    /// Overall statistics
    /// </summary>
    public EmailValidationStatistics Statistics { get; set; } = new();

    /// <summary>
    /// Processing information
    /// </summary>
    public ValidationProcessingInfo ProcessingInfo { get; set; } = new();
}

/// <summary>
/// Email validation statistics
/// </summary>
public class EmailValidationStatistics
{
    /// <summary>
    /// Total emails processed
    /// </summary>
    public int TotalProcessed { get; set; }

    /// <summary>
    /// Number of valid emails
    /// </summary>
    public int ValidCount { get; set; }

    /// <summary>
    /// Number of invalid emails
    /// </summary>
    public int InvalidCount { get; set; }

    /// <summary>
    /// Number of disposable emails
    /// </summary>
    public int DisposableCount { get; set; }

    /// <summary>
    /// Number of role emails
    /// </summary>
    public int RoleCount { get; set; }

    /// <summary>
    /// Number of validation errors
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Overall validation rate percentage
    /// </summary>
    public double ValidationRate { get; set; }

    /// <summary>
    /// Average validation time
    /// </summary>
    public double AverageValidationTimeMs { get; set; }
}

/// <summary>
/// Validation processing information
/// </summary>
public class ValidationProcessingInfo
{
    /// <summary>
    /// Processing start time
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Processing completion time
    /// </summary>
    public DateTime CompletedAt { get; set; }

    /// <summary>
    /// Total processing time
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Number of batches processed
    /// </summary>
    public int BatchesProcessed { get; set; }

    /// <summary>
    /// Validation provider used
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Processing status
    /// </summary>
    public string Status { get; set; } = "Completed";

    /// <summary>
    /// Any processing errors
    /// </summary>
    public List<string> Errors { get; set; } = new();
}
