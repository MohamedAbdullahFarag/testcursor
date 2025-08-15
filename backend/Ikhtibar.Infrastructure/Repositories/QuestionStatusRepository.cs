using Dapper;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for QuestionStatus lookup operations
/// </summary>
public class QuestionStatusRepository : BaseRepository<QuestionStatus>, IQuestionStatusRepository
{
    public QuestionStatusRepository(IDbConnectionFactory connectionFactory, ILogger<QuestionStatusRepository> logger) 
        : base(connectionFactory, logger, "QuestionStatuses", "QuestionStatusId")
    {
    }

    /// <summary>
    /// Gets question status by name
    /// </summary>
    /// <param name="name">Question status name</param>
    /// <returns>Question status if found, null otherwise</returns>
    public async Task<QuestionStatus?> GetByNameAsync(string name)
    {
        const string sql = "SELECT * FROM QuestionStatuses WHERE Name = @Name";
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<QuestionStatus>(sql, new { Name = name });
    }

    /// <summary>
    /// Gets all active question statuses
    /// </summary>
    /// <returns>Collection of active question statuses</returns>
    public async Task<IEnumerable<QuestionStatus>> GetActiveStatusesAsync()
    {
        const string sql = "SELECT * FROM QuestionStatuses ORDER BY Name";
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<QuestionStatus>(sql);
    }
}
