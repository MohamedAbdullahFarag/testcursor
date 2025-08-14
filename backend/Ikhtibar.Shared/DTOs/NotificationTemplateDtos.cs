using System.ComponentModel.DataAnnotations;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for creating a new notification template
/// </summary>
public class CreateNotificationTemplateDto
{
    /// <summary>
    /// Type of notification this template is for
    /// </summary>
    [Required]
    [EnumDataType(typeof(NotificationType))]
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Language code (e.g., "en", "ar")
    /// </summary>
    [Required]
    [StringLength(10)]
    [RegularExpression(@"^[a-z]{2}(-[A-Z]{2})?$", ErrorMessage = "Language must be in format 'en' or 'en-US'")]
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Template subject/title
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Template message content with placeholder variables
    /// </summary>
    [Required]
    [StringLength(5000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// HTML content for email notifications
    /// </summary>
    [StringLength(10000)]
    public string? EmailHtml { get; set; }

    /// <summary>
    /// SMS-specific content (shorter format)
    /// </summary>
    [StringLength(1000)]
    public string? SmsContent { get; set; }

    /// <summary>
    /// Push notification title
    /// </summary>
    [StringLength(100)]
    public string? PushTitle { get; set; }

    /// <summary>
    /// Push notification body
    /// </summary>
    [StringLength(500)]
    public string? PushBody { get; set; }

    /// <summary>
    /// Available variables for this template
    /// </summary>
    public Dictionary<string, object> Variables { get; set; } = new();

    /// <summary>
    /// Whether the template is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    public string Template { get; set; } = string.Empty;
    public string SmsTemplate { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating a notification template
/// </summary>
public class UpdateNotificationTemplateDto
{
    /// <summary>
    /// Template subject/title
    /// </summary>
    [StringLength(500)]
    public string? Subject { get; set; }

    /// <summary>
    /// Template message content with placeholder variables
    /// </summary>
    [StringLength(5000)]
    public string? Message { get; set; }

    /// <summary>
    /// HTML content for email notifications
    /// </summary>
    [StringLength(10000)]
    public string? EmailHtml { get; set; }

    /// <summary>
    /// SMS-specific content (shorter format)
    /// </summary>
    [StringLength(1000)]
    public string? SmsContent { get; set; }

    /// <summary>
    /// Push notification title
    /// </summary>
    [StringLength(100)]
    public string? PushTitle { get; set; }

    /// <summary>
    /// Push notification body
    /// </summary>
    [StringLength(500)]
    public string? PushBody { get; set; }

    /// <summary>
    /// Available variables for this template
    /// </summary>
    public Dictionary<string, object>? Variables { get; set; }

    /// <summary>
    /// Whether the template is active
    /// </summary>
    public bool? IsActive { get; set; }
    public string? Template { get; set; }
}

/// <summary>
/// DTO for notification template response data
/// </summary>
public class NotificationTemplateDto
{
    /// <summary>
    /// Unique identifier for the template
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Type of notification this template is for
    /// </summary>
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Language code
    /// </summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Template subject/title
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Template message content
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// HTML content for email notifications
    /// </summary>
    public string? EmailHtml { get; set; }

    /// <summary>
    /// SMS-specific content
    /// </summary>
    public string? SmsContent { get; set; }

    /// <summary>
    /// Push notification title
    /// </summary>
    public string? PushTitle { get; set; }

    /// <summary>
    /// Push notification body
    /// </summary>
    public string? PushBody { get; set; }

    /// <summary>
    /// Available variables for this template
    /// </summary>
    public Dictionary<string, object> Variables { get; set; } = new();

    /// <summary>
    /// Whether the template is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// When the template was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the template was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; }
}

/// <summary>
/// DTO for template filter and search parameters
/// </summary>
public class TemplateFilterDto
{
    /// <summary>
    /// Filter by notification type
    /// </summary>
    [EnumDataType(typeof(NotificationType))]
    public NotificationType? NotificationType { get; set; }

    /// <summary>
    /// Filter by language
    /// </summary>
    [StringLength(10)]
    public string? Language { get; set; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Search term for template content
    /// </summary>
    [StringLength(100)]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Sort field
    /// </summary>
    [StringLength(50)]
    public string SortBy { get; set; } = "CreatedAt";

    /// <summary>
    /// Sort direction
    /// </summary>
    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'")]
    public string SortDirection { get; set; } = "desc";
}

/// <summary>
/// DTO for template processing with variables
/// </summary>
public class ProcessTemplateDto
{
    /// <summary>
    /// Template ID to process
    /// </summary>
    [Required]
    public Guid TemplateId { get; set; }

    /// <summary>
    /// Variables to substitute in the template
    /// </summary>
    [Required]
    public Dictionary<string, object> Variables { get; set; } = new();
}

/// <summary>
/// DTO for processed template result
/// </summary>
public class ProcessedTemplateDto
{
    /// <summary>
    /// Processed subject/title
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Processed message content
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Processed HTML content for email
    /// </summary>
    public string? EmailHtml { get; set; }

    /// <summary>
    /// Processed SMS content
    /// </summary>
    public string? SmsContent { get; set; }

    /// <summary>
    /// Processed push notification title
    /// </summary>
    public string? PushTitle { get; set; }

    /// <summary>
    /// Processed push notification body
    /// </summary>
    public string? PushBody { get; set; }

    /// <summary>
    /// Variables that were used in processing
    /// </summary>
    public Dictionary<string, object> UsedVariables { get; set; } = new();
}

/// <summary>
/// DTO for template validation request
/// </summary>
public class ValidateTemplateDto
{
    /// <summary>
    /// Template content to validate
    /// </summary>
    [Required]
    [StringLength(10000)]
    public string TemplateContent { get; set; } = string.Empty;

    /// <summary>
    /// Notification type for context-specific validation
    /// </summary>
    [EnumDataType(typeof(NotificationType))]
    public NotificationType? NotificationType { get; set; }
}

/// <summary>
/// DTO for template validation result
/// </summary>
public class TemplateValidationResultDto
{
    /// <summary>
    /// Whether the template is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// List of validation warnings
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Variables found in the template
    /// </summary>
    public List<string> FoundVariables { get; set; } = new();

    /// <summary>
    /// Available variables for the notification type
    /// </summary>
    public List<TemplateVariableDto> AvailableVariables { get; set; } = new();
}

/// <summary>
/// DTO for template variable definition
/// </summary>
public class TemplateVariableDto
{
    /// <summary>
    /// Variable name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Variable description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Data type of the variable
    /// </summary>
    public string DataType { get; set; } = "string";

    /// <summary>
    /// Whether the variable is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Example value for the variable
    /// </summary>
    public string? Example { get; set; }
}

/// <summary>
/// DTO for template import request
/// </summary>
public class ImportTemplatesDto
{
    /// <summary>
    /// File content to import (JSON, CSV, or Excel format)
    /// </summary>
    [Required]
    public string FileContent { get; set; } = string.Empty;

    /// <summary>
    /// Type of file being imported
    /// </summary>
    [Required]
    [EnumDataType(typeof(TemplateImportType))]
    public TemplateImportType FileType { get; set; }

    /// <summary>
    /// Whether to overwrite existing templates
    /// </summary>
    public bool OverwriteExisting { get; set; } = false;
}

/// <summary>
/// DTO for template import result
/// </summary>
public class TemplateImportResultDto
{
    /// <summary>
    /// Number of templates created
    /// </summary>
    public int TemplatesCreated { get; set; }

    /// <summary>
    /// Number of templates updated
    /// </summary>
    public int TemplatesUpdated { get; set; }

    /// <summary>
    /// Number of templates skipped
    /// </summary>
    public int TemplatesSkipped { get; set; }

    /// <summary>
    /// List of import errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// List of import warnings
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Total number of templates processed
    /// </summary>
    public int TotalProcessed => TemplatesCreated + TemplatesUpdated + TemplatesSkipped;

    /// <summary>
    /// Success rate as a percentage
    /// </summary>
    public double SuccessRate => TotalProcessed > 0 ? (double)(TemplatesCreated + TemplatesUpdated) / TotalProcessed * 100 : 0;
}


/// <summary>
/// DTO for duplicating a template
/// </summary>
public class DuplicateTemplateDto
{
    /// <summary>
    /// Source template ID to duplicate
    /// </summary>
    [Required]
    public Guid SourceTemplateId { get; set; }

    /// <summary>
    /// Target language for the duplicated template
    /// </summary>
    [Required]
    [StringLength(10)]
    [RegularExpression(@"^[a-z]{2}(-[A-Z]{2})?$", ErrorMessage = "Language must be in format 'en' or 'en-US'")]
    public string TargetLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Whether to attempt auto-translation of content
    /// </summary>
    public bool TranslateContent { get; set; } = false;
}

/// <summary>
/// DTO for paginated template results
/// </summary>
public class PagedTemplateResultDto
{
    /// <summary>
    /// List of templates for the current page
    /// </summary>
    public List<NotificationTemplateDto> Templates { get; set; } = new();

    /// <summary>
    /// Total number of templates matching the criteria
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;
}

/// <summary>
/// Template import types enumeration
/// </summary>
public enum TemplateImportType
{
    Json,
    Csv,
    Excel
}
