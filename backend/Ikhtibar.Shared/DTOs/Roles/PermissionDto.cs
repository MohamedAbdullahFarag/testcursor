namespace Ikhtibar.Shared.DTOs.Roles;

/// <summary>
/// Data transfer object for permission responses.
/// </summary>
public class PermissionDto
{
    /// <summary>
    /// Unique identifier for the permission.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the permission.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the permission.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Unique code for the permission.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Category/group of the permission.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Type of the permission (e.g., 'read', 'write', 'delete', 'admin').
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Whether the permission is active/enabled.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether this permission is system-defined (cannot be deleted).
    /// </summary>
    public bool IsSystem { get; set; }

    /// <summary>
    /// Priority level of the permission (higher number = higher priority).
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Whether this permission requires additional approval.
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// When the permission was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Who created the permission.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// When the permission was last modified.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Who last modified the permission.
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Version number for optimistic concurrency.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Count of roles that have this permission.
    /// </summary>
    public int RoleCount { get; set; }

    /// <summary>
    /// Gets a display name for the permission.
    /// </summary>
    /// <returns>Display name</returns>
    public string GetDisplayName() => string.IsNullOrEmpty(Description) ? Name : $"{Name} - {Description}";

    /// <summary>
    /// Checks if this permission can be deleted.
    /// </summary>
    /// <returns>True if deletable, false otherwise</returns>
    public bool CanBeDeleted() => !IsSystem && RoleCount == 0;

    /// <summary>
    /// Gets the permission priority level as a string.
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

    /// <summary>
    /// Gets a formatted permission code with category and type.
    /// </summary>
    /// <returns>Formatted permission code</returns>
    public string GetFormattedCode() => $"{Category}.{Type}";

    /// <summary>
    /// Checks if this is a read permission.
    /// </summary>
    /// <returns>True if read permission, false otherwise</returns>
    public bool IsReadPermission() => Type.Equals("read", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if this is a write permission.
    /// </summary>
    /// <returns>True if write permission, false otherwise</returns>
    public bool IsWritePermission() => Type.Equals("write", StringComparison.OrdinalIgnoreCase) || 
                                      Type.Equals("create", StringComparison.OrdinalIgnoreCase) || 
                                      Type.Equals("update", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if this is a delete permission.
    /// </summary>
    /// <returns>True if delete permission, false otherwise</returns>
    public bool IsDeletePermission() => Type.Equals("delete", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if this is an admin permission.
    /// </summary>
    /// <returns>True if admin permission, false otherwise</returns>
    public bool IsAdminPermission() => Type.Equals("admin", StringComparison.OrdinalIgnoreCase);
}
