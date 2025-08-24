using Dapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories
{
    public class QuestionValidationRepository : BaseRepository<QuestionValidation>//, IQuestionValidationRepository
    {
        private new readonly ILogger<QuestionValidationRepository> _logger;

        public QuestionValidationRepository(IDbConnectionFactory connectionFactory, ILogger<QuestionValidationRepository> logger)
            : base(connectionFactory, logger, "QuestionValidations", "Id")
        {
            _logger = logger;
        }

        public async Task<QuestionValidation?> GetByIdWithDetailsAsync(int validationId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qv.*, q.Text as QuestionText, u.Name as ValidatedByUserName
                    FROM QuestionValidations qv
                    LEFT JOIN Questions q ON qv.QuestionId = q.Id
                    LEFT JOIN Users u ON qv.ValidatedBy = u.Id
                    WHERE qv.Id = @ValidationId AND qv.IsActive = 1";

                var validation = await connection.QueryFirstOrDefaultAsync<QuestionValidation>(sql, new { ValidationId = validationId });
                return validation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question validation with details: {ValidationId}", validationId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionValidation>> GetByQuestionIdAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qv.*, u.Name as ValidatedByUserName
                    FROM QuestionValidations qv
                    LEFT JOIN Users u ON qv.ValidatedBy = u.Id
                    WHERE qv.QuestionId = @QuestionId AND qv.IsActive = 1
                    ORDER BY qv.ValidatedAt DESC";

                var validations = await connection.QueryAsync<QuestionValidation>(sql, new { QuestionId = questionId });
                return validations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving validations for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<QuestionValidation?> GetLatestValidationAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT TOP 1 qv.*, u.Name as ValidatedByUserName
                    FROM QuestionValidations qv
                    LEFT JOIN Users u ON qv.ValidatedBy = u.Id
                    WHERE qv.QuestionId = @QuestionId AND qv.IsActive = 1
                    ORDER BY qv.ValidatedAt DESC";

                var validation = await connection.QueryFirstOrDefaultAsync<QuestionValidation>(sql, new { QuestionId = questionId });
                return validation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving latest validation for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionValidation>> GetByStatusAsync(Shared.Enums.ValidationStatus status)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qv.*, q.Text as QuestionText, u.Name as ValidatedByUserName
                    FROM QuestionValidations qv
                    LEFT JOIN Questions q ON qv.QuestionId = q.Id
                    LEFT JOIN Users u ON qv.ValidatedBy = u.Id
                    WHERE qv.Status = @Status AND qv.IsActive = 1
                    ORDER BY qv.ValidatedAt DESC";

                var validations = await connection.QueryAsync<QuestionValidation>(sql, new { Status = status });
                return validations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving validations by status: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionValidation>> GetByValidatorAsync(int validatedBy)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qv.*, q.Text as QuestionText
                    FROM QuestionValidations qv
                    LEFT JOIN Questions q ON qv.QuestionId = q.Id
                    WHERE qv.ValidatedBy = @ValidatedBy AND qv.IsActive = 1
                    ORDER BY qv.ValidatedAt DESC";

                var validations = await connection.QueryAsync<QuestionValidation>(sql, new { ValidatedBy = validatedBy });
                return validations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving validations by validator: {ValidatedBy}", validatedBy);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionValidation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qv.*, q.Text as QuestionText, u.Name as ValidatedByUserName
                    FROM QuestionValidations qv
                    LEFT JOIN Questions q ON qv.QuestionId = q.Id
                    LEFT JOIN Users u ON qv.ValidatedBy = u.Id
                    WHERE qv.ValidatedAt >= @StartDate AND qv.ValidatedAt <= @EndDate AND qv.IsActive = 1
                    ORDER BY qv.ValidatedAt DESC";

                var validations = await connection.QueryAsync<QuestionValidation>(sql, 
                    new { StartDate = startDate, EndDate = endDate });
                return validations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving validations by date range: {StartDate} to {EndDate}", startDate, endDate);
                throw;
            }
        }

        public async Task<int> GetValidationCountAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM QuestionValidations WHERE QuestionId = @QuestionId AND IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { QuestionId = questionId });
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting validation count for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<double> GetAverageValidationScoreAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT AVG(CAST(Score AS FLOAT)) 
                    FROM QuestionValidations 
                    WHERE QuestionId = @QuestionId AND IsActive = 1 AND Score IS NOT NULL";
                
                var score = await connection.ExecuteScalarAsync<double?>(sql, new { QuestionId = questionId });
                return score ?? 0.0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting average validation score for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionValidation>> GetPagedAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var offset = (page - 1) * pageSize;
                var sql = @"
                    SELECT qv.*, q.Text as QuestionText, u.Name as ValidatedByUserName
                    FROM QuestionValidations qv
                    LEFT JOIN Questions q ON qv.QuestionId = q.Id
                    LEFT JOIN Users u ON qv.ValidatedBy = u.Id
                    WHERE qv.IsActive = 1
                    ORDER BY qv.ValidatedAt DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var validations = await connection.QueryAsync<QuestionValidation>(sql, 
                    new { Offset = offset, PageSize = pageSize });
                return validations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged validations");
                throw;
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM QuestionValidations WHERE IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total validation count");
                throw;
            }
        }

        public async Task<IEnumerable<QuestionValidation>> GetFailedValidationsAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qv.*, u.Name as ValidatedByUserName
                    FROM QuestionValidations qv
                    LEFT JOIN Users u ON qv.ValidatedBy = u.Id
                    WHERE qv.QuestionId = @QuestionId AND qv.Status = @Status AND qv.IsActive = 1
                    ORDER BY qv.ValidatedAt DESC";

                var validations = await connection.QueryAsync<QuestionValidation>(sql, 
                    new { QuestionId = questionId, Status = Shared.Enums.ValidationStatus.Rejected });
                return validations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving failed validations for question: {QuestionId}", questionId);
                throw;
            }
        }
    }
}
