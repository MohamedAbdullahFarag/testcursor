using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Entities;

/// <summary>
/// Media access tracking entity for analytics and security
/// Logs when and how media files are accessed
/// </summary>
[Table("MediaAccessLogs")]
public class MediaAccessLog : BaseEntity
{
    /// <summary>
    /// Media file that was accessed
    /// </summary>
    [Required]
    public Guid MediaFileId { get; set; }

    /// <summary>
    /// User who accessed the file (if authenticated)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Type of access that occurred
    /// </summary>
    [Required]
    public AccessType AccessType { get; set; }

    /// <summary>
    /// IP address of the client
    /// </summary>
    [StringLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string from the client
    /// </summary>
    [StringLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Referrer URL if available
    /// </summary>
    [StringLength(500)]
    public string? Referrer { get; set; }

    /// <summary>
    /// Session ID if available
    /// </summary>
    [StringLength(100)]
    public string? SessionId { get; set; }

    /// <summary>
    /// Geographic location information (JSON)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? GeoLocation { get; set; }

    /// <summary>
    /// Additional context about the access
    /// </summary>
    [StringLength(500)]
    public string? AccessContext { get; set; }

    /// <summary>
    /// Time spent accessing the media (for analytics)
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Bytes downloaded/transferred
    /// </summary>
    public long? BytesTransferred { get; set; }

    /// <summary>
    /// HTTP status code returned
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Whether the access was successful
    /// </summary>
    [Required]
    public bool Success { get; set; } = true;

    /// <summary>
    /// Error message if access failed
    /// </summary>
    [StringLength(500)]
    public string? ErrorMessage { get; set; }

    // Navigation Properties

    /// <summary>
    /// Media file that was accessed
    /// </summary>
    [ForeignKey(nameof(MediaFileId))]
    public virtual MediaFile MediaFile { get; set; } = null!;
}

/// <summary>
/// Types of media access operations
/// </summary>
public enum AccessType
{
    /// <summary>
    /// File was viewed/displayed
    /// </summary>
    View = 1,

    /// <summary>
    /// File was downloaded
    /// </summary>
    Download = 2,

    /// <summary>
    /// Thumbnail was accessed
    /// </summary>
    Thumbnail = 3,

    /// <summary>
    /// File metadata was accessed
    /// </summary>
    Metadata = 4,

    /// <summary>
    /// File was streamed (for video/audio)
    /// </summary>
    Stream = 5,

    /// <summary>
    /// File was embedded in another page
    /// </summary>
    Embed = 6,

    /// <summary>
    /// File was shared via link
    /// </summary>
    Share = 7,

    /// <summary>
    /// File was printed
    /// </summary>
    Print = 8
}
