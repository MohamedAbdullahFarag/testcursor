using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for tree node data operations.
/// Extends the base repository with tree-specific operations.
/// </summary>
public interface ITreeNodeRepository : IBaseRepository<TreeNode>
{
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
    /// Gets all descendants of a specific node.
    /// </summary>
    /// <param name="nodeId">The node ID</param>
    /// <returns>Collection of descendant nodes</returns>
    Task<IEnumerable<TreeNode>> GetDescendantsAsync(int nodeId);

    /// <summary>
    /// Gets all ancestors of a specific node.
    /// </summary>
    /// <param name="nodeId">The node ID</param>
    /// <returns>Collection of ancestor nodes</returns>
    Task<IEnumerable<TreeNode>> GetAncestorsAsync(int nodeId);

    /// <summary>
    /// Gets nodes by their materialized path.
    /// </summary>
    /// <param name="path">The materialized path</param>
    /// <returns>Collection of nodes matching the path</returns>
    Task<IEnumerable<TreeNode>> GetByPathAsync(string path);

    /// <summary>
    /// Gets nodes by type.
    /// </summary>
    /// <param name="nodeType">The node type</param>
    /// <returns>Collection of nodes of the specified type</returns>
    Task<IEnumerable<TreeNode>> GetByTypeAsync(string nodeType);

    /// <summary>
    /// Gets nodes at a specific depth level.
    /// </summary>
    /// <param name="depth">The depth level</param>
    /// <returns>Collection of nodes at the specified depth</returns>
    Task<IEnumerable<TreeNode>> GetByDepthAsync(int depth);

    /// <summary>
    /// Gets nodes by search term (name or description).
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <returns>Collection of matching nodes</returns>
    Task<IEnumerable<TreeNode>> SearchAsync(string searchTerm);

    /// <summary>
    /// Gets the next available order index for a parent.
    /// </summary>
    /// <param name="parentId">The parent node ID</param>
    /// <returns>The next available order index</returns>
    Task<int> GetNextOrderIndexAsync(int? parentId);

    /// <summary>
    /// Updates the order of children within a parent.
    /// </summary>
    /// <param name="parentId">The parent node ID</param>
    /// <param name="childOrders">Dictionary of child ID to new order</param>
    /// <returns>True if successful</returns>
    Task<bool> UpdateChildOrderAsync(int parentId, IDictionary<int, int> childOrders);

    /// <summary>
    /// Gets the depth of a node.
    /// </summary>
    /// <param name="nodeId">The node ID</param>
    /// <returns>The depth level</returns>
    Task<int> GetDepthAsync(int nodeId);

    /// <summary>
    /// Checks if a node has children.
    /// </summary>
    /// <param name="nodeId">The node ID</param>
    /// <returns>True if the node has children</returns>
    Task<bool> HasChildrenAsync(int nodeId);

    /// <summary>
    /// Gets the count of children for a node.
    /// </summary>
    /// <param name="nodeId">The node ID</param>
    /// <returns>The child count</returns>
    Task<int> GetChildCountAsync(int nodeId);

    /// <summary>
    /// Gets the count of descendants for a node.
    /// </summary>
    /// <param name="nodeId">The node ID</param>
    /// <returns>The descendant count</returns>
    Task<int> GetDescendantCountAsync(int nodeId);

    /// <summary>
    /// Gets nodes by parent path and type.
    /// </summary>
    /// <param name="parentPath">The parent path</param>
    /// <param name="nodeType">The node type</param>
    /// <returns>Collection of matching nodes</returns>
    Task<IEnumerable<TreeNode>> GetByParentPathAndTypeAsync(string parentPath, string nodeType);

    /// <summary>
    /// Gets all active nodes.
    /// </summary>
    /// <returns>Collection of active nodes</returns>
    Task<IEnumerable<TreeNode>> GetActiveNodesAsync();

    /// <summary>
    /// Gets all visible nodes.
    /// </summary>
    /// <returns>Collection of visible nodes</returns>
    Task<IEnumerable<TreeNode>> GetVisibleNodesAsync();
}
