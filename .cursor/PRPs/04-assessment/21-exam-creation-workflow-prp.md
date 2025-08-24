# Product Requirements Prompt: Exam Creation Workflow

## 🎯 Goal
Implement a comprehensive exam creation workflow system that manages the lifecycle of exam papers from initiation through quality review to storage in the paper bank. The workflow should handle state transitions with proper validation, maintain an audit trail, and ensure exams meet all quality standards before being made available for scheduling and deployment.

## 🧠 Why
A structured exam creation workflow ensures:
1. Consistency in exam quality and structure
2. Proper review processes for academic integrity
3. Complete metadata for searchability and alignment with learning outcomes
4. Comprehensive tracking of the exam creation lifecycle
5. Clear separation of responsibilities between creators and reviewers

## 📋 Context

### Current State
Currently, the Ikhtibar system has defined the exam creation workflow stages in the requirements but lacks implementation. The workflow needs to follow a state machine pattern with defined transitions, validation rules, and actions for each state.

### Workflow States
The exam creation workflow consists of these key states:
1. **Draft** - Initial creation state
2. **Review** - Under evaluation by reviewers
3. **Approved** - Passed review and ready for use
4. **Published** - Made available in exam scheduling
5. **Archived** - No longer active but preserved for records

### Backend Implementation Details
- Create a workflow state machine in C# using a proper state pattern
- Implement validation rules for state transitions
- Track state changes in an audit log
- Implement authorization checks for different roles

### Frontend Implementation Details
- Create UI components to visualize the workflow state
- Implement state transition actions with appropriate validations
- Display appropriate UI based on current state and user permissions

### Key Files for Implementation

#### Backend
- New Entity: `ExamWorkflow` to track the state and transitions
- Controller: `ExamWorkflowController.cs` for API endpoints
- Service: `ExamWorkflowService.cs` for business logic
- Repository: `ExamWorkflowRepository.cs` for data access

#### Frontend
- New Component: `ExamWorkflowStatus.tsx` for visual representation
- New Hook: `useExamWorkflow.ts` for state management
- New Service: `examWorkflowService.ts` for API integration

## 🔨 Implementation Blueprint

### Task 1: Create Workflow State Machine
```csharp
// Define the state enum
public enum ExamWorkflowState
{
    Draft,
    Review,
    Approved,
    Published,
    Archived
}

// Define the workflow entity
public class ExamWorkflow
{
    public int id { get; set; }
    public int ExamId { get; set; }
    public ExamWorkflowState CurrentState { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public List<ExamWorkflowTransition> Transitions { get; set; }
}

// Define the transition entity for audit trail
public class ExamWorkflowTransition
{
    public int id { get; set; }
    public int WorkflowId { get; set; }
    public ExamWorkflowState FromState { get; set; }
    public ExamWorkflowState ToState { get; set; }
    public string TransitionedBy { get; set; }
    public DateTime TransitionedAt { get; set; }
    public string Comments { get; set; }
}
```

### Task 2: Create Backend Services and Controllers
```csharp
// Service interface
public interface IExamWorkflowService
{
    Task<ExamWorkflowDto> GetExamWorkflowAsync(int examId);
    Task<ExamWorkflowDto> CreateWorkflowAsync(int examId);
    Task<ExamWorkflowDto> TransitionStateAsync(int examId, ExamWorkflowTransitionDto transitionDto);
    Task<IEnumerable<ExamWorkflowTransitionDto>> GetWorkflowHistoryAsync(int examId);
}

// Controller
[ApiController]
[Route("api/exams/{examId}/workflow")]
[Authorize]
public class ExamWorkflowController : ControllerBase
{
    private readonly IExamWorkflowService _workflowService;
    
    // GET: api/exams/{examId}/workflow
    [HttpGet]
    [ProducesResponseType(typeof(ExamWorkflowDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExamWorkflowDto>> GetWorkflow(int examId)
    
    // POST: api/exams/{examId}/workflow/transitions
    [HttpPost("transitions")]
    [ProducesResponseType(typeof(ExamWorkflowDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExamWorkflowDto>> TransitionState(int examId, [FromBody] ExamWorkflowTransitionDto transitionDto)
    
    // GET: api/exams/{examId}/workflow/history
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<ExamWorkflowTransitionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ExamWorkflowTransitionDto>>> GetHistory(int examId)
}
```

### Task 3: Create Frontend Components and Hooks
```typescript
// Workflow types
export enum ExamWorkflowState {
  DRAFT = 'Draft',
  REVIEW = 'Review',
  APPROVED = 'Approved',
  PUBLISHED = 'Published',
  ARCHIVED = 'Archived'
}

export interface ExamWorkflow {
  id: string;
  examId: string;
  currentState: ExamWorkflowState;
  createdAt: string;
  updatedAt: string | null;
  createdBy: string;
  updatedBy: string | null;
}

export interface ExamWorkflowTransition {
  id: string;
  workflowId: string;
  fromState: ExamWorkflowState;
  toState: ExamWorkflowState;
  transitionedBy: string;
  transitionedAt: string;
  comments: string | null;
}

// Workflow hook
export const useExamWorkflow = (examId: string) => {
  const [workflow, setWorkflow] = useState<ExamWorkflow | null>(null);
  const [history, setHistory] = useState<ExamWorkflowTransition[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // Fetch workflow state
  const fetchWorkflow = async () => {
    // Implementation
  };
  
  // Transition to new state
  const transitionState = async (toState: ExamWorkflowState, comments?: string) => {
    // Implementation
  };
  
  // Fetch workflow history
  const fetchHistory = async () => {
    // Implementation
  };
  
  return {
    workflow,
    history,
    loading,
    error,
    fetchWorkflow,
    transitionState,
    fetchHistory,
    canTransitionTo: (state: ExamWorkflowState) => {
      // Validation logic based on current state and permissions
    }
  };
};
```

### Task 4: Create UI Components
```tsx
export const ExamWorkflowStatus: React.FC<{
  workflow: ExamWorkflow;
  onTransition: (toState: ExamWorkflowState, comments?: string) => Promise<void>;
  canTransitionTo: (state: ExamWorkflowState) => boolean;
}> = ({ workflow, onTransition, canTransitionTo }) => {
  // Component implementation
};
```

## 🧪 Validation Loop

### Backend Validation
```powershell
# Test the state machine implementation
dotnet test backend/Ikhtibar.Tests/Api/ExamWorkflowControllerTests.cs

# Validate proper state transitions
curl -X POST https://localhost:7001/api/exams/{examId}/workflow/transitions \
  -H "Content-Type: application/json" \
  -d '{"toState": "Review", "comments": "Ready for review"}'
# Expected: {"id": "int", "examId": "int", "currentState": "Review", ...}

# Test invalid state transition
curl -X POST https://localhost:7001/api/exams/{examId}/workflow/transitions \
  -H "Content-Type: application/json" \
  -d '{"toState": "Published", "comments": "Invalid transition from Draft"}'
# Expected: 400 Bad Request with validation error
```

### Frontend Validation
```typescript
// Test proper rendering of workflow states
render(<ExamWorkflowStatus workflow={mockWorkflow} onTransition={mockTransition} canTransitionTo={mockCanTransitionTo} />);
expect(screen.getByText('Review')).toBeInTheDocument();

// Test proper state transition buttons
const reviewButton = screen.getByRole('button', { name: /submit for review/i });
expect(reviewButton).toBeEnabled();
const publishButton = screen.getByRole('button', { name: /publish/i });
expect(publishButton).toBeDisabled();
```

## 📋 Acceptance Criteria

1. The system must enforce proper state transitions (e.g., Draft → Review → Approved → Published → Archived)
2. Invalid state transitions must be prevented with appropriate error messages
3. Each transition must be recorded in the workflow history with timestamp and user
4. The UI must visually indicate the current workflow state
5. The UI must only enable valid state transition actions based on current state and user permissions
6. The workflow must integrate with the existing exam entity model
7. Appropriate notifications must be sent on key state transitions
8. The system must support adding comments when transitioning states
9. The workflow history must be viewable by authorized users
10. All workflow actions must be properly authorized based on user roles

## 🔄 Development Iterations

### Phase 1: Core Workflow Foundation
- Implement basic state machine pattern
- Create database schema for workflow and transitions
- Create basic API endpoints for state management
- Implement authorization based on roles

### Phase 2: UI Implementation
- Create workflow status visualization component
- Implement state transition UI with validation
- Add workflow history display
- Implement comment functionality for transitions

### Phase 3: Integration & Testing
- Integrate with exam management system
- Implement notification system for state changes
- Add comprehensive tests for all state transitions
- Add performance monitoring for workflow operations

## 📚 Reference Materials
- Refer to section 3.2 in Ikhtibar features document for workflow details
- Follow state machine patterns in existing codebase
- Use the folder-per-feature architecture described in COPILOT.md
- Implement validation-first development as per project guidelines
