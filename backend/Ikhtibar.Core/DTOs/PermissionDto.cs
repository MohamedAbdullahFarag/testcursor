using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Core.DTOs;

/// <summary>
/// Data transfer object for Permission entity
/// </summary>
public class PermissionDto
{
    /// <summary>
    /// Permission ID
    /// </summary>
    public int PermissionId { get; set; }

    /// <summary>
    /// Permission code (unique identifier)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Permission display name
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Permission description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Permission category for grouping
    /// </summary>
    [StringLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Whether the permission is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the permission is a system permission
    /// </summary>
    public bool IsSystemPermission { get; set; } = false;

    /// <summary>
    /// When the permission was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the permission was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new permission
/// </summary>
public class CreatePermissionDto
{
    /// <summary>
    /// Permission code (unique identifier)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Permission display name
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Permission description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Permission category for grouping
    /// </summary>
    [StringLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Whether the permission is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the permission is a system permission
    /// </summary>
    public bool IsSystemPermission { get; set; } = false;
}

/// <summary>
    /// Data transfer object for updating an existing permission
    /// </summary>
public class UpdatePermissionDto
{
    /// <summary>
    /// Permission display name
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Permission description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Permission category for grouping
    /// </summary>
    [StringLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Whether the permission is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
