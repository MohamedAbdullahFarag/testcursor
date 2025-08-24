using Dapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Ikhtibar.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories
{
    public class QuestionBankRepository : BaseRepository<QuestionBank>//, IQuestionBankRepository
    {
        private new readonly ILogger<QuestionBankRepository> _logger;

        public QuestionBankRepository(IDbConnectionFactory connectionFactory, ILogger<QuestionBankRepository> logger)
            : base(connectionFactory, logger, "QuestionBanks", "Id")
        {
            _logger = logger;
        }

        public async Task<QuestionBank?> GetByIdWithDetailsAsync(int questionBankId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qb.*, u.Name as CreatedByUserName
                    FROM QuestionBanks qb
                    LEFT JOIN Users u ON qb.CreatedBy = u.Id
                    WHERE qb.Id = @QuestionBankId AND qb.IsActive = 1";

                var questionBank = await connection.QueryFirstOrDefaultAsync<QuestionBank>(sql, new { QuestionBankId = questionBankId });
                return questionBank;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question bank with details: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<QuestionBank?> GetByIdWithQuestionsAsync(int questionBankId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qb.*, q.*
                    FROM QuestionBanks qb
                    LEFT JOIN Questions q ON qb.Id = q.QuestionBankId
                    WHERE qb.Id = @QuestionBankId AND qb.IsActive = 1 AND (q.IsActive = 1 OR q.IsActive IS NULL)";

                var questionBankDictionary = new Dictionary<int, QuestionBank>();
                
                await connection.QueryAsync<QuestionBank, Question, QuestionBank>(
                    sql,
                    (questionBank, question) =>
                    {
                        if (!questionBankDictionary.TryGetValue(questionBank.Id, out var questionBankEntry))
                        {
                            questionBankEntry = questionBank;
                            questionBankEntry.Questions = new List<Question>();
                            questionBankDictionary.Add(questionBank.Id, questionBankEntry);
                        }

                        if (question != null)
                        {
                            questionBankEntry.Questions.Add(question);
                        }

                        return questionBankEntry;
                    },
                    new { QuestionBankId = questionBankId },
                    splitOn: "Id"
                );

                return questionBankDictionary.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question bank with questions: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<QuestionBank?> GetByIdWithAccessAsync(int questionBankId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qb.*, qba.*, u.Name as UserName
                    FROM QuestionBanks qb
                    LEFT JOIN QuestionBankAccess qba ON qb.Id = qba.QuestionBankId
                    LEFT JOIN Users u ON qba.UserId = u.Id
                    WHERE qb.Id = @QuestionBankId AND qb.IsActive = 1";

                var questionBankDictionary = new Dictionary<int, QuestionBank>();
                
                await connection.QueryAsync<QuestionBank, QuestionBankAccess, QuestionBank>(
                    sql,
                    (questionBank, access) =>
                    {
                        if (!questionBankDictionary.TryGetValue(questionBank.Id, out var questionBankEntry))
                        {
                            questionBankEntry = questionBank;
                            questionBankEntry.AccessPermissions = new List<QuestionBankAccess>();
                            questionBankDictionary.Add(questionBank.Id, questionBankEntry);
                        }

                        if (access != null)
                        {
                            questionBankEntry.AccessPermissions.Add(access);
                        }

                        return questionBankEntry;
                    },
                    new { QuestionBankId = questionBankId },
                    splitOn: "Id"
                );

                return questionBankDictionary.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question bank with access: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<QuestionBank?> GetByIdWithAllDetailsAsync(int questionBankId)
        {
            try
            {
                var questionBank = await GetByIdWithDetailsAsync(questionBankId);
                if (questionBank == null) return null;

                // Load questions
                questionBank.Questions = (await GetQuestionsInBankAsync(questionBankId, new QuestionFilterDto())).ToList();
                
                // Load access permissions
                questionBank.AccessPermissions = (await GetBankAccessAsync(questionBankId)).ToList();

                return questionBank;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question bank with all details: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<(IEnumerable<QuestionBank> QuestionBanks, int TotalCount)> GetPagedAsync(int page, int pageSize, string? whereClause = null, object? parameters = null)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var offset = (page - 1) * pageSize;
                
                var countSql = "SELECT COUNT(*) FROM QuestionBanks WHERE IsActive = 1";
                if (!string.IsNullOrEmpty(whereClause))
                {
                    countSql += " AND " + whereClause;
                }
                
                var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
                
                var sql = @"
                    SELECT qb.*, u.Name as CreatedByUserName
                    FROM QuestionBanks qb
                    LEFT JOIN Users u ON qb.CreatedBy = u.Id
                    WHERE qb.IsActive = 1";
                
                if (!string.IsNullOrEmpty(whereClause))
                {
                    sql += " AND " + whereClause;
                }
                
                sql += " ORDER BY qb.CreatedAt DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                
                var queryParams = new DynamicParameters(parameters);
                queryParams.Add("@Offset", offset);
                queryParams.Add("@PageSize", pageSize);
                
                var questionBanks = await connection.QueryAsync<QuestionBank>(sql, queryParams);
                
                return (questionBanks, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged question banks");
                throw;
            }
        }

        public async Task<IEnumerable<QuestionBank>> GetByFilterAsync(QuestionBankFilterDto filter)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qb.*, u.Name as CreatedByUserName
                    FROM QuestionBanks qb
                    LEFT JOIN Users u ON qb.CreatedBy = u.Id
                    WHERE qb.IsActive = 1";
                
                var parameters = new DynamicParameters();
                
                if (filter.CreatedBy.HasValue)
                {
                    sql += " AND qb.CreatedBy = @CreatedBy";
                    parameters.Add("@CreatedBy", filter.CreatedBy.Value);
                }
                
                if (filter.IsPublic.HasValue)
                {
                    sql += " AND qb.IsPublic = @IsPublic";
                    parameters.Add("@IsPublic", filter.IsPublic.Value);
                }
                
                if (!string.IsNullOrEmpty(filter.Subject))
                {
                    sql += " AND qb.Subject = @Subject";
                    parameters.Add("@Subject", filter.Subject);
                }
                
                if (!string.IsNullOrEmpty(filter.GradeLevel))
                {
                    sql += " AND qb.GradeLevel = @GradeLevel";
                    parameters.Add("@GradeLevel", filter.GradeLevel);
                }
                
                if (!string.IsNullOrEmpty(filter.Language))
                {
                    sql += " AND qb.Language = @Language";
                    parameters.Add("@Language", filter.Language);
                }
                
                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    sql += " AND (qb.Name LIKE @SearchText OR qb.Description LIKE @SearchText)";
                    parameters.Add("@SearchText", $"%{filter.SearchText}%");
                }
                
                sql += " ORDER BY qb.CreatedAt DESC";
                
                var questionBanks = await connection.QueryAsync<QuestionBank>(sql, parameters);
                return questionBanks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question banks by filter");
                throw;
            }
        }

        public async Task<IEnumerable<QuestionBank>> GetByCreatorAsync(int creatorId)
        {
            try
            {
                var filter = new QuestionBankFilterDto { CreatedBy = creatorId };
                return await GetByFilterAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question banks by creator: {CreatorId}", creatorId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionBank>> GetPublicBanksAsync()
        {
            try
            {
                var filter = new QuestionBankFilterDto { IsPublic = true };
                return await GetByFilterAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving public question banks");
                throw;
            }
        }

        public async Task<IEnumerable<QuestionBank>> GetBySubjectAsync(string subject)
        {
            try
            {
                var filter = new QuestionBankFilterDto { Subject = subject };
                return await GetByFilterAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question banks by subject: {Subject}", subject);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionBank>> GetByGradeLevelAsync(string gradeLevel)
        {
            try
            {
                var filter = new QuestionBankFilterDto { GradeLevel = gradeLevel };
                return await GetByFilterAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question banks by grade level: {GradeLevel}", gradeLevel);
                throw;
            }
        }

        public async Task<bool> AddQuestionToBankAsync(int questionBankId, int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "UPDATE Questions SET QuestionBankId = @QuestionBankId WHERE Id = @QuestionId";
                var rowsAffected = await connection.ExecuteAsync(sql, new { QuestionBankId = questionBankId, QuestionId = questionId });
                
                if (rowsAffected > 0)
                {
                    await UpdateBankStatisticsAsync(questionBankId);
                }
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding question to bank: {QuestionBankId}, {QuestionId}", questionBankId, questionId);
                throw;
            }
        }

        public async Task<bool> RemoveQuestionFromBankAsync(int questionBankId, int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "UPDATE Questions SET QuestionBankId = NULL WHERE Id = @QuestionId AND QuestionBankId = @QuestionBankId";
                var rowsAffected = await connection.ExecuteAsync(sql, new { QuestionBankId = questionBankId, QuestionId = questionId });
                
                if (rowsAffected > 0)
                {
                    await UpdateBankStatisticsAsync(questionBankId);
                }
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing question from bank: {QuestionBankId}, {QuestionId}", questionBankId, questionId);
                throw;
            }
        }

        public async Task<bool> MoveQuestionBetweenBanksAsync(int questionId, int sourceBankId, int targetBankId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "UPDATE Questions SET QuestionBankId = @TargetBankId WHERE Id = @QuestionId AND QuestionBankId = @SourceBankId";
                var rowsAffected = await connection.ExecuteAsync(sql, new { TargetBankId = targetBankId, QuestionId = questionId, SourceBankId = sourceBankId });
                
                if (rowsAffected > 0)
                {
                    await UpdateBankStatisticsAsync(sourceBankId);
                    await UpdateBankStatisticsAsync(targetBankId);
                }
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving question between banks: {QuestionId}, {SourceBankId} -> {TargetBankId}", questionId, sourceBankId, targetBankId);
                throw;
            }
        }

        public async Task<IEnumerable<Question>> GetQuestionsInBankAsync(int questionBankId, QuestionFilterDto filter)
        {
            try
            {
                var bankFilter = filter.Clone();
                bankFilter.QuestionBankId = questionBankId;
                
                // This would typically call the question repository
                // For now, return empty list as placeholder
                return new List<Question>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions in bank: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<int> GetQuestionCountInBankAsync(int questionBankId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM Questions WHERE QuestionBankId = @QuestionBankId AND IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { QuestionBankId = questionBankId });
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting question count in bank: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionBankAccess>> GetBankAccessAsync(int questionBankId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qba.*, u.Name as UserName
                    FROM QuestionBankAccess qba
                    LEFT JOIN Users u ON qba.UserId = u.Id
                    WHERE qba.QuestionBankId = @QuestionBankId
                    ORDER BY qba.GrantedAt";
                
                var access = await connection.QueryAsync<QuestionBankAccess>(sql, new { QuestionBankId = questionBankId });
                return access;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bank access: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<bool> GrantAccessAsync(int questionBankId, int userId, string permission)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                // Check if access already exists
                var existingSql = "SELECT COUNT(*) FROM QuestionBankAccess WHERE QuestionBankId = @QuestionBankId AND UserId = @UserId";
                var existingCount = await connection.ExecuteScalarAsync<int>(existingSql, new { QuestionBankId = questionBankId, UserId = userId });
                
                if (existingCount > 0)
                {
                    // Update existing access
                    var updateSql = "UPDATE QuestionBankAccess SET Permission = @Permission WHERE QuestionBankId = @QuestionBankId AND UserId = @UserId";
                    var rowsAffected = await connection.ExecuteAsync(updateSql, new { Permission = permission, QuestionBankId = questionBankId, UserId = userId });
                    return rowsAffected > 0;
                }
                else
                {
                    // Create new access
                    var insertSql = @"
                        INSERT INTO QuestionBankAccess (QuestionBankId, UserId, Permission, GrantedAt)
                        VALUES (@QuestionBankId, @UserId, @Permission, @GrantedAt)";
                    
                    var parameters = new
                    {
                        QuestionBankId = questionBankId,
                        UserId = userId,
                        Permission = permission,
                        GrantedAt = DateTime.UtcNow
                    };
                    
                    var rowsAffected = await connection.ExecuteAsync(insertSql, parameters);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error granting access to bank: {QuestionBankId}, {UserId}", questionBankId, userId);
                throw;
            }
        }

        public async Task<bool> RevokeAccessAsync(int questionBankId, int userId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "DELETE FROM QuestionBankAccess WHERE QuestionBankId = @QuestionBankId AND UserId = @UserId";
                var rowsAffected = await connection.ExecuteAsync(sql, new { QuestionBankId = questionBankId, UserId = userId });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking access from bank: {QuestionBankId}, {UserId}", questionBankId, userId);
                throw;
            }
        }

        public async Task<bool> HasAccessAsync(int questionBankId, int userId, string permission)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM QuestionBankAccess WHERE QuestionBankId = @QuestionBankId AND UserId = @UserId AND Permission = @Permission";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { QuestionBankId = questionBankId, UserId = userId, Permission = permission });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking bank access: {QuestionBankId}, {UserId}", questionBankId, userId);
                throw;
            }
        }

        public async Task<QuestionBankAccess?> GetUserAccessAsync(int questionBankId, int userId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT * FROM QuestionBankAccess WHERE QuestionBankId = @QuestionBankId AND UserId = @UserId";
                var access = await connection.QueryFirstOrDefaultAsync<QuestionBankAccess>(sql, new { QuestionBankId = questionBankId, UserId = userId });
                return access;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user access to bank: {QuestionBankId}, {UserId}", questionBankId, userId);
                throw;
            }
        }

        public async Task<QuestionBankStatisticsDto> GetBankStatisticsAsync(int questionBankId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT 
                        COUNT(*) as TotalQuestions,
                        SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) as ActiveQuestions,
                        SUM(CASE WHEN QuestionStatusId = 4 THEN 1 ELSE 0 END) as PublishedQuestions
                    FROM Questions 
                    WHERE QuestionBankId = @QuestionBankId";
                
                var stats = await connection.QueryFirstOrDefaultAsync<QuestionBankStatisticsDto>(sql, new { QuestionBankId = questionBankId });
                return stats ?? new QuestionBankStatisticsDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bank statistics: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<bool> UpdateBankStatisticsAsync(int questionBankId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var stats = await GetBankStatisticsAsync(questionBankId);
                
                var sql = @"
                    UPDATE QuestionBanks 
                    SET TotalQuestions = @TotalQuestions, 
                        ActiveQuestions = @ActiveQuestions,
                        LastModified = @LastModified
                    WHERE Id = @QuestionBankId";
                
                var parameters = new
                {
                    QuestionBankId = questionBankId,
                    TotalQuestions = stats.TotalQuestions,
                    ActiveQuestions = stats.ActiveQuestions,
                    LastModified = DateTime.UtcNow
                };
                
                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bank statistics: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionBankStatisticsDto>> GetBanksStatisticsAsync(IEnumerable<int> questionBankIds)
        {
            try
            {
                var stats = new List<QuestionBankStatisticsDto>();
                foreach (var bankId in questionBankIds)
                {
                    var bankStats = await GetBankStatisticsAsync(bankId);
                    stats.Add(bankStats);
                }
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting banks statistics");
                throw;
            }
        }

        public async Task<QuestionBankAnalyticsDto> GetBankAnalyticsAsync(int questionBankId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                // This would implement comprehensive analytics
                // For now, return empty analytics as placeholder
                return new QuestionBankAnalyticsDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bank analytics: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<bool> BulkAddQuestionsToBankAsync(int questionBankId, IEnumerable<int> questionIds)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "UPDATE Questions SET QuestionBankId = @QuestionBankId WHERE Id IN @QuestionIds";
                var rowsAffected = await connection.ExecuteAsync(sql, new { QuestionBankId = questionBankId, QuestionIds = questionIds });
                
                if (rowsAffected > 0)
                {
                    await UpdateBankStatisticsAsync(questionBankId);
                }
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk adding questions to bank: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<bool> BulkRemoveQuestionsFromBankAsync(int questionBankId, IEnumerable<int> questionIds)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "UPDATE Questions SET QuestionBankId = NULL WHERE Id IN @QuestionIds AND QuestionBankId = @QuestionBankId";
                var rowsAffected = await connection.ExecuteAsync(sql, new { QuestionIds = questionIds, QuestionBankId = questionBankId });
                
                if (rowsAffected > 0)
                {
                    await UpdateBankStatisticsAsync(questionBankId);
                }
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk removing questions from bank: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<bool> BulkUpdateBankQuestionsAsync(BulkUpdateBankQuestionsDto dto)
        {
            try
            {
                // This would implement bulk update logic
                // For now, return true as placeholder
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk updating bank questions");
                throw;
            }
        }

        // Additional methods would be implemented here...
        // For brevity, I'm implementing the core functionality first
    }
}
