using System.ComponentModel.DataAnnotations;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Data transfer object for TreeNodeType entity
/// </summary>
public class TreeNodeTypeDto
{
    /// <summary>
    /// Tree node type identifier
    /// </summary>
    public int TreeNodeTypeId { get; set; }

    /// <summary>
    /// Type name (e.g., "Subject", "Chapter", "Topic")
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the tree node type
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Color code for UI representation (hexadecimal)
    /// </summary>
    [StringLength(7)]
    public string? ColorCode { get; set; }

    /// <summary>
    /// Icon name for UI representation
    /// </summary>
    [StringLength(50)]
    public string? IconName { get; set; }

    /// <summary>
    /// Display order for UI organization
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Whether this type can have children
    /// </summary>
    public bool AllowsChildren { get; set; } = true;

    /// <summary>
    /// Maximum allowed depth for this type (null = unlimited)
    /// </summary>
    public int? MaxDepth { get; set; }

    /// <summary>
    /// Whether this type is active and can be used
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last modification timestamp
    /// </summary>
    public DateTime? ModifiedAt { get; set; }
}

/// <summary>
/// DTO for creating a new tree node type
/// </summary>
public class CreateTreeNodeTypeDto
{
    /// <summary>
    /// Type name (e.g., "Subject", "Chapter", "Topic")
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the tree node type
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Color code for UI representation (hexadecimal)
    /// </summary>
    [StringLength(7)]
    public string? ColorCode { get; set; }

    /// <summary>
    /// Icon name for UI representation
    /// </summary>
    [StringLength(50)]
    public string? IconName { get; set; }

    /// <summary>
    /// Display order for UI organization
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Whether this type can have children
    /// </summary>
    public bool AllowsChildren { get; set; } = true;

    /// <summary>
    /// Maximum allowed depth for this type (null = unlimited)
    /// </summary>
    public int? MaxDepth { get; set; }

    /// <summary>
    /// Whether this type is active and can be used
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating an existing tree node type
/// </summary>
public class UpdateTreeNodeTypeDto
{
    /// <summary>
    /// Type name (e.g., "Subject", "Chapter", "Topic")
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the tree node type
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Color code for UI representation (hexadecimal)
    /// </summary>
    [StringLength(7)]
    public string? ColorCode { get; set; }

    /// <summary>
    /// Icon name for UI representation
    /// </summary>
    [StringLength(50)]
    public string? IconName { get; set; }

    /// <summary>
    /// Display order for UI organization
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Whether this type can have children
    /// </summary>
    public bool AllowsChildren { get; set; } = true;

    /// <summary>
    /// Maximum allowed depth for this type (null = unlimited)
    /// </summary>
    public int? MaxDepth { get; set; }

    /// <summary>
    /// Whether this type is active and can be used
    /// </summary>
    public bool IsActive { get; set; } = true;
}
