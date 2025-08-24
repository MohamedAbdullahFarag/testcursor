
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Repositories.Interfaces
{
    public interface IQuestionTemplateRepository : IBaseRepository<QuestionTemplate>
    {
        // Template retrieval
        Task<QuestionTemplate?> GetByIdWithDetailsAsync(int templateId);
        Task<QuestionTemplate?> GetByNameAsync(string name);
        Task<IEnumerable<QuestionTemplate>> GetByTypeAsync(int questionTypeId);
        Task<IEnumerable<QuestionTemplate>> GetByCreatorAsync(int createdBy);
        Task<IEnumerable<QuestionTemplate>> GetPublicTemplatesAsync();
        
        // Template listing
        Task<(IEnumerable<QuestionTemplate> Templates, int TotalCount)> GetPagedAsync(int page, int pageSize, string? whereClause = null, object? parameters = null);
        Task<IEnumerable<QuestionTemplate>> GetByFilterAsync(TemplateFilterDto filter);
        Task<IEnumerable<QuestionTemplate>> GetActiveTemplatesAsync();
        Task<IEnumerable<QuestionTemplate>> GetTemplatesBySubjectAsync(string subject);
        Task<IEnumerable<QuestionTemplate>> GetTemplatesByGradeLevelAsync(string gradeLevel);
        
        // Template management
        Task<QuestionTemplate> CreateTemplateAsync(QuestionTemplate template);
        Task<bool> UpdateTemplateAsync(QuestionTemplate template);
        Task<bool> DeleteTemplateAsync(int templateId);
        Task<bool> CloneTemplateAsync(int templateId, string newName, int newCreatorId);
        Task<bool> MakePublicAsync(int templateId);
        Task<bool> MakePrivateAsync(int templateId);
        
        // Template usage
        Task<int> GetUsageCountAsync(int templateId);
        Task<IEnumerable<QuestionTemplate>> GetMostUsedTemplatesAsync(int limit = 10);
        Task<bool> IncrementUsageCountAsync(int templateId);
        Task<QuestionTemplateUsageDto> GetUsageStatisticsAsync(int templateId);
        
        // Template categories
        Task<IEnumerable<TemplateCategoryDto>> GetTemplateCategoriesAsync();
        Task<TemplateCategoryDto?> GetCategoryByIdAsync(int categoryId);
        Task<TemplateCategoryDto> CreateCategoryAsync(TemplateCategoryDto category);
        Task<bool> UpdateCategoryAsync(TemplateCategoryDto category);
        Task<bool> DeleteCategoryAsync(int categoryId);
        
        // Template validation
        Task<bool> ValidateTemplateAsync(int templateId);
        Task<TemplateValidationResult> ValidateTemplateContentAsync(int templateId);
        Task<bool> SetTemplateQualityAsync(int templateId, double qualityScore);
        Task<TemplateQualityDto> GetTemplateQualityAsync(int templateId);
        
        // Template analytics
        Task<TemplateAnalyticsDto> GetTemplateAnalyticsAsync(int templateId);
        Task<IEnumerable<TemplateTrendDto>> GetTemplateTrendsAsync(int templateId);
        Task<TemplatePerformanceDto> GetTemplatePerformanceAsync(int templateId);
        Task<IEnumerable<TemplateUsageStatisticsDto>> GetUsageStatisticsAsync();
        
        // Template import/export
        Task<ExportTemplateDto> ExportTemplateAsync(int templateId, ExportFormat format);
        Task<QuestionTemplate> ImportTemplateAsync(ImportTemplateDto importDto);
        Task<bool> BulkImportTemplatesAsync(IEnumerable<ImportTemplateDto> importDtos);
        Task<bool> ExportTemplatesAsync(IEnumerable<int> templateIds, ExportFormat format, string exportPath);
        
        // Template cleanup
        Task<int> CleanupUnusedTemplatesAsync(int daysUnused = 365);
        Task<int> CleanupLowQualityTemplatesAsync(double minQualityScore = 5.0);
        Task<bool> ArchiveTemplateAsync(int templateId);
        Task<bool> RestoreTemplateAsync(int templateId);
    }
}
