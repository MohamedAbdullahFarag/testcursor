using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for category management operations
/// Provides business logic for category CRUD, validation, and analytics
/// </summary>
public interface ICategoryManagementService
{
    // CRUD Operations
    Task<QuestionBankCategoryDto> CreateCategoryAsync(CreateQuestionBankCategoryDto dto, int? currentUserId = null);
    Task<QuestionBankCategoryDto?> GetCategoryByIdAsync(int categoryId);
    Task<QuestionBankCategoryDto> UpdateCategoryAsync(int categoryId, UpdateQuestionBankCategoryDto dto, int? currentUserId = null);
    Task<bool> DeleteCategoryAsync(int categoryId, bool cascade = false, int? currentUserId = null);
    Task<bool> SoftDeleteCategoryAsync(int categoryId, int? currentUserId = null);
    Task<bool> RestoreCategoryAsync(int categoryId, int? currentUserId = null);

    // Category Queries
    Task<IEnumerable<QuestionBankCategoryDto>> GetAllCategoriesAsync(bool includeInactive = false);
    Task<IEnumerable<QuestionBankCategoryDto>> GetActiveCategoriesAsync();
    Task<IEnumerable<QuestionBankCategoryDto>> GetCategoriesByParentAsync(int? parentId);
    Task<IEnumerable<QuestionBankCategoryDto>> SearchCategoriesAsync(string searchTerm, int maxResults = 50);

    // Category Hierarchy
    Task<IEnumerable<QuestionBankCategoryDto>> GetCategoryPathAsync(int categoryId);
    Task<IEnumerable<QuestionBankCategoryDto>> GetCategoryAncestorsAsync(int categoryId);
    Task<IEnumerable<QuestionBankCategoryDto>> GetCategoryDescendantsAsync(int categoryId, int maxDepth = 10);
    Task<bool> IsCategoryAncestorOfAsync(int ancestorId, int descendantId);

    // Category Validation
    Task<bool> IsCategoryNameUniqueAsync(string name, int? parentId = null, int? excludeCategoryId = null);
    Task<bool> CanDeleteCategoryAsync(int categoryId);
    Task<bool> CanMoveCategoryAsync(int categoryId, int? newParentId);
    Task<IEnumerable<string>> ValidateCategoryAsync(CreateQuestionBankCategoryDto dto);
    Task<IEnumerable<string>> ValidateCategoryUpdateAsync(int categoryId, UpdateQuestionBankCategoryDto dto);

    // Category Management
    Task<bool> MoveCategoryAsync(int categoryId, int? newParentId, int? position = null, int? currentUserId = null);
    Task<bool> ReorderCategoriesAsync(int? parentId, IEnumerable<int> categoryIds, int? currentUserId = null);
    Task<bool> MergeCategoriesAsync(int sourceCategoryId, int targetCategoryId, int? currentUserId = null);
    Task<bool> SplitCategoryAsync(int categoryId, IEnumerable<int> questionIds, CreateQuestionBankCategoryDto newCategoryDto, int? currentUserId = null);

    // Bulk Operations
    Task<bool> BulkCreateCategoriesAsync(IEnumerable<CreateQuestionBankCategoryDto> dtos, int? currentUserId = null);
    Task<bool> BulkUpdateCategoriesAsync(IDictionary<int, UpdateQuestionBankCategoryDto> updates, int? currentUserId = null);
    Task<bool> BulkDeleteCategoriesAsync(IEnumerable<int> categoryIds, bool cascade = false, int? currentUserId = null);
    Task<bool> BulkActivateCategoriesAsync(IEnumerable<int> categoryIds, int? currentUserId = null);
    Task<bool> BulkDeactivateCategoriesAsync(IEnumerable<int> categoryIds, int? currentUserId = null);

    // Category Analytics
    Task<CategoryAnalyticsDto> GetCategoryAnalyticsAsync(int categoryId);
    Task<IEnumerable<CategoryUsageStatsDto>> GetCategoryUsageStatsAsync(int? parentId = null);
    Task<CategoryTreeStatsDto> GetCategoryTreeStatsAsync();

    // Import/Export
    Task<string> ExportCategoriesAsync(int? parentId = null, bool includeQuestions = false);
    Task<ImportResultDto> ImportCategoriesAsync(string data, int? parentId = null, int? currentUserId = null);
    Task<bool> ValidateImportDataAsync(string data);

    // Category Templates and Suggestions
    Task<IEnumerable<QuestionBankCategoryDto>> GetCategoryTemplatesAsync();
    Task<IEnumerable<string>> SuggestCategoryNamesAsync(string partialName, int? parentId = null);
    Task<QuestionBankCategoryDto> CreateCategoryFromTemplateAsync(int templateId, CreateQuestionBankCategoryDto dto, int? currentUserId = null);
}

/// <summary>
/// Category analytics DTO
/// </summary>
public class CategoryAnalyticsDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int DirectQuestionCount { get; set; }
    public int TotalQuestionCount { get; set; }
    public int SubcategoryCount { get; set; }
    public decimal AverageDifficulty { get; set; }
    public IDictionary<int, int> QuestionTypeDistribution { get; set; } = new Dictionary<int, int>();
    public IDictionary<int, int> DifficultyDistribution { get; set; } = new Dictionary<int, int>();
    public DateTime LastModified { get; set; }
    public DateTime AnalysisDate { get; set; }
}

/// <summary>
/// Category usage statistics DTO
/// </summary>
public class CategoryUsageStatsDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int QuestionCount { get; set; }
    public int ActiveQuestionCount { get; set; }
    public int ViewCount { get; set; }
    public int UsageCount { get; set; }
    public DateTime LastUsed { get; set; }
    public double PopularityScore { get; set; }
}

/// <summary>
/// Category tree statistics DTO
/// </summary>
public class CategoryTreeStatsDto
{
    public int TotalCategories { get; set; }
    public int ActiveCategories { get; set; }
    public int RootCategories { get; set; }
    public int MaxDepth { get; set; }
    public int TotalQuestions { get; set; }
    public double AverageQuestionsPerCategory { get; set; }
    public IDictionary<int, int> CategoriesPerLevel { get; set; } = new Dictionary<int, int>();
}

/// <summary>
/// Import result DTO
/// </summary>
public class ImportResultDto
{
    public bool Success { get; set; }
    public int ImportedCategories { get; set; }
    public int ImportedQuestions { get; set; }
    public int SkippedRecords { get; set; }
    public IEnumerable<string> Errors { get; set; } = new List<string>();
    public IEnumerable<string> Warnings { get; set; } = new List<string>();
}
