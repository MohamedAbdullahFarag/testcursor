using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IQuestionTemplateService
    {
        // Template CRUD operations
        Task<QuestionTemplateDto> CreateTemplateAsync(CreateQuestionTemplateDto dto);
        Task<QuestionTemplateDto> GetTemplateAsync(int templateId);
        Task<PagedResult<QuestionTemplateDto>> GetTemplatesAsync(TemplateFilterDto filter);
        Task<QuestionTemplateDto> UpdateTemplateAsync(int templateId, UpdateQuestionTemplateDto dto);
        Task<bool> DeleteTemplateAsync(int templateId);
        Task<bool> ArchiveTemplateAsync(int templateId);
        
        // Template management
        Task<QuestionTemplateDto> CloneTemplateAsync(int templateId, CloneTemplateDto dto);
        Task<bool> MakeTemplatePublicAsync(int templateId);
        Task<bool> MakeTemplatePrivateAsync(int templateId);
        Task<IEnumerable<QuestionTemplateDto>> GetPublicTemplatesAsync();
        Task<IEnumerable<QuestionTemplateDto>> GetTemplatesByTypeAsync(int questionTypeId);
        
        // Template usage
        Task<QuestionDto> CreateQuestionFromTemplateAsync(int templateId, CreateQuestionFromTemplateDto dto);
        Task<IEnumerable<QuestionDto>> GetQuestionsCreatedFromTemplateAsync(int templateId);
        Task<TemplateUsageStatisticsDto> GetTemplateUsageStatisticsAsync(int templateId);
        Task<bool> ValidateTemplateAsync(int templateId);
        
        // Template categories and organization
        Task<IEnumerable<QuestionTemplateDto>> GetTemplatesByCategoryAsync(string category);
        Task<IEnumerable<string>> GetTemplateCategoriesAsync();
        Task<bool> AssignTemplateToCategoryAsync(int templateId, string category);
        Task<bool> RemoveTemplateFromCategoryAsync(int templateId, string category);
        
        // Template validation and quality
        Task<TemplateValidationResult> ValidateTemplateContentAsync(int templateId);
        Task<TemplateQualityScoreDto> AssessTemplateQualityAsync(int templateId);
        Task<IEnumerable<TemplateQualityIssueDto>> GetTemplateQualityIssuesAsync(int templateId);
        Task<bool> MarkTemplateQualityIssueResolvedAsync(int issueId, int resolvedBy, string resolution);

    // API-expected convenience methods
    Task<IEnumerable<QuestionTemplateDto>> GetAllAsync();
    Task<QuestionTemplateDto> GetByIdAsync(int id);
    Task<QuestionTemplateDto> GetByCodeAsync(string code);
    Task<QuestionTemplateDto> CreateAsync(CreateQuestionTemplateDto dto);
    Task<QuestionTemplateDto> UpdateAsync(int id, UpdateQuestionTemplateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<QuestionTemplateDto> CloneAsync(int id, CloneTemplateDto dto);
    Task<IEnumerable<QuestionTemplateDto>> SearchAsync(TemplateSearchDto dto);
    // Additional controller-expected variants
    Task<IEnumerable<QuestionTemplateDto>> SearchAsync(string query, int limit);
    Task<IEnumerable<QuestionTemplateDto>> GetByQuestionTypeAsync(int questionTypeId);
    Task<IEnumerable<QuestionTemplateDto>> GetByCategoryAsync(string category);
    Task<IEnumerable<QuestionTemplateDto>> GetByDifficultyLevelAsync(int difficultyLevelId);
    Task<IEnumerable<QuestionTemplateDto>> GetByQuestionBankAsync(int questionBankId);
    Task<IEnumerable<QuestionTemplateDto>> GetByCurriculumAsync(int curriculumId);
    Task<IEnumerable<QuestionTemplateDto>> GetBySubjectAsync(int subjectId);
    Task<IEnumerable<QuestionTemplateDto>> GetByGradeLevelAsync(int gradeLevelId);
    Task<IEnumerable<QuestionTemplateDto>> GetActiveAsync();
    Task<IEnumerable<QuestionTemplateDto>> GetSystemTemplatesAsync();
    // Accept CreateQuestionTemplateDto as clone input for controller compatibility
    Task<QuestionTemplateDto> CloneAsync(int id, CreateQuestionTemplateDto dto);
    Task<TemplateStatisticsDto> GetStatisticsAsync();
    }
}
