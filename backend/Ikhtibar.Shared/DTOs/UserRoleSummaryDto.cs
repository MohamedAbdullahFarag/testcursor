namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for user role summary information
/// Contains user details with role count summary
/// </summary>
public class UserRoleSummaryDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User's full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Number of roles assigned to this user
    /// </summary>
    public int RoleCount { get; set; }

    /// <summary>
    /// List of role names assigned to user
    /// </summary>
    public IEnumerable<string> RoleNames { get; set; } = new List<string>();

    /// <summary>
    /// When the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the user's roles were last modified
    /// </summary>
    public DateTime? LastRoleModified { get; set; }
}
