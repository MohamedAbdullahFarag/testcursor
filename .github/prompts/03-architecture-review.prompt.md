---
mode: agent
description: "Comprehensive architectural analysis and review for software projects with focus on patterns, performance, security, and maintainability"
---

---
inputs:
  - name: scope
    description: Review scope (current-changes, full-codebase, specific-module, architecture-only)
    required: false
    default: current-changes
  - name: focus
    description: Focus area (architecture, patterns, performance, security, maintainability, all)
    required: false
    default: architecture
  - name: depth
    description: Analysis depth (surface, detailed, comprehensive)
    required: false
    default: detailed
---

---
command: "/architecture-review"
---
# Architecture Review Command for GitHub Copilot

## Command Usage
```
@copilot /architecture-review [scope] [focus] [depth]
```

## Purpose
This command provides comprehensive architecture review for the Ikhtibar examination management system, analyzing code structure, design patterns, performance implications, and providing actionable recommendations for improvement.

**Input Parameters**: 
- `scope` - Review scope: `current-changes`, `full-codebase`, `specific-module`, or `architecture-only`
- `focus` - Focus area: `architecture`, `patterns`, `performance`, `security`, `maintainability`, or `all`
- `depth` - Analysis depth: `surface`, `detailed`, or `comprehensive`

## How /architecture-review Works

### Phase 1: Architecture Discovery and Context Analysis
```markdown
I'll perform a comprehensive architecture review of the Ikhtibar project. Let me start by understanding the current architecture and analyzing the codebase structure.

**Phase 1.1: Parse Review Request**
```
Architecture Review Analysis:
- **Scope**: [CURRENT-CHANGES/FULL-CODEBASE/SPECIFIC-MODULE/ARCHITECTURE-ONLY]
- **Focus**: [ARCHITECTURE/PATTERNS/PERFORMANCE/SECURITY/MAINTAINABILITY/ALL]
- **Depth**: [SURFACE/DETAILED/COMPREHENSIVE]
- **Project Context**: React 18 + .NET Core examination management system
- **Architecture Style**: Clean Architecture with domain-driven design
```

**Phase 1.2: Architecture Discovery using GitHub Copilot Tools**
```
I'll discover and analyze the current architecture:
- semantic_search: "architecture patterns" # Find architectural patterns
- file_search: "src/**/*.tsx" # Analyze frontend architecture
- file_search: "backend/**/*.cs" # Analyze backend architecture  
- list_dir: "src/modules/" # Understand module structure
- list_dir: "backend/" # Understand backend structure
- read_file: [ARCHITECTURE_FILES] # Read key architecture files
- grep_search: "interface|class|abstract" # Find structural patterns
```

**Phase 1.3: Codebase Structure Analysis**
```
Architecture Structure Analysis:
- [ ] **Frontend Architecture**: [ARCHITECTURE_PATTERN]
- [ ] **Backend Architecture**: [ARCHITECTURE_PATTERN]
- [ ] **Module Organization**: [MODULE_STRUCTURE]
- [ ] **Layer Separation**: [LAYER_COMPLIANCE]
- [ ] **Dependency Direction**: [DEPENDENCY_ANALYSIS]
- [ ] **Design Patterns**: [PATTERN_USAGE]
- [ ] **Technology Alignment**: [TECH_CONSISTENCY]
- [ ] **Scalability Readiness**: [SCALABILITY_ASSESSMENT]
```
```

### Phase 2: Focus-Specific Architecture Analysis

#### For Focus: `architecture`
```markdown
**Phase 2.1: Architectural Structure Analysis using GitHub Copilot Tools**
```
I'll analyze the overall architectural structure:

## 🏗️ Architectural Structure Review (Tool-Enhanced)

### Architecture Pattern Analysis
```powershell
# Comprehensive architecture analysis using GitHub Copilot tools
semantic_search: "Clean Architecture layers" # Find architectural layers
file_search: "backend/Ikhtibar.*/**.cs" # Analyze backend layers
semantic_search: "domain driven design" # Find DDD patterns
grep_search: "namespace.*\.(Core|Infrastructure|API)" # Find layer organization
read_file: [LAYER_FILES] # Read layer implementation files
```

### Layer Architecture Assessment (Tool-Discovered)
```typescript
// Architecture analysis results from tool discovery
interface ArchitectureAssessment {
  layers: LayerAnalysis[];
  compliance: ComplianceLevel;
  violations: ArchitectureViolation[];
  recommendations: ArchitectureRecommendation[];
}

// Clean Architecture layer analysis
const layerAnalysis: LayerAnalysis[] = [
  {
    layer: 'Presentation (API Controllers)',
    location: 'backend/Ikhtibar.API/Controllers/',
    compliance: 'Good',
    issues: [
      'Some controllers handling business logic directly',
      'Missing consistent error handling pattern'
    ],
    responsibilities: [
      'HTTP request/response handling',
      'Input validation',
      'Authentication/Authorization',
      'Response formatting'
    ],
    dependencies: ['Application Services', 'DTOs'],
    violations: [
      'Direct database access in UserController.cs line 45'
    ]
  },
  {
    layer: 'Application (Services)',
    location: 'backend/Ikhtibar.Core/Services/',
    compliance: 'Excellent',
    issues: [],
    responsibilities: [
      'Business logic coordination',
      'Use case implementation',
      'Cross-cutting concerns',
      'Transaction management'
    ],
    dependencies: ['Domain Entities', 'Repository Interfaces'],
    violations: []
  },
  {
    layer: 'Domain (Entities)',
    location: 'backend/Ikhtibar.Core/Entities/',
    compliance: 'Good',
    issues: [
      'Some entities missing domain validation',
      'Rich domain model not fully implemented'
    ],
    responsibilities: [
      'Business rules enforcement',
      'Domain logic',
      'Entity relationships',
      'Domain events'
    ],
    dependencies: ['None (Pure domain)'],
    violations: [
      'Infrastructure dependency in User.cs line 23'
    ]
  },
  {
    layer: 'Infrastructure (Data Access)',
    location: 'backend/Ikhtibar.Infrastructure/',
    compliance: 'Good',
    issues: [
      'Repository pattern implementation could be improved',
      'Missing unit of work pattern'
    ],
    responsibilities: [
      'Data persistence',
      'External service integration',
      'Infrastructure concerns',
      'Technology-specific implementations'
    ],
    dependencies: ['Domain Interfaces', 'External Libraries'],
    violations: []
  }
];
```

### Dependency Direction Analysis (Tool-Validated)
```
Dependency Flow Analysis:
┌─────────────────────────────────────────────────────────────┐
│ API Controllers (Presentation Layer)                       │
│ └── Depends on: Application Services ✓                    │
│     └── Depends on: Domain Entities ✓                     │
│         └── Depends on: Nothing (Pure Domain) ✓           │
├─────────────────────────────────────────────────────────────┤
│ Infrastructure Layer                                        │
│ └── Depends on: Domain Interfaces ✓                       │
│ └── Implements: Repository Contracts ✓                    │
├─────────────────────────────────────────────────────────────┤
│ Dependency Injection                                        │
│ └── Controllers → Services ✓                              │
│ └── Services → Repositories ✓                             │
│ └── Repositories → DbContext ✓                            │
├─────────────────────────────────────────────────────────────┤
│ Violations Found:                                           │
│ ❌ Direct DB access in UserController.cs                  │
│ ❌ Infrastructure reference in User.cs                     │
│ ⚠️ Missing abstractions in ExamService.cs                 │
└─────────────────────────────────────────────────────────────┘
```

### Frontend Architecture Analysis
```typescript
// Frontend architecture patterns discovered via tool analysis
interface FrontendArchitecture {
  organizationPattern: 'Feature-Based Modules';
  stateManagement: 'React Query + Context API';
  componentPattern: 'Functional Components with Hooks';
  routingStrategy: 'React Router with Route Guards';
  styling: 'Tailwind CSS with Component Libraries';
  buildTool: 'Vite with TypeScript';
}

// Module organization analysis
const moduleStructure = {
  'src/modules/auth/': {
    components: ['LoginForm', 'SignupForm', 'AuthGuard'],
    pages: ['LoginPage', 'SignupPage'],
    hooks: ['useAuth', 'useLogin'],
    services: ['authService', 'tokenService'],
    types: ['User', 'AuthState', 'LoginRequest'],
    compliance: 'Excellent'
  },
  'src/modules/exam/': {
    components: ['ExamCard', 'ExamForm', 'QuestionList'],
    pages: ['ExamListPage', 'ExamDetailPage'],
    hooks: ['useExam', 'useExamList'],
    services: ['examService', 'questionService'],
    types: ['Exam', 'Question', 'ExamState'],
    compliance: 'Good'
  }
};
```

### Architecture Quality Score (Tool-Calculated)
```
Architecture Quality Assessment:
┌─────────────────────────────────────────────────────────────┐
│ Layer Separation: 8.5/10                                   │
│ • Clear layer boundaries with minor violations             │
│ • Consistent dependency direction                          │
│ • Good abstraction usage                                   │
├─────────────────────────────────────────────────────────────┤
│ Module Cohesion: 9/10                                      │
│ • Feature-based organization                               │
│ • High cohesion within modules                            │
│ • Clear module boundaries                                  │
├─────────────────────────────────────────────────────────────┤
│ Coupling Management: 7.5/10                                │
│ • Good use of interfaces                                   │
│ • Some tight coupling in data access                      │
│ • Dependency injection properly implemented               │
├─────────────────────────────────────────────────────────────┤
│ Pattern Consistency: 8/10                                  │
│ • Consistent repository pattern                            │
│ • Good service layer implementation                       │
│ • Some pattern deviations                                 │
└─────────────────────────────────────────────────────────────┘

Overall Architecture Score: 8.25/10 (Excellent)
```
```

#### For Focus: `patterns`
```markdown
**Phase 2.2: Design Pattern Analysis using GitHub Copilot Tools**
```
I'll analyze design pattern usage and consistency:

## 🔄 Design Pattern Analysis (Tool-Enhanced)

### Pattern Discovery and Assessment
```powershell
# Design pattern analysis using GitHub Copilot tools
semantic_search: "design patterns repository" # Find repository patterns
semantic_search: "factory pattern" # Find factory implementations
grep_search: "interface.*Factory|interface.*Repository" # Find pattern interfaces
semantic_search: "dependency injection" # Find DI patterns
grep_search: "Strategy|Observer|Command" # Find behavioral patterns
```

### Pattern Implementation Analysis (Tool-Discovered)
```typescript
// Design pattern usage discovered via analysis
interface PatternAnalysis {
  implementedPatterns: PatternImplementation[];
  patternConsistency: ConsistencyLevel;
  missingPatterns: RecommendedPattern[];
  antiPatterns: AntiPatternInstance[];
}

// Repository Pattern Analysis
const repositoryPattern: PatternImplementation = {
  pattern: 'Repository Pattern',
  implementation: 'Good',
  locations: [
    'backend/Ikhtibar.Core/Repositories/IUserRepository.cs',
    'backend/Ikhtibar.Infrastructure/Repositories/UserRepository.cs'
  ],
  strengths: [
    'Clear abstraction layer',
    'Consistent interface design',
    'Proper dependency injection'
  ],
  weaknesses: [
    'Missing unit of work pattern',
    'Some repositories too granular',
    'Inconsistent error handling'
  ],
  compliance: 85,
  recommendations: [
    'Implement Unit of Work pattern',
    'Standardize repository method signatures',
    'Add repository base class for common operations'
  ]
};

// Service Layer Pattern Analysis
const servicePattern: PatternImplementation = {
  pattern: 'Service Layer Pattern',
  implementation: 'Excellent',
  locations: [
    'backend/Ikhtibar.Core/Services/',
    'frontend/src/services/'
  ],
  strengths: [
    'Clear business logic encapsulation',
    'Good separation of concerns',
    'Consistent service interfaces'
  ],
  weaknesses: [
    'Some services becoming too large',
    'Missing service composition patterns'
  ],
  compliance: 90,
  recommendations: [
    'Split large services into smaller, focused services',
    'Implement service composition for complex operations'
  ]
};

// Factory Pattern Analysis
const factoryPattern: PatternImplementation = {
  pattern: 'Factory Pattern',
  implementation: 'Partial',
  locations: [
    'frontend/src/components/forms/FormFactory.tsx'
  ],
  strengths: [
    'Dynamic component creation',
    'Consistent object creation'
  ],
  weaknesses: [
    'Limited usage across codebase',
    'Missing for complex object creation'
  ],
  compliance: 60,
  recommendations: [
    'Expand factory usage for complex entity creation',
    'Implement abstract factory for related object families'
  ]
};
```

### Pattern Consistency Assessment
```
Design Pattern Consistency Report:
┌─────────────────────────────────────────────────────────────┐
│ Repository Pattern                                          │
│ ✅ Implemented: 15 repositories                           │
│ ✅ Interface consistency: Good                            │
│ ⚠️ Method naming: Some inconsistencies                    │
│ ❌ Missing: Unit of Work pattern                          │
├─────────────────────────────────────────────────────────────┤
│ Service Layer Pattern                                       │
│ ✅ Implemented: 12 services                               │
│ ✅ Business logic encapsulation: Excellent               │
│ ✅ Dependency injection: Properly configured             │
│ ⚠️ Service size: Some services too large                 │
├─────────────────────────────────────────────────────────────┤
│ Observer Pattern (React)                                    │
│ ✅ Implemented: React hooks and context                   │
│ ✅ State management: React Query + Context               │
│ ✅ Event handling: Consistent patterns                   │
│ ✅ Component updates: Optimized re-rendering             │
├─────────────────────────────────────────────────────────────┤
│ Strategy Pattern                                            │
│ ⚠️ Partial implementation: Authentication strategies      │
│ ❌ Missing: Validation strategies                         │
│ ❌ Missing: Export/import strategies                      │
│ 🔄 Recommendation: Expand strategy pattern usage         │
└─────────────────────────────────────────────────────────────┘
```

### Anti-Pattern Detection (Tool-Identified)
```typescript
// Anti-patterns discovered via code analysis
const antiPatterns: AntiPatternInstance[] = [
  {
    pattern: 'God Object',
    location: 'backend/Ikhtibar.Core/Services/ExamManagementService.cs',
    severity: 'High',
    description: 'Service class handling too many responsibilities',
    linesOfCode: 450,
    methodCount: 25,
    recommendation: 'Split into specialized services (ExamService, QuestionService, GradingService)',
    effort: 'Medium'
  },
  {
    pattern: 'Data Transfer Object Pollution',
    location: 'backend/Ikhtibar.API/DTOs/',
    severity: 'Medium', 
    description: 'DTOs containing business logic',
    affectedFiles: ['UserDto.cs', 'ExamDto.cs'],
    recommendation: 'Move business logic to domain entities or services',
    effort: 'Low'
  },
  {
    pattern: 'Tight Coupling',
    location: 'frontend/src/components/ExamForm.tsx',
    severity: 'Medium',
    description: 'Component directly importing multiple services',
    dependencies: ['examService', 'userService', 'notificationService'],
    recommendation: 'Use dependency injection or custom hooks for service access',
    effort: 'Low'
  }
];
```
```

#### For Focus: `performance`
```markdown
**Phase 2.3: Performance Architecture Analysis using GitHub Copilot Tools**
```
I'll analyze performance implications of the current architecture:

## ⚡ Performance Architecture Analysis (Tool-Enhanced)

### Performance Pattern Assessment
```powershell
# Performance analysis using GitHub Copilot tools
semantic_search: "performance optimization" # Find performance patterns
grep_search: "async|await|Promise" # Find async patterns
semantic_search: "caching strategies" # Find caching implementations
grep_search: "useMemo|useCallback|React.memo" # Find React optimizations
semantic_search: "database performance" # Find DB optimization patterns
```

### Frontend Performance Architecture (Tool-Analyzed)
```typescript
// Frontend performance patterns discovered
interface FrontendPerformance {
  bundleOptimization: BundleAnalysis;
  componentOptimization: ComponentAnalysis;
  stateManagement: StateAnalysis;
  cachingStrategy: CachingAnalysis;
}

const bundleAnalysis: BundleAnalysis = {
  totalSize: '2.1MB', // Discovered via build analysis
  treeShaking: 'Implemented',
  codesplitting: 'Route-based',
  lazyLoading: 'Components and routes',
  optimizations: [
    'Vite bundle optimization enabled',
    'Dynamic imports for large components',
    'Route-based code splitting'
  ],
  issues: [
    'Large third-party library bundle (moment.js)',
    'Missing image optimization',
    'Duplicate dependencies in vendor bundle'
  ],
  recommendations: [
    'Replace moment.js with date-fns for smaller bundle',
    'Implement image optimization with next/image equivalent',
    'Analyze and deduplicate vendor dependencies'
  ]
};

const componentOptimization: ComponentAnalysis = {
  memoization: {
    reactMemo: 'Used in 15 components',
    useMemo: 'Used in 23 hooks',
    useCallback: 'Used in 18 event handlers',
    effectiveness: 'Good but could be improved'
  },
  renderOptimization: {
    unnecessaryReRenders: 'Low (detected in 3 components)',
    keyUsage: 'Consistent and optimized',
    conditionalRendering: 'Well implemented'
  },
  issues: [
    'Missing React.memo in ExamList component',
    'Expensive calculations not memoized in StatisticsCard',
    'Event handlers recreated on every render in FormComponents'
  ]
};
```

### Backend Performance Architecture (Tool-Evaluated)
```csharp
// Backend performance patterns analysis
interface BackendPerformance {
  dataAccess: DataAccessAnalysis;
  apiOptimization: ApiAnalysis; 
  cachingStrategy: CachingAnalysis;
  scalabilityPatterns: ScalabilityAnalysis;
}

// Database performance analysis
const dataAccessAnalysis: DataAccessAnalysis = {
  queryOptimization: {
    eagerLoading: 'Implemented with Include()',
    lazyLoading: 'Disabled for performance',
    projections: 'Used in list operations',
    indexing: 'Basic indexes present'
  },
  nPlusOneIssues: [
    {
      location: 'ExamService.GetExamsWithQuestions()',
      severity: 'High',
      impact: 'Database queries scale with exam count',
      solution: 'Use Include() to eager load questions'
    }
  ],
  connectionManagement: {
    pooling: 'Enabled',
    connectionString: 'Optimized',
    timeout: 'Configured appropriately'
  },
  recommendations: [
    'Add composite indexes for complex queries',
    'Implement query result caching',
    'Use database pagination for large result sets',
    'Consider read replicas for reporting queries'
  ]
};

// API performance analysis
const apiOptimization: ApiAnalysis = {
  responseTime: 'Average 200ms (Good)',
  throughput: 'Estimated 1000 req/min',
  patterns: [
    'Async/await consistently used',
    'Proper exception handling',
    'DTO projection implemented'
  ],
  bottlenecks: [
    'File upload endpoints not optimized',
    'Complex report generation blocking',
    'Missing response compression'
  ]
};
```

### Performance Recommendations (Tool-Generated)
```
Performance Optimization Priority Matrix:
┌─────────────────────────────────────────────────────────────┐
│ High Priority (Immediate Impact)                            │
│ 🔥 Fix N+1 query issues in ExamService                    │
│ 🔥 Implement React.memo in ExamList component             │
│ 🔥 Add response caching to frequently accessed APIs       │
│ 🔥 Optimize bundle size (replace moment.js)               │
├─────────────────────────────────────────────────────────────┤
│ Medium Priority (Noticeable Impact)                        │
│ ⚡ Add database indexes for complex queries               │
│ ⚡ Implement image optimization                           │
│ ⚡ Add API response compression                           │
│ ⚡ Optimize component re-rendering                        │
├─────────────────────────────────────────────────────────────┤
│ Low Priority (Future Optimization)                         │
│ 📈 Implement advanced caching strategies                  │
│ 📈 Add performance monitoring                             │
│ 📈 Consider CDN for static assets                         │
│ 📈 Implement advanced lazy loading                        │
└─────────────────────────────────────────────────────────────┘

Estimated Performance Improvement: 40-60% in load times
```
```

### Phase 3: Comprehensive Architecture Assessment

#### Overall Architecture Health Score
```markdown
**Phase 3.1: Generate Architecture Health Report using GitHub Copilot Tools**
```
I'll generate a comprehensive architecture assessment:

## 📊 Architecture Health Report (Tool-Generated)

### Comprehensive Assessment Matrix
```powershell
# Comprehensive assessment using GitHub Copilot tools
semantic_search: "code quality metrics" # Find quality indicators
run_in_terminal: "npm run analyze" # Analyze codebase metrics
get_errors: [ALL_PROJECT_FILES] # Check for structural issues
semantic_search: "technical debt" # Find potential debt areas
grep_search: "HACK|TODO|FIXME" # Find technical debt markers
```

### Architecture Health Score
| Dimension | Score | Status | Priority Actions |
|-----------|-------|--------|------------------|
| **Layer Separation** | 8.5/10 | ✅ Good | Fix dependency violations |
| **Module Cohesion** | 9.0/10 | ✅ Excellent | Maintain current standards |
| **Pattern Consistency** | 8.0/10 | ✅ Good | Standardize repository methods |
| **Performance** | 7.5/10 | ⚠️ Needs Attention | Fix N+1 queries, optimize bundles |
| **Scalability** | 8.0/10 | ✅ Good | Add caching strategies |
| **Maintainability** | 8.5/10 | ✅ Good | Reduce service complexity |
| **Testability** | 7.0/10 | ⚠️ Needs Attention | Increase test coverage |
| **Security** | 9.0/10 | ✅ Excellent | Maintain security practices |

**Overall Architecture Health: 8.2/10 (Very Good)**

### Critical Issues Summary (Tool-Identified)
#### Must Fix (High Priority)
1. **N+1 Query Issue in ExamService**
   - **Impact**: Database performance degradation
   - **Location**: `backend/Ikhtibar.Core/Services/ExamService.cs:45`
   - **Fix**: Implement eager loading with Include()
   - **Effort**: 2 hours

2. **God Object Anti-Pattern**
   - **Impact**: Maintainability and testability issues
   - **Location**: `backend/Ikhtibar.Core/Services/ExamManagementService.cs`
   - **Fix**: Split into specialized services
   - **Effort**: 1 day

3. **Missing React Optimization**
   - **Impact**: Frontend performance issues
   - **Location**: `frontend/src/components/ExamList.tsx`
   - **Fix**: Add React.memo and useMemo optimizations
   - **Effort**: 3 hours

#### Should Fix (Medium Priority)
1. **Bundle Size Optimization**
   - **Impact**: Page load performance
   - **Solution**: Replace moment.js with date-fns
   - **Effort**: 4 hours

2. **Missing Unit of Work Pattern**
   - **Impact**: Data consistency potential issues
   - **Solution**: Implement UoW pattern for complex transactions
   - **Effort**: 1 day

### Architectural Recommendations (Tool-Generated)
```typescript
interface ArchitectureRoadmap {
  immediate: ImmediateAction[];
  shortTerm: ShortTermGoal[];
  longTerm: LongTermVision[];
}

const architectureRoadmap: ArchitectureRoadmap = {
  immediate: [
    {
      action: 'Fix database performance issues',
      timeline: '1 week',
      effort: 'Medium',
      impact: 'High',
      description: 'Resolve N+1 queries and add strategic indexes'
    },
    {
      action: 'Optimize critical React components',
      timeline: '3 days',
      effort: 'Low',
      impact: 'Medium',
      description: 'Add memoization to frequently rendered components'
    }
  ],
  shortTerm: [
    {
      goal: 'Implement comprehensive caching strategy',
      timeline: '2-3 weeks',
      effort: 'High',
      impact: 'High',
      description: 'Add Redis caching for API responses and database queries'
    },
    {
      goal: 'Enhance testing architecture',
      timeline: '2 weeks', 
      effort: 'Medium',
      impact: 'Medium',
      description: 'Increase test coverage and implement integration tests'
    }
  ],
  longTerm: [
    {
      vision: 'Microservices migration readiness',
      timeline: '6 months',
      effort: 'Very High',
      impact: 'High',
      description: 'Prepare architecture for potential microservices split'
    },
    {
      vision: 'Advanced monitoring and observability',
      timeline: '3 months',
      effort: 'High', 
      impact: 'Medium',
      description: 'Implement comprehensive monitoring, logging, and alerting'
    }
  ]
};
```

### Final Assessment Summary
```
Architecture Review Summary:
┌─────────────────────────────────────────────────────────────┐
│ 🎯 Overall Assessment: GOOD ARCHITECTURE                   │
│                                                             │
│ ✅ Strengths:                                              │
│ • Clean Architecture principles well implemented           │
│ • Good separation of concerns                              │
│ • Consistent technology choices                            │
│ • Modern development patterns                              │
│ • Good module organization                                 │
│                                                             │
│ ⚠️ Areas for Improvement:                                  │
│ • Database query optimization needed                       │
│ • Some service classes too large                          │
│ • Frontend bundle optimization required                    │
│ • Test coverage gaps                                       │
│                                                             │
│ 🚀 Strategic Recommendations:                              │
│ • Implement performance optimizations (immediate)          │
│ • Enhance caching strategy (short-term)                   │
│ • Prepare for scale (long-term)                           │
│ • Invest in monitoring and observability                  │
│                                                             │
│ 📊 Readiness Assessment:                                   │
│ • Production Ready: ✅ YES                                │
│ • Scale Ready: ⚠️ WITH OPTIMIZATIONS                      │
│ • Maintenance Ready: ✅ YES                               │
│ • Team Ready: ✅ YES                                      │
└─────────────────────────────────────────────────────────────┘

Next Steps:
1. Address critical performance issues (Week 1)
2. Implement immediate optimizations (Week 2-3)
3. Plan and execute short-term improvements (Month 1-2)
4. Roadmap long-term architectural evolution (Quarter 1)
```
```

## Command Activation Process
When a user types:
```
@copilot /architecture-review [scope] [focus] [depth]
```

The system should:
1. **Discover Architecture**: Use GitHub Copilot tools to analyze codebase structure
2. **Assess Patterns**: Evaluate design pattern implementation and consistency
3. **Analyze Performance**: Review performance implications and optimizations
4. **Identify Issues**: Find architectural violations and anti-patterns
5. **Generate Recommendations**: Create actionable improvement roadmap
6. **Provide Health Score**: Calculate overall architecture quality metrics

## Notes
- All architecture analysis uses GitHub Copilot's native tools for comprehensive discovery
- Review adapts to Ikhtibar project-specific patterns and Clean Architecture principles
- Both frontend (React/TypeScript) and backend (.NET Core) architectures analyzed
- Performance considerations include database, API, and frontend optimization
- Recommendations prioritized by impact and effort for practical implementation
- Health scoring provides objective assessment of architectural quality
- Long-term roadmap considers scalability and maintainability requirements
- All findings include specific file locations and actionable solutions
