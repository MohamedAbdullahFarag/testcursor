using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Tree;

/// <summary>
/// Data transfer object for creating a new tree node.
/// </summary>
public class CreateTreeNodeDto
{
    /// <summary>
    /// Name of the tree node.
    /// </summary>
    [Required]
    [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the tree node.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Type/category of the tree node.
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "Node type cannot exceed 100 characters")]
    public string NodeType { get; set; } = string.Empty;

    /// <summary>
    /// ID of the parent node (null for root nodes).
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Order/position among siblings.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Order index must be non-negative")]
    public int OrderIndex { get; set; }

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Whether the node is active/enabled.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the node is visible to users.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Icon or visual representation for the node.
    /// </summary>
    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    /// <summary>
    /// Color theme for the node.
    /// </summary>
    [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
    public string? Color { get; set; }

    /// <summary>
    /// Who is creating the node.
    /// </summary>
    [StringLength(100, ErrorMessage = "Created by cannot exceed 100 characters")]
    public string? CreatedBy { get; set; }
}
