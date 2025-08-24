using Dapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories
{
    public class QuestionTagRepository : BaseRepository<QuestionTag>//, IQuestionTagRepository
    {
        private new readonly ILogger<QuestionTagRepository> _logger;

        public QuestionTagRepository(IDbConnectionFactory connectionFactory, ILogger<QuestionTagRepository> logger)
            : base(connectionFactory, logger, "QuestionTags", "Id")
        {
            _logger = logger;
        }

        public async Task<QuestionTag?> GetByIdWithDetailsAsync(int tagId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTags qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.Id = @TagId AND qt.IsActive = 1";

                var tag = await connection.QueryFirstOrDefaultAsync<QuestionTag>(sql, new { TagId = tagId });
                return tag;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question tag with details: {TagId}", tagId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTag>> GetByCategoryAsync(int categoryId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTags qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.CategoryId = @CategoryId AND qt.IsActive = 1
                    ORDER BY qt.Name";

                var tags = await connection.QueryAsync<QuestionTag>(sql, new { CategoryId = categoryId });
                return tags;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tags by category: {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTag>> GetByCreatorAsync(int createdBy)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTags qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.CreatedBy = @CreatedBy AND qt.IsActive = 1
                    ORDER BY qt.Name";

                var tags = await connection.QueryAsync<QuestionTag>(sql, new { CreatedBy = createdBy });
                return tags;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tags by creator: {CreatedBy}", createdBy);
                throw;
            }
        }

        public async Task<QuestionTag?> GetByNameAsync(string name)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTags qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.Name = @Name AND qt.IsActive = 1";

                var tag = await connection.QueryFirstOrDefaultAsync<QuestionTag>(sql, new { Name = name });
                return tag;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tag by name: {Name}", name);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTag>> GetPopularTagsAsync(int limit = 20)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT TOP (@Limit) qt.*, COUNT(qta.QuestionId) as UsageCount
                    FROM QuestionTags qt
                    LEFT JOIN QuestionTagAssignments qta ON qt.Id = qta.TagId
                    WHERE qt.IsActive = 1
                    GROUP BY qt.Id, qt.Name, qt.Description, qt.CategoryId, qt.CreatedBy, qt.CreatedAt, qt.LastModified, qt.IsActive
                    ORDER BY UsageCount DESC, qt.Name";

                var tags = await connection.QueryAsync<QuestionTag>(sql, new { Limit = limit });
                return tags;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving popular tags");
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTag>> SearchTagsAsync(string searchTerm)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTags qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.IsActive = 1 AND (qt.Name LIKE @SearchTerm OR qt.Description LIKE @SearchTerm)
                    ORDER BY qt.Name";

                var searchPattern = $"%{searchTerm}%";
                var tags = await connection.QueryAsync<QuestionTag>(sql, new { SearchTerm = searchPattern });
                return tags;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tags: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTag>> GetPagedAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var offset = (page - 1) * pageSize;
                var sql = @"
                    SELECT qt.*, u.Name as CreatedByUserName
                    FROM QuestionTags qt
                    LEFT JOIN Users u ON qt.CreatedBy = u.Id
                    WHERE qt.IsActive = 1
                    ORDER BY qt.Name
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var tags = await connection.QueryAsync<QuestionTag>(sql, 
                    new { Offset = offset, PageSize = pageSize });
                return tags;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged tags");
                throw;
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM QuestionTags WHERE IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total tag count");
                throw;
            }
        }

        public async Task<bool> TagExistsAsync(string name)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM QuestionTags WHERE Name = @Name AND IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { Name = name });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if tag exists: {Name}", name);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTag>> GetTagsForQuestionAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.*, qta.AssignedAt, qta.AssignedBy, qta.Notes
                    FROM QuestionTags qt
                    INNER JOIN QuestionTagAssignments qta ON qt.Id = qta.TagId
                    WHERE qta.QuestionId = @QuestionId AND qt.IsActive = 1
                    ORDER BY qta.AssignedAt DESC";

                var tags = await connection.QueryAsync<QuestionTag>(sql, new { QuestionId = questionId });
                return tags;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tags for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<int> GetQuestionTagCountAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT COUNT(*) FROM QuestionTagAssignments qta
                    INNER JOIN QuestionTags qt ON qta.TagId = qt.Id
                    WHERE qta.QuestionId = @QuestionId AND qt.IsActive = 1";
                
                var count = await connection.ExecuteScalarAsync<int>(sql, new { QuestionId = questionId });
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tag count for question: {QuestionId}", questionId);
                throw;
            }
        }
    }
}
