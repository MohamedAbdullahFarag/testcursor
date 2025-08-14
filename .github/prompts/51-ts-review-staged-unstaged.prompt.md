---
mode: agent
description: "Review TypeScript/React staged and unstaged changes focusing on quality, performance, and best practices"
---

# TypeScript Staged/Unstaged Review - Frontend Change Analysis

## Purpose
This prompt performs a detailed review of TypeScript/React files in the staging area (both staged and unstaged), analyzing specific changes for quality, performance, security, and adherence to React/TypeScript best practices within the Ikhtibar project.

## Review Scope
The review targets:
- All TypeScript/React files in the git staging area
- Modified frontend files not yet staged
- New components and hooks added
- Updated type definitions and interfaces
- Changes to API integration and services
- Previous review context integration

## Review Process

### Step 1: Frontend Change Discovery
```markdown
I'll systematically identify and analyze all frontend changes:

**Change Detection:**
- Use `git status` to identify TypeScript/React file changes
- Use `git diff --staged` for staged frontend changes
- Use `git diff` for unstaged TypeScript/React changes
- Filter for relevant file extensions (.ts, .tsx, .js, .jsx)
- Analyze both new files and modifications

**Frontend-Specific Analysis:**
- Component structure changes
- Hook implementations and updates
- Type definition modifications
- API service integration changes
- Style and layout adjustments
```

### Step 2: Change Impact Assessment
```markdown
For each TypeScript/React file change, I'll analyze:

**Code Quality Impact:**
- TypeScript type safety improvements or regressions
- React pattern adherence and best practices
- Performance optimization additions or concerns
- Accessibility enhancements or issues

**Integration Impact:**
- Component prop interface changes
- Hook signature modifications
- API contract updates
- State management changes
```

## Review Focus Areas

### 1. **TypeScript Quality for Changes**

#### Type Safety and Interface Changes
```markdown
✅ **TypeScript Change Validation:**
- [ ] **New Interfaces**: Proper TypeScript interfaces for new components/props
- [ ] **Type Imports**: Using `import type` for type-only imports
- [ ] **Explicit Typing**: No `any` usage, explicit return types for functions
- [ ] **Generic Constraints**: Proper generic usage with appropriate constraints
- [ ] **Breaking Changes**: Interface changes maintain backward compatibility
- [ ] **Utility Types**: Effective use of TypeScript utility types where appropriate

**TypeScript Change Analysis:**
```typescript
// ✅ Good TypeScript change example
// BEFORE (if applicable)
interface UserProps {
  user: any;
  onEdit: Function;
}

// AFTER - Improved with proper typing
interface UserCardProps {
  user: User;
  onEdit: (user: User) => void;
  onDelete?: (userId: string) => Promise<void>;
  variant?: 'default' | 'compact' | 'detailed';
  className?: string;
}

const UserCard: React.FC<UserCardProps> = memo(({
  user,
  onEdit,
  onDelete,
  variant = 'default',
  className = ''
}) => {
  const handleEdit = useCallback(() => {
    onEdit(user);
  }, [user, onEdit]);
  
  return (
    <article className={`user-card user-card--${variant} ${className}`}>
      {/* Component content */}
    </article>
  );
});

// ❌ Poor TypeScript change to flag
interface UserProps {
  user: any;  // Still using any
  onEdit: (data: any) => any;  // Weak typing
}

const UserCard = ({ user, onEdit, ...props }: UserProps) => {
  return <div onClick={() => onEdit(user)}>{user.name}</div>;  // No memoization
};
```
```

#### API Integration Type Changes
```markdown
✅ **API Type Change Validation:**
- [ ] **Response Types**: All API responses have proper TypeScript interfaces
- [ ] **Request Types**: API request payloads properly typed
- [ ] **Error Types**: Error responses typed with specific error interfaces
- [ ] **Query Keys**: TanStack Query keys properly typed and consistent
- [ ] **Service Methods**: API service methods have explicit return types

**API Integration Change Analysis:**
```typescript
// ✅ Well-typed API change
// BEFORE
const getUsers = async () => {
  const response = await fetch('/api/users');
  return response.json();
};

// AFTER - Improved with proper typing
export interface User {
  id: string;
  name: string;
  email: string;
  roles: Role[];
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface GetUsersResponse {
  users: User[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface ApiError {
  message: string;
  code: string;
  details?: Record<string, string[]>;
}

export const userService = {
  async getUsers(params?: GetUsersParams): Promise<GetUsersResponse> {
    const response = await apiClient.get<GetUsersResponse>('/api/users', { params });
    return response.data;
  }
};

export const useUsers = (params?: GetUsersParams) => {
  return useQuery<GetUsersResponse, ApiError>({
    queryKey: ['users', params],
    queryFn: () => userService.getUsers(params),
    staleTime: 5 * 60 * 1000
  });
};

// ❌ Poor API typing to flag
const useUsers = () => {
  return useQuery({  // No type parameters
    queryKey: ['users'],
    queryFn: async () => {
      const response = await fetch('/api/users');
      return response.json();  // Untyped response
    }
  });
};
```
```

### 2. **React Component Quality Changes**

#### Component Pattern Improvements
```markdown
✅ **React Change Validation:**
- [ ] **Component Structure**: Proper functional component with TypeScript
- [ ] **Props Interface**: Well-defined props interface with proper types
- [ ] **Performance**: Appropriate use of memo, useCallback, useMemo for optimization
- [ ] **Accessibility**: ARIA attributes and semantic HTML additions
- [ ] **Internationalization**: i18next integration for new user-facing text
- [ ] **Error Handling**: Proper error boundaries and error state management
- [ ] **State Management**: Appropriate hook usage and state lifting

**React Component Change Analysis:**
```typescript
// ✅ Excellent React component change
// BEFORE (if showing improvement)
const UserList = ({ users, onSelect }) => {
  return (
    <div>
      {users.map(user => (
        <div key={user.id} onClick={() => onSelect(user)}>
          {user.name}
        </div>
      ))}
    </div>
  );
};

// AFTER - Comprehensive improvement
interface UserListProps {
  users: User[];
  onUserSelect: (user: User) => void;
  onUserDelete: (userId: string) => Promise<void>;
  loading?: boolean;
  error?: Error | null;
  selectedUserId?: string;
  className?: string;
}

const UserList: React.FC<UserListProps> = memo(({
  users,
  onUserSelect,
  onUserDelete,
  loading = false,
  error = null,
  selectedUserId,
  className = ''
}) => {
  const { t } = useTranslation('users');
  
  const sortedUsers = useMemo(() => {
    return [...users].sort((a, b) => a.name.localeCompare(b.name));
  }, [users]);
  
  const handleUserSelect = useCallback((user: User) => {
    onUserSelect(user);
  }, [onUserSelect]);
  
  const handleUserDelete = useCallback(async (userId: string) => {
    try {
      await onUserDelete(userId);
    } catch (error) {
      console.error('Failed to delete user:', error);
    }
  }, [onUserDelete]);
  
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
      <div className="user-list__error" role="alert">
        <ErrorMessage 
          error={error} 
          onRetry={() => window.location.reload()}
        />
      </div>
    );
  }
  
  if (sortedUsers.length === 0) {
    return (
      <div className="user-list__empty" role="status">
        {t('empty.noUsers')}
      </div>
    );
  }
  
  return (
    <div className={`user-list ${className}`} role="list">
      {sortedUsers.map(user => (
        <UserCard
          key={user.id}
          user={user}
          onEdit={handleUserSelect}
          onDelete={handleUserDelete}
          isSelected={user.id === selectedUserId}
          className="user-list__item"
        />
      ))}
    </div>
  );
});

UserList.displayName = 'UserList';

// ❌ Poor React patterns to flag
const UserList = ({ users, onSelect }: any) => {  // Any usage
  const [loading, setLoading] = useState(false);  // Misplaced loading state
  
  return (
    <div>
      {users.map(user => (  // Missing key prop type checking
        <div onClick={() => onSelect(user)}>  // No accessibility
          {user.name}  // Hardcoded text
        </div>
      ))}
    </div>
  );
};
```
```

#### Custom Hook Changes
```markdown
✅ **Hook Change Validation:**
- [ ] **Single Responsibility**: Hook manages single concern or related state
- [ ] **Return Type**: Explicit return type interface defined
- [ ] **Dependency Array**: Proper dependency arrays in useEffect and useCallback
- [ ] **Cleanup**: Proper cleanup in useEffect for subscriptions/timers
- [ ] **Error Handling**: Error states managed and exposed appropriately
- [ ] **Performance**: Optimized with useCallback and useMemo where needed

**Custom Hook Change Analysis:**
```typescript
// ✅ Well-designed custom hook change
// BEFORE (if showing improvement)
const useUserData = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);
  
  useEffect(() => {
    fetchUsers().then(setUsers);
  }, []);
  
  return { users, loading };
};

// AFTER - Comprehensive improvement
interface UseUserManagementReturn {
  users: User[];
  selectedUser: User | null;
  isLoading: boolean;
  error: Error | null;
  createUser: (data: CreateUserRequest) => Promise<User>;
  updateUser: (id: string, data: UpdateUserRequest) => Promise<User>;
  deleteUser: (id: string) => Promise<void>;
  selectUser: (user: User | null) => void;
  refreshUsers: () => Promise<void>;
}

export const useUserManagement = (): UseUserManagementReturn => {
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const queryClient = useQueryClient();
  
  const { 
    data: users = [], 
    isLoading, 
    error,
    refetch: refreshUsers
  } = useQuery<User[], ApiError>({
    queryKey: ['users'],
    queryFn: userService.getUsers,
    staleTime: 5 * 60 * 1000,
    onError: (error) => {
      console.error('Failed to fetch users:', error.message);
    }
  });
  
  const createUserMutation = useMutation<User, ApiError, CreateUserRequest>({
    mutationFn: userService.createUser,
    onSuccess: (newUser) => {
      queryClient.setQueryData<User[]>(['users'], (oldUsers = []) => [
        ...oldUsers,
        newUser
      ]);
    },
    onError: (error) => {
      console.error('Failed to create user:', error.message);
    }
  });
  
  const updateUserMutation = useMutation<User, ApiError, { id: string; data: UpdateUserRequest }>({
    mutationFn: ({ id, data }) => userService.updateUser(id, data),
    onSuccess: (updatedUser) => {
      queryClient.setQueryData<User[]>(['users'], (oldUsers = []) =>
        oldUsers.map(user => user.id === updatedUser.id ? updatedUser : user)
      );
      if (selectedUser?.id === updatedUser.id) {
        setSelectedUser(updatedUser);
      }
    }
  });
  
  const deleteUserMutation = useMutation<void, ApiError, string>({
    mutationFn: userService.deleteUser,
    onSuccess: (_, deletedUserId) => {
      queryClient.setQueryData<User[]>(['users'], (oldUsers = []) =>
        oldUsers.filter(user => user.id !== deletedUserId)
      );
      if (selectedUser?.id === deletedUserId) {
        setSelectedUser(null);
      }
    }
  });
  
  const selectUser = useCallback((user: User | null) => {
    setSelectedUser(user);
  }, []);
  
  const createUser = useCallback(async (data: CreateUserRequest): Promise<User> => {
    const result = await createUserMutation.mutateAsync(data);
    return result;
  }, [createUserMutation]);
  
  const updateUser = useCallback(async (id: string, data: UpdateUserRequest): Promise<User> => {
    const result = await updateUserMutation.mutateAsync({ id, data });
    return result;
  }, [updateUserMutation]);
  
  const deleteUser = useCallback(async (id: string): Promise<void> => {
    await deleteUserMutation.mutateAsync(id);
  }, [deleteUserMutation]);
  
  return {
    users,
    selectedUser,
    isLoading,
    error,
    createUser,
    updateUser,
    deleteUser,
    selectUser,
    refreshUsers
  };
};

// ❌ Poor hook patterns to flag
const useUserStuff = () => {  // Poor naming
  const [users, setUsers] = useState([]);
  const [posts, setPosts] = useState([]);  // Mixed concerns
  const [auth, setAuth] = useState(null);  // Unrelated state
  
  useEffect(() => {
    // No cleanup, no error handling
    fetchUsers().then(setUsers);
    fetchPosts().then(setPosts);  // Multiple async operations
  }, []);  // Missing dependencies
  
  return { users, posts, auth };  // No clear interface
};
```
```

### 3. **Performance Impact Assessment**

#### Performance Optimization Changes
```markdown
✅ **Performance Change Validation:**
- [ ] **Memoization**: Appropriate use of React.memo, useCallback, useMemo
- [ ] **Bundle Impact**: Changes don't significantly increase bundle size
- [ ] **Lazy Loading**: New routes/components use lazy loading appropriately
- [ ] **API Efficiency**: Changes improve or maintain API call efficiency
- [ ] **Memory Leaks**: No new memory leaks introduced with useEffect cleanup
- [ ] **Render Optimization**: Changes reduce unnecessary re-renders

**Performance Change Analysis:**
```typescript
// ✅ Performance-optimized change
// BEFORE
const UserDashboard = ({ users, filters }) => {
  const filteredUsers = users.filter(user => 
    user.name.includes(filters.search) && 
    (filters.role ? user.roles.includes(filters.role) : true)
  );
  
  const totalScore = filteredUsers.reduce((sum, user) => sum + user.score, 0);
  
  return (
    <div>
      <div>Total Score: {totalScore}</div>
      {filteredUsers.map(user => (
        <UserCard key={user.id} user={user} onEdit={() => editUser(user)} />
      ))}
    </div>
  );
};

// AFTER - Performance optimized
interface UserDashboardProps {
  users: User[];
  filters: UserFilters;
  onUserEdit: (user: User) => void;
}

const UserDashboard: React.FC<UserDashboardProps> = memo(({
  users,
  filters,
  onUserEdit
}) => {
  // Memoize expensive computations
  const filteredUsers = useMemo(() => {
    return users.filter(user => 
      user.name.toLowerCase().includes(filters.search.toLowerCase()) && 
      (filters.role ? user.roles.includes(filters.role) : true)
    );
  }, [users, filters.search, filters.role]);
  
  const totalScore = useMemo(() => {
    return filteredUsers.reduce((sum, user) => sum + user.score, 0);
  }, [filteredUsers]);
  
  // Memoize callback to prevent unnecessary re-renders
  const handleUserEdit = useCallback((user: User) => {
    onUserEdit(user);
  }, [onUserEdit]);
  
  return (
    <div className="user-dashboard">
      <div className="user-dashboard__summary">
        Total Score: {totalScore}
      </div>
      <div className="user-dashboard__list">
        {filteredUsers.map(user => (
          <UserCard 
            key={user.id} 
            user={user} 
            onEdit={handleUserEdit}
          />
        ))}
      </div>
    </div>
  );
});

// ❌ Performance issues to flag
const UserDashboard = ({ users, filters, onUserEdit }) => {
  // Expensive operation on every render
  const filteredUsers = users.filter(user => 
    expensiveFilterFunction(user, filters)  // Expensive operation
  );
  
  return (
    <div>
      {filteredUsers.map(user => (
        <UserCard 
          key={user.id} 
          user={user} 
          onEdit={() => onUserEdit(user)}  // New function every render
        />
      ))}
    </div>
  );
};
```
```

### 4. **Security and Validation Changes**

#### Security Enhancement Validation
```markdown
✅ **Security Change Validation:**
- [ ] **Input Validation**: New forms include proper validation with schemas
- [ ] **XSS Prevention**: User inputs properly sanitized and encoded
- [ ] **Authentication**: Protected routes and components have proper auth checks
- [ ] **Authorization**: Role-based access control implemented correctly
- [ ] **Error Handling**: Error messages don't expose sensitive information
- [ ] **Data Sanitization**: User-generated content properly sanitized

**Security Change Analysis:**
```typescript
// ✅ Secure implementation change
// BEFORE
const UserForm = ({ user, onSave }) => {
  const [formData, setFormData] = useState(user || {});
  
  const handleSubmit = (e) => {
    e.preventDefault();
    onSave(formData);  // No validation
  };
  
  return (
    <form onSubmit={handleSubmit}>
      <input 
        value={formData.name} 
        onChange={(e) => setFormData({...formData, name: e.target.value})}
      />
    </form>
  );
};

// AFTER - Secure with validation
import { z } from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

const UserFormSchema = z.object({
  name: z.string()
    .min(1, 'Name is required')
    .max(100, 'Name must be less than 100 characters')
    .regex(/^[a-zA-Z\s]+$/, 'Name must contain only letters and spaces'),
  email: z.string()
    .email('Invalid email format')
    .max(255, 'Email must be less than 255 characters'),
  roles: z.array(z.string().uuid('Invalid role ID'))
    .min(1, 'At least one role is required')
});

type UserFormData = z.infer<typeof UserFormSchema>;

interface UserFormProps {
  user?: User;
  onSave: (data: UserFormData) => Promise<void>;
  onCancel: () => void;
}

const UserForm: React.FC<UserFormProps> = memo(({ user, onSave, onCancel }) => {
  const { t } = useTranslation('users');
  
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    setError
  } = useForm<UserFormData>({
    resolver: zodResolver(UserFormSchema),
    defaultValues: {
      name: user?.name || '',
      email: user?.email || '',
      roles: user?.roles.map(r => r.id) || []
    }
  });
  
  const onSubmit = async (data: UserFormData) => {
    try {
      await onSave(data);
    } catch (error) {
      if (error instanceof ValidationError) {
        // Set field-specific errors
        Object.entries(error.fieldErrors).forEach(([field, message]) => {
          setError(field as keyof UserFormData, { message });
        });
      } else {
        // Generic error handling
        setError('root', { 
          message: t('errors.saveFailed')
        });
      }
    }
  };
  
  return (
    <form onSubmit={handleSubmit(onSubmit)} className="user-form">
      <div className="form-field">
        <label htmlFor="name">{t('fields.name')}</label>
        <input
          id="name"
          type="text"
          {...register('name')}
          aria-invalid={!!errors.name}
          aria-describedby={errors.name ? 'name-error' : undefined}
        />
        {errors.name && (
          <span id="name-error" role="alert" className="error">
            {errors.name.message}
          </span>
        )}
      </div>
      
      <div className="form-actions">
        <button 
          type="submit" 
          disabled={isSubmitting}
          aria-label={t('actions.save')}
        >
          {isSubmitting ? t('actions.saving') : t('actions.save')}
        </button>
        <button 
          type="button" 
          onClick={onCancel}
          aria-label={t('actions.cancel')}
        >
          {t('actions.cancel')}
        </button>
      </div>
    </form>
  );
});

// ❌ Security issues to flag
const UserForm = ({ user, onSave }) => {
  const [name, setName] = useState(user?.name || '');
  
  const handleSubmit = (e) => {
    e.preventDefault();
    onSave({ name });  // No validation, direct submission
  };
  
  return (
    <form onSubmit={handleSubmit}>
      <input 
        value={name}
        onChange={(e) => setName(e.target.value)}  // No sanitization
      />
      <button type="submit">Save</button>
    </form>
  );
};
```
```

### 5. **Testing Coverage for Changes**

#### Test Requirements for Frontend Changes
```markdown
✅ **Testing Change Validation:**
- [ ] **Component Tests**: New components have comprehensive tests
- [ ] **Hook Tests**: Custom hooks tested with renderHook
- [ ] **Integration Tests**: API integration changes tested
- [ ] **User Interaction**: User interactions tested with fireEvent/userEvent
- [ ] **Error Scenarios**: Error states and edge cases tested
- [ ] **Accessibility**: Accessibility features tested
- [ ] **Performance**: Performance-critical changes have performance tests

**Testing Change Analysis:**
```typescript
// ✅ Comprehensive test addition
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { vi } from 'vitest';
import { UserForm } from '../UserForm';

const mockUser: User = {
  id: '1',
  name: 'John Doe',
  email: 'john@example.com',
  roles: [{ id: 'role1', name: 'Admin' }]
};

const renderWithProviders = (ui: React.ReactElement) => {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } }
  });
  
  return render(
    <QueryClientProvider client={queryClient}>
      {ui}
    </QueryClientProvider>
  );
};

describe('UserForm', () => {
  const mockOnSave = vi.fn();
  const mockOnCancel = vi.fn();
  
  beforeEach(() => {
    vi.clearAllMocks();
  });
  
  it('should render form with user data', () => {
    renderWithProviders(
      <UserForm 
        user={mockUser} 
        onSave={mockOnSave} 
        onCancel={mockOnCancel} 
      />
    );
    
    expect(screen.getByDisplayValue('John Doe')).toBeInTheDocument();
    expect(screen.getByDisplayValue('john@example.com')).toBeInTheDocument();
  });
  
  it('should validate required fields', async () => {
    const user = userEvent.setup();
    
    renderWithProviders(
      <UserForm 
        onSave={mockOnSave} 
        onCancel={mockOnCancel} 
      />
    );
    
    const submitButton = screen.getByRole('button', { name: /save/i });
    await user.click(submitButton);
    
    await waitFor(() => {
      expect(screen.getByText('Name is required')).toBeInTheDocument();
      expect(screen.getByText('Invalid email format')).toBeInTheDocument();
    });
    
    expect(mockOnSave).not.toHaveBeenCalled();
  });
  
  it('should submit valid form data', async () => {
    const user = userEvent.setup();
    mockOnSave.mockResolvedValue(undefined);
    
    renderWithProviders(
      <UserForm 
        onSave={mockOnSave} 
        onCancel={mockOnCancel} 
      />
    );
    
    await user.type(screen.getByLabelText(/name/i), 'Jane Smith');
    await user.type(screen.getByLabelText(/email/i), 'jane@example.com');
    
    const submitButton = screen.getByRole('button', { name: /save/i });
    await user.click(submitButton);
    
    await waitFor(() => {
      expect(mockOnSave).toHaveBeenCalledWith({
        name: 'Jane Smith',
        email: 'jane@example.com',
        roles: []
      });
    });
  });
  
  it('should handle server validation errors', async () => {
    const user = userEvent.setup();
    const validationError = new ValidationError({
      fieldErrors: { email: 'Email already exists' }
    });
    mockOnSave.mockRejectedValue(validationError);
    
    renderWithProviders(
      <UserForm 
        onSave={mockOnSave} 
        onCancel={mockOnCancel} 
      />
    );
    
    await user.type(screen.getByLabelText(/name/i), 'John Doe');
    await user.type(screen.getByLabelText(/email/i), 'existing@example.com');
    
    const submitButton = screen.getByRole('button', { name: /save/i });
    await user.click(submitButton);
    
    await waitFor(() => {
      expect(screen.getByText('Email already exists')).toBeInTheDocument();
    });
  });
  
  it('should be accessible', () => {
    renderWithProviders(
      <UserForm 
        onSave={mockOnSave} 
        onCancel={mockOnCancel} 
      />
    );
    
    // Check for proper labels
    expect(screen.getByLabelText(/name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
    
    // Check for ARIA attributes
    const nameInput = screen.getByLabelText(/name/i);
    expect(nameInput).toHaveAttribute('aria-invalid', 'false');
  });
});

// Hook test example
describe('useUserManagement', () => {
  it('should manage user selection state', () => {
    const { result } = renderHook(() => useUserManagement());
    
    expect(result.current.selectedUser).toBeNull();
    
    act(() => {
      result.current.selectUser(mockUser);
    });
    
    expect(result.current.selectedUser).toEqual(mockUser);
  });
});
```
```

## Implementation Process

### Frontend Change Review Execution
```markdown
I'll systematically review all TypeScript/React changes:

**Phase 1: Frontend Change Identification**
- Execute `git status` filtered for frontend files
- Use `git diff --staged` for staged TypeScript/React changes
- Use `git diff` for unstaged frontend changes
- Identify new components, hooks, services, and type definitions

**Phase 2: TypeScript Quality Assessment**
- Analyze type safety improvements or regressions
- Check interface changes and breaking compatibility
- Validate generic usage and utility type application
- Review API integration type definitions

**Phase 3: React Pattern Analysis**
- Evaluate component structure and performance optimizations
- Check hook implementations and state management
- Analyze accessibility and internationalization improvements
- Validate error handling and user experience enhancements

**Phase 4: Integration Impact Assessment**
- Check component prop interface compatibility
- Validate hook signature changes
- Assess API contract modifications
- Analyze routing and navigation updates
```

### Frontend Change Review Report Format
```markdown
# TypeScript/React Staged Changes Review Report

## Change Summary
- **Frontend Files Changed**: X (Staged: X, Unstaged: X)
- **Components Modified**: X
- **Hooks Updated**: X
- **API Services Changed**: X
- **Type Definitions Updated**: X

## TypeScript Quality Assessment

### ✅ backend/api/types/user.types.ts (+20, -5)
**Changes Made:**
- Added comprehensive User interface with proper typing
- Included role-based typing with union types
- Added API response wrapper types

**Quality Assessment:** ✅ Excellent
- Strong TypeScript patterns implemented
- Proper generic usage for API responses
- Breaking changes avoided with optional properties

### ⚠️ components/UserCard.tsx (+35, -15)
**Changes Made:**
- Added TypeScript interface for props
- Implemented React.memo for performance
- Added accessibility attributes

**Quality Assessment:** ⚠️ Good with suggestions
- ✅ TypeScript typing improved significantly
- ✅ Performance optimization added
- ⚠️ Missing internationalization for hardcoded text
- ⚠️ Could benefit from error boundary

**Recommendations:**
```typescript
// Add i18n support
const { t } = useTranslation('users');

// Replace hardcoded text
<button>{t('edit.button')}</button>

// Add error boundary wrapper
<ErrorBoundary fallback={<ErrorMessage />}>
  <UserCard {...props} />
</ErrorBoundary>
```

## Performance Impact Analysis

### Positive Changes
- Added React.memo to UserCard component (-15% re-renders)
- Implemented useCallback for event handlers
- Added useMemo for expensive computations

### Areas for Improvement
- UserList component could benefit from virtualization
- API calls could implement better caching strategies
- Bundle size increased by 2KB (consider code splitting)

## Security Assessment

### Security Enhancements
- ✅ Added input validation with Zod schemas
- ✅ Implemented proper error handling without data exposure
- ✅ Added CSRF protection to forms

### Security Concerns
- ⚠️ Missing rate limiting on API calls
- ⚠️ User input sanitization could be stronger

## Testing Coverage

### Tests Added
- ✅ UserForm component tests with user interactions
- ✅ useUserManagement hook tests
- ✅ API service integration tests

### Missing Tests
- ❌ UserCard accessibility tests
- ❌ Error boundary tests
- ❌ Performance regression tests

## Priority Actions

### Critical (Fix Before Commit)
- [ ] Add missing internationalization keys
- [ ] Implement proper error boundaries
- [ ] Add input sanitization for user content

### High Priority (This Session)
- [ ] Add missing component tests
- [ ] Implement accessibility improvements
- [ ] Optimize bundle size impact

### Medium Priority (Next Sprint)
- [ ] Add performance monitoring
- [ ] Implement advanced caching strategies
- [ ] Add E2E tests for new user flows

## Overall Assessment
- **TypeScript Quality**: 9/10
- **React Patterns**: 8/10
- **Performance**: 7/10
- **Security**: 8/10
- **Testing**: 6/10
- **Ready for Commit**: ⚠️ (Minor improvements needed)
```

## Validation Commands

### Pre-Review Validation
```bash
# Check TypeScript compilation
npm run type-check

# View frontend changes
git diff --staged -- "*.ts" "*.tsx" "*.js" "*.jsx"

# Check build status
npm run build

# Run frontend tests
npm run test
```

### Post-Fix Validation
```bash
# Verify TypeScript compliance
npm run type-check

# Check code formatting
npm run lint

# Run all tests with coverage
npm run test:coverage

# Performance audit
npm run lighthouse
```

## Success Criteria
- All TypeScript changes analyzed for type safety and best practices
- React component changes evaluated for performance and accessibility
- Security implications of changes assessed and documented
- Testing coverage evaluated with specific improvement recommendations
- Performance impact measured and optimization suggestions provided
- Clear prioritization of required improvements before commit
- Integration compatibility validated for all interface changes
- Actionable recommendations with specific code examples provided

This focused TypeScript/React change review ensures that frontend code changes maintain high quality, performance, and security standards while following React and TypeScript best practices.
