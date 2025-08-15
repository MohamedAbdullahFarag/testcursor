namespace Ikhtibar.Core.DTOs;

/// <summary>
/// DTO for role responses
/// Following SRP: ONLY role response data
/// </summary>
public class RoleDto
{
    /// <summary>
    /// Role's unique identifier
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Role's unique code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Role's display name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Role's description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the role is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether this is a system role
    /// </summary>
    public bool IsSystemRole { get; set; }

    /// <summary>
    /// When the role was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the role was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
