using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;

using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for TreeNode entity operations
/// Provides hierarchical tree operations with materialized path pattern
/// </summary>
public interface ITreeNodeRepository
{
    /// <summary>
    /// Get tree node by ID
    /// </summary>
    /// <param name="treeNodeId">Tree node ID</param>
    /// <returns>Tree node if found, null otherwise</returns>
    Task<TreeNode?> GetByIdAsync(int treeNodeId);

    /// <summary>
    /// Get tree node by code
    /// </summary>
    /// <param name="code">Tree node code</param>
    /// <returns>Tree node if found, null otherwise</returns>
    Task<TreeNode?> GetByCodeAsync(string code);

    /// <summary>
    /// Get all tree nodes
    /// </summary>
    /// <returns>Collection of all tree nodes</returns>
    Task<IEnumerable<TreeNode>> GetAllAsync();

    /// <summary>
    /// Get root nodes (nodes without parent)
    /// </summary>
    /// <returns>Collection of root tree nodes ordered by OrderIndex</returns>
    Task<IEnumerable<TreeNode>> GetRootNodesAsync();

    /// <summary>
    /// Get direct children of a parent node
    /// </summary>
    /// <param name="parentId">Parent node ID</param>
    /// <returns>Collection of child nodes ordered by OrderIndex</returns>
    Task<IEnumerable<TreeNode>> GetChildrenAsync(int parentId);

    /// <summary>
    /// Get all descendants of a node using materialized path
    /// </summary>
    /// <param name="nodeId">Node ID to get descendants for</param>
    /// <returns>Collection of descendant nodes</returns>
    Task<IEnumerable<TreeNode>> GetDescendantsAsync(int nodeId);

    /// <summary>
    /// Get all ancestors of a node using materialized path
    /// </summary>
    /// <param name="nodeId">Node ID to get ancestors for</param>
    /// <returns>Collection of ancestor nodes ordered by path</returns>
    Task<IEnumerable<TreeNode>> GetAncestorsAsync(int nodeId);

    /// <summary>
    /// Get nodes by path pattern (for subtree queries)
    /// </summary>
    /// <param name="pathQuery">Path pattern to search</param>
    /// <returns>Collection of nodes matching the path pattern</returns>
    Task<IEnumerable<TreeNode>> GetByPathAsync(string pathQuery);

    /// <summary>
    /// Get nodes by tree node type
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type ID</param>
    /// <returns>Collection of nodes of the specified type</returns>
    Task<IEnumerable<TreeNode>> GetByTypeAsync(int treeNodeTypeId);

    /// <summary>
    /// Create a new tree node
    /// </summary>
    /// <param name="treeNode">Tree node to create</param>
    /// <returns>Created tree node with generated ID</returns>
    Task<TreeNode> CreateAsync(TreeNode treeNode);

    /// <summary>
    /// Update an existing tree node
    /// </summary>
    /// <param name="treeNode">Tree node to update</param>
    /// <returns>Updated tree node</returns>
    Task<TreeNode> UpdateAsync(TreeNode treeNode);

    /// <summary>
    /// Delete a tree node (soft delete)
    /// </summary>
    /// <param name="treeNodeId">Tree node ID to delete</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteAsync(int treeNodeId);

    /// <summary>
    /// Move a node to a new parent (updates paths of node and descendants)
    /// </summary>
    /// <param name="nodeId">Node ID to move</param>
    /// <param name="newParentId">New parent ID (null for root)</param>
    /// <param name="newOrderIndex">New order index</param>
    /// <returns>True if moved successfully, false otherwise</returns>
    Task<bool> MoveNodeAsync(int nodeId, int? newParentId, int newOrderIndex);

    /// <summary>
    /// Update paths for all descendants when a node is moved
    /// </summary>
    /// <param name="oldPath">Old materialized path</param>
    /// <param name="newPath">New materialized path</param>
    /// <returns>True if updated successfully, false otherwise</returns>
    Task<bool> UpdatePathsAsync(string oldPath, string newPath);

    /// <summary>
    /// Get the maximum order index for children of a parent
    /// </summary>
    /// <param name="parentId">Parent ID (null for root level)</param>
    /// <returns>Maximum order index</returns>
    Task<int> GetMaxOrderIndexAsync(int? parentId);

    /// <summary>
    /// Reorder nodes within the same parent
    /// </summary>
    /// <param name="parentId">Parent ID</param>
    /// <param name="nodeOrders">Dictionary of node ID to new order index</param>
    /// <returns>True if reordered successfully, false otherwise</returns>
    Task<bool> ReorderNodesAsync(int parentId, IDictionary<int, int> nodeOrders);

    /// <summary>
    /// Check if node exists
    /// </summary>
    /// <param name="treeNodeId">Tree node ID to check</param>
    /// <returns>True if exists, false otherwise</returns>
    Task<bool> ExistsAsync(int treeNodeId);

    /// <summary>
    /// Check if node has children
    /// </summary>
    /// <param name="treeNodeId">Tree node ID to check</param>
    /// <returns>True if has children, false otherwise</returns>
    Task<bool> HasChildrenAsync(int treeNodeId);

    /// <summary>
    /// Get tree statistics for a node
    /// </summary>
    /// <param name="nodeId">Node ID to get statistics for</param>
    /// <returns>Tree statistics including depth, children count, etc.</returns>
    Task<TreeNodeStatistics> GetStatisticsAsync(int nodeId);
}
