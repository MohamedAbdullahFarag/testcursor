import { useAuthStore } from '../store/authStore';
import { tokenRefreshService } from './tokenRefreshService';

/**
 * Session Restoration Service
 * Handles automatic session restoration on page load
 */
class SessionRestorationService {
  private isInitialized = false;

  /**
   * Initialize session restoration
   * Should be called once when the app starts
   */
  initialize(): void {
    if (this.isInitialized) {
      return;
    }

    this.isInitialized = true;
    this.restoreSession();
  }

  /**
   * Restore user session from persisted storage
   */
  private async restoreSession(): Promise<void> {
    try {
      const store = useAuthStore.getState();
      
      // Check if we have persisted authentication data
      if (!store.accessToken || !store.user) {
        return;
      }

      // Check if token is expired
      if (tokenRefreshService.isTokenExpired(store.accessToken)) {
        // Try to refresh the token
        if (store.refreshToken) {
          try {
            await store.refreshAccessToken();
            // If refresh successful, start auto-refresh
            tokenRefreshService.startAutoRefresh();
          } catch (error) {
            console.error('Session restoration failed - token refresh failed:', error);
            // Clear invalid session
            store.clearAuth();
          }
        } else {
          // No refresh token, clear session
          store.clearAuth();
        }
      } else {
        // Token is valid, start auto-refresh
        tokenRefreshService.startAutoRefresh();
      }
    } catch (error) {
      console.error('Session restoration failed:', error);
      // Clear any corrupted session data
      useAuthStore.getState().clearAuth();
    }
  }

  /**
   * Check if user has a valid session
   */
  hasValidSession(): boolean {
    const store = useAuthStore.getState();
    
    if (!store.accessToken || !store.user) {
      return false;
    }

    return !tokenRefreshService.isTokenExpired(store.accessToken);
  }

  /**
   * Get current session info
   */
  getSessionInfo(): {
    isAuthenticated: boolean;
    user: any;
    tokenExpiry: Date | null;
    isExpiringSoon: boolean;
  } {
    const store = useAuthStore.getState();
    
    if (!store.accessToken || !store.user) {
      return {
        isAuthenticated: false,
        user: null,
        tokenExpiry: null,
        isExpiringSoon: false
      };
    }

    const tokenExpiry = tokenRefreshService.getTokenExpiry(store.accessToken);
    const isExpiringSoon = tokenRefreshService.isTokenExpiringSoon(store.accessToken);

    return {
      isAuthenticated: true,
      user: store.user,
      tokenExpiry,
      isExpiringSoon
    };
  }
}

export const sessionRestorationService = new SessionRestorationService();
