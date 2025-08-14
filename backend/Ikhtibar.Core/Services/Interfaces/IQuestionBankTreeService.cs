using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Service interface for question bank tree operations
/// Provides business logic for tree navigation, manipulation, and maintenance
/// </summary>
public interface IQuestionBankTreeService
{
    // Tree Navigation
    Task<QuestionBankTreeDto> GetTreeAsync(int? rootCategoryId = null, int maxDepth = 10);
    Task<IEnumerable<QuestionBankCategoryDto>> GetChildCategoriesAsync(int parentId);
    Task<QuestionBankCategoryDto?> GetParentCategoryAsync(int categoryId);
    Task<IEnumerable<QuestionBankCategoryDto>> GetAncestorsAsync(int categoryId);
    Task<IEnumerable<QuestionBankCategoryDto>> GetDescendantsAsync(int categoryId, int maxDepth = 10);
    Task<IEnumerable<QuestionBankCategoryDto>> GetSiblingsAsync(int categoryId);

    // Tree Manipulation
    Task<QuestionBankCategoryDto> CreateCategoryAsync(CreateQuestionBankCategoryDto dto);
    Task<QuestionBankCategoryDto> UpdateCategoryAsync(int categoryId, UpdateQuestionBankCategoryDto dto);
    Task<bool> DeleteCategoryAsync(int categoryId, bool cascade = false);
    Task<bool> MoveCategoryAsync(int categoryId, int? newParentId, int? insertAtPosition = null);
    Task<bool> ReorderCategoriesAsync(int parentId, IEnumerable<CategoryOrderDto> categoryOrders);

    // Tree Queries
    Task<QuestionBankCategoryDto?> GetCategoryByIdAsync(int categoryId);
    Task<IEnumerable<QuestionBankCategoryDto>> SearchCategoriesAsync(string searchTerm, int maxResults = 50);
    Task<QuestionBankCategoryDto?> FindCategoryByPathAsync(string treePath);
    Task<int> GetCategoryDepthAsync(int categoryId);
    Task<int> GetSubtreeQuestionCountAsync(int categoryId);

    // Tree Validation and Maintenance
    Task<TreeValidationResultDto> ValidateTreeIntegrityAsync();
    Task<bool> RebuildTreePathsAsync();
    Task<bool> RecalculateQuestionCountsAsync();
    Task<TreeStatisticsDto> GetTreeStatisticsAsync();

    // Bulk Operations
    Task<bool> BulkCreateCategoriesAsync(IEnumerable<CreateQuestionBankCategoryDto> categories);
    Task<bool> BulkMoveCategoriesAsync(IEnumerable<MoveCategoryDto> moves);
    Task<bool> BulkDeleteCategoriesAsync(IEnumerable<int> categoryIds, bool cascade = false);

    // Tree Export/Import
    Task<string> ExportTreeToJsonAsync(int? rootCategoryId = null);
    Task<bool> ImportTreeFromJsonAsync(string jsonData, int? parentCategoryId = null);
}

/// <summary>
/// DTOs for tree service operations
/// </summary>
public class CategoryOrderDto
{
    public int CategoryId { get; set; }
    public int Position { get; set; }
}

public class MoveCategoryDto
{
    public int CategoryId { get; set; }
    public int? NewParentId { get; set; }
    public int? Position { get; set; }
}

public class TreeValidationResultDto
{
    public bool IsValid { get; set; }
    public IEnumerable<string> Errors { get; set; } = new List<string>();
    public IEnumerable<string> Warnings { get; set; } = new List<string>();
}

public class TreeStatisticsDto
{
    public int TotalCategories { get; set; }
    public int MaxDepth { get; set; }
    public int TotalQuestions { get; set; }
    public IDictionary<int, int> CategoriesPerLevel { get; set; } = new Dictionary<int, int>();
    public IDictionary<int, int> QuestionsPerCategory { get; set; } = new Dictionary<int, int>();
}
