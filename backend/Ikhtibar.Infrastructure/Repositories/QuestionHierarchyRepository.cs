using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using QuestionCategoryDetailCore = Ikhtibar.Core.Repositories.Interfaces.QuestionCategoryDetail;
using QuestionCategorySearchCriteriaCore = Ikhtibar.Core.Repositories.Interfaces.QuestionCategorySearchCriteria;
using CategorizationValidationResultCore = Ikhtibar.Core.Repositories.Interfaces.CategorizationValidationResult;
using CategorizationStatisticsCore = Ikhtibar.Core.Repositories.Interfaces.CategorizationStatistics;
using CategoryUsageAnalyticsCore = Ikhtibar.Core.Repositories.Interfaces.CategoryUsageAnalytics;
using AutoCategorizationMetricsCore = Ikhtibar.Core.Repositories.Interfaces.AutoCategorizationMetrics;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository for managing question-category relationships and hierarchy operations
/// Implements efficient querying and bulk operations for question categorization
/// </summary>
public class QuestionHierarchyRepository : IQuestionHierarchyRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<QuestionHierarchyRepository> _logger;

    public QuestionHierarchyRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<QuestionHierarchyRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<QuestionCategorization> AssignQuestionToCategoryAsync(
        int questionId, 
        int categoryId, 
        bool isPrimary = false, 
        int? createdByUserId = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Check if assignment already exists
            const string checkExistsSql = @"
                SELECT COUNT(1) 
                FROM QuestionCategorization 
                WHERE QuestionId = @QuestionId AND CategoryId = @CategoryId AND IsDeleted = 0";

            var exists = await connection.QuerySingleAsync<int>(
                checkExistsSql,
                new { QuestionId = questionId, CategoryId = categoryId },
                transaction);

            if (exists > 0)
            {
                throw new InvalidOperationException("Question is already assigned to this category");
            }

            // If setting as primary, remove existing primary assignments
            if (isPrimary)
            {
                const string removePrimarySql = @"
                    UPDATE QuestionCategorization 
                    SET IsPrimary = 0, ModifiedAt = @ModifiedAt
                    WHERE QuestionId = @QuestionId AND IsPrimary = 1 AND IsDeleted = 0";

                await connection.ExecuteAsync(
                    removePrimarySql,
                    new { QuestionId = questionId, ModifiedAt = DateTime.UtcNow },
                    transaction);
            }

            // Create new assignment
            var categorization = new QuestionCategorization
            {
                QuestionId = questionId,
                CategoryId = categoryId,
                IsPrimary = isPrimary,
                AssignedBy = createdByUserId ?? 0,
                CreatedAt = DateTime.UtcNow
            };

            const string insertSql = @"
                INSERT INTO QuestionCategorization 
                (QuestionId, CategoryId, IsPrimary, CreatedByUserId, CreatedAt, IsDeleted)
                VALUES (@QuestionId, @CategoryId, @IsPrimary, @CreatedByUserId, @CreatedAt, 0)";

            await connection.ExecuteAsync(insertSql, categorization, transaction);
            transaction.Commit();

            _logger.LogInformation("Question {QuestionId} assigned to category {CategoryId} as {Type}",
                questionId, categoryId, isPrimary ? "primary" : "secondary");

            return categorization;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error assigning question {QuestionId} to category {CategoryId}",
                questionId, categoryId);
            throw;
        }
    }

    public async Task<bool> RemoveQuestionFromCategoryAsync(int questionId, int categoryId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = @"
            UPDATE QuestionCategorization 
            SET IsDeleted = 1, ModifiedAt = @ModifiedAt
            WHERE QuestionId = @QuestionId AND CategoryId = @CategoryId AND IsDeleted = 0";

        var rowsAffected = await connection.ExecuteAsync(sql,
            new { QuestionId = questionId, CategoryId = categoryId, ModifiedAt = DateTime.UtcNow });

        var success = rowsAffected > 0;
        if (success)
        {
            _logger.LogInformation("Question {QuestionId} removed from category {CategoryId}",
                questionId, categoryId);
        }

        return success;
    }

    public async Task<bool> SetPrimaryCategoryAsync(int questionId, int categoryId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Check if the assignment exists
            const string checkSql = @"
                SELECT COUNT(1) 
                FROM QuestionCategorization 
                WHERE QuestionId = @QuestionId AND CategoryId = @CategoryId AND IsDeleted = 0";

            var exists = await connection.QuerySingleAsync<int>(
                checkSql,
                new { QuestionId = questionId, CategoryId = categoryId },
                transaction);

            if (exists == 0)
            {
                throw new InvalidOperationException("Question is not assigned to this category");
            }

            // Remove existing primary assignments
            const string removePrimarySql = @"
                UPDATE QuestionCategorization 
                SET IsPrimary = 0, ModifiedAt = @ModifiedAt
                WHERE QuestionId = @QuestionId AND IsPrimary = 1 AND IsDeleted = 0";

            await connection.ExecuteAsync(
                removePrimarySql,
                new { QuestionId = questionId, ModifiedAt = DateTime.UtcNow },
                transaction);

            // Set new primary assignment
            const string setPrimarySql = @"
                UPDATE QuestionCategorization 
                SET IsPrimary = 1, ModifiedAt = @ModifiedAt
                WHERE QuestionId = @QuestionId AND CategoryId = @CategoryId AND IsDeleted = 0";

            var rowsAffected = await connection.ExecuteAsync(
                setPrimarySql,
                new { QuestionId = questionId, CategoryId = categoryId, ModifiedAt = DateTime.UtcNow },
                transaction);

            transaction.Commit();

            _logger.LogInformation("Category {CategoryId} set as primary for question {QuestionId}",
                categoryId, questionId);

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error setting primary category {CategoryId} for question {QuestionId}",
                categoryId, questionId);
            throw;
        }
    }

    public async Task<QuestionCategorization?> GetPrimaryCategorizationAsync(int questionId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = @"
            SELECT qc.*, qbc.Name as CategoryName
            FROM QuestionCategorization qc
            INNER JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
            WHERE qc.QuestionId = @QuestionId AND qc.IsPrimary = 1 AND qc.IsDeleted = 0";

        return await connection.QueryFirstOrDefaultAsync<QuestionCategorization>(sql,
            new { QuestionId = questionId });
    }

    public async Task<IEnumerable<QuestionCategorization>> GetQuestionCategorizationsAsync(int questionId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = @"
            SELECT qc.*, qbc.Name as CategoryName, qbc.TreePath
            FROM QuestionCategorization qc
            INNER JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
            WHERE qc.QuestionId = @QuestionId AND qc.IsDeleted = 0
            ORDER BY qc.IsPrimary DESC, qbc.Name";

        return await connection.QueryAsync<QuestionCategorization>(sql,
            new { QuestionId = questionId });
    }

    public async Task<IEnumerable<QuestionCategorization>> GetCategoryQuestionsAsync(int categoryId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = @"
            SELECT qc.*, q.Title as QuestionTitle
            FROM QuestionCategorization qc
            INNER JOIN Questions q ON qc.QuestionId = q.QuestionId
            WHERE qc.CategoryId = @CategoryId AND qc.IsDeleted = 0
            ORDER BY qc.IsPrimary DESC, q.Title";

        return await connection.QueryAsync<QuestionCategorization>(sql,
            new { CategoryId = categoryId });
    }

    public async Task<IEnumerable<int>> GetQuestionsWithoutPrimaryCategoryAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = @"
            SELECT DISTINCT q.QuestionId
            FROM Questions q
            LEFT JOIN QuestionCategorization qc ON q.QuestionId = qc.QuestionId 
                AND qc.IsPrimary = 1 AND qc.IsDeleted = 0
            WHERE qc.QuestionId IS NULL AND q.IsDeleted = 0";

        return await connection.QueryAsync<int>(sql);
    }

    public async Task<IEnumerable<int>> GetQuestionsWithMultiplePrimaryCategoriesAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = @"
            SELECT QuestionId
            FROM QuestionCategorization
            WHERE IsPrimary = 1 AND IsDeleted = 0
            GROUP BY QuestionId
            HAVING COUNT(*) > 1";

        return await connection.QueryAsync<int>(sql);
    }

    public async Task<bool> BulkAssignQuestionsToCategoryAsync(
        IEnumerable<int> questionIds,
        int categoryId,
        bool isPrimary = false,
        bool overrideExisting = false,
        int? createdByUserId = null)
    {
        if (!questionIds.Any()) return true;

        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var questionIdsList = questionIds.ToList();
            var currentDateTime = DateTime.UtcNow;

            // Remove existing assignments if override is requested
            if (overrideExisting)
            {
                const string removeExistingSql = @"
                    UPDATE QuestionCategorization 
                    SET IsDeleted = 1, ModifiedAt = @ModifiedAt
                    WHERE QuestionId IN @QuestionIds AND CategoryId = @CategoryId AND IsDeleted = 0";

                await connection.ExecuteAsync(removeExistingSql,
                    new { QuestionIds = questionIdsList, CategoryId = categoryId, ModifiedAt = currentDateTime },
                    transaction);
            }

            // If setting as primary, remove existing primary assignments
            if (isPrimary)
            {
                const string removePrimarySql = @"
                    UPDATE QuestionCategorization 
                    SET IsPrimary = 0, ModifiedAt = @ModifiedAt
                    WHERE QuestionId IN @QuestionIds AND IsPrimary = 1 AND IsDeleted = 0";

                await connection.ExecuteAsync(removePrimarySql,
                    new { QuestionIds = questionIdsList, ModifiedAt = currentDateTime },
                    transaction);
            }

            // Create new assignments
            var assignments = questionIdsList
                .Where(qId => !overrideExisting || true) // Filter logic can be added here
                .Select(qId => new
                {
                    QuestionId = qId,
                    CategoryId = categoryId,
                    IsPrimary = isPrimary,
                    CreatedByUserId = createdByUserId,
                    CreatedAt = currentDateTime,
                    IsDeleted = false
                });

            const string insertSql = @"
                INSERT INTO QuestionCategorization 
                (QuestionId, CategoryId, IsPrimary, CreatedByUserId, CreatedAt, IsDeleted)
                VALUES (@QuestionId, @CategoryId, @IsPrimary, @CreatedByUserId, @CreatedAt, @IsDeleted)";

            await connection.ExecuteAsync(insertSql, assignments, transaction);
            transaction.Commit();

            _logger.LogInformation("Bulk assigned {Count} questions to category {CategoryId}",
                questionIdsList.Count, categoryId);

            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error bulk assigning questions to category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<bool> BulkRemoveQuestionsFromCategoryAsync(IEnumerable<int> questionIds, int categoryId)
    {
        if (!questionIds.Any()) return true;

        using var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = @"
            UPDATE QuestionCategorization 
            SET IsDeleted = 1, ModifiedAt = @ModifiedAt
            WHERE QuestionId IN @QuestionIds AND CategoryId = @CategoryId AND IsDeleted = 0";

        var rowsAffected = await connection.ExecuteAsync(sql,
            new { QuestionIds = questionIds.ToList(), CategoryId = categoryId, ModifiedAt = DateTime.UtcNow });

        _logger.LogInformation("Bulk removed {Count} questions from category {CategoryId}",
            rowsAffected, categoryId);

        return rowsAffected > 0;
    }

    public async Task<bool> MoveQuestionsToNewCategoryAsync(
        IEnumerable<int> questionIds,
        int fromCategoryId,
        int toCategoryId,
        bool maintainPrimaryStatus = true)
    {
        if (!questionIds.Any()) return true;

        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var questionIdsList = questionIds.ToList();
            var currentDateTime = DateTime.UtcNow;

            // Get existing assignments to preserve primary status if needed
            const string getExistingSql = @"
                SELECT QuestionId, IsPrimary
                FROM QuestionCategorization
                WHERE QuestionId IN @QuestionIds AND CategoryId = @FromCategoryId AND IsDeleted = 0";

            var existingAssignments = await connection.QueryAsync<(int QuestionId, bool IsPrimary)>(
                getExistingSql,
                new { QuestionIds = questionIdsList, FromCategoryId = fromCategoryId },
                transaction);

            // Remove from old category
            const string removeSql = @"
                UPDATE QuestionCategorization 
                SET IsDeleted = 1, ModifiedAt = @ModifiedAt
                WHERE QuestionId IN @QuestionIds AND CategoryId = @FromCategoryId AND IsDeleted = 0";

            await connection.ExecuteAsync(removeSql,
                new { QuestionIds = questionIdsList, FromCategoryId = fromCategoryId, ModifiedAt = currentDateTime },
                transaction);

            // Add to new category
            var newAssignments = existingAssignments.Select(ea => new
            {
                QuestionId = ea.QuestionId,
                CategoryId = toCategoryId,
                IsPrimary = maintainPrimaryStatus ? ea.IsPrimary : false,
                CreatedAt = currentDateTime,
                IsDeleted = false
            });

            const string insertSql = @"
                INSERT INTO QuestionCategorization 
                (QuestionId, CategoryId, IsPrimary, CreatedAt, IsDeleted)
                VALUES (@QuestionId, @CategoryId, @IsPrimary, @CreatedAt, @IsDeleted)";

            await connection.ExecuteAsync(insertSql, newAssignments, transaction);
            transaction.Commit();

            _logger.LogInformation("Moved {Count} questions from category {FromCategory} to {ToCategory}",
                questionIdsList.Count, fromCategoryId, toCategoryId);

            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error moving questions from category {FromCategory} to {ToCategory}",
                fromCategoryId, toCategoryId);
            throw;
        }
    }

    // Analytics and Statistics Methods
    public async Task<IDictionary<int, int>> GetCategoryQuestionCountsAsync(IEnumerable<int> categoryIds)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = @"
            SELECT CategoryId, COUNT(*) as QuestionCount
            FROM QuestionCategorization
            WHERE CategoryId IN @CategoryIds AND IsDeleted = 0
            GROUP BY CategoryId";

        var results = await connection.QueryAsync<(int CategoryId, int QuestionCount)>(sql,
            new { CategoryIds = categoryIds.ToList() });

        return results.ToDictionary(r => r.CategoryId, r => r.QuestionCount);
    }

    public async Task<IDictionary<int, int>> GetQuestionDifficultyDistributionAsync(int categoryId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = @"
            SELECT q.DifficultyLevelId, COUNT(*) as Count
            FROM QuestionCategorization qc
            INNER JOIN Questions q ON qc.QuestionId = q.QuestionId
            WHERE qc.CategoryId = @CategoryId AND qc.IsDeleted = 0 AND q.IsDeleted = 0
            GROUP BY q.DifficultyLevelId";

        var results = await connection.QueryAsync<(int DifficultyLevelId, int Count)>(sql,
            new { CategoryId = categoryId });

        return results.ToDictionary(r => r.DifficultyLevelId, r => r.Count);
    }

    public async Task<IDictionary<int, int>> GetQuestionTypeDistributionAsync(int categoryId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = @"
            SELECT q.QuestionTypeId, COUNT(*) as Count
            FROM QuestionCategorization qc
            INNER JOIN Questions q ON qc.QuestionId = q.QuestionId
            WHERE qc.CategoryId = @CategoryId AND qc.IsDeleted = 0 AND q.IsDeleted = 0
            GROUP BY q.QuestionTypeId";

        var results = await connection.QueryAsync<(int QuestionTypeId, int Count)>(sql,
            new { CategoryId = categoryId });

        return results.ToDictionary(r => r.QuestionTypeId, r => r.Count);
    }

    public async Task<decimal> GetAverageQuestionDifficultyAsync(int categoryId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = @"
            SELECT AVG(CAST(dl.Level as DECIMAL(5,2))) as AverageDifficulty
            FROM QuestionCategorization qc
            INNER JOIN Questions q ON qc.QuestionId = q.QuestionId
            INNER JOIN DifficultyLevels dl ON q.DifficultyLevelId = dl.DifficultyLevelId
            WHERE qc.CategoryId = @CategoryId AND qc.IsDeleted = 0 AND q.IsDeleted = 0";

        return await connection.QuerySingleAsync<decimal>(sql, new { CategoryId = categoryId });
    }

    // Search and Validation Methods
    public async Task<IEnumerable<QuestionCategorization>> SearchQuestionCategorizationsAsync(
        string? searchTerm = null,
        int? categoryId = null,
        int? questionTypeId = null,
        int? difficultyLevelId = null,
        bool? isPrimary = null,
        int skip = 0,
        int take = 50)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var whereConditions = new List<string> { "qc.IsDeleted = 0", "q.IsDeleted = 0" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            whereConditions.Add("(q.Title LIKE @SearchTerm OR qbc.Name LIKE @SearchTerm)");
            parameters.Add("SearchTerm", $"%{searchTerm}%");
        }

        if (categoryId.HasValue)
        {
            whereConditions.Add("qc.CategoryId = @CategoryId");
            parameters.Add("CategoryId", categoryId.Value);
        }

        if (questionTypeId.HasValue)
        {
            whereConditions.Add("q.QuestionTypeId = @QuestionTypeId");
            parameters.Add("QuestionTypeId", questionTypeId.Value);
        }

        if (difficultyLevelId.HasValue)
        {
            whereConditions.Add("q.DifficultyLevelId = @DifficultyLevelId");
            parameters.Add("DifficultyLevelId", difficultyLevelId.Value);
        }

        if (isPrimary.HasValue)
        {
            whereConditions.Add("qc.IsPrimary = @IsPrimary");
            parameters.Add("IsPrimary", isPrimary.Value);
        }

        parameters.Add("Skip", skip);
        parameters.Add("Take", take);

        var sql = $@"
            SELECT qc.*, q.Title as QuestionTitle, qbc.Name as CategoryName, qbc.TreePath
            FROM QuestionCategorization qc
            INNER JOIN Questions q ON qc.QuestionId = q.QuestionId
            INNER JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
            WHERE {string.Join(" AND ", whereConditions)}
            ORDER BY qc.IsPrimary DESC, q.Title
            OFFSET @Skip ROWS
            FETCH NEXT @Take ROWS ONLY";

        return await connection.QueryAsync<QuestionCategorization>(sql, parameters);
    }

    public async Task<int> GetSearchResultCountAsync(
        string? searchTerm = null,
        int? categoryId = null,
        int? questionTypeId = null,
        int? difficultyLevelId = null,
        bool? isPrimary = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var whereConditions = new List<string> { "qc.IsDeleted = 0", "q.IsDeleted = 0" };
        var parameters = new DynamicParameters();

        // Apply same filtering logic as search method
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            whereConditions.Add("(q.Title LIKE @SearchTerm OR qbc.Name LIKE @SearchTerm)");
            parameters.Add("SearchTerm", $"%{searchTerm}%");
        }

        if (categoryId.HasValue)
        {
            whereConditions.Add("qc.CategoryId = @CategoryId");
            parameters.Add("CategoryId", categoryId.Value);
        }

        if (questionTypeId.HasValue)
        {
            whereConditions.Add("q.QuestionTypeId = @QuestionTypeId");
            parameters.Add("QuestionTypeId", questionTypeId.Value);
        }

        if (difficultyLevelId.HasValue)
        {
            whereConditions.Add("q.DifficultyLevelId = @DifficultyLevelId");
            parameters.Add("DifficultyLevelId", difficultyLevelId.Value);
        }

        if (isPrimary.HasValue)
        {
            whereConditions.Add("qc.IsPrimary = @IsPrimary");
            parameters.Add("IsPrimary", isPrimary.Value);
        }

        var sql = $@"
            SELECT COUNT(*)
            FROM QuestionCategorization qc
            INNER JOIN Questions q ON qc.QuestionId = q.QuestionId
            INNER JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
            WHERE {string.Join(" AND ", whereConditions)}";

        return await connection.QuerySingleAsync<int>(sql, parameters);
    }

    public async Task<bool> ValidateQuestionCategoryRelationshipsAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        // Check for invalid relationships
        const string validationSql = @"
            SELECT COUNT(*) as ErrorCount FROM (
                -- Questions with multiple primary categories
                SELECT QuestionId FROM QuestionCategorization 
                WHERE IsPrimary = 1 AND IsDeleted = 0
                GROUP BY QuestionId HAVING COUNT(*) > 1
                
                UNION ALL
                
                -- Categorizations pointing to non-existent questions
                SELECT qc.QuestionId FROM QuestionCategorization qc
                LEFT JOIN Questions q ON qc.QuestionId = q.QuestionId
                WHERE q.QuestionId IS NULL AND qc.IsDeleted = 0
                
                UNION ALL
                
                -- Categorizations pointing to non-existent categories
                SELECT qc.QuestionId FROM QuestionCategorization qc
                LEFT JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
                WHERE qbc.CategoryId IS NULL AND qc.IsDeleted = 0
            ) ValidationErrors";

        var errorCount = await connection.QuerySingleAsync<int>(validationSql);
        return errorCount == 0;
    }

    public async Task<bool> FixInvalidQuestionCategoryRelationshipsAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var currentDateTime = DateTime.UtcNow;

            // Fix multiple primary categories - keep only the most recent one
            const string fixMultiplePrimarySql = @"
                WITH RankedPrimaries AS (
                    SELECT QuestionId, CategoryId,
                           ROW_NUMBER() OVER (PARTITION BY QuestionId ORDER BY CreatedAt DESC) as rn
                    FROM QuestionCategorization
                    WHERE IsPrimary = 1 AND IsDeleted = 0
                )
                UPDATE qc SET IsPrimary = 0, ModifiedAt = @ModifiedAt
                FROM QuestionCategorization qc
                INNER JOIN RankedPrimaries rp ON qc.QuestionId = rp.QuestionId AND qc.CategoryId = rp.CategoryId
                WHERE rp.rn > 1 AND qc.IsPrimary = 1 AND qc.IsDeleted = 0";

            await connection.ExecuteAsync(fixMultiplePrimarySql,
                new { ModifiedAt = currentDateTime }, transaction);

            // Remove categorizations pointing to non-existent questions
            const string removeInvalidQuestionsSql = @"
                UPDATE qc SET IsDeleted = 1, ModifiedAt = @ModifiedAt
                FROM QuestionCategorization qc
                LEFT JOIN Questions q ON qc.QuestionId = q.QuestionId
                WHERE q.QuestionId IS NULL AND qc.IsDeleted = 0";

            await connection.ExecuteAsync(removeInvalidQuestionsSql,
                new { ModifiedAt = currentDateTime }, transaction);

            // Remove categorizations pointing to non-existent categories
            const string removeInvalidCategoriesSql = @"
                UPDATE qc SET IsDeleted = 1, ModifiedAt = @ModifiedAt
                FROM QuestionCategorization qc
                LEFT JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
                WHERE qbc.CategoryId IS NULL AND qc.IsDeleted = 0";

            await connection.ExecuteAsync(removeInvalidCategoriesSql,
                new { ModifiedAt = currentDateTime }, transaction);

            transaction.Commit();

            _logger.LogInformation("Fixed invalid question-category relationships");
            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error fixing invalid question-category relationships");
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetValidationErrorsAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var errors = new List<string>();

        // Check for questions with multiple primary categories
        const string multiplePrimarySql = @"
            SELECT QuestionId, COUNT(*) as PrimaryCount
            FROM QuestionCategorization
            WHERE IsPrimary = 1 AND IsDeleted = 0
            GROUP BY QuestionId
            HAVING COUNT(*) > 1";

        var multiplePrimaries = await connection.QueryAsync<(int QuestionId, int PrimaryCount)>(multiplePrimarySql);
        foreach (var mp in multiplePrimaries)
        {
            errors.Add($"Question {mp.QuestionId} has {mp.PrimaryCount} primary categories");
        }

        // Check for orphaned categorizations
        const string orphanedSql = @"
            SELECT qc.QuestionId, qc.CategoryId
            FROM QuestionCategorization qc
            LEFT JOIN Questions q ON qc.QuestionId = q.QuestionId
            LEFT JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
            WHERE (q.QuestionId IS NULL OR qbc.CategoryId IS NULL) AND qc.IsDeleted = 0";

        var orphaned = await connection.QueryAsync<(int QuestionId, int CategoryId)>(orphanedSql);
        foreach (var o in orphaned)
        {
            errors.Add($"Orphaned categorization: Question {o.QuestionId}, Category {o.CategoryId}");
        }

        return errors;
    }

    // Missing interface methods - temporary stub implementations
    public async Task<QuestionCategorization> AssignQuestionToCategoryAsync(
        int questionId, 
        int categoryId, 
        bool isPrimary = false, 
        int? assignedByUserId = null,
        decimal? weight = null,
        decimal? confidenceScore = null)
    {
        // This is the overload that matches the interface signature
        return await AssignQuestionToCategoryAsync(questionId, categoryId, isPrimary, 
            assignedByUserId);
    }

    public Task<QuestionCategorization?> UpdateQuestionCategorizationAsync(
        int questionId, 
        int categoryId, 
        bool? isPrimary = null,
        decimal? weight = null,
        decimal? confidenceScore = null)
    {
        throw new NotImplementedException("UpdateQuestionCategorizationAsync not implemented");
    }

    public Task<IEnumerable<QuestionCategoryDetailCore>> GetQuestionCategoriesAsync(int questionId, bool includeInactive = false)
    {
        throw new NotImplementedException("GetQuestionCategoriesAsync not implemented");
    }

    public Task<QuestionBankCategory?> GetPrimaryCategoryAsync(int questionId)
    {
        throw new NotImplementedException("GetPrimaryCategoryAsync returning QuestionBankCategory not implemented");
    }

    // Remove this duplicate method - there's already one that returns QuestionCategorization?
    // public Task<QuestionBankCategory?> GetPrimaryCategoryAsync(int questionId)

    public Task<PagedResult<QuestionCategoryDetailCore>> GetCategoryQuestionsAsync(
        int categoryId, 
        bool includeDescendants = false, 
        int pageNumber = 1, 
        int pageSize = 50)
    {
        throw new NotImplementedException("GetCategoryQuestionsAsync with pagination not implemented");
    }

    public Task<int> GetCategoryQuestionCountAsync(int categoryId, bool includeDescendants = false)
    {
        throw new NotImplementedException("GetCategoryQuestionCountAsync not implemented");
    }

    public Task<IDictionary<int, int>> GetQuestionDistributionAsync(int? parentCategoryId = null)
    {
        throw new NotImplementedException("GetQuestionDistributionAsync not implemented");
    }

    public Task<IEnumerable<QuestionBankHierarchy>> GetAllHierarchyEntriesAsync()
    {
        throw new NotImplementedException("GetAllHierarchyEntriesAsync not implemented");
    }

    public Task<QuestionBankHierarchy> CreateHierarchyEntryAsync(QuestionBankHierarchy hierarchy)
    {
        throw new NotImplementedException("CreateHierarchyEntryAsync not implemented");
    }

    public Task<int> DeleteHierarchyEntriesAsync(int categoryId)
    {
        throw new NotImplementedException("DeleteHierarchyEntriesAsync not implemented");
    }

    public Task<int> UpdateHierarchyAfterMoveAsync(int categoryId, int? oldParentId, int? newParentId)
    {
        throw new NotImplementedException("UpdateHierarchyAfterMoveAsync not implemented");
    }

    public Task<int> AssignQuestionsToCategory(IEnumerable<int> questionIds, int categoryId, int? assignedByUserId = null)
    {
        throw new NotImplementedException("AssignQuestionsToCategory not implemented");
    }

    public Task<int> RemoveQuestionsFromCategory(IEnumerable<int> questionIds, int categoryId)
    {
        throw new NotImplementedException("RemoveQuestionsFromCategory not implemented");
    }

    public Task<int> MoveQuestionsBetweenCategories(
        IEnumerable<int> questionIds, 
        int fromCategoryId, 
        int toCategoryId, 
        int? assignedByUserId = null)
    {
        throw new NotImplementedException("MoveQuestionsBetweenCategories not implemented");
    }

    public Task<int> CopyQuestionsBetweenCategories(
        IEnumerable<int> questionIds, 
        int fromCategoryId, 
        int toCategoryId, 
        int? assignedByUserId = null)
    {
        throw new NotImplementedException("CopyQuestionsBetweenCategories not implemented");
    }

    public Task<PagedResult<QuestionCategoryDetailCore>> SearchQuestionsByCategoryAsync(QuestionCategorySearchCriteriaCore searchCriteria)
    {
        throw new NotImplementedException("SearchQuestionsByCategoryAsync not implemented");
    }

    public Task<PagedResult<Question>> GetUncategorizedQuestionsAsync(int pageNumber = 1, int pageSize = 50)
    {
        throw new NotImplementedException("GetUncategorizedQuestionsAsync not implemented");
    }

    public Task<PagedResult<QuestionCategoryDetailCore>> GetMultiCategorizedQuestionsAsync(int pageNumber = 1, int pageSize = 50)
    {
        throw new NotImplementedException("GetMultiCategorizedQuestionsAsync not implemented");
    }

    public Task<PagedResult<QuestionCategoryDetailCore>> GetQuestionsWithoutPrimaryCategoryAsync(int pageNumber = 1, int pageSize = 50)
    {
        throw new NotImplementedException("GetQuestionsWithoutPrimaryCategoryAsync not implemented");
    }

    public Task<bool> IsQuestionInCategoryAsync(int questionId, int categoryId)
    {
        throw new NotImplementedException("IsQuestionInCategoryAsync not implemented");
    }

    public Task<bool> HasCategoryAssignmentsAsync(int questionId)
    {
        throw new NotImplementedException("HasCategoryAssignmentsAsync not implemented");
    }

    public Task<CategorizationValidationResultCore> ValidateCategorizationIntegrityAsync()
    {
        throw new NotImplementedException("ValidateCategorizationIntegrityAsync not implemented");
    }

    public Task<CategorizationStatisticsCore> GetCategorizationStatisticsAsync()
    {
        throw new NotImplementedException("GetCategorizationStatisticsAsync not implemented");
    }

    public Task<IEnumerable<CategoryUsageAnalyticsCore>> GetCategoryUsageAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        throw new NotImplementedException("GetCategoryUsageAnalyticsAsync not implemented");
    }

    public Task<AutoCategorizationMetricsCore> GetAutoCategorizationMetricsAsync()
    {
        throw new NotImplementedException("GetAutoCategorizationMetricsAsync not implemented");
    }

    public async Task<bool> BulkAssignQuestionsToCategoryAsync(
        IEnumerable<int> questionIds, 
        int categoryId, 
        int? createdByUserId = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            int assignedCount = 0;

            foreach (var questionId in questionIds)
            {
                // Check if assignment already exists
                const string checkExistsSql = @"
                    SELECT COUNT(1) 
                    FROM QuestionCategorization 
                    WHERE QuestionId = @QuestionId AND CategoryId = @CategoryId AND IsDeleted = 0";

                var exists = await connection.QuerySingleAsync<int>(
                    checkExistsSql,
                    new { QuestionId = questionId, CategoryId = categoryId },
                    transaction);

                if (exists == 0)
                {
                    const string insertSql = @"
                        INSERT INTO QuestionCategorization (QuestionId, CategoryId, IsPrimary, CreatedAt, CreatedByUserId)
                        VALUES (@QuestionId, @CategoryId, @IsPrimary, @CreatedAt, @CreatedByUserId)";

                    await connection.ExecuteAsync(
                        insertSql,
                        new
                        {
                            QuestionId = questionId,
                            CategoryId = categoryId,
                            IsPrimary = false,
                            CreatedAt = DateTime.UtcNow,
                            CreatedByUserId = createdByUserId
                        },
                        transaction);

                    assignedCount++;
                }
            }

            transaction.Commit();
            _logger.LogInformation("Bulk assigned {Count} questions to category {CategoryId}", assignedCount, categoryId);

            return assignedCount > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> BulkUnassignQuestionsFromCategoryAsync(IEnumerable<int> questionIds, int categoryId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = @"
            UPDATE QuestionCategorization 
            SET IsDeleted = 1, DeletedAt = @DeletedAt
            WHERE QuestionId IN @QuestionIds AND CategoryId = @CategoryId AND IsDeleted = 0";

        var affectedRows = await connection.ExecuteAsync(
            sql,
            new
            {
                QuestionIds = questionIds.ToArray(),
                CategoryId = categoryId,
                DeletedAt = DateTime.UtcNow
            });

        _logger.LogInformation("Bulk unassigned {Count} questions from category {CategoryId}", affectedRows, categoryId);
        return affectedRows > 0;
    }

    public async Task<bool> MoveQuestionsToNewCategoryAsync(IEnumerable<int> questionIds, int fromCategoryId, int toCategoryId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // First, remove from old category
            const string removeSql = @"
                UPDATE QuestionCategorization 
                SET IsDeleted = 1, DeletedAt = @DeletedAt
                WHERE QuestionId IN @QuestionIds AND CategoryId = @FromCategoryId AND IsDeleted = 0";

            await connection.ExecuteAsync(
                removeSql,
                new
                {
                    QuestionIds = questionIds.ToArray(),
                    FromCategoryId = fromCategoryId,
                    DeletedAt = DateTime.UtcNow
                },
                transaction);

            // Then, add to new category (check for existing assignments first)
            foreach (var questionId in questionIds)
            {
                const string checkExistsSql = @"
                    SELECT COUNT(1) 
                    FROM QuestionCategorization 
                    WHERE QuestionId = @QuestionId AND CategoryId = @ToCategoryId AND IsDeleted = 0";

                var exists = await connection.QuerySingleAsync<int>(
                    checkExistsSql,
                    new { QuestionId = questionId, ToCategoryId = toCategoryId },
                    transaction);

                if (exists == 0)
                {
                    const string insertSql = @"
                        INSERT INTO QuestionCategorization (QuestionId, CategoryId, IsPrimary, CreatedAt)
                        VALUES (@QuestionId, @ToCategoryId, @IsPrimary, @CreatedAt)";

                    await connection.ExecuteAsync(
                        insertSql,
                        new
                        {
                            QuestionId = questionId,
                            ToCategoryId = toCategoryId,
                            IsPrimary = false,
                            CreatedAt = DateTime.UtcNow
                        },
                        transaction);
                }
            }

            transaction.Commit();
            _logger.LogInformation("Moved {Count} questions from category {FromCategoryId} to {ToCategoryId}", 
                questionIds.Count(), fromCategoryId, toCategoryId);

            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
