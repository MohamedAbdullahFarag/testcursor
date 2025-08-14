import DevtoolsMiddlewares from '@/shared/store/middleware'
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
    DevtoolsMiddlewares(
        (persist as AuthStorePersist)(
            set => ({
                user: null,
                accessToken: null,
                refreshToken: null,
                isAuthenticated: false,
                isLoading: false,
                error: null,

                login: ({ user, accessToken, refreshToken }) =>
                    set({
                        user,
                        accessToken,
                        refreshToken,
                        isAuthenticated: true,
                        error: null,
                    }),

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
                        const refreshResult = await authService.refreshToken(
                            currentState.refreshToken ? { refreshToken: currentState.refreshToken } : undefined
                        );

                        if (refreshResult.success && refreshResult.data) {
                            set({
                                accessToken: refreshResult.data.accessToken,
                                refreshToken: refreshResult.data.refreshToken,
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
                                error: refreshResult.error?.message || 'Token refresh failed',
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
        {
            name: 'AuthStore',
        },
    ),
)
