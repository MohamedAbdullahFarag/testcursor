# Product Requirements Prompt: Auto-Grading System

## 🎯 Goal
Implement a comprehensive automated grading system that accurately scores objective question types (multiple choice, true/false, matching, fill-in-the-blank, etc.) according to predefined answer keys and rubrics, providing immediate feedback and results while ensuring consistency and fairness in the assessment process.

## 🧠 Why
An automated grading system is essential because:
1. It significantly reduces the time and resources required for grading standardized questions
2. It ensures consistent and unbiased application of scoring rules across all submissions
3. It enables immediate feedback to students, enhancing the learning experience
4. It provides reliable data for analytics and reporting on student performance
5. It allows instructors to focus their time on grading subjective responses that require human judgment

## 📋 Context

### Current State
The Ikhtibar system has implemented exam creation and publishing workflows, but lacks the necessary components to automatically grade objective question types after student submissions. The auto-grading system needs to integrate with the existing exam session management and student submission components.

### Question Types to Support
1. **Multiple Choice** (single and multiple answers)
   - Exact matching with predefined correct answers
   - Partial credit for partially correct answers (when multiple selections allowed)

2. **True/False**
   - Binary matching with predefined correct answers

3. **Matching**
   - Pair matching with predefined correct pairings
   - Partial credit for partially correct pairings

4. **Fill-in-the-Blank**
   - Exact text matching
   - Case-sensitive/insensitive options
   - Pattern matching with regular expressions
   - Multiple acceptable answers

5. **Ordering**
   - Sequence matching with predefined correct order
   - Partial credit for partially correct sequences

6. **Numerical**
   - Exact value matching
   - Range-based matching with tolerances
   - Unit conversion support

### Backend Implementation Details
- Design a flexible auto-grading service that can evaluate different question types
- Implement scoring algorithms with support for partial credit
- Create validation rules for each question type
- Integrate with the exam session management system
- Store detailed grading results with explanations

### Frontend Implementation Details
- Display auto-grading results in the student's exam review
- Provide visual indicators for correct/incorrect answers
- Show detailed feedback for each question
- Support viewing of correct answers (when enabled)

### Key Files for Implementation
- Backend:
  - Core Services: `AutoGradingService`, `ScoringService`
  - Repositories: `StudentResponseRepository`, `GradingResultRepository`
  - Question Type Handlers: `MultipleChoiceHandler`, `TrueFalseHandler`, etc.

- Frontend:
  - Components: `AutoGradedQuestionResult`, `GradingSummary`
  - Hooks: `useAutoGradingResults`
  - Services: `autoGradingService`

## 🔨 Implementation Blueprint

### Task 1: Create Auto-Grading Core Models

```csharp
// Define grading result entity
public class GradingResultEntity : BaseEntity
{
    public Guid StudentResponseId { get; set; }
    public Guid QuestionId { get; set; }
    public double Score { get; set; }
    public double MaxScore { get; set; }
    public double Percentage { get; set; }
    public bool IsCorrect { get; set; }
    public bool IsPartiallyCorrect { get; set; }
    public string FeedbackText { get; set; }
    public string GradingMethod { get; set; } // "Auto" or "Manual"
    public Dictionary<string, object> GradingMetadata { get; set; }
    public DateTime GradedAt { get; set; }
}

// Define auto-grading configuration entity
public class AutoGradingConfigEntity : BaseEntity
{
    public Guid QuestionId { get; set; }
    public bool CaseSensitive { get; set; } // For text-based answers
    public double PartialCreditEnabled { get; set; }
    public double ToleranceValue { get; set; } // For numerical questions
    public string ToleranceType { get; set; } // "Absolute" or "Percentage"
    public bool AllowMultipleAnswers { get; set; } // For multiple choice
    public List<string> AcceptableAnswers { get; set; } // For fill-in-blank
    public Dictionary<string, object> GradingRules { get; set; } // Type-specific rules
}
```

### Task 2: Create Auto-Grading Service

```csharp
public interface IAutoGradingService
{
    Task<GradingResultDto> GradeResponseAsync(Guid studentResponseId);
    Task<List<GradingResultDto>> GradeExamSessionAsync(Guid examSessionId);
    Task<bool> ValidateAnswerKeyAsync(Guid questionId, object answerKey);
    Task<GradingSummaryDto> GetSessionGradingSummaryAsync(Guid examSessionId);
}

public class AutoGradingService : IAutoGradingService
{
    private readonly IStudentResponseRepository _studentResponseRepository;
    private readonly IGradingResultRepository _gradingResultRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly ILogger<AutoGradingService> _logger;
    private readonly IDictionary<QuestionType, IQuestionTypeGradingHandler> _gradingHandlers;
    
    public AutoGradingService(
        IStudentResponseRepository studentResponseRepository,
        IGradingResultRepository gradingResultRepository,
        IQuestionRepository questionRepository,
        ILogger<AutoGradingService> logger,
        IEnumerable<IQuestionTypeGradingHandler> gradingHandlers)
    {
        _studentResponseRepository = studentResponseRepository;
        _gradingResultRepository = gradingResultRepository;
        _questionRepository = questionRepository;
        _logger = logger;
        _gradingHandlers = gradingHandlers.ToDictionary(h => h.SupportedQuestionType);
    }
    
    public async Task<GradingResultDto> GradeResponseAsync(Guid studentResponseId)
    {
        try
        {
            using var scope = _logger.BeginScope("Grading student response {ResponseId}", studentResponseId);
            
            // Get student response
            var response = await _studentResponseRepository.GetByIdAsync(studentResponseId);
            if (response == null)
            {
                _logger.LogWarning("Student response not found");
                return null;
            }
            
            // Get question
            var question = await _questionRepository.GetByIdAsync(response.QuestionId);
            if (question == null)
            {
                _logger.LogWarning("Question not found");
                return null;
            }
            
            // Get grading handler for question type
            if (!_gradingHandlers.TryGetValue(question.QuestionType, out var handler))
            {
                _logger.LogWarning("No grading handler found for question type {QuestionType}", question.QuestionType);
                return null;
            }
            
            // Grade response
            var result = await handler.GradeAsync(question, response);
            
            // Save grading result
            await _gradingResultRepository.CreateAsync(result);
            
            return result.ToDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error grading student response {ResponseId}", studentResponseId);
            throw;
        }
    }
    
    // Implementation of other methods...
}
```

### Task 3: Create Question Type Grading Handlers

```csharp
public interface IQuestionTypeGradingHandler
{
    QuestionType SupportedQuestionType { get; }
    Task<GradingResultEntity> GradeAsync(QuestionEntity question, StudentResponseEntity response);
    Task<bool> ValidateAnswerKeyAsync(QuestionEntity question);
}

public class MultipleChoiceGradingHandler : IQuestionTypeGradingHandler
{
    public QuestionType SupportedQuestionType => QuestionType.MultipleChoice;
    
    public async Task<GradingResultEntity> GradeAsync(QuestionEntity question, StudentResponseEntity response)
    {
        var result = new GradingResultEntity
        {
            QuestionId = question.Id,
            StudentResponseId = response.Id,
            MaxScore = question.Points,
            GradingMethod = "Auto",
            GradedAt = DateTime.UtcNow,
            GradingMetadata = new Dictionary<string, object>()
        };
        
        try
        {
            // Parse question answer key
            var correctAnswers = JsonSerializer.Deserialize<List<string>>(question.AnswerKey);
            
            // Parse student response
            var studentAnswers = JsonSerializer.Deserialize<List<string>>(response.ResponseData);
            
            // Check if configuration allows partial credit
            var config = JsonSerializer.Deserialize<AutoGradingConfigEntity>(question.GradingConfig ?? "{}");
            bool allowPartial = config?.PartialCreditEnabled ?? false;
            
            // Calculate score
            if (correctAnswers.Count == 0 || studentAnswers.Count == 0)
            {
                result.Score = 0;
                result.Percentage = 0;
                result.IsCorrect = false;
                result.FeedbackText = "No answer provided or no correct answer defined";
                return result;
            }
            
            // For single answer (most common case)
            if (correctAnswers.Count == 1 && studentAnswers.Count == 1)
            {
                bool isCorrect = correctAnswers[0] == studentAnswers[0];
                result.Score = isCorrect ? question.Points : 0;
                result.Percentage = isCorrect ? 100 : 0;
                result.IsCorrect = isCorrect;
                result.FeedbackText = isCorrect ? "Correct answer" : "Incorrect answer";
                return result;
            }
            
            // For multiple answers with partial credit
            int correctCount = studentAnswers.Count(a => correctAnswers.Contains(a));
            int incorrectCount = studentAnswers.Count(a => !correctAnswers.Contains(a));
            
            if (allowPartial)
            {
                // Calculate partial credit
                double correctRatio = (double)correctCount / correctAnswers.Count;
                // Deduct for incorrect selections
                double penalty = incorrectCount > 0 ? (double)incorrectCount / studentAnswers.Count : 0;
                double finalRatio = Math.Max(0, correctRatio - penalty);
                
                result.Score = finalRatio * question.Points;
                result.Percentage = finalRatio * 100;
                result.IsCorrect = finalRatio >= 0.99; // Allow for floating point error
                result.IsPartiallyCorrect = finalRatio > 0 && finalRatio < 0.99;
                result.FeedbackText = GetFeedbackText(finalRatio);
            }
            else
            {
                // All or nothing scoring
                bool allCorrect = correctCount == correctAnswers.Count && incorrectCount == 0;
                result.Score = allCorrect ? question.Points : 0;
                result.Percentage = allCorrect ? 100 : 0;
                result.IsCorrect = allCorrect;
                result.FeedbackText = allCorrect ? "Correct answer" : "Incorrect answer";
            }
            
            result.GradingMetadata["CorrectAnswers"] = correctAnswers;
            result.GradingMetadata["StudentAnswers"] = studentAnswers;
            result.GradingMetadata["CorrectCount"] = correctCount;
            result.GradingMetadata["IncorrectCount"] = incorrectCount;
            
            return result;
        }
        catch (Exception ex)
        {
            result.Score = 0;
            result.Percentage = 0;
            result.IsCorrect = false;
            result.FeedbackText = "Error during grading: " + ex.Message;
            return result;
        }
    }
    
    private string GetFeedbackText(double ratio)
    {
        if (ratio >= 0.99) return "Correct answer";
        if (ratio >= 0.75) return "Mostly correct";
        if (ratio >= 0.5) return "Partially correct";
        if (ratio > 0) return "Mostly incorrect";
        return "Incorrect answer";
    }
    
    public async Task<bool> ValidateAnswerKeyAsync(QuestionEntity question)
    {
        try
        {
            var answerKey = JsonSerializer.Deserialize<List<string>>(question.AnswerKey);
            return answerKey != null && answerKey.Count > 0;
        }
        catch
        {
            return false;
        }
    }
}

// Similar implementations for other question types...
```

### Task 4: Create Grading Result Repository

```csharp
public interface IGradingResultRepository : IBaseRepository<GradingResultEntity>
{
    Task<List<GradingResultEntity>> GetByExamSessionIdAsync(Guid examSessionId);
    Task<GradingResultEntity> GetByStudentResponseIdAsync(Guid studentResponseId);
    Task<GradingSummaryDto> GetSessionSummaryAsync(Guid examSessionId);
}

public class GradingResultRepository : BaseRepository<GradingResultEntity>, IGradingResultRepository
{
    public GradingResultRepository(IDbContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<List<GradingResultEntity>> GetByExamSessionIdAsync(Guid examSessionId)
    {
        const string sql = @"
            SELECT gr.*
            FROM GradingResults gr
            JOIN StudentResponses sr ON gr.StudentResponseId = sr.Id
            WHERE sr.ExamSessionId = @ExamSessionId";
            
        return (await DbContext.QueryAsync<GradingResultEntity>(sql, new { ExamSessionId = examSessionId })).ToList();
    }
    
    public async Task<GradingResultEntity> GetByStudentResponseIdAsync(Guid studentResponseId)
    {
        const string sql = @"
            SELECT *
            FROM GradingResults
            WHERE StudentResponseId = @StudentResponseId";
            
        return await DbContext.QueryFirstOrDefaultAsync<GradingResultEntity>(sql, new { StudentResponseId = studentResponseId });
    }
    
    public async Task<GradingSummaryDto> GetSessionSummaryAsync(Guid examSessionId)
    {
        const string sql = @"
            SELECT 
                COUNT(gr.Id) AS TotalQuestions,
                SUM(gr.Score) AS TotalScore,
                SUM(gr.MaxScore) AS MaxPossibleScore,
                AVG(gr.Percentage) AS AveragePercentage,
                SUM(CASE WHEN gr.IsCorrect = 1 THEN 1 ELSE 0 END) AS CorrectAnswers,
                SUM(CASE WHEN gr.IsPartiallyCorrect = 1 THEN 1 ELSE 0 END) AS PartiallyCorrectAnswers,
                SUM(CASE WHEN gr.IsCorrect = 0 AND gr.IsPartiallyCorrect = 0 THEN 1 ELSE 0 END) AS IncorrectAnswers
            FROM GradingResults gr
            JOIN StudentResponses sr ON gr.StudentResponseId = sr.Id
            WHERE sr.ExamSessionId = @ExamSessionId";
            
        var summary = await DbContext.QueryFirstOrDefaultAsync<GradingSummaryDto>(sql, new { ExamSessionId = examSessionId });
        
        // Calculate percentage score
        if (summary != null && summary.MaxPossibleScore > 0)
        {
            summary.PercentageScore = (summary.TotalScore / summary.MaxPossibleScore) * 100;
        }
        
        return summary;
    }
}
```

### Task 5: Create API Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class AutoGradingController : ControllerBase
{
    private readonly IAutoGradingService _autoGradingService;
    private readonly ILogger<AutoGradingController> _logger;
    
    public AutoGradingController(
        IAutoGradingService autoGradingService,
        ILogger<AutoGradingController> logger)
    {
        _autoGradingService = autoGradingService;
        _logger = logger;
    }
    
    [HttpPost("response/{responseId}")]
    public async Task<ActionResult<GradingResultDto>> GradeResponse(Guid responseId)
    {
        try
        {
            var result = await _autoGradingService.GradeResponseAsync(responseId);
            if (result == null)
            {
                return NotFound("Student response not found or not gradable");
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error grading response {ResponseId}", responseId);
            return StatusCode(500, "An error occurred while grading the response");
        }
    }
    
    [HttpPost("session/{sessionId}")]
    public async Task<ActionResult<List<GradingResultDto>>> GradeExamSession(Guid sessionId)
    {
        try
        {
            var results = await _autoGradingService.GradeExamSessionAsync(sessionId);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error grading exam session {SessionId}", sessionId);
            return StatusCode(500, "An error occurred while grading the exam session");
        }
    }
    
    [HttpGet("summary/{sessionId}")]
    public async Task<ActionResult<GradingSummaryDto>> GetSessionSummary(Guid sessionId)
    {
        try
        {
            var summary = await _autoGradingService.GetSessionGradingSummaryAsync(sessionId);
            if (summary == null)
            {
                return NotFound("Exam session not found or not graded");
            }
            
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grading summary for session {SessionId}", sessionId);
            return StatusCode(500, "An error occurred while retrieving the grading summary");
        }
    }
}
```

### Task 6: Create Frontend Types and Services

```typescript
// Types
export interface GradingResult {
  id: string;
  questionId: string;
  score: number;
  maxScore: number;
  percentage: number;
  isCorrect: boolean;
  isPartiallyCorrect: boolean;
  feedbackText: string;
  gradingMethod: 'Auto' | 'Manual';
  gradingMetadata: Record<string, any>;
  gradedAt: string;
}

export interface GradingSummary {
  totalQuestions: number;
  totalScore: number;
  maxPossibleScore: number;
  percentageScore: number;
  averagePercentage: number;
  correctAnswers: number;
  partiallyCorrectAnswers: number;
  incorrectAnswers: number;
}

// Auto-grading service
export const autoGradingService = {
  // Grade a single response
  gradeResponse: async (responseId: string): Promise<GradingResult> => {
    const response = await axios.post<GradingResult>(
      `${API_BASE_URL}/api/AutoGrading/response/${responseId}`
    );
    return response.data;
  },
  
  // Grade an entire exam session
  gradeExamSession: async (sessionId: string): Promise<GradingResult[]> => {
    const response = await axios.post<GradingResult[]>(
      `${API_BASE_URL}/api/AutoGrading/session/${sessionId}`
    );
    return response.data;
  },
  
  // Get grading summary for an exam session
  getSessionSummary: async (sessionId: string): Promise<GradingSummary> => {
    const response = await axios.get<GradingSummary>(
      `${API_BASE_URL}/api/AutoGrading/summary/${sessionId}`
    );
    return response.data;
  }
};
```

### Task 7: Create Frontend Hooks

```typescript
// Hook for working with auto-graded results
export const useAutoGradingResults = (sessionId: string) => {
  const [results, setResults] = useState<GradingResult[]>([]);
  const [summary, setSummary] = useState<GradingSummary | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // Load results for an exam session
  const loadResults = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      
      // Get grading results
      const gradingResults = await autoGradingService.gradeExamSession(sessionId);
      setResults(gradingResults);
      
      // Get summary
      const gradingSummary = await autoGradingService.getSessionSummary(sessionId);
      setSummary(gradingSummary);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
      console.error('Error loading grading results:', err);
    } finally {
      setLoading(false);
    }
  }, [sessionId]);
  
  // Grade a single response
  const gradeResponse = useCallback(async (responseId: string) => {
    try {
      setLoading(true);
      setError(null);
      
      const result = await autoGradingService.gradeResponse(responseId);
      
      // Update results list
      setResults(prev => {
        const index = prev.findIndex(r => r.id === result.id);
        if (index >= 0) {
          // Update existing result
          const updated = [...prev];
          updated[index] = result;
          return updated;
        } else {
          // Add new result
          return [...prev, result];
        }
      });
      
      return result;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
      console.error('Error grading response:', err);
      return null;
    } finally {
      setLoading(false);
    }
  }, []);
  
  // Load results on mount
  useEffect(() => {
    if (sessionId) {
      loadResults();
    }
  }, [sessionId, loadResults]);
  
  return {
    results,
    summary,
    loading,
    error,
    loadResults,
    gradeResponse
  };
};
```

### Task 8: Create Frontend Components

```typescript
// Result item component
export const AutoGradedQuestionResult: React.FC<{
  result: GradingResult;
  showCorrectAnswer?: boolean;
}> = ({ result, showCorrectAnswer = false }) => {
  const { t } = useTranslation();
  
  // Get status class based on result
  const getStatusClass = () => {
    if (result.isCorrect) return 'bg-green-100 border-green-500';
    if (result.isPartiallyCorrect) return 'bg-yellow-100 border-yellow-500';
    return 'bg-red-100 border-red-500';
  };
  
  // Get status text
  const getStatusText = () => {
    if (result.isCorrect) return t('grading.status.correct');
    if (result.isPartiallyCorrect) return t('grading.status.partiallyCorrect');
    return t('grading.status.incorrect');
  };
  
  return (
    <div className={`border-l-4 p-4 mb-4 rounded ${getStatusClass()}`}>
      <div className="flex justify-between items-start">
        <div>
          <div className="text-lg font-semibold">
            {getStatusText()}
          </div>
          <div className="mt-2">
            {result.feedbackText}
          </div>
        </div>
        <div className="text-right">
          <div className="text-xl font-bold">
            {result.score}/{result.maxScore}
          </div>
          <div className="text-sm text-gray-600">
            {result.percentage.toFixed(1)}%
          </div>
        </div>
      </div>
      
      {showCorrectAnswer && result.gradingMetadata?.CorrectAnswers && (
        <div className="mt-3 pt-3 border-t border-gray-300">
          <div className="text-sm font-semibold text-gray-700">
            {t('grading.correctAnswer')}:
          </div>
          <div className="mt-1">
            {Array.isArray(result.gradingMetadata.CorrectAnswers) 
              ? result.gradingMetadata.CorrectAnswers.join(', ')
              : result.gradingMetadata.CorrectAnswers.toString()}
          </div>
        </div>
      )}
    </div>
  );
};

// Grading summary component
export const GradingSummaryComponent: React.FC<{
  summary: GradingSummary;
}> = ({ summary }) => {
  const { t } = useTranslation();
  
  // Determine grade status class
  const getScoreClass = () => {
    const score = summary.percentageScore;
    if (score >= 90) return 'text-green-600';
    if (score >= 70) return 'text-blue-600';
    if (score >= 50) return 'text-yellow-600';
    return 'text-red-600';
  };
  
  return (
    <div className="bg-white shadow rounded-lg p-6">
      <h2 className="text-2xl font-bold mb-4">
        {t('grading.summary.title')}
      </h2>
      
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div>
          <div className="text-4xl font-bold mb-2 flex items-end">
            <span className={getScoreClass()}>
              {summary.percentageScore.toFixed(1)}%
            </span>
            <span className="text-xl text-gray-500 ml-2">
              ({summary.totalScore}/{summary.maxPossibleScore})
            </span>
          </div>
          
          <div className="mt-4 grid grid-cols-2 gap-2">
            <div className="text-sm text-gray-600">
              {t('grading.summary.totalQuestions')}
            </div>
            <div className="text-sm font-semibold">
              {summary.totalQuestions}
            </div>
            
            <div className="text-sm text-gray-600">
              {t('grading.summary.correctAnswers')}
            </div>
            <div className="text-sm font-semibold text-green-600">
              {summary.correctAnswers} ({((summary.correctAnswers / summary.totalQuestions) * 100).toFixed(1)}%)
            </div>
            
            <div className="text-sm text-gray-600">
              {t('grading.summary.partiallyCorrect')}
            </div>
            <div className="text-sm font-semibold text-yellow-600">
              {summary.partiallyCorrectAnswers} ({((summary.partiallyCorrectAnswers / summary.totalQuestions) * 100).toFixed(1)}%)
            </div>
            
            <div className="text-sm text-gray-600">
              {t('grading.summary.incorrectAnswers')}
            </div>
            <div className="text-sm font-semibold text-red-600">
              {summary.incorrectAnswers} ({((summary.incorrectAnswers / summary.totalQuestions) * 100).toFixed(1)}%)
            </div>
          </div>
        </div>
        
        <div>
          <div className="h-36">
            {/* Visualization chart placeholder */}
            <canvas id="results-chart"></canvas>
          </div>
        </div>
      </div>
    </div>
  );
};
```

## 🧪 Validation Loop

### Backend Validation

```bash
# Build the backend to verify compilation
cd backend
dotnet build

# Run unit tests to verify auto-grading functionality
dotnet test Ikhtibar.Tests/Core/Services/AutoGradingServiceTests.cs
dotnet test Ikhtibar.Tests/Infrastructure/Repositories/GradingResultRepositoryTests.cs
dotnet test Ikhtibar.Tests/Api/Controllers/AutoGradingControllerTests.cs

# Test individual grading handlers
dotnet test Ikhtibar.Tests/Core/Services/MultipleChoiceGradingHandlerTests.cs
dotnet test Ikhtibar.Tests/Core/Services/TrueFalseGradingHandlerTests.cs
```

#### Integration Tests

```bash
# Test grading a single response
curl -X POST http://localhost:5000/api/AutoGrading/response/{responseId}
# Expected: GradingResult object with score, feedback, etc.

# Test grading an entire exam session
curl -X POST http://localhost:5000/api/AutoGrading/session/{sessionId}
# Expected: Array of GradingResult objects

# Test retrieving session summary
curl -X GET http://localhost:5000/api/AutoGrading/summary/{sessionId}
# Expected: GradingSummary object with totals, averages, etc.
```

### Frontend Validation

```bash
# Type checking
cd frontend
npm run type-check

# Lint check
npm run lint

# Unit tests
npm run test src/modules/grading/hooks/useAutoGradingResults.test.ts
npm run test src/modules/grading/components/AutoGradedQuestionResult.test.ts
npm run test src/modules/grading/components/GradingSummaryComponent.test.ts
```

## 📋 Acceptance Criteria

1. The auto-grading system must accurately grade all supported objective question types
2. The system must support partial credit for applicable question types (multiple choice with multiple answers, matching, ordering)
3. Performance must be acceptable (grading an entire exam session should take < 2 seconds for typical exams)
4. The system must store detailed grading results with explanations for each answer
5. The frontend must clearly display grading results with visual indicators for correct/incorrect answers
6. The system must handle edge cases gracefully (empty answers, invalid responses, etc.)
7. The grading process must be consistent and produce the same results for identical responses
8. Integration with the exam session management system must be seamless
9. The API must provide endpoints for grading individual responses and entire exam sessions
10. The system must generate summary statistics for exam sessions (total score, percentage, correct/incorrect counts)

## 🔄 Development Iterations

### Phase 1: Core Auto-Grading Engine
- Implement data models and base repositories
- Create core auto-grading service and interface
- Develop handler for multiple-choice questions
- Implement basic API endpoints
- Add unit tests for core functionality

### Phase 2: Additional Question Type Support
- Implement handlers for remaining question types
- Add specialized scoring algorithms for each type
- Enhance answer validation logic
- Add support for partial credit scoring
- Expand test coverage for all question types

### Phase 3: Frontend Integration
- Create frontend types and services
- Develop hooks for consuming auto-grading API
- Build result display components
- Implement summary visualization
- Add frontend validation and error handling

### Phase 4: Optimization and Enhancement
- Optimize performance for large exam sessions
- Add batch processing for multiple responses
- Implement caching strategies for frequently accessed results
- Enhance error handling and logging
- Add support for localized feedback messages

## 📚 Reference Materials
- Refer to section 3.4 in Ikhtibar features document for grading workflow details
- Review question type definitions in the question bank implementation
- Follow the folder-per-feature architecture described in COPILOT.md
- Implement validation-first development as per project guidelines
- Reference existing StudentResponse entities for response data structure
- Use the data model described in section 6.4 of the requirements document
