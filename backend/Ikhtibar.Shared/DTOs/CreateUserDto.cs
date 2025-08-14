using System.ComponentModel.DataAnnotations;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Data transfer object for creating a new user
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// Login username
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Contact email
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User first name
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User last name
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User password
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Optional phone number
    /// </summary>
    [StringLength(20)]
    [Phone]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Preferred language (e.g., en, ar)
    /// </summary>
    [StringLength(10)]
    public string? PreferredLanguage { get; set; }

    /// <summary>
    /// Flag if account should be active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Initial roles to assign to the user
    /// </summary>
    public IEnumerable<int>? RoleIds { get; set; }
}
