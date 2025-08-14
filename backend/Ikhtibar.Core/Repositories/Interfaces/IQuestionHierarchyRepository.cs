using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for managing question-category relationships and hierarchy data
/// Handles the many-to-many relationship between questions and categories
/// Provides efficient query operations using closure table pattern
/// </summary>
public interface IQuestionHierarchyRepository
{
    // Question-Category relationship operations
    /// <summary>
    /// Assign question to a category
    /// </summary>
    /// <param name="questionId">Question ID</param>
    /// <param name="categoryId">Category ID</param>
    /// <param name="isPrimary">Whether this is the primary category</param>
    /// <param name="assignedByUserId">User performing the assignment</param>
    /// <param name="weight">Weight for automatic categorization (optional)</param>
    /// <param name="confidenceScore">Confidence score for automatic categorization (optional)</param>
    /// <returns>Created categorization record</returns>
    Task<QuestionCategorization> AssignQuestionToCategoryAsync(
        int questionId, 
        int categoryId, 
        bool isPrimary = false, 
        int? assignedByUserId = null,
        decimal? weight = null,
        decimal? confidenceScore = null);

    /// <summary>
    /// Remove question from category
    /// </summary>
    /// <param name="questionId">Question ID</param>
    /// <param name="categoryId">Category ID</param>
    /// <returns>True if removed successfully</returns>
    Task<bool> RemoveQuestionFromCategoryAsync(int questionId, int categoryId);

    /// <summary>
    /// Update question categorization details
    /// </summary>
    /// <param name="questionId">Question ID</param>
    /// <param name="categoryId">Category ID</param>
    /// <param name="isPrimary">Whether this is the primary category</param>
    /// <param name="weight">Weight for automatic categorization</param>
    /// <param name="confidenceScore">Confidence score</param>
    /// <returns>Updated categorization record</returns>
    Task<QuestionCategorization?> UpdateQuestionCategorizationAsync(
        int questionId, 
        int categoryId, 
        bool? isPrimary = null,
        decimal? weight = null,
        decimal? confidenceScore = null);

    /// <summary>
    /// Set primary category for a question
    /// </summary>
    /// <param name="questionId">Question ID</param>
    /// <param name="primaryCategoryId">Primary category ID</param>
    /// <returns>True if set successfully</returns>
    Task<bool> SetPrimaryCategoryAsync(int questionId, int primaryCategoryId);

    // Question retrieval operations
    /// <summary>
    /// Get all categories for a specific question
    /// </summary>
    /// <param name="questionId">Question ID</param>
    /// <param name="includeInactive">Include inactive categories</param>
    /// <returns>Collection of categories with categorization details</returns>
    Task<IEnumerable<QuestionCategoryDetail>> GetQuestionCategoriesAsync(int questionId, bool includeInactive = false);

    /// <summary>
    /// Get primary category for a question
    /// </summary>
    /// <param name="questionId">Question ID</param>
    /// <returns>Primary category if set</returns>
    Task<QuestionBankCategory?> GetPrimaryCategoryAsync(int questionId);

    /// <summary>
    /// Get questions in a specific category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="includeDescendants">Include questions from descendant categories</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paged result of questions</returns>
    Task<PagedResult<QuestionCategoryDetail>> GetCategoryQuestionsAsync(
        int categoryId, 
        bool includeDescendants = false, 
        int pageNumber = 1, 
        int pageSize = 50);

    /// <summary>
    /// Get question count for category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="includeDescendants">Include questions from descendant categories</param>
    /// <returns>Total question count</returns>
    Task<int> GetCategoryQuestionCountAsync(int categoryId, bool includeDescendants = false);

    /// <summary>
    /// Get question distribution across categories
    /// </summary>
    /// <param name="parentCategoryId">Parent category to analyze (null for all)</param>
    /// <returns>Question count by category</returns>
    Task<IDictionary<int, int>> GetQuestionDistributionAsync(int? parentCategoryId = null);

    // Hierarchy operations using closure table
    /// <summary>
    /// Get all hierarchy entries for rebuilding
    /// </summary>
    /// <returns>All hierarchy entries</returns>
    Task<IEnumerable<QuestionBankHierarchy>> GetAllHierarchyEntriesAsync();

    /// <summary>
    /// Create hierarchy entry
    /// </summary>
    /// <param name="hierarchy">Hierarchy entry to create</param>
    /// <returns>Created hierarchy entry</returns>
    Task<QuestionBankHierarchy> CreateHierarchyEntryAsync(QuestionBankHierarchy hierarchy);

    /// <summary>
    /// Delete hierarchy entries for category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>Number of entries deleted</returns>
    Task<int> DeleteHierarchyEntriesAsync(int categoryId);

    /// <summary>
    /// Update hierarchy entries after category move
    /// </summary>
    /// <param name="categoryId">Category that was moved</param>
    /// <param name="oldParentId">Old parent ID</param>
    /// <param name="newParentId">New parent ID</param>
    /// <returns>Number of entries updated</returns>
    Task<int> UpdateHierarchyAfterMoveAsync(int categoryId, int? oldParentId, int? newParentId);

    // Bulk operations
    /// <summary>
    /// Assign multiple questions to a category
    /// </summary>
    /// <param name="questionIds">Question IDs to assign</param>
    /// <param name="categoryId">Target category ID</param>
    /// <param name="assignedByUserId">User performing the assignment</param>
    /// <returns>Number of questions assigned</returns>
    Task<int> AssignQuestionsToCategory(IEnumerable<int> questionIds, int categoryId, int? assignedByUserId = null);

    /// <summary>
    /// Remove multiple questions from category
    /// </summary>
    /// <param name="questionIds">Question IDs to remove</param>
    /// <param name="categoryId">Category ID</param>
    /// <returns>Number of questions removed</returns>
    Task<int> RemoveQuestionsFromCategory(IEnumerable<int> questionIds, int categoryId);

    /// <summary>
    /// Move questions from one category to another
    /// </summary>
    /// <param name="questionIds">Question IDs to move</param>
    /// <param name="fromCategoryId">Source category ID</param>
    /// <param name="toCategoryId">Target category ID</param>
    /// <param name="assignedByUserId">User performing the move</param>
    /// <returns>Number of questions moved</returns>
    Task<int> MoveQuestionsBetweenCategories(
        IEnumerable<int> questionIds, 
        int fromCategoryId, 
        int toCategoryId, 
        int? assignedByUserId = null);

    /// <summary>
    /// Copy questions from one category to another
    /// </summary>
    /// <param name="questionIds">Question IDs to copy</param>
    /// <param name="fromCategoryId">Source category ID</param>
    /// <param name="toCategoryId">Target category ID</param>
    /// <param name="assignedByUserId">User performing the copy</param>
    /// <returns>Number of questions copied</returns>
    Task<int> CopyQuestionsBetweenCategories(
        IEnumerable<int> questionIds, 
        int fromCategoryId, 
        int toCategoryId, 
        int? assignedByUserId = null);

    // Search and filtering operations
    /// <summary>
    /// Search questions by category criteria
    /// </summary>
    /// <param name="searchCriteria">Search criteria</param>
    /// <returns>Matching questions with category context</returns>
    Task<PagedResult<QuestionCategoryDetail>> SearchQuestionsByCategoryAsync(QuestionCategorySearchCriteria searchCriteria);

    /// <summary>
    /// Get uncategorized questions
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Questions without any category assignment</returns>
    Task<PagedResult<Question>> GetUncategorizedQuestionsAsync(int pageNumber = 1, int pageSize = 50);

    /// <summary>
    /// Get questions with multiple categories
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Questions assigned to multiple categories</returns>
    Task<PagedResult<QuestionCategoryDetail>> GetMultiCategorizedQuestionsAsync(int pageNumber = 1, int pageSize = 50);

    /// <summary>
    /// Get questions without primary category
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Questions without primary category designation</returns>
    Task<PagedResult<QuestionCategoryDetail>> GetQuestionsWithoutPrimaryCategoryAsync(int pageNumber = 1, int pageSize = 50);

    // Validation operations
    /// <summary>
    /// Check if question is assigned to category
    /// </summary>
    /// <param name="questionId">Question ID</param>
    /// <param name="categoryId">Category ID</param>
    /// <returns>True if assigned</returns>
    Task<bool> IsQuestionInCategoryAsync(int questionId, int categoryId);

    /// <summary>
    /// Check if question has any category assignments
    /// </summary>
    /// <param name="questionId">Question ID</param>
    /// <returns>True if has category assignments</returns>
    Task<bool> HasCategoryAssignmentsAsync(int questionId);

    /// <summary>
    /// Validate categorization integrity
    /// </summary>
    /// <returns>Validation result</returns>
    Task<CategorizationValidationResult> ValidateCategorizationIntegrityAsync();

    // Analytics and reporting
    /// <summary>
    /// Get categorization statistics
    /// </summary>
    /// <returns>Statistics about question categorization</returns>
    Task<CategorizationStatistics> GetCategorizationStatisticsAsync();

    /// <summary>
    /// Get category usage analytics
    /// </summary>
    /// <param name="fromDate">Start date for analysis</param>
    /// <param name="toDate">End date for analysis</param>
    /// <returns>Category usage analytics</returns>
    Task<IEnumerable<CategoryUsageAnalytics>> GetCategoryUsageAnalyticsAsync(DateTime? fromDate = null, DateTime? toDate = null);

    /// <summary>
    /// Get automatic categorization performance metrics
    /// </summary>
    /// <returns>Performance metrics for automatic categorization</returns>
    Task<AutoCategorizationMetrics> GetAutoCategorizationMetricsAsync();

    // Additional methods used by QuestionHierarchyService
    
    /// <summary>
    /// Get question categorizations for a specific question
    /// </summary>
    /// <param name="questionId">Question ID</param>
    /// <returns>Collection of question categorizations</returns>
    Task<IEnumerable<QuestionCategorization>> GetQuestionCategorizationsAsync(int questionId);

    /// <summary>
    /// Get questions without primary category
    /// </summary>
    /// <returns>Collection of question IDs without primary category</returns>
    Task<IEnumerable<int>> GetQuestionsWithoutPrimaryCategoryAsync();

    /// <summary>
    /// Get questions with multiple primary categories
    /// </summary>
    /// <returns>Collection of question IDs with multiple primary categories</returns>
    Task<IEnumerable<int>> GetQuestionsWithMultiplePrimaryCategoriesAsync();

    /// <summary>
    /// Bulk assign questions to category
    /// </summary>
    /// <param name="questionIds">Question IDs to assign</param>
    /// <param name="categoryId">Category ID</param>
    /// <param name="assignedByUserId">User performing assignment</param>
    /// <returns>True if successful</returns>
    Task<bool> BulkAssignQuestionsToCategoryAsync(IEnumerable<int> questionIds, int categoryId, int? assignedByUserId = null);

    /// <summary>
    /// Bulk unassign questions from category
    /// </summary>
    /// <param name="questionIds">Question IDs to unassign</param>
    /// <param name="categoryId">Category ID</param>
    /// <returns>True if successful</returns>
    Task<bool> BulkUnassignQuestionsFromCategoryAsync(IEnumerable<int> questionIds, int categoryId);

    /// <summary>
    /// Move questions to new category
    /// </summary>
    /// <param name="questionIds">Question IDs to move</param>
    /// <param name="fromCategoryId">Source category ID</param>
    /// <param name="toCategoryId">Target category ID</param>
    /// <returns>True if successful</returns>
    Task<bool> MoveQuestionsToNewCategoryAsync(IEnumerable<int> questionIds, int fromCategoryId, int toCategoryId);

    /// <summary>
    /// Get category question counts
    /// </summary>
    /// <param name="categoryIds">Category IDs</param>
    /// <returns>Dictionary of category ID to question count</returns>
    Task<IDictionary<int, int>> GetCategoryQuestionCountsAsync(IEnumerable<int> categoryIds);

    /// <summary>
    /// Get question difficulty distribution for category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>Dictionary of difficulty level to question count</returns>
    Task<IDictionary<int, int>> GetQuestionDifficultyDistributionAsync(int categoryId);

    /// <summary>
    /// Get question type distribution for category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>Dictionary of question type to question count</returns>
    Task<IDictionary<int, int>> GetQuestionTypeDistributionAsync(int categoryId);

    /// <summary>
    /// Get average question difficulty for category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>Average difficulty score</returns>
    Task<decimal> GetAverageQuestionDifficultyAsync(int categoryId);

    /// <summary>
    /// Search question categorizations
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="categoryId">Category ID filter</param>
    /// <param name="questionTypeId">Question type filter</param>
    /// <param name="difficultyLevelId">Difficulty level filter</param>
    /// <param name="isPrimary">Primary category filter</param>
    /// <param name="skip">Skip count</param>
    /// <param name="take">Take count</param>
    /// <returns>Collection of question categorizations</returns>
    Task<IEnumerable<QuestionCategorization>> SearchQuestionCategorizationsAsync(
        string? searchTerm = null,
        int? categoryId = null,
        int? questionTypeId = null,
        int? difficultyLevelId = null,
        bool? isPrimary = null,
        int skip = 0,
        int take = 50);

    /// <summary>
    /// Get search result count
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="categoryId">Category ID filter</param>
    /// <param name="questionTypeId">Question type filter</param>
    /// <param name="difficultyLevelId">Difficulty level filter</param>
    /// <param name="isPrimary">Primary category filter</param>
    /// <returns>Total count of search results</returns>
    Task<int> GetSearchResultCountAsync(
        string? searchTerm = null,
        int? categoryId = null,
        int? questionTypeId = null,
        int? difficultyLevelId = null,
        bool? isPrimary = null);

    /// <summary>
    /// Validate question category relationships
    /// </summary>
    /// <returns>True if valid</returns>
    Task<bool> ValidateQuestionCategoryRelationshipsAsync();

    /// <summary>
    /// Fix invalid question category relationships
    /// </summary>
    /// <returns>True if fixed</returns>
    Task<bool> FixInvalidQuestionCategoryRelationshipsAsync();
}

// Supporting models for question hierarchy operations

/// <summary>
/// Question with category details
/// </summary>
public class QuestionCategoryDetail
{
    public Question Question { get; set; } = null!;
    public QuestionBankCategory Category { get; set; } = null!;
    public QuestionCategorization Categorization { get; set; } = null!;
    public List<CategoryBreadcrumb> CategoryPath { get; set; } = new();
}

/// <summary>
/// Search criteria for questions by category
/// </summary>
public class QuestionCategorySearchCriteria
{
    public int? CategoryId { get; set; }
    public bool IncludeDescendants { get; set; } = false;
    public CategoryType? CategoryType { get; set; }
    public CategoryLevel? CategoryLevel { get; set; }
    public bool? IsPrimary { get; set; }
    public string? QuestionText { get; set; }
    public QuestionType? QuestionType { get; set; }
    public DifficultyLevel? DifficultyLevel { get; set; }
    public decimal? MinWeight { get; set; }
    public decimal? MinConfidenceScore { get; set; }
    public DateTime? AssignedAfter { get; set; }
    public DateTime? AssignedBefore { get; set; }
    public int? AssignedByUserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "Question.CreatedAt";
    public bool SortDescending { get; set; } = true;
}

/// <summary>
/// Categorization validation result
/// </summary>
public class CategorizationValidationResult
{
    public bool IsValid { get; set; }
    public List<CategorizationValidationIssue> Issues { get; set; } = new();
    public int OrphanedCategorizations { get; set; }
    public int DuplicateCategorizations { get; set; }
    public int InvalidHierarchyEntries { get; set; }
}

/// <summary>
/// Categorization validation issue
/// </summary>
public class CategorizationValidationIssue
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? QuestionId { get; set; }
    public int? CategoryId { get; set; }
    public string Severity { get; set; } = "Warning";
}

/// <summary>
/// Categorization statistics
/// </summary>
public class CategorizationStatistics
{
    public int TotalQuestions { get; set; }
    public int CategorizedQuestions { get; set; }
    public int UncategorizedQuestions { get; set; }
    public int QuestionsWithPrimaryCategory { get; set; }
    public int QuestionsWithMultipleCategories { get; set; }
    public double AverageCategoriesPerQuestion { get; set; }
    public int TotalCategorizations { get; set; }
    public int AutomaticCategorizations { get; set; }
    public int ManualCategorizations { get; set; }
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Category usage analytics
/// </summary>
public class CategoryUsageAnalytics
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryPath { get; set; } = string.Empty;
    public int QuestionCount { get; set; }
    public int NewQuestionsCount { get; set; }
    public int RemovedQuestionsCount { get; set; }
    public DateTime AnalysisPeriodStart { get; set; }
    public DateTime AnalysisPeriodEnd { get; set; }
    public double UsageGrowthRate { get; set; }
}

/// <summary>
/// Automatic categorization performance metrics
/// </summary>
public class AutoCategorizationMetrics
{
    public int TotalAutoCategorizationAttempts { get; set; }
    public int SuccessfulAutoCategorizationAttempts { get; set; }
    public double SuccessRate { get; set; }
    public double AverageConfidenceScore { get; set; }
    public double AverageWeight { get; set; }
    public int HighConfidenceCategorizations { get; set; } // > 0.8
    public int MediumConfidenceCategorizations { get; set; } // 0.5 - 0.8
    public int LowConfidenceCategorizations { get; set; } // < 0.5
    public DateTime LastCalculated { get; set; }
}

/// <summary>
/// Question type enumeration (referenced from Question entity)
/// </summary>
public enum QuestionType
{
    MultipleChoice = 1,
    TrueFalse = 2,
    Essay = 3,
    FillInBlank = 4,
    Matching = 5,
    Ordering = 6,
    Numerical = 7
}

/// <summary>
/// Difficulty level enumeration (referenced from Question entity)
/// </summary>
public enum DifficultyLevel
{
    Easy = 1,
    Medium = 2,
    Hard = 3,
    Expert = 4
}
