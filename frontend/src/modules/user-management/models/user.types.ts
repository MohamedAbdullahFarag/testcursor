/**
 * User Management Types
 * TypeScript interfaces for user-related data structures
 * Re-exports from core entities for module organization
 */

// Re-export core types for this module
export type {
  UserDto,
  CreateUserDto,
  UpdateUserDto,
  UserFilterOptions as UserFilters,
  PagedResult,
  ApiResponse,
} from '../../../shared/types/core-entities';

// Use the proper UserDto from core entities as the main User type
export type User = UserDto;

export interface CreateUserRequest {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  password: string;
  phoneNumber?: string;
  preferredLanguage?: string;
  roleIds?: number[];
}

export interface UpdateUserRequest {
  id: number;
  username?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
  preferredLanguage?: string;
  isActive?: boolean;
  roleIds?: number[];
}

export interface UserListResponse {
  items: User[];
  total: number;
  page: number;
  pageSize: number;
}

/**
 * User form validation errors
 */
export interface UserFormErrors {
  username?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  password?: string;
  phoneNumber?: string;
  roles?: string;
  general?: string;
}

/**
 * Bulk action result for user operations
 */
export interface BulkUserActionResult {
  /** Number of users processed successfully */
  successCount: number;
  
  /** Number of users that failed to process */
  failureCount: number;
  
  /** Error messages for failed operations */
  errors: string[];
  
  /** Success message */
  message: string;
}

/**
 * User export format options
 */
export type UserExportFormat = 'csv' | 'excel' | 'pdf';

/**
 * User activity log entry
 */
export interface UserActivityLog {
  /** Log entry identifier */
  id: number;
  
  /** User identifier */
  userId: number;
  
  /** Activity type */
  activityType: string;
  
  /** Activity description */
  description: string;
  
  /** IP address */
  ipAddress?: string;
  
  /** User agent */
  userAgent?: string;
  
  /** Timestamp */
  createdAt: string;
}

/**
 * User status update request
 */
export interface UserStatusUpdateRequest {
  isActive: boolean;
  reason?: string;
}

/**
 * User role assignment request
 */
export interface UserRoleAssignmentRequest {
  roleIds: number[];
  reason?: string;
}
