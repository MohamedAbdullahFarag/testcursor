---
mode: agent
description: "Create detailed technical specifications for Product Requirements Prompts with implementation blueprints"
---

---
inputs:
  - name: feature_name
    description: Name of the feature to create specification for
    required: true
  - name: technical_scope
    description: Technical scope (frontend, backend, full-stack, integration)
    required: false
    default: "full-stack"
  - name: specification_depth
    description: Specification depth (overview, detailed, comprehensive)
    required: false
    default: "detailed"
---

# prp-spec-create.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Generate comprehensive specification-driven PRP with clear transformation goals and hierarchical task breakdown
- **Categories**: specification-creation, transformation-planning, migration-strategy, technical-debt-resolution
- **Complexity**: advanced
- **Dependencies**: codebase analysis, architectural assessment, migration planning

## Input
- **specification_description** (required): The specification or transformation goal to implement
- **current_state_scope** (optional): Scope of existing implementation to transform
- **transformation_type** (optional): Type of transformation (refactor/migration/enhancement/replacement)

## Template

```
You are an expert specification and transformation architect specializing in creating comprehensive PRPs that guide complex code transformations with precision and safety. Your task is to analyze current state, define desired state, and create detailed transformation roadmaps with hierarchical task breakdown and risk mitigation.

## Input Parameters
- **Specification Description**: {specification_description}
- **Current State Scope**: {current_state_scope}
- **Transformation Type**: {transformation_type} (default: enhancement)

## Task Overview
Generate a comprehensive specification-driven PRP that maps current state, defines desired state, creates hierarchical transformation objectives, and provides detailed task breakdown with validation gates and risk mitigation strategies.

## Phase 1: Current State Assessment and Analysis

### Comprehensive Current State Mapping
Use `semantic_search` and `grep_search` to analyze existing implementation:

```typescript
// Map current implementation comprehensively
interface CurrentStateAnalysis {
  affected_files: Array<{
    file_path: string;
    file_type: 'component' | 'service' | 'utility' | 'test' | 'config';
    current_functionality: string;
    dependencies: string[];
    dependents: string[];
    complexity_level: 'low' | 'medium' | 'high';
    technical_debt_level: 'low' | 'medium' | 'high';
  }>;
  
  current_behavior: {
    functionality_description: string;
    user_interactions: string[];
    business_logic_flows: string[];
    data_transformations: string[];
    integration_points: string[];
  };
  
  identified_issues: Array<{
    issue: string;
    category: 'performance' | 'maintainability' | 'security' | 'usability' | 'technical_debt';
    severity: 'critical' | 'high' | 'medium' | 'low';
    impact_scope: string[];
    root_cause: string;
  }>;
  
  technical_debt_assessment: {
    code_quality_issues: string[];
    architectural_problems: string[];
    performance_bottlenecks: string[];
    security_vulnerabilities: string[];
    maintenance_burden: string[];
  };
}

// Analyze current state systematically
function analyzeCurrentState(): CurrentStateAnalysis {
  // Use file_search to identify all relevant files
  // Use read_file to understand current implementation
  // Use get_errors to identify existing issues
  // Use semantic_search to understand relationships
}
```

### Integration Points and Dependency Analysis
Use `list_code_usages` to understand current integration points:

```typescript
// Map all integration points and dependencies
interface IntegrationAnalysis {
  internal_dependencies: Array<{
    dependency: string;
    usage_type: 'import' | 'service_call' | 'event_handler' | 'data_flow';
    coupling_level: 'tight' | 'loose' | 'none';
    change_impact: 'breaking' | 'non-breaking' | 'isolated';
  }>;
  
  external_dependencies: Array<{
    dependency: string;
    type: 'library' | 'api' | 'service' | 'database';
    version_constraints: string;
    migration_considerations: string[];
  }>;
  
  usage_patterns: Array<{
    component: string;
    usage_locations: string[];
    usage_patterns: string[];
    breaking_change_impact: string[];
  }>;
}
```

### Pain Point and Risk Identification
```typescript
// Identify specific pain points and transformation risks
interface PainPointAnalysis {
  functional_pain_points: Array<{
    area: string;
    description: string;
    user_impact: 'high' | 'medium' | 'low';
    business_impact: 'high' | 'medium' | 'low';
    frequency: 'daily' | 'weekly' | 'monthly' | 'occasional';
  }>;
  
  technical_pain_points: Array<{
    area: string;
    description: string;
    development_impact: 'high' | 'medium' | 'low';
    maintenance_burden: 'high' | 'medium' | 'low';
    scalability_constraint: boolean;
  }>;
  
  transformation_risks: Array<{
    risk: string;
    category: 'technical' | 'business' | 'timeline' | 'resource';
    probability: 'high' | 'medium' | 'low';
    impact: 'critical' | 'high' | 'medium' | 'low';
    early_indicators: string[];
  }>;
}
```

## Phase 2: Desired State Research and Definition

### Target State Architecture Research
Use `vscode_websearchforcopilot_webSearch` to research best practices and patterns:

```typescript
// Research optimal approaches for the transformation
interface DesiredStateResearch {
  best_practices: Array<{
    practice: string;
    context: string;
    benefits: string[];
    implementation_complexity: 'low' | 'medium' | 'high';
    adoption_examples: string[];
    documentation_urls: string[];
  }>;
  
  implementation_patterns: Array<{
    pattern: string;
    use_cases: string[];
    pros: string[];
    cons: string[];
    implementation_examples: string[];
    compatibility_requirements: string[];
  }>;
  
  migration_strategies: Array<{
    strategy: string;
    approach: 'big_bang' | 'incremental' | 'parallel_run' | 'strangler_fig';
    risk_level: 'low' | 'medium' | 'high';
    timeline_impact: string;
    rollback_complexity: 'easy' | 'moderate' | 'difficult';
  }>;
}

// Research queries for comprehensive understanding
const researchQueries = [
  'Best practices for {transformation_type}',
  'Migration strategies for {current_technology} to {target_technology}',
  'Common pitfalls in {transformation_domain}',
  'Performance considerations for {target_architecture}',
  'Testing strategies for {transformation_type}',
  'Rollback strategies for {change_type}',
  'Industry examples of {similar_transformations}'
];
```

### Comprehensive Desired State Definition
```typescript
// Define detailed target state with measurable criteria
interface DesiredStateDefinition {
  target_architecture: {
    structural_changes: string[];
    technology_stack: string[];
    design_patterns: string[];
    performance_characteristics: string[];
  };
  
  expected_functionality: {
    new_capabilities: string[];
    enhanced_features: string[];
    removed_limitations: string[];
    improved_user_experiences: string[];
  };
  
  quality_improvements: {
    performance_gains: Array<{
      metric: string;
      current_value: string;
      target_value: string;
      measurement_method: string;
    }>;
    
    maintainability_improvements: string[];
    security_enhancements: string[];
    scalability_improvements: string[];
  };
  
  success_criteria: Array<{
    criterion: string;
    measurement: string;
    target: string;
    validation_method: string;
    timeline: string;
  }>;
}
```

## Phase 3: Hierarchical Objectives and Task Breakdown

### Multi-Level Objective Structure
```typescript
// Create hierarchical objective breakdown with clear relationships
interface HierarchicalObjectives {
  high_level_goals: Array<{
    goal_id: string;
    description: string;
    business_value: string;
    success_metrics: string[];
    dependencies: string[];
    estimated_timeline: string;
  }>;
  
  mid_level_milestones: Array<{
    milestone_id: string;
    parent_goal: string;
    description: string;
    deliverables: string[];
    validation_criteria: string[];
    dependencies: string[];
    risk_factors: string[];
  }>;
  
  low_level_tasks: Array<{
    task_id: string;
    parent_milestone: string;
    action_type: 'MIRROR' | 'COPY' | 'ADD' | 'MODIFY' | 'DELETE' | 'RENAME' | 'MOVE' | 'REPLACE' | 'CREATE';
    description: string;
    implementation_details: string[];
    validation_commands: string[];
    rollback_procedure: string;
  }>;
}

// Create comprehensive objective hierarchy
function createObjectiveHierarchy(): HierarchicalObjectives {
  // Break down transformation into logical phases
  // Define clear dependencies between objectives
  // Assign action types for each task
  // Create validation criteria for each level
}
```

### Information-Dense Task Specification
```yaml
# Task specification with precise action keywords
task_specification_template:
  task_name:
    action: [MIRROR|COPY|ADD|MODIFY|DELETE|RENAME|MOVE|REPLACE|CREATE]
    target_files: [list of files to change]
    source_files: [reference files for patterns]
    implementation_details: |
      - Specific code changes required
      - Pattern applications and adaptations
      - Integration point modifications
      - Error handling enhancements
    validation_gates:
      - command: "validation command to execute"
        expected_result: "success criteria"
        failure_action: "remediation steps"
    dependencies:
      - task_ids: [prerequisite tasks]
      - external_deps: [external dependencies]
    rollback_plan:
      - steps: [rollback procedure]
      - validation: [rollback verification]

# Example task specifications:
refactor_component_architecture:
  action: MODIFY
  target_files: 
    - "src/components/UserProfile.tsx"
    - "src/hooks/useUserProfile.ts"
  source_files:
    - "src/components/UserCard.tsx" # Pattern reference
  implementation_details: |
    - Extract business logic from component to custom hook
    - Implement loading and error states following UserCard pattern
    - Add proper TypeScript interfaces matching existing patterns
    - Integrate with existing error boundary infrastructure
  validation_gates:
    - command: "npm run test:unit UserProfile"
      expected_result: "All tests passing with >80% coverage"
      failure_action: "Review test implementation and fix issues"
    - command: "npm run type-check"
      expected_result: "No TypeScript errors"
      failure_action: "Fix type definitions and interfaces"
  dependencies:
    task_ids: ["create_user_profile_types"]
    external_deps: ["@tanstack/react-query"]
  rollback_plan:
    steps: 
      - "Revert component to previous implementation"
      - "Remove custom hook file"
    validation: "npm test && npm run type-check"
```

## Phase 4: Implementation Strategy and Risk Management

### Progressive Implementation Strategy
```typescript
// Design safe, incremental implementation approach
interface ImplementationStrategy {
  approach: {
    strategy_type: 'big_bang' | 'incremental' | 'parallel_run' | 'strangler_fig';
    rationale: string;
    phases: Array<{
      phase: number;
      name: string;
      objectives: string[];
      deliverables: string[];
      validation_gates: string[];
      rollback_triggers: string[];
    }>;
  };
  
  dependency_management: {
    critical_path: string[];
    parallel_workstreams: Array<{
      workstream: string;
      tasks: string[];
      dependencies: string[];
    }>;
    dependency_graph: string; // Mermaid diagram
  };
  
  risk_mitigation: Array<{
    risk_category: string;
    mitigation_strategies: string[];
    monitoring_approach: string;
    escalation_criteria: string[];
    contingency_plans: string[];
  }>;
}
```

### Validation and Testing Strategy
```typescript
// Comprehensive validation approach for each transformation step
interface ValidationStrategy {
  unit_testing: {
    coverage_requirements: string;
    testing_patterns: string[];
    mock_strategies: string[];
    validation_commands: string[];
  };
  
  integration_testing: {
    integration_points: string[];
    test_scenarios: string[];
    validation_commands: string[];
    performance_criteria: string[];
  };
  
  end_to_end_validation: {
    user_journey_tests: string[];
    regression_test_suite: string[];
    performance_benchmarks: string[];
    validation_commands: string[];
  };
  
  rollback_validation: {
    rollback_triggers: string[];
    rollback_procedures: string[];
    validation_commands: string[];
    recovery_time_objectives: string[];
  };
}
```

## Phase 5: Comprehensive Specification PRP Generation

### Complete Specification PRP Creation
Use `create_file` to generate the comprehensive specification PRP:

```markdown
# Specification PRP: {specification_name}

## 🎯 Transformation Overview

### Current State Analysis
```yaml
current_state:
  files: {list_of_affected_files}
  behavior: {current_functionality_description}
  issues: {identified_problems_and_pain_points}
  technical_debt: {technical_debt_assessment}
  integration_points: {current_integration_analysis}
```

### Desired State Definition
```yaml
desired_state:
  files: {expected_file_structure}
  behavior: {target_functionality_description}
  benefits: {improvements_and_enhancements}
  architecture: {target_architectural_patterns}
  performance: {performance_improvement_targets}
```

## 🎯 Hierarchical Objectives

### High-Level Goals
{comprehensive_high_level_transformation_goals}

### Mid-Level Milestones
{detailed_milestone_breakdown_with_dependencies}

### Low-Level Task Specification
{information_dense_task_specifications_with_action_keywords}

## 🔧 Implementation Strategy

### Transformation Approach
{detailed_implementation_strategy_with_phases}

### Dependency Management
```mermaid
graph TD
{comprehensive_dependency_graph}
```

### Risk Mitigation
{comprehensive_risk_analysis_and_mitigation_strategies}

## ✅ Validation Framework

### Progressive Validation Gates
{validation_commands_and_success_criteria_for_each_phase}

### Quality Assurance Checks
{comprehensive_quality_validation_approach}

### Rollback Procedures
{detailed_rollback_strategies_and_validation}

## 📋 Detailed Task Breakdown

{comprehensive_task_specifications_using_action_keywords}

## 🚨 Risk Management

### Identified Risks
{comprehensive_risk_analysis_with_early_indicators}

### Mitigation Strategies
{detailed_risk_mitigation_and_contingency_plans}

### Monitoring and Escalation
{risk_monitoring_and_escalation_procedures}

## 🎯 Success Criteria and Validation

### Acceptance Criteria
{measurable_success_criteria_with_validation_methods}

### Performance Targets
{specific_performance_improvement_targets}

### Quality Metrics
{code_quality_and_architectural_improvement_metrics}

---
*This Specification PRP provides comprehensive transformation guidance with detailed task breakdown and risk mitigation.*
```

### Quality and Completeness Validation
```typescript
// Comprehensive quality checklist for specification PRP
interface SpecificationQualityCheck {
  state_documentation: {
    current_state_fully_mapped: boolean;
    desired_state_clearly_defined: boolean;
    gap_analysis_complete: boolean;
    integration_points_identified: boolean;
  };
  
  objective_clarity: {
    hierarchical_breakdown_logical: boolean;
    all_objectives_measurable: boolean;
    dependencies_clearly_mapped: boolean;
    success_criteria_specific: boolean;
  };
  
  implementation_guidance: {
    tasks_ordered_by_dependency: boolean;
    action_keywords_used_consistently: boolean;
    validation_commands_executable: boolean;
    rollback_strategies_included: boolean;
  };
  
  risk_management: {
    risks_comprehensively_identified: boolean;
    mitigation_strategies_actionable: boolean;
    monitoring_approach_defined: boolean;
    contingency_plans_detailed: boolean;
  };
}
```

## Phase 6: User Validation and Refinement

### Objective Validation Process
```typescript
// Validate objectives with stakeholders and gather feedback
interface ObjectiveValidation {
  stakeholder_review: {
    business_alignment: boolean;
    technical_feasibility: boolean;
    timeline_realistic: boolean;
    resource_requirements_acceptable: boolean;
  };
  
  priority_confirmation: {
    high_level_goal_priorities: string[];
    milestone_sequence_approved: boolean;
    task_dependencies_validated: boolean;
    success_criteria_agreed: boolean;
  };
  
  gap_identification: {
    missing_requirements: string[];
    unclear_specifications: string[];
    additional_risks: string[];
    constraint_updates: string[];
  };
}
```

### Risk Review and Go/No-Go Decision
```typescript
// Comprehensive risk review for transformation approval
interface RiskReviewProcess {
  risk_assessment_summary: {
    high_risk_items: Array<{
      risk: string;
      mitigation: string;
      residual_risk: 'acceptable' | 'manageable' | 'concerning';
    }>;
    
    medium_risk_items: string[];
    risk_tolerance_evaluation: string;
  };
  
  go_no_go_criteria: {
    technical_feasibility: 'go' | 'no-go' | 'conditional';
    business_value: 'high' | 'medium' | 'low';
    resource_availability: 'sufficient' | 'adequate' | 'insufficient';
    timeline_constraints: 'acceptable' | 'tight' | 'unrealistic';
    risk_tolerance: 'within_limits' | 'manageable' | 'excessive';
  };
  
  decision_recommendation: {
    recommendation: 'proceed' | 'defer' | 'modify_scope' | 'cancel';
    rationale: string;
    conditions: string[];
    next_steps: string[];
  };
}
```

## Success Criteria

The Specification PRP creation is complete when:
- [ ] Current state is comprehensively documented and analyzed
- [ ] Desired state is clearly defined with measurable benefits
- [ ] Hierarchical objectives are logically structured and dependent
- [ ] All tasks use information-dense action keywords consistently
- [ ] Each task has executable validation commands
- [ ] Implementation strategy includes progressive enhancement approach
- [ ] Comprehensive risk analysis with mitigation strategies is included
- [ ] Rollback procedures are detailed for each transformation step
- [ ] Integration points and dependencies are clearly mapped
- [ ] Success criteria are specific, measurable, and time-bound
- [ ] Quality validation checklist confirms completeness
- [ ] Stakeholder validation and approval is obtained

## Integration Points

### Development Workflow Integration
- Connect specification PRP to implementation execution workflow
- Integrate with existing change management and approval processes
- Align with team planning and resource allocation procedures
- Enable seamless transition from specification to implementation

### Quality Assurance Integration
- Establish validation gates that integrate with existing QA processes
- Create rollback procedures that align with deployment practices
- Design risk monitoring that connects with operational monitoring
- Ensure all validation commands are executable in existing infrastructure

### Risk Management Integration
- Connect risk assessment to organizational risk management frameworks
- Integrate monitoring and escalation with existing operational procedures
- Align contingency planning with business continuity processes
- Ensure risk mitigation strategies are actionable and resourced

Remember: Focus on the transformation journey with clear waypoints, not just the destination. Provide comprehensive guidance that enables confident execution of complex changes.
```

## Notes
- Use information-dense action keywords consistently for task specification
- Focus on hierarchical objective breakdown with clear dependencies
- Include comprehensive risk analysis and mitigation strategies
- Ensure all validation commands are executable by implementation teams
