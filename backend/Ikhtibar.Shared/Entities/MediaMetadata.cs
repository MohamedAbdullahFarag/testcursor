using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Extended metadata entity for storing additional file properties
/// Supports flexible key-value metadata storage for different media types
/// </summary>
[Table("MediaMetadata")]
public class MediaMetadata : BaseEntity
{
    /// <summary>
    /// Media file this metadata belongs to
    /// </summary>
    [Required]
    public int MediaFileId { get; set; }

    /// <summary>
    /// Metadata key/property name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string MetadataKey { get; set; } = string.Empty;

    /// <summary>
    /// Metadata value
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? MetadataValue { get; set; }

    /// <summary>
    /// Data type of the metadata value
    /// </summary>
    [Required]
    public MetadataType DataType { get; set; } = MetadataType.String;

    /// <summary>
    /// Whether this metadata is searchable
    /// </summary>
    [Required]
    public bool IsSearchable { get; set; } = false;

    /// <summary>
    /// Whether this metadata is publicly visible
    /// </summary>
    [Required]
    public bool IsPublic { get; set; } = true;

    /// <summary>
    /// Display order for UI presentation
    /// </summary>
    [Required]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Group/category for organizing metadata
    /// </summary>
    [StringLength(50)]
    public string? MetadataGroup { get; set; }

    // Navigation Properties

    /// <summary>
    /// Media file this metadata belongs to
    /// </summary>
    [ForeignKey(nameof(MediaFileId))]
    public virtual MediaFile MediaFile { get; set; } = null!;
}

/// <summary>
/// Data type classification for metadata values
/// </summary>
public enum MetadataType
{
    /// <summary>
    /// String/text value
    /// </summary>
    String = 1,

    /// <summary>
    /// Integer number
    /// </summary>
    Integer = 2,

    /// <summary>
    /// Decimal number
    /// </summary>
    Decimal = 3,

    /// <summary>
    /// Boolean true/false
    /// </summary>
    Boolean = 4,

    /// <summary>
    /// Date/time value
    /// </summary>
    DateTime = 5,

    /// <summary>
    /// JSON object/array
    /// </summary>
    Json = 6,

    /// <summary>
    /// URL/link
    /// </summary>
    Url = 7
}
