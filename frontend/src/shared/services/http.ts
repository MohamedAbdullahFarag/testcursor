import axios, { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse, InternalAxiosRequestConfig } from 'axios'
import { StatusCodes } from '../models/enums'
import { APIResponseError } from '../models/interfaces'

// Queue interface for failed requests
interface QueueItem {
    resolve: (value: unknown) => void
    reject: (reason?: unknown) => void
    config: AxiosRequestConfig
}

const headers: Readonly<Record<string, string | boolean>> = {
    Accept: 'application/json',
    'Content-Type': 'application/json; charset=utf-8',
    'Access-Control-Allow-Credentials': true,
    'X-Requested-With': 'XMLHttpRequest',
    Clientid: '64991F09-44E4-4F7B-A2DE-BFDE8F33CADB',
}

class Http {
    private baseURL: string
    private instance: AxiosInstance | null = null
    private isRefreshing = false
    private failedQueue: QueueItem[] = []

    constructor(baseUrl: string) {
        this.baseURL = baseUrl
    }

    private get http(): AxiosInstance {
        return this.instance != null ? this.instance : this.initHttp()
    }

    private processQueue(error: Error | null, token: string | null = null): void {
        this.failedQueue.forEach(promise => {
            if (error) {
                promise.reject(error)
            } else {
                // Retry the request with new token
                const config = promise.config
                if (token && config.headers) {
                    config.headers.Authorization = `Bearer ${token}`
                }
                promise.resolve(this.http.request(config))
            }
        })

        this.failedQueue = []
    }

    initHttp() {
        const http = axios.create({
            baseURL: this.baseURL,
            headers,
        })

        http.interceptors.request.use(
            (config: InternalAxiosRequestConfig) => {
                // Don't inject token for refresh token requests
                if (config.url?.includes('/refresh-token')) {
                    return config
                }

                try {
                    const authStorage = localStorage.getItem('auth-storage')
                    const token = authStorage ? JSON.parse(authStorage).state?.accessToken : undefined
                    const roleCode = authStorage ? JSON.parse(authStorage).state?.user?.roleCode : undefined
                    const instituteId = authStorage ? JSON.parse(authStorage).state?.user?.instituteId : undefined
                    if (token) {
                        config.headers.Authorization = `Bearer ${token}`
                    }
                    if (roleCode) {
                        config.headers['X-Role-Code'] = roleCode
                    }
                    if (instituteId) {
                        config.headers['X-Institute-Id'] = instituteId
                    }
                    return config
                } catch (error) {
                    throw (error as Error).message
                }
            },
            error => Promise.reject(new Error(error)),
        )

        http.interceptors.response.use(
            response => response,
            async (error: AxiosError) => {
                const originalRequest = error.config as AxiosRequestConfig & { _retry?: boolean }

                // If error is not 401 or it's a refresh token request that failed, reject
                if (error.response?.status !== StatusCodes.Unauthorized || originalRequest.url?.includes('/refresh-token')) {
                    return this.handleError(error)
                }

                // If we already tried to refresh, reject
                if (originalRequest._retry) {
                    this.handleUnauthorized()
                    return Promise.reject(new Error(error.message))
                }

                if (!this.isRefreshing) {
                    this.isRefreshing = true
                    originalRequest._retry = true

                    try {
                        const authStorage = localStorage.getItem('auth-storage')
                        const refreshToken = authStorage ? JSON.parse(authStorage).state.refreshToken : undefined
                        if (!refreshToken) {
                            throw new Error('No refresh token available')
                        }

                        // Call your refresh token endpoint
                        const response = await this.http.post<{ accessToken: string; refreshToken: string }>('/refresh-token', {
                            refreshToken,
                        })

                        const { accessToken, refreshToken: newRefreshToken } = response.data

                        // Save new tokens
                        localStorage.setItem('accessToken', accessToken)
                        localStorage.setItem('refreshToken', newRefreshToken)

                        // Update axios default header
                        this.http.defaults.headers.common.Authorization = `Bearer ${accessToken}`

                        // Process queue with new token
                        this.processQueue(null, accessToken)

                        // Retry original request
                        if (originalRequest.headers) {
                            originalRequest.headers.Authorization = `Bearer ${accessToken}`
                        }
                        return this.http(originalRequest)
                    } catch (refreshError) {
                        this.processQueue(refreshError as Error, null)
                        this.handleUnauthorized()
                        return Promise.reject(new Error(refreshError instanceof Error ? refreshError.message : 'Unknown Error'))
                    } finally {
                        this.isRefreshing = false
                    }
                }

                // If refreshing is in progress, add request to queue
                return new Promise((resolve, reject) => {
                    this.failedQueue.push({
                        resolve,
                        reject,
                        config: originalRequest,
                    })
                })
            },
        )

        this.instance = http
        return http
    }

    private handleUnauthorized(): void {
        // Clear tokens
        localStorage.removeItem('auth-storage')
        window.location.href = '/'
        // You might want to redirect to login or dispatch a logout action
        // For example: store.dispatch(logout());
    }

    request<T = unknown, R = AxiosResponse<T>>(config: AxiosRequestConfig): Promise<R> {
        return this.http.request(config)
    }

    get<T = unknown, R = AxiosResponse<T>>(url: string, config?: AxiosRequestConfig): Promise<R> {
        return this.http.get<T, R>(url, config)
    }

    post<T = unknown, R = AxiosResponse<T>>(url: string, data?: T, config?: AxiosRequestConfig): Promise<R> {
        return this.http.post<T, R>(url, data, config)
    }

    put<T = unknown, R = AxiosResponse<T>>(url: string, data?: T, config?: AxiosRequestConfig): Promise<R> {
        return this.http.put<T, R>(url, data, config)
    }

    delete<T = unknown, R = AxiosResponse<T>>(url: string, config?: AxiosRequestConfig): Promise<R> {
        return this.http.delete<T, R>(url, config)
    }

    private handleError(error: AxiosError): Promise<never> {
        if (error instanceof AxiosError) {
            if (!error?.response) {
                return Promise.reject(error)
            }

            const status = error.response?.status

            switch (status) {
                case StatusCodes.InternalServerError: {
                    // Handle InternalServerError
                    break
                }
                case StatusCodes.Forbidden: {
                    // Handle Forbidden
                    break
                }
                case StatusCodes.Unauthorized: {
                    // Already handled in interceptor
                    break
                }
                case StatusCodes.TooManyRequests: {
                    // Handle TooManyRequests
                    break
                }
                case StatusCodes.BadRequest: {
                    // throw { ...error }
                    break
                }
            }
        }
        return Promise.reject(error as AxiosError<APIResponseError>)
    }
}

const http = new Http(import.meta.env.VITE_API_BASE_URL)
export default http
