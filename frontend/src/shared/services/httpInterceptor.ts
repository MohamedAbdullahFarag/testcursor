// HTTP Interceptor for Automatic Token Refresh
// Following Single Responsibility Principle - only HTTP request/response interception

import { authService } from '@/modules/auth/services/authService';
import type { AuthServiceResponse, RefreshTokenResponse } from '@/modules/auth/models/auth.types';

interface QueuedRequest {
  resolve: (value: Response) => void;
  reject: (reason?: unknown) => void;
  url: string;
  options: RequestInit;
}

class HttpInterceptor {
  private static instance: HttpInterceptor;
  private isRefreshing = false;
  private failedQueue: QueuedRequest[] = [];

  public static getInstance(): HttpInterceptor {
    if (!HttpInterceptor.instance) {
      HttpInterceptor.instance = new HttpInterceptor();
    }
    return HttpInterceptor.instance;
  }

  /**
   * Enhanced fetch with automatic token refresh on 401
   * @param url Request URL
   * @param options Fetch options
   * @returns Promise with response
   */
  public async fetch(url: string, options: RequestInit = {}): Promise<Response> {
    // Get auth store dynamically to avoid circular dependencies
    const getAuthStore = () => {
      // Lazy import to avoid circular dependency
      return import('@/modules/auth/store/authStore').then(module => module.useAuthStore.getState());
    };

    const authState = await getAuthStore();
    
    // Add authorization header if token exists
    if (authState.accessToken) {
      const headers = new Headers(options.headers);
      if (!headers.has('Authorization')) {
        headers.set('Authorization', `Bearer ${authState.accessToken}`);
        options.headers = headers;
      }
    }

    const response = await fetch(url, options);

    // If 401, attempt token refresh
    if (response.status === 401 && authState.accessToken) {
      return this.handleTokenRefresh(url, options);
    }

    return response;
  }

  /**
   * Handle token refresh and retry failed request
   * @param url Original request URL
   * @param options Original request options
   * @returns Promise with response after token refresh
   */
  private async handleTokenRefresh(url: string, options: RequestInit): Promise<Response> {
    if (this.isRefreshing) {
      // If refresh is already in progress, queue this request
      return new Promise((resolve, reject) => {
        this.failedQueue.push({ resolve, reject, url, options });
      });
    }

    this.isRefreshing = true;

    try {
      // Get current auth state
      const getAuthStore = () => {
        return import('@/modules/auth/store/authStore').then(module => module.useAuthStore.getState());
      };

      const authState = await getAuthStore();

      // Attempt token refresh
      const refreshResult: AuthServiceResponse<RefreshTokenResponse> = await authService.refreshToken(
        authState.refreshToken ? { refreshToken: authState.refreshToken } : undefined
      );

      if (refreshResult.success && refreshResult.data) {
        // Update auth store with new tokens
        const { login } = await getAuthStore();
        login({
          user: authState.user,
          accessToken: refreshResult.data.accessToken,
          refreshToken: refreshResult.data.refreshToken,
        });

        // Update authorization header with new token
        const updatedOptions = { ...options };
        const headers = new Headers(updatedOptions.headers);
        headers.set('Authorization', `Bearer ${refreshResult.data.accessToken}`);
        updatedOptions.headers = headers;

        // Retry original request with new token
        const retryResponse = await fetch(url, updatedOptions);

        // Process queued requests with new token
        this.processQueue(null, refreshResult.data.accessToken);

        return retryResponse;
      } else {
        // Refresh failed, clear auth and reject all queued requests
        const { clearAuth } = await getAuthStore();
        clearAuth();
        
        this.processQueue(new Error('Token refresh failed'), null);
        
        // Redirect to login or throw error
        throw new Error('Authentication expired. Please login again.');
      }
    } catch (error) {
      // Clear auth state and process queue with error
      const { clearAuth } = await import('@/modules/auth/store/authStore').then(module => module.useAuthStore.getState());
      clearAuth();
      
      this.processQueue(error, null);
      throw error;
    } finally {
      this.isRefreshing = false;
    }
  }

  /**
   * Process queued requests after token refresh
   * @param error Error if refresh failed
   * @param token New access token if refresh succeeded
   */
  private processQueue(error: unknown, token: string | null): void {
    this.failedQueue.forEach(({ resolve, reject, url, options }) => {
      if (error) {
        reject(error);
      } else if (token) {
        // Update authorization header and retry request
        const updatedOptions = { ...options };
        const headers = new Headers(updatedOptions.headers);
        headers.set('Authorization', `Bearer ${token}`);
        updatedOptions.headers = headers;
        
        fetch(url, updatedOptions)
          .then(resolve)
          .catch(reject);
      }
    });

    this.failedQueue = [];
  }

  /**
   * Create a configured fetch instance with interceptors
   * @returns Configured fetch function
   */
  public createFetch(): (url: string, options?: RequestInit) => Promise<Response> {
    return (url: string, options?: RequestInit) => this.fetch(url, options);
  }
}

// Export singleton instance
export const httpInterceptor = HttpInterceptor.getInstance();

// Create enhanced fetch function
export const interceptedFetch = httpInterceptor.createFetch();

// ❌ DON'T: Handle UI logic in interceptor
// ❌ DON'T: Mix auth business logic with HTTP concerns
// ❌ DON'T: Use sync methods for I/O operations
// ❌ DON'T: Hardcode error messages (should be localized)
// ❌ DON'T: Skip proper error propagation
