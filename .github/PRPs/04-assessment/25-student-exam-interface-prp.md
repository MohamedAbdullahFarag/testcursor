# Student Exam Interface PRP

## Overview

This PRP defines the student-facing exam interface, providing a robust, accessible, and user-friendly experience for taking online exams while ensuring security and reliability.

## Core Features

### 1. Exam Access and Authentication
- Secure login and session management
- Access code validation
- Time zone handling
- Browser compatibility check
- Device requirement validation

### 2. Exam Navigation
- Question-by-question navigation
- Section navigation (if allowed)
- Progress tracking
- Time remaining display
- Auto-save functionality

### 3. Question Interface
- Multiple question type support
- Rich text formatting
- Media integration
- Answer input validation
- Response autosave

### 4. Exam Controls
- Start/resume exam
- Submit section/exam
- Save progress
- Request assistance
- Emergency contacts

## Implementation Blueprint

### 1. Data Models

```csharp
public class ExamSessionEntity : BaseEntity
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Status { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public string TimeZoneId { get; set; }
    public DateTime LastActivityTime { get; set; }
    public string BrowserInfo { get; set; }
    public string DeviceInfo { get; set; }
    public string IpAddress { get; set; }
}

public class ExamResponseEntity : BaseEntity
{
    public int SessionId { get; set; }
    public int QuestionId { get; set; }
    public string Response { get; set; }
    public DateTime ResponseTime { get; set; }
    public bool IsComplete { get; set; }
    public int TimeSpentSeconds { get; set; }
}

public class StudentExamStateEntity : BaseEntity
{
    public int SessionId { get; set; }
    public Dictionary<string, object> State { get; set; }
    public DateTime LastSaved { get; set; }
}
```

### 2. Service Layer

```csharp
public interface IExamSessionService
{
    Task<ExamSessionDto> InitiateSessionAsync(int examId, int studentId);
    Task<ExamSessionDto> ResumeSessionAsync(int sessionId);
    Task<bool> ValidateAccessAsync(int examId, int studentId, string accessCode);
    Task<bool> SaveResponseAsync(int sessionId, ExamResponseDto response);
    Task<bool> SubmitSectionAsync(int sessionId, int sectionId);
    Task<bool> SubmitExamAsync(int sessionId);
    Task<ExamStateDto> GetCurrentStateAsync(int sessionId);
    Task<bool> UpdateSessionStateAsync(int sessionId, ExamStateDto state);
}

public class ExamSessionService : IExamSessionService
{
    private readonly IExamSessionRepository _sessionRepository;
    private readonly IExamResponseRepository _responseRepository;
    private readonly IExamRepository _examRepository;
    private readonly ILogger<ExamSessionService> _logger;

    public async Task<ExamSessionDto> InitiateSessionAsync(int examId, int studentId)
    {
        using var scope = _logger.BeginScope("Initiating exam session for student {StudentId} exam {ExamId}",
            studentId, examId);

        try
        {
            // Validate exam availability
            if (!await ValidateExamAvailabilityAsync(examId))
            {
                throw new InvalidOperationException("Exam is not available");
            }

            // Check for existing sessions
            var existingSession = await _sessionRepository.GetActiveSessionAsync(examId, studentId);
            if (existingSession != null)
            {
                return await ResumeSessionAsync(existingSession.Id);
            }

            // Create new session
            var session = new ExamSessionEntity
            {
                ExamId = examId,
                StudentId = studentId,
                StartTime = DateTime.UtcNow,
                Status = "InProgress",
                TimeZoneId = TimeZoneInfo.Local.Id,
                LastActivityTime = DateTime.UtcNow
            };

            var createdSession = await _sessionRepository.AddAsync(session);
            return _mapper.Map<ExamSessionDto>(createdSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating exam session");
            throw;
        }
    }

    public async Task<bool> SaveResponseAsync(int sessionId, ExamResponseDto response)
    {
        using var scope = _logger.BeginScope("Saving response for session {SessionId}", sessionId);

        try
        {
            var responseEntity = _mapper.Map<ExamResponseEntity>(response);
            responseEntity.ResponseTime = DateTime.UtcNow;

            await _responseRepository.AddAsync(responseEntity);
            await UpdateLastActivityAsync(sessionId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving exam response");
            throw;
        }
    }
}
```

### 3. Frontend Components

```typescript
// Types
interface ExamSession {
  id: number;
  examId: number;
  startTime: string;
  endTime?: string;
  status: string;
  currentQuestionIndex: number;
  timeRemaining: number;
}

interface ExamState {
  session: ExamSession;
  currentQuestion: Question;
  responses: { [key: string]: string };
  navigation: NavigationState;
  timeTracking: TimeTrackingState;
}

// Components
const ExamInterface: React.FC = () => {
  const { examId } = useParams<{ examId: string }>();
  const { data: session } = useExamSession(examId);
  const { data: examState } = useExamState(session?.id);

  return (
    <div className="h-screen flex flex-col">
      <ExamHeader session={session} />
      <div className="flex-1 flex">
        <QuestionDisplay
          question={examState?.currentQuestion}
          response={examState?.responses[examState?.currentQuestion?.id]}
          onResponseChange={handleResponseChange}
        />
        <ExamNavigation
          state={examState?.navigation}
          onNavigate={handleNavigate}
        />
      </div>
      <ExamControls
        session={session}
        onSave={handleSave}
        onSubmit={handleSubmit}
      />
    </div>
  );
};

const ExamHeader: React.FC<{ session: ExamSession }> = ({ session }) => {
  const { t } = useTranslation();
  
  return (
    <header className="bg-white shadow-sm py-4 px-6">
      <div className="flex justify-between items-center">
        <h1 className="text-xl font-semibold">{t('exam.title')}</h1>
        <TimeRemaining
          startTime={session?.startTime}
          duration={session?.duration}
        />
      </div>
    </header>
  );
};

const QuestionDisplay: React.FC<QuestionDisplayProps> = ({
  question,
  response,
  onResponseChange
}) => {
  const { t } = useTranslation();

  return (
    <div className="flex-1 p-6">
      <div className="prose max-w-none">
        <QuestionContent content={question?.content} />
        <AnswerInput
          type={question?.type}
          value={response}
          onChange={onResponseChange}
        />
      </div>
    </div>
  );
};

const ExamNavigation: React.FC<NavigationProps> = ({
  state,
  onNavigate
}) => {
  const { t } = useTranslation();

  return (
    <nav className="w-64 bg-gray-50 p-4">
      <div className="space-y-4">
        <ProgressIndicator
          total={state?.totalQuestions}
          answered={state?.answeredCount}
        />
        <QuestionList
          questions={state?.questions}
          current={state?.currentIndex}
          onSelect={onNavigate}
        />
      </div>
    </nav>
  );
};
```

### 4. Real-time Features

```typescript
// Hooks
const useExamSession = (examId: string) => {
  const queryClient = useQueryClient();

  return useQuery({
    queryKey: ['examSession', examId],
    queryFn: () => examService.getSession(examId),
    refetchInterval: 30000, // Poll every 30 seconds
  });
};

const useAutoSave = (sessionId: string) => {
  const { mutate } = useMutation({
    mutationFn: examService.saveResponse,
    onError: (error) => {
      toast.error(t('exam.autosave.error'));
    }
  });

  useEffect(() => {
    const interval = setInterval(() => {
      mutate();
    }, 60000); // Auto-save every minute

    return () => clearInterval(interval);
  }, [mutate]);
};
```

### Integration Points

```yaml
DATABASE:
  - tables:
    - ExamSessions
    - ExamResponses
    - StudentExamState
  - indexes:
    - IX_ExamSessions_StudentId
    - IX_ExamSessions_Status
    - IX_ExamResponses_SessionId

CACHING:
  - keys:
    - ExamSession:{sessionId}
    - ExamState:{sessionId}
    - ExamResponses:{sessionId}
  - patterns:
    - Short TTL for active sessions
    - Longer TTL for completed sessions

SECURITY:
  - features:
    - Session timeout handling
    - Browser focus tracking
    - Copy/paste prevention
    - Screen capture prevention
    - Tab switching detection

MONITORING:
  - metrics:
    - Active sessions count
    - Response save rate
    - Error frequency
    - Network latency
  - alerts:
    - Session timeouts
    - Save failures
    - Suspicious activity

ACCESSIBILITY:
  - features:
    - Keyboard navigation
    - Screen reader support
    - High contrast mode
    - Font size adjustment
    - Reading time calculations
```

### Error Handling

```typescript
const handleResponseSave = async (response: ExamResponse) => {
  try {
    await saveResponse(response);
    setSaveStatus('saved');
  } catch (error) {
    setSaveStatus('error');
    
    if (error instanceof NetworkError) {
      storeInLocalBuffer(response);
      showRetryNotification();
    } else {
      showErrorNotification(error);
    }
  }
};

const handleConnectionLoss = () => {
  activateOfflineMode();
  startReconnectionAttempts();
  showOfflineNotification();
};
```

### Performance Optimizations

1. **Frontend**:
   - Question preloading
   - Response debouncing
   - State persistence
   - Resource caching
   - Image optimization

2. **Backend**:
   - Session data caching
   - Response batching
   - Async processing
   - Connection pooling
   - Query optimization

### Internationalization

```typescript
const examInterfaceTranslations = {
  en: {
    exam: {
      interface: {
        start: "Start Exam",
        submit: "Submit Exam",
        save: "Save Progress",
        next: "Next Question",
        previous: "Previous Question",
        timeRemaining: "Time Remaining: {{time}}",
        questionProgress: "Question {{current}} of {{total}}",
        confirmSubmit: "Are you sure you want to submit the exam?",
        saveStatus: {
          saving: "Saving...",
          saved: "All changes saved",
          error: "Error saving changes"
        }
      }
    }
  },
  ar: {
    exam: {
      interface: {
        start: "بدء الاختبار",
        submit: "تسليم الاختبار",
        save: "حفظ التقدم",
        next: "السؤال التالي",
        previous: "السؤال السابق",
        timeRemaining: "الوقت المتبقي: {{time}}",
        questionProgress: "السؤال {{current}} من {{total}}",
        confirmSubmit: "هل أنت متأكد من تسليم الاختبار؟",
        saveStatus: {
          saving: "جاري الحفظ...",
          saved: "تم حفظ جميع التغييرات",
          error: "خطأ في حفظ التغييرات"
        }
      }
    }
  }
};
```
