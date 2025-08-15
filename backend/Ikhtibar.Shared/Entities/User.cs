using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// User entity representing system users
/// </summary>
[Table("Users")]
public class User : BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int UserId { get; set; }

    /// <summary>
    /// Login username
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Contact email
    /// </summary>
    [Required]
    [StringLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User first name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User last name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password
    /// </summary>
    [Required]
    [StringLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Optional phone number
    /// </summary>
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Preferred language (e.g., en, ar)
    /// </summary>
    [StringLength(10)]
    public string? PreferredLanguage { get; set; }

    /// <summary>
    /// Flag if account is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Has email been verified
    /// </summary>
    [Required]
    public bool EmailVerified { get; set; } = false;

    /// <summary>
    /// Has phone been verified
    /// </summary>
    [Required]
    public bool PhoneVerified { get; set; } = false;

    /// <summary>
    /// User's unique code (for external systems)
    /// </summary>
    [StringLength(50)]
    public string? Code { get; set; }

    /// <summary>
    /// User's last login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Navigation property for user roles
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// Navigation property for refresh tokens
    /// </summary>
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    /// <summary>
    /// Navigation property for login attempts
    /// </summary>
    public virtual ICollection<LoginAttempt> LoginAttempts { get; set; } = new List<LoginAttempt>();

    /// <summary>
    /// Navigation property for questions created by this user
    /// </summary>
    public virtual ICollection<Question> QuestionsCreated { get; set; } = new List<Question>();

    /// <summary>
    /// Navigation property for answers created by this user
    /// </summary>
    public virtual ICollection<Answer> AnswersCreated { get; set; } = new List<Answer>();

    /// <summary>
    /// Navigation property for media uploaded by this user
    /// </summary>
    public virtual ICollection<Media> MediaUploaded { get; set; } = new List<Media>();

    /// <summary>
    /// Navigation property for exams created by this user
    /// </summary>
    public virtual ICollection<Exam> ExamsCreated { get; set; } = new List<Exam>();
}
