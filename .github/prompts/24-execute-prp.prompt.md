`````prompt
````prompt
---
mode: agent
description: "Execute Product Requirements Prompts with comprehensive validation and GitHub Copilot tool integration"
---

---
inputs:
  - name: prp_file_path
    description: Path to the PRP file to execute (e.g., "PRPs/01-user-management/01-authentication-prp.md")
    required: true
    type: string
  - name: execution_mode
    description: Execution mode - full (complete implementation), incremental (MVP approach), or validation-only (validate existing)
    required: false
    type: string
    default: full
    enum: [full, incremental, validation-only]
  - name: start_phase
    description: Starting phase - planning, implementation, testing, or deployment
    required: false
    type: string
    default: planning
    enum: [planning, implementation, testing, deployment]
  - name: force_validation
    description: Force re-validation of all steps even if previously passed
    required: false
    type: boolean
    default: false
---

---
command: "/execute-prp"
---

# Execute PRP Command for GitHub Copilot

## Purpose
Implements features based on comprehensive Product Requirements Prompt (PRP) files in the Ikhtibar examination management system, following a structured multi-phase execution process with validation loops and quality gates using GitHub Copilot's native tools.

## Command Usage
```
@copilot /execute-prp <prp_file_path> [execution_mode] [start_phase] [force_validation]
```

## Core TypeScript Interfaces
```typescript
// Unified interface definitions for all execution modes
interface PRPExecutionContext {
  prpFile: string;
  mode: 'full' | 'incremental' | 'validation-only';
  startPhase: 'planning' | 'implementation' | 'testing' | 'deployment';
  forceValidation: boolean;
  projectContext: ProjectContext;
}

interface ProjectContext {
  architecture: 'clean-architecture';
  backend: 'dotnet-core-8';
  frontend: 'react-18-typescript';
  database: 'sql-server-dapper';
  i18n: ['english', 'arabic'];
}

interface Phase {
  id: string;
  name: string;
  tasks: Task[];
  validationCriteria: ValidationCriteria[];
  dependencies: string[];
  estimatedDuration: string;
}

interface Task {
  id: string;
  name: string;
  files: string[];
  toolActions: ToolAction[];
  validation: ValidationCommand[];
  dependencies: string[];
  srp: string; // Single Responsibility Principle validation
}

interface ValidationCommand {
  command: string;
  expected: string;
  tool: 'run_in_terminal' | 'get_errors' | 'run_tests';
  onFailure: string;
}

interface QualityGate {
  name: string;
  criteria: string[];
  threshold: number;
  commands: ValidationCommand[];
}

interface ImplementationPlan {
  phases: Phase[];
  totalTasks: number;
  estimatedDuration: string;
  dependencies: Dependency[];
  qualityGates: QualityGate[];
  riskAssessment: RiskLevel;
}

interface RiskAssessment {
  technicalRisks: Risk[];
  timelineRisks: Risk[];
  qualityRisks: Risk[];
  integrationRisks: Risk[];
  mitigationStrategies: MitigationStrategy[];
}

interface PRPComplianceAssessment {
  prpRequirements: string;
  implementationGaps: string;
  qualityScore: string;
  recommendations: string;
}
```

## Execution Workflow

### Phase 1: Context Discovery & Analysis
```markdown
I'll implement the feature defined in your PRP file. Starting with comprehensive context loading:

if [prp_file_path] is not a relative path to existing file find the full path in the repository and set [prp_file_path] to the correct path.

**1.1 Parse & Validate Request**
- PRP File: [PRP_FILE_PATH]
- Execution Mode: [EXECUTION_MODE] 
- Start Phase: [START_PHASE]
- Force Validation: [FORCE_VALIDATION]

**1.2 Status File Review** (GitHub Copilot Tools)
```typescript
const statusFileReview = async () => {
  // Check for existing status file
  const statusFilePath = `${PRP_FILE_PATH.replace('.md', '-status.md')}`;
  const prpStatusFiles = await file_search("**/*-status.md");
  
  try {
    const existingStatus = await read_file(statusFilePath);
    console.log("📊 Found existing status file - analyzing previous progress...");
    
    // Parse existing progress
    const progressMatch = existingStatus.match(/Completed.*?(\d+)\/(\d+).*?(\d+)%/);
    const currentPhaseMatch = existingStatus.match(/Phase.*?:\s*(.+)/);
    const qualityScoreMatch = existingStatus.match(/Quality Score.*?(\d+)\/10/);
    const issuesMatch = existingStatus.match(/Issues & Resolutions[\s\S]*?(?=##|$)/);
    
    return {
      exists: true,
      filePath: statusFilePath,
      progress: progressMatch ? {
        completed: parseInt(progressMatch[1]),
        total: parseInt(progressMatch[2]),
        percentage: parseInt(progressMatch[3])
      } : null,
      currentPhase: currentPhaseMatch ? currentPhaseMatch[1].trim() : null,
      qualityScore: qualityScoreMatch ? parseInt(qualityScoreMatch[1]) : null,
      hasIssues: issuesMatch && issuesMatch[0].includes('**Issue**'),
      content: existingStatus,
      lastUpdated: await getFileModificationTime(statusFilePath)
    };
  } catch (error) {
    console.log("📝 No existing status file found - will create new one");
    return {
      exists: false,
      filePath: statusFilePath,
      content: null
    };
  }
};
```

**1.3 Resume or Start Decision**
```typescript
const determineExecutionStrategy = (statusReview, startPhase, forceValidation) => {
  if (!statusReview.exists || forceValidation) {
    return {
      strategy: 'fresh-start',
      reason: statusReview.exists ? 'Force validation requested' : 'No previous progress found',
      startPhase: startPhase
    };
  }
  
  if (statusReview.progress?.percentage === 100) {
    return {
      strategy: 'validation-only',
      reason: 'Implementation appears complete - validating quality',
      startPhase: 'testing'
    };
  }
  
  if (statusReview.hasIssues) {
    return {
      strategy: 'issue-resolution',
      reason: 'Unresolved issues found - addressing before continuing',
      startPhase: statusReview.currentPhase || startPhase
    };
  }
  
  return {
    strategy: 'resume-progress',
    reason: `Resuming from ${statusReview.progress?.percentage || 0}% completion`,
    startPhase: statusReview.currentPhase || startPhase
  };
};
```

**1.4 PRP Context Discovery** (GitHub Copilot Tools)
**1.4 PRP Context Discovery** (GitHub Copilot Tools)
```typescript
const contextDiscovery = async () => {
  // Core PRP analysis
  const prpContent = await read_file(PRP_FILE_PATH);
  const relatedPRPs = await file_search("PRPs/**/*.md");
  const existingPatterns = await semantic_search("PRP implementation patterns");
  
  // Project state analysis
  const moduleStructure = await list_dir("src/modules/");
  const backendServices = await file_search("backend/**/*.cs");
  const frontendComponents = await file_search("src/**/*.tsx");
  
  // Implementation structure extraction
  const prpStructure = await grep_search("Phase|Task|Validation", {includePattern: PRP_FILE_PATH});
  const dependencies = await semantic_search("project dependencies");
  
  return {prpContent, relatedPRPs, existingPatterns, moduleStructure, prpStructure};
};
```

**1.5 PRP Completeness Validation**
```yaml
Required Sections Validation:
  - Goal: Clear implementation objective ✓/✗
  - Context: Complete codebase analysis ✓/✗
  - Blueprint: Detailed implementation plan ✓/✗
  - Validation: Executable test commands ✓/✗
  - Integration: Configuration updates ✓/✗
  - Quality Gates: Acceptance criteria (min 8/10) ✓/✗
  - Anti-Patterns: Project-specific examples ✓/✗
  - File Structure: Complete organization plan ✓/✗
  - i18n: English/Arabic support ✓/✗
  - Performance: Optimization strategies ✓/✗

Analysis Results:
  - Feature Scope: [DESCRIPTION]
  - Phases: [COUNT] identified
  - Tasks: [COUNT] total
  - Dependencies: [LIST]
  - Quality Gates: [COUNT] validation points
  - Success Criteria: [METRICS]
```
```

### Phase 2: Implementation Planning by Mode

#### Mode: `full` - Complete Implementation
```typescript
const fullImplementationPlan = {
  statusFileManagement: {
    initialize: async (statusReview) => {
      if (statusReview.exists && !forceValidation) {
        // Update existing status file with resume information
        await insert_edit_into_file(statusReview.filePath, 
          `## Execution Resume\n- **Resumed**: ${new Date().toISOString()}\n- **Strategy**: ${executionStrategy.strategy}\n- **Reason**: ${executionStrategy.reason}\n\n`
        );
      } else {
        // Create new status file
        await create_file(statusReview.filePath, generateInitialStatusContent());
      }
    },
    
    updateProgress: async (phase, task, validation) => {
      const progressUpdate = `
## Phase Progress Update: ${phase.name}
- **Task Completed**: ${task.name}
- **Timestamp**: ${new Date().toISOString()}
- **Validation**: ${validation.passed ? '✅ PASSED' : '❌ FAILED'}
- **Files Modified**: ${task.files.join(', ')}
${validation.errors ? `- **Errors**: ${validation.errors.join(', ')}` : ''}

`;
      await insert_edit_into_file(statusReview.filePath, progressUpdate);
    },
    
    updateQualityGate: async (gateName, results) => {
      const gateUpdate = `
## Quality Gate: ${gateName}
- **Timestamp**: ${new Date().toISOString()}
- **Status**: ${results.passed ? '✅ PASSED' : '❌ FAILED'}
- **Score**: ${results.score}/10
- **Details**: ${results.details}

`;
      await replace_string_in_file(statusReview.filePath, 
        /## Quality Validation[\s\S]*?(?=##|$)/, 
        `## Quality Validation\n${gateUpdate}[DYNAMIC_VALIDATION_RESULTS]`
      );
    }
  },
  
  phases: [
    {
      id: 'backend-infrastructure',
      name: 'Backend Infrastructure',
      duration: 'Day 1-2',
      tasks: [
        {
          id: 'task-1.1',
          name: 'Create base entities and common classes',
          files: [
            'backend/src/Shared/Common/BaseEntity.cs',
            'backend/src/Shared/Common/ApiResponse.cs'
          ],
          validation: 'dotnet build backend/src/Shared/',
          dependencies: [],
          estimatedTime: '2 hours'
        }
      ],
      prerequisites: ['Development environment setup'],
      deliverables: ['Working backend infrastructure'],
      validationCriteria: ['All backend builds pass', 'Database migrations work'],
      validation: 'Backend builds and tests pass'
    },
    {
      id: 'core-implementation', 
      name: 'Core Feature Implementation',
      duration: 'Day 3-5',
      tasks: ['Data models/DTOs', 'Business logic', 'API controllers', 'Auth integration'],
      validation: 'API endpoints functional'
    },
    {
      id: 'frontend-components',
      name: 'Frontend Components', 
      duration: 'Day 6-8',
      tasks: ['React components', 'State management', 'API integration', 'Form handling'],
      validation: 'Components render and interact correctly'
    },
    {
      id: 'integration-testing',
      name: 'Integration & Testing',
      duration: 'Day 9-10', 
      tasks: ['E2E integration', 'Test suite', 'Performance optimization', 'Security validation'],
      validation: 'All tests pass, performance criteria met'
    },
    {
      id: 'deployment-docs',
      name: 'Deployment & Documentation',
      duration: 'Day 11-12',
      tasks: ['Production prep', 'Documentation', 'UAT', 'Monitoring setup'],
      validation: 'Feature deployed and operational'
    }
  ],
  
  riskMitigation: {
    technical: ['Database migration conflicts → Feature flags + rollback'],
    timeline: ['Underestimated complexity → Smaller increments + validation'],
    quality: ['Insufficient coverage → TDD + mandatory gates']
  }
};
```

#### Mode: `incremental` - MVP Approach  
```typescript
const incrementalPlan = {
  currentStateAnalysis: async () => {
    const workInProgress = await get_changed_files(['unstaged', 'staged']);
    const gitStatus = await run_in_terminal('git status');
    const existingFeatures = await semantic_search('existing implementation');
    return {workInProgress, gitStatus, existingFeatures};
  },
  
  mvpStrategy: {
    increment1: {
      name: 'Core Functionality',
      scope: ['Essential API endpoints', 'Basic CRUD', 'Simple UI', 'Core validation'],
      excludes: ['Advanced filtering', 'Complex validation', 'Analytics'],
      deliveryTime: '2-3 days',
      gate: 'Core workflow demonstrates value'
    },
    increment2: {
      name: 'Enhanced Features',
      scope: ['Additional functionality', 'Advanced validation', 'Improved UX'],
      gate: 'Feature set meets user expectations'
    },
    increment3: {
      name: 'Performance & Polish',
      scope: ['Optimization', 'Advanced UI', 'Comprehensive testing'],
      gate: 'Production-ready quality achieved'
    }
  }
};
```

#### Mode: `validation-only` - Existing Implementation Validation
```typescript
const validationOnlyPlan = {
  existingAnalysis: async () => {
    const backendFiles = await file_search('**/*.cs');
    const frontendFiles = await file_search('**/*.tsx');
    const incompleteWork = await grep_search('TODO|FIXME|INCOMPLETE');
    const errors = await get_errors(ALL_PROJECT_FILES);
    return {backendFiles, frontendFiles, incompleteWork, errors};
  },
  
  complianceAssessment: {
    prpRequirements: '[REQUIREMENTS_FROM_PRP]',
    implementationGaps: '[IDENTIFIED_GAPS]',
    qualityScore: '[SCORE]/10',
    recommendations: '[IMPROVEMENT_SUGGESTIONS]'
  }
};
```

### Phase 3: Progressive Implementation with Validation

#### Optimized Implementation Loop with Status Tracking
```typescript
// Unified implementation process for all modes with continuous status updates
const implementationLoop = async (phase: Phase, statusFilePath: string) => {
  // Update status file with phase start
  await updatePhaseStart(statusFilePath, phase);
  
  for (const task of phase.tasks) {
    console.log(`\n### Executing: ${task.name}`);
    
    // Update status with task start
    await updateTaskStart(statusFilePath, phase, task);
    
    try {
      // File creation with optimal tool selection
      const toolStrategy = selectOptimalTool(task);
      await executeTaskWithTool(task, toolStrategy);
      
      // Immediate validation
      const validationResults = await runValidationSuite(task.validation);
      
      // Update status with task completion and validation results
      await updateTaskCompletion(statusFilePath, phase, task, validationResults);
      
      // SRP compliance check
      const srpValidation = await validateSingleResponsibility(task.files, task.srp);
      await updateSRPValidation(statusFilePath, task, srpValidation);
      
      if (!validationResults.passed) {
        await updateTaskFailure(statusFilePath, task, validationResults);
        await handleValidationFailure(task, validationResults);
      }
      
    } catch (error) {
      // Update status with error information
      await updateTaskError(statusFilePath, task, error);
      throw error;
    }
  }
  
  // Phase completion validation
  const integrationResults = await runIntegrationCheckpoint(phase);
  await updatePhaseCompletion(statusFilePath, phase, integrationResults);
};

// Status update functions with GitHub Copilot tools
const updatePhaseStart = async (statusFilePath: string, phase: Phase) => {
  const phaseStartUpdate = `
## Phase Started: ${phase.name}
- **Started**: ${new Date().toISOString()}
- **Duration Estimate**: ${phase.duration}
- **Tasks**: ${phase.tasks.length} total
- **Status**: IN_PROGRESS

`;
  await insert_edit_into_file(statusFilePath, phaseStartUpdate);
};

const updateTaskStart = async (statusFilePath: string, phase: Phase, task: Task) => {
  const taskStartUpdate = `
### Task Started: ${task.name}
- **Phase**: ${phase.name}
- **Started**: ${new Date().toISOString()}
- **Files**: ${task.files.join(', ')}
- **Dependencies**: ${task.dependencies.join(', ') || 'None'}

`;
  await insert_edit_into_file(statusFilePath, taskStartUpdate);
};

const updateTaskCompletion = async (statusFilePath: string, phase: Phase, task: Task, validation: any) => {
  const taskCompletionUpdate = `
### Task Completed: ${task.name} ✅
- **Phase**: ${phase.name}
- **Completed**: ${new Date().toISOString()}
- **Validation**: ${validation.passed ? '✅ PASSED' : '❌ FAILED'}
- **Build Status**: ${validation.buildPassed ? '✅ PASSED' : '❌ FAILED'}
- **Type Check**: ${validation.typeCheckPassed ? '✅ PASSED' : '❌ FAILED'}
- **Tests**: ${validation.testsPassed ? '✅ PASSED' : '❌ FAILED'}
${validation.errors?.length ? `- **Errors**: ${validation.errors.join(', ')}` : ''}

`;
  await insert_edit_into_file(statusFilePath, taskCompletionUpdate);
  
  // Update progress percentage
  await updateProgressPercentage(statusFilePath, phase);
};

const updateTaskFailure = async (statusFilePath: string, task: Task, validation: any) => {
  const failureUpdate = `
### Task Failed: ${task.name} ❌
- **Failed**: ${new Date().toISOString()}
- **Validation Errors**: ${validation.errors?.join(', ') || 'Unknown error'}
- **Mitigation**: ${validation.mitigation || 'Manual intervention required'}
- **Status**: FAILED

`;
  await insert_edit_into_file(statusFilePath, failureUpdate);
};

const updateProgressPercentage = async (statusFilePath: string, phase: Phase) => {
  // Calculate overall progress and update the Progress Overview section
  const currentContent = await read_file(statusFilePath);
  const totalTasks = extractTotalTasksFromContent(currentContent);
  const completedTasks = extractCompletedTasksFromContent(currentContent);
  const percentage = Math.round((completedTasks / totalTasks) * 100);
  
  const progressPattern = /- \*\*Completed\*\*: \d+\/\d+ tasks \(\d+%\)/;
  const newProgress = `- **Completed**: ${completedTasks}/${totalTasks} tasks (${percentage}%)`;
  
  await replace_string_in_file(statusFilePath, progressPattern, newProgress);
};

// Optimized tool selection logic
const selectOptimalTool = (task: Task) => {
  const fileExists = checkFileExists(task.files[0]);
  const editType = task.editType || 'create';
  
  return {
    tool: fileExists ? 
      (editType === 'replace' ? 'replace_string_in_file' : 'insert_edit_into_file') : 
      'create_file',
    rationale: `${fileExists ? 'Existing' : 'New'} file, ${editType} operation optimal`
  };
};
```

#### Implementation Strategy with GitHub Copilot Integration
```
File Creation Order (Following PRP Blueprint with Tool Optimization):
1. **Infrastructure First**: Base classes, contexts, configurations
   - Validation: Immediate syntax checking with get_errors tool
2. **Data Models**: Entities, DTOs, request/response models (per PRP)
   - Validation: dotnet build + BaseEntity pattern compliance
3. **Data Access**: Repositories and database interactions (per PRP)
   - Validation: Unit tests + integration with existing BaseRepository
4. **Business Logic**: Services and business rules (per PRP)
   - Validation: Service tests + business rule validation
5. **API Layer**: Controllers and endpoint definitions (per PRP)
   - Validation: API tests + Swagger documentation validation
6. **Frontend Types**: TypeScript interfaces and types (per PRP)
   - Validation: TypeScript compilation + type safety checks
7. **Frontend Services**: API communication layer (per PRP)
   - Validation: API integration tests + error handling validation
8. **Frontend Components**: React components and hooks (per PRP)
   - Validation: Component tests + accessibility validation
9. **Integration**: Routes, navigation, translations (per PRP)
   - Validation: Integration tests + user workflow validation
10. **Testing**: Unit tests, integration tests, component tests (per PRP)
    - Validation: Test execution + coverage validation (>80%)
```

#### File-by-File Implementation with Immediate Validation and Tool Integration
```
For each file creation, I'll follow this enhanced pattern with GitHub Copilot tools:

### Creating: [FILE_PATH]
**Purpose**: [FILE_PURPOSE_FROM_PRP]
**Tool Selected**: [create_file/insert_edit_into_file/replace_string_in_file]
**Tool Rationale**: [WHY_THIS_TOOL_IS_OPTIMAL]
**Dependencies**: [DEPENDENCY_LIST]
**Expected Content**: [CONTENT_DESCRIPTION_FROM_PRP]
**SRP Validation**: [SINGLE_RESPONSIBILITY_CHECK]

#### Implementation Process:
- Use appropriate GitHub Copilot tool based on file state (new vs existing)
- Create file following PRP specifications exactly
- Apply SRP validation (single responsibility principle)
- Include error handling as specified in PRP
- Add internationalization support if required (en/ar)
- Follow established patterns from PRP analysis
- Include proper logging with correlation IDs
- Implement caching strategies where appropriate
- Add comprehensive documentation and comments

#### Immediate Validation with GitHub Copilot Tools:
```powershell
# Syntax validation using GitHub Copilot integration
get_errors: [FILE_PATH]  # Check for compilation errors
run_in_terminal: "dotnet build [relevant-project]"  # For C# files
run_in_terminal: "npm run type-check"              # For TypeScript files

# Style validation
run_in_terminal: "dotnet format --verify-no-changes"
run_in_terminal: "npm run lint"

# Unit test validation (if applicable)
run_in_terminal: "dotnet test [test-project]"
run_in_terminal: "npm run test"

# Integration validation
test_search: [FILE_PATH]  # Find related test files
run_tests: [TEST_FILES]   # Execute relevant tests
```

#### Validation Results with Tool Integration:
- [ ] **Syntax Check**: [PASSED/FAILED] - verified with get_errors
- [ ] **Style Check**: [PASSED/FAILED] - verified with linting tools
- [ ] **Type Check**: [PASSED/FAILED] - verified with TypeScript compiler
- [ ] **Unit Tests**: [PASSED/FAILED/N/A] - verified with test execution
- [ ] **SRP Compliance**: [PASSED/FAILED] - verified against responsibility matrix
- [ ] **Pattern Consistency**: [PASSED/FAILED] - verified against PRP patterns
- [ ] **Integration**: [PASSED/FAILED] - verified with related components

### Next File: [NEXT_FILE_PATH]
**Dependency Status**: [DEPENDENCIES_MET/PENDING]
**Blocking Issues**: [NONE/LIST_OF_ISSUES]
**Estimated Completion**: [TIME_ESTIMATE]
```

#### Progressive Integration Validation with Tool-Enhanced Testing
```
After each logical group of files, I'll run integration validation using GitHub Copilot tools:

### Integration Checkpoint: [CHECKPOINT_NAME]
**Files Completed**: [LIST_OF_COMPLETED_FILES]
**Integration Points**: [INTEGRATION_UPDATES_MADE]
**Tools Used**: [LIST_OF_GITHUB_COPILOT_TOOLS]

#### Integration Validation with GitHub Copilot Tools:
```bash
# API integration testing with run_in_terminal
run_in_terminal: "curl -X GET http://localhost:5000/api/health"
run_in_terminal: "curl -X GET http://localhost:5000/api/[feature-endpoint]"

# Frontend integration testing
run_in_terminal: "npm run dev"  # Verify components render
run_in_terminal: "npm run test:e2e"  # Run end-to-end tests if applicable

# Database integration testing
run_in_terminal: "./reset-database.ps1"  # Reset to clean state
run_in_terminal: "dotnet ef database update"  # Apply migrations

# Comprehensive integration validation
get_errors: [ALL_MODIFIED_FILES]  # Check for any new errors
run_tests: [INTEGRATION_TEST_FILES]  # Execute integration test suite
```

#### Integration Results with Tool Validation:
- [ ] **API Endpoints**: [RESPONDING/FAILED] - verified with curl commands
- [ ] **Frontend Components**: [RENDERING/FAILED] - verified with dev server
- [ ] **Database Operations**: [WORKING/FAILED] - verified with migration tests
- [ ] **Authentication**: [WORKING/FAILED/N/A] - verified with auth tests
- [ ] **i18n Support**: [WORKING/FAILED] - verified with language switching
- [ ] **Accessibility**: [WORKING/FAILED] - verified with a11y tests
- [ ] **Performance**: [ACCEPTABLE/NEEDS_OPTIMIZATION] - verified with benchmarks

#### Issue Resolution with GitHub Copilot Tools:
If any integration fails:
1. **Identify Root Cause**: Use get_errors and semantic_search for similar issues
2. **Apply Targeted Fix**: Use appropriate edit tool (insert_edit_into_file/replace_string_in_file)
3. **Re-validate**: Use run_in_terminal for immediate verification
4. **Update Status**: Use insert_edit_into_file to update status tracking file
5. **Document Resolution**: Record fix in status file for future reference
```

### Phase 4: Comprehensive Validation & QA

#### Multi-Level Validation Suite (Optimized)
```typescript
const validationSuite = async () => {
  // Level 1: Syntax & Style
  const level1 = await Promise.all([
    run_in_terminal('dotnet build backend/src/ --configuration Release'),
    run_in_terminal('dotnet format --verify-no-changes backend/src/'),
    run_in_terminal('npm run type-check'),
    run_in_terminal('npm run lint')
  ]);
  
  // Level 2: Testing
  const level2 = await Promise.all([
    run_in_terminal('dotnet test backend/tests/ --logger "console;verbosity=detailed"'),
    run_in_terminal('npm run test --coverage')
  ]);
  
  // Level 3: Integration
  const level3 = await Promise.all([
    run_in_terminal('npm run test:integration'),
    run_in_terminal('npm run test:e2e')
  ]);
  
  // Level 4: Performance & Security
  const level4 = await Promise.all([
    run_in_terminal('npm run build:analyze'),
    run_in_terminal('dotnet test --collect:"XPlat Code Coverage"'),
    get_errors(ALL_MODIFIED_FILES)
  ]);
  
  return {level1, level2, level3, level4};
};
```

#### Execute All PRP Validation Commands using GitHub Copilot Tools
```powershell
# Backend validation (from PRP validation section) using run_in_terminal
run_in_terminal: "dotnet build --configuration Release"
✓/✗ Build status: [RESULT] - verified with get_errors for compilation issues

run_in_terminal: "dotnet format --verify-no-changes"
✓/✗ Code formatting: [RESULT] - style compliance verified

run_in_terminal: "dotnet test --logger 'console;verbosity=detailed'"
✓/✗ All tests: [X/Y] passed - comprehensive test execution

# Frontend validation (from PRP validation section) using run_in_terminal
run_in_terminal: "npm run type-check"
✓/✗ TypeScript compilation: [RESULT] - type safety verified

run_in_terminal: "npm run lint"
✓/✗ ESLint checks: [RESULT] - code quality standards met

run_in_terminal: "npm run test"
✓/✗ Unit tests: [X/Y] passed - frontend logic validated

run_in_terminal: "npm run build"
✓/✗ Build validation: [RESULT] - production readiness verified

# Additional GitHub Copilot tool validation
get_errors: [ALL_MODIFIED_FILES]  # Comprehensive error checking
file_search: "**/*.cs" | get_errors  # Backend file validation
file_search: "**/*.ts*" | get_errors  # Frontend file validation
```

#### Quality Gates Matrix (Comprehensive)
```yaml
Quality Gates (All Must Pass):
  Build & Syntax:
    - Backend Build: dotnet build --configuration Release ✓/✗
    - Code Formatting: dotnet format --verify-no-changes ✓/✗  
    - TypeScript: npm run type-check ✓/✗
    - Linting: npm run lint ✓/✗
    
  Testing:
    - Backend Tests: dotnet test ✓/✗ ([X/Y] passed)
    - Frontend Tests: npm run test ✓/✗ ([X/Y] passed)
    - Coverage: >80% ✓/✗
    
  Integration:
    - API Endpoints: Functional ✓/✗
    - Frontend/Backend: Connected ✓/✗
    - Database: Migrations applied ✓/✗
    - Authentication: Working ✓/✗
    
  Quality:
    - SRP Compliance: All files ✓/✗
    - Performance: <500ms API, <3s load ✓/✗
    - Security: No vulnerabilities ✓/✗
    - i18n: English/Arabic support ✓/✗
    - Accessibility: WCAG compliant ✓/✗

Final Score: [X/10] (Minimum: 8/10 for deployment)
```

## Status Tracking & Reporting

### Enhanced Status File Management with Continuous Updates

```typescript
// Status file management with real-time updates
const statusFileManager = {
  // Initialize status file based on existing state
  initialize: async (prpFilePath: string, statusReview: any, executionMode: string) => {
    const statusFilePath = statusReview.filePath;
    
    if (statusReview.exists && !forceValidation) {
      // Resume existing implementation
      const resumeHeader = `
## Execution Resumed
- **Resumed**: ${new Date().toISOString()}
- **Previous Progress**: ${statusReview.progress?.percentage || 0}%
- **Previous Phase**: ${statusReview.currentPhase || 'Unknown'}
- **Resume Strategy**: ${executionStrategy.strategy}
- **Resume Reason**: ${executionStrategy.reason}

---

`;
      await insert_edit_into_file(statusFilePath, resumeHeader);
    } else {
      // Create new status file
      await create_file(statusFilePath, generateInitialStatusContent(prpFilePath, executionMode));
    }
    
    return statusFilePath;
  },

  // Real-time progress tracking
  updateProgress: async (statusFilePath: string, progressData: any) => {
    const progressSection = `
## Progress Overview
- **Completed**: ${progressData.completed}/${progressData.total} tasks (${progressData.percentage}%)
- **Current Phase**: ${progressData.currentPhase}
- **Current Task**: ${progressData.currentTask}
- **Next Task**: ${progressData.nextTask}
- **Quality Score**: ${progressData.qualityScore}/10
- **Last Updated**: ${new Date().toISOString()}
`;

    // Use replace_string_in_file to update existing progress section
    await replace_string_in_file(statusFilePath, 
      /## Progress Overview[\s\S]*?(?=##|$)/, 
      progressSection
    );
  },

  // Phase completion tracking
  updatePhaseCompletion: async (statusFilePath: string, phase: any, results: any) => {
    const phaseCompletionUpdate = `
## Phase Completed: ${phase.name} ${results.passed ? '✅' : '❌'}
- **Completed**: ${new Date().toISOString()}
- **Duration**: ${phase.actualDuration}
- **Tasks Completed**: ${results.tasksCompleted}/${phase.tasks.length}
- **Quality Score**: ${results.qualityScore}/10
- **Integration Tests**: ${results.integrationPassed ? '✅ PASSED' : '❌ FAILED'}
- **Ready for Next Phase**: ${results.readyForNext ? '✅ YES' : '❌ NO'}

`;
    await insert_edit_into_file(statusFilePath, phaseCompletionUpdate);
  },

  // Quality gate tracking
  updateQualityGate: async (statusFilePath: string, gateName: string, results: any) => {
    const qualityUpdate = `
### Quality Gate: ${gateName}
- **Timestamp**: ${new Date().toISOString()}
- **Status**: ${results.passed ? '✅ PASSED' : '❌ FAILED'}
- **Score**: ${results.score}/10
- **Build**: ${results.buildPassed ? '✅' : '❌'}
- **Tests**: ${results.testsPassed ? '✅' : '❌'}
- **Coverage**: ${results.coverage}%
- **Performance**: ${results.performancePassed ? '✅' : '❌'}
- **Security**: ${results.securityPassed ? '✅' : '❌'}
${results.errors?.length ? `- **Errors**: ${results.errors.join(', ')}` : ''}
${results.warnings?.length ? `- **Warnings**: ${results.warnings.join(', ')}` : ''}

`;

    // Find and update the Quality Validation section
    const currentContent = await read_file(statusFilePath);
    if (currentContent.includes('## Quality Validation')) {
      await insert_edit_into_file(statusFilePath, qualityUpdate);
    } else {
      await insert_edit_into_file(statusFilePath, `## Quality Validation\n${qualityUpdate}`);
    }
  },

  // Issue tracking and resolution
  updateIssue: async (statusFilePath: string, issue: any) => {
    const issueUpdate = `
### Issue ${issue.id}: ${issue.description}
- **File**: ${issue.file}
- **Error**: ${issue.error}
- **Severity**: ${issue.severity}
- **Status**: ${issue.status}
- **Fix Applied**: ${issue.fix || 'Pending'}
- **Timestamp**: ${new Date().toISOString()}

`;

    // Update Issues & Resolutions section
    const currentContent = await read_file(statusFilePath);
    if (currentContent.includes('## Issues & Resolutions')) {
      await insert_edit_into_file(statusFilePath, issueUpdate);
    } else {
      await insert_edit_into_file(statusFilePath, `## Issues & Resolutions\n${issueUpdate}`);
    }
  },

  // Final completion summary
  updateCompletion: async (statusFilePath: string, completionData: any) => {
    const completionSummary = `
## Completion Summary
- **Status**: ${completionData.status}
- **Final Score**: ${completionData.finalScore}/10
- **Files Created**: ${completionData.filesCreated}
- **Files Modified**: ${completionData.filesModified}
- **Tests Written**: ${completionData.testsWritten}
- **Coverage**: ${completionData.coverage}%
- **Build Status**: ${completionData.buildStatus}
- **All Tests Pass**: ${completionData.allTestsPass ? '✅' : '❌'}
- **Ready for**: ${completionData.readyFor}
- **Deployment Ready**: ${completionData.deploymentReady ? '✅ YES' : '❌ NO'}
- **Completed**: ${new Date().toISOString()}

## Success Metrics
- **Implementation Quality**: ${completionData.implementationQuality}/10
- **Code Coverage**: ${completionData.coverage}%
- **Performance**: ${completionData.performance}
- **Security**: ${completionData.security}
- **User Experience**: ${completionData.userExperience}/10

## Next Steps
${completionData.nextSteps.map(step => `- ${step}`).join('\n')}
`;

    await replace_string_in_file(statusFilePath, 
      /## Completion Summary[\s\S]*?$/, 
      completionSummary
    );
  }
};

// Generate initial status file content
const generateInitialStatusContent = (prpFilePath: string, executionMode: string) => `
# PRP Implementation Status: ${extractFeatureName(prpFilePath)}

## Execution Context
- **PRP File**: ${prpFilePath}
- **Mode**: ${executionMode}
- **Started**: ${new Date().toISOString()}
- **Phase**: Planning
- **Status**: INITIALIZING

## Progress Overview
- **Completed**: 0/[TOTAL] tasks (0%)
- **Current Phase**: Context Discovery
- **Current Task**: PRP Analysis
- **Next Task**: Implementation Planning
- **Quality Score**: 0/10

## Phase Status
### Phase 1: Context Discovery & Analysis
- **Status**: IN_PROGRESS
- **Started**: ${new Date().toISOString()}

## Quality Validation
[No validation performed yet]

## Issues & Resolutions
[No issues identified yet]

## Completion Summary
- **Status**: IN_PROGRESS
- **Files Created**: 0
- **Tests Written**: 0
- **Ready for**: Implementation Planning
`;
```

### Dynamic Status Updates During Execution

The status file is continuously updated throughout execution using GitHub Copilot tools:

1. **Real-time Progress**: Updated after each task completion
2. **Quality Gates**: Updated after each validation checkpoint  
3. **Issue Tracking**: Updated when errors occur with resolution status
4. **Phase Transitions**: Updated when moving between phases
5. **Final Summary**: Complete implementation metrics and readiness assessment

### Optimized Status File Structure
```markdown
# PRP Implementation Status: [FEATURE_NAME]

## Execution Context
- **PRP File**: [PATH]
- **Mode**: [EXECUTION_MODE]
- **Started**: [TIMESTAMP]
- **Phase**: [CURRENT_PHASE]

## Progress Overview
- **Completed**: [X]/[Y] tasks ([PERCENTAGE]%)
- **Current**: [CURRENT_TASK]
- **Next**: [NEXT_TASK]  
- **Quality Score**: [SCORE]/10

## Phase Status
[DYNAMIC_PHASE_STATUS_FROM_EXECUTION]

## Issues & Resolutions
[DYNAMIC_ISSUE_TRACKING]

1. **Issue**: [DESCRIPTION]
   - **File**: [FILE_PATH]
   - **Error**: [ERROR_MESSAGE]
   - **Fix Applied**: [SOLUTION]
   - **Status**: [RESOLVED/PENDING]

## Quality Validation
[DYNAMIC_VALIDATION_RESULTS]

## Completion Summary
- **Status**: [COMPLETED/IN_PROGRESS/FAILED]
- **Files Created**: [COUNT]
- **Tests Written**: [COUNT]
- **Ready for**: [REVIEW/STAGING/PRODUCTION]
```

## Command Activation
When user executes:
```
@copilot /execute-prp <prp_file_path> [execution_mode] [start_phase] [force_validation]
```

The system follows this enhanced workflow:

### 1. Initial Setup & Status Review
- **Parse Command**: Extract and validate all parameters
- **Locate PRP File**: Find full path if relative path provided
- **Review Existing Status**: Check for existing status file and analyze progress
- **Determine Strategy**: Decide whether to resume, restart, or validate only
- **Initialize Status Tracking**: Create or update status file with execution context

### 2. Context Loading & Analysis  
- **Load PRP Context** using GitHub Copilot tools (read_file, semantic_search)
- **Analyze Project State** using file_search and grep_search for existing implementations
- **Extract Requirements** from PRP structure and validation criteria
- **Assess Dependencies** and integration points with existing codebase

### 3. Implementation Planning
- **Generate Implementation Plan** based on execution mode and PRP analysis
- **Create Task Breakdown** with dependencies and validation criteria  
- **Setup Quality Gates** with measurable acceptance criteria (minimum 8/10)
- **Initialize Progress Tracking** with real-time status updates

### 4. Progressive Implementation  
- **Execute Tasks Sequentially** with optimal GitHub Copilot tool selection
- **Continuous Validation** after each task with immediate feedback
- **Real-time Status Updates** using insert_edit_into_file and replace_string_in_file
- **Quality Gate Validation** at each milestone with detailed reporting
- **Issue Tracking & Resolution** with automatic status file updates

### 5. Quality Assurance & Validation
- **Multi-level Validation Suite** using run_in_terminal for build/test execution
- **Integration Testing** with get_errors for comprehensive error checking
- **Performance Validation** with benchmarking and optimization verification
- **Security Assessment** with vulnerability scanning and compliance checks

### 6. Completion & Reporting
- **Final Quality Assessment** with comprehensive scoring (minimum 8/10)
- **Status File Completion** with full metrics and readiness assessment
- **Deployment Preparation** with production readiness validation
- **Documentation Generation** with lessons learned and next steps
5. **Prepares Deployment** with final validation
6. **Generates Report** with comprehensive status and lessons learned

## Key Features Enhanced & Optimized
- ✅ **All Execution Modes**: full, incremental, validation-only
- ✅ **All Phases**: planning, implementation, testing, deployment  
- ✅ **GitHub Copilot Tool Integration**: All native tools used optimally
- ✅ **Progressive Validation**: Continuous quality gates
- ✅ **Enhanced Status Tracking**: Real-time progress monitoring with existing status file review
- ✅ **Resume Capability**: Automatic detection and resumption of previous implementations
- ✅ **Dynamic Status Updates**: Continuous status file updates during execution
- ✅ **Issue Tracking**: Real-time error tracking and resolution documentation
- ✅ **Progress Percentage**: Live calculation and updates of completion percentage
- ✅ **Quality Gate Reporting**: Detailed validation results with timestamp tracking
- ✅ **Risk Assessment**: Built-in mitigation strategies
- ✅ **Quality Gates**: 8/10 minimum score requirement
- ✅ **SRP Compliance**: Single Responsibility Principle validation
- ✅ **i18n Support**: English/Arabic internationalization
- ✅ **Performance Validation**: Optimization and benchmarking
- ✅ **Security Validation**: Comprehensive security checks
- ✅ **Force Validation**: Re-run validation option with status file reset
- ✅ **Multiple Start Phases**: Begin from any phase with context preservation
- ✅ **Smart Resume**: Intelligent resumption based on previous progress analysis
- ✅ **Tool Selection Logic**: Optimal tool selection per task
- ✅ **Integration Checkpoints**: Regular integration validation
- ✅ **Error Recovery**: Automated issue resolution with tools
- ✅ **File-by-File Implementation**: Detailed implementation patterns with immediate validation
- ✅ **Progressive Integration**: Tool-enhanced testing and validation loops
- ✅ **Comprehensive Documentation**: Complete status tracking and reporting
- ✅ **Status File Management**: Create, update, and maintain comprehensive execution tracking
- ✅ **Execution History**: Maintain detailed history of all execution attempts and outcomes

````

`````
