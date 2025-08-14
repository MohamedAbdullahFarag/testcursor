using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Login attempts tracking entity for security auditing
/// </summary>
[Table("LoginAttempts")]
public class LoginAttempt
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int LoginAttemptId { get; set; }

    /// <summary>
    /// Username attempted (may not exist)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to Users (NULL for failed logins with invalid username)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// When attempt occurred
    /// </summary>
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Was login successful
    /// </summary>
    [Required]
    public bool Success { get; set; } = false;

    /// <summary>
    /// IP address of client (IPv4/IPv6)
    /// </summary>
    [StringLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// Browser/client user agent
    /// </summary>
    [StringLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Reason for failure if unsuccessful
    /// </summary>
    [StringLength(100)]
    public string? FailureReason { get; set; }

    /// <summary>
    /// Optional geolocation data
    /// </summary>
    [StringLength(100)]
    public string? AttemptLocation { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Soft delete flag
    /// </summary>
    [Required]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Navigation property to User
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}
