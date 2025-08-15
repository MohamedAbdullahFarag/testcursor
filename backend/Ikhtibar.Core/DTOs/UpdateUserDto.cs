using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Core.DTOs;

/// <summary>
/// DTO for updating an existing user
/// Following SRP: ONLY user update data
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// User's username
    /// </summary>
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string? Username { get; set; }

    /// <summary>
    /// User's first name
    /// </summary>
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string? FirstName { get; set; }

    /// <summary>
    /// User's last name
    /// </summary>
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string? LastName { get; set; }

    /// <summary>
    /// User's phone number
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's preferred language
    /// </summary>
    [StringLength(10, ErrorMessage = "Language code cannot exceed 10 characters")]
    public string? PreferredLanguage { get; set; }

    /// <summary>
    /// Whether the user is active
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Whether the user's email is verified
    /// </summary>
    public bool? EmailVerified { get; set; }

    /// <summary>
    /// Whether the user's phone is verified
    /// </summary>
    public bool? PhoneVerified { get; set; }
}
