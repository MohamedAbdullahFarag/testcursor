using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Refresh token entity for secure token rotation
/// Stores hashed refresh tokens with expiration and user association
/// </summary>
[Table("RefreshTokens")]
public class RefreshTokens : BaseEntity
{
    /// <summary>
    /// Unique identifier for the refresh token
    /// </summary>
    [Key]
    [Column("RefreshTokenId")]
    public override int Id { get; set; }

    /// <summary>
    /// Hashed refresh token value (never store plain text)
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// User ID associated with this refresh token
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// When the token was issued
    /// </summary>
    [Required]
    public DateTime IssuedAt { get; set; }

    /// <summary>
    /// When the token expires
    /// </summary>
    [Required]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When the token was revoked (if applicable)
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Reason for revocation (if applicable)
    /// </summary>
    [MaxLength(200)]
    public string? RevocationReason { get; set; }

    /// <summary>
    /// Client IP address that requested the token
    /// </summary>
    [MaxLength(45)] // IPv6 max length
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// User agent that requested the token
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Whether the token is currently active
    /// </summary>
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

    /// <summary>
    /// Whether the token has expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    /// <summary>
    /// Whether the token has been revoked
    /// </summary>
    public bool IsRevoked => RevokedAt != null;

    // Navigation property removed for Dapper compatibility
}
