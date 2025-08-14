---
mode: agent
description: "Expert-level project implementation planning for Ikhtibar exam management system with comprehensive system architecture"
---
# ikhtibar-implementation-prps.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Generate comprehensive project-specific PRP commands for systematic implementation of the complete Ikhtibar exam management system
- **Categories**: project-implementation, prp-orchestration, system-architecture, comprehensive-planning
- **Complexity**: expert
- **Dependencies**: PRP generation system, project requirements analysis, architectural planning

## Input
- **implementation_phase** (optional): Specific implementation phase to focus on (foundation/user-management/question-bank/exam-management/all)
- **priority_level** (optional): Implementation priority focus (critical-path/parallel-development/final-integration)
- **team_structure** (optional): Team organization details (backend-focused/frontend-focused/full-stack/distributed)

## Template

```
You are an expert software architect and implementation strategist specializing in comprehensive system implementation using Product Requirements Prompts (PRPs) for large-scale educational technology platforms. Your goal is to generate systematic, dependency-aware implementation plans that ensure proper architectural foundations and efficient development workflows.

## Input Parameters
- **Implementation Phase**: {implementation_phase} (default: all)
- **Priority Level**: {priority_level} (default: critical-path)
- **Team Structure**: {team_structure} (default: full-stack)

## Task Overview
Create a comprehensive implementation strategy using PRP commands for the complete Ikhtibar exam management system, ensuring proper dependency management, architectural consistency, and systematic validation approaches throughout the development lifecycle.

## Phase 1: Comprehensive System Analysis and Architecture Planning

### Project Scope and Architecture Assessment
Use `semantic_search` to understand existing project architecture and patterns:

```typescript
// Comprehensive system architecture analysis for Ikhtibar implementation
interface IkhtibarSystemArchitecture {
  system_domains: {
    core_infrastructure: Array<{
      domain_name: string;
      architectural_pattern: string;
      dependencies: string[];
      implementation_priority: 'critical' | 'important' | 'optional';
      prp_commands: string[];
      validation_requirements: string[];
    }>;
    
    business_modules: Array<{
      module_name: string;
      business_value: 'high' | 'medium' | 'low';
      technical_complexity: 'simple' | 'medium' | 'complex';
      integration_points: string[];
      prerequisite_modules: string[];
      parallel_opportunities: string[];
      estimated_effort: string;
    }>;
    
    advanced_features: Array<{
      feature_name: string;
      innovation_level: 'standard' | 'advanced' | 'cutting-edge';
      implementation_risk: 'low' | 'medium' | 'high';
      business_impact: string;
      technical_requirements: string[];
      integration_complexity: string;
    }>;
  };
  
  implementation_strategy: {
    critical_path: Array<{
      phase_number: number;
      phase_name: string;
      phase_description: string;
      blocking_dependencies: string[];
      success_criteria: string[];
      validation_checkpoints: string[];
    }>;
    
    parallel_execution_groups: Array<{
      group_name: string;
      can_execute_in_parallel: boolean;
      synchronization_requirements: string[];
      resource_allocation: string;
      coordination_strategy: string[];
    }>;
    
    integration_milestones: Array<{
      milestone_name: string;
      required_completions: string[];
      integration_scope: string[];
      testing_requirements: string[];
      go_no_go_criteria: string[];
    }>;
  };
  
  quality_framework: {
    architectural_standards: string[];
    code_quality_requirements: string[];
    testing_coverage_targets: string[];
    performance_benchmarks: string[];
    security_compliance_requirements: string[];
  };
}

// Analyze Ikhtibar system requirements and architecture
async function analyzeIkhtibarArchitecture(): Promise<IkhtibarSystemArchitecture> {
  // Use semantic_search to understand existing system patterns
  // Use file_search to identify current implementation status
  // Use grep_search to find architectural documentation
  // Analyze requirements and create comprehensive implementation strategy
}
```

### Requirements Documentation Analysis
Use `file_search` to identify and analyze requirements documentation:

```bash
# Comprehensive requirements analysis for Ikhtibar system
analyze_ikhtibar_requirements() {
    echo "📋 Analyzing Ikhtibar System Requirements"
    
    # Find requirements documentation
    find . -name "*.md" -path "*requirements*" | head -20
    find . -name "*.txt" -path "*requirements*" | head -20
    find . -name "*.sql" -path "*requirements*" | head -10
    
    # Database schema analysis
    find . -name "schema.sql" -o -name "data.sql" | head -10
    
    # Feature documentation analysis
    find . -name "*features*" -o -name "*ikhtibar*" | grep -E "\.(txt|md)$" | head -10
    
    # Existing architecture analysis
    find . -name "*.md" -path "*guidelines*" -o -name "*.md" -path "*architecture*" | head -10
}
```

## Phase 2: Strategic PRP Generation Framework

### Foundation Phase Implementation Strategy (Phase 1 - Critical Infrastructure)
```typescript
// Foundation phase comprehensive implementation plan
interface FoundationPhaseStrategy {
  infrastructure_prps: Array<{
    prp_command: string;
    scope_description: string;
    deliverables: string[];
    prerequisites: string[];
    validation_commands: string[];
    success_criteria: string[];
    integration_points: string[];
    estimated_duration: string;
    
    technical_specifications: {
      backend_components: string[];
      frontend_components: string[];
      database_changes: string[];
      configuration_requirements: string[];
    };
    
    quality_requirements: {
      test_coverage_target: number;
      performance_benchmarks: string[];
      security_validations: string[];
      code_quality_gates: string[];
    };
  }>;
  
  dependency_matrix: Array<{
    prp_name: string;
    depends_on: string[];
    enables: string[];
    parallel_with: string[];
    blocking_for: string[];
  }>;
  
  validation_framework: {
    checkpoint_validations: string[];
    integration_tests: string[];
    performance_tests: string[];
    security_scans: string[];
  };
}
```

### Foundation Phase PRPs - Detailed Implementation
```bash
# Foundation phase implementation with comprehensive validation
generate_foundation_phase_prps() {
    echo "🏗️ Phase 1: Foundation Infrastructure Implementation"
    
    cat > "phase-1-foundation-comprehensive.md" << 'EOF'
# Phase 1: Foundation Infrastructure (Critical Path - Est: 4-6 days)

## 1.1 Core Entities & Database Setup
```bash
@copilot /generate-prp core core-entities-setup .github/copilot/requirements/schema.sql
```

### Scope & Deliverables
**Purpose**: Create all base entities, DbContext, and entity configurations following schema.sql structure
**Deliverables**:
- BaseEntity.cs with audit fields (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
- All entity classes: User, Role, Permission, TreeNode, Question, Answer, Media, etc.
- IkhtibarDbContext with proper entity configurations
- Entity relationship mappings and constraints
- Database migration scripts and seed data

**Prerequisites**: 
- Database connection string configured
- Dapper packages installed
- Development database accessible

**Validation Commands**:
```bash
# Backend validation
dotnet build
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet test --filter "Category=Entity"

# Database validation
sqlcmd -S localhost -d IkhtibarDb -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES"
```

**Success Criteria**:
- [ ] All entities compile without errors
- [ ] Database migrations execute successfully  
- [ ] Entity relationships properly configured
- [ ] Audit fields functional across all entities
- [ ] Seed data populates correctly
- [ ] Entity tests achieve >90% coverage

**Integration Points**:
- Repository pattern implementation depends on these entities
- Authentication system requires User/Role entities
- All business modules depend on these base entities

---

## 1.2 Authentication & Authorization System
```bash
@copilot /generate-prp authentication authentication-system .github/copilot/requirements/ikhtibar-features.txt
```

### Scope & Deliverables
**Purpose**: Implement comprehensive JWT authentication with OIDC integration and role-based authorization
**Deliverables**:
- JWT token generation and validation service
- OIDC integration for SSO (Microsoft Azure AD)
- AuthController with login, logout, refresh token endpoints
- Role-based authorization policies and middleware
- Login attempt tracking and account lockout
- Password policy enforcement
- Security headers and CORS configuration

**Prerequisites**:
- Core entities implemented (User, Role, Permission)
- Azure AD application registration completed
- JWT configuration settings defined

**Validation Commands**:
```bash
# Authentication validation
curl -X POST http://localhost:5000/api/auth/login -H "Content-Type: application/json" -d '{"email":"admin@example.com","password":"Test123!"}'
curl -X GET http://localhost:5000/api/auth/profile -H "Authorization: Bearer {token}"
curl -X POST http://localhost:5000/api/auth/refresh -H "Content-Type: application/json" -d '{"refreshToken":"{refresh_token}"}'

# Authorization validation
curl -X GET http://localhost:5000/api/admin/users -H "Authorization: Bearer {admin_token}"
curl -X GET http://localhost:5000/api/admin/users -H "Authorization: Bearer {student_token}" # Should return 403
```

**Success Criteria**:
- [ ] JWT tokens generate and validate correctly
- [ ] OIDC SSO flow functional
- [ ] Role-based access control enforced
- [ ] Refresh token rotation working
- [ ] Login attempt tracking operational
- [ ] Security headers properly configured
- [ ] Authentication tests achieve >95% coverage

**Integration Points**:
- All API controllers require authentication middleware
- Frontend authentication context depends on these endpoints
- User management module requires role-based authorization

---

## 1.3 Base Repository Pattern
```bash
@copilot /generate-prp core base-repository-pattern .github/copilot/requirements/schema.sql
```

### Scope & Deliverables
**Purpose**: Create generic repository pattern with comprehensive CRUD operations and audit logging
**Deliverables**:
- IBaseRepository<T> interface with generic CRUD operations
- BaseRepository<T> implementation with Dapper
- Audit logging integration for all data operations
- Soft delete functionality across all entities
- Generic query builder for filtering and sorting
- Repository dependency injection configuration
- Repository unit test base classes

**Prerequisites**:
- Core entities implemented and configured
- Database context operational
- Audit logging infrastructure ready

**Validation Commands**:
```bash
# Repository pattern validation
dotnet test --filter "Category=Repository"
dotnet build

# Integration testing
curl -X GET http://localhost:5000/api/test/repository-health
```

**Success Criteria**:
- [ ] All CRUD operations functional across entities
- [ ] Audit logging captures all data changes
- [ ] Soft delete working for all applicable entities
- [ ] Generic filtering and sorting operational
- [ ] Repository dependency injection configured
- [ ] Repository tests achieve >90% coverage
- [ ] Performance benchmarks meet requirements

**Integration Points**:
- All service layers depend on repository interfaces
- User management module requires User repository
- Question bank module requires Question repository
- Audit logging system integrates with all repositories

---

## 1.4 API Foundation & Middleware
```bash
@copilot /generate-prp core api-foundation .github/copilot/requirements/ikhtibar-features.txt
```

### Scope & Deliverables
**Purpose**: Setup comprehensive API infrastructure with middleware, error handling, and documentation
**Deliverables**:
- BaseController with common functionality
- Global error handling middleware with structured responses
- Request/response logging middleware
- CORS configuration for frontend integration
- API versioning strategy and implementation
- Swagger/OpenAPI documentation setup
- Health check endpoints
- Request validation middleware
- Rate limiting middleware

**Prerequisites**:
- Authentication system implemented
- Core entities and repositories ready
- Logging infrastructure configured

**Validation Commands**:
```bash
# API foundation validation
curl -f http://localhost:5000/api/health
curl -f http://localhost:5000/swagger/index.html
curl -X POST http://localhost:5000/api/test/validation -H "Content-Type: application/json" -d '{}'
curl -X GET http://localhost:5000/api/test/error-handling

# CORS validation
curl -X OPTIONS http://localhost:5000/api/health -H "Origin: http://localhost:3000"
```

**Success Criteria**:
- [ ] Health endpoints responding correctly
- [ ] Swagger documentation accessible and accurate
- [ ] Global error handling catches all exceptions
- [ ] CORS properly configured for frontend
- [ ] Request validation working across endpoints
- [ ] Rate limiting functional
- [ ] Logging captures all API requests
- [ ] API tests achieve >85% coverage

**Integration Points**:
- All API controllers inherit from BaseController
- Frontend HTTP client depends on CORS configuration
- Error handling integrates with frontend error boundaries
- Health checks integrate with monitoring systems

---

## 1.5 Frontend Foundation & Layout
```bash
@copilot /generate-prp frontend frontend-foundation .github/copilot/requirements/ikhtibar-features.txt
```

### Scope & Deliverables
**Purpose**: Create comprehensive React application foundation with routing, layouts, and core infrastructure
**Deliverables**:
- React application structure with TypeScript
- React Router configuration with protected routes
- DashboardLayout and PortalLayout components
- Authentication context and hooks
- HTTP client with interceptors and error handling
- i18n setup with English and Arabic support
- Global state management setup
- Theme configuration and design system foundation
- Error boundary components

**Prerequisites**:
- Backend API foundation operational
- Authentication endpoints available
- CORS configured for frontend access

**Validation Commands**:
```bash
# Frontend foundation validation
npm run type-check
npm run lint
npm run test
npm run build

# Application functionality
npm run dev &
sleep 10
curl -f http://localhost:3000
curl -f http://localhost:3000/login
curl -f http://localhost:3000/dashboard
```

**Success Criteria**:
- [ ] Application builds without errors
- [ ] All routes accessible and functional
- [ ] Authentication flow working end-to-end
- [ ] Layout components responsive
- [ ] i18n system supporting both languages
- [ ] HTTP client handling API integration
- [ ] Error boundaries catching runtime errors
- [ ] Frontend tests achieve >80% coverage

**Integration Points**:
- Authentication integrates with backend auth endpoints
- All feature modules use foundation components
- HTTP client integrates with all API services
- Layout components used by all application pages

## Phase 1 Integration Validation
```bash
# Comprehensive Phase 1 validation
validate_foundation_phase() {
    echo "🧪 Comprehensive Foundation Phase Validation"
    
    # Backend integration validation
    cd backend
    dotnet clean && dotnet build
    dotnet test --logger "console;verbosity=detailed"
    
    # Start backend for integration testing
    dotnet run &
    BACKEND_PID=$!
    sleep 15
    
    # API integration tests
    echo "Testing authentication flow..."
    TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login \
        -H "Content-Type: application/json" \
        -d '{"email":"admin@example.com","password":"Test123!"}' \
        | jq -r '.token')
    
    echo "Testing authenticated endpoints..."
    curl -f -H "Authorization: Bearer $TOKEN" http://localhost:5000/api/health
    curl -f -H "Authorization: Bearer $TOKEN" http://localhost:5000/api/users
    
    # Frontend integration validation
    cd ../frontend
    npm ci
    npm run type-check
    npm run lint
    npm run test -- --coverage --watchAll=false
    npm run build
    
    # Start frontend for integration testing
    npm run preview &
    FRONTEND_PID=$!
    sleep 10
    
    # Frontend functionality tests
    curl -f http://localhost:4173
    
    # Cleanup
    kill $BACKEND_PID $FRONTEND_PID 2>/dev/null || true
    
    echo "✅ Foundation phase validation complete"
}
```

## Phase 1 Success Criteria Summary
- [ ] **Database**: All entities created, migrations successful, relationships configured
- [ ] **Authentication**: JWT auth working, OIDC integration functional, role-based access operational
- [ ] **Repository Pattern**: Generic CRUD working, audit logging functional, soft delete operational
- [ ] **API Foundation**: All middleware operational, error handling comprehensive, documentation available
- [ ] **Frontend Foundation**: React app functional, routing working, authentication integrated
- [ ] **Integration**: End-to-end authentication flow working, API-frontend communication established
- [ ] **Quality**: >85% test coverage, all validation commands passing, performance benchmarks met
- [ ] **Documentation**: All integration points documented, setup instructions complete

## Phase 1 Risk Mitigation
**Database Connection Issues**: 
- Mitigation: Verify connection strings, test database connectivity before implementation
- Contingency: Use SQLite for development if SQL Server unavailable

**Authentication Complexity**: 
- Mitigation: Implement basic JWT first, add OIDC incrementally
- Contingency: Start with cookie-based auth if JWT implementation blocked

**Frontend Build Issues**: 
- Mitigation: Lock Node.js and npm versions, use exact dependency versions
- Contingency: Use Create React App if Vite configuration problematic

**Integration Problems**: 
- Mitigation: Implement health checks and detailed logging throughout
- Contingency: Use mock endpoints for frontend development if backend delayed

EOF
}
```

### User Management Phase Implementation (Phase 2 - Critical Business Module)
```bash
# User management phase with comprehensive business logic
generate_user_management_phase_prps() {
    echo "👥 Phase 2: User Management System Implementation"
    
    cat > "phase-2-user-management-comprehensive.md" << 'EOF'
# Phase 2: User Management System (Critical Business Module - Est: 5-7 days)

## 2.1 User Management Backend Services
```bash
@copilot /generate-prp user-management backend-services .github/copilot/requirements/data.sql
```

### Scope & Deliverables
**Purpose**: Implement comprehensive user management with CRUD operations, role assignments, and business rules
**Deliverables**:
- UserService with all business logic operations
- UserRepository with advanced query capabilities
- RoleService and PermissionService implementations
- UserController with full REST API endpoints
- User DTOs (CreateUserDto, UpdateUserDto, UserResponseDto)
- Role assignment and permission management logic
- User profile management and password change functionality
- User search and filtering capabilities

**Prerequisites**:
- Foundation phase completed (authentication, repository pattern)
- User, Role, Permission entities operational
- Authentication middleware configured

**Validation Commands**:
```bash
# User management API validation
curl -X GET http://localhost:5000/api/users -H "Authorization: Bearer {admin_token}"
curl -X POST http://localhost:5000/api/users -H "Content-Type: application/json" -H "Authorization: Bearer {admin_token}" -d '{"email":"test@example.com","firstName":"Test","lastName":"User","roles":["Student"]}'
curl -X PUT http://localhost:5000/api/users/{id} -H "Content-Type: application/json" -H "Authorization: Bearer {admin_token}" -d '{"firstName":"Updated","lastName":"User"}'
curl -X DELETE http://localhost:5000/api/users/{id} -H "Authorization: Bearer {admin_token}"

# Role management validation
curl -X POST http://localhost:5000/api/users/{id}/roles -H "Content-Type: application/json" -H "Authorization: Bearer {admin_token}" -d '{"roleId":"{role_id}"}'
curl -X GET http://localhost:5000/api/users/{id}/permissions -H "Authorization: Bearer {admin_token}"
```

**Success Criteria**:
- [ ] All user CRUD operations functional
- [ ] Role assignment system operational
- [ ] Permission management working
- [ ] User search and filtering efficient
- [ ] Business rule validation enforced
- [ ] User service tests achieve >90% coverage
- [ ] API endpoints return proper HTTP status codes
- [ ] Data validation prevents invalid user creation

**Integration Points**:
- Integrates with authentication system for login verification
- Required by all other modules for user context
- Audit logging tracks all user management operations
- Notification system depends on user data for communications

---

## 2.2 User Management Frontend Components
```bash
@copilot /generate-prp user-management frontend-components .github/copilot/requirements/ikhtibar-features.txt
```

### Scope & Deliverables
**Purpose**: Create comprehensive user management interface with responsive design and accessibility
**Deliverables**:
- UserList component with pagination and sorting
- UserForm components for create/edit operations
- UserProfile component for self-service profile management
- RoleAssignment component for role management
- UserSearch component with advanced filtering
- PermissionMatrix component for permission visualization
- UserCard component for user display
- User import/export functionality
- Responsive design for mobile and desktop

**Prerequisites**:
- Frontend foundation completed
- User management backend APIs operational
- Design system components available

**Validation Commands**:
```bash
# Frontend component validation
npm run test:user-management
npm run lint:user-management
npm run type-check

# Component integration testing
npm run test:integration:user-management

# Visual regression testing
npm run test:visual:user-management
```

**Success Criteria**:
- [ ] All user management components functional
- [ ] Responsive design working on all devices
- [ ] User creation/editing forms validate properly
- [ ] Role assignment interface intuitive
- [ ] Search and filtering responsive and fast
- [ ] Component tests achieve >85% coverage
- [ ] Accessibility requirements met (WCAG 2.1 AA)
- [ ] Performance benchmarks met for large user lists

**Integration Points**:
- Uses authentication context for user permissions
- Integrates with notification system for user alerts
- Connected to audit logging for user activity tracking
- Uses global state management for user data caching

---

## 2.3 User Authentication Frontend Integration
```bash
@copilot /generate-prp authentication frontend-auth .github/copilot/requirements/ikhtibar-features.txt
```

### Scope & Deliverables
**Purpose**: Complete frontend authentication integration with SSO and user experience optimization
**Deliverables**:
- LoginForm component with validation and error handling
- AuthContext for global authentication state management
- useAuth hook for authentication operations
- ProtectedRoute component for route protection
- LogoutConfirmation component
- SSO integration components
- Password reset and change components
- Remember me functionality
- Multi-factor authentication UI (if required)

**Prerequisites**:
- Authentication backend system operational
- Frontend foundation and routing configured
- User management components ready

**Validation Commands**:
```bash
# Authentication flow validation
npm run test:auth
npm run test:integration:auth

# SSO flow validation (manual testing required)
# 1. Navigate to /login
# 2. Click "Sign in with Microsoft"
# 3. Complete Azure AD authentication
# 4. Verify redirect to dashboard
# 5. Verify user context populated
```

**Success Criteria**:
- [ ] Login/logout flow working smoothly
- [ ] SSO integration functional
- [ ] Protected routes enforcing authentication
- [ ] Password reset flow operational
- [ ] User context maintained across browser sessions
- [ ] Authentication errors handled gracefully
- [ ] Auth components achieve >90% test coverage
- [ ] Performance optimized for authentication checks

**Integration Points**:
- Central authentication state used by all components
- Integrates with user management for profile data
- Connected to API client for token management
- Used by all protected routes and components

## Phase 2 Integration Validation
```bash
# Comprehensive Phase 2 validation
validate_user_management_phase() {
    echo "🧪 User Management Phase Integration Validation"
    
    # Start backend with user management endpoints
    cd backend && dotnet run &
    BACKEND_PID=$!
    sleep 15
    
    # Test complete user management workflow
    echo "Testing user management workflow..."
    
    # Admin login
    ADMIN_TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login \
        -H "Content-Type: application/json" \
        -d '{"email":"admin@example.com","password":"Test123!"}' \
        | jq -r '.token')
    
    # Create new user
    NEW_USER=$(curl -s -X POST http://localhost:5000/api/users \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $ADMIN_TOKEN" \
        -d '{"email":"newuser@example.com","firstName":"New","lastName":"User","roles":["Student"]}' \
        | jq -r '.id')
    
    # Update user
    curl -s -X PUT http://localhost:5000/api/users/$NEW_USER \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $ADMIN_TOKEN" \
        -d '{"firstName":"Updated","lastName":"User"}'
    
    # Test new user login
    USER_TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login \
        -H "Content-Type: application/json" \
        -d '{"email":"newuser@example.com","password":"TempPassword123!"}' \
        | jq -r '.token')
    
    # Test role-based access
    curl -f -H "Authorization: Bearer $USER_TOKEN" http://localhost:5000/api/users/profile
    
    # Frontend integration validation
    cd ../frontend
    npm run dev &
    FRONTEND_PID=$!
    sleep 10
    
    # Test frontend authentication flow
    curl -f http://localhost:3000/login
    curl -f http://localhost:3000/dashboard
    
    # Cleanup
    kill $BACKEND_PID $FRONTEND_PID 2>/dev/null || true
    
    echo "✅ User Management phase validation complete"
}
```

EOF
}
```

### Question Bank Phase Implementation (Phase 4 - Core Business Feature)
```bash
# Question bank implementation with advanced features
generate_question_bank_phase_prps() {
    echo "❓ Phase 4: Question Bank System Implementation"
    
    cat > "phase-4-question-bank-comprehensive.md" << 'EOF'
# Phase 4: Question Bank System (Core Business Feature - Est: 8-12 days)

## 4.1 Question Management Backend Infrastructure
```bash
@copilot /generate-prp question-bank backend-management .github/copilot/requirements/schema.sql
```

### Scope & Deliverables
**Purpose**: Comprehensive question management system with versioning, workflow, and advanced capabilities
**Deliverables**:
- Question entity with polymorphic support for different question types
- Answer entity with flexible structure for various answer formats
- QuestionVersion entity for version control and history tracking
- QuestionService with comprehensive business logic
- QuestionRepository with advanced query capabilities
- QuestionController with full REST API endpoints
- Question workflow management (Draft → Review → Approved → Published)
- Question tagging and categorization system
- Question difficulty assessment and analytics
- Question usage tracking and statistics

**Prerequisites**:
- Foundation phase and user management completed
- Tree structure for question organization implemented
- Media management system operational

**Validation Commands**:
```bash
# Question management validation
curl -X GET http://localhost:5000/api/questions -H "Authorization: Bearer {token}"
curl -X POST http://localhost:5000/api/questions -H "Content-Type: application/json" -H "Authorization: Bearer {token}" -d '{
  "type": "MultipleChoice",
  "content": "What is the capital of France?",
  "answers": [
    {"content": "Paris", "isCorrect": true},
    {"content": "London", "isCorrect": false},
    {"content": "Berlin", "isCorrect": false}
  ],
  "difficulty": "Easy",
  "treeNodeId": "{category_id}"
}'

# Question workflow validation
curl -X PUT http://localhost:5000/api/questions/{id}/status -H "Content-Type: application/json" -H "Authorization: Bearer {token}" -d '{"status": "Review"}'
curl -X GET http://localhost:5000/api/questions/{id}/history -H "Authorization: Bearer {token}"
```

**Success Criteria**:
- [ ] All question types supported (MultipleChoice, TrueFalse, FillInBlank, Essay, Matching)
- [ ] Question versioning system operational
- [ ] Workflow state management functional
- [ ] Question search and filtering efficient
- [ ] Question analytics providing insights
- [ ] Question service tests achieve >90% coverage
- [ ] Performance optimized for large question banks
- [ ] Data integrity maintained across all operations

---

## 4.2 Advanced Question Types Support
```bash
@copilot /generate-prp question-bank question-types .github/copilot/requirements/data.sql
```

### Scope & Deliverables
**Purpose**: Support for all question types with specialized handlers and validation
**Deliverables**:
- MultipleChoiceQuestion with single and multiple correct answers
- TrueFalseQuestion with simple boolean logic
- FillInBlankQuestion with multiple blank support
- EssayQuestion with rubric integration
- MatchingQuestion with flexible matching pairs
- NumericQuestion with range validation
- QuestionTypeHandler factory pattern
- Answer validation for each question type
- Scoring algorithms for automatic grading
- Question rendering templates

**Prerequisites**:
- Question management backend operational
- Media management for question content
- Grading system interfaces defined

**Validation Commands**:
```bash
# Test each question type
curl -X POST http://localhost:5000/api/questions -H "Content-Type: application/json" -H "Authorization: Bearer {token}" -d '{
  "type": "FillInBlank",
  "content": "The capital of ___ is Paris, and it is located in ___.",
  "answers": [
    {"content": "France", "position": 1},
    {"content": "Europe", "position": 2}
  ]
}'

curl -X POST http://localhost:5000/api/questions -H "Content-Type: application/json" -H "Authorization: Bearer {token}" -d '{
  "type": "Matching",
  "content": "Match the countries with their capitals:",
  "answers": [
    {"content": "France", "matchWith": "Paris"},
    {"content": "Germany", "matchWith": "Berlin"}
  ]
}'
```

**Success Criteria**:
- [ ] All question types create and render correctly
- [ ] Answer validation working for each type
- [ ] Scoring algorithms accurate for automatic grading
- [ ] Question type conversion supported
- [ ] Performance optimized for complex question types
- [ ] Question type tests achieve >95% coverage

---

## 4.3 Media Management Integration
```bash
@copilot /generate-prp question-bank media-management .github/copilot/requirements/schema.sql
```

### Scope & Deliverables
**Purpose**: Comprehensive media support for questions with secure storage and processing
**Deliverables**:
- Media entity with support for images, audio, video, documents
- MediaService with upload, processing, and storage capabilities
- Azure Blob Storage integration for secure media storage
- Media validation and virus scanning
- Image resizing and optimization
- Audio/video transcoding capabilities
- Media watermarking for copyright protection
- Media usage tracking and analytics
- CDN integration for performance optimization

**Prerequisites**:
- Azure Blob Storage account configured
- Media processing libraries installed
- Question management system operational

**Validation Commands**:
```bash
# Media upload validation
curl -X POST http://localhost:5000/api/media/upload \
  -H "Authorization: Bearer {token}" \
  -F "file=@test-image.jpg" \
  -F "type=Image" \
  -F "category=Question"

# Media association with questions
curl -X POST http://localhost:5000/api/questions -H "Content-Type: application/json" -H "Authorization: Bearer {token}" -d '{
  "type": "MultipleChoice",
  "content": "What is shown in this image?",
  "mediaIds": ["{media_id}"],
  "answers": [
    {"content": "A cat", "isCorrect": true},
    {"content": "A dog", "isCorrect": false}
  ]
}'
```

**Success Criteria**:
- [ ] All media types supported (images, audio, video, documents)
- [ ] Media upload and storage working securely
- [ ] Media processing and optimization functional
- [ ] Media-question association operational
- [ ] CDN integration improving performance
- [ ] Media management tests achieve >85% coverage
- [ ] Security measures preventing unauthorized access

---

## 4.4 Question Management Frontend Interface
```bash
@copilot /generate-prp question-bank frontend-management .github/copilot/requirements/ikhtibar-features.txt
```

### Scope & Deliverables
**Purpose**: Intuitive question creation and management interface with rich editing capabilities
**Deliverables**:
- QuestionBuilder component with step-by-step wizard
- QuestionEditor with rich text editing capabilities
- AnswerEditor for different answer types
- MediaUpload component with drag-drop functionality
- QuestionPreview component showing final rendering
- QuestionForm with validation and error handling
- QuestionTypeSelector with visual examples
- DifficultySelector with guidance
- TagEditor for question categorization
- BulkQuestionOperations for batch management

**Prerequisites**:
- Question management backend APIs operational
- Media management system functional
- Rich text editor library integrated

**Validation Commands**:
```bash
# Question creation interface validation
npm run test:question-management
npm run test:components:question-builder
npm run test:integration:question-workflow

# Visual testing
npm run test:visual:question-editor
npm run test:accessibility:question-forms
```

**Success Criteria**:
- [ ] Question creation wizard intuitive and efficient
- [ ] Rich text editor supporting formatting and media
- [ ] Answer editing working for all question types
- [ ] Media upload interface user-friendly
- [ ] Question preview accurate and responsive
- [ ] Form validation preventing invalid submissions
- [ ] Component tests achieve >85% coverage
- [ ] Accessibility requirements met for all interfaces

EOF
}
```

## Phase 3: Implementation Orchestration and Quality Assurance

### Comprehensive Quality Framework
```typescript
// Quality assurance and validation framework for Ikhtibar implementation
interface QualityAssuranceFramework {
  quality_gates: Array<{
    gate_name: string;
    phase_applicability: string[];
    quality_criteria: Array<{
      criterion_name: string;
      measurement_method: string;
      target_threshold: string;
      validation_command: string;
      failure_remediation: string[];
    }>;
    
    automated_checks: Array<{
      check_name: string;
      automation_tool: string;
      execution_frequency: 'pre_commit' | 'build' | 'deploy' | 'continuous';
      failure_action: 'block' | 'warn' | 'report';
    }>;
  }>;
  
  testing_strategy: {
    unit_testing: {
      coverage_targets: Record<string, number>;
      testing_frameworks: string[];
      mock_strategies: string[];
      performance_benchmarks: string[];
    };
    
    integration_testing: {
      api_testing_approach: string[];
      database_testing_strategy: string[];
      external_service_mocking: string[];
      contract_testing_requirements: string[];
    };
    
    e2e_testing: {
      critical_user_journeys: string[];
      test_automation_tools: string[];
      browser_compatibility_matrix: string[];
      performance_monitoring: string[];
    };
  };
  
  performance_monitoring: {
    backend_metrics: string[];
    frontend_metrics: string[];
    database_metrics: string[];
    infrastructure_metrics: string[];
  };
}
```

### Continuous Integration and Deployment Framework
Use `create_file` to generate CI/CD automation:

```yaml
# Comprehensive CI/CD pipeline for Ikhtibar implementation
name: Ikhtibar Implementation Pipeline

on:
  push:
    branches: [ main, develop, feature/* ]
  pull_request:
    branches: [ main, develop ]

env:
  DOTNET_VERSION: '8.0.x'
  NODE_VERSION: '20.x'
  AZURE_WEBAPP_NAME: 'ikhtibar-app'
  AZURE_WEBAPP_PACKAGE_PATH: '.'

jobs:
  # Backend Quality Gates
  backend-quality:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore backend/Ikhtibar.sln
    
    - name: Build
      run: dotnet build backend/Ikhtibar.sln --no-restore --configuration Release
    
    - name: Code formatting check
      run: dotnet format backend/Ikhtibar.sln --verify-no-changes --verbosity diagnostic
    
    - name: Unit tests
      run: dotnet test backend/Ikhtibar.sln --no-build --configuration Release --logger trx --collect:"XPlat Code Coverage"
    
    - name: Code coverage report
      uses: codecov/codecov-action@v3
      with:
        files: coverage.cobertura.xml
        flags: backend
    
    - name: Security scan
      run: dotnet list backend/Ikhtibar.sln package --vulnerable --include-transitive
    
    - name: Performance benchmarks
      run: dotnet run --project backend/Ikhtibar.Benchmarks --configuration Release

  # Frontend Quality Gates
  frontend-quality:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: ${{ env.NODE_VERSION }}
        cache: 'npm'
        cache-dependency-path: frontend/package-lock.json
    
    - name: Install dependencies
      run: cd frontend && npm ci
    
    - name: Type checking
      run: cd frontend && npm run type-check
    
    - name: Linting
      run: cd frontend && npm run lint
    
    - name: Unit tests
      run: cd frontend && npm run test -- --coverage --watchAll=false
    
    - name: Build
      run: cd frontend && npm run build
    
    - name: Bundle analysis
      run: cd frontend && npm run analyze
    
    - name: Accessibility testing
      run: cd frontend && npm run test:a11y
    
    - name: Visual regression testing
      run: cd frontend && npm run test:visual

  # Integration Testing
  integration-testing:
    needs: [backend-quality, frontend-quality]
    runs-on: ubuntu-latest
    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server:2022-latest
        env:
          SA_PASSWORD: Test123!
          ACCEPT_EULA: Y
        ports:
          - 1433:1433
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: ${{ env.NODE_VERSION }}
    
    - name: Setup test database
      run: |
        cd backend
        dotnet ef database update --connection "Server=localhost,1433;Database=IkhtibarTest;User Id=sa;Password=Test123!;TrustServerCertificate=true;"
    
    - name: Run integration tests
      run: |
        cd backend
        dotnet test --filter "Category=Integration" --logger trx
    
    - name: Start backend for E2E tests
      run: |
        cd backend
        dotnet run --urls http://localhost:5000 &
        echo $! > backend.pid
    
    - name: Wait for backend
      run: |
        timeout 60 bash -c 'until curl -f http://localhost:5000/api/health; do sleep 2; done'
    
    - name: Run E2E tests
      run: |
        cd frontend
        npm ci
        npm run test:e2e
    
    - name: Cleanup
      run: |
        if [ -f backend.pid ]; then kill $(cat backend.pid); fi

  # Security and Compliance
  security-compliance:
    needs: [integration-testing]
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    
    - name: Run Trivy vulnerability scanner
      uses: aquasecurity/trivy-action@master
      with:
        scan-type: 'fs'
        scan-ref: '.'
        format: 'sarif'
        output: 'trivy-results.sarif'
    
    - name: Upload Trivy scan results
      uses: github/codeql-action/upload-sarif@v2
      with:
        sarif_file: 'trivy-results.sarif'
    
    - name: OWASP ZAP Baseline Scan
      uses: zaproxy/action-baseline@v0.7.0
      with:
        target: 'http://localhost:5000'

  # Deployment
  deploy-staging:
    if: github.ref == 'refs/heads/develop'
    needs: [security-compliance]
    runs-on: ubuntu-latest
    environment: staging
    steps:
    - uses: actions/checkout@v4
    
    - name: Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME }}-staging
        package: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}
    
    - name: Run smoke tests
      run: |
        curl -f https://${{ env.AZURE_WEBAPP_NAME }}-staging.azurewebsites.net/api/health

  deploy-production:
    if: github.ref == 'refs/heads/main'
    needs: [security-compliance]
    runs-on: ubuntu-latest
    environment: production
    steps:
    - uses: actions/checkout@v4
    
    - name: Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME }}
        package: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}
    
    - name: Run smoke tests
      run: |
        curl -f https://${{ env.AZURE_WEBAPP_NAME }}.azurewebsites.net/api/health
    
    - name: Notify deployment success
      uses: 8398a7/action-slack@v3
      with:
        status: success
        webhook_url: ${{ secrets.SLACK_WEBHOOK }}
```

## Phase 4: Success Metrics and Comprehensive Monitoring

### Implementation Success Dashboard
Use `create_file` to generate comprehensive tracking:

```markdown
# Ikhtibar Implementation Success Dashboard

## Executive Summary
**Overall Project Health**: {health_status}
**Completion Percentage**: {completion_percentage}%
**Quality Score**: {quality_score}/100
**Timeline Status**: {timeline_status}
**Risk Level**: {overall_risk_level}

## Phase Completion Matrix

### Phase 1: Foundation Infrastructure
- [x] Core Entities & Database Setup (100%) ✅
- [x] Authentication & Authorization (100%) ✅
- [x] Base Repository Pattern (100%) ✅
- [x] API Foundation & Middleware (100%) ✅
- [x] Frontend Foundation & Layout (100%) ✅
**Phase Status**: Complete | **Quality**: 95/100 | **Risk**: Low

### Phase 2: User Management
- [x] User Management Backend (100%) ✅
- [x] User Management Frontend (95%) 🔄
- [x] Authentication Frontend (100%) ✅
**Phase Status**: 98% Complete | **Quality**: 92/100 | **Risk**: Low

### Phase 3: Tree Structure & Content
- [x] Tree Node Backend (100%) ✅
- [x] Tree Management Frontend (90%) 🔄
- [ ] Curriculum Alignment (60%) 🔄
**Phase Status**: 83% Complete | **Quality**: 88/100 | **Risk**: Medium

### Phase 4: Question Bank
- [x] Question Management Backend (100%) ✅
- [x] Question Types Support (95%) 🔄
- [x] Media Management (90%) 🔄
- [x] Question Frontend Management (85%) 🔄
- [ ] Question Browser Frontend (70%) 🔄
- [ ] Review Workflow (50%) 🔄
**Phase Status**: 82% Complete | **Quality**: 85/100 | **Risk**: Medium

### Phase 5: Exam Management
- [ ] Exam Creation Backend (40%) 🔄
- [ ] Exam Scheduling (30%) 🔄
- [ ] Blueprint System (20%) 🔄
- [ ] Exam Creation Frontend (25%) 🔄
- [ ] Exam Scheduling Frontend (15%) 🔄
**Phase Status**: 26% Complete | **Quality**: Pending | **Risk**: High

## Quality Metrics Dashboard

### Code Quality
- **Backend Test Coverage**: 91.5% (Target: >90%) ✅
- **Frontend Test Coverage**: 87.2% (Target: >85%) ✅
- **Code Duplication**: 3.1% (Target: <5%) ✅
- **Technical Debt Ratio**: 2.8% (Target: <5%) ✅
- **Code Smells**: 23 (Target: <50) ✅
- **Security Vulnerabilities**: 1 (Target: 0) ⚠️

### Performance Metrics
- **API Response Time (95th percentile)**: 245ms (Target: <500ms) ✅
- **Frontend Bundle Size**: 1.2MB (Target: <2MB) ✅
- **Database Query Performance**: 89ms avg (Target: <100ms) ✅
- **Memory Usage**: 512MB (Target: <1GB) ✅
- **CPU Usage**: 12% (Target: <25%) ✅

### Security & Compliance
- **Authentication Security**: ✅ Implemented
- **Authorization Policies**: ✅ Configured
- **Input Validation**: ✅ Comprehensive
- **SQL Injection Prevention**: ✅ Parameterized queries
- **XSS Protection**: ✅ Input sanitization
- **HTTPS Enforcement**: ✅ Configured
- **Security Headers**: ✅ Implemented

## Team Performance Metrics

### Development Velocity
- **PRPs Completed This Week**: 4
- **Average PRP Completion Time**: 1.8 days
- **Stories Delivered**: 12
- **Bugs Found**: 3
- **Bugs Fixed**: 5
- **Team Velocity Trend**: ↗️ Increasing

### Team Coordination
- **Backend Team**: On track, high productivity
- **Frontend Team**: Slight delays in complex components
- **Full-Stack Coordination**: Good, regular sync meetings
- **Blockers**: 2 active, 5 resolved this week

## Risk Assessment

### High Priority Risks
1. **Exam Management Complexity** (Risk: High)
   - Impact: Could delay Phase 5 by 2-3 weeks
   - Mitigation: Additional architect review, prototype first
   - Status: Mitigation in progress

2. **Integration Testing Gaps** (Risk: Medium)
   - Impact: Quality issues in production
   - Mitigation: Dedicated integration testing sprint
   - Status: Planning phase

### Medium Priority Risks
1. **Performance Under Load** (Risk: Medium)
   - Impact: System slowdown with large question banks
   - Mitigation: Performance testing and optimization
   - Status: Monitoring implemented

2. **Team Knowledge Gaps** (Risk: Medium)
   - Impact: Slower development in specialized areas
   - Mitigation: Knowledge sharing sessions, documentation
   - Status: Ongoing training sessions

## Upcoming Milestones

### Next 2 Weeks (Sprint 8)
- [ ] Complete Question Bank Phase (Target: 100%)
- [ ] Begin Exam Management Backend (Target: 60%)
- [ ] Integration testing for completed phases
- [ ] Performance optimization for Question Bank

### Next Month (Sprints 9-10)
- [ ] Complete Exam Management Phase (Target: 90%)
- [ ] Begin Exam Execution Phase (Target: 40%)
- [ ] Security audit and penetration testing
- [ ] User acceptance testing with stakeholders

### Next Quarter (Q2 2024)
- [ ] Complete core functionality (Phases 1-7)
- [ ] Advanced features implementation
- [ ] Production deployment preparation
- [ ] Go-live readiness assessment

## Action Items

### Immediate (Next 24 hours)
1. **Resolve security vulnerability** in user authentication
2. **Complete Question Browser frontend** component
3. **Review Exam Management architecture** with team lead

### This Week
1. **Finalize Question Bank testing** and documentation
2. **Start Exam Management prototype** development
3. **Conduct performance testing** for Question Bank under load
4. **Team retrospective** and process improvements

### This Month
1. **Complete integration testing** for all implemented phases
2. **Implement monitoring and alerting** for production readiness
3. **Conduct security review** with external consultant
4. **Stakeholder demo** of completed functionality

---
*Dashboard automatically updated every 4 hours based on CI/CD pipeline results and team progress tracking*
```

## Success Criteria

The Ikhtibar implementation PRP generation is complete when:
- [ ] All 16 implementation phases have detailed PRP commands
- [ ] Critical path dependencies are clearly defined
- [ ] Parallel execution opportunities are identified
- [ ] Comprehensive quality gates are established
- [ ] Integration validation procedures are documented
- [ ] Risk mitigation strategies are comprehensive
- [ ] Team coordination workflows are optimized
- [ ] Success tracking mechanisms are operational
- [ ] CI/CD pipeline supports all phases
- [ ] Documentation is complete and accessible

## Integration Points

### Enterprise Integration
- Connect with existing enterprise architecture and governance frameworks
- Integrate with organizational project management and tracking systems
- Align with enterprise security policies and compliance requirements
- Ensure compatibility with existing technology stack and infrastructure

### Stakeholder Coordination
- Synchronize with business stakeholders for requirement validation
- Integrate with user acceptance testing and feedback collection processes
- Connect with training and change management initiatives
- Align with business continuity and disaster recovery planning

### Technology Ecosystem Integration
- Leverage existing development tools and automation infrastructure
- Integrate with organizational knowledge management and documentation systems
- Connect to monitoring, logging, and observability platforms
- Align with organizational software supply chain and security practices

### Knowledge Transfer Integration
- Contribute implementation patterns to organizational best practices
- Share PRP methodology with other development teams and projects
- Document lessons learned for future large-scale implementations
- Update organizational development standards based on project experiences

Remember: Execute PRP commands systematically, maintain architectural consistency, and ensure comprehensive quality validation throughout the implementation process.
```

## Notes
- Focus on comprehensive system-wide implementation planning
- Emphasize dependency management and critical path optimization
- Include detailed quality assurance and validation frameworks
- Provide systematic progress tracking and risk mitigation strategies
