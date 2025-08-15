using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Core.DTOs;

/// <summary>
/// Email request DTO
/// </summary>
public class EmailRequest
{
    /// <summary>
    /// Recipient email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Sender email address (optional, uses configured default if not provided)
    /// </summary>
    [EmailAddress]
    public string? From { get; set; }

    /// <summary>
    /// Email subject
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Plain text email content
    /// </summary>
    [StringLength(10000)]
    public string? TextContent { get; set; }

    /// <summary>
    /// HTML email content
    /// </summary>
    [StringLength(50000)]
    public string? HtmlContent { get; set; }

    /// <summary>
    /// Reply-to email address
    /// </summary>
    [EmailAddress]
    public string? ReplyTo { get; set; }

    /// <summary>
    /// CC email addresses
    /// </summary>
    public List<string> Cc { get; set; } = new();

    /// <summary>
    /// BCC email addresses
    /// </summary>
    public List<string> Bcc { get; set; } = new();

    /// <summary>
    /// Email attachments
    /// </summary>
    public List<EmailAttachment> Attachments { get; set; } = new();

    /// <summary>
    /// Custom headers
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Email priority
    /// </summary>
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;

    /// <summary>
    /// Track email opens
    /// </summary>
    public bool TrackOpens { get; set; } = true;

    /// <summary>
    /// Track email clicks
    /// </summary>
    public bool TrackClicks { get; set; } = true;

    /// <summary>
    /// Custom metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    // Alias properties for backward compatibility
    /// <summary>
    /// Alias for To property
    /// </summary>
    public string ToEmail 
    { 
        get => To; 
        set => To = value; 
    }

    /// <summary>
    /// Alias for From property
    /// </summary>
    public string? FromEmail 
    { 
        get => From; 
        set => From = value; 
    }

    /// <summary>
    /// Recipient display name
    /// </summary>
    public string? ToName { get; set; }

    /// <summary>
    /// Sender display name
    /// </summary>
    public string? FromName { get; set; }

    /// <summary>
    /// Alias for ReplyTo property
    /// </summary>
    public string? ReplyToEmail 
    { 
        get => ReplyTo; 
        set => ReplyTo = value; 
    }

    /// <summary>
    /// Email tags for categorization
    /// </summary>
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Templated email request DTO
/// </summary>
public class TemplatedEmailRequest : EmailRequest
{
    /// <summary>
    /// Template ID to use
    /// </summary>
    [Required]
    public Guid TemplateId { get; set; }

    /// <summary>
    /// Template variables for substitution
    /// </summary>
    public Dictionary<string, object> TemplateData { get; set; } = new();
}

/// <summary>
/// Template-based email request for providers
/// </summary>
public class TemplateEmailRequest : EmailRequest
{
    /// <summary>
    /// Template identifier in the email provider.
    /// </summary>
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>
    /// Variables for template substitution.
    /// </summary>
    public Dictionary<string, object> TemplateVariables { get; set; } = new();

    /// <summary>
    /// Language/locale for template selection.
    /// </summary>
    public string Language { get; set; } = "en";
}

/// <summary>
/// Email attachment model.
/// </summary>
public class EmailAttachment
{
    /// <summary>
    /// File name for the attachment.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME content type.
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File content as base64 string
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Content disposition (attachment or inline)
    /// </summary>
    public string Disposition { get; set; } = "attachment";

    /// <summary>
    /// Content ID for inline attachments
    /// </summary>
    public string? ContentId { get; set; }
}

/// <summary>
/// Email result DTO
/// </summary>
public class EmailResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? MessageId { get; set; }
}

/// <summary>
/// Email delivery result DTO
/// </summary>
public class EmailDeliveryResult
{
    /// <summary>
    /// Indicates whether the email was successfully sent.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Provider-specific message identifier for tracking.
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Error message if delivery failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Error code if delivery failed.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Timestamp when the email was sent.
    /// </summary>
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Additional metadata from the provider.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Result of bulk email delivery operation.
/// </summary>
public class BulkEmailDeliveryResult
{
    /// <summary>
    /// Total number of emails processed.
    /// </summary>
    public int TotalProcessed { get; set; }

    /// <summary>
    /// Number of messages successfully sent.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of messages that failed to send.
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Individual results for each email.
    /// </summary>
    public List<EmailDeliveryResult> Results { get; set; } = new();

    /// <summary>
    /// Overall error message if the bulk operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Total cost of the bulk operation (if available).
    /// </summary>
    public decimal? TotalCost { get; set; }

    /// <summary>
    /// Currency of the cost (if available).
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Success rate as a percentage.
    /// </summary>
    public double SuccessRate => TotalProcessed > 0 ? (double)SuccessCount / TotalProcessed * 100 : 0;
}

/// <summary>
/// Email validation result
/// </summary>
public class EmailValidationResult
{
    /// <summary>
    /// Whether the email is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Validation score (0-100)
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Validation details
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Whether the email domain exists
    /// </summary>
    public bool? DomainExists { get; set; }

    /// <summary>
    /// Whether the email is disposable
    /// </summary>
    public bool? IsDisposable { get; set; }

    /// <summary>
    /// Whether the email is free
    /// </summary>
    public bool? IsFree { get; set; }

    /// <summary>
    /// Validation reason
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Risk score
    /// </summary>
    public int RiskScore { get; set; }

    /// <summary>
    /// Whether the email is deliverable
    /// </summary>
    public bool IsDeliverable { get; set; }

    /// <summary>
    /// Validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Validation warnings
    /// </summary>
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Email priority enum
/// </summary>
public enum EmailPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}
