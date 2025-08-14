using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Media type lookup entity (e.g., Image, Video, Audio, Document)
/// </summary>
[Table("MediaTypes")]
public class MediaType : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int MediaTypeId { get; set; }

    /// <summary>
    /// Media type name
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Media items that belong to this type
    /// </summary>
    public virtual ICollection<Media> MediaItems { get; set; } = new List<Media>();
}
