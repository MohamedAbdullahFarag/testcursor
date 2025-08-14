namespace Ikhtibar.API.Models;

/// <summary>
/// User information retrieved from OIDC provider
/// </summary>
public class OidcUserInfo
{
    /// <summary>
    /// Subject identifier (unique user ID from OIDC provider)
    /// </summary>
    public string Sub { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Whether email is verified
    /// </summary>
    public bool EmailVerified { get; set; }

    /// <summary>
    /// User's given name (first name)
    /// </summary>
    public string? GivenName { get; set; }

    /// <summary>
    /// User's family name (last name)
    /// </summary>
    public string? FamilyName { get; set; }

    /// <summary>
    /// User's full name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// User's preferred username
    /// </summary>
    public string? PreferredUsername { get; set; }

    /// <summary>
    /// User's phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Whether phone number is verified
    /// </summary>
    public bool PhoneNumberVerified { get; set; }

    /// <summary>
    /// User's profile picture URL
    /// </summary>
    public string? Picture { get; set; }

    /// <summary>
    /// User's locale preference
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// User's timezone
    /// </summary>
    public string? Zoneinfo { get; set; }

    /// <summary>
    /// Time when user information was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
