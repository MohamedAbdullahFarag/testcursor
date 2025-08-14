using Ikhtibar.Shared.Entities;

using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for TreeNodeType entity operations
/// Manages tree node type definitions and metadata
/// </summary>
public interface ITreeNodeTypeRepository
{
    /// <summary>
    /// Get tree node type by ID
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type ID</param>
    /// <returns>Tree node type if found, null otherwise</returns>
    Task<TreeNodeType?> GetByIdAsync(int treeNodeTypeId);

    /// <summary>
    /// Get tree node type by name
    /// </summary>
    /// <param name="name">Tree node type name</param>
    /// <returns>Tree node type if found, null otherwise</returns>
    Task<TreeNodeType?> GetByNameAsync(string name);

    /// <summary>
    /// Get all tree node types
    /// </summary>
    /// <returns>Collection of all tree node types</returns>
    Task<IEnumerable<TreeNodeType>> GetAllAsync();

    /// <summary>
    /// Get active tree node types ordered by display order
    /// </summary>
    /// <returns>Collection of active tree node types</returns>
    Task<IEnumerable<TreeNodeType>> GetActiveAsync();

    /// <summary>
    /// Create a new tree node type
    /// </summary>
    /// <param name="treeNodeType">Tree node type to create</param>
    /// <returns>Created tree node type with generated ID</returns>
    Task<TreeNodeType> CreateAsync(TreeNodeType treeNodeType);

    /// <summary>
    /// Update an existing tree node type
    /// </summary>
    /// <param name="treeNodeType">Tree node type to update</param>
    /// <returns>Updated tree node type</returns>
    Task<TreeNodeType> UpdateAsync(TreeNodeType treeNodeType);

    /// <summary>
    /// Delete a tree node type (soft delete)
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type ID to delete</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteAsync(int treeNodeTypeId);

    /// <summary>
    /// Check if tree node type exists
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type ID to check</param>
    /// <returns>True if exists, false otherwise</returns>
    Task<bool> ExistsAsync(int treeNodeTypeId);

    /// <summary>
    /// Check if tree node type is in use by any nodes
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type ID to check</param>
    /// <returns>True if in use, false otherwise</returns>
    Task<bool> IsInUseAsync(int treeNodeTypeId);
}
