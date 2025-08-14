/**
 * useRole - Role-based access control hook
 * Provides role checking utilities
 */

import { useMemo } from 'react';
import { useAuth } from './useAuth';

/**
 * Interface for role-based utilities
 */
export interface UseRoleReturn {
  /** Check if user has specific role */
  hasRole: (role: string | string[]) => boolean;
  /** Check if user has any of the specified roles */
  hasAnyRole: (roles: string[]) => boolean;
  /** Check if user has all specified roles */
  hasAllRoles: (roles: string[]) => boolean;
  /** Check if user is system admin */
  isSystemAdmin: boolean;
  /** Get user's roles */
  userRoles: string[];
}

/**
 * Custom hook for role-based access control
 * Provides utilities for checking user roles
 */
export const useRole = (): UseRoleReturn => {
  const { user, isAuthenticated } = useAuth();

  // Extract roles from user object
  const userRoles = useMemo(() => {
    if (!user || !isAuthenticated) return [];
    return user.roles || [];
  }, [user, isAuthenticated]);

  // Check if user has specific role
  const hasRole = useMemo(() => {
    return (role: string | string[]): boolean => {
      if (!isAuthenticated || !userRoles.length) return false;
      
      if (Array.isArray(role)) {
        return role.some(r => userRoles.includes(r));
      }
      return userRoles.includes(role);
    };
  }, [isAuthenticated, userRoles]);

  // Check if user has any of the specified roles
  const hasAnyRole = useMemo(() => {
    return (roles: string[]): boolean => {
      if (!isAuthenticated || !userRoles.length) return false;
      return roles.some(role => userRoles.includes(role));
    };
  }, [isAuthenticated, userRoles]);

  // Check if user has all specified roles
  const hasAllRoles = useMemo(() => {
    return (roles: string[]): boolean => {
      if (!isAuthenticated || !userRoles.length) return false;
      return roles.every(role => userRoles.includes(role));
    };
  }, [isAuthenticated, userRoles]);

  // Check if user is system admin
  const isSystemAdmin = useMemo(() => {
    return hasRole('system-admin');
  }, [hasRole]);

  return {
    hasRole,
    hasAnyRole,
    hasAllRoles,
    isSystemAdmin,
    userRoles,
  };
};
