using Ikhtibar.Core.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for MediaCategory entity operations
/// Provides specialized methods for hierarchical category management
/// </summary>
public interface IMediaCategoryRepository : IRepository<MediaCategory>
{
    /// <summary>
    /// Gets all root categories (categories without parent)
    /// </summary>
    /// <param name="activeOnly">Whether to return only active categories</param>
    /// <returns>Collection of root categories</returns>
    Task<IEnumerable<MediaCategory>> GetRootCategoriesAsync(bool activeOnly = true);

    /// <summary>
    /// Gets child categories of a parent category
    /// </summary>
    /// <param name="parentCategoryId">Parent category identifier</param>
    /// <param name="activeOnly">Whether to return only active categories</param>
    /// <returns>Collection of child categories</returns>
    Task<IEnumerable<MediaCategory>> GetChildCategoriesAsync(Guid parentCategoryId, bool activeOnly = true);

    /// <summary>
    /// Gets all descendant categories of a parent category (recursive)
    /// </summary>
    /// <param name="parentCategoryId">Parent category identifier</param>
    /// <param name="activeOnly">Whether to return only active categories</param>
    /// <returns>Collection of all descendant categories</returns>
    Task<IEnumerable<MediaCategory>> GetDescendantCategoriesAsync(Guid parentCategoryId, bool activeOnly = true);

    /// <summary>
    /// Gets the full path of a category (all ancestors)
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <returns>Collection of categories from root to the specified category</returns>
    Task<IEnumerable<MediaCategory>> GetCategoryPathAsync(Guid categoryId);

    /// <summary>
    /// Gets a category by its slug
    /// </summary>
    /// <param name="slug">Category slug</param>
    /// <param name="activeOnly">Whether to return only active categories</param>
    /// <returns>Category if found, null otherwise</returns>
    Task<MediaCategory?> GetBySlugAsync(string slug, bool activeOnly = true);

    /// <summary>
    /// Gets categories by name (partial match)
    /// </summary>
    /// <param name="name">Category name to search for</param>
    /// <param name="activeOnly">Whether to return only active categories</param>
    /// <returns>Collection of matching categories</returns>
    Task<IEnumerable<MediaCategory>> GetByNameAsync(string name, bool activeOnly = true);

    /// <summary>
    /// Gets the hierarchical tree structure of all categories
    /// </summary>
    /// <param name="activeOnly">Whether to include only active categories</param>
    /// <returns>Root categories with their complete hierarchies</returns>
    Task<IEnumerable<MediaCategory>> GetCategoryTreeAsync(bool activeOnly = true);

    /// <summary>
    /// Checks if a category has any child categories
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <param name="activeOnly">Whether to check only active categories</param>
    /// <returns>True if the category has children</returns>
    Task<bool> HasChildrenAsync(Guid categoryId, bool activeOnly = true);

    /// <summary>
    /// Checks if a category has any media files
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <param name="includeDescendants">Whether to include files from descendant categories</param>
    /// <returns>True if the category has media files</returns>
    Task<bool> HasMediaFilesAsync(Guid categoryId, bool includeDescendants = false);

    /// <summary>
    /// Gets the count of media files in a category
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <param name="includeDescendants">Whether to include files from descendant categories</param>
    /// <returns>Number of media files in the category</returns>
    Task<int> GetMediaFileCountAsync(Guid categoryId, bool includeDescendants = false);

    /// <summary>
    /// Checks if a slug is available for a new category
    /// </summary>
    /// <param name="slug">Slug to check</param>
    /// <param name="excludeId">Optional category ID to exclude from check</param>
    /// <returns>True if the slug is available</returns>
    Task<bool> IsSlugAvailableAsync(string slug, Guid? excludeId = null);

    /// <summary>
    /// Updates the sort order of categories within a parent
    /// </summary>
    /// <param name="parentCategoryId">Parent category identifier (null for root level)</param>
    /// <param name="categoryIds">Ordered list of category IDs</param>
    /// <returns>Number of categories updated</returns>
    Task<int> UpdateSortOrderAsync(Guid? parentCategoryId, IEnumerable<Guid> categoryIds);

    /// <summary>
    /// Moves a category to a new parent
    /// </summary>
    /// <param name="categoryId">Category to move</param>
    /// <param name="newParentId">New parent category (null for root level)</param>
    /// <returns>True if moved successfully</returns>
    Task<bool> MoveCategoryAsync(Guid categoryId, Guid? newParentId);

    /// <summary>
    /// Gets categories with their media file statistics
    /// </summary>
    /// <param name="activeOnly">Whether to include only active categories</param>
    /// <returns>Categories with file count and total size information</returns>
    Task<IEnumerable<dynamic>> GetCategoriesWithStatsAsync(bool activeOnly = true);

    /// <summary>
    /// Searches categories by name, description, or metadata
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="activeOnly">Whether to search only active categories</param>
    /// <returns>Collection of matching categories</returns>
    Task<IEnumerable<MediaCategory>> SearchAsync(string searchTerm, bool activeOnly = true);
}
