// useAuth Hook - Comprehensive authentication hook
// Following Single Responsibility Principle - only auth state management and operations

import { useCallback } from 'react';
import { useAuthStore } from '../store/authStore';
import { authService } from '../services/authService';
import type { LoginRequest, AuthContextType } from '../models/auth.types';

/**
 * Custom hook for authentication operations
 * Provides centralized access to auth state and operations
 * @returns AuthContextType with user, tokens, and auth operations
 */
export const useAuth = (): AuthContextType => {
  // Get auth state and actions from store
  const {
    user,
    accessToken,
    isAuthenticated,
    isLoading,
    error,
    login: loginStore,
    clearAuth,
    refreshAccessToken,
    setLoading,
    setError,
  } = useAuthStore();

  /**
   * Login with email and password
   * @param credentials User credentials
   */
  const login = useCallback(async (credentials: LoginRequest): Promise<void> => {
    try {
      setLoading(true);
      setError(null);

      const result = await authService.login(credentials);

      if (result.success && result.data) {
        loginStore({
          user: result.data.user,
          accessToken: result.data.accessToken,
          refreshToken: result.data.refreshToken,
        });
      } else {
        const errorMessage = result.error?.message || 'Login failed';
        setError(errorMessage);
        throw new Error(errorMessage);
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Login failed';
      setError(errorMessage);
      throw error;
    } finally {
      setLoading(false);
    }
  }, [loginStore, setLoading, setError]);

  /**
   * Logout user and clear auth state
   */
  const logout = useCallback(async (): Promise<void> => {
    try {
      setLoading(true);
      setError(null);

      // Call logout API to invalidate tokens on server
      await authService.logout();
    } catch (error) {
      // Even if server logout fails, clear local state
      console.warn('Logout API call failed:', error);
    } finally {
      // Always clear local auth state
      clearAuth();
      setLoading(false);
    }
  }, [clearAuth, setLoading, setError]);

  /**
   * Refresh access token
   */
  const refreshToken = useCallback(async (): Promise<void> => {
    const success = await refreshAccessToken();
    if (!success) {
      throw new Error('Token refresh failed');
    }
  }, [refreshAccessToken]);

  /**
   * Clear any authentication errors
   */
  const clearError = useCallback((): void => {
    setError(null);
  }, [setError]);

  return {
    // State
    user,
    accessToken,
    isAuthenticated,
    isLoading,
    error,
    
    // Actions
    login,
    logout,
    refreshToken,
    clearError,
  };
};

// ❌ DON'T: Mix UI logic in auth hook
// ❌ DON'T: Handle routing in auth hook
// ❌ DON'T: Store sensitive data in local state
// ❌ DON'T: Skip error handling in async operations
// ❌ DON'T: Forget to cleanup in finally blocks
