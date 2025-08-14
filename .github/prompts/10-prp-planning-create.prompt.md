---
mode: agent
description: "Create comprehensive planning documents for Product Requirements Prompts with dependency mapping"
---

---
inputs:
  - name: project_scope
    description: Project scope for planning (module, feature, system)
    required: true
  - name: planning_horizon
    description: Planning horizon (sprint, release, quarter)
    required: false
    default: "release"
  - name: dependency_analysis
    description: Include dependency analysis (true, false)
    required: false
    default: "true"
---

# prp-planning-create.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Transform rough ideas into comprehensive Product Requirements Documents (PRDs) with rich visual documentation and detailed planning
- **Categories**: product-planning, requirements-analysis, documentation, visual-design
- **Complexity**: advanced
- **Dependencies**: market research, technical analysis, diagram creation tools

## Input
- **idea_description** (required): The core idea or concept to develop into a PRD
- **business_context** (optional): Business goals and constraints
- **target_users** (optional): Primary user personas and use cases

## Template

```
You are an expert product planning specialist focused on transforming rough ideas into comprehensive, actionable Product Requirements Documents (PRDs). Your task is to conduct thorough research, create detailed visual documentation, and develop strategic implementation plans that ensure successful product development.

## Input Parameters
- **Idea Description**: {idea_description}
- **Business Context**: {business_context}
- **Target Users**: {target_users}

## Task Overview
Transform the provided idea into a comprehensive PRD with detailed user research, market analysis, technical architecture, visual diagrams, and strategic implementation planning that enables confident development execution.

## Phase 1: Concept Expansion and Core Definition

### Idea Analysis and Decomposition
Use `semantic_search` to understand existing related features and patterns:

```typescript
// Break down the core idea into fundamental components
interface ConceptAnalysis {
  core_problem: string;
  proposed_solution: string;
  user_value_proposition: string;
  business_value_proposition: string;
  success_definition: string;
  scope_boundaries: {
    included: string[];
    excluded: string[];
    assumptions: string[];
  };
}

// Systematically analyze and expand the concept
function analyzeCoreConcept(idea: string): ConceptAnalysis {
  // Extract problem statement
  // Define solution approach
  // Identify value propositions
  // Set clear boundaries
  // Document key assumptions
}
```

### Business Alignment and Success Criteria
```typescript
// Define measurable success criteria and business alignment
interface BusinessAlignment {
  business_objectives: Array<{
    objective: string;
    alignment_level: 'high' | 'medium' | 'low';
    success_metric: string;
    measurement_approach: string;
  }>;
  
  success_criteria: Array<{
    criterion: string;
    metric: string;
    target_value: string;
    measurement_method: string;
    timeframe: string;
  }>;
  
  constraints: Array<{
    type: 'technical' | 'business' | 'regulatory' | 'resource';
    description: string;
    impact: 'high' | 'medium' | 'low';
    mitigation: string;
  }>;
}
```

## Phase 2: Comprehensive Market and Technical Research

### Market Analysis and Competitive Research
Use `vscode_websearchforcopilot_webSearch` for extensive market research:

```typescript
// Conduct comprehensive market analysis
interface MarketResearch {
  market_landscape: {
    market_size: string;
    growth_trends: string[];
    key_players: Array<{
      company: string;
      solution: string;
      market_position: string;
      differentiation: string;
    }>;
  };
  
  competitive_analysis: Array<{
    competitor: string;
    solution_approach: string;
    strengths: string[];
    weaknesses: string[];
    market_share: string;
    pricing_model: string;
    differentiation_opportunity: string;
  }>;
  
  user_feedback_insights: Array<{
    source: string;
    insight: string;
    frequency: 'common' | 'occasional' | 'rare';
    implications: string;
  }>;
}

// Research strategy for market understanding
const researchQueries = [
  'Market analysis for {idea_domain}',
  'Competitor solutions for {problem_space}',
  'User pain points in {target_domain}',
  'Best practices for {solution_type}',
  'Technical implementation approaches for {feature_type}',
  'Pricing models for {solution_category}',
  'User experience patterns for {interaction_type}'
];
```

### Technical Feasibility and Architecture Research
```typescript
// Analyze technical feasibility and architecture options
interface TechnicalResearch {
  feasibility_assessment: {
    complexity_level: 'low' | 'medium' | 'high';
    development_effort: string;
    technical_risks: Array<{
      risk: string;
      probability: 'low' | 'medium' | 'high';
      impact: 'low' | 'medium' | 'high';
      mitigation: string;
    }>;
  };
  
  architecture_options: Array<{
    approach: string;
    pros: string[];
    cons: string[];
    complexity: 'low' | 'medium' | 'high';
    scalability: 'limited' | 'moderate' | 'high';
    recommended: boolean;
    rationale: string;
  }>;
  
  technology_requirements: {
    frontend_technologies: string[];
    backend_technologies: string[];
    database_requirements: string[];
    third_party_integrations: string[];
    infrastructure_needs: string[];
  };
}
```

### Best Practices and Implementation Research
Use `fetch_webpage` to gather detailed implementation guidance:

```typescript
// Research implementation best practices and patterns
interface ImplementationResearch {
  design_patterns: Array<{
    pattern: string;
    use_case: string;
    benefits: string[];
    implementation_complexity: 'low' | 'medium' | 'high';
    examples: string[];
    documentation_url: string;
  }>;
  
  user_experience_patterns: Array<{
    pattern: string;
    context: string;
    benefits: string[];
    considerations: string[];
    examples: string[];
  }>;
  
  performance_considerations: Array<{
    aspect: string;
    optimization: string;
    impact: 'low' | 'medium' | 'high';
    implementation_effort: 'low' | 'medium' | 'high';
  }>;
}
```

## Phase 3: User Research and Requirements Gathering

### User Persona Development
```typescript
// Develop detailed user personas based on research
interface UserPersona {
  persona_name: string;
  demographics: {
    role: string;
    experience_level: string;
    context: string;
    goals: string[];
  };
  
  pain_points: Array<{
    pain_point: string;
    frequency: 'daily' | 'weekly' | 'monthly';
    severity: 'high' | 'medium' | 'low';
    current_workaround: string;
  }>;
  
  user_journey: Array<{
    stage: string;
    actions: string[];
    pain_points: string[];
    opportunities: string[];
  }>;
  
  success_metrics: Array<{
    metric: string;
    current_state: string;
    desired_state: string;
    measurement: string;
  }>;
}

// Generate comprehensive user personas
function developUserPersonas(): UserPersona[] {
  // Based on research and business context
  // Include multiple persona types if relevant
  // Document user journey stages
  // Identify key pain points and opportunities
}
```

### User Story Development with Comprehensive Context
```typescript
// Create detailed user stories with acceptance criteria
interface UserStory {
  epic: string;
  story_id: string;
  title: string;
  
  story_statement: {
    as_a: string;
    i_want: string;
    so_that: string;
  };
  
  acceptance_criteria: Array<{
    criterion: string;
    behavior_description: string;
    edge_cases: string[];
    error_handling: string[];
  }>;
  
  technical_requirements: {
    api_endpoints: string[];
    data_models: string[];
    ui_components: string[];
    business_logic: string[];
  };
  
  dependencies: string[];
  priority: 'must-have' | 'should-have' | 'could-have';
  effort_estimate: string;
}
```

## Phase 4: Visual Documentation and Diagram Creation

### Comprehensive Diagram Planning
```yaml
# Visual documentation strategy
diagrams_needed:
  user_flows:
    - primary_user_journey: "Happy path from start to completion"
    - error_recovery_flows: "How users recover from errors"
    - edge_case_scenarios: "Unusual but important use cases"
    - onboarding_flow: "First-time user experience"
  
  technical_architecture:
    - system_overview: "High-level system components"
    - data_flow: "How data moves through the system"
    - integration_points: "External system connections"
    - deployment_architecture: "Infrastructure and scaling"
  
  interaction_flows:
    - api_sequences: "API interaction patterns"
    - event_flows: "Event-driven interactions"
    - state_transitions: "State management flows"
    - error_handling: "Error propagation and handling"
  
  data_models:
    - entity_relationships: "Database schema design"
    - state_machines: "Business logic states"
    - data_transformation: "Data processing flows"
```

### Mermaid Diagram Implementation
Use `create_file` to create comprehensive visual documentation:

```mermaid
# User Journey Flow
graph TD
    A[User Starts] --> B{Authentication Status}
    B -->|Authenticated| C[Main Dashboard]
    B -->|Not Authenticated| D[Login Flow]
    D --> E{Login Success}
    E -->|Success| C
    E -->|Failure| F[Error Handling]
    F --> D
    C --> G[Feature Access]
    G --> H[Core Functionality]
    H --> I[Success State]
    H --> J[Error State]
    J --> K[Error Recovery]
    K --> H

# Technical Architecture
graph TB
    subgraph "Frontend Layer"
        UI[React Components]
        State[State Management]
        Services[API Services]
    end
    
    subgraph "Backend Layer"
        API[REST API]
        Business[Business Logic]
        Data[Data Access]
    end
    
    subgraph "Data Layer"
        DB[(Database)]
        Cache[(Cache)]
        Files[(File Storage)]
    end
    
    UI --> State
    State --> Services
    Services --> API
    API --> Business
    Business --> Data
    Data --> DB
    Data --> Cache
    Data --> Files

# API Sequence Diagram
sequenceDiagram
    participant User
    participant Frontend
    participant API
    participant Database
    
    User->>Frontend: Initiate Action
    Frontend->>API: Request with Validation
    API->>Database: Query/Update Data
    Database-->>API: Return Results
    API-->>Frontend: Formatted Response
    Frontend-->>User: Updated UI
    
    Note over API,Database: Error handling at each step
```

### Wireframe and UI Specification
```typescript
// UI/UX specifications and wireframe documentation
interface UISpecification {
  layout_structure: {
    header: string;
    navigation: string;
    main_content: string;
    sidebar: string;
    footer: string;
  };
  
  component_specifications: Array<{
    component: string;
    purpose: string;
    props: string[];
    states: string[];
    interactions: string[];
    accessibility_requirements: string[];
  }>;
  
  responsive_behavior: {
    mobile: string;
    tablet: string;
    desktop: string;
    breakpoints: string[];
  };
  
  design_system_integration: {
    colors: string[];
    typography: string[];
    spacing: string[];
    components: string[];
  };
}
```

## Phase 5: Technical Architecture and API Specification

### Detailed Technical Architecture
```typescript
// Comprehensive technical architecture specification
interface TechnicalArchitecture {
  system_overview: {
    architecture_pattern: string;
    scalability_approach: string;
    performance_targets: string[];
    security_requirements: string[];
  };
  
  component_architecture: Array<{
    component: string;
    responsibility: string;
    interfaces: string[];
    dependencies: string[];
    scaling_considerations: string;
  }>;
  
  data_architecture: {
    storage_strategy: string;
    data_models: string[];
    relationships: string[];
    performance_optimizations: string[];
  };
  
  integration_architecture: {
    external_apis: string[];
    event_systems: string[];
    authentication_flow: string;
    authorization_model: string;
  };
}
```

### Comprehensive API Specification
```typescript
// Detailed API specification with examples
interface APISpecification {
  endpoints: Array<{
    method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';
    path: string;
    purpose: string;
    
    request: {
      headers: Record<string, string>;
      parameters: Record<string, any>;
      body_schema: string;
      example: any;
    };
    
    response: {
      success_codes: string[];
      error_codes: string[];
      schema: string;
      examples: Record<string, any>;
    };
    
    authentication: boolean;
    authorization: string[];
    rate_limiting: string;
  }>;
  
  data_models: Array<{
    model: string;
    properties: Record<string, {
      type: string;
      required: boolean;
      validation: string;
      description: string;
    }>;
    relationships: string[];
    examples: any[];
  }>;
}
```

## Phase 6: Implementation Strategy and Risk Management

### Phased Implementation Plan
```typescript
// Strategic implementation phases with dependencies
interface ImplementationStrategy {
  phases: Array<{
    phase: number;
    name: string;
    duration_estimate: string;
    
    objectives: string[];
    deliverables: Array<{
      deliverable: string;
      acceptance_criteria: string[];
      dependencies: string[];
    }>;
    
    risks: Array<{
      risk: string;
      probability: 'low' | 'medium' | 'high';
      impact: 'low' | 'medium' | 'high';
      mitigation: string;
    }>;
    
    validation_criteria: string[];
    success_metrics: string[];
  }>;
  
  mvp_definition: {
    core_features: string[];
    success_criteria: string[];
    exclusions: string[];
    future_enhancements: string[];
  };
  
  dependency_management: Array<{
    dependency: string;
    type: 'technical' | 'business' | 'external';
    impact: 'blocking' | 'slowing' | 'enhancing';
    mitigation: string;
  }>;
}
```

### Risk Assessment and Mitigation
```typescript
// Comprehensive risk analysis and mitigation strategies
interface RiskManagement {
  technical_risks: Array<{
    risk: string;
    category: 'complexity' | 'performance' | 'integration' | 'scalability';
    probability: 'low' | 'medium' | 'high';
    impact: 'low' | 'medium' | 'high';
    indicators: string[];
    mitigation_strategies: string[];
    contingency_plans: string[];
  }>;
  
  business_risks: Array<{
    risk: string;
    category: 'market' | 'competitive' | 'resource' | 'timeline';
    probability: 'low' | 'medium' | 'high';
    impact: 'low' | 'medium' | 'high';
    mitigation: string;
  }>;
  
  risk_monitoring: Array<{
    risk_area: string;
    monitoring_approach: string;
    escalation_criteria: string[];
    review_frequency: string;
  }>;
}
```

## Phase 7: Comprehensive PRD Generation

### Complete PRD Document Creation
Use `create_file` to generate the comprehensive PRD:

```markdown
# Product Requirements Document: {feature_name}

## 📋 Executive Summary

### Problem Statement
{clear_articulation_of_problem_being_solved}

### Solution Overview
{high_level_solution_description_with_value_proposition}

### Business Impact
{expected_business_outcomes_and_success_metrics}

### Implementation Summary
{phases_timeline_and_resource_requirements}

## 🎯 Problem & Solution Analysis

### Market Context
{market_research_findings_and_opportunity_analysis}

### User Pain Points
{detailed_pain_point_analysis_with_supporting_research}

### Proposed Solution
{comprehensive_solution_description_with_rationale}

### Competitive Analysis
{competitor_analysis_and_differentiation_strategy}

## 👥 User Stories and Journey Mapping

### User Personas
{detailed_persona_descriptions_with_journey_maps}

### Epic Breakdown
{comprehensive_user_stories_with_acceptance_criteria}

### User Flow Diagrams
```mermaid
{user_flow_diagrams_showing_all_scenarios}
```

## 🏗️ Technical Architecture

### System Architecture
```mermaid
{comprehensive_architecture_diagrams}
```

### Component Specifications
{detailed_component_architecture_and_responsibilities}

### Data Models
```mermaid
{entity_relationship_diagrams_and_data_models}
```

### Integration Points
{external_system_integrations_and_api_specifications}

## 🔌 API Specifications

### Endpoint Documentation
{comprehensive_api_documentation_with_examples}

### Authentication & Authorization
{security_model_and_access_control_specifications}

### Error Handling
{error_response_patterns_and_recovery_strategies}

## 📊 Data Models and Storage

### Database Schema
{detailed_database_design_and_relationships}

### Performance Considerations
{indexing_caching_and_optimization_strategies}

### Data Migration Strategy
{if_applicable_migration_plans_and_rollback_procedures}

## 🚀 Implementation Phases

### Phase Breakdown
{detailed_phase_descriptions_with_deliverables_and_timelines}

### MVP Definition
{core_features_and_success_criteria_for_minimum_viable_product}

### Future Enhancements
{planned_features_for_subsequent_releases}

## ⚠️ Risks & Mitigations

### Technical Risks
{comprehensive_technical_risk_analysis_with_mitigation_strategies}

### Business Risks
{business_and_market_risk_assessment_with_contingencies}

### Dependency Management
{external_dependencies_and_risk_mitigation_approaches}

## 📈 Success Metrics & KPIs

### Primary Success Metrics
{key_performance_indicators_with_targets_and_measurement_methods}

### User Experience Metrics
{user_satisfaction_and_engagement_measurement_approaches}

### Technical Performance Metrics
{system_performance_and_reliability_measurement_criteria}

## 📚 Appendices

### Research References
{links_to_market_research_competitive_analysis_and_technical_documentation}

### Detailed Wireframes
{comprehensive_ui_wireframes_and_mockups}

### Technical Deep Dives
{detailed_technical_specifications_and_implementation_notes}

---
*This PRD was generated through comprehensive research and analysis to ensure successful product development.*
```

### Quality Validation Checklist
```typescript
// Comprehensive PRD quality validation
interface PRDQualityCheck {
  completeness: {
    problem_clearly_articulated: boolean;
    solution_addresses_problem: boolean;
    user_flows_documented: boolean;
    architecture_visualized: boolean;
    apis_fully_specified: boolean;
    data_models_included: boolean;
    risks_identified_and_mitigated: boolean;
    success_metrics_measurable: boolean;
  };
  
  clarity: {
    technical_specifications_clear: boolean;
    business_requirements_unambiguous: boolean;
    implementation_path_logical: boolean;
    dependencies_clearly_identified: boolean;
  };
  
  actionability: {
    ready_for_implementation_prp: boolean;
    validation_criteria_testable: boolean;
    success_metrics_achievable: boolean;
    timeline_realistic: boolean;
  };
}
```

## Success Criteria

The PRD creation is complete when:
- [ ] Problem is clearly articulated with supporting research
- [ ] Solution comprehensively addresses identified problems
- [ ] All user flows are documented with visual diagrams
- [ ] Technical architecture is fully visualized and specified
- [ ] APIs are completely specified with examples
- [ ] Data models are detailed with relationships
- [ ] Implementation phases are logical with clear dependencies
- [ ] Risks are identified with mitigation strategies
- [ ] Success metrics are specific and measurable
- [ ] All diagrams are created using Mermaid syntax
- [ ] PRD is ready for conversion to implementation PRP
- [ ] Quality validation checklist is satisfied

## Integration Points

### Product Development Workflow
- Connect PRD to implementation PRP creation process
- Integrate with development team planning and estimation
- Align with business stakeholder review and approval processes
- Enable smooth transition from planning to development

### Research and Analysis Integration
- Incorporate market research into ongoing competitive monitoring
- Use technical feasibility analysis for technology roadmap planning
- Apply user research insights to broader product strategy
- Document lessons learned for future product planning

### Quality Assurance Integration
- Establish validation criteria that enable effective testing
- Create success metrics that support ongoing product measurement
- Design implementation phases that allow for iterative validation
- Ensure all requirements are testable and measurable

Remember: Great PRDs prevent implementation confusion and enable confident development execution through comprehensive planning and clear specifications.
```

## Notes
- Focus on visual documentation and comprehensive diagrams using Mermaid
- Conduct thorough market and technical research before planning
- Create detailed user stories with complete acceptance criteria
- Ensure all requirements are actionable and measurable
