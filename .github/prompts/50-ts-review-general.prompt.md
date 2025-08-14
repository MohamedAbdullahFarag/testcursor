---
mode: agent
description: "Comprehensive TypeScript/React codebase review focusing on architecture, patterns, performance, and best practices"
---

# TypeScript General Review - Comprehensive Frontend Codebase Analysis

## Purpose
This prompt performs a comprehensive review of the TypeScript/React frontend codebase, focusing on architecture quality, component patterns, performance optimization, and adherence to modern React/TypeScript best practices within the Ikhtibar project.

## Review Scope
The review covers:
- Overall project architecture and structure
- TypeScript usage and type safety
- React component patterns and organization
- Performance optimization and bundle analysis
- Security and validation patterns
- Testing coverage and quality
- Tooling and development experience

## Review Process

### Step 1: Architecture Analysis
```markdown
I'll analyze the overall frontend architecture:

**Project Structure Assessment:**
- Component organization and modularity
- Folder-per-feature implementation
- Shared component identification
- Import/export patterns
- Module boundaries and dependencies

**Architecture Pattern Evaluation:**
- React patterns and best practices
- State management architecture
- API integration patterns
- Routing and navigation structure
- Error boundary implementation
```

### Step 2: Code Quality Analysis
```markdown
I'll assess code quality across multiple dimensions:

**TypeScript Quality:**
- Strict mode compliance
- Type safety and explicit typing
- Interface definitions and exports
- Generic usage and constraints
- Utility type usage

**React Best Practices:**
- Component composition patterns
- Hook usage and custom hooks
- Performance optimization patterns
- Accessibility implementation
- Error handling strategies
```

## Review Focus Areas

### 1. **Architecture & Structure Quality**

#### Component Organization
```markdown
✅ **Architecture Assessment:**
- [ ] **Folder-per-Feature**: Each module is self-contained with clear boundaries
- [ ] **Component Hierarchy**: Logical component composition and reusability
- [ ] **Shared Components**: Only truly reusable components in shared folder
- [ ] **Module Dependencies**: No circular dependencies or tight coupling
- [ ] **Import Patterns**: Consistent relative imports within features, absolute for shared

**Expected Structure Validation:**
```
frontend/src/
├── modules/[feature]/
│   ├── components/        # Feature-specific UI components
│   ├── hooks/            # Feature state management
│   ├── services/         # API integration
│   ├── types/           # Feature type definitions
│   └── views/           # Page components
├── shared/
│   ├── components/      # Truly reusable UI components
│   ├── hooks/          # Shared state management
│   ├── services/       # Common API utilities
│   ├── types/         # Global type definitions
│   └── utils/         # Helper functions
├── layout/            # Application layout components
└── pages/            # Route page components
```

**Architecture Quality Metrics:**
- Component count and size distribution
- Dependency graph complexity
- Code reusability percentage
- Module coupling analysis
```

#### State Management Patterns
```markdown
✅ **State Management Assessment:**
- [ ] **Hook Organization**: Custom hooks follow single responsibility
- [ ] **State Lifting**: Proper state elevation and prop drilling avoidance
- [ ] **Context Usage**: Appropriate context API usage for shared state
- [ ] **Query Management**: TanStack Query for server state management
- [ ] **Form State**: Proper form state management with validation

**State Management Patterns:**
```typescript
// ✅ Good state management pattern
export const useUserManagement = () => {
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [isEditing, setIsEditing] = useState(false);
  
  const { data: users, isLoading, error } = useUsers();
  
  const handleUserSelect = useCallback((user: User) => {
    setSelectedUser(user);
    setIsEditing(false);
  }, []);
  
  const handleEditToggle = useCallback(() => {
    setIsEditing(prev => !prev);
  }, []);
  
  return {
    users,
    selectedUser,
    isEditing,
    isLoading,
    error,
    handleUserSelect,
    handleEditToggle
  };
};

// ❌ Poor state management to flag
export const useEverything = () => {
  // Mixing multiple concerns in one hook
  const [users, setUsers] = useState([]);
  const [posts, setPosts] = useState([]);
  const [auth, setAuth] = useState(null);
  // ... multiple unrelated state concerns
};
```
```

### 2. **TypeScript Quality Assessment**

#### Type Safety and Strict Mode Compliance
```markdown
✅ **TypeScript Quality Standards:**
- [ ] **Strict Mode**: All files comply with TypeScript strict mode
- [ ] **Explicit Typing**: No implicit `any` types, explicit return types
- [ ] **Interface Definitions**: Proper interfaces for all component props
- [ ] **Type Imports**: Using `import type` for type-only imports
- [ ] **Generic Constraints**: Proper generic usage with constraints
- [ ] **Utility Types**: Effective use of TypeScript utility types

**Type Safety Patterns:**
```typescript
// ✅ Excellent TypeScript usage
interface UserCardProps {
  user: User;
  onEdit: (user: User) => void;
  onDelete: (userId: string) => Promise<void>;
  className?: string;
  variant?: 'default' | 'compact' | 'detailed';
}

const UserCard: React.FC<UserCardProps> = memo(({
  user,
  onEdit,
  onDelete,
  className = '',
  variant = 'default'
}) => {
  const { t } = useTranslation('users');
  
  const handleDelete = useCallback(async () => {
    try {
      await onDelete(user.id);
      // Success notification
    } catch (error) {
      // Error handling with proper typing
      if (error instanceof Error) {
        console.error('Delete failed:', error.message);
      }
    }
  }, [user.id, onDelete]);
  
  return (
    <article className={`user-card user-card--${variant} ${className}`}>
      {/* Component content */}
    </article>
  );
});

// ❌ Poor TypeScript patterns to flag
const UserCard = ({ user, onEdit, ...props }: any) => {  // Any usage
  const handleClick = (e) => {  // No event typing
    onEdit(user);  // No validation
  };
  
  return <div onClick={handleClick}>{user.name}</div>;  // No accessibility
};
```
```

#### API Integration and Data Models
```markdown
✅ **API Integration Quality:**
- [ ] **Response Typing**: All API responses have proper TypeScript interfaces
- [ ] **Error Handling**: Typed error responses and proper error boundaries
- [ ] **Loading States**: Consistent loading and error state management
- [ ] **Data Validation**: Runtime validation with libraries like Zod
- [ ] **Cache Management**: Proper query key management and cache invalidation

**API Integration Patterns:**
```typescript
// ✅ Well-typed API service
export interface User {
  id: string;
  name: string;
  email: string;
  roles: Role[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateUserRequest {
  name: string;
  email: string;
  roleIds: string[];
}

export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
}

export interface ApiError {
  message: string;
  code: string;
  details?: Record<string, string[]>;
}

export const userService = {
  async getUsers(): Promise<ApiResponse<User[]>> {
    const response = await apiClient.get<ApiResponse<User[]>>('/api/users');
    return response.data;
  },
  
  async createUser(data: CreateUserRequest): Promise<ApiResponse<User>> {
    const response = await apiClient.post<ApiResponse<User>>('/api/users', data);
    return response.data;
  }
};

// Custom hook with proper error handling
export const useUsers = () => {
  return useQuery<ApiResponse<User[]>, ApiError>({
    queryKey: ['users'],
    queryFn: userService.getUsers,
    onError: (error) => {
      console.error('Failed to fetch users:', error.message);
    }
  });
};
```
```

### 3. **React Component Quality**

#### Component Design and Patterns
```markdown
✅ **Component Quality Standards:**
- [ ] **Single Responsibility**: Each component has one clear purpose
- [ ] **Composition Over Inheritance**: Proper component composition patterns
- [ ] **Performance Optimization**: Appropriate use of memo, useCallback, useMemo
- [ ] **Accessibility**: ARIA attributes and semantic HTML
- [ ] **Internationalization**: i18next integration for all user-facing text
- [ ] **Error Boundaries**: Proper error handling and fallback UI
- [ ] **Testing**: Component tests with good coverage

**Component Quality Patterns:**
```typescript
// ✅ High-quality component example
interface UserListProps {
  users: User[];
  onUserSelect: (user: User) => void;
  onUserDelete: (userId: string) => Promise<void>;
  loading?: boolean;
  error?: Error | null;
  className?: string;
}

const UserList: React.FC<UserListProps> = memo(({
  users,
  onUserSelect,
  onUserDelete,
  loading = false,
  error = null,
  className = ''
}) => {
  const { t } = useTranslation('users');
  
  const handleUserSelect = useCallback((user: User) => {
    onUserSelect(user);
  }, [onUserSelect]);
  
  const sortedUsers = useMemo(() => {
    return [...users].sort((a, b) => a.name.localeCompare(b.name));
  }, [users]);
  
  if (loading) {
    return (
      <div className="user-list__loading" role="status" aria-live="polite">
        <LoadingSpinner />
        <span className="sr-only">{t('loading.users')}</span>
      </div>
    );
  }
  
  if (error) {
    return (
      <ErrorBoundary error={error} onRetry={() => window.location.reload()}>
        <div className="user-list__error" role="alert">
          {t('error.loadingUsers')}
        </div>
      </ErrorBoundary>
    );
  }
  
  return (
    <div className={`user-list ${className}`} role="list">
      {sortedUsers.map(user => (
        <UserCard
          key={user.id}
          user={user}
          onEdit={handleUserSelect}
          onDelete={onUserDelete}
          className="user-list__item"
        />
      ))}
      {sortedUsers.length === 0 && (
        <div className="user-list__empty" role="status">
          {t('empty.noUsers')}
        </div>
      )}
    </div>
  );
});

UserList.displayName = 'UserList';

// ❌ Poor component patterns to flag
const UserList = ({ users, onSelect }) => {  // No TypeScript
  return (
    <div>
      {users.map(user => (  // No key prop
        <div onClick={() => onSelect(user)}>  // No accessibility
          {user.name}  // Hardcoded text
        </div>
      ))}
    </div>
  );
};
```
```

### 4. **Performance Analysis**

#### Bundle and Performance Optimization
```markdown
✅ **Performance Assessment:**
- [ ] **Bundle Size**: Optimized bundle size with code splitting
- [ ] **Lazy Loading**: Proper lazy loading for routes and components
- [ ] **Memoization**: Appropriate use of React optimization hooks
- [ ] **Image Optimization**: Proper image loading and sizing
- [ ] **API Optimization**: Efficient data fetching and caching
- [ ] **Memory Leaks**: Proper cleanup in useEffect hooks

**Performance Optimization Patterns:**
```typescript
// ✅ Performance-optimized patterns
// Route-level code splitting
const UserManagement = lazy(() => import('./modules/users/views/UserManagement'));
const QuestionBank = lazy(() => import('./modules/questions/views/QuestionBank'));

// Component-level optimization
const ExpensiveComponent = memo(({ data }: ExpensiveComponentProps) => {
  const expensiveValue = useMemo(() => {
    return data.reduce((acc, item) => acc + item.value, 0);
  }, [data]);
  
  const handleClick = useCallback((id: string) => {
    // Handle click
  }, []);
  
  return (
    <div>
      {/* Render expensive computation */}
    </div>
  );
});

// Efficient data fetching
export const useUsersWithPagination = (page: number, pageSize: number) => {
  return useQuery({
    queryKey: ['users', page, pageSize],
    queryFn: () => userService.getUsers({ page, pageSize }),
    keepPreviousData: true,  // Smooth pagination
    staleTime: 5 * 60 * 1000,  // 5 minutes cache
  });
};

// ❌ Performance issues to flag
const SlowComponent = ({ users }) => {
  // Expensive operation on every render
  const total = users.reduce((acc, user) => acc + user.score, 0);
  
  return (
    <div>
      {users.map(user => (
        <div key={user.id} onClick={() => alert(user.name)}>  // Inline function
          {user.name}
        </div>
      ))}
    </div>
  );
};
```
```

### 5. **Security and Validation**

#### Security Best Practices
```markdown
✅ **Security Assessment:**
- [ ] **Input Validation**: All user inputs validated client and server-side
- [ ] **XSS Prevention**: Proper input sanitization and output encoding
- [ ] **Authentication**: Secure token management and refresh flows
- [ ] **Authorization**: Route-level and component-level access control
- [ ] **Environment Variables**: Proper environment variable management
- [ ] **Content Security Policy**: CSP headers implemented
- [ ] **Dependency Security**: Regular security audits and updates

**Security Implementation Patterns:**
```typescript
// ✅ Secure patterns
// Input validation with runtime checking
import { z } from 'zod';

const CreateUserSchema = z.object({
  name: z.string().min(1).max(100),
  email: z.string().email(),
  roleIds: z.array(z.string().uuid())
});

type CreateUserData = z.infer<typeof CreateUserSchema>;

export const useCreateUser = () => {
  return useMutation({
    mutationFn: async (data: CreateUserData) => {
      // Runtime validation
      const validatedData = CreateUserSchema.parse(data);
      return userService.createUser(validatedData);
    },
    onError: (error) => {
      if (error instanceof z.ZodError) {
        // Handle validation errors
        console.error('Validation failed:', error.errors);
      }
    }
  });
};

// Protected route component
const ProtectedRoute: React.FC<{ children: React.ReactNode; requiredRole?: string }> = ({
  children,
  requiredRole
}) => {
  const { user, isAuthenticated } = useAuth();
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  
  if (requiredRole && !user?.roles.includes(requiredRole)) {
    return <AccessDenied />;
  }
  
  return <>{children}</>;
};
```
```

### 6. **Testing Quality**

#### Test Coverage and Quality
```markdown
✅ **Testing Assessment:**
- [ ] **Unit Tests**: All hooks and utilities tested
- [ ] **Component Tests**: React components tested with user interactions
- [ ] **Integration Tests**: API integration and data flow tested
- [ ] **E2E Tests**: Critical user flows automated
- [ ] **Test Quality**: Tests are maintainable and reliable
- [ ] **Mock Usage**: Proper mocking of external dependencies
- [ ] **Coverage Goals**: >80% code coverage target

**Testing Patterns:**
```typescript
// ✅ High-quality component test
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { vi } from 'vitest';
import { UserList } from '../UserList';

const mockUsers: User[] = [
  { id: '1', name: 'John Doe', email: 'john@example.com', roles: [] },
  { id: '2', name: 'Jane Smith', email: 'jane@example.com', roles: [] }
];

const renderWithQueryClient = (ui: React.ReactElement) => {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } }
  });
  
  return render(
    <QueryClientProvider client={queryClient}>
      {ui}
    </QueryClientProvider>
  );
};

describe('UserList', () => {
  it('should render users correctly', () => {
    const onUserSelect = vi.fn();
    const onUserDelete = vi.fn();
    
    renderWithQueryClient(
      <UserList
        users={mockUsers}
        onUserSelect={onUserSelect}
        onUserDelete={onUserDelete}
      />
    );
    
    expect(screen.getByText('John Doe')).toBeInTheDocument();
    expect(screen.getByText('Jane Smith')).toBeInTheDocument();
  });
  
  it('should call onUserSelect when user is clicked', async () => {
    const onUserSelect = vi.fn();
    const onUserDelete = vi.fn();
    
    renderWithQueryClient(
      <UserList
        users={mockUsers}
        onUserSelect={onUserSelect}
        onUserDelete={onUserDelete}
      />
    );
    
    fireEvent.click(screen.getByText('John Doe'));
    
    await waitFor(() => {
      expect(onUserSelect).toHaveBeenCalledWith(mockUsers[0]);
    });
  });
  
  it('should show loading state', () => {
    renderWithQueryClient(
      <UserList
        users={[]}
        onUserSelect={vi.fn()}
        onUserDelete={vi.fn()}
        loading={true}
      />
    );
    
    expect(screen.getByRole('status')).toBeInTheDocument();
    expect(screen.getByText(/loading/i)).toBeInTheDocument();
  });
});

// ✅ Custom hook test
describe('useUsers', () => {
  it('should fetch users successfully', async () => {
    const mockUsers = [{ id: '1', name: 'John' }];
    vi.mocked(userService.getUsers).mockResolvedValue({
      data: mockUsers,
      success: true
    });
    
    const { result } = renderHook(() => useUsers(), {
      wrapper: QueryClientProvider
    });
    
    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
      expect(result.current.data).toEqual(mockUsers);
    });
  });
});
```
```

## Implementation Process

### Review Execution
```markdown
I'll perform a systematic review of the TypeScript/React codebase:

**Phase 1: Project Analysis**
- Analyze project structure and organization
- Review package.json and dependencies
- Check TypeScript configuration and strict mode
- Assess build and development tooling

**Phase 2: Code Quality Assessment**
- Review component patterns and organization
- Analyze TypeScript usage and type safety
- Check React best practices adherence
- Evaluate performance optimization patterns

**Phase 3: Architecture Validation**
- Validate folder-per-feature implementation
- Check module boundaries and dependencies
- Review state management patterns
- Assess API integration architecture

**Phase 4: Quality Metrics Collection**
- Bundle size analysis
- Test coverage assessment
- Performance metrics gathering
- Security vulnerability scanning
```

### Review Report Generation
```markdown
# TypeScript/React Codebase Review Report

## Executive Summary
- **Project Health**: [Overall assessment]
- **Architecture Quality**: [Grade A-F]
- **TypeScript Compliance**: [Percentage]
- **React Best Practices**: [Grade A-F]
- **Performance Score**: [Grade A-F]
- **Security Assessment**: [Grade A-F]

## Critical Findings

### 🔴 Critical Issues (Fix Immediately)
- [Security vulnerabilities]
- [Performance bottlenecks]
- [Architecture violations]

### 🟡 Improvement Areas (Next Sprint)
- [Code quality improvements]
- [Pattern inconsistencies]
- [Missing best practices]

### 🟢 Optimization Opportunities (Future)
- [Performance enhancements]
- [Developer experience improvements]
- [Maintainability enhancements]

## Detailed Analysis

### Architecture Assessment
- **Structure Quality**: [Assessment]
- **Module Organization**: [Evaluation]
- **Dependency Management**: [Analysis]
- **Pattern Consistency**: [Review]

### TypeScript Quality
- **Type Safety**: [Coverage percentage]
- **Strict Mode Compliance**: [Status]
- **Interface Quality**: [Assessment]
- **Generic Usage**: [Evaluation]

### React Component Quality
- **Component Design**: [Assessment]
- **Performance Optimization**: [Status]
- **Accessibility**: [Coverage]
- **Testing**: [Coverage percentage]

### Performance Metrics
- **Bundle Size**: [Size analysis]
- **Load Time**: [Performance metrics]
- **Memory Usage**: [Assessment]
- **Optimization Level**: [Grade]

## Recommendations

### Immediate Actions
1. [Critical fixes with file references]
2. [Security improvements]
3. [Performance optimizations]

### Short-term Improvements
1. [Code quality enhancements]
2. [Pattern standardization]
3. [Testing improvements]

### Long-term Strategy
1. [Architecture evolution]
2. [Performance optimization strategy]
3. [Developer experience enhancements]

## Quality Metrics Dashboard
```
Overall Score: X/100
├── Architecture: X/25
├── TypeScript: X/25
├── React Patterns: X/25
└── Performance: X/25

Bundle Size: X MB (Target: <2MB)
Test Coverage: X% (Target: >80%)
TypeScript Compliance: X%
Performance Score: X/100
```
```

## Validation Commands

### Analysis Commands
```bash
# Project structure analysis
tree -I 'node_modules|dist|.git' -L 3

# TypeScript analysis
npm run type-check

# Bundle analysis
npm run build && du -sh dist/

# Test coverage
npm run test:coverage

# Linting
npm run lint

# Dependency analysis
npm audit
```

### Quality Validation
```bash
# Build validation
npm run build

# Type checking
npm run type-check

# All tests
npm run test

# Performance testing
npm run lighthouse
```

## Success Criteria
- Comprehensive architecture assessment provided
- TypeScript quality evaluated with specific improvements
- React patterns analyzed with best practice recommendations
- Performance bottlenecks identified with optimization strategies
- Security assessment completed with actionable fixes
- Testing coverage evaluated with improvement plan
- Clear prioritization of improvements with effort estimates
- Actionable recommendations with code examples
- Quality metrics dashboard for tracking progress

This comprehensive TypeScript/React review ensures the frontend codebase maintains high quality, follows modern best practices, and provides excellent developer and user experiences.
