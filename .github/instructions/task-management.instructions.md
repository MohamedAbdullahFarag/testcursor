---
description: "Task breakdown and management with validation loops and concise execution steps"
applyTo: "**/*.md,**/tasks/**,**/project-management/**"
---

# Task Management Instructions

You are breaking down complex work into manageable, executable tasks for the Ikhtibar educational exam management system with embedded validation and debugging guidance.

## Task Management Philosophy

### Information Dense Tasks
- Use concise, executable task descriptions
- Include embedded context and validation commands
- Provide clear action keywords for meaningful descriptions
- Embed debugging hints for common issues

### Validation-First Approach
- Each task includes validation commands
- Progressive enhancement through iterative refinement
- Self-correcting feedback loops
- Clear success/failure criteria

### Context Embedded
- Include relevant documentation references
- Reference existing patterns and files
- Provide gotchas and common pitfalls
- Include rollback procedures for risky changes

## Task Format Structure

### Basic Task Template
```
[ACTION] path/to/file:
  - [OPERATION]: [DETAILS]
  - VALIDATE: [COMMAND]
  - IF_FAIL: [DEBUG_HINT]
  - DEPENDS_ON: [PREVIOUS_TASK]
```

### Action Keywords
Use these standardized action keywords for consistent task descriptions:

- **READ**: Understand existing patterns and structure
- **CREATE**: New file with specific content and purpose
- **UPDATE**: Modify existing file with specific changes
- **DELETE**: Remove file, code, or configuration
- **FIND**: Search for patterns or existing implementations
- **TEST**: Verify behavior and functionality
- **FIX**: Debug and repair issues
- **DEPLOY**: Release or publish changes
- **CONFIGURE**: Setup or modify configuration
- **MIGRATE**: Database or data structure changes

## Task Templates by Category

### Setup and Investigation Tasks

```markdown
## Initial Investigation Tasks

READ backend/src/Features/Auth/Controllers/AuthController.cs:
  - UNDERSTAND: Current authentication flow and patterns
  - FIND: Error handling approach used
  - NOTE: JWT token validation implementation
  - VALIDATE: dotnet build
  - IF_FAIL: Check for missing dependencies

READ frontend/src/modules/auth/services/authService.ts:
  - UNDERSTAND: API integration patterns
  - FIND: Error handling and response transformation
  - NOTE: TypeScript interface usage
  - VALIDATE: npm run type-check
  - IF_FAIL: Install missing type dependencies

READ backend/src/Infrastructure/Data/DbConnectionFactory.cs:
  - UNDERSTAND: Database connection management
  - FIND: Dapper configuration and usage patterns
  - NOTE: Connection string handling
  - VALIDATE: Test connection string in appsettings.json
  - IF_FAIL: Verify SQL Server connection
```

### Backend Implementation Tasks

```markdown
## Backend Development Tasks

CREATE backend/src/Features/ExamManagement/Models/ExamEntity.cs:
  - COPY_PATTERN: backend/src/Features/Auth/Models/UserEntity.cs
  - IMPLEMENT: Exam entity following BaseEntity inheritance
  - INCLUDE: Proper data annotations and relationships
  - VALIDATE: dotnet build --configuration Release
  - IF_FAIL: Check BaseEntity import and namespace

UPDATE backend/src/Infrastructure/Data/IkhtibarDbContext.cs:
  - FIND: DbSet<UserEntity> Users
  - ADD: DbSet<ExamEntity> Exams
  - ADD: DbSet<QuestionEntity> Questions
  - VALIDATE: dotnet ef migrations add ExamManagement
  - IF_FAIL: Check Dapper installation

CREATE backend/src/Features/ExamManagement/Repositories/IExamRepository.cs:
  - COPY_PATTERN: backend/src/Features/Auth/Repositories/IUserRepository.cs
  - IMPLEMENT: Exam-specific repository interface
  - INCLUDE: Async methods for CRUD operations
  - VALIDATE: dotnet build
  - IF_FAIL: Verify interface naming conventions

CREATE backend/src/Features/ExamManagement/Repositories/ExamRepository.cs:
  - COPY_PATTERN: backend/src/Features/Auth/Repositories/UserRepository.cs
  - IMPLEMENT: Dapper-based repository implementation
  - INCLUDE: Parameterized queries for security
  - VALIDATE: dotnet test backend/tests/Repositories/ExamRepositoryTests.cs
  - IF_FAIL: Check SQL query syntax and connection string

CREATE backend/src/Features/ExamManagement/Services/ExamService.cs:
  - COPY_PATTERN: backend/src/Features/Auth/Services/AuthService.cs
  - IMPLEMENT: Business logic and validation
  - INCLUDE: Structured logging with correlation IDs
  - VALIDATE: dotnet test backend/tests/Services/ExamServiceTests.cs
  - IF_FAIL: Check dependency injection registration

UPDATE backend/src/Program.cs:
  - FIND: services.AddScoped<IUserService, UserService>()
  - ADD: services.AddScoped<IExamService, ExamService>()
  - ADD: services.AddScoped<IExamRepository, ExamRepository>()
  - VALIDATE: dotnet run --project backend/src/Ikhtibar.API
  - IF_FAIL: Check for circular dependencies in DI container
```

### Frontend Implementation Tasks

```markdown
## Frontend Development Tasks

CREATE frontend/src/modules/exam-management/types/exam.types.ts:
  - COPY_PATTERN: frontend/src/modules/auth/types/auth.types.ts
  - IMPLEMENT: TypeScript interfaces for Exam domain
  - INCLUDE: Request/response types and component props
  - VALIDATE: npm run type-check
  - IF_FAIL: Check TypeScript configuration and imports

CREATE frontend/src/modules/exam-management/services/examService.ts:
  - COPY_PATTERN: frontend/src/modules/auth/services/authService.ts
  - IMPLEMENT: API service class with HTTP client
  - INCLUDE: Error handling and response transformation
  - VALIDATE: npm run test -- examService.test.ts
  - IF_FAIL: Check API base URL configuration

CREATE frontend/src/modules/exam-management/hooks/useExams.ts:
  - COPY_PATTERN: frontend/src/modules/auth/hooks/useAuth.ts
  - IMPLEMENT: React Query hooks for exam data
  - INCLUDE: Caching, error states, and optimistic updates
  - VALIDATE: npm run test -- useExams.test.ts
  - IF_FAIL: Check React Query provider setup

CREATE frontend/src/modules/exam-management/components/ExamForm.tsx:
  - COPY_PATTERN: frontend/src/shared/components/BaseForm.tsx
  - IMPLEMENT: Exam creation/editing form
  - INCLUDE: Validation, loading states, and accessibility
  - VALIDATE: npm run test -- ExamForm.test.tsx
  - IF_FAIL: Check form validation library integration

UPDATE frontend/src/routes/DashboardRoutes/index.tsx:
  - FIND: {path: '/users', element: <UserManagement />}
  - ADD: {path: '/exams', element: <ExamManagement />}
  - VALIDATE: npm run dev and navigate to /dashboard/exams
  - IF_FAIL: Check route component imports and lazy loading
```

### Database Tasks

```markdown
## Database Management Tasks

CREATE backend/database/migrations/001_ExamManagement.sql:
  - IMPLEMENT: CREATE TABLE statements for Exams and Questions
  - INCLUDE: Proper foreign keys, indexes, and constraints
  - INCLUDE: Seed data for question types and sample content
  - VALIDATE: sqlcmd -S localhost -d IkhtibarDB -i 001_ExamManagement.sql
  - IF_FAIL: Check SQL syntax and database connection

UPDATE backend/database/migrations/002_UpdatePermissions.sql:
  - FIND: Existing role permissions
  - ADD: Exam management permissions for Teacher and Admin roles
  - VALIDATE: SELECT * FROM RolePermissions WHERE PermissionName LIKE '%Exam%'
  - IF_FAIL: Check role and permission table structure

MIGRATE backend/database/:
  - RUN: dotnet ef database update
  - VALIDATE: dotnet ef migrations list
  - IF_FAIL: Check migration order and dependencies
  - ROLLBACK: dotnet ef database update [PreviousMigration]
```

### Testing Tasks

```markdown
## Testing Implementation Tasks

CREATE backend/tests/IntegrationTests/ExamManagementTests.cs:
  - COPY_PATTERN: backend/tests/IntegrationTests/AuthTests.cs
  - IMPLEMENT: End-to-end workflow tests
  - INCLUDE: Database setup, cleanup, and test data
  - VALIDATE: dotnet test --filter "Category=Integration"
  - IF_FAIL: Check test database configuration

CREATE frontend/src/modules/exam-management/__tests__/ExamForm.test.tsx:
  - COPY_PATTERN: frontend/src/modules/auth/__tests__/LoginForm.test.tsx
  - IMPLEMENT: Component testing with React Testing Library
  - INCLUDE: User interactions, validation, and error states
  - VALIDATE: npm run test -- --coverage --watchAll=false
  - IF_FAIL: Check test environment setup and mocks

CREATE frontend/tests/e2e/exam-creation.spec.ts:
  - COPY_PATTERN: frontend/tests/e2e/user-login.spec.ts
  - IMPLEMENT: Playwright end-to-end tests
  - INCLUDE: Complete user workflow from login to exam creation
  - VALIDATE: npm run test:e2e
  - IF_FAIL: Check test environment and browser setup
```

## Validation Checkpoints

### Syntax and Style Validation
```markdown
CHECKPOINT syntax:
  - RUN: dotnet build --configuration Release
  - RUN: dotnet format --verify-no-changes
  - RUN: npm run type-check
  - RUN: npm run lint
  - CONTINUE: Only when all checks pass clean

CHECKPOINT tests:
  - RUN: dotnet test --logger "console;verbosity=detailed"
  - RUN: npm run test -- --coverage --watchAll=false
  - CONTINUE: Only when all tests are green
  - MINIMUM: 80% code coverage for new code

CHECKPOINT integration:
  - START: dotnet run --project backend/src/Ikhtibar.API
  - START: npm run dev
  - VALIDATE: Manual smoke test of key workflows
  - CLEANUP: Stop development servers
```

### Performance and Security Validation
```markdown
CHECKPOINT performance:
  - RUN: dotnet run --configuration Release
  - TEST: API response times < 500ms
  - TEST: Frontend bundle size < 2MB
  - TEST: Page load times < 3 seconds

CHECKPOINT security:
  - RUN: dotnet list package --vulnerable
  - RUN: npm audit
  - TEST: Authentication and authorization flows
  - TEST: Input validation and sanitization
```

## Common Debug Patterns

### Backend Debug Patterns
```markdown
DEBUG compile_error:
  - CHECK: Namespace imports and using statements
  - CHECK: Project references in .csproj files
  - FIX: dotnet restore and rebuild solution

DEBUG database_error:
  - CHECK: Connection string in appsettings.json
  - CHECK: SQL Server service running
  - FIX: Update database and run migrations

DEBUG dependency_injection_error:
  - CHECK: Service registration in Program.cs
  - CHECK: Interface and implementation naming
  - FIX: Verify DI container configuration

DEBUG api_error:
  - CHECK: Controller routing and HTTP verbs
  - CHECK: Authentication and authorization attributes
  - FIX: Verify middleware pipeline configuration
```

### Frontend Debug Patterns
```markdown
DEBUG typescript_error:
  - CHECK: Type definitions and imports
  - CHECK: Interface property naming and types
  - FIX: Update TypeScript configuration

DEBUG component_error:
  - CHECK: Props interface and component implementation
  - CHECK: Hook dependencies and state management
  - FIX: Verify React Query provider setup

DEBUG build_error:
  - CHECK: Package.json dependencies and versions
  - CHECK: Vite configuration and plugins
  - FIX: Clear node_modules and reinstall

DEBUG api_integration_error:
  - CHECK: Base URL and endpoint configuration
  - CHECK: Request/response type definitions
  - FIX: Verify API service error handling
```

## Task Examples by Complexity

### Simple Tasks (30 minutes - 2 hours)
```markdown
CREATE frontend/src/modules/exam-management/constants/examTypes.ts:
  - IMPLEMENT: Enum for question types (MCQ, Essay, TrueFalse)
  - INCLUDE: Display labels and validation rules
  - VALIDATE: npm run type-check
  - IF_FAIL: Check enum syntax and exports

UPDATE frontend/src/shared/locales/en.json:
  - FIND: "auth" section
  - ADD: "exam" section with all UI text
  - VALIDATE: npm run dev and check UI text display
  - IF_FAIL: Check i18n configuration and key structure
```

### Medium Tasks (4-8 hours)
```markdown
CREATE backend/src/Features/ExamManagement/Services/ExamService.cs:
  - IMPLEMENT: Complete business logic for exam CRUD
  - INCLUDE: Validation, authorization, and audit logging
  - INCLUDE: Error handling and performance optimization
  - VALIDATE: dotnet test backend/tests/Services/ExamServiceTests.cs
  - IF_FAIL: Check business rule implementation and dependencies

CREATE frontend/src/modules/exam-management/components/ExamBuilder.tsx:
  - IMPLEMENT: Drag-and-drop exam creation interface
  - INCLUDE: Question selection, preview, and validation
  - INCLUDE: Responsive design and accessibility
  - VALIDATE: npm run test -- ExamBuilder.test.tsx
  - IF_FAIL: Check drag-and-drop library integration and styling
```

### Complex Tasks (1-3 days)
```markdown
IMPLEMENT backend/src/Features/ExamManagement/ (Complete Feature):
  - PHASE_1: Entity models and database schema
  - PHASE_2: Repository and service layer implementation
  - PHASE_3: API controllers and authentication
  - PHASE_4: Integration tests and documentation
  - VALIDATE: Complete end-to-end API testing
  - IF_FAIL: Break down into smaller tasks and debug incrementally

IMPLEMENT frontend/src/modules/exam-management/ (Complete Module):
  - PHASE_1: Type definitions and API services
  - PHASE_2: Custom hooks and state management
  - PHASE_3: UI components and forms
  - PHASE_4: Integration with routing and navigation
  - VALIDATE: Complete user workflow testing
  - IF_FAIL: Focus on individual components and build up
```

## Tips for Effective Task Management

### Task Dependencies
- Always specify task dependencies clearly
- Use `DEPENDS_ON` to indicate prerequisite tasks
- Consider parallel execution where possible
- Plan for rollback procedures in risky changes

### Validation Strategy
- Include validation commands after every change
- Use fast feedback loops for immediate issue detection
- Implement comprehensive testing at multiple levels
- Plan for manual validation of user workflows

### Documentation
- Keep tasks self-documenting with clear action keywords
- Include context and rationale for complex decisions
- Reference existing patterns and conventions
- Provide troubleshooting guidance for common issues

Always ensure tasks are atomic, testable, and provide clear success criteria. Break down complex work into manageable chunks that can be completed and validated independently.
