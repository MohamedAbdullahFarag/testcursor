
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces
{
    public interface IQuestionTagRepository : IBaseRepository<QuestionTag>
    {
        // Tag retrieval
        Task<QuestionTag?> GetByIdWithDetailsAsync(int tagId);
        Task<QuestionTag?> GetByNameAsync(string name);
        Task<QuestionTag?> GetByNameAndCategoryAsync(string name, string category);
        Task<IEnumerable<QuestionTag>> GetByFilterAsync(TagFilterDto filter);
        Task<IEnumerable<QuestionTag>> GetByCategoryAsync(string category);
        
        // Tag listing
        Task<(IEnumerable<QuestionTag> Tags, int TotalCount)> GetPagedAsync(int page, int pageSize, string? whereClause = null, object? parameters = null);
        Task<IEnumerable<QuestionTag>> GetActiveTagsAsync();
        Task<IEnumerable<QuestionTag>> GetPopularTagsAsync(int limit = 20);
        Task<IEnumerable<QuestionTag>> GetRecentlyUsedTagsAsync(int limit = 20);
        Task<IEnumerable<QuestionTag>> GetTagsByCreatorAsync(int creatorId);
        
        // Tag management
        Task<QuestionTag> CreateTagAsync(QuestionTag tag);
        Task<bool> UpdateTagAsync(QuestionTag tag);
        Task<bool> DeleteTagAsync(int tagId);
        Task<bool> ArchiveTagAsync(int tagId);
        Task<bool> ActivateTagAsync(int tagId);
        
        // Tag operations
        Task<bool> MergeTagsAsync(int sourceTagId, int targetTagId);
        Task<bool> SplitTagAsync(int tagId, IEnumerable<string> newTagNames);
        Task<bool> RenameTagAsync(int tagId, string newName);
        Task<bool> ChangeTagCategoryAsync(int tagId, string newCategory);
        Task<bool> UpdateTagColorAsync(int tagId, string color);
        
        // Tag assignments
        Task<IEnumerable<QuestionTagAssignment>> GetTagAssignmentsAsync(int questionId);
        Task<bool> AssignTagToQuestionAsync(int questionId, int tagId, int assignedBy);
        Task<bool> RemoveTagFromQuestionAsync(int questionId, int tagId);
        Task<bool> BulkAssignTagsAsync(int questionId, IEnumerable<int> tagIds, int assignedBy);
        Task<bool> BulkRemoveTagsAsync(int questionId, IEnumerable<int> tagIds);
        
        // Tag analytics
        Task<TagUsageStatisticsDto> GetTagUsageStatisticsAsync(int tagId);
        Task<IEnumerable<PopularTagDto>> GetPopularTagsAsync(int limit = 20, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<TagTrendDto>> GetTagTrendsAsync(DateTime fromDate, DateTime toDate);
        Task<TagAnalyticsDto> GetTagAnalyticsAsync(int tagId, DateTime fromDate, DateTime toDate);
        
        // Tag suggestions
        Task<IEnumerable<QuestionTag>> GetSuggestedTagsAsync(int questionId, int limit = 10);
        Task<IEnumerable<QuestionTag>> GetRelatedTagsAsync(int tagId, int limit = 10);
        Task<IEnumerable<string>> GetAutoCompleteSuggestionsAsync(string partialQuery, int limit = 20);
        Task<IEnumerable<QuestionTag>> GetTagsByUsagePatternAsync(int questionId, int limit = 10);
        
        // Tag categories
        Task<IEnumerable<string>> GetTagCategoriesAsync();
        Task<IEnumerable<TagCategoryDto>> GetTagCategoriesWithCountsAsync();
        Task<bool> CreateTagCategoryAsync(string categoryName, string? description = null);
        Task<bool> DeleteTagCategoryAsync(string categoryName);
        Task<bool> RenameTagCategoryAsync(string oldName, string newName);
        
        // Tag cleanup
        Task<bool> CleanupUnusedTagsAsync();
        Task<bool> CleanupOrphanedTagAssignmentsAsync();
        Task<bool> OptimizeTagStorageAsync();
    }
}
