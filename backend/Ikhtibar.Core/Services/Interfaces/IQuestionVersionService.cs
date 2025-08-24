using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Core.Services.Interfaces
{
    // Thin alias to preserve API compatibility where controllers expect IQuestionVersionService
    public interface IQuestionVersionService : IQuestionVersioningService
    {
        // API-friendly convenience methods expected by controllers
        Task<IEnumerable<QuestionVersionDto>> GetAllAsync();
        Task<QuestionVersionDto?> GetByIdAsync(int id);
        Task<IEnumerable<QuestionVersionDto>> GetByQuestionIdAsync(int questionId);
        Task<QuestionVersionDto> CreateAsync(CreateQuestionVersionDto dto);
        Task<QuestionVersionDto> UpdateAsync(int id, UpdateQuestionVersionDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ActivateAsync(int id);
        Task<bool> PublishAsync(int id);
        Task<bool> ArchiveAsync(int id);
        Task<IEnumerable<QuestionVersionDto>> GetPublishedAsync(int questionId);
        Task<IEnumerable<QuestionVersionDto>> GetArchivedAsync(int questionId);
    // Controller-friendly parameterless overloads (e.g., for listing recent published/archived versions)
    Task<IEnumerable<QuestionVersionDto>> GetPublishedAsync();
    Task<IEnumerable<QuestionVersionDto>> GetArchivedAsync();
        Task<IEnumerable<QuestionVersionDto>> GetHistoryAsync(int questionId);
        Task<VersionCompareResultDto> CompareAsync(int id, int otherVersionId);
        Task<QuestionVersionStatisticsDto> GetStatisticsAsync(int questionId);
    Task<QuestionVersionStatisticsDto> GetStatisticsAsync();
        Task<bool> RevertToVersionAsync(int questionId, int versionId);
    }
}
