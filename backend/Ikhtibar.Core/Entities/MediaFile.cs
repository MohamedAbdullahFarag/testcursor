using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Entities;

/// <summary>
/// Core media file entity for storing multimedia content metadata
/// Supports images, videos, audio files, and documents for question bank
/// </summary>
[Table("MediaFiles")]
public class MediaFile : BaseEntity
{
    /// <summary>
    /// Original filename as uploaded by user
    /// </summary>
    [Required]
    [StringLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// Internal filename for storage (may be different from original)
    /// </summary>
    [Required]
    [StringLength(255)]
    public string StorageFileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME content type (e.g., image/jpeg, video/mp4)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    [Required]
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Storage path or container location
    /// </summary>
    [Required]
    [StringLength(500)]
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// Media type classification
    /// </summary>
    [Required]
    public MediaType MediaType { get; set; }

    /// <summary>
    /// Current processing status
    /// </summary>
    [Required]
    public MediaStatus Status { get; set; } = MediaStatus.Uploading;

    /// <summary>
    /// Optional title for the media file
    /// </summary>
    [StringLength(255)]
    public string? Title { get; set; }

    /// <summary>
    /// Optional description
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Alternative text for accessibility
    /// </summary>
    [StringLength(255)]
    public string? AltText { get; set; }

    /// <summary>
    /// File hash for duplicate detection and integrity verification
    /// </summary>
    [StringLength(64)]
    public string? FileHash { get; set; }

    /// <summary>
    /// Category this media file belongs to
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// User who uploaded this file
    /// </summary>
    [Required]
    public int UploadedBy { get; set; }

    /// <summary>
    /// Whether this file is publicly accessible
    /// </summary>
    [Required]
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Number of times this file has been accessed
    /// </summary>
    [Required]
    public int AccessCount { get; set; } = 0;

    /// <summary>
    /// Last time this file was accessed
    /// </summary>
    public DateTime? LastAccessedAt { get; set; }

    /// <summary>
    /// JSON metadata for file-specific properties (dimensions, duration, etc.)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Metadata { get; set; }

    // Navigation Properties

    /// <summary>
    /// Category this media file belongs to
    /// </summary>
    [ForeignKey(nameof(CategoryId))]
    public virtual MediaCategory? Category { get; set; }

    /// <summary>
    /// Extended metadata entries for this file
    /// </summary>
    public virtual ICollection<MediaMetadata> ExtendedMetadata { get; set; } = new List<MediaMetadata>();

    /// <summary>
    /// Thumbnails generated for this media file
    /// </summary>
    public virtual ICollection<MediaThumbnail> Thumbnails { get; set; } = new List<MediaThumbnail>();

    /// <summary>
    /// Access logs for this media file
    /// </summary>
    public virtual ICollection<MediaAccessLog> AccessLogs { get; set; } = new List<MediaAccessLog>();

    /// <summary>
    /// Collections this media file belongs to
    /// </summary>
    public virtual ICollection<MediaCollection> Collections { get; set; } = new List<MediaCollection>();

    /// <summary>
    /// Processing jobs for this media file
    /// </summary>
    public virtual ICollection<MediaProcessingJob> ProcessingJobs { get; set; } = new List<MediaProcessingJob>();
}

/// <summary>
/// Media type classification
/// </summary>
public enum MediaType
{
    /// <summary>
    /// Image files (JPEG, PNG, GIF, WebP, etc.)
    /// </summary>
    Image = 1,

    /// <summary>
    /// Video files (MP4, WebM, AVI, etc.)
    /// </summary>
    Video = 2,

    /// <summary>
    /// Audio files (MP3, WAV, OGG, etc.)
    /// </summary>
    Audio = 3,

    /// <summary>
    /// Document files (PDF, DOC, PPT, etc.)
    /// </summary>
    Document = 4,

    /// <summary>
    /// Archive files (ZIP, RAR, etc.)
    /// </summary>
    Archive = 5,

    /// <summary>
    /// Other file types
    /// </summary>
    Other = 99
}

/// <summary>
/// Media file processing and availability status
/// </summary>
public enum MediaStatus
{
    /// <summary>
    /// File is being uploaded
    /// </summary>
    Uploading = 0,
    
    /// <summary>
    /// File is being processed (thumbnails, metadata extraction, etc.)
    /// </summary>
    Processing = 1,
    
    /// <summary>
    /// File is available and ready for use
    /// </summary>
    Available = 2,
    
    /// <summary>
    /// File processing failed
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// File is quarantined (security scan, virus detected, etc.)
    /// </summary>
    Quarantined = 4,
    
    /// <summary>
    /// File is archived (moved to cold storage)
    /// </summary>
    Archived = 5,
    
    /// <summary>
    /// File is deleted (soft delete)
    /// </summary>
    Deleted = 6
}
