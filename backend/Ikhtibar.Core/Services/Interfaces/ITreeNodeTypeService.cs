using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for managing tree node types.
/// Provides operations for creating, updating, and managing node type definitions.
/// </summary>
public interface ITreeNodeTypeService
{
    /// <summary>
    /// Creates a new tree node type.
    /// </summary>
    /// <param name="nodeType">The node type to create</param>
    /// <returns>The created node type with generated ID</returns>
    Task<TreeNodeType> CreateAsync(TreeNodeType nodeType);

    /// <summary>
    /// Gets a tree node type by its ID.
    /// </summary>
    /// <param name="id">The type ID</param>
    /// <returns>The type if found, null otherwise</returns>
    Task<TreeNodeType?> GetByIdAsync(int id);

    /// <summary>
    /// Gets a tree node type by its code.
    /// </summary>
    /// <param name="code">The type code</param>
    /// <returns>The type if found, null otherwise</returns>
    Task<TreeNodeType?> GetByCodeAsync(string code);

    /// <summary>
    /// Gets all tree node types.
    /// </summary>
    /// <returns>Collection of all types</returns>
    Task<IEnumerable<TreeNodeType>> GetAllAsync();

    /// <summary>
    /// Gets all active tree node types.
    /// </summary>
    /// <returns>Collection of active types</returns>
    Task<IEnumerable<TreeNodeType>> GetActiveTypesAsync();

    /// <summary>
    /// Gets all visible tree node types.
    /// </summary>
    /// <returns>Collection of visible types</returns>
    Task<IEnumerable<TreeNodeType>> GetVisibleTypesAsync();

    /// <summary>
    /// Gets tree node types that allow children.
    /// </summary>
    /// <returns>Collection of types that allow children</returns>
    Task<IEnumerable<TreeNodeType>> GetTypesThatAllowChildrenAsync();

    /// <summary>
    /// Updates an existing tree node type.
    /// </summary>
    /// <param name="nodeType">The updated node type data</param>
    /// <returns>The updated node type</returns>
    Task<TreeNodeType> UpdateAsync(TreeNodeType nodeType);

    /// <summary>
    /// Deletes a tree node type.
    /// </summary>
    /// <param name="id">The type ID to delete</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Searches for tree node types by name or description.
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <returns>Collection of matching types</returns>
    Task<IEnumerable<TreeNodeType>> SearchAsync(string searchTerm);

    /// <summary>
    /// Gets system-defined tree node types.
    /// </summary>
    /// <returns>Collection of system types</returns>
    Task<IEnumerable<TreeNodeType>> GetSystemTypesAsync();

    /// <summary>
    /// Gets user-defined tree node types.
    /// </summary>
    /// <returns>Collection of user-defined types</returns>
    Task<IEnumerable<TreeNodeType>> GetUserDefinedTypesAsync();

    /// <summary>
    /// Validates if a node type can be deleted.
    /// </summary>
    /// <param name="id">The type ID to validate</param>
    /// <returns>Validation result with details</returns>
    Task<(bool CanDelete, string Reason)> ValidateDeletionAsync(int id);

    /// <summary>
    /// Gets usage statistics for a node type.
    /// </summary>
    /// <param name="id">The type ID</param>
    /// <returns>Usage statistics</returns>
    Task<(int TotalNodes, int ActiveNodes, int VisibleNodes)> GetUsageStatsAsync(int id);

    /// <summary>
    /// Gets all node types with their usage statistics.
    /// </summary>
    /// <returns>Collection of types with usage stats</returns>
    Task<IEnumerable<TreeNodeType>> GetTypesWithUsageStatsAsync();

    /// <summary>
    /// Checks if a type code is unique.
    /// </summary>
    /// <param name="code">The type code to check</param>
    /// <param name="excludeId">ID to exclude from uniqueness check</param>
    /// <returns>True if the code is unique</returns>
    Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);

    /// <summary>
    /// Activates a tree node type.
    /// </summary>
    /// <param name="id">The type ID to activate</param>
    /// <returns>True if activation was successful</returns>
    Task<bool> ActivateAsync(int id);

    /// <summary>
    /// Deactivates a tree node type.
    /// </summary>
    /// <param name="id">The type ID to deactivate</param>
    /// <returns>True if deactivation was successful</returns>
    Task<bool> DeactivateAsync(int id);

    /// <summary>
    /// Makes a tree node type visible.
    /// </summary>
    /// <param name="id">The type ID to make visible</param>
    /// <returns>True if operation was successful</returns>
    Task<bool> MakeVisibleAsync(int id);

    /// <summary>
    /// Hides a tree node type.
    /// </summary>
    /// <param name="id">The type ID to hide</param>
    /// <returns>True if operation was successful</returns>
    Task<bool> HideAsync(int id);

    /// <summary>
    /// Gets a tree node type by its ID (alias for GetByIdAsync).
    /// </summary>
    /// <param name="id">The type ID</param>
    /// <returns>The type if found, null otherwise</returns>
    Task<TreeNodeType?> GetTreeNodeTypeAsync(int id);

    /// <summary>
    /// Checks if a tree node type exists.
    /// </summary>
    /// <param name="id">The type ID</param>
    /// <returns>True if the type exists, false otherwise</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Gets a tree node type by its name.
    /// </summary>
    /// <param name="name">The type name</param>
    /// <returns>The type if found, null otherwise</returns>
    Task<TreeNodeType?> GetTreeNodeTypeByNameAsync(string name);

    /// <summary>
    /// Creates a tree node type from DTO.
    /// </summary>
    /// <param name="createDto">The creation DTO</param>
    /// <returns>The created type</returns>
    Task<TreeNodeType> CreateTreeNodeTypeAsync(object createDto);

    /// <summary>
    /// Updates a tree node type from DTO.
    /// </summary>
    /// <param name="id">The type ID</param>
    /// <param name="updateDto">The update DTO</param>
    /// <returns>The updated type</returns>
    Task<TreeNodeType> UpdateTreeNodeTypeAsync(int id, object updateDto);

    /// <summary>
    /// Deletes a tree node type.
    /// </summary>
    /// <param name="id">The type ID</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteTreeNodeTypeAsync(int id);
}
