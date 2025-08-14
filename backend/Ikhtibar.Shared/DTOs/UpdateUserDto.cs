using System.ComponentModel.DataAnnotations;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Data transfer object for updating an existing user
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// Login username
    /// </summary>
    [StringLength(100, MinimumLength = 3)]
    public string? Username { get; set; }

    /// <summary>
    /// Contact email
    /// </summary>
    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    /// <summary>
    /// User first name
    /// </summary>
    [StringLength(100, MinimumLength = 1)]
    public string? FirstName { get; set; }

    /// <summary>
    /// User last name
    /// </summary>
    [StringLength(100, MinimumLength = 1)]
    public string? LastName { get; set; }

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
    public bool? IsActive { get; set; }

    /// <summary>
    /// Flag if email should be marked as verified
    /// </summary>
    public bool? EmailVerified { get; set; }

    /// <summary>
    /// Flag if phone should be marked as verified
    /// </summary>
    public bool? PhoneVerified { get; set; }
}
