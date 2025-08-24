using DifficultyLevelEntity = Ikhtibar.Shared.Entities.DifficultyLevel;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for DifficultyLevel lookup operations
/// </summary>
public interface IDifficultyLevelRepository : IBaseRepository<DifficultyLevelEntity>
{
    /// <summary>
    /// Gets difficulty level by name
    /// </summary>
    /// <param name="name">Difficulty level name</param>
    /// <returns>Difficulty level if found, null otherwise</returns>
    Task<DifficultyLevelEntity?> GetByNameAsync(string name);

    /// <summary>
    /// Gets all active difficulty levels
    /// </summary>
    /// <returns>Collection of active difficulty levels</returns>
    Task<IEnumerable<DifficultyLevelEntity>> GetActiveLevelsAsync();
}
