
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Repositories.Interfaces
{
    public interface IQuestionBankRepository : IBaseRepository<QuestionBank>
    {
        // Question bank retrieval
        Task<QuestionBank?> GetByIdWithDetailsAsync(int questionBankId);
        Task<QuestionBank?> GetByIdWithQuestionsAsync(int questionBankId);
        Task<QuestionBank?> GetByIdWithAccessAsync(int questionBankId);
        Task<QuestionBank?> GetByIdWithAllDetailsAsync(int questionBankId);
        
        // Question bank listing
        Task<(IEnumerable<QuestionBank> QuestionBanks, int TotalCount)> GetPagedAsync(int page, int pageSize, string? whereClause = null, object? parameters = null);
        Task<IEnumerable<QuestionBank>> GetByFilterAsync(QuestionBankFilterDto filter);
        Task<IEnumerable<QuestionBank>> GetByCreatorAsync(int creatorId);
        Task<IEnumerable<QuestionBank>> GetPublicBanksAsync();
        Task<IEnumerable<QuestionBank>> GetBySubjectAsync(string subject);
        Task<IEnumerable<QuestionBank>> GetByGradeLevelAsync(string gradeLevel);
        
        // Question bank management
        Task<bool> AddQuestionToBankAsync(int questionBankId, int questionId);
        Task<bool> RemoveQuestionFromBankAsync(int questionBankId, int questionId);
        Task<bool> MoveQuestionBetweenBanksAsync(int questionId, int sourceBankId, int targetBankId);
        Task<IEnumerable<Question>> GetQuestionsInBankAsync(int questionBankId, QuestionFilterDto filter);
        Task<int> GetQuestionCountInBankAsync(int questionBankId);
        
        // Access control
        Task<IEnumerable<QuestionBankAccess>> GetBankAccessAsync(int questionBankId);
        Task<bool> GrantAccessAsync(int questionBankId, int userId, string permission);
        Task<bool> RevokeAccessAsync(int questionBankId, int userId);
        Task<bool> HasAccessAsync(int questionBankId, int userId, string permission);
        Task<QuestionBankAccess?> GetUserAccessAsync(int questionBankId, int userId);
        
        // Statistics and analytics
        Task<QuestionBankStatisticsDto> GetBankStatisticsAsync(int questionBankId);
        Task<bool> UpdateBankStatisticsAsync(int questionBankId);
        Task<IEnumerable<QuestionBankStatisticsDto>> GetBanksStatisticsAsync(IEnumerable<int> questionBankIds);
        Task<QuestionBankAnalyticsDto> GetBankAnalyticsAsync(int questionBankId, DateTime fromDate, DateTime toDate);
        
        // Bulk operations
        Task<bool> BulkAddQuestionsToBankAsync(int questionBankId, IEnumerable<int> questionIds);
        Task<bool> BulkRemoveQuestionsFromBankAsync(int questionBankId, IEnumerable<int> questionIds);
        Task<bool> BulkUpdateBankQuestionsAsync(BulkUpdateBankQuestionsDto dto);
        
        // Question bank templates
        Task<IEnumerable<QuestionBank>> GetTemplateBanksAsync();
        Task<bool> CreateBankFromTemplateAsync(int templateBankId, CreateQuestionBankDto dto);
        Task<bool> ExportBankAsTemplateAsync(int questionBankId, string templateName);
        
        // Question bank validation
        Task<bool> ValidateBankStructureAsync(int questionBankId);
        Task<IEnumerable<BankValidationIssueDto>> GetBankValidationIssuesAsync(int questionBankId);
        Task<bool> MarkBankValidationIssueResolvedAsync(int issueId, int resolvedBy, string resolution);
    }
}
