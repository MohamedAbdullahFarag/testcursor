---
mode: agent
description: "Execute base-level Product Requirements Prompts with systematic implementation and validation"
---

---
inputs:
  - name: prp_file_path
    description: Path to the PRP file to execute
    required: true
  - name: execution_mode
    description: Execution mode (standard, debug, fast)
    required: false
    default: "standard"
  - name: validation_level
    description: Validation level (basic, comprehensive, expert)
    required: false
    default: "comprehensive"
---
# prp-base-execute.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Execute and implement features using comprehensive Product Requirements Prompt (PRP) files with systematic validation
- **Categories**: prp-execution, implementation, validation, quality-assurance
- **Complexity**: advanced
- **Dependencies**: PRP file, codebase access, testing frameworks, validation tools

## Input
- **prp_file** (required): Path to the PRP file to execute
- **validation_level** (optional): Level of validation to perform (basic/standard/comprehensive)
- **implementation_mode** (optional): Implementation approach (incremental/complete)

## Template

```
You are an expert implementation specialist focused on executing Product Requirements Prompts (PRPs) with systematic precision and comprehensive validation. Your task is to understand the PRP context completely, create detailed implementation plans, execute them methodically, and validate thoroughly to ensure successful feature delivery.

## Input Parameters
- **PRP File**: {prp_file}
- **Validation Level**: {validation_level} (default: comprehensive)
- **Implementation Mode**: {implementation_mode} (default: complete)

## Task Overview
Execute the specified PRP by thoroughly understanding its context, creating detailed implementation plans, implementing the feature systematically, and validating comprehensively to ensure all requirements are met with high quality.

## Phase 1: PRP Context Loading and Understanding

### Comprehensive PRP Analysis
Use `read_file` to thoroughly understand the PRP requirements:

```typescript
// Load and analyze the PRP file completely
interface PRPContext {
  feature_overview: string;
  implementation_requirements: string[];
  architecture_approach: string;
  technical_specifications: string[];
  validation_gates: string[];
  success_criteria: string[];
  context_documentation: string[];
  gotchas_and_pitfalls: string[];
  confidence_score: number;
}

// Extract all critical information from the PRP
function analyzePRPContent(prpContent: string): PRPContext {
  // Parse sections systematically
  // Extract implementation requirements
  // Identify validation gates
  // Note gotchas and patterns
  // Understand success criteria
}
```

### Context Validation and Enhancement
Use `semantic_search` and `grep_search` to validate and enhance PRP context:

```typescript
// Validate PRP context against current codebase state
interface ContextValidation {
  referenced_files_exist: boolean;
  patterns_still_valid: boolean;
  dependencies_available: boolean;
  examples_accessible: boolean;
  documentation_current: boolean;
}

// If context needs enhancement, perform additional research
async function enhanceContextIfNeeded() {
  // Use vscode_websearchforcopilot_webSearch for updated documentation
  // Use semantic_search for current codebase patterns
  // Use file_search to verify referenced files exist
  // Update understanding based on current state
}
```

### Requirements Deep Dive
```typescript
// Break down PRP requirements into actionable components
interface ImplementationRequirement {
  category: 'component' | 'hook' | 'service' | 'api' | 'utility' | 'test';
  description: string;
  files_to_create: string[];
  files_to_modify: string[];
  dependencies: string[];
  validation_criteria: string[];
  implementation_order: number;
}

// Create comprehensive requirement breakdown
function parseImplementationRequirements(prp: PRPContext): ImplementationRequirement[] {
  // Systematically extract all requirements
  // Organize by implementation order
  // Identify dependencies and relationships
  // Map to specific deliverables
}
```

## Phase 2: Strategic Implementation Planning (ULTRATHINK)

### Comprehensive Implementation Strategy
```typescript
// Create detailed implementation strategy based on PRP analysis
interface ImplementationStrategy {
  approach: 'incremental' | 'complete' | 'modular';
  phases: Array<{
    phase: number;
    name: string;
    objectives: string[];
    deliverables: string[];
    validation_points: string[];
    estimated_effort: string;
  }>;
  
  technical_approach: {
    architecture_pattern: string;
    component_structure: string;
    state_management: string;
    api_integration: string;
    testing_strategy: string;
  };
  
  risk_mitigation: Array<{
    risk: string;
    probability: 'high' | 'medium' | 'low';
    impact: 'high' | 'medium' | 'low';
    mitigation: string;
  }>;
}

// Generate comprehensive strategy before implementation
function createImplementationStrategy(requirements: ImplementationRequirement[]): ImplementationStrategy {
  // Analyze complexity and dependencies
  // Create logical implementation phases
  // Identify potential risks and mitigation strategies
  // Plan validation points throughout implementation
}
```

### Detailed Task Breakdown
```typescript
// Break down implementation into specific, actionable tasks
interface ImplementationTask {
  id: string;
  phase: number;
  title: string;
  description: string;
  type: 'create' | 'modify' | 'test' | 'validate' | 'integrate';
  files_involved: string[];
  dependencies: string[];
  acceptance_criteria: string[];
  validation_commands: string[];
  estimated_time: string;
}

// Create comprehensive task list from PRP requirements
function createDetailedTasks(strategy: ImplementationStrategy): ImplementationTask[] {
  // Break each phase into specific tasks
  // Ensure all PRP requirements are covered
  // Include validation tasks for each deliverable
  // Order tasks by dependencies and logic
}
```

### Pattern and Context Integration Planning
Use `semantic_search` to identify existing patterns to follow:

```typescript
// Identify existing patterns and conventions to follow
interface PatternAnalysis {
  component_patterns: Array<{
    pattern: string;
    files: string[];
    usage_examples: string[];
    conventions: string[];
  }>;
  
  naming_conventions: {
    components: string;
    hooks: string;
    services: string;
    types: string;
    tests: string;
  };
  
  architecture_patterns: {
    folder_structure: string;
    import_patterns: string;
    export_patterns: string;
    dependency_injection: string;
  };
}

// Extract patterns to ensure consistent implementation
function analyzeExistingPatterns(): PatternAnalysis {
  // Use semantic_search for similar implementations
  // Use file_search for naming conventions
  // Use grep_search for import/export patterns
  // Document conventions to follow
}
```

## Phase 3: Systematic Implementation Execution

### Phase-by-Phase Implementation
Execute implementation in systematic phases with validation at each step:

```typescript
// Phase 1: Foundation and Setup
async function implementFoundation() {
  // Create base file structure
  // Set up imports and dependencies
  // Create type definitions
  // Implement basic interfaces
  
  // Validate foundation
  await runValidation(['type-check', 'lint']);
}

// Phase 2: Core Implementation
async function implementCoreFeatures() {
  // Implement main components/hooks/services
  // Follow established patterns from PRP
  // Include proper error handling
  // Add comprehensive logging
  
  // Validate core implementation
  await runValidation(['type-check', 'lint', 'unit-tests']);
}

// Phase 3: Integration and Polish
async function implementIntegration() {
  // Integrate with existing systems
  // Implement edge case handling
  // Add performance optimizations
  // Complete documentation
  
  // Validate integration
  await runValidation(['integration-tests', 'e2e-tests']);
}
```

### Real-Time Context Validation
Throughout implementation, continuously validate against PRP context:

```typescript
// Validate implementation matches PRP requirements
interface ImplementationValidation {
  requirements_coverage: Array<{
    requirement: string;
    implemented: boolean;
    validation_notes: string;
  }>;
  
  pattern_adherence: Array<{
    pattern: string;
    followed: boolean;
    deviations: string[];
  }>;
  
  quality_metrics: {
    type_safety: boolean;
    error_handling: boolean;
    testing_coverage: number;
    performance_considerations: boolean;
  };
}

// Continuously validate against PRP during implementation
function validateAgainstPRP(): ImplementationValidation {
  // Check each requirement is addressed
  // Verify patterns are followed
  // Ensure quality standards are met
  // Document any deviations with rationale
}
```

### Code Implementation with Error Handling
Use GitHub Copilot tools for implementation:

```typescript
// Implement each component with comprehensive error handling
// Example for a React component implementation:

// Use insert_edit_into_file for new components
const componentImplementation = `
// Component following PRP specifications
import React, { useState, useCallback, useMemo } from 'react';
import { useQuery, useMutation } from '@tanstack/react-query';
// Additional imports based on PRP requirements

interface {ComponentName}Props {
  // Props based on PRP specifications
}

export const {ComponentName}: React.FC<{ComponentName}Props> = memo(({ ... }) => {
  // Implementation following PRP patterns
  // Include error boundaries and loading states
  // Follow accessibility guidelines
  // Implement proper TypeScript typing
});

// Error boundary for component
export class {ComponentName}ErrorBoundary extends React.Component {
  // Comprehensive error handling
}
`;

// Use replace_string_in_file for modifications
// Use create_file for new files
// Use get_errors to validate implementations
```

## Phase 4: Comprehensive Validation Execution

### Systematic Validation Gate Execution
Execute all validation gates specified in the PRP:

```bash
# Phase 1: Syntax and Type Validation
npm run type-check
npm run lint
npm run format:check

# Fix any issues found before proceeding
if [[ $? -ne 0 ]]; then
  echo "❌ Syntax/Type validation failed - fixing issues..."
  npm run lint -- --fix
  npm run format
  npm run type-check
fi

# Phase 2: Unit Testing Validation
npm test -- --coverage --watchAll=false
npm run test:unit

# Ensure test coverage meets requirements
COVERAGE=$(npm test -- --coverage --silent | grep "All files" | awk '{print $10}')
if [[ ${COVERAGE%\%} -lt 80 ]]; then
  echo "❌ Test coverage below 80% - adding more tests..."
  # Add additional tests as needed
fi

# Phase 3: Integration Testing
npm run test:integration
npm run test:e2e

# Phase 4: Build and Bundle Validation
npm run build
if [[ $? -ne 0 ]]; then
  echo "❌ Build failed - fixing build issues..."
  # Fix build issues
fi

# Phase 5: Quality Gates
npm run audit
npm run test:a11y
npm run test:performance

# Phase 6: PRP-Specific Validation
# Execute any custom validation commands from the PRP
```

### Validation Failure Recovery
```typescript
// Systematic approach to handling validation failures
interface ValidationFailure {
  gate: string;
  error_type: string;
  error_message: string;
  affected_files: string[];
  suggested_fix: string;
  retry_count: number;
}

async function handleValidationFailure(failure: ValidationFailure): Promise<boolean> {
  // Analyze the failure using PRP gotchas and patterns
  // Apply systematic fixes based on error type
  // Re-run validation to confirm fix
  // Track retry attempts to prevent infinite loops
  
  switch (failure.error_type) {
    case 'type_error':
      // Fix TypeScript issues using PRP patterns
      break;
    case 'test_failure':
      // Debug and fix test issues
      break;
    case 'lint_error':
      // Apply code style fixes
      break;
    case 'build_error':
      // Resolve build configuration issues
      break;
    default:
      // Handle unexpected errors using PRP guidance
  }
  
  return await retryValidation(failure.gate);
}
```

## Phase 5: Quality Assurance and Final Validation

### PRP Requirements Verification
Use `read_file` to re-read the PRP and verify all requirements are met:

```typescript
// Comprehensive verification against PRP requirements
interface RequirementVerification {
  requirement: string;
  implementation_status: 'complete' | 'partial' | 'missing';
  validation_evidence: string[];
  quality_notes: string;
}

function verifyAllRequirements(): RequirementVerification[] {
  // Re-read PRP to extract all requirements
  // Check each requirement against implementation
  // Gather evidence of completion
  // Note any deviations or improvements made
}
```

### Success Criteria Validation
```typescript
// Validate against PRP success criteria
interface SuccessCriteriaCheck {
  criterion: string;
  met: boolean;
  evidence: string;
  measurement: string;
}

function validateSuccessCriteria(): SuccessCriteriaCheck[] {
  // Check each success criterion from PRP
  // Gather measurable evidence
  // Document any criteria that exceed expectations
  // Note any criteria requiring additional work
}
```

### Final Quality Assessment
```bash
# Comprehensive final quality check
echo "🔍 Final Quality Assessment"

# Code Quality Metrics
echo "📊 Code Quality:"
npm run lint -- --format=compact | grep -c "error\|warning"
npm run type-check 2>&1 | grep -c "error"

# Test Coverage and Quality
echo "🧪 Test Quality:"
npm test -- --coverage --silent
npm run test:integration --silent

# Performance Metrics
echo "⚡ Performance:"
npm run build:analyze
npm run lighthouse-ci

# Security and Dependencies
echo "🔒 Security:"
npm audit --audit-level high
npm run test:security

# Accessibility Compliance
echo "♿ Accessibility:"
npm run test:a11y

# Bundle and Build Quality
echo "📦 Build Quality:"
npm run build
npm run bundle-analyzer
```

## Phase 6: Completion Reporting and Documentation

### Implementation Summary Report
Use `create_file` to generate comprehensive completion report:

```markdown
# Implementation Report: {feature_name}

## 📋 Implementation Summary
- **PRP File**: {prp_file}
- **Implementation Date**: {current_timestamp}
- **Implementation Mode**: {implementation_mode}
- **Validation Level**: {validation_level}

## ✅ Requirements Fulfillment
{comprehensive_list_of_met_requirements}

## 🎯 Success Criteria Achievement
{verification_of_all_success_criteria}

## 📊 Quality Metrics
- **Type Safety**: {typescript_errors_count} errors
- **Code Quality**: {lint_issues_count} issues
- **Test Coverage**: {coverage_percentage}%
- **Build Status**: {build_success_status}
- **Performance**: {performance_metrics}

## 🏗️ Implementation Details
{summary_of_implementation_approach_and_decisions}

## 🧪 Validation Results
{comprehensive_validation_results}

## 📝 Notes and Deviations
{any_deviations_from_prp_with_rationale}

## 🚀 Next Steps
{recommendations_for_deployment_or_further_development}
```

### Final PRP Re-Validation
```typescript
// Final check against original PRP
function finalPRPValidation(): boolean {
  // Re-read the PRP completely
  // Verify every section is addressed
  // Check all checklist items are complete
  // Validate all validation gates pass
  // Confirm confidence score expectations are met
  
  return allRequirementsMet && allValidationsPassed && qualityStandardsMet;
}
```

## Success Criteria

The PRP execution is complete when:
- [ ] PRP file is fully understood and analyzed
- [ ] Comprehensive implementation plan is created and executed
- [ ] All requirements from PRP are implemented
- [ ] All validation gates pass successfully
- [ ] Code quality meets or exceeds standards
- [ ] Test coverage is comprehensive and passing
- [ ] Build and deployment validation succeeds
- [ ] Success criteria from PRP are verified
- [ ] Implementation follows established patterns
- [ ] Error handling and edge cases are addressed
- [ ] Documentation is complete and accurate
- [ ] Final quality assessment passes all checks

## Integration Points

### Development Workflow Integration
- Follow established code review and quality processes
- Integrate with existing CI/CD pipelines
- Ensure compatibility with deployment processes
- Maintain consistency with team coding standards

### Quality Assurance Integration
- Execute comprehensive validation at each phase
- Integrate with automated testing frameworks
- Ensure compatibility with monitoring and observability
- Validate against accessibility and performance standards

### Documentation and Knowledge Sharing
- Generate implementation reports for team knowledge
- Document any pattern improvements or lessons learned
- Share validation approaches for future implementations
- Contribute to organizational development practices

Remember: The goal is systematic, high-quality implementation that fulfills all PRP requirements with comprehensive validation and documentation.
```

## Notes
- Always re-read the PRP throughout implementation to ensure requirements are met
- Use systematic validation at each phase to catch issues early
- Follow established patterns and conventions from the codebase
- Implement comprehensive error handling and edge case management
