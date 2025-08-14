---
mode: agent
description: "Execute task-level Product Requirements Prompts with focused implementation and validation"
inputs:
  - name: task_prp_path
    description: Path to the task PRP file to execute
    required: true
  - name: parallel_execution
    description: Allow parallel execution (true, false)
    required: false
    default: "false"
---
# prp-task-execute.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Execute task PRPs systematically with comprehensive validation, error handling, and progress tracking
- **Categories**: task-execution, systematic-validation, progress-tracking, error-recovery
- **Complexity**: advanced
- **Dependencies**: task PRP files, validation frameworks, testing tools, rollback procedures

## Input
- **task_prp_file** (required): Path to the task PRP file to execute
- **execution_mode** (optional): Execution approach (sequential/parallel/validation-first)
- **checkpoint_frequency** (optional): Validation checkpoint frequency (per-task/per-phase/custom)

## Template

```
You are an expert task execution specialist focused on systematically implementing task PRPs with comprehensive validation, error handling, and progress tracking. Your goal is to execute tasks safely and efficiently while maintaining quality gates and providing detailed progress feedback.

## Input Parameters
- **Task PRP File**: {task_prp_file}
- **Execution Mode**: {execution_mode} (default: sequential)
- **Checkpoint Frequency**: {checkpoint_frequency} (default: per-task)

## Task Overview
Execute the specified task PRP by loading and understanding the task structure, executing each task with systematic validation, handling failures gracefully, and ensuring comprehensive verification of all changes. Focus on maintaining safety, quality, and progress visibility throughout the execution process.

## Phase 1: Task PRP Loading and Preparation

### Task PRP Analysis and Context Understanding
Use `read_file` to thoroughly understand the task PRP structure:

```typescript
// Comprehensive task PRP parsing and analysis
interface TaskPRPStructure {
  task_metadata: {
    task_name: string;
    description: string;
    scope: string;
    success_criteria: string[];
    estimated_duration: string;
  };
  
  context_information: {
    documentation_references: Array<{
      url: string;
      focus: string;
      relevance: string;
    }>;
    implementation_patterns: Array<{
      file_path: string;
      pattern_type: string;
      copy_approach: string;
      adaptation_notes: string;
    }>;
    critical_gotchas: Array<{
      issue: string;
      context: string;
      fix: string;
      validation: string;
    }>;
    dependency_considerations: Array<{
      component: string;
      relationship: string;
      impact: string;
      coordination: string;
    }>;
  };
  
  task_breakdown: {
    setup_tasks: TaskPhase;
    core_implementation: TaskPhase;
    integration_tasks: TaskPhase;
    validation_tasks: TaskPhase;
    cleanup_tasks: TaskPhase;
  };
  
  execution_strategy: {
    task_sequencing: Array<{
      phase: string;
      tasks: string[];
      dependencies: string[];
      parallel_opportunities: string[];
    }>;
    validation_framework: {
      validation_types: string[];
      quality_gates: string[];
      success_criteria: string[];
    };
    risk_management: {
      rollback_procedures: string[];
      error_recovery: string[];
      escalation_procedures: string[];
    };
  };
}

interface TaskPhase {
  phase_name: string;
  tasks: Array<{
    task_id: string;
    description: string;
    actions: Array<{
      action_type: string;
      target: string;
      operation: string;
      validation: string;
      if_fail: string;
      rollback: string;
    }>;
    completion_criteria: string[];
  }>;
}

// Parse and understand the complete task PRP
async function parseTaskPRP(filePath: string): Promise<TaskPRPStructure> {
  // Read and parse the task PRP file
  // Extract all phases and task definitions
  // Understand context and validation requirements
  // Map dependencies and execution order
}
```

### Execution Environment Preparation
```bash
# Prepare execution environment and safety measures
echo "🚀 Preparing Task Execution Environment"

prepare_execution_environment() {
    echo "📋 Task PRP Analysis..."
    # Parse task PRP file structure
    # Validate all referenced files exist
    # Check prerequisites and dependencies
    
    echo "🔒 Safety Measures Setup..."
    # Create backup branch with timestamp
    git checkout -b "task-execution-$(date +%Y%m%d-%H%M%S)"
    git add .
    git commit -m "Backup before task PRP execution: ${task_name}"
    
    # Tag current state for rollback reference
    git tag "pre-task-${task_name}-$(date +%Y%m%d-%H%M%S)"
    
    echo "📊 Progress Tracking Setup..."
    # Create execution log directory
    mkdir -p ".task-execution-logs/$(date +%Y%m%d-%H%M%S)"
    
    # Initialize progress tracking
    cat > "execution_progress.json" << 'EOF'
{
  "execution_id": "'$(date +%Y%m%d-%H%M%S)'",
  "task_name": "'${task_name}'",
  "start_time": "'$(date -Iseconds)'",
  "phases": {},
  "completed_tasks": [],
  "failed_tasks": [],
  "current_checkpoint": "initialization"
}
EOF
    
    echo "✅ Execution environment ready"
}

# Validate execution prerequisites
validate_execution_prerequisites() {
    echo "🔍 Validating Execution Prerequisites"
    
    # Check that all required tools are available
    command -v npm >/dev/null 2>&1 || { echo "❌ npm is required"; exit 1; }
    command -v git >/dev/null 2>&1 || { echo "❌ git is required"; exit 1; }
    
    # Verify project state is clean
    if [[ -n $(git status --porcelain) ]]; then
        echo "⚠️ Working directory has uncommitted changes"
        echo "🔄 Creating safety backup..."
        git stash push -m "Pre-task-execution stash $(date)"
    fi
    
    # Run initial validation suite
    echo "🧪 Running Pre-Execution Validation..."
    npm run type-check
    npm run lint
    npm test -- --watchAll=false
    
    if [[ $? -eq 0 ]]; then
        echo "✅ Prerequisites validation passed"
        return 0
    else
        echo "❌ Prerequisites validation failed"
        echo "🚨 Execution cannot proceed safely"
        return 1
    fi
}
```

### Task Dependency Analysis and Execution Planning
```typescript
// Analyze task dependencies and create optimized execution plan
interface ExecutionPlan {
  execution_phases: Array<{
    phase_name: string;
    phase_order: number;
    tasks: Array<{
      task_id: string;
      execution_order: number;
      dependencies: string[];
      estimated_duration: string;
      complexity_level: 'low' | 'medium' | 'high';
      risk_level: 'low' | 'medium' | 'high';
    }>;
    phase_validation: {
      entry_criteria: string[];
      progress_checkpoints: string[];
      exit_criteria: string[];
    };
    rollback_strategy: {
      checkpoint_name: string;
      rollback_commands: string[];
      validation_commands: string[];
    };
  }>;
  
  parallel_opportunities: Array<{
    parallel_group: string;
    tasks: string[];
    coordination_requirements: string[];
    shared_resources: string[];
  }>;
  
  critical_path: {
    critical_tasks: string[];
    bottlenecks: string[];
    optimization_opportunities: string[];
  };
  
  validation_strategy: {
    continuous_validation: string[];
    checkpoint_validation: string[];
    final_validation: string[];
  };
}

// Create optimized execution plan
function createExecutionPlan(taskPRP: TaskPRPStructure): ExecutionPlan {
  // Analyze task dependencies and constraints
  // Identify critical path and optimization opportunities
  // Plan validation checkpoints and rollback strategies
  // Design parallel execution where possible
}
```

## Phase 2: Systematic Task Execution with Validation

### Task Execution Framework
```typescript
// Comprehensive task execution with validation and error handling
interface TaskExecutionResult {
  task_id: string;
  status: 'success' | 'failed' | 'skipped' | 'rollback';
  execution_start: string;
  execution_end: string;
  actions_completed: string[];
  validation_results: Array<{
    validation_type: string;
    result: 'pass' | 'fail';
    details: string;
  }>;
  error_details?: {
    error_message: string;
    failure_point: string;
    recovery_attempted: boolean;
    rollback_applied: boolean;
  };
  performance_metrics: {
    execution_time: string;
    resource_usage: string;
    complexity_actual: string;
  };
}

// Execute single task with comprehensive validation
async function executeTaskWithValidation(
  task: any,
  context: TaskPRPStructure
): Promise<TaskExecutionResult> {
  const startTime = new Date().toISOString();
  const result: TaskExecutionResult = {
    task_id: task.task_id,
    status: 'failed',
    execution_start: startTime,
    execution_end: '',
    actions_completed: [],
    validation_results: [],
    performance_metrics: {
      execution_time: '',
      resource_usage: '',
      complexity_actual: ''
    }
  };
  
  try {
    // Phase 1: Pre-execution validation
    await validateTaskPreConditions(task, context);
    
    // Phase 2: Execute task actions systematically
    for (const action of task.actions) {
      await executeTaskAction(action, context);
      result.actions_completed.push(action.action_type);
      
      // Immediate validation after each action
      const actionValidation = await validateActionCompletion(action);
      result.validation_results.push(actionValidation);
      
      if (actionValidation.result === 'fail') {
        throw new Error(`Action validation failed: ${actionValidation.details}`);
      }
    }
    
    // Phase 3: Task completion validation
    const completionValidation = await validateTaskCompletion(task);
    result.validation_results.push(...completionValidation);
    
    // Phase 4: Integration validation
    const integrationValidation = await validateTaskIntegration(task, context);
    result.validation_results.push(...integrationValidation);
    
    result.status = 'success';
    
  } catch (error) {
    // Handle task execution failure
    result.error_details = {
      error_message: error.message,
      failure_point: result.actions_completed[result.actions_completed.length - 1] || 'pre-execution',
      recovery_attempted: false,
      rollback_applied: false
    };
    
    // Attempt error recovery
    const recoveryResult = await attemptErrorRecovery(task, error, context);
    result.error_details.recovery_attempted = recoveryResult.attempted;
    
    if (!recoveryResult.success) {
      // Apply rollback procedures
      const rollbackResult = await applyTaskRollback(task, context);
      result.error_details.rollback_applied = rollbackResult.success;
      result.status = rollbackResult.success ? 'rollback' : 'failed';
    } else {
      result.status = 'success';
    }
  } finally {
    result.execution_end = new Date().toISOString();
    result.performance_metrics.execution_time = 
      calculateExecutionTime(startTime, result.execution_end);
  }
  
  return result;
}
```

### Action-Specific Execution Strategies
```typescript
// Execute different action types with appropriate GitHub Copilot tools
async function executeTaskAction(action: any, context: TaskPRPStructure): Promise<void> {
  const { action_type, target, operation, validation } = action;
  
  switch (action_type) {
    case 'CREATE':
      await executeCreateAction(target, operation, validation);
      break;
      
    case 'MODIFY':
      await executeModifyAction(target, operation, validation);
      break;
      
    case 'REPLACE':
      await executeReplaceAction(target, operation, validation);
      break;
      
    case 'MIRROR':
      await executeMirrorAction(target, operation, validation, context);
      break;
      
    case 'COPY':
      await executeCopyAction(target, operation, validation, context);
      break;
      
    case 'DELETE':
      await executeDeleteAction(target, operation, validation);
      break;
      
    case 'CONFIGURE':
      await executeConfigureAction(target, operation, validation);
      break;
      
    case 'TEST':
      await executeTestAction(target, operation, validation);
      break;
      
    case 'VALIDATE':
      await executeValidateAction(target, operation, validation);
      break;
      
    default:
      throw new Error(`Unknown action type: ${action_type}`);
  }
}

// CREATE action implementation
async function executeCreateAction(target: string, operation: string, validation: string): Promise<void> {
  // Use create_file for new file creation
  // Follow patterns from context references
  // Apply proper TypeScript typing
  // Include comprehensive error handling
  
  if (target.endsWith('.tsx') || target.endsWith('.ts')) {
    // Create TypeScript/React component
    await createTypeScriptFile(target, operation);
  } else if (target.endsWith('.test.ts') || target.endsWith('.test.tsx')) {
    // Create test file
    await createTestFile(target, operation);
  } else if (target.endsWith('.json')) {
    // Create configuration file
    await createConfigFile(target, operation);
  }
  
  // Execute validation command
  await executeValidationCommand(validation);
}

// MODIFY action implementation
async function executeModifyAction(target: string, operation: string, validation: string): Promise<void> {
  // Use read_file to understand current state
  // Use replace_string_in_file or insert_edit_into_file for changes
  // Preserve existing functionality
  // Apply incremental improvements
  
  const currentContent = await getCurrentFileContent(target);
  const modifications = parseModificationOperation(operation);
  
  for (const modification of modifications) {
    if (modification.type === 'replace') {
      await applyReplaceModification(target, modification);
    } else if (modification.type === 'insert') {
      await applyInsertModification(target, modification);
    }
  }
  
  // Execute validation command
  await executeValidationCommand(validation);
}
```

### Comprehensive Validation and Progress Tracking
```bash
# Progressive validation and checkpoint management
execute_task_validation() {
    local task_id=$1
    local validation_commands=$2
    
    echo "🔍 Executing Validation for Task: $task_id"
    
    # Update progress tracking
    update_progress_tracking() {
        local phase=$1
        local status=$2
        
        # Update execution progress JSON
        jq --arg task "$task_id" --arg phase "$phase" --arg status "$status" --arg timestamp "$(date -Iseconds)" \
           '.phases[$phase].tasks[$task] = {status: $status, timestamp: $timestamp}' \
           execution_progress.json > temp.json && mv temp.json execution_progress.json
    }
    
    # Execute validation commands systematically
    echo "📝 Syntax and Type Validation..."
    npm run type-check
    if [[ $? -ne 0 ]]; then
        echo "❌ Type check failed for task $task_id"
        update_progress_tracking "validation" "type_check_failed"
        return 1
    fi
    
    echo "🎨 Code Style Validation..."
    npm run lint
    if [[ $? -ne 0 ]]; then
        echo "❌ Lint check failed for task $task_id"
        update_progress_tracking "validation" "lint_failed"
        return 1
    fi
    
    echo "🧪 Unit Test Validation..."
    npm test -- --testNamePattern="$task_id" --watchAll=false
    if [[ $? -ne 0 ]]; then
        echo "❌ Unit tests failed for task $task_id"
        update_progress_tracking "validation" "unit_tests_failed"
        return 1
    fi
    
    echo "🔗 Integration Validation..."
    npm run test:integration
    if [[ $? -ne 0 ]]; then
        echo "❌ Integration tests failed for task $task_id"
        update_progress_tracking "validation" "integration_tests_failed"
        return 1
    fi
    
    # Execute task-specific validation commands
    echo "🎯 Task-Specific Validation..."
    IFS=',' read -ra VALIDATION_COMMANDS <<< "$validation_commands"
    for cmd in "${VALIDATION_COMMANDS[@]}"; do
        echo "Executing: $cmd"
        eval "$cmd"
        if [[ $? -ne 0 ]]; then
            echo "❌ Task-specific validation failed: $cmd"
            update_progress_tracking "validation" "custom_validation_failed"
            return 1
        fi
    done
    
    echo "✅ All validations passed for task $task_id"
    update_progress_tracking "validation" "passed"
    return 0
}

# Checkpoint creation and management
create_execution_checkpoint() {
    local checkpoint_name=$1
    local task_id=$2
    
    echo "💾 Creating Execution Checkpoint: $checkpoint_name"
    
    # Create git checkpoint
    git add .
    git commit -m "Checkpoint: $checkpoint_name after completing $task_id"
    git tag "checkpoint-$checkpoint_name-$(date +%Y%m%d-%H%M%S)"
    
    # Update progress tracking
    jq --arg checkpoint "$checkpoint_name" --arg timestamp "$(date -Iseconds)" \
       '.current_checkpoint = $checkpoint | .checkpoints[$checkpoint] = $timestamp' \
       execution_progress.json > temp.json && mv temp.json execution_progress.json
    
    echo "✅ Checkpoint created successfully"
}
```

## Phase 3: Error Handling and Recovery Management

### Comprehensive Error Recovery Strategies
```typescript
// Advanced error recovery and rollback management
interface ErrorRecoveryResult {
  attempted: boolean;
  success: boolean;
  recovery_strategy: string;
  actions_taken: string[];
  remaining_issues: string[];
  rollback_required: boolean;
}

// Attempt intelligent error recovery
async function attemptErrorRecovery(
  task: any,
  error: Error,
  context: TaskPRPStructure
): Promise<ErrorRecoveryResult> {
  const result: ErrorRecoveryResult = {
    attempted: true,
    success: false,
    recovery_strategy: '',
    actions_taken: [],
    remaining_issues: [],
    rollback_required: false
  };
  
  try {
    // Analyze error type and context
    const errorAnalysis = analyzeError(error, task);
    result.recovery_strategy = errorAnalysis.recommended_strategy;
    
    switch (errorAnalysis.error_category) {
      case 'syntax_error':
        result.success = await recoverFromSyntaxError(task, error);
        break;
        
      case 'type_error':
        result.success = await recoverFromTypeError(task, error);
        break;
        
      case 'dependency_error':
        result.success = await recoverFromDependencyError(task, error);
        break;
        
      case 'validation_error':
        result.success = await recoverFromValidationError(task, error);
        break;
        
      case 'integration_error':
        result.success = await recoverFromIntegrationError(task, error);
        break;
        
      default:
        // Generic recovery approach
        result.success = await attemptGenericRecovery(task, error);
        break;
    }
    
    if (result.success) {
      // Validate recovery success
      const validationResult = await validateTaskCompletion(task);
      result.success = validationResult.every(v => v.result === 'pass');
    }
    
  } catch (recoveryError) {
    result.success = false;
    result.remaining_issues.push(`Recovery failed: ${recoveryError.message}`);
    result.rollback_required = true;
  }
  
  return result;
}

// Syntax error recovery
async function recoverFromSyntaxError(task: any, error: Error): Promise<boolean> {
  // Parse error message to identify syntax issue
  // Use automated formatting tools
  // Apply common syntax fixes
  // Re-validate syntax
  
  try {
    // Run automated formatting
    await runCommand('npm run format');
    
    // Check for common syntax issues and fix
    await fixCommonSyntaxIssues(task.target);
    
    // Validate syntax correction
    const syntaxCheck = await runCommand('npm run type-check');
    return syntaxCheck.exitCode === 0;
    
  } catch (fixError) {
    return false;
  }
}

// Dependency error recovery
async function recoverFromDependencyError(task: any, error: Error): Promise<boolean> {
  // Identify missing or incompatible dependencies
  // Attempt automatic dependency resolution
  // Update package configurations
  // Re-install dependencies
  
  try {
    // Clean and reinstall dependencies
    await runCommand('rm -rf node_modules package-lock.json');
    await runCommand('npm install');
    
    // Check for specific dependency issues
    await resolveDependencyConflicts(error.message);
    
    // Validate dependency resolution
    const dependencyCheck = await runCommand('npm run build');
    return dependencyCheck.exitCode === 0;
    
  } catch (fixError) {
    return false;
  }
}
```

### Task Rollback and System Recovery
```bash
# Comprehensive rollback procedures
execute_task_rollback() {
    local task_id=$1
    local rollback_strategy=$2
    
    echo "🔄 Executing Rollback for Task: $task_id"
    echo "📋 Rollback Strategy: $rollback_strategy"
    
    # Phase 1: Immediate rollback
    immediate_rollback() {
        echo "⚡ Immediate Rollback Actions..."
        
        # Restore files from git
        git checkout HEAD -- $(get_task_affected_files "$task_id")
        
        # Clean any generated artifacts
        npm run clean
        
        # Restore dependencies if changed
        if [[ -f "package.json.backup" ]]; then
            mv package.json.backup package.json
            npm install
        fi
    }
    
    # Phase 2: Systematic rollback verification
    verify_rollback_success() {
        echo "✅ Verifying Rollback Success..."
        
        # Run comprehensive validation suite
        npm run type-check
        if [[ $? -ne 0 ]]; then
            echo "❌ Type check failed after rollback"
            return 1
        fi
        
        npm run lint
        if [[ $? -ne 0 ]]; then
            echo "❌ Lint check failed after rollback"
            return 1
        fi
        
        npm test -- --watchAll=false
        if [[ $? -ne 0 ]]; then
            echo "❌ Tests failed after rollback"
            return 1
        fi
        
        # Check service health if applicable
        if command -v curl >/dev/null 2>&1; then
            curl -f http://localhost:3000/health >/dev/null 2>&1
            if [[ $? -ne 0 ]]; then
                echo "⚠️ Service health check failed after rollback"
            fi
        fi
        
        echo "✅ Rollback verification successful"
        return 0
    }
    
    # Phase 3: Update progress tracking
    update_rollback_progress() {
        jq --arg task "$task_id" --arg timestamp "$(date -Iseconds)" \
           '.failed_tasks += [$task] | .rollback_completed += [$task] | .rollback_timestamp = $timestamp' \
           execution_progress.json > temp.json && mv temp.json execution_progress.json
    }
    
    # Execute rollback sequence
    immediate_rollback
    verify_rollback_success
    update_rollback_progress
    
    if [[ $? -eq 0 ]]; then
        echo "✅ Task rollback completed successfully"
        return 0
    else
        echo "❌ Task rollback failed - manual intervention required"
        return 1
    fi
}

# System-wide emergency rollback
execute_emergency_rollback() {
    echo "🚨 Executing Emergency System Rollback"
    
    # Rollback to pre-execution state
    echo "🔄 Rolling back to pre-execution state..."
    git reset --hard "pre-task-${task_name}-$(date +%Y%m%d)"
    git clean -fd
    
    # Restore dependencies
    npm install
    
    # Run comprehensive validation
    npm run build
    npm test
    
    # Update progress tracking
    jq '.status = "emergency_rollback" | .rollback_timestamp = "'$(date -Iseconds)'"' \
       execution_progress.json > temp.json && mv temp.json execution_progress.json
    
    echo "✅ Emergency rollback completed"
}
```

## Phase 4: Progress Tracking and Final Validation

### Comprehensive Progress Reporting
```typescript
// Generate detailed progress and execution reports
interface ExecutionReport {
  execution_summary: {
    task_name: string;
    execution_id: string;
    start_time: string;
    end_time: string;
    total_duration: string;
    overall_status: 'success' | 'partial_success' | 'failed' | 'rollback';
  };
  
  task_results: Array<{
    task_id: string;
    phase: string;
    status: 'success' | 'failed' | 'skipped' | 'rollback';
    execution_time: string;
    actions_completed: string[];
    validation_results: any[];
    error_details?: any;
  }>;
  
  quality_metrics: {
    tests_passed: number;
    tests_failed: number;
    code_coverage: string;
    performance_impact: string;
    security_scan_results: string;
  };
  
  final_validation: {
    all_tasks_completed: boolean;
    integration_tests_passed: boolean;
    regression_tests_passed: boolean;
    performance_benchmarks_met: boolean;
    security_validations_passed: boolean;
  };
  
  recommendations: {
    lessons_learned: string[];
    process_improvements: string[];
    technical_debt_identified: string[];
    follow_up_actions: string[];
  };
}

// Generate comprehensive execution report
async function generateExecutionReport(): Promise<ExecutionReport> {
  // Analyze execution progress and results
  // Calculate quality metrics and performance impact
  // Perform final validation and verification
  // Generate recommendations and insights
}
```

### Final Validation and Completion Verification
```bash
# Comprehensive final validation suite
execute_final_validation() {
    echo "🎯 Executing Final Validation Suite"
    
    # Phase 1: Core functionality validation
    echo "🔍 Core Functionality Validation..."
    npm run build
    if [[ $? -ne 0 ]]; then
        echo "❌ Build validation failed"
        return 1
    fi
    
    # Phase 2: Comprehensive testing
    echo "🧪 Comprehensive Test Suite..."
    npm test -- --watchAll=false --coverage
    npm run test:integration
    npm run test:e2e
    
    # Phase 3: Performance validation
    echo "📊 Performance Validation..."
    npm run test:performance
    
    # Phase 4: Security validation
    echo "🛡️ Security Validation..."
    npm audit --audit-level moderate
    npm run security:scan
    
    # Phase 5: Quality metrics validation
    echo "📈 Quality Metrics Validation..."
    npm run analyze:quality
    npm run analyze:complexity
    
    # Phase 6: Final checklist verification
    echo "✅ Final Checklist Verification..."
    verify_final_checklist
    
    if [[ $? -eq 0 ]]; then
        echo "✅ All final validations passed"
        return 0
    else
        echo "❌ Final validation failed"
        return 1
    fi
}

# Verify completion checklist
verify_final_checklist() {
    echo "📋 Verifying Task Completion Checklist"
    
    local checklist_items=(
        "All tasks completed successfully"
        "No failing tests"
        "Code coverage maintained or improved"
        "Performance benchmarks met"
        "Security scans passed"
        "Documentation updated"
        "No regressions introduced"
    )
    
    for item in "${checklist_items[@]}"; do
        echo "Checking: $item"
        # Implement specific validation for each checklist item
        # Return failure if any item fails
    done
    
    echo "✅ Final checklist verification completed"
    return 0
}
```

### Execution Report Generation
Use `create_file` to generate comprehensive execution report:

```markdown
# Task Execution Report: {task_name}

## 📋 Execution Summary
- **Task Name**: {task_name}
- **Execution ID**: {execution_id}
- **Start Time**: {start_time}
- **End Time**: {end_time}
- **Total Duration**: {total_duration}
- **Overall Status**: {overall_status}

## 🎯 Task Results

### Phase-by-Phase Results
{detailed_phase_by_phase_execution_results}

### Individual Task Results
{comprehensive_task_by_task_results}

## 📊 Quality Metrics

### Test Results
- **Tests Passed**: {tests_passed}
- **Tests Failed**: {tests_failed}
- **Code Coverage**: {code_coverage}

### Performance Impact
{performance_impact_analysis}

### Security Validation
{security_scan_results}

## ✅ Final Validation Results

### Completion Verification
{final_validation_checklist_results}

### Integration Testing
{integration_test_results}

### Regression Testing
{regression_test_results}

## 🚨 Issues and Resolutions

### Encountered Issues
{issues_faced_during_execution}

### Resolution Strategies
{how_issues_were_resolved}

### Rollback Actions
{any_rollback_actions_taken}

## 🎓 Lessons Learned

### Successful Approaches
{what_worked_well}

### Areas for Improvement
{identified_improvement_opportunities}

### Process Enhancements
{recommended_process_improvements}

## 🚀 Next Steps and Recommendations

### Follow-up Actions
{required_follow_up_actions}

### Technical Debt
{technical_debt_identified}

### Future Optimizations
{potential_future_optimizations}

---
*This execution report documents the complete task implementation and validation process.*
```

## Success Criteria

The task PRP execution is complete when:
- [ ] Task PRP is fully loaded and understood
- [ ] Execution environment is properly prepared
- [ ] All tasks are executed with validation
- [ ] Error handling and recovery procedures work
- [ ] Progress tracking provides visibility
- [ ] Final validation suite passes completely
- [ ] Quality metrics meet or exceed targets
- [ ] Comprehensive execution report is generated
- [ ] All rollback procedures are tested and ready
- [ ] Knowledge and lessons learned are captured

## Integration Points

### Development Workflow Integration
- Connect with existing development and review processes
- Integrate with continuous integration and deployment pipelines
- Align with team communication and collaboration practices
- Ensure compatibility with project management systems

### Quality Assurance Integration
- Leverage existing testing frameworks and validation procedures
- Integrate with code quality tools and automated checking systems
- Connect to monitoring and observability infrastructure
- Align with organizational quality standards

### Knowledge Management Integration
- Contribute execution patterns to organizational knowledge base
- Share lessons learned with development teams
- Document best practices for task execution
- Update process documentation and procedures

Remember: Focus on systematic execution with comprehensive validation at each step while maintaining safety, quality, and progress visibility throughout the process.
```

## Notes
- Emphasize systematic execution with comprehensive validation
- Focus on error handling and recovery procedures
- Provide detailed progress tracking and reporting
- Include comprehensive rollback and safety measures
