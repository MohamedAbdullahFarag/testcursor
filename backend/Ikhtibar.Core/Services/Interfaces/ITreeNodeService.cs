using Ikhtibar.Shared.DTOs;

using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for TreeNode management operations
/// Following SRP: ONLY tree node business logic operations
/// </summary>
public interface ITreeNodeService
{
    /// <summary>
    /// Gets all tree nodes
    /// </summary>
    /// <returns>Collection of tree nodes</returns>
    Task<IEnumerable<TreeNodeDto>> GetAllAsync();

    /// <summary>
    /// Gets root nodes (nodes without parent)
    /// </summary>
    /// <returns>Collection of root tree nodes</returns>
    Task<IEnumerable<TreeNodeDto>> GetRootNodesAsync();

    /// <summary>
    /// Gets direct children of a parent node
    /// </summary>
    /// <param name="parentId">Parent node identifier</param>
    /// <returns>Collection of child nodes</returns>
    Task<IEnumerable<TreeNodeDto>> GetChildrenAsync(int parentId);

    /// <summary>
    /// Gets a tree node by ID with detailed information
    /// </summary>
    /// <param name="treeNodeId">Tree node identifier</param>
    /// <returns>Tree node detail data if found, null otherwise</returns>
    Task<TreeNodeDetailDto?> GetTreeNodeAsync(int treeNodeId);

    /// <summary>
    /// Gets a tree node by code
    /// </summary>
    /// <param name="code">Tree node code</param>
    /// <returns>Tree node data if found, null otherwise</returns>
    Task<TreeNodeDto?> GetTreeNodeByCodeAsync(string code);

    /// <summary>
    /// Gets tree structure from a root node with specified depth
    /// </summary>
    /// <param name="rootId">Root node identifier</param>
    /// <param name="levels">Number of levels to include (default: 1)</param>
    /// <returns>Tree structure with nested children</returns>
    Task<TreeNodeDetailDto?> GetTreeStructureAsync(int rootId, int levels = 1);

    /// <summary>
    /// Gets all ancestors of a node
    /// </summary>
    /// <param name="nodeId">Node identifier</param>
    /// <returns>Collection of ancestor nodes</returns>
    Task<IEnumerable<TreeNodeDto>> GetAncestorsAsync(int nodeId);

    /// <summary>
    /// Gets all descendants of a node
    /// </summary>
    /// <param name="nodeId">Node identifier</param>
    /// <returns>Collection of descendant nodes</returns>
    Task<IEnumerable<TreeNodeDto>> GetDescendantsAsync(int nodeId);

    /// <summary>
    /// Gets nodes by tree node type
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type identifier</param>
    /// <returns>Collection of nodes of the specified type</returns>
    Task<IEnumerable<TreeNodeDto>> GetNodesByTypeAsync(int treeNodeTypeId);

    /// <summary>
    /// Creates a new tree node
    /// </summary>
    /// <param name="createDto">Tree node creation data</param>
    /// <returns>Created tree node data</returns>
    Task<TreeNodeDto> CreateTreeNodeAsync(CreateTreeNodeDto createDto);

    /// <summary>
    /// Updates an existing tree node
    /// </summary>
    /// <param name="treeNodeId">Tree node identifier</param>
    /// <param name="updateDto">Tree node update data</param>
    /// <returns>Updated tree node data</returns>
    Task<TreeNodeDto> UpdateTreeNodeAsync(int treeNodeId, UpdateTreeNodeDto updateDto);

    /// <summary>
    /// Moves a tree node to a new parent
    /// </summary>
    /// <param name="treeNodeId">Tree node identifier</param>
    /// <param name="moveDto">Move operation data</param>
    /// <returns>Updated tree node data</returns>
    Task<TreeNodeDto> MoveTreeNodeAsync(int treeNodeId, MoveTreeNodeDto moveDto);

    /// <summary>
    /// Reorders nodes within the same parent
    /// </summary>
    /// <param name="parentId">Parent node identifier</param>
    /// <param name="nodeOrders">Dictionary of node ID to new order index</param>
    /// <returns>True if reordered successfully, false otherwise</returns>
    Task<bool> ReorderNodesAsync(int parentId, IDictionary<int, int> nodeOrders);

    /// <summary>
    /// Deletes a tree node (soft delete)
    /// </summary>
    /// <param name="treeNodeId">Tree node identifier</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteTreeNodeAsync(int treeNodeId);

    /// <summary>
    /// Checks if tree node exists
    /// </summary>
    /// <param name="treeNodeId">Tree node identifier</param>
    /// <returns>True if exists, false otherwise</returns>
    Task<bool> ExistsAsync(int treeNodeId);

    /// <summary>
    /// Checks if tree node has children
    /// </summary>
    /// <param name="treeNodeId">Tree node identifier</param>
    /// <returns>True if has children, false otherwise</returns>
    Task<bool> HasChildrenAsync(int treeNodeId);

    /// <summary>
    /// Gets tree statistics for a node
    /// </summary>
    /// <param name="nodeId">Node identifier</param>
    /// <returns>Tree statistics data</returns>
    Task<TreeNodeStatistics> GetStatisticsAsync(int nodeId);

    /// <summary>
    /// Gets the complete tree structure
    /// </summary>
    /// <returns>Complete tree structure with all root nodes and their children</returns>
    Task<TreeStructureDto> GetCompleteTreeAsync();
}
