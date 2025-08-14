/**
 * Authentication store using Zustand
 * Manages user authentication state, tokens, and auth-related operations
 */

import { create } from 'zustand'
import { persist, createJSONStorage } from 'zustand/middleware'
import type { AuthUser, AuthTokens } from '../models'
import { apiClient } from '../services/apiClient'

interface AuthState {
    // State
    user: AuthUser | null
    tokens: AuthTokens | null
    isAuthenticated: boolean
    isLoading: boolean
    error: string | null

    // Actions
    login: (email: string, password: string) => Promise<void>
    logout: () => void
    refreshToken: () => Promise<void>
    updateUser: (user: Partial<AuthUser>) => void
    clearError: () => void
    setLoading: (loading: boolean) => void
}

interface LoginResponse {
    user: AuthUser
    accessToken: string
    refreshToken: string
    expiresAt: string
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set, get) => ({
            // Initial state
            user: null,
            tokens: null,
            isAuthenticated: false,
            isLoading: false,
            error: null,

            // Login action
            login: async (email: string, password: string) => {
                try {
                    set({ isLoading: true, error: null })

                    const response = await apiClient.post<LoginResponse>('/auth/login', {
                        email,
                        password
                    })

                    if (response.success && response.data) {
                        const { user, accessToken, refreshToken, expiresAt } = response.data
                        
                        const tokens: AuthTokens = {
                            accessToken,
                            refreshToken,
                            expiresAt: new Date(expiresAt)
                        }

                        set({
                            user,
                            tokens,
                            isAuthenticated: true,
                            isLoading: false,
                            error: null
                        })
                    } else {
                        throw new Error(response.message || 'Login failed')
                    }
                } catch (error) {
                    const errorMessage = error instanceof Error ? error.message : 'Login failed'
                    set({
                        user: null,
                        tokens: null,
                        isAuthenticated: false,
                        isLoading: false,
                        error: errorMessage
                    })
                    throw error
                }
            },

            // Logout action
            logout: () => {
                try {
                    // Call logout API if available
                    apiClient.post('/auth/logout').catch(() => {
                        // Ignore errors for logout API call
                    })
                } catch {
                    // Ignore errors
                } finally {
                    // Clear local state
                    apiClient.logout()
                    set({
                        user: null,
                        tokens: null,
                        isAuthenticated: false,
                        isLoading: false,
                        error: null
                    })
                }
            },

            // Refresh token action
            refreshToken: async () => {
                try {
                    const currentTokens = get().tokens
                    if (!currentTokens?.refreshToken) {
                        throw new Error('No refresh token available')
                    }

                    set({ isLoading: true, error: null })

                    const response = await apiClient.post<LoginResponse>('/auth/refresh', {
                        refreshToken: currentTokens.refreshToken
                    })

                    if (response.success && response.data) {
                        const { user, accessToken, refreshToken, expiresAt } = response.data
                        
                        const tokens: AuthTokens = {
                            accessToken,
                            refreshToken,
                            expiresAt: new Date(expiresAt)
                        }

                        set({
                            user,
                            tokens,
                            isAuthenticated: true,
                            isLoading: false,
                            error: null
                        })
                    } else {
                        throw new Error(response.message || 'Token refresh failed')
                    }
                } catch (error) {
                    const errorMessage = error instanceof Error ? error.message : 'Token refresh failed'
                    set({
                        user: null,
                        tokens: null,
                        isAuthenticated: false,
                        isLoading: false,
                        error: errorMessage
                    })
                    throw error
                }
            },

            // Update user action
            updateUser: (userUpdate: Partial<AuthUser>) => {
                const currentUser = get().user
                if (currentUser) {
                    set({
                        user: { ...currentUser, ...userUpdate }
                    })
                }
            },

            // Clear error action
            clearError: () => {
                set({ error: null })
            },

            // Set loading action
            setLoading: (loading: boolean) => {
                set({ isLoading: loading })
            }
        }),
        {
            name: 'ikhtibar-auth-store',
            storage: createJSONStorage(() => localStorage),
            partialize: (state) => ({
                user: state.user,
                tokens: state.tokens,
                isAuthenticated: state.isAuthenticated
            })
        }
    )
)

// Selectors for commonly used state combinations
export const useAuthUser = () => useAuthStore((state) => state.user)
export const useIsAuthenticated = () => useAuthStore((state) => state.isAuthenticated)
export const useAuthLoading = () => useAuthStore((state) => state.isLoading)
export const useAuthError = () => useAuthStore((state) => state.error)

// Action selectors
export const useAuthActions = () => useAuthStore((state) => ({
    login: state.login,
    logout: state.logout,
    refreshToken: state.refreshToken,
    updateUser: state.updateUser,
    clearError: state.clearError,
    setLoading: state.setLoading
}))

// Token expiration check utility
export const useTokenExpiration = () => {
    const tokens = useAuthStore((state) => state.tokens)
    
    return {
        isTokenExpired: () => {
            if (!tokens?.expiresAt) return true
            return new Date() >= tokens.expiresAt
        },
        timeUntilExpiration: () => {
            if (!tokens?.expiresAt) return 0
            return Math.max(0, tokens.expiresAt.getTime() - Date.now())
        }
    }
}
