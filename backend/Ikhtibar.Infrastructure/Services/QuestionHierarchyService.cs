using AutoMapper;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// Service implementation for question hierarchy operations
/// Provides business logic for question-category relationships and analytics
/// </summary>
public class QuestionHierarchyService : IQuestionHierarchyService
{
    private readonly IQuestionHierarchyRepository _hierarchyRepository;
    private readonly IQuestionBankCategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<QuestionHierarchyService> _logger;

    public QuestionHierarchyService(
        IQuestionHierarchyRepository hierarchyRepository,
        IQuestionBankCategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<QuestionHierarchyService> logger)
    {
        _hierarchyRepository = hierarchyRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<QuestionCategorizationDto> AssignQuestionToCategoryAsync(CreateQuestionCategorizationDto dto, int? currentUserId = null)
    {
        using var scope = _logger.BeginScope("Assigning question {QuestionId} to category {CategoryId}", 
            dto.QuestionId, dto.CategoryId);

        try
        {
            // Validate the assignment
            await ValidateQuestionAssignmentAsync(dto);

            // Check if this should become the primary category
            var existingPrimary = await _hierarchyRepository.GetPrimaryCategoryAsync(dto.QuestionId);
            var isPrimary = dto.IsPrimary || existingPrimary == null;

            // Create the categorization
            var categorization = await _hierarchyRepository.AssignQuestionToCategoryAsync(
                dto.QuestionId,
                dto.CategoryId,
                isPrimary,
                currentUserId,
                dto.Weight,
                dto.ConfidenceScore);

            _logger.LogInformation("Question {QuestionId} assigned to category {CategoryId} successfully", 
                dto.QuestionId, dto.CategoryId);

            return _mapper.Map<QuestionCategorizationDto>(categorization);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign question {QuestionId} to category {CategoryId}", 
                dto.QuestionId, dto.CategoryId);
            throw;
        }
    }

    public async Task<bool> UnassignQuestionFromCategoryAsync(int questionId, int categoryId)
    {
        using var scope = _logger.BeginScope("Unassigning question {QuestionId} from category {CategoryId}", 
            questionId, categoryId);

        try
        {
            // Check if this is the primary category
            var primary = await _hierarchyRepository.GetPrimaryCategoryAsync(questionId);
            var wasPrimary = primary?.CategoryId == categoryId;

            var result = await _hierarchyRepository.RemoveQuestionFromCategoryAsync(questionId, categoryId);

            // If we removed the primary category, assign a new primary if other categories exist
            if (wasPrimary && result)
            {
                await EnsurePrimaryCategoryAsync(questionId);
            }

            _logger.LogInformation("Question {QuestionId} unassigned from category {CategoryId} successfully", 
                questionId, categoryId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unassign question {QuestionId} from category {CategoryId}", 
                questionId, categoryId);
            throw;
        }
    }

    public async Task<bool> SetPrimaryCategoryAsync(int questionId, int categoryId)
    {
        using var scope = _logger.BeginScope("Setting primary category for question {QuestionId} to {CategoryId}", 
            questionId, categoryId);

        try
        {
            // Validate that the categorization exists
            var categorizations = await _hierarchyRepository.GetQuestionCategoriesAsync(questionId);
            if (!categorizations.Any(c => c.Category.CategoryId == categoryId))
            {
                throw new InvalidOperationException($"Question {questionId} is not assigned to category {categoryId}");
            }

            var result = await _hierarchyRepository.SetPrimaryCategoryAsync(questionId, categoryId);
            
            _logger.LogInformation("Primary category set for question {QuestionId} to category {CategoryId} successfully", 
                questionId, categoryId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set primary category for question {QuestionId} to category {CategoryId}", 
                questionId, categoryId);
            throw;
        }
    }

    public async Task<QuestionCategorizationDto?> GetPrimaryCategoryAsync(int questionId)
    {
        try
        {
            var primaryCategorization = await _hierarchyRepository.GetPrimaryCategoryAsync(questionId);
            return primaryCategorization != null ? _mapper.Map<QuestionCategorizationDto>(primaryCategorization) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get primary category for question {QuestionId}", questionId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionCategorizationDto>> GetQuestionCategorizationsAsync(int questionId)
    {
        try
        {
            var categorizations = await _hierarchyRepository.GetQuestionCategorizationsAsync(questionId);
            return _mapper.Map<IEnumerable<QuestionCategorizationDto>>(categorizations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get categorizations for question {QuestionId}", questionId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionCategorizationDto>> GetCategoryQuestionsAsync(int categoryId)
    {
        try
        {
            var categorizations = await _hierarchyRepository.GetCategoryQuestionsAsync(categoryId);
            return _mapper.Map<IEnumerable<QuestionCategorizationDto>>(categorizations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get questions for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<int>> GetQuestionsWithoutPrimaryCategoryAsync()
    {
        try
        {
            return await _hierarchyRepository.GetQuestionsWithoutPrimaryCategoryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get questions without primary category");
            throw;
        }
    }

    public async Task<IEnumerable<int>> GetQuestionsWithMultiplePrimaryCategoriesAsync()
    {
        try
        {
            return await _hierarchyRepository.GetQuestionsWithMultiplePrimaryCategoriesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get questions with multiple primary categories");
            throw;
        }
    }

    public async Task<bool> BulkAssignQuestionsToCategoryAsync(BulkAssignQuestionsDto dto, int? currentUserId = null)
    {
        using var scope = _logger.BeginScope("Bulk assigning {Count} questions to category {CategoryId}", 
            dto.QuestionIds.Count(), dto.CategoryId);

        try
        {
            // Validate the category exists
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                throw new ArgumentException($"Category with ID {dto.CategoryId} not found", nameof(dto.CategoryId));
            }

            var result = await _hierarchyRepository.BulkAssignQuestionsToCategoryAsync(
                dto.QuestionIds, dto.CategoryId, currentUserId);

            // If setting as primary, update primary categories
            if (dto.SetAsPrimary && result)
            {
                foreach (var questionId in dto.QuestionIds)
                {
                    await _hierarchyRepository.SetPrimaryCategoryAsync(questionId, dto.CategoryId);
                }
            }

            _logger.LogInformation("Bulk assigned {Count} questions to category {CategoryId} successfully", 
                dto.QuestionIds.Count(), dto.CategoryId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to bulk assign questions to category {CategoryId}", dto.CategoryId);
            throw;
        }
    }

    public async Task<bool> BulkUnassignQuestionsFromCategoryAsync(IEnumerable<int> questionIds, int categoryId)
    {
        using var scope = _logger.BeginScope("Bulk unassigning {Count} questions from category {CategoryId}", 
            questionIds.Count(), categoryId);

        try
        {
            var result = await _hierarchyRepository.BulkUnassignQuestionsFromCategoryAsync(questionIds, categoryId);

            // Ensure each question still has a primary category
            if (result)
            {
                foreach (var questionId in questionIds)
                {
                    await EnsurePrimaryCategoryAsync(questionId);
                }
            }

            _logger.LogInformation("Bulk unassigned {Count} questions from category {CategoryId} successfully", 
                questionIds.Count(), categoryId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to bulk unassign questions from category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<bool> MoveQuestionsToNewCategoryAsync(MoveQuestionsDto dto)
    {
        using var scope = _logger.BeginScope("Moving {Count} questions from category {FromId} to {ToId}", 
            dto.QuestionIds.Count(), dto.FromCategoryId, dto.ToCategoryId);

        try
        {
            // Validate categories exist
            var fromCategory = await _categoryRepository.GetByIdAsync(dto.FromCategoryId);
            var toCategory = await _categoryRepository.GetByIdAsync(dto.ToCategoryId);

            if (fromCategory == null)
                throw new ArgumentException($"Source category {dto.FromCategoryId} not found");
            if (toCategory == null)
                throw new ArgumentException($"Target category {dto.ToCategoryId} not found");

            var result = await _hierarchyRepository.MoveQuestionsToNewCategoryAsync(
                dto.QuestionIds, dto.FromCategoryId, dto.ToCategoryId);

            _logger.LogInformation("Moved {Count} questions from category {FromId} to {ToId} successfully", 
                dto.QuestionIds.Count(), dto.FromCategoryId, dto.ToCategoryId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to move questions from category {FromId} to {ToId}", 
                dto.FromCategoryId, dto.ToCategoryId);
            throw;
        }
    }

    public async Task<QuestionHierarchyAnalyticsDto> GetCategoryAnalyticsAsync(int categoryId)
    {
        using var scope = _logger.BeginScope("Getting analytics for category {CategoryId}", categoryId);

        try
        {
            var questionCounts = await _hierarchyRepository.GetCategoryQuestionCountsAsync(new[] { categoryId });
            var difficultyDistribution = await _hierarchyRepository.GetQuestionDifficultyDistributionAsync(categoryId);
            var typeDistribution = await _hierarchyRepository.GetQuestionTypeDistributionAsync(categoryId);
            var averageDifficulty = await _hierarchyRepository.GetAverageQuestionDifficultyAsync(categoryId);

            return new QuestionHierarchyAnalyticsDto
            {
                CategoryQuestionCounts = questionCounts,
                DifficultyDistribution = difficultyDistribution,
                QuestionTypeDistribution = typeDistribution,
                AverageDifficulty = averageDifficulty,
                TotalQuestions = questionCounts.TryGetValue(categoryId, out var count) ? count : 0,
                AnalysisDate = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get analytics for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IDictionary<int, int>> GetCategoryQuestionCountsAsync(IEnumerable<int> categoryIds)
    {
        try
        {
            return await _hierarchyRepository.GetCategoryQuestionCountsAsync(categoryIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get question counts for categories");
            throw;
        }
    }

    public async Task<IDictionary<int, int>> GetQuestionDifficultyDistributionAsync(int categoryId)
    {
        try
        {
            return await _hierarchyRepository.GetQuestionDifficultyDistributionAsync(categoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get difficulty distribution for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IDictionary<int, int>> GetQuestionTypeDistributionAsync(int categoryId)
    {
        try
        {
            return await _hierarchyRepository.GetQuestionTypeDistributionAsync(categoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get type distribution for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<decimal> GetAverageQuestionDifficultyAsync(int categoryId)
    {
        try
        {
            return await _hierarchyRepository.GetAverageQuestionDifficultyAsync(categoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get average difficulty for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionCategorizationDto>> SearchQuestionCategorizationsAsync(
        string? searchTerm = null,
        int? categoryId = null,
        int? questionTypeId = null,
        int? difficultyLevelId = null,
        bool? isPrimary = null,
        int skip = 0,
        int take = 50)
    {
        try
        {
            var categorizations = await _hierarchyRepository.SearchQuestionCategorizationsAsync(
                searchTerm, categoryId, questionTypeId, difficultyLevelId, isPrimary, skip, take);
            
            return _mapper.Map<IEnumerable<QuestionCategorizationDto>>(categorizations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search question categorizations");
            throw;
        }
    }

    public async Task<int> GetSearchResultCountAsync(
        string? searchTerm = null,
        int? categoryId = null,
        int? questionTypeId = null,
        int? difficultyLevelId = null,
        bool? isPrimary = null)
    {
        try
        {
            return await _hierarchyRepository.GetSearchResultCountAsync(
                searchTerm, categoryId, questionTypeId, difficultyLevelId, isPrimary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get search result count");
            throw;
        }
    }

    public async Task<bool> ValidateQuestionCategoryRelationshipsAsync()
    {
        using var scope = _logger.BeginScope("Validating question-category relationships");

        try
        {
            var isValid = await _hierarchyRepository.ValidateQuestionCategoryRelationshipsAsync();
            
            if (!isValid)
            {
                _logger.LogWarning("Question-category relationship validation failed");
            }
            
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate question-category relationships");
            throw;
        }
    }

    public async Task<bool> FixInvalidQuestionCategoryRelationshipsAsync()
    {
        using var scope = _logger.BeginScope("Fixing invalid question-category relationships");

        try
        {
            var result = await _hierarchyRepository.FixInvalidQuestionCategoryRelationshipsAsync();
            
            _logger.LogInformation("Fixed invalid question-category relationships: {Success}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fix invalid question-category relationships");
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetValidationErrorsAsync()
    {
        try
        {
            var errors = new List<string>();
            
            var questionsWithoutPrimary = await GetQuestionsWithoutPrimaryCategoryAsync();
            if (questionsWithoutPrimary.Any())
            {
                errors.Add($"{questionsWithoutPrimary.Count()} questions without primary category");
            }
            
            var questionsWithMultiplePrimary = await GetQuestionsWithMultiplePrimaryCategoriesAsync();
            if (questionsWithMultiplePrimary.Any())
            {
                errors.Add($"{questionsWithMultiplePrimary.Count()} questions with multiple primary categories");
            }
            
            return errors;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get validation errors");
            throw;
        }
    }

    #region Private Helper Methods

    private async Task ValidateQuestionAssignmentAsync(CreateQuestionCategorizationDto dto)
    {
        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {dto.CategoryId} not found", nameof(dto.CategoryId));
        }

        if (!category.AllowQuestions)
        {
            throw new InvalidOperationException($"Category {dto.CategoryId} does not allow direct question assignment");
        }

        // Check if assignment already exists
        var existingCategorizations = await _hierarchyRepository.GetQuestionCategorizationsAsync(dto.QuestionId);
        if (existingCategorizations.Any(c => c.CategoryId == dto.CategoryId))
        {
            throw new InvalidOperationException($"Question {dto.QuestionId} is already assigned to category {dto.CategoryId}");
        }
    }

    private async Task EnsurePrimaryCategoryAsync(int questionId)
    {
        try
        {
            var primary = await _hierarchyRepository.GetPrimaryCategoryAsync(questionId);
            if (primary == null)
            {
                // Find any categorization for this question and make it primary
                var categorizations = await _hierarchyRepository.GetQuestionCategorizationsAsync(questionId);
                var firstCategorization = categorizations.FirstOrDefault();
                
                if (firstCategorization != null)
                {
                    await _hierarchyRepository.SetPrimaryCategoryAsync(questionId, firstCategorization.CategoryId);
                    _logger.LogInformation("Set primary category for question {QuestionId} to category {CategoryId}", 
                        questionId, firstCategorization.CategoryId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure primary category for question {QuestionId}", questionId);
        }
    }

    #endregion
}
