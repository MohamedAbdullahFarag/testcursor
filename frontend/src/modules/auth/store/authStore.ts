import { create, StateCreator } from 'zustand'
import { persist, PersistOptions } from 'zustand/middleware'
import type { User, LoginParams } from '../models/auth.types'
import { authService } from '../services/authService'

// Auth state interface with store-specific methods
interface AuthState {
    // State properties
    user: User | null
    accessToken: string | null
    refreshToken: string | null
    isAuthenticated: boolean
    isLoading: boolean
    error: string | null
    
    // Actions
    login: (params: LoginParams) => void
    logout: () => Promise<void>
    clearAuth: () => void
    updateUser: (userData: Partial<User>) => void
    refreshAccessToken: () => Promise<boolean>
    setLoading: (loading: boolean) => void
    setError: (error: string | null) => void
}

// State properties to persist
interface PersistedState {
    user: User | null
    accessToken: string | null
    refreshToken: string | null
    isAuthenticated: boolean
}

type AuthStorePersist = (config: StateCreator<AuthState>, options: PersistOptions<PersistedState>) => StateCreator<AuthState>

export const useAuthStore = create<AuthState>()(
    (persist as AuthStorePersist)(
        (set) => ({
            user: null,
            accessToken: null,
            refreshToken: null,
            isAuthenticated: false,
            isLoading: false,
            error: null,

            login: ({ user, accessToken, refreshToken }) => {
                console.log('Auth store login called with:', { user, accessToken, refreshToken })
                set({
                    user,
                    accessToken,
                    refreshToken,
                    isAuthenticated: true,
                    error: null,
                })
                console.log('Auth store state updated successfully')
            },

            logout: async () => {
                try {
                    const currentState = useAuthStore.getState();
                    if (currentState.refreshToken) {
                        await authService.logout(currentState.refreshToken);
                    }
                } catch (error) {
                    console.error('Logout error:', error);
                } finally {
                    set({
                        user: null,
                        accessToken: null,
                        refreshToken: null,
                        isAuthenticated: false,
                        error: null,
                    });
                }
            },

            clearAuth: () =>
                set({
                    user: null,
                    accessToken: null,
                    refreshToken: null,
                    isAuthenticated: false,
                    error: null,
                }),

            updateUser: userData =>
                set(state => ({
                    user: state.user ? { ...state.user, ...userData } : null,
                })),

            refreshAccessToken: async () => {
                try {
                    set({ isLoading: true, error: null });
                    
                    const currentState = useAuthStore.getState();
                    if (!currentState.refreshToken) {
                        set({
                            user: null,
                            accessToken: null,
                            refreshToken: null,
                            isAuthenticated: false,
                            isLoading: false,
                            error: 'No refresh token available',
                        });
                        return false;
                    }
                    
                    const refreshResult = await authService.refresh(currentState.refreshToken);

                    if (refreshResult && refreshResult.accessToken) {
                        set({
                            accessToken: refreshResult.accessToken,
                            refreshToken: refreshResult.refreshToken,
                            isLoading: false,
                            error: null,
                        });
                        return true;
                    } else {
                        set({
                            user: null,
                            accessToken: null,
                            refreshToken: null,
                            isAuthenticated: false,
                            isLoading: false,
                            error: 'Token refresh failed',
                        });
                        return false;
                    }
                } catch (error) {
                    set({
                        user: null,
                        accessToken: null,
                        refreshToken: null,
                        isAuthenticated: false,
                        isLoading: false,
                        error: error instanceof Error ? error.message : 'Token refresh failed',
                    });
                    return false;
                }
            },

            setLoading: (loading: boolean) => set({ isLoading: loading }),

            setError: (error: string | null) => set({ error }),
        }),
        {
            name: 'auth-storage',
            partialize: (state): PersistedState => ({
                user: state.user,
                accessToken: state.accessToken,
                refreshToken: state.refreshToken,
                isAuthenticated: state.isAuthenticated,
            }),
        },
    ),
)
