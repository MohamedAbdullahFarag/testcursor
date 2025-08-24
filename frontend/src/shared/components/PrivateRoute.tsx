import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../modules/auth/hooks/useAuth';
import { CircularProgress, Box } from '@mui/material';

interface PrivateRouteProps {
  children: React.ReactElement;
  redirectTo?: string;
}

export const PrivateRoute: React.FC<PrivateRouteProps> = ({ 
  children, 
  redirectTo = '/login' 
}) => {
  const { accessToken, isLoading } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="100vh"
      >
        <CircularProgress />
      </Box>
    );
  }

  if (!accessToken) {
    // Redirect to login page with the current location as state
    // This allows us to redirect back after successful login
    return <Navigate to={redirectTo} state={{ from: location }} replace />;
  }

  return children;
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
