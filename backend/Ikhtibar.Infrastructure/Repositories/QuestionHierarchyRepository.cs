using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using QuestionCategoryDetailCore = Ikhtibar.Core.Repositories.Interfaces.QuestionCategoryDetail;
using QuestionCategorySearchCriteriaCore = Ikhtibar.Core.Repositories.Interfaces.QuestionCategorySearchCriteria;
using CategorizationValidationResultCore = Ikhtibar.Core.Repositories.Interfaces.CategorizationValidationResult;
using CategorizationValidationIssueCore = Ikhtibar.Core.Repositories.Interfaces.CategorizationValidationIssue;
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

    public async Task<QuestionCategorization> UpdateQuestionCategorizationAsync(
        int questionId, 
        int categoryId, 
        bool? isPrimary = null,
        decimal? weight = null,
        decimal? confidenceScore = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var updateFields = new List<string>();
            var parameters = new Dapper.DynamicParameters();
            parameters.Add("QuestionId", questionId);
            parameters.Add("CategoryId", categoryId);
            parameters.Add("ModifiedAt", DateTime.UtcNow);

            if (isPrimary.HasValue)
            {
                updateFields.Add("IsPrimary = @IsPrimary");
                parameters.Add("IsPrimary", isPrimary.Value);
            }

            if (weight.HasValue)
            {
                updateFields.Add("Weight = @Weight");
                parameters.Add("Weight", weight.Value);
            }

            if (confidenceScore.HasValue)
            {
                updateFields.Add("ConfidenceScore = @ConfidenceScore");
                parameters.Add("ConfidenceScore", confidenceScore.Value);
            }

            if (!updateFields.Any())
            {
                throw new ArgumentException("At least one field must be specified for update");
            }

            updateFields.Add("ModifiedAt = @ModifiedAt");

            var sql = $@"
                UPDATE QuestionCategorization 
                SET {string.Join(", ", updateFields)}
                WHERE QuestionId = @QuestionId AND CategoryId = @CategoryId AND IsDeleted = 0";

            var rowsAffected = await connection.ExecuteAsync(sql, parameters, transaction);

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException("Question categorization not found or already deleted");
            }

            transaction.Commit();
            _logger.LogInformation("Updated question categorization for question {QuestionId} in category {CategoryId}", questionId, categoryId);

            // Return updated categorization
            const string getSql = @"
                SELECT * FROM QuestionCategorization 
                WHERE QuestionId = @QuestionId AND CategoryId = @CategoryId AND IsDeleted = 0";

            var result = await connection.QuerySingleOrDefaultAsync<QuestionCategorization>(getSql, new { QuestionId = questionId, CategoryId = categoryId });
            return result;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error updating question categorization for question {QuestionId} in category {CategoryId}", questionId, categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionCategoryDetailCore>> GetQuestionCategoriesAsync(int questionId, bool includeInactive = false)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var sql = @"
                SELECT 
                    qc.QuestionId,
                    qc.CategoryId,
                    qc.IsPrimary,
                    qc.Weight,
                    qc.ConfidenceScore,
                    qc.CreatedAt,
                    qc.CreatedByUserId,
                    qc.ModifiedAt,
                    qc.ModifiedByUserId,
                    qbc.Name as CategoryName,
                    qbc.Description as CategoryDescription,
                    qbc.IsActive as CategoryIsActive,
                    qbc.ParentCategoryId as CategoryParentId
                FROM QuestionCategorization qc
                INNER JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
                WHERE qc.QuestionId = @QuestionId AND qc.IsDeleted = 0";

            if (!includeInactive)
            {
                sql += " AND qbc.IsActive = 1";
            }

            sql += " ORDER BY qc.IsPrimary DESC, qc.CreatedAt";

            var results = await connection.QueryAsync<QuestionCategoryDetailCore>(sql, new { QuestionId = questionId });

            _logger.LogInformation("Retrieved {Count} categories for question {QuestionId}", results.Count(), questionId);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories for question {QuestionId}", questionId);
            throw;
        }
    }

    public async Task<QuestionBankCategory?> GetPrimaryCategoryAsync(int questionId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            const string sql = @"
                SELECT qbc.*
                FROM QuestionCategorization qc
                INNER JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
                WHERE qc.QuestionId = @QuestionId AND qc.IsPrimary = 1 AND qc.IsDeleted = 0 AND qbc.IsDeleted = 0";

            var result = await connection.QuerySingleOrDefaultAsync<QuestionBankCategory>(sql, new { QuestionId = questionId });

            if (result != null)
            {
                _logger.LogInformation("Retrieved primary category {CategoryId} for question {QuestionId}", result.CategoryId, questionId);
            }
            else
            {
                _logger.LogInformation("No primary category found for question {QuestionId}", questionId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving primary category for question {QuestionId}", questionId);
            throw;
        }
    }

    // Remove this duplicate method - there's already one that returns QuestionCategorization?
    // public Task<QuestionBankCategory?> GetPrimaryCategoryAsync(int questionId)

    public async Task<PagedResult<QuestionCategoryDetailCore>> GetCategoryQuestionsAsync(
        int categoryId, 
        bool includeDescendants = false, 
        int pageNumber = 1, 
        int pageSize = 50)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var offset = (pageNumber - 1) * pageSize;
            var categoryIds = new List<int> { categoryId };

            if (includeDescendants)
            {
                // Get all descendant category IDs
                var descendantSql = @"
                    WITH RECURSIVE CategoryHierarchy AS (
                        SELECT CategoryId, ParentCategoryId, 0 as Level
                        FROM QuestionBankCategory
                        WHERE CategoryId = @CategoryId AND IsDeleted = 0
                        
                        UNION ALL
                        
                        SELECT c.CategoryId, c.ParentCategoryId, h.Level + 1
                        FROM QuestionBankCategory c
                        INNER JOIN CategoryHierarchy h ON c.ParentCategoryId = h.CategoryId
                        WHERE c.IsDeleted = 0
                    )
                    SELECT CategoryId FROM CategoryHierarchy";

                var descendants = await connection.QueryAsync<int>(descendantSql, new { CategoryId = categoryId });
                categoryIds.AddRange(descendants);
            }

            // Get total count
            var countSql = @"
                SELECT COUNT(DISTINCT qc.QuestionId)
                FROM QuestionCategorization qc
                WHERE qc.CategoryId IN @CategoryIds AND qc.IsDeleted = 0";

            var totalCount = await connection.QuerySingleAsync<int>(countSql, new { CategoryIds = categoryIds });

            // Get paginated results
            var sql = @"
                SELECT DISTINCT
                    qc.QuestionId,
                    qc.CategoryId,
                    qc.IsPrimary,
                    qc.Weight,
                    qc.ConfidenceScore,
                    qc.CreatedAt,
                    qc.CreatedByUserId,
                    qc.ModifiedAt,
                    qc.ModifiedByUserId,
                    qbc.Name as CategoryName,
                    qbc.Description as CategoryDescription,
                    qbc.IsActive as CategoryIsActive,
                    qbc.ParentCategoryId as CategoryParentId
                FROM QuestionCategorization qc
                INNER JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
                WHERE qc.CategoryId IN @CategoryIds AND qc.IsDeleted = 0
                ORDER BY qc.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var results = await connection.QueryAsync<QuestionCategoryDetailCore>(
                sql, 
                new { CategoryIds = categoryIds, Offset = offset, PageSize = pageSize });

            _logger.LogInformation("Retrieved {Count} questions for category {CategoryId} (page {Page}, size {PageSize})", 
                results.Count(), categoryId, pageNumber, pageSize);

            return new PagedResult<QuestionCategoryDetailCore>
            {
                Items = results,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving questions for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<int> GetCategoryQuestionCountAsync(int categoryId, bool includeDescendants = false)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var categoryIds = new List<int> { categoryId };

            if (includeDescendants)
            {
                // Get all descendant category IDs
                var descendantSql = @"
                    WITH RECURSIVE CategoryHierarchy AS (
                        SELECT CategoryId, ParentCategoryId, 0 as Level
                        FROM QuestionBankCategory
                        WHERE CategoryId = @CategoryId AND IsDeleted = 0
                        
                        UNION ALL
                        
                        SELECT c.CategoryId, c.ParentCategoryId, h.Level + 1
                        FROM QuestionBankCategory c
                        INNER JOIN CategoryHierarchy h ON c.ParentCategoryId = h.CategoryId
                        WHERE c.IsDeleted = 0
                    )
                    SELECT CategoryId FROM CategoryHierarchy";

                var descendants = await connection.QueryAsync<int>(descendantSql, new { CategoryId = categoryId });
                categoryIds.AddRange(descendants);
            }

            const string sql = @"
                SELECT COUNT(DISTINCT qc.QuestionId)
                FROM QuestionCategorization qc
                WHERE qc.CategoryId IN @CategoryIds AND qc.IsDeleted = 0";

            var count = await connection.QuerySingleAsync<int>(sql, new { CategoryIds = categoryIds });

            _logger.LogInformation("Category {CategoryId} has {Count} questions (includeDescendants: {IncludeDescendants})", 
                categoryId, count, includeDescendants);

            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting question count for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IDictionary<int, int>> GetQuestionDistributionAsync(int? parentCategoryId = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var sql = @"
                SELECT 
                    qbc.CategoryId,
                    COUNT(DISTINCT qc.QuestionId) as QuestionCount
                FROM QuestionBankCategory qbc
                LEFT JOIN QuestionCategorization qc ON qbc.CategoryId = qc.CategoryId AND qc.IsDeleted = 0
                WHERE qbc.IsDeleted = 0";

            if (parentCategoryId.HasValue)
            {
                sql += " AND qbc.ParentCategoryId = @ParentCategoryId";
            }
            else
            {
                sql += " AND qbc.ParentCategoryId IS NULL";
            }

            sql += " GROUP BY qbc.CategoryId ORDER BY QuestionCount DESC";

            var results = await connection.QueryAsync(sql, new { ParentCategoryId = parentCategoryId });
            
            var distribution = results.ToDictionary(
                row => (int)row.CategoryId, 
                row => (int)row.QuestionCount);

            _logger.LogInformation("Retrieved question distribution for {CategoryCount} categories", distribution.Count);
            return distribution;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting question distribution for parent category {ParentCategoryId}", parentCategoryId);
            throw;
        }
    }

    public async Task<IEnumerable<QuestionBankHierarchy>> GetAllHierarchyEntriesAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            const string sql = @"
                SELECT 
                    h.*,
                    c.Name as CategoryName,
                    c.Description as CategoryDescription,
                    c.IsActive as CategoryIsActive,
                    p.Name as AncestorCategoryName
                FROM QuestionBankHierarchy h
                INNER JOIN QuestionBankCategory c ON h.DescendantId = c.CategoryId
                LEFT JOIN QuestionBankCategory p ON h.AncestorId = p.CategoryId
                WHERE h.IsDeleted = 0 AND c.IsDeleted = 0
                ORDER BY h.Depth, h.SortOrder, c.Name";

            var results = await connection.QueryAsync<QuestionBankHierarchy>(sql);

            _logger.LogInformation("Retrieved {Count} hierarchy entries", results.Count());
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all hierarchy entries");
            throw;
        }
    }

    public async Task<QuestionBankHierarchy> CreateHierarchyEntryAsync(QuestionBankHierarchy hierarchy)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Validate parent category exists if specified
            if (hierarchy.AncestorId > 0)
            {
                const string validateParentSql = @"
                    SELECT COUNT(1) FROM QuestionBankCategory 
                    WHERE CategoryId = @AncestorId AND IsDeleted = 0";

                var parentExists = await connection.QuerySingleAsync<int>(
                    validateParentSql, 
                    new { AncestorId = hierarchy.AncestorId },
                    transaction);

                if (parentExists == 0)
                {
                    throw new InvalidOperationException($"Ancestor category {hierarchy.AncestorId} does not exist");
                }
            }

            // Calculate depth
            if (hierarchy.AncestorId > 0)
            {
                const string getAncestorDepthSql = @"
                    SELECT Depth FROM QuestionBankHierarchy 
                    WHERE DescendantId = @AncestorId AND IsDeleted = 0";

                var ancestorDepth = await connection.QuerySingleAsync<int>(
                    getAncestorDepthSql, 
                    new { AncestorId = hierarchy.AncestorId },
                    transaction);

                hierarchy.Depth = ancestorDepth + 1;
            }
            else
            {
                hierarchy.Depth = 0;
            }

            // Get next sort order for this depth and ancestor
            const string getNextSortOrderSql = @"
                SELECT ISNULL(MAX(SortOrder), 0) + 1
                FROM QuestionBankHierarchy 
                WHERE Depth = @Depth AND (@AncestorId = 0 AND AncestorId = 0 OR AncestorId = @AncestorId) AND IsDeleted = 0";

            var nextSortOrder = await connection.QuerySingleAsync<int>(
                getNextSortOrderSql, 
                new { Depth = hierarchy.Depth, AncestorId = hierarchy.AncestorId },
                transaction);

            hierarchy.SortOrder = nextSortOrder;
            hierarchy.EstablishedAt = DateTime.UtcNow;

            const string insertSql = @"
                INSERT INTO QuestionBankHierarchy 
                (AncestorId, DescendantId, Depth, Path, IsDirect, SortOrder, EstablishedAt, IsDeleted)
                VALUES (@AncestorId, @DescendantId, @Depth, @Path, @IsDirect, @SortOrder, @EstablishedAt, 0);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            var hierarchyId = await connection.QuerySingleAsync<int>(insertSql, hierarchy, transaction);
            hierarchy.HierarchyId = hierarchyId;

            transaction.Commit();
            _logger.LogInformation("Created hierarchy entry {HierarchyId} for category {CategoryId}", hierarchyId, hierarchy.DescendantId);

            return hierarchy;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error creating hierarchy entry for category {DescendantId}", hierarchy.DescendantId);
            throw;
        }
    }

    public async Task<int> DeleteHierarchyEntriesAsync(int categoryId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Get all descendant category IDs
            var descendantSql = @"
                WITH RECURSIVE CategoryHierarchy AS (
                    SELECT CategoryId, ParentCategoryId, 0 as Level
                    FROM QuestionBankCategory
                    WHERE CategoryId = @CategoryId AND IsDeleted = 0
                    
                    UNION ALL
                    
                    SELECT c.CategoryId, c.ParentCategoryId, h.Level + 1
                    FROM QuestionBankCategory c
                    INNER JOIN CategoryHierarchy h ON c.ParentCategoryId = h.CategoryId
                    WHERE c.IsDeleted = 0
                )
                SELECT CategoryId FROM CategoryHierarchy";

            var descendants = await connection.QueryAsync<int>(descendantSql, new { CategoryId = categoryId }, transaction);
            var allCategoryIds = new List<int> { categoryId };
            allCategoryIds.AddRange(descendants);

            // Soft delete hierarchy entries
            const string deleteHierarchySql = @"
                UPDATE QuestionBankHierarchy 
                SET IsDeleted = 1, ModifiedAt = @ModifiedAt
                WHERE CategoryId IN @CategoryIds";

            var hierarchyDeleted = await connection.ExecuteAsync(
                deleteHierarchySql, 
                new { CategoryIds = allCategoryIds, ModifiedAt = DateTime.UtcNow },
                transaction);

            // Soft delete question categorizations
            const string deleteCategorizationsSql = @"
                UPDATE QuestionCategorization 
                SET IsDeleted = 1, ModifiedAt = @ModifiedAt
                WHERE CategoryId IN @CategoryIds";

            var categorizationsDeleted = await connection.ExecuteAsync(
                deleteCategorizationsSql, 
                new { CategoryIds = allCategoryIds, ModifiedAt = DateTime.UtcNow },
                transaction);

            transaction.Commit();
            _logger.LogInformation("Deleted {HierarchyCount} hierarchy entries and {CategorizationCount} categorizations for category {CategoryId}", 
                hierarchyDeleted, categorizationsDeleted, categoryId);

            return hierarchyDeleted + categorizationsDeleted;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error deleting hierarchy entries for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<int> UpdateHierarchyAfterMoveAsync(int categoryId, int? oldParentId, int? newParentId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Calculate new depth
            int newDepth = 0;
            if (newParentId.HasValue)
            {
                const string getParentDepthSql = @"
                    SELECT Depth FROM QuestionBankHierarchy 
                    WHERE DescendantId = @ParentId AND IsDeleted = 0";

                var parentDepth = await connection.QuerySingleAsync<int>(
                    getParentDepthSql, 
                    new { ParentId = newParentId.Value },
                    transaction);

                newDepth = parentDepth + 1;
            }

            // Update the moved category
            const string updateCategorySql = @"
                UPDATE QuestionBankHierarchy 
                SET AncestorId = @NewParentId, Depth = @NewDepth, ModifiedAt = @ModifiedAt
                WHERE DescendantId = @CategoryId AND IsDeleted = 0";

            var rowsAffected = await connection.ExecuteAsync(
                updateCategorySql, 
                new { CategoryId = categoryId, NewParentId = newParentId, NewDepth = newDepth, ModifiedAt = DateTime.UtcNow },
                transaction);

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Category {categoryId} not found in hierarchy");
            }

            // Update all descendant depths
            var updateDescendantsSql = @"
                WITH RECURSIVE CategoryHierarchy AS (
                    SELECT DescendantId, AncestorId, @NewDepth as NewDepth, 0 as Depth
                    FROM QuestionBankCategory
                    WHERE CategoryId = @CategoryId AND IsDeleted = 0
                    
                    UNION ALL
                    
                    SELECT c.CategoryId, c.ParentCategoryId, h.NewDepth + 1, h.Depth + 1
                    FROM QuestionBankCategory c
                    INNER JOIN CategoryHierarchy h ON c.ParentCategoryId = h.DescendantId
                    WHERE c.IsDeleted = 0
                )
                UPDATE h SET Depth = ch.NewDepth, ModifiedAt = @ModifiedAt
                FROM QuestionBankHierarchy h
                INNER JOIN CategoryHierarchy ch ON h.DescendantId = ch.DescendantId
                WHERE h.IsDeleted = 0";

            var descendantsUpdated = await connection.ExecuteAsync(
                updateDescendantsSql, 
                new { CategoryId = categoryId, NewDepth = newDepth, ModifiedAt = DateTime.UtcNow },
                transaction);

            transaction.Commit();
            _logger.LogInformation("Updated hierarchy for category {CategoryId} moved from parent {OldParentId} to {NewParentId}. {DescendantsUpdated} descendants updated", 
                categoryId, oldParentId, newParentId, descendantsUpdated);

            return rowsAffected + descendantsUpdated;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error updating hierarchy after move for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<int> AssignQuestionsToCategory(IEnumerable<int> questionIds, int categoryId, int? assignedByUserId = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var questionIdList = questionIds.ToList();
            if (!questionIdList.Any())
            {
                return 0;
            }

            // Check if category exists and is active
            const string validateCategorySql = @"
                SELECT COUNT(1) FROM QuestionBankCategory 
                WHERE CategoryId = @CategoryId AND IsActive = 1 AND IsDeleted = 0";

            var categoryExists = await connection.QuerySingleAsync<int>(
                validateCategorySql, 
                new { CategoryId = categoryId },
                transaction);

            if (categoryExists == 0)
            {
                throw new InvalidOperationException($"Category {categoryId} does not exist or is not active");
            }

            // Check existing assignments to avoid duplicates
            const string checkExistingSql = @"
                SELECT QuestionId FROM QuestionCategorization 
                WHERE QuestionId IN @QuestionIds AND CategoryId = @CategoryId AND IsDeleted = 0";

            var existingAssignments = await connection.QueryAsync<int>(
                checkExistingSql, 
                new { QuestionIds = questionIdList, CategoryId = categoryId },
                transaction);

            var newQuestionIds = questionIdList.Except(existingAssignments).ToList();

            if (!newQuestionIds.Any())
            {
                _logger.LogInformation("All questions are already assigned to category {CategoryId}", categoryId);
                return 0;
            }

            // Insert new assignments
            const string insertSql = @"
                INSERT INTO QuestionCategorization 
                (QuestionId, CategoryId, IsPrimary, CreatedByUserId, CreatedAt, IsDeleted)
                VALUES (@QuestionId, @CategoryId, @IsPrimary, @CreatedByUserId, @CreatedAt, 0)";

            var assignments = newQuestionIds.Select(questionId => new
            {
                QuestionId = questionId,
                CategoryId = categoryId,
                IsPrimary = false, // New assignments are secondary by default
                CreatedByUserId = assignedByUserId ?? 0,
                CreatedAt = DateTime.UtcNow
            });

            var rowsAffected = await connection.ExecuteAsync(insertSql, assignments, transaction);

            transaction.Commit();
            _logger.LogInformation("Assigned {Count} questions to category {CategoryId}", rowsAffected, categoryId);

            return rowsAffected;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error assigning questions to category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<int> RemoveQuestionsFromCategory(IEnumerable<int> questionIds, int categoryId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var questionIdList = questionIds.ToList();
            if (!questionIdList.Any())
            {
                return 0;
            }

            // Check if any questions are primary in this category
            const string checkPrimarySql = @"
                SELECT QuestionId FROM QuestionCategorization 
                WHERE QuestionId IN @QuestionIds AND CategoryId = @CategoryId AND IsPrimary = 1 AND IsDeleted = 0";

            var primaryQuestions = await connection.QueryAsync<int>(
                checkPrimarySql, 
                new { QuestionIds = questionIdList, CategoryId = categoryId },
                transaction);

            if (primaryQuestions.Any())
            {
                throw new InvalidOperationException($"Cannot remove primary category assignments for questions: {string.Join(", ", primaryQuestions)}");
            }

            // Soft delete the assignments
            const string deleteSql = @"
                UPDATE QuestionCategorization 
                SET IsDeleted = 1, ModifiedAt = @ModifiedAt
                WHERE QuestionId IN @QuestionIds AND CategoryId = @CategoryId AND IsDeleted = 0";

            var rowsAffected = await connection.ExecuteAsync(
                deleteSql, 
                new { QuestionIds = questionIdList, CategoryId = categoryId, ModifiedAt = DateTime.UtcNow },
                transaction);

            transaction.Commit();
            _logger.LogInformation("Removed {Count} questions from category {CategoryId}", rowsAffected, categoryId);

            return rowsAffected;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error removing questions from category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<int> MoveQuestionsBetweenCategories(
        IEnumerable<int> questionIds, 
        int fromCategoryId, 
        int toCategoryId, 
        int? assignedByUserId = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var questionIdList = questionIds.ToList();
            if (!questionIdList.Any())
            {
                return 0;
            }

            // Validate both categories exist and are active
            const string validateCategoriesSql = @"
                SELECT CategoryId, IsActive FROM QuestionBankCategory 
                WHERE CategoryId IN @CategoryIds AND IsDeleted = 0";

            var categories = await connection.QueryAsync(validateCategoriesSql, 
                new { CategoryIds = new[] { fromCategoryId, toCategoryId } },
                transaction);

            var categoryDict = categories.ToDictionary(row => (int)row.CategoryId, row => (bool)row.IsActive);

            if (!categoryDict.ContainsKey(fromCategoryId) || !categoryDict.ContainsKey(toCategoryId))
            {
                throw new InvalidOperationException("One or both categories do not exist");
            }

            if (!categoryDict[fromCategoryId] || !categoryDict[toCategoryId])
            {
                throw new InvalidOperationException("One or both categories are not active");
            }

            // Check if questions are assigned to the source category
            const string checkSourceSql = @"
                SELECT QuestionId, IsPrimary FROM QuestionCategorization 
                WHERE QuestionId IN @QuestionIds AND CategoryId = @FromCategoryId AND IsDeleted = 0";

            var sourceAssignments = await connection.QueryAsync(checkSourceSql, 
                new { QuestionIds = questionIdList, FromCategoryId = fromCategoryId },
                transaction);

            if (!sourceAssignments.Any())
            {
                _logger.LogInformation("No questions found in source category {FromCategoryId}", fromCategoryId);
                return 0;
            }

            // Remove from source category
            const string removeFromSourceSql = @"
                UPDATE QuestionCategorization 
                SET IsDeleted = 1, ModifiedAt = @ModifiedAt
                WHERE QuestionId IN @QuestionIds AND CategoryId = @FromCategoryId AND IsDeleted = 0";

            var removedFromSource = await connection.ExecuteAsync(removeFromSourceSql, 
                new { QuestionIds = questionIdList, FromCategoryId = fromCategoryId, ModifiedAt = DateTime.UtcNow },
                transaction);

            // Add to target category
            const string addToTargetSql = @"
                INSERT INTO QuestionCategorization 
                (QuestionId, CategoryId, IsPrimary, CreatedByUserId, CreatedAt, IsDeleted)
                VALUES (@QuestionId, @CategoryId, @IsPrimary, @CreatedByUserId, @CreatedAt, 0)";

            var assignments = sourceAssignments.Select(assignment => new
            {
                QuestionId = (int)assignment.QuestionId,
                CategoryId = toCategoryId,
                IsPrimary = (bool)assignment.IsPrimary,
                CreatedByUserId = assignedByUserId ?? 0,
                CreatedAt = DateTime.UtcNow
            });

            var addedToTarget = await connection.ExecuteAsync(addToTargetSql, assignments, transaction);

            transaction.Commit();
            _logger.LogInformation("Moved {Count} questions from category {FromCategoryId} to {ToCategoryId}", 
                addedToTarget, fromCategoryId, toCategoryId);

            return addedToTarget;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error moving questions from category {FromCategoryId} to {ToCategoryId}", fromCategoryId, toCategoryId);
            throw;
        }
    }

    public async Task<int> CopyQuestionsBetweenCategories(
        IEnumerable<int> questionIds, 
        int fromCategoryId, 
        int toCategoryId, 
        int? assignedByUserId = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var questionIdList = questionIds.ToList();
            if (!questionIdList.Any())
            {
                return 0;
            }

            // Validate both categories exist and are active
            const string validateCategoriesSql = @"
                SELECT CategoryId, IsActive FROM QuestionBankCategory 
                WHERE CategoryId IN @CategoryIds AND IsDeleted = 0";

            var categories = await connection.QueryAsync(validateCategoriesSql, 
                new { CategoryIds = new[] { fromCategoryId, toCategoryId } },
                transaction);

            var categoryDict = categories.ToDictionary(row => (int)row.CategoryId, row => (bool)row.IsActive);

            if (!categoryDict.ContainsKey(fromCategoryId) || !categoryDict.ContainsKey(toCategoryId))
            {
                throw new InvalidOperationException("One or both categories do not exist");
            }

            if (!categoryDict[fromCategoryId] || !categoryDict[toCategoryId])
            {
                throw new InvalidOperationException("One or both categories are not active");
            }

            // Check if questions are assigned to the source category
            const string checkSourceSql = @"
                SELECT QuestionId, IsPrimary FROM QuestionCategorization 
                WHERE QuestionId IN @QuestionIds AND CategoryId = @FromCategoryId AND IsDeleted = 0";

            var sourceAssignments = await connection.QueryAsync(checkSourceSql, 
                new { QuestionIds = questionIdList, FromCategoryId = fromCategoryId },
                transaction);

            if (!sourceAssignments.Any())
            {
                _logger.LogInformation("No questions found in source category {FromCategoryId}", fromCategoryId);
                return 0;
            }

            // Check existing assignments in target category to avoid duplicates
            const string checkExistingSql = @"
                SELECT QuestionId FROM QuestionCategorization 
                WHERE QuestionId IN @QuestionIds AND CategoryId = @ToCategoryId AND IsDeleted = 0";

            var existingAssignments = await connection.QueryAsync<int>(
                checkExistingSql, 
                new { QuestionIds = questionIdList, ToCategoryId = toCategoryId },
                transaction);

            var newQuestionIds = questionIdList.Except(existingAssignments).ToList();

            if (!newQuestionIds.Any())
            {
                _logger.LogInformation("All questions are already assigned to target category {ToCategoryId}", toCategoryId);
                return 0;
            }

            // Add to target category (copy, so IsPrimary is always false)
            const string addToTargetSql = @"
                INSERT INTO QuestionCategorization 
                (QuestionId, CategoryId, IsPrimary, CreatedByUserId, CreatedAt, IsDeleted)
                VALUES (@QuestionId, @CategoryId, @IsPrimary, @CreatedByUserId, @CreatedAt, 0)";

            var assignments = newQuestionIds.Select(questionId => new
            {
                QuestionId = questionId,
                CategoryId = toCategoryId,
                IsPrimary = false, // Copy is always secondary
                CreatedByUserId = assignedByUserId ?? 0,
                CreatedAt = DateTime.UtcNow
            });

            var addedToTarget = await connection.ExecuteAsync(addToTargetSql, assignments, transaction);

            transaction.Commit();
            _logger.LogInformation("Copied {Count} questions from category {FromCategoryId} to {ToCategoryId}", 
                addedToTarget, fromCategoryId, toCategoryId);

            return addedToTarget;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error copying questions from category {FromCategoryId} to {ToCategoryId}", fromCategoryId, toCategoryId);
            throw;
        }
    }

    public async Task<PagedResult<QuestionCategoryDetailCore>> SearchQuestionsByCategoryAsync(QuestionCategorySearchCriteriaCore searchCriteria)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var offset = (searchCriteria.PageNumber - 1) * searchCriteria.PageSize;
            var whereConditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            // Build search conditions
            if (searchCriteria.CategoryId.HasValue)
            {
                whereConditions.Add("qc.CategoryId = @CategoryId");
                parameters["CategoryId"] = searchCriteria.CategoryId.Value;
            }

            if (!string.IsNullOrEmpty(searchCriteria.QuestionText))
            {
                whereConditions.Add("q.Text LIKE @QuestionText");
                parameters["QuestionText"] = $"%{searchCriteria.QuestionText}%";
            }

            if (searchCriteria.IsPrimary.HasValue)
            {
                whereConditions.Add("qc.IsPrimary = @IsPrimary");
                parameters["IsPrimary"] = searchCriteria.IsPrimary.Value;
            }

            if (searchCriteria.MinWeight.HasValue)
            {
                whereConditions.Add("qc.Weight >= @MinWeight");
                parameters["MinWeight"] = searchCriteria.MinWeight.Value;
            }



            if (searchCriteria.AssignedAfter.HasValue)
            {
                whereConditions.Add("qc.CreatedAt >= @AssignedAfter");
                parameters["AssignedAfter"] = searchCriteria.AssignedAfter.Value;
            }

            if (searchCriteria.AssignedBefore.HasValue)
            {
                whereConditions.Add("qc.CreatedAt <= @AssignedBefore");
                parameters["AssignedBefore"] = searchCriteria.AssignedBefore.Value;
            }

            if (searchCriteria.MinConfidenceScore.HasValue)
            {
                whereConditions.Add("qc.ConfidenceScore >= @MinConfidenceScore");
                parameters["MinConfidenceScore"] = searchCriteria.MinConfidenceScore.Value;
            }

            // Add pagination parameters
            parameters["Offset"] = offset;
            parameters["PageSize"] = searchCriteria.PageSize;

            var whereClause = whereConditions.Any() ? $"WHERE {string.Join(" AND ", whereConditions)}" : "";

            // Get total count
            var countSql = $@"
                SELECT COUNT(DISTINCT qc.QuestionId)
                FROM QuestionCategorization qc
                INNER JOIN Question q ON qc.QuestionId = q.QuestionId
                INNER JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
                {whereClause} AND qc.IsDeleted = 0 AND q.IsDeleted = 0 AND qbc.IsDeleted = 0";

            var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

            // Get paginated results
            var sql = $@"
                SELECT DISTINCT
                    qc.QuestionId,
                    qc.CategoryId,
                    qc.IsPrimary,
                    qc.Weight,
                    qc.ConfidenceScore,
                    qc.CreatedAt,
                    qc.CreatedByUserId,
                    qc.ModifiedAt,
                    qc.ModifiedByUserId,
                    qbc.Name as CategoryName,
                    qbc.Description as CategoryDescription,
                    qbc.IsActive as CategoryIsActive,
                    qbc.ParentCategoryId as CategoryParentId
                FROM QuestionCategorization qc
                INNER JOIN Question q ON qc.QuestionId = q.QuestionId
                INNER JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
                {whereClause} AND qc.IsDeleted = 0 AND q.IsDeleted = 0 AND qbc.IsDeleted = 0
                ORDER BY qc.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var results = await connection.QueryAsync<QuestionCategoryDetailCore>(sql, parameters);

            _logger.LogInformation("Search returned {Count} questions for criteria: {Criteria}", 
                results.Count(), searchCriteria.ToString());

            return new PagedResult<QuestionCategoryDetailCore>
            {
                Items = results,
                TotalCount = totalCount,
                PageNumber = searchCriteria.PageNumber,
                PageSize = searchCriteria.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching questions by category criteria");
            throw;
        }
    }

    public async Task<PagedResult<Question>> GetUncategorizedQuestionsAsync(int pageNumber = 1, int pageSize = 50)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var offset = (pageNumber - 1) * pageSize;

            // Get total count
            const string countSql = @"
                SELECT COUNT(1)
                FROM Question q
                WHERE q.IsDeleted = 0 
                AND NOT EXISTS (
                    SELECT 1 FROM QuestionCategorization qc 
                    WHERE qc.QuestionId = q.QuestionId AND qc.IsDeleted = 0
                )";

            var totalCount = await connection.QuerySingleAsync<int>(countSql);

            // Get paginated results
            const string sql = @"
                SELECT q.*
                FROM Question q
                WHERE q.IsDeleted = 0 
                AND NOT EXISTS (
                    SELECT 1 FROM QuestionCategorization qc 
                    WHERE qc.QuestionId = q.QuestionId AND qc.IsDeleted = 0
                )
                ORDER BY q.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var results = await connection.QueryAsync<Question>(
                sql, 
                new { Offset = offset, PageSize = pageSize });

            _logger.LogInformation("Retrieved {Count} uncategorized questions (page {Page}, size {PageSize})", 
                results.Count(), pageNumber, pageSize);

            return new PagedResult<Question>
            {
                Items = results,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving uncategorized questions");
            throw;
        }
    }

    public async Task<PagedResult<QuestionCategoryDetailCore>> GetMultiCategorizedQuestionsAsync(int pageNumber = 1, int pageSize = 50)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var offset = (pageNumber - 1) * pageSize;

            // Get total count
            const string countSql = @"
                SELECT COUNT(DISTINCT q.QuestionId)
                FROM Question q
                INNER JOIN QuestionCategorization qc ON q.QuestionId = qc.QuestionId
                WHERE q.IsDeleted = 0 AND qc.IsDeleted = 0
                GROUP BY q.QuestionId
                HAVING COUNT(qc.CategoryId) > 1";

            var totalCount = await connection.QuerySingleAsync<int>(countSql);

            // Get paginated results
            const string sql = @"
                SELECT 
                    qc.QuestionId,
                    qc.CategoryId,
                    qc.IsPrimary,
                    qc.Weight,
                    qc.ConfidenceScore,
                    qc.CreatedAt,
                    qc.CreatedByUserId,
                    qc.ModifiedAt,
                    qc.ModifiedByUserId,
                    qbc.Name as CategoryName,
                    qbc.Description as CategoryDescription,
                    qbc.IsActive as CategoryIsActive,
                    qbc.ParentCategoryId as CategoryParentId
                FROM Question q
                INNER JOIN QuestionCategorization qc ON q.QuestionId = qc.QuestionId
                INNER JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
                WHERE q.IsDeleted = 0 AND qc.IsDeleted = 0 AND qbc.IsDeleted = 0
                AND q.QuestionId IN (
                    SELECT q2.QuestionId
                    FROM Question q2
                    INNER JOIN QuestionCategorization qc2 ON q2.QuestionId = qc2.QuestionId
                    WHERE q2.IsDeleted = 0 AND qc2.IsDeleted = 0
                    GROUP BY q2.QuestionId
                    HAVING COUNT(qc2.CategoryId) > 1
                )
                ORDER BY qc.QuestionId, qc.IsPrimary DESC, qc.CreatedAt
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var results = await connection.QueryAsync<QuestionCategoryDetailCore>(
                sql, 
                new { Offset = offset, PageSize = pageSize });

            _logger.LogInformation("Retrieved {Count} multi-categorized question entries (page {Page}, size {PageSize})", 
                results.Count(), pageNumber, pageSize);

            return new PagedResult<QuestionCategoryDetailCore>
            {
                Items = results,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving multi-categorized questions");
            throw;
        }
    }

    public async Task<PagedResult<QuestionCategoryDetailCore>> GetQuestionsWithoutPrimaryCategoryAsync(int pageNumber = 1, int pageSize = 50)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var offset = (pageNumber - 1) * pageSize;

            // Get total count
            const string countSql = @"
                SELECT COUNT(DISTINCT q.QuestionId)
                FROM Question q
                INNER JOIN QuestionCategorization qc ON q.QuestionId = qc.QuestionId
                WHERE q.IsDeleted = 0 AND qc.IsDeleted = 0
                GROUP BY q.QuestionId
                HAVING SUM(CAST(qc.IsPrimary AS INT)) = 0";

            var totalCount = await connection.QuerySingleAsync<int>(countSql);

            // Get paginated results
            const string sql = @"
                SELECT 
                    qc.QuestionId,
                    qc.CategoryId,
                    qc.IsPrimary,
                    qc.Weight,
                    qc.ConfidenceScore,
                    qc.CreatedAt,
                    qc.CreatedByUserId,
                    qc.ModifiedAt,
                    qc.ModifiedByUserId,
                    qbc.Name as CategoryName,
                    qbc.Description as CategoryDescription,
                    qbc.IsActive as CategoryIsActive,
                    qbc.ParentCategoryId as CategoryParentId
                FROM Question q
                INNER JOIN QuestionCategorization qc ON q.QuestionId = qc.QuestionId
                INNER JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
                WHERE q.IsDeleted = 0 AND qc.IsDeleted = 0 AND qbc.IsDeleted = 0
                AND q.QuestionId IN (
                    SELECT q2.QuestionId
                    FROM Question q2
                    INNER JOIN QuestionCategorization qc2 ON q2.QuestionId = qc2.QuestionId
                    WHERE q2.IsDeleted = 0 AND qc2.IsDeleted = 0
                    GROUP BY q2.QuestionId
                    HAVING SUM(CAST(qc2.IsPrimary AS INT)) = 0
                )
                ORDER BY qc.QuestionId, qc.CreatedAt
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var results = await connection.QueryAsync<QuestionCategoryDetailCore>(
                sql, 
                new { Offset = offset, PageSize = pageSize });

            _logger.LogInformation("Retrieved {Count} questions without primary category (page {Page}, size {PageSize})", 
                results.Count(), pageNumber, pageSize);

            return new PagedResult<QuestionCategoryDetailCore>
            {
                Items = results,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving questions without primary category");
            throw;
        }
    }

    public async Task<bool> IsQuestionInCategoryAsync(int questionId, int categoryId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            const string sql = @"
                SELECT COUNT(1) FROM QuestionCategorization 
                WHERE QuestionId = @QuestionId AND CategoryId = @CategoryId AND IsDeleted = 0";

            var count = await connection.QuerySingleAsync<int>(sql, new { QuestionId = questionId, CategoryId = categoryId });

            var result = count > 0;
            _logger.LogInformation("Question {QuestionId} {IsIn} category {CategoryId}", 
                questionId, result ? "is in" : "is not in", categoryId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if question {QuestionId} is in category {CategoryId}", questionId, categoryId);
            throw;
        }
    }

    public async Task<bool> HasCategoryAssignmentsAsync(int questionId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            const string sql = @"
                SELECT COUNT(1) FROM QuestionCategorization 
                WHERE QuestionId = @QuestionId AND IsDeleted = 0";

            var count = await connection.QuerySingleAsync<int>(sql, new { QuestionId = questionId });

            var result = count > 0;
            _logger.LogInformation("Question {QuestionId} {HasAssignments} category assignments", 
                questionId, result ? "has" : "does not have");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if question {QuestionId} has category assignments", questionId);
            throw;
        }
    }

    public async Task<CategorizationValidationResultCore> ValidateCategorizationIntegrityAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var validationResults = new List<string>();
            var errorCount = 0;

            // Check for questions with multiple primary categories
            const string multiplePrimarySql = @"
                SELECT QuestionId, COUNT(1) as PrimaryCount
                FROM QuestionCategorization 
                WHERE IsPrimary = 1 AND IsDeleted = 0
                GROUP BY QuestionId
                HAVING COUNT(1) > 1";

            var multiplePrimaryQuestions = await connection.QueryAsync(multiplePrimarySql);
            if (multiplePrimaryQuestions.Any())
            {
                var questionIds = string.Join(", ", multiplePrimaryQuestions.Select(q => q.QuestionId));
                validationResults.Add($"Questions with multiple primary categories: {questionIds}");
                errorCount += multiplePrimaryQuestions.Count();
            }

            // Check for orphaned categorizations (category doesn't exist)
            const string orphanedCategorizationsSql = @"
                SELECT qc.QuestionId, qc.CategoryId
                FROM QuestionCategorization qc
                LEFT JOIN QuestionBankCategory qbc ON qc.CategoryId = qbc.CategoryId
                WHERE qbc.CategoryId IS NULL AND qc.IsDeleted = 0";

            var orphanedCategorizations = await connection.QueryAsync(orphanedCategorizationsSql);
            if (orphanedCategorizations.Any())
            {
                var orphaned = string.Join(", ", orphanedCategorizations.Select(o => $"Q{o.QuestionId}:C{o.CategoryId}"));
                validationResults.Add($"Orphaned categorizations (category doesn't exist): {orphaned}");
                errorCount += orphanedCategorizations.Count();
            }

            // Check for questions without any categorization
            const string uncategorizedQuestionsSql = @"
                SELECT q.QuestionId
                FROM Question q
                WHERE q.IsDeleted = 0 
                AND NOT EXISTS (
                    SELECT 1 FROM QuestionCategorization qc 
                    WHERE qc.QuestionId = q.QuestionId AND qc.IsDeleted = 0
                )";

            var uncategorizedQuestions = await connection.QueryAsync(uncategorizedQuestionsSql);
            if (uncategorizedQuestions.Any())
            {
                var uncategorized = string.Join(", ", uncategorizedQuestions.Select(q => q.QuestionId));
                validationResults.Add($"Uncategorized questions: {uncategorized}");
                errorCount += uncategorizedQuestions.Count();
            }

            var isValid = errorCount == 0;
            var result = new CategorizationValidationResultCore
            {
                IsValid = isValid,
                Issues = validationResults.Select(msg => new CategorizationValidationIssueCore
                {
                    Type = "Validation",
                    Description = msg,
                    Severity = "Warning"
                }).ToList(),
                OrphanedCategorizations = orphanedCategorizations.Count(),
                DuplicateCategorizations = multiplePrimaryQuestions.Count(),
                InvalidHierarchyEntries = 0
            };

            _logger.LogInformation("Categorization validation completed. Valid: {IsValid}, Errors: {ErrorCount}", isValid, errorCount);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating categorization integrity");
            throw;
        }
    }

    public async Task<CategorizationStatisticsCore> GetCategorizationStatisticsAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            // Get total questions
            const string totalQuestionsSql = @"
                SELECT COUNT(1) FROM Question WHERE IsDeleted = 0";

            var totalQuestions = await connection.QuerySingleAsync<int>(totalQuestionsSql);

            // Get categorized questions
            const string categorizedQuestionsSql = @"
                SELECT COUNT(DISTINCT QuestionId) FROM QuestionCategorization WHERE IsDeleted = 0";

            var categorizedQuestions = await connection.QuerySingleAsync<int>(categorizedQuestionsSql);

            // Get questions with primary categories
            const string primaryCategorizedSql = @"
                SELECT COUNT(DISTINCT QuestionId) FROM QuestionCategorization 
                WHERE IsPrimary = 1 AND IsDeleted = 0";

            var primaryCategorizedQuestions = await connection.QuerySingleAsync<int>(primaryCategorizedSql);

            // Get questions with multiple categories
            const string multiCategorizedSql = @"
                SELECT COUNT(DISTINCT QuestionId)
                FROM QuestionCategorization 
                WHERE IsDeleted = 0
                GROUP BY QuestionId
                HAVING COUNT(CategoryId) > 1";

            var multiCategorizedQuestions = await connection.QuerySingleAsync<int>(multiCategorizedSql);

            // Get category distribution
            const string categoryDistributionSql = @"
                SELECT 
                    qbc.Name as CategoryName,
                    COUNT(DISTINCT qc.QuestionId) as QuestionCount
                FROM QuestionBankCategory qbc
                LEFT JOIN QuestionCategorization qc ON qbc.CategoryId = qc.CategoryId AND qc.IsDeleted = 0
                WHERE qbc.IsDeleted = 0
                GROUP BY qbc.CategoryId, qbc.Name
                ORDER BY QuestionCount DESC";

            var categoryDistribution = await connection.QueryAsync(categoryDistributionSql);

            var result = new CategorizationStatisticsCore
            {
                TotalQuestions = totalQuestions,
                CategorizedQuestions = categorizedQuestions,
                UncategorizedQuestions = totalQuestions - categorizedQuestions,
                QuestionsWithPrimaryCategory = primaryCategorizedQuestions,
                QuestionsWithMultipleCategories = multiCategorizedQuestions,
                AverageCategoriesPerQuestion = totalQuestions > 0 ? (double)categorizedQuestions / totalQuestions : 0,
                TotalCategorizations = categorizedQuestions,
                AutomaticCategorizations = 0, // TODO: Implement when auto-categorization is available
                ManualCategorizations = categorizedQuestions,
                LastUpdated = DateTime.UtcNow
            };

            _logger.LogInformation("Generated categorization statistics. Total: {Total}, Categorized: {Categorized}, Rate: {Rate:F1}%", 
                totalQuestions, categorizedQuestions, result.AverageCategoriesPerQuestion * 100);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating categorization statistics");
            throw;
        }
    }

    public async Task<IEnumerable<CategoryUsageAnalyticsCore>> GetCategoryUsageAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            var whereConditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (startDate.HasValue)
            {
                whereConditions.Add("qc.CreatedAt >= @StartDate");
                parameters["StartDate"] = startDate.Value;
            }

            if (endDate.HasValue)
            {
                whereConditions.Add("qc.CreatedAt <= @EndDate");
                parameters["EndDate"] = endDate.Value;
            }

            var whereClause = whereConditions.Any() ? $"WHERE {string.Join(" AND ", whereConditions)}" : "";

            const string sql = @"
                SELECT 
                    qbc.CategoryId,
                    qbc.Name as CategoryName,
                    COUNT(DISTINCT qc.QuestionId) as TotalQuestions,
                    COUNT(CASE WHEN qc.IsPrimary = 1 THEN 1 END) as PrimaryQuestions,
                    COUNT(CASE WHEN qc.IsPrimary = 0 THEN 1 END) as SecondaryQuestions,
                    AVG(CAST(qc.Weight AS FLOAT)) as AverageWeight,
                    MIN(qc.CreatedAt) as FirstAssignment,
                    MAX(qc.CreatedAt) as LastAssignment,
                    COUNT(DISTINCT qc.CreatedByUserId) as UniqueAssigners
                FROM QuestionBankCategory qbc
                LEFT JOIN QuestionCategorization qc ON qbc.CategoryId = qc.CategoryId AND qc.IsDeleted = 0
                {whereClause} AND qbc.IsDeleted = 0
                GROUP BY qbc.CategoryId, qbc.Name
                ORDER BY TotalQuestions DESC";

            var results = await connection.QueryAsync(sql, parameters);

            var analytics = results.Select(row => new CategoryUsageAnalyticsCore
            {
                CategoryId = (int)row.CategoryId,
                CategoryName = (string)row.CategoryName,
                CategoryPath = "", // TODO: Implement category path calculation
                QuestionCount = (int)row.TotalQuestions,
                NewQuestionsCount = 0, // TODO: Implement when tracking is available
                RemovedQuestionsCount = 0, // TODO: Implement when tracking is available
                AnalysisPeriodStart = startDate ?? DateTime.UtcNow.AddDays(-30),
                AnalysisPeriodEnd = endDate ?? DateTime.UtcNow,
                UsageGrowthRate = 0.0 // TODO: Implement when historical data is available
            });

            _logger.LogInformation("Generated category usage analytics for {CategoryCount} categories", analytics.Count());
            return analytics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating category usage analytics");
            throw;
        }
    }

    public async Task<AutoCategorizationMetricsCore> GetAutoCategorizationMetricsAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            // Get auto-categorization attempts
            const string autoCategorizationSql = @"
                SELECT 
                    COUNT(1) as TotalAttempts,
                    COUNT(CASE WHEN ConfidenceScore >= 0.8 THEN 1 END) as HighConfidence,
                    COUNT(CASE WHEN ConfidenceScore >= 0.6 AND ConfidenceScore < 0.8 THEN 1 END) as MediumConfidence,
                    COUNT(CASE WHEN ConfidenceScore < 0.6 THEN 1 END) as LowConfidence,
                    AVG(CAST(ConfidenceScore AS FLOAT)) as AverageConfidence
                FROM QuestionCategorization 
                WHERE ConfidenceScore IS NOT NULL AND IsDeleted = 0";

            var autoCategorizationStats = await connection.QuerySingleAsync(autoCategorizationSql);

            // Get recent auto-categorization performance
            const string recentPerformanceSql = @"
                SELECT 
                    CAST(qc.CreatedAt AS DATE) as Date,
                    COUNT(1) as Attempts,
                    AVG(CAST(qc.ConfidenceScore AS FLOAT)) as AverageConfidence,
                    COUNT(CASE WHEN qc.ConfidenceScore >= 0.8 THEN 1 END) as HighConfidenceCount
                FROM QuestionCategorization qc
                WHERE qc.ConfidenceScore IS NOT NULL 
                AND qc.CreatedAt >= @StartDate
                AND qc.IsDeleted = 0
                GROUP BY CAST(qc.CreatedAt AS DATE)
                ORDER BY Date DESC";

            var startDate = DateTime.UtcNow.AddDays(-30);
            var recentPerformance = await connection.QueryAsync(recentPerformanceSql, new { StartDate = startDate });

            var result = new AutoCategorizationMetricsCore
            {
                TotalAutoCategorizationAttempts = (int)autoCategorizationStats.TotalAttempts,
                SuccessfulAutoCategorizationAttempts = (int)autoCategorizationStats.HighConfidence,
                SuccessRate = autoCategorizationStats.TotalAttempts > 0 ? 
                    (double)autoCategorizationStats.HighConfidence / (int)autoCategorizationStats.TotalAttempts * 100 : 0,
                AverageConfidenceScore = autoCategorizationStats.AverageConfidence != null ? (double)autoCategorizationStats.AverageConfidence : 0,
                AverageWeight = 0.0, // TODO: Implement when weight tracking is available
                HighConfidenceCategorizations = (int)autoCategorizationStats.HighConfidence,
                MediumConfidenceCategorizations = (int)autoCategorizationStats.MediumConfidence,
                LowConfidenceCategorizations = (int)autoCategorizationStats.LowConfidence,
                LastCalculated = DateTime.UtcNow
            };

            _logger.LogInformation("Generated auto-categorization metrics. Total attempts: {Total}, Success rate: {SuccessRate:F1}%", 
                result.TotalAutoCategorizationAttempts, result.SuccessRate);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating auto-categorization metrics");
            throw;
        }
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
                        VALUES (@QuestionId, @CategoryId, @IsPrimary, @CreatedAt, @CreatedByUserId);";

                    await connection.ExecuteAsync(
                        insertSql,
                        new {
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

            // Add to new category
            foreach (var questionId in questionIds)
            {
                const string checkExistsSql = @"
                    SELECT COUNT(1) 
                    FROM QuestionCategorization 
                    WHERE QuestionId = @QuestionId AND CategoryId = @CategoryId AND IsDeleted = 0";

                var exists = await connection.QuerySingleAsync<int>(
                    checkExistsSql,
                    new { QuestionId = questionId, CategoryId = toCategoryId },
                    transaction);

                if (exists == 0)
                {
                    const string insertSql = @"
                        INSERT INTO QuestionCategorization (QuestionId, CategoryId, IsPrimary, CreatedAt)
                        VALUES (@QuestionId, @CategoryId, @IsPrimary, @CreatedAt)";

                    await connection.ExecuteAsync(
                        insertSql,
                        new {
                            QuestionId = questionId,
                            CategoryId = toCategoryId,
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
