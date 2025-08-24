using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IQuestionSearchService
    {
        // Advanced search
    Task<PagedResult<QuestionDto>> SearchQuestionsAsync(QuestionSearchDto searchDto);
    Task<PagedResult<QuestionDto>> FullTextSearchAsync(string query, int page = 1, int pageSize = 20);
    Task<PagedResult<QuestionDto>> SearchByCriteriaAsync(QuestionCriteriaDto criteria);
    Task<PagedResult<QuestionDto>> SearchByTagsAsync(TagFilterDto tagFilter);
    Task<PagedResult<QuestionDto>> SearchByDifficultyAsync(int difficultyLevel, int page = 1, int pageSize = 20);
    Task<PagedResult<QuestionDto>> SearchByTypeAsync(int questionType, int page = 1, int pageSize = 20);
    Task<PagedResult<QuestionDto>> SearchByTreeNodeAsync(int treeNodeId, int page = 1, int pageSize = 20);
    Task<PagedResult<QuestionDto>> SearchByDateRangeAsync(DateTime fromDate, DateTime toDate, int page = 1, int pageSize = 20);
    Task<PagedResult<QuestionDto>> SearchByAuthorAsync(int authorId, int page = 1, int pageSize = 20);
    Task<PagedResult<QuestionDto>> SearchByStatusAsync(int status, int page = 1, int pageSize = 20);
        
        // Filtered search
        Task<PagedResult<QuestionDto>> GetQuestionsByFilterAsync(QuestionFilterDto filter);
        Task<IEnumerable<QuestionDto>> GetQuestionsByTreeNodeAsync(int treeNodeId, QuestionFilterDto filter);
        Task<IEnumerable<QuestionDto>> GetQuestionsByBankAsync(int questionBankId, QuestionFilterDto filter);
        Task<IEnumerable<QuestionDto>> GetQuestionsByTypeAsync(int questionTypeId, QuestionFilterDto filter);
        
    // Search analytics
    Task<SearchStatisticsDto> GetSearchStatisticsAsync();
    Task<IEnumerable<PopularSearchDto>> GetPopularSearchesAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<SearchTrendDto>> GetSearchTrendsAsync(DateTime fromDate, DateTime toDate);
    Task<SearchPerformanceDto> GetSearchPerformanceAsync(SearchPerformanceFilterDto filter);
        
    // Search suggestions
    Task<IEnumerable<string>> GetSearchSuggestionsAsync(string partialQuery, int limit = 10);
    Task<IEnumerable<PopularSearchTermDto>> GetPopularSearchTermsAsync(int limit = 20);
    Task<IEnumerable<string>> GetPopularTagsAsync(int limit = 20);
    Task<IEnumerable<string>> GetRelatedSearchesAsync(string searchQuery);
    Task<IEnumerable<string>> GetAutoCompleteSuggestionsAsync(string partialQuery);
        
    // Search optimization
    Task<bool> UpdateSearchIndexAsync(int questionId);
    Task<bool> RebuildSearchIndexAsync();
    Task<SearchIndexStatusDto> GetSearchIndexStatusAsync();
    Task<bool> OptimizeSearchIndexAsync();

    // Saved queries
    Task<bool> SaveSearchQueryAsync(SaveSearchQueryDto saveQueryDto);
    Task<IEnumerable<SavedSearchQueryDto>> GetSavedSearchQueriesAsync();
    Task<bool> DeleteSavedSearchQueryAsync(int queryId);
    }
}
