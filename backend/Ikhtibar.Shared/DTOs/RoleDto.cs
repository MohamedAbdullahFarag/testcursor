namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Data transfer object for role read operations
/// </summary>
public class RoleDto
{
    /// <summary>
    /// Role unique identifier
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Unique role identifier
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable role name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional role description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Flag indicating if this is a system role (non-deletable)
    /// </summary>
    public bool IsSystemRole { get; set; }

    /// <summary>
    /// Flag indicating if role is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Role creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last modification timestamp
    /// </summary>
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// Number of users assigned to this role
    /// </summary>
    public int UserCount { get; set; }
}
