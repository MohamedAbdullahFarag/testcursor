---
mode: agent
description: "Create task-specific Product Requirements Prompts with granular implementation steps"
---

---
inputs:
  - name: task_name
    description: Name of the specific task to create PRP for
    required: true
  - name: task_scope
    description: Task scope (component, service, integration, testing)
    required: false
  - name: complexity_level
    description: Task complexity (simple, medium, complex)
    required: false
    default: "medium"
---
# prp-task-create.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Generate comprehensive task lists for focused changes with systematic validation and dependency management
- **Categories**: task-decomposition, change-management, validation-planning, dependency-mapping
- **Complexity**: advanced
- **Dependencies**: PRP template system, file analysis tools, pattern recognition, validation frameworks

## Input
- **task_description** (required): Description of the task or change to be implemented
- **scope_preference** (optional): Scope preference (minimal/focused/comprehensive)
- **validation_level** (optional): Validation depth (basic/standard/comprehensive)

## Template

```
You are an expert task decomposition specialist focused on breaking down complex development tasks into manageable, validated, and sequenced implementation steps. Your goal is to create comprehensive task PRPs that ensure successful implementation through detailed planning, pattern analysis, and systematic validation approaches.

## Input Parameters
- **Task Description**: {task_description}
- **Scope Preference**: {scope_preference} (default: focused)
- **Validation Level**: {validation_level} (default: comprehensive)

## Task Overview
Create a comprehensive task PRP that breaks down the requested change into specific, actionable tasks with clear validation steps, dependency mapping, and rollback procedures. Focus on creating small, focused changes with immediate validation while ensuring comprehensive coverage of all requirements.

## Phase 1: Comprehensive Scope Definition and Analysis

### Task Scope Discovery and Mapping
Use `semantic_search` and `file_search` to identify all affected components:

```typescript
// Comprehensive scope analysis for task implementation
interface TaskScopeAnalysis {
  primary_changes: Array<{
    file_path: string;
    change_type: 'create' | 'modify' | 'delete' | 'rename' | 'move';
    change_description: string;
    complexity_level: 'low' | 'medium' | 'high';
    estimated_effort: string;
  }>;
  
  secondary_impacts: Array<{
    file_path: string;
    impact_type: 'configuration' | 'test' | 'documentation' | 'dependency';
    impact_description: string;
    required_action: string;
  }>;
  
  dependency_mapping: {
    upstream_dependencies: Array<{
      component: string;
      dependency_type: 'hard' | 'soft' | 'optional';
      impact_if_changed: string;
      mitigation_strategy: string;
    }>;
    downstream_impacts: Array<{
      component: string;
      impact_severity: 'critical' | 'high' | 'medium' | 'low';
      impact_description: string;
      validation_required: string[];
    }>;
  };
  
  side_effect_analysis: Array<{
    potential_side_effect: string;
    likelihood: 'high' | 'medium' | 'low';
    impact_severity: 'critical' | 'high' | 'medium' | 'low';
    detection_method: string;
    prevention_strategy: string;
  }>;
  
  test_coverage_assessment: {
    existing_tests: string[];
    coverage_gaps: string[];
    required_new_tests: string[];
    integration_test_needs: string[];
  };
}

// Discover and analyze task scope systematically
async function analyzeTaskScope(taskDescription: string): Promise<TaskScopeAnalysis> {
  // Use semantic_search to find related code and patterns
  // Use file_search to identify all potentially affected files
  // Use grep_search to find usage patterns and dependencies
  // Use list_code_usages to understand impact scope
}
```

### Pattern Research and Convention Analysis
Use tools to identify existing patterns and conventions:

```typescript
// Research existing patterns for similar implementations
interface PatternResearch {
  similar_implementations: Array<{
    file_path: string;
    pattern_type: string;
    implementation_approach: string;
    key_insights: string[];
    reusable_components: string[];
    adaptation_required: string[];
  }>;
  
  convention_analysis: {
    naming_conventions: Array<{
      context: string;
      pattern: string;
      examples: string[];
    }>;
    architectural_patterns: Array<{
      pattern_name: string;
      usage_context: string;
      implementation_details: string[];
      best_practices: string[];
    }>;
    testing_patterns: Array<{
      test_type: string;
      pattern_structure: string;
      validation_approach: string[];
      coverage_expectations: string[];
    }>;
  };
  
  helper_functions: Array<{
    function_name: string;
    file_location: string;
    functionality: string;
    usage_pattern: string;
    integration_considerations: string[];
  }>;
  
  historical_changes: Array<{
    similar_change: string;
    implementation_approach: string;
    lessons_learned: string[];
    pitfalls_to_avoid: string[];
    success_factors: string[];
  }>;
}

// Research patterns and conventions systematically
async function researchImplementationPatterns(): Promise<PatternResearch> {
  // Use semantic_search to find similar implementations
  // Use grep_search to identify naming conventions
  // Use list_code_usages to understand helper functions
  // Analyze git history for similar changes
}
```

### Comprehensive Dependency and Integration Analysis
```bash
# Analyze dependencies and integration points
echo "🔍 Dependency and Integration Analysis"

analyze_dependencies() {
    echo "📦 Package Dependencies..."
    # Check package.json for dependency requirements
    # Analyze import statements and usage patterns
    # Identify version compatibility requirements
    
    echo "🔗 Code Dependencies..."
    # Map function and class dependencies
    # Identify interface and type dependencies
    # Check for circular dependency risks
    
    echo "🏗️ Architecture Dependencies..."
    # Analyze module and component relationships
    # Check service and repository dependencies
    # Identify configuration dependencies
    
    echo "🧪 Test Dependencies..."
    # Map test setup and teardown requirements
    # Identify mock and stub dependencies
    # Check test data and fixture requirements
}
```

## Phase 2: User Clarification and Requirements Validation

### Interactive Requirements Clarification
```typescript
// Gather comprehensive requirements and clarifications
interface RequirementsClarification {
  scope_confirmation: {
    confirmed_changes: string[];
    excluded_changes: string[];
    boundary_clarifications: string[];
    assumption_validations: string[];
  };
  
  acceptance_criteria: Array<{
    criterion: string;
    validation_method: string;
    success_metrics: string[];
    failure_conditions: string[];
  }>;
  
  deployment_considerations: {
    deployment_strategy: 'immediate' | 'phased' | 'feature_flag';
    rollback_requirements: string[];
    monitoring_needs: string[];
    communication_plan: string[];
  };
  
  constraint_identification: {
    technical_constraints: string[];
    business_constraints: string[];
    timeline_constraints: string[];
    resource_constraints: string[];
  };
  
  risk_tolerance: {
    acceptable_risk_level: 'low' | 'medium' | 'high';
    critical_failure_scenarios: string[];
    mitigation_requirements: string[];
    rollback_triggers: string[];
  };
}

// Generate clarification questions for comprehensive understanding
function generateClarificationQuestions(taskScope: TaskScopeAnalysis): string[] {
  return [
    // Scope clarification questions
    "Are the identified primary changes complete, or are there additional requirements?",
    "Should secondary impacts be addressed as part of this task or separately?",
    "Are there any components that should be explicitly excluded from changes?",
    
    // Acceptance criteria questions
    "What are the specific success criteria for this implementation?",
    "How will we validate that the changes meet business requirements?",
    "Are there any performance benchmarks that must be maintained?",
    
    // Deployment and risk questions
    "Are there any deployment timing constraints or dependencies?",
    "What is the acceptable downtime or risk tolerance for this change?",
    "Are there any rollback or recovery requirements?",
    
    // Integration questions
    "Are there any specific integration testing requirements?",
    "Should this change be coordinated with other ongoing developments?",
    "Are there any communication or documentation requirements?"
  ];
}
```

### Risk Assessment and Mitigation Planning
```typescript
// Comprehensive risk assessment for task implementation
interface RiskAssessment {
  technical_risks: Array<{
    risk_scenario: string;
    probability: 'high' | 'medium' | 'low';
    impact: 'critical' | 'high' | 'medium' | 'low';
    early_detection: string[];
    mitigation_strategies: string[];
    contingency_plans: string[];
  }>;
  
  business_risks: Array<{
    risk_scenario: string;
    business_impact: string;
    stakeholder_communication: string[];
    mitigation_approach: string[];
  }>;
  
  integration_risks: Array<{
    integration_point: string;
    failure_scenarios: string[];
    detection_methods: string[];
    rollback_procedures: string[];
  }>;
  
  timeline_risks: Array<{
    risk_factor: string;
    impact_on_delivery: string;
    mitigation_options: string[];
    escalation_triggers: string[];
  }>;
}
```

## Phase 3: Task PRP Generation and Structure Creation

### Context Section Generation
Use `create_file` to generate comprehensive context section:

```yaml
# Generate comprehensive context for task PRP
context:
  task_overview:
    description: "{comprehensive_task_description}"
inputs:
  - name: task_name
    description: Name of the specific task to create PRP for
    required: true
  - name: task_scope
    description: Task scope (component, service, integration, testing)
    required: false
  - name: complexity_level
    description: Task complexity (simple, medium, complex)
    required: false
    default: "medium"
    scope: "{scope_boundaries_and_limitations}"
    success_criteria: "{specific_success_metrics}"
    
  documentation_references:
    - url: "{api_documentation_links}"
      focus: "{specific_methods_or_concepts}"
      relevance: "{how_it_applies_to_task}"
    - url: "{architectural_documentation}"
      focus: "{design_patterns_and_principles}"
      relevance: "{implementation_guidance}"
    - url: "{business_requirements}"
      focus: "{acceptance_criteria}"
      relevance: "{validation_approach}"
      
  implementation_patterns:
    - file: "{reference_implementation_file}"
      pattern_type: "{architectural_pattern_type}"
      copy_approach: "{specific_patterns_to_follow}"
      adaptation_notes: "{required_modifications}"
    - file: "{similar_component_file}"
      pattern_type: "{component_pattern_type}"
      copy_approach: "{reusable_elements}"
      adaptation_notes: "{context_specific_changes}"
      
  critical_gotchas:
    - issue: "{specific_technical_challenge}"
      context: "{when_this_occurs}"
      fix: "{prevention_or_resolution_strategy}"
      validation: "{how_to_detect_and_verify}"
    - issue: "{integration_challenge}"
      context: "{dependency_or_timing_issue}"
      fix: "{coordination_or_sequencing_approach}"
      validation: "{integration_test_strategy}"
      
  dependency_considerations:
    - component: "{upstream_dependency}"
      relationship: "{dependency_type_and_strength}"
      impact: "{potential_changes_needed}"
      coordination: "{required_communication}"
    - component: "{downstream_impact}"
      relationship: "{impact_type_and_severity}"
      impact: "{validation_requirements}"
      coordination: "{stakeholder_notification}"
      
  validation_framework:
    unit_testing: "{unit_test_strategy_and_coverage}"
    integration_testing: "{integration_test_approach}"
    performance_testing: "{performance_validation_requirements}"
    security_testing: "{security_validation_needs}"
    user_acceptance: "{user_validation_approach}"
```

### Detailed Task Structure and Sequencing
```typescript
// Generate comprehensive task breakdown structure
interface TaskBreakdownStructure {
  setup_tasks: Array<{
    task_id: string;
    description: string;
    prerequisites: string[];
    actions: Array<{
      action_type: 'CREATE' | 'MODIFY' | 'CONFIGURE' | 'VALIDATE';
      target: string;
      operation: string;
      validation: string;
      rollback: string;
    }>;
    completion_criteria: string[];
  }>;
  
  core_implementation: Array<{
    task_id: string;
    description: string;
    dependencies: string[];
    actions: Array<{
      action_type: 'CREATE' | 'MODIFY' | 'REPLACE' | 'MIRROR' | 'COPY';
      target: string;
      operation: string;
      validation: string;
      if_fail: string;
      rollback: string;
    }>;
    intermediate_validation: string[];
    completion_criteria: string[];
  }>;
  
  integration_tasks: Array<{
    task_id: string;
    description: string;
    integration_points: string[];
    actions: Array<{
      action_type: 'CONNECT' | 'CONFIGURE' | 'VALIDATE' | 'TEST';
      target: string;
      operation: string;
      validation: string;
      performance_check: string;
      rollback: string;
    }>;
    integration_tests: string[];
    completion_criteria: string[];
  }>;
  
  comprehensive_validation: Array<{
    task_id: string;
    description: string;
    validation_scope: string[];
    actions: Array<{
      action_type: 'TEST' | 'BENCHMARK' | 'SECURITY_SCAN' | 'REGRESSION_TEST';
      target: string;
      operation: string;
      success_criteria: string;
      failure_recovery: string;
    }>;
    quality_gates: string[];
    completion_criteria: string[];
  }>;
  
  cleanup_and_finalization: Array<{
    task_id: string;
    description: string;
    cleanup_scope: string[];
    actions: Array<{
      action_type: 'REMOVE' | 'REFACTOR' | 'DOCUMENT' | 'DEPLOY';
      target: string;
      operation: string;
      validation: string;
      final_check: string;
    }>;
    documentation_requirements: string[];
    completion_criteria: string[];
  }>;
}

// Generate structured task breakdown
function generateTaskBreakdown(scope: TaskScopeAnalysis, patterns: PatternResearch): TaskBreakdownStructure {
  // Create setup tasks for prerequisites
  // Define core implementation tasks with validation
  // Plan integration tasks with comprehensive testing
  // Design validation tasks with quality gates
  // Include cleanup and finalization tasks
}
```

### Advanced Validation Strategy Design
```bash
# Comprehensive validation strategy implementation
design_validation_strategy() {
    echo "🧪 Designing Comprehensive Validation Strategy"
    
    # Unit Testing Strategy
    echo "📝 Unit Testing Approach..."
    cat > validation_strategy.md << 'EOF'
## Unit Testing Strategy

### Test Coverage Requirements
- [ ] All new functions have unit tests
- [ ] All modified functions have updated tests
- [ ] Edge cases and error conditions covered
- [ ] Mock strategies for external dependencies

### Test Execution Commands
```bash
# Run tests after each core task
npm test -- --testNamePattern="{task_pattern}" --watchAll=false

# Check coverage thresholds
npm run test:coverage -- --threshold="{coverage_requirement}"
```

### Validation Gates
- Unit tests must pass before proceeding to next task
- Coverage must meet or exceed existing levels
- No new linting errors introduced
- Type checking must pass without errors
EOF

    # Integration Testing Strategy
    echo "🔗 Integration Testing Approach..."
    cat >> validation_strategy.md << 'EOF'

## Integration Testing Strategy

### Integration Points Validation
- [ ] API endpoints respond correctly
- [ ] Database operations complete successfully
- [ ] Service integrations function properly
- [ ] Component interactions work as expected

### Integration Test Commands
```bash
# Run integration tests after task groups
npm run test:integration -- --testNamePattern="{integration_pattern}"

# API integration validation
curl -X POST http://localhost:3000/api/{endpoint} -d '{test_data}'

# Database integration check
npm run db:test:validate
```
EOF

    # Performance Testing Strategy
    echo "📊 Performance Validation Approach..."
    cat >> validation_strategy.md << 'EOF'

## Performance Testing Strategy

### Performance Benchmarks
- [ ] Response times within acceptable ranges
- [ ] Memory usage stays within limits
- [ ] No performance regression detected
- [ ] Scalability requirements met

### Performance Test Commands
```bash
# Benchmark critical paths
npm run test:performance -- --benchmark="{performance_target}"

# Memory usage analysis
npm run analyze:memory -- --threshold="{memory_limit}"

# Load testing for APIs
npm run test:load -- --concurrent="{user_count}" --duration="{test_duration}"
```
EOF

    # Security Testing Strategy
    echo "🛡️ Security Validation Approach..."
    cat >> validation_strategy.md << 'EOF'

## Security Testing Strategy

### Security Validation Checks
- [ ] Input validation and sanitization
- [ ] Authentication and authorization
- [ ] Data encryption and protection
- [ ] Vulnerability scanning results

### Security Test Commands
```bash
# Security vulnerability scan
npm audit --audit-level moderate

# OWASP dependency check
npm run security:scan

# Authentication flow validation
npm run test:auth -- --comprehensive
```
EOF
}
```

## Phase 4: Task Sequencing and Dependency Management

### Intelligent Task Ordering and Dependencies
```typescript
// Create optimal task sequencing with dependency management
interface TaskSequencing {
  execution_phases: Array<{
    phase_number: number;
    phase_name: string;
    phase_description: string;
    parallel_workstreams: Array<{
      workstream_name: string;
      tasks: string[];
      estimated_duration: string;
      resource_requirements: string[];
    }>;
    sequential_dependencies: Array<{
      task_id: string;
      depends_on: string[];
      reason: string;
      coordination_required: string[];
    }>;
    phase_validation: {
      entry_criteria: string[];
      progress_checkpoints: string[];
      exit_criteria: string[];
      rollback_triggers: string[];
    };
  }>;
  
  critical_path_analysis: {
    critical_tasks: string[];
    bottleneck_identification: string[];
    optimization_opportunities: string[];
    risk_mitigation_for_critical_path: string[];
  };
  
  parallel_execution_opportunities: Array<{
    parallel_group: string;
    tasks: string[];
    shared_resources: string[];
    coordination_requirements: string[];
    integration_checkpoints: string[];
  }>;
}

// Generate optimal task sequencing
function optimizeTaskSequencing(tasks: TaskBreakdownStructure): TaskSequencing {
  // Analyze task dependencies and constraints
  // Identify critical path and bottlenecks
  // Create parallel execution opportunities
  // Design validation checkpoints and gates
}
```

### Rollback and Recovery Planning
```bash
# Comprehensive rollback and recovery strategy
design_rollback_strategy() {
    echo "🔄 Designing Comprehensive Rollback Strategy"
    
    # Task-level rollback procedures
    create_task_rollback_procedures() {
        for task in "${core_tasks[@]}"; do
            echo "📋 Rollback Procedure for $task"
            cat > "rollback_${task}.md" << EOF
# Rollback Procedure: $task

## Automated Rollback Commands
\`\`\`bash
# Quick rollback for $task
git checkout -- {affected_files}
git clean -fd {affected_directories}

# Restore database state if applicable
npm run db:rollback -- --to-checkpoint="${task}_start"

# Restart services if needed
npm run services:restart
\`\`\`

## Validation After Rollback
\`\`\`bash
# Verify rollback success
npm test -- --testNamePattern="${task}_rollback_validation"
npm run lint
npm run type-check

# Check service health
curl -f http://localhost:3000/health
\`\`\`

## Manual Recovery Steps
1. {step_by_step_manual_recovery}
2. {verification_procedures}
3. {stakeholder_communication}

## Rollback Success Criteria
- [ ] All automated tests pass
- [ ] No linting or type errors
- [ ] Services respond to health checks
- [ ] Performance metrics within normal ranges
EOF
        done
    }
    
    # System-level recovery procedures
    create_system_recovery_procedures() {
        cat > "system_recovery.md" << 'EOF'
# System-Level Recovery Procedures

## Emergency Rollback
```bash
# Complete system rollback
git reset --hard {backup_commit_hash}
git clean -fd
npm install
npm run build
npm run test:critical
```

## Progressive Recovery
```bash
# Rollback task by task in reverse order
for task in $(echo "${completed_tasks[@]}" | tac); do
    echo "Rolling back $task..."
    source "rollback_${task}.sh"
    npm test -- --testNamePattern="${task}_validation"
done
```

## Recovery Validation
```bash
# Comprehensive system validation after recovery
npm run test:all
npm run test:integration
npm run test:performance
npm run security:scan
```
EOF
    }
}
```

## Phase 5: Quality Assurance and Documentation Generation

### Comprehensive Quality Checklist Generation
```typescript
// Generate detailed quality assurance checklist
interface QualityAssurance {
  code_quality_checks: Array<{
    check_category: string;
    specific_checks: Array<{
      check_description: string;
      validation_command: string;
      acceptance_criteria: string;
      failure_remediation: string;
    }>;
  }>;
  
  testing_quality_gates: Array<{
    test_type: string;
    coverage_requirements: string;
    quality_metrics: string[];
    validation_procedures: string[];
  }>;
  
  documentation_requirements: Array<{
    documentation_type: string;
    content_requirements: string[];
    audience: string;
    maintenance_procedures: string[];
  }>;
  
  deployment_readiness: Array<{
    readiness_category: string;
    validation_steps: string[];
    sign_off_requirements: string[];
    rollback_preparedness: string[];
  }>;
}

// Generate comprehensive quality checklist
function generateQualityChecklist(): string {
  return `
## Comprehensive Quality Checklist

### Code Quality and Standards
- [ ] All changes follow established coding conventions
- [ ] Code complexity remains within acceptable limits
- [ ] No code duplication or anti-patterns introduced
- [ ] All functions and classes have appropriate documentation
- [ ] TypeScript strict mode compliance maintained
- [ ] ESLint rules pass without warnings
- [ ] Prettier formatting applied consistently
- [ ] No dead code or unused imports remain

### Testing and Validation
- [ ] Unit tests cover all new functionality
- [ ] Unit tests cover all modified functionality
- [ ] Integration tests validate component interactions
- [ ] End-to-end tests verify user workflows
- [ ] Performance tests confirm no regression
- [ ] Security tests validate input sanitization
- [ ] Error handling tests cover failure scenarios
- [ ] Test coverage meets or exceeds existing levels

### Documentation and Communication
- [ ] API documentation updated for changes
- [ ] README files reflect new functionality
- [ ] Code comments explain complex logic
- [ ] Change log documents user-facing changes
- [ ] Team documentation updated for processes
- [ ] Stakeholder communication completed
- [ ] Knowledge transfer sessions conducted
- [ ] Runbooks updated for operational changes

### Deployment and Operations
- [ ] Configuration changes documented
- [ ] Environment variables updated
- [ ] Database migrations tested
- [ ] Deployment procedures validated
- [ ] Monitoring and alerting configured
- [ ] Rollback procedures tested
- [ ] Performance monitoring baseline established
- [ ] Security scanning completed without issues

### Business Value and Acceptance
- [ ] All acceptance criteria met
- [ ] Business stakeholder sign-off obtained
- [ ] User experience tested and validated
- [ ] Accessibility requirements satisfied
- [ ] Internationalization concerns addressed
- [ ] Performance targets achieved
- [ ] Security requirements satisfied
- [ ] Compliance requirements met
  `;
}
```

### Task PRP File Generation
Use `create_file` to create the complete task PRP:

```markdown
# Task PRP: {task_name}

## 📋 Task Overview

### Task Description
{comprehensive_task_description_with_context}

### Scope and Boundaries
{detailed_scope_definition_with_inclusions_and_exclusions}

### Success Criteria
{specific_measurable_success_criteria}

## 🎯 Context and Requirements

{generated_context_yaml_section}

## 📝 Task Breakdown Structure

### Phase 1: Setup and Prerequisites
{setup_tasks_with_detailed_actions}

### Phase 2: Core Implementation
{core_implementation_tasks_with_validation}

### Phase 3: Integration and Testing
{integration_tasks_with_comprehensive_validation}

### Phase 4: Validation and Quality Assurance
{validation_tasks_with_quality_gates}

### Phase 5: Cleanup and Finalization
{cleanup_tasks_with_documentation}

## 🔄 Execution Strategy

### Task Sequencing
{optimized_task_sequence_with_dependencies}

### Parallel Execution Opportunities
{parallel_workstreams_and_coordination}

### Critical Path Analysis
{critical_path_and_bottleneck_identification}

## ✅ Validation Framework

{comprehensive_validation_strategy_with_commands}

## 🚨 Risk Management

### Risk Assessment
{identified_risks_with_mitigation_strategies}

### Rollback Procedures
{detailed_rollback_and_recovery_procedures}

### Emergency Procedures
{emergency_response_and_escalation_procedures}

## 📊 Quality Assurance

{comprehensive_quality_checklist}

## 🚀 Deployment and Operations

### Deployment Strategy
{deployment_approach_and_procedures}

### Monitoring and Observability
{monitoring_setup_and_alerting_configuration}

### Post-Deployment Validation
{post_deployment_validation_and_monitoring}

## 📚 Knowledge Management

### Documentation Updates
{required_documentation_updates_and_maintenance}

### Team Knowledge Transfer
{knowledge_transfer_requirements_and_procedures}

### Lessons Learned Capture
{process_for_capturing_and_sharing_insights}

---
*This task PRP provides comprehensive guidance for safe, validated, and successful task implementation.*
```

## Success Criteria

The task PRP creation is complete when:
- [ ] Comprehensive scope analysis identifies all affected components
- [ ] Pattern research reveals relevant implementation approaches
- [ ] User clarification confirms requirements and constraints
- [ ] Task breakdown structure covers all necessary changes
- [ ] Validation strategy ensures quality at each step
- [ ] Rollback procedures provide safety and recovery options
- [ ] Quality checklist ensures comprehensive validation
- [ ] Documentation supports implementation and maintenance
- [ ] Risk assessment identifies and mitigates potential issues
- [ ] Task sequencing optimizes implementation efficiency

## Integration Points

### Development Workflow Integration
- Connect with existing development and review processes
- Integrate with continuous integration and deployment pipelines
- Align with team communication and collaboration practices
- Ensure compatibility with project management and tracking systems

### Quality Assurance Integration
- Leverage existing testing frameworks and validation procedures
- Integrate with code quality tools and automated checking systems
- Connect to existing monitoring and observability infrastructure
- Align with organizational quality standards and compliance requirements

### Knowledge Management Integration
- Contribute patterns and approaches to organizational knowledge base
- Share implementation strategies with development teams
- Document lessons learned for continuous improvement
- Update process documentation and best practices

Remember: Focus on creating small, focused changes with immediate validation while ensuring comprehensive coverage and systematic implementation approaches.
```

## Notes
- Emphasize comprehensive scope analysis and pattern research
- Focus on user clarification and requirements validation
- Create detailed task breakdown with systematic validation
- Include comprehensive rollback and recovery procedures
