/**
 * Enhanced API client with interceptors, error handling, and type safety
 * Built on top of axios with token management and standardized responses
 */

import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse, AxiosError } from 'axios'
import type { ApiResponse, ApiError, ApiRequestConfig } from '../models'

/**
 * API client configuration
 */
interface ApiClientConfig {
    baseURL: string
    timeout: number
    retries: number
}

/**
 * Token management interface
 */
interface TokenManager {
    getAccessToken(): string | null
    getRefreshToken(): string | null
    setTokens(accessToken: string, refreshToken: string): void
    clearTokens(): void
    isTokenExpired(token: string): boolean
}

/**
 * Default token manager implementation using direct localStorage access
 */
class LocalStorageTokenManager implements TokenManager {
    private readonly ACCESS_TOKEN_KEY = 'ikhtibar_access_token'
    private readonly REFRESH_TOKEN_KEY = 'ikhtibar_refresh_token'

    getAccessToken(): string | null {
        return localStorage.getItem(this.ACCESS_TOKEN_KEY)
    }

    getRefreshToken(): string | null {
        return localStorage.getItem(this.REFRESH_TOKEN_KEY)
    }

    setTokens(accessToken: string, refreshToken: string): void {
        localStorage.setItem(this.ACCESS_TOKEN_KEY, accessToken)
        localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken)
    }

    clearTokens(): void {
        localStorage.removeItem(this.ACCESS_TOKEN_KEY)
        localStorage.removeItem(this.REFRESH_TOKEN_KEY)
    }

    isTokenExpired(token: string): boolean {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]))
            return payload.exp * 1000 < Date.now()
        } catch {
            return true
        }
    }
}

/**
 * Zustand auth store compatible token manager
 * Reads tokens from the persisted auth store in localStorage
 */
class ZustandTokenManager implements TokenManager {
    private readonly AUTH_STORAGE_KEY = 'auth-storage'

    private getAuthState(): any {
        try {
            const stored = localStorage.getItem(this.AUTH_STORAGE_KEY)
            if (!stored) return null
            
            const parsed = JSON.parse(stored)
            return parsed.state || null
        } catch {
            return null
        }
    }

    getAccessToken(): string | null {
        const authState = this.getAuthState()
        return authState?.accessToken || null
    }

    getRefreshToken(): string | null {
        const authState = this.getAuthState()
        return authState?.refreshToken || null
    }

    setTokens(accessToken: string, refreshToken: string): void {
        // This should be handled by the auth store, not directly here
        // We'll implement this for completeness but it's not the primary flow
        const authState = this.getAuthState() || {}
        const newState = {
            ...authState,
            accessToken,
            refreshToken,
            isAuthenticated: true
        }
        
        try {
            localStorage.setItem(this.AUTH_STORAGE_KEY, JSON.stringify({
                state: newState,
                version: 0
            }))
        } catch {
            // Silent fail - auth store should handle this
        }
    }

    clearTokens(): void {
        // This should be handled by the auth store clearAuth method
        // We'll implement this for completeness but it's not the primary flow
        try {
            localStorage.removeItem(this.AUTH_STORAGE_KEY)
        } catch {
            // Silent fail - auth store should handle this
        }
    }

    isTokenExpired(token: string): boolean {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]))
            return payload.exp * 1000 < Date.now()
        } catch {
            return true
        }
    }
}

/**
 * Enhanced API client class
 */
class ApiClient {
    private instance: AxiosInstance
    private tokenManager: TokenManager
    private isRefreshing = false
    private refreshPromise: Promise<string> | null = null

    constructor(config: ApiClientConfig, tokenManager?: TokenManager) {
        this.tokenManager = tokenManager || new LocalStorageTokenManager()
        
        this.instance = axios.create({
            baseURL: config.baseURL,
            timeout: config.timeout,
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'X-Requested-With': 'XMLHttpRequest',
            },
        })

        this.setupInterceptors()
    }

    /**
     * Setup request and response interceptors
     */
    private setupInterceptors(): void {
        // Request interceptor for adding auth token
        this.instance.interceptors.request.use(
            (config) => {
                const token = this.tokenManager.getAccessToken()
                if (token && !this.tokenManager.isTokenExpired(token)) {
                    config.headers.Authorization = `Bearer ${token}`
                }
                return config
            },
            (error) => Promise.reject(error)
        )

        // Response interceptor for handling errors and token refresh
        this.instance.interceptors.response.use(
            (response) => response,
            async (error: AxiosError) => {
                const originalRequest = error.config as any

                if (error.response?.status === 401 && originalRequest && !originalRequest._retry) {
                    if (this.isRefreshing) {
                        // Wait for the current refresh to complete
                        try {
                            const newToken = await this.refreshPromise
                            originalRequest.headers.Authorization = `Bearer ${newToken}`
                            return this.instance(originalRequest)
                        } catch (refreshError) {
                            return Promise.reject(refreshError)
                        }
                    }

                    originalRequest._retry = true
                    return this.refreshToken().then((newToken) => {
                        originalRequest.headers.Authorization = `Bearer ${newToken}`
                        return this.instance(originalRequest)
                    }).catch((refreshError) => {
                        this.tokenManager.clearTokens()
                        // Redirect to login or emit logout event
                        window.location.href = '/login'
                        return Promise.reject(refreshError)
                    })
                }

                return Promise.reject(this.handleError(error))
            }
        )
    }

    /**
     * Refresh access token using refresh token
     */
    private async refreshToken(): Promise<string> {
        if (this.refreshPromise) {
            return this.refreshPromise
        }

        this.isRefreshing = true
        this.refreshPromise = this.performTokenRefresh()

        try {
            const newToken = await this.refreshPromise
            return newToken
        } finally {
            this.isRefreshing = false
            this.refreshPromise = null
        }
    }

    /**
     * Perform the actual token refresh
     */
    private async performTokenRefresh(): Promise<string> {
        const refreshToken = this.tokenManager.getRefreshToken()
        
        if (!refreshToken || this.tokenManager.isTokenExpired(refreshToken)) {
            throw new Error('No valid refresh token available')
        }

        try {
            const response = await axios.post(`${this.instance.defaults.baseURL}/auth/refresh`, {
                refreshToken
            })

            const { accessToken, refreshToken: newRefreshToken } = response.data.data
            this.tokenManager.setTokens(accessToken, newRefreshToken)
            
            return accessToken
        } catch (error) {
            this.tokenManager.clearTokens()
            throw error
        }
    }

    /**
     * Handle and standardize API errors
     */
    private handleError(error: AxiosError): ApiError {
        if (error.response) {
            // Server responded with error status
            const { status, data } = error.response
            return {
                success: false,
                message: (data as any)?.message || error.message,
                errors: (data as any)?.errors || [],
                statusCode: status,
                timestamp: new Date().toISOString(),
                path: error.config?.url || '',
            }
        } else if (error.request) {
            // Network error
            return {
                success: false,
                message: 'Network error - please check your connection',
                statusCode: 0,
                timestamp: new Date().toISOString(),
            }
        } else {
            // Request setup error
            return {
                success: false,
                message: error.message,
                timestamp: new Date().toISOString(),
            }
        }
    }

    /**
     * GET request
     */
    async get<T>(url: string, config?: ApiRequestConfig): Promise<ApiResponse<T>> {
        const axiosConfig: any = {}
        if (config?.params) {
            axiosConfig.params = config.params
        }
        if (config?.responseType) {
            axiosConfig.responseType = config.responseType
        }
        if (config?.signal) {
            axiosConfig.signal = config.signal
        }
        
        const response = await this.instance.get<ApiResponse<T>>(url, axiosConfig)
        return response.data
    }

    /**
     * POST request
     */
    async post<T, D = unknown>(url: string, data?: D, config?: ApiRequestConfig): Promise<ApiResponse<T>> {
        const axiosConfig: any = {}
        if (config?.params) {
            axiosConfig.params = config.params
        }
        if (config?.responseType) {
            axiosConfig.responseType = config.responseType
        }
        if (config?.signal) {
            axiosConfig.signal = config.signal
        }
        
        const response = await this.instance.post<ApiResponse<T>>(url, data, axiosConfig)
        return response.data
    }

    /**
     * PUT request
     */
    async put<T, D = unknown>(url: string, data?: D, config?: ApiRequestConfig): Promise<ApiResponse<T>> {
        const response = await this.instance.put<ApiResponse<T>>(url, data, config)
        return response.data
    }

    /**
     * DELETE request
     */
    async delete<T>(url: string, config?: ApiRequestConfig): Promise<ApiResponse<T>> {
        const response = await this.instance.delete<ApiResponse<T>>(url, config)
        return response.data
    }

    /**
     * PATCH request
     */
    async patch<T, D = unknown>(url: string, data?: D, config?: ApiRequestConfig): Promise<ApiResponse<T>> {
        const response = await this.instance.patch<ApiResponse<T>>(url, data, config)
        return response.data
    }

    /**
     * Upload file
     */
    async upload<T>(url: string, file: File, onProgress?: (progress: number) => void): Promise<ApiResponse<T>> {
        const formData = new FormData()
        formData.append('file', file)

        const response = await this.instance.post<ApiResponse<T>>(url, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
            onUploadProgress: (progressEvent) => {
                if (onProgress && progressEvent.total) {
                    const progress = Math.round((progressEvent.loaded * 100) / progressEvent.total)
                    onProgress(progress)
                }
            },
        })

        return response.data
    }

    /**
     * Download file
     */
    async download(url: string, filename?: string): Promise<void> {
        const response = await this.instance.get(url, {
            responseType: 'blob',
        })

        const blob = new Blob([response.data])
        const downloadUrl = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = downloadUrl
        link.download = filename || 'download'
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(downloadUrl)
    }

    /**
     * Set new base URL
     */
    setBaseURL(baseURL: string): void {
        this.instance.defaults.baseURL = baseURL
    }

    /**
     * Get current base URL
     */
    getBaseURL(): string {
        return this.instance.defaults.baseURL || ''
    }

    /**
     * Clear all tokens
     */
    logout(): void {
        this.tokenManager.clearTokens()
    }
}

// Create and export default API client instance
const apiConfig: ApiClientConfig = {
    baseURL: import.meta.env.DEV ? '' : (import.meta.env.VITE_API_BASE_URL || 'https://localhost:7001'),
    timeout: 30000, // 30 seconds
    retries: 3,
}

// Use LocalStorageTokenManager by default for reading tokens from localStorage
export const apiClient = new ApiClient(apiConfig, new ZustandTokenManager())

// Export types and classes for advanced usage
export { ApiClient, LocalStorageTokenManager, ZustandTokenManager }
export type { ApiClientConfig, TokenManager }
