using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Core.DTOs;

/// <summary>
/// DTO for creating a new user
/// Following SRP: ONLY user creation data
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// User's email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's username
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User's first name
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number (optional)
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's preferred language
    /// </summary>
    [StringLength(10, ErrorMessage = "Language code cannot exceed 10 characters")]
    public string PreferredLanguage { get; set; } = "en";

    /// <summary>
    /// Whether the user is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Initial roles to assign to the user
    /// </summary>
    public List<int>? RoleIds { get; set; }
}
