---
mode: agent
description: "TypeScript code review with type safety analysis and best practice recommendations"
---

---
inputs:
  - name: scope
    description: Review scope (current, staged, unstaged, specific-files, full-codebase)
    required: false
    default: current
  - name: focus
    description: Review focus area (architecture, patterns, performance, security, types)
    required: false
    default: comprehensive
---

---
command: "/ts-review"
---
# TypeScript Review Command for GitHub Copilot

## Command Usage
```
@copilot /ts-review [scope] [focus]
```

## Purpose
This command provides comprehensive TypeScript and React code review for the Ikhtibar examination management system, focusing on type safety, modern React patterns, performance optimization, and architectural compliance with project standards.

**Input Parameters**: 
- `scope` - Review scope: `current`, `staged`, `unstaged`, `specific-files`, or `full-codebase`
- `focus` - Focus area: `architecture`, `patterns`, `performance`, `security`, or `types`

## How /ts-review Works

### Phase 1: TypeScript Codebase Analysis
```markdown
I'll help you with comprehensive TypeScript code review for the Ikhtibar project. Let me analyze the codebase and focus on your specified area.

**Phase 1.1: Parse Review Request**
```
TypeScript Review Analysis:
- **Scope**: [CURRENT/STAGED/UNSTAGED/SPECIFIC-FILES/FULL-CODEBASE]
- **Focus**: [ARCHITECTURE/PATTERNS/PERFORMANCE/SECURITY/TYPES/COMPREHENSIVE]
- **Project Context**: React 18 + TypeScript with Vite build system
- **Architecture**: Folder-per-feature with modern React patterns
```

**Phase 1.2: Scope Discovery using GitHub Copilot Tools**
```
I'll discover TypeScript files in the specified scope:
- get_changed_files: [SCOPE_FILTER] # Get files based on scope
- file_search: "**/*.ts" # Find TypeScript files
- file_search: "**/*.tsx" # Find React TypeScript files
- semantic_search: "TypeScript patterns" # Find existing TS patterns
- read_file: [KEY_TYPE_FILES] # Read important type definitions
- grep_search: "interface|type|enum" # Find type definitions
```

**Phase 1.3: Project TypeScript Configuration Analysis**
```
TypeScript Setup Analysis:
- [ ] **TypeScript Version**: [VERSION] (from package.json)
- [ ] **Compiler Configuration**: [TSCONFIG_ANALYSIS]
- [ ] **Strict Mode Settings**: [STRICT_CONFIG_STATUS]
- [ ] **Path Mapping**: [ALIAS_CONFIGURATION]
- [ ] **Build Target**: [ES_TARGET_VERSION]
- [ ] **Module System**: [MODULE_RESOLUTION]
- [ ] **React Integration**: [JSX_CONFIGURATION]
- [ ] **Testing Setup**: [JEST_TS_CONFIGURATION]
```
```

### Phase 2: Focus-Specific TypeScript Review

#### For Focus: `types`
```markdown
**Phase 2.1: Type Safety Analysis using GitHub Copilot Tools**
```
I'll perform comprehensive type safety analysis:

## 🔍 TypeScript Type Safety Review (Tool-Enhanced)

### Type Definition Analysis
```powershell
# Comprehensive type analysis using GitHub Copilot tools
grep_search: "interface.*{|type.*=" # Find type definitions
semantic_search: "type definitions patterns" # Find typing patterns
file_search: "**/*.types.ts" # Find type declaration files
read_file: [TYPE_DEFINITION_FILES] # Read existing type files
get_errors: [TYPESCRIPT_FILES] # Check TypeScript compilation errors
```

### Type Safety Assessment (Tool-Measured)
#### Interface and Type Definitions
```typescript
// Analysis results from type definition discovery
interface CodebaseTypeAnalysis {
  totalInterfaces: number; // Discovered via grep_search
  totalTypes: number; // Discovered via grep_search
  totalEnums: number; // Discovered via grep_search
  genericUsage: number; // Found via pattern analysis
  utilityTypes: string[]; // Found via semantic analysis
  customTypes: string[]; // Project-specific types found
}
```

#### Type Coverage Analysis (Tool-Generated)
| File Category | Type Coverage | Issues Found | Recommendations |
|---------------|---------------|--------------|------------------|
| API Services | [COVERAGE]% | [ISSUE_COUNT] | [RECOMMENDATIONS] |
| Components | [COVERAGE]% | [ISSUE_COUNT] | [RECOMMENDATIONS] |
| Hooks | [COVERAGE]% | [ISSUE_COUNT] | [RECOMMENDATIONS] |
| Utilities | [COVERAGE]% | [ISSUE_COUNT] | [RECOMMENDATIONS] |

### Type Issues Discovered (Tool-Identified)
#### Critical Type Issues
1. **Issue**: [ANY_TYPE_USAGE]
   - **File**: [FILE_PATH:LINE_NUMBER]
   - **Problem**: Excessive use of `any` type
   - **Fix**: [SPECIFIC_TYPE_RECOMMENDATION]
   - **Impact**: Type safety compromise

2. **Issue**: [MISSING_RETURN_TYPES]
   - **File**: [FILE_PATH:LINE_NUMBER]
   - **Problem**: Functions without explicit return types
   - **Fix**: [RETURN_TYPE_SPECIFICATION]
   - **Impact**: Type inference issues

#### Type Enhancement Opportunities
```typescript
// Before: Type issues found via tool analysis
const handleSubmit = (data: any) => { // ❌ any type
  // implementation
}

// After: Recommended type-safe approach
interface FormData {
  email: string;
  password: string;
  rememberMe: boolean;
}

const handleSubmit = (data: FormData): Promise<void> => {
  // implementation with proper typing
}
```

### Advanced TypeScript Patterns (Tool-Recommended)
```typescript
// Utility types usage discovered in codebase
type ApiResponse<T> = {
  data: T;
  status: 'success' | 'error';
  message?: string;
};

// Conditional types for complex scenarios
type EntityWithId<T> = T extends { id: infer U } ? T : T & { id: string };

// Template literal types for type-safe string manipulation
type EventName<T extends string> = `on${Capitalize<T>}`;

// Branded types for type safety
type UserId = string & { readonly brand: unique symbol };
type ExamId = string & { readonly brand: unique symbol };
```
```

#### For Focus: `patterns`
```markdown
**Phase 2.2: React/TypeScript Pattern Analysis using GitHub Copilot Tools**
```
I'll analyze React and TypeScript patterns:

## ⚛️ React TypeScript Pattern Review (Tool-Enhanced)

### Component Pattern Analysis
```powershell
# Component pattern discovery using GitHub Copilot tools
file_search: "src/modules/**/*.tsx" # Find all React components
semantic_search: "React component patterns" # Find component patterns
grep_search: "FC<|FunctionComponent<|React\.FC" # Find component type usage
semantic_search: "custom hooks patterns" # Find hook patterns
```

### Component Architecture Analysis (Tool-Discovered)
#### Component Type Patterns
```typescript
// Pattern analysis results from tool discovery

// 1. Component Props Interface Pattern (Status: [CONSISTENT/INCONSISTENT])
interface ComponentNameProps {
  // Props discovered via interface analysis
  children?: React.ReactNode;
  className?: string;
  onAction?: (data: ActionData) => void;
}

// 2. Component Definition Pattern (Status: [MODERN/NEEDS_UPDATE])
const ComponentName: React.FC<ComponentNameProps> = ({ 
  children, 
  className,
  onAction 
}) => {
  // Implementation analysis
};

// 3. Generic Component Pattern (Usage: [FREQUENCY])
interface GenericComponentProps<T> {
  data: T[];
  renderItem: (item: T) => React.ReactNode;
  onSelect: (item: T) => void;
}

const GenericComponent = <T,>({ data, renderItem, onSelect }: GenericComponentProps<T>) => {
  // Generic component implementation
};
```

### Hook Pattern Analysis (Tool-Evaluated)
```typescript
// Custom hook patterns discovered via semantic search

// 1. Data Fetching Hook Pattern
interface UseApiHookResult<T> {
  data: T | null;
  loading: boolean;
  error: Error | null;
  refetch: () => Promise<void>;
}

const useApiData = <T>(url: string): UseApiHookResult<T> => {
  // Hook implementation with proper typing
};

// 2. Form Hook Pattern
interface UseFormHookProps<T> {
  initialValues: T;
  validationSchema?: ValidationSchema<T>;
  onSubmit: (values: T) => Promise<void>;
}

const useForm = <T extends Record<string, any>>(props: UseFormHookProps<T>) => {
  // Form hook with generic constraints
};

// 3. State Management Hook Pattern
const useEntityState = <T extends { id: string }>(initialState: T[]) => {
  // Entity state management with proper typing
};
```

### Pattern Compliance Assessment
| Pattern Category | Compliance Score | Issues Found | Recommendations |
|------------------|------------------|--------------|------------------|
| Component Props | [SCORE]/10 | [ISSUE_LIST] | [RECOMMENDATIONS] |
| Hook Typing | [SCORE]/10 | [ISSUE_LIST] | [RECOMMENDATIONS] |
| State Management | [SCORE]/10 | [ISSUE_LIST] | [RECOMMENDATIONS] |
| Event Handlers | [SCORE]/10 | [ISSUE_LIST] | [RECOMMENDATIONS] |
| Generic Usage | [SCORE]/10 | [ISSUE_LIST] | [RECOMMENDATIONS] |
```

#### For Focus: `performance`
```markdown
**Phase 2.3: TypeScript Performance Analysis using GitHub Copilot Tools**
```
I'll analyze TypeScript compilation and runtime performance:

## ⚡ TypeScript Performance Review (Tool-Enhanced)

### Compilation Performance Analysis
```powershell
# TypeScript compilation analysis using GitHub Copilot tools
run_in_terminal: "npm run type-check -- --listFiles" # Analyze compilation files
semantic_search: "performance bottlenecks TypeScript" # Find performance issues
grep_search: "import.*from.*\"|require\(" # Analyze import patterns
file_search: "**/*.d.ts" # Find type declaration files
```

### Build Performance Metrics (Tool-Measured)
```
TypeScript Compilation Analysis:
┌─────────────────────────────────────────────────────────────┐
│ Compilation Performance:                                    │
│ • Total Files: [COUNT] TypeScript files                    │
│ • Compilation Time: [TIME]ms                               │
│ • Memory Usage: [MEMORY]MB                                 │
│ • Type Checking Time: [TIME]ms                             │
│ • Bundle Size Impact: [SIZE]KB                             │
├─────────────────────────────────────────────────────────────┤
│ Performance Issues Detected:                                │
│ • Large Type Files: [COUNT]                                │
│ • Circular Dependencies: [COUNT]                           │
│ • Unused Imports: [COUNT]                                  │
│ • Type-only Imports Missing: [COUNT]                       │
└─────────────────────────────────────────────────────────────┘
```

### Runtime Performance Optimization
```typescript
// Performance patterns discovered via analysis

// 1. Optimized Component Re-rendering
interface OptimizedComponentProps {
  data: readonly Item[]; // Immutable data for better memoization
  onSelect: (id: string) => void; // Stable callback reference
}

const OptimizedComponent = React.memo<OptimizedComponentProps>(({ data, onSelect }) => {
  // Memoized component with proper typing
  const handleSelect = useCallback((id: string) => {
    onSelect(id);
  }, [onSelect]);

  return (
    <div>
      {data.map(item => (
        <ItemComponent 
          key={item.id} 
          item={item} 
          onSelect={handleSelect}
        />
      ))}
    </div>
  );
});

// 2. Type-safe Lazy Loading
const LazyComponent = React.lazy((): Promise<{ default: React.ComponentType<ComponentProps> }> => 
  import('./ComponentName')
);

// 3. Optimized Hook Dependencies
const useOptimizedEffect = <T>(dependency: T, effect: () => void) => {
  const stableDependency = useMemo(() => dependency, [JSON.stringify(dependency)]);
  
  useEffect(() => {
    effect();
  }, [stableDependency, effect]);
};
```

### Bundle Optimization Recommendations (Tool-Generated)
1. **Tree Shaking Optimization**
   - Use type-only imports: `import type { TypeName } from './types'`
   - Avoid namespace imports: Use specific imports instead
   - Eliminate unused type definitions

2. **Code Splitting Improvements**
   - Dynamic imports for type-heavy modules
   - Route-based code splitting with proper typing
   - Component-level lazy loading with TypeScript

3. **Compilation Optimization**
   - Enable incremental compilation
   - Use project references for large codebases
   - Optimize tsconfig.json for faster builds
```

### Phase 3: Comprehensive Review Report

```markdown
**Phase 3.1: TypeScript Quality Assessment using GitHub Copilot Tools**
```
I'll generate a comprehensive quality report:

## 📊 TypeScript Review Summary (Tool-Enhanced)

### Overall TypeScript Health Score
```powershell
# Comprehensive quality assessment using GitHub Copilot tools
run_in_terminal: "npx tsc --noEmit --skipLibCheck" # Type checking
run_in_terminal: "npm run lint -- --ext .ts,.tsx" # Linting
get_errors: [ALL_TYPESCRIPT_FILES] # Error analysis
semantic_search: "best practices TypeScript React" # Pattern validation
```

### Quality Metrics (Tool-Calculated)
- [ ] **Type Safety**: [SCORE]/10 (verified with compilation check)
- [ ] **Pattern Consistency**: [SCORE]/10 (verified with pattern analysis)
- [ ] **Performance**: [SCORE]/10 (verified with build analysis)
- [ ] **Maintainability**: [SCORE]/10 (verified with complexity analysis)
- [ ] **Documentation**: [SCORE]/10 (verified with type documentation)

**Overall TypeScript Quality Score: [SCORE]/10**

### Critical Issues (Fix Immediately)
1. **Issue**: [CRITICAL_TYPE_ISSUE]
   - **Location**: [FILE_PATH:LINE_NUMBER]
   - **Impact**: [IMPACT_DESCRIPTION]
   - **Fix**: [SPECIFIC_SOLUTION]
   - **Tool Detection**: [ANALYSIS_METHOD]

### Major Issues (Fix Soon)
1. **Issue**: [MAJOR_PATTERN_ISSUE]
   - **Location**: [FILE_PATH:LINE_NUMBER]
   - **Impact**: [IMPACT_DESCRIPTION]
   - **Fix**: [SPECIFIC_SOLUTION]
   - **Tool Detection**: [ANALYSIS_METHOD]

### Best Practices Implemented ✅
- [GOOD_PATTERNS_FOUND] (discovered via pattern analysis)
- [PROPER_TYPE_USAGE] (verified via type checking)
- [PERFORMANCE_OPTIMIZATIONS] (confirmed via build analysis)

### Recommended Improvements
```typescript
// Type-safe API integration example
interface ApiEndpoints {
  users: {
    get: (id: string) => Promise<User>;
    create: (data: CreateUserRequest) => Promise<User>;
    update: (id: string, data: UpdateUserRequest) => Promise<User>;
  };
  exams: {
    get: (id: string) => Promise<Exam>;
    create: (data: CreateExamRequest) => Promise<Exam>;
  };
}

// Type-safe event handling
type EventMap = {
  'user:created': { user: User };
  'exam:started': { examId: string; userId: string };
  'exam:completed': { examId: string; userId: string; score: number };
};

type EventHandler<K extends keyof EventMap> = (data: EventMap[K]) => void;

// Enhanced component prop validation
interface StrictComponentProps {
  readonly data: ReadonlyArray<Item>;
  readonly loading: boolean;
  readonly error: Error | null;
  readonly onAction: (action: Action) => void;
}
```

### Validation Commands (Tool-Generated)
```powershell
# TypeScript validation commands
npm run type-check # Full type checking
npx tsc --noEmit --skipLibCheck # Quick type validation
npm run lint -- --ext .ts,.tsx --fix # Auto-fix linting issues
npm run test -- --coverage # Test coverage with types

# Build validation
npm run build # Production build validation
npm run build:analyze # Bundle analysis

# Performance validation
npx tsc --generateTrace trace # Compilation performance trace
npx bundle-analyzer build/static/js/*.js # Bundle size analysis
```
```

## Command Activation Process
When a user types:
```
@copilot /ts-review [scope] [focus]
```

The system should:
1. **Parse Parameters**: Extract scope and focus parameters
2. **Discover Files**: Use GitHub Copilot tools to find TypeScript files in scope
3. **Analyze Focus Area**: Run focus-specific analysis with tool integration
4. **Generate Report**: Create comprehensive review with actionable recommendations
5. **Provide Validation**: Include commands for verification and improvement

## Notes
- All TypeScript analysis uses GitHub Copilot's native tools for maximum accuracy
- Review adapts to Ikhtibar project-specific patterns and React architecture
- Type safety is prioritized throughout all recommendations
- Performance considerations include both compilation and runtime optimization
- Modern React 18 patterns and TypeScript 4.9+ features are recommended
- Internationalization (en/ar) considerations are included in component typing
- All recommendations include validation commands for immediate verification
