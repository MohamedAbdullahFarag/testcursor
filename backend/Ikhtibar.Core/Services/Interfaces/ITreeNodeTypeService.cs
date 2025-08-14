using Ikhtibar.Shared.DTOs;

using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for TreeNodeType management operations
/// Following SRP: ONLY tree node type business logic operations
/// </summary>
public interface ITreeNodeTypeService
{
    /// <summary>
    /// Gets all tree node types
    /// </summary>
    /// <returns>Collection of tree node types</returns>
    Task<IEnumerable<TreeNodeTypeDto>> GetAllAsync();

    /// <summary>
    /// Gets active tree node types
    /// </summary>
    /// <returns>Collection of active tree node types</returns>
    Task<IEnumerable<TreeNodeTypeDto>> GetActiveAsync();

    /// <summary>
    /// Gets a tree node type by ID
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type identifier</param>
    /// <returns>Tree node type data if found, null otherwise</returns>
    Task<TreeNodeTypeDto?> GetTreeNodeTypeAsync(int treeNodeTypeId);

    /// <summary>
    /// Gets a tree node type by name
    /// </summary>
    /// <param name="name">Tree node type name</param>
    /// <returns>Tree node type data if found, null otherwise</returns>
    Task<TreeNodeTypeDto?> GetTreeNodeTypeByNameAsync(string name);

    /// <summary>
    /// Creates a new tree node type
    /// </summary>
    /// <param name="createDto">Tree node type creation data</param>
    /// <returns>Created tree node type data</returns>
    Task<TreeNodeTypeDto> CreateTreeNodeTypeAsync(CreateTreeNodeTypeDto createDto);

    /// <summary>
    /// Updates an existing tree node type
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type identifier</param>
    /// <param name="updateDto">Tree node type update data</param>
    /// <returns>Updated tree node type data</returns>
    Task<TreeNodeTypeDto> UpdateTreeNodeTypeAsync(int treeNodeTypeId, UpdateTreeNodeTypeDto updateDto);

    /// <summary>
    /// Deletes a tree node type
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type identifier</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteTreeNodeTypeAsync(int treeNodeTypeId);

    /// <summary>
    /// Checks if tree node type exists
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type identifier</param>
    /// <returns>True if exists, false otherwise</returns>
    Task<bool> ExistsAsync(int treeNodeTypeId);

    /// <summary>
    /// Checks if tree node type is in use
    /// </summary>
    /// <param name="treeNodeTypeId">Tree node type identifier</param>
    /// <returns>True if in use, false otherwise</returns>
    Task<bool> IsInUseAsync(int treeNodeTypeId);
}
