---
mode: agent
description: "Rapid implementation planning with PRP orchestration and development workflow optimization"
---

---
inputs:
  - name: project_type
    description: Project type for quick start (web-app, api, desktop, mobile)
    required: true
  - name: technology_stack
    description: Technology stack preference
    required: false
---

# quick-start-prps.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Generate systematic quick-start implementation plan using Product Requirements Prompts (PRPs) for rapid development setup
- **Categories**: project-setup, prp-workflows, implementation-planning, development-orchestration
- **Complexity**: intermediate
- **Dependencies**: PRP generation system, development environment setup

## Input
- **project_phase** (optional): Specific development phase to focus on (foundation/user-management/content-structure/question-bank/exam-management/all)
- **team_size** (optional): Number of developers on the team (default: 2-4)
- **timeline** (optional): Target timeline for implementation (rapid/standard/comprehensive)

## Template

```
You are an expert development workflow architect specializing in creating systematic implementation plans using Product Requirements Prompts (PRPs) for rapid project development. Your goal is to generate phase-based development plans that maximize team efficiency and ensure proper architectural foundations.

## Input Parameters
- **Project Phase**: {project_phase} (default: all)
- **Team Size**: {team_size} (default: 2-4)
- **Timeline**: {timeline} (default: standard)

## Task Overview
Create a comprehensive quick-start implementation plan using PRPs that establishes proper development foundations, ensures critical path optimization, and provides systematic validation approaches for rapid but robust development.

## Phase 1: Comprehensive Project Analysis and Foundation Planning

### Current Project State Assessment
Use `semantic_search` to understand existing project structure:

```typescript
// Analyze current project state and capabilities
interface ProjectStateAnalysis {
  existing_infrastructure: {
    backend_foundation: Array<{
      component: string;
      completeness: 'missing' | 'partial' | 'complete';
      dependencies: string[];
      priority: 'critical' | 'important' | 'optional';
    }>;
    
    frontend_foundation: Array<{
      component: string;
      setup_status: 'missing' | 'partial' | 'complete';
      dependencies: string[];
      priority: 'critical' | 'important' | 'optional';
    }>;
    
    shared_infrastructure: Array<{
      component: string;
      configuration_status: 'missing' | 'partial' | 'complete';
      impact_scope: 'backend' | 'frontend' | 'both';
      setup_complexity: 'simple' | 'medium' | 'complex';
    }>;
  };
  
  development_readiness: {
    environment_setup: 'ready' | 'partial' | 'needs_setup';
    tooling_configuration: 'complete' | 'partial' | 'missing';
    team_onboarding: 'ready' | 'needs_documentation' | 'needs_training';
    ci_cd_pipeline: 'operational' | 'basic' | 'missing';
  };
  
  critical_path_analysis: {
    blocking_dependencies: string[];
    parallel_work_opportunities: string[];
    resource_allocation_recommendations: string[];
    risk_mitigation_priorities: string[];
  };
}

// Perform comprehensive project assessment
async function assessProjectState(): Promise<ProjectStateAnalysis> {
  // Use semantic_search to understand existing patterns
  // Use file_search to identify completed components
  // Use grep_search to find configuration files
  // Analyze current project structure and capabilities
}
```

### Foundation Requirements Analysis
Use `file_search` to identify existing infrastructure:

```bash
# Search for existing infrastructure components
find_existing_infrastructure() {
    echo "🔍 Analyzing Existing Project Infrastructure"
    
    # Backend infrastructure analysis
    find . -name "*.cs" -path "*/Controllers/*" | head -10
    find . -name "*.cs" -path "*/Services/*" | head -10
    find . -name "*.cs" -path "*/Repositories/*" | head -10
    find . -name "*.cs" -path "*/Entities/*" | head -10
    
    # Frontend infrastructure analysis
    find . -name "*.tsx" -path "*/components/*" | head -10
    find . -name "*.ts" -path "*/hooks/*" | head -10
    find . -name "*.ts" -path "*/services/*" | head -10
    find . -name "*.ts" -path "*/types/*" | head -10
    
    # Configuration analysis
    find . -name "*.json" -o -name "*.yml" -o -name "*.yaml" | grep -E "(package|tsconfig|eslint|vite|docker)" | head -10
    
    # Database and schema analysis
    find . -name "*.sql" -o -name "*.cs" -path "*/Migrations/*" | head -10
}
```

## Phase 2: Strategic PRP Generation Plan

### Phase-Based Implementation Strategy
```typescript
// Comprehensive PRP implementation roadmap
interface PRPImplementationRoadmap {
  implementation_phases: Array<{
    phase_number: number;
    phase_name: string;
    phase_description: string;
    estimated_duration: string;
    team_allocation: string;
    
    prerequisite_phases: number[];
    parallel_opportunities: number[];
    
    core_prps: Array<{
      prp_command: string;
      prp_purpose: string;
      implementation_priority: 'critical' | 'important' | 'optional';
      estimated_effort: 'small' | 'medium' | 'large';
      dependencies: string[];
      deliverables: string[];
      validation_criteria: string[];
    }>;
    
    validation_checkpoints: Array<{
      checkpoint_name: string;
      validation_commands: string[];
      success_criteria: string[];
      rollback_procedures: string[];
    }>;
    
    risk_factors: Array<{
      risk_description: string;
      mitigation_strategy: string;
      early_warning_signs: string[];
      contingency_plan: string;
    }>;
  }>;
  
  critical_path: {
    essential_prps: string[];
    blocking_dependencies: string[];
    parallel_execution_groups: Array<{
      group_name: string;
      parallel_prps: string[];
      synchronization_points: string[];
    }>;
  };
  
  team_coordination: {
    backend_focused_prps: string[];
    frontend_focused_prps: string[];
    full_stack_prps: string[];
    integration_prps: string[];
  };
}
```

### Foundation Phase PRPs (Phase 1 - Critical Infrastructure)
```bash
# Foundation phase implementation
generate_foundation_prps() {
    echo "🏗️ Phase 1: Foundation Infrastructure Setup"
    
    cat > "phase-1-foundation-prps.md" << 'EOF'
# Phase 1: Foundation Infrastructure (Est: 3-5 days)

## Critical Infrastructure PRPs (Execute in Order)

### 1. Core Backend Foundation
```bash
# Essential backend infrastructure
@copilot /generate-prp core core-entities-setup .github/copilot/requirements/schema.sql
# Purpose: Establish base entity classes, audit fields, and common interfaces
# Deliverables: BaseEntity.cs, IAuditable.cs, IEntity.cs
# Validation: dotnet build succeeds, entities follow SRP

@copilot /generate-prp core base-repository-pattern .github/copilot/requirements/schema.sql  
# Purpose: Create generic repository pattern with CRUD operations
# Deliverables: IBaseRepository.cs, BaseRepository.cs, dependency injection
# Validation: Repository tests pass, follows existing patterns

@copilot /generate-prp core api-foundation .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Set up API controllers, middleware, error handling
# Deliverables: Base controllers, error middleware, API documentation
# Validation: Health endpoints work, Swagger UI accessible
```

### 2. Authentication & Security Infrastructure  
```bash
# Security and authentication setup
@copilot /generate-prp authentication authentication-system .github/copilot/requirements/ikhtibar-features.txt
# Purpose: JWT authentication, authorization policies, security middleware
# Deliverables: AuthController, JWT service, security configuration
# Validation: Login/logout work, protected endpoints secured

@copilot /generate-prp authentication role-based-access .github/copilot/requirements/data.sql
# Purpose: Role-based authorization system with permissions
# Deliverables: Role/Permission entities, authorization policies
# Validation: Role assignments work, permission checks functional
```

### 3. Frontend Foundation
```bash
# Frontend infrastructure setup
@copilot /generate-prp frontend frontend-foundation .github/copilot/requirements/ikhtibar-features.txt
# Purpose: React setup, routing, state management, API integration
# Deliverables: App structure, routing, HTTP client, global state
# Validation: App builds, routing works, API calls functional

@copilot /generate-prp frontend authentication-frontend .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Login UI, authentication state, protected routes
# Deliverables: Login forms, auth context, route guards
# Validation: Login flow works, protected routes enforced
```

## Phase 1 Validation Checkpoint
```bash
# Comprehensive validation after Phase 1
validate_phase_1() {
    echo "🧪 Validating Foundation Phase"
    
    # Backend validation
    cd backend
    dotnet clean
    dotnet build
    dotnet test
    dotnet format --verify-no-changes
    
    # Check critical endpoints
    dotnet run &
    BACKEND_PID=$!
    sleep 10
    
    curl -f http://localhost:5000/api/health || echo "❌ Health endpoint failed"
    curl -f http://localhost:5000/swagger/index.html || echo "❌ Swagger UI failed"
    
    kill $BACKEND_PID
    
    # Frontend validation
    cd ../frontend
    npm run type-check
    npm run lint
    npm run test
    npm run build
    
    # Check application starts
    npm run dev &
    FRONTEND_PID=$!
    sleep 10
    
    curl -f http://localhost:5173 || echo "❌ Frontend failed to start"
    
    kill $FRONTEND_PID
    
    echo "✅ Phase 1 validation complete"
}
```

## Phase 1 Success Criteria
- [ ] Backend builds without errors
- [ ] All foundation tests pass
- [ ] Health endpoints respond correctly
- [ ] Swagger documentation accessible
- [ ] Frontend builds and starts without errors
- [ ] Authentication flow functional
- [ ] All linting and type checking passes
- [ ] Repository pattern working correctly
- [ ] Security middleware operational

## Risk Mitigation - Phase 1
- **Database Connection Issues**: Verify connection strings and permissions
- **Authentication Complexity**: Start with basic JWT, enhance incrementally
- **Frontend Build Issues**: Ensure Node.js and npm versions compatible
- **Dependency Conflicts**: Use exact versions in package.json and .csproj
EOF
}
```

### User Management Phase PRPs (Phase 2 - Critical First Module)
```bash
# User management phase implementation
generate_user_management_prps() {
    echo "👥 Phase 2: User Management System"
    
    cat > "phase-2-user-management-prps.md" << 'EOF'
# Phase 2: User Management System (Est: 4-6 days)

## User Management PRPs (Critical Business Module)

### 1. Backend User Services
```bash
# Complete user management backend
@copilot /generate-prp user-management backend-services .github/copilot/requirements/data.sql
# Purpose: User CRUD operations, role assignments, profile management
# Deliverables: UserService, UserRepository, UserController, DTOs
# Validation: CRUD operations work, role assignments functional

@copilot /generate-prp user-management user-validation-business-rules .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Business rules for user creation, validation, constraints
# Deliverables: Validation services, business rule engine, constraints
# Validation: Business rules enforced, validation errors clear

@copilot /generate-prp user-management audit-logging .github/copilot/requirements/schema.sql
# Purpose: Audit trail for user changes, security logging
# Deliverables: Audit entities, logging service, audit trails
# Validation: User changes logged, audit reports available
```

### 2. Frontend User Interface
```bash
# User management UI components
@copilot /generate-prp user-management frontend-components .github/copilot/requirements/ikhtibar-features.txt
# Purpose: User list, create/edit forms, role assignment UI
# Deliverables: User components, forms, data grids, role management
# Validation: UI functional, responsive, accessible

@copilot /generate-prp user-management user-profile-management .github/copilot/requirements/ikhtibar-features.txt
# Purpose: User profile editing, password changes, preferences
# Deliverables: Profile components, password forms, preference settings
# Validation: Profile updates work, password changes functional

@copilot /generate-prp user-management user-search-filtering .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Advanced user search, filtering, pagination
# Deliverables: Search components, filters, pagination, sorting
# Validation: Search functional, filters work, performance acceptable
```

## Phase 2 Validation Checkpoint
```bash
# User management validation
validate_phase_2() {
    echo "🧪 Validating User Management Phase"
    
    # Backend API validation
    cd backend && dotnet run &
    BACKEND_PID=$!
    sleep 10
    
    # Test user management endpoints
    curl -X GET http://localhost:5000/api/users || echo "❌ Get users failed"
    curl -X POST http://localhost:5000/api/users -H "Content-Type: application/json" -d '{"email":"test@example.com","name":"Test User"}' || echo "❌ Create user failed"
    
    kill $BACKEND_PID
    
    # Frontend validation
    cd ../frontend
    npm run test:user-management
    npm run build
    
    # Integration test
    npm run test:integration:user-management
    
    echo "✅ Phase 2 validation complete"
}
```

## Phase 2 Success Criteria
- [ ] User CRUD operations functional
- [ ] Role assignment system working
- [ ] User search and filtering operational
- [ ] Profile management functional
- [ ] Audit logging capturing changes
- [ ] Frontend UI responsive and accessible
- [ ] Integration tests passing
- [ ] Performance benchmarks met

EOF
}
```

### Content Structure Phase PRPs (Phase 3 - Enables Question Organization)
```bash
# Content structure phase implementation
generate_content_structure_prps() {
    echo "🌳 Phase 3: Hierarchical Content Structure"
    
    cat > "phase-3-content-structure-prps.md" << 'EOF'
# Phase 3: Hierarchical Content Structure (Est: 3-4 days)

## Content Organization PRPs (Foundation for Question Bank)

### 1. Tree Structure Backend
```bash
# Hierarchical content organization
@copilot /generate-prp tree-structure backend-hierarchy .github/copilot/requirements/schema.sql
# Purpose: Tree node management, hierarchy operations, category system
# Deliverables: TreeNode entities, hierarchy service, category management
# Validation: Tree operations work, hierarchy integrity maintained

@copilot /generate-prp tree-structure tree-operations-advanced .github/copilot/requirements/schema.sql
# Purpose: Advanced tree operations (move, copy, merge, reorganize)
# Deliverables: Tree operation services, batch operations, validation
# Validation: Complex tree operations functional, data integrity preserved

@copilot /generate-prp tree-structure content-classification .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Content tagging, classification, metadata management
# Deliverables: Tag system, classification engine, metadata service
# Validation: Tagging functional, classification accurate, search efficient
```

### 2. Tree Structure Frontend
```bash
# Tree management UI components
@copilot /generate-prp tree-structure frontend-components .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Tree view components, drag-drop operations, category management
# Deliverables: Tree components, drag-drop UI, category interfaces
# Validation: Tree UI functional, drag-drop works, categories manageable

@copilot /generate-prp tree-structure tree-visualization-advanced .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Advanced tree visualization, search within tree, navigation
# Deliverables: Enhanced tree views, search integration, breadcrumbs
# Validation: Visualizations clear, search functional, navigation intuitive
```

## Phase 3 Success Criteria
- [ ] Tree structure backend operational
- [ ] Advanced tree operations functional
- [ ] Content classification system working
- [ ] Tree UI components responsive
- [ ] Drag-drop operations smooth
- [ ] Tree visualization clear and intuitive
- [ ] Integration with user management complete

EOF
}
```

### Question Bank Phase PRPs (Phase 4 - Core Feature)
```bash
# Question bank phase implementation
generate_question_bank_prps() {
    echo "❓ Phase 4: Question Bank System"
    
    cat > "phase-4-question-bank-prps.md" << 'EOF'
# Phase 4: Question Bank System (Est: 7-10 days)

## Question Management PRPs (Core Business Feature)

### 1. Question Backend Infrastructure
```bash
# Core question management system
@copilot /generate-prp question-bank backend-management .github/copilot/requirements/schema.sql
# Purpose: Question CRUD, versioning, workflow management
# Deliverables: Question entities, question service, workflow engine
# Validation: Question operations functional, workflows working

@copilot /generate-prp question-bank question-types .github/copilot/requirements/data.sql
# Purpose: Multiple choice, essay, matching, multimedia question types
# Deliverables: Question type handlers, polymorphic question system
# Validation: All question types supported, rendering correct

@copilot /generate-prp question-bank media-management .github/copilot/requirements/schema.sql
# Purpose: Image, audio, video support for questions
# Deliverables: Media upload service, media processing, media storage
# Validation: Media upload works, processing functional, storage secure

@copilot /generate-prp question-bank review-workflow .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Question review process, approval workflow, quality control
# Deliverables: Review system, approval workflow, quality metrics
# Validation: Review process works, approvals functional, quality maintained
```

### 2. Question Frontend Interface
```bash
# Question management UI
@copilot /generate-prp question-bank frontend-management .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Question creation, editing, preview, management interface
# Deliverables: Question forms, editors, preview components, management UI
# Validation: Question creation intuitive, editing functional, previews accurate

@copilot /generate-prp question-bank question-editor-advanced .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Rich text editor, formula support, media integration
# Deliverables: Advanced editor, formula renderer, media integration
# Validation: Editor functional, formulas render, media integrated properly

@copilot /generate-prp question-bank question-search-analytics .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Question search, filtering, analytics, usage tracking
# Deliverables: Search engine, analytics dashboard, usage reports
# Validation: Search accurate, analytics meaningful, reports useful
```

## Phase 4 Success Criteria
- [ ] All question types supported
- [ ] Media integration functional
- [ ] Review workflow operational
- [ ] Question editor advanced and intuitive
- [ ] Search and filtering efficient
- [ ] Analytics providing insights
- [ ] Integration with content structure complete

EOF
}
```

### Exam Management Phase PRPs (Phase 5 - Core Feature)
```bash
# Exam management phase implementation
generate_exam_management_prps() {
    echo "📝 Phase 5: Exam Management System"
    
    cat > "phase-5-exam-management-prps.md" << 'EOF'
# Phase 5: Exam Management System (Est: 8-12 days)

## Exam Creation and Management PRPs

### 1. Exam Backend Infrastructure
```bash
# Core exam management system
@copilot /generate-prp exam-management backend-creation .github/copilot/requirements/schema.sql
# Purpose: Exam creation, template management, exam configuration
# Deliverables: Exam entities, exam service, template system
# Validation: Exam creation functional, templates working

@copilot /generate-prp exam-management blueprint-system .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Exam blueprints, question selection rules, automated generation
# Deliverables: Blueprint engine, selection algorithms, generation service
# Validation: Blueprints accurate, selection optimal, generation reliable

@copilot /generate-prp exam-management scheduling-system .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Exam scheduling, availability management, conflict resolution
# Deliverables: Scheduling service, availability checker, conflict resolver
# Validation: Scheduling functional, conflicts detected, resolution working

@copilot /generate-prp exam-management session-management .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Exam session control, proctoring support, security measures
# Deliverables: Session controller, proctoring service, security measures
# Validation: Sessions controlled, proctoring functional, security maintained
```

### 2. Exam Frontend Interface
```bash
# Exam management UI
@copilot /generate-prp exam-management frontend-creation .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Exam creation wizard, blueprint configuration, preview system
# Deliverables: Creation wizard, blueprint UI, preview components
# Validation: Wizard intuitive, blueprint configuration clear, previews accurate

@copilot /generate-prp exam-management frontend-scheduling .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Scheduling interface, calendar integration, availability display
# Deliverables: Scheduling UI, calendar components, availability views
# Validation: Scheduling intuitive, calendar functional, availability clear

@copilot /generate-prp exam-execution student-interface .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Student exam taking interface, progress tracking, submission
# Deliverables: Exam taking UI, progress indicators, submission system
# Validation: Interface user-friendly, progress accurate, submission reliable
```

## Phase 5 Success Criteria
- [ ] Exam creation wizard functional
- [ ] Blueprint system operational
- [ ] Scheduling system working
- [ ] Session management robust
- [ ] Student interface intuitive
- [ ] Security measures effective
- [ ] Integration with question bank complete

EOF
}
```

### Grading Phase PRPs (Phase 6 - Completion Feature)
```bash
# Grading phase implementation
generate_grading_prps() {
    echo "📊 Phase 6: Grading and Analytics System"
    
    cat > "phase-6-grading-prps.md" << 'EOF'
# Phase 6: Grading and Analytics System (Est: 5-7 days)

## Grading System PRPs

### 1. Grading Backend Infrastructure
```bash
# Automated and manual grading systems
@copilot /generate-prp grading auto-grading-system .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Automated grading for objective questions, scoring algorithms
# Deliverables: Auto-grading engine, scoring service, result calculation
# Validation: Auto-grading accurate, scoring consistent, results reliable

@copilot /generate-prp grading manual-grading-system .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Manual grading for subjective questions, rubric system
# Deliverables: Manual grading interface, rubric engine, scoring tools
# Validation: Manual grading efficient, rubrics functional, scoring fair

@copilot /generate-prp grading student-submissions .github/copilot/requirements/schema.sql
# Purpose: Submission tracking, version control, plagiarism detection
# Deliverables: Submission service, version tracking, detection algorithms
# Validation: Submissions tracked, versions managed, detection accurate

@copilot /generate-prp grading calculation-analytics .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Grade calculation, statistics, performance analytics
# Deliverables: Calculation engine, statistics service, analytics dashboard
# Validation: Calculations accurate, statistics meaningful, analytics insightful
```

### 2. Grading Frontend Interface
```bash
# Grading and analytics UI
@copilot /generate-prp grading frontend-interface .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Grading interface, rubric management, result display
# Deliverables: Grading UI, rubric components, result views
# Validation: Interface efficient, rubrics manageable, results clear

@copilot /generate-prp grading analytics-dashboard .github/copilot/requirements/ikhtibar-features.txt
# Purpose: Performance analytics, reporting, data visualization
# Deliverables: Analytics dashboard, report generators, visualization components
# Validation: Dashboard informative, reports comprehensive, visualizations clear
```

## Phase 6 Success Criteria
- [ ] Auto-grading system accurate
- [ ] Manual grading interface efficient
- [ ] Submission tracking reliable
- [ ] Grade calculations correct
- [ ] Analytics dashboard informative
- [ ] Reporting system comprehensive
- [ ] Integration with exam management complete

EOF
}
```

## Phase 3: Implementation Orchestration and Team Coordination

### Team Workflow Optimization
```typescript
// Team coordination and parallel execution strategy
interface TeamWorkflowOrchestration {
  parallel_execution_groups: Array<{
    group_name: string;
    execution_strategy: 'sequential' | 'parallel' | 'mixed';
    
    team_assignments: Array<{
      developer_role: 'backend_lead' | 'frontend_lead' | 'full_stack' | 'devops';
      assigned_prps: string[];
      dependency_coordination: string[];
      communication_checkpoints: string[];
    }>;
    
    synchronization_points: Array<{
      checkpoint_name: string;
      required_completions: string[];
      validation_requirements: string[];
      go_no_go_criteria: string[];
    }>;
    
    integration_planning: Array<{
      integration_point: string;
      participating_teams: string[];
      integration_strategy: string;
      testing_approach: string[];
    }>;
  }>;
  
  risk_management: {
    dependency_risks: Array<{
      dependency_description: string;
      impact_assessment: 'low' | 'medium' | 'high';
      mitigation_strategy: string[];
      contingency_plan: string;
    }>;
    
    resource_risks: Array<{
      resource_constraint: string;
      impact_on_timeline: string;
      optimization_strategies: string[];
      alternative_approaches: string[];
    }>;
    
    technical_risks: Array<{
      technical_challenge: string;
      complexity_rating: 'low' | 'medium' | 'high';
      research_requirements: string[];
      prototype_needs: string[];
    }>;
  };
}
```

### Continuous Validation Framework
Use `run_in_terminal` to implement automated validation:

```bash
# Comprehensive validation automation
setup_continuous_validation() {
    echo "🔄 Setting Up Continuous Validation Framework"
    
    cat > "continuous-validation.sh" << 'EOF'
#!/bin/bash

# Continuous validation script for PRP implementations
VALIDATION_LOG="validation-results-$(date +%Y%m%d-%H%M%S).log"

echo "🚀 Starting Continuous Validation Pipeline" | tee $VALIDATION_LOG

# Phase 1: Foundation Validation
validate_foundation_phase() {
    echo "📋 Validating Foundation Phase..." | tee -a $VALIDATION_LOG
    
    # Backend foundation validation
    cd backend
    echo "  🔧 Backend build validation..." | tee -a $VALIDATION_LOG
    dotnet clean >> $VALIDATION_LOG 2>&1
    dotnet build >> $VALIDATION_LOG 2>&1 || { echo "❌ Backend build failed"; exit 1; }
    
    echo "  🧪 Backend test validation..." | tee -a $VALIDATION_LOG
    dotnet test >> $VALIDATION_LOG 2>&1 || { echo "❌ Backend tests failed"; exit 1; }
    
    echo "  📏 Code formatting validation..." | tee -a $VALIDATION_LOG
    dotnet format --verify-no-changes >> $VALIDATION_LOG 2>&1 || { echo "❌ Code formatting issues"; exit 1; }
    
    # Frontend foundation validation
    cd ../frontend
    echo "  📦 Frontend dependencies validation..." | tee -a $VALIDATION_LOG
    npm ci >> $VALIDATION_LOG 2>&1 || { echo "❌ Frontend dependency install failed"; exit 1; }
    
    echo "  🔍 TypeScript validation..." | tee -a $VALIDATION_LOG
    npm run type-check >> $VALIDATION_LOG 2>&1 || { echo "❌ TypeScript errors found"; exit 1; }
    
    echo "  📐 Linting validation..." | tee -a $VALIDATION_LOG
    npm run lint >> $VALIDATION_LOG 2>&1 || { echo "❌ Linting errors found"; exit 1; }
    
    echo "  🧪 Frontend test validation..." | tee -a $VALIDATION_LOG
    npm run test >> $VALIDATION_LOG 2>&1 || { echo "❌ Frontend tests failed"; exit 1; }
    
    echo "  🏗️ Build validation..." | tee -a $VALIDATION_LOG
    npm run build >> $VALIDATION_LOG 2>&1 || { echo "❌ Frontend build failed"; exit 1; }
    
    echo "✅ Foundation phase validation complete" | tee -a $VALIDATION_LOG
    cd ..
}

# Phase 2: Integration Validation
validate_integration_phase() {
    echo "🔗 Validating Integration Phase..." | tee -a $VALIDATION_LOG
    
    # Start backend for integration testing
    cd backend
    echo "  🚀 Starting backend server..." | tee -a $VALIDATION_LOG
    dotnet run >> $VALIDATION_LOG 2>&1 &
    BACKEND_PID=$!
    
    # Wait for backend to be ready
    echo "  ⏳ Waiting for backend to be ready..." | tee -a $VALIDATION_LOG
    for i in {1..30}; do
        curl -f http://localhost:5000/api/health >> $VALIDATION_LOG 2>&1 && break
        sleep 2
    done
    
    # Validate critical endpoints
    echo "  🔍 Testing critical endpoints..." | tee -a $VALIDATION_LOG
    curl -f http://localhost:5000/api/health >> $VALIDATION_LOG 2>&1 || { echo "❌ Health endpoint failed"; kill $BACKEND_PID; exit 1; }
    curl -f http://localhost:5000/swagger/index.html >> $VALIDATION_LOG 2>&1 || { echo "❌ Swagger UI failed"; kill $BACKEND_PID; exit 1; }
    
    # Test authentication endpoints if available
    curl -X POST http://localhost:5000/api/auth/login -H "Content-Type: application/json" -d '{"email":"test@example.com","password":"test"}' >> $VALIDATION_LOG 2>&1 || echo "⚠️ Auth endpoint not yet available"
    
    # Start frontend for integration testing
    cd ../frontend
    echo "  🎨 Starting frontend server..." | tee -a $VALIDATION_LOG
    npm run dev >> $VALIDATION_LOG 2>&1 &
    FRONTEND_PID=$!
    
    # Wait for frontend to be ready
    echo "  ⏳ Waiting for frontend to be ready..." | tee -a $VALIDATION_LOG
    for i in {1..30}; do
        curl -f http://localhost:5173 >> $VALIDATION_LOG 2>&1 && break
        sleep 2
    done
    
    # Run integration tests if available
    echo "  🧪 Running integration tests..." | tee -a $VALIDATION_LOG
    npm run test:integration >> $VALIDATION_LOG 2>&1 || echo "⚠️ Integration tests not yet available"
    
    # Cleanup
    echo "  🧹 Cleaning up test servers..." | tee -a $VALIDATION_LOG
    kill $BACKEND_PID $FRONTEND_PID 2>/dev/null || true
    
    echo "✅ Integration phase validation complete" | tee -a $VALIDATION_LOG
    cd ..
}

# Phase 3: Performance Validation
validate_performance_phase() {
    echo "⚡ Validating Performance Phase..." | tee -a $VALIDATION_LOG
    
    # Database performance checks
    echo "  🗄️ Database performance validation..." | tee -a $VALIDATION_LOG
    # Add database performance tests here
    
    # API performance checks
    echo "  🚀 API performance validation..." | tee -a $VALIDATION_LOG
    # Add API performance tests here
    
    # Frontend performance checks
    echo "  🎨 Frontend performance validation..." | tee -a $VALIDATION_LOG
    cd frontend
    npm run build >> $VALIDATION_LOG 2>&1
    
    # Analyze bundle size
    echo "  📦 Bundle size analysis..." | tee -a $VALIDATION_LOG
    npm run analyze >> $VALIDATION_LOG 2>&1 || echo "⚠️ Bundle analysis not configured"
    
    echo "✅ Performance phase validation complete" | tee -a $VALIDATION_LOG
    cd ..
}

# Phase 4: Security Validation
validate_security_phase() {
    echo "🛡️ Validating Security Phase..." | tee -a $VALIDATION_LOG
    
    # Backend security validation
    echo "  🔒 Backend security validation..." | tee -a $VALIDATION_LOG
    cd backend
    
    # Check for hardcoded secrets
    echo "  🔍 Scanning for hardcoded secrets..." | tee -a $VALIDATION_LOG
    grep -r "password\|secret\|key" --include="*.cs" --include="*.json" . | grep -v "\.git" | grep -v "test" >> $VALIDATION_LOG 2>&1 || echo "  ✅ No hardcoded secrets found"
    
    # Dependency vulnerability check
    echo "  🔍 Checking for vulnerable dependencies..." | tee -a $VALIDATION_LOG
    dotnet list package --vulnerable >> $VALIDATION_LOG 2>&1 || echo "  ⚠️ Vulnerability check not available"
    
    # Frontend security validation
    echo "  🔒 Frontend security validation..." | tee -a $VALIDATION_LOG
    cd ../frontend
    
    # Dependency vulnerability check
    echo "  🔍 Checking npm vulnerabilities..." | tee -a $VALIDATION_LOG
    npm audit >> $VALIDATION_LOG 2>&1 || echo "  ⚠️ Some npm vulnerabilities found - review required"
    
    echo "✅ Security phase validation complete" | tee -a $VALIDATION_LOG
    cd ..
}

# Execute all validation phases
main() {
    echo "🎯 Starting Comprehensive Validation Pipeline" | tee -a $VALIDATION_LOG
    
    validate_foundation_phase
    validate_integration_phase
    validate_performance_phase
    validate_security_phase
    
    echo "🎉 All validation phases completed successfully!" | tee -a $VALIDATION_LOG
    echo "📊 Validation log saved to: $VALIDATION_LOG"
}

# Execute main function
main "$@"
EOF

chmod +x continuous-validation.sh
}
```

## Phase 4: Success Metrics and Monitoring

### Implementation Success Dashboard
Use `create_file` to generate implementation tracking:

```markdown
# Implementation Success Tracking Dashboard

## Phase Completion Metrics

### Foundation Phase (Phase 1)
- [ ] Backend infrastructure operational (Target: 100%)
- [ ] Authentication system functional (Target: 100%)
- [ ] Frontend foundation complete (Target: 100%)
- [ ] Security middleware operational (Target: 100%)
- [ ] Documentation complete (Target: 100%)

**Current Status**: X% Complete
**Estimated Completion**: X days remaining
**Risk Level**: Low/Medium/High

### User Management Phase (Phase 2)
- [ ] User CRUD operations (Target: 100%)
- [ ] Role assignment system (Target: 100%)
- [ ] Profile management (Target: 100%)
- [ ] Audit logging (Target: 100%)
- [ ] Frontend UI components (Target: 100%)

**Current Status**: X% Complete
**Estimated Completion**: X days remaining
**Risk Level**: Low/Medium/High

### Content Structure Phase (Phase 3)
- [ ] Tree structure backend (Target: 100%)
- [ ] Advanced tree operations (Target: 100%)
- [ ] Content classification (Target: 100%)
- [ ] Tree UI components (Target: 100%)
- [ ] Visualization system (Target: 100%)

**Current Status**: X% Complete
**Estimated Completion**: X days remaining
**Risk Level**: Low/Medium/High

### Question Bank Phase (Phase 4)
- [ ] Question management system (Target: 100%)
- [ ] Multiple question types (Target: 100%)
- [ ] Media integration (Target: 100%)
- [ ] Review workflow (Target: 100%)
- [ ] Advanced editor (Target: 100%)

**Current Status**: X% Complete
**Estimated Completion**: X days remaining
**Risk Level**: Low/Medium/High

### Exam Management Phase (Phase 5)
- [ ] Exam creation system (Target: 100%)
- [ ] Blueprint system (Target: 100%)
- [ ] Scheduling system (Target: 100%)
- [ ] Session management (Target: 100%)
- [ ] Student interface (Target: 100%)

**Current Status**: X% Complete
**Estimated Completion**: X days remaining
**Risk Level**: Low/Medium/High

### Grading Phase (Phase 6)
- [ ] Auto-grading system (Target: 100%)
- [ ] Manual grading interface (Target: 100%)
- [ ] Submission tracking (Target: 100%)
- [ ] Analytics dashboard (Target: 100%)
- [ ] Reporting system (Target: 100%)

**Current Status**: X% Complete
**Estimated Completion**: X days remaining
**Risk Level**: Low/Medium/High

## Overall Project Health

**Total Completion**: X% of 6 phases
**Timeline Status**: On Track / Behind / Ahead
**Quality Metrics**: X% test coverage, X technical debt items
**Team Velocity**: X PRPs completed per week
**Risk Assessment**: Low/Medium/High overall risk

## Next Actions

### Immediate (Next 24 hours)
1. [Priority action item]
2. [Priority action item]
3. [Priority action item]

### Short Term (Next Week)
1. [Week priority 1]
2. [Week priority 2]
3. [Week priority 3]

### Medium Term (Next Sprint)
1. [Sprint priority 1]
2. [Sprint priority 2]
3. [Sprint priority 3]

## Team Coordination

### Backend Team Focus
- Current PRPs in progress
- Next PRP assignments
- Integration dependencies

### Frontend Team Focus
- Current PRPs in progress
- Next PRP assignments
- Design system alignment

### Integration Team Focus
- Current integration points
- Testing coordination
- Deployment preparation

---
*Dashboard updated automatically based on PRP completion status*
```

## Success Criteria

The quick-start PRP implementation plan is complete when:
- [ ] All six phases have detailed PRP implementation plans
- [ ] Team coordination strategies are defined
- [ ] Continuous validation framework is operational
- [ ] Success metrics tracking is implemented
- [ ] Risk mitigation strategies are comprehensive
- [ ] Integration checkpoints are scheduled
- [ ] Quality gates are defined and enforced
- [ ] Documentation is complete and accessible

## Integration Points

### Development Environment Integration
- Connect with existing IDE configurations and development tools
- Integrate with version control workflows and branching strategies
- Align with team collaboration platforms and communication tools
- Ensure compatibility with CI/CD pipeline requirements

### Project Management Integration
- Synchronize with existing project tracking and management systems
- Integrate milestone tracking with organizational reporting requirements
- Connect progress metrics with team performance dashboards
- Align deliverable schedules with organizational timelines

### Quality Assurance Integration
- Leverage existing testing frameworks and validation procedures
- Integrate with code quality tools and automated checks
- Connect to monitoring and observability infrastructure
- Align with organizational quality standards and compliance requirements

### Knowledge Management Integration
- Contribute implementation patterns to organizational knowledge base
- Share PRP methodologies with other development teams
- Document lessons learned for future project planning
- Update organizational development guidelines based on experiences

Remember: Execute PRPs systematically, validate continuously, and maintain team coordination throughout the implementation process.
```

## Notes
- Focus on systematic phase-based implementation
- Emphasize continuous validation and quality gates
- Include comprehensive team coordination strategies
- Provide detailed success tracking and monitoring
