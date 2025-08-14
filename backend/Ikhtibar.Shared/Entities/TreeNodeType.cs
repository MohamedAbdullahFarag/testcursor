using System.ComponentModel.DataAnnotations;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Tree node type lookup entity (e.g., Category, Topic, Subtopic)
/// </summary>
public class TreeNodeType
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public int TreeNodeTypeId { get; set; }

    /// <summary>
    /// Tree node type name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tree nodes that belong to this type
    /// </summary>
    public virtual ICollection<TreeNode> TreeNodes { get; set; } = new List<TreeNode>();
}
