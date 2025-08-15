import React, { createContext, useContext, useState, useCallback, useEffect } from 'react';
import { authService } from '../services/authService';
import { AuthContextType, LoginCredentials, AuthResult } from '../models/auth.types';

// Create authentication context
export const AuthContext = createContext<AuthContextType | null>(null);

// Authentication provider component
export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<AuthResult['user'] | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [refreshToken, setRefreshToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Check if user is authenticated
  const isAuthenticated = !!user && !!accessToken;

  // Login function
  const login = useCallback(async (credentials: LoginCredentials): Promise<void> => {
    try {
      setIsLoading(true);
      setError(null);

      const result = await authService.login(credentials);
      
      // Store tokens securely
      setAccessToken(result.accessToken);
      setRefreshToken(result.refreshToken);
      setUser(result.user);

      // Store access token in localStorage for API calls
      localStorage.setItem('accessToken', result.accessToken);
      
      // Store refresh token in httpOnly cookie (handled by backend)
      // The refresh token is automatically included in subsequent requests
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Login failed';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  // Logout function
  const logout = useCallback(async (): Promise<void> => {
    try {
      setIsLoading(true);
      
      // Call logout API to invalidate tokens
      await authService.logout();
      
      // Clear local state
      setUser(null);
      setAccessToken(null);
      setRefreshToken(null);
      
      // Clear localStorage
      localStorage.removeItem('accessToken');
      
    } catch (err) {
      console.error('Logout error:', err);
      // Even if API call fails, clear local state
      setUser(null);
      setAccessToken(null);
      setRefreshToken(null);
      localStorage.removeItem('accessToken');
    } finally {
      setIsLoading(false);
    }
  }, []);

  // Refresh token function
  const refreshToken = useCallback(async (): Promise<void> => {
    try {
      const result = await authService.refresh();
      
      // Update tokens
      setAccessToken(result.accessToken);
      setRefreshToken(result.refreshToken);
      setUser(result.user);
      
      // Update localStorage
      localStorage.setItem('accessToken', result.accessToken);
      
    } catch (err) {
      console.error('Token refresh failed:', err);
      // If refresh fails, logout user
      await logout();
    }
  }, [logout]);

  // Clear error function
  const clearError = useCallback(() => {
    setError(null);
  }, []);

  // Check authentication status on mount
  useEffect(() => {
    const checkAuthStatus = async () => {
      try {
        const storedToken = localStorage.getItem('accessToken');
        if (storedToken) {
          // Validate stored token
          const isValid = await authService.validateToken();
          if (isValid) {
            // Get current user info
            const currentUser = await authService.getCurrentUser();
            if (currentUser) {
              setUser(currentUser);
              setAccessToken(storedToken);
            } else {
              // Clear invalid token
              localStorage.removeItem('accessToken');
            }
          } else {
            // Clear invalid token
            localStorage.removeItem('accessToken');
          }
        }
      } catch (err) {
        console.error('Error checking auth status:', err);
        localStorage.removeItem('accessToken');
      }
    };

    checkAuthStatus();
  }, []);

  // Set up automatic token refresh
  useEffect(() => {
    if (!accessToken) return;

    // Set up interval to refresh token before it expires
    const refreshInterval = setInterval(() => {
      // Refresh token every 14 minutes (assuming 15-minute expiry)
      refreshToken();
    }, 14 * 60 * 1000);

    return () => clearInterval(refreshInterval);
  }, [accessToken, refreshToken]);

  // Context value
  const contextValue: AuthContextType = {
    user,
    accessToken,
    isAuthenticated,
    isLoading,
    error,
    login,
    logout,
    refreshToken,
    clearError,
  };

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
};

// Custom hook to use authentication context
export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
};
