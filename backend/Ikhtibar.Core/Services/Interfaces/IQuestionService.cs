using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IQuestionService
    {
        // Basic CRUD operations
        Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto dto);
        Task<QuestionDto> GetQuestionAsync(int questionId);
        Task<PagedResult<QuestionDto>> GetQuestionsAsync(QuestionFilterDto filter);
        Task<QuestionDto> UpdateQuestionAsync(int questionId, UpdateQuestionDto dto);
        Task<bool> DeleteQuestionAsync(int questionId);
        Task<bool> ArchiveQuestionAsync(int questionId);
        
        // Question validation
        Task<QuestionValidationResult> ValidateQuestionAsync(ValidateQuestionDto dto);
        Task<bool> PublishQuestionAsync(int questionId);
        Task<bool> UnpublishQuestionAsync(int questionId);
        
        // Question duplicates and versions
        Task<QuestionDto> DuplicateQuestionAsync(int questionId, DuplicateQuestionDto dto);
        Task<IEnumerable<QuestionVersionDto>> GetQuestionVersionsAsync(int questionId);
        Task<QuestionDto> CreateQuestionVersionAsync(int questionId, CreateVersionDto dto);
        Task<bool> RestoreQuestionVersionAsync(int questionId, string version);
        
        // Question relationships
        Task<IEnumerable<QuestionDto>> GetRelatedQuestionsAsync(int questionId);
        Task<bool> LinkQuestionsAsync(int sourceQuestionId, int targetQuestionId, string relationshipType);
        Task<bool> UnlinkQuestionsAsync(int sourceQuestionId, int targetQuestionId);
        
        // Question media
        Task<bool> AttachMediaToQuestionAsync(int questionId, int mediaId);
        Task<bool> DetachMediaFromQuestionAsync(int questionId, int mediaId);
        Task<IEnumerable<MediaFileDto>> GetQuestionMediaAsync(int questionId);
        
        // Question analytics
        Task<QuestionUsageStatisticsDto> GetQuestionUsageAsync(int questionId);
        Task<QuestionPerformanceDto> GetQuestionPerformanceAsync(int questionId);
        Task<IEnumerable<QuestionDto>> GetSimilarQuestionsAsync(int questionId);
        
        // Bulk operations
        Task<BulkOperationResult> BulkCreateQuestionsAsync(IEnumerable<CreateQuestionDto> questions);
        Task<BulkOperationResult> BulkUpdateQuestionsAsync(BulkUpdateQuestionsDto dto);
        Task<BulkOperationResult> BulkDeleteQuestionsAsync(IEnumerable<int> questionIds);
        Task<BulkOperationResult> BulkTagQuestionsAsync(BulkTagDto dto);
    }
}
