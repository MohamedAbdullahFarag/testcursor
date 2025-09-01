import { authService } from './authService';
import { useAuthStore } from '../store/authStore';

/**
 * Token Refresh Service
 * Handles automatic token refresh and expiration
 */
class TokenRefreshService {
  private refreshTimer: NodeJS.Timeout | null = null;
  private readonly REFRESH_THRESHOLD_MINUTES = 5; // Refresh token 5 minutes before expiration

  /**
   * Start automatic token refresh
   */
  startAutoRefresh(): void {
    this.stopAutoRefresh();
    
    const store = useAuthStore.getState();
    if (!store.accessToken) {
      return;
    }

    // Calculate when to refresh (5 minutes before expiration)
    const tokenExpiry = this.getTokenExpiry(store.accessToken);
    if (!tokenExpiry) {
      return;
    }

    const refreshTime = new Date(tokenExpiry.getTime() - (this.REFRESH_THRESHOLD_MINUTES * 60 * 1000));
    const now = new Date();
    
    if (refreshTime > now) {
      const delay = refreshTime.getTime() - now.getTime();
      this.refreshTimer = setTimeout(() => {
        this.refreshToken();
      }, delay);
    } else {
      // Token is already expired or close to expiry, refresh immediately
      this.refreshToken();
    }
  }

  /**
   * Stop automatic token refresh
   */
  stopAutoRefresh(): void {
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = null;
    }
  }

  /**
   * Refresh the access token
   */
  private async refreshToken(): Promise<void> {
    try {
      const store = useAuthStore.getState();
      if (!store.refreshToken) {
        this.handleTokenExpired();
        return;
      }

      const result = await authService.refresh(store.refreshToken);
      
      // Update store with new tokens
      useAuthStore.getState().login({
        user: result.user,
        accessToken: result.accessToken,
        refreshToken: result.refreshToken
      });

      // Restart auto-refresh with new token
      this.startAutoRefresh();
      
    } catch (error) {
      console.error('Token refresh failed:', error);
      this.handleTokenExpired();
    }
  }

  /**
   * Handle token expiration
   */
  private handleTokenExpired(): void {
    // Clear tokens and redirect to login
    useAuthStore.getState().clearAuth();
    
    // Redirect to login page
    if (typeof window !== 'undefined') {
      window.location.href = '/login';
    }
  }

  /**
   * Get token expiry time from JWT
   */
  getTokenExpiry(token: string): Date | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      if (payload.exp) {
        return new Date(payload.exp * 1000);
      }
    } catch (error) {
      console.error('Failed to parse token:', error);
    }
    return null;
  }

  /**
   * Check if token is expired
   */
  isTokenExpired(token: string): boolean {
    const expiry = this.getTokenExpiry(token);
    if (!expiry) {
      return true;
    }
    return new Date() >= expiry;
  }

  /**
   * Check if token will expire soon
   */
  isTokenExpiringSoon(token: string, thresholdMinutes: number = 5): boolean {
    const expiry = this.getTokenExpiry(token);
    if (!expiry) {
      return true;
    }
    
    const threshold = new Date(expiry.getTime() - (thresholdMinutes * 60 * 1000));
    return new Date() >= threshold;
  }
}

export const tokenRefreshService = new TokenRefreshService();
