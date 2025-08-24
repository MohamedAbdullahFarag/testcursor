using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for question difficulty level management
/// </summary>
public interface IQuestionDifficultyService
{
    /// <summary>
    /// Get all question difficulty levels
    /// </summary>
    Task<IEnumerable<QuestionDifficultyDto>> GetQuestionDifficultiesAsync();

    /// <summary>
    /// Get a specific question difficulty level by ID
    /// </summary>
    Task<QuestionDifficultyDto> GetQuestionDifficultyAsync(int id);

    /// <summary>
    /// Get question difficulty level by name
    /// </summary>
    Task<QuestionDifficultyDto> GetQuestionDifficultyByNameAsync(string name);

    /// <summary>
    /// Create a new question difficulty level
    /// </summary>
    Task<QuestionDifficultyDto> CreateQuestionDifficultyAsync(CreateQuestionDifficultyDto createDifficultyDto);

    /// <summary>
    /// Update an existing question difficulty level
    /// </summary>
    Task<QuestionDifficultyDto> UpdateQuestionDifficultyAsync(int id, UpdateQuestionDifficultyDto updateDifficultyDto);

    /// <summary>
    /// Delete a question difficulty level
    /// </summary>
    Task<bool> DeleteQuestionDifficultyAsync(int id);

    /// <summary>
    /// Get question difficulty levels by category
    /// </summary>
    Task<IEnumerable<QuestionDifficultyDto>> GetQuestionDifficultiesByCategoryAsync(string category);

    /// <summary>
    /// Get question difficulty levels by level
    /// </summary>
    Task<IEnumerable<QuestionDifficultyDto>> GetQuestionDifficultiesByLevelAsync(int level);

    /// <summary>
    /// Search question difficulty levels
    /// </summary>
    Task<IEnumerable<QuestionDifficultyDto>> SearchQuestionDifficultiesAsync(SearchQuestionDifficultiesDto searchDto);

    /// <summary>
    /// Get questions by difficulty level
    /// </summary>
    Task<IEnumerable<QuestionDto>> GetQuestionsByDifficultyAsync(int id, int page, int pageSize);

    /// <summary>
    /// Get difficulty level statistics
    /// </summary>
    Task<DifficultyStatisticsDto> GetDifficultyStatisticsAsync(int id);

    /// <summary>
    /// Get difficulty level analytics
    /// </summary>
    Task<DifficultyAnalyticsDto> GetDifficultyAnalyticsAsync(int id);

    /// <summary>
    /// Get difficulty level usage trends
    /// </summary>
    Task<IEnumerable<UsageTrendDto>> GetDifficultyUsageTrendsAsync(int id, DateTime? fromDate, DateTime? toDate);

    /// <summary>
    /// Get difficulty level categories
    /// </summary>
    Task<IEnumerable<string>> GetDifficultyCategoriesAsync();

    /// <summary>
    /// Get difficulty level suggestions
    /// </summary>
    Task<IEnumerable<string>> GetDifficultySuggestionsAsync(string query, int limit);

    /// <summary>
    /// Clone difficulty level
    /// </summary>
    Task<QuestionDifficultyDto> CloneDifficultyLevelAsync(int id, CloneDifficultyDto cloneDto);

    /// <summary>
    /// Get difficulty level distribution
    /// </summary>
    Task<IEnumerable<DifficultyDistributionDto>> GetDifficultyDistributionAsync();

    /// <summary>
    /// Get difficulty level performance metrics
    /// </summary>
    Task<IEnumerable<DifficultyPerformanceDto>> GetDifficultyPerformanceMetricsAsync();
    
    // Additional methods expected by API controllers
    Task<IEnumerable<QuestionDifficultyDto>> GetAllAsync();
    Task<QuestionDifficultyDto> GetByIdAsync(int id);
    Task<QuestionDifficultyDto> GetByNameAsync(string name);
    Task<QuestionDifficultyDto> CreateAsync(CreateQuestionDifficultyDto dto);
    Task<QuestionDifficultyDto> UpdateAsync(int id, UpdateQuestionDifficultyDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<QuestionDifficultyDto>> GetByCategoryAsync(string category);
    Task<IEnumerable<QuestionDifficultyDto>> GetByLevelRangeAsync(int minLevel, int maxLevel);
    Task<IEnumerable<QuestionDifficultyDto>> GetActiveAsync();
    Task<DifficultyStatisticsDto> GetStatisticsAsync();
}
