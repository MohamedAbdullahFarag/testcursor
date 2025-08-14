using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for QuestionBankCategory entity operations
/// Provides comprehensive data access methods for hierarchical category management
/// Supports efficient tree operations using materialized path and closure table patterns
/// </summary>
public interface IQuestionBankCategoryRepository
{
    // Basic CRUD operations
    /// <summary>
    /// Get category by ID with full details
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <returns>Category if found, null otherwise</returns>
    Task<QuestionBankCategory?> GetByIdAsync(int categoryId);

    /// <summary>
    /// Get category by unique code
    /// </summary>
    /// <param name="code">Category code</param>
    /// <returns>Category if found, null otherwise</returns>
    Task<QuestionBankCategory?> GetByCodeAsync(string code);

    /// <summary>
    /// Get all categories with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paged result of categories</returns>
    Task<PagedResult<QuestionBankCategory>> GetAllAsync(int pageNumber = 1, int pageSize = 50);

    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="category">Category to create</param>
    /// <returns>Created category with generated ID</returns>
    Task<QuestionBankCategory> CreateAsync(QuestionBankCategory category);

    /// <summary>
    /// Update an existing category
    /// </summary>
    /// <param name="category">Category to update</param>
    /// <returns>Updated category</returns>
    Task<QuestionBankCategory> UpdateAsync(QuestionBankCategory category);

    /// <summary>
    /// Delete a category (soft delete)
    /// </summary>
    /// <param name="categoryId">Category ID to delete</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int categoryId);

    // Tree structure operations
    /// <summary>
    /// Get root categories (categories without parent)
    /// </summary>
    /// <returns>Collection of root categories</returns>
    Task<IEnumerable<QuestionBankCategory>> GetRootCategoriesAsync();

    /// <summary>
    /// Get direct children of a category
    /// </summary>
    /// <param name="parentId">Parent category ID</param>
    /// <returns>Collection of child categories</returns>
    Task<IEnumerable<QuestionBankCategory>> GetChildrenAsync(int parentId);

    /// <summary>
    /// Get all descendants of a category using materialized path
    /// </summary>
    /// <param name="categoryId">Category ID to get descendants for</param>
    /// <returns>Collection of descendant categories</returns>
    Task<IEnumerable<QuestionBankCategory>> GetDescendantsAsync(int categoryId);

    /// <summary>
    /// Get all ancestors of a category using materialized path
    /// </summary>
    /// <param name="categoryId">Category ID to get ancestors for</param>
    /// <returns>Collection of ancestor categories ordered by level</returns>
    Task<IEnumerable<QuestionBankCategory>> GetAncestorsAsync(int categoryId);

    /// <summary>
    /// Get categories by type
    /// </summary>
    /// <param name="categoryType">Category type to filter by</param>
    /// <returns>Collection of categories of specified type</returns>
    Task<IEnumerable<QuestionBankCategory>> GetByTypeAsync(CategoryType categoryType);

    /// <summary>
    /// Get categories by level
    /// </summary>
    /// <param name="level">Category level to filter by</param>
    /// <returns>Collection of categories at specified level</returns>
    Task<IEnumerable<QuestionBankCategory>> GetByLevelAsync(CategoryLevel level);

    // Path operations
    /// <summary>
    /// Update tree paths for categories after move operation
    /// </summary>
    /// <param name="oldPath">Old path pattern</param>
    /// <param name="newPath">New path pattern</param>
    /// <returns>Number of affected categories</returns>
    Task<int> UpdateTreePathsAsync(string oldPath, string newPath);

    /// <summary>
    /// Get category path from root to specified category
    /// </summary>
    /// <param name="categoryId">Target category ID</param>
    /// <returns>Ordered path from root to category</returns>
    Task<IEnumerable<QuestionBankCategory>> GetCategoryPathAsync(int categoryId);

    // Search and filtering
    /// <summary>
    /// Search categories by name or description
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="includeInactive">Whether to include inactive categories</param>
    /// <returns>Collection of matching categories</returns>
    Task<IEnumerable<QuestionBankCategory>> SearchAsync(string searchTerm, bool includeInactive = false);

    /// <summary>
    /// Filter categories by multiple criteria
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <returns>Paged result of filtered categories</returns>
    Task<PagedResult<QuestionBankCategory>> GetFilteredAsync(CategoryFilterCriteria filter);

    // Validation operations
    /// <summary>
    /// Check if category exists by ID
    /// </summary>
    /// <param name="categoryId">Category ID to check</param>
    /// <returns>True if exists</returns>
    Task<bool> ExistsAsync(int categoryId);

    /// <summary>
    /// Check if category code is unique
    /// </summary>
    /// <param name="code">Code to check</param>
    /// <param name="excludeCategoryId">Category ID to exclude from check</param>
    /// <returns>True if code is unique</returns>
    Task<bool> IsCodeUniqueAsync(string code, int? excludeCategoryId = null);

    /// <summary>
    /// Check if category has children
    /// </summary>
    /// <param name="categoryId">Category ID to check</param>
    /// <returns>True if has children</returns>
    Task<bool> HasChildrenAsync(int categoryId);

    /// <summary>
    /// Check if category has questions assigned
    /// </summary>
    /// <param name="categoryId">Category ID to check</param>
    /// <returns>True if has questions</returns>
    Task<bool> HasQuestionsAsync(int categoryId);

    // Ordering operations
    /// <summary>
    /// Get maximum sort order within parent
    /// </summary>
    /// <param name="parentId">Parent category ID (null for root level)</param>
    /// <returns>Maximum sort order</returns>
    Task<int> GetMaxSortOrderAsync(int? parentId);

    /// <summary>
    /// Reorder categories within parent
    /// </summary>
    /// <param name="parentId">Parent category ID</param>
    /// <param name="categoryOrders">Dictionary of category ID to new sort order</param>
    /// <returns>Number of categories reordered</returns>
    Task<int> ReorderCategoriesAsync(int? parentId, IDictionary<int, int> categoryOrders);

    // Batch operations
    /// <summary>
    /// Create multiple categories in batch
    /// </summary>
    /// <param name="categories">Categories to create</param>
    /// <returns>Created categories with generated IDs</returns>
    Task<IEnumerable<QuestionBankCategory>> CreateBatchAsync(IEnumerable<QuestionBankCategory> categories);

    /// <summary>
    /// Update multiple categories in batch
    /// </summary>
    /// <param name="categories">Categories to update</param>
    /// <returns>Number of categories updated</returns>
    Task<int> UpdateBatchAsync(IEnumerable<QuestionBankCategory> categories);

    /// <summary>
    /// Soft delete multiple categories
    /// </summary>
    /// <param name="categoryIds">Category IDs to delete</param>
    /// <returns>Number of categories deleted</returns>
    Task<int> DeleteBatchAsync(IEnumerable<int> categoryIds);
}

/// <summary>
/// Filter criteria for category queries
/// </summary>
public class CategoryFilterCriteria
{
    public CategoryType? Type { get; set; }
    public CategoryLevel? Level { get; set; }
    public int? ParentId { get; set; }
    public bool? IsActive { get; set; }
    public bool? AllowQuestions { get; set; }
    public string? Subject { get; set; }
    public string? GradeLevel { get; set; }
    public string? CurriculumCode { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "SortOrder";
    public bool SortDescending { get; set; } = false;
}
