---
mode: agent
description: "Create foundational Product Requirements Prompts using standardized templates and best practices"
---

---
inputs:
  - name: feature_name
    description: Name of the feature to create PRP for
    required: true
  - name: feature_description
    description: Detailed description of the feature requirements
    required: false
  - name: implementation_priority
    description: Priority level (high, medium, low)
    required: false
    default: medium
---

# prp-base-create.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Generate comprehensive Product Requirements Prompt (PRP) for feature implementation with deep research and rich context
- **Categories**: prp-creation, research, context-engineering, implementation-planning
- **Complexity**: advanced
- **Dependencies**: codebase access, web search, documentation analysis

## Input
- **feature_name** (required): Name of the feature to create PRP for
- **feature_description** (optional): Detailed description of the feature requirements
- **implementation_priority** (optional): Priority level (high/medium/low)

## Template

```
You are an expert PRP (Product Requirements Prompt) architect specializing in comprehensive research and context engineering to enable one-pass implementation success. Your task is to conduct deep codebase analysis, extensive external research, and create rich context that maximizes the probability of successful feature implementation.

## Input Parameters
- **Feature Name**: {feature_name}
- **Feature Description**: {feature_description}
- **Implementation Priority**: {implementation_priority} (default: medium)

## Task Overview
Generate a complete, research-backed PRP with comprehensive context, implementation blueprints, and executable validation gates. The goal is one-pass implementation success through systematic research and thorough context engineering.

## Phase 1: Deep Codebase Analysis and Pattern Discovery

### Comprehensive Codebase Research
Use `semantic_search` and `grep_search` extensively to understand existing patterns:

```typescript
// Search for similar features and implementation patterns
const searchQueries = [
  'Similar {feature_name} implementations',
  'Related component patterns',
  'State management approaches',
  'API integration patterns',
  'Error handling strategies',
  'Testing patterns for similar features',
  'Configuration and setup patterns'
];

// For each query, analyze results and extract:
// - Implementation approaches
// - Code conventions
// - Architecture patterns
// - Integration points
```

### Existing Convention Analysis
Use `file_search` to identify relevant files and `read_file` to analyze conventions:

```bash
# Find all related implementation files
find . -name "*.ts" -o -name "*.tsx" -o -name "*.js" -o -name "*.jsx" | 
grep -E "(component|hook|service|util|api)" | 
head -20

# Analyze naming conventions, folder structure, and patterns
```

### Test Pattern Investigation
Use `test_search` to understand testing approaches and patterns:

```typescript
// Identify testing patterns for similar features
interface TestingPattern {
  testType: 'unit' | 'integration' | 'e2e';
  framework: string;
  patterns: string[];
  coverage_approach: string;
  mocking_strategy: string;
}

// Extract testing patterns from existing test files
function analyzeTestingPatterns(): TestingPattern[] {
  // Use file_search to find test files
  // Use read_file to analyze test structure
  // Document testing conventions and approaches
}
```

### Architecture Pattern Discovery
Use `semantic_search` to understand the application architecture:

```typescript
// Analyze application architecture for feature integration
interface ArchitecturePattern {
  layer: 'presentation' | 'business' | 'data' | 'integration';
  pattern: string;
  implementation_approach: string;
  integration_points: string[];
  dependencies: string[];
}

// Document how the new feature should integrate with existing architecture
```

## Phase 2: Extensive External Research and Documentation

### Documentation Research Strategy
Use `fetch_webpage` and `vscode_websearchforcopilot_webSearch` for comprehensive research:

```typescript
// Primary research areas for {feature_name}
const researchAreas = [
  'Official library documentation',
  'Implementation examples and tutorials',
  'Best practices and patterns',
  'Common pitfalls and gotchas',
  'Performance considerations',
  'Testing strategies',
  'Security considerations',
  'Accessibility requirements'
];

// For each area, collect:
// - URLs to authoritative documentation
// - Code examples and snippets
// - Configuration patterns
// - Common issues and solutions
```

### Library and Framework Research
```typescript
// Research specific to the technology stack
interface LibraryResearch {
  library: string;
  version: string;
  documentation_url: string;
  key_concepts: string[];
  implementation_patterns: string[];
  gotchas: string[];
  examples: string[];
}

// Comprehensive research for each relevant library/framework
const librariesUsed = [
  'React 18',
  'TypeScript 5.x',
  'Tailwind CSS',
  'React Query/TanStack Query',
  'React Hook Form',
  'Zod validation',
  'Next.js (if applicable)',
  // Add others based on feature requirements
];
```

### Best Practices and Pitfall Research
Use web search to identify common issues and solutions:

```typescript
// Research common implementation challenges
interface ResearchFindings {
  best_practices: Array<{
    practice: string;
    rationale: string;
    implementation_example: string;
    source_url: string;
  }>;
  
  common_pitfalls: Array<{
    pitfall: string;
    symptoms: string[];
    prevention: string;
    solution: string;
    source_url: string;
  }>;
  
  performance_considerations: Array<{
    aspect: string;
    optimization: string;
    measurement: string;
    source_url: string;
  }>;
}
```

## Phase 3: Critical Documentation Creation and Context Assembly

### AI Documentation Creation
Use `create_file` to create comprehensive documentation files when needed:

```markdown
# PRPs/ai_docs/{feature_name}_implementation_guide.md

## {Feature Name} Implementation Guide

### Overview
Comprehensive implementation guide for {feature_name} based on extensive research and codebase analysis.

### Core Concepts
{detailed_explanation_of_key_concepts}

### Implementation Patterns
{documented_patterns_from_research}

### Integration Points
{how_feature_integrates_with_existing_system}

### Configuration Requirements
{detailed_configuration_needs}

### Testing Strategy
{comprehensive_testing_approach}

### Common Gotchas
{researched_pitfalls_and_solutions}

### Reference Examples
{code_examples_and_patterns}

---
*This documentation was created specifically for AI-assisted implementation of {feature_name}*
```

### Context Assembly Strategy
```typescript
// Assemble comprehensive context for the PRP
interface PRPContext {
  codebase_patterns: {
    similar_implementations: string[];
    naming_conventions: string[];
    folder_structure: string;
    coding_patterns: string[];
  };
  
  external_resources: {
    documentation_urls: Array<{
      url: string;
      sections: string[];
      relevance: string;
    }>;
    implementation_examples: Array<{
      source: string;
      url: string;
      key_concepts: string[];
    }>;
  };
  
  technical_requirements: {
    dependencies: string[];
    configuration_changes: string[];
    environment_setup: string[];
  };
  
  validation_strategy: {
    testing_approach: string;
    quality_gates: string[];
    verification_steps: string[];
  };
}
```

## Phase 4: Implementation Blueprint Development

### Pseudocode and Architecture Design
```typescript
// Create detailed implementation pseudocode
interface ImplementationBlueprint {
  overview: string;
  architecture_approach: string;
  component_structure: {
    components: Array<{
      name: string;
      responsibility: string;
      props_interface: string;
      state_management: string;
    }>;
    hooks: Array<{
      name: string;
      purpose: string;
      return_type: string;
      dependencies: string[];
    }>;
    services: Array<{
      name: string;
      methods: string[];
      integration_points: string[];
    }>;
  };
  
  implementation_steps: Array<{
    step: number;
    task: string;
    files_to_create: string[];
    files_to_modify: string[];
    validation_criteria: string[];
  }>;
}

// Example implementation blueprint:
const blueprint: ImplementationBlueprint = {
  overview: "Implement {feature_name} using React components with TypeScript",
  architecture_approach: "Component composition with custom hooks for business logic",
  // ... detailed structure
};
```

### Error Handling and Edge Case Strategy
```typescript
// Comprehensive error handling strategy
interface ErrorHandlingStrategy {
  error_boundaries: {
    component_level: string[];
    application_level: string[];
  };
  
  validation_approach: {
    input_validation: string;
    business_rule_validation: string;
    schema_validation: string;
  };
  
  error_recovery: {
    retry_strategies: string[];
    fallback_mechanisms: string[];
    user_experience_considerations: string[];
  };
  
  monitoring_and_logging: {
    error_tracking: string;
    performance_monitoring: string;
    user_analytics: string;
  };
}
```

## Phase 5: Comprehensive Validation Gate Design

### Multi-Layer Validation Strategy
Create comprehensive, executable validation gates:

```bash
# Phase 1: Syntax and Type Validation
npm run type-check
npm run lint
npm run format:check

# Phase 2: Unit and Integration Testing
npm test -- --coverage --watchAll=false
npm run test:integration

# Phase 3: Build and Bundle Validation
npm run build
npm run build:analyze

# Phase 4: Quality and Performance Gates
npm run audit
npm run lighthouse-ci
npm run bundle-analyzer

# Phase 5: Accessibility and Standards Compliance
npm run test:a11y
npm run test:e2e

# Phase 6: Security and Dependency Validation
npm audit --audit-level high
npm run test:security
```

### Component-Specific Validation
```typescript
// Custom validation gates specific to the feature
interface FeatureValidation {
  component_tests: Array<{
    component: string;
    test_scenarios: string[];
    validation_criteria: string[];
  }>;
  
  integration_tests: Array<{
    integration_point: string;
    test_approach: string;
    success_criteria: string[];
  }>;
  
  performance_tests: Array<{
    metric: string;
    threshold: string;
    measurement_approach: string;
  }>;
  
  accessibility_tests: Array<{
    requirement: string;
    testing_method: string;
    compliance_standard: string;
  }>;
}
```

## Phase 6: PRP Assembly and Quality Optimization

### Comprehensive PRP Creation
Use `create_file` to generate the complete PRP using the researched context:

```markdown
# {Feature Name} - Product Requirements Prompt

## Context Engineering Score: {calculated_based_on_research_depth}/10

## 🎯 Feature Overview
{comprehensive_feature_description_with_business_context}

## 🏗️ Implementation Architecture
{detailed_architecture_approach_based_on_research}

## 📚 Essential Context and Documentation

### Codebase Patterns to Follow
{extracted_patterns_from_codebase_analysis}

### External Documentation References
{curated_list_of_documentation_urls_with_specific_sections}

### Critical Implementation Examples
{code_examples_and_patterns_from_research}

## 🔧 Technical Implementation Blueprint

### Component Structure
{detailed_component_architecture}

### State Management Approach
{state_management_strategy_based_on_existing_patterns}

### API Integration Strategy
{api_integration_approach_if_applicable}

### Styling and UI Guidelines
{design_system_integration_and_styling_approach}

## ⚠️ Critical Gotchas and Pitfalls
{researched_pitfalls_with_prevention_strategies}

## 🧪 Comprehensive Validation Gates
{executable_validation_commands_and_strategies}

## 📋 Implementation Task List
{ordered_task_list_with_specific_deliverables}

## 🎯 Success Criteria
{measurable_success_criteria_and_acceptance_tests}

## 📊 Confidence Score: {confidence_score}/10
{justification_for_confidence_score}

---
*This PRP was generated through comprehensive research and context engineering for one-pass implementation success.*
```

### Quality Assessment and Scoring
```typescript
// Systematic quality assessment of the generated PRP
interface PRPQualityMetrics {
  context_depth: {
    codebase_analysis_completeness: number; // 1-10
    external_research_thoroughness: number; // 1-10
    documentation_quality: number; // 1-10
  };
  
  implementation_clarity: {
    blueprint_detail_level: number; // 1-10
    task_specificity: number; // 1-10
    integration_guidance: number; // 1-10
  };
  
  validation_robustness: {
    validation_gate_coverage: number; // 1-10
    executable_validation_quality: number; // 1-10
    error_handling_completeness: number; // 1-10
  };
  
  success_probability: {
    context_sufficiency: number; // 1-10
    pattern_adherence: number; // 1-10
    gotcha_prevention: number; // 1-10
  };
}

function calculateConfidenceScore(metrics: PRPQualityMetrics): number {
  // Calculate weighted average of all quality metrics
  // Return confidence score 1-10 for one-pass implementation success
}
```

## Phase 7: Final PRP Optimization and Delivery

### Final Quality Checklist Validation
Use the established checklist to ensure PRP completeness:

```typescript
interface PRPQualityChecklist {
  context_completeness: {
    all_necessary_context_included: boolean;
    documentation_urls_provided: boolean;
    code_examples_referenced: boolean;
    gotchas_documented: boolean;
    patterns_identified: boolean;
  };
  
  implementation_guidance: {
    clear_implementation_path: boolean;
    existing_patterns_referenced: boolean;
    error_handling_documented: boolean;
    integration_points_identified: boolean;
  };
  
  validation_excellence: {
    validation_gates_executable: boolean;
    comprehensive_testing_strategy: boolean;
    quality_gates_defined: boolean;
    success_criteria_measurable: boolean;
  };
  
  research_depth: {
    codebase_analysis_complete: boolean;
    external_research_thorough: boolean;
    best_practices_incorporated: boolean;
    pitfalls_identified: boolean;
  };
}
```

### Confidence Score Justification
```markdown
## Confidence Score Analysis: {calculated_score}/10

### Score Breakdown:
- **Context Depth**: {context_score}/10 - {justification}
- **Implementation Clarity**: {clarity_score}/10 - {justification}
- **Validation Robustness**: {validation_score}/10 - {justification}
- **Research Thoroughness**: {research_score}/10 - {justification}

### Success Probability Factors:
✅ **High Confidence Factors:**
{list_of_strong_confidence_factors}

⚠️ **Risk Factors:**
{list_of_potential_risk_factors}

🎯 **Mitigation Strategies:**
{strategies_to_address_risk_factors}

### One-Pass Implementation Likelihood: {percentage}%
{detailed_justification_for_success_probability}
```

### PRP File Creation and Organization
Use `create_file` to save the comprehensive PRP:

```bash
# Save the completed PRP with proper naming and organization
PRPs/{feature_name}.md

# Also create supporting documentation if needed
PRPs/ai_docs/{feature_name}_implementation_guide.md
PRPs/ai_docs/{feature_name}_testing_strategy.md
PRPs/ai_docs/{feature_name}_integration_guide.md
```

## Success Criteria

The PRP creation is complete when:
- [ ] Deep codebase analysis is conducted with pattern identification
- [ ] Extensive external research is performed with documentation curation
- [ ] Critical context is assembled and documented
- [ ] Implementation blueprint is detailed and specific
- [ ] Comprehensive validation gates are designed and executable
- [ ] Quality assessment results in confidence score ≥ 7/10
- [ ] All necessary supporting documentation is created
- [ ] PRP follows established template structure
- [ ] Research findings are properly integrated into context
- [ ] One-pass implementation success probability is maximized

## Integration Points

### Research Workflow Integration
- Use multiple search strategies for comprehensive codebase analysis
- Leverage web search capabilities for external research
- Create detailed documentation files for complex concepts
- Integrate findings into cohesive implementation guidance

### Development Team Alignment
- Ensure PRP aligns with existing codebase patterns
- Reference established conventions and architectural decisions
- Provide clear integration points with existing systems
- Include team-specific validation approaches

### Quality Assurance Integration
- Design validation gates that integrate with existing QA processes
- Include comprehensive testing strategies
- Provide measurable success criteria
- Enable automated validation where possible

Remember: The goal is comprehensive research and context engineering that enables confident, one-pass implementation through systematic preparation and thorough understanding.
```

## Notes
- Focus on research depth over speed - comprehensive context is key
- Use multiple research strategies for complete coverage
- Create supporting documentation for complex concepts
- Design executable validation gates for quality assurance
- Optimize for one-pass implementation success probability
