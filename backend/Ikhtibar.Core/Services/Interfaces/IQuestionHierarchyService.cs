using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for question hierarchy operations
/// Provides business logic for question-category relationships and analytics
/// </summary>
public interface IQuestionHierarchyService
{
    // Question-Category Assignment
    Task<QuestionCategorizationDto> AssignQuestionToCategoryAsync(CreateQuestionCategorizationDto dto, int? currentUserId = null);
    Task<bool> UnassignQuestionFromCategoryAsync(int questionId, int categoryId);
    Task<bool> SetPrimaryCategoryAsync(int questionId, int categoryId);
    Task<QuestionCategorizationDto?> GetPrimaryCategoryAsync(int questionId);

    // Question Categorization Queries
    Task<IEnumerable<QuestionCategorizationDto>> GetQuestionCategorizationsAsync(int questionId);
    Task<IEnumerable<QuestionCategorizationDto>> GetCategoryQuestionsAsync(int categoryId);
    Task<IEnumerable<int>> GetQuestionsWithoutPrimaryCategoryAsync();
    Task<IEnumerable<int>> GetQuestionsWithMultiplePrimaryCategoriesAsync();

    // Bulk Operations
    Task<bool> BulkAssignQuestionsToCategoryAsync(BulkAssignQuestionsDto dto, int? currentUserId = null);
    Task<bool> BulkUnassignQuestionsFromCategoryAsync(IEnumerable<int> questionIds, int categoryId);
    Task<bool> MoveQuestionsToNewCategoryAsync(MoveQuestionsDto dto);

    // Analytics and Statistics
    Task<QuestionHierarchyAnalyticsDto> GetCategoryAnalyticsAsync(int categoryId);
    Task<IDictionary<int, int>> GetCategoryQuestionCountsAsync(IEnumerable<int> categoryIds);
    Task<IDictionary<int, int>> GetQuestionDifficultyDistributionAsync(int categoryId);
    Task<IDictionary<int, int>> GetQuestionTypeDistributionAsync(int categoryId);
    Task<decimal> GetAverageQuestionDifficultyAsync(int categoryId);

    // Search and Filtering
    Task<IEnumerable<QuestionCategorizationDto>> SearchQuestionCategorizationsAsync(
        string? searchTerm = null,
        int? categoryId = null,
        int? questionTypeId = null,
        int? difficultyLevelId = null,
        bool? isPrimary = null,
        int skip = 0,
        int take = 50);
    Task<int> GetSearchResultCountAsync(
        string? searchTerm = null,
        int? categoryId = null,
        int? questionTypeId = null,
        int? difficultyLevelId = null,
        bool? isPrimary = null);

    // Validation and Maintenance
    Task<bool> ValidateQuestionCategoryRelationshipsAsync();
    Task<bool> FixInvalidQuestionCategoryRelationshipsAsync();
    Task<IEnumerable<string>> GetValidationErrorsAsync();
}
