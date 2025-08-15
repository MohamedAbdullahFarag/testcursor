// Auth Service - API-based authentication
// Following Single Responsibility Principle - only API communication logic

import { LoginRequest, AuthResult } from '../models/auth.types';

const API = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

export const authService = {
  /**
   * Authenticate user with email and password
   */
  login: async (data: LoginRequest): Promise<AuthResult> => {
    const response = await fetch(`${API}/api/auth/login`, {
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

    return response.json() as Promise<AuthResult>;
  },

  /**
   * Refresh access token using refresh token
   */
  refresh: async (): Promise<AuthResult> => {
    const response = await fetch(`${API}/api/auth/refresh`, {
      method: 'POST',
      credentials: 'include',
    });

    if (!response.ok) {
      throw new Error('Token refresh failed');
    }

    return response.json() as Promise<AuthResult>;
  },

  /**
   * Logout user and invalidate tokens
   */
  logout: async (): Promise<void> => {
    const response = await fetch(`${API}/api/auth/logout`, {
      method: 'POST',
      credentials: 'include',
    });

    if (!response.ok) {
      throw new Error('Logout failed');
    }
  },

  /**
   * Get current user information
   */
  getCurrentUser: async (): Promise<AuthResult['user'] | null> => {
    try {
      const response = await fetch(`${API}/api/auth/me`, {
        credentials: 'include',
      });

      if (!response.ok) {
        return null;
      }

      const userData = await response.json();
      return userData.user;
    } catch (error) {
      console.error('Error getting current user:', error);
      return null;
    }
  },

  /**
   * Validate if current token is still valid
   */
  validateToken: async (): Promise<boolean> => {
    try {
      const response = await fetch(`${API}/api/auth/validate`, {
        credentials: 'include',
      });

      return response.ok;
    } catch (error) {
      console.error('Error validating token:', error);
      return false;
    }
  },
};
