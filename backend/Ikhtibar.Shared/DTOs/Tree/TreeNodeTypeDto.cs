namespace Ikhtibar.Shared.DTOs.Tree;

/// <summary>
/// Data transfer object for tree node type responses.
/// </summary>
public class TreeNodeTypeDto
{
    /// <summary>
    /// Unique identifier for the tree node type.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the tree node type.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for the tree node type.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Description of the tree node type.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Icon or visual representation for the type.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Color theme for the type.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Whether this type allows children.
    /// </summary>
    public bool AllowsChildren { get; set; }

    /// <summary>
    /// Maximum number of children allowed.
    /// </summary>
    public int? MaxChildren { get; set; }

    /// <summary>
    /// Maximum depth allowed for this type.
    /// </summary>
    public int? MaxDepth { get; set; }

    /// <summary>
    /// Whether this type is system-defined (cannot be deleted).
    /// </summary>
    public bool IsSystem { get; set; }

    /// <summary>
    /// Whether this type is active/enabled.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether this type is visible to users.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Additional metadata stored as JSON.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// When the type was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Who created the type.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// When the type was last modified.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Who last modified the type.
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Version number for optimistic concurrency.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Count of tree nodes using this type.
    /// </summary>
    public int NodeCount { get; set; }

    /// <summary>
    /// Gets a display name for the type.
    /// </summary>
    /// <returns>Display name</returns>
    public string GetDisplayName() => string.IsNullOrEmpty(Description) ? Name : $"{Name} - {Description}";

    /// <summary>
    /// Checks if this type can have children.
    /// </summary>
    /// <returns>True if children are allowed, false otherwise</returns>
    public bool CanHaveChildren() => AllowsChildren && (!MaxChildren.HasValue || MaxChildren.Value > 0);

    /// <summary>
    /// Checks if this type can be deleted.
    /// </summary>
    /// <returns>True if deletable, false otherwise</returns>
    public bool CanBeDeleted() => !IsSystem && NodeCount == 0;
}
