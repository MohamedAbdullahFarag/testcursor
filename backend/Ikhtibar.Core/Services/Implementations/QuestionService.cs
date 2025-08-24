using AutoMapper;

using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.Exceptions;
using ValidationException = Ikhtibar.Core.Exceptions.ValidationException;
using NotFoundException = Ikhtibar.Core.Exceptions.NotFoundException;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Models;
using Microsoft.Extensions.Logging;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionBankRepository _questionBankRepository;
        private readonly IQuestionValidationService _validationService;
        private readonly IQuestionVersioningService _versioningService;
        private readonly IQuestionUsageRepository _questionUsageRepository;
        private readonly ITreeNodeService _treeNodeService;
        private readonly ILogger<QuestionService> _logger;
        private readonly IMapper _mapper;

        public QuestionService(
            IQuestionRepository questionRepository,
            IQuestionBankRepository questionBankRepository,
            IQuestionValidationService validationService,
            IQuestionVersioningService versioningService,
            IQuestionUsageRepository questionUsageRepository,
            ITreeNodeService treeNodeService,
            ILogger<QuestionService> logger,
            IMapper mapper)
        {
            _questionRepository = questionRepository;
            _questionBankRepository = questionBankRepository;
            _validationService = validationService;
            _versioningService = versioningService;
            _questionUsageRepository = questionUsageRepository;
            _treeNodeService = treeNodeService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto dto)
        {
            try
            {
                _logger.LogInformation(0, "Creating new question: {Text}", dto.Text.Substring(0, Math.Min(50, dto.Text.Length)));

                // Validate question data
                var validationResult = await _validationService.ValidateQuestionDataAsync(dto);
                if (!validationResult.IsValid)
                {
                    var errorMessages = validationResult.ValidationErrors.Select(e => e.Message);
                    throw new ValidationException($"Question validation failed: {string.Join(", ", errorMessages)}");
                }

                // Create question entity
                var question = new Question
                {
                    Text = dto.Text,
                    QuestionTypeId = dto.QuestionTypeId,
                    DifficultyLevelId = dto.DifficultyLevelId ?? 0,
                    Solution = dto.Solution,
                    EstimatedTimeSec = dto.EstimatedTimeSec,
                    Points = dto.Points,
                    QuestionStatusId = 1, // Draft status
                    PrimaryTreeNodeId = dto.PrimaryTreeNodeId ?? 1,
                    Version = "1.0",
                    Tags = (dto.Tags != null && dto.Tags.Any()) ? string.Join(',', dto.Tags) : null,
                    Metadata = dto.MetadataJson,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var savedQuestion = await _questionRepository.AddAsync(question);
                _logger.LogInformation(0, "Question created successfully with ID: {QuestionId}", savedQuestion.Id);

                return await GetQuestionAsync(savedQuestion.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                throw;
            }
        }

        public async Task<QuestionDto> GetQuestionAsync(int questionId)
        {
            try
            {
                var question = await _questionRepository.GetByIdWithDetailsAsync(questionId);
                if (question == null)
                {
                    throw new NotFoundException($"Question with ID {questionId} not found");
                }

                var questionDto = _mapper.Map<QuestionDto>(question);
                return questionDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<PagedResult<QuestionDto>> GetQuestionsAsync(QuestionFilterDto filter)
        {
            try
            {
                var (questions, totalCount) = await _questionRepository.GetPagedAsync(
                    filter.Page, 
                    filter.PageSize
                );

                var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);

                return new PagedResult<QuestionDto>
                {
                    Items = questionDtos,
                    TotalCount = totalCount,
                    PageNumber = filter.Page,
                    PageSize = filter.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions with filter");
                throw;
            }
        }

        public async Task<QuestionDto> UpdateQuestionAsync(int questionId, UpdateQuestionDto dto)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(questionId);
                if (question == null)
                {
                    throw new NotFoundException($"Question with ID {questionId} not found");
                }

                // Update question fields
                if (!string.IsNullOrEmpty(dto.Text))
                    question.Text = dto.Text;
                
                if (dto.QuestionTypeId.HasValue)
                    question.QuestionTypeId = dto.QuestionTypeId.Value;
                
                if (dto.DifficultyLevelId.HasValue)
                    question.DifficultyLevelId = dto.DifficultyLevelId.Value;
                
                if (dto.Solution != null)
                    question.Solution = dto.Solution;
                
                if (dto.EstimatedTimeSec.HasValue)
                    question.EstimatedTimeSec = dto.EstimatedTimeSec.Value;
                
                if (dto.Points.HasValue)
                    question.Points = dto.Points.Value;

                question.ModifiedAt = DateTime.UtcNow;
                question.ModifiedBy = dto.UpdatedBy;

                await _questionRepository.UpdateAsync(question);
                return await GetQuestionAsync(questionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(questionId);
                if (question == null)
                {
                    throw new NotFoundException($"Question with ID {questionId} not found");
                }

                question.IsActive = false;
                question.ModifiedAt = DateTime.UtcNow;

                await _questionRepository.UpdateAsync(question);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<bool> ArchiveQuestionAsync(int questionId)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(questionId);
                if (question == null)
                {
                    throw new NotFoundException($"Question with ID {questionId} not found");
                }

                question.IsActive = false;
                question.QuestionStatusId = 5; // Archived status
                question.ModifiedAt = DateTime.UtcNow;

                await _questionRepository.UpdateAsync(question);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving question: {QuestionId}", questionId);
                throw;
            }
        }

        public Task<QuestionValidationResult> ValidateQuestionAsync(ValidateQuestionDto dto)
        {
            try
            {
                var validationResult = new QuestionValidationResult
                {
                    QuestionId = dto.QuestionId,
                    IsValid = true,
                    ValidationErrors = new List<ValidationError>(),
                    ValidationWarnings = new List<ValidationWarning>(),
                    Score = 100
                };

                return Task.FromResult(validationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating question: {QuestionId}", dto.QuestionId);
                throw;
            }
        }

        public async Task<bool> PublishQuestionAsync(int questionId)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(questionId);
                if (question == null)
                {
                    throw new NotFoundException($"Question with ID {questionId} not found");
                }

                question.QuestionStatusId = 4; // Published status
                question.PublishedAt = DateTime.UtcNow;
                question.ModifiedAt = DateTime.UtcNow;

                await _questionRepository.UpdateAsync(question);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<bool> UnpublishQuestionAsync(int questionId)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(questionId);
                if (question == null)
                {
                    throw new NotFoundException($"Question with ID {questionId} not found");
                }

                question.QuestionStatusId = 1; // Draft status
                question.PublishedAt = null;
                question.ModifiedAt = DateTime.UtcNow;

                await _questionRepository.UpdateAsync(question);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<QuestionDto> DuplicateQuestionAsync(int questionId, DuplicateQuestionDto dto)
        {
            try
            {
                var sourceQuestion = await _questionRepository.GetByIdAsync(questionId);
                if (sourceQuestion == null)
                {
                    throw new NotFoundException($"Question with ID {questionId} not found");
                }

                var duplicateQuestion = new Question
                {
                    Text = dto.NewText ?? sourceQuestion.Text,
                    QuestionTypeId = sourceQuestion.QuestionTypeId,
                    DifficultyLevelId = sourceQuestion.DifficultyLevelId,
                    Solution = sourceQuestion.Solution,
                    EstimatedTimeSec = sourceQuestion.EstimatedTimeSec,
                    Points = sourceQuestion.Points,
                    QuestionStatusId = 1,
                    PrimaryTreeNodeId = dto.NewTreeNodeId ?? sourceQuestion.PrimaryTreeNodeId,
                    Version = "1.0",
                    Tags = sourceQuestion.Tags,
                    Metadata = sourceQuestion.Metadata,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var savedDuplicate = await _questionRepository.AddAsync(duplicateQuestion);
                return await GetQuestionAsync(savedDuplicate.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error duplicating question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionVersionDto>> GetQuestionVersionsAsync(int questionId)
        {
            try
            {
                var versions = await _questionRepository.GetQuestionVersionsAsync(questionId);
                return _mapper.Map<IEnumerable<QuestionVersionDto>>(versions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question versions: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<QuestionDto> CreateQuestionVersionAsync(int questionId, CreateVersionDto dto)
        {
            try
            {
                var version = await _versioningService.CreateVersionAsync(questionId, dto);
                return await GetQuestionAsync(questionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question version: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<bool> RestoreQuestionVersionAsync(int questionId, string version)
        {
            try
            {
                return await _versioningService.RestoreQuestionVersionAsync(questionId, version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring question version: {QuestionId}, {Version}", questionId, version);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionDto>> GetRelatedQuestionsAsync(int questionId)
        {
            try
            {
                var relatedQuestions = await _questionRepository.GetRelatedQuestionsAsync(questionId);
                return _mapper.Map<IEnumerable<QuestionDto>>(relatedQuestions);
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
                return await _questionRepository.LinkQuestionsAsync(sourceQuestionId, targetQuestionId, relationshipType);
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
                return await _questionRepository.UnlinkQuestionsAsync(sourceQuestionId, targetQuestionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlinking questions: {SourceQuestionId} -> {TargetQuestionId}", sourceQuestionId, targetQuestionId);
                throw;
            }
        }

        public async Task<bool> AttachMediaToQuestionAsync(int questionId, int mediaId)
        {
            try
            {
                return await _questionRepository.AttachMediaAsync(questionId, mediaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error attaching media to question: {QuestionId}, {MediaId}", questionId, mediaId);
                throw;
            }
        }

        public async Task<bool> DetachMediaFromQuestionAsync(int questionId, int mediaId)
        {
            try
            {
                return await _questionRepository.DetachMediaAsync(questionId, mediaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error detaching media from question: {QuestionId}, {MediaId}", questionId, mediaId);
                throw;
            }
        }

        public Task<IEnumerable<MediaFileDto>> GetQuestionMediaAsync(int questionId)
        {
            try
            {
                // TODO: Implement media retrieval when media service is available
                // For now, return empty list to maintain interface compatibility
                return Task.FromResult<IEnumerable<MediaFileDto>>(new List<MediaFileDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question media: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<QuestionUsageStatisticsDto> GetQuestionUsageAsync(int questionId)
        {
            try
            {
                var fromDate = DateTime.UtcNow.AddMonths(-6);
                var toDate = DateTime.UtcNow;
                
                return await _questionRepository.GetUsageStatisticsAsync(questionId, fromDate, toDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question usage: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<QuestionPerformanceDto> GetQuestionPerformanceAsync(int questionId)
        {
            try
            {
                var fromDate = DateTime.UtcNow.AddMonths(-6);
                var toDate = DateTime.UtcNow;
                
                var filter = new PerformanceFilterDto
                {
                    QuestionId = questionId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                var performanceResults = await _questionUsageRepository.GetQuestionPerformanceAsync(filter);
                return performanceResults.FirstOrDefault() ?? new QuestionPerformanceDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question performance: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionDto>> GetSimilarQuestionsAsync(int questionId)
        {
            try
            {
                var similarQuestions = await _questionRepository.GetRelatedQuestionsAsync(questionId);
                return _mapper.Map<IEnumerable<QuestionDto>>(similarQuestions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving similar questions: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<BulkOperationResult> BulkCreateQuestionsAsync(IEnumerable<CreateQuestionDto> questions)
        {
            try
            {
                var results = new List<BulkOperationItemResult>();
                var successfulQuestions = new List<QuestionDto>();

                foreach (var questionDto in questions)
                {
                    try
                    {
                        var question = await CreateQuestionAsync(questionDto);
                        successfulQuestions.Add(question);
                        results.Add(new BulkOperationItemResult
                        {
                            Id = question.Id,
                            Success = true,
                            Message = "Question created successfully"
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new BulkOperationItemResult
                        {
                            Id = 0,
                            Success = false,
                            Message = ex.Message
                        });
                    }
                }

                return new BulkOperationResult
                {
                    TotalCount = questions.Count(),
                    SuccessfulCount = successfulQuestions.Count,
                    FailedCount = questions.Count() - successfulQuestions.Count,
                    Results = results,
                    Data = successfulQuestions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk question creation");
                throw;
            }
        }

        public async Task<BulkOperationResult> BulkUpdateQuestionsAsync(BulkUpdateQuestionsDto dto)
        {
            try
            {
                var results = new List<BulkOperationItemResult>();
                var successfulUpdates = 0;

                foreach (var questionId in dto.QuestionIds)
                {
                    try
                    {
                        var updateDto = new UpdateQuestionDto
                        {
                            Text = dto.Text,
                            QuestionTypeId = dto.QuestionTypeId,
                            DifficultyLevelId = dto.DifficultyLevelId,
                            Solution = dto.Solution,
                            EstimatedTimeSec = dto.EstimatedTimeSec,
                            Points = dto.Points.HasValue ? (int?)Convert.ToInt32(dto.Points.Value) : null,
                            Tags = !string.IsNullOrWhiteSpace(dto.Tags) ? dto.Tags.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList() : null,
                            MetadataJson = dto.MetadataJson,
                            UpdatedBy = dto.UpdatedBy
                        };

                        await UpdateQuestionAsync(questionId, updateDto);
                        successfulUpdates++;
                        results.Add(new BulkOperationItemResult
                        {
                            Id = questionId,
                            Success = true,
                            Message = "Question updated successfully"
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new BulkOperationItemResult
                        {
                            Id = questionId,
                            Success = false,
                            Message = ex.Message
                        });
                    }
                }

                return new BulkOperationResult
                {
                    TotalCount = dto.QuestionIds.Count(),
                    SuccessfulCount = successfulUpdates,
                    FailedCount = dto.QuestionIds.Count() - successfulUpdates,
                    Results = results
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk question update");
                throw;
            }
        }

        public async Task<BulkOperationResult> BulkDeleteQuestionsAsync(IEnumerable<int> questionIds)
        {
            try
            {
                var results = new List<BulkOperationItemResult>();
                var successfulDeletions = 0;

                foreach (var questionId in questionIds)
                {
                    try
                    {
                        await DeleteQuestionAsync(questionId);
                        successfulDeletions++;
                        results.Add(new BulkOperationItemResult
                        {
                            Id = questionId,
                            Success = true,
                            Message = "Question deleted successfully"
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new BulkOperationItemResult
                        {
                            Id = questionId,
                            Success = false,
                            Message = ex.Message
                        });
                    }
                }

                return new BulkOperationResult
                {
                    TotalCount = questionIds.Count(),
                    SuccessfulCount = successfulDeletions,
                    FailedCount = questionIds.Count() - successfulDeletions,
                    Results = results
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk question deletion");
                throw;
            }
        }

        public async Task<BulkOperationResult> BulkTagQuestionsAsync(BulkTagDto dto)
        {
            try
            {
                var results = new List<BulkOperationItemResult>();
                var successfulTagging = 0;

                foreach (var questionId in dto.QuestionIds)
                {
                    try
                    {
                        foreach (var tagId in dto.TagIds)
                        {
                            await _questionRepository.ApplyTagAsync(questionId, tagId);
                        }
                        successfulTagging++;
                        results.Add(new BulkOperationItemResult
                        {
                            Id = questionId,
                            Success = true,
                            Message = "Tags applied successfully"
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new BulkOperationItemResult
                        {
                            Id = questionId,
                            Success = false,
                            Message = ex.Message
                        });
                    }
                }

                return new BulkOperationResult
                {
                    TotalCount = dto.QuestionIds.Count(),
                    SuccessfulCount = successfulTagging,
                    FailedCount = dto.QuestionIds.Count() - successfulTagging,
                    Results = results
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk question tagging");
                throw;
            }
        }
    }
}
