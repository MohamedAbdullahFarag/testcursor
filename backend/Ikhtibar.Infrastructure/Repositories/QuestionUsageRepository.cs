using Dapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories
{
    public class QuestionUsageRepository : BaseRepository<QuestionUsageHistory>//, IQuestionUsageRepository
    {
        private new readonly ILogger<QuestionUsageRepository> _logger;

        public QuestionUsageRepository(IDbConnectionFactory connectionFactory, ILogger<QuestionUsageRepository> logger)
            : base(connectionFactory, logger, "QuestionUsageHistory", "Id")
        {
            _logger = logger;
        }

        public async Task<QuestionUsageHistory?> GetByIdWithDetailsAsync(int usageId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT quh.*, q.Text as QuestionText, u.Name as UserName
                    FROM QuestionUsageHistory quh
                    LEFT JOIN Questions q ON quh.QuestionId = q.Id
                    LEFT JOIN Users u ON quh.UserId = u.Id
                    WHERE quh.Id = @UsageId AND quh.IsActive = 1";

                var usage = await connection.QueryFirstOrDefaultAsync<QuestionUsageHistory>(sql, new { UsageId = usageId });
                return usage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question usage with details: {UsageId}", usageId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionUsageHistory>> GetByQuestionIdAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT quh.*, u.Name as UserName
                    FROM QuestionUsageHistory quh
                    LEFT JOIN Users u ON quh.UserId = u.Id
                    WHERE quh.QuestionId = @QuestionId AND quh.IsActive = 1
                    ORDER BY quh.UsedAt DESC";

                var usage = await connection.QueryAsync<QuestionUsageHistory>(sql, new { QuestionId = questionId });
                return usage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving usage for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionUsageHistory>> GetByUserIdAsync(int userId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT quh.*, q.Text as QuestionText
                    FROM QuestionUsageHistory quh
                    LEFT JOIN Questions q ON quh.QuestionId = q.Id
                    WHERE quh.UserId = @UserId AND quh.IsActive = 1
                    ORDER BY quh.UsedAt DESC";

                var usage = await connection.QueryAsync<QuestionUsageHistory>(sql, new { UserId = userId });
                return usage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving usage by user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionUsageHistory>> GetByExamIdAsync(int examId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT quh.*, q.Text as QuestionText, u.Name as UserName
                    FROM QuestionUsageHistory quh
                    LEFT JOIN Questions q ON quh.QuestionId = q.Id
                    LEFT JOIN Users u ON quh.UserId = u.Id
                    WHERE quh.ExamId = @ExamId AND quh.IsActive = 1
                    ORDER BY quh.UsedAt DESC";

                var usage = await connection.QueryAsync<QuestionUsageHistory>(sql, new { ExamId = examId });
                return usage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving usage by exam: {ExamId}", examId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionUsageHistory>> GetByUsageTypeAsync(string usageType)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT quh.*, q.Text as QuestionText, u.Name as UserName
                    FROM QuestionUsageHistory quh
                    LEFT JOIN Questions q ON quh.QuestionId = q.Id
                    LEFT JOIN Users u ON quh.UserId = u.Id
                    WHERE quh.UsageType = @UsageType AND quh.IsActive = 1
                    ORDER BY quh.UsedAt DESC";

                var usage = await connection.QueryAsync<QuestionUsageHistory>(sql, new { UsageType = usageType });
                return usage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving usage by type: {UsageType}", usageType);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionUsageHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT quh.*, q.Text as QuestionText, u.Name as UserName
                    FROM QuestionUsageHistory quh
                    LEFT JOIN Questions q ON quh.QuestionId = q.Id
                    LEFT JOIN Users u ON quh.UserId = u.Id
                    WHERE quh.UsedAt >= @StartDate AND quh.UsedAt <= @EndDate AND quh.IsActive = 1
                    ORDER BY quh.UsedAt DESC";

                var usage = await connection.QueryAsync<QuestionUsageHistory>(sql, 
                    new { StartDate = startDate, EndDate = endDate });
                return usage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving usage by date range: {StartDate} to {EndDate}", startDate, endDate);
                throw;
            }
        }

        public async Task<int> GetUsageCountAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM QuestionUsageHistory WHERE QuestionId = @QuestionId AND IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { QuestionId = questionId });
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting usage count for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<double> GetAverageScoreAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT AVG(CAST(Score AS FLOAT)) 
                    FROM QuestionUsageHistory 
                    WHERE QuestionId = @QuestionId AND IsActive = 1 AND Score IS NOT NULL";
                
                var score = await connection.ExecuteScalarAsync<double?>(sql, new { QuestionId = questionId });
                return score ?? 0.0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting average score for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<double> GetAverageTimeSpentAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT AVG(CAST(TimeSpent AS FLOAT)) 
                    FROM QuestionUsageHistory 
                    WHERE QuestionId = @QuestionId AND IsActive = 1 AND TimeSpent IS NOT NULL";
                
                var time = await connection.ExecuteScalarAsync<double?>(sql, new { QuestionId = questionId });
                return time ?? 0.0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting average time spent for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionUsageHistory>> GetPagedAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var offset = (page - 1) * pageSize;
                var sql = @"
                    SELECT quh.*, q.Text as QuestionText, u.Name as UserName
                    FROM QuestionUsageHistory quh
                    LEFT JOIN Questions q ON quh.QuestionId = q.Id
                    LEFT JOIN Users u ON quh.UserId = u.Id
                    WHERE quh.IsActive = 1
                    ORDER BY quh.UsedAt DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var usage = await connection.QueryAsync<QuestionUsageHistory>(sql, 
                    new { Offset = offset, PageSize = pageSize });
                return usage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged usage history");
                throw;
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM QuestionUsageHistory WHERE IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total usage count");
                throw;
            }
        }

        public async Task<IEnumerable<QuestionUsageHistory>> GetTopPerformersAsync(int questionId, int limit = 10)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT TOP (@Limit) quh.*, u.Name as UserName
                    FROM QuestionUsageHistory quh
                    LEFT JOIN Users u ON quh.UserId = u.Id
                    WHERE quh.QuestionId = @QuestionId AND quh.IsActive = 1 AND quh.Score IS NOT NULL
                    ORDER BY quh.Score DESC, quh.TimeSpent ASC";

                var usage = await connection.QueryAsync<QuestionUsageHistory>(sql, 
                    new { QuestionId = questionId, Limit = limit });
                return usage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top performers for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionUsageHistory>> GetRecentUsageAsync(int questionId, int limit = 20)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT TOP (@Limit) quh.*, u.Name as UserName
                    FROM QuestionUsageHistory quh
                    LEFT JOIN Users u ON quh.UserId = u.Id
                    WHERE quh.QuestionId = @QuestionId AND quh.IsActive = 1
                    ORDER BY quh.UsedAt DESC";

                var usage = await connection.QueryAsync<QuestionUsageHistory>(sql, 
                    new { QuestionId = questionId, Limit = limit });
                return usage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent usage for question: {QuestionId}", questionId);
                throw;
            }
        }
    }
}
