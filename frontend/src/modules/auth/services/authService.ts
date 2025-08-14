// Auth Service - API-based authentication
// Following Single Responsibility Principle - only API communication logic

import type { 
  LoginRequest, 
  AuthResult, 
  RefreshTokenRequest, 
  RefreshTokenResponse,
  AuthServiceResponse 
} from '../models/auth.types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

class AuthService {
  private static instance: AuthService;

  public static getInstance(): AuthService {
    if (!AuthService.instance) {
      AuthService.instance = new AuthService();
    }
    return AuthService.instance;
  }

  /**
   * Login with email and password
   * @param credentials Login credentials
   * @returns Promise with auth result
   */
  public async login(credentials: LoginRequest): Promise<AuthServiceResponse<AuthResult>> {
    try {
      const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include', // Include cookies for refresh token
        body: JSON.stringify(credentials),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        return {
          success: false,
          error: {
            message: errorData.message || 'Invalid credentials',
            statusCode: response.status,
            details: errorData.details,
          },
        };
      }

      const data: AuthResult = await response.json();
      return {
        success: true,
        data,
      };
    } catch (error) {
      return {
        success: false,
        error: {
          message: 'Network error occurred',
          statusCode: 0,
          details: error instanceof Error ? error.message : 'Unknown error',
        },
      };
    }
  }

  /**
   * Refresh access token using refresh token
   * @param refreshTokenData Refresh token data
   * @returns Promise with new tokens
   */
  public async refreshToken(refreshTokenData?: RefreshTokenRequest): Promise<AuthServiceResponse<RefreshTokenResponse>> {
    try {
      const response = await fetch(`${API_BASE_URL}/api/auth/refresh`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include', // Use httpOnly cookie if available
        body: refreshTokenData ? JSON.stringify(refreshTokenData) : undefined,
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        return {
          success: false,
          error: {
            message: errorData.message || 'Token refresh failed',
            statusCode: response.status,
            details: errorData.details,
          },
        };
      }

      const data: RefreshTokenResponse = await response.json();
      return {
        success: true,
        data,
      };
    } catch (error) {
      return {
        success: false,
        error: {
          message: 'Network error during token refresh',
          statusCode: 0,
          details: error instanceof Error ? error.message : 'Unknown error',
        },
      };
    }
  }

  /**
   * Logout user and invalidate tokens
   * @returns Promise indicating success/failure
   */
  public async logout(): Promise<AuthServiceResponse<void>> {
    try {
      const response = await fetch(`${API_BASE_URL}/api/auth/logout`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        return {
          success: false,
          error: {
            message: errorData.message || 'Logout failed',
            statusCode: response.status,
            details: errorData.details,
          },
        };
      }

      return {
        success: true,
      };
    } catch (error) {
      return {
        success: false,
        error: {
          message: 'Network error during logout',
          statusCode: 0,
          details: error instanceof Error ? error.message : 'Unknown error',
        },
      };
    }
  }

  /**
   * Validate current access token
   * @param token Access token to validate
   * @returns Promise indicating if token is valid
   */
  public async validateToken(token: string): Promise<AuthServiceResponse<boolean>> {
    try {
      const response = await fetch(`${API_BASE_URL}/api/auth/validate`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      return {
        success: true,
        data: response.ok,
      };
    } catch (error) {
      return {
        success: false,
        error: {
          message: 'Token validation failed',
          statusCode: 0,
          details: error instanceof Error ? error.message : 'Unknown error',
        },
      };
    }
  }
}

// Export singleton instance
export const authService = AuthService.getInstance();

// ❌ DON'T: Mix business logic in service
// ❌ DON'T: Handle UI state in service
// ❌ DON'T: Catch generic exceptions without specific handling
// ❌ DON'T: Hardcode configuration values (using env vars)
// ❌ DON'T: Skip input validation (handled by TypeScript)
