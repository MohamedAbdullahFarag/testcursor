# Question Bank Question Management System - Comprehensive Implementation PRP

## 🎯 Executive Summary
Generate a comprehensive question management system for the Ikhtibar question bank module that handles the complete lifecycle of questions including creation, editing, validation, versioning, and organization. This system will provide robust CRUD operations, advanced search capabilities, question import/export, and seamless integration with the tree management and media systems.

## 📋 What to Generate

### 1. Backend Question Management System
```
backend/Ikhtibar.Core/Entities/
├── QuestionBank.cs                    # Question bank collection entity
├── QuestionTemplate.cs                # Question template entity
├── QuestionVersion.cs                 # Question versioning entity
├── QuestionTag.cs                     # Question tagging entity
├── QuestionValidation.cs              # Question validation entity
├── QuestionImportBatch.cs             # Bulk import tracking entity
└── QuestionUsageHistory.cs            # Question usage tracking entity

backend/Ikhtibar.Core/Services/Interfaces/
├── IQuestionService.cs                # Core question operations
├── IQuestionBankService.cs            # Question bank management
├── IQuestionValidationService.cs      # Question validation
├── IQuestionVersioningService.cs      # Version management
├── IQuestionImportService.cs          # Import/export operations
├── IQuestionSearchService.cs          # Advanced search
├── IQuestionTemplateService.cs        # Template management
├── IQuestionTagService.cs             # Tag management
└── IQuestionAnalyticsService.cs       # Usage analytics

backend/Ikhtibar.Core/Services/Implementations/
├── QuestionService.cs                 # Main question logic
├── QuestionBankService.cs             # Bank management
├── QuestionValidationService.cs       # Validation logic
├── QuestionVersioningService.cs       # Version control
├── QuestionImportService.cs           # Import/export logic
├── QuestionSearchService.cs           # Search implementation
├── QuestionTemplateService.cs         # Template operations
├── QuestionTagService.cs              # Tag management
└── QuestionAnalyticsService.cs        # Analytics and reporting

backend/Ikhtibar.Core/Repositories/Interfaces/
├── IQuestionRepository.cs             # Question data access
├── IQuestionBankRepository.cs         # Question bank data access
├── IQuestionVersionRepository.cs      # Version data access
├── IQuestionTagRepository.cs          # Tag data access
├── IQuestionValidationRepository.cs   # Validation data access
├── IQuestionTemplateRepository.cs     # Template data access
└── IQuestionUsageRepository.cs        # Usage tracking data access

backend/Ikhtibar.Infrastructure/Repositories/
├── QuestionRepository.cs              # Question repository
├── QuestionBankRepository.cs          # Question bank repository
├── QuestionVersionRepository.cs       # Version repository
├── QuestionTagRepository.cs           # Tag repository
├── QuestionValidationRepository.cs    # Validation repository
├── QuestionTemplateRepository.cs      # Template repository
└── QuestionUsageRepository.cs         # Usage repository

backend/Ikhtibar.API/Controllers/
├── QuestionsController.cs             # Question CRUD endpoints
├── QuestionBanksController.cs         # Question bank endpoints
├── QuestionSearchController.cs        # Search endpoints
├── QuestionImportController.cs        # Import/export endpoints
├── QuestionValidationController.cs    # Validation endpoints
├── QuestionVersionsController.cs      # Version management
└── QuestionAnalyticsController.cs     # Analytics endpoints

backend/Ikhtibar.API/DTOs/
├── QuestionDto.cs                     # Question data objects
├── CreateQuestionDto.cs               # Question creation objects
├── UpdateQuestionDto.cs               # Question update objects
├── QuestionBankDto.cs                 # Question bank objects
├── QuestionSearchDto.cs               # Search request objects
├── QuestionValidationDto.cs           # Validation objects
├── QuestionVersionDto.cs              # Version objects
├── QuestionImportDto.cs               # Import/export objects
├── QuestionTemplateDto.cs             # Template objects
└── QuestionAnalyticsDto.cs            # Analytics objects
```

### 2. Frontend Question Management Interface
```
frontend/src/modules/question-bank/questions/
├── components/
│   ├── QuestionManager.tsx            # Main question management
│   ├── QuestionEditor.tsx             # Question creation/editing
│   ├── QuestionList.tsx               # Question listing
│   ├── QuestionGrid.tsx               # Grid view component
│   ├── QuestionCard.tsx               # Individual question card
│   ├── QuestionPreview.tsx            # Question preview modal
│   ├── QuestionSearch.tsx             # Search interface
│   ├── QuestionFilters.tsx            # Filter controls
│   ├── QuestionBankManager.tsx        # Bank management
│   ├── QuestionImporter.tsx           # Import interface
│   ├── QuestionExporter.tsx           # Export interface
│   ├── QuestionValidator.tsx          # Validation interface
│   ├── QuestionVersionHistory.tsx     # Version management
│   ├── QuestionTemplates.tsx          # Template management
│   ├── QuestionTags.tsx               # Tag management
│   └── QuestionAnalytics.tsx          # Analytics dashboard
├── forms/
│   ├── CreateQuestionForm.tsx         # Question creation form
│   ├── EditQuestionForm.tsx           # Question editing form
│   ├── QuestionBankForm.tsx           # Bank creation form
│   ├── ImportQuestionsForm.tsx        # Import configuration
│   ├── QuestionValidationForm.tsx     # Validation setup
│   └── QuestionTemplateForm.tsx       # Template creation
├── hooks/
│   ├── useQuestionManager.tsx         # Main question hook
│   ├── useQuestionCRUD.tsx            # CRUD operations
│   ├── useQuestionSearch.tsx          # Search functionality
│   ├── useQuestionValidation.tsx      # Validation hooks
│   ├── useQuestionVersioning.tsx      # Version management
│   ├── useQuestionImport.tsx          # Import/export
│   ├── useQuestionBanks.tsx           # Bank management
│   ├── useQuestionTemplates.tsx       # Template management
│   └── useQuestionAnalytics.tsx       # Analytics hooks
├── services/
│   ├── questionService.ts             # Question API service
│   ├── questionBankService.ts         # Bank API service
│   ├── searchService.ts               # Search service
│   ├── validationService.ts           # Validation service
│   ├── importService.ts               # Import/export service
│   ├── versionService.ts              # Version service
│   └── analyticsService.ts            # Analytics service
├── types/
│   ├── question.types.ts              # Question type definitions
│   ├── questionBank.types.ts          # Bank types
│   ├── search.types.ts                # Search types
│   ├── validation.types.ts            # Validation types
│   ├── import.types.ts                # Import/export types
│   └── analytics.types.ts             # Analytics types
├── utils/
│   ├── questionUtils.ts               # Question utility functions
│   ├── validationUtils.ts             # Validation utilities
│   ├── importUtils.ts                 # Import processing utils
│   ├── exportUtils.ts                 # Export formatting utils
│   └── analyticsUtils.ts              # Analytics calculations
├── constants/
│   ├── questionTypes.ts               # Question type constants
│   ├── difficultyLevels.ts            # Difficulty constants
│   ├── questionStatuses.ts            # Status constants
│   └── validationRules.ts             # Validation rules
└── locales/
    ├── en.json                        # English translations
    └── ar.json                        # Arabic translations
```

### 3. Advanced Question Features
```
backend/Ikhtibar.Core/Features/
├── QuestionAI/
│   ├── IQuestionGenerationService.cs  # AI question generation
│   ├── IQuestionAnalysisService.cs    # AI question analysis
│   └── IQuestionSimilarityService.cs  # Similarity detection
├── QuestionQuality/
│   ├── IQuestionQualityService.cs     # Quality assessment
│   ├── IQuestionReviewService.cs      # Peer review system
│   └── IQuestionFeedbackService.cs    # Feedback collection
└── QuestionSecurity/
    ├── IQuestionEncryptionService.cs  # Question encryption
    ├── IQuestionWatermarkService.cs   # Watermarking
    └── IQuestionAccessService.cs      # Access control
```

### 4. Question Processing Pipeline
```
backend/Ikhtibar.Infrastructure/Processing/
├── QuestionProcessingPipeline.cs      # Main processing pipeline
├── QuestionValidationProcessor.cs     # Validation processing
├── QuestionEnrichmentProcessor.cs     # Content enrichment
├── QuestionIndexingProcessor.cs       # Search indexing
└── QuestionAnalyticsProcessor.cs      # Analytics processing

backend/Ikhtibar.Infrastructure/BackgroundServices/
├── QuestionProcessingService.cs       # Background processing
├── QuestionIndexingService.cs         # Search index updates
├── QuestionAnalyticsService.cs        # Analytics processing
└── QuestionCleanupService.cs          # Maintenance tasks
```

## 🏗 Implementation Architecture

### Entity Design Patterns

#### Question Bank Entity
```csharp
[Table("QuestionBanks")]
public class QuestionBank : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public int CreatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastModified { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public bool IsPublic { get; set; } = false;

    [MaxLength(100)]
    public string? Subject { get; set; }

    [MaxLength(100)]
    public string? GradeLevel { get; set; }

    [MaxLength(100)]
    public string? Language { get; set; } = "en";

    [MaxLength(2000)]
    public string? MetadataJson { get; set; }

    // Statistics
    [Required]
    public int TotalQuestions { get; set; } = 0;

    [Required]
    public int ActiveQuestions { get; set; } = 0;

    [Required]
    public int ReviewedQuestions { get; set; } = 0;

    // Navigation properties
    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<QuestionBankAccess> AccessPermissions { get; set; } = new List<QuestionBankAccess>();
}
```

#### Question Template Entity
```csharp
[Table("QuestionTemplates")]
public class QuestionTemplate : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public int QuestionTypeId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Template { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? AnswerTemplate { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public bool IsPublic { get; set; } = false;

    [MaxLength(100)]
    public string? Subject { get; set; }

    [MaxLength(1000)]
    public string? Tags { get; set; }

    [MaxLength(2000)]
    public string? Instructions { get; set; }

    [MaxLength(2000)]
    public string? ValidationRules { get; set; }

    // Navigation properties
    [ForeignKey("QuestionTypeId")]
    public virtual QuestionType QuestionType { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; } = null!;

    public virtual ICollection<Question> QuestionsCreated { get; set; } = new List<Question>();
}
```

#### Question Version Entity
```csharp
[Table("QuestionVersions")]
public class QuestionVersion : BaseEntity
{
    [Required]
    public int QuestionId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Version { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Text { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Solution { get; set; }

    [MaxLength(2000)]
    public string? AnswersJson { get; set; }

    [MaxLength(2000)]
    public string? MediaJson { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? ChangeDescription { get; set; }

    [Required]
    public bool IsCurrent { get; set; } = false;

    [Required]
    public VersionStatus Status { get; set; } = VersionStatus.Draft;

    public DateTime? PublishedAt { get; set; }

    public int? ApprovedBy { get; set; }

    [MaxLength(2000)]
    public string? MetadataJson { get; set; }

    // Navigation properties
    [ForeignKey("QuestionId")]
    public virtual Question Question { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("ApprovedBy")]
    public virtual User? ApprovedByUser { get; set; }
}

public enum VersionStatus
{
    Draft = 1,
    PendingReview = 2,
    Approved = 3,
    Published = 4,
    Archived = 5,
    Rejected = 6
}
```

#### Question Tag Entity
```csharp
[Table("QuestionTags")]
public class QuestionTag : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public int CreatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public int UsageCount { get; set; } = 0;

    [MaxLength(7)]
    public string? Color { get; set; }

    // Navigation properties
    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; } = null!;

    public virtual ICollection<QuestionTagAssignment> QuestionAssignments { get; set; } = new List<QuestionTagAssignment>();
}

[Table("QuestionTagAssignments")]
public class QuestionTagAssignment : BaseEntity
{
    [Required]
    public int QuestionId { get; set; }

    [Required]
    public int TagId { get; set; }

    [Required]
    public int AssignedBy { get; set; }

    [Required]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("QuestionId")]
    public virtual Question Question { get; set; } = null!;

    [ForeignKey("TagId")]
    public virtual QuestionTag Tag { get; set; } = null!;

    [ForeignKey("AssignedBy")]
    public virtual User AssignedByUser { get; set; } = null!;
}
```

### Service Implementation Patterns

#### Core Question Service
```csharp
public interface IQuestionService
{
    // Basic CRUD operations
    Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto dto);
    Task<QuestionDto> GetQuestionAsync(int questionId);
    Task<PagedResult<QuestionDto>> GetQuestionsAsync(QuestionFilterDto filter);
    Task<QuestionDto> UpdateQuestionAsync(int questionId, UpdateQuestionDto dto);
    Task<bool> DeleteQuestionAsync(int questionId);
    Task<bool> ArchiveQuestionAsync(int questionId);
    
    // Question validation
    Task<QuestionValidationResult> ValidateQuestionAsync(ValidateQuestionDto dto);
    Task<bool> PublishQuestionAsync(int questionId);
    Task<bool> UnpublishQuestionAsync(int questionId);
    
    // Question duplicates and versions
    Task<QuestionDto> DuplicateQuestionAsync(int questionId, DuplicateQuestionDto dto);
    Task<IEnumerable<QuestionVersionDto>> GetQuestionVersionsAsync(int questionId);
    Task<QuestionDto> CreateQuestionVersionAsync(int questionId, CreateVersionDto dto);
    Task<bool> RestoreQuestionVersionAsync(int questionId, string version);
    
    // Question relationships
    Task<IEnumerable<QuestionDto>> GetRelatedQuestionsAsync(int questionId);
    Task<bool> LinkQuestionsAsync(int sourceQuestionId, int targetQuestionId, string relationshipType);
    Task<bool> UnlinkQuestionsAsync(int sourceQuestionId, int targetQuestionId);
    
    // Question media
    Task<bool> AttachMediaToQuestionAsync(int questionId, int mediaId);
    Task<bool> DetachMediaFromQuestionAsync(int questionId, int mediaId);
    Task<IEnumerable<MediaFileDto>> GetQuestionMediaAsync(int questionId);
    
    // Question analytics
    Task<QuestionUsageStatisticsDto> GetQuestionUsageAsync(int questionId);
    Task<QuestionPerformanceDto> GetQuestionPerformanceAsync(int questionId);
    Task<IEnumerable<QuestionDto>> GetSimilarQuestionsAsync(int questionId);
    
    // Bulk operations
    Task<BulkOperationResult> BulkCreateQuestionsAsync(IEnumerable<CreateQuestionDto> questions);
    Task<BulkOperationResult> BulkUpdateQuestionsAsync(BulkUpdateQuestionsDto dto);
    Task<BulkOperationResult> BulkDeleteQuestionsAsync(IEnumerable<int> questionIds);
    Task<BulkOperationResult> BulkTagQuestionsAsync(BulkTagDto dto);
}

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IQuestionBankRepository _questionBankRepository;
    private readonly IQuestionValidationService _validationService;
    private readonly IQuestionVersioningService _versioningService;
    private readonly IMediaService _mediaService;
    private readonly ITreeNodeService _treeNodeService;
    private readonly ILogger<QuestionService> _logger;
    private readonly IMapper _mapper;

    public QuestionService(
        IQuestionRepository questionRepository,
        IQuestionBankRepository questionBankRepository,
        IQuestionValidationService validationService,
        IQuestionVersioningService versioningService,
        IMediaService mediaService,
        ITreeNodeService treeNodeService,
        ILogger<QuestionService> logger,
        IMapper mapper)
    {
        _questionRepository = questionRepository;
        _questionBankRepository = questionBankRepository;
        _validationService = validationService;
        _versioningService = versioningService;
        _mediaService = mediaService;
        _treeNodeService = treeNodeService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto dto)
    {
        try
        {
            _logger.LogInformation("Creating new question: {Text}", dto.Text.Substring(0, Math.Min(50, dto.Text.Length)));

            // Validate question data
            var validationResult = await _validationService.ValidateQuestionDataAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Question validation failed: {string.Join(", ", validationResult.Errors)}");
            }

            // Validate tree node exists and is valid for questions
            if (dto.PrimaryTreeNodeId.HasValue)
            {
                var treeNode = await _treeNodeService.GetTreeNodeAsync(dto.PrimaryTreeNodeId.Value);
                if (treeNode == null || !treeNode.AllowQuestions)
                {
                    throw new ValidationException("Invalid tree node for question assignment");
                }
            }

            // Create question entity
            var question = new Question
            {
                Text = dto.Text,
                QuestionTypeId = dto.QuestionTypeId,
                DifficultyLevelId = dto.DifficultyLevelId,
                Solution = dto.Solution,
                EstimatedTimeSec = dto.EstimatedTimeSec,
                Points = dto.Points,
                QuestionStatusId = 1, // Draft status
                PrimaryTreeNodeId = dto.PrimaryTreeNodeId ?? 1, // Default tree node
                Version = "1.0",
                Tags = dto.Tags,
                Metadata = dto.MetadataJson,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Save question
            var savedQuestion = await _questionRepository.AddAsync(question);

            // Create initial version
            await _versioningService.CreateInitialVersionAsync(savedQuestion.Id, dto);

            // Process answers if provided
            if (dto.Answers?.Any() == true)
            {
                await ProcessQuestionAnswersAsync(savedQuestion.Id, dto.Answers);
            }

            // Attach media if provided
            if (dto.MediaIds?.Any() == true)
            {
                await AttachMediaToQuestionAsync(savedQuestion.Id, dto.MediaIds);
            }

            // Apply tags if provided
            if (dto.TagIds?.Any() == true)
            {
                await ApplyTagsToQuestionAsync(savedQuestion.Id, dto.TagIds);
            }

            // Update question bank statistics
            if (dto.QuestionBankId.HasValue)
            {
                await _questionBankRepository.UpdateStatisticsAsync(dto.QuestionBankId.Value);
            }

            _logger.LogInformation("Question created successfully with ID: {QuestionId}", savedQuestion.Id);

            // Return complete question DTO
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
            _logger.LogInformation("Retrieving question: {QuestionId}", questionId);

            var question = await _questionRepository.GetByIdWithDetailsAsync(questionId);
            if (question == null)
            {
                throw new NotFoundException($"Question with ID {questionId} not found");
            }

            var questionDto = _mapper.Map<QuestionDto>(question);

            // Enrich with additional data
            questionDto.Answers = await GetQuestionAnswersAsync(questionId);
            questionDto.Media = await GetQuestionMediaAsync(questionId);
            questionDto.Tags = await GetQuestionTagsAsync(questionId);
            questionDto.TreePath = await GetQuestionTreePathAsync(questionId);
            questionDto.UsageStatistics = await GetQuestionUsageAsync(questionId);

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
            _logger.LogInformation("Retrieving questions with filter: {Filter}", filter);

            // Build query based on filter
            var queryBuilder = new QuestionQueryBuilder()
                .WithQuestionBankId(filter.QuestionBankId)
                .WithQuestionTypeId(filter.QuestionTypeId)
                .WithDifficultyLevelId(filter.DifficultyLevelId)
                .WithStatusId(filter.StatusId)
                .WithTreeNodeId(filter.TreeNodeId)
                .WithTags(filter.Tags)
                .WithDateRange(filter.FromDate, filter.ToDate)
                .WithCreatedBy(filter.CreatedBy)
                .WithTextSearch(filter.SearchText)
                .WithSortBy(filter.SortBy, filter.SortDirection);

            var (questions, totalCount) = await _questionRepository.GetPagedAsync(
                filter.Page, 
                filter.PageSize, 
                queryBuilder.Build()
            );

            var questionDtos = new List<QuestionDto>();
            foreach (var question in questions)
            {
                var dto = _mapper.Map<QuestionDto>(question);
                
                // Add lightweight additional data for list view
                if (filter.IncludeAnswers)
                {
                    dto.AnswerCount = await _questionRepository.GetAnswerCountAsync(question.Id);
                }
                
                if (filter.IncludeMedia)
                {
                    dto.MediaCount = await _questionRepository.GetMediaCountAsync(question.Id);
                }
                
                if (filter.IncludeTags)
                {
                    dto.TagNames = await _questionRepository.GetTagNamesAsync(question.Id);
                }

                questionDtos.Add(dto);
            }

            return new PagedResult<QuestionDto>
            {
                Items = questionDtos,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
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
            _logger.LogInformation("Updating question: {QuestionId}", questionId);

            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
            {
                throw new NotFoundException($"Question with ID {questionId} not found");
            }

            // Check if user has permission to edit
            if (!await CanUserEditQuestionAsync(questionId, dto.UpdatedBy))
            {
                throw new UnauthorizedAccessException("User does not have permission to edit this question");
            }

            // Validate changes
            var validationResult = await _validationService.ValidateQuestionUpdateAsync(questionId, dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"Question update validation failed: {string.Join(", ", validationResult.Errors)}");
            }

            // Create new version if this is a significant change
            if (await _versioningService.RequiresNewVersionAsync(questionId, dto))
            {
                await _versioningService.CreateVersionFromUpdateAsync(questionId, dto);
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
                question.EstimatedTimeSec = dto.EstimatedTimeSec;
            
            if (dto.Points.HasValue)
                question.Points = dto.Points;
            
            if (dto.Tags != null)
                question.Tags = dto.Tags;
            
            if (dto.MetadataJson != null)
                question.Metadata = dto.MetadataJson;

            question.UpdatedAt = DateTime.UtcNow;
            question.UpdatedBy = dto.UpdatedBy;

            await _questionRepository.UpdateAsync(question);

            // Update answers if provided
            if (dto.Answers?.Any() == true)
            {
                await UpdateQuestionAnswersAsync(questionId, dto.Answers);
            }

            // Update media attachments if provided
            if (dto.MediaIds != null)
            {
                await UpdateQuestionMediaAsync(questionId, dto.MediaIds);
            }

            // Update tags if provided
            if (dto.TagIds != null)
            {
                await UpdateQuestionTagsAsync(questionId, dto.TagIds);
            }

            _logger.LogInformation("Question updated successfully: {QuestionId}", questionId);

            return await GetQuestionAsync(questionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question: {QuestionId}", questionId);
            throw;
        }
    }

    public async Task<QuestionValidationResult> ValidateQuestionAsync(ValidateQuestionDto dto)
    {
        try
        {
            _logger.LogInformation("Validating question: {QuestionId}", dto.QuestionId);

            var validationResult = new QuestionValidationResult
            {
                QuestionId = dto.QuestionId,
                IsValid = true,
                ValidationErrors = new List<ValidationError>(),
                ValidationWarnings = new List<ValidationWarning>(),
                Score = 100
            };

            // Content validation
            var contentValidation = await _validationService.ValidateContentAsync(dto);
            validationResult.ValidationErrors.AddRange(contentValidation.Errors);
            validationResult.ValidationWarnings.AddRange(contentValidation.Warnings);

            // Answer validation
            var answerValidation = await _validationService.ValidateAnswersAsync(dto);
            validationResult.ValidationErrors.AddRange(answerValidation.Errors);
            validationResult.ValidationWarnings.AddRange(answerValidation.Warnings);

            // Media validation
            var mediaValidation = await _validationService.ValidateMediaAsync(dto);
            validationResult.ValidationErrors.AddRange(mediaValidation.Errors);
            validationResult.ValidationWarnings.AddRange(mediaValidation.Warnings);

            // Business rules validation
            var businessValidation = await _validationService.ValidateBusinessRulesAsync(dto);
            validationResult.ValidationErrors.AddRange(businessValidation.Errors);
            validationResult.ValidationWarnings.AddRange(businessValidation.Warnings);

            // Calculate final validation score
            validationResult.Score = CalculateValidationScore(validationResult);
            validationResult.IsValid = validationResult.ValidationErrors.Count == 0;

            // Save validation result
            await SaveValidationResultAsync(validationResult);

            return validationResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating question: {QuestionId}", dto.QuestionId);
            throw;
        }
    }

    private async Task ProcessQuestionAnswersAsync(int questionId, IEnumerable<CreateAnswerDto> answers)
    {
        foreach (var answerDto in answers)
        {
            var answer = new Answer
            {
                QuestionId = questionId,
                Text = answerDto.Text,
                IsCorrect = answerDto.IsCorrect,
                Explanation = answerDto.Explanation,
                SortOrder = answerDto.SortOrder,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _questionRepository.AddAnswerAsync(answer);
        }
    }

    private async Task AttachMediaToQuestionAsync(int questionId, IEnumerable<int> mediaIds)
    {
        foreach (var mediaId in mediaIds)
        {
            await _questionRepository.AttachMediaAsync(questionId, mediaId);
        }
    }

    private async Task ApplyTagsToQuestionAsync(int questionId, IEnumerable<int> tagIds)
    {
        foreach (var tagId in tagIds)
        {
            await _questionRepository.ApplyTagAsync(questionId, tagId);
        }
    }

    private int CalculateValidationScore(QuestionValidationResult result)
    {
        var score = 100;
        
        // Deduct points for errors
        score -= result.ValidationErrors.Count * 10;
        
        // Deduct points for warnings
        score -= result.ValidationWarnings.Count * 5;
        
        return Math.Max(0, score);
    }
}
```

### Frontend Implementation Patterns

#### Question Manager Component
```typescript
// frontend/src/modules/question-bank/questions/components/QuestionManager.tsx
import React, { useState, useEffect, useCallback } from 'react';
import { Add, Search, Filter, Import, Export, Analytics } from '@mui/icons-material';
import { useQuestionManager } from '../hooks/useQuestionManager';
import { QuestionList } from './QuestionList';
import { QuestionEditor } from './QuestionEditor';
import { QuestionSearch } from './QuestionSearch';
import { QuestionFilters } from './QuestionFilters';
import { QuestionImporter } from './QuestionImporter';
import { QuestionAnalytics } from './QuestionAnalytics';
import { Tabs, Tab, Button, IconButton, Dialog } from '@mui/material';
import { QuestionDto, QuestionFilterDto, ViewMode } from '../types/question.types';

interface QuestionManagerProps {
  questionBankId?: number;
  treeNodeId?: number;
  readonly?: boolean;
  onQuestionSelect?: (question: QuestionDto) => void;
  className?: string;
}

export const QuestionManager: React.FC<QuestionManagerProps> = ({
  questionBankId,
  treeNodeId,
  readonly = false,
  onQuestionSelect,
  className
}) => {
  const [activeTab, setActiveTab] = useState(0);
  const [viewMode, setViewMode] = useState<ViewMode>('list');
  const [selectedQuestions, setSelectedQuestions] = useState<QuestionDto[]>([]);
  const [showEditor, setShowEditor] = useState(false);
  const [showImporter, setShowImporter] = useState(false);
  const [showAnalytics, setShowAnalytics] = useState(false);
  const [editingQuestion, setEditingQuestion] = useState<QuestionDto | null>(null);

  const {
    questions,
    isLoading,
    error,
    totalCount,
    currentFilter,
    
    refreshQuestions,
    createQuestion,
    updateQuestion,
    deleteQuestion,
    searchQuestions,
    filterQuestions,
    bulkDeleteQuestions,
    bulkUpdateQuestions,
    exportQuestions,
    getQuestionAnalytics
  } = useQuestionManager({
    questionBankId,
    treeNodeId
  });

  useEffect(() => {
    refreshQuestions();
  }, [refreshQuestions, questionBankId, treeNodeId]);

  const handleTabChange = useCallback((event: React.SyntheticEvent, newValue: number) => {
    setActiveTab(newValue);
  }, []);

  const handleCreateQuestion = useCallback(() => {
    setEditingQuestion(null);
    setShowEditor(true);
  }, []);

  const handleEditQuestion = useCallback((question: QuestionDto) => {
    setEditingQuestion(question);
    setShowEditor(true);
  }, []);

  const handleQuestionSaved = useCallback(async (questionData: any) => {
    try {
      if (editingQuestion) {
        await updateQuestion(editingQuestion.id, questionData);
      } else {
        await createQuestion({
          ...questionData,
          questionBankId,
          primaryTreeNodeId: treeNodeId
        });
      }
      await refreshQuestions();
      setShowEditor(false);
      setEditingQuestion(null);
    } catch (error) {
      console.error('Error saving question:', error);
    }
  }, [editingQuestion, updateQuestion, createQuestion, refreshQuestions, questionBankId, treeNodeId]);

  const handleQuestionSelect = useCallback((question: QuestionDto) => {
    if (onQuestionSelect) {
      onQuestionSelect(question);
    }
  }, [onQuestionSelect]);

  const handleFilterChange = useCallback(async (filter: QuestionFilterDto) => {
    await filterQuestions(filter);
  }, [filterQuestions]);

  const handleSearchQuery = useCallback(async (query: string) => {
    if (query.trim()) {
      await searchQuestions({ 
        searchText: query,
        includeAnswers: true,
        includeTags: true 
      });
    } else {
      await refreshQuestions();
    }
  }, [searchQuestions, refreshQuestions]);

  const handleBulkDelete = useCallback(async () => {
    if (selectedQuestions.length === 0) return;
    
    try {
      const questionIds = selectedQuestions.map(q => q.id);
      await bulkDeleteQuestions(questionIds);
      await refreshQuestions();
      setSelectedQuestions([]);
    } catch (error) {
      console.error('Error deleting questions:', error);
    }
  }, [selectedQuestions, bulkDeleteQuestions, refreshQuestions]);

  const handleExport = useCallback(async () => {
    try {
      const questionIds = selectedQuestions.length > 0 
        ? selectedQuestions.map(q => q.id)
        : questions.map(q => q.id);
      
      await exportQuestions(questionIds, {
        format: 'json',
        includeAnswers: true,
        includeMedia: true,
        includeTags: true
      });
    } catch (error) {
      console.error('Error exporting questions:', error);
    }
  }, [selectedQuestions, questions, exportQuestions]);

  const renderTabContent = useCallback(() => {
    switch (activeTab) {
      case 0: // Questions
        return (
          <QuestionList
            questions={questions}
            viewMode={viewMode}
            selectedQuestions={selectedQuestions}
            onQuestionSelect={handleQuestionSelect}
            onQuestionEdit={handleEditQuestion}
            onQuestionDelete={deleteQuestion}
            onSelectionChange={setSelectedQuestions}
            readonly={readonly}
            isLoading={isLoading}
          />
        );
      case 1: // Analytics
        return (
          <QuestionAnalytics
            questionBankId={questionBankId}
            treeNodeId={treeNodeId}
            selectedQuestions={selectedQuestions}
          />
        );
      default:
        return null;
    }
  }, [
    activeTab,
    questions,
    viewMode,
    selectedQuestions,
    handleQuestionSelect,
    handleEditQuestion,
    deleteQuestion,
    readonly,
    isLoading,
    questionBankId,
    treeNodeId
  ]);

  return (
    <div className={`question-manager ${className || ''}`}>
      {/* Header */}
      <div className="question-manager-header p-4 border-b">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold">
            Question Management
            {totalCount > 0 && <span className="text-gray-500 ml-2">({totalCount})</span>}
          </h2>
          
          <div className="flex items-center gap-2">
            {/* Action Buttons */}
            {!readonly && (
              <>
                <Button
                  variant="contained"
                  startIcon={<Add />}
                  onClick={handleCreateQuestion}
                >
                  Create Question
                </Button>
                <Button
                  variant="outlined"
                  startIcon={<Import />}
                  onClick={() => setShowImporter(true)}
                >
                  Import
                </Button>
                <Button
                  variant="outlined"
                  startIcon={<Export />}
                  onClick={handleExport}
                  disabled={questions.length === 0}
                >
                  Export
                </Button>
              </>
            )}
            
            <IconButton
              onClick={() => setShowAnalytics(true)}
              title="Analytics"
            >
              <Analytics />
            </IconButton>
          </div>
        </div>

        {/* Search and Filters */}
        <div className="flex items-center gap-4 mb-4">
          <div className="flex-1">
            <QuestionSearch
              onSearch={handleSearchQuery}
              placeholder="Search questions..."
            />
          </div>
          <QuestionFilters
            currentFilter={currentFilter}
            onFilterChange={handleFilterChange}
            questionBankId={questionBankId}
            treeNodeId={treeNodeId}
          />
        </div>

        {/* Bulk Actions */}
        {selectedQuestions.length > 0 && !readonly && (
          <div className="flex items-center gap-2 p-3 bg-blue-50 rounded">
            <span className="text-sm text-blue-800">
              {selectedQuestions.length} question{selectedQuestions.length !== 1 ? 's' : ''} selected
            </span>
            <div className="flex items-center gap-2 ml-auto">
              <Button
                size="small"
                variant="outlined"
                onClick={handleBulkDelete}
                color="error"
              >
                Delete Selected
              </Button>
              <Button
                size="small"
                variant="outlined"
                onClick={handleExport}
              >
                Export Selected
              </Button>
            </div>
          </div>
        )}
      </div>

      {/* Tabs */}
      <div className="question-manager-tabs">
        <Tabs value={activeTab} onChange={handleTabChange}>
          <Tab label="Questions" />
          <Tab label="Analytics" />
        </Tabs>
      </div>

      {/* Content */}
      <div className="question-manager-content flex-1 overflow-auto">
        {error && (
          <div className="p-4 bg-red-50 border border-red-200 rounded m-4">
            <p className="text-red-800">{error}</p>
            <Button
              variant="outlined"
              size="small"
              onClick={refreshQuestions}
              className="mt-2"
            >
              Retry
            </Button>
          </div>
        )}

        {renderTabContent()}
      </div>

      {/* Modals */}
      <Dialog
        open={showEditor}
        onClose={() => setShowEditor(false)}
        maxWidth="lg"
        fullWidth
      >
        <QuestionEditor
          question={editingQuestion}
          onSave={handleQuestionSaved}
          onCancel={() => setShowEditor(false)}
          questionBankId={questionBankId}
          treeNodeId={treeNodeId}
        />
      </Dialog>

      <Dialog
        open={showImporter}
        onClose={() => setShowImporter(false)}
        maxWidth="md"
        fullWidth
      >
        <QuestionImporter
          questionBankId={questionBankId}
          treeNodeId={treeNodeId}
          onImportComplete={() => {
            setShowImporter(false);
            refreshQuestions();
          }}
          onCancel={() => setShowImporter(false)}
        />
      </Dialog>

      <Dialog
        open={showAnalytics}
        onClose={() => setShowAnalytics(false)}
        maxWidth="xl"
        fullWidth
      >
        <QuestionAnalytics
          questionBankId={questionBankId}
          treeNodeId={treeNodeId}
          onClose={() => setShowAnalytics(false)}
        />
      </Dialog>
    </div>
  );
};
```

#### Question Editor Hook
```typescript
// frontend/src/modules/question-bank/questions/hooks/useQuestionCRUD.tsx
import { useState, useCallback } from 'react';
import { questionService } from '../services/questionService';
import { CreateQuestionDto, UpdateQuestionDto, QuestionDto } from '../types/question.types';

interface UseQuestionCRUDResult {
  isLoading: boolean;
  error: string | null;
  
  createQuestion: (dto: CreateQuestionDto) => Promise<QuestionDto>;
  updateQuestion: (id: number, dto: UpdateQuestionDto) => Promise<QuestionDto>;
  deleteQuestion: (id: number) => Promise<boolean>;
  duplicateQuestion: (id: number, dto: DuplicateQuestionDto) => Promise<QuestionDto>;
  validateQuestion: (dto: ValidateQuestionDto) => Promise<QuestionValidationResult>;
  publishQuestion: (id: number) => Promise<boolean>;
  clearError: () => void;
}

export const useQuestionCRUD = (): UseQuestionCRUDResult => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createQuestion = useCallback(async (dto: CreateQuestionDto): Promise<QuestionDto> => {
    try {
      setIsLoading(true);
      setError(null);
      
      const result = await questionService.createQuestion(dto);
      return result;
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to create question';
      setError(errorMessage);
      throw error;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const updateQuestion = useCallback(async (id: number, dto: UpdateQuestionDto): Promise<QuestionDto> => {
    try {
      setIsLoading(true);
      setError(null);
      
      const result = await questionService.updateQuestion(id, dto);
      return result;
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to update question';
      setError(errorMessage);
      throw error;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const deleteQuestion = useCallback(async (id: number): Promise<boolean> => {
    try {
      setIsLoading(true);
      setError(null);
      
      const result = await questionService.deleteQuestion(id);
      return result;
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to delete question';
      setError(errorMessage);
      throw error;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const duplicateQuestion = useCallback(async (id: number, dto: DuplicateQuestionDto): Promise<QuestionDto> => {
    try {
      setIsLoading(true);
      setError(null);
      
      const result = await questionService.duplicateQuestion(id, dto);
      return result;
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to duplicate question';
      setError(errorMessage);
      throw error;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const validateQuestion = useCallback(async (dto: ValidateQuestionDto): Promise<QuestionValidationResult> => {
    try {
      setIsLoading(true);
      setError(null);
      
      const result = await questionService.validateQuestion(dto);
      return result;
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to validate question';
      setError(errorMessage);
      throw error;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const publishQuestion = useCallback(async (id: number): Promise<boolean> => {
    try {
      setIsLoading(true);
      setError(null);
      
      const result = await questionService.publishQuestion(id);
      return result;
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to publish question';
      setError(errorMessage);
      throw error;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  return {
    isLoading,
    error,
    
    createQuestion,
    updateQuestion,
    deleteQuestion,
    duplicateQuestion,
    validateQuestion,
    publishQuestion,
    clearError
  };
};
```

## 🔄 Integration Validation Loops

### Backend Validation Loop
```bash
# 1. Entity Validation
dotnet ef migrations add AddQuestionManagementEntities
dotnet ef database update

# 2. Repository Testing
dotnet test --filter Category=QuestionRepository

# 3. Service Layer Testing
dotnet test --filter Category=QuestionService

# 4. Validation Service Testing
dotnet test --filter Category=QuestionValidationService

# 5. Controller Testing
dotnet test --filter Category=QuestionsController

# 6. Integration Testing
dotnet test --filter Category=QuestionIntegration

# 7. Performance Testing
dotnet test --filter Category=QuestionPerformance
```

### Frontend Validation Loop
```bash
# 1. Component Testing
npm run test -- --testPathPattern=questions

# 2. Hook Testing
npm run test -- --testPathPattern=useQuestion

# 3. Service Testing
npm run test -- --testPathPattern=questionService

# 4. Type Checking
npx tsc --noEmit --project tsconfig.json

# 5. Integration Testing
npm run test:integration -- --testPathPattern=questions

# 6. E2E Testing
npm run test:e2e -- --testPathPattern=question-management
```

### Question Operations Validation Loop
```bash
# 1. Question Creation Test
curl -X POST "http://localhost:5000/api/questions" \
  -H "Content-Type: application/json" \
  -d '{"text":"Sample question?","questionTypeId":1,"difficultyLevelId":1}'
# Expected: 201 Created with question data

# 2. Question Retrieval Test
curl -X GET "http://localhost:5000/api/questions/1"
# Expected: 200 OK with complete question data

# 3. Question Update Test
curl -X PUT "http://localhost:5000/api/questions/1" \
  -H "Content-Type: application/json" \
  -d '{"text":"Updated question?","points":5}'
# Expected: 200 OK with updated question data

# 4. Question Search Test
curl -X GET "http://localhost:5000/api/questions?searchText=sample&page=1&pageSize=10"
# Expected: 200 OK with paginated search results

# 5. Question Validation Test
curl -X POST "http://localhost:5000/api/questions/1/validate"
# Expected: 200 OK with validation result
```

## 🎯 Success Criteria

### Functional Requirements ✅
- [ ] Complete CRUD operations for questions and question banks
- [ ] Advanced search with filters (type, difficulty, tags, content)
- [ ] Question validation with comprehensive rule engine
- [ ] Version management with history tracking
- [ ] Bulk operations (create, update, delete, tag)
- [ ] Question import/export in multiple formats
- [ ] Template-based question creation
- [ ] Tag management and categorization
- [ ] Question analytics and usage tracking
- [ ] Media attachment and management
- [ ] Question duplication and variants

### Performance Requirements ✅
- [ ] Question list loading < 2 seconds for 1000 questions
- [ ] Question creation/update < 1 second
- [ ] Search results < 3 seconds for complex queries
- [ ] Bulk operations < 30 seconds for 100 questions
- [ ] Question validation < 5 seconds
- [ ] Import processing < 60 seconds for 500 questions
- [ ] Export generation < 30 seconds for 1000 questions

### Quality Requirements ✅
- [ ] Comprehensive validation rules engine
- [ ] Question content integrity checks
- [ ] Answer validation and consistency
- [ ] Media attachment validation
- [ ] Duplicate detection and prevention
- [ ] Version control with change tracking
- [ ] Audit logging for all operations

### Integration Requirements ✅
- [ ] Seamless integration with tree management system
- [ ] Media management system integration
- [ ] User management and permissions
- [ ] Question bank organization
- [ ] Exam integration readiness
- [ ] Analytics and reporting integration

## ⚠ Anti-Patterns to Avoid

### Backend Anti-Patterns ❌
```csharp
// ❌ DON'T: Loading unnecessary data
public async Task<QuestionDto> GetQuestion(int id)
{
    var question = await _context.Questions
        .Include(q => q.Answers)
        .Include(q => q.Media)
        .Include(q => q.Tags)
        .Include(q => q.Versions) // Loading all versions unnecessarily
        .Include(q => q.UsageHistory) // Heavy data for simple get
        .FirstOrDefaultAsync(q => q.Id == id);
}

// ❌ DON'T: Ignoring validation
public async Task<Question> CreateQuestion(CreateQuestionDto dto)
{
    // No validation - dangerous
    var question = new Question
    {
        Text = dto.Text // Could be empty, malicious, etc.
    };
    return await _repository.AddAsync(question);
}

// ❌ DON'T: Missing transaction management
public async Task UpdateQuestionWithAnswers(int questionId, UpdateQuestionDto dto)
{
    await _questionRepository.UpdateAsync(questionId, dto); // Could fail
    await _answerRepository.DeleteAllAsync(questionId); // Could fail
    await _answerRepository.AddRangeAsync(dto.Answers); // Could fail
    // No transaction - data inconsistency risk
}

// ❌ DON'T: Inefficient bulk operations
public async Task BulkDeleteQuestions(IEnumerable<int> questionIds)
{
    foreach (var id in questionIds) // N+1 problem
    {
        await _repository.DeleteAsync(id); // Individual database calls
    }
}
```

### Frontend Anti-Patterns ❌
```typescript
// ❌ DON'T: Rendering large lists without virtualization
const QuestionList = ({ questions }) => {
  return (
    <div>
      {questions.map(question => ( // Could be thousands of items
        <QuestionCard key={question.id} question={question} />
      ))}
    </div>
  );
};

// ❌ DON'T: Missing error boundaries
const QuestionEditor = () => {
  const [question, setQuestion] = useState(null);
  
  // If question creation fails, entire app could crash
  const handleSave = async () => {
    await questionService.create(question); // No error handling
  };
};

// ❌ DON'T: Inefficient re-renders
const QuestionManager = () => {
  const [questions, setQuestions] = useState([]);
  const [filters, setFilters] = useState({});
  
  // Re-fetches on every filter change, even minor ones
  useEffect(() => {
    fetchQuestions(filters);
  }, [filters]); // No debouncing
};

// ❌ DON'T: Memory leaks with large data
const QuestionSearch = () => {
  const [searchResults, setSearchResults] = useState([]);
  
  const handleSearch = async (query) => {
    const results = await searchQuestions(query);
    setSearchResults(prev => [...prev, ...results]); // Accumulating without cleanup
  };
};
```

### Architecture Anti-Patterns ❌
```csharp
// ❌ DON'T: Mixed responsibilities
public class QuestionService
{
    public async Task<Question> CreateQuestionAndSendEmail(CreateQuestionDto dto) 
    {
        var question = await CreateQuestion(dto); // Question responsibility
        await _emailService.SendNotification(dto.CreatedBy); // Email responsibility
        await _logService.LogActivity(dto.CreatedBy); // Logging responsibility
        return question;
    }
}

// ❌ DON'T: Tight coupling to UI concerns
public class QuestionController
{
    public async Task<IActionResult> GetQuestions(QuestionFilterDto filter)
    {
        var questions = await _questionService.GetQuestions(filter);
        
        // Controller shouldn't know about UI formatting
        foreach (var q in questions)
        {
            q.DisplayText = q.Text.Substring(0, 50) + "..."; // UI concern
        }
        
        return Ok(questions);
    }
}

// ❌ DON'T: Inconsistent error handling
public class QuestionValidationService
{
    public async Task<ValidationResult> ValidateQuestion(Question question)
    {
        if (string.IsNullOrEmpty(question.Text))
            throw new Exception("Text required"); // Inconsistent error type
        
        if (question.Answers.Count == 0)
            return new ValidationResult { IsValid = false }; // Different error handling
        
        if (question.DifficultyLevelId == 0)
            return null; // Another different approach
    }
}
```

## 📚 Implementation Guide

### Phase 1: Core Question Management (Week 1)
1. **Entity Implementation**
   - Create Question, QuestionBank, QuestionVersion entities
   - Configure Dapper relationships

2. **Repository Layer**
   - Implement question and question bank repositories
   - Add efficient querying and filtering methods

3. **Basic CRUD Services**
   - Create core question service with CRUD operations
   - Add basic validation and error handling

### Phase 2: Advanced Question Features (Week 2)
1. **Question Validation**
   - Implement comprehensive validation engine
   - Add business rules and content validation

2. **Version Management**
   - Add question versioning system
   - Implement version comparison and restoration

3. **Search and Filtering**
   - Build advanced search functionality
   - Add complex filtering capabilities

### Phase 3: Frontend Question Interface (Week 3)
1. **Core Components**
   - Build question manager component
   - Create question editor with rich features
   - Implement question list and grid views

2. **Advanced Features**
   - Add search and filtering interface
   - Implement bulk operations
   - Create import/export functionality

3. **Integration Features**
   - Connect with tree management system
   - Integrate media management
   - Add analytics dashboard

### Phase 4: Enterprise Features & Optimization (Week 4)
1. **Advanced Processing**
   - Implement question templates
   - Add AI-powered features
   - Create quality assessment tools

2. **Performance & Scalability**
   - Optimize database queries
   - Add caching strategies
   - Implement background processing

3. **Testing & Deployment**
   - Comprehensive testing suite
   - Performance optimization
   - Production deployment preparation

## 🛡️ Security Checklist
- [ ] Input validation and sanitization
- [ ] Permission-based access control
- [ ] Question content encryption for sensitive data
- [ ] Audit logging for all operations
- [ ] Rate limiting for API endpoints
- [ ] SQL injection prevention
- [ ] XSS protection in question content

## 📊 Performance Checklist
- [ ] Database indexing for search queries
- [ ] Pagination for large question lists
- [ ] Lazy loading for question details
- [ ] Caching for frequently accessed questions
- [ ] Background processing for bulk operations
- [ ] Query optimization for complex filters
- [ ] Memory management for large datasets

## 🌐 Integration Points

```yaml
DATABASE:
  - tables: "Questions, QuestionBanks, QuestionVersions, QuestionTags"
  - indexes: "IX_Question_Type, IX_Question_Status, IX_Question_TreeNode"
  - foreign_keys: "TreeNodes, Media, Users, QuestionTypes"

SERVICES:
  - tree_management: "Question categorization and organization"
  - media_management: "Question media attachments"
  - user_management: "Question ownership and permissions"

CONFIG:
  - backend: "Register question services and repositories"
  - frontend: "Configure question editor and validation"
  - search: "Configure search indexing and filtering"

ROUTES:
  - api: "/api/questions, /api/question-banks, /api/question-search"
  - frontend: "/question-bank/questions, /questions/editor"

FEATURES:
  - validation: "Real-time question validation"
  - versioning: "Question history and change tracking"
  - analytics: "Question usage and performance metrics"
  - import_export: "Bulk question management"

INTEGRATION:
  - exam_system: "Question selection for exams"
  - grading_system: "Question performance analytics"
  - reporting_system: "Question bank reports"
```

This comprehensive PRP provides a complete implementation guide for the question bank question management system that handles all aspects of question lifecycle management while maintaining high performance, security, and seamless integration with the existing system architecture.
