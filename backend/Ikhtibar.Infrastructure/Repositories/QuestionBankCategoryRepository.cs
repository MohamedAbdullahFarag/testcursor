using System.Data;
using Dapper;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Ikhtibar.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Dapper-based implementation of question bank category repository
/// Provides efficient data access operations for hierarchical category management
/// Uses materialized path pattern for optimal tree traversal performance
/// </summary>
public class QuestionBankCategoryRepository : IQuestionBankCategoryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<QuestionBankCategoryRepository> _logger;

    public QuestionBankCategoryRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<QuestionBankCategoryRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    // Basic CRUD operations
    public async Task<QuestionBankCategory?> GetByIdAsync(int categoryId)
    {
        using var scope = _logger.BeginScope("Getting category by ID {CategoryId}", categoryId);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT CategoryId, Name, Code, Description, ParentId, CategoryType, CategoryLevel, 
                       TreePath, IsActive, AllowQuestions, SortOrder, Subject, GradeLevel, 
                       CurriculumCode, Metadata, CreatedAt, ModifiedAt, CreatedByUserId, ModifiedByUserId, 
                       IsDeleted, DeletedAt, RowVersion
                FROM QuestionBankCategories 
                WHERE CategoryId = @CategoryId AND IsDeleted = 0";

            var category = await connection.QuerySingleOrDefaultAsync<QuestionBankCategory>(sql, new { CategoryId = categoryId });
            
            _logger.LogDebug("Retrieved category: {Found}", category != null ? "Found" : "Not Found");
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category by ID {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<QuestionBankCategory?> GetByCodeAsync(string code)
    {
        using var scope = _logger.BeginScope("Getting category by code {Code}", code);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT CategoryId, Name, Code, Description, ParentId, CategoryType, CategoryLevel, 
                       TreePath, IsActive, AllowQuestions, SortOrder, Subject, GradeLevel, 
                       CurriculumCode, Metadata, CreatedAt, ModifiedAt, CreatedByUserId, ModifiedByUserId, 
                       IsDeleted, DeletedAt, RowVersion
                FROM QuestionBankCategories 
                WHERE Code = @Code AND IsDeleted = 0";

            var category = await connection.QuerySingleOrDefaultAsync<QuestionBankCategory>(sql, new { Code = code });
            
            _logger.LogDebug("Retrieved category by code: {Found}", category != null ? "Found" : "Not Found");
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category by code {Code}", code);
            throw;
        }
    }

    public async Task<PagedResult<QuestionBankCategory>> GetAllAsync(int pageNumber = 1, int pageSize = 50)
    {
        using var scope = _logger.BeginScope("Getting all categories - Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // Calculate offset
            var offset = (pageNumber - 1) * pageSize;
            
            // Get total count
            const string countSql = "SELECT COUNT(*) FROM QuestionBankCategories WHERE IsDeleted = 0";
            var totalCount = await connection.QuerySingleAsync<int>(countSql);
            
            // Get paged data
            const string dataSql = @"
                SELECT CategoryId, Name, Code, Description, ParentId, CategoryType, CategoryLevel, 
                       TreePath, IsActive, AllowQuestions, SortOrder, Subject, GradeLevel, 
                       CurriculumCode, Metadata, CreatedAt, ModifiedAt, CreatedByUserId, ModifiedByUserId, 
                       IsDeleted, DeletedAt, RowVersion
                FROM QuestionBankCategories 
                WHERE IsDeleted = 0
                ORDER BY TreePath, SortOrder
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var categories = await connection.QueryAsync<QuestionBankCategory>(dataSql, new { Offset = offset, PageSize = pageSize });
            
            _logger.LogDebug("Retrieved {Count} categories out of {Total} total", categories.Count(), totalCount);
            
            return new PagedResult<QuestionBankCategory>
            {
                Items = categories.ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all categories");
            throw;
        }
    }

    public async Task<QuestionBankCategory> CreateAsync(QuestionBankCategory category)
    {
        using var scope = _logger.BeginScope("Creating category {Name}", category.Name);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // Set creation metadata
            category.CreatedAt = DateTime.UtcNow;
            category.IsDeleted = false;
            
            // Build tree path
            if (category.ParentId.HasValue)
            {
                var parent = await GetByIdAsync(category.ParentId.Value);
                if (parent == null)
                    throw new InvalidOperationException($"Parent category {category.ParentId} not found");
                    
                category.TreePath = $"{parent.TreePath}.{category.ParentId}";
            }
            else
            {
                category.TreePath = string.Empty;
            }
            
            // Set sort order if not specified
            if (category.SortOrder == 0)
            {
                var maxOrder = await GetMaxSortOrderAsync(category.ParentId);
                category.SortOrder = maxOrder + 1;
            }
            
            const string sql = @"
                INSERT INTO QuestionBankCategories 
                (Name, Code, Description, ParentId, CategoryType, CategoryLevel, TreePath, 
                 IsActive, AllowQuestions, SortOrder, Subject, GradeLevel, CurriculumCode, 
                 Metadata, CreatedAt, CreatedByUserId, IsDeleted)
                OUTPUT INSERTED.CategoryId
                VALUES 
                (@Name, @Code, @Description, @ParentId, @CategoryType, @CategoryLevel, @TreePath, 
                 @IsActive, @AllowQuestions, @SortOrder, @Subject, @GradeLevel, @CurriculumCode, 
                 @Metadata, @CreatedAt, @CreatedByUserId, @IsDeleted)";

            var categoryId = await connection.QuerySingleAsync<int>(sql, category);
            category.CategoryId = categoryId;
            
            _logger.LogInformation("Created category {CategoryId} with name {Name}", categoryId, category.Name);
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category {Name}", category.Name);
            throw;
        }
    }

    public async Task<QuestionBankCategory> UpdateAsync(QuestionBankCategory category)
    {
        using var scope = _logger.BeginScope("Updating category {CategoryId}", category.CategoryId);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // Set modification metadata
            category.ModifiedAt = DateTime.UtcNow;
            
            const string sql = @"
                UPDATE QuestionBankCategories 
                SET Name = @Name, Description = @Description, CategoryType = @CategoryType, 
                    CategoryLevel = @CategoryLevel, IsActive = @IsActive, AllowQuestions = @AllowQuestions, 
                    SortOrder = @SortOrder, Subject = @Subject, GradeLevel = @GradeLevel, 
                    CurriculumCode = @CurriculumCode, Metadata = @Metadata, ModifiedAt = @ModifiedAt, 
                    ModifiedByUserId = @ModifiedByUserId
                WHERE CategoryId = @CategoryId AND IsDeleted = 0";

            var rowsAffected = await connection.ExecuteAsync(sql, category);
            
            if (rowsAffected == 0)
                throw new InvalidOperationException($"Category {category.CategoryId} not found or was deleted");
            
            _logger.LogInformation("Updated category {CategoryId}", category.CategoryId);
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", category.CategoryId);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int categoryId)
    {
        using var scope = _logger.BeginScope("Deleting category {CategoryId}", categoryId);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // Check if category has children
            if (await HasChildrenAsync(categoryId))
                throw new InvalidOperationException($"Cannot delete category {categoryId} because it has children");
            
            // Check if category has questions
            if (await HasQuestionsAsync(categoryId))
                throw new InvalidOperationException($"Cannot delete category {categoryId} because it has assigned questions");
            
            const string sql = @"
                UPDATE QuestionBankCategories 
                SET IsDeleted = 1, DeletedAt = @DeletedAt 
                WHERE CategoryId = @CategoryId AND IsDeleted = 0";

            var rowsAffected = await connection.ExecuteAsync(sql, new { CategoryId = categoryId, DeletedAt = DateTime.UtcNow });
            
            var success = rowsAffected > 0;
            _logger.LogInformation("Delete category {CategoryId}: {Result}", categoryId, success ? "Success" : "Not Found");
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", categoryId);
            throw;
        }
    }

    // Tree structure operations
    public async Task<IEnumerable<QuestionBankCategory>> GetRootCategoriesAsync()
    {
        using var scope = _logger.BeginScope("Getting root categories");
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT CategoryId, Name, Code, Description, ParentId, CategoryType, CategoryLevel, 
                       TreePath, IsActive, AllowQuestions, SortOrder, Subject, GradeLevel, 
                       CurriculumCode, Metadata, CreatedAt, ModifiedAt, CreatedByUserId, ModifiedByUserId, 
                       IsDeleted, DeletedAt, RowVersion
                FROM QuestionBankCategories 
                WHERE ParentId IS NULL AND IsDeleted = 0
                ORDER BY SortOrder";

            var categories = await connection.QueryAsync<QuestionBankCategory>(sql);
            
            _logger.LogDebug("Retrieved {Count} root categories", categories.Count());
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving root categories");
            throw;
        }
    }

    public async Task<IEnumerable<QuestionBankCategory>> GetChildrenAsync(int parentId)
    {
        using var scope = _logger.BeginScope("Getting children for category {ParentId}", parentId);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT CategoryId, Name, Code, Description, ParentId, CategoryType, CategoryLevel, 
                       TreePath, IsActive, AllowQuestions, SortOrder, Subject, GradeLevel, 
                       CurriculumCode, Metadata, CreatedAt, ModifiedAt, CreatedByUserId, ModifiedByUserId, 
                       IsDeleted, DeletedAt, RowVersion
                FROM QuestionBankCategories 
                WHERE ParentId = @ParentId AND IsDeleted = 0
                ORDER BY SortOrder";

            var categories = await connection.QueryAsync<QuestionBankCategory>(sql, new { ParentId = parentId });
            
            _logger.LogDebug("Retrieved {Count} children for category {ParentId}", categories.Count(), parentId);
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving children for category {ParentId}", parentId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionBankCategory>> GetDescendantsAsync(int categoryId)
    {
        using var scope = _logger.BeginScope("Getting descendants for category {CategoryId}", categoryId);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // Get the category's tree path
            var category = await GetByIdAsync(categoryId);
            if (category == null)
                return Enumerable.Empty<QuestionBankCategory>();
            
            // Build path pattern for descendants
            var pathPattern = string.IsNullOrEmpty(category.TreePath) 
                ? $"%{categoryId}%" 
                : $"{category.TreePath}.{categoryId}%";
            
            const string sql = @"
                SELECT CategoryId, Name, Code, Description, ParentId, CategoryType, CategoryLevel, 
                       TreePath, IsActive, AllowQuestions, SortOrder, Subject, GradeLevel, 
                       CurriculumCode, Metadata, CreatedAt, ModifiedAt, CreatedByUserId, ModifiedByUserId, 
                       IsDeleted, DeletedAt, RowVersion
                FROM QuestionBankCategories 
                WHERE TreePath LIKE @PathPattern AND CategoryId != @CategoryId AND IsDeleted = 0
                ORDER BY TreePath, SortOrder";

            var categories = await connection.QueryAsync<QuestionBankCategory>(sql, new { PathPattern = pathPattern, CategoryId = categoryId });
            
            _logger.LogDebug("Retrieved {Count} descendants for category {CategoryId}", categories.Count(), categoryId);
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving descendants for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionBankCategory>> GetAncestorsAsync(int categoryId)
    {
        using var scope = _logger.BeginScope("Getting ancestors for category {CategoryId}", categoryId);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // Get the category's tree path
            var category = await GetByIdAsync(categoryId);
            if (category == null || string.IsNullOrEmpty(category.TreePath))
                return Enumerable.Empty<QuestionBankCategory>();
            
            // Parse ancestor IDs from tree path
            var ancestorIds = category.TreePath.Split('.', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(int.Parse)
                                             .ToList();
            
            if (!ancestorIds.Any())
                return Enumerable.Empty<QuestionBankCategory>();
            
            const string sql = @"
                SELECT CategoryId, Name, Code, Description, ParentId, CategoryType, CategoryLevel, 
                       TreePath, IsActive, AllowQuestions, SortOrder, Subject, GradeLevel, 
                       CurriculumCode, Metadata, CreatedAt, ModifiedAt, CreatedByUserId, ModifiedByUserId, 
                       IsDeleted, DeletedAt, RowVersion
                FROM QuestionBankCategories 
                WHERE CategoryId IN @AncestorIds AND IsDeleted = 0
                ORDER BY LEN(TreePath)";

            var categories = await connection.QueryAsync<QuestionBankCategory>(sql, new { AncestorIds = ancestorIds });
            
            _logger.LogDebug("Retrieved {Count} ancestors for category {CategoryId}", categories.Count(), categoryId);
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ancestors for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionBankCategory>> GetByTypeAsync(CategoryType categoryType)
    {
        using var scope = _logger.BeginScope("Getting categories by type {CategoryType}", categoryType);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT CategoryId, Name, Code, Description, ParentId, CategoryType, CategoryLevel, 
                       TreePath, IsActive, AllowQuestions, SortOrder, Subject, GradeLevel, 
                       CurriculumCode, Metadata, CreatedAt, ModifiedAt, CreatedByUserId, ModifiedByUserId, 
                       IsDeleted, DeletedAt, RowVersion
                FROM QuestionBankCategories 
                WHERE CategoryType = @CategoryType AND IsDeleted = 0
                ORDER BY TreePath, SortOrder";

            var categories = await connection.QueryAsync<QuestionBankCategory>(sql, new { CategoryType = (int)categoryType });
            
            _logger.LogDebug("Retrieved {Count} categories of type {CategoryType}", categories.Count(), categoryType);
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories by type {CategoryType}", categoryType);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionBankCategory>> GetByLevelAsync(CategoryLevel level)
    {
        using var scope = _logger.BeginScope("Getting categories by level {Level}", level);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                SELECT CategoryId, Name, Code, Description, ParentId, CategoryType, CategoryLevel, 
                       TreePath, IsActive, AllowQuestions, SortOrder, Subject, GradeLevel, 
                       CurriculumCode, Metadata, CreatedAt, ModifiedAt, CreatedByUserId, ModifiedByUserId, 
                       IsDeleted, DeletedAt, RowVersion
                FROM QuestionBankCategories 
                WHERE CategoryLevel = @Level AND IsDeleted = 0
                ORDER BY TreePath, SortOrder";

            var categories = await connection.QueryAsync<QuestionBankCategory>(sql, new { Level = (int)level });
            
            _logger.LogDebug("Retrieved {Count} categories at level {Level}", categories.Count(), level);
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories by level {Level}", level);
            throw;
        }
    }

    // Path operations
    public async Task<int> UpdateTreePathsAsync(string oldPath, string newPath)
    {
        using var scope = _logger.BeginScope("Updating tree paths from {OldPath} to {NewPath}", oldPath, newPath);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = @"
                UPDATE QuestionBankCategories 
                SET TreePath = REPLACE(TreePath, @OldPath, @NewPath),
                    ModifiedAt = @ModifiedAt
                WHERE TreePath LIKE @OldPathPattern AND IsDeleted = 0";

            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                OldPath = oldPath, 
                NewPath = newPath, 
                OldPathPattern = $"{oldPath}%",
                ModifiedAt = DateTime.UtcNow 
            });
            
            _logger.LogInformation("Updated {Count} tree paths", rowsAffected);
            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tree paths from {OldPath} to {NewPath}", oldPath, newPath);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionBankCategory>> GetCategoryPathAsync(int categoryId)
    {
        using var scope = _logger.BeginScope("Getting category path for {CategoryId}", categoryId);
        
        try
        {
            var ancestors = await GetAncestorsAsync(categoryId);
            var category = await GetByIdAsync(categoryId);
            
            var path = ancestors.ToList();
            if (category != null)
                path.Add(category);
            
            _logger.LogDebug("Retrieved path with {Count} categories for {CategoryId}", path.Count, categoryId);
            return path.OrderBy(c => c.TreePath.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category path for {CategoryId}", categoryId);
            throw;
        }
    }

    // Search and filtering
    public async Task<IEnumerable<QuestionBankCategory>> SearchAsync(string searchTerm, bool includeInactive = false)
    {
        using var scope = _logger.BeginScope("Searching categories with term {SearchTerm}", searchTerm);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var whereClause = includeInactive ? "IsDeleted = 0" : "IsDeleted = 0 AND IsActive = 1";
            
            var sql = $@"
                SELECT CategoryId, Name, Code, Description, ParentId, CategoryType, CategoryLevel, 
                       TreePath, IsActive, AllowQuestions, SortOrder, Subject, GradeLevel, 
                       CurriculumCode, Metadata, CreatedAt, ModifiedAt, CreatedByUserId, ModifiedByUserId, 
                       IsDeleted, DeletedAt, RowVersion
                FROM QuestionBankCategories 
                WHERE {whereClause} 
                  AND (Name LIKE @SearchTerm OR Description LIKE @SearchTerm OR Code LIKE @SearchTerm)
                ORDER BY TreePath, SortOrder";

            var searchPattern = $"%{searchTerm}%";
            var categories = await connection.QueryAsync<QuestionBankCategory>(sql, new { SearchTerm = searchPattern });
            
            _logger.LogDebug("Found {Count} categories matching {SearchTerm}", categories.Count(), searchTerm);
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching categories with term {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<PagedResult<QuestionBankCategory>> GetFilteredAsync(CategoryFilterCriteria filter)
    {
        using var scope = _logger.BeginScope("Getting filtered categories");
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // Build WHERE clause
            var conditions = new List<string> { "IsDeleted = 0" };
            var parameters = new DynamicParameters();
            
            if (filter.Type.HasValue)
            {
                conditions.Add("CategoryType = @Type");
                parameters.Add("Type", (int)filter.Type.Value);
            }
            
            if (filter.Level.HasValue)
            {
                conditions.Add("CategoryLevel = @Level");
                parameters.Add("Level", (int)filter.Level.Value);
            }
            
            if (filter.ParentId.HasValue)
            {
                conditions.Add("ParentId = @ParentId");
                parameters.Add("ParentId", filter.ParentId.Value);
            }
            
            if (filter.IsActive.HasValue)
            {
                conditions.Add("IsActive = @IsActive");
                parameters.Add("IsActive", filter.IsActive.Value);
            }
            
            if (filter.AllowQuestions.HasValue)
            {
                conditions.Add("AllowQuestions = @AllowQuestions");
                parameters.Add("AllowQuestions", filter.AllowQuestions.Value);
            }
            
            if (!string.IsNullOrEmpty(filter.Subject))
            {
                conditions.Add("Subject = @Subject");
                parameters.Add("Subject", filter.Subject);
            }
            
            if (!string.IsNullOrEmpty(filter.GradeLevel))
            {
                conditions.Add("GradeLevel = @GradeLevel");
                parameters.Add("GradeLevel", filter.GradeLevel);
            }
            
            if (!string.IsNullOrEmpty(filter.CurriculumCode))
            {
                conditions.Add("CurriculumCode = @CurriculumCode");
                parameters.Add("CurriculumCode", filter.CurriculumCode);
            }
            
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                conditions.Add("(Name LIKE @SearchTerm OR Description LIKE @SearchTerm OR Code LIKE @SearchTerm)");
                parameters.Add("SearchTerm", $"%{filter.SearchTerm}%");
            }
            
            var whereClause = string.Join(" AND ", conditions);
            
            // Build ORDER BY clause
            var orderByClause = filter.SortBy switch
            {
                "Name" => "Name",
                "Code" => "Code",
                "CreatedAt" => "CreatedAt",
                "ModifiedAt" => "ModifiedAt",
                _ => "TreePath, SortOrder"
            };
            
            if (filter.SortDescending)
                orderByClause += " DESC";
            
            // Calculate offset
            var offset = (filter.PageNumber - 1) * filter.PageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", filter.PageSize);
            
            // Get total count
            var countSql = $"SELECT COUNT(*) FROM QuestionBankCategories WHERE {whereClause}";
            var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);
            
            // Get paged data
            var dataSql = $@"
                SELECT CategoryId, Name, Code, Description, ParentId, CategoryType, CategoryLevel, 
                       TreePath, IsActive, AllowQuestions, SortOrder, Subject, GradeLevel, 
                       CurriculumCode, Metadata, CreatedAt, ModifiedAt, CreatedByUserId, ModifiedByUserId, 
                       IsDeleted, DeletedAt, RowVersion
                FROM QuestionBankCategories 
                WHERE {whereClause}
                ORDER BY {orderByClause}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var categories = await connection.QueryAsync<QuestionBankCategory>(dataSql, parameters);
            
            _logger.LogDebug("Retrieved {Count} filtered categories out of {Total} total", categories.Count(), totalCount);
            
            return new PagedResult<QuestionBankCategory>
            {
                Items = categories.ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving filtered categories");
            throw;
        }
    }

    // Validation operations
    public async Task<bool> ExistsAsync(int categoryId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = "SELECT COUNT(1) FROM QuestionBankCategories WHERE CategoryId = @CategoryId AND IsDeleted = 0";
            var count = await connection.QuerySingleAsync<int>(sql, new { CategoryId = categoryId });
            
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if category exists {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<bool> IsCodeUniqueAsync(string code, int? excludeCategoryId = null)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            string sql = "SELECT COUNT(1) FROM QuestionBankCategories WHERE Code = @Code AND IsDeleted = 0";
            object parameters = new { Code = code };
            
            if (excludeCategoryId.HasValue)
            {
                sql += " AND CategoryId != @ExcludeCategoryId";
                parameters = new { Code = code, ExcludeCategoryId = excludeCategoryId.Value };
            }
            
            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking code uniqueness for {Code}", code);
            throw;
        }
    }

    public async Task<bool> HasChildrenAsync(int categoryId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = "SELECT COUNT(1) FROM QuestionBankCategories WHERE ParentId = @CategoryId AND IsDeleted = 0";
            var count = await connection.QuerySingleAsync<int>(sql, new { CategoryId = categoryId });
            
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if category has children {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<bool> HasQuestionsAsync(int categoryId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            const string sql = "SELECT COUNT(1) FROM QuestionCategorizations WHERE CategoryId = @CategoryId";
            var count = await connection.QuerySingleAsync<int>(sql, new { CategoryId = categoryId });
            
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if category has questions {CategoryId}", categoryId);
            throw;
        }
    }

    // Ordering operations
    public async Task<int> GetMaxSortOrderAsync(int? parentId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            string sql;
            object parameters;
            
            if (parentId.HasValue)
            {
                sql = "SELECT ISNULL(MAX(SortOrder), 0) FROM QuestionBankCategories WHERE ParentId = @ParentId AND IsDeleted = 0";
                parameters = new { ParentId = parentId.Value };
            }
            else
            {
                sql = "SELECT ISNULL(MAX(SortOrder), 0) FROM QuestionBankCategories WHERE ParentId IS NULL AND IsDeleted = 0";
                parameters = new { };
            }
            
            var maxOrder = await connection.QuerySingleAsync<int>(sql, parameters);
            return maxOrder;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting max sort order for parent {ParentId}", parentId);
            throw;
        }
    }

    public async Task<int> ReorderCategoriesAsync(int? parentId, IDictionary<int, int> categoryOrders)
    {
        using var scope = _logger.BeginScope("Reordering categories for parent {ParentId}", parentId);
        
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();
            
            var updated = 0;
            
            foreach (var kvp in categoryOrders)
            {
                const string sql = @"
                    UPDATE QuestionBankCategories 
                    SET SortOrder = @SortOrder, ModifiedAt = @ModifiedAt 
                    WHERE CategoryId = @CategoryId AND ParentId = @ParentId AND IsDeleted = 0";

                var rowsAffected = await connection.ExecuteAsync(sql, new 
                { 
                    CategoryId = kvp.Key, 
                    SortOrder = kvp.Value, 
                    ParentId = parentId,
                    ModifiedAt = DateTime.UtcNow 
                }, transaction);
                
                updated += rowsAffected;
            }
            
            transaction.Commit();
            
            _logger.LogInformation("Reordered {Count} categories for parent {ParentId}", updated, parentId);
            return updated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering categories for parent {ParentId}", parentId);
            throw;
        }
    }

    // Batch operations
    public async Task<IEnumerable<QuestionBankCategory>> CreateBatchAsync(IEnumerable<QuestionBankCategory> categories)
    {
        using var scope = _logger.BeginScope("Creating batch of {Count} categories", categories.Count());
        
        try
        {
            var createdCategories = new List<QuestionBankCategory>();
            
            foreach (var category in categories)
            {
                var created = await CreateAsync(category);
                createdCategories.Add(created);
            }
            
            _logger.LogInformation("Created batch of {Count} categories", createdCategories.Count);
            return createdCategories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating batch of categories");
            throw;
        }
    }

    public async Task<int> UpdateBatchAsync(IEnumerable<QuestionBankCategory> categories)
    {
        using var scope = _logger.BeginScope("Updating batch of {Count} categories", categories.Count());
        
        try
        {
            var updated = 0;
            
            foreach (var category in categories)
            {
                await UpdateAsync(category);
                updated++;
            }
            
            _logger.LogInformation("Updated batch of {Count} categories", updated);
            return updated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating batch of categories");
            throw;
        }
    }

    public async Task<int> DeleteBatchAsync(IEnumerable<int> categoryIds)
    {
        using var scope = _logger.BeginScope("Deleting batch of {Count} categories", categoryIds.Count());
        
        try
        {
            var deleted = 0;
            
            foreach (var categoryId in categoryIds)
            {
                var success = await DeleteAsync(categoryId);
                if (success) deleted++;
            }
            
            _logger.LogInformation("Deleted batch of {Count} categories", deleted);
            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting batch of categories");
            throw;
        }
    }
}
