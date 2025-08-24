using Dapper;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Ikhtibar.Shared.DTOs;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for tree-specific operations on question bank categories
/// Implements efficient tree traversal using materialized path and closure table patterns
/// </summary>
public class QuestionBankTreeRepository : IQuestionBankTreeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<QuestionBankTreeRepository> _logger;

    public QuestionBankTreeRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<QuestionBankTreeRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    // Tree navigation operations
    public async Task<CategoryTreeNode?> GetTreeAsync(int? rootId = null, int? maxDepth = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            WITH TreeCTE AS (
                -- Base case: root categories
                SELECT CategoryId, Name, Code, Description, Type, Level, ParentId, SortOrder, TreePath, 0 as Depth
                FROM QuestionBankCategories 
                WHERE (@RootId IS NULL AND ParentId IS NULL) OR (@RootId IS NOT NULL AND CategoryId = @RootId)
                    AND IsActive = 1
                
                UNION ALL
                
                -- Recursive case: child categories
                SELECT c.CategoryId, c.Name, c.Code, c.Description, c.Type, c.Level, c.ParentId, c.SortOrder, c.TreePath, t.Depth + 1
                FROM QuestionBankCategories c
                INNER JOIN TreeCTE t ON c.ParentId = t.CategoryId
                WHERE (@MaxDepth IS NULL OR t.Depth < @MaxDepth) AND c.IsActive = 1
            )
            SELECT * FROM TreeCTE ORDER BY TreePath, SortOrder";

        var categories = await connection.QueryAsync<QuestionBankCategory>(sql, new { RootId = rootId, MaxDepth = maxDepth });
        
        if (!categories.Any())
            return null;

        return BuildTreeStructure(categories, rootId);
    }

    public async Task<CategoryTreeNode?> GetTreeWithQuestionCountsAsync(int? rootId = null, bool includeInactive = false)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            WITH TreeCTE AS (
                SELECT CategoryId, Name, Code, Description, Type, Level, ParentId, SortOrder, TreePath, 0 as Depth
                FROM QuestionBankCategories 
                WHERE (@RootId IS NULL AND ParentId IS NULL) OR (@RootId IS NOT NULL AND CategoryId = @RootId)
                    AND (@IncludeInactive = 1 OR IsActive = 1)
                
                UNION ALL
                
                SELECT c.CategoryId, c.Name, c.Code, c.Description, c.Type, c.Level, c.ParentId, c.SortOrder, c.TreePath, t.Depth + 1
                FROM QuestionBankCategories c
                INNER JOIN TreeCTE t ON c.ParentId = t.CategoryId
                WHERE (@IncludeInactive = 1 OR c.IsActive = 1)
            ),
            CategoryCounts AS (
                SELECT 
                    t.*,
                    COALESCE(qc.DirectQuestionCount, 0) as DirectQuestionCount,
                    COALESCE(qc.TotalQuestionCount, 0) as TotalQuestionCount
                FROM TreeCTE t
                LEFT JOIN (
                    SELECT 
                        CategoryId,
                        COUNT(*) as DirectQuestionCount,
                        COUNT(*) as TotalQuestionCount
                    FROM QuestionCategorizations qcat
                    INNER JOIN Questions q ON qcat.QuestionId = q.Id
                    WHERE q.IsDeleted = 0
                    GROUP BY CategoryId
                ) qc ON t.CategoryId = qc.CategoryId
            )
            SELECT * FROM CategoryCounts ORDER BY TreePath, SortOrder";

        var categoriesWithCounts = await connection.QueryAsync<dynamic>(sql, new { RootId = rootId, IncludeInactive = includeInactive });
        
        if (!categoriesWithCounts.Any())
            return null;

        return BuildTreeStructureWithCounts(categoriesWithCounts, rootId);
    }

    public async Task<CategoryTreeNode?> GetSubtreeAsync(int categoryId, int depth = 0)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            WITH SubtreeCTE AS (
                SELECT CategoryId, Name, Code, Description, Type, Level, ParentId, SortOrder, TreePath, 0 as CurrentDepth
                FROM QuestionBankCategories 
                WHERE CategoryId = @CategoryId AND IsActive = 1
                
                UNION ALL
                
                SELECT c.CategoryId, c.Name, c.Code, c.Description, c.Type, c.Level, c.ParentId, c.SortOrder, c.TreePath, s.CurrentDepth + 1
                FROM QuestionBankCategories c
                INNER JOIN SubtreeCTE s ON c.ParentId = s.CategoryId
                WHERE (@Depth = 0 OR s.CurrentDepth < @Depth) AND c.IsActive = 1
            )
            SELECT * FROM SubtreeCTE ORDER BY TreePath, SortOrder";

        var categories = await connection.QueryAsync<QuestionBankCategory>(sql, new { CategoryId = categoryId, Depth = depth });
        
        if (!categories.Any())
            return null;

        return BuildTreeStructure(categories, categoryId);
    }

    public async Task<IEnumerable<CategoryBreadcrumb>> GetBreadcrumbsAsync(int categoryId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            WITH BreadcrumbCTE AS (
                SELECT CategoryId, Name, Code, ParentId, TreePath, Type, Level, 0 as Level
                FROM QuestionBankCategories 
                WHERE CategoryId = @CategoryId
                
                UNION ALL
                
                SELECT c.CategoryId, c.Name, c.Code, c.ParentId, c.TreePath, c.Type, c.Level, b.Level + 1
                FROM QuestionBankCategories c
                INNER JOIN BreadcrumbCTE b ON c.CategoryId = b.ParentId
            )
            SELECT CategoryId, Name, Code, Type, Level FROM BreadcrumbCTE ORDER BY Level DESC";

        var breadcrumbs = await connection.QueryAsync<CategoryBreadcrumb>(sql, new { CategoryId = categoryId });
        return breadcrumbs;
    }

    // Tree manipulation operations
    public async Task<TreeMoveResult> MoveCategoryAsync(int categoryId, int? newParentId, int? newSortOrder = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Check for circular reference
            if (newParentId.HasValue && await WouldCreateCircularReferenceAsync(categoryId, newParentId.Value))
            {
                return new TreeMoveResult { Success = false, ErrorMessage = "Move would create circular reference" };
            }

            // Get current category
            var category = await connection.QuerySingleOrDefaultAsync<QuestionBankCategory>(
                "SELECT * FROM QuestionBankCategories WHERE CategoryId = @Id", 
                new { Id = categoryId }, transaction);

            if (category == null)
            {
                return new TreeMoveResult { Success = false, ErrorMessage = "Category not found" };
            }

            // Update parent and sort order
            var updateSql = @"
                UPDATE QuestionBankCategories 
                SET ParentId = @NewParentId, 
                    SortOrder = COALESCE(@NewSortOrder, (SELECT COALESCE(MAX(SortOrder), 0) + 1 FROM QuestionBankCategories WHERE ParentId = @NewParentId)),
                    ModifiedAt = GETUTCDATE()
                WHERE CategoryId = @CategoryId";

            await connection.ExecuteAsync(updateSql, new { CategoryId = categoryId, NewParentId = newParentId, NewSortOrder = newSortOrder }, transaction);

            // Rebuild tree paths for moved subtree
            var affectedCount = await RebuildTreePathsForSubtreeAsync(categoryId, connection, transaction);

            transaction.Commit();
            
            _logger.LogInformation("Category {CategoryId} moved to parent {ParentId}", categoryId, newParentId);
            
            return new TreeMoveResult { Success = true, CategoriesAffected = affectedCount };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error moving category {CategoryId}", categoryId);
            return new TreeMoveResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<TreeCopyResult> CopyCategoryAsync(int categoryId, int? newParentId, bool includeDescendants = true, string? newName = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Get source category
            var sourceCategory = await connection.QuerySingleOrDefaultAsync<QuestionBankCategory>(
                "SELECT * FROM QuestionBankCategories WHERE CategoryId = @Id", 
                new { Id = categoryId }, transaction);

            if (sourceCategory == null)
            {
                return new TreeCopyResult { Success = false, ErrorMessage = "Source category not found" };
            }

            // Create new category
            var newCategoryId = await CreateCopiedCategoryAsync(sourceCategory, newParentId, newName, connection, transaction);
            var categoriesCopied = 1;
            var idMapping = new Dictionary<int, int> { { categoryId, newCategoryId } };

            // Copy descendants if requested
            if (includeDescendants)
            {
                var (copiedCount, mappings) = await CopyDescendantsAsync(categoryId, newCategoryId, connection, transaction);
                categoriesCopied += copiedCount;
                foreach (var mapping in mappings)
                {
                    idMapping[mapping.Key] = mapping.Value;
                }
            }

            transaction.Commit();
            
            _logger.LogInformation("Category {CategoryId} copied to new category {NewCategoryId}", categoryId, newCategoryId);
            
            return new TreeCopyResult 
            { 
                Success = true, 
                NewCategoryId = newCategoryId, 
                CategoriesCopied = categoriesCopied,
                IdMapping = idMapping
            };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error copying category {CategoryId}", categoryId);
            return new TreeCopyResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<TreeDeleteResult> DeleteCategoryAsync(int categoryId, ChildHandlingStrategy strategy)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Get category and its children
            var category = await connection.QuerySingleOrDefaultAsync<QuestionBankCategory>(
                "SELECT * FROM QuestionBankCategories WHERE CategoryId = @Id", 
                new { Id = categoryId }, transaction);

            if (category == null)
            {
                return new TreeDeleteResult { Success = false, ErrorMessage = "Category not found" };
            }

            var children = await connection.QueryAsync<QuestionBankCategory>(
                "SELECT * FROM QuestionBankCategories WHERE ParentId = @ParentId", 
                new { ParentId = categoryId }, transaction);

            if (strategy == ChildHandlingStrategy.Prevent && children.Any())
            {
                return new TreeDeleteResult { Success = false, ErrorMessage = "Cannot delete category with children" };
            }

            var deletedCount = 1;
            var questionsReassigned = 0;

            // Handle children based on strategy
            var childResults = await HandleChildrenOnDeleteAsync(categoryId, children, strategy, connection, transaction);
            deletedCount += childResults.deleted;
            questionsReassigned += childResults.questionsReassigned;

            // Delete the category
            await connection.ExecuteAsync(
                "DELETE FROM QuestionBankCategories WHERE CategoryId = @Id", 
                new { Id = categoryId }, transaction);

            transaction.Commit();
            
            _logger.LogInformation("Category {CategoryId} deleted with strategy {Strategy}", categoryId, strategy);
            
            return new TreeDeleteResult 
            { 
                Success = true, 
                CategoriesDeleted = deletedCount, 
                QuestionsReassigned = questionsReassigned 
            };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error deleting category {CategoryId}", categoryId);
            return new TreeDeleteResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    // Tree validation operations
    public async Task<TreeValidationResult> ValidateTreeIntegrityAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var issues = new List<TreeValidationIssue>();

        // Check for orphaned categories
        var orphansSql = @"
            SELECT CategoryId, Name FROM QuestionBankCategories c
            WHERE ParentId IS NOT NULL 
            AND NOT EXISTS (SELECT 1 FROM QuestionBankCategories p WHERE p.CategoryId = c.ParentId)";
        
        var orphans = await connection.QueryAsync<dynamic>(orphansSql);
        foreach (var orphan in orphans)
        {
            issues.Add(new TreeValidationIssue
            {
                Type = "Orphaned Category",
                Description = $"Category '{orphan.Name}' has invalid parent reference",
                CategoryId = orphan.CategoryId,
                Severity = "Error"
            });
        }

        // Check for circular references (shouldn't happen with proper implementation)
        var circularSql = @"
            WITH CircularCheck AS (
                SELECT CategoryId, ParentId, CategoryId as RootId, 0 as Level
                FROM QuestionBankCategories
                WHERE ParentId IS NOT NULL
                
                UNION ALL
                
                SELECT c.CategoryId, c.ParentId, cc.RootId, cc.Level + 1
                FROM QuestionBankCategories c
                INNER JOIN CircularCheck cc ON c.CategoryId = cc.ParentId
                WHERE cc.Level < 10 AND c.CategoryId = cc.RootId
            )
            SELECT CategoryId, RootId FROM CircularCheck WHERE Level > 0";

        var circularRefs = await connection.QueryAsync<dynamic>(circularSql);
        foreach (var circular in circularRefs)
        {
            issues.Add(new TreeValidationIssue
            {
                Type = "Circular Reference",
                Description = $"Category {circular.CategoryId} has circular reference with {circular.RootId}",
                CategoryId = circular.CategoryId,
                Severity = "Critical"
            });
        }

        return new TreeValidationResult
        {
            IsValid = !issues.Any(),
            Issues = issues,
            OrphanedCategories = orphans.Count(),
            CircularReferences = circularRefs.Count()
        };
    }

    public async Task<bool> WouldCreateCircularReferenceAsync(int categoryId, int newParentId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            WITH AncestorCheck AS (
                SELECT CategoryId, ParentId
                FROM QuestionBankCategories
                WHERE CategoryId = @NewParentId
                
                UNION ALL
                
                SELECT c.CategoryId, c.ParentId
                FROM QuestionBankCategories c
                INNER JOIN AncestorCheck ac ON c.CategoryId = ac.ParentId
            )
            SELECT COUNT(*) FROM AncestorCheck WHERE CategoryId = @CategoryId";

        var count = await connection.QuerySingleAsync<int>(sql, new { CategoryId = categoryId, NewParentId = newParentId });
        return count > 0;
    }

    public async Task<int> GetCategoryDepthAsync(int categoryId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            WITH DepthCTE AS (
                SELECT CategoryId, ParentId, 0 as Depth
                FROM QuestionBankCategories
                WHERE CategoryId = @CategoryId
                
                UNION ALL
                
                SELECT c.CategoryId, c.ParentId, d.Depth + 1
                FROM QuestionBankCategories c
                INNER JOIN DepthCTE d ON c.CategoryId = d.ParentId
            )
            SELECT MAX(Depth) FROM DepthCTE";

        var depth = await connection.QuerySingleOrDefaultAsync<int?>(sql, new { CategoryId = categoryId });
        return depth ?? 0;
    }

    public async Task<bool> IsDescendantOfAsync(int categoryId, int potentialAncestorId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            WITH AncestorCTE AS (
                SELECT CategoryId, ParentId
                FROM QuestionBankCategories
                WHERE CategoryId = @CategoryId
                
                UNION ALL
                
                SELECT c.CategoryId, c.ParentId
                FROM QuestionBankCategories c
                INNER JOIN AncestorCTE a ON c.CategoryId = a.ParentId
            )
            SELECT COUNT(*) FROM AncestorCTE WHERE CategoryId = @PotentialAncestorId";

        var count = await connection.QuerySingleAsync<int>(sql, new { CategoryId = categoryId, PotentialAncestorId = potentialAncestorId });
        return count > 0;
    }

    // Tree statistics operations
    public async Task<TreeStatistics> GetTreeStatisticsAsync(int? rootId = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            WITH TreeCTE AS (
                SELECT CategoryId, ParentId, 0 as Depth
                FROM QuestionBankCategories 
                WHERE (@RootId IS NULL AND ParentId IS NULL) OR (@RootId IS NOT NULL AND CategoryId = @RootId)
                
                UNION ALL
                
                SELECT c.CategoryId, c.ParentId, t.Depth + 1
                FROM QuestionBankCategories c
                INNER JOIN TreeCTE t ON c.ParentId = t.CategoryId
            )
            SELECT 
                COUNT(*) as TotalCategories,
                MAX(Depth) as MaxDepth,
                COUNT(CASE WHEN ParentId IS NULL THEN 1 END) as RootCategories,
                COUNT(CASE WHEN NOT EXISTS(SELECT 1 FROM QuestionBankCategories child WHERE child.ParentId = TreeCTE.CategoryId) THEN 1 END) as LeafCategories
            FROM TreeCTE";

        var stats = await connection.QuerySingleAsync<TreeStatistics>(sql, new { RootId = rootId });
        return stats;
    }

    public async Task<IDictionary<CategoryType, int>> GetCategoryDistributionAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT Type, COUNT(*) as Count FROM QuestionBankCategories WHERE IsActive = 1 GROUP BY Type";
        var results = await connection.QueryAsync<dynamic>(sql);
        
        return results.ToDictionary(r => (CategoryType)r.Type, r => (int)r.Count);
    }

    public async Task<IDictionary<CategoryLevel, int>> GetLevelDistributionAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT Level, COUNT(*) as Count FROM QuestionBankCategories WHERE IsActive = 1 GROUP BY Level";
        var results = await connection.QueryAsync<dynamic>(sql);
        
        return results.ToDictionary(r => (CategoryLevel)r.Level, r => (int)r.Count);
    }

    // Tree maintenance operations
    public async Task<int> RebuildTreePathsAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            var updatedCount = 0;
            
            // Get all root categories first
            var rootCategories = await connection.QueryAsync<QuestionBankCategory>(
                "SELECT * FROM QuestionBankCategories WHERE ParentId IS NULL ORDER BY SortOrder",
                transaction: transaction);

            foreach (var root in rootCategories)
            {
                updatedCount += await RebuildTreePathsForSubtreeAsync(root.CategoryId, connection, transaction);
            }

            transaction.Commit();
            
            _logger.LogInformation("Rebuilt tree paths for {Count} categories", updatedCount);
            
            return updatedCount;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error rebuilding tree paths");
            throw;
        }
    }

    public async Task<int> RebuildHierarchyTableAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Clear existing hierarchy entries
            await connection.ExecuteAsync("DELETE FROM QuestionBankHierarchies", transaction: transaction);

            // Rebuild hierarchy table using recursive CTE
            var sql = @"
                WITH HierarchyCTE AS (
                    -- Direct parent-child relationships
                    SELECT CategoryId as AncestorId, CategoryId as DescendantId, 0 as Distance
                    FROM QuestionBankCategories
                    
                    UNION ALL
                    
                    -- Indirect relationships
                    SELECT h.AncestorId, c.CategoryId as DescendantId, h.Distance + 1
                    FROM HierarchyCTE h
                    INNER JOIN QuestionBankCategories c ON c.ParentId = h.DescendantId
                )
                INSERT INTO QuestionBankHierarchies (AncestorId, DescendantId, Distance, CreatedAt)
                SELECT AncestorId, DescendantId, Distance, GETUTCDATE()
                FROM HierarchyCTE";

            var insertedCount = await connection.ExecuteAsync(sql, transaction: transaction);
            
            transaction.Commit();
            
            _logger.LogInformation("Rebuilt hierarchy table with {Count} entries", insertedCount);
            
            return insertedCount;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error rebuilding hierarchy table");
            throw;
        }
    }

    public async Task<int> OptimizeSortOrdersAsync(int? parentId = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            var sql = @"
                WITH SortedCategories AS (
                    SELECT CategoryId, ROW_NUMBER() OVER (ORDER BY SortOrder, Name) * 10 as NewSortOrder
                    FROM QuestionBankCategories
                    WHERE (@ParentId IS NULL AND ParentId IS NULL) OR ParentId = @ParentId
                )
                UPDATE c SET SortOrder = s.NewSortOrder, ModifiedAt = GETUTCDATE()
                FROM QuestionBankCategories c
                INNER JOIN SortedCategories s ON c.CategoryId = s.CategoryId
                WHERE c.SortOrder != s.NewSortOrder";

            var updatedCount = await connection.ExecuteAsync(sql, new { ParentId = parentId }, transaction);
            
            transaction.Commit();
            
            _logger.LogInformation("Optimized sort orders for {Count} categories under parent {ParentId}", updatedCount, parentId);
            
            return updatedCount;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error optimizing sort orders for parent {ParentId}", parentId);
            throw;
        }
    }

    public async Task<int> CompactTreeAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Find empty intermediate categories (no direct questions, only one child)
            var sql = @"
                WITH EmptyIntermediates AS (
                    SELECT c.CategoryId, c.ParentId, child.CategoryId as OnlyChildId
                    FROM QuestionBankCategories c
                    INNER JOIN QuestionBankCategories child ON child.ParentId = c.CategoryId
                    WHERE NOT EXISTS (SELECT 1 FROM QuestionCategorizations qc WHERE qc.CategoryId = c.CategoryId)
                    AND (SELECT COUNT(*) FROM QuestionBankCategories child2 WHERE child2.ParentId = c.CategoryId) = 1
                )
                SELECT * FROM EmptyIntermediates";

            var emptyCategories = await connection.QueryAsync<dynamic>(sql, transaction: transaction);
            var removedCount = 0;

            foreach (var empty in emptyCategories)
            {
                // Move the only child to the empty category's parent
                await connection.ExecuteAsync(
                    "UPDATE QuestionBankCategories SET ParentId = @NewParentId WHERE CategoryId = @ChildId",
                    new { NewParentId = empty.ParentId, ChildId = empty.OnlyChildId },
                    transaction);

                // Delete the empty intermediate category
                await connection.ExecuteAsync(
                    "DELETE FROM QuestionBankCategories WHERE CategoryId = @Id",
                    new { Id = empty.CategoryId },
                    transaction);

                removedCount++;
            }

            // Rebuild tree paths after compaction
            if (removedCount > 0)
            {
                await RebuildTreePathsAsync();
            }

            transaction.Commit();
            
            _logger.LogInformation("Compacted tree by removing {Count} empty intermediate categories", removedCount);
            
            return removedCount;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error compacting tree");
            throw;
        }
    }

    // Bulk operations
    public async Task<TreeImportResult> ImportTreeAsync(CategoryTreeImport treeData, int? parentId = null, MergeStrategy mergeStrategy = MergeStrategy.Skip)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            var importResult = new TreeImportResult { Success = true };
            
            foreach (var category in treeData.Categories)
            {
                await ImportCategoryRecursiveAsync(category, parentId, mergeStrategy, importResult, connection, transaction);
            }
            
            transaction.Commit();
            
            _logger.LogInformation("Imported tree with {Count} categories", importResult.CategoriesCreated);
            
            return importResult;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error importing tree");
            return new TreeImportResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<CategoryTreeExport> ExportTreeAsync(int? rootId = null, bool includeQuestionCounts = false)
    {
        var treeData = includeQuestionCounts 
            ? await GetTreeWithQuestionCountsAsync(rootId)
            : await GetTreeAsync(rootId);

        if (treeData == null)
        {
            return new CategoryTreeExport { Categories = new List<CategoryExportNode>() };
        }

        return new CategoryTreeExport
        {
            Categories = new List<CategoryExportNode> { ConvertToExportFormat(treeData) },
            ExportedAt = DateTime.UtcNow,
            Version = "1.0"
        };
    }

    // Search and filtering
    public async Task<IEnumerable<CategorySearchResult>> SearchTreeAsync(TreeSearchCriteria searchCriteria)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "c.IsActive = 1" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(searchCriteria.SearchTerm))
        {
            var searchConditions = new List<string>();
            if (searchCriteria.SearchInCodes)
                searchConditions.Add("c.Code LIKE @SearchTerm");
            if (searchCriteria.SearchInDescriptions)
                searchConditions.Add("c.Name LIKE @SearchTerm OR c.Description LIKE @SearchTerm");
            
            if (searchConditions.Any())
            {
                whereConditions.Add($"({string.Join(" OR ", searchConditions)})");
                parameters.Add("SearchTerm", $"%{searchCriteria.SearchTerm}%");
            }
        }

        if (searchCriteria.Type.HasValue)
        {
            whereConditions.Add("c.Type = @CategoryType");
            parameters.Add("CategoryType", searchCriteria.Type.Value);
        }

        if (searchCriteria.Level.HasValue)
        {
            whereConditions.Add("c.Level = @CategoryLevel");
            parameters.Add("CategoryLevel", searchCriteria.Level.Value);
        }

        if (searchCriteria.WithinCategoryId.HasValue)
        {
            whereConditions.Add("c.TreePath LIKE @ParentPath");
            var parentPath = await connection.QuerySingleOrDefaultAsync<string>(
                "SELECT TreePath FROM QuestionBankCategories WHERE CategoryId = @Id",
                new { Id = searchCriteria.WithinCategoryId.Value });
            parameters.Add("ParentPath", $"{parentPath}%");
        }

        var sql = $@"
            SELECT TOP (@MaxResults)
                c.*,
                p.Name as ParentName,
                (SELECT COUNT(*) FROM QuestionCategorizations qc WHERE qc.CategoryId = c.CategoryId) as QuestionCount
            FROM QuestionBankCategories c
            LEFT JOIN QuestionBankCategories p ON c.ParentId = p.CategoryId
            WHERE {string.Join(" AND ", whereConditions)}
            ORDER BY c.TreePath, c.SortOrder";

        parameters.Add("MaxResults", searchCriteria.MaxResults);

        var results = await connection.QueryAsync<dynamic>(sql, parameters);
        
        return results.Select(r => new CategorySearchResult
        {
            Category = MapToCategory(r),
            Breadcrumbs = new List<CategoryBreadcrumb>(), // Could be populated separately if needed
            MatchType = "Search",
            RelevanceScore = 1.0
        });
    }

    public async Task<IEnumerable<QuestionBankCategory>> GetRecentlyModifiedAsync(DateTime sinceDate, int maxResults = 100)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT TOP (@MaxResults) *
            FROM QuestionBankCategories
            WHERE ModifiedAt > @SinceDate AND IsActive = 1
            ORDER BY ModifiedAt DESC";

        var categories = await connection.QueryAsync<QuestionBankCategory>(sql, new { SinceDate = sinceDate, MaxResults = maxResults });
        return categories;
    }

    // Private helper methods
    private CategoryTreeNode BuildTreeStructure(IEnumerable<QuestionBankCategory> categories, int? rootId)
    {
        var categoryDict = categories.ToDictionary(c => c.CategoryId);
        var root = rootId.HasValue ? categoryDict[rootId.Value] : categories.FirstOrDefault(c => c.ParentId == null);
        
        if (root == null) return null!;

        var rootNode = new CategoryTreeNode
        {
            Category = root,
            Children = new List<CategoryTreeNode>(),
            Depth = 0
        };

        BuildChildNodes(rootNode, categoryDict);
        return rootNode;
    }

    private void BuildChildNodes(CategoryTreeNode parentNode, Dictionary<int, QuestionBankCategory> categoryDict)
    {
        var children = categoryDict.Values
            .Where(c => c.ParentId == parentNode.Category.CategoryId)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name);

        foreach (var child in children)
        {
            var childNode = new CategoryTreeNode
            {
                Category = child,
                Children = new List<CategoryTreeNode>(),
                Depth = parentNode.Depth + 1
            };

            BuildChildNodes(childNode, categoryDict);
            parentNode.Children.Add(childNode);
        }

        parentNode.HasChildren = parentNode.Children.Any();
    }

    private CategoryTreeNode BuildTreeStructureWithCounts(IEnumerable<dynamic> categoriesWithCounts, int? rootId)
    {
        var root = rootId.HasValue 
            ? categoriesWithCounts.FirstOrDefault(c => c.CategoryId == rootId.Value)
            : categoriesWithCounts.FirstOrDefault(c => c.ParentId == null);
        
        if (root == null) return null!;

        var rootNode = new CategoryTreeNode
        {
            Category = MapToCategory(root),
            DirectQuestionCount = root.DirectQuestionCount,
            QuestionCount = root.TotalQuestionCount,
            Children = new List<CategoryTreeNode>(),
            Depth = 0
        };

        BuildChildNodesWithCounts(rootNode, categoriesWithCounts.ToList());
        return rootNode;
    }

    private void BuildChildNodesWithCounts(CategoryTreeNode parentNode, List<dynamic> categoriesWithCounts)
    {
        var children = categoriesWithCounts
            .Where(c => c.ParentId == parentNode.Category.CategoryId)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name);

        foreach (var child in children)
        {
            var childNode = new CategoryTreeNode
            {
                Category = MapToCategory(child),
                DirectQuestionCount = child.DirectQuestionCount,
                QuestionCount = child.TotalQuestionCount,
                Children = new List<CategoryTreeNode>(),
                Depth = parentNode.Depth + 1
            };

            BuildChildNodesWithCounts(childNode, categoriesWithCounts);
            parentNode.Children.Add(childNode);
        }

        parentNode.HasChildren = parentNode.Children.Any();
    }

    private QuestionBankCategory MapToCategory(dynamic item)
    {
        return new QuestionBankCategory
        {
            CategoryId = item.CategoryId,
            Name = item.Name,
            Code = item.Code,
            Description = item.Description,
            Type = (CategoryType)item.Type,
            Level = (CategoryLevel)item.Level,
            ParentId = item.ParentId,
            SortOrder = item.SortOrder,
            TreePath = item.TreePath
        };
    }

    private async Task<int> RebuildTreePathsForSubtreeAsync(int rootId, IDbConnection connection, IDbTransaction transaction)
    {
        var updatedCount = 0;

        // Get the root category and build its path
        var rootCategory = await connection.QuerySingleAsync<QuestionBankCategory>(
            "SELECT * FROM QuestionBankCategories WHERE CategoryId = @Id",
            new { Id = rootId }, transaction);

        var rootPath = await BuildTreePathAsync(rootCategory.CategoryId, connection, transaction);
        
        await connection.ExecuteAsync(
            "UPDATE QuestionBankCategories SET TreePath = @TreePath WHERE CategoryId = @Id",
            new { TreePath = rootPath, Id = rootId }, transaction);
        
        updatedCount++;

        // Recursively update children
        var children = await connection.QueryAsync<QuestionBankCategory>(
            "SELECT * FROM QuestionBankCategories WHERE ParentId = @ParentId ORDER BY SortOrder",
            new { ParentId = rootId }, transaction);

        foreach (var child in children)
        {
            updatedCount += await RebuildTreePathsForSubtreeAsync(child.CategoryId, connection, transaction);
        }

        return updatedCount;
    }

    private async Task<string> BuildTreePathAsync(int categoryId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = @"
            WITH PathCTE AS (
                SELECT CategoryId, ParentId, CAST(CategoryId as VARCHAR(MAX)) as Path
                FROM QuestionBankCategories
                WHERE CategoryId = @CategoryId
                
                UNION ALL
                
                SELECT c.CategoryId, c.ParentId, CAST(c.CategoryId as VARCHAR(MAX)) + '/' + p.Path
                FROM QuestionBankCategories c
                INNER JOIN PathCTE p ON c.CategoryId = p.ParentId
            )
            SELECT TOP 1 Path FROM PathCTE ORDER BY LEN(Path) DESC";

        var path = await connection.QuerySingleOrDefaultAsync<string>(sql, new { CategoryId = categoryId }, transaction);
        return path ?? categoryId.ToString();
    }

    private async Task<int> CreateCopiedCategoryAsync(QuestionBankCategory source, int? newParentId, string? newName, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO QuestionBankCategories (Name, Code, Description, Type, Level, ParentId, SortOrder, TreePath, IsActive, CreatedAt)
            OUTPUT INSERTED.CategoryId
            VALUES (@Name, @Code, @Description, @Type, @Level, @ParentId, @SortOrder, @TreePath, @IsActive, GETUTCDATE())";

        var parameters = new
        {
            Name = newName ?? $"{source.Name} (Copy)",
            Code = $"{source.Code}_COPY",
            source.Description,
            source.Type,
            source.Level,
            ParentId = newParentId,
            SortOrder = await GetNextSortOrderAsync(newParentId, connection, transaction),
            TreePath = "", // Will be updated later
            IsActive = true
        };

        var newId = await connection.QuerySingleAsync<int>(sql, parameters, transaction);
        
        // Update tree path
        var newPath = await BuildTreePathAsync(newId, connection, transaction);
        await connection.ExecuteAsync(
            "UPDATE QuestionBankCategories SET TreePath = @TreePath WHERE CategoryId = @Id",
            new { TreePath = newPath, Id = newId }, transaction);

        return newId;
    }

    private async Task<(int copiedCount, Dictionary<int, int> idMapping)> CopyDescendantsAsync(int sourceCategoryId, int newParentId, IDbConnection connection, IDbTransaction transaction)
    {
        var children = await connection.QueryAsync<QuestionBankCategory>(
            "SELECT * FROM QuestionBankCategories WHERE ParentId = @ParentId ORDER BY SortOrder",
            new { ParentId = sourceCategoryId }, transaction);

        var copiedCount = 0;
        var idMapping = new Dictionary<int, int>();

        foreach (var child in children)
        {
            var newChildId = await CreateCopiedCategoryAsync(child, newParentId, null, connection, transaction);
            idMapping[child.CategoryId] = newChildId;
            copiedCount++;

            var (descendantCount, descendantMapping) = await CopyDescendantsAsync(child.CategoryId, newChildId, connection, transaction);
            copiedCount += descendantCount;
            foreach (var mapping in descendantMapping)
            {
                idMapping[mapping.Key] = mapping.Value;
            }
        }

        return (copiedCount, idMapping);
    }

    private async Task<int> GetNextSortOrderAsync(int? parentId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = "SELECT COALESCE(MAX(SortOrder), 0) + 10 FROM QuestionBankCategories WHERE ParentId = @ParentId";
        return await connection.QuerySingleAsync<int>(sql, new { ParentId = parentId }, transaction);
    }

    private async Task<(int deleted, int questionsReassigned)> HandleChildrenOnDeleteAsync(int categoryId, IEnumerable<QuestionBankCategory> children, ChildHandlingStrategy strategy, IDbConnection connection, IDbTransaction transaction)
    {
        var deletedCount = 0;
        var questionsReassigned = 0;

        switch (strategy)
        {
            case ChildHandlingStrategy.MoveToParent:
                var parentId = await connection.QuerySingleOrDefaultAsync<int?>(
                    "SELECT ParentId FROM QuestionBankCategories WHERE CategoryId = @Id",
                    new { Id = categoryId }, transaction);

                foreach (var child in children)
                {
                    await connection.ExecuteAsync(
                        "UPDATE QuestionBankCategories SET ParentId = @ParentId WHERE CategoryId = @Id",
                        new { ParentId = parentId, Id = child.CategoryId }, transaction);
                }
                break;

            case ChildHandlingStrategy.MoveToRoot:
                foreach (var child in children)
                {
                    await connection.ExecuteAsync(
                        "UPDATE QuestionBankCategories SET ParentId = NULL WHERE CategoryId = @Id",
                        new { Id = child.CategoryId }, transaction);
                }
                break;

            case ChildHandlingStrategy.Delete:
                foreach (var child in children)
                {
                    var childResult = await DeleteCategoryAsync(child.CategoryId, strategy);
                    deletedCount += childResult.CategoriesDeleted;
                    questionsReassigned += childResult.QuestionsReassigned;
                }
                break;
        }

        return (deletedCount, questionsReassigned);
    }

    private async Task ImportCategoryRecursiveAsync(CategoryImportNode node, int? parentId, MergeStrategy mergeStrategy, TreeImportResult result, IDbConnection connection, IDbTransaction transaction)
    {
        int categoryId;

        // Check if category already exists
        var existingId = await connection.QuerySingleOrDefaultAsync<int?>(
            "SELECT CategoryId FROM QuestionBankCategories WHERE Code = @Code",
            new { Code = node.Code }, transaction);

        if (existingId.HasValue)
        {
            switch (mergeStrategy)
            {
                case MergeStrategy.Skip:
                    categoryId = existingId.Value;
                    result.CategoriesSkipped++;
                    break;
                case MergeStrategy.Overwrite:
                    await connection.ExecuteAsync(
                        "UPDATE QuestionBankCategories SET Name = @Name, Description = @Description, ModifiedAt = GETUTCDATE() WHERE CategoryId = @Id",
                        new { node.Name, node.Description, Id = existingId.Value }, transaction);
                    categoryId = existingId.Value;
                    result.CategoriesUpdated++;
                    break;
                case MergeStrategy.CreateNew:
                    categoryId = await CreateImportedCategoryAsync(node, parentId, connection, transaction);
                    result.CategoriesCreated++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mergeStrategy));
            }
        }
        else
        {
            categoryId = await CreateImportedCategoryAsync(node, parentId, connection, transaction);
            result.CategoriesCreated++;
        }

        result.CodeToIdMapping[node.Code] = categoryId;

        // Import children
        foreach (var child in node.Children)
        {
            await ImportCategoryRecursiveAsync(child, categoryId, mergeStrategy, result, connection, transaction);
        }
    }

    private async Task<int> CreateImportedCategoryAsync(CategoryImportNode node, int? parentId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO QuestionBankCategories (Name, Code, Description, Type, Level, ParentId, SortOrder, TreePath, IsActive, AllowQuestions, CreatedAt)
            OUTPUT INSERTED.CategoryId
            VALUES (@Name, @Code, @Description, @Type, @Level, @ParentId, @SortOrder, '', @IsActive, @AllowQuestions, GETUTCDATE())";

        var parameters = new
        {
            node.Name,
            node.Code,
            node.Description,
            node.Type,
            node.Level,
            ParentId = parentId,
            SortOrder = node.SortOrder > 0 ? node.SortOrder : await GetNextSortOrderAsync(parentId, connection, transaction),
            node.IsActive,
            node.AllowQuestions
        };

        var newId = await connection.QuerySingleAsync<int>(sql, parameters, transaction);
        
        // Update tree path
        var newPath = await BuildTreePathAsync(newId, connection, transaction);
        await connection.ExecuteAsync(
            "UPDATE QuestionBankCategories SET TreePath = @TreePath WHERE CategoryId = @Id",
            new { TreePath = newPath, Id = newId }, transaction);

        return newId;
    }

    private CategoryExportNode ConvertToExportFormat(CategoryTreeNode node)
    {
        return new CategoryExportNode
        {
            Id = node.Category.CategoryId,
            Name = node.Category.Name,
            Code = node.Category.Code,
            Description = node.Category.Description,
            Type = node.Category.Type,
            Level = node.Category.Level,
            SortOrder = node.Category.SortOrder,
            QuestionCount = node.QuestionCount,
            IsActive = node.Category.IsActive,
            AllowQuestions = node.Category.AllowQuestions,
            CreatedAt = node.Category.CreatedAt,
            ModifiedAt = node.Category.ModifiedAt,
            Children = node.Children.Select(ConvertToExportFormat).ToList()
        };
    }
}
