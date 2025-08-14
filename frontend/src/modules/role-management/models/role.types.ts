/**
 * Role Management Types
 * TypeScript interfaces for role-related data structures
 * Aligned with backend RoleDto.cs and UserRoleDto.cs
 */

/**
 * Role entity interface
 */
export interface Role {
  /** Role identifier */
  id: number;
  
  /** Unique role code used for system identification */
  code: string;
  
  /** User-friendly role name */
  name: string;
  
  /** Optional role description */
  description?: string;
  
  /** Whether this is a system-defined role that cannot be modified/deleted */
  isSystemRole: boolean;
  
  /** Whether the role is active */
  isActive: boolean;
  
  /** Associated permissions */
  permissions: string[];
  
  /** Creation timestamp */
  createdAt: string;
  
  /** Last modification timestamp */
  updatedAt: string;
}

/**
 * Request payload for role creation
 */
export interface CreateRoleRequest {
  /** Unique role code (must be unique in the system) */
  code: string;
  
  /** User-friendly role name */
  name: string;
  
  /** Optional role description */
  description?: string;
  
  /** Initial permission codes to assign */
  permissions?: string[];
  
  /** Whether the role is active upon creation */
  isActive?: boolean;
}

/**
 * Request payload for role updates
 */
export interface UpdateRoleRequest {
  /** Updated role name */
  name: string;
  
  /** Updated role description */
  description?: string;
  
  /** Updated permission codes */
  permissions?: string[];
  
  /** Updated active status */
  isActive?: boolean;
}

/**
 * Role list response with pagination
 */
export interface RoleListResponse {
  /** Array of roles */
  items: Role[];
  
  /** Total number of roles */
  totalCount: number;
  
  /** Current page */
  page: number;
  
  /** Page size */
  pageSize: number;
}

/**
 * Role pagination parameters
 */
export interface RolePaginationParams {
  /** Page number (1-based) */
  page: number;
  
  /** Items per page */
  pageSize: number;
  
  /** Optional filters */
  filters?: RoleFilterOptions;
}

/**
 * Role filtering options
 */
export interface RoleFilterOptions {
  /** Search term for name, code, or description */
  searchTerm?: string;
  
  /** Filter by system role status */
  isSystemRole?: boolean;
  
  /** Filter by active status */
  isActive?: boolean;
  
  /** Filter by permission code */
  hasPermission?: string;
}

/**
 * User role assignment request
 */
export interface AssignRoleRequest {
  /** User ID to assign role to */
  userId: number;
  
  /** Role ID to assign */
  roleId: number;
}

/**
 * Batch user role assignment request
 */
export interface UpdateUserRolesRequest {
  /** User ID to update roles for */
  userId: number;
  
  /** List of role IDs to assign */
  roleIds: number[];
}

/**
 * User role assignment response
 */
export interface UserRoleSummary {
  /** User ID */
  userId: number;
  
  /** Username */
  username: string;
  
  /** User's full name */
  fullName: string;
  
  /** Assigned roles */
  roles: Role[];
}

/**
 * Role users response with pagination
 */
export interface RoleUsersResponse {
  /** Array of user summaries */
  items: UserRoleSummary[];
  
  /** Total number of users with this role */
  totalCount: number;
  
  /** Current page */
  page: number;
  
  /** Page size */
  pageSize: number;
}

/**
 * Bulk action result for role operations
 */
export interface BulkRoleActionResult {
  /** Number of successful operations */
  successCount: number;
  
  /** Number of failed operations */
  failureCount: number;
  
  /** Error messages for failed operations */
  errors?: string[];
}

/**
 * Permission entity for role management
 */
export interface Permission {
  /** Permission code */
  code: string;
  
  /** User-friendly permission name */
  name: string;
  
  /** Permission category for grouping */
  category: string;
  
  /** Optional description */
  description?: string;
}

/**
 * Role permission matrix item
 */
export interface RolePermissionMatrix {
  /** Role ID */
  roleId: number;
  
  /** Role name */
  roleName: string;
  
  /** Map of permission code to boolean indicating if granted */
  permissions: Record<string, boolean>;
}
