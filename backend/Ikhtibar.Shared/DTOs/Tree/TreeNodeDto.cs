namespace Ikhtibar.Shared.DTOs.Tree;

/// <summary>
/// Data transfer object for tree node responses.
/// </summary>
public class TreeNodeDto
{
    /// <summary>
    /// Unique identifier for the tree node.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the tree node.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the tree node.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Type/category of the tree node.
    /// </summary>
    public string NodeType { get; set; } = string.Empty;

    /// <summary>
    /// Materialized path from root to this node.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Level/depth of the node in the tree (0 for root).
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Order/position among siblings.
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// ID of the parent node (null for root nodes).
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Whether the node is active/enabled.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the node is visible to users.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Icon or visual representation for the node.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Color theme for the node.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// When the node was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Who created the node.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// When the node was last modified.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Who last modified the node.
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Version number for optimistic concurrency.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Collection of child nodes.
    /// </summary>
    public List<TreeNodeDto> Children { get; set; } = new List<TreeNodeDto>();

    /// <summary>
    /// Gets the full path including the current node.
    /// </summary>
    /// <returns>Full path string</returns>
    public string GetFullPath() => $"{Path}/{Id}";

    /// <summary>
    /// Gets the parent path (path without current node).
    /// </summary>
    /// <returns>Parent path string</returns>
    public string GetParentPath() => Path;

    /// <summary>
    /// Checks if this node is a root node.
    /// </summary>
    /// <returns>True if root, false otherwise</returns>
    public bool IsRoot() => ParentId == null;

    /// <summary>
    /// Checks if this node is a leaf node (no children).
    /// </summary>
    /// <returns>True if leaf, false otherwise</returns>
    public bool IsLeaf() => !Children.Any();

    /// <summary>
    /// Gets the number of children.
    /// </summary>
    /// <returns>Child count</returns>
    public int GetChildCount() => Children.Count;

    /// <summary>
    /// Gets the total number of descendants.
    /// </summary>
    /// <returns>Total descendant count</returns>
    public int GetDescendantCount() => Children.Sum(c => 1 + c.GetDescendantCount());
}
