using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Media thumbnail entity for storing generated thumbnails
/// </summary>
public class MediaThumbnail
{
    /// <summary>
    /// Unique identifier for the thumbnail
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Reference to the original media file
    /// </summary>
    [Required]
    public int MediaFileId { get; set; }

    /// <summary>
    /// Thumbnail size category
    /// </summary>
    [Required]
    public ThumbnailSize Size { get; set; }

    /// <summary>
    /// Thumbnail width in pixels
    /// </summary>
    [Required]
    public int Width { get; set; }

    /// <summary>
    /// Thumbnail height in pixels
    /// </summary>
    [Required]
    public int Height { get; set; }

    /// <summary>
    /// Storage path for the thumbnail file
    /// </summary>
    [Required]
    [StringLength(500)]
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// Thumbnail file size in bytes
    /// </summary>
    [Required]
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// MIME content type of the thumbnail
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Thumbnail generation method used
    /// </summary>
    public ThumbnailGenerationMethod GenerationMethod { get; set; }

    /// <summary>
    /// Processing status of the thumbnail
    /// </summary>
    public ThumbnailStatus Status { get; set; }

    /// <summary>
    /// Quality setting used for generation (1-100)
    /// </summary>
    public int Quality { get; set; } = 85;

    /// <summary>
    /// Additional metadata for the thumbnail
    /// </summary>
    [StringLength(1000)]
    public string? Metadata { get; set; }

    /// <summary>
    /// When the thumbnail was created
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the thumbnail was last modified
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Processing duration in milliseconds
    /// </summary>
    public long? ProcessingDurationMs { get; set; }

    /// <summary>
    /// Error message if generation failed
    /// </summary>
    [StringLength(500)]
    public string? ErrorMessage { get; set; }

    // Navigation Properties

    /// <summary>
    /// Original media file
    /// </summary>
    [ForeignKey(nameof(MediaFileId))]
    public virtual MediaFile MediaFile { get; set; } = null!;
}

/// <summary>
/// Thumbnail generation methods
/// </summary>
public enum ThumbnailGenerationMethod
{
    /// <summary>
    /// Simple resize maintaining aspect ratio
    /// </summary>
    Resize = 1,

    /// <summary>
    /// Crop to exact dimensions
    /// </summary>
    Crop = 2,

    /// <summary>
    /// Fit within dimensions with padding
    /// </summary>
    Fit = 3,

    /// <summary>
    /// Fill dimensions by cropping excess
    /// </summary>
    Fill = 4,

    /// <summary>
    /// Stretch to exact dimensions (may distort)
    /// </summary>
    Stretch = 5
}

/// <summary>
/// Thumbnail processing status
/// </summary>
public enum ThumbnailStatus
{
    /// <summary>
    /// Thumbnail is being generated
    /// </summary>
    Generating = 1,

    /// <summary>
    /// Thumbnail generation completed successfully
    /// </summary>
    Generated = 2,

    /// <summary>
    /// Thumbnail generation failed
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Thumbnail was deleted or is no longer available
    /// </summary>
    Deleted = 4
}
