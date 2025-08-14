---
mode: agent
description: "Initialize comprehensive task management system with dependency tracking and progress monitoring"
---

---
inputs:
  - name: project_context
    description: Project context for task initialization
    required: true
  - name: task_granularity
    description: Task granularity (high-level, detailed, granular)
    required: false
    default: "detailed"
---

# task-list-init.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Create comprehensive task lists for building projects with information-dense keywords and systematic implementation approach
- **Categories**: task-planning, project-initialization, systematic-implementation, ultra-thinking
- **Complexity**: advanced
- **Dependencies**: project analysis tools, codebase understanding, task planning frameworks

## Input
- **project_requirements** (required): Description of the project or feature requirements to build
- **output_file** (optional): Target file for task list (default: PRPs/checklist.md)
- **task_granularity** (optional): Level of task detail (high/medium/low)

## Template

```
You are an expert project planning specialist focused on creating comprehensive, systematic task lists for complex development projects using information-dense keywords and ULTRATHINK methodology. Your goal is to analyze project requirements deeply, understand existing codebase patterns, and create detailed implementation plans with clear, actionable tasks.

## Input Parameters
- **Project Requirements**: {project_requirements}
- **Output File**: {output_file} (default: PRPs/checklist.md)
- **Task Granularity**: {task_granularity} (default: high)

## Task Overview
Create a comprehensive task list for the specified project requirements by deeply analyzing the existing codebase, understanding patterns and architecture, applying ULTRATHINK methodology for comprehensive planning, and generating detailed tasks with information-dense keywords and systematic implementation approach.

## Phase 1: Deep Codebase Analysis and Pattern Discovery

### Comprehensive Codebase Understanding
Use `semantic_search` and `file_search` to deeply understand the existing project structure:

```typescript
// Comprehensive codebase analysis for task planning
interface CodebaseAnalysis {
  project_structure: {
    architectural_patterns: Array<{
      pattern_name: string;
      pattern_location: string[];
      usage_frequency: number;
      implementation_approach: string;
      key_characteristics: string[];
    }>;
    
    module_organization: Array<{
      module_path: string;
      module_purpose: string;
      dependencies: string[];
      export_patterns: string[];
      integration_points: string[];
    }>;
    
    coding_conventions: Array<{
      convention_type: string;
      examples: string[];
      consistency_level: 'high' | 'medium' | 'low';
      deviation_notes: string[];
    }>;
    
    technology_stack: {
      frontend_technologies: string[];
      backend_technologies: string[];
      database_technologies: string[];
      testing_frameworks: string[];
      build_tools: string[];
    };
  };
  
  existing_implementations: Array<{
    feature_name: string;
    implementation_files: string[];
    implementation_patterns: string[];
    reusable_components: string[];
    integration_approach: string[];
  }>;
  
  development_workflows: {
    testing_patterns: string[];
    deployment_procedures: string[];
    code_quality_practices: string[];
    documentation_standards: string[];
  };
  
  technical_debt_areas: Array<{
    area: string;
    impact: 'high' | 'medium' | 'low';
    improvement_opportunities: string[];
    refactoring_considerations: string[];
  }>;
}

// Analyze codebase comprehensively for task planning
async function analyzeCodebaseForTaskPlanning(): Promise<CodebaseAnalysis> {
  // Use semantic_search to understand architectural patterns
  // Use file_search to identify module structures
  // Use grep_search to find coding conventions
  // Use list_code_usages to understand integration patterns
}
```

### Pattern Mining and Reusability Analysis
```bash
# Deep pattern mining for reusable implementations
mine_implementation_patterns() {
    echo "🔍 Mining Implementation Patterns for Reusability"
    
    # Create pattern analysis directory
    mkdir -p ".task-planning-analysis/$(date +%Y%m%d-%H%M%S)"
    local analysis_dir=".task-planning-analysis/$(date +%Y%m%d-%H%M%S)"
    
    # Analyze component patterns
    echo "🧩 Component Pattern Analysis..."
    find . -name "*.tsx" -o -name "*.ts" -o -name "*.jsx" -o -name "*.js" | \
        head -50 | \
        xargs grep -l "export.*Component\|export.*function.*Props" > "$analysis_dir/component_patterns.txt"
    
    # Analyze service patterns
    echo "🔧 Service Pattern Analysis..."
    find . -name "*Service.ts" -o -name "*service.ts" -o -name "*Service.js" | \
        head -30 | \
        xargs grep -l "class.*Service\|export.*service" > "$analysis_dir/service_patterns.txt"
    
    # Analyze repository patterns
    echo "📊 Repository Pattern Analysis..."
    find . -name "*Repository.ts" -o -name "*repository.ts" -o -name "*Repository.cs" | \
        head -20 | \
        xargs grep -l "class.*Repository\|interface.*Repository" > "$analysis_dir/repository_patterns.txt"
    
    # Analyze test patterns
    echo "🧪 Test Pattern Analysis..."
    find . -name "*.test.ts" -o -name "*.test.tsx" -o -name "*.spec.ts" | \
        head -50 | \
        xargs grep -l "describe\|it\|test" > "$analysis_dir/test_patterns.txt"
    
    # Analyze configuration patterns
    echo "⚙️ Configuration Pattern Analysis..."
    find . -name "*.config.ts" -o -name "*.config.js" -o -name "appsettings*.json" | \
        head -20 > "$analysis_dir/config_patterns.txt"
    
    echo "✅ Pattern mining completed in: $analysis_dir"
}

# Analyze existing feature implementations for pattern reuse
analyze_existing_features() {
    echo "🏗️ Analyzing Existing Feature Implementations"
    
    # Identify feature modules
    local feature_modules=$(find . -type d -name "*module*" -o -name "*feature*" -o -name "*domain*" | head -20)
    
    for module in $feature_modules; do
        if [[ -d "$module" ]]; then
            echo "📋 Analyzing feature module: $module"
            
            # Count different file types
            local components=$(find "$module" -name "*.tsx" -o -name "*.jsx" | wc -l)
            local services=$(find "$module" -name "*service*" -o -name "*Service*" | wc -l)
            local tests=$(find "$module" -name "*.test.*" -o -name "*.spec.*" | wc -l)
            local configs=$(find "$module" -name "*.config.*" | wc -l)
            
            echo "  Components: $components, Services: $services, Tests: $tests, Configs: $configs"
        fi
    done
}
```

### Requirements Analysis and Decomposition
```typescript
// Decompose project requirements into actionable components
interface RequirementDecomposition {
  high_level_objectives: Array<{
    objective_id: string;
    description: string;
    business_value: string;
    success_criteria: string[];
    dependencies: string[];
  }>;
  
  functional_requirements: Array<{
    requirement_id: string;
    feature_area: string;
    user_story: string;
    acceptance_criteria: string[];
    technical_considerations: string[];
  }>;
  
  technical_requirements: Array<{
    requirement_id: string;
    category: 'performance' | 'security' | 'scalability' | 'maintainability' | 'integration';
    specification: string;
    validation_approach: string[];
    implementation_impact: string[];
  }>;
  
  implementation_components: Array<{
    component_id: string;
    component_type: 'frontend' | 'backend' | 'database' | 'infrastructure' | 'testing';
    component_description: string;
    reusable_patterns: string[];
    new_implementation_needed: string[];
  }>;
  
  integration_requirements: Array<{
    integration_point: string;
    integration_type: 'api' | 'database' | 'service' | 'ui' | 'workflow';
    existing_integrations: string[];
    new_integrations_needed: string[];
  }>;
}

// Decompose requirements using ULTRATHINK methodology
function decomposeRequirements(requirements: string): RequirementDecomposition {
  // Apply systematic requirement analysis
  // Identify reusable patterns and components
  // Map technical and functional requirements
  // Plan integration and implementation approach
}
```

## Phase 2: ULTRATHINK Strategic Planning

### Comprehensive Strategic Analysis
```typescript
// Apply ULTRATHINK methodology for comprehensive planning
interface UltraThinkAnalysis {
  strategic_context: {
    project_scope: {
      in_scope: string[];
      out_of_scope: string[];
      scope_boundaries: string[];
      assumptions: string[];
    };
    
    success_definition: {
      primary_outcomes: string[];
      secondary_outcomes: string[];
      success_metrics: string[];
      acceptance_criteria: string[];
    };
    
    constraint_analysis: {
      technical_constraints: string[];
      business_constraints: string[];
      resource_constraints: string[];
      timeline_constraints: string[];
    };
  };
  
  implementation_strategy: {
    development_approach: 'incremental' | 'parallel' | 'sequential' | 'hybrid';
    risk_mitigation: Array<{
      risk: string;
      mitigation_strategy: string;
      contingency_plan: string;
    }>;
    
    quality_assurance: {
      testing_strategy: string[];
      validation_approach: string[];
      quality_gates: string[];
    };
    
    integration_strategy: {
      integration_points: string[];
      integration_sequence: string[];
      validation_checkpoints: string[];
    };
  };
  
  task_breakdown_strategy: {
    granularity_level: 'atomic' | 'component' | 'feature' | 'system';
    parallelization_opportunities: string[];
    dependency_management: string[];
    validation_frequency: string;
  };
}

// Apply ULTRATHINK for comprehensive strategic planning
function ultraThinkAnalysis(
  requirements: RequirementDecomposition,
  codebase: CodebaseAnalysis
): UltraThinkAnalysis {
  // Analyze strategic context and constraints
  // Design implementation strategy
  // Plan task breakdown approach
  // Identify optimization opportunities
}
```

### Pattern-Based Implementation Planning
```typescript
// Plan implementation using identified patterns and best practices
interface ImplementationPlan {
  implementation_phases: Array<{
    phase_number: number;
    phase_name: string;
    phase_objectives: string[];
    deliverables: string[];
    
    pattern_applications: Array<{
      pattern_name: string;
      source_implementation: string;
      target_implementation: string;
      adaptation_required: string[];
      reuse_percentage: number;
    }>;
    
    new_implementations: Array<{
      component_name: string;
      implementation_approach: string;
      design_considerations: string[];
      testing_requirements: string[];
    }>;
    
    integration_points: Array<{
      integration_target: string;
      integration_method: string;
      validation_approach: string[];
    }>;
  }>;
  
  cross_cutting_concerns: {
    error_handling: string[];
    logging_strategy: string[];
    performance_considerations: string[];
    security_requirements: string[];
    accessibility_requirements: string[];
  };
  
  validation_strategy: {
    unit_testing: string[];
    integration_testing: string[];
    end_to_end_testing: string[];
    performance_testing: string[];
    security_testing: string[];
  };
}
```

## Phase 3: Information-Dense Task Creation

### Task Generation with Information-Dense Keywords
```typescript
// Generate detailed tasks using information-dense keywords
interface TaskDefinition {
  task_id: string;
  status: 'DONE' | 'IN_PROGRESS' | 'PENDING';
  
  primary_action: {
    action_keyword: 'ADD' | 'CREATE' | 'MODIFY' | 'MIRROR' | 'FIND' | 'EXECUTE' | 'KEEP' | 'PRESERVE' | 'INJECT' | 'REPLACE' | 'MOVE' | 'DELETE' | 'CONFIGURE' | 'VALIDATE' | 'TEST' | 'DEPLOY';
    target: string;
    operation_details: string[];
  };
  
  sub_actions: Array<{
    action_keyword: string;
    operation: string;
    context: string;
    validation: string;
  }>;
  
  pattern_references: Array<{
    pattern_type: 'MIRROR' | 'COPY' | 'ADAPT' | 'EXTEND';
    source_pattern: string;
    target_application: string;
    modification_notes: string[];
  }>;
  
  validation_requirements: {
    unit_tests: string[];
    integration_tests: string[];
    manual_validation: string[];
    performance_checks: string[];
  };
  
  dependencies: string[];
  estimated_effort: string;
  complexity_level: 'low' | 'medium' | 'high';
}

// Generate comprehensive task definitions
function generateInformationDenseTasks(
  implementationPlan: ImplementationPlan,
  patterns: CodebaseAnalysis
): TaskDefinition[] {
  // Create tasks with information-dense keywords
  // Apply pattern-based implementation approach
  // Include comprehensive validation requirements
  // Ensure systematic progression and dependencies
}
```

### Systematic Task Sequencing and Dependencies
```bash
# Create systematic task sequencing with clear dependencies
create_task_sequence() {
    echo "📋 Creating Systematic Task Sequence"
    
    # Task categories for systematic organization
    local setup_tasks=()
    local infrastructure_tasks=()
    local backend_tasks=()
    local frontend_tasks=()
    local integration_tasks=()
    local testing_tasks=()
    local deployment_tasks=()
    
    # Generate task sequence based on dependencies
    generate_task_sequence() {
        local task_category=$1
        local task_prefix=$2
        
        echo "Creating $task_category tasks with prefix $task_prefix"
        
        # Example task generation pattern
        case $task_category in
            "setup")
                generate_setup_tasks "$task_prefix"
                ;;
            "infrastructure")
                generate_infrastructure_tasks "$task_prefix"
                ;;
            "backend")
                generate_backend_tasks "$task_prefix"
                ;;
            "frontend")
                generate_frontend_tasks "$task_prefix"
                ;;
            "integration")
                generate_integration_tasks "$task_prefix"
                ;;
            "testing")
                generate_testing_tasks "$task_prefix"
                ;;
            "deployment")
                generate_deployment_tasks "$task_prefix"
                ;;
        esac
    }
}

# Generate backend tasks with information-dense keywords
generate_backend_tasks() {
    local prefix=$1
    
    cat << EOF
### Backend Implementation Tasks

${prefix}-BE-001:
STATUS [ ]
CREATE src/services/UserService.ts:
  - MIRROR pattern from: src/services/ExistingService.ts
  - MODIFY class name to UserService
  - KEEP error handling pattern identical
  - ADD business logic methods: createUser, updateUser, deleteUser
  - PRESERVE async/await patterns
  - INJECT dependency injection configuration

${prefix}-BE-002:
STATUS [ ]
CREATE src/repositories/UserRepository.ts:
  - MIRROR pattern from: src/repositories/BaseRepository.ts
  - EXTEND BaseRepository with User-specific methods
  - ADD database queries: findByEmail, findByRole, updateProfile
  - KEEP transaction handling patterns
  - PRESERVE connection management approach

${prefix}-BE-003:
STATUS [ ]
MODIFY src/controllers/UserController.ts:
  - FIND pattern: "class BaseController"
  - INJECT after line containing "constructor"
  - ADD route handlers: GET /users, POST /users, PUT /users/:id
  - PRESERVE error response format
  - KEEP authentication middleware pattern
EOF
}

# Generate frontend tasks with information-dense keywords  
generate_frontend_tasks() {
    local prefix=$1
    
    cat << EOF
### Frontend Implementation Tasks

${prefix}-FE-001:
STATUS [ ]
CREATE src/components/UserCard.tsx:
  - MIRROR pattern from: src/components/ExistingCard.tsx
  - MODIFY component name and props interface
  - KEEP styling approach with Tailwind CSS
  - ADD user-specific display logic
  - PRESERVE accessibility attributes
  - INJECT TypeScript strict typing

${prefix}-FE-002:
STATUS [ ]
CREATE src/hooks/useUsers.ts:
  - MIRROR pattern from: src/hooks/useExistingHook.ts
  - ADD API integration: fetchUsers, createUser, updateUser
  - KEEP error handling and loading states
  - PRESERVE React Query patterns
  - INJECT proper TypeScript interfaces

${prefix}-FE-003:
STATUS [ ]
MODIFY src/pages/UsersPage.tsx:
  - FIND pattern: "export default function"
  - ADD user management functionality
  - INJECT useUsers hook integration
  - KEEP page layout and navigation patterns
  - PRESERVE responsive design approach
EOF
}
```

## Phase 4: Comprehensive Task List Generation

### YAML-Based Task Structure Creation
Use `create_file` to generate the comprehensive task list:

```yaml
# Comprehensive Task List: {project_name}

## Project Overview
project_name: "{project_name}"
generated_date: "{current_timestamp}"
requirements_source: "{project_requirements}"
task_granularity: "{task_granularity}"
estimated_total_effort: "{total_effort_estimate}"

## Implementation Strategy
development_approach: "{development_approach}"
primary_patterns: {identified_reusable_patterns}
integration_points: {key_integration_requirements}
quality_gates: {validation_and_testing_strategy}

## Phase 1: Setup and Infrastructure

Task 1:
STATUS [ ]
CREATE project infrastructure:
  - EXECUTE: npm init or dotnet new setup
  - CONFIGURE: package.json with required dependencies
  - ADD: ESLint, Prettier, TypeScript configuration
  - MIRROR: .gitignore from existing project template
  - VALIDATE: Build process runs without errors

Task 2:
STATUS [ ]
CONFIGURE development environment:
  - ADD: VS Code workspace settings
  - CONFIGURE: Debug configurations
  - INJECT: Environment variables template
  - MIRROR: Docker setup from existing project
  - VALIDATE: Development server starts successfully

Task 3:
STATUS [ ]
CREATE database schema:
  - MIRROR: Database setup from existing schema
  - ADD: New tables for {specific_requirements}
  - CONFIGURE: Connection strings and migrations
  - EXECUTE: Initial database setup scripts
  - VALIDATE: Database connectivity and schema creation

## Phase 2: Backend Implementation

Task 4:
STATUS [ ]
CREATE core backend services:
  - MIRROR pattern from: {existing_service_pattern}
  - CREATE: {ServiceName}Service.ts
  - ADD: Business logic methods: {specific_methods}
  - KEEP: Error handling and logging patterns
  - PRESERVE: Dependency injection approach
  - VALIDATE: Unit tests pass for all service methods

Task 5:
STATUS [ ]
CREATE data access layer:
  - MIRROR pattern from: {existing_repository_pattern}
  - CREATE: {EntityName}Repository.ts
  - EXTEND: BaseRepository with entity-specific methods
  - ADD: CRUD operations and custom queries
  - PRESERVE: Transaction handling and connection management
  - VALIDATE: Repository tests pass with mocked database

Task 6:
STATUS [ ]
CREATE API controllers:
  - MIRROR pattern from: {existing_controller_pattern}
  - CREATE: {EntityName}Controller.ts
  - ADD: REST endpoints following OpenAPI standards
  - INJECT: Authentication and authorization middleware
  - KEEP: Error response format and status codes
  - VALIDATE: API tests pass for all endpoints

Task 7:
STATUS [ ]
MODIFY authentication system:
  - FIND: Existing authentication configuration
  - ADD: New role-based permissions for {feature}
  - INJECT: Authorization policies
  - PRESERVE: JWT token handling approach
  - VALIDATE: Authentication flow works end-to-end

## Phase 3: Frontend Implementation

Task 8:
STATUS [ ]
CREATE frontend components:
  - MIRROR pattern from: {existing_component_pattern}
  - CREATE: {ComponentName}.tsx with proper TypeScript
  - ADD: Props interface and component logic
  - KEEP: Tailwind CSS styling approach
  - PRESERVE: Accessibility attributes and ARIA labels
  - VALIDATE: Component renders correctly in Storybook

Task 9:
STATUS [ ]
CREATE custom React hooks:
  - MIRROR pattern from: {existing_hook_pattern}
  - CREATE: use{EntityName}.ts
  - ADD: API integration with React Query
  - INJECT: Error handling and loading states
  - PRESERVE: TypeScript interfaces and generics
  - VALIDATE: Hook tests pass with mocked API

Task 10:
STATUS [ ]
CREATE page components:
  - MIRROR pattern from: {existing_page_pattern}
  - CREATE: {PageName}Page.tsx
  - INJECT: Hook integration and state management
  - ADD: User interaction handlers
  - KEEP: Responsive design and layout patterns
  - VALIDATE: Page navigation and functionality work

Task 11:
STATUS [ ]
MODIFY routing configuration:
  - FIND: Existing route definitions
  - ADD: New routes for {feature_pages}
  - INJECT: Route guards and permissions
  - PRESERVE: Lazy loading and code splitting
  - VALIDATE: All routes work with proper access control

## Phase 4: Integration and Testing

Task 12:
STATUS [ ]
CREATE integration tests:
  - MIRROR pattern from: {existing_integration_test_pattern}
  - CREATE: {Feature}.integration.test.ts
  - ADD: End-to-end workflow validation
  - TEST: API and database integration
  - VALIDATE: All user workflows function correctly

Task 13:
STATUS [ ]
EXECUTE performance testing:
  - ADD: Performance benchmarks for critical paths
  - CONFIGURE: Load testing with realistic data
  - VALIDATE: Response times meet requirements
  - OPTIMIZE: Identified performance bottlenecks

Task 14:
STATUS [ ]
VALIDATE security requirements:
  - EXECUTE: Security scanning and penetration testing
  - VALIDATE: Input sanitization and SQL injection prevention
  - TEST: Authentication and authorization edge cases
  - VERIFY: Data encryption and secure communication

## Phase 5: Documentation and Deployment

Task 15:
STATUS [ ]
CREATE comprehensive documentation:
  - ADD: API documentation with OpenAPI/Swagger
  - CREATE: User documentation and guides
  - UPDATE: Developer README and setup instructions
  - MIRROR: Documentation structure from existing projects
  - VALIDATE: Documentation accuracy and completeness

Task 16:
STATUS [ ]
CONFIGURE deployment pipeline:
  - MIRROR: CI/CD setup from existing projects
  - ADD: Environment-specific configurations
  - CONFIGURE: Automated testing in pipeline
  - INJECT: Security scanning and quality gates
  - VALIDATE: Successful deployment to staging environment

Task 17:
STATUS [ ]
EXECUTE production deployment:
  - VALIDATE: All tests pass in production environment
  - EXECUTE: Database migration scripts
  - CONFIGURE: Monitoring and alerting
  - VERIFY: Application functionality in production
  - CREATE: Rollback procedures and documentation

## Quality Assurance Requirements

### Unit Testing Requirements
- Each service method must have corresponding unit tests
- Component tests must cover all props and user interactions
- Test coverage must exceed 80% for all new code
- Mock external dependencies in isolation tests

### Integration Testing Requirements
- API endpoints tested with realistic data scenarios
- Database operations validated with actual database
- Frontend components tested with backend integration
- Error scenarios and edge cases thoroughly tested

### Performance Requirements
- API response times under 200ms for standard operations
- Page load times under 2 seconds
- Database queries optimized with proper indexing
- Memory usage stays within acceptable limits

### Security Requirements
- Input validation on all API endpoints
- SQL injection prevention in all queries
- XSS protection in all user-facing components
- Authentication and authorization thoroughly tested

## Success Criteria

The task list implementation is complete when:
- [ ] All tasks marked as STATUS [DONE]
- [ ] All unit tests pass with required coverage
- [ ] All integration tests verify complete workflows
- [ ] Performance benchmarks meet requirements
- [ ] Security validation passes all checks
- [ ] Documentation is complete and accurate
- [ ] Production deployment is successful
- [ ] Monitoring and alerting are configured
- [ ] Rollback procedures are tested and documented

## Notes
- Use information-dense keywords consistently throughout implementation
- Follow existing patterns and conventions from codebase analysis
- Validate each task completion before proceeding to dependent tasks
- Maintain comprehensive test coverage at all levels
- Document any deviations from planned approach
```

### Task Validation and Quality Assurance
```bash
# Validate task completion and quality
validate_task_completion() {
    local task_id=$1
    
    echo "🔍 Validating Task Completion: $task_id"
    
    # Run unit tests for task
    echo "🧪 Running Unit Tests..."
    npm test -- --testNamePattern="$task_id" --watchAll=false
    if [[ $? -ne 0 ]]; then
        echo "❌ Unit tests failed for task $task_id"
        return 1
    fi
    
    # Check code quality
    echo "📊 Checking Code Quality..."
    npm run lint
    npm run type-check
    if [[ $? -ne 0 ]]; then
        echo "❌ Code quality checks failed for task $task_id"
        return 1
    fi
    
    # Validate task-specific requirements
    echo "✅ Task-Specific Validation..."
    case $task_id in
        *"CREATE"*)
            validate_creation_task "$task_id"
            ;;
        *"MODIFY"*)
            validate_modification_task "$task_id"
            ;;
        *"MIRROR"*)
            validate_mirroring_task "$task_id"
            ;;
    esac
    
    echo "✅ Task validation completed for $task_id"
    return 0
}

# Update task status in checklist
update_task_status() {
    local task_id=$1
    local status=$2
    local checklist_file="PRPs/checklist.md"
    
    # Update status in the checklist file
    sed -i "s/^$task_id:.*STATUS \[ \]/$task_id:\nSTATUS [DONE]/" "$checklist_file"
    
    echo "✅ Updated $task_id status to $status"
}
```

## Success Criteria

The comprehensive task list creation is complete when:
- [ ] Deep codebase analysis identifies all reusable patterns
- [ ] Requirements are decomposed using ULTRATHINK methodology
- [ ] Tasks use information-dense keywords consistently
- [ ] Task dependencies and sequencing are logical
- [ ] Validation requirements are comprehensive
- [ ] Pattern reuse is maximized appropriately
- [ ] Quality assurance requirements are specified
- [ ] Success criteria are measurable and clear

## Integration Points

### Development Workflow Integration
- Connect with existing development and review processes
- Integrate with project management and tracking systems
- Align with team communication and collaboration practices
- Ensure compatibility with continuous integration pipelines

### Quality Assurance Integration
- Leverage existing testing frameworks and procedures
- Integrate with code quality tools and validation systems
- Connect to monitoring and observability infrastructure
- Align with organizational quality standards

### Knowledge Management Integration
- Contribute task planning approaches to knowledge base
- Share pattern analysis insights with development teams
- Document task breakdown methodologies for reuse
- Update planning procedures based on lessons learned

Remember: Focus on systematic analysis, pattern reuse, and information-dense task creation for effective project implementation.
```

## Notes
- Emphasize deep codebase analysis and pattern discovery
- Use ULTRATHINK methodology for comprehensive planning
- Focus on information-dense keywords for clear task definition
- Include comprehensive validation and quality assurance requirements
