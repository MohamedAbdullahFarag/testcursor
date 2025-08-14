/**
 * User Management Module - Index
 * Exports all components, hooks, services, and types for the user management feature
 * Following folder-per-feature architecture with clean separation of concerns
 */

// Core Components
export { UserList } from './components/UserList';
export { UserForm } from './components/UserForm';
export { UserManagementView } from './components/UserManagementView';

// Hooks
export { useUserManagement } from './hooks/useUserManagement';

// Services
export { userService } from './services/userService';

// Types
export type {
  User,
  CreateUserDto,
  UpdateUserDto,
  UserFilters,
  UserExportFormat,
  BulkUserActionResult,
  UserFormErrors,
  UserActivityLog,
  UserStatusUpdateRequest,
  UserRoleAssignmentRequest,
} from './models/user.types';

// Component Types
export type { UserListProps } from './components/UserList';
export type { Role } from './components/UserForm';

// Hook Types
export type { 
  UseUserManagementOptions, 
  UseUserManagementReturn 
} from './hooks/useUserManagement';

// Constants
export const USER_MANAGEMENT_MODULE = 'user-management';
export const DEFAULT_PAGE_SIZE = 20;
export const SEARCH_DEBOUNCE_MS = 300;

/**
 * Module configuration object
 */
export const userManagementConfig = {
  module: USER_MANAGEMENT_MODULE,
  defaultPageSize: DEFAULT_PAGE_SIZE,
  searchDebounceMs: SEARCH_DEBOUNCE_MS,
  supportedExportFormats: ['csv', 'excel', 'pdf'] as const,
  supportedLanguages: ['en', 'ar'] as const,
} as const;
