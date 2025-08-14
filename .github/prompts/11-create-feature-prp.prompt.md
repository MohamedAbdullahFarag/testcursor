---
mode: agent
description: "Create comprehensive Product Requirements Prompt (PRP) for new features with full-stack implementation guidance and validation frameworks"
---

---
inputs:
  - name: feature_type
    description: Type of feature to create (component, page, api, service, module)
    required: true
  - name: feature_name
    description: Name of the feature to create
    required: true
  - name: scope
    description: Feature scope (frontend, backend, full-stack)
    required: false
    default: full-stack
  - name: complexity
    description: Feature complexity level (simple, moderate, complex)
    required: false
    default: moderate
---

---
command: "/create-feature-prp"
---
# Create Feature PRP Command for GitHub Copilot

## Command Usage
```
@copilot /create-feature-prp <feature_type> <feature_name> [scope] [complexity]
```

## Purpose
This command creates comprehensive Project Requirement & Planning (PRP) documents for new features in the Ikhtibar examination management system, ensuring proper planning, architecture design, and implementation roadmap.

**Input Parameters**: 
- `feature_type` - Type of feature: `component`, `page`, `api`, `service`, or `module`
- `feature_name` - Name of the feature to create
- `scope` - Development scope: `frontend`, `backend`, or `full-stack`
- `complexity` - Complexity level: `simple`, `moderate`, or `complex`

## How /create-feature-prp Works

### Phase 1: Feature Analysis and Requirements Gathering
```markdown
I'll help you create a comprehensive PRP document for the new feature in the Ikhtibar project. Let me analyze the requirements and plan the implementation.

**Phase 1.1: Parse Feature Request**
```
Feature PRP Creation:
- **Feature Type**: [COMPONENT/PAGE/API/SERVICE/MODULE]
- **Feature Name**: [FEATURE_NAME]
- **Scope**: [FRONTEND/BACKEND/FULL-STACK]
- **Complexity**: [SIMPLE/MODERATE/COMPLEX]
- **Project Context**: React 18 + .NET Core examination management system
- **Architecture**: Clean Architecture with domain-driven design
```

**Phase 1.2: Context Discovery using GitHub Copilot Tools**
```
I'll analyze the existing codebase to understand patterns and architecture:
- semantic_search: "[FEATURE_TYPE] patterns" # Find existing similar features
- file_search: "src/modules/**/*.tsx" # Analyze frontend structure
- file_search: "backend/**/*.cs" # Analyze backend structure
- grep_search: "interface.*Props|class.*Controller" # Find patterns
- read_file: [SIMILAR_FEATURES] # Study existing implementations
- semantic_search: "architecture patterns" # Understand project architecture
```

**Phase 1.3: Requirements Analysis**
```
Feature Requirements Analysis:
- [ ] **Business Requirements**: [BUSINESS_VALUE_ANALYSIS]
- [ ] **Technical Requirements**: [TECHNICAL_SPECIFICATIONS]
- [ ] **User Experience Requirements**: [UX_CONSIDERATIONS]
- [ ] **Performance Requirements**: [PERFORMANCE_CRITERIA]
- [ ] **Security Requirements**: [SECURITY_CONSIDERATIONS]
- [ ] **Integration Requirements**: [INTEGRATION_POINTS]
- [ ] **Testing Requirements**: [TESTING_STRATEGY]
- [ ] **Documentation Requirements**: [DOCUMENTATION_NEEDS]
```
```

### Phase 2: Architecture Design and Planning

#### For Feature Type: `component`
```markdown
**Phase 2.1: Component Architecture Design using GitHub Copilot Tools**
```
I'll design the component architecture:

## 🏗️ Component Architecture Design (Tool-Enhanced)

### Component Analysis and Design
```powershell
# Component architecture analysis using GitHub Copilot tools
semantic_search: "React component architecture" # Find component patterns
file_search: "src/modules/**/components/*.tsx" # Study existing components
grep_search: "interface.*Props" # Analyze prop patterns
semantic_search: "component composition patterns" # Find composition strategies
read_file: [SIMILAR_COMPONENTS] # Study similar component implementations
```

### Component Specification (Tool-Designed)
```typescript
// Component Design Specification
interface [FeatureName]ComponentProps {
  // Props interface based on analysis of existing patterns
  data: [DataType]; // Determined from domain analysis
  loading?: boolean; // Standard loading pattern
  error?: Error | null; // Standard error pattern
  onAction?: (action: ActionType) => void; // Event handling pattern
  className?: string; // Style customization
  children?: React.ReactNode; // Composition support
  // Additional props based on complexity and requirements
}

// Component implementation strategy
const [FeatureName]Component: React.FC<[FeatureName]ComponentProps> = ({
  data,
  loading = false,
  error = null,
  onAction,
  className,
  children
}) => {
  // Component implementation following project patterns
};

// Component variations and extensions
interface [FeatureName]ComponentVariants {
  size: 'small' | 'medium' | 'large';
  variant: 'primary' | 'secondary' | 'outline';
  state: 'default' | 'loading' | 'error' | 'success';
}
```

### Component Architecture Plan
```
Component Development Plan:
┌─────────────────────────────────────────────────────────────┐
│ Component: [FeatureName]Component                           │
├─────────────────────────────────────────────────────────────┤
│ Structure:                                                  │
│ • Base Component: Core functionality                       │
│ • Sub-components: Modular pieces                          │
│ • Hooks: Custom hooks for logic                           │
│ • Types: TypeScript interfaces                            │
│ • Styles: Tailwind CSS classes                           │
│ • Tests: Unit and integration tests                       │
├─────────────────────────────────────────────────────────────┤
│ Dependencies:                                               │
│ • External: [EXTERNAL_DEPENDENCIES]                       │
│ • Internal: [INTERNAL_DEPENDENCIES]                       │
│ • Services: [SERVICE_DEPENDENCIES]                        │
│ • State: [STATE_MANAGEMENT]                               │
├─────────────────────────────────────────────────────────────┤
│ Implementation Phases:                                      │
│ 1. Core component structure                                │
│ 2. Props interface and TypeScript types                   │
│ 3. Basic functionality implementation                      │
│ 4. Styling and responsive design                          │
│ 5. Error handling and loading states                      │
│ 6. Integration with existing systems                      │
│ 7. Testing and validation                                  │
│ 8. Documentation and examples                             │
└─────────────────────────────────────────────────────────────┘
```

### Component Integration Strategy
```typescript
// Integration with existing Ikhtibar patterns

// 1. State Management Integration
interface ComponentState {
  data: [DataType];
  loading: boolean;
  error: Error | null;
}

const use[FeatureName] = (): ComponentState => {
  // Custom hook following project patterns
  // Integrates with existing state management
};

// 2. API Integration
const [FeatureName]Service = {
  get: (id: string): Promise<[DataType]> => {
    // API service following repository pattern
  },
  create: (data: Create[DataType]Request): Promise<[DataType]> => {
    // Follows existing API patterns
  },
  update: (id: string, data: Update[DataType]Request): Promise<[DataType]> => {
    // Consistent with other services
  }
};

// 3. Form Integration
interface [FeatureName]FormData {
  // Form data structure based on requirements
}

const [FeatureName]Form: React.FC = () => {
  // Form component using project's form patterns
  // Integrates with validation and submission logic
};
```
```

#### For Feature Type: `api`
```markdown
**Phase 2.2: API Architecture Design using GitHub Copilot Tools**
```
I'll design the API architecture:

## 🔌 API Architecture Design (Tool-Enhanced)

### API Analysis and Design
```powershell
# API architecture analysis using GitHub Copilot tools
semantic_search: "API controller patterns" # Find controller patterns
file_search: "backend/**/*Controller.cs" # Study existing controllers
grep_search: "public.*ActionResult" # Analyze action patterns
semantic_search: "service layer patterns" # Find service patterns
read_file: [SIMILAR_CONTROLLERS] # Study similar API implementations
semantic_search: "data transfer object patterns" # Find DTO patterns
```

### API Specification (Tool-Designed)
```csharp
// API Design Specification based on Clean Architecture

// 1. Controller Design
[ApiController]
[Route("api/[controller]")]
public class [FeatureName]Controller : ControllerBase
{
    private readonly I[FeatureName]Service _featureService;
    private readonly ILogger<[FeatureName]Controller> _logger;

    public [FeatureName]Controller(
        I[FeatureName]Service featureService,
        ILogger<[FeatureName]Controller> logger)
    {
        _featureService = featureService;
        _logger = logger;
    }

    // GET endpoint following project patterns
    [HttpGet]
    public async Task<ActionResult<IEnumerable<[FeatureName]Dto>>> GetAll(
        [FromQuery] [FeatureName]QueryParameters queryParams)
    {
        // Implementation following existing patterns
    }

    // POST endpoint with validation
    [HttpPost]
    public async Task<ActionResult<[FeatureName]Dto>> Create(
        [FromBody] Create[FeatureName]Dto createDto)
    {
        // Implementation with proper error handling
    }

    // PUT endpoint for updates
    [HttpPut("{id}")]
    public async Task<ActionResult<[FeatureName]Dto>> Update(
        int id, 
        [FromBody] Update[FeatureName]Dto updateDto)
    {
        // Implementation following REST conventions
    }

    // DELETE endpoint
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        // Soft delete implementation
    }
}

// 2. Service Layer Design
public interface I[FeatureName]Service
{
    Task<IEnumerable<[FeatureName]Dto>> GetAllAsync([FeatureName]QueryParameters queryParams);
    Task<[FeatureName]Dto?> GetByIdAsync(int id);
    Task<[FeatureName]Dto> CreateAsync(Create[FeatureName]Dto createDto);
    Task<[FeatureName]Dto> UpdateAsync(int id, Update[FeatureName]Dto updateDto);
    Task<bool> DeleteAsync(int id);
}

public class [FeatureName]Service : I[FeatureName]Service
{
    private readonly I[FeatureName]Repository _repository;
    private readonly IMapper _mapper;

    // Service implementation following project patterns
}

// 3. Data Transfer Objects
public class [FeatureName]Dto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    // Additional properties based on requirements
}

public class Create[FeatureName]Dto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    // Create-specific properties
}

public class Update[FeatureName]Dto
{
    [StringLength(100)]
    public string? Name { get; set; }
    // Update-specific properties
}
```

### API Development Plan
```
API Development Plan:
┌─────────────────────────────────────────────────────────────┐
│ API: [FeatureName] REST API                                 │
├─────────────────────────────────────────────────────────────┤
│ Endpoints:                                                  │
│ • GET /api/[feature-name] - List with pagination          │
│ • GET /api/[feature-name]/{id} - Get by ID                │
│ • POST /api/[feature-name] - Create new                   │
│ • PUT /api/[feature-name]/{id} - Update existing          │
│ • DELETE /api/[feature-name]/{id} - Soft delete           │
├─────────────────────────────────────────────────────────────┤
│ Architecture Layers:                                        │
│ • Controller: HTTP request handling                        │
│ • Service: Business logic implementation                   │
│ • Repository: Data access abstraction                     │
│ • Entity: Domain model                                     │
│ • DTOs: Data transfer objects                             │
├─────────────────────────────────────────────────────────────┤
│ Implementation Phases:                                      │
│ 1. Entity and database design                             │
│ 2. Repository layer implementation                        │
│ 3. Service layer with business logic                      │
│ 4. Controller with HTTP endpoints                         │
│ 5. DTO mapping and validation                             │
│ 6. Authentication and authorization                       │
│ 7. Error handling and logging                             │
│ 8. Testing and documentation                              │
└─────────────────────────────────────────────────────────────┘
```
```

#### For Feature Type: `module`
```markdown
**Phase 2.3: Module Architecture Design using GitHub Copilot Tools**
```
I'll design the complete module architecture:

## 🏛️ Module Architecture Design (Tool-Enhanced)

### Module Analysis and Design
```powershell
# Module architecture analysis using GitHub Copilot tools
semantic_search: "module architecture patterns" # Find module patterns
file_search: "src/modules/**" # Study existing modules
semantic_search: "feature module organization" # Find organization patterns
grep_search: "export.*from" # Analyze module exports
read_file: [EXISTING_MODULES] # Study module implementations
semantic_search: "dependency injection patterns" # Find DI patterns
```

### Module Specification (Tool-Designed)
```typescript
// Module Architecture Specification

// 1. Module Structure
interface ModuleStructure {
  components: ComponentStructure;
  pages: PageStructure;
  services: ServiceStructure;
  hooks: HookStructure;
  types: TypeStructure;
  constants: ConstantStructure;
  utils: UtilityStructure;
}

// Frontend Module Structure
src/modules/[feature-name]/
├── components/
│   ├── [FeatureName]List.tsx
│   ├── [FeatureName]Card.tsx
│   ├── [FeatureName]Form.tsx
│   ├── [FeatureName]Detail.tsx
│   └── index.ts
├── pages/
│   ├── [FeatureName]ListPage.tsx
│   ├── [FeatureName]DetailPage.tsx
│   ├── [FeatureName]CreatePage.tsx
│   └── index.ts
├── hooks/
│   ├── use[FeatureName].ts
│   ├── use[FeatureName]List.ts
│   ├── use[FeatureName]Form.ts
│   └── index.ts
├── services/
│   ├── [feature-name].service.ts
│   ├── [feature-name].api.ts
│   └── index.ts
├── types/
│   ├── [feature-name].types.ts
│   ├── api.types.ts
│   └── index.ts
├── constants/
│   ├── [feature-name].constants.ts
│   └── index.ts
├── utils/
│   ├── [feature-name].utils.ts
│   └── index.ts
└── index.ts

// Backend Module Structure
backend/Ikhtibar.[FeatureName]/
├── Controllers/
│   └── [FeatureName]Controller.cs
├── Services/
│   ├── I[FeatureName]Service.cs
│   └── [FeatureName]Service.cs
├── Repositories/
│   ├── I[FeatureName]Repository.cs
│   └── [FeatureName]Repository.cs
├── DTOs/
│   ├── [FeatureName]Dto.cs
│   ├── Create[FeatureName]Dto.cs
│   └── Update[FeatureName]Dto.cs
├── Entities/
│   └── [FeatureName].cs
├── Mappings/
│   └── [FeatureName]MappingProfile.cs
└── Ikhtibar.[FeatureName].csproj
```

### Module Integration Strategy
```typescript
// Module integration with existing Ikhtibar architecture

// 1. Module Registration and Configuration
export const [featureName]Module = {
  // Frontend module exports
  components: {
    [FeatureName]List,
    [FeatureName]Card,
    [FeatureName]Form,
    [FeatureName]Detail
  },
  pages: {
    [FeatureName]ListPage,
    [FeatureName]DetailPage,
    [FeatureName]CreatePage
  },
  hooks: {
    use[FeatureName],
    use[FeatureName]List,
    use[FeatureName]Form
  },
  services: {
    [featureName]Service,
    [featureName]Api
  },
  routes: [featureName]Routes,
  permissions: [featureName]Permissions
};

// 2. Route Integration
const [featureName]Routes: RouteObject[] = [
  {
    path: '/[feature-name]',
    element: <[FeatureName]Layout />,
    children: [
      { index: true, element: <[FeatureName]ListPage /> },
      { path: 'create', element: <[FeatureName]CreatePage /> },
      { path: ':id', element: <[FeatureName]DetailPage /> },
      { path: ':id/edit', element: <[FeatureName]EditPage /> }
    ]
  }
];

// 3. State Management Integration
interface [FeatureName]State {
  items: [FeatureName][];
  currentItem: [FeatureName] | null;
  loading: boolean;
  error: string | null;
  filters: [FeatureName]Filters;
  pagination: PaginationState;
}

// 4. Permission Integration
const [featureName]Permissions = {
  view: '[FEATURE_NAME].VIEW',
  create: '[FEATURE_NAME].CREATE',
  update: '[FEATURE_NAME].UPDATE',
  delete: '[FEATURE_NAME].DELETE',
  export: '[FEATURE_NAME].EXPORT'
};
```

### Module Development Plan
```
Module Development Plan:
┌─────────────────────────────────────────────────────────────┐
│ Module: [FeatureName] Complete Feature Module              │
├─────────────────────────────────────────────────────────────┤
│ Frontend Development (React/TypeScript):                   │
│ • Phase 1: Core types and interfaces                      │
│ • Phase 2: Service layer and API integration             │
│ • Phase 3: Base components development                    │
│ • Phase 4: Page components and routing                   │
│ • Phase 5: State management and hooks                    │
│ • Phase 6: Forms and validation                          │
│ • Phase 7: Testing and optimization                      │
├─────────────────────────────────────────────────────────────┤
│ Backend Development (.NET Core):                           │
│ • Phase 1: Entity and database design                    │
│ • Phase 2: Repository pattern implementation             │
│ • Phase 3: Service layer with business logic            │
│ • Phase 4: API controllers and endpoints                │
│ • Phase 5: DTOs and AutoMapper configuration           │
│ • Phase 6: Authentication and authorization             │
│ • Phase 7: Testing and documentation                    │
├─────────────────────────────────────────────────────────────┤
│ Integration and Testing:                                   │
│ • Database migrations and seeding                        │
│ • API endpoint testing                                   │
│ • Frontend-backend integration                           │
│ • User interface testing                                 │
│ • Performance optimization                               │
│ • Security validation                                    │
│ • Documentation completion                               │
└─────────────────────────────────────────────────────────────┘
```
```

### Phase 3: Implementation Roadmap and Documentation

```markdown
**Phase 3.1: Generate Comprehensive PRP Document using GitHub Copilot Tools**
```
I'll create the complete PRP document:

## 📋 Feature PRP Document (Tool-Generated)

### PRP Document Creation
```powershell
# Generate comprehensive documentation using GitHub Copilot tools
create_file: "[feature-name]-prp.md" # Create PRP document
semantic_search: "implementation best practices" # Find best practices
semantic_search: "testing strategies" # Find testing approaches
grep_search: "TODO|FIXME" # Check for existing issues
run_in_terminal: "npm run analyze" # Analyze codebase complexity
```

### Feature PRP Document Template
```markdown
# [FeatureName] Feature - Project Requirement & Planning Document

## 📋 Executive Summary
- **Feature**: [FeatureName] - [Brief Description]
- **Type**: [COMPONENT/PAGE/API/SERVICE/MODULE]
- **Scope**: [FRONTEND/BACKEND/FULL-STACK]
- **Complexity**: [SIMPLE/MODERATE/COMPLEX]
- **Priority**: [HIGH/MEDIUM/LOW]
- **Estimated Effort**: [TIME_ESTIMATE]
- **Dependencies**: [DEPENDENCY_LIST]

## 🎯 Business Requirements
### Problem Statement
[BUSINESS_PROBLEM_DESCRIPTION]

### Business Value
- **Primary Value**: [PRIMARY_BUSINESS_VALUE]
- **Secondary Benefits**: [SECONDARY_BENEFITS]
- **Success Metrics**: [SUCCESS_CRITERIA]
- **ROI Expectations**: [ROI_PROJECTIONS]

### User Stories
```typescript
interface UserStory {
  id: string;
  role: 'student' | 'teacher' | 'admin' | 'examiner';
  description: string;
  acceptanceCriteria: string[];
  priority: 'high' | 'medium' | 'low';
}

const userStories: UserStory[] = [
  {
    id: 'US-001',
    role: 'student',
    description: 'As a student, I want to [action] so that [benefit]',
    acceptanceCriteria: [
      'Given [context], when [action], then [expected outcome]',
      // Additional criteria
    ],
    priority: 'high'
  }
  // Additional user stories
];
```

## 🏗️ Technical Architecture
### System Design
```
Architecture Overview:
┌─────────────────────────────────────────────────────────────┐
│ Frontend (React 18 + TypeScript)                           │
│ ┌─────────────────┐ ┌─────────────────┐ ┌──────────────┐   │
│ │   Components    │ │      Pages      │ │    Hooks     │   │
│ │                 │ │                 │ │              │   │
│ └─────────────────┘ └─────────────────┘ └──────────────┘   │
│ ┌─────────────────┐ ┌─────────────────┐ ┌──────────────┐   │
│ │    Services     │ │     Types       │ │   Utils      │   │
│ │                 │ │                 │ │              │   │
│ └─────────────────┘ └─────────────────┘ └──────────────┘   │
├─────────────────────────────────────────────────────────────┤
│ Backend (.NET Core 8)                                      │
│ ┌─────────────────┐ ┌─────────────────┐ ┌──────────────┐   │
│ │  Controllers    │ │    Services     │ │ Repositories │   │
│ │                 │ │                 │ │              │   │
│ └─────────────────┘ └─────────────────┘ └──────────────┘   │
│ ┌─────────────────┐ ┌─────────────────┐ ┌──────────────┐   │
│ │    Entities     │ │      DTOs       │ │  Mappings    │   │
│ │                 │ │                 │ │              │   │
│ └─────────────────┘ └─────────────────┘ └──────────────┘   │
├─────────────────────────────────────────────────────────────┤
│ Database (SQL Server)                                      │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │           [FeatureName] Tables & Relations              │ │
│ └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### Technology Stack
- **Frontend**: React 18, TypeScript, Vite, Tailwind CSS
- **Backend**: .NET Core 8, Dapper Core, AutoMapper
- **Database**: SQL Server with Dapper
- **Testing**: Jest, React Testing Library, xUnit, Moq
- **State Management**: React Query, Context API
- **Validation**: React Hook Form, FluentValidation

### API Design
```yaml
# OpenAPI/Swagger specification
paths:
  /api/[feature-name]:
    get:
      summary: Get all [feature-name] items
      parameters:
        - name: page
          in: query
          type: integer
        - name: pageSize
          in: query
          type: integer
        - name: search
          in: query
          type: string
      responses:
        200:
          description: Success
          schema:
            $ref: '#/definitions/[FeatureName]ListResponse'
    post:
      summary: Create new [feature-name]
      requestBody:
        $ref: '#/definitions/Create[FeatureName]Request'
      responses:
        201:
          description: Created
          schema:
            $ref: '#/definitions/[FeatureName]Response'
```

## 📅 Implementation Timeline
### Development Phases
```
Implementation Timeline:
┌─────────────────────────────────────────────────────────────┐
│ Phase 1: Foundation (Week 1)                              │
│ • Database design and migrations                          │
│ • Entity models and relationships                         │
│ • Basic repository setup                                  │
│ • Core TypeScript interfaces                             │
├─────────────────────────────────────────────────────────────┤
│ Phase 2: Backend API (Week 2)                            │
│ • Service layer implementation                            │
│ • Controller endpoints                                    │
│ • DTO mapping configuration                              │
│ • Basic validation and error handling                    │
├─────────────────────────────────────────────────────────────┤
│ Phase 3: Frontend Core (Week 3)                          │
│ • Component development                                   │
│ • Service integration                                     │
│ • Basic page layouts                                      │
│ • Type-safe API integration                              │
├─────────────────────────────────────────────────────────────┤
│ Phase 4: Features & Polish (Week 4)                      │
│ • Advanced features implementation                        │
│ • Form validation and error handling                     │
│ • Loading states and error boundaries                    │
│ • Responsive design and accessibility                    │
├─────────────────────────────────────────────────────────────┤
│ Phase 5: Testing & Integration (Week 5)                  │
│ • Unit test implementation                                │
│ • Integration test setup                                  │
│ • End-to-end testing                                      │
│ • Performance optimization                                │
├─────────────────────────────────────────────────────────────┤
│ Phase 6: Documentation & Deployment (Week 6)             │
│ • API documentation completion                            │
│ • User guide creation                                     │
│ • Deployment preparation                                  │
│ • Final review and approval                               │
└─────────────────────────────────────────────────────────────┘
```

### Milestone Deliverables
| Milestone | Deliverable | Due Date | Success Criteria |
|-----------|-------------|----------|------------------|
| M1 | Database & Models | Week 1 | All entities created, migrations run |
| M2 | API Endpoints | Week 2 | All CRUD operations working |
| M3 | UI Components | Week 3 | All components render correctly |
| M4 | Feature Complete | Week 4 | All user stories implemented |
| M5 | Testing Complete | Week 5 | 80%+ test coverage achieved |
| M6 | Production Ready | Week 6 | Feature deployed and validated |

## 🧪 Testing Strategy
### Test Coverage Plan
```typescript
interface TestingStrategy {
  unit: UnitTestPlan;
  integration: IntegrationTestPlan;
  e2e: E2ETestPlan;
  performance: PerformanceTestPlan;
}

// Unit Testing (80% coverage target)
const unitTests = {
  frontend: [
    'Component rendering tests',
    'Hook behavior tests',
    'Service method tests',
    'Utility function tests'
  ],
  backend: [
    'Controller action tests',
    'Service method tests',
    'Repository tests',
    'Validation tests'
  ]
};

// Integration Testing
const integrationTests = [
  'API endpoint integration',
  'Database operation tests',
  'Authentication flow tests',
  'Error handling tests'
];

// End-to-End Testing
const e2eTests = [
  'Complete user workflows',
  'Cross-browser compatibility',
  'Mobile responsiveness',
  'Performance benchmarks'
];
```

### Quality Gates
- [ ] **Code Coverage**: Minimum 80% for new code
- [ ] **Type Safety**: 100% TypeScript compliance
- [ ] **Performance**: Page load < 2s, API response < 500ms
- [ ] **Accessibility**: WCAG 2.1 AA compliance
- [ ] **Security**: No critical vulnerabilities
- [ ] **Documentation**: 100% API documentation coverage

## 🔒 Security Considerations
### Security Requirements
- **Authentication**: Integration with existing JWT auth system
- **Authorization**: Role-based access control (RBAC)
- **Data Protection**: Encryption at rest and in transit
- **Input Validation**: Comprehensive validation on client and server
- **SQL Injection**: Parameterized queries and ORM protection
- **XSS Protection**: Content Security Policy and input sanitization

### Security Implementation
```typescript
// Security measures implementation
interface SecurityMeasures {
  authentication: 'JWT + Refresh Token';
  authorization: 'Role-Based Access Control';
  dataValidation: 'Client + Server Validation';
  encryption: 'TLS 1.3 + AES-256';
  logging: 'Comprehensive Audit Trail';
  monitoring: 'Real-time Security Alerts';
}
```

## 📊 Success Metrics
### Key Performance Indicators
- **Functionality**: 100% user stories implemented
- **Quality**: <2% defect rate in production
- **Performance**: <2s page load time, <500ms API response
- **Usability**: >4.5/5 user satisfaction score
- **Maintainability**: <1 day for minor feature additions
- **Documentation**: 100% API and component documentation

### Monitoring and Analytics
```typescript
interface FeatureMetrics {
  usage: {
    dailyActiveUsers: number;
    featureAdoption: number;
    sessionDuration: number;
  };
  performance: {
    pageLoadTime: number;
    apiResponseTime: number;
    errorRate: number;
  };
  business: {
    conversionRate: number;
    userSatisfaction: number;
    taskCompletionRate: number;
  };
}
```

## 🚀 Deployment Plan
### Deployment Strategy
1. **Development**: Feature branch development and testing
2. **Staging**: Integration testing and user acceptance
3. **Production**: Blue-green deployment with rollback capability

### Environment Configuration
```typescript
interface EnvironmentConfig {
  development: {
    apiUrl: 'http://localhost:5000/api';
    database: 'IkhtibarDb_Dev';
    logging: 'debug';
  };
  staging: {
    apiUrl: 'https://staging-api.ikhtibar.com/api';
    database: 'IkhtibarDb_Staging';
    logging: 'info';
  };
  production: {
    apiUrl: 'https://api.ikhtibar.com/api';
    database: 'IkhtibarDb_Prod';
    logging: 'warn';
  };
}
```

## 📚 Additional Resources
### Documentation Links
- API Documentation: [SWAGGER_URL]
- Component Storybook: [STORYBOOK_URL]
- User Guide: [USER_GUIDE_URL]
- Technical Specifications: [TECH_SPECS_URL]

### Team Responsibilities
- **Frontend Developer**: React components, TypeScript, styling
- **Backend Developer**: .NET Core API, database, business logic
- **QA Engineer**: Testing strategy, test automation, quality assurance
- **DevOps Engineer**: Deployment, monitoring, infrastructure
- **Product Owner**: Requirements, user stories, acceptance criteria
- **UI/UX Designer**: User interface, user experience, accessibility

## ✅ Acceptance Criteria
### Definition of Done
- [ ] All user stories implemented and tested
- [ ] Code review completed and approved
- [ ] Unit tests written and passing (80%+ coverage)
- [ ] Integration tests written and passing
- [ ] Documentation completed and reviewed
- [ ] Security review passed
- [ ] Performance benchmarks met
- [ ] Accessibility requirements satisfied
- [ ] Deployment to staging successful
- [ ] User acceptance testing completed
- [ ] Production deployment approved

### Sign-off Requirements
- [ ] **Product Owner**: Business requirements satisfied
- [ ] **Technical Lead**: Architecture and code quality approved
- [ ] **QA Lead**: Testing strategy executed and passed
- [ ] **Security Team**: Security review completed
- [ ] **DevOps Team**: Deployment strategy validated
```

### Implementation Commands (Tool-Generated)
```powershell
# Feature development commands
mkdir src/modules/[feature-name]
mkdir backend/Ikhtibar.[FeatureName]

# Frontend setup
npm create component [FeatureName]
npm run test -- [feature-name]
npm run build:analyze

# Backend setup
dotnet new classlib -n Ikhtibar.[FeatureName]
dotnet add reference Ikhtibar.Core
dotnet ef migrations add Add[FeatureName]

# Quality assurance
npm run type-check
npm run lint -- --fix
dotnet test --collect:"XPlat Code Coverage"

# Documentation
npm run storybook
swagger generate docs
```
```

## Command Activation Process
When a user types:
```
@copilot /create-feature-prp <feature_type> <feature_name> [scope] [complexity]
```

The system should:
1. **Analyze Requirements**: Parse input parameters and understand feature context
2. **Discover Architecture**: Use GitHub Copilot tools to analyze existing patterns
3. **Design Architecture**: Create comprehensive architecture plan
4. **Generate Documentation**: Create complete PRP document with implementation plan
5. **Provide Roadmap**: Include detailed timeline and success metrics
6. **Validate Strategy**: Ensure alignment with project standards and best practices

## Notes
- All PRP creation uses GitHub Copilot's native tools for maximum accuracy
- Generated PRPs adapt to Ikhtibar project-specific patterns and architecture
- Both frontend (React/TypeScript) and backend (.NET Core) considerations included
- Comprehensive testing strategies and quality gates are provided
- Security, performance, and accessibility requirements are integrated
- Documentation includes implementation commands and validation steps
- All recommendations follow Clean Architecture and modern development practices
- PRPs include detailed timeline, metrics, and success criteria for project management
