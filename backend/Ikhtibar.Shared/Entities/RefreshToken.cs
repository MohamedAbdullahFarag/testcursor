using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Refresh token entity for authentication
/// Note: Does not inherit from BaseEntity due to different schema structure
/// </summary>
[Table("RefreshTokens")]
public class RefreshToken
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int RefreshTokenId { get; set; }

    /// <summary>
    /// Hashed refresh token value
    /// </summary>
    [Required]
    [StringLength(256)]
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to Users
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// When token was issued
    /// </summary>
    [Required]
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When token expires
    /// </summary>
    [Required]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When token was revoked
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Hash of token that replaced this one
    /// </summary>
    [StringLength(256)]
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Why was token revoked
    /// </summary>
    [StringLength(500)]
    public string? ReasonRevoked { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User ID who created the record
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// Last modification timestamp
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// User ID who last modified the record
    /// </summary>
    public int? ModifiedBy { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    [Required]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Optimistic concurrency token (for Dapper row versioning)
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Navigation property to User
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
