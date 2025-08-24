using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Roles;

/// <summary>
/// Data transfer object for creating a new permission.
/// </summary>
public class CreatePermissionDto
{
    /// <summary>
    /// Name of the permission.
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "Permission name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the permission.
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Unique code for the permission.
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "Permission code cannot exceed 100 characters")]
    [RegularExpression(@"^[a-z_]+(\.[a-z_]+)*$", ErrorMessage = "Permission code must use dot notation (e.g., 'users.create', 'reports.view')")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Category/group of the permission.
    /// </summary>
    [Required]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Type of the permission (e.g., 'read', 'write', 'delete', 'admin').
    /// </summary>
    [Required]
    [StringLength(20, ErrorMessage = "Type cannot exceed 20 characters")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Whether the permission is active/enabled.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this permission is system-defined (cannot be deleted).
    /// </summary>
    public bool IsSystem { get; set; } = false;

    /// <summary>
    /// Priority level of the permission (higher number = higher priority).
    /// </summary>
    [Range(1, 100, ErrorMessage = "Priority must be between 1 and 100")]
    public int Priority { get; set; } = 50;

    /// <summary>
    /// Whether this permission requires additional approval.
    /// </summary>
    public bool RequiresApproval { get; set; } = false;

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Who is creating the permission.
    /// </summary>
    [StringLength(100, ErrorMessage = "Created by cannot exceed 100 characters")]
    public string? CreatedBy { get; set; }
}
