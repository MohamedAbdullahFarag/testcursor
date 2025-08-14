---
description: "Frontend architecture guidelines with component patterns and state management"
applyTo: "**/*.tsx,**/components/**,**/modules/**"
---

# Frontend Architecture Guidelines

## Project Structure (Folder-per-Feature)
```
frontend/
├── src/
│   ├── modules/               # Feature folders (folder-per-feature)
│   │   ├── auth/
│   │   ├── content-management/
│   │   ├── customerExperience/
│   │   ├── dashboard/
│   │   ├── eparticipation/
│   │   ├── faq/
│   │   ├── Home/
│   │   ├── PrivacyPolicy/
│   │   ├── support/
│   │   └── TermsAndConditions/
│   ├── layout/                # Layout components
│   │   ├── DashboardLayout/
│   │   └── PortalLayout/
│   ├── pages/                 # Top-level pages
│   │   ├── ErrorPage/
│   │   └── NotFoundPage/
│   ├── routes/                # Route configurations
│   │   ├── DashboardRoutes/
│   │   └── PortalRoutes/
│   ├── shared/                # Shared utilities, hooks, services, components
│   │   ├── components/
│   │   ├── constants/
│   │   ├── hooks/
│   │   ├── Lib/
│   │   ├── locales/
│   │   ├── models/
│   │   ├── services/
│   │   └── utils/
│   ├── App.tsx                # App entry point
│   └── main.tsx               # Vite entry point
```

## Code Templates and Patterns

### Component Template
```typescript
import React, { memo } from 'react';
import { {Feature}Props } from '../types/{feature}Types';

interface {Feature}ComponentProps {
  data: {Feature}Props;
  onAction?: (id: string) => void;
  className?: string;
}

const {Feature}Component: React.FC<{Feature}ComponentProps> = memo(({ 
  data, 
  onAction, 
  className = '' 
}) => {
  const handleClick = () => {
    if (onAction) {
      onAction(data.id);
    }
  };

  return (
    <div className={`{feature}-component ${className}`}>
      <h3 className="text-lg font-semibold">{data.title}</h3>
      <p className="text-gray-600">{data.description}</p>
      <button 
        onClick={handleClick}
        className="mt-2 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
      >
        Action
      </button>
    </div>
  );
});

{Feature}Component.displayName = '{Feature}Component';

export default {Feature}Component;
```

### Custom Hook Template
```typescript
import { useState, useEffect, useCallback } from 'react';
import { {feature}Service } from '../services/{feature}Service';
import { {Feature}Type } from '../types/{feature}Types';

interface Use{Feature}Return {
  {feature}s: {Feature}Type[];
  loading: boolean;
  error: string | null;
  refresh: () => Promise<void>;
  create: (data: Create{Feature}Type) => Promise<void>;
  update: (id: string, data: Update{Feature}Type) => Promise<void>;
  delete: (id: string) => Promise<void>;
}

export const use{Feature} = (): Use{Feature}Return => {
  const [{feature}s, set{Feature}s] = useState<{Feature}Type[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await {feature}Service.getAll();
      set{Feature}s(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  }, []);

  const create = useCallback(async (data: Create{Feature}Type) => {
    try {
      setLoading(true);
      setError(null);
      await {feature}Service.create(data);
      await refresh();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create {feature}');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [refresh]);

  useEffect(() => {
    refresh();
  }, [refresh]);

  return {
    {feature}s,
    loading,
    error,
    refresh,
    create,
    update: async (id: string, data: Update{Feature}Type) => {
      // Implementation here
    },
    delete: async (id: string) => {
      // Implementation here
    }
  };
};
```

### Service Template
```typescript
import { apiService } from '../../Shared/services/apiService';
import { {Feature}Type, Create{Feature}Type, Update{Feature}Type } from '../types/{feature}Types';

class {Feature}Service {
  private readonly baseUrl = '/{feature}s';

  async getAll(): Promise<{Feature}Type[]> {
    return await apiService.get<{Feature}Type[]>(this.baseUrl);
  }

  async getById(id: string): Promise<{Feature}Type> {
    return await apiService.get<{Feature}Type>(`${this.baseUrl}/${id}`);
  }

  async create(data: Create{Feature}Type): Promise<{Feature}Type> {
    return await apiService.post<{Feature}Type>(this.baseUrl, data);
  }

  async update(id: string, data: Update{Feature}Type): Promise<{Feature}Type> {
    return await apiService.put<{Feature}Type>(`${this.baseUrl}/${id}`, data);
  }

  async delete(id: string): Promise<void> {
    await apiService.delete(`${this.baseUrl}/${id}`);
  }
}

export const {feature}Service = new {Feature}Service();
```

### Type Definitions Template
```typescript
// Base types
export interface {Feature}Type {
  id: string;
  title: string;
  description: string;
  createdAt: Date;
  updatedAt: Date;
}

// Create/Update types
export interface Create{Feature}Type {
  title: string;
  description: string;
}

export interface Update{Feature}Type extends Partial<Create{Feature}Type> {}

// Component props types
export interface {Feature}ComponentProps {
  data: {Feature}Type;
  onEdit?: (id: string) => void;
  onDelete?: (id: string) => void;
  className?: string;
}

// Hook return types
export interface Use{Feature}Options {
  autoRefresh?: boolean;
  refreshInterval?: number;
}
```

## State Management Guidelines

### Context Provider Template
```typescript
import React, { createContext, useContext, useReducer, ReactNode } from 'react';

interface {Feature}State {
  {feature}s: {Feature}Type[];
  selected{Feature}: {Feature}Type | null;
  loading: boolean;
  error: string | null;
}

type {Feature}Action =
  | { type: 'SET_LOADING'; payload: boolean }
  | { type: 'SET_ERROR'; payload: string | null }
  | { type: 'SET_{FEATURE}S'; payload: {Feature}Type[] }
  | { type: 'SET_SELECTED_{FEATURE}'; payload: {Feature}Type | null };

const {feature}Reducer = (state: {Feature}State, action: {Feature}Action): {Feature}State => {
  switch (action.type) {
    case 'SET_LOADING':
      return { ...state, loading: action.payload };
    case 'SET_ERROR':
      return { ...state, error: action.payload };
    case 'SET_{FEATURE}S':
      return { ...state, {feature}s: action.payload };
    case 'SET_SELECTED_{FEATURE}':
      return { ...state, selected{Feature}: action.payload };
    default:
      return state;
  }
};

const {Feature}Context = createContext<{
  state: {Feature}State;
  dispatch: React.Dispatch<{Feature}Action>;
} | null>(null);

export const {Feature}Provider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [state, dispatch] = useReducer({feature}Reducer, {
    {feature}s: [],
    selected{Feature}: null,
    loading: false,
    error: null,
  });

  return (
    <{Feature}Context.Provider value={{ state, dispatch }}>
      {children}
    </{Feature}Context.Provider>
  );
};

export const use{Feature}Context = () => {
  const context = useContext({Feature}Context);
  if (!context) {
    throw new Error('use{Feature}Context must be used within a {Feature}Provider');
  }
  return context;
};
```

## Styling Guidelines

### Tailwind CSS Patterns
```typescript
// Component variants using Tailwind
const buttonVariants = {
  primary: 'bg-blue-500 hover:bg-blue-600 text-white',
  secondary: 'bg-gray-500 hover:bg-gray-600 text-white',
  danger: 'bg-red-500 hover:bg-red-600 text-white',
  ghost: 'bg-transparent hover:bg-gray-100 text-gray-700',
};

const sizeVariants = {
  sm: 'px-2 py-1 text-sm',
  md: 'px-4 py-2 text-base',
  lg: 'px-6 py-3 text-lg',
};

// Reusable class combinations
const cardClasses = 'bg-white rounded-lg shadow-md p-4 border border-gray-200';
const inputClasses = 'w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500';
```

## Performance Optimization
- Use React.memo for components that don't need frequent re-renders
- Implement useCallback and useMemo for expensive operations
- Use React.lazy for code splitting
- Implement virtual scrolling for large lists
- Optimize bundle size with proper tree shaking

## Error Handling & Loading States

Ikhtibar implements comprehensive error handling and loading state management patterns to ensure a consistent user experience across the application.

### Error Boundary Implementation

```tsx
// File: src/shared/components/ErrorBoundary/ErrorBoundary.tsx
import React, { Component, ReactNode, ErrorInfo } from 'react';
import { useTranslation } from 'react-i18next';
import { ErrorFallback } from './ErrorFallback';
import { errorService } from '@/shared/services/errorService';

interface ErrorBoundaryProps {
  children: ReactNode;
  fallback?: ReactNode;
  onError?: (error: Error, errorInfo: ErrorInfo) => void;
  resetKey?: any; // Key that changes to reset the error boundary
  errorType?: 'component' | 'page' | 'module';
}

interface ErrorBoundaryState {
  hasError: boolean;
  error: Error | null;
}

/**
 * Error Boundary component for catching React rendering errors
 * 
 * Features:
 * - Captures and logs rendering errors
 * - Prevents app crashes with graceful degradation
 * - Customizable fallback UI for different contexts
 * - Integrates with error tracking service
 * - Supports i18n for error messages
 * - Can be reset programmatically via props
 */
export class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  constructor(props: ErrorBoundaryProps) {
    super(props);
    this.state = {
      hasError: false,
      error: null
    };
  }

  static getDerivedStateFromError(error: Error): ErrorBoundaryState {
    return {
      hasError: true,
      error
    };
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo): void {
    // Log error to service for tracking
    errorService.logError(error, {
      errorInfo,
      errorType: this.props.errorType || 'component'
    });
    
    // Call custom error handler if provided
    if (this.props.onError) {
      this.props.onError(error, errorInfo);
    }
  }

  componentDidUpdate(prevProps: ErrorBoundaryProps): void {
    // Reset error state if resetKey changes
    if (this.state.hasError && this.props.resetKey !== prevProps.resetKey) {
      this.setState({
        hasError: false,
        error: null
      });
    }
  }

  render(): ReactNode {
    if (this.state.hasError) {
      // Use custom fallback or default to ErrorFallback
      return this.props.fallback || (
        <ErrorFallback 
          error={this.state.error as Error} 
          resetError={() => this.setState({ hasError: false, error: null })} 
          errorType={this.props.errorType || 'component'} 
        />
      );
    }

    return this.props.children;
  }
}

// File: src/shared/components/ErrorBoundary/ErrorFallback.tsx
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Button } from '@/shared/components/ui/Button';

interface ErrorFallbackProps {
  error: Error;
  resetError: () => void;
  errorType?: 'component' | 'page' | 'module';
}

/**
 * Default error fallback UI with i18n support and actions
 */
export const ErrorFallback: React.FC<ErrorFallbackProps> = ({ 
  error, 
  resetError,
  errorType = 'component' 
}) => {
  const { t } = useTranslation('common');
  
  // Different styling based on error context
  const containerStyles = {
    component: 'p-4 border border-red-300 rounded-md bg-red-50 text-red-900',
    module: 'p-6 m-4 border-2 border-red-400 rounded-lg bg-red-50 text-red-900 shadow-md',
    page: 'flex flex-col items-center justify-center min-h-[50vh] p-8 bg-red-50 text-red-900'
  };
  
  return (
    <div className={containerStyles[errorType]} role="alert">
      <div className="flex items-center mb-4">
        <svg className="w-6 h-6 mr-2 text-red-500" fill="currentColor" viewBox="0 0 20 20">
          <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
        </svg>
        <h3 className="text-lg font-medium">{t('error.title')}</h3>
      </div>
      
      <p className="mb-4">{t('error.message')}</p>
      
      {process.env.NODE_ENV !== 'production' && (
        <div className="p-3 mb-4 overflow-auto text-sm bg-red-100 rounded max-h-40">
          {error.message}
        </div>
      )}
      
      <div className="flex space-x-3">
        <Button 
          onClick={resetError}
          variant="secondary"
        >
          {t('error.tryAgain')}
        </Button>
        
        <Button 
          onClick={() => window.location.reload()}
          variant="outline"
        >
          {t('error.refresh')}
        </Button>
      </div>
    </div>
  );
};

// File: src/shared/hooks/useErrorHandler.ts
import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { useNotification } from '@/shared/hooks/useNotification';
import { errorService } from '@/shared/services/errorService';

interface UseErrorHandlerOptions {
  context: string;
  showNotification?: boolean;
  logToService?: boolean;
}

/**
 * Custom hook for standardized error handling across the application
 */
export const useErrorHandler = (options: UseErrorHandlerOptions) => {
  const { t } = useTranslation('common');
  const { showError } = useNotification();
  
  const handleError = useCallback((error: unknown, action?: string) => {
    // Format user-friendly error message
    const message = errorService.getErrorMessage(error);
    const errorContext = action ? `${options.context}.${action}` : options.context;
    
    // Log to error service if enabled
    if (options.logToService !== false) {
      errorService.logError(error, { context: errorContext });
    }
    
    // Show notification if enabled
    if (options.showNotification !== false) {
      showError(
        t(`errors.${errorContext}`, { defaultValue: t('errors.generic') }),
        message
      );
    }
    
    // Return the error message for inline display if needed
    return message;
  }, [options.context, options.logToService, options.showNotification, showError, t]);
  
  return { handleError };
};

// Usage example
export const MyComponent: React.FC = () => {
  const { handleError } = useErrorHandler({ context: 'userManagement' });
  
  const handleSubmit = async (data) => {
    try {
      await userService.createUser(data);
    } catch (error) {
      handleError(error, 'createUser');
    }
  };
  
  // Component implementation
};
```

### Nested Error Boundaries Strategy

```tsx
// File: src/App.tsx
import { ErrorBoundary } from '@/shared/components/ErrorBoundary';
import { AppRoutes } from '@/routes';

const App: React.FC = () => {
  return (
    // Root error boundary catches application-level errors
    <ErrorBoundary errorType="page">
      <AppRoutes />
    </ErrorBoundary>
  );
};

// File: src/routes/index.tsx
const AppRoutes: React.FC = () => {
  return (
    <Routes>
      {/* Each major route has its own error boundary */}
      <Route path="/dashboard/*" element={
        <ErrorBoundary errorType="module">
          <DashboardRoutes />
        </ErrorBoundary>
      } />
      
      <Route path="/users/*" element={
        <ErrorBoundary errorType="module">
          <UserRoutes />
        </ErrorBoundary>
      } />
      
      {/* Global error route */}
      <Route path="/error" element={<ErrorPage />} />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
};

// File: src/modules/user-management/views/UserListPage.tsx
const UserListPage: React.FC = () => {
  return (
    <div className="page-container">
      <PageHeader title="User Management" />
      
      <div className="grid grid-cols-12 gap-6">
        {/* Each section has its own error boundary */}
        <ErrorBoundary errorType="component">
          <UserFilters className="col-span-12" />
        </ErrorBoundary>
        
        <div className="col-span-12 md:col-span-8">
          <ErrorBoundary errorType="component">
            <UserTable />
          </ErrorBoundary>
        </div>
        
        <div className="col-span-12 md:col-span-4">
          <ErrorBoundary errorType="component">
            <UserStatistics />
          </ErrorBoundary>
        </div>
      </div>
    </div>
  );
};
```

### Comprehensive Loading State System

```tsx
// File: src/shared/components/LoadingState/LoadingState.tsx
import React from 'react';
import { Spinner } from '@/shared/components/ui/Spinner';
import { cn } from '@/shared/utils/cn';

export type LoadingSize = 'sm' | 'md' | 'lg';
export type LoadingType = 'spinner' | 'skeleton' | 'progress';
export type LoadingVariant = 'overlay' | 'inline' | 'block' | 'fullscreen';

interface LoadingStateProps {
  isLoading: boolean;
  size?: LoadingSize;
  type?: LoadingType;
  variant?: LoadingVariant;
  text?: string;
  children?: React.ReactNode;
  showContentWhileLoading?: boolean;
  className?: string;
}

/**
 * Reusable loading state component with multiple variations
 */
export const LoadingState: React.FC<LoadingStateProps> = ({
  isLoading,
  size = 'md',
  type = 'spinner',
  variant = 'block',
  text,
  children,
  showContentWhileLoading = false,
  className
}) => {
  // Skip rendering if not loading and no children
  if (!isLoading && !children) return null;
  
  // Show content without loading UI if not loading
  if (!isLoading) return <>{children}</>;
  
  // Size mapping for spinner
  const spinnerSizes = {
    sm: 'h-4 w-4',
    md: 'h-8 w-8',
    lg: 'h-12 w-12'
  };
  
  // Return appropriate loading UI based on variant
  const renderLoadingUI = () => {
    switch (type) {
      case 'spinner':
        return <Spinner className={spinnerSizes[size]} />;
      case 'skeleton':
        return (
          <div className={cn(
            'animate-pulse bg-gray-200 rounded',
            size === 'sm' && 'h-4',
            size === 'md' && 'h-8',
            size === 'lg' && 'h-12',
            'w-full'
          )} />
        );
      case 'progress':
        return (
          <div className="w-full bg-gray-200 rounded">
            <div className="h-2 bg-blue-500 rounded animate-pulse" style={{ width: '75%' }} />
          </div>
        );
      default:
        return <Spinner className={spinnerSizes[size]} />;
    }
  };
  
  // Full screen overlay loading
  if (variant === 'fullscreen') {
    return (
      <div className="fixed inset-0 z-50 flex flex-col items-center justify-center bg-white/80 dark:bg-gray-900/80">
        {renderLoadingUI()}
        {text && <p className="mt-4 text-gray-600 dark:text-gray-300">{text}</p>}
      </div>
    );
  }
  
  // Overlay loading with relative positioning
  if (variant === 'overlay') {
    return (
      <div className={cn('relative', className)}>
        {showContentWhileLoading && children}
        <div className="absolute inset-0 flex items-center justify-center bg-white/70 dark:bg-gray-900/70 z-10 rounded">
          {renderLoadingUI()}
          {text && <p className="ml-3 text-gray-600 dark:text-gray-300">{text}</p>}
        </div>
      </div>
    );
  }
  
  // Inline loading (horizontal layout)
  if (variant === 'inline') {
    return (
      <div className={cn('flex items-center', className)}>
        {renderLoadingUI()}
        {text && <p className="ml-3 text-gray-600 dark:text-gray-300">{text}</p>}
        {showContentWhileLoading && children}
      </div>
    );
  }
  
  // Block loading (vertical layout)
  return (
    <div className={cn('flex flex-col items-center justify-center p-8', className)}>
      {renderLoadingUI()}
      {text && <p className="mt-4 text-gray-600 dark:text-gray-300">{text}</p>}
      {showContentWhileLoading && <div className="mt-4 w-full">{children}</div>}
    </div>
  );
};

// File: src/shared/hooks/useAsyncState.ts
import { useState, useCallback } from 'react';
import { useErrorHandler } from './useErrorHandler';

interface UseAsyncStateOptions {
  initialLoading?: boolean;
  context: string;
  showErrorNotification?: boolean;
}

/**
 * Hook for managing async operations with loading and error states
 */
export function useAsyncState<T>(options: UseAsyncStateOptions) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState<boolean>(options.initialLoading ?? false);
  const [error, setError] = useState<string | null>(null);
  
  const { handleError } = useErrorHandler({
    context: options.context,
    showNotification: options.showErrorNotification
  });
  
  const execute = useCallback(async <R>(
    asyncFn: () => Promise<R>,
    action?: string
  ): Promise<R | null> => {
    setLoading(true);
    setError(null);
    
    try {
      const result = await asyncFn();
      return result;
    } catch (err) {
      const errorMessage = handleError(err, action);
      setError(errorMessage);
      return null;
    } finally {
      setLoading(false);
    }
  }, [handleError]);
  
  const setAsyncData = useCallback(async <R>(
    asyncFn: () => Promise<R>,
    action?: string
  ): Promise<boolean> => {
    const result = await execute(asyncFn, action);
    
    if (result !== null) {
      setData(result as unknown as T);
      return true;
    }
    
    return false;
  }, [execute]);
  
  return {
    data,
    setData,
    loading,
    setLoading,
    error,
    setError,
    execute,
    setAsyncData,
    reset: useCallback(() => {
      setData(null);
      setLoading(false);
      setError(null);
    }, [])
  };
}

// Example: Component using loading states and error handling
const UserProfile: React.FC<{ userId: string }> = ({ userId }) => {
  const { t } = useTranslation('users');
  const { data: user, loading, error, setAsyncData } = useAsyncState<User>({
    context: 'userProfile',
    initialLoading: true
  });
  
  useEffect(() => {
    if (userId) {
      setAsyncData(() => userService.getUserById(userId), 'fetchUser');
    }
  }, [userId, setAsyncData]);
  
  return (
    <LoadingState
      isLoading={loading}
      variant="overlay"
      text={t('users.loading')}
    >
      {error ? (
        <div className="error-message">
          {error}
          <button onClick={() => setAsyncData(() => userService.getUserById(userId), 'fetchUser')}>
            {t('common.retry')}
          </button>
        </div>
      ) : user ? (
        <div className="user-profile">
          <h2>{user.name}</h2>
          <p>{user.email}</p>
          {/* Additional user details */}
        </div>
      ) : null}
    </LoadingState>
  );
};
```

### Skeleton Loading Patterns

```tsx
// File: src/shared/components/Skeleton/SkeletonCard.tsx
import React from 'react';

interface SkeletonCardProps {
  lines?: number;
  hasImage?: boolean;
  hasFooter?: boolean;
  className?: string;
}

/**
 * Skeleton loader for card components
 */
export const SkeletonCard: React.FC<SkeletonCardProps> = ({
  lines = 3,
  hasImage = false,
  hasFooter = false,
  className = ''
}) => {
  return (
    <div className={`animate-pulse rounded-lg border border-gray-200 p-4 ${className}`}>
      {/* Card header */}
      <div className="flex items-center space-x-3 mb-4">
        {/* Avatar placeholder */}
        <div className="rounded-full bg-gray-200 h-10 w-10"></div>
        
        <div className="space-y-2 flex-1">
          {/* Title placeholder */}
          <div className="h-4 bg-gray-200 rounded w-3/4"></div>
          
          {/* Subtitle placeholder */}
          <div className="h-3 bg-gray-200 rounded w-1/2"></div>
        </div>
      </div>
      
      {/* Image placeholder */}
      {hasImage && (
        <div className="h-40 bg-gray-200 rounded w-full mb-4"></div>
      )}
      
      {/* Content lines */}
      <div className="space-y-2 mb-4">
        {Array.from({ length: lines }).map((_, i) => (
          <div 
            key={i} 
            className={`h-3 bg-gray-200 rounded ${i === lines - 1 ? 'w-4/6' : 'w-full'}`}
          />
        ))}
      </div>
      
      {/* Footer placeholder */}
      {hasFooter && (
        <div className="flex items-center justify-between pt-2 mt-2 border-t">
          <div className="h-4 bg-gray-200 rounded w-1/4"></div>
          <div className="h-4 bg-gray-200 rounded w-1/4"></div>
        </div>
      )}
    </div>
  );
};

// File: src/modules/user-management/views/UserListPage.tsx
const UserListPage: React.FC = () => {
  const { users, loading, error, fetchUsers } = useUsers();
  
  return (
    <div className="container mx-auto py-6">
      <h1 className="text-2xl font-bold mb-6">Users</h1>
      
      {error ? (
        <ErrorDisplay 
          message={error}
          onRetry={fetchUsers}
        />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {loading ? (
            // Show skeleton cards while loading
            Array.from({ length: 6 }).map((_, i) => (
              <SkeletonCard key={i} hasImage hasFooter />
            ))
          ) : (
            users.map(user => (
              <UserCard key={user.id} user={user} />
            ))
          )}
        </div>
      )}
    </div>
  );
};
```

### Global Loading Indicator

```tsx
// File: src/shared/components/LoadingIndicator/GlobalLoadingIndicator.tsx
import React, { useEffect } from 'react';
import NProgress from 'nprogress';
import { useNavigationType } from 'react-router-dom';
import { useLoadingStore } from '@/shared/store/loadingStore';

/**
 * Global loading indicator component
 * Shows a progress bar at the top of the page for:
 * - Route transitions
 * - Global API requests
 * - User-triggered loading states
 */
export const GlobalLoadingIndicator: React.FC = () => {
  const navigationType = useNavigationType();
  const { globalLoadingState } = useLoadingStore();
  
  // Configure NProgress
  useEffect(() => {
    NProgress.configure({ 
      minimum: 0.1,
      showSpinner: false,
      trickleSpeed: 200
    });
    
    return () => {
      NProgress.done();
    };
  }, []);
  
  // Handle route changes
  useEffect(() => {
    if (navigationType) {
      NProgress.start();
      
      const timer = setTimeout(() => {
        NProgress.done();
      }, 500);
      
      return () => {
        clearTimeout(timer);
        NProgress.done();
      };
    }
  }, [navigationType]);
  
  // Handle global loading state
  useEffect(() => {
    if (globalLoadingState.isLoading) {
      NProgress.start();
    } else {
      NProgress.done();
    }
  }, [globalLoadingState.isLoading]);
  
  return null; // This component doesn't render anything
};

// File: src/shared/store/loadingStore.ts
import create from 'zustand';

interface LoadingState {
  isLoading: boolean;
  message?: string;
  source?: string;
}

interface LoadingStore {
  globalLoadingState: LoadingState;
  setGlobalLoading: (state: LoadingState) => void;
  startLoading: (source?: string, message?: string) => void;
  stopLoading: (source?: string) => void;
}

export const useLoadingStore = create<LoadingStore>((set, get) => ({
  globalLoadingState: {
    isLoading: false
  },
  setGlobalLoading: (state) => set({ globalLoadingState: state }),
  startLoading: (source, message) => set({
    globalLoadingState: {
      isLoading: true,
      source,
      message
    }
  }),
  stopLoading: (source) => {
    const currentState = get().globalLoadingState;
    
    // Only stop loading if the source matches or no source specified
    if (!source || currentState.source === source) {
      set({
        globalLoadingState: {
          isLoading: false
        }
      });
    }
  }
}));

// File: src/shared/hooks/useGlobalLoading.ts
import { useCallback } from 'react';
import { useLoadingStore } from '@/shared/store/loadingStore';

/**
 * Hook for controlling global loading state
 */
export const useGlobalLoading = (source?: string) => {
  const { startLoading, stopLoading } = useLoadingStore();
  
  const startGlobalLoading = useCallback((message?: string) => {
    startLoading(source, message);
  }, [startLoading, source]);
  
  const stopGlobalLoading = useCallback(() => {
    stopLoading(source);
  }, [stopLoading, source]);
  
  const withGlobalLoading = useCallback(async <T>(
    asyncFn: () => Promise<T>,
    message?: string
  ): Promise<T> => {
    try {
      startGlobalLoading(message);
      return await asyncFn();
    } finally {
      stopGlobalLoading();
    }
  }, [startGlobalLoading, stopGlobalLoading]);
  
  return {
    startGlobalLoading,
    stopGlobalLoading,
    withGlobalLoading
  };
};

// Usage example
const ImportUsersButton: React.FC = () => {
  const { t } = useTranslation('users');
  const { withGlobalLoading } = useGlobalLoading('userImport');
  
  const handleImport = async () => {
    await withGlobalLoading(
      () => userService.importUsers(),
      t('users.importing')
    );
  };
  
  return (
    <Button onClick={handleImport}>
      {t('users.import')}
    </Button>
  );
};
```
