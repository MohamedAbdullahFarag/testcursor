using System.ComponentModel.DataAnnotations;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Data transfer object for TreeNode entity
/// </summary>
public class TreeNodeDto
{
    /// <summary>
    /// Tree node identifier
    /// </summary>
    public int TreeNodeId { get; set; }

    /// <summary>
    /// Node display name
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for node
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Optional description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Tree node type identifier
    /// </summary>
    public int TreeNodeTypeId { get; set; }

    /// <summary>
    /// Tree node type name
    /// </summary>
    public string TreeNodeTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Parent node identifier
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Materialized path (e.g., -1-4-9-)
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Active flag
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Number of direct children
    /// </summary>
    public int ChildrenCount { get; set; }

    /// <summary>
    /// Tree depth level (calculated from path)
    /// </summary>
    public int Level { get; set; }

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
/// Detailed DTO for TreeNode with children and additional information
/// </summary>
public class TreeNodeDetailDto : TreeNodeDto
{
    /// <summary>
    /// Child nodes (for tree structure display)
    /// </summary>
    public IEnumerable<TreeNodeDto> Children { get; set; } = new List<TreeNodeDto>();

    /// <summary>
    /// Curriculum alignments for this node
    /// </summary>
    public IEnumerable<CurriculumAlignmentDto> CurriculumAlignments { get; set; } = new List<CurriculumAlignmentDto>();

    /// <summary>
    /// Total questions count (including descendants)
    /// </summary>
    public int TotalQuestionsCount { get; set; }
}

/// <summary>
/// DTO for creating a new tree node
/// </summary>
public class CreateTreeNodeDto
{
    /// <summary>
    /// Node display name
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for node
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Optional description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Tree node type identifier
    /// </summary>
    [Required]
    public int TreeNodeTypeId { get; set; }

    /// <summary>
    /// Parent node identifier (null for root nodes)
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Display order (auto-calculated if not provided)
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Active flag
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating an existing tree node
/// </summary>
public class UpdateTreeNodeDto
{
    /// <summary>
    /// Node display name
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for node
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Optional description
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Tree node type identifier
    /// </summary>
    [Required]
    public int TreeNodeTypeId { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Active flag
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for moving a tree node to a new parent
/// </summary>
public class MoveTreeNodeDto
{
    /// <summary>
    /// New parent node identifier (null to move to root)
    /// </summary>
    public int? NewParentId { get; set; }

    /// <summary>
    /// New display order in the target parent
    /// </summary>
    public int NewOrderIndex { get; set; }
}

/// <summary>
/// DTO for tree structure response
/// </summary>
public class TreeStructureDto
{
    /// <summary>
    /// Root nodes of the tree
    /// </summary>
    public IEnumerable<TreeNodeDetailDto> Roots { get; set; } = new List<TreeNodeDetailDto>();

    /// <summary>
    /// Total nodes count in the tree
    /// </summary>
    public int TotalNodesCount { get; set; }

    /// <summary>
    /// Maximum depth of the tree
    /// </summary>
    public int MaxDepth { get; set; }
}

/// <summary>
/// Tree node statistics data
/// </summary>
public class TreeNodeStatistics
{
    /// <summary>
    /// Number of direct children
    /// </summary>
    public int DirectChildrenCount { get; set; }

    /// <summary>
    /// Total number of descendants
    /// </summary>
    public int TotalDescendantsCount { get; set; }

    /// <summary>
    /// Tree depth from this node
    /// </summary>
    public int MaxDepth { get; set; }

    /// <summary>
    /// Level of this node in the tree (0 for root)
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Total questions count (including descendants)
    /// </summary>
    public int TotalQuestionsCount { get; set; }
}
