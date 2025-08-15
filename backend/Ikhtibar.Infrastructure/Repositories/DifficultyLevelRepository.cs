using Dapper;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using DifficultyLevelEntity = Ikhtibar.Shared.Entities.DifficultyLevel;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for DifficultyLevel lookup operations
/// </summary>
public class DifficultyLevelRepository : BaseRepository<DifficultyLevelEntity>, IDifficultyLevelRepository
{
    public DifficultyLevelRepository(IDbConnectionFactory connectionFactory, ILogger<DifficultyLevelRepository> logger) 
        : base(connectionFactory, logger, "DifficultyLevels", "DifficultyLevelId")
    {
    }

    /// <summary>
    /// Gets difficulty level by name
    /// </summary>
    /// <param name="name">Difficulty level name</param>
    /// <returns>Difficulty level if found, null otherwise</returns>
    public async Task<DifficultyLevelEntity?> GetByNameAsync(string name)
    {
        const string sql = "SELECT * FROM DifficultyLevels WHERE Name = @Name";
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<DifficultyLevelEntity>(sql, new { Name = name });
    }

    /// <summary>
    /// Gets all active difficulty levels
    /// </summary>
    /// <returns>Collection of active difficulty levels</returns>
    public async Task<IEnumerable<DifficultyLevelEntity>> GetActiveLevelsAsync()
    {
        const string sql = "SELECT * FROM DifficultyLevels ORDER BY Name";
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<DifficultyLevelEntity>(sql);
    }
}
