using Dapper;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using QuestionTypeEntity = Ikhtibar.Shared.Entities.QuestionType;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for QuestionType lookup operations
/// </summary>
public class QuestionTypeRepository : BaseRepository<QuestionTypeEntity>, IQuestionTypeRepository
{
    public QuestionTypeRepository(IDbConnectionFactory connectionFactory) 
        : base(connectionFactory)
    {
    }

    /// <summary>
    /// Gets question type by name
    /// </summary>
    /// <param name="name">Question type name</param>
    /// <returns>Question type if found, null otherwise</returns>
    public async Task<QuestionTypeEntity?> GetByNameAsync(string name)
    {
        const string sql = "SELECT * FROM QuestionTypes WHERE Name = @Name";
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<QuestionTypeEntity>(sql, new { Name = name });
    }

    /// <summary>
    /// Gets all active question types
    /// </summary>
    /// <returns>Collection of active question types</returns>
    public async Task<IEnumerable<QuestionTypeEntity>> GetActiveTypesAsync()
    {
        const string sql = "SELECT * FROM QuestionTypes ORDER BY Name";
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<QuestionTypeEntity>(sql);
    }
}
