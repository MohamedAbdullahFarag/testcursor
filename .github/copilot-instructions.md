# GitHub Copilot Instructions - Ikhtibar Project
*Comprehensive AI Development Guide with Context-Rich Workflows*

## 🎯 Project Overview

You are working on the **Ikhtibar** educational exam management system - a production-ready full-stack web application with ASP.NET Core 8.0 Web API backend and React.js 18 frontend with TypeScript. This system enables comprehensive examination workflows from question creation to grading and analytics.

### Architecture Summary
- **Backend**: ASP.NET Core 8.0 Web API with Dapper ORM, folder-per-feature structure
- **Frontend**: React.js 18 with TypeScript, Vite, Tailwind CSS, folder-per-feature structure  
- **Database**: SQL Server with Dapper micro-ORM
- **Authentication**: JWT Bearer tokens with role-based access control
- **Internationalization**: English and Arabic support (RTL/LTR)
- **Development**: GitHub Copilot-optimized workflows with validation loops

## 🏗️ Established Foundation (Completed PRPs)

 

### ✅ Core Entities & Database (PRP-01)
- **Database Schema**: Complete 348-line schema with Users, Roles, Permissions, Questions, Answers, TreeNodes, Media
- **BaseEntity Pattern**: Audit fields (CreatedAt, ModifiedAt, DeletedAt, IsDeleted) and RowVersion for concurrency
- **Entity Relationships**: Proper foreign key relationships and constraints
- **Location**: `backend/Ikhtibar.API/Data/schema.sql`, `backend/Ikhtibar.Core/Entities/BaseEntity.cs`

### ✅ Repository Pattern (PRP-02)
- **Generic Repository**: `IRepository<T>` with comprehensive CRUD operations
- **Base Implementation**: `BaseRepository<T>` using Dapper for efficient data access
- **Connection Management**: `DbConnectionFactory` for SQL Server connections
- **Location**: `backend/Ikhtibar.Core/Repositories/`, `backend/Ikhtibar.Infrastructure/Repositories/`

### ✅ API Foundation (PRP-03)
- **Middleware Pipeline**: Complete ASP.NET Core pipeline with authentication, CORS, error handling
- **Authentication**: JWT Bearer token authentication with refresh token support
- **API Documentation**: Swagger UI available at `/swagger`
- **Health Checks**: Health endpoint at `/api/health/ping`
- **Location**: `backend/Ikhtibar.API/Program.cs`, `backend/Ikhtibar.API/Controllers/`

### ✅ Frontend Foundation (PRP-04)
- **React Architecture**: React 18 with TypeScript, Vite build system
- **State Management**: Zustand stores for authentication and global state
- **API Integration**: Axios-based API client with token management
- **Routing**: React Router v6 with protected routes
- **Location**: `frontend/src/`, validated and running on `https://localhost:5173/`

## 🧠 Specialized Instruction Files Reference

The `.github/instructions/` folder contains specialized guidance for different development scenarios:

### 🎯 Core Development Instructions
- **`general-features.instructions.md`**: Backend/Frontend feature development, API creation, component building
  - **Auto-applies to**: `**/*.cs,**/*.js,**/*.ts,**/*.tsx,**/*.json`
  - **Use for**: Standard feature implementation with validation loops

- **`typescript-features.instructions.md`**: Frontend components, hooks, TypeScript interfaces, React patterns
  - **Auto-applies to**: `**/*.ts,**/*.tsx,**/tsconfig.json,**/package.json`
  - **Use for**: Type-safe frontend development

### 📋 Planning & Architecture Instructions
- **`planning-prd.instructions.md`**: Feature planning, PRD creation, Mermaid diagrams, specifications
  - **Auto-applies to**: `**/*.md,**/README.md,**/*.txt,**/docs/**`
  - **Use for**: High-level feature planning and documentation

- **`feature-specifications.instructions.md`**: Breaking down complex features into implementation tasks
  - **Auto-applies to**: `**/*.md,**/specifications/**,**/docs/**`
  - **Use for**: Detailed technical specifications

- **`task-management.instructions.md`**: Task breakdown, validation commands, project workflow
  - **Auto-applies to**: `**/*.md,**/tasks/**,**/project-management/**`
  - **Use for**: Executable task management with validation

### 🚀 Specialized Development Instructions
- **`api-guidelines.instructions.md`**: REST API design, HTTP status codes, validation patterns
  - **Auto-applies to**: `**/*.cs,**/Controllers/**,**/DTOs/**`
  - **Use for**: Backend API development

- **`backend-guidelines.instructions.md`**: Clean architecture, repository patterns, service layer design
  - **Auto-applies to**: `**/*.cs,**/Services/**,**/Repositories/**`
  - **Use for**: Backend architecture implementation

- **`frontend-guidelines.instructions.md`**: Component architecture, state management, UI patterns
  - **Auto-applies to**: `**/*.tsx,**/components/**,**/modules/**`
  - **Use for**: Frontend architecture implementation

- **`react-guidelines.instructions.md`**: React 19 patterns, hooks, performance optimization
  - **Auto-applies to**: `**/*.tsx,**/*.jsx,**/hooks/**`
  - **Use for**: Advanced React development

### � Implementation & Quality Instructions
- **`implementation-guide.instructions.md`**: Dependency-ordered feature implementation, PRP execution
  - **Auto-applies to**: `**/docs/**,**/planning/**`
  - **Use for**: Systematic feature implementation

- **`general-rules.instructions.md`**: Project-wide conventions, GitHub Copilot optimization
  - **Auto-applies to**: `**/*`
  - **Use for**: Cross-cutting concerns and standards

## 🔥 Critical Development Principles

### Single Responsibility Principle (ENFORCED)
Every class, method, and component must have exactly ONE reason to change:

```csharp
// ✅ CORRECT: Focused responsibility
public class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    Task<UserEntity?> GetByIdAsync(Guid id);
    Task<UserEntity> CreateAsync(UserEntity user);
    // ONLY User entity operations
}

// ❌ NEVER: Mixed responsibilities  
public class UserRepository : IUserRepository
{
    Task<UserEntity> CreateAsync(UserEntity user);
    Task<bool> SendEmailAsync(UserEntity user);  // ❌ Email responsibility
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId);  // ❌ Role responsibility
}
```

### Layer Responsibility Matrix
- **Controllers**: HTTP concerns ONLY (request/response, routing, status codes)
- **Services**: Business logic and workflow orchestration ONLY
- **Repositories**: Data access and persistence ONLY  
- **DTOs**: Data transfer between layers ONLY
- **Entities**: Data representation ONLY

### Established Patterns (Must Follow)
```csharp
// Backend: Follow BaseRepository inheritance pattern
public class ExamRepository : BaseRepository<ExamEntity>, IExamRepository
{
    public ExamRepository(IDbConnectionFactory connectionFactory, ILogger<ExamRepository> logger)
        : base(connectionFactory, logger) { }
    
    // Use structured logging with scopes
    using var scope = _logger.BeginScope("Creating exam for user {UserId}", userId);
    
    // Always use async methods for I/O
    public async Task<ExamEntity> CreateAsync(ExamEntity entity)
    {
        // Use parameterized queries with Dapper
        // Include proper error handling
        // Follow existing repository patterns
    }
}
```

```typescript
// Frontend: Follow established patterns
export interface Exam {
  id: string;
  title: string;
  createdAt: Date;
  // Follow TypeScript interface patterns from existing code
}

// Use React Query for server state (already configured)
export const useExams = () => {
  return useQuery({
    queryKey: ['exams'],
    queryFn: () => examService.getAll()
  });
};

// Memoize components for performance (established pattern)
const ExamComponent: React.FC<ExamProps> = memo(({ exam }) => {
  // Support both RTL/LTR layouts (i18n configured)
  // Include loading and error states (established pattern)
  // Use translation keys for all text (i18next configured)
});
```

## 🧪 Validation Workflows (Established)

### Level 1: Syntax & Style Validation
```powershell
# Backend validation (Working)
cd backend
dotnet build --configuration Release
dotnet format --verify-no-changes
dotnet test

# Frontend validation (Working)
cd frontend
pnpm run type-check
pnpm run lint
pnpm run test
```

### Level 2: Runtime Validation
```powershell
# Both services running successfully
pnpm run dev
# Backend: http://localhost:5000 (✅ Verified working)
# Frontend: https://localhost:5173/ (✅ Verified working)
# Health check: http://localhost:5000/api/health/ping (✅ Verified working)
# Swagger: http://localhost:5000/swagger (✅ Verified working)
```

### Level 3: Integration Validation
```powershell
# API Integration Test (Validated pattern)
Invoke-WebRequest -Uri "http://localhost:5000/api/health/ping" -Method GET
# Expected: {"success":true,"message":"API is running successfully",...}
```

## 🏆 Code Generation Guidelines

### Context-Driven Development
1. **Always reference existing patterns** from the established codebase
2. **Use the instruction files** in `.github/instructions/` for specific guidance
3. **Follow the validation loops** provided in each instruction file
4. **Maintain consistency** with established architecture patterns
5. **Include comprehensive error handling** following existing patterns
6. **Document architectural decisions** referencing existing conventions

### Implementation Workflow
```bash
# 1. Reference appropriate instruction file
# Example: For API development, use api-guidelines.instructions.md

# 2. Follow established patterns
# Example: Copy pattern from existing controllers/services/repositories

# 3. Implement with validation
# Example: Include unit tests and validation commands

# 4. Validate implementation
# Example: Run syntax checks, tests, and integration validation
```

## 📁 Project Structure Reference

### Backend Structure (Established)
```
backend/
├── Ikhtibar.API/              # Web API layer (✅ Configured)
│   ├── Controllers/           # API controllers (✅ Working patterns)
│   ├── Data/                  # Database context and schema (✅ Implemented)
│   ├── DTOs/                  # Data transfer objects (✅ Established)
│   ├── Middleware/            # Custom middleware (✅ Error handling implemented)
│   └── Program.cs             # Application configuration (✅ Complete pipeline)
├── Ikhtibar.Core/             # Core business logic (✅ Base patterns)
│   ├── Entities/              # Domain entities (✅ BaseEntity pattern)
│   ├── Repositories/          # Repository interfaces (✅ Generic pattern)
│   └── Services/              # Service interfaces (✅ Ready for implementation)
└── Ikhtibar.Infrastructure/   # Infrastructure layer (✅ Data access)
    ├── Repositories/          # Repository implementations (✅ Dapper pattern)
    └── Services/              # Service implementations (✅ Ready for extension)
```

### Frontend Structure (Established)
```
frontend/
├── src/
│   ├── modules/               # Feature modules (✅ Folder-per-feature)
│   │   ├── auth/              # Authentication module (✅ Implemented)
│   │   └── [feature]/         # Feature-specific modules (✅ Pattern established)
│   ├── shared/                # Shared components and utilities (✅ Configured)
│   │   ├── components/        # Reusable UI components (✅ Base components)
│   │   ├── services/          # API services (✅ apiClient implemented)
│   │   ├── store/             # Global state management (✅ Zustand configured)
│   │   ├── types/             # TypeScript type definitions (✅ API types)
│   │   └── utils/             # Utility functions (✅ Helper functions)
│   ├── routes/                # Application routing (✅ Router configured)
│   └── layout/                # Layout components (✅ Base layout)
```

## 🚨 Anti-Patterns to Avoid

### Backend Anti-Patterns
```csharp
// ❌ DON'T: Mix business logic in controllers
// ❌ DON'T: Use sync methods for I/O operations
// ❌ DON'T: Catch generic exceptions without specific handling
// ❌ DON'T: Hardcode configuration values
// ❌ DON'T: Skip input validation
// ❌ DON'T: Ignore the BaseRepository pattern
// ❌ DON'T: Create repositories without interfaces
```

### Frontend Anti-Patterns
```typescript
// ❌ DON'T: Create components without proper TypeScript typing
// ❌ DON'T: Forget to handle loading and error states
// ❌ DON'T: Use useEffect without proper cleanup
// ❌ DON'T: Skip memoization for expensive operations
// ❌ DON'T: Ignore accessibility requirements
// ❌ DON'T: Hardcode text strings (use i18n)
// ❌ DON'T: Skip the established API client patterns
```

## 🔧 Development Environment (Configured)

### Required Tools (Verified Working)
- ✅ .NET 8.0 SDK
- ✅ Node.js 18+
- ✅ pnpm package manager
- ✅ SQL Server LocalDB
- ✅ Visual Studio Code with Copilot

### Available Commands (Tested)
```powershell
# Start development environment
pnpm run dev  # Both backend and frontend (✅ Working)

# Backend only
cd backend && dotnet run --project Ikhtibar.API  # (✅ Working)

# Frontend only
cd frontend && pnpm start  # (✅ Working)

# Database operations
cd backend && dotnet ef migrations add [Name]  # (✅ Ready)
cd backend && dotnet ef database update  # (✅ Ready)
```

## 📚 Quick Reference

### Manual Instruction Access
When you need specific guidance, reference these instruction files:
- General development: Read `.github/instructions/general-features.instructions.md`
- TypeScript/React: Read `.github/instructions/typescript-features.instructions.md`
- API development: Read `.github/instructions/api-guidelines.instructions.md`
- Backend architecture: Read `.github/instructions/backend-guidelines.instructions.md`
- Frontend guidelines: Read `.github/instructions/frontend-guidelines.instructions.md`
- Planning: Read `.github/instructions/planning-prd.instructions.md`
- Task management: Read `.github/instructions/task-management.instructions.md`

### Established File Patterns
```csharp
// Backend entity example (Follow BaseEntity pattern)
public class ExamEntity : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid CreatedByUserId { get; set; }
    
    // Navigation properties
    public UserEntity CreatedByUser { get; set; } = null!;
    public ICollection<QuestionEntity> Questions { get; set; } = new List<QuestionEntity>();
}
```

```typescript
// Frontend component example (Follow established patterns)
interface ExamFormProps {
  exam?: Exam;
  onSubmit: (exam: CreateExamDto) => Promise<void>;
  onCancel: () => void;
}

const ExamForm: React.FC<ExamFormProps> = memo(({ exam, onSubmit, onCancel }) => {
  const { t } = useTranslation('exam-management');
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  // Follow established form patterns
  const handleSubmit = useCallback(async (data: CreateExamDto) => {
    try {
      setIsSubmitting(true);
      await onSubmit(data);
    } catch (error) {
      // Use established error handling
    } finally {
      setIsSubmitting(false);
    }
  }, [onSubmit]);
  
  return (
    // Follow established UI patterns with Tailwind CSS
    // Include loading states, error handling, and accessibility
    // Support RTL/LTR layouts
  );
});
```

## Activation Instructions

1. **Enable instruction files** in VS Code settings:
   ```json
   "github.copilot.chat.codeGeneration.useInstructionFiles": true
   ```

2. **File-based auto-application**: Instructions automatically apply based on file types and patterns

3. **Manual reference**: Use the specialized instruction files in `.github/instructions/` for specific guidance

## 🎯 Success Metrics

Your code generation is successful when:
- ✅ Follows established patterns from existing codebase
- ✅ Includes comprehensive validation commands
- ✅ Passes all syntax and style checks
- ✅ Integrates seamlessly with existing architecture
- ✅ Includes proper error handling and logging
- ✅ Supports internationalization requirements
- ✅ Maintains single responsibility principle
- ✅ Includes unit tests and integration validation

## 🚀 Next Development Phases

The foundation is complete. Future development should focus on:
1. **Feature Implementation**: Using the established patterns for new features
2. **Business Logic**: Implementing domain-specific requirements
3. **UI/UX Enhancement**: Building on the frontend foundation
4. **Performance Optimization**: Following established performance patterns
5. **Security Hardening**: Extending the authentication and authorization framework

---

*This comprehensive guide reflects the complete foundation established through PRPs 01-04 and provides context-rich guidance for all future development on the Ikhtibar educational exam management system.*
