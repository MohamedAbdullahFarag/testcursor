using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for tree-specific operations on question bank categories
/// Focuses on tree manipulation, navigation, and structural operations
/// Optimized for efficient tree traversal using materialized path and closure table patterns
/// </summary>
public interface IQuestionBankTreeRepository
{
    // Tree navigation operations
    /// <summary>
    /// Get complete tree structure starting from specified root
    /// </summary>
    /// <param name="rootId">Root category ID (null for entire tree)</param>
    /// <param name="maxDepth">Maximum depth to retrieve (null for unlimited)</param>
    /// <returns>Hierarchical tree structure</returns>
    Task<CategoryTreeNode?> GetTreeAsync(int? rootId = null, int? maxDepth = null);

    /// <summary>
    /// Get tree structure with question counts
    /// </summary>
    /// <param name="rootId">Root category ID (null for entire tree)</param>
    /// <param name="includeInactive">Whether to include inactive categories</param>
    /// <returns>Tree with question counts at each level</returns>
    Task<CategoryTreeNode?> GetTreeWithQuestionCountsAsync(int? rootId = null, bool includeInactive = false);

    /// <summary>
    /// Get subtree for specific category
    /// </summary>
    /// <param name="categoryId">Category ID to get subtree for</param>
    /// <param name="depth">Maximum depth of subtree</param>
    /// <returns>Subtree starting from specified category</returns>
    Task<CategoryTreeNode?> GetSubtreeAsync(int categoryId, int depth = 0);

    /// <summary>
    /// Get breadcrumb trail for a category
    /// </summary>
    /// <param name="categoryId">Target category ID</param>
    /// <returns>Ordered list from root to target category</returns>
    Task<IEnumerable<CategoryBreadcrumb>> GetBreadcrumbsAsync(int categoryId);

    // Tree manipulation operations
    /// <summary>
    /// Move category to new parent (including all descendants)
    /// </summary>
    /// <param name="categoryId">Category to move</param>
    /// <param name="newParentId">New parent category ID (null for root level)</param>
    /// <param name="newSortOrder">New sort order within parent</param>
    /// <returns>Move operation result</returns>
    Task<TreeMoveResult> MoveCategoryAsync(int categoryId, int? newParentId, int? newSortOrder = null);

    /// <summary>
    /// Copy category and optionally its descendants to new parent
    /// </summary>
    /// <param name="categoryId">Category to copy</param>
    /// <param name="newParentId">Target parent category ID</param>
    /// <param name="includeDescendants">Whether to copy descendants as well</param>
    /// <param name="newName">New name for copied category (optional)</param>
    /// <returns>Copy operation result with new category ID</returns>
    Task<TreeCopyResult> CopyCategoryAsync(int categoryId, int? newParentId, bool includeDescendants = true, string? newName = null);

    /// <summary>
    /// Delete category and handle children according to strategy
    /// </summary>
    /// <param name="categoryId">Category to delete</param>
    /// <param name="strategy">Strategy for handling children</param>
    /// <returns>Delete operation result</returns>
    Task<TreeDeleteResult> DeleteCategoryAsync(int categoryId, ChildHandlingStrategy strategy);

    // Tree validation operations
    /// <summary>
    /// Validate tree integrity and detect issues
    /// </summary>
    /// <returns>Validation result with any detected issues</returns>
    Task<TreeValidationResult> ValidateTreeIntegrityAsync();

    /// <summary>
    /// Check if move operation would create circular reference
    /// </summary>
    /// <param name="categoryId">Category to move</param>
    /// <param name="newParentId">Proposed new parent</param>
    /// <returns>True if move would create circular reference</returns>
    Task<bool> WouldCreateCircularReferenceAsync(int categoryId, int newParentId);

    /// <summary>
    /// Get depth of specific category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>Depth level (0 for root categories)</returns>
    Task<int> GetCategoryDepthAsync(int categoryId);

    /// <summary>
    /// Check if category is descendant of another category
    /// </summary>
    /// <param name="categoryId">Category to check</param>
    /// <param name="potentialAncestorId">Potential ancestor category</param>
    /// <returns>True if category is descendant</returns>
    Task<bool> IsDescendantOfAsync(int categoryId, int potentialAncestorId);

    // Tree statistics operations
    /// <summary>
    /// Get tree statistics for specified root
    /// </summary>
    /// <param name="rootId">Root category ID (null for entire tree)</param>
    /// <returns>Tree statistics including depth, node count, etc.</returns>
    Task<TreeStatistics> GetTreeStatisticsAsync(int? rootId = null);

    /// <summary>
    /// Get category distribution by type
    /// </summary>
    /// <returns>Distribution of categories by type</returns>
    Task<IDictionary<CategoryType, int>> GetCategoryDistributionAsync();

    /// <summary>
    /// Get category distribution by level
    /// </summary>
    /// <returns>Distribution of categories by level</returns>
    Task<IDictionary<CategoryLevel, int>> GetLevelDistributionAsync();

    // Tree maintenance operations
    /// <summary>
    /// Rebuild materialized paths for entire tree
    /// </summary>
    /// <returns>Number of paths updated</returns>
    Task<int> RebuildTreePathsAsync();

    /// <summary>
    /// Rebuild hierarchy table (closure table) for efficient queries
    /// </summary>
    /// <returns>Number of hierarchy entries created</returns>
    Task<int> RebuildHierarchyTableAsync();

    /// <summary>
    /// Optimize tree structure by fixing gaps in sort orders
    /// </summary>
    /// <param name="parentId">Parent to optimize (null for entire tree)</param>
    /// <returns>Number of categories reordered</returns>
    Task<int> OptimizeSortOrdersAsync(int? parentId = null);

    /// <summary>
    /// Compact tree by removing empty intermediate categories
    /// </summary>
    /// <returns>Number of categories removed</returns>
    Task<int> CompactTreeAsync();

    // Bulk operations
    /// <summary>
    /// Import category tree from structured data
    /// </summary>
    /// <param name="treeData">Tree structure to import</param>
    /// <param name="parentId">Parent category to import under</param>
    /// <param name="mergeStrategy">How to handle conflicts</param>
    /// <returns>Import result with created category mapping</returns>
    Task<TreeImportResult> ImportTreeAsync(CategoryTreeImport treeData, int? parentId = null, MergeStrategy mergeStrategy = MergeStrategy.Skip);

    /// <summary>
    /// Export category tree to structured format
    /// </summary>
    /// <param name="rootId">Root category to export (null for entire tree)</param>
    /// <param name="includeQuestionCounts">Whether to include question counts</param>
    /// <returns>Exportable tree structure</returns>
    Task<CategoryTreeExport> ExportTreeAsync(int? rootId = null, bool includeQuestionCounts = false);

    // Search and filtering
    /// <summary>
    /// Find categories matching criteria within tree structure
    /// </summary>
    /// <param name="searchCriteria">Search criteria</param>
    /// <returns>Matching categories with their tree context</returns>
    Task<IEnumerable<CategorySearchResult>> SearchTreeAsync(TreeSearchCriteria searchCriteria);

    /// <summary>
    /// Get recently modified categories in tree
    /// </summary>
    /// <param name="sinceDate">Date to search from</param>
    /// <param name="maxResults">Maximum number of results</param>
    /// <returns>Recently modified categories</returns>
    Task<IEnumerable<QuestionBankCategory>> GetRecentlyModifiedAsync(DateTime sinceDate, int maxResults = 100);
}
