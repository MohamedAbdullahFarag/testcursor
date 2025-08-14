/**
 * Core API response types and interfaces for the Ikhtibar frontend
 * These types ensure type safety across all API interactions
 */

/**
 * Standard API response wrapper
 * @template T - The type of data being returned
 */
export interface ApiResponse<T = unknown> {
    success: boolean
    data: T
    message?: string
    errors?: string[]
    timestamp?: string
}

/**
 * Paginated response structure for list endpoints
 * @template T - The type of items in the data array
 */
export interface PaginatedResponse<T = unknown> {
    data: T[]
    pagination: {
        currentPage: number
        totalPages: number
        totalCount: number
        pageSize: number
        hasNextPage: boolean
        hasPreviousPage: boolean
    }
    success: boolean
    message?: string
    errors?: string[]
}

/**
 * API error response structure
 */
export interface ApiError {
    success: false
    message: string
    errors?: string[]
    statusCode?: number
    timestamp?: string
    path?: string
}

/**
 * Request configuration for API calls
 */
export interface ApiRequestConfig {
    timeout?: number
    retries?: number
    cache?: boolean
    signal?: AbortSignal
}

/**
 * Loading state for async operations
 */
export interface LoadingState {
    isLoading: boolean
    error: string | null
    lastUpdated?: Date
}

/**
 * Generic form state
 */
export interface FormState<T = unknown> extends LoadingState {
    data: T
    isDirty: boolean
    isValid: boolean
    validationErrors: Record<string, string>
}

/**
 * Authentication related types
 */
export interface AuthTokens {
    accessToken: string
    refreshToken: string
    expiresAt: Date
}

export interface AuthUser {
    id: number
    email: string
    name: string
    roles: string[]
    permissions: string[]
    isActive: boolean
}

/**
 * App configuration types
 */
export interface AppConfig {
    apiBaseUrl: string
    environment: 'development' | 'staging' | 'production'
    features: {
        darkMode: boolean
        notifications: boolean
        analytics: boolean
    }
    theme: {
        primaryColor: string
        secondaryColor: string
        mode: 'light' | 'dark'
    }
    locale: {
        language: 'en' | 'ar'
        direction: 'ltr' | 'rtl'
        timeZone: string
    }
}

/**
 * HTTP status codes commonly used in the application
 */
export enum HttpStatusCode {
    OK = 200,
    CREATED = 201,
    NO_CONTENT = 204,
    BAD_REQUEST = 400,
    UNAUTHORIZED = 401,
    FORBIDDEN = 403,
    NOT_FOUND = 404,
    UNPROCESSABLE_ENTITY = 422,
    INTERNAL_SERVER_ERROR = 500,
}

/**
 * API endpoint methods
 */
export type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH'

/**
 * Generic entity with common fields
 */
export interface BaseEntity {
    id: string
    createdAt: Date
    updatedAt: Date
    createdBy?: string
    updatedBy?: string
}
