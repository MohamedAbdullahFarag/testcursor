---
description: "Specification template for detailed feature implementation with low-level tasks"
applyTo: "**/*.md,**/specifications/**,**/docs/**"
---

# Feature Specification Instructions

You are creating detailed technical specifications for the Ikhtibar educational exam management system using a structured approach that breaks down high-level objectives into actionable implementation tasks.

## Specification Philosophy

### High-Level to Low-Level Decomposition
- Start with clear business objectives
- Break down into technical components
- Define specific implementation tasks
- Include validation and testing steps

### Implementation-Ready Output
- Provide executable tasks in proper sequence
- Include file paths and function specifications
- Define acceptance criteria for each task
- Ensure seamless integration with existing codebase

### Context-Aware Development
- Reference existing patterns and conventions
- Consider project architecture and constraints
- Include integration points and dependencies
- Follow established coding standards

## Specification Template Structure

### High-Level Objective
Define the overarching goal in business terms:

```markdown
## High-Level Objective

Build a comprehensive exam creation interface that allows teachers to:
- Create and configure exams with multiple question types
- Set timing and attempt restrictions
- Preview exams before publication
- Manage question banks and categories
- Integrate with the existing authentication and user management system
```

### Mid-Level Objectives
Break down the high-level goal into concrete, measurable components:

```markdown
## Mid-Level Objectives

1. **Question Bank Management**
   - Implement CRUD operations for questions
   - Support multiple question types (MCQ, True/False, Essay, Fill-in-blank)
   - Enable categorization and tagging system
   - Provide search and filtering capabilities

2. **Exam Configuration Interface**
   - Create intuitive exam builder UI
   - Implement drag-and-drop question selection
   - Configure exam settings (time limits, attempts, randomization)
   - Preview functionality with student view

3. **Data Persistence Layer**
   - Design database schema for exams and questions
   - Implement repository pattern for data access
   - Ensure ACID compliance for exam operations
   - Optimize queries for performance

4. **API Development**
   - Create RESTful endpoints for exam management
   - Implement proper authentication and authorization
   - Include comprehensive input validation
   - Provide consistent error handling and responses

5. **Integration Points**
   - Connect with existing user management system
   - Integrate with notification service
   - Link to analytics and reporting modules
   - Ensure role-based access control
```

### Implementation Notes
Provide technical guidance and constraints:

```markdown
## Implementation Notes

### Technical Requirements
- **Backend**: ASP.NET Core 8.0 Web API with Clean Architecture
- **Frontend**: React.js 18 with TypeScript, using Vite build tool
- **Database**: SQL Server with Dapper ORM
- **Authentication**: JWT Bearer tokens with role-based authorization
- **UI Framework**: Tailwind CSS with responsive design
- **Internationalization**: Support for English and Arabic (RTL/LTR)

### Architecture Constraints
- Follow folder-per-feature organization
- Implement repository pattern for data access
- Use service layer for business logic
- Maintain separation of concerns across layers
- Include comprehensive error handling and logging

### Performance Requirements
- API response times < 500ms for standard operations
- Support for 1000+ concurrent users
- Efficient pagination for large question banks
- Optimized database queries with proper indexing

### Security Requirements
- Input validation and sanitization
- SQL injection prevention through parameterized queries
- XSS protection in frontend components
- CSRF protection for state-changing operations
- Secure file upload handling for media questions

### Quality Standards
- Unit test coverage > 80%
- Integration tests for critical workflows
- Code review for all changes
- Automated linting and formatting
- Documentation for all public APIs
```

### Context Definition
Define the current and desired state:

```markdown
## Context

### Beginning Context
**Existing Files and Components:**
- `backend/src/Features/Auth/` - User authentication system
- `backend/src/Features/UserManagement/` - User and role management
- `backend/src/Shared/Common/BaseEntity.cs` - Base entity class
- `backend/src/Infrastructure/Data/DbConnectionFactory.cs` - Database connection
- `frontend/src/modules/auth/` - Authentication UI components
- `frontend/src/shared/services/apiService.ts` - HTTP client service
- `frontend/src/shared/components/` - Reusable UI components

**Current Capabilities:**
- User registration and login
- Role-based access control
- Basic dashboard layout
- API infrastructure
- Database connection management

### Ending Context
**New Files to be Created:**
- `backend/src/Features/ExamManagement/` - Complete exam management feature
- `backend/src/Features/QuestionBank/` - Question bank management
- `frontend/src/modules/exam-management/` - Exam creation UI
- `frontend/src/modules/question-bank/` - Question management UI
- Database migration scripts for exam-related tables

**New Capabilities:**
- Complete exam creation and management workflow
- Question bank with categorization and search
- Preview and testing functionality
- Integration with existing user and auth systems
- Responsive UI supporting multiple languages
```

### Low-Level Tasks
Provide ordered, executable tasks:

```markdown
## Low-Level Tasks

### Task 1: Database Schema Design
**Action**: CREATE database migration scripts
**Files**: `backend/database/migrations/001_ExamManagement.sql`
**Prompt**: "Create database schema for exam management with tables for Exams, Questions, QuestionTypes, ExamQuestions, and UserExamAttempts. Include proper foreign keys, indexes, and constraints."
**Details**:
- Exam table with metadata (title, description, settings, created_by, etc.)
- Question table with content, type, correct_answers, points
- ExamQuestions junction table for exam-question relationships
- Proper indexing for performance optimization
**Validation**: Run migration script and verify table creation

### Task 2: Backend Entity Models
**Action**: CREATE entity classes
**Files**: 
- `backend/src/Features/ExamManagement/Models/ExamEntity.cs`
- `backend/src/Features/QuestionBank/Models/QuestionEntity.cs`
**Prompt**: "Create entity models following BaseEntity pattern with proper data annotations and relationships."
**Details**:
- Inherit from BaseEntity for common fields
- Include proper data annotations for validation
- Define navigation properties for relationships
- Follow established naming conventions
**Validation**: Compile without errors and pass entity validation tests

### Task 3: Repository Interfaces and Implementation
**Action**: CREATE repository layer
**Files**:
- `backend/src/Features/ExamManagement/Repositories/IExamRepository.cs`
- `backend/src/Features/ExamManagement/Repositories/ExamRepository.cs`
- `backend/src/Features/QuestionBank/Repositories/IQuestionRepository.cs`
- `backend/src/Features/QuestionBank/Repositories/QuestionRepository.cs`
**Prompt**: "Implement repository pattern with Dapper for exam and question data access, following existing repository patterns in the codebase."
**Details**:
- Implement CRUD operations with async methods
- Use parameterized queries for security
- Include pagination support for large datasets
- Follow existing repository patterns from UserRepository
**Validation**: Unit tests pass for all repository methods

### Task 4: Service Layer Implementation
**Action**: CREATE service layer
**Files**:
- `backend/src/Features/ExamManagement/Services/IExamService.cs`
- `backend/src/Features/ExamManagement/Services/ExamService.cs`
- `backend/src/Features/QuestionBank/Services/IQuestionService.cs`
- `backend/src/Features/QuestionBank/Services/QuestionService.cs`
**Prompt**: "Implement business logic services with proper validation, error handling, and logging following established service patterns."
**Details**:
- Implement business rules and validation
- Include comprehensive error handling
- Use structured logging with correlation IDs
- Follow single responsibility principle
**Validation**: Integration tests pass for all service methods

### Task 5: API Controllers
**Action**: CREATE REST API controllers
**Files**:
- `backend/src/Features/ExamManagement/Controllers/ExamsController.cs`
- `backend/src/Features/QuestionBank/Controllers/QuestionsController.cs`
**Prompt**: "Create RESTful API controllers with proper HTTP status codes, authentication, and documentation following existing controller patterns."
**Details**:
- Implement standard REST endpoints (GET, POST, PUT, DELETE)
- Include proper authentication and authorization attributes
- Add Swagger documentation with examples
- Follow existing controller patterns from AuthController
**Validation**: API tests pass and Swagger documentation is complete

### Task 6: Frontend TypeScript Interfaces
**Action**: CREATE TypeScript type definitions
**Files**:
- `frontend/src/modules/exam-management/types/exam.types.ts`
- `frontend/src/modules/question-bank/types/question.types.ts`
**Prompt**: "Define comprehensive TypeScript interfaces for exam and question entities with proper typing for all properties and methods."
**Details**:
- Include all entity properties with correct types
- Define request/response interfaces for API calls
- Add component prop interfaces
- Include utility types for forms and validation
**Validation**: TypeScript compilation passes without errors

### Task 7: API Service Layer
**Action**: CREATE frontend API services
**Files**:
- `frontend/src/modules/exam-management/services/examService.ts`
- `frontend/src/modules/question-bank/services/questionService.ts`
**Prompt**: "Implement API service classes for exam and question management using existing apiService patterns with proper error handling."
**Details**:
- Use existing HTTP client configuration
- Implement request/response transformation
- Include comprehensive error handling
- Follow existing service patterns from authService
**Validation**: API integration tests pass

### Task 8: React Custom Hooks
**Action**: CREATE custom hooks for state management
**Files**:
- `frontend/src/modules/exam-management/hooks/useExams.ts`
- `frontend/src/modules/exam-management/hooks/useCreateExam.ts`
- `frontend/src/modules/question-bank/hooks/useQuestions.ts`
**Prompt**: "Create custom React hooks using React Query for data fetching and state management with proper caching and error handling."
**Details**:
- Use React Query for server state management
- Implement optimistic updates where appropriate
- Include loading and error states
- Follow existing hook patterns from useAuth
**Validation**: Hook tests pass and data fetching works correctly

### Task 9: UI Components
**Action**: CREATE React components
**Files**:
- `frontend/src/modules/exam-management/components/ExamForm.tsx`
- `frontend/src/modules/exam-management/components/ExamList.tsx`
- `frontend/src/modules/question-bank/components/QuestionForm.tsx`
- `frontend/src/modules/question-bank/components/QuestionList.tsx`
**Prompt**: "Create responsive React components with proper accessibility, internationalization, and error handling following existing component patterns."
**Details**:
- Use Tailwind CSS for styling
- Include proper ARIA attributes for accessibility
- Support RTL/LTR layouts for Arabic/English
- Follow existing component patterns from shared components
**Validation**: Component tests pass and UI renders correctly

### Task 10: Integration and Testing
**Action**: CREATE integration tests
**Files**:
- `backend/tests/IntegrationTests/ExamManagementTests.cs`
- `frontend/src/modules/exam-management/__tests__/integration.test.tsx`
**Prompt**: "Create comprehensive integration tests covering end-to-end workflows for exam creation and management."
**Details**:
- Test complete user workflows
- Include error scenarios and edge cases
- Verify database transactions and rollbacks
- Test authentication and authorization
**Validation**: All integration tests pass

### Task 11: Documentation
**Action**: CREATE API and user documentation
**Files**:
- `docs/api/exam-management.md`
- `docs/user-guides/exam-creation.md`
**Prompt**: "Create comprehensive documentation including API specifications, user guides, and development notes."
**Details**:
- Document all API endpoints with examples
- Include user workflows with screenshots
- Provide troubleshooting guides
- Document configuration and deployment steps
**Validation**: Documentation review completed and approved
```

## Implementation Validation

### Quality Gates
Each task should include validation criteria:

```markdown
## Validation Checklist

### Code Quality
- [ ] All unit tests pass (>80% coverage)
- [ ] Integration tests validate end-to-end workflows
- [ ] Code review completed and approved
- [ ] Linting and formatting standards met
- [ ] No security vulnerabilities detected

### Functionality
- [ ] All acceptance criteria met
- [ ] Error handling works correctly
- [ ] Performance requirements satisfied
- [ ] UI is responsive and accessible
- [ ] Internationalization works properly

### Integration
- [ ] APIs integrate with existing systems
- [ ] Database changes don't break existing functionality
- [ ] Authentication and authorization work correctly
- [ ] Notifications and analytics integration functional
- [ ] Deployment process tested and documented
```

### Success Criteria
Define measurable outcomes:

```markdown
## Success Criteria

### Technical Success
- API response times < 500ms for 95% of requests
- Zero critical security vulnerabilities
- Test coverage > 80% for all new code
- Database queries optimized with proper indexing

### User Experience Success
- Teachers can create complete exam in < 10 minutes
- Question bank search returns results in < 2 seconds
- UI works flawlessly on mobile and desktop
- Support for Arabic RTL layout is complete

### Business Success
- Feature adoption rate > 70% within first month
- User satisfaction score > 4.5/5
- Performance improvement > 30% vs. previous system
- Zero data loss incidents during migration
```

Always ensure specifications are detailed enough for immediate implementation while maintaining flexibility for technical decisions. Include comprehensive validation criteria and success metrics to ensure quality delivery.
