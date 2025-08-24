using Dapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories
{
    public class QuestionVersionRepository : BaseRepository<QuestionVersion>//, IQuestionVersionRepository
    {
        private new readonly ILogger<QuestionVersionRepository> _logger;

        public QuestionVersionRepository(IDbConnectionFactory connectionFactory, ILogger<QuestionVersionRepository> logger)
            : base(connectionFactory, logger, "QuestionVersions", "Id")
        {
            _logger = logger;
        }

        public async Task<QuestionVersion?> GetByIdWithDetailsAsync(int versionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qv.*, q.Text as QuestionText, u.Name as CreatedByUserName
                    FROM QuestionVersions qv
                    LEFT JOIN Questions q ON qv.QuestionId = q.Id
                    LEFT JOIN Users u ON qv.CreatedBy = u.Id
                    WHERE qv.Id = @VersionId AND qv.IsActive = 1";

                var version = await connection.QueryFirstOrDefaultAsync<QuestionVersion>(sql, new { VersionId = versionId });
                return version;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question version with details: {VersionId}", versionId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionVersion>> GetByQuestionIdAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qv.*, u.Name as CreatedByUserName
                    FROM QuestionVersions qv
                    LEFT JOIN Users u ON qv.CreatedBy = u.Id
                    WHERE qv.QuestionId = @QuestionId AND qv.IsActive = 1
                    ORDER BY qv.VersionNumber DESC";

                var versions = await connection.QueryAsync<QuestionVersion>(sql, new { QuestionId = questionId });
                return versions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving versions for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<QuestionVersion?> GetLatestVersionAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT TOP 1 qv.*, u.Name as CreatedByUserName
                    FROM QuestionVersions qv
                    LEFT JOIN Users u ON qv.CreatedBy = u.Id
                    WHERE qv.QuestionId = @QuestionId AND qv.IsActive = 1
                    ORDER BY qv.VersionNumber DESC";

                var version = await connection.QueryFirstOrDefaultAsync<QuestionVersion>(sql, new { QuestionId = questionId });
                return version;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving latest version for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<QuestionVersion?> GetByVersionNumberAsync(int questionId, string versionNumber)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qv.*, u.Name as CreatedByUserName
                    FROM QuestionVersions qv
                    LEFT JOIN Users u ON qv.CreatedBy = u.Id
                    WHERE qv.QuestionId = @QuestionId AND qv.VersionNumber = @VersionNumber AND qv.IsActive = 1";

                var version = await connection.QueryFirstOrDefaultAsync<QuestionVersion>(sql, 
                    new { QuestionId = questionId, VersionNumber = versionNumber });
                return version;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving version {VersionNumber} for question: {QuestionId}", versionNumber, questionId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionVersion>> GetByStatusAsync(Shared.Enums.VersionStatus status)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qv.*, q.Text as QuestionText, u.Name as CreatedByUserName
                    FROM QuestionVersions qv
                    LEFT JOIN Questions q ON qv.QuestionId = q.Id
                    LEFT JOIN Users u ON qv.CreatedBy = u.Id
                    WHERE qv.Status = @Status AND qv.IsActive = 1
                    ORDER BY qv.CreatedAt DESC";

                var versions = await connection.QueryAsync<QuestionVersion>(sql, new { Status = status });
                return versions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving versions by status: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionVersion>> GetByCreatorAsync(int createdBy, int page = 1, int pageSize = 20)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var offset = (page - 1) * pageSize;
                var sql = @"
                    SELECT qv.*, q.Text as QuestionText
                    FROM QuestionVersions qv
                    LEFT JOIN Questions q ON qv.QuestionId = q.Id
                    WHERE qv.CreatedBy = @CreatedBy AND qv.IsActive = 1
                    ORDER BY qv.CreatedAt DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var versions = await connection.QueryAsync<QuestionVersion>(sql, 
                    new { CreatedBy = createdBy, Offset = offset, PageSize = pageSize });
                return versions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving versions by creator: {CreatedBy}", createdBy);
                throw;
            }
        }

        public async Task<int> GetVersionCountAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM QuestionVersions WHERE QuestionId = @QuestionId AND IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { QuestionId = questionId });
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting version count for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<string> GetNextVersionNumberAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT TOP 1 VersionNumber 
                    FROM QuestionVersions 
                    WHERE QuestionId = @QuestionId AND IsActive = 1 
                    ORDER BY VersionNumber DESC";

                var latestVersion = await connection.QueryFirstOrDefaultAsync<string>(sql, new { QuestionId = questionId });
                
                if (string.IsNullOrEmpty(latestVersion))
                {
                    return "1.0";
                }

                if (decimal.TryParse(latestVersion, out var version))
                {
                    return (version + 0.1m).ToString("F1");
                }

                return "1.0";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting next version number for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<bool> VersionExistsAsync(int questionId, string versionNumber)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT COUNT(*) FROM QuestionVersions 
                    WHERE QuestionId = @QuestionId AND VersionNumber = @VersionNumber AND IsActive = 1";
                
                var count = await connection.ExecuteScalarAsync<int>(sql, 
                    new { QuestionId = questionId, VersionNumber = versionNumber });
                
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if version exists: {QuestionId}, {VersionNumber}", questionId, versionNumber);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionVersion>> GetPagedAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var offset = (page - 1) * pageSize;
                var sql = @"
                    SELECT qv.*, q.Text as QuestionText, u.Name as CreatedByUserName
                    FROM QuestionVersions qv
                    LEFT JOIN Questions q ON qv.QuestionId = q.Id
                    LEFT JOIN Users u ON qv.CreatedBy = u.Id
                    WHERE qv.IsActive = 1
                    ORDER BY qv.CreatedAt DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var versions = await connection.QueryAsync<QuestionVersion>(sql, 
                    new { Offset = offset, PageSize = pageSize });
                return versions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged versions");
                throw;
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM QuestionVersions WHERE IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total version count");
                throw;
            }
        }
    }
}
