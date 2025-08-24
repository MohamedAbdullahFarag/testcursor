using Dapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories
{
    public class QuestionTemplateRepository : BaseRepository<QuestionTemplate>//, IQuestionTemplateRepository
    {
        private new readonly ILogger<QuestionTemplateRepository> _logger;

        public QuestionTemplateRepository(IDbConnectionFactory connectionFactory, ILogger<QuestionTemplateRepository> logger)
            : base(connectionFactory, logger, "QuestionTemplates", "Id")
        {
            _logger = logger;
        }

        public async Task<QuestionTemplate?> GetByIdWithDetailsAsync(int templateId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTemplates qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.Id = @TemplateId AND qt.IsActive = 1";

                var template = await connection.QueryFirstOrDefaultAsync<QuestionTemplate>(sql, new { TemplateId = templateId });
                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question template with details: {TemplateId}", templateId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTemplate>> GetByCreatorAsync(int createdBy)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTemplates qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.CreatedBy = @CreatedBy AND qt.IsActive = 1
                    ORDER BY qt.CreatedAt DESC";

                var templates = await connection.QueryAsync<QuestionTemplate>(sql, new { CreatedBy = createdBy });
                return templates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving templates by creator: {CreatedBy}", createdBy);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTemplate>> GetPublicTemplatesAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTemplates qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.IsPublic = 1 AND qt.IsActive = 1
                    ORDER BY qt.CreatedAt DESC";

                var templates = await connection.QueryAsync<QuestionTemplate>(sql);
                return templates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving public templates");
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTemplate>> GetByCategoryAsync(int categoryId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTemplates qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.CategoryId = @CategoryId AND qt.IsActive = 1
                    ORDER BY qt.CreatedAt DESC";

                var templates = await connection.QueryAsync<QuestionTemplate>(sql, new { CategoryId = categoryId });
                return templates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving templates by category: {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTemplate>> SearchTemplatesAsync(string searchTerm)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTemplates qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.IsActive = 1 AND (qt.Name LIKE @SearchTerm OR qt.Description LIKE @SearchTerm)
                    ORDER BY qt.CreatedAt DESC";

                var searchPattern = $"%{searchTerm}%";
                var templates = await connection.QueryAsync<QuestionTemplate>(sql, new { SearchTerm = searchPattern });
                return templates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching templates: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTemplate>> GetPopularTemplatesAsync(int limit = 20)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT TOP (@Limit) qt.*, u.Name as CreatedByUserName, COUNT(q.Id) as UsageCount
                    FROM QuestionTemplates qt
                    LEFT JOIN Questions q ON qt.Id = q.TemplateId
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.IsActive = 1
                    GROUP BY qt.Id, qt.Name, qt.Description, qt.CategoryId, qt.CreatedBy, qt.CreatedAt, qt.LastModified, qt.IsActive, qt.IsPublic, u.Name
                    ORDER BY UsageCount DESC, qt.CreatedAt DESC";

                var templates = await connection.QueryAsync<QuestionTemplate>(sql, new { Limit = limit });
                return templates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving popular templates");
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTemplate>> GetPagedAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var offset = (page - 1) * pageSize;
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTemplates qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.IsActive = 1
                    ORDER BY qt.CreatedAt DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var templates = await connection.QueryAsync<QuestionTemplate>(sql, 
                    new { Offset = offset, PageSize = pageSize });
                return templates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged templates");
                throw;
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM QuestionTemplates WHERE IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total template count");
                throw;
            }
        }

        public async Task<bool> TemplateExistsAsync(string name)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM QuestionTemplates WHERE Name = @Name AND IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { Name = name });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if template exists: {Name}", name);
                throw;
            }
        }

        public async Task<int> GetTemplateUsageCountAsync(int templateId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM Questions WHERE TemplateId = @TemplateId AND IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { TemplateId = templateId });
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting usage count for template: {TemplateId}", templateId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTemplate>> GetTemplatesBySubjectAsync(string subject)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTemplates qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.Subject = @Subject AND qt.IsActive = 1
                    ORDER BY qt.CreatedAt DESC";

                var templates = await connection.QueryAsync<QuestionTemplate>(sql, new { Subject = subject });
                return templates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving templates by subject: {Subject}", subject);
                throw;
            }
        }
    }
}
