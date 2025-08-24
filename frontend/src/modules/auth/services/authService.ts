// Auth Service - API-based authentication
// Following Single Responsibility Principle - only API communication logic

import { LoginRequest, AuthResult } from '../models/auth.types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7001';

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

    return response.json() as Promise<AuthResult>;
  },

  /**
   * Refresh access token using refresh token
   */
  refresh: async (): Promise<AuthResult> => {
    const response = await fetch(`${API_BASE_URL}/api/auth/refresh`, {
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
    const response = await fetch(`${API_BASE_URL}/api/auth/logout`, {
      method: 'POST',
      credentials: 'include',
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
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      return response.ok;
    } catch {
      return false;
    }
  },
};
