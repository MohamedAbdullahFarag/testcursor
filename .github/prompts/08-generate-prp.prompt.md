---
mode: agent
description: "Generate new Product Requirements Prompts using templates and best practices"
---

---
   inputs:
     - name: functionName
       description: Name of the function under test
     - name: module-name
       description: Optional module name to organize PRPs (e.g., "user-management", "product-catalog")
     - name: feature-name
       description: The name of the feature to generate a PRP for (e.g., "user-management-permissions","user-management-CRUD")
     - name: initial-requirements-file
       description: Optional path to an existing requirements file to analyze
     - name: module-order-2digits
       description: The order of the module in the overall system (prp folders inside "PRPs" folder +)
     - name: feature-order-2digits
       description: The order of the feature according to dependency in the overall system (prp files inside "PRPs" folder +1)
---

---
command: "/generate-prp"
---
# Generate PRP Command for GitHub Copilot

## Command Usage
```
@copilot /generate-prp [module-name] [feature-name] [initial-requirements-file]
```

## Purpose
This command researches the codebase, analyzes patterns, and creates a comprehensive Product Requirements Prompt (PRP) for implementing a new feature using a structured multi-phase approach.

**Output Location**: All generated PRP files are created in the `.github/copilot/PRPs/{module-order-2digits}-{module-name}/` folder with the naming convention `{feature-order-2digits}-{feature-name}-prp.md`. where {module-name} is the name of the module (e.g., "authentication", "core") and {feature-name} is the name of the feature (e.g., "core-entities-setup", "frontend-auth").


## How /generate-prp Works

### Phase 1: Research Phase
```markdown
I need to generate a comprehensive PRP for implementing [FEATURE_NAME]. Let me start with thorough research of your codebase.

**Research Checklist:**
- [ ] **Pattern Analysis**: Search for similar features in the codebase
- [ ] **Architecture Review**: Analyze folder structure and naming conventions
- [ ] **Technology Stack**: Identify current libraries, frameworks, and versions
- [ ] **API Patterns**: Examine existing API endpoints and response formats
- [ ] **Database Models**: Review existing entity structures and relationships
- [ ] **Frontend Components**: Analyze React component patterns and hooks
- [ ] **Testing Patterns**: Review test structures and coverage approaches
- [ ] **Configuration**: Check environment variables and configuration patterns
- [ ] **Error Handling**: Identify error handling patterns and conventions
- [ ] **Security Patterns**: Review authentication and authorization approaches
```

### Phase 2: Documentation Gathering
```markdown
**Documentation Collection:**
- [ ] **API Documentation**: Relevant external API docs (ASP.NET Core, Dapper, etc.)
- [ ] **Frontend Documentation**: React, TypeScript, Tailwind CSS, i18next
- [ ] **Project Guidelines**: Internal guidelines and best practices
- [ ] **Library Quirks**: Known issues and gotchas with current libraries
- [ ] **Integration Points**: Database, authentication, routing, navigation
- [ ] **Performance Considerations**: Caching, optimization, lazy loading
- [ ] **Security Requirements**: Input validation, XSS prevention, SQL injection
- [ ] **Internationalization**: Translation keys and RTL support
```

### Phase 3: Blueprint Creation
```markdown
**Implementation Blueprint:**
- [ ] **Data Models**: Entity and DTO structures based on existing patterns
- [ ] **Task Breakdown**: Step-by-step implementation plan with file-by-file details
- [ ] **Validation Gates**: Test requirements and quality checkpoints
- [ ] **Integration Points**: Configuration, routing, navigation updates
- [ ] **Test Strategy**: Unit tests, integration tests, component tests
- [ ] **Anti-Patterns**: Common mistakes to avoid based on codebase analysis
- [ ] **Performance Optimizations**: Caching, memoization, lazy loading
- [ ] **Security Validations**: Input validation, authorization checks
```

### Phase 3.5: SRP Validation
```markdown
**SRP Compliance Check:**
- [ ] **Repository Layer**: One entity per repository, data access only
- [ ] **Service Layer**: Single business domain per service
- [ ] **Controller Layer**: HTTP concerns only, thin controllers
- [ ] **Component Layer**: Single UI responsibility
- [ ] **Hook Layer**: Single state management concern
- [ ] **Model Layer**: Pure data representation

**Layer Responsibility Matrix:**
| Layer | Single Responsibility | What NOT to Include |
|-------|---------------------|-------------------|
| Controllers | HTTP request/response handling | Business logic, data access |
| Services | Business domain logic | Data access, HTTP concerns |
| Repositories | Data access operations | Business logic, validation |
| Models | Data structure | Business behavior |
| Components | UI representation | Business logic |
| Hooks | State management | UI rendering logic |
```

### Phase 3.6: Internationalization Requirements
```markdown
**i18n Implementation Plan:**
- [ ] **Translation Keys**: Both English and Arabic translations
- [ ] **RTL Support**: Right-to-left layout considerations for Arabic
- [ ] **Date/Time**: Proper localization formats
- [ ] **Number Formatting**: Culture-specific number formats
- [ ] **Currency Display**: Localized currency presentation
- [ ] **Text Direction**: Component adaptability for RTL/LTR
- [ ] **Resource Files**: Backend localization structure
- [ ] **Translation Workflow**: Process for managing translations

**Key i18n Integration Points:**
- Frontend translation files (en.ts, ar.ts)
- Backend resource files
- Component RTL styling
- Form validation messages
- Error messages
- Navigation elements
- Data presentation components
```

### Phase 3.7: Performance Optimization Plan
```markdown
**Performance Checklist:**
- [ ] **Backend Optimizations**
  - [ ] Async/await for I/O operations
  - [ ] Database query optimization
  - [ ] Proper indexing strategy
  - [ ] Caching implementation
  - [ ] Response compression
  - [ ] Memory management
  - [ ] Connection pooling

- [ ] **Frontend Optimizations**
  - [ ] React component memoization
  - [ ] Code splitting plan
  - [ ] Bundle size optimization
  - [ ] Image optimization strategy
  - [ ] State management efficiency
  - [ ] Lazy loading implementation
  - [ ] Service worker strategy

- [ ] **API Performance**
  - [ ] Pagination implementation
  - [ ] Response data optimization
  - [ ] Request batching strategy
  - [ ] API versioning plan
  - [ ] Cache headers strategy
```

### Phase 3.8: Implementation Order
```markdown
**Implementation Sequence:**
1. **Data Layer**
   - Database schema updates
   - Entity definitions
   - DTOs and mappings
   - Repository interfaces and implementations

2. **Business Layer**
   - Service interfaces
   - Service implementations
   - Business validations
   - Event handlers

3. **API Layer**
   - Controller endpoints
   - Request/response models
   - Middleware updates
   - API documentation

4. **Frontend Foundation**
   - TypeScript interfaces
   - API services
   - State management
   - Routing updates

5. **UI Components**
   - Core components
   - Feature components
   - Form implementations
   - RTL styling

6. **Cross-Cutting**
   - Translations (en/ar)
   - Error handling
   - Logging
   - Security

7. **Testing**
   - Unit tests
   - Integration tests
   - Component tests
   - E2E tests

8. **Documentation**
   - API documentation
   - Component documentation
   - Usage examples
   - Integration guide
```

### Phase 3.9: Validation Commands
```markdown
**Backend Validation:**
```powershell
# Build validation
dotnet build --configuration Release

# Code format check
dotnet format --verify-no-changes

# Run all tests
dotnet test --logger "console;verbosity=detailed"

# Run specific test category
dotnet test --filter "Category=Integration"
```

**Frontend Validation:**
```bash
# Type checking
npm run type-check

# Linting
npm run lint

# Unit tests
npm run test

# Component tests
npm run test:components

# E2E tests
npm run test:e2e

# Build validation
npm run build
```

**Database Validation:**
```powershell
# Reset to clean state
./reset-database.ps1

# Run migrations
dotnet ef database update

# Verify schema
dotnet ef dbcontext info
```

**API Validation:**
```bash
# Health check
curl http://localhost:5000/api/health

# Authentication test
curl -X POST http://localhost:5000/api/auth/login -d '{"username":"test","password":"test"}'

# Feature endpoint test
curl http://localhost:5000/api/[feature-name] -H "Authorization: Bearer [token]"
```
```

### Phase 4: Quality Check
```markdown
**Quality Assurance (Confidence Scoring 1-10):**
- [ ] **Context Completeness** (Score: X/10): All necessary context included
- [ ] **Pattern Consistency** (Score: X/10): Follows established codebase patterns
- [ ] **Documentation Quality** (Score: X/10): Comprehensive and accurate documentation
- [ ] **Implementation Detail** (Score: X/10): Sufficient detail for one-pass implementation
- [ ] **Validation Coverage** (Score: X/10): Complete validation and testing strategy
- [ ] **Integration Coverage** (Score: X/10): All integration points identified
- [ ] **Security Coverage** (Score: X/10): Security requirements addressed
- [ ] **Performance Coverage** (Score: X/10): Performance considerations included

**Overall Confidence Score: X/10**
*(Minimum 8/10 required for PRP to be considered complete)*
```

## Command Implementation Template

### Step 1: Codebase Research
```markdown
I'm generating a comprehensive PRP for [FEATURE_NAME]. Let me start with thorough research of your codebase using semantic search and file analysis.

**Phase 1.1: Semantic Codebase Analysis**
```
I'll use semantic_search to find similar features and patterns:
- @semantic_search: "Similar [feature-type] implementations"
- @semantic_search: "Controller patterns for [domain]" 
- @semantic_search: "Service layer implementations"
- @semantic_search: "Repository patterns and data access"
- @semantic_search: "React components for [feature-type]"
- @semantic_search: "Custom hooks and state management"
```

**Phase 1.2: File Pattern Analysis**
```
I'll use file_search and read_file to understand structure:
- @file_search: "**/*Controller.cs" (API patterns)
- @file_search: "**/*Service.cs" (Business logic patterns)  
- @file_search: "**/*Repository.cs" (Data access patterns)
- @file_search: "src/modules/**/*.tsx" (Component patterns)
- @file_search: "src/modules/**/hooks/*.ts" (Hook patterns)
- @read_file: Key example files for pattern analysis
```

**Phase 1.3: Architecture Documentation Review**
```
I'll analyze project guidelines and conventions:
- @read_file: ".github/copilot-instructions.md"
- @read_file: ".github/copilot/backend-guidelines.md" 
- @read_file: ".github/copilot/frontend-guidelines.md"
- @read_file: ".github/copilot/examples/README.md"
- @read_file: "COPILOT.md"
```

**Phase 1.4: Technology Stack Discovery**
```
I'll identify current tech stack and versions:
- @read_file: "package.json" (Frontend dependencies)
- @read_file: "backend/Ikhtibar.API/Ikhtibar.API.csproj" (Backend dependencies)
- @read_file: ".github/copilot/requirements/schema.sql" (Database schema)
- @read_file: ".github/copilot/requirements/data.sql" (Sample data)
- @grep_search: "using.*;" (Backend namespaces and patterns)
- @grep_search: "import.*from" (Frontend import patterns)
```

**Research Output Template:**
- **Similar Features Found**: [List with file references]
- **Naming Conventions**: [Identified patterns]
- **Folder Structure**: [Current organization]
- **API Patterns**: [Endpoint design, response formats]
- **Database Relationships**: [Entity connections]
- **Component Patterns**: [React component structure]
- **State Management**: [Hook patterns, context usage]
- **Testing Approaches**: [Test file locations and patterns]
- **Technology Versions**: [Current framework versions]
- **Build Patterns**: [Scripts and deployment setup]
```

### Step 2: Documentation Collection and Analysis
```markdown
**Phase 2.1: Internal Documentation Analysis**
```
I'll gather project-specific documentation:
- @read_file: ".github/copilot/api-guidelines.md"
- @read_file: ".github/copilot/PRPs/templates/prp_base.md"
- @read_file: ".github/copilot/commands/README.md"
- @read_file: "README.md"
- @file_search: "**/*.md" (All documentation files)
```

**Phase 2.2: Example Pattern Collection**
```
I'll analyze existing code examples:
- @read_file: ".github/copilot/examples/backend/controllers/[SimilarController].cs"
- @read_file: ".github/copilot/examples/backend/services/[SimilarService].cs"
- @read_file: ".github/copilot/examples/frontend/components/[SimilarComponent].tsx"
- @read_file: ".github/copilot/examples/frontend/hooks/[SimilarHook].ts"
- @list_dir: ".github/copilot/examples/" (Available examples)
```

**Phase 2.3: Configuration Pattern Analysis**
```
I'll examine configuration and setup patterns:
- @read_file: "backend/Program.cs" (DI registration patterns)
- @read_file: "backend/appsettings.json" (Configuration structure)
- @read_file: "frontend/src/main.tsx" (App initialization)
- @read_file: "frontend/vite.config.ts" (Build configuration)
- @grep_search: "services\.Add" (Service registration patterns)
```

**Phase 2.4: Testing Pattern Discovery**
```
I'll identify testing approaches and structures:
- @file_search: "**/*Test*.cs" (Backend test files)
- @file_search: "**/*.test.ts*" (Frontend test files)
- @read_file: Representative test files for patterns
- @grep_search: "\[Test\]|\[Fact\]" (Test method patterns)
```

**Phase 2.5: Known Issues and Gotchas**
```
I'll check for documented issues and workarounds:
- @grep_search: "TODO|FIXME|HACK|BUG" (Code comments about issues)
- @grep_search: "\/\/ NOTE|\/\/ IMPORTANT|\/\/ WARNING" (Important notes)
- @semantic_search: "Known issues" OR "gotchas" OR "workarounds"
```

**Documentation Analysis Output:**
- **Internal Guidelines**: [Summarized key points]
- **Code Examples**: [Relevant patterns with file references]
- **Configuration Patterns**: [DI, routing, build setup]
- **Testing Strategies**: [Unit, integration, component testing]
- **Known Limitations**: [Performance bottlenecks, library issues]
- **Anti-Patterns**: [Things to avoid based on codebase]
```

### Step 3: Comprehensive Blueprint Generation
```markdown
**Phase 3.1: Requirements Analysis**
```
I'll analyze the feature requirements:
- Parse [initial-requirements-file] if provided
- Identify core functionality needed
- Map to existing system capabilities
- Determine integration points
- Assess complexity and dependencies
```

**Phase 3.2: Architecture Mapping**
```
I'll map the feature to the established architecture:
- Database layer design (entities, relationships)
- Repository layer design (data access patterns)
- Service layer design (business logic organization)
- API layer design (endpoints, DTOs)
- Frontend layer design (components, hooks, services)
- Authentication/Authorization requirements
- Cross-cutting concerns (logging, caching, validation)
```

**Phase 3.3: File Structure Planning**
```
I'll plan the exact file structure following folder-per-feature:

Backend Files:
├── Entities/
│   └── [Feature]Entity.cs
├── DTOs/
│   ├── [Feature]Dto.cs
│   ├── Create[Feature]Dto.cs
│   └── Update[Feature]Dto.cs
├── Repositories/
│   ├── I[Feature]Repository.cs
│   └── [Feature]Repository.cs
├── Services/
│   ├── I[Feature]Service.cs
│   └── [Feature]Service.cs
├── Controllers/
│   └── [Feature]Controller.cs
└── Tests/
    ├── [Feature]ServiceTests.cs
    ├── [Feature]RepositoryTests.cs
    └── [Feature]ControllerTests.cs

Frontend Files:
src/modules/[feature]/
├── types/
│   └── [feature].types.ts
├── services/
│   └── [feature]Service.ts
├── hooks/
│   ├── use[Feature].ts
│   ├── use[Feature]List.ts
│   └── use[Feature]Form.ts
├── components/
│   ├── [Feature]Card.tsx
│   ├── [Feature]Form.tsx
│   ├── [Feature]List.tsx
│   └── [Feature]Detail.tsx
├── views/
│   ├── [Feature]View.tsx
│   └── [Feature]Management.tsx
└── tests/
    ├── [Feature]Service.test.ts
    ├── use[Feature].test.ts
    └── [Feature]Card.test.tsx
```

**Phase 3.4: Implementation Blueprint Creation**
```
Based on research and analysis, I'll create the comprehensive PRP:

## � Goal
[Specific, measurable implementation goal]

## 🔥 Why  
[Business value and technical rationale]

## 🧠 All Needed Context
[Complete context from research phase]

## 🏗️ Implementation Blueprint
[Step-by-step plan with SRP compliance]

## 🔄 Validation Loop
[Executable commands for verification]

## 🚨 Anti-Patterns to Avoid
[Project-specific anti-patterns]

## 🎯 Integration Points
[All configuration and setup changes]

## ✅ Final Validation Checklist
[Quality gates and acceptance criteria]
```
```

### Step 4: Quality Validation and Scoring
```markdown
**Phase 4.1: Context Completeness Validation**
```
I'll verify all necessary context is included:
- [ ] Codebase patterns identified and documented
- [ ] Similar features found and analyzed
- [ ] Technology stack fully documented
- [ ] Configuration patterns understood
- [ ] Integration points mapped
- [ ] Dependencies identified
- [ ] Testing strategies defined
Score: X/10
```

**Phase 4.2: Pattern Consistency Validation**
```
I'll ensure alignment with existing codebase:
- [ ] Naming conventions match existing code
- [ ] Folder structure follows established patterns
- [ ] API design matches current endpoints
- [ ] Component structure follows React patterns
- [ ] Database design aligns with existing schema
- [ ] Error handling follows project standards
- [ ] Logging patterns are consistent
Score: X/10
```

**Phase 4.3: Implementation Detail Validation**
```
I'll verify sufficient detail for one-pass implementation:
- [ ] Step-by-step instructions provided
- [ ] File contents and structure specified
- [ ] Code examples included where needed
- [ ] Configuration changes documented
- [ ] Database migrations planned
- [ ] Testing approach detailed
- [ ] Deployment considerations included
Score: X/10
```

**Phase 4.4: SRP Compliance Validation**
```
I'll verify Single Responsibility Principle adherence:
- [ ] Each class has exactly one responsibility
- [ ] Controllers handle only HTTP concerns
- [ ] Services contain only business logic
- [ ] Repositories handle only data access
- [ ] Components have single UI responsibility
- [ ] Hooks manage single state concern
- [ ] Clear separation of concerns maintained
Score: X/10
```

**Phase 4.5: Security and Performance Validation**
```
I'll ensure security and performance considerations:
- [ ] Input validation strategies defined
- [ ] Authorization requirements specified
- [ ] SQL injection prevention measures
- [ ] XSS protection considerations
- [ ] Performance optimization plans
- [ ] Caching strategies defined
- [ ] Database indexing planned
Score: X/10
```

**Phase 4.6: i18n and Accessibility Validation**
```
I'll verify internationalization and accessibility:
- [ ] Translation keys planned for English/Arabic
- [ ] RTL layout considerations included
- [ ] Accessibility requirements defined
- [ ] Cultural formatting considerations
- [ ] Error message localization planned
- [ ] Date/time localization handled
Score: X/10
```

**Final Quality Assessment:**
- Context Completeness: X/10
- Pattern Consistency: X/10  
- Implementation Detail: X/10
- SRP Compliance: X/10
- Security/Performance: X/10
- i18n/Accessibility: X/10

**Overall Confidence Score: X/10**
**Minimum Required: 8/10**

**PRP Status: [READY_FOR_IMPLEMENTATION/NEEDS_IMPROVEMENT/REQUIRES_REVISION]**

If score < 8/10, I'll iterate and improve the PRP until it meets quality standards.
```

## Command Activation Process
When a user types:
```
@copilot /generate-prp [module-name] [feature-name] [initial-requirements-file]
```

The system should:
1. **Parse Parameters**: Extract module-name, feature-name, and optional requirements file
2. **Execute Research**: Run all four research phases systematically
3. **Generate PRP**: Create comprehensive PRP file using the template
4. **Validate Quality**: Ensure minimum 8/10 confidence score
5. **Save Output**: Create file in proper directory with correct naming
6. **Provide Summary**: Give user overview of generated PRP and next steps

## PRP Output Template

The generated PRP file will follow this comprehensive structure:

```markdown
# [Feature Name] Implementation PRP

> **Generated by**: GitHub Copilot PRP Generator  
> **Date**: [Current Date]  
> **Module**: {module-name}  
> **Feature**: {feature-name}  
> **Confidence Score**: X/10

## 🎯 Goal
[Clear, specific implementation goal based on requirements analysis]

## 🔥 Why
[Business value, technical rationale, and integration benefits]

## 📋 Requirements Summary
[Parsed requirements from analysis]

## 🧠 All Needed Context

### Codebase Analysis Results
- **Similar Features**: [List with file references]
- **Architecture Patterns**: [Identified patterns]
- **Technology Stack**: [Current versions and configurations]
- **Database Schema**: [Relevant entities and relationships]
- **API Patterns**: [Endpoint design standards]
- **Frontend Patterns**: [Component and hook structures]

### Documentation References
- **Internal Guidelines**: [Relevant guideline files]
- **Code Examples**: [Specific example files to follow]
- **Configuration Files**: [Setup and config references]
- **Testing Patterns**: [Test structure examples]

### Integration Points Identified
- **Backend**: [API endpoints, services, repositories]
- **Frontend**: [Components, hooks, routing]
- **Database**: [Schema changes, migrations]
- **Authentication**: [Permission requirements]
- **Internationalization**: [Translation requirements]

## 🏗️ Implementation Blueprint

### Phase 1: Data Layer Implementation
```csharp
// Entity Definition
// File: backend/Ikhtibar.Core/Entities/[Feature]Entity.cs
[Detailed code structure with SRP compliance]

// DTOs
// File: backend/Ikhtibar.API/DTOs/[Feature]Dto.cs
[Complete DTO definitions]

// Repository Interface
// File: backend/Ikhtibar.Core/Repositories/I[Feature]Repository.cs
[Interface with all required methods]

// Repository Implementation  
// File: backend/Ikhtibar.Infrastructure/Repositories/[Feature]Repository.cs
[Full implementation with error handling]
```

### Phase 2: Business Layer Implementation
```csharp
// Service Interface
// File: backend/Ikhtibar.Core/Services/I[Feature]Service.cs
[Business logic interface]

// Service Implementation
// File: backend/Ikhtibar.Core/Services/[Feature]Service.cs
[Complete service with validation and error handling]
```

### Phase 3: API Layer Implementation
```csharp
// Controller Implementation
// File: backend/Ikhtibar.API/Controllers/[Feature]Controller.cs
[RESTful endpoints with proper HTTP status codes]
```

### Phase 4: Frontend Implementation
```typescript
// Type Definitions
// File: frontend/src/modules/[feature]/types/[feature].types.ts
[Complete TypeScript interfaces]

// API Service
// File: frontend/src/modules/[feature]/services/[feature]Service.ts
[API integration with error handling]

// Custom Hooks
// File: frontend/src/modules/[feature]/hooks/use[Feature].ts
[State management hooks]

// Components
// File: frontend/src/modules/[feature]/components/[Feature]Card.tsx
[React components with proper TypeScript and i18n]
```

### Phase 5: Testing Implementation
```csharp
// Backend Tests
[Unit test examples for services and repositories]
```

```typescript
// Frontend Tests
[Component and hook test examples]
```

### Phase 6: Integration Setup
- **Database Migration**: [SQL migration script]
- **DI Registration**: [Service registration code]
- **Routing**: [API and frontend route configuration]
- **Permissions**: [Authorization setup]
- **Translations**: [i18n key definitions for en/ar]

## 🔄 Validation Loop

### Level 1: Syntax Validation
```powershell
# Backend validation
dotnet build --configuration Release
dotnet format --verify-no-changes
dotnet test --filter "Category=Unit"

# Frontend validation
npm run type-check
npm run lint
npm run test:unit
```

### Level 2: Integration Validation
```bash
# API endpoint testing
curl -X GET http://localhost:5000/api/[feature]
curl -X POST http://localhost:5000/api/[feature] -d '[sample-data]'

# Frontend component testing
npm run test:components
npm run test:e2e
```

### Level 3: Quality Validation
```powershell
# Code quality checks
dotnet test --collect:"XPlat Code Coverage"
npm run test -- --coverage
```

## 🚨 Anti-Patterns to Avoid

### Backend Anti-Patterns
- ❌ **Mixed Responsibilities**: Don't put business logic in controllers
- ❌ **Data Access in Services**: Use repositories for all data access
- ❌ **Sync I/O Operations**: Always use async/await for I/O
- ❌ **Generic Exception Handling**: Catch specific exceptions
- ❌ **Hardcoded Values**: Use configuration for all settings

### Frontend Anti-Patterns
- ❌ **Business Logic in Components**: Keep components focused on UI
- ❌ **Direct API Calls**: Use service layer for all API communication
- ❌ **Missing Error Boundaries**: Always handle error states
- ❌ **Inline Styling**: Use Tailwind classes consistently
- ❌ **Hardcoded Text**: Use i18n for all user-facing text

## 🎯 Integration Points

### Configuration Changes
```json
// appsettings.json additions
// package.json script updates
// Route configuration updates
```

### Database Changes
```sql
-- Migration script
-- Index creation
-- Constraint additions
```

### Navigation Updates
```typescript
// Menu item additions
// Route definitions
// Permission mappings
```

## 📊 Performance Considerations
- **Database**: [Indexing strategy, query optimization]
- **API**: [Pagination, caching, response optimization]
- **Frontend**: [Code splitting, memoization, lazy loading]

## 🔒 Security Considerations
- **Input Validation**: [Validation strategy]
- **Authorization**: [Permission requirements]
- **Data Protection**: [Sensitive data handling]

## 🌐 Internationalization Plan
- **Translation Keys**: [Required keys for en/ar]
- **RTL Support**: [Layout considerations]
- **Cultural Formatting**: [Dates, numbers, currency]

## ✅ Final Validation Checklist

### Implementation Complete
- [ ] All backend files created and tested
- [ ] All frontend files created and tested
- [ ] Database migrations applied
- [ ] API endpoints functional
- [ ] UI components working
- [ ] Tests passing
- [ ] Code quality checks passing

### SRP Compliance
- [ ] Each class has single responsibility
- [ ] Controllers handle only HTTP concerns
- [ ] Services contain only business logic
- [ ] Repositories handle only data access
- [ ] Components have single UI purpose

### Integration Complete
- [ ] DI registration updated
- [ ] Routes configured
- [ ] Permissions assigned
- [ ] Translations added
- [ ] Documentation updated

### Quality Gates
- [ ] Code coverage > 80%
- [ ] All tests passing
- [ ] No linting errors
- [ ] Performance requirements met
- [ ] Security requirements met
- [ ] Accessibility requirements met

## 📝 Notes and Considerations
[Additional notes, future enhancements, and maintenance considerations]
```

## Example Usage

### Generate PRP for User Management
```
@copilot /generate-prp user-management user-permissions
```

**Expected Process:**
1. Research existing user-related code patterns
2. Gather ASP.NET Core and React documentation
3. Create comprehensive implementation blueprint
4. Validate completeness and accuracy
5. Generate `PRPs/01-user-management/01-user-permissions-prp.md`

### Generate PRP from Existing Requirements
```
@copilot /generate-prp notification notification-system requirements/notification.md
```

**Expected Process:**
1. Analyze existing requirements file (if provided)
2. Research Ikhtibar codebase for relevant patterns
3. Gather documentation specific to feature requirements
4. Create tailored implementation blueprint following folder-per-feature architecture
5. Generate comprehensive PRP file with all necessary context

## Complete Workflow with Expected Outputs

### 1. Command Execution
```
@copilot /generate-prp audit audit-trail requirements/audit.md
```

### 2. Analysis Phase Output
```markdown
## Codebase Analysis Results

### Related Patterns Identified
- **Controllers**: Found similar pattern in `AuthController.cs`
- **Services**: Found similar service layer in `src/modules/auth/services/`
- **Components**: Found reusable components in `src/shared/components/`
- **Hooks**: Found API integration patterns in `src/modules/auth/hooks/`

### Key Integration Points
- **Backend**: New API endpoints needed at `/api/audit`
- **Frontend**: New module required in `src/modules/audit/`
- **Database**: New tables for audit trail storage
- **Auth**: Integration with existing permission system
```

### 3. PRP Output Structure
A comprehensive PRP file will be generated at:
```
.github/copilot/PRPs/06-audit/01-audit-trail-prp.md
```

The generated PRP includes:

#### Backend Components
- **Models**: `AuditEntity.cs`, `AuditDto.cs`, `CreateAuditRequest.cs`
- **Repository**: `IAuditRepository.cs`, `AuditRepository.cs` 
- **Service**: `IAuditService.cs`, `AuditService.cs`
- **Controller**: `AuditController.cs`
- **Tests**: `AuditServiceTests.cs`, `AuditControllerTests.cs`

#### Frontend Components
- **Types**: `audit.types.ts` (interfaces)
- **API**: `auditService.ts` (API integration)
- **Hooks**: `useAudit.ts` (data fetching)
- **Components**: `AuditCard.tsx`, `AuditList.tsx`, `AuditDetail.tsx`
- **Views**: `AuditView.tsx`, `AuditManagement.tsx`
- **Tests**: Component and hook tests

#### Integration Points
- Database migration script
- API endpoint configuration
- Frontend route registration
- Permission configuration
- i18n translation keys for Arabic and English

## Quality Gates

### Minimum Requirements for PRP Completion
- **Context Completeness**: 8/10 - All necessary context included
- **Pattern Consistency**: 9/10 - Follows established Ikhtibar codebase patterns
- **Implementation Detail**: 8/10 - Sufficient detail for one-pass success
- **Validation Coverage**: 9/10 - Complete testing for both .NET and React components
- **Integration Coverage**: 8/10 - All integration points identified
- **Overall Confidence**: 8/10 - Ready for execution

### Output Structure
The generated PRP will include:
- Complete research findings with links to existing code patterns
- Comprehensive documentation references from both backend and frontend
- Detailed implementation blueprint with file locations
- Executable validation commands for .NET and React
- Project-specific anti-patterns to avoid
- Integration point specifications for all systems
- Quality validation checklist for SRP compliance

## Notes
- The research phase ensures pattern consistency with existing codebase
- Documentation gathering includes both external and internal resources
- Blueprint creation follows established project conventions
- Quality checking ensures one-pass implementation success
- Generated PRPs are tailored to your specific project setup and patterns
