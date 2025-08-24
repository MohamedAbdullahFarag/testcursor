using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for tree node type data operations.
/// Extends the base repository with type-specific operations.
/// </summary>
public interface ITreeNodeTypeRepository : IBaseRepository<TreeNodeType>
{
    /// <summary>
    /// Gets a tree node type by its code.
    /// </summary>
    /// <param name="code">The type code</param>
    /// <returns>The type if found, null otherwise</returns>
    Task<TreeNodeType?> GetByCodeAsync(string code);

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
    /// Gets tree node types by search term.
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
    /// Checks if a type code is unique.
    /// </summary>
    /// <param name="code">The type code to check</param>
    /// <param name="excludeId">ID to exclude from uniqueness check</param>
    /// <returns>True if the code is unique</returns>
    Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);

    /// <summary>
    /// Gets the count of tree nodes using a specific type.
    /// </summary>
    /// <param name="typeId">The type ID</param>
    /// <returns>The count of nodes using this type</returns>
    Task<int> GetNodeCountByTypeAsync(int typeId);

    /// <summary>
    /// Gets tree node types with their usage statistics.
    /// </summary>
    /// <returns>Collection of types with node counts</returns>
    Task<IEnumerable<TreeNodeType>> GetTypesWithUsageStatsAsync();
}
