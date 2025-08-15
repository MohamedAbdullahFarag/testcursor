import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../modules/auth/hooks/useAuth';

interface PrivateRouteProps {
  children: React.ReactNode;
  requiredRoles?: string[];
}

export const PrivateRoute: React.FC<PrivateRouteProps> = ({ 
  children, 
  requiredRoles 
}) => {
  const { isAuthenticated, user, isLoading } = useAuth();
  const location = useLocation();

  // Show loading state while checking authentication
  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="flex items-center space-x-2">
          <svg className="animate-spin h-8 w-8 text-indigo-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          <span className="text-lg text-gray-600">Loading...</span>
        </div>
      </div>
    );
  }

  // Redirect to login if not authenticated
  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // Check role requirements if specified
  if (requiredRoles && user) {
    const hasRequiredRole = requiredRoles.some(role => 
      user.roles.includes(role)
    );
    
    if (!hasRequiredRole) {
      return (
        <div className="min-h-screen flex items-center justify-center">
          <div className="text-center">
            <h1 className="text-2xl font-bold text-red-600 mb-4">Access Denied</h1>
            <p className="text-gray-600 mb-4">
              You don't have permission to access this page.
            </p>
            <p className="text-sm text-gray-500">
              Required roles: {requiredRoles.join(', ')}
            </p>
          </div>
        </div>
      );
    }
  }

  // User is authenticated and has required roles (if any)
  return <>{children}</>;
};

/**
 * PublicRoute component for routes that should only be accessible when NOT authenticated
 * Useful for login, register, etc.
 */
export const PublicRoute: React.FC<{ children: React.ReactNode; fallbackPath?: string }> = ({ 
  children, 
  fallbackPath = '/dashboard' 
}) => {
  const { isAuthenticated, isLoading } = useAuth();

  // Show loading state while checking authentication
  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  // Redirect to dashboard if already authenticated
  if (isAuthenticated) {
    return <Navigate to={fallbackPath} replace />;
  }

  // User is not authenticated, show the public route
  return children;
};

/**
 * Role-based route component for fine-grained access control
 */
export const RoleRoute: React.FC<{ 
  children: React.ReactNode; 
  roles: string[]; 
  fallbackPath?: string;
}> = ({ 
  children, 
  roles, 
  fallbackPath = '/unauthorized' 
}) => {
  const { isAuthenticated, user, isLoading } = useAuth();

  // Show loading state while checking authentication
  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  // Redirect to login if not authenticated
  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: window.location.pathname }} replace />;
  }

  // Check if user has any of the required roles
  if (user && roles.length > 0) {
    const hasRequiredRole = roles.some(role => 
      user.roles?.includes(role)
    );

    if (!hasRequiredRole) {
      return <Navigate to={fallbackPath} replace />;
    }
  }

  // User has required role, show the protected content
  return children;
};

/**
 * AdminRoute component for admin-only routes
 */
export const AdminRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <RoleRoute roles={['admin', 'Admin']} fallbackPath="/unauthorized">
    {children}
  </RoleRoute>
);

/**
 * TeacherRoute component for teacher-only routes
 */
export const TeacherRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <RoleRoute roles={['teacher', 'Teacher']} fallbackPath="/unauthorized">
    {children}
  </RoleRoute>
);

/**
 * StudentRoute component for student-only routes
 */
export const StudentRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <RoleRoute roles={['student', 'Student']} fallbackPath="/unauthorized">
    {children}
  </RoleRoute>
);
