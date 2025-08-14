/**
 * Role Management Module
 * Exports all components, hooks, and models for the role management module
 */

// Components
export { RoleList } from './components/RoleList';
export { RoleForm } from './components/RoleForm';
export { RoleManagementView } from './components/RoleManagementView';
export { RoleManagementPage } from './components/RoleManagementPage';
export { RolePermissionMatrix } from './components/RolePermissionMatrix';
export { UserRoleAssignmentComponent } from './components/UserRoleAssignmentComponent';

// Hooks
export { useRoleManagement } from './hooks/useRoleManagement';

// Services
export { roleService } from './services/roleService';

// Models
export * from './models/role.types';
