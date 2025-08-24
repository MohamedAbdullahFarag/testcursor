# Manual Grading System PRP

## 🎯 Goal
Implement a comprehensive manual grading system for subjective questions that provides a rubric-based assessment interface, supports collaborative grading workflows, and ensures consistent evaluation standards across multiple graders.

## 🧠 Why
A robust manual grading system is essential because:
1. Subjective questions require human judgment for accurate assessment
2. Standardized rubrics ensure fair and consistent grading
3. Collaborative grading workflows enhance assessment quality
4. Detailed feedback improves the learning experience
5. Quality control measures ensure grading reliability

## 📋 Context

### Current State
- Auto-grading system handles objective questions
- Basic question and answer storage implemented
- Exam session management system in place
- User roles and permissions defined

### Grading Requirements
1. **Rubric Support**
   - Customizable scoring criteria
   - Multiple performance levels
   - Weighted scoring components
   - Feedback templates

2. **Grader Interface**
   - Side-by-side answer viewing
   - Rubric application tools
   - Comment and annotation features
   - Score calculation assistance

3. **Workflow Features**
   - Grader assignment management
   - Workload distribution
   - Progress tracking
   - Quality control reviews

4. **Quality Control**
   - Second grader reviews
   - Score moderation
   - Grading consistency checks
   - Appeal handling

## 🔨 Implementation Blueprint

### Task 1: Create Manual Grading Core Models

\`\`\`csharp
public class RubricEntity : BaseEntity
{
    public int QuestionId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<RubricCriterion> Criteria { get; set; }
    public double TotalPoints { get; set; }
    public bool RequiresSecondGrader { get; set; }
    public string Status { get; set; } // Draft, Active, Archived
}

public class RubricCriterion
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<PerformanceLevel> Levels { get; set; }
    public double Weight { get; set; }
    public double MaxPoints { get; set; }
}

public class PerformanceLevel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double Score { get; set; }
    public string Feedback { get; set; }
}

public class ManualGradingResultEntity : BaseEntity
{
    public int StudentResponseId { get; set; }
    public int RubricId { get; set; }
    public int GraderId { get; set; }
    public double Score { get; set; }
    public double MaxScore { get; set; }
    public Dictionary<string, double> CriteriaScores { get; set; }
    public string Comments { get; set; }
    public string Status { get; set; } // Pending, InProgress, Completed, UnderReview
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<GradingAnnotation> Annotations { get; set; }
    public bool RequiresModeration { get; set; }
}
\`\`\`

### Task 2: Create Manual Grading Service

\`\`\`csharp
public interface IManualGradingService
{
    Task<RubricDto> CreateRubricAsync(CreateRubricDto dto);
    Task<RubricDto> UpdateRubricAsync(int rubricId, UpdateRubricDto dto);
    Task<ManualGradingResultDto> StartGradingAsync(int responseId, int graderId);
    Task<ManualGradingResultDto> SaveGradingProgressAsync(int resultId, SaveGradingDto dto);
    Task<ManualGradingResultDto> CompleteGradingAsync(int resultId, CompleteGradingDto dto);
    Task<List<ManualGradingResultDto>> GetPendingGradingAsync(int graderId);
    Task<bool> AssignResponseToGraderAsync(int responseId, int graderId);
    Task<bool> RequestModerationAsync(int resultId, string reason);
}

public class ManualGradingService : IManualGradingService
{
    private readonly IManualGradingRepository _manualGradingRepository;
    private readonly IRubricRepository _rubricRepository;
    private readonly IStudentResponseRepository _studentResponseRepository;
    private readonly ILogger<ManualGradingService> _logger;

    // Implementation methods...
}
\`\`\`

### Task 3: Create Manual Grading Repository

\`\`\`csharp
public interface IManualGradingRepository : IBaseRepository<ManualGradingResultEntity>
{
    Task<List<ManualGradingResultEntity>> GetPendingByGraderIdAsync(int graderId);
    Task<ManualGradingResultEntity> GetByResponseIdAsync(int responseId);
    Task<List<ManualGradingResultEntity>> GetByExamSessionIdAsync(int sessionId);
    Task<GradingSummaryDto> GetGradingSummaryAsync(int sessionId);
}

public class ManualGradingRepository : BaseRepository<ManualGradingResultEntity>, IManualGradingRepository
{
    // Implementation methods...
}
\`\`\`

### Task 4: Create API Controllers

\`\`\`csharp
[ApiController]
[Route("api/[controller]")]
public class ManualGradingController : ControllerBase
{
    private readonly IManualGradingService _manualGradingService;
    private readonly ILogger<ManualGradingController> _logger;

    [HttpPost("rubrics")]
    public async Task<ActionResult<RubricDto>> CreateRubric(CreateRubricDto dto)
    {
        // Implementation...
    }

    [HttpPost("responses/{responseId}/start")]
    public async Task<ActionResult<ManualGradingResultDto>> StartGrading(int responseId)
    {
        // Implementation...
    }

    [HttpPost("results/{resultId}/save")]
    public async Task<ActionResult<ManualGradingResultDto>> SaveProgress(int resultId, SaveGradingDto dto)
    {
        // Implementation...
    }

    // Additional endpoints...
}
\`\`\`

### Task 5: Create Frontend Types and Services

\`\`\`typescript
// Types
export interface Rubric {
  id: string;
  questionId: string;
  name: string;
  description: string;
  criteria: RubricCriterion[];
  totalPoints: number;
  requiresSecondGrader: boolean;
  status: 'Draft' | 'Active' | 'Archived';
}

export interface ManualGradingResult {
  id: string;
  studentResponseId: string;
  rubricId: string;
  graderId: string;
  score: number;
  maxScore: number;
  criteriaScores: Record<string, number>;
  comments: string;
  status: 'Pending' | 'InProgress' | 'Completed' | 'UnderReview';
  startedAt: string | null;
  completedAt: string | null;
  annotations: GradingAnnotation[];
  requiresModeration: boolean;
}

// Service
export const manualGradingService = {
  createRubric: async (dto: CreateRubricDto): Promise<Rubric> => {
    const response = await axios.post<Rubric>('/api/ManualGrading/rubrics', dto);
    return response.data;
  },

  startGrading: async (responseId: string): Promise<ManualGradingResult> => {
    const response = await axios.post<ManualGradingResult>(
      \`/api/ManualGrading/responses/\${responseId}/start\`
    );
    return response.data;
  },

  // Additional methods...
};
\`\`\`

### Task 6: Create Frontend Hooks

\`\`\`typescript
export const useManualGrading = (responseId: string) => {
  const [result, setResult] = useState<ManualGradingResult | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const startGrading = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const result = await manualGradingService.startGrading(responseId);
      setResult(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
      console.error('Error starting grading:', err);
    } finally {
      setLoading(false);
    }
  }, [responseId]);

  // Additional functionality...

  return {
    result,
    loading,
    error,
    startGrading,
    // Additional methods...
  };
};
\`\`\`

### Task 7: Create Frontend Components

\`\`\`typescript
export const RubricGradingComponent: React.FC<{
  result: ManualGradingResult;
  rubric: Rubric;
  onSave: (scores: Record<string, number>, comments: string) => Promise<void>;
}> = ({ result, rubric, onSave }) => {
  const { t } = useTranslation();
  const [scores, setScores] = useState<Record<string, number>>({});
  const [comments, setComments] = useState('');
  
  // Render rubric criteria and scoring interface
  return (
    <div className="bg-white shadow-lg rounded-lg p-6">
      <h2 className="text-2xl font-bold mb-4">
        {rubric.name}
      </h2>
      
      {rubric.criteria.map(criterion => (
        <div key={criterion.name} className="mb-6 border-b pb-4">
          <h3 className="text-lg font-semibold mb-2">
            {criterion.name}
          </h3>
          <p className="text-gray-600 mb-4">
            {criterion.description}
          </p>
          
          <div className="grid grid-cols-1 gap-4">
            {criterion.levels.map(level => (
              <div
                key={level.name}
                className="flex items-center p-3 border rounded hover:bg-gray-50 cursor-pointer"
                onClick={() => {
                  setScores(prev => ({
                    ...prev,
                    [criterion.name]: level.score
                  }));
                }}
              >
                <RadioButton
                  checked={scores[criterion.name] === level.score}
                  onChange={() => {}}
                />
                <div className="ml-3">
                  <div className="font-medium">{level.name}</div>
                  <div className="text-sm text-gray-500">
                    {level.description}
                  </div>
                  <div className="text-sm font-semibold text-blue-600">
                    {level.score} / {criterion.maxPoints}
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      ))}
      
      <div className="mt-6">
        <label className="block text-sm font-medium text-gray-700 mb-2">
          {t('grading.comments')}
        </label>
        <textarea
          value={comments}
          onChange={e => setComments(e.target.value)}
          className="w-full h-32 p-2 border rounded"
        />
      </div>
      
      <div className="mt-6 flex justify-end space-x-4">
        <button
          type="button"
          className="px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 rounded"
          onClick={() => {/* Save as draft */}}
        >
          {t('common.saveAsDraft')}
        </button>
        <button
          type="button"
          className="px-4 py-2 text-sm text-white bg-blue-600 hover:bg-blue-700 rounded"
          onClick={() => onSave(scores, comments)}
        >
          {t('common.saveAndComplete')}
        </button>
      </div>
    </div>
  );
};
\`\`\`

## 🧪 Validation Loop

### Backend Validation

\`\`\`bash
# Build the backend
cd backend
dotnet build

# Run unit tests
dotnet test Ikhtibar.Tests/Core/Services/ManualGradingServiceTests.cs
dotnet test Ikhtibar.Tests/Infrastructure/Repositories/ManualGradingRepositoryTests.cs
dotnet test Ikhtibar.Tests/Api/Controllers/ManualGradingControllerTests.cs
\`\`\`

### Frontend Validation

\`\`\`bash
# Type checking
cd frontend
npm run type-check

# Lint check
npm run lint

# Unit tests
npm run test src/modules/grading/hooks/useManualGrading.test.ts
npm run test src/modules/grading/components/RubricGradingComponent.test.ts
\`\`\`

### Integration Testing

\`\`\`bash
# Test rubric creation
curl -X POST https://localhost:7001/api/ManualGrading/rubrics -d '{rubricData}'

# Test starting grading session
curl -X POST https://localhost:7001/api/ManualGrading/responses/{responseId}/start

# Test saving progress
curl -X POST https://localhost:7001/api/ManualGrading/results/{resultId}/save -d '{gradingData}'
\`\`\`

## 📋 Acceptance Criteria

1. **Rubric Management**
   - [x] Create and edit grading rubrics
   - [x] Define performance levels with descriptions
   - [x] Set scoring weights and criteria
   - [x] Support rubric versioning

2. **Grading Interface**
   - [x] Clean and intuitive UI for graders
   - [x] Side-by-side answer display
   - [x] Rubric application tools
   - [x] Comment and annotation features

3. **Workflow Support**
   - [x] Assign responses to graders
   - [x] Track grading progress
   - [x] Support second reviews
   - [x] Handle grading disputes

4. **Quality Control**
   - [x] Validate scoring consistency
   - [x] Support moderation workflow
   - [x] Track grader performance
   - [x] Generate quality reports

5. **Performance**
   - [x] Load responses quickly
   - [x] Save progress automatically
   - [x] Handle concurrent grading
   - [x] Scale for large response sets

## 🔄 Development Iterations

### Phase 1: Core Grading System
- Implement rubric data models
- Create basic grading interface
- Build core grading service
- Add response management

### Phase 2: Workflow Enhancement
- Add grader assignment
- Implement progress tracking
- Create review workflows
- Build moderation tools

### Phase 3: Quality Control
- Add scoring validation
- Implement consistency checks
- Create performance monitoring
- Build dispute resolution

### Phase 4: UI/UX Enhancement
- Optimize grading interface
- Add keyboard shortcuts
- Improve feedback tools
- Enhance progress visualization

## 📚 Reference Materials
- Section 3.4 in requirements document for grading workflow details
- Existing rubric schemas in question bank implementation
- Best practices for assessment in digital environments
- UI/UX guidelines for assessment tools
