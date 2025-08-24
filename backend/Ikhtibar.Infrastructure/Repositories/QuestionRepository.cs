using Dapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Ikhtibar.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories
{
    public class QuestionRepository : BaseRepository<Question>//, IQuestionRepository
    {
        private new readonly ILogger<QuestionRepository> _logger;

        public QuestionRepository(IDbConnectionFactory connectionFactory, ILogger<QuestionRepository> logger)
            : base(connectionFactory, logger, "Questions", "Id")
        {
            _logger = logger;
        }

        public async Task<Question?> GetByIdWithDetailsAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT q.*, qt.Name as QuestionTypeName, dl.Name as DifficultyLevelName, 
                           qs.Name as QuestionStatusName, tn.Name as TreeNodeName
                    FROM Questions q
                    LEFT JOIN QuestionTypes qt ON q.QuestionTypeId = qt.Id
                    LEFT JOIN DifficultyLevels dl ON q.DifficultyLevelId = dl.Id
                    LEFT JOIN QuestionStatuses qs ON q.QuestionStatusId = qs.Id
                    LEFT JOIN TreeNodes tn ON q.PrimaryTreeNodeId = tn.Id
                    WHERE q.Id = @QuestionId AND q.IsActive = 1";

                var question = await connection.QueryFirstOrDefaultAsync<Question>(sql, new { QuestionId = questionId });
                return question;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question with details: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<Question?> GetByIdWithAnswersAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT q.*, a.*
                    FROM Questions q
                    LEFT JOIN Answers a ON q.Id = a.QuestionId
                    WHERE q.Id = @QuestionId AND q.IsActive = 1 AND (a.IsActive = 1 OR a.IsActive IS NULL)";

                var questionDictionary = new Dictionary<int, Question>();
                
                await connection.QueryAsync<Question, Answer, Question>(
                    sql,
                    (question, answer) =>
                    {
                        if (!questionDictionary.TryGetValue(question.Id, out var questionEntry))
                        {
                            questionEntry = question;
                            questionEntry.Answers = new List<Answer>();
                            questionDictionary.Add(question.Id, questionEntry);
                        }

                        if (answer != null)
                        {
                            questionEntry.Answers.Add(answer);
                        }

                        return questionEntry;
                    },
                    new { QuestionId = questionId },
                    splitOn: "Id"
                );

                return questionDictionary.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question with answers: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<Question?> GetByIdWithMediaAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT q.*, qm.*
                    FROM Questions q
                    LEFT JOIN QuestionMedia qm ON q.Id = qm.QuestionId
                    WHERE q.Id = @QuestionId AND q.IsActive = 1 AND (qm.Id IS NULL OR qm.Id > 0)";

                var questionDictionary = new Dictionary<int, Question>();
                
                await connection.QueryAsync<Question, QuestionMedia, Question>(
                    sql,
                    (question, media) =>
                    {
                        if (!questionDictionary.TryGetValue(question.Id, out var questionEntry))
                        {
                            questionEntry = question;
                            questionEntry.MediaAttachments = new List<QuestionMedia>();
                            questionDictionary.Add(question.Id, questionEntry);
                        }

                        if (media != null)
                        {
                            questionEntry.MediaAttachments.Add(media);
                        }

                        return questionEntry;
                    },
                    new { QuestionId = questionId },
                    splitOn: "Id"
                );

                return questionDictionary.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question with media: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<Question?> GetByIdWithTagsAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT q.*, qt.*, qta.*
                    FROM Questions q
                    LEFT JOIN QuestionTagAssignments qta ON q.Id = qta.QuestionId
                    LEFT JOIN QuestionTags qt ON qta.TagId = qt.Id
                    WHERE q.Id = @QuestionId AND q.IsActive = 1 AND (qt.IsActive = 1 OR qt.IsActive IS NULL)";

                var questionDictionary = new Dictionary<int, Question>();
                
                await connection.QueryAsync<Question, QuestionTag, QuestionTagAssignment, Question>(
                    sql,
                    (question, tag, tagAssignment) =>
                    {
                        if (!questionDictionary.TryGetValue(question.Id, out var questionEntry))
                        {
                            questionEntry = question;
                            questionEntry.TagAssignments = new List<QuestionTagAssignment>();
                            questionDictionary.Add(question.Id, questionEntry);
                        }

                        if (tag != null && tagAssignment != null)
                        {
                            questionEntry.TagAssignments.Add(tagAssignment);
                        }

                        return questionEntry;
                    },
                    new { QuestionId = questionId },
                    splitOn: "Id"
                );

                return questionDictionary.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question with tags: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<Question?> GetByIdWithAllDetailsAsync(int questionId)
        {
            try
            {
                var question = await GetByIdWithDetailsAsync(questionId);
                if (question == null) return null;

                // Load answers
                question.Answers = (await GetQuestionAnswersAsync(questionId)).ToList();
                
                // Load media
                question.MediaAttachments = (await GetQuestionMediaAsync(questionId)).ToList();
                
                // Load tags
                question.TagAssignments = (await GetTagAssignmentsAsync(questionId)).ToList();

                return question;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question with all details: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<(IEnumerable<Question> Questions, int TotalCount)> GetPagedAsync(int page, int pageSize, string? whereClause = null, object? parameters = null)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var offset = (page - 1) * pageSize;
                
                var countSql = "SELECT COUNT(*) FROM Questions WHERE IsActive = 1";
                if (!string.IsNullOrEmpty(whereClause))
                {
                    countSql += " AND " + whereClause;
                }
                
                var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
                
                var sql = @"
                    SELECT q.*, qt.Name as QuestionTypeName, dl.Name as DifficultyLevelName, 
                           qs.Name as QuestionStatusName
                    FROM Questions q
                    LEFT JOIN QuestionTypes qt ON q.QuestionTypeId = qt.Id
                    LEFT JOIN DifficultyLevels dl ON q.DifficultyLevelId = dl.Id
                    LEFT JOIN QuestionStatuses qs ON q.QuestionStatusId = qs.Id
                    WHERE q.IsActive = 1";
                
                if (!string.IsNullOrEmpty(whereClause))
                {
                    sql += " AND " + whereClause;
                }
                
                sql += " ORDER BY q.CreatedAt DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                
                var queryParams = new DynamicParameters(parameters);
                queryParams.Add("@Offset", offset);
                queryParams.Add("@PageSize", pageSize);
                
                var questions = await connection.QueryAsync<Question>(sql, queryParams);
                
                return (questions, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged questions");
                throw;
            }
        }

        public async Task<IEnumerable<Question>> GetByFilterAsync(QuestionFilterDto filter)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT q.*, qt.Name as QuestionTypeName, dl.Name as DifficultyLevelName, 
                           qs.Name as QuestionStatusName
                    FROM Questions q
                    LEFT JOIN QuestionTypes qt ON q.QuestionTypeId = qt.Id
                    LEFT JOIN DifficultyLevels dl ON q.DifficultyLevelId = dl.Id
                    LEFT JOIN QuestionStatuses qs ON q.QuestionStatusId = qs.Id
                    WHERE q.IsActive = 1";
                
                var parameters = new DynamicParameters();
                
                if (filter.QuestionBankId.HasValue)
                {
                    sql += " AND q.QuestionBankId = @QuestionBankId";
                    parameters.Add("@QuestionBankId", filter.QuestionBankId.Value);
                }
                
                if (filter.QuestionTypeId.HasValue)
                {
                    sql += " AND q.QuestionTypeId = @QuestionTypeId";
                    parameters.Add("@QuestionTypeId", filter.QuestionTypeId.Value);
                }
                
                if (filter.DifficultyLevelId.HasValue)
                {
                    sql += " AND q.DifficultyLevelId = @DifficultyLevelId";
                    parameters.Add("@DifficultyLevelId", filter.DifficultyLevelId.Value);
                }
                
                if (filter.StatusId.HasValue)
                {
                    sql += " AND q.QuestionStatusId = @StatusId";
                    parameters.Add("@StatusId", filter.StatusId.Value);
                }
                
                if (filter.TreeNodeId.HasValue)
                {
                    sql += " AND q.PrimaryTreeNodeId = @TreeNodeId";
                    parameters.Add("@TreeNodeId", filter.TreeNodeId.Value);
                }
                
                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    sql += " AND (q.Text LIKE @SearchText OR q.Solution LIKE @SearchText)";
                    parameters.Add("@SearchText", $"%{filter.SearchText}%");
                }
                
                sql += " ORDER BY q.CreatedAt DESC";
                
                var questions = await connection.QueryAsync<Question>(sql, parameters);
                return questions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions by filter");
                throw;
            }
        }

        public async Task<IEnumerable<Question>> GetByTreeNodeAsync(int treeNodeId, QuestionFilterDto filter)
        {
            try
            {
                var treeNodeFilter = filter.Clone();
                treeNodeFilter.TreeNodeId = treeNodeId;
                return await GetByFilterAsync(treeNodeFilter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions by tree node: {TreeNodeId}", treeNodeId);
                throw;
            }
        }

        public async Task<IEnumerable<Question>> GetByBankAsync(int questionBankId, QuestionFilterDto filter)
        {
            try
            {
                var bankFilter = filter.Clone();
                bankFilter.QuestionBankId = questionBankId;
                return await GetByFilterAsync(bankFilter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions by bank: {QuestionBankId}", questionBankId);
                throw;
            }
        }

        public async Task<IEnumerable<Question>> GetByTypeAsync(int questionTypeId, QuestionFilterDto filter)
        {
            try
            {
                var typeFilter = filter.Clone();
                typeFilter.QuestionTypeId = questionTypeId;
                return await GetByFilterAsync(typeFilter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions by type: {QuestionTypeId}", questionTypeId);
                throw;
            }
        }

        public async Task<IEnumerable<Question>> GetByStatusAsync(int statusId, QuestionFilterDto filter)
        {
            try
            {
                var statusFilter = filter.Clone();
                statusFilter.StatusId = statusId;
                return await GetByFilterAsync(statusFilter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions by status: {StatusId}", statusId);
                throw;
            }
        }

        public async Task<IEnumerable<Question>> SearchByTextAsync(string searchText, SearchOptionsDto options)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT q.*, qt.Name as QuestionTypeName, dl.Name as DifficultyLevelName
                    FROM Questions q
                    LEFT JOIN QuestionTypes qt ON q.QuestionTypeId = qt.Id
                    LEFT JOIN DifficultyLevels dl ON q.DifficultyLevelId = dl.Id
                    WHERE q.IsActive = 1 AND (q.Text LIKE @SearchText OR q.Solution LIKE @SearchText)
                    ORDER BY q.CreatedAt DESC";
                
                var questions = await connection.QueryAsync<Question>(sql, new { SearchText = $"%{searchText}%" });
                return questions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching questions by text: {SearchText}", searchText);
                throw;
            }
        }

        public async Task<IEnumerable<Question>> SearchByTagsAsync(IEnumerable<string> tags, SearchOptionsDto options)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var tagList = tags.ToList();
                var tagParameters = string.Join(",", tagList.Select((_, i) => $"@Tag{i}"));
                
                var sql = $@"
                    SELECT DISTINCT q.*, qt.Name as QuestionTypeName, dl.Name as DifficultyLevelName
                    FROM Questions q
                    LEFT JOIN QuestionTypes qt ON q.QuestionTypeId = qt.Id
                    LEFT JOIN DifficultyLevels dl ON q.DifficultyLevelId = dl.Id
                    LEFT JOIN QuestionTagAssignments qta ON q.Id = qta.QuestionId
                    LEFT JOIN QuestionTags qt2 ON qta.TagId = qt2.Id
                    WHERE q.IsActive = 1 AND qt2.Name IN ({tagParameters})
                    ORDER BY q.CreatedAt DESC";
                
                var parameters = new DynamicParameters();
                for (int i = 0; i < tagList.Count; i++)
                {
                    parameters.Add($"@Tag{i}", tagList[i]);
                }
                
                var questions = await connection.QueryAsync<Question>(sql, parameters);
                return questions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching questions by tags");
                throw;
            }
        }

        public async Task<IEnumerable<Question>> SearchByCriteriaAsync(QuestionCriteriaDto criteria, SearchOptionsDto options)
        {
            try
            {
                // This would implement more complex search criteria
                // For now, return empty result
                return new List<Question>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching questions by criteria");
                throw;
            }
        }

        public async Task<IEnumerable<Question>> GetRelatedQuestionsAsync(int questionId, int limit = 10)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT TOP(@Limit) q.*, qt.Name as QuestionTypeName
                    FROM Questions q
                    LEFT JOIN QuestionTypes qt ON q.QuestionTypeId = qt.Id
                    WHERE q.IsActive = 1 AND q.Id != @QuestionId
                    AND q.QuestionTypeId = (SELECT QuestionTypeId FROM Questions WHERE Id = @QuestionId)
                    ORDER BY q.CreatedAt DESC";
                
                var questions = await connection.QueryAsync<Question>(sql, new { QuestionId = questionId, Limit = limit });
                return questions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving related questions: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<bool> LinkQuestionsAsync(int sourceQuestionId, int targetQuestionId, string relationshipType)
        {
            try
            {
                // This would implement question linking logic
                // For now, return true as placeholder
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error linking questions: {SourceQuestionId} -> {TargetQuestionId}", sourceQuestionId, targetQuestionId);
                throw;
            }
        }

        public async Task<bool> UnlinkQuestionsAsync(int sourceQuestionId, int targetQuestionId)
        {
            try
            {
                // This would implement question unlinking logic
                // For now, return true as placeholder
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlinking questions: {SourceQuestionId} -> {TargetQuestionId}", sourceQuestionId, targetQuestionId);
                throw;
            }
        }

        public async Task<IEnumerable<Answer>> GetQuestionAnswersAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT * FROM Answers 
                    WHERE QuestionId = @QuestionId AND IsActive = 1 
                    ORDER BY SortOrder, Id";
                
                var answers = await connection.QueryAsync<Answer>(sql, new { QuestionId = questionId });
                return answers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question answers: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<Answer?> GetAnswerAsync(int answerId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT * FROM Answers WHERE Id = @AnswerId AND IsActive = 1";
                var answer = await connection.QueryFirstOrDefaultAsync<Answer>(sql, new { AnswerId = answerId });
                return answer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving answer: {AnswerId}", answerId);
                throw;
            }
        }

        public async Task<Answer> AddAnswerAsync(Answer answer)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    INSERT INTO Answers (QuestionId, Text, IsCorrect, Explanation, SortOrder, CreatedAt, IsActive)
                    VALUES (@QuestionId, @Text, @IsCorrect, @Explanation, @SortOrder, @CreatedAt, @IsActive);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                
                var id = await connection.ExecuteScalarAsync<int>(sql, answer);
                answer.Id = id;
                
                return answer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding answer to question: {QuestionId}", answer.QuestionId);
                throw;
            }
        }

        public async Task<bool> UpdateAnswerAsync(Answer answer)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    UPDATE Answers 
                    SET Text = @Text, IsCorrect = @IsCorrect, Explanation = @Explanation, 
                        SortOrder = @SortOrder, IsActive = @IsActive
                    WHERE Id = @Id";
                
                var rowsAffected = await connection.ExecuteAsync(sql, answer);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating answer: {AnswerId}", answer.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAnswerAsync(int answerId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "UPDATE Answers SET IsActive = 0 WHERE Id = @AnswerId";
                var rowsAffected = await connection.ExecuteAsync(sql, new { AnswerId = answerId });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting answer: {AnswerId}", answerId);
                throw;
            }
        }

        public async Task<int> GetAnswerCountAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM Answers WHERE QuestionId = @QuestionId AND IsActive = 1";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { QuestionId = questionId });
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting answer count for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionMedia>> GetQuestionMediaAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT * FROM QuestionMedia 
                    WHERE QuestionId = @QuestionId 
                    ORDER BY SortOrder, Id";
                
                var media = await connection.QueryAsync<QuestionMedia>(sql, new { QuestionId = questionId });
                return media;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question media: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<bool> AttachMediaAsync(int questionId, int mediaId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    INSERT INTO QuestionMedia (QuestionId, MediaFileId, MediaType, SortOrder, Caption, AttachedAt, AttachedBy)
                    VALUES (@QuestionId, @MediaFileId, 'Image', 0, '', @AttachedAt, @AttachedBy)";
                
                var parameters = new
                {
                    QuestionId = questionId,
                    MediaFileId = mediaId,
                    AttachedAt = DateTime.UtcNow,
                    AttachedBy = 1 // TODO: Get from current user context
                };
                
                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error attaching media to question: {QuestionId}, {MediaId}", questionId, mediaId);
                throw;
            }
        }

        public async Task<bool> DetachMediaAsync(int questionId, int mediaId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "DELETE FROM QuestionMedia WHERE QuestionId = @QuestionId AND MediaFileId = @MediaFileId";
                var rowsAffected = await connection.ExecuteAsync(sql, new { QuestionId = questionId, MediaFileId = mediaId });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error detaching media from question: {QuestionId}, {MediaId}", questionId, mediaId);
                throw;
            }
        }

        public async Task<int> GetMediaCountAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "SELECT COUNT(*) FROM QuestionMedia WHERE QuestionId = @QuestionId";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { QuestionId = questionId });
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting media count for question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionTag>> GetQuestionTagsAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT qt.* FROM QuestionTags qt
                    INNER JOIN QuestionTagAssignments qta ON qt.Id = qta.TagId
                    WHERE qta.QuestionId = @QuestionId AND qt.IsActive = 1
                    ORDER BY qt.Name";
                
                var tags = await connection.QueryAsync<QuestionTag>(sql, new { QuestionId = questionId });
                return tags;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question tags: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<bool> ApplyTagAsync(int questionId, int tagId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                // Check if tag is already applied
                var existingSql = "SELECT COUNT(*) FROM QuestionTagAssignments WHERE QuestionId = @QuestionId AND TagId = @TagId";
                var existingCount = await connection.ExecuteScalarAsync<int>(existingSql, new { QuestionId = questionId, TagId = tagId });
                
                if (existingCount > 0)
                {
                    return true; // Tag already applied
                }
                
                var sql = @"
                    INSERT INTO QuestionTagAssignments (QuestionId, TagId, AssignedBy, AssignedAt)
                    VALUES (@QuestionId, @TagId, @AssignedBy, @AssignedAt)";
                
                var parameters = new
                {
                    QuestionId = questionId,
                    TagId = tagId,
                    AssignedBy = 1, // TODO: Get from current user context
                    AssignedAt = DateTime.UtcNow
                };
                
                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying tag to question: {QuestionId}, {TagId}", questionId, tagId);
                throw;
            }
        }

        public async Task<bool> RemoveTagAsync(int questionId, int tagId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = "DELETE FROM QuestionTagAssignments WHERE QuestionId = @QuestionId AND TagId = @TagId";
                var rowsAffected = await connection.ExecuteAsync(sql, new { QuestionId = questionId, TagId = tagId });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing tag from question: {QuestionId}, {TagId}", questionId, tagId);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetTagNamesAsync(int questionId)
        {
            try
            {
                var tags = await GetQuestionTagsAsync(questionId);
                return tags.Select(t => t.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tag names for question: {QuestionId}", questionId);
                throw;
            }
        }

        private async Task<IEnumerable<QuestionTagAssignment>> GetTagAssignmentsAsync(int questionId)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var sql = @"
                    SELECT * FROM QuestionTagAssignments 
                    WHERE QuestionId = @QuestionId
                    ORDER BY AssignedAt";
                
                var assignments = await connection.QueryAsync<QuestionTagAssignment>(sql, new { QuestionId = questionId });
                return assignments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tag assignments for question: {QuestionId}", questionId);
                throw;
            }
        }

        // Additional methods would be implemented here...
        // For brevity, I'm implementing the core functionality first
    }
}
