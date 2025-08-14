/**
 * User Types for the Ikhtibar Project
 * 
 * This file demonstrates PRP (Product Requirements Prompt) methodology:
 * - Context is King: Clear, well-documented type definitions with comments
 * - Validation Loops: Types that enforce data constraints
 * - Information Dense: Complete type coverage for the entire feature
 * - Progressive Success: Basic types first, then more complex derived types
 * - One-Pass Implementation: All types needed for the feature in one file
 */

/**
 * User entity representing a user in the system
 */
export interface User {
  id: string;
  name: string;
  email: string;
  phoneNumber?: string;
  avatarUrl?: string;
  role: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  preferredLanguage: 'ar' | 'en';
  country?: string;
  permissions?: string[];
  lastLoginDate?: string;
}

/**
 * User role types available in the system
 */
export type UserRole = 'admin' | 'manager' | 'editor' | 'user' | 'guest';

/**
 * User status types
 */
export type UserStatus = 'active' | 'inactive' | 'suspended' | 'pending';

/**
 * User filter options for searching and filtering users
 */
export interface UserFilterOptions {
  status?: string;
  role?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  startDate?: string;
  endDate?: string;
  country?: string;
  preferredLanguage?: 'ar' | 'en';
}

/**
 * Request for getting users with pagination and filtering
 */
export interface GetUsersRequest {
  page: number;
  pageSize: number;
  searchTerm?: string;
  status?: UserStatus;
  role?: UserRole;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  startDate?: string;
  endDate?: string;
}

/**
 * Request for creating a new user
 */
export interface CreateUserRequest {
  name: string;
  email: string;
  phoneNumber?: string;
  role: UserRole;
  avatarUrl?: string;
  password: string;
  confirmPassword: string;
  isActive?: boolean;
  preferredLanguage?: 'ar' | 'en';
  country?: string;
}

/**
 * Request for updating an existing user
 */
export interface UpdateUserRequest {
  name?: string;
  email?: string;
  phoneNumber?: string;
  role?: UserRole;
  avatarUrl?: string;
  isActive?: boolean;
  preferredLanguage?: 'ar' | 'en';
  country?: string;
}

/**
 * Request for changing user password
 */
export interface ChangePasswordRequest {
  userId: string;
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

/**
 * Format options for exporting users
 */
export type ExportFormat = 'csv' | 'xlsx' | 'pdf';

/**
 * Response for bulk user operations
 */
export interface BulkActionResult {
  successCount: number;
  failedCount: number;
  failedIds?: string[];
}

/**
 * User profile preferences
 */
export interface UserPreferences {
  theme: 'light' | 'dark' | 'system';
  notifications: {
    email: boolean;
    sms: boolean;
    push: boolean;
  };
  accessibility: {
    highContrast: boolean;
    largeText: boolean;
    screenReader: boolean;
  };
  display: {
    language: 'ar' | 'en';
    timeFormat: '12h' | '24h';
    dateFormat: 'MM/DD/YYYY' | 'DD/MM/YYYY' | 'YYYY-MM-DD';
  };
}
