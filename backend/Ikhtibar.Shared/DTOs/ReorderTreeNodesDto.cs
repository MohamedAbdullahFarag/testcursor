using System.ComponentModel.DataAnnotations;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// DTO for reordering tree nodes within a parent
/// </summary>
public class ReorderTreeNodesDto
{
    /// <summary>
    /// Dictionary mapping tree node ID to new order index
    /// Key: TreeNodeId, Value: New OrderIndex
    /// </summary>
    [Required]
    public required IDictionary<int, int> NodeOrders { get; set; } = new Dictionary<int, int>();
}
