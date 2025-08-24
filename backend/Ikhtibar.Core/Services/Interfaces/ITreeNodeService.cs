using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for managing hierarchical tree structures.
/// Provides operations for creating, updating, and traversing tree nodes.
/// </summary>
public interface ITreeNodeService
{
    /// <summary>
    /// Creates a new tree node.
    /// </summary>
    /// <param name="node">The node to create</param>
    /// <returns>The created node with generated ID</returns>
    Task<TreeNode> CreateAsync(TreeNode node);

    /// <summary>
    /// Gets a tree node by its ID.
    /// </summary>
    /// <param name="id">The node ID</param>
    /// <returns>The node if found, null otherwise</returns>
    Task<TreeNode?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all tree nodes.
    /// </summary>
    /// <returns>Collection of all tree nodes</returns>
    Task<IEnumerable<TreeNode>> GetAllAsync();

    /// <summary>
    /// Gets all root nodes (nodes without parent).
    /// </summary>
    /// <returns>Collection of root nodes</returns>
    Task<IEnumerable<TreeNode>> GetRootNodesAsync();

    /// <summary>
    /// Gets all children of a specific node.
    /// </summary>
    /// <param name="parentId">The parent node ID</param>
    /// <returns>Collection of child nodes</returns>
    Task<IEnumerable<TreeNode>> GetChildrenAsync(int parentId);

    /// <summary>
    /// Gets the complete subtree starting from a specific node.
    /// </summary>
    /// <param name="nodeId">The root node ID for the subtree</param>
    /// <returns>The complete subtree structure</returns>
    Task<TreeNode> GetSubtreeAsync(int nodeId);

    /// <summary>
    /// Gets the path from root to a specific node.
    /// </summary>
    /// <param name="nodeId">The target node ID</param>
    /// <returns>Ordered collection of nodes from root to target</returns>
    Task<IEnumerable<TreeNode>> GetPathToNodeAsync(int nodeId);

    /// <summary>
    /// Updates an existing tree node.
    /// </summary>
    /// <param name="node">The updated node data</param>
    /// <returns>The updated node</returns>
    Task<TreeNode> UpdateAsync(TreeNode node);

    /// <summary>
    /// Deletes a tree node and optionally its children.
    /// </summary>
    /// <param name="id">The node ID to delete</param>
    /// <param name="deleteChildren">Whether to delete child nodes</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteAsync(int id, bool deleteChildren = false);

    /// <summary>
    /// Moves a node to a new parent.
    /// </summary>
    /// <param name="nodeId">The node to move</param>
    /// <param name="newParentId">The new parent ID (null for root)</param>
    /// <returns>The updated node</returns>
    Task<TreeNode> MoveNodeAsync(int nodeId, int? newParentId);

    /// <summary>
    /// Gets all nodes of a specific type.
    /// </summary>
    /// <param name="nodeType">The node type to filter by</param>
    /// <returns>Collection of nodes of the specified type</returns>
    Task<IEnumerable<TreeNode>> GetByTypeAsync(string nodeType);

    /// <summary>
    /// Searches for nodes by name or description.
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <returns>Collection of matching nodes</returns>
    Task<IEnumerable<TreeNode>> SearchAsync(string searchTerm);

    /// <summary>
    /// Gets the depth level of a node in the tree.
    /// </summary>
    /// <param name="nodeId">The node ID</param>
    /// <returns>The depth level (0 for root)</returns>
    Task<int> GetNodeDepthAsync(int nodeId);

    /// <summary>
    /// Gets all nodes at a specific depth level.
    /// </summary>
    /// <param name="depth">The depth level</param>
    /// <returns>Collection of nodes at the specified depth</returns>
    Task<IEnumerable<TreeNode>> GetNodesAtDepthAsync(int depth);

    /// <summary>
    /// Reorders children of a parent node.
    /// </summary>
    /// <param name="parentId">The parent node ID</param>
    /// <param name="childOrder">Ordered list of child IDs</param>
    /// <returns>True if reordering was successful</returns>
    Task<bool> ReorderChildrenAsync(int parentId, IEnumerable<int> childOrder);

    /// <summary>
    /// Gets a tree node by its ID (alias for GetByIdAsync).
    /// </summary>
    /// <param name="id">The node ID</param>
    /// <returns>The node if found, null otherwise</returns>
    Task<TreeNode?> GetTreeNodeAsync(int id);

    /// <summary>
    /// Gets a tree node by its code.
    /// </summary>
    /// <param name="code">The node code</param>
    /// <returns>The node if found, null otherwise</returns>
    Task<TreeNode?> GetTreeNodeByCodeAsync(string code);

    /// <summary>
    /// Checks if a tree node exists.
    /// </summary>
    /// <param name="id">The node ID</param>
    /// <returns>True if the node exists, false otherwise</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Gets all ancestors of a node.
    /// </summary>
    /// <param name="nodeId">The node ID</param>
    /// <returns>Collection of ancestor nodes</returns>
    Task<IEnumerable<TreeNode>> GetAncestorsAsync(int nodeId);

    /// <summary>
    /// Gets all descendants of a node.
    /// </summary>
    /// <param name="nodeId">The node ID</param>
    /// <returns>Collection of descendant nodes</returns>
    Task<IEnumerable<TreeNode>> GetDescendantsAsync(int nodeId);

    /// <summary>
    /// Gets tree structure up to specified levels.
    /// </summary>
    /// <param name="nodeId">The root node ID</param>
    /// <param name="levels">Number of levels to retrieve</param>
    /// <returns>Tree structure</returns>
    Task<TreeNode> GetTreeStructureAsync(int nodeId, int levels);

    /// <summary>
    /// Gets the complete tree structure.
    /// </summary>
    /// <returns>Complete tree structure</returns>
    Task<TreeNode> GetCompleteTreeAsync();

    /// <summary>
    /// Gets nodes by type ID.
    /// </summary>
    /// <param name="typeId">The type ID</param>
    /// <returns>Collection of nodes of the specified type</returns>
    Task<IEnumerable<TreeNode>> GetNodesByTypeAsync(int typeId);

    /// <summary>
    /// Gets statistics for a node.
    /// </summary>
    /// <param name="nodeId">The node ID</param>
    /// <returns>Node statistics</returns>
    Task<object> GetStatisticsAsync(int nodeId);

    /// <summary>
    /// Creates a tree node from DTO.
    /// </summary>
    /// <param name="createDto">The creation DTO</param>
    /// <returns>The created node</returns>
    Task<TreeNode> CreateTreeNodeAsync(object createDto);

    /// <summary>
    /// Updates a tree node from DTO.
    /// </summary>
    /// <param name="id">The node ID</param>
    /// <param name="updateDto">The update DTO</param>
    /// <returns>The updated node</returns>
    Task<TreeNode> UpdateTreeNodeAsync(int id, object updateDto);

    /// <summary>
    /// Moves a tree node.
    /// </summary>
    /// <param name="id">The node ID</param>
    /// <param name="moveDto">The move DTO</param>
    /// <returns>The moved node</returns>
    Task<TreeNode> MoveTreeNodeAsync(int id, object moveDto);

    /// <summary>
    /// Reorders nodes.
    /// </summary>
    /// <param name="parentId">The parent ID</param>
    /// <param name="nodeOrders">The node order mapping</param>
    /// <returns>True if reordering was successful</returns>
    Task<bool> ReorderNodesAsync(int parentId, IDictionary<int, int> nodeOrders);

    /// <summary>
    /// Deletes a tree node.
    /// </summary>
    /// <param name="id">The node ID</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteTreeNodeAsync(int id);

    /// <summary>
    /// Checks if a node has children.
    /// </summary>
    /// <param name="nodeId">The node ID</param>
    /// <returns>True if the node has children, false otherwise</returns>
    Task<bool> HasChildrenAsync(int nodeId);
}
