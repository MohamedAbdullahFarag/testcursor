# Exam Creation PRP

## Codebase Research Results

### Pattern Analysis

**Existing Features and Patterns**

The project follows a clear layered architecture with:
- ASP.NET Core 8.0 Web API backend using Dapper ORM
- React.js 18 frontend with TypeScript and Tailwind CSS
- Folder-per-feature architecture
- JWT-based authentication with role-based permissions
- i18next internationalization support for English and Arabic

### Technology Stack

- **Backend**: ASP.NET Core 8.0 with Dapper ORM
- **Frontend**: React 18, TypeScript, TanStack Query, Tailwind CSS
- **Architecture**: Folder-per-feature with clear separation of concerns
- **Database**: SQL Server with tables for exams, questions, and related entities
- **Auth**: JWT-based authentication with role-based permissions
- **Internationalization**: i18next with both English and Arabic support

## Exam Creation Feature Requirements

### Core Features

1. **Exam Metadata Management**:
   - Title in multiple languages (English/Arabic)
   - Description and instructions
   - Duration settings
   - Maximum attempts allowed
   - Passing score threshold
   - Start and end dates for availability
   - Time zone handling for global accessibility

2. **Question Selection and Organization**:
   - Select questions from question bank
   - Support for different question types
   - Group questions by sections/topics
   - Set points/weights for each question
   - Random question selection from pools
   - Question order randomization options

3. **Exam Settings**:
   - Navigation controls (allow/prevent backwards navigation)
   - Time limit settings and grace periods
   - Calculator and reference material permissions
   - Auto-save frequency configuration
   - Browser security settings
   - Proctor requirements

4. **Preview and Validation**:
   - Full exam preview mode
   - Question mix validation
   - Total points calculation
   - Time allocation review
   - Accessibility compliance check

### Implementation Blueprint

#### 1. Data Models

```csharp
// Entities
public class ExamEntity : BaseEntity
{
    public string TitleEn { get; set; }
    public string TitleAr { get; set; }
    public string DescriptionEn { get; set; }
    public string DescriptionAr { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxAttempts { get; set; }
    public decimal PassingScore { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string TimeZoneId { get; set; }
    public bool AllowBackNavigation { get; set; }
    public int GracePeriodMinutes { get; set; }
    public int AutoSaveIntervalSeconds { get; set; }
    public bool RequiresProctor { get; set; }
    public string Status { get; set; } // Draft, Published, Archived
}

public class ExamSectionEntity : BaseEntity
{
    public int ExamId { get; set; }
    public string TitleEn { get; set; }
    public string TitleAr { get; set; }
    public string InstructionsEn { get; set; }
    public string InstructionsAr { get; set; }
    public int OrderIndex { get; set; }
    public bool RandomizeQuestions { get; set; }
}

public class ExamQuestionEntity : BaseEntity
{
    public int ExamId { get; set; }
    public int SectionId { get; set; }
    public int QuestionId { get; set; }
    public decimal Points { get; set; }
    public int OrderIndex { get; set; }
    public bool IsRequired { get; set; }
}

// DTOs
public class CreateExamDto
{
    public string TitleEn { get; set; }
    public string TitleAr { get; set; }
    public string DescriptionEn { get; set; }
    public string DescriptionAr { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxAttempts { get; set; }
    public decimal PassingScore { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string TimeZoneId { get; set; }
    public ExamSettingsDto Settings { get; set; }
    public List<CreateExamSectionDto> Sections { get; set; }
}

public class ExamSettingsDto
{
    public bool AllowBackNavigation { get; set; }
    public int GracePeriodMinutes { get; set; }
    public int AutoSaveIntervalSeconds { get; set; }
    public bool RequiresProctor { get; set; }
    public bool AllowCalculator { get; set; }
    public bool RandomizeQuestions { get; set; }
}
```

#### 2. Repository Layer

```csharp
public interface IExamRepository : IRepository<ExamEntity>
{
    Task<ExamEntity> GetExamWithSectionsAsync(int examId);
    Task<IEnumerable<ExamQuestionEntity>> GetExamQuestionsAsync(int examId);
    Task<bool> UpdateExamSectionsAsync(int examId, List<ExamSectionEntity> sections);
    Task<bool> UpdateExamQuestionsAsync(int examId, List<ExamQuestionEntity> questions);
}

public class ExamRepository : BaseRepository<ExamEntity>, IExamRepository
{
    // Implementation details following BaseRepository pattern
}
```

#### 3. Service Layer

```csharp
public interface IExamService
{
    Task<ExamDto> CreateExamAsync(CreateExamDto createDto);
    Task<ExamDto> UpdateExamAsync(int examId, UpdateExamDto updateDto);
    Task<ExamDto> GetExamByIdAsync(int examId);
    Task<bool> DeleteExamAsync(int examId);
    Task<bool> ValidateExamAsync(int examId);
    Task<ExamPreviewDto> GetExamPreviewAsync(int examId);
}

public class ExamService : IExamService
{
    private readonly IExamRepository _examRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ExamService> _logger;

    // Implementation with proper error handling and business logic
}
```

#### 4. Frontend Components

```typescript
// Types
interface Exam {
  id: number;
  titleEn: string;
  titleAr: string;
  descriptionEn: string;
  descriptionAr: string;
  durationMinutes: number;
  maxAttempts: number;
  passingScore: number;
  startDate: string;
  endDate: string;
  timeZoneId: string;
  settings: ExamSettings;
  sections: ExamSection[];
}

// Components
const ExamForm: React.FC = () => {
  // Form implementation with proper validation and error handling
};

const ExamSectionManager: React.FC = () => {
  // Section management UI with drag-and-drop reordering
};

const QuestionSelector: React.FC = () => {
  // Question selection interface with search and filters
};
```

### Integration Points

```yaml
DATABASE:
  - tables:
      - Exams
      - ExamSections
      - ExamQuestions
      - ExamSettings
  - indexes:
      - IX_Exams_Status
      - IX_ExamQuestions_ExamId
      - IX_ExamSections_ExamId

CONFIG:
  - backend:
      - Add exam configuration section to appsettings.json
      - Configure auto-save intervals
  - frontend:
      - Add exam creation routes
      - Configure TanStack Query settings

ROUTES:
  - api:
      - POST /api/exams
      - PUT /api/exams/{id}
      - GET /api/exams/{id}
      - DELETE /api/exams/{id}
      - POST /api/exams/{id}/validate
      - GET /api/exams/{id}/preview
  
  - frontend:
      - /exam-management/create
      - /exam-management/edit/:id
      - /exam-management/preview/:id

SECURITY:
  - permissions:
      - ExamCreator
      - ExamManager
      - ExamReviewer
  - validations:
      - Input sanitization
      - CSRF protection
      - Role-based access control

MONITORING:
  - metrics:
      - Exam creation time
      - Question count per exam
      - Section count per exam
  - logging:
      - Exam creation events
      - Validation failures
      - Access attempts
</yaml>

### Testing Strategy

1. **Unit Tests**:
   ```csharp
   [Test]
   public async Task CreateExam_WithValidData_ShouldSucceed()
   [Test]
   public async Task UpdateExam_WithInvalidSection_ShouldFail()
   ```

2. **Integration Tests**:
   ```csharp
   [Test]
   public async Task ExamCreation_CompleteWorkflow_ShouldSucceed()
   ```

3. **Frontend Tests**:
   ```typescript
   describe('ExamForm', () => {
     it('should validate required fields')
     it('should handle section reordering')
   })
   ```

### Performance Considerations

1. **Backend**:
   - Optimize question loading with pagination
   - Cache frequently accessed question bank data
   - Use efficient bulk operations for question assignment

2. **Frontend**:
   - Implement virtual scrolling for large question lists
   - Optimize re-renders with proper memoization
   - Progressive loading of exam sections

### Error Handling

```csharp
try
{
    await _examService.CreateExamAsync(createDto);
}
catch (ValidationException ex)
{
    _logger.LogWarning(ex, "Exam validation failed");
    return BadRequest(new { message = ex.Message });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error creating exam");
    return StatusCode(500, "An error occurred while creating the exam");
}
```

### Internationalization

```typescript
// Translation keys
const examTranslations = {
  en: {
    exam: {
      create: {
        title: "Create New Exam",
        form: {
          titleLabel: "Exam Title",
          descriptionLabel: "Description",
          // ...
        }
      }
    }
  },
  ar: {
    exam: {
      create: {
        title: "إنشاء امتحان جديد",
        form: {
          titleLabel: "عنوان الامتحان",
          descriptionLabel: "الوصف",
          // ...
        }
      }
    }
  }
};
```
