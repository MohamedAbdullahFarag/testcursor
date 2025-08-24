using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Tree;

/// <summary>
/// Data transfer object for creating a new tree node type.
/// </summary>
public class CreateTreeNodeTypeDto
{
    /// <summary>
    /// Name of the tree node type.
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for the tree node type.
    /// </summary>
    [Required]
    [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Code can only contain letters, numbers, hyphens, and underscores")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Description of the tree node type.
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Icon or visual representation for the type.
    /// </summary>
    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    /// <summary>
    /// Color theme for the type.
    /// </summary>
    [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
    public string? Color { get; set; }

    /// <summary>
    /// Whether this type allows children.
    /// </summary>
    public bool AllowsChildren { get; set; } = true;

    /// <summary>
    /// Maximum number of children allowed.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maximum children must be non-negative")]
    public int? MaxChildren { get; set; }

    /// <summary>
    /// Maximum depth allowed for this type.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maximum depth must be non-negative")]
    public int? MaxDepth { get; set; }

    /// <summary>
    /// Whether this type is system-defined (cannot be deleted).
    /// </summary>
    public bool IsSystem { get; set; } = false;

    /// <summary>
    /// Whether this type is active/enabled.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this type is visible to users.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Who is creating the type.
    /// </summary>
    [StringLength(100, ErrorMessage = "Created by cannot exceed 100 characters")]
    public string? CreatedBy { get; set; }
}
