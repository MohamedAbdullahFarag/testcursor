namespace Ikhtibar.Shared.DTOs.Roles;

/// <summary>
/// Data transfer object for role responses.
/// </summary>
public class RoleDto
{
    /// <summary>
    /// Unique identifier for the role.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the role.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the role.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Unique code for the role.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Whether the role is active/enabled.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the role is visible to users.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Whether this role is system-defined (cannot be deleted).
    /// </summary>
    public bool IsSystem { get; set; }

    /// <summary>
    /// Priority level of the role (higher number = higher priority).
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Maximum number of users that can have this role.
    /// </summary>
    public int? MaxUsers { get; set; }

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// When the role was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Who created the role.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// When the role was last modified.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Who last modified the role.
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Version number for optimistic concurrency.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Count of users assigned to this role.
    /// </summary>
    public int UserCount { get; set; }

    /// <summary>
    /// Count of permissions assigned to this role.
    /// </summary>
    public int PermissionCount { get; set; }

    /// <summary>
    /// Collection of permissions assigned to this role.
    /// </summary>
    public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();

    /// <summary>
    /// Gets a display name for the role.
    /// </summary>
    /// <returns>Display name</returns>
    public string GetDisplayName() => string.IsNullOrEmpty(Description) ? Name : $"{Name} - {Description}";

    /// <summary>
    /// Checks if this role can be deleted.
    /// </summary>
    /// <returns>True if deletable, false otherwise</returns>
    public bool CanBeDeleted() => !IsSystem && UserCount == 0;

    /// <summary>
    /// Checks if this role can accept more users.
    /// </summary>
    /// <returns>True if more users can be assigned, false otherwise</returns>
    public bool CanAcceptMoreUsers() => !MaxUsers.HasValue || UserCount < MaxUsers.Value;

    /// <summary>
    /// Gets the role priority level as a string.
    /// </summary>
    /// <returns>Priority level string</returns>
    public string GetPriorityLevel()
    {
        return Priority switch
        {
            >= 80 => "Critical",
            >= 60 => "High",
            >= 40 => "Medium",
            >= 20 => "Low",
            _ => "Minimal"
        };
    }
}
