using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Tree;

/// <summary>
/// Data transfer object for updating an existing tree node.
/// </summary>
public class UpdateTreeNodeDto
{
    /// <summary>
    /// ID of the tree node to update.
    /// </summary>
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// Name of the tree node.
    /// </summary>
    [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string? Name { get; set; }

    /// <summary>
    /// Description of the tree node.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Type/category of the tree node.
    /// </summary>
    [StringLength(100, ErrorMessage = "Node type cannot exceed 100 characters")]
    public string? NodeType { get; set; }

    /// <summary>
    /// Order/position among siblings.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Order index must be non-negative")]
    public int? OrderIndex { get; set; }

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Whether the node is active/enabled.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Whether the node is visible to users.
    /// </summary>
    public bool? IsVisible { get; set; }

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
    /// Who is updating the node.
    /// </summary>
    [StringLength(100, ErrorMessage = "Modified by cannot exceed 100 characters")]
    public string? ModifiedBy { get; set; }
}
