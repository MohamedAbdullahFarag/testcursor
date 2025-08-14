namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Data transfer object for user read operations
/// </summary>
public class UserDto
{
    /// <summary>
    /// User unique identifier
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Login username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Contact email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Optional phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Preferred language (e.g., en, ar)
    /// </summary>
    public string? PreferredLanguage { get; set; }

    /// <summary>
    /// Flag indicating if account is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Flag indicating if email is verified
    /// </summary>
    public bool EmailVerified { get; set; }

    /// <summary>
    /// Flag indicating if phone is verified
    /// </summary>
    public bool PhoneVerified { get; set; }

    /// <summary>
    /// Account creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last modification timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// User's roles
    /// </summary>
    public List<RoleDto>? Roles { get; set; }
}
