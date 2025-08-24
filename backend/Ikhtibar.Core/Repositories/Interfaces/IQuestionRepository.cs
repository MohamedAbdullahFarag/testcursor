
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Repositories.Interfaces
{
    public interface IQuestionRepository : IBaseRepository<Question>
    {
        // Question retrieval
        Task<Question?> GetByIdWithDetailsAsync(int questionId);
        Task<Question?> GetByIdWithAnswersAsync(int questionId);
        Task<Question?> GetByIdWithMediaAsync(int questionId);
        Task<Question?> GetByIdWithTagsAsync(int questionId);
        Task<Question?> GetByIdWithAllDetailsAsync(int questionId);
        
        // Question listing
        Task<(IEnumerable<Question> Questions, int TotalCount)> GetPagedAsync(int page, int pageSize, string? whereClause = null, object? parameters = null);
        Task<IEnumerable<Question>> GetByFilterAsync(QuestionFilterDto filter);
        Task<IEnumerable<Question>> GetByTreeNodeAsync(int treeNodeId, QuestionFilterDto filter);
        Task<IEnumerable<Question>> GetByBankAsync(int questionBankId, QuestionFilterDto filter);
        Task<IEnumerable<Question>> GetByTypeAsync(int questionTypeId, QuestionFilterDto filter);
        Task<IEnumerable<Question>> GetByStatusAsync(int statusId, QuestionFilterDto filter);
        
        // Question search
        Task<IEnumerable<Question>> SearchByTextAsync(string searchText, SearchOptionsDto options);
        Task<IEnumerable<Question>> SearchByTagsAsync(IEnumerable<string> tags, SearchOptionsDto options);
        Task<IEnumerable<Question>> SearchByCriteriaAsync(QuestionCriteriaDto criteria, SearchOptionsDto options);
        
        // Question relationships
        Task<IEnumerable<Question>> GetRelatedQuestionsAsync(int questionId, int limit = 10);
        Task<bool> LinkQuestionsAsync(int sourceQuestionId, int targetQuestionId, string relationshipType);
        Task<bool> UnlinkQuestionsAsync(int sourceQuestionId, int targetQuestionId);
        
        // Question answers
        Task<IEnumerable<Answer>> GetQuestionAnswersAsync(int questionId);
        Task<Answer?> GetAnswerAsync(int answerId);
        Task<Answer> AddAnswerAsync(Answer answer);
        Task<bool> UpdateAnswerAsync(Answer answer);
        Task<bool> DeleteAnswerAsync(int answerId);
        Task<int> GetAnswerCountAsync(int questionId);
        
        // Question media
        Task<IEnumerable<QuestionMedia>> GetQuestionMediaAsync(int questionId);
        Task<bool> AttachMediaAsync(int questionId, int mediaId);
        Task<bool> DetachMediaAsync(int questionId, int mediaId);
        Task<int> GetMediaCountAsync(int questionId);
        
        // Question tags
        Task<IEnumerable<QuestionTag>> GetQuestionTagsAsync(int questionId);
        Task<bool> ApplyTagAsync(int questionId, int tagId);
        Task<bool> RemoveTagAsync(int questionId, int tagId);
        Task<IEnumerable<string>> GetTagNamesAsync(int questionId);
        
        // Question statistics
        Task<int> GetQuestionCountAsync(QuestionFilterDto filter);
        Task<QuestionStatisticsDto> GetQuestionStatisticsAsync(QuestionStatisticsFilterDto filter);
        Task<IEnumerable<QuestionTypeCountDto>> GetQuestionTypeCountsAsync();
        Task<IEnumerable<DifficultyLevelCountDto>> GetDifficultyLevelCountsAsync();
        
        // Question versions
        Task<IEnumerable<QuestionVersion>> GetQuestionVersionsAsync(int questionId);
        Task<QuestionVersion?> GetCurrentVersionAsync(int questionId);
        Task<QuestionVersion> AddVersionAsync(QuestionVersion version);
        Task<bool> UpdateVersionAsync(QuestionVersion version);
        Task<bool> SetCurrentVersionAsync(int questionId, string version);
        
        // Question usage
        Task<QuestionUsageHistory> AddUsageHistoryAsync(QuestionUsageHistory usage);
        Task<IEnumerable<QuestionUsageHistory>> GetUsageHistoryAsync(int questionId, DateTime fromDate, DateTime toDate);
        Task<QuestionUsageStatisticsDto> GetUsageStatisticsAsync(int questionId, DateTime fromDate, DateTime toDate);
        
        // Bulk operations
        Task<bool> BulkUpdateQuestionsAsync(IEnumerable<Question> questions);
        Task<bool> BulkDeleteQuestionsAsync(IEnumerable<int> questionIds);
        Task<bool> BulkTagQuestionsAsync(IEnumerable<int> questionIds, IEnumerable<int> tagIds);
        Task<bool> BulkMoveQuestionsAsync(IEnumerable<int> questionIds, int targetTreeNodeId);
        
        // Question validation
        Task<QuestionValidation> AddValidationAsync(QuestionValidation validation);
        Task<QuestionValidation?> GetLatestValidationAsync(int questionId);
        Task<IEnumerable<QuestionValidation>> GetValidationHistoryAsync(int questionId);
        
        // Question templates
        Task<IEnumerable<QuestionTemplate>> GetTemplatesByTypeAsync(int questionTypeId);
        Task<QuestionTemplate?> GetTemplateAsync(int templateId);
        Task<QuestionTemplate> AddTemplateAsync(QuestionTemplate template);
        Task<bool> UpdateTemplateAsync(QuestionTemplate template);
        Task<bool> DeleteTemplateAsync(int templateId);
    }
}
