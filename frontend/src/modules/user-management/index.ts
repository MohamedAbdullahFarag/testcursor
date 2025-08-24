/**
 * User Management Module - Index
 * Exports all components, hooks, services, and types for the user management feature
 * Following folder-per-feature architecture with clean separation of concerns
 */

// Components
export { UserList } from './components/UserList';
export { UserForm } from './components/UserForm';
export { UserManagementView } from './views/UserManagementView';

// Hooks
export { useUserManagement } from './hooks/useUserManagement';

// Services
export { userService } from './services/userService';

// Types
export type {
  User,
  CreateUserRequest,
  UpdateUserRequest,
  UserListResponse
} from './models/user.types';

// Locales
export { default as userManagementEn } from './locales/en';
export { default as userManagementAr } from './locales/ar';

// Constants
export const USER_MANAGEMENT_MODULE = 'user-management';
export const DEFAULT_PAGE_SIZE = 25;
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
