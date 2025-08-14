using QuestionTypeEntity = Ikhtibar.Shared.Entities.QuestionType;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for QuestionType lookup operations
/// </summary>
public interface IQuestionTypeRepository : IRepository<QuestionTypeEntity>
{
    /// <summary>
    /// Gets question type by name
    /// </summary>
    /// <param name="name">Question type name</param>
    /// <returns>Question type if found, null otherwise</returns>
    Task<QuestionTypeEntity?> GetByNameAsync(string name);

    /// <summary>
    /// Gets all active question types
    /// </summary>
    /// <returns>Collection of active question types</returns>
    Task<IEnumerable<QuestionTypeEntity>> GetActiveTypesAsync();
}
