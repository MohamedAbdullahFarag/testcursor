// Auth Service - API-based authentication
// Following Single Responsibility Principle - only API communication logic

import { LoginRequest, AuthResult } from '../models/auth.types';

// Use relative URLs to leverage Vite's proxy configuration in development
const API_BASE_URL = import.meta.env.DEV ? '' : (import.meta.env.VITE_API_BASE_URL || 'https://localhost:7001');

export const authService = {
  /**
   * Authenticate user with email and password
   */
  login: async (data: LoginRequest): Promise<AuthResult> => {
    const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      if (response.status === 401) {
        throw new Error('Invalid credentials');
      }
      throw new Error('Login failed');
    }

    const backendResult = await response.json();
    
    // Map backend response to frontend format
    if (backendResult.success) {
      return {
        accessToken: backendResult.accessToken,
        refreshToken: backendResult.refreshTokens,
        user: {
          id: backendResult.user?.userId?.toString() || '',
          fullName: `${backendResult.user?.firstName || ''} ${backendResult.user?.lastName || ''}`.trim(),
          email: backendResult.user?.email || '',
          roles: backendResult.roles || []
        }
      };
    } else {
      throw new Error(backendResult.errorMessage || 'Login failed');
    }
  },

  /**
   * Refresh access token using refresh token
   */
  refresh: async (refreshToken: string): Promise<AuthResult> => {
    const response = await fetch(`${API_BASE_URL}/api/auth/refresh`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(refreshToken),
    });

    if (!response.ok) {
      throw new Error('Token refresh failed');
    }

    const backendResult = await response.json();
    
    // Map backend response to frontend format
    if (backendResult.success) {
      return {
        accessToken: backendResult.accessToken,
        refreshToken: backendResult.refreshTokens,
        user: {
          id: backendResult.user?.userId?.toString() || '',
          fullName: `${backendResult.user?.firstName || ''} ${backendResult.user?.lastName || ''}`.trim(),
          email: backendResult.user?.email || '',
          roles: backendResult.roles || []
        }
      };
    } else {
      throw new Error(backendResult.errorMessage || 'Token refresh failed');
    }
  },

  /**
   * Logout user and invalidate tokens
   */
  logout: async (refreshToken: string): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/api/auth/logout`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(refreshToken),
    });

    if (!response.ok) {
      throw new Error('Logout failed');
    }
  },

  /**
   * Validate current access token
   */
  validateToken: async (token: string): Promise<boolean> => {
    try {
      const response = await fetch(`${API_BASE_URL}/api/auth/validate`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(token),
      });

      return response.ok;
    } catch {
      return false;
    }
  },
};
