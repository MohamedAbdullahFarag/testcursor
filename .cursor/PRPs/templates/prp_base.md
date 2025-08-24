name: "Base PRP Template v2 - Context-Rich with Validation Loops"
description: |

## Purpose

Template optimized for AI agents to implement features with sufficient context and self-validation capabilities to achieve working code through iterative refinement.

## Core Principles

1. **Context is King**: Include ALL necessary documentation, examples, and caveats
2. **Validation Loops**: Provide executable tests/lints the AI can run and fix
3. **Information Dense**: Use keywords and patterns from the codebase
4. **Progressive Success**: Start simple, validate, then enhance

---

## Goal

[What needs to be built - be specific about the end state and desires]

## Why

- [Business value and user impact]
- [Integration with existing features]
- [Problems this solves and for whom]

## What

[User-visible behavior and technical requirements]

### Success Criteria

- [ ] [Specific measurable outcomes]

## All Needed Context

### Documentation & References (list all context needed to implement the feature)

```yaml
# MUST READ - Include these in your context window
- url: [Official API docs URL]
  why: [Specific sections/methods you'll need]

- file: [path/to/existing/Controller.cs]
  why: [Pattern to follow, gotchas to avoid]

- file: [path/to/frontend/components/ExistingComponent.tsx]
  why: [React component pattern to follow]

- doc: [Library documentation URL]
  section: [Specific section about common pitfalls]
  critical: [Key insight that prevents common errors]

- docfile: [PRPs/ai_docs/file.md]
  why: [docs that the user has pasted in to the project]
```

### Current Codebase tree (run `dir /s /b` in the root of the project) to get an overview of the codebase

```bash
# Sample backend structure
backend/src/Features/
backend/src/Features/Auth/
backend/src/Features/Auth/Controllers/
backend/src/Features/Auth/Services/
backend/src/Features/Auth/Repositories/
backend/src/Features/Auth/Models/
backend/src/Features/Auth/DTOs/

# Sample frontend structure
frontend/src/modules/
frontend/src/modules/auth/
frontend/src/modules/auth/components/
frontend/src/modules/auth/hooks/
frontend/src/modules/auth/services/
frontend/src/modules/auth/models/
```

### Desired Codebase tree with files to be added and responsibility of file

```bash
# New Backend Files
backend/src/Features/[FeatureName]/
backend/src/Features/[FeatureName]/Controllers/[FeatureName]Controller.cs - API endpoints
backend/src/Features/[FeatureName]/Services/[FeatureName]Service.cs - Business logic
backend/src/Features/[FeatureName]/Services/I[FeatureName]Service.cs - Service interface
backend/src/Features/[FeatureName]/Repositories/[FeatureName]Repository.cs - Data access
backend/src/Features/[FeatureName]/Repositories/I[FeatureName]Repository.cs - Repository interface
backend/src/Features/[FeatureName]/Models/[FeatureName]Entity.cs - Database entity
backend/src/Features/[FeatureName]/DTOs/[FeatureName]Dto.cs - Data transfer objects
backend/src/Features/[FeatureName]/DTOs/Create[FeatureName]Request.cs - Create request DTO
backend/src/Features/[FeatureName]/DTOs/Update[FeatureName]Request.cs - Update request DTO

# New Frontend Files
frontend/src/modules/[featureName]/
frontend/src/modules/[featureName]/components/[FeatureName]Form.tsx - Form component
frontend/src/modules/[featureName]/components/[FeatureName]List.tsx - List component
frontend/src/modules/[featureName]/hooks/use[FeatureName].ts - Custom hook
frontend/src/modules/[featureName]/services/[featureName]Service.ts - API service
frontend/src/modules/[featureName]/models/[featureName].types.ts - TypeScript interfaces
frontend/src/modules/[featureName]/views/[FeatureName]View.tsx - Main view component
frontend/src/modules/[featureName]/locales/en.ts - English translations
frontend/src/modules/[featureName]/locales/ar.ts - Arabic translations
```

### Known Gotchas of our codebase & Library Quirks

```csharp
// CRITICAL: ASP.NET Core Controllers must follow SRP (Single Responsibility Principle)
// CRITICAL: Always use async/await for I/O operations in services and repositories
// CRITICAL: Repository pattern must use proper parameterized queries with Dapper
// CRITICAL: Services must handle all business logic - never in controllers
// CRITICAL: Use proper exception handling with try/catch and ILogger
// CRITICAL: All DTOs must have proper validation attributes

// Frontend:
// CRITICAL: React components must use memo() for optimization
// CRITICAL: Always clean up effects with return function
// CRITICAL: Use proper TypeScript typing for all props and state
// CRITICAL: Support both Arabic (RTL) and English (LTR) in all components
// CRITICAL: Always handle loading and error states in components
```

## Implementation Blueprint

### Data models and structure

Create the core data models to ensure type safety and consistency.

```csharp
// Backend Entity Model
public class ExampleEntity : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public int CategoryId { get; set; }
    public virtual CategoryEntity Category { get; set; }
}

// Backend DTO
public class ExampleDto
{
    public int id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public CategoryDto Category { get; set; }
}

// Backend Request DTO with validation
public class CreateExampleRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
    public string Name { get; set; }
    
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; }
    
    [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "Category is required")]
    public int CategoryId { get; set; }
}
```

```typescript
// Frontend TypeScript interfaces
export interface Example {
  id: string;
  name: string;
  description: string | null;
  price: number;
  isActive: boolean;
  createdAt: string;
  category: Category;
}

export interface Category {
  id: string;
  name: string;
}

export interface CreateExampleRequest {
  name: string;
  description?: string;
  price: number;
  categoryId: string;
}

export interface UpdateExampleRequest {
  name?: string;
  description?: string;
  price?: number;
  isActive?: boolean;
  categoryId?: string;
}
```

### List of tasks to be completed to fulfill the PRP in the order they should be completed

```yaml
# Backend Tasks
Task 1 - Create Entity and Context:
CREATE backend/src/Features/Example/Models/ExampleEntity.cs:
  - MIRROR pattern from: backend/src/Features/Auth/Models/UserEntity.cs
  - ADD required properties and data annotations
  - IMPLEMENT proper relationships

Task 2 - Create DTOs:
CREATE backend/src/Features/Example/DTOs/ExampleDto.cs:
  - IMPLEMENT data transfer object for API responses
  - ENSURE all properties mapped from entity

CREATE backend/src/Features/Example/DTOs/CreateExampleRequest.cs:
  - ADD validation attributes for all properties
  - FOLLOW API contract standards

CREATE backend/src/Features/Example/DTOs/UpdateExampleRequest.cs:
  - MAKE properties nullable for partial updates
  - ADD validation attributes

Task 3 - Repository Layer:
CREATE backend/src/Features/Example/Repositories/IExampleRepository.cs:
  - DECLARE async CRUD operations
  - FOLLOW repository interface pattern

CREATE backend/src/Features/Example/Repositories/ExampleRepository.cs:
  - IMPLEMENT repository with Dapper
  - ADD parameterized queries
  - HANDLE database exceptions properly

Task 4 - Service Layer:
CREATE backend/src/Features/Example/Services/IExampleService.cs:
  - DECLARE business logic methods
  - USE DTOs for parameters and return types

CREATE backend/src/Features/Example/Services/ExampleService.cs:
  - IMPLEMENT business logic
  - ADD validation logic
  - USE proper exception handling
  - IMPLEMENT logging

Task 5 - Controller:
CREATE backend/src/Features/Example/Controllers/ExampleController.cs:
  - IMPLEMENT REST endpoints
  - ADD authorization attributes
  - USE proper HTTP status codes
  - ADD Swagger documentation

# Frontend Tasks
Task 6 - TypeScript Models:
CREATE frontend/src/modules/example/models/example.types.ts:
  - DEFINE TypeScript interfaces
  - ENSURE alignment with backend DTOs

Task 7 - API Service:
CREATE frontend/src/modules/example/services/exampleService.ts:
  - IMPLEMENT API integration
  - ADD error handling
  - USE axios interceptors

Task 8 - React Hooks:
CREATE frontend/src/modules/example/hooks/useExample.ts:
  - IMPLEMENT data fetching
  - ADD loading and error states
  - HANDLE component cleanup

Task 9 - React Components:
CREATE frontend/src/modules/example/components/ExampleForm.tsx:
  - IMPLEMENT form with validation
  - HANDLE both create and update

CREATE frontend/src/modules/example/components/ExampleList.tsx:
  - IMPLEMENT data listing with pagination
  - ADD sorting and filtering

Task 10 - View Component:
CREATE frontend/src/modules/example/views/ExampleView.tsx:
  - COMBINE components
  - ADD routing logic

Task 11 - Internationalization:
CREATE frontend/src/modules/example/locales/en.ts:
  - ADD English translations

CREATE frontend/src/modules/example/locales/ar.ts:
  - ADD Arabic translations
  - ENSURE proper RTL support
```

### Per task pseudocode as needed added to each task

```python

# Task 1
# Pseudocode with CRITICAL details don't write entire code
async def new_feature(param: str) -> Result:
    # PATTERN: Always validate input first (see src/validators.py)
    validated = validate_input(param)  # raises ValidationError

    # GOTCHA: This library requires connection pooling
    async with get_connection() as conn:  # see src/db/pool.py
        # PATTERN: Use existing retry decorator
        @retry(attempts=3, backoff=exponential)
        async def _inner():
            # CRITICAL: API returns 429 if >10 req/sec
            await rate_limiter.acquire()
            return await external_api.call(validated)

        result = await _inner()

    # PATTERN: Standardized response format
    return format_response(result)  # see src/utils/responses.py
```

### Integration Points

```yaml
DATABASE:
  - migration: "CREATE TABLE Examples (Id UNIQUEIDENTIFIER PRIMARY KEY, Name NVARCHAR(100) NOT NULL, Description NVARCHAR(MAX), Price DECIMAL(18,2) NOT NULL, IsActive BIT NOT NULL, CreatedAt DATETIME2 NOT NULL, CategoryId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Categories(Id))"
  - index: "CREATE INDEX IX_Examples_CategoryId ON Examples(CategoryId)"

CONFIG:
  - add to: backend/appsettings.json
  - pattern: "\"ExampleSettings\": { \"PageSize\": 20, \"CacheDurationMinutes\": 5 }"

  - add to: frontend/src/shared/constants/endpoints.ts
  - pattern: "export const EXAMPLE_API = `${API_BASE_URL}/api/examples`;"

ROUTES:
  - add to: backend/Startup.cs
  - pattern: "services.AddScoped<IExampleService, ExampleService>();"
  - pattern: "services.AddScoped<IExampleRepository, ExampleRepository>();"

  - add to: frontend/src/routes/index.tsx
  - pattern: "{ path: '/examples', element: <ExampleView /> }"

PERMISSIONS:
  - add to: backend/src/Constants/Permissions.cs
  - pattern: "public const string ViewExamples = \"examples:view\";"
  - pattern: "public const string ManageExamples = \"examples:manage\";"

NAVIGATION:
  - add to: frontend/src/layout/DashboardLayout/DashboardSideBar.tsx
  - pattern: "{ title: t('examples.navigation'), path: '/examples', icon: <ListIcon /> }"

TRANSLATIONS:
  - add to: frontend/src/shared/locales/en.ts and ar.ts
  - pattern: "examples: { title: 'Examples', create: 'Create New', edit: 'Edit', delete: 'Delete' }"
```

## Validation Loop

### Level 1: Syntax & Style

```bash
# Backend validation
dotnet build backend/src/Features/Example/
dotnet format --verify-no-changes
dotnet format --severity error

# Frontend validation
cd frontend
npm run lint
npm run type-check

# Expected: No errors. If errors, READ the error and fix.
```

### Level 2: Unit Tests for each component

```csharp
// Backend Unit Tests (C#)
[TestFixture]
public class ExampleServiceTests
{
    private IExampleService _service;
    private Mock<IExampleRepository> _repositoryMock;
    private Mock<ILogger<ExampleService>> _loggerMock;
    
    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IExampleRepository>();
        _loggerMock = new Mock<ILogger<ExampleService>>();
        _service = new ExampleService(_repositoryMock.Object, _loggerMock.Object);
    }
    
    [Test]
    public async Task GetByIdAsync_WhenExampleExists_ReturnsExample()
    {
        // Arrange
        var id = int.NewGuid();
        var entity = new ExampleEntity { Id = id, Name = "Test" };
        _repositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(entity);
        
        // Act
        var result = await _service.GetByIdAsync(id);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(id));
    }
    
    [Test]
    public async Task GetByIdAsync_WhenExampleDoesNotExist_ReturnsNull()
    {
        // Arrange
        var id = int.NewGuid();
        _repositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((ExampleEntity)null);
        
        // Act
        var result = await _service.GetByIdAsync(id);
        
        // Assert
        Assert.That(result, Is.Null);
    }
}
```

```typescript
// Frontend Unit Tests (TypeScript/React)
import { renderHook, act } from '@testing-library/react-hooks';
import { useExample } from '../hooks/useExample';
import * as exampleService from '../services/exampleService';

jest.mock('../services/exampleService');

describe('useExample hook', () => {
  it('should fetch examples on mount', async () => {
    // Arrange
    const mockData = [{ id: '1', name: 'Test Example' }];
    jest.spyOn(exampleService, 'getExamples').mockResolvedValue(mockData);
    
    // Act
    const { result, waitForNextUpdate } = renderHook(() => useExample());
    
    // Initial state
    expect(result.current.isLoading).toBe(true);
    expect(result.current.examples).toEqual([]);
    
    // Wait for the hook to update
    await waitForNextUpdate();
    
    // Assert
    expect(result.current.isLoading).toBe(false);
    expect(result.current.examples).toEqual(mockData);
    expect(result.current.error).toBeNull();
  });
  
  it('should handle errors when fetching examples fails', async () => {
    // Arrange
    const mockError = new Error('Failed to fetch');
    jest.spyOn(exampleService, 'getExamples').mockRejectedValue(mockError);
    
    // Act
    const { result, waitForNextUpdate } = renderHook(() => useExample());
    await waitForNextUpdate();
    
    // Assert
    expect(result.current.isLoading).toBe(false);
    expect(result.current.examples).toEqual([]);
    expect(result.current.error).toBe('Failed to fetch examples');
  });
});
```

```bash
# Run backend tests
cd backend
dotnet test tests/Features/Example.Tests/

# Run frontend tests
cd frontend
npm test -- --watchAll=false --testPathPattern=src/modules/example
```

### Level 3: Integration Tests

```bash
# Backend API Tests
dotnet test tests/Api.IntegrationTests/ExampleControllerTests.cs

# Frontend E2E Tests (with Cypress)
cd frontend
npm run cypress:run -- --spec "cypress/integration/example/**/*.spec.js"

# Manual API Testing
curl -X GET https://localhost:7001/api/examples \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json"

# Expected: HTTP 200 with JSON array response
# If error: Check logs in backend/logs/
```

### Level 4: Cross-Browser & Internationalization Validation

```bash
# Test in multiple browsers
npm run cypress:run -- --browser chrome,firefox,edge

# Test with Arabic language setting
REACT_APP_LANGUAGE=ar npm start

# Verify RTL layout renders correctly
# Verify Arabic translations display properly
# Verify form validation works with Arabic text

# Accessibility testing
npm run axe -- --tags wcag21a,wcag21aa
```

## Final Validation Checklist

### Backend
- [ ] All backend code builds: `dotnet build backend/`
- [ ] All backend tests pass: `dotnet test backend/tests/`
- [ ] No linting errors: `dotnet format --verify-no-changes`
- [ ] All API endpoints return expected status codes
- [ ] Proper validation for all DTOs
- [ ] Error responses include meaningful messages
- [ ] Logging implemented at appropriate levels
- [ ] Proper authorization checks implemented
- [ ] SRP (Single Responsibility Principle) followed

### Frontend
- [ ] All TypeScript code compiles: `npm run type-check`
- [ ] No linting errors: `npm run lint`
- [ ] All component tests pass: `npm test`
- [ ] Components handle loading states
- [ ] Components handle error states
- [ ] Form validation working properly
- [ ] English translations complete
- [ ] Arabic translations complete
- [ ] RTL layout rendering correctly
- [ ] Responsive design working on all breakpoints

### Integration
- [ ] API integration tests pass
- [ ] Frontend can communicate with backend
- [ ] Authentication/authorization working
- [ ] Database migrations applied successfully
- [ ] Routes registered and navigable
- [ ] Error handling between layers working

### Documentation
- [ ] API endpoints documented in Swagger
- [ ] Component usage examples provided
- [ ] Required environment variables documented
- [ ] Integration points documented
- [ ] Known limitations documented

---

## Anti-Patterns to Avoid

### Backend Anti-Patterns
- ❌ **Don't mix responsibilities**: Keep controllers thin, services for business logic, repositories for data access
- ❌ **Don't use sync methods for I/O**: Always use async/await for database and external calls
- ❌ **Don't catch generic exceptions**: Be specific about which exceptions to catch
- ❌ **Don't return domain entities from API**: Always map to DTOs
- ❌ **Don't skip input validation**: Use data annotations or fluent validation
- ❌ **Don't hardcode configuration**: Use IOptions pattern and appsettings.json
- ❌ **Don't use magic strings**: Use constants for resource paths, claim types, etc.

### Frontend Anti-Patterns
- ❌ **Don't skip memoization**: Use React.memo and useCallback for optimization
- ❌ **Don't forget cleanup in useEffect**: Return cleanup function to prevent memory leaks
- ❌ **Don't use 'any' type**: Provide proper TypeScript interfaces for all data
- ❌ **Don't skip loading/error states**: Always handle these states in components
- ❌ **Don't put business logic in components**: Move to custom hooks or services
- ❌ **Don't mix i18n string literals**: Use translation keys consistently
- ❌ **Don't forget accessibility**: Use proper ARIA attributes and semantic HTML
