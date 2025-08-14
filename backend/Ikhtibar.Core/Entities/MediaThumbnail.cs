using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Entities;

/// <summary>
/// Thumbnail management entity for storing generated thumbnails
/// Supports multiple sizes and formats for different use cases
/// </summary>
[Table("MediaThumbnails")]
public class MediaThumbnail : BaseEntity
{
    /// <summary>
    /// Original media file this thumbnail was generated from
    /// </summary>
    [Required]
    public Guid MediaFileId { get; set; }

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
    /// Thumbnail file format (e.g., webp, jpeg, png)
    /// </summary>
    [Required]
    [StringLength(10)]
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Storage path for the thumbnail file
    /// </summary>
    [Required]
    [StringLength(500)]
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// File size of the thumbnail in bytes
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
    /// Quality setting used for generation (1-100)
    /// </summary>
    public int? Quality { get; set; }

    /// <summary>
    /// Whether this thumbnail is the default for its size
    /// </summary>
    [Required]
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Generation method used
    /// </summary>
    [Required]
    public ThumbnailGenerationMethod GenerationMethod { get; set; } = ThumbnailGenerationMethod.Resize;

    /// <summary>
    /// Processing status of the thumbnail
    /// </summary>
    [Required]
    public ThumbnailStatus Status { get; set; } = ThumbnailStatus.Generating;

    /// <summary>
    /// Time taken to generate this thumbnail (in milliseconds)
    /// </summary>
    public int? GenerationTimeMs { get; set; }

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
/// Standard thumbnail size categories
/// </summary>
public enum ThumbnailSize
{
    /// <summary>
    /// Small thumbnail (typically 64x64 or 100x100)
    /// </summary>
    Small = 1,

    /// <summary>
    /// Medium thumbnail (typically 150x150 or 200x200)
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Large thumbnail (typically 300x300 or 400x400)
    /// </summary>
    Large = 3,

    /// <summary>
    /// Extra large thumbnail (typically 600x600 or 800x800)
    /// </summary>
    ExtraLarge = 4,

    /// <summary>
    /// Custom size thumbnail
    /// </summary>
    Custom = 5
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
