# Exam Publishing PRP

## Overview

This PRP defines the exam publishing feature, which handles the process of making approved exams available to students, managing exam schedules, and controlling access to exam content.

## Feature Requirements

### 1. Publishing Controls
- Publish/unpublish functionality
- Scheduled publishing
- Access control settings
- Student group assignments
- Time zone management
- Availability windows

### 2. Student Access
- Exam visibility rules
- Prerequisites checking
- Attempt management
- Time zone handling
- Access code distribution

### 3. Monitoring
- Publication status tracking
- Access logs
- Attempt tracking
- Real-time monitoring
- Proctor assignment

## Implementation Blueprint

### 1. Data Models

```csharp
// Entities
public class ExamPublicationEntity : BaseEntity
{
    public int ExamId { get; set; }
    public DateTime PublishedAt { get; set; }
    public DateTime? UnpublishedAt { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string TimeZoneId { get; set; }
    public bool RequiresAccessCode { get; set; }
    public string AccessCode { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; }
    public int PublishedById { get; set; }
}

public class ExamAccessEntity : BaseEntity
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public DateTime? FirstAccessedAt { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public int AttemptCount { get; set; }
    public bool IsBlocked { get; set; }
    public string BlockReason { get; set; }
}

public class ExamGroupAssignmentEntity : BaseEntity
{
    public int ExamId { get; set; }
    public int GroupId { get; set; }
    public DateTime AssignedAt { get; set; }
    public int AssignedById { get; set; }
}

// DTOs
public class PublishExamDto
{
    public int ExamId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string TimeZoneId { get; set; }
    public bool RequiresAccessCode { get; set; }
    public List<int> GroupIds { get; set; }
    public ExamAccessSettingsDto AccessSettings { get; set; }
}

public class ExamAccessSettingsDto
{
    public bool RequireProctor { get; set; }
    public bool AllowMultipleAttempts { get; set; }
    public int MaxAttempts { get; set; }
    public int MinTimeBetweenAttempts { get; set; }
    public bool RequirePrerequisites { get; set; }
    public List<int> PrerequisiteExamIds { get; set; }
}
```

### 2. Repository Layer

```csharp
public interface IExamPublicationRepository : IRepository<ExamPublicationEntity>
{
    Task<ExamPublicationEntity> GetActivePublicationAsync(int examId);
    Task<IEnumerable<ExamPublicationEntity>> GetUpcomingPublicationsAsync();
    Task<bool> HasActivePublicationAsync(int examId);
    Task<bool> UpdatePublicationStatusAsync(int examId, string status);
}

public class ExamPublicationRepository : BaseRepository<ExamPublicationEntity>, IExamPublicationRepository
{
    public async Task<ExamPublicationEntity> GetActivePublicationAsync(int examId)
    {
        const string sql = @"
            SELECT * FROM ExamPublications
            WHERE ExamId = @ExamId 
            AND IsActive = 1 
            AND IsDeleted = 0";

        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<ExamPublicationEntity>(sql, new { ExamId = examId });
    }

    public async Task<IEnumerable<ExamPublicationEntity>> GetUpcomingPublicationsAsync()
    {
        const string sql = @"
            SELECT * FROM ExamPublications
            WHERE IsActive = 1 
            AND IsDeleted = 0
            AND StartDate > GETUTCDATE()
            ORDER BY StartDate ASC";

        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<ExamPublicationEntity>(sql);
    }

    // Additional repository methods...
}
```

### 3. Service Layer

```csharp
public interface IExamPublicationService
{
    Task<ExamPublicationDto> PublishExamAsync(PublishExamDto publishDto);
    Task<bool> UnpublishExamAsync(int examId);
    Task<bool> ValidatePublicationAsync(int examId);
    Task<bool> IsExamAccessibleToStudentAsync(int examId, int studentId);
    Task<ExamAccessTokenDto> GenerateAccessTokenAsync(int examId, int studentId);
    Task<IEnumerable<ExamPublicationDto>> GetUpcomingPublicationsAsync();
}

public class ExamPublicationService : IExamPublicationService
{
    private readonly IExamPublicationRepository _publicationRepository;
    private readonly IExamRepository _examRepository;
    private readonly IExamAccessRepository _accessRepository;
    private readonly ILogger<ExamPublicationService> _logger;
    private readonly ICurrentUserService _currentUserService;

    public async Task<ExamPublicationDto> PublishExamAsync(PublishExamDto publishDto)
    {
        using var scope = _logger.BeginScope("Publishing exam {ExamId}", publishDto.ExamId);

        try
        {
            // Validate exam state
            if (!await ValidatePublicationAsync(publishDto.ExamId))
            {
                throw new InvalidOperationException("Exam is not in a publishable state");
            }

            // Create publication record
            var publication = new ExamPublicationEntity
            {
                ExamId = publishDto.ExamId,
                PublishedAt = DateTime.UtcNow,
                StartDate = publishDto.StartDate,
                EndDate = publishDto.EndDate,
                TimeZoneId = publishDto.TimeZoneId,
                RequiresAccessCode = publishDto.RequiresAccessCode,
                AccessCode = publishDto.RequiresAccessCode ? GenerateAccessCode() : null,
                IsActive = true,
                Status = "Published",
                PublishedById = _currentUserService.UserId
            };

            // Save publication
            var savedPublication = await _publicationRepository.AddAsync(publication);

            // Assign to groups
            if (publishDto.GroupIds?.Any() == true)
            {
                await AssignToGroupsAsync(publishDto.ExamId, publishDto.GroupIds);
            }

            // Update exam status
            await _examRepository.UpdateStatusAsync(publishDto.ExamId, "Published");

            var dto = _mapper.Map<ExamPublicationDto>(savedPublication);
            _logger.LogInformation("Exam {ExamId} published successfully", publishDto.ExamId);

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing exam {ExamId}", publishDto.ExamId);
            throw;
        }
    }

    public async Task<bool> ValidatePublicationAsync(int examId)
    {
        try
        {
            // Check exam exists and is approved
            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null || exam.Status != "Approved")
                return false;

            // Check no active publication exists
            if (await _publicationRepository.HasActivePublicationAsync(examId))
                return false;

            // Validate exam content
            if (!await ValidateExamContentAsync(examId))
                return false;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating exam {ExamId} for publication", examId);
            return false;
        }
    }

    private async Task<bool> ValidateExamContentAsync(int examId)
    {
        // Implement validation logic
        return true;
    }

    private string GenerateAccessCode()
    {
        // Implement secure access code generation
        return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
    }
}
```

### 4. Frontend Components

```typescript
// Types
interface ExamPublication {
  examId: number;
  publishedAt: string;
  startDate: string;
  endDate: string;
  timeZoneId: string;
  requiresAccessCode: boolean;
  accessCode?: string;
  isActive: boolean;
  status: string;
}

interface PublishExamRequest {
  examId: number;
  startDate: string;
  endDate: string;
  timeZoneId: string;
  requiresAccessCode: boolean;
  groupIds: number[];
  accessSettings: ExamAccessSettings;
}

// Components
const ExamPublishingForm: React.FC<{ examId: number }> = ({ examId }) => {
  const { t } = useTranslation();
  const queryClient = useQueryClient();
  const { mutate: publishExam } = usePublishExam();

  const handleSubmit = async (data: PublishExamFormData) => {
    await publishExam(data, {
      onSuccess: () => {
        queryClient.invalidateQueries(['exam', examId]);
        toast.success(t('exam.publish.success'));
      }
    });
  };

  return (
    <Form onSubmit={handleSubmit}>
      <DateTimeInput
        name="startDate"
        label={t('exam.publish.startDate')}
        rules={{ required: true }}
      />
      <DateTimeInput
        name="endDate"
        label={t('exam.publish.endDate')}
        rules={{ required: true }}
      />
      <TimeZoneSelect
        name="timeZoneId"
        label={t('exam.publish.timeZone')}
      />
      <GroupSelector
        name="groupIds"
        label={t('exam.publish.groups')}
        multiple
      />
      <AccessSettingsForm />
      <Button type="submit">
        {t('exam.publish.submit')}
      </Button>
    </Form>
  );
};

const ExamPublicationStatus: React.FC<{ examId: number }> = ({ examId }) => {
  const { data: publication } = useExamPublication(examId);

  if (!publication) return null;

  return (
    <div className="rounded-lg bg-white p-4 shadow">
      <h3 className="text-lg font-semibold">
        {t('exam.publish.status')}
      </h3>
      <div className="mt-2 space-y-2">
        <StatusBadge status={publication.status} />
        <DateTimeDisplay
          label={t('exam.publish.publishedAt')}
          value={publication.publishedAt}
        />
        <DateRangeDisplay
          startDate={publication.startDate}
          endDate={publication.endDate}
          timeZone={publication.timeZoneId}
        />
        {publication.requiresAccessCode && (
          <AccessCodeDisplay code={publication.accessCode} />
        )}
      </div>
    </div>
  );
};
```

### Integration Points

```yaml
DATABASE:
  - tables:
    - ExamPublications
    - ExamAccess
    - ExamGroupAssignments
  - indexes:
    - IX_ExamPublications_ExamId
    - IX_ExamPublications_StartDate
    - IX_ExamAccess_StudentId
    - IX_ExamGroupAssignments_GroupId

SECURITY:
  - roles:
    - ExamPublisher
    - ExamProctor
  - permissions:
    - PublishExam
    - UnpublishExam
    - ViewExamStatus
    - ManageExamAccess

MONITORING:
  - metrics:
    - ActivePublications
    - StudentAccess
    - AttemptCounts
  - alerts:
    - PublicationFailure
    - UnauthorizedAccess
    - HighAttemptRate

NOTIFICATIONS:
  - templates:
    - ExamPublished
    - AccessCodeDistribution
    - ExamReminder
    - ProctorAssignment
```

### Testing Strategy

```csharp
[TestFixture]
public class ExamPublicationTests
{
    [Test]
    public async Task PublishExam_WithValidData_ShouldSucceed()
    [Test]
    public async Task PublishExam_WithInvalidState_ShouldFail()
    [Test]
    public async Task ValidateAccess_WithValidCode_ShouldSucceed()
    [Test]
    public async Task ValidateAccess_WithExpiredCode_ShouldFail()
}
```

### Performance Considerations

1. **Caching**:
   - Cache publication status
   - Cache access tokens
   - Cache group assignments

2. **Optimization**:
   - Index publication dates
   - Optimize access checks
   - Batch group assignments

### Error Handling

```csharp
public async Task<IActionResult> PublishExam([FromBody] PublishExamDto publishDto)
{
    try
    {
        if (!await _examService.ValidatePublicationAsync(publishDto.ExamId))
        {
            return BadRequest("Exam is not in a publishable state");
        }

        var result = await _examService.PublishExamAsync(publishDto);
        return Ok(result);
    }
    catch (InvalidOperationException ex)
    {
        _logger.LogWarning(ex, "Invalid operation during exam publication");
        return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error publishing exam");
        return StatusCode(500, "An error occurred while publishing the exam");
    }
}
```

### Internationalization

```typescript
const publishingTranslations = {
  en: {
    exam: {
      publish: {
        title: "Publish Exam",
        startDate: "Start Date",
        endDate: "End Date",
        timeZone: "Time Zone",
        accessCode: "Access Code",
        groups: "Assign to Groups",
        submit: "Publish Exam",
        success: "Exam published successfully",
        error: "Error publishing exam"
      }
    }
  },
  ar: {
    exam: {
      publish: {
        title: "نشر الامتحان",
        startDate: "تاريخ البدء",
        endDate: "تاريخ الانتهاء",
        timeZone: "المنطقة الزمنية",
        accessCode: "رمز الوصول",
        groups: "تعيين للمجموعات",
        submit: "نشر الامتحان",
        success: "تم نشر الامتحان بنجاح",
        error: "خطأ في نشر الامتحان"
      }
    }
  }
};
```
