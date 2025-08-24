using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IQuestionBankService
    {
        // Basic CRUD operations
        Task<QuestionBankDto> CreateQuestionBankAsync(CreateQuestionBankDto dto);
        Task<QuestionBankDto> GetQuestionBankAsync(int questionBankId);
        Task<PagedResult<QuestionBankDto>> GetQuestionBanksAsync(QuestionBankFilterDto filter);
        Task<QuestionBankDto> UpdateQuestionBankAsync(int questionBankId, UpdateQuestionBankDto dto);
        Task<bool> DeleteQuestionBankAsync(int questionBankId);
        Task<bool> ArchiveQuestionBankAsync(int questionBankId);
        
        // Question bank management
        Task<bool> AddQuestionToBankAsync(int questionBankId, int questionId);
        Task<bool> RemoveQuestionFromBankAsync(int questionBankId, int questionId);
        Task<bool> MoveQuestionBetweenBanksAsync(int questionId, int sourceBankId, int targetBankId);
    Task<IEnumerable<QuestionDto>> GetQuestionsInBankAsync(int questionBankId, QuestionFilterDto filter);
    // Controller-friendly overload: get questions without passing a filter
    Task<IEnumerable<QuestionDto>> GetQuestionsInBankAsync(int questionBankId);
    Task<QuestionBankStatisticsDto> GetQuestionBankStatisticsAsync(int questionBankId);
    Task<bool> AddQuestionsToBankAsync(int questionBankId, IEnumerable<int> questionIds);
    Task<bool> RemoveQuestionsFromBankAsync(int questionBankId, IEnumerable<int> questionIds);
    Task<QuestionBankDto> CloneQuestionBankAsync(int questionBankId, CloneQuestionBankDto dto);
    Task<ExportResultDto> ExportQuestionBankAsync(int questionBankId, ExportFormat format);
    Task<ExportResultDto> ExportQuestionBankAsync(int questionBankId, string format);
    Task<bool> ImportQuestionsAsync(int questionBankId, ImportQuestionsDto dto);
    Task<bool> ImportQuestionsAsync(int questionBankId, Ikhtibar.Shared.DTOs.QuestionBankImportDto dto);
        
        // Access control
    Task<bool> GrantAccessAsync(int questionBankId, int userId, string permission);
    // Controller-friendly overload accepting shared DTO
    Task<bool> GrantAccessAsync(int questionBankId, GrantQuestionBankAccessDto accessDto);
        Task<bool> RevokeAccessAsync(int questionBankId, int userId);
    Task<IEnumerable<QuestionBankAccessDto>> GetBankAccessAsync(int questionBankId);
    // Controller-friendly alias
    Task<IEnumerable<QuestionBankAccessDto>> GetQuestionBankAccessAsync(int questionBankId);

    // Controller-friendly analytics alias (parameterless)
    Task<QuestionBankAnalyticsDto> GetQuestionBankAnalyticsAsync(int questionBankId);
        Task<bool> HasAccessAsync(int questionBankId, int userId, string permission);
        
        // Statistics and analytics
        Task<QuestionBankStatisticsDto> GetBankStatisticsAsync(int questionBankId);
        Task<bool> UpdateBankStatisticsAsync(int questionBankId);
        Task<QuestionBankAnalyticsDto> GetBankAnalyticsAsync(int questionBankId, DateTime fromDate, DateTime toDate);
        
        // Bulk operations
        Task<BulkOperationResult> BulkAddQuestionsToBankAsync(int questionBankId, IEnumerable<int> questionIds);
        Task<BulkOperationResult> BulkRemoveQuestionsFromBankAsync(int questionBankId, IEnumerable<int> questionIds);
        Task<BulkOperationResult> BulkUpdateBankQuestionsAsync(BulkUpdateBankQuestionsDto dto);
    }
}
