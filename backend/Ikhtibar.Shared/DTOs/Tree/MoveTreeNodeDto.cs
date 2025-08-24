using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs.Tree;

/// <summary>
/// Data transfer object for moving a tree node to a new parent.
/// </summary>
public class MoveTreeNodeDto
{
    /// <summary>
    /// ID of the node to move.
    /// </summary>
    [Required]
    public int NodeId { get; set; }

    /// <summary>
    /// ID of the new parent node (null for root level).
    /// </summary>
    public int? NewParentId { get; set; }

    /// <summary>
    /// New order index among siblings.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Order index must be non-negative")]
    public int NewOrderIndex { get; set; }

    /// <summary>
    /// Who is performing the move operation.
    /// </summary>
    [StringLength(100, ErrorMessage = "Modified by cannot exceed 100 characters")]
    public string? ModifiedBy { get; set; }
}
