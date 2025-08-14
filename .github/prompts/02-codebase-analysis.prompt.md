---
mode: agent
description: "Deep codebase analysis providing architectural insights, pattern identification, and actionable recommendations for code quality improvement"
---

---
inputs:
  - name: complexity_level
    description: Analysis complexity level (basic, standard, comprehensive, expert)
    required: false
    default: standard
  - name: focus_area
    description: Specific focus area (architecture, patterns, performance, security, maintainability)
    required: false
    default: architecture
  - name: output_format
    description: Output format (summary, detailed, actionable, report)
    required: false
    default: actionable
---

---
command: "/codebase-analysis"
---
# Codebase Analysis Command for GitHub Copilot

## Command Usage
```
@copilot /codebase-analysis [complexity_level] [focus_area] [output_format]
```

## Purpose
This command performs comprehensive analysis of the entire codebase using GitHub Copilot's native tools to provide insights on architecture, code quality, patterns, dependencies, and improvement opportunities across both backend and frontend implementations.

**Input Parameters**:
- `complexity_level` - Analysis depth: `basic`, `standard`, `comprehensive`, `expert`
- `focus_area` - Analysis focus: `architecture`, `patterns`, `performance`, `security`, `maintainability`
- `output_format` - Result format: `summary`, `detailed`, `actionable`, `report`

## How /codebase-analysis Works

### Phase 1: Comprehensive Discovery and Mapping
```markdown
I'll perform a comprehensive analysis of your codebase. Let me start with systematic discovery using GitHub Copilot's tools.

**Phase 1.1: Project Structure Discovery**
```
I'll map the entire project structure and identify key components:
- list_dir: "/" # Root structure analysis
- list_dir: "backend/" # Backend project discovery
- list_dir: "frontend/src/" # Frontend structure discovery
- file_search: "**/*.csproj" # .NET project files
- file_search: "**/package.json" # Node.js project files
- file_search: "**/*.sln" # Solution files
- semantic_search: "project architecture patterns" # Architectural discovery
```

**Phase 1.2: Technology Stack Discovery**
```
I'll analyze the complete technology stack:
- read_file: "backend/Ikhtibar.API/Ikhtibar.API.csproj" # Backend dependencies
- read_file: "frontend/package.json" # Frontend dependencies
- read_file: "backend/Program.cs" # Application configuration
- read_file: "frontend/vite.config.ts" # Build configuration
- grep_search: "using\s+.*;" # Backend namespace analysis
- grep_search: "import.*from" # Frontend import analysis
- semantic_search: "framework versions" # Version discovery
```

**Phase 1.3: Architecture Pattern Recognition**
```
I'll identify architectural patterns and design approaches:
- semantic_search: "design patterns repository service" # Pattern identification
- file_search: "**/*Controller.cs" # API layer discovery
- file_search: "**/*Service.cs" # Service layer discovery
- file_search: "**/*Repository.cs" # Data layer discovery
- file_search: "src/modules/**/*.tsx" # Component layer discovery
- semantic_search: "dependency injection patterns" # DI analysis
- grep_search: "interface\s+I[A-Z]" # Interface pattern analysis
```
```

### Phase 2: Code Quality and Pattern Analysis

#### Backend Analysis
```markdown
**Phase 2.1: Backend Architecture Analysis using GitHub Copilot Tools**
```
I'll analyze the ASP.NET Core backend architecture and patterns:

## 🏗️ Backend Architecture Analysis (Tool-Enhanced)

### Layer Architecture Discovery
```powershell
# Backend layer analysis using GitHub Copilot tools
semantic_search: "layered architecture backend" # Find layer patterns
list_dir: "backend/Ikhtibar.Core/" # Core layer analysis
list_dir: "backend/Ikhtibar.Infrastructure/" # Infrastructure layer
list_dir: "backend/Ikhtibar.API/" # API layer analysis
grep_search: "namespace\s+Ikhtibar\.(Core|Infrastructure|API)" # Namespace analysis
```

### Service Layer Analysis
```csharp
// Service pattern analysis using GitHub Copilot tools
interface BackendArchitectureAnalysis {
  layerStructure: LayerAnalysis;
  dependencyPatterns: DependencyPattern[];
  designPatterns: PatternUsage[];
  qualityMetrics: QualityMetric[];
}

// Tool-discovered service patterns
const serviceLayerAnalysis = {
  discovered: [
    'file_search: "**/*Service.cs"', // Find all service implementations
    'semantic_search: "service layer patterns"', // Pattern discovery
    'grep_search: "public.*Service.*:"', // Service class patterns
    'read_file: [SERVICE_FILES]' // Analyze service implementations
  ],
  patterns: {
    dependencyInjection: 'Analysis via DI container registration patterns',
    interfaceSegregation: 'Analysis via I[ServiceName] interface patterns',
    singleResponsibility: 'Analysis via service method grouping',
    repositoryPattern: 'Analysis via repository dependency usage'
  },
  qualityIndicators: {
    testCoverage: 'Analysis via test file discovery',
    codeComplexity: 'Analysis via method line count patterns',
    documentationLevel: 'Analysis via XML documentation comments',
    errorHandling: 'Analysis via try-catch and exception patterns'
  }
};
```

### Repository Pattern Analysis
```csharp
// Repository pattern analysis using GitHub Copilot tools
const repositoryAnalysis = {
  discoveryCommands: [
    'file_search: "**/*Repository.cs"', // Find repository implementations
    'semantic_search: "repository pattern dapper"', // Pattern analysis
    'grep_search: "BaseRepository|IRepository"', // Base pattern usage
    'read_file: [REPOSITORY_FILES]' // Implementation analysis
  ],
  patterns: {
    baseRepository: 'Analysis via BaseRepository inheritance',
    genericRepository: 'Analysis via generic type usage',
    unitOfWork: 'Analysis via transaction handling',
    queryOptimization: 'Analysis via SQL query patterns'
  },
  dataAccess: {
    ormUsage: 'Dapper ORM pattern analysis',
    connectionManagement: 'Connection string and disposal patterns',
    transactionHandling: 'Transaction scope and rollback patterns',
    parameterization: 'SQL injection prevention analysis'
  }
};
```

### API Design Analysis
```csharp
// API design pattern analysis using GitHub Copilot tools
const apiAnalysis = {
  discoveryCommands: [
    'file_search: "**/*Controller.cs"', // Find all controllers
    'semantic_search: "RESTful API patterns"', // REST pattern analysis
    'grep_search: "\\[Http(Get|Post|Put|Delete)\\]"', // HTTP verb usage
    'read_file: [CONTROLLER_FILES]' // Controller implementation analysis
  ],
  restfulPatterns: {
    resourceNaming: 'Analysis via route attribute patterns',
    httpVerbUsage: 'Analysis via HTTP method decoration',
    statusCodeUsage: 'Analysis via ActionResult patterns',
    contentNegotiation: 'Analysis via Accept header handling'
  },
  security: {
    authentication: 'Analysis via [Authorize] attribute usage',
    authorization: 'Analysis via policy-based authorization',
    inputValidation: 'Analysis via model validation attributes',
    corsConfiguration: 'Analysis via CORS policy setup'
  }
};
```
```

#### Frontend Analysis
```markdown
**Phase 2.2: Frontend Architecture Analysis using GitHub Copilot Tools**
```
I'll analyze the React TypeScript frontend architecture and patterns:

## ⚛️ Frontend Architecture Analysis (Tool-Enhanced)

### Component Architecture Discovery
```powershell
# Frontend component analysis using GitHub Copilot tools
semantic_search: "React component patterns" # Find component patterns
list_dir: "frontend/src/modules/" # Module structure analysis
file_search: "**/*.tsx" # Find all React components
file_search: "**/*.ts" # Find TypeScript files
semantic_search: "custom hooks patterns" # Hook pattern discovery
grep_search: "export.*component|export.*function" # Export patterns
```

### Module Structure Analysis
```typescript
// Module architecture analysis using GitHub Copilot tools
interface FrontendArchitectureAnalysis {
  moduleStructure: ModuleAnalysis;
  componentPatterns: ComponentPattern[];
  stateManagement: StatePattern[];
  performanceOptimizations: OptimizationPattern[];
}

// Tool-discovered module patterns
const moduleAnalysis = {
  discoveryCommands: [
    'list_dir: "frontend/src/modules/"', // Module discovery
    'semantic_search: "folder per feature"', // Architecture pattern
    'file_search: "src/modules/**/components/*.tsx"', // Component discovery
    'file_search: "src/modules/**/hooks/*.ts"', // Hook discovery
    'file_search: "src/modules/**/services/*.ts"', // Service discovery
    'file_search: "src/modules/**/types/*.ts"' // Type discovery
  ],
  patterns: {
    folderPerFeature: 'Analysis via module directory structure',
    componentHierarchy: 'Analysis via component import dependencies',
    typeDefinitions: 'Analysis via TypeScript interface usage',
    serviceIntegration: 'Analysis via API service patterns'
  },
  architecture: {
    componentComposition: 'Analysis via component tree structure',
    propsDrilling: 'Analysis via props interface complexity',
    contextUsage: 'Analysis via React Context patterns',
    stateColocation: 'Analysis via useState and useEffect patterns'
  }
};
```

### React Patterns Analysis
```typescript
// React patterns analysis using GitHub Copilot tools
const reactPatternsAnalysis = {
  discoveryCommands: [
    'semantic_search: "React hooks patterns"', // Hook pattern analysis
    'grep_search: "use[A-Z][a-zA-Z]*"', // Custom hook discovery
    'semantic_search: "React component optimization"', // Performance patterns
    'grep_search: "React\\.memo|useMemo|useCallback"', // Optimization usage
    'semantic_search: "React error boundaries"', // Error handling patterns
  ],
  hookPatterns: {
    customHooks: 'Analysis via use[Name] pattern discovery',
    stateManagement: 'Analysis via useState/useReducer usage',
    effectManagement: 'Analysis via useEffect cleanup patterns',
    performanceHooks: 'Analysis via useMemo/useCallback usage'
  },
  componentPatterns: {
    functionalComponents: 'Analysis via function component patterns',
    componentComposition: 'Analysis via children prop usage',
    renderProps: 'Analysis via render prop patterns',
    higherOrderComponents: 'Analysis via HOC patterns'
  },
  optimizationPatterns: {
    memoization: 'Analysis via React.memo usage',
    lazyLoading: 'Analysis via React.lazy and Suspense',
    codesplitting: 'Analysis via dynamic import patterns',
    bundleOptimization: 'Analysis via Vite configuration'
  }
};
```

### TypeScript Analysis
```typescript
// TypeScript usage analysis using GitHub Copilot tools
const typeScriptAnalysis = {
  discoveryCommands: [
    'semantic_search: "TypeScript interfaces types"', // Type definition analysis
    'file_search: "**/*.d.ts"', // Type declaration files
    'grep_search: "interface\\s+[A-Z][a-zA-Z]*"', // Interface discovery
    'grep_search: "type\\s+[A-Z][a-zA-Z]*"', // Type alias discovery
    'semantic_search: "TypeScript strict mode"' // Configuration analysis
  ],
  typeDefinitions: {
    interfaceUsage: 'Analysis via interface declaration patterns',
    typeAliases: 'Analysis via type alias usage',
    genericTypes: 'Analysis via generic type parameters',
    utilityTypes: 'Analysis via Pick, Omit, Partial usage'
  },
  typeScriptFeatures: {
    strictMode: 'Analysis via tsconfig.json configuration',
    nullSafety: 'Analysis via optional chaining usage',
    typeGuards: 'Analysis via type predicate functions',
    decorators: 'Analysis via decorator usage patterns'
  }
};
```
```

### Phase 3: Quality Assessment and Metrics

#### Code Quality Analysis
```markdown
**Phase 3.1: Code Quality Assessment using GitHub Copilot Tools**
```
I'll assess code quality across multiple dimensions:

## 📊 Code Quality Assessment (Tool-Enhanced)

### Quality Metrics Discovery
```powershell
# Code quality analysis using GitHub Copilot tools
semantic_search: "code quality metrics" # Find quality indicators
grep_search: "TODO|FIXME|HACK|BUG" # Technical debt discovery
semantic_search: "testing patterns coverage" # Test quality analysis
file_search: "**/*Test*.cs" # Backend test discovery
file_search: "**/*.test.ts*" # Frontend test discovery
semantic_search: "error handling patterns" # Error handling analysis
grep_search: "try\\s*{|catch\\s*\\(" # Exception handling discovery
```

### Technical Debt Analysis
```typescript
// Technical debt assessment using GitHub Copilot tools
interface TechnicalDebtAnalysis {
  codeSmells: CodeSmell[];
  duplicatedCode: DuplicationPattern[];
  complexityIssues: ComplexityIssue[];
  maintainabilityIndex: QualityScore;
}

const technicalDebtAssessment = {
  discoveryCommands: [
    'grep_search: "TODO|FIXME|HACK|DEPRECATED"', // Technical debt markers
    'semantic_search: "code duplication patterns"', // Duplication analysis
    'semantic_search: "complex methods functions"', // Complexity analysis
    'grep_search: "if\\s*\\(.*&&.*\\|\\|"', // Complex condition discovery
    'semantic_search: "magic numbers strings"' // Magic value discovery
  ],
  codeSmells: {
    longMethods: 'Analysis via method line count patterns',
    largeClasses: 'Analysis via class size and responsibility',
    duplicatedCode: 'Analysis via similar code block patterns',
    magicNumbers: 'Analysis via hardcoded value usage',
    deepNesting: 'Analysis via indentation and brace patterns'
  },
  maintainabilityFactors: {
    singleResponsibility: 'Analysis via class and method purpose clarity',
    openClosed: 'Analysis via extension point patterns',
    dependencyInversion: 'Analysis via abstraction usage',
    interfaceSegregation: 'Analysis via interface size and scope'
  }
};
```

### Performance Analysis
```typescript
// Performance analysis using GitHub Copilot tools
const performanceAnalysis = {
  discoveryCommands: [
    'semantic_search: "performance optimization patterns"', // Performance patterns
    'grep_search: "async\\s+.*await"', // Async pattern usage
    'semantic_search: "caching mechanisms"', // Caching analysis
    'grep_search: "useMemo|useCallback|React\\.memo"', // React optimization
    'semantic_search: "database query optimization"' // Query optimization
  ],
  backendPerformance: {
    asyncPatterns: 'Analysis via async/await usage patterns',
    databaseOptimization: 'Analysis via query patterns and indexing',
    cachingStrategies: 'Analysis via caching implementation patterns',
    memoryManagement: 'Analysis via IDisposable and using patterns'
  },
  frontendPerformance: {
    componentOptimization: 'Analysis via React optimization hooks',
    bundleSize: 'Analysis via dynamic import and code splitting',
    renderOptimization: 'Analysis via key props and reconciliation',
    stateManagement: 'Analysis via state update patterns'
  }
};
```

### Security Analysis
```typescript
// Security analysis using GitHub Copilot tools
const securityAnalysis = {
  discoveryCommands: [
    'semantic_search: "security authentication authorization"', // Security patterns
    'grep_search: "\\[Authorize\\]|\\[AllowAnonymous\\]"', // Authorization discovery
    'semantic_search: "input validation sanitization"', // Input validation
    'grep_search: "sql.*query|SELECT.*FROM"', // SQL injection analysis
    'semantic_search: "CORS CSRF XSS protection"' // Security vulnerability analysis
  ],
  authenticationSecurity: {
    jwtImplementation: 'Analysis via JWT token handling patterns',
    passwordSecurity: 'Analysis via password hashing and validation',
    sessionManagement: 'Analysis via session handling patterns',
    roleBasedAccess: 'Analysis via role and permission patterns'
  },
  inputValidation: {
    modelValidation: 'Analysis via data annotation usage',
    sanitization: 'Analysis via input cleaning patterns',
    sqlInjectionPrevention: 'Analysis via parameterized query usage',
    xssProtection: 'Analysis via output encoding patterns'
  }
};
```
```

### Phase 4: Improvement Recommendations and Action Plan

#### Actionable Recommendations Generation
```markdown
**Phase 4.1: Generate Improvement Recommendations using GitHub Copilot Tools**
```
I'll provide specific, actionable recommendations based on analysis:

## 🎯 Improvement Recommendations (Tool-Generated)

### Architecture Improvements
```powershell
# Architecture improvement analysis using GitHub Copilot tools
semantic_search: "architectural anti-patterns" # Find architecture issues
semantic_search: "clean architecture principles" # Best practice discovery
semantic_search: "SOLID principles violations" # SOLID analysis
list_code_usages: "specific-class-or-method" # Usage pattern analysis
get_errors: [ANALYZED_FILES] # Error and warning discovery
```

### Immediate Action Items (High Priority)
```typescript
// Immediate improvements based on tool analysis
interface ImmediateActions {
  priority: 'critical' | 'high' | 'medium' | 'low';
  category: string;
  description: string;
  impact: string;
  effort: string;
  commands: string[];
}

const immediateActions: ImmediateActions[] = [
  {
    priority: 'critical',
    category: 'Architecture',
    description: 'Fix SRP violations in [DISCOVERED_CLASSES]',
    impact: 'Improves maintainability and testability',
    effort: '2-3 days',
    commands: [
      'semantic_search: "single responsibility violations"',
      'list_code_usages: "[VIOLATING_CLASS]"',
      'Refactor classes based on analysis'
    ]
  },
  {
    priority: 'high',
    category: 'Performance',
    description: 'Optimize discovered N+1 query patterns',
    impact: 'Reduces database load by 70%',
    effort: '1-2 days',
    commands: [
      'semantic_search: "database query patterns"',
      'grep_search: "await.*foreach|for.*await"',
      'Implement batch loading strategies'
    ]
  },
  {
    priority: 'high',
    category: 'Security',
    description: 'Implement missing input validation',
    impact: 'Prevents security vulnerabilities',
    effort: '1 day',
    commands: [
      'grep_search: "\\[HttpPost\\].*without.*\\[.*Valid"',
      'Add validation attributes to DTOs',
      'Implement custom validation where needed'
    ]
  }
];
```

### Medium-term Improvements (2-4 weeks)
```typescript
const mediumTermImprovements = [
  {
    category: 'Testing',
    description: 'Increase test coverage to 85%+',
    currentState: 'Analysis via test file discovery',
    targetState: 'Comprehensive unit and integration tests',
    actions: [
      'file_search: "**/*Test*.cs"', // Find existing tests
      'semantic_search: "test coverage gaps"', // Find untested code
      'Generate tests for critical business logic',
      'Implement integration tests for API endpoints'
    ],
    metrics: [
      'Unit test coverage >= 85%',
      'Integration test coverage >= 70%',
      'Critical path coverage = 100%'
    ]
  },
  {
    category: 'Performance',
    description: 'Implement comprehensive caching strategy',
    currentState: 'Analysis via caching pattern discovery',
    targetState: 'Multi-level caching with cache invalidation',
    actions: [
      'semantic_search: "caching strategies patterns"',
      'Implement Redis caching for frequently accessed data',
      'Add response caching for static content',
      'Implement cache invalidation patterns'
    ],
    metrics: [
      'Page load time < 2 seconds',
      'API response time < 500ms',
      'Cache hit ratio > 80%'
    ]
  }
];
```

### Long-term Strategic Improvements (1-3 months)
```typescript
const longTermImprovements = [
  {
    category: 'Architecture',
    description: 'Implement microservices architecture for scalability',
    currentState: 'Monolithic architecture with good layer separation',
    targetState: 'Domain-driven microservices with event sourcing',
    actions: [
      'semantic_search: "domain boundaries"', // Find service boundaries
      'Identify aggregate roots and bounded contexts',
      'Implement event-driven communication',
      'Design service discovery and configuration'
    ],
    benefits: [
      'Independent deployment and scaling',
      'Technology diversity per service',
      'Improved fault isolation',
      'Better team autonomy'
    ]
  },
  {
    category: 'DevOps',
    description: 'Implement comprehensive CI/CD pipeline',
    currentState: 'Basic build and deployment scripts',
    targetState: 'Automated CI/CD with quality gates',
    actions: [
      'file_search: "**/*.yml" "**/*.yaml"', // Find existing pipelines
      'Implement automated testing in pipeline',
      'Add security scanning and quality gates',
      'Implement blue-green deployment strategy'
    ],
    benefits: [
      'Reduced deployment risk',
      'Faster time to market',
      'Consistent environment promotion',
      'Automated quality assurance'
    ]
  }
];
```
```

### Implementation Priority Matrix
```markdown
**Phase 4.2: Priority Matrix and Resource Planning**
```
I'll create implementation priority matrix and resource allocation:

## 📋 Implementation Priority Matrix (Tool-Optimized)

### Impact vs Effort Analysis
```
Priority Matrix (Based on Tool Analysis):
┌─────────────────────────────────────────────────────────────┐
│ HIGH IMPACT, LOW EFFORT (Quick Wins)                       │
│ 🚀 Fix discovered SRP violations                          │
│ 🚀 Add missing input validation                           │
│ 🚀 Implement discovered error handling patterns           │
│ 🚀 Optimize identified N+1 queries                        │
├─────────────────────────────────────────────────────────────┤
│ HIGH IMPACT, HIGH EFFORT (Major Projects)                  │
│ 🎯 Comprehensive test coverage implementation             │
│ 🎯 Performance optimization across all layers             │
│ 🎯 Security hardening and audit compliance               │
│ 🎯 Scalability improvements and caching                   │
├─────────────────────────────────────────────────────────────┤
│ LOW IMPACT, LOW EFFORT (Fill-ins)                         │
│ 📝 Documentation improvements                             │
│ 📝 Code formatting and style consistency                  │
│ 📝 Logging enhancement                                    │
├─────────────────────────────────────────────────────────────┤
│ LOW IMPACT, HIGH EFFORT (Avoid/Future)                    │
│ ⏰ Complete architecture refactoring                      │
│ ⏰ Technology stack migration                             │
│ ⏰ Legacy code elimination                                │
└─────────────────────────────────────────────────────────────┘
```

### Resource Allocation Strategy
```typescript
// Resource allocation based on analysis
interface ResourceAllocation {
  timeframe: string;
  developerDays: number;
  priority: string;
  dependencies: string[];
  validationCommands: string[];
}

const resourcePlan: ResourceAllocation[] = [
  {
    timeframe: 'Week 1-2',
    developerDays: 8,
    priority: 'Critical quick wins',
    dependencies: [],
    validationCommands: [
      'get_errors: [FIXED_FILES]', // Validate fixes
      'run_in_terminal: "dotnet build"', // Build validation
      'run_in_terminal: "npm run type-check"' // Type validation
    ]
  },
  {
    timeframe: 'Week 3-6',
    developerDays: 20,
    priority: 'High-impact improvements',
    dependencies: ['Week 1-2 completion'],
    validationCommands: [
      'run_tests: [TEST_FILES]', // Test validation
      'Performance benchmarking',
      'Security vulnerability scanning'
    ]
  }
];
```

### Validation and Monitoring
```powershell
# Continuous validation commands
@copilot /code-quality comprehensive # Regular quality assessment
@copilot /performance-analysis detailed # Performance monitoring
@copilot /test comprehensive # Test coverage validation

# Monthly health checks
@copilot /codebase-analysis expert architecture report # Architecture review
@copilot /security-analysis comprehensive # Security assessment
@copilot /technical-debt-analysis actionable # Debt monitoring
```
```

## Command Activation Process
When a user types:
```
@copilot /codebase-analysis [complexity_level] [focus_area] [output_format]
```

The system should:
1. **Discovery Phase**: Use GitHub Copilot tools to comprehensively map the codebase
2. **Analysis Phase**: Analyze patterns, quality, performance, and security using semantic search and file analysis
3. **Assessment Phase**: Generate quality metrics, technical debt analysis, and improvement opportunities
4. **Recommendation Phase**: Provide prioritized, actionable recommendations with specific commands
5. **Planning Phase**: Create implementation roadmap with resource allocation and validation strategies
6. **Validation Framework**: Establish ongoing monitoring and quality assurance processes

## Output Format Options

### Summary Format
- High-level overview with key metrics
- Top 5 critical issues and quick wins
- Overall quality score and recommendations

### Detailed Format
- Comprehensive analysis across all areas
- Detailed technical findings with code examples
- Complete improvement roadmap with timelines

### Actionable Format
- Prioritized action items with specific commands
- Resource allocation and timeline estimates
- Validation commands for each improvement

### Report Format
- Executive summary with business impact
- Technical analysis with supporting evidence
- Strategic recommendations with ROI analysis

## Notes
- All analysis uses GitHub Copilot's native tools for accuracy and consistency
- Analysis adapts to complexity level and focus area for targeted insights
- Recommendations are prioritized by impact and effort for practical implementation
- Validation commands ensure continuous improvement tracking
- All recommendations follow industry best practices and modern development standards
