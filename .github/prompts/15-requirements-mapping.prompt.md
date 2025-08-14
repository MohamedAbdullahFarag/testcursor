---
mode: agent
description: "Analyze project requirements and create comprehensive mapping to PRP commands for systematic implementation planning"
---

---
inputs:
  - name: requirements_source
    description: Path to requirements documentation or description
    required: true
  - name: analysis_depth
    description: Level of analysis detail (overview, detailed, comprehensive)
    required: false
    default: "comprehensive"
  - name: mapping_scope
    description: Scope of mapping focus (technical, business, integration, all)
    required: false
    default: "all"
---

# requirements-mapping.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Analyze project requirements and create comprehensive mapping to PRP commands for systematic implementation planning
- **Categories**: requirements-analysis, prp-mapping, implementation-planning, traceability-matrix
- **Complexity**: expert
- **Dependencies**: requirements documentation analysis, PRP generation system, architectural planning

## Input
- **requirements_source** (required): Path to requirements documentation or description of requirements to analyze
- **analysis_depth** (optional): Level of analysis detail (overview/detailed/comprehensive)
- **mapping_scope** (optional): Scope of mapping focus (technical/business/integration/all)

## Template

```
You are an expert business analyst and technical architect specializing in requirements analysis and systematic implementation planning using Product Requirements Prompts (PRPs). Your goal is to comprehensively analyze project requirements and create detailed mappings to specific PRP commands that ensure complete requirement coverage and optimal implementation sequencing.

## Input Parameters
- **Requirements Source**: {requirements_source}
- **Analysis Depth**: {analysis_depth} (default: comprehensive)
- **Mapping Scope**: {mapping_scope} (default: all)

## Task Overview
Perform comprehensive requirements analysis of the provided documentation to extract business needs, technical specifications, user roles, workflows, and system constraints, then create detailed mappings to specific PRP commands that ensure complete requirement coverage and systematic implementation.

## Phase 1: Comprehensive Requirements Analysis

### Requirements Documentation Discovery and Analysis
Use `file_search` and `semantic_search` to identify and analyze all requirements sources:

```typescript
// Comprehensive requirements analysis framework
interface RequirementsAnalysisFramework {
  documentation_sources: {
    primary_requirements: Array<{
      document_path: string;
      document_type: 'functional' | 'technical' | 'business' | 'architectural';
      analysis_priority: 'critical' | 'important' | 'supporting';
      content_summary: string;
      key_insights: string[];
      requirement_count: number;
    }>;
    
    supporting_documentation: Array<{
      document_path: string;
      content_type: 'schema' | 'data' | 'configuration' | 'guidelines';
      relevance_score: number;
      integration_points: string[];
      technical_details: string[];
    }>;
    
    architectural_artifacts: Array<{
      artifact_path: string;
      artifact_type: 'diagram' | 'specification' | 'pattern' | 'example';
      architectural_insights: string[];
      implementation_guidance: string[];
    }>;
  };
  
  requirements_extraction: {
    functional_requirements: Array<{
      requirement_id: string;
      requirement_category: string;
      description: string;
      acceptance_criteria: string[];
      business_value: 'high' | 'medium' | 'low';
      implementation_complexity: 'simple' | 'medium' | 'complex';
      dependencies: string[];
      stakeholders: string[];
    }>;
    
    non_functional_requirements: Array<{
      requirement_id: string;
      category: 'performance' | 'security' | 'usability' | 'scalability' | 'reliability';
      description: string;
      measurable_criteria: string[];
      validation_methods: string[];
      implementation_considerations: string[];
    }>;
    
    technical_requirements: Array<{
      requirement_id: string;
      component: string;
      specification: string;
      technology_constraints: string[];
      integration_requirements: string[];
      quality_attributes: string[];
    }>;
  };
  
  business_context: {
    user_roles: Array<{
      role_name: string;
      role_description: string;
      key_responsibilities: string[];
      system_permissions: string[];
      user_journeys: string[];
      business_processes: string[];
    }>;
    
    business_workflows: Array<{
      workflow_name: string;
      workflow_description: string;
      process_steps: Array<{
        step_number: number;
        step_description: string;
        actor: string;
        system_interactions: string[];
        business_rules: string[];
      }>;
      success_criteria: string[];
      exception_handling: string[];
    }>;
    
    business_rules: Array<{
      rule_id: string;
      rule_category: string;
      rule_description: string;
      enforcement_level: 'mandatory' | 'recommended' | 'optional';
      validation_approach: string[];
      implementation_impact: string[];
    }>;
  };
}

// Perform comprehensive requirements analysis
async function analyzeProjectRequirements(requirementsSource: string): Promise<RequirementsAnalysisFramework> {
  // Use file_search to discover all requirements-related files
  // Use semantic_search to understand requirement context and relationships
  // Use grep_search to extract specific requirement patterns and details
  // Analyze and categorize all discovered requirements
}
```

### Requirements Documentation Discovery
Use `file_search` to systematically discover requirements documentation:

```bash
# Comprehensive requirements discovery process
discover_requirements_documentation() {
    echo "📋 Discovering Project Requirements Documentation"
    
    # Primary requirements documentation
    echo "🔍 Searching for primary requirements files..."
    find . -name "*requirements*" -type f | head -20
    find . -name "*features*" -type f | head -10
    find . -name "*specification*" -type f | head -10
    find . -name "*business*" -type f | head -10
    
    # Technical specifications
    echo "🔧 Searching for technical specifications..."
    find . -name "*.sql" -path "*requirements*" -o -name "*.sql" -path "*schema*" | head -10
    find . -name "*.json" -path "*requirements*" -o -name "*.yaml" -path "*requirements*" | head -10
    
    # Architecture and design documentation
    echo "🏗️ Searching for architectural documentation..."
    find . -name "*architecture*" -type f | head -10
    find . -name "*design*" -type f | head -10
    find . -name "*guidelines*" -type f | head -10
    
    # Configuration and examples
    echo "⚙️ Searching for configuration and examples..."
    find . -name "*config*" -type f | head -10
    find . -name "*example*" -type f | head -10
    find . -name "*template*" -type f | head -10
    
    # User and business documentation
    echo "👥 Searching for user and business documentation..."
    find . -name "*user*" -type f | head -10
    find . -name "*workflow*" -type f | head -10
    find . -name "*process*" -type f | head -10
}
```

### Detailed Requirements Extraction and Categorization
Use `read_file` and `grep_search` to extract specific requirements:

```bash
# Systematic requirements extraction process
extract_detailed_requirements() {
    echo "📝 Extracting and Categorizing Requirements"
    
    # Extract functional requirements
    echo "🎯 Extracting functional requirements..."
    grep -r -i "shall\|must\|should\|requirement\|feature" --include="*.md" --include="*.txt" . | head -50
    
    # Extract user roles and permissions
    echo "👤 Extracting user roles and permissions..."
    grep -r -i "role\|permission\|user\|access\|authorization" --include="*.md" --include="*.txt" . | head -30
    
    # Extract business workflows
    echo "🔄 Extracting business workflows..."
    grep -r -i "workflow\|process\|procedure\|step\|journey" --include="*.md" --include="*.txt" . | head -30
    
    # Extract technical constraints
    echo "⚙️ Extracting technical constraints..."
    grep -r -i "technology\|framework\|database\|api\|integration" --include="*.md" --include="*.txt" . | head -30
    
    # Extract quality attributes
    echo "📊 Extracting quality attributes..."
    grep -r -i "performance\|security\|scalability\|usability\|reliability" --include="*.md" --include="*.txt" . | head -30
}
```

## Phase 2: Requirements-to-PRP Mapping Framework

### Systematic Requirements-to-PRP Mapping Strategy
```typescript
// Comprehensive requirements-to-PRP mapping framework
interface RequirementsToPRPMapping {
  mapping_categories: {
    user_management_mapping: Array<{
      requirement_source: string;
      business_need: string;
      technical_specifications: string[];
      mapped_prp_commands: Array<{
        prp_command: string;
        prp_purpose: string;
        requirement_coverage: string[];
        deliverables: string[];
        validation_approach: string[];
      }>;
      coverage_completeness: number;
      implementation_priority: 'critical' | 'important' | 'optional';
    }>;
    
    content_management_mapping: Array<{
      content_requirements: string[];
      hierarchical_needs: string[];
      organization_patterns: string[];
      mapped_prp_commands: Array<{
        prp_command: string;
        scope_description: string;
        architectural_approach: string[];
        business_value: string;
      }>;
      integration_dependencies: string[];
    }>;
    
    business_process_mapping: Array<{
      workflow_name: string;
      process_requirements: string[];
      automation_needs: string[];
      user_experience_requirements: string[];
      mapped_prp_commands: Array<{
        prp_command: string;
        workflow_coverage: string[];
        automation_scope: string[];
        ux_considerations: string[];
      }>;
      success_metrics: string[];
    }>;
    
    technical_integration_mapping: Array<{
      integration_point: string;
      technical_requirements: string[];
      compliance_needs: string[];
      security_considerations: string[];
      mapped_prp_commands: Array<{
        prp_command: string;
        integration_approach: string[];
        security_implementation: string[];
        compliance_coverage: string[];
      }>;
      risk_mitigation: string[];
    }>;
  };
  
  coverage_analysis: {
    requirement_coverage_matrix: Array<{
      requirement_id: string;
      requirement_description: string;
      covering_prps: string[];
      coverage_percentage: number;
      gap_analysis: string[];
      mitigation_strategy: string[];
    }>;
    
    prp_utilization_analysis: Array<{
      prp_command: string;
      requirements_addressed: string[];
      scope_efficiency: number;
      implementation_value: string;
      optimization_opportunities: string[];
    }>;
    
    implementation_sequencing: Array<{
      sequence_number: number;
      phase_name: string;
      prerequisite_requirements: string[];
      prp_commands_in_phase: string[];
      phase_success_criteria: string[];
      risk_factors: string[];
    }>;
  };
}
```

### User Management Requirements Mapping
```bash
# User management requirements analysis and PRP mapping
generate_user_management_mapping() {
    echo "👥 User Management Requirements to PRP Mapping"
    
    cat > "user-management-requirements-mapping.md" << 'EOF'
# User Management Requirements to PRP Mapping

## Requirements Analysis Summary

### Extracted User Management Requirements
1. **User Account Management**
   - Source: Requirements documentation lines 45-67
   - Business Need: "Creates and manages user accounts and permissions"
   - Technical Specifications:
     - User CRUD operations with role-based access control
     - Profile management with audit logging
     - Password policies and account security
     - Group management and permission assignment

2. **Authentication and Authorization**
   - Source: Requirements documentation lines 120-145
   - Business Need: "Secure access control with SSO integration"
   - Technical Specifications:
     - JWT token-based authentication
     - OIDC/SSO integration with Microsoft Azure AD
     - Role-based authorization policies
     - Refresh token management and rotation

3. **User Role Management**
   - Source: Seed data analysis (8 distinct roles identified)
   - Business Need: "Granular permission control for different user types"
   - Technical Specifications:
     - System Administrator, Question Bank Creator, Reviewer roles
     - Exam Manager, Supervisor, Student, Grader roles
     - Quality Reviewer with specialized permissions
     - Permission matrix with resource-based access control

### PRP Command Mapping

#### Core User Management Backend
```bash
@copilot /generate-prp user-management backend-services .github/copilot/requirements/data.sql
```

**Requirement Coverage**:
- ✅ User CRUD operations (Create, Read, Update, Delete users)
- ✅ Role assignment and management system
- ✅ Permission matrix implementation
- ✅ Profile management functionality
- ✅ Audit logging for all user operations
- ✅ User search and filtering capabilities

**Deliverables**:
- UserService with comprehensive business logic
- UserRepository with advanced query capabilities
- RoleService and PermissionService implementations
- UserController with full REST API endpoints
- User DTOs (CreateUserDto, UpdateUserDto, UserResponseDto)
- Audit logging integration for user operations

**Validation Approach**:
```bash
# User management API validation
curl -X GET http://localhost:5000/api/users -H "Authorization: Bearer {admin_token}"
curl -X POST http://localhost:5000/api/users -H "Content-Type: application/json" -d '{user_data}'
curl -X PUT http://localhost:5000/api/users/{id}/roles -H "Content-Type: application/json" -d '{role_assignment}'
```

#### Authentication System Implementation
```bash
@copilot /generate-prp authentication authentication-system .github/copilot/requirements/ikhtibar-features.txt
```

**Requirement Coverage**:
- ✅ JWT token generation and validation
- ✅ OIDC/SSO integration with Azure AD
- ✅ Role-based authorization policies
- ✅ Refresh token rotation and management
- ✅ Login attempt tracking and account lockout
- ✅ Security headers and CORS configuration

**Deliverables**:
- AuthService with JWT implementation
- OIDC integration components
- AuthController with login/logout/refresh endpoints
- Authorization middleware and policies
- Security configuration and headers
- Login attempt tracking system

#### User Management Frontend Components
```bash
@copilot /generate-prp user-management frontend-components .github/copilot/requirements/ikhtibar-features.txt
```

**Requirement Coverage**:
- ✅ User list with pagination and sorting
- ✅ User creation and editing forms
- ✅ Role assignment interface
- ✅ Permission management dashboard
- ✅ User search and filtering interface
- ✅ Profile management for self-service

**Deliverables**:
- UserList component with advanced filtering
- UserForm components for CRUD operations
- RoleAssignment interface with intuitive UX
- UserProfile component for self-management
- PermissionMatrix visualization component
- Responsive design for all screen sizes

#### Authentication Frontend Integration
```bash
@copilot /generate-prp authentication frontend-auth .github/copilot/requirements/ikhtibar-features.txt
```

**Requirement Coverage**:
- ✅ Login/logout user interface
- ✅ SSO integration with Microsoft authentication
- ✅ Protected route implementation
- ✅ Authentication state management
- ✅ Password reset and change functionality
- ✅ Remember me and session management

**Deliverables**:
- LoginForm with validation and error handling
- AuthContext for global state management
- ProtectedRoute component for security
- SSO integration components
- Password management interface
- Authentication error handling

### Requirements Coverage Analysis

#### Coverage Completeness: 95%
- **Fully Covered Requirements**: User CRUD, Authentication, Authorization, Role Management
- **Partially Covered Requirements**: Advanced user analytics (70% coverage)
- **Gap Analysis**: Real-time user session monitoring needs additional PRP
- **Mitigation Strategy**: Add session monitoring PRP in security phase

#### Implementation Priority: Critical (Phase 1)
- **Business Value**: High - Foundation for all other system functionality
- **Technical Dependency**: Required by all other modules
- **Risk Level**: Low - Well-established patterns and technologies
- **Estimated Effort**: 5-7 days with 2-3 developers

### Integration Dependencies
- **Audit Logging System**: Requires audit PRP implementation
- **Notification System**: User events trigger notifications
- **Reporting System**: User activity feeds into analytics
- **All Business Modules**: Depend on user context and permissions

### Success Metrics
- **Functional**: 100% of user management workflows operational
- **Performance**: User operations complete within 200ms (95th percentile)
- **Security**: Zero authentication vulnerabilities in security scan
- **Usability**: User management interface passes WCAG 2.1 AA compliance
- **Test Coverage**: >90% code coverage for user management components

EOF
}
```

### Question Bank Requirements Mapping
```bash
# Question bank requirements analysis and PRP mapping
generate_question_bank_mapping() {
    echo "❓ Question Bank Requirements to PRP Mapping"
    
    cat > "question-bank-requirements-mapping.md" << 'EOF'
# Question Bank Requirements to PRP Mapping

## Requirements Analysis Summary

### Extracted Question Bank Requirements
1. **Question Creation and Management**
   - Source: Requirements documentation lines 89-156
   - Business Need: "Develops structured questions with metadata tagging"
   - Technical Specifications:
     - Support for 11 question types (Multiple Choice, Essay, Coding, etc.)
     - Question versioning and history tracking
     - Workflow management (Draft → Review → Published)
     - Metadata tagging and categorization

2. **Media Integration**
   - Source: Requirements documentation lines 167-189
   - Business Need: "Uploads elements to support diverse question types"
   - Technical Specifications:
     - Image, audio, video, document support
     - Azure Blob Storage integration
     - Media processing and optimization
     - Secure media access and watermarking

3. **Review and Collaboration Workflow**
   - Source: Requirements documentation lines 234-267
   - Business Need: "Collaborates with reviewers for approval"
   - Technical Specifications:
     - Multi-stage review process
     - Reviewer assignment and notifications
     - Quality control metrics
     - Collaborative commenting system

### PRP Command Mapping

#### Question Management Backend Infrastructure
```bash
@copilot /generate-prp question-bank backend-management .github/copilot/requirements/schema.sql
```

**Requirement Coverage**:
- ✅ Question entity with polymorphic type support
- ✅ Answer entity with flexible structure
- ✅ Question versioning and history tracking
- ✅ Workflow state management (Draft/Review/Published)
- ✅ Question search and filtering capabilities
- ✅ Question analytics and usage tracking

**Deliverables**:
- Question and Answer entities with proper relationships
- QuestionService with comprehensive business logic
- QuestionRepository with advanced query capabilities
- Question workflow management system
- Question analytics and reporting services
- API controllers with full REST endpoints

#### Question Types Support System
```bash
@copilot /generate-prp question-bank question-types .github/copilot/requirements/data.sql
```

**Requirement Coverage**:
- ✅ 11 question types from requirements analysis
- ✅ Polymorphic question handling
- ✅ Type-specific answer validation
- ✅ Scoring algorithms for each type
- ✅ Question type conversion capabilities
- ✅ Rendering templates for each type

**Question Types Supported**:
1. MultipleChoice - Standard single/multiple selection
2. TrueFalse - Binary choice questions
3. FillInBlank - Text input with validation
4. ShortAnswer - Open text responses
5. Essay - Long-form text with rubrics
6. Matching - Connect related items
7. Ordering - Sequence arrangement
8. Coding - Programming challenges
9. MathEquation - Mathematical expressions
10. Numeric - Number input with ranges
11. Hotspot - Image-based selection

#### Media Management Integration
```bash
@copilot /generate-prp question-bank media-management .github/copilot/requirements/schema.sql
```

**Requirement Coverage**:
- ✅ Media entity with comprehensive metadata
- ✅ Azure Blob Storage integration
- ✅ Media processing and optimization
- ✅ Virus scanning and security validation
- ✅ CDN integration for performance
- ✅ Media watermarking for copyright

**Media Types Supported**:
- Images (JPEG, PNG, GIF, SVG)
- Audio (MP3, WAV, OGG)
- Video (MP4, WebM, MOV)
- Documents (PDF, DOC, PPT)

#### Question Management Frontend Interface
```bash
@copilot /generate-prp question-bank frontend-management .github/copilot/requirements/ikhtibar-features.txt
```

**Requirement Coverage**:
- ✅ Question builder with step-by-step wizard
- ✅ Rich text editor with media integration
- ✅ Type-specific answer editors
- ✅ Question preview and validation
- ✅ Metadata tagging interface
- ✅ Bulk operations for question management

#### Question Bank Browser and Search
```bash
@copilot /generate-prp question-bank frontend-browser .github/copilot/requirements/ikhtibar-features.txt
```

**Requirement Coverage**:
- ✅ Advanced search with multiple filters
- ✅ Hierarchical category navigation
- ✅ Question grid with sorting and pagination
- ✅ Quick preview and detailed view
- ✅ Bulk selection and operations
- ✅ Export functionality for questions

#### Review Workflow System
```bash
@copilot /generate-prp question-bank review-workflow .github/copilot/requirements/ikhtibar-features.txt
```

**Requirement Coverage**:
- ✅ Multi-stage review process implementation
- ✅ Reviewer assignment and notifications
- ✅ Review comments and feedback system
- ✅ Quality metrics and scoring
- ✅ Review history and audit trail
- ✅ Collaborative review interface

### Requirements Coverage Analysis

#### Coverage Completeness: 98%
- **Fully Covered Requirements**: Question CRUD, Media Integration, Review Workflow
- **Partially Covered Requirements**: Advanced AI-powered question analysis (planned)
- **Gap Analysis**: Minimal gaps, mainly in advanced analytics features
- **Mitigation Strategy**: Advanced features can be added in enhancement phase

#### Implementation Priority: High (Phase 2-3)
- **Business Value**: High - Core content management functionality
- **Technical Dependency**: Requires user management and tree structure
- **Risk Level**: Medium - Complex business logic and media handling
- **Estimated Effort**: 10-14 days with 3-4 developers

### Question Bank Integration Points
- **Tree Structure**: Questions organized in hierarchical categories
- **User Management**: Creator and reviewer role permissions
- **Exam Management**: Questions selected for exam creation
- **Media System**: Rich content integration throughout
- **Audit System**: All question operations logged
- **Notification System**: Review workflow triggers notifications

EOF
}
```

### Exam Management Requirements Mapping
```bash
# Exam management requirements analysis and PRP mapping
generate_exam_management_mapping() {
    echo "📝 Exam Management Requirements to PRP Mapping"
    
    cat > "exam-management-requirements-mapping.md" << 'EOF'
# Exam Management Requirements to PRP Mapping

## Requirements Analysis Summary

### Extracted Exam Management Requirements
1. **Exam Configuration and Templates**
   - Source: Requirements documentation lines 278-321
   - Business Need: "Configures exam templates with settings such as time limits, scores, and randomization"
   - Technical Specifications:
     - Exam blueprint creation and management
     - Question selection algorithms
     - Time limits and scoring configuration
     - Randomization and security settings

2. **Exam Scheduling and Assignment**
   - Source: Requirements documentation lines 334-367
   - Business Need: "Schedules exams and assigns them to groups"
   - Technical Specifications:
     - Calendar-based scheduling system
     - Student group assignment
     - Availability management
     - Conflict detection and resolution

3. **Live Exam Session Management**
   - Source: Requirements documentation lines 389-425
   - Business Need: "Manages live exam sessions with monitoring"
   - Technical Specifications:
     - Real-time session tracking
     - Proctoring and supervision tools
     - Session control and termination
     - Anti-cheating measures

### PRP Command Mapping

#### Exam Creation Backend Infrastructure
```bash
@copilot /generate-prp exam-management backend-creation .github/copilot/requirements/schema.sql
```

**Requirement Coverage**:
- ✅ Exam entity with comprehensive configuration
- ✅ ExamQuestion relationship management
- ✅ Exam template system
- ✅ Question selection algorithms
- ✅ Scoring and grading configuration
- ✅ Exam versioning and history

**Deliverables**:
- Exam and ExamQuestion entities
- ExamService with creation logic
- ExamRepository with complex queries
- Template management system
- Question selection algorithms
- Exam configuration validation

#### Exam Blueprint and Question Selection
```bash
@copilot /generate-prp exam-management blueprint-system .github/copilot/requirements/ikhtibar-features.txt
```

**Requirement Coverage**:
- ✅ Exam blueprint creation
- ✅ Section and category management
- ✅ Learning outcome mapping
- ✅ Difficulty distribution algorithms
- ✅ Question pool management
- ✅ Automated question selection

**Blueprint Features**:
- Section-based organization
- Question type distribution
- Difficulty level balancing
- Learning outcome coverage
- Time allocation per section
- Scoring weight configuration

#### Exam Scheduling System
```bash
@copilot /generate-prp exam-management scheduling-system .github/copilot/requirements/ikhtibar-features.txt
```

**Requirement Coverage**:
- ✅ Calendar-based scheduling interface
- ✅ Student group assignment
- ✅ Resource availability checking
- ✅ Conflict detection and resolution
- ✅ Notification triggers for scheduling
- ✅ Recurring exam support

#### Exam Creation Frontend Interface
```bash
@copilot /generate-prp exam-management frontend-creation .github/copilot/requirements/ikhtibar-features.txt
```

**Requirement Coverage**:
- ✅ Exam creation wizard
- ✅ Blueprint designer interface
- ✅ Question selection tools
- ✅ Configuration panels
- ✅ Preview and validation
- ✅ Template management interface

#### Exam Scheduling Frontend
```bash
@copilot /generate-prp exam-management frontend-scheduling .github/copilot/requirements/ikhtibar-features.txt
```

**Requirement Coverage**:
- ✅ Calendar-based scheduling UI
- ✅ Student assignment interface
- ✅ Availability visualization
- ✅ Conflict resolution tools
- ✅ Batch scheduling capabilities
- ✅ Scheduling dashboard

#### Exam Session Management
```bash
@copilot /generate-prp exam-execution session-management .github/copilot/requirements/ikhtibar-features.txt
```

**Requirement Coverage**:
- ✅ Real-time session tracking
- ✅ Session state management
- ✅ Monitoring dashboard
- ✅ Session control tools
- ✅ Emergency termination
- ✅ Session analytics

### Requirements Coverage Analysis

#### Coverage Completeness: 96%
- **Fully Covered Requirements**: Exam Creation, Scheduling, Session Management
- **Partially Covered Requirements**: Advanced AI proctoring (future enhancement)
- **Gap Analysis**: Some advanced proctoring features planned for later phases
- **Mitigation Strategy**: Core proctoring sufficient for initial release

#### Implementation Priority: High (Phase 4-5)
- **Business Value**: High - Core examination functionality
- **Technical Dependency**: Requires question bank and user management
- **Risk Level**: High - Complex business logic and real-time requirements
- **Estimated Effort**: 12-16 days with 4-5 developers

### Exam Management Integration Points
- **Question Bank**: Questions selected and configured for exams
- **User Management**: Students, supervisors, and administrators
- **Grading System**: Exam results feed into grading workflows
- **Notification System**: Scheduling and session events trigger notifications
- **Reporting System**: Exam performance data for analytics
- **Security System**: Proctoring and anti-cheating measures

EOF
}
```

## Phase 3: Implementation Sequencing and Traceability

### Requirements Traceability Matrix Generation
Use `create_file` to generate comprehensive traceability documentation:

```markdown
# Requirements Traceability Matrix

## Traceability Overview
**Total Requirements Identified**: {total_requirements_count}
**Total PRP Commands Generated**: {total_prp_commands}
**Coverage Percentage**: {coverage_percentage}%
**Gap Analysis**: {gap_count} gaps identified

## Functional Requirements Traceability

### User Management Domain
| Requirement ID | Description | Source | PRP Command | Coverage | Status |
|---|---|---|---|---|---|
| REQ-UM-001 | User account creation and management | Features.txt:45-67 | user-management backend-services | 100% | ✅ Mapped |
| REQ-UM-002 | Role-based access control | Features.txt:120-145 | authentication authentication-system | 100% | ✅ Mapped |
| REQ-UM-003 | User profile management | Features.txt:156-178 | user-management frontend-components | 100% | ✅ Mapped |
| REQ-UM-004 | SSO integration | Features.txt:234-267 | authentication frontend-auth | 100% | ✅ Mapped |

### Question Bank Domain
| Requirement ID | Description | Source | PRP Command | Coverage | Status |
|---|---|---|---|---|---|
| REQ-QB-001 | Question creation and editing | Features.txt:89-156 | question-bank backend-management | 100% | ✅ Mapped |
| REQ-QB-002 | Multiple question type support | Data.sql:45-89 | question-bank question-types | 100% | ✅ Mapped |
| REQ-QB-003 | Media integration | Schema.sql:234-267 | question-bank media-management | 100% | ✅ Mapped |
| REQ-QB-004 | Review workflow | Features.txt:345-389 | question-bank review-workflow | 100% | ✅ Mapped |

### Exam Management Domain
| Requirement ID | Description | Source | PRP Command | Coverage | Status |
|---|---|---|---|---|---|
| REQ-EM-001 | Exam creation and configuration | Features.txt:278-321 | exam-management backend-creation | 100% | ✅ Mapped |
| REQ-EM-002 | Exam scheduling | Features.txt:334-367 | exam-management scheduling-system | 100% | ✅ Mapped |
| REQ-EM-003 | Live session management | Features.txt:389-425 | exam-execution session-management | 100% | ✅ Mapped |

## Non-Functional Requirements Traceability

### Performance Requirements
| Requirement ID | Description | Target Metric | PRP Commands | Validation Method |
|---|---|---|---|---|
| REQ-PERF-001 | API response time | <500ms (95th percentile) | All backend PRPs | Performance testing |
| REQ-PERF-002 | Concurrent user support | 1000+ simultaneous users | Session management PRPs | Load testing |
| REQ-PERF-003 | Database query optimization | <100ms average | Repository pattern PRPs | Query analysis |

### Security Requirements  
| Requirement ID | Description | Implementation | PRP Commands | Validation Method |
|---|---|---|---|---|
| REQ-SEC-001 | Authentication security | JWT + OIDC | Authentication PRPs | Security audit |
| REQ-SEC-002 | Authorization enforcement | RBAC | User management PRPs | Penetration testing |
| REQ-SEC-003 | Data encryption | TLS + field encryption | Security hardening PRPs | Compliance scan |

### Usability Requirements
| Requirement ID | Description | Standard | PRP Commands | Validation Method |
|---|---|---|---|---|
| REQ-UX-001 | Accessibility compliance | WCAG 2.1 AA | All frontend PRPs | Accessibility testing |
| REQ-UX-002 | Responsive design | Mobile + desktop | Frontend foundation PRPs | Cross-device testing |
| REQ-UX-003 | Internationalization | Arabic + English | I18n PRPs | Localization testing |

## Implementation Phase Mapping

### Phase 1: Foundation (Weeks 1-2)
**Critical Path Requirements**: REQ-UM-001, REQ-UM-002, REQ-AUTH-001
**PRP Commands**: 
- core core-entities-setup
- authentication authentication-system  
- user-management backend-services
- frontend frontend-foundation

**Success Criteria**:
- All foundation requirements fully implemented
- Authentication and user management operational
- Development environment ready for business modules

### Phase 2: Content Management (Weeks 3-4)
**Dependent Requirements**: REQ-QB-001, REQ-QB-002, REQ-TREE-001
**PRP Commands**:
- tree-structure backend-hierarchy
- question-bank backend-management
- question-bank question-types
- question-bank media-management

**Success Criteria**:
- Question creation and management functional
- Media integration operational
- Content organization through tree structure

### Phase 3: Exam System (Weeks 5-7)
**Dependent Requirements**: REQ-EM-001, REQ-EM-002, REQ-EM-003
**PRP Commands**:
- exam-management backend-creation
- exam-management scheduling-system
- exam-execution session-management
- exam-execution student-interface

**Success Criteria**:
- Exam creation and scheduling operational
- Live exam sessions manageable
- Student exam-taking experience functional

### Phase 4: Assessment (Weeks 8-9)
**Dependent Requirements**: REQ-GRAD-001, REQ-GRAD-002, REQ-REP-001
**PRP Commands**:
- grading auto-grading-system
- grading manual-grading-system
- reporting infrastructure-setup
- reporting frontend-dashboard

**Success Criteria**:
- Automated and manual grading systems operational
- Comprehensive reporting and analytics available
- Complete exam lifecycle functional

## Gap Analysis and Mitigation

### Identified Gaps
1. **Advanced AI Proctoring** (REQ-PROC-ADV)
   - Current Coverage: 60%
   - Mitigation: Phase 2 enhancement with AI services
   - Timeline: Q3 2024 implementation

2. **Real-time Collaboration** (REQ-COLLAB-RT)
   - Current Coverage: 40% 
   - Mitigation: WebSocket integration in collaboration PRPs
   - Timeline: Q4 2024 implementation

3. **Advanced Analytics** (REQ-ANALYTICS-ADV)
   - Current Coverage: 70%
   - Mitigation: Machine learning integration in analytics PRPs
   - Timeline: Q1 2025 implementation

### Risk Mitigation Strategies
1. **Dependency Risks**: Strict phase sequencing with validation gates
2. **Scope Creep**: Clear requirement boundaries with change control
3. **Technical Risks**: Prototype complex features before full implementation
4. **Integration Risks**: Comprehensive integration testing between phases

## Continuous Traceability Maintenance

### Traceability Updates
- **Weekly Reviews**: Requirement coverage assessment
- **Sprint Retrospectives**: Gap analysis and mitigation updates  
- **Phase Completions**: Comprehensive traceability validation
- **Stakeholder Reviews**: Business requirement alignment verification

### Tools and Automation
- **Automated Coverage Reports**: CI/CD integration for traceability metrics
- **Requirement Impact Analysis**: Change impact assessment automation
- **Compliance Monitoring**: Continuous compliance tracking and reporting
- **Quality Gates**: Automated requirement coverage validation

---
*Traceability matrix maintained and updated continuously throughout implementation*
```

## Success Criteria

The requirements mapping and PRP generation is complete when:
- [ ] All requirements documentation is comprehensively analyzed
- [ ] Every functional requirement is mapped to specific PRP commands
- [ ] Non-functional requirements have implementation strategies
- [ ] Requirements traceability matrix is complete and accurate
- [ ] Implementation sequencing respects all dependencies
- [ ] Gap analysis identifies and addresses coverage issues
- [ ] Risk mitigation strategies are comprehensive
- [ ] Stakeholder requirements are fully represented
- [ ] Quality criteria and validation methods are defined
- [ ] Continuous traceability processes are established

## Integration Points

### Business Process Integration
- Connect requirements analysis with business process documentation
- Integrate with stakeholder feedback and approval workflows
- Align with organizational change management processes
- Ensure compatibility with business continuity requirements

### Development Workflow Integration
- Synchronize with development planning and sprint management
- Integrate with code review and quality assurance processes
- Connect to testing and validation framework requirements
- Align with deployment and release management procedures

### Compliance and Governance Integration
- Leverage organizational compliance frameworks and standards
- Integrate with audit and quality assurance requirements
- Connect to risk management and mitigation procedures
- Align with regulatory and industry standard compliance needs

### Knowledge Management Integration
- Contribute requirements patterns to organizational knowledge base
- Share analysis methodologies with other teams and projects
- Document lessons learned for future requirements analysis
- Update organizational requirements management standards

Remember: Maintain comprehensive traceability throughout the implementation process to ensure all stakeholder needs are met and business value is delivered.
```

## Notes
- Focus on comprehensive requirements coverage and traceability
- Emphasize systematic mapping from business needs to implementation
- Include detailed gap analysis and mitigation strategies
- Provide continuous traceability and validation approaches
