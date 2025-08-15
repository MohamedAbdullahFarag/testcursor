// Authentication Types and Models
// Following Single Responsibility Principle - only type definitions here

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResult {
  accessToken: string;
  refreshToken: string;
  user: {
    id: string;
    fullName: string;
    email: string;
    roles: string[];
  };
}

export interface AuthState {
  user: AuthResult['user'] | null;
  accessToken: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface AuthContextType {
  user: AuthResult['user'] | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  login: (credentials: LoginCredentials) => Promise<void>;
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
