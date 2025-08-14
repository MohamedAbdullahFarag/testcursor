# Grading Workflow System PRP

## 🎯 Goal
Implement a comprehensive grading workflow system that orchestrates both automated and manual grading processes, manages state transitions, and ensures consistent evaluation across different types of questions and grading methods.

## 🧠 Why
A robust grading workflow system is essential because:
1. Complex assessments require multiple grading stages
2. Different question types need different grading approaches
3. Quality control requires structured review processes
4. Grading progress needs to be tracked and managed
5. Results must be validated before finalization

## 📋 Context

### Current State
- Auto-grading system implemented for objective questions
- Manual grading system available for subjective questions
- Basic exam session management in place
- User roles and permissions defined

### Workflow Requirements
1. **State Management**
   - Clear workflow states
   - State transition rules
   - State validation logic
   - Progress tracking

2. **Process Orchestration**
   - Auto-grading integration
   - Manual grading coordination
   - Review process management
   - Results aggregation

3. **Quality Control**
   - Validation checkpoints
   - Review triggers
   - Moderation workflows
   - Consistency checks

4. **Progress Monitoring**
   - Status dashboards
   - Progress notifications
   - Bottleneck identification
   - Performance metrics

## 🔨 Implementation Blueprint

### Task 1: Create Workflow State Models

\`\`\`csharp
public class GradingWorkflowEntity : BaseEntity
{
    public Guid ExamSessionId { get; set; }
    public string Status { get; set; } // Created, AutoGrading, ManualGrading, Review, Finalized
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<GradingWorkflowStep> Steps { get; set; }
    public Dictionary<string, int> Progress { get; set; }
    public bool RequiresManualGrading { get; set; }
    public bool RequiresModeration { get; set; }
    public string CurrentPhase { get; set; }
}

public class GradingWorkflowStep
{
    public string StepType { get; set; } // Auto, Manual, Review, Moderation
    public string Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? AssignedTo { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

public class WorkflowTransitionEntity : BaseEntity
{
    public string FromState { get; set; }
    public string ToState { get; set; }
    public string TriggerType { get; set; }
    public List<string> RequiredRoles { get; set; }
    public bool RequiresApproval { get; set; }
    public List<string> ValidationRules { get; set; }
}
\`\`\`

### Task 2: Create Workflow Service

\`\`\`csharp
public interface IGradingWorkflowService
{
    Task<GradingWorkflowDto> InitializeWorkflowAsync(Guid examSessionId);
    Task<GradingWorkflowDto> TransitionStateAsync(Guid workflowId, string newState);
    Task<GradingWorkflowDto> AssignStepAsync(Guid workflowId, string stepType, Guid assigneeId);
    Task<GradingWorkflowDto> CompleteStepAsync(Guid workflowId, string stepType);
    Task<List<GradingWorkflowDto>> GetPendingWorkflowsAsync();
    Task<WorkflowProgressDto> GetWorkflowProgressAsync(Guid workflowId);
    Task<bool> ValidateTransitionAsync(Guid workflowId, string newState);
}

public class GradingWorkflowService : IGradingWorkflowService
{
    private readonly IGradingWorkflowRepository _workflowRepository;
    private readonly IExamSessionRepository _sessionRepository;
    private readonly IManualGradingService _manualGradingService;
    private readonly IAutoGradingService _autoGradingService;
    private readonly ILogger<GradingWorkflowService> _logger;

    // Implementation methods...
}
\`\`\`

### Task 3: Create Workflow Repository

\`\`\`csharp
public interface IGradingWorkflowRepository : IBaseRepository<GradingWorkflowEntity>
{
    Task<List<GradingWorkflowEntity>> GetPendingWorkflowsAsync();
    Task<List<GradingWorkflowEntity>> GetByExamSessionIdAsync(Guid sessionId);
    Task<WorkflowTransitionEntity> GetTransitionAsync(string fromState, string toState);
    Task<WorkflowProgressDto> GetWorkflowProgressAsync(Guid workflowId);
}

public class GradingWorkflowRepository : BaseRepository<GradingWorkflowEntity>, IGradingWorkflowRepository
{
    // Implementation methods...
}
\`\`\`

### Task 4: Create API Controllers

\`\`\`csharp
[ApiController]
[Route("api/[controller]")]
public class GradingWorkflowController : ControllerBase
{
    private readonly IGradingWorkflowService _workflowService;
    private readonly ILogger<GradingWorkflowController> _logger;

    [HttpPost("exams/{examSessionId}/workflow")]
    public async Task<ActionResult<GradingWorkflowDto>> InitializeWorkflow(Guid examSessionId)
    {
        // Implementation...
    }

    [HttpPost("workflows/{workflowId}/transition")]
    public async Task<ActionResult<GradingWorkflowDto>> TransitionState(
        Guid workflowId, 
        [FromBody] TransitionStateDto dto)
    {
        // Implementation...
    }

    [HttpGet("workflows/pending")]
    public async Task<ActionResult<List<GradingWorkflowDto>>> GetPendingWorkflows()
    {
        // Implementation...
    }

    // Additional endpoints...
}
\`\`\`

### Task 5: Create Frontend Types and Services

\`\`\`typescript
// Types
export interface GradingWorkflow {
  id: string;
  examSessionId: string;
  status: WorkflowStatus;
  startedAt: string;
  completedAt: string | null;
  steps: WorkflowStep[];
  progress: Record<string, number>;
  requiresManualGrading: boolean;
  requiresModeration: boolean;
  currentPhase: string;
}

export type WorkflowStatus = 
  | 'Created'
  | 'AutoGrading'
  | 'ManualGrading'
  | 'Review'
  | 'Finalized';

export interface WorkflowStep {
  stepType: 'Auto' | 'Manual' | 'Review' | 'Moderation';
  status: string;
  startedAt: string | null;
  completedAt: string | null;
  assignedTo: string | null;
  metadata: Record<string, any>;
}

// Service
export const gradingWorkflowService = {
  initializeWorkflow: async (examSessionId: string): Promise<GradingWorkflow> => {
    const response = await axios.post<GradingWorkflow>(
      \`/api/GradingWorkflow/exams/\${examSessionId}/workflow\`
    );
    return response.data;
  },

  transitionState: async (
    workflowId: string, 
    newState: WorkflowStatus
  ): Promise<GradingWorkflow> => {
    const response = await axios.post<GradingWorkflow>(
      \`/api/GradingWorkflow/workflows/\${workflowId}/transition\`,
      { newState }
    );
    return response.data;
  },

  // Additional methods...
};
\`\`\`

### Task 6: Create Frontend Hooks

\`\`\`typescript
export const useGradingWorkflow = (workflowId: string) => {
  const [workflow, setWorkflow] = useState<GradingWorkflow | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const refreshWorkflow = useCallback(async () => {
    try {
      setLoading(true);
      const response = await gradingWorkflowService.getWorkflow(workflowId);
      setWorkflow(response);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load workflow');
    } finally {
      setLoading(false);
    }
  }, [workflowId]);

  const transitionState = useCallback(async (newState: WorkflowStatus) => {
    try {
      setLoading(true);
      const updated = await gradingWorkflowService.transitionState(workflowId, newState);
      setWorkflow(updated);
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to transition state');
      return false;
    } finally {
      setLoading(false);
    }
  }, [workflowId]);

  // Additional hooks and functionality...

  return {
    workflow,
    loading,
    error,
    refreshWorkflow,
    transitionState,
  };
};
\`\`\`

### Task 7: Create Frontend Components

\`\`\`typescript
export const WorkflowProgressComponent: React.FC<{
  workflow: GradingWorkflow;
  onTransition: (newState: WorkflowStatus) => Promise<void>;
}> = ({ workflow, onTransition }) => {
  const { t } = useTranslation();
  
  return (
    <div className="bg-white shadow-lg rounded-lg p-6">
      <h2 className="text-2xl font-bold mb-4">
        {t('workflow.title')}
      </h2>
      
      <div className="space-y-8">
        {/* Progress Steps */}
        <div className="relative">
          {workflow.steps.map((step, index) => (
            <div
              key={step.stepType}
              className={clsx(
                'flex items-center',
                index !== workflow.steps.length - 1 && 'mb-4'
              )}
            >
              {/* Step Status Icon */}
              <div
                className={clsx(
                  'w-10 h-10 rounded-full flex items-center justify-center',
                  step.completedAt ? 'bg-green-500' : 'bg-gray-200'
                )}
              >
                <Icon
                  name={getStepIcon(step.stepType)}
                  className={clsx(
                    'w-6 h-6',
                    step.completedAt ? 'text-white' : 'text-gray-500'
                  )}
                />
              </div>
              
              {/* Step Details */}
              <div className="ml-4 flex-1">
                <h3 className="font-medium">
                  {t(\`workflow.steps.\${step.stepType}\`)}
                </h3>
                <p className="text-sm text-gray-500">
                  {getStepStatus(step, t)}
                </p>
              </div>
              
              {/* Action Button */}
              {canTransition(workflow, step) && (
                <button
                  type="button"
                  className="px-4 py-2 text-sm text-white bg-blue-600 
                           hover:bg-blue-700 rounded"
                  onClick={() => onTransition(getNextState(step.stepType))}
                >
                  {t('workflow.actions.proceed')}
                </button>
              )}
            </div>
          ))}
          
          {/* Progress Line */}
          <div className="absolute left-5 top-0 transform -translate-x-1/2 h-full">
            <div className="h-full w-0.5 bg-gray-200" />
          </div>
        </div>
        
        {/* Overall Progress */}
        <div className="mt-6">
          <h3 className="text-lg font-medium mb-2">
            {t('workflow.progress.title')}
          </h3>
          <div className="grid grid-cols-2 gap-4">
            {Object.entries(workflow.progress).map(([key, value]) => (
              <div key={key} className="bg-gray-50 p-4 rounded">
                <div className="text-sm text-gray-500">
                  {t(\`workflow.progress.\${key}\`)}
                </div>
                <div className="text-xl font-semibold">
                  {value}%
                </div>
              </div>
            ))}
          </div>
        </div>
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
dotnet test Ikhtibar.Tests/Core/Services/GradingWorkflowServiceTests.cs
dotnet test Ikhtibar.Tests/Infrastructure/Repositories/GradingWorkflowRepositoryTests.cs
dotnet test Ikhtibar.Tests/Api/Controllers/GradingWorkflowControllerTests.cs
\`\`\`

### Frontend Validation

\`\`\`bash
# Type checking
cd frontend
npm run type-check

# Lint check
npm run lint

# Unit tests
npm run test src/modules/grading/hooks/useGradingWorkflow.test.ts
npm run test src/modules/grading/components/WorkflowProgressComponent.test.ts
\`\`\`

### Integration Testing

\`\`\`bash
# Initialize workflow
curl -X POST http://localhost:5000/api/GradingWorkflow/exams/{examSessionId}/workflow

# Transition state
curl -X POST http://localhost:5000/api/GradingWorkflow/workflows/{workflowId}/transition -d '{
  "newState": "AutoGrading"
}'

# Get workflow progress
curl -X GET http://localhost:5000/api/GradingWorkflow/workflows/{workflowId}/progress
\`\`\`

## 📋 Acceptance Criteria

1. **Workflow States**
   - [x] Clear state definitions
   - [x] Valid state transitions
   - [x] State validation rules
   - [x] Progress tracking

2. **Process Management**
   - [x] Auto-grading integration
   - [x] Manual grading coordination
   - [x] Review process handling
   - [x] Status monitoring

3. **Quality Control**
   - [x] Validation checkpoints
   - [x] Moderation triggers
   - [x] Consistency checks
   - [x] Review workflows

4. **User Interface**
   - [x] Progress visualization
   - [x] Action controls
   - [x] Status updates
   - [x] Error handling

5. **Performance**
   - [x] Fast state transitions
   - [x] Efficient progress tracking
   - [x] Scalable workflow handling
   - [x] Concurrent workflow support

## 🔄 Development Iterations

### Phase 1: Core Workflow System
- Implement state models
- Create transition logic
- Build basic workflow service
- Add progress tracking

### Phase 2: Process Integration
- Connect auto-grading
- Integrate manual grading
- Add review workflows
- Build moderation system

### Phase 3: Quality Control
- Add validation rules
- Implement consistency checks
- Create review triggers
- Build monitoring tools

### Phase 4: UI/UX Enhancement
- Improve progress visualization
- Add action controls
- Enhance status updates
- Implement notifications

## 📚 Reference Materials
- Section 4.2 in requirements document for workflow specifications
- Existing auto-grading implementation details
- Manual grading system documentation
- Workflow state transition matrices
