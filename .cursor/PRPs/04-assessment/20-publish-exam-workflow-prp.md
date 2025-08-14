# Product Requirements Prompt: Publish Exam Workflow

## 🎯 Goal
Implement a comprehensive exam publishing workflow system that manages the process from selecting an exam paper through configuration, student assignment, deployment, monitoring, and termination. The workflow should enforce proper state transitions, validation rules, and ensure exams are published with appropriate settings, notifications, and monitoring capabilities.

## 🧠 Why
A structured publish exam workflow ensures:
1. Consistent exam delivery process with proper validation at each stage
2. Clear scheduling and notification system for all stakeholders
3. Proper security and access controls during exam deployment
4. Comprehensive monitoring during exam sessions
5. Controlled termination and archival processes
6. Detailed audit trail of all actions in the publishing process

## 📋 Context

### Current State
The Ikhtibar system has detailed requirements for the publishing workflow but lacks implementation. The workflow needs to follow a state machine pattern with defined transitions, validation rules, and actions for each state of the publishing process.

### Workflow States
The publish exam workflow consists of these key states:
1. **PrePublicationReview** - Initial setup and verification
2. **ScheduledForPublication** - Configured and scheduled
3. **Published** - Active and available for students
4. **Suspended** - Temporarily halted (optional state)
5. **Unpublished** - Completed and archived

### Backend Implementation Details
- Create a workflow state machine in C# using a proper state pattern
- Implement validation rules for state transitions
- Implement notification system for status changes
- Track all actions in an audit log
- Implement authorization checks for different roles

### Frontend Implementation Details
- Create UI components to visualize the publishing workflow state
- Implement state transition actions with appropriate validations
- Provide dashboards for monitoring active exams
- Display appropriate UI based on current state and user permissions

### Key Files for Implementation

#### Backend
- New Entity: `ExamPublishWorkflow` to track the publish state and transitions
- Controller: `ExamPublishController.cs` for API endpoints
- Service: `ExamPublishService.cs` for business logic
- Repository: `ExamPublishRepository.cs` for data access

#### Frontend
- New Component: `ExamPublishStatus.tsx` for visual representation
- New Hook: `useExamPublish.ts` for state management
- New Service: `examPublishService.ts` for API integration
- New Component: `ExamMonitoringDashboard.tsx` for active exam monitoring

## 🔨 Implementation Blueprint

### Task 1: Create Publish Workflow State Machine
```csharp
// Define the state enum
public enum ExamPublishState
{
    PrePublicationReview,
    ScheduledForPublication,
    Published,
    Suspended,
    Unpublished
}

// Define the workflow entity
public class ExamPublishWorkflow
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public ExamPublishState CurrentState { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? ScheduledStartDate { get; set; }
    public DateTime? ScheduledEndDate { get; set; }
    public bool IsActive => CurrentState == ExamPublishState.Published;
    public List<ExamPublishTransition> Transitions { get; set; }
    public List<Student> AssignedStudents { get; set; }
}

// Define the transition entity for audit trail
public class ExamPublishTransition
{
    public Guid Id { get; set; }
    public Guid WorkflowId { get; set; }
    public ExamPublishState FromState { get; set; }
    public ExamPublishState ToState { get; set; }
    public string TransitionedBy { get; set; }
    public DateTime TransitionedAt { get; set; }
    public string Comments { get; set; }
    public string Reason { get; set; }
}
```

### Task 2: Create Backend Services and Controllers
```csharp
// Service interface
public interface IExamPublishService
{
    Task<ExamPublishWorkflowDto> GetExamPublishWorkflowAsync(Guid examId);
    Task<ExamPublishWorkflowDto> CreateWorkflowAsync(Guid examId, ExamPublishConfigDto configDto);
    Task<ExamPublishWorkflowDto> TransitionStateAsync(Guid examId, ExamPublishTransitionDto transitionDto);
    Task<ExamPublishWorkflowDto> AssignStudentsAsync(Guid examId, List<Guid> studentIds);
    Task<ExamPublishWorkflowDto> UpdateScheduleAsync(Guid examId, ExamScheduleDto scheduleDto);
    Task<List<ActiveExamSessionDto>> GetActiveSessionsAsync(Guid examId);
    Task<bool> TerminateExamAsync(Guid examId, ExamTerminationDto terminationDto);
    Task<IEnumerable<ExamPublishTransitionDto>> GetWorkflowHistoryAsync(Guid examId);
}

// Controller
[ApiController]
[Route("api/exams/{examId}/publish")]
[Authorize]
public class ExamPublishController : ControllerBase
{
    private readonly IExamPublishService _publishService;
    
    // GET: api/exams/{examId}/publish
    [HttpGet]
    [ProducesResponseType(typeof(ExamPublishWorkflowDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExamPublishWorkflowDto>> GetPublishWorkflow(Guid examId)
    
    // POST: api/exams/{examId}/publish
    [HttpPost]
    [ProducesResponseType(typeof(ExamPublishWorkflowDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ExamPublishWorkflowDto>> CreatePublishWorkflow(Guid examId, [FromBody] ExamPublishConfigDto configDto)
    
    // POST: api/exams/{examId}/publish/transitions
    [HttpPost("transitions")]
    [ProducesResponseType(typeof(ExamPublishWorkflowDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExamPublishWorkflowDto>> TransitionState(Guid examId, [FromBody] ExamPublishTransitionDto transitionDto)
    
    // POST: api/exams/{examId}/publish/students
    [HttpPost("students")]
    [ProducesResponseType(typeof(ExamPublishWorkflowDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExamPublishWorkflowDto>> AssignStudents(Guid examId, [FromBody] List<Guid> studentIds)
    
    // PUT: api/exams/{examId}/publish/schedule
    [HttpPut("schedule")]
    [ProducesResponseType(typeof(ExamPublishWorkflowDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExamPublishWorkflowDto>> UpdateSchedule(Guid examId, [FromBody] ExamScheduleDto scheduleDto)
    
    // GET: api/exams/{examId}/publish/sessions
    [HttpGet("sessions")]
    [ProducesResponseType(typeof(List<ActiveExamSessionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ActiveExamSessionDto>>> GetActiveSessions(Guid examId)
    
    // POST: api/exams/{examId}/publish/terminate
    [HttpPost("terminate")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> TerminateExam(Guid examId, [FromBody] ExamTerminationDto terminationDto)
    
    // GET: api/exams/{examId}/publish/history
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<ExamPublishTransitionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ExamPublishTransitionDto>>> GetHistory(Guid examId)
}
```

### Task 3: Create Frontend Types and Hooks
```typescript
// Workflow types
export enum ExamPublishState {
  PRE_PUBLICATION_REVIEW = 'PrePublicationReview',
  SCHEDULED_FOR_PUBLICATION = 'ScheduledForPublication',
  PUBLISHED = 'Published',
  SUSPENDED = 'Suspended',
  UNPUBLISHED = 'Unpublished'
}

export interface ExamPublishWorkflow {
  id: string;
  examId: string;
  currentState: ExamPublishState;
  createdAt: string;
  updatedAt: string | null;
  createdBy: string;
  updatedBy: string | null;
  scheduledStartDate: string | null;
  scheduledEndDate: string | null;
  isActive: boolean;
  assignedStudentCount: number;
  activeSessionCount: number;
}

export interface ExamPublishTransition {
  id: string;
  workflowId: string;
  fromState: ExamPublishState;
  toState: ExamPublishState;
  transitionedBy: string;
  transitionedAt: string;
  comments: string | null;
  reason: string | null;
}

export interface ExamSchedule {
  startDate: string;
  endDate: string;
  duration: number; // in minutes
  accessWindowDuration: number; // in minutes
}

export interface ExamTermination {
  reason: string;
  isImmediate: boolean;
  notifyStudents: boolean;
  comments: string;
}

// Workflow hook
export const useExamPublish = (examId: string) => {
  const [workflow, setWorkflow] = useState<ExamPublishWorkflow | null>(null);
  const [history, setHistory] = useState<ExamPublishTransition[]>([]);
  const [activeSessions, setActiveSessions] = useState<ActiveExamSession[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // Fetch workflow state
  const fetchWorkflow = async () => {
    // Implementation
  };
  
  // Create publish workflow
  const createPublishWorkflow = async (config: ExamPublishConfig) => {
    // Implementation
  };
  
  // Transition to new state
  const transitionState = async (toState: ExamPublishState, comments?: string, reason?: string) => {
    // Implementation with validation
  };
  
  // Assign students
  const assignStudents = async (studentIds: string[]) => {
    // Implementation
  };
  
  // Update schedule
  const updateSchedule = async (schedule: ExamSchedule) => {
    // Implementation
  };
  
  // Get active sessions
  const fetchActiveSessions = async () => {
    // Implementation
  };
  
  // Terminate exam
  const terminateExam = async (termination: ExamTermination) => {
    // Implementation
  };
  
  // Fetch workflow history
  const fetchHistory = async () => {
    // Implementation
  };
  
  return {
    workflow,
    history,
    activeSessions,
    loading,
    error,
    createPublishWorkflow,
    fetchWorkflow,
    transitionState,
    assignStudents,
    updateSchedule,
    fetchActiveSessions,
    terminateExam,
    fetchHistory,
    canTransitionTo: (state: ExamPublishState) => {
      // Validation logic based on current state and permissions
    }
  };
};
```

### Task 4: Create UI Components
```tsx
// Publish workflow status component
export const ExamPublishStatus: React.FC<{
  workflow: ExamPublishWorkflow;
  onTransition: (toState: ExamPublishState, comments?: string, reason?: string) => Promise<void>;
  canTransitionTo: (state: ExamPublishState) => boolean;
}> = ({ workflow, onTransition, canTransitionTo }) => {
  // Component implementation
};

// Exam monitoring dashboard
export const ExamMonitoringDashboard: React.FC<{
  examId: string;
  activeSessions: ActiveExamSession[];
  onTerminate: (termination: ExamTermination) => Promise<void>;
  onRefreshSessions: () => Promise<void>;
}> = ({ examId, activeSessions, onTerminate, onRefreshSessions }) => {
  // Component implementation
};
```

## 🧪 Validation Loop

### Backend Validation
```powershell
# Test the publish workflow creation
curl -X POST http://localhost:5000/api/exams/{examId}/publish \
  -H "Content-Type: application/json" \
  -d '{"examPaperId": "guid", "accessSettings": {"allowCalculator": true}}'
# Expected: {"id": "guid", "examId": "guid", "currentState": "PrePublicationReview", ...}

# Test state transition to scheduled
curl -X POST http://localhost:5000/api/exams/{examId}/publish/transitions \
  -H "Content-Type: application/json" \
  -d '{"toState": "ScheduledForPublication", "comments": "Ready for scheduling"}'
# Expected: {"id": "guid", "examId": "guid", "currentState": "ScheduledForPublication", ...}

# Test schedule update
curl -X PUT http://localhost:5000/api/exams/{examId}/publish/schedule \
  -H "Content-Type: application/json" \
  -d '{"startDate": "2023-12-01T09:00:00Z", "endDate": "2023-12-01T11:00:00Z", "duration": 120, "accessWindowDuration": 150}'
# Expected: Updated workflow object with schedule

# Test invalid state transition
curl -X POST http://localhost:5000/api/exams/{examId}/publish/transitions \
  -H "Content-Type: application/json" \
  -d '{"toState": "Unpublished", "comments": "Invalid transition from PrePublicationReview"}'
# Expected: 400 Bad Request with validation error

# Test exam termination
curl -X POST http://localhost:5000/api/exams/{examId}/publish/terminate \
  -H "Content-Type: application/json" \
  -d '{"reason": "Technical issues", "isImmediate": true, "notifyStudents": true, "comments": "System outage"}'
# Expected: true if successful
```

### Frontend Validation
```typescript
// Test proper rendering of workflow status
render(<ExamPublishStatus workflow={mockWorkflow} onTransition={mockTransition} canTransitionTo={mockCanTransitionTo} />);
expect(screen.getByText('Scheduled for Publication')).toBeInTheDocument();

// Test proper state transition buttons
const publishButton = screen.getByRole('button', { name: /publish exam/i });
expect(publishButton).toBeEnabled();
const unpublishButton = screen.getByRole('button', { name: /unpublish/i });
expect(unpublishButton).toBeDisabled();

// Test monitoring dashboard
render(<ExamMonitoringDashboard examId="exam-id" activeSessions={mockSessions} onTerminate={mockTerminate} onRefreshSessions={mockRefresh} />);
expect(screen.getByText(/3 active sessions/i)).toBeInTheDocument();
```

## 📋 Acceptance Criteria

1. The system must enforce proper state transitions in the publishing workflow
2. Invalid state transitions must be prevented with appropriate error messages
3. Exams must be properly configured before being published (timing, access settings, navigation rules)
4. Students must be assigned to exams before publishing
5. The system must allow scheduling of future exam sessions
6. Active exam sessions must be monitorable in real-time
7. The system must support both immediate and scheduled termination of exams
8. Each action in the publishing workflow must be recorded in the history with timestamp and user
9. The UI must visually indicate the current publish workflow state
10. The UI must only enable valid state transition actions based on current state and user permissions
11. Appropriate notifications must be sent to students and staff on key state transitions
12. Security settings must be properly enforced during the exam session
13. The system must support emergency termination of exams with proper logging
14. The workflow history must be viewable by authorized users
15. All workflow actions must be properly authorized based on user roles

## 🔄 Development Iterations

### Phase 1: Core Publishing Workflow
- Implement basic state machine for publish workflow
- Create database schema for workflow and transitions
- Implement core API endpoints for state management
- Implement authorization based on roles

### Phase 2: Configuration & Scheduling
- Add exam configuration settings
- Implement scheduling functionality
- Create student assignment functionality
- Add validation for required configurations

### Phase 3: Monitoring & Control
- Create active session monitoring
- Implement exam termination functionality
- Add notification system for state changes
- Implement security controls for exam access

### Phase 4: UI Implementation
- Create workflow status visualization
- Implement monitoring dashboard
- Add configuration forms with validation
- Create student assignment interface

### Phase 5: Integration & Testing
- Integrate with exam creation workflow
- Implement comprehensive logging
- Add performance monitoring for exam sessions
- Conduct end-to-end testing of publishing process

## 📚 Reference Materials
- Refer to section 3.3 in Ikhtibar features document for detailed publishing workflow
- Follow state machine patterns in existing codebase
- Use the folder-per-feature architecture described in COPILOT.md
- Implement validation-first development as per project guidelines
- Ensure proper integration with notification system
- Reference security best practices for exam access control
