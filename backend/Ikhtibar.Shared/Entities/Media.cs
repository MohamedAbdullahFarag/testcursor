using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Media entity for file attachments
/// </summary>
[Table("Media")]
public class Media
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int MediaId { get; set; }

    /// <summary>
    /// Resource URL
    /// </summary>
    [Required]
    [StringLength(2083)]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to MediaTypes
    /// </summary>
    [Required]
    public int MediaTypeId { get; set; }

    /// <summary>
    /// User ID who uploaded the media
    /// </summary>
    [Required]
    public int UploadedBy { get; set; }

    /// <summary>
    /// Upload timestamp
    /// </summary>
    [Required]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optimistic concurrency token
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Navigation property to MediaType
    /// </summary>
    [ForeignKey("MediaTypeId")]
    public virtual MediaType MediaType { get; set; } = null!;

    /// <summary>
    /// Navigation property to User who uploaded
    /// </summary>
    [ForeignKey("UploadedBy")]
    public virtual User UploadedByUser { get; set; } = null!;

    /// <summary>
    /// Navigation property to QuestionMedia
    /// </summary>
    public virtual ICollection<QuestionMedia> QuestionMedia { get; set; } = new List<QuestionMedia>();

    /// <summary>
    /// Navigation property to AnswerMedia
    /// </summary>
    public virtual ICollection<AnswerMedia> AnswerMedia { get; set; } = new List<AnswerMedia>();
}
