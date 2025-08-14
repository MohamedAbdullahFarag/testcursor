// Authentication Types and Models
// Following Single Responsibility Principle - only type definitions here

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResult {
  accessToken: string;
  refreshToken: string;
  user: User;
}

export interface User {
  id: number;
  fullName: string;
  email: string;
  roles: string[];
  defaultRole?: number;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
}

export interface AuthState {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

export interface LoginParams {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
}

// API Error Types
export interface ApiError {
  message: string;
  statusCode: number;
  details?: string;
}

// Form validation types
export interface LoginFormData {
  email: string;
  password: string;
}

export interface LoginFormErrors {
  email?: string;
  password?: string;
  general?: string;
}

// Auth Context Types
export interface AuthContextType {
  user: User | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  login: (credentials: LoginRequest) => Promise<void>;
  logout: () => Promise<void>;
  refreshToken: () => Promise<void>;
  clearError: () => void;
}

// Route Protection Types
export interface PrivateRouteProps {
  children: React.ReactNode;
  requiredRoles?: string[];
  fallbackPath?: string;
}

// Auth Service Response Types
export interface AuthServiceResponse<T = unknown> {
  success: boolean;
  data?: T;
  error?: ApiError;
}
