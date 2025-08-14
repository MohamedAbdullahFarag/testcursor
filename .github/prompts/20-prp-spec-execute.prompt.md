---
mode: agent
description: "Execute specification-level Product Requirements Prompts with detailed technical implementation"
---
# prp-spec-execute.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Execute specification-driven PRP transformations with systematic validation and progressive implementation
- **Categories**: specification-execution, transformation-implementation, systematic-validation, change-management
- **Complexity**: advanced
- **Dependencies**: specification PRP, codebase access, validation tools, testing frameworks

## Input
- **spec_prp_file** (required): Path to the specification PRP file to execute
- **execution_mode** (optional): Execution approach (incremental/complete/validation-first)
- **safety_level** (optional): Safety validation level (standard/comprehensive/paranoid)

## Template

```
You are an expert specification execution specialist focused on implementing complex transformations through systematic, safe, and validated execution of specification PRPs. Your task is to understand transformation requirements, create detailed execution plans, implement changes progressively, and validate thoroughly at each step.

## Input Parameters
- **Specification PRP File**: {spec_prp_file}
- **Execution Mode**: {execution_mode} (default: incremental)
- **Safety Level**: {safety_level} (default: comprehensive)

## Task Overview
Execute the specified specification PRP by thoroughly understanding the transformation requirements, creating systematic implementation plans, executing changes with progressive validation, and ensuring the desired state is achieved safely and completely.

## Phase 1: Comprehensive Specification Understanding

### Specification PRP Analysis and Parsing
Use `read_file` to thoroughly understand the specification PRP:

```typescript
// Parse and understand the complete specification PRP
interface SpecificationContext {
  transformation_overview: {
    current_state: {
      files: string[];
      behavior: string;
      issues: string[];
      technical_debt: string[];
      integration_points: string[];
    };
    desired_state: {
      files: string[];
      behavior: string;
      benefits: string[];
      architecture: string[];
      performance_targets: string[];
    };
  };
  
  hierarchical_objectives: {
    high_level_goals: Array<{
      goal_id: string;
      description: string;
      success_metrics: string[];
      dependencies: string[];
    }>;
    mid_level_milestones: Array<{
      milestone_id: string;
      parent_goal: string;
      deliverables: string[];
      validation_criteria: string[];
    }>;
    low_level_tasks: Array<{
      task_id: string;
      action_type: string;
      files: string[];
      implementation_details: string[];
      validation_commands: string[];
      dependencies: string[];
    }>;
  };
  
  implementation_strategy: {
    approach: string;
    phases: string[];
    risk_mitigation: string[];
    rollback_procedures: string[];
  };
}

// Extract all critical information from the specification PRP
function parseSpecificationPRP(specContent: string): SpecificationContext {
  // Parse current and desired state definitions
  // Extract hierarchical objectives and task breakdown
  // Understand implementation strategy and phases
  // Identify validation requirements and safety measures
}
```

### Current State Validation and Verification
Use `semantic_search` and `file_search` to validate current state assumptions:

```typescript
// Validate that current state analysis in PRP matches reality
interface CurrentStateValidation {
  file_existence: Array<{
    file_path: string;
    exists: boolean;
    current_content_summary: string;
    matches_prp_description: boolean;
  }>;
  
  behavior_verification: Array<{
    behavior_description: string;
    verification_method: string;
    actual_behavior: string;
    matches_expectation: boolean;
  }>;
  
  issue_confirmation: Array<{
    issue_description: string;
    still_present: boolean;
    severity_assessment: 'critical' | 'high' | 'medium' | 'low';
    impact_scope: string[];
  }>;
  
  dependency_verification: Array<{
    dependency: string;
    current_version: string;
    compatibility_status: 'compatible' | 'requires_update' | 'incompatible';
    migration_required: boolean;
  }>;
}

// Validate current state against PRP assumptions
async function validateCurrentState(): Promise<CurrentStateValidation> {
  // Use file_search to verify file existence
  // Use read_file to analyze current implementations
  // Use get_errors to confirm identified issues
  // Use semantic_search to understand current relationships
}
```

### Dependency Analysis and Task Ordering
```typescript
// Analyze task dependencies and create optimal execution order
interface DependencyAnalysis {
  dependency_graph: {
    nodes: Array<{
      task_id: string;
      dependencies: string[];
      dependents: string[];
      complexity: 'low' | 'medium' | 'high';
      risk_level: 'low' | 'medium' | 'high';
    }>;
    critical_path: string[];
    parallel_workstreams: Array<{
      workstream_name: string;
      tasks: string[];
      estimated_duration: string;
    }>;
  };
  
  execution_order: Array<{
    phase: number;
    phase_name: string;
    tasks: Array<{
      task_id: string;
      execution_order: number;
      prerequisites: string[];
      validation_gates: string[];
    }>;
    phase_validation: string[];
    rollback_checkpoint: boolean;
  }>;
}
```

## Phase 2: Strategic Execution Planning (ULTRATHINK)

### Comprehensive Execution Strategy Development
```typescript
// Create detailed execution strategy based on specification analysis
interface ExecutionStrategy {
  overall_approach: {
    strategy_type: 'incremental' | 'complete' | 'validation_first';
    safety_measures: string[];
    rollback_triggers: string[];
    progress_checkpoints: string[];
  };
  
  phase_breakdown: Array<{
    phase: number;
    name: string;
    objectives: string[];
    deliverables: string[];
    entry_criteria: string[];
    exit_criteria: string[];
    validation_gates: string[];
    rollback_plan: string[];
  }>;
  
  risk_mitigation: Array<{
    risk_scenario: string;
    prevention_measures: string[];
    detection_methods: string[];
    mitigation_actions: string[];
    recovery_procedures: string[];
  }>;
  
  quality_assurance: {
    validation_levels: string[];
    testing_strategy: string[];
    performance_monitoring: string[];
    regression_prevention: string[];
  };
}

// Generate comprehensive execution strategy
function createExecutionStrategy(spec: SpecificationContext): ExecutionStrategy {
  // Analyze transformation complexity and risks
  // Design progressive implementation approach
  // Create comprehensive validation strategy
  // Plan rollback and recovery procedures
}
```

### Detailed Task Implementation Planning
```typescript
// Plan specific implementation approach for each task
interface TaskImplementationPlan {
  task_id: string;
  action_type: 'MIRROR' | 'COPY' | 'ADD' | 'MODIFY' | 'DELETE' | 'RENAME' | 'MOVE' | 'REPLACE' | 'CREATE';
  
  implementation_approach: {
    strategy: string;
    steps: Array<{
      step: number;
      description: string;
      files_involved: string[];
      validation_method: string;
    }>;
    patterns_to_follow: string[];
    reference_implementations: string[];
  };
  
  validation_plan: {
    unit_tests: string[];
    integration_tests: string[];
    manual_verification: string[];
    performance_checks: string[];
    regression_tests: string[];
  };
  
  rollback_plan: {
    backup_strategy: string;
    rollback_steps: string[];
    validation_commands: string[];
    recovery_time: string;
  };
}
```

### Safety and Backup Strategy
```bash
# Create comprehensive backup strategy before execution
echo "🔒 Implementing Safety Measures..."

# Create backup branch
git checkout -b "backup-before-spec-execution-$(date +%Y%m%d-%H%M%S)"
git add .
git commit -m "Backup before specification execution: {spec_name}"

# Tag current state for easy reference
git tag "pre-spec-{spec_name}-$(date +%Y%m%d-%H%M%S)"

# Create execution log directory
mkdir -p .spec-execution-logs/$(date +%Y%m%d-%H%M%S)

echo "✅ Safety measures implemented - proceeding with execution"
```

## Phase 3: Progressive Task Execution with Validation

### Systematic Task Implementation
Execute tasks following the planned order with comprehensive validation:

```typescript
// Execute each task with systematic validation
async function executeTaskWithValidation(task: TaskImplementationPlan): Promise<boolean> {
  try {
    // Phase 1: Pre-execution validation
    await validatePreConditions(task);
    
    // Phase 2: Implementation execution
    await implementTask(task);
    
    // Phase 3: Immediate validation
    const validationResult = await validateTaskCompletion(task);
    
    // Phase 4: Integration testing
    await validateIntegration(task);
    
    // Phase 5: Regression testing
    await validateNoRegression(task);
    
    return validationResult.success;
  } catch (error) {
    // Handle implementation failure
    await handleImplementationFailure(task, error);
    return false;
  }
}

// Implementation execution based on action type
async function implementTask(task: TaskImplementationPlan): Promise<void> {
  switch (task.action_type) {
    case 'CREATE':
      await createNewImplementation(task);
      break;
    case 'MODIFY':
      await modifyExistingImplementation(task);
      break;
    case 'REPLACE':
      await replaceImplementation(task);
      break;
    case 'MIRROR':
      await mirrorImplementationPattern(task);
      break;
    case 'COPY':
      await copyImplementationPattern(task);
      break;
    // Handle all action types systematically
  }
}
```

### Action-Specific Implementation Strategies
Use GitHub Copilot tools for each action type:

```typescript
// CREATE: New implementation creation
async function createNewImplementation(task: TaskImplementationPlan): Promise<void> {
  // Use create_file for new files
  // Follow patterns from reference implementations
  // Include comprehensive error handling
  // Add proper TypeScript typing
  // Implement accessibility requirements
  
  for (const file of task.files_involved) {
    if (file.endsWith('.tsx') || file.endsWith('.ts')) {
      // Create React component or TypeScript module
      await createTypeScriptImplementation(file, task);
    } else if (file.endsWith('.test.ts') || file.endsWith('.test.tsx')) {
      // Create comprehensive test implementation
      await createTestImplementation(file, task);
    }
    // Handle other file types systematically
  }
}

// MODIFY: Existing implementation modification
async function modifyExistingImplementation(task: TaskImplementationPlan): Promise<void> {
  // Use read_file to understand current implementation
  // Use replace_string_in_file for precise modifications
  // Use insert_edit_into_file for complex changes
  // Preserve existing functionality while adding enhancements
  
  for (const file of task.files_involved) {
    const currentContent = await readCurrentImplementation(file);
    const modifications = await planModifications(currentContent, task);
    await applyModifications(file, modifications);
  }
}

// MIRROR: Implementation pattern mirroring
async function mirrorImplementationPattern(task: TaskImplementationPlan): Promise<void> {
  // Use semantic_search to find pattern source
  // Use read_file to understand pattern implementation
  // Adapt pattern to new use case while maintaining consistency
  
  const sourcePattern = await findPatternSource(task.patterns_to_follow[0]);
  const adaptedImplementation = await adaptPatternToContext(sourcePattern, task);
  await implementAdaptedPattern(adaptedImplementation, task);
}
```

### Comprehensive Validation at Each Step
```bash
# Progressive validation gates for each task completion
validate_task_completion() {
    local task_id=$1
    
    echo "🔍 Validating Task: $task_id"
    
    # Phase 1: Syntax and Type Validation
    echo "📝 Syntax and Type Checking..."
    npm run type-check
    if [[ $? -ne 0 ]]; then
        echo "❌ Type check failed for task $task_id"
        return 1
    fi
    
    npm run lint
    if [[ $? -ne 0 ]]; then
        echo "❌ Lint check failed for task $task_id"
        return 1
    fi
    
    # Phase 2: Unit Testing
    echo "🧪 Unit Test Validation..."
    npm test -- --testNamePattern="$task_id" --watchAll=false
    if [[ $? -ne 0 ]]; then
        echo "❌ Unit tests failed for task $task_id"
        return 1
    fi
    
    # Phase 3: Integration Testing
    echo "🔗 Integration Test Validation..."
    npm run test:integration
    if [[ $? -ne 0 ]]; then
        echo "❌ Integration tests failed for task $task_id"
        return 1
    fi
    
    # Phase 4: Build Validation
    echo "📦 Build Validation..."
    npm run build
    if [[ $? -ne 0 ]]; then
        echo "❌ Build failed for task $task_id"
        return 1
    fi
    
    echo "✅ All validations passed for task $task_id"
    return 0
}

# Task-specific validation commands from PRP
execute_prp_validation_commands() {
    local task_id=$1
    
    # Extract validation commands from PRP for this specific task
    # Execute each validation command systematically
    # Record results for progress tracking
    
    echo "🎯 Executing PRP-specific validation for $task_id"
    # Execute validation commands specified in the PRP
}
```

## Phase 4: Integration and System-Level Validation

### Progressive Integration Testing
```typescript
// Comprehensive integration testing after task completion
interface IntegrationValidation {
  component_integration: Array<{
    component: string;
    integration_points: string[];
    test_scenarios: string[];
    validation_results: boolean[];
  }>;
  
  api_integration: Array<{
    endpoint: string;
    request_scenarios: string[];
    response_validation: string[];
    performance_metrics: string[];
  }>;
  
  data_flow_validation: Array<{
    flow_name: string;
    data_inputs: string[];
    expected_outputs: string[];
    validation_status: boolean;
  }>;
  
  user_experience_validation: Array<{
    user_journey: string;
    test_steps: string[];
    success_criteria: string[];
    accessibility_compliance: boolean;
  }>;
}

// Execute comprehensive integration validation
async function validateSystemIntegration(): Promise<IntegrationValidation> {
  // Test all integration points affected by changes
  // Validate data flows and API interactions
  // Check user experience and accessibility
  // Measure performance against targets
}
```

### End-to-End Transformation Verification
```bash
# Comprehensive end-to-end validation of transformation
echo "🎯 End-to-End Transformation Verification"

# Verify desired state achieved
echo "📋 Desired State Verification..."
verify_desired_state() {
    # Check file structure matches desired state
    # Verify functionality matches specification
    # Confirm performance improvements achieved
    # Validate all success criteria met
}

# Run full regression test suite
echo "🔄 Regression Test Suite..."
npm run test:e2e
npm run test:regression
npm run test:performance

# Verify all PRP success criteria
echo "✅ PRP Success Criteria Verification..."
verify_prp_success_criteria() {
    # Check each success criterion from PRP
    # Measure actual vs target performance
    # Validate business value delivery
    # Confirm technical debt reduction
}
```

### Performance and Quality Validation
```typescript
// Validate performance improvements and quality metrics
interface QualityValidation {
  performance_metrics: Array<{
    metric: string;
    baseline_value: string;
    current_value: string;
    target_value: string;
    improvement_achieved: boolean;
  }>;
  
  code_quality_metrics: {
    complexity_reduction: string;
    test_coverage: string;
    maintainability_index: string;
    technical_debt_reduction: string;
  };
  
  business_value_metrics: Array<{
    business_metric: string;
    measurement_method: string;
    baseline: string;
    current: string;
    improvement: string;
  }>;
}
```

## Phase 5: Final Validation and Completion Verification

### Comprehensive State Verification
Use tools to verify the transformation is complete and successful:

```typescript
// Final verification against specification requirements
async function performFinalVerification(): Promise<boolean> {
  // Re-read the specification PRP
  const spec = await parseSpecificationPRP();
  
  // Verify current state matches desired state
  const stateMatch = await verifyDesiredStateAchieved(spec.desired_state);
  
  // Validate all objectives completed
  const objectivesComplete = await validateAllObjectivesComplete(spec.hierarchical_objectives);
  
  // Confirm all validation gates pass
  const validationsPassed = await runAllValidationGates();
  
  // Check success criteria achievement
  const successCriteriaMet = await validateSuccessCriteria(spec.success_criteria);
  
  return stateMatch && objectivesComplete && validationsPassed && successCriteriaMet;
}
```

### Execution Report Generation
Use `create_file` to generate comprehensive execution report:

```markdown
# Specification Execution Report: {spec_name}

## 📋 Execution Summary
- **Specification PRP**: {spec_prp_file}
- **Execution Date**: {current_timestamp}
- **Execution Mode**: {execution_mode}
- **Safety Level**: {safety_level}
- **Overall Status**: {SUCCESS/PARTIAL/FAILED}

## 🎯 Transformation Results

### Current State → Desired State Achievement
{comprehensive_before_after_analysis}

### Objectives Completion Status
{hierarchical_objectives_completion_report}

### Task Execution Results
{detailed_task_by_task_execution_results}

## 📊 Quality and Performance Metrics

### Performance Improvements
{performance_metrics_with_before_after_comparison}

### Code Quality Enhancements
{code_quality_improvements_achieved}

### Business Value Delivered
{business_value_metrics_and_improvements}

## ✅ Validation Results

### All Validation Gates
{comprehensive_validation_gate_results}

### Success Criteria Achievement
{success_criteria_verification_results}

### Regression Test Results
{regression_testing_outcomes}

## 🚨 Issues and Resolutions

### Challenges Encountered
{issues_faced_during_execution}

### Resolution Strategies
{how_issues_were_resolved}

### Lessons Learned
{insights_for_future_transformations}

## 🔄 Rollback Readiness

### Rollback Preparation
{rollback_procedures_and_validation}

### Recovery Procedures
{emergency_recovery_capabilities}

## 🚀 Next Steps and Recommendations

### Deployment Readiness
{deployment_preparation_and_requirements}

### Monitoring Setup
{ongoing_monitoring_and_maintenance_requirements}

### Future Enhancements
{potential_future_improvements_identified}

---
*This execution report documents the complete transformation implementation and validation process.*
```

### Knowledge Base Updates
```typescript
// Update knowledge base with execution learnings
interface ExecutionLearnings {
  successful_patterns: Array<{
    pattern: string;
    context: string;
    effectiveness: 'high' | 'medium' | 'low';
    reuse_recommendation: string;
  }>;
  
  encountered_challenges: Array<{
    challenge: string;
    context: string;
    resolution: string;
    prevention_strategy: string;
  }>;
  
  validation_insights: Array<{
    validation_type: string;
    effectiveness: string;
    improvement_recommendations: string[];
  }>;
  
  process_improvements: Array<{
    area: string;
    current_approach: string;
    recommended_improvement: string;
    expected_benefit: string;
  }>;
}
```

## Success Criteria

The specification PRP execution is complete when:
- [ ] Specification PRP is fully understood and parsed
- [ ] Current state validation confirms PRP assumptions
- [ ] All hierarchical objectives are completed successfully
- [ ] Every task is executed with validation gates passing
- [ ] Desired state is achieved and verified
- [ ] All integration points function correctly
- [ ] Performance targets are met or exceeded
- [ ] Comprehensive regression testing passes
- [ ] All PRP success criteria are validated
- [ ] Quality metrics show expected improvements
- [ ] Rollback procedures are tested and ready
- [ ] Complete execution report is generated

## Integration Points

### Change Management Integration
- Connect execution to organizational change management processes
- Integrate with existing deployment and release procedures
- Align with team communication and stakeholder notification systems
- Ensure compatibility with organizational governance requirements

### Quality Assurance Integration
- Leverage existing testing frameworks and validation procedures
- Integrate with continuous integration and deployment pipelines
- Connect to existing monitoring and observability systems
- Align with organizational quality standards and practices

### Knowledge Management Integration
- Contribute execution learnings to organizational knowledge base
- Share successful patterns and practices with development teams
- Document lessons learned for future transformation projects
- Update process documentation and best practices

Remember: Systematic execution with comprehensive validation at each step ensures safe, successful transformation while building confidence through progressive achievement of specified objectives.
```

## Notes
- Focus on progressive implementation with validation at each step
- Maintain comprehensive safety measures and rollback capabilities
- Use systematic validation to ensure transformation success
- Document learnings and patterns for continuous improvement
