using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Entities;

/// <summary>
/// Media collection entity for organizing related media files
/// Supports grouping media files into albums, galleries, or sets
/// </summary>
[Table("MediaCollections")]
public class MediaCollection : BaseEntity
{
    /// <summary>
    /// Collection name/title
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Collection description
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Collection slug for URLs
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Type of collection
    /// </summary>
    [Required]
    public CollectionType CollectionType { get; set; } = CollectionType.General;

    /// <summary>
    /// User who created this collection
    /// </summary>
    [Required]
    public Guid CreatedByUserId { get; set; }

    /// <summary>
    /// Whether this collection is publicly visible
    /// </summary>
    [Required]
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Whether this collection is featured
    /// </summary>
    [Required]
    public bool IsFeatured { get; set; } = false;

    /// <summary>
    /// Display order for sorting collections
    /// </summary>
    [Required]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Cover image for the collection
    /// </summary>
    public Guid? CoverImageId { get; set; }

    /// <summary>
    /// Tags for categorizing the collection
    /// </summary>
    [StringLength(500)]
    public string? Tags { get; set; }

    /// <summary>
    /// View count for analytics
    /// </summary>
    [Required]
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// JSON metadata for collection-specific properties
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Metadata { get; set; }

    // Navigation Properties

    /// <summary>
    /// Cover image for the collection
    /// </summary>
    [ForeignKey(nameof(CoverImageId))]
    public virtual MediaFile? CoverImage { get; set; }

    /// <summary>
    /// Media files in this collection
    /// </summary>
    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

    /// <summary>
    /// Collection items with ordering information
    /// </summary>
    public virtual ICollection<MediaCollectionItem> CollectionItems { get; set; } = new List<MediaCollectionItem>();
}

/// <summary>
/// Junction entity for media files in collections with ordering
/// </summary>
[Table("MediaCollectionItems")]
public class MediaCollectionItem : BaseEntity
{
    /// <summary>
    /// Collection this item belongs to
    /// </summary>
    [Required]
    public Guid CollectionId { get; set; }

    /// <summary>
    /// Media file in the collection
    /// </summary>
    [Required]
    public Guid MediaFileId { get; set; }

    /// <summary>
    /// Order within the collection
    /// </summary>
    [Required]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Optional caption for this item in the collection
    /// </summary>
    [StringLength(500)]
    public string? Caption { get; set; }

    /// <summary>
    /// Whether this item is featured in the collection
    /// </summary>
    [Required]
    public bool IsFeatured { get; set; } = false;

    // Navigation Properties

    /// <summary>
    /// Collection this item belongs to
    /// </summary>
    [ForeignKey(nameof(CollectionId))]
    public virtual MediaCollection Collection { get; set; } = null!;

    /// <summary>
    /// Media file in the collection
    /// </summary>
    [ForeignKey(nameof(MediaFileId))]
    public virtual MediaFile MediaFile { get; set; } = null!;
}

/// <summary>
/// Types of media collections
/// </summary>
public enum CollectionType
{
    /// <summary>
    /// General collection/album
    /// </summary>
    General = 1,

    /// <summary>
    /// Photo gallery
    /// </summary>
    Gallery = 2,

    /// <summary>
    /// Video playlist
    /// </summary>
    Playlist = 3,

    /// <summary>
    /// Document folder
    /// </summary>
    DocumentSet = 4,

    /// <summary>
    /// Question-related media
    /// </summary>
    QuestionMedia = 5,

    /// <summary>
    /// Educational resources
    /// </summary>
    EducationalResources = 6,

    /// <summary>
    /// Template collection
    /// </summary>
    Template = 7
}
