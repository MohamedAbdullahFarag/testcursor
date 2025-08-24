// Authentication Types and Models
// Following Single Responsibility Principle - only type definitions here

import type { ApiError } from '@/shared/models/api'

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

export interface User {
  id: string;
  fullName: string;
  email: string;
  roles: string[];
}

export interface AuthContextType {
  user: User | null;
  accessToken: string | null;
  login: (credentials: LoginRequest) => Promise<void>;
  logout: () => Promise<void>;
  isLoading: boolean;
  error: string | null;
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
