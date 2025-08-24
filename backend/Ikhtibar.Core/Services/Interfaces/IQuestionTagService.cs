using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IQuestionTagService
    {
        // Tag CRUD operations
        Task<QuestionTagDto> CreateTagAsync(CreateQuestionTagDto dto);
        Task<QuestionTagDto> GetTagAsync(int tagId);
        Task<PagedResult<QuestionTagDto>> GetTagsAsync(TagFilterDto filter);
        Task<QuestionTagDto> UpdateTagAsync(int tagId, UpdateQuestionTagDto dto);
        Task<bool> DeleteTagAsync(int tagId);
        Task<bool> ArchiveTagAsync(int tagId);
        
        // Tag management
        Task<IEnumerable<QuestionTagDto>> GetTagsByCategoryAsync(string category);
        Task<IEnumerable<string>> GetTagCategoriesAsync();
        Task<bool> AssignTagToQuestionAsync(int questionId, int tagId, int assignedBy);
        Task<bool> RemoveTagFromQuestionAsync(int questionId, int tagId);
        Task<IEnumerable<QuestionTagDto>> GetQuestionTagsAsync(int questionId);
        
        // Tag operations
        Task<bool> MergeTagsAsync(int sourceTagId, int targetTagId);
        Task<bool> SplitTagAsync(int tagId, IEnumerable<string> newTagNames);
        Task<bool> RenameTagAsync(int tagId, string newName);
        Task<bool> ChangeTagCategoryAsync(int tagId, string newCategory);
        
        // Tag analytics
        Task<TagUsageStatisticsDto> GetTagUsageStatisticsAsync(int tagId);
        Task<IEnumerable<PopularTagDto>> GetPopularTagsAsync(int limit = 20);
        Task<IEnumerable<TagTrendDto>> GetTagTrendsAsync(DateTime fromDate, DateTime toDate);
        Task<TagAnalyticsDto> GetTagAnalyticsAsync(int tagId, DateTime fromDate, DateTime toDate);
        
        // Tag suggestions
        Task<IEnumerable<QuestionTagDto>> GetSuggestedTagsAsync(int questionId);
        Task<IEnumerable<QuestionTagDto>> GetRelatedTagsAsync(int tagId);
        Task<IEnumerable<string>> GetAutoCompleteTagSuggestionsAsync(string partialQuery);
        Task<IEnumerable<QuestionTagDto>> GetTagsByUsagePatternAsync(int questionId);
        
    // API-expected convenience methods
    Task<IEnumerable<QuestionTagDto>> GetAllAsync();
    Task<QuestionTagDto> GetByIdAsync(int id);
    Task<QuestionTagDto> GetByCodeAsync(string code);
    Task<QuestionTagDto> CreateAsync(CreateQuestionTagDto dto);
    Task<QuestionTagDto> UpdateAsync(int id, UpdateQuestionTagDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<QuestionTagDto>> GetByCategoryAsync(string category);
    Task<IEnumerable<QuestionTagDto>> GetActiveAsync();
    Task<IEnumerable<QuestionTagDto>> GetSystemTagsAsync();
    Task<IEnumerable<QuestionTagDto>> SearchAsync(TagSearchDto dto);
    // API-friendly overloads
    Task<IEnumerable<QuestionTagDto>> SearchAsync(string query, int limit = 20);
    Task<IEnumerable<QuestionTagDto>> GetSuggestionsAsync(string partialQuery);
    Task<IEnumerable<string>> GetSuggestionsAsync(string query, int limit = 10);
    Task<TagStatisticsDto> GetStatisticsAsync();
    Task<IEnumerable<PopularTagDto>> GetPopularAsync(int limit = 20);
    }
}
