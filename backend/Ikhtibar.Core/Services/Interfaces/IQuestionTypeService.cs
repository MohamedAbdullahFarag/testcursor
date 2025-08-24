using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for question type management
/// </summary>
public interface IQuestionTypeService
{
    /// <summary>
    /// Get all question types
    /// </summary>
    Task<IEnumerable<QuestionTypeDto>> GetQuestionTypesAsync();

    /// <summary>
    /// Get a specific question type by ID
    /// </summary>
    Task<QuestionTypeDto> GetQuestionTypeAsync(int id);

    /// <summary>
    /// Get question type by name
    /// </summary>
    Task<QuestionTypeDto> GetQuestionTypeByNameAsync(string name);

    /// <summary>
    /// Create a new question type
    /// </summary>
    Task<QuestionTypeDto> CreateQuestionTypeAsync(CreateQuestionTypeDto createTypeDto);

    /// <summary>
    /// Update an existing question type
    /// </summary>
    Task<QuestionTypeDto> UpdateQuestionTypeAsync(int id, UpdateQuestionTypeDto updateTypeDto);

    /// <summary>
    /// Delete a question type
    /// </summary>
    Task<bool> DeleteQuestionTypeAsync(int id);

    /// <summary>
    /// Get question types by category
    /// </summary>
    Task<IEnumerable<QuestionTypeDto>> GetQuestionTypesByCategoryAsync(string category);

    /// <summary>
    /// Get question types by difficulty level
    /// </summary>
    Task<IEnumerable<QuestionTypeDto>> GetQuestionTypesByDifficultyAsync(string difficultyLevel);

    /// <summary>
    /// Search question types
    /// </summary>
    Task<IEnumerable<QuestionTypeDto>> SearchQuestionTypesAsync(SearchQuestionTypesDto searchDto);

    /// <summary>
    /// Get question type templates
    /// </summary>
    Task<IEnumerable<QuestionTemplateDto>> GetQuestionTypeTemplatesAsync(int id);

    /// <summary>
    /// Get question type validation rules
    /// </summary>
    Task<IEnumerable<ValidationRuleDto>> GetQuestionTypeValidationRulesAsync(int id);

    /// <summary>
    /// Add validation rule to question type
    /// </summary>
    Task<bool> AddValidationRuleAsync(int id, AddValidationRuleDto addRuleDto);

    /// <summary>
    /// Remove validation rule from question type
    /// </summary>
    Task<bool> RemoveValidationRuleAsync(int id, int ruleId);

    /// <summary>
    /// Get question type statistics
    /// </summary>
    Task<QuestionTypeStatisticsDto> GetQuestionTypeStatisticsAsync(int id);

    /// <summary>
    /// Get question type analytics
    /// </summary>
    Task<QuestionTypeAnalyticsDto> GetQuestionTypeAnalyticsAsync(int id);

    /// <summary>
    /// Get question type usage trends
    /// </summary>
    Task<IEnumerable<UsageTrendDto>> GetQuestionTypeUsageTrendsAsync(int id, DateTime? fromDate, DateTime? toDate);

    /// <summary>
    /// Get question type categories
    /// </summary>
    Task<IEnumerable<string>> GetQuestionTypeCategoriesAsync();

    /// <summary>
    /// Get question type difficulty levels
    /// </summary>
    Task<IEnumerable<string>> GetQuestionTypeDifficultyLevelsAsync();

    /// <summary>
    /// Get question type suggestions
    /// </summary>
    Task<IEnumerable<string>> GetQuestionTypeSuggestionsAsync(string query, int limit);

    /// <summary>
    /// Clone question type
    /// </summary>
    Task<QuestionTypeDto> CloneQuestionTypeAsync(int id, CloneQuestionTypeDto cloneDto);

    // API-friendly method names
    Task<IEnumerable<QuestionTypeDto>> GetAllAsync();
    Task<QuestionTypeDto> GetByIdAsync(int id);
    Task<QuestionTypeDto> GetByCodeAsync(string code);
    Task<QuestionTypeDto> CreateAsync(CreateQuestionTypeDto dto);
    Task<QuestionTypeDto> UpdateAsync(int id, UpdateQuestionTypeDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<QuestionTypeDto>> GetByCategoryAsync(string category);
    Task<IEnumerable<QuestionTypeDto>> GetMultipleChoiceTypesAsync();
    Task<IEnumerable<QuestionTypeDto>> GetTrueFalseTypesAsync();
    Task<IEnumerable<QuestionTypeDto>> GetEssayTypesAsync();
    Task<IEnumerable<QuestionTypeDto>> GetNumericTypesAsync();
    Task<IEnumerable<QuestionTypeDto>> GetFileUploadTypesAsync();
    Task<IEnumerable<QuestionTypeDto>> GetActiveAsync();
    Task<QuestionTypeStatisticsDto> GetStatisticsAsync();
}
