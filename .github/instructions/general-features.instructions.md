---
description: "General feature implementation with context-rich development and validation loops"
applyTo: "**/*.cs,**/*.js,**/*.ts,**/*.tsx,**/*.json"
---

# Feature Implementation Instructions

You are implementing features in the Ikhtibar educational exam management system - a full-stack application with ASP.NET Core 8.0 backend and React.js 18 frontend.

## Core Development Principles

### Context is King
- Include ALL necessary documentation, examples, and caveats
- Reference existing patterns from the codebase before creating new ones
- Use descriptive variable names and function signatures
- Provide comprehensive examples and patterns

### Validation Loops
- Provide executable tests/lints that can be run and fixed
- Progressive enhancement through iterative refinement
- Self-documenting code with clear intent
- Include anti-pattern warnings embedded in comments

### Information Dense
- Use keywords and patterns from the existing codebase
- Follow established naming conventions consistently
- Maintain architectural consistency
- Consider internationalization (English/Arabic support)

## Architecture Overview

### Backend (ASP.NET Core 8.0)
- **Architecture**: Clean Architecture with folder-per-feature
- **ORM**: Dapper (micro-ORM)
- **Database**: SQL Server
- **Authentication**: JWT Bearer tokens
- **Pattern**: Repository Pattern with Services

### Frontend (React.js 18)
- **Language**: TypeScript with strict mode
- **Build Tool**: Vite
- **Styling**: Tailwind CSS
- **State Management**: React Query + Zustand
- **Internationalization**: i18next (Arabic/English)

## Implementation Blueprint

### Backend Implementation Order

1. **Data Models First**
```csharp
// Entity (Database representation)
public class FeatureEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    // Follow BaseEntity inheritance pattern
}

// DTOs (Data Transfer Objects)
public class FeatureDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateFeatureRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

2. **Repository Pattern**
```csharp
public interface IFeatureRepository
{
    Task<IEnumerable<FeatureEntity>> GetAllAsync();
    Task<FeatureEntity?> GetByIdAsync(Guid id);
    Task<FeatureEntity> CreateAsync(FeatureEntity entity);
    Task<FeatureEntity> UpdateAsync(FeatureEntity entity);
    Task DeleteAsync(Guid id);
}

public class FeatureRepository : BaseRepository<FeatureEntity>, IFeatureRepository
{
    public FeatureRepository(IDbConnectionFactory connectionFactory) 
        : base(connectionFactory)
    {
    }
    
    // Implement specific methods with Dapper
    // Use parameterized queries for security
    // Include proper error handling
}
```

3. **Service Layer**
```csharp
public interface IFeatureService
{
    Task<IEnumerable<FeatureDto>> GetAllAsync();
    Task<FeatureDto?> GetByIdAsync(Guid id);
    Task<FeatureDto> CreateAsync(CreateFeatureRequest request);
    Task<FeatureDto> UpdateAsync(Guid id, UpdateFeatureRequest request);
    Task DeleteAsync(Guid id);
}

public class FeatureService : IFeatureService
{
    private readonly IFeatureRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<FeatureService> _logger;
    
    // Implement business logic
    // Use structured logging with scopes
    // Include proper validation and error handling
}
```

4. **Controller Layer**
```csharp
[ApiController]
[Route("api/[controller]")]
public class FeaturesController : ControllerBase
{
    private readonly IFeatureService _service;
    private readonly ILogger<FeaturesController> _logger;
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FeatureDto>>> GetAll()
    {
        try
        {
            var features = await _service.GetAllAsync();
            return Ok(features);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving features");
            return StatusCode(500, "Internal server error");
        }
    }
    
    // Follow REST conventions
    // Include proper HTTP status codes
    // Implement comprehensive error handling
}
```

### Frontend Implementation Order

1. **TypeScript Interfaces**
```typescript
// types/feature.types.ts
export interface Feature {
  id: string;
  name: string;
  description: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface CreateFeatureRequest {
  name: string;
  description?: string;
}

export interface FeatureComponentProps {
  feature: Feature;
  onEdit?: (feature: Feature) => void;
  onDelete?: (id: string) => void;
  className?: string;
}
```

2. **API Service**
```typescript
// services/featureService.ts
import { apiService } from '../../shared/services/apiService';

class FeatureService {
  private readonly baseUrl = '/api/features';

  async getAll(): Promise<Feature[]> {
    return apiService.get<Feature[]>(this.baseUrl);
  }

  async create(data: CreateFeatureRequest): Promise<Feature> {
    return apiService.post<Feature>(this.baseUrl, data);
  }
  
  // Include proper error handling
  // Use consistent response transformation
}

export const featureService = new FeatureService();
```

3. **Custom Hooks**
```typescript
// hooks/useFeatures.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';

export const useFeatures = () => {
  return useQuery({
    queryKey: ['features'],
    queryFn: () => featureService.getAll(),
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
};

export const useCreateFeature = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: featureService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['features'] });
    },
  });
};
```

4. **React Components**
```typescript
// components/FeatureCard.tsx
import React, { memo } from 'react';
import { Feature } from '../types/feature.types';

interface FeatureCardProps {
  feature: Feature;
  onEdit?: (feature: Feature) => void;
  onDelete?: (id: string) => void;
  className?: string;
}

const FeatureCard: React.FC<FeatureCardProps> = memo(({
  feature,
  onEdit,
  onDelete,
  className = ''
}) => {
  const { t } = useTranslation();
  
  // Include loading and error states
  // Support RTL/LTR layouts
  // Follow accessibility guidelines
  // Use translation keys for all text
});

FeatureCard.displayName = 'FeatureCard';
export default FeatureCard;
```

## Folder Structure Requirements

### Backend Structure
```
backend/src/Features/[FeatureName]/
├── Controllers/[FeatureName]Controller.cs
├── Services/I[FeatureName]Service.cs
├── Services/[FeatureName]Service.cs
├── Repositories/I[FeatureName]Repository.cs
├── Repositories/[FeatureName]Repository.cs
├── Models/[FeatureName]Entity.cs
├── DTOs/[FeatureName]Dto.cs
├── DTOs/Create[FeatureName]Request.cs
└── DTOs/Update[FeatureName]Request.cs
```

### Frontend Structure
```
frontend/src/modules/[featureName]/
├── components/         # UI components
├── hooks/             # Custom React hooks
├── services/          # API integration
├── types/             # TypeScript interfaces
├── utils/             # Helper functions
└── views/             # Page components
```

## Validation Commands

### Backend Validation
```bash
# Build and type checking
dotnet build --configuration Release

# Code formatting
dotnet format --verify-no-changes

# Run tests
dotnet test --logger "console;verbosity=detailed"

# Security analysis
dotnet run --configuration Release --project Security.Analysis
```

### Frontend Validation
```bash
# TypeScript validation
npm run type-check

# Linting
npm run lint

# Formatting
npm run format

# Testing
npm run test
npm run test:coverage

# Build verification
npm run build
```

## Anti-Patterns to Avoid

### Backend Anti-Patterns
```csharp
// ❌ DON'T: Mix business logic in controllers
public async Task<ActionResult> Create(CreateRequest request)
{
    // Business logic should be in service layer
    var entity = new Entity { Name = request.Name.ToUpper() };
}

// ❌ DON'T: Use sync methods for I/O operations
public User GetUser(int id) // Should be async

// ❌ DON'T: Catch generic exceptions without specific handling
catch (Exception ex) // Be more specific

// ❌ DON'T: Skip input validation
public async Task Create(CreateRequest request) // Validate first
```

### Frontend Anti-Patterns
```typescript
// ❌ DON'T: Create components without proper typing
function MyComponent(props: any) // Use proper interfaces

// ❌ DON'T: Forget to handle loading and error states
const { data } = useQuery(['key']); // Missing isLoading, error

// ❌ DON'T: Use useEffect without cleanup
useEffect(() => {
  const subscription = subscribe();
  // Missing return () => subscription.unsubscribe();
}, []);

// ❌ DON'T: Skip memoization for expensive operations
const expensiveValue = computeExpensiveValue(data); // Use useMemo

// ❌ DON'T: Ignore internationalization
<h1>Welcome</h1> // Use {t('welcome')}
```

## Success Criteria

### Backend Success Criteria
- [ ] Controllers only handle HTTP concerns
- [ ] Business logic is in service layer
- [ ] Data access is in repository layer
- [ ] All methods are async for I/O operations
- [ ] Proper error handling and logging
- [ ] Input validation is implemented
- [ ] Tests are written and passing
- [ ] Code follows SOLID principles

### Frontend Success Criteria
- [ ] Components are properly typed
- [ ] Loading and error states are handled
- [ ] Custom hooks follow established patterns
- [ ] Internationalization is implemented
- [ ] RTL/LTR layouts are supported
- [ ] Accessibility guidelines are followed
- [ ] Tests are written and passing
- [ ] Code follows React best practices

## Integration Requirements

### Database Integration
- Use Dapper for data access
- Implement parameterized queries
- Include proper connection management
- Handle database exceptions gracefully

### API Integration
- Follow REST conventions
- Use consistent response formats
- Implement proper status codes
- Include comprehensive error handling

### Authentication Integration
- Use JWT Bearer tokens
- Implement role-based access control
- Include proper authorization checks
- Handle authentication errors

### State Management Integration
- Use React Query for server state
- Use Zustand for client state
- Implement optimistic updates
- Handle state synchronization

Always reference existing patterns in the codebase and follow the established architectural conventions. When implementing new features, ensure they integrate seamlessly with existing functionality and maintain consistency across the application.
