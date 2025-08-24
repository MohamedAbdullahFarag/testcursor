using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for QuestionStatus lookup operations
/// </summary>
public interface IQuestionStatusRepository : IBaseRepository<QuestionStatus>
{
    /// <summary>
    /// Gets question status by name
    /// </summary>
    /// <param name="name">Question status name</param>
    /// <returns>Question status if found, null otherwise</returns>
    Task<QuestionStatus?> GetByNameAsync(string name);

    /// <summary>
    /// Gets all active question statuses
    /// </summary>
    /// <returns>Collection of active question statuses</returns>
    Task<IEnumerable<QuestionStatus>> GetActiveStatusesAsync();
}
