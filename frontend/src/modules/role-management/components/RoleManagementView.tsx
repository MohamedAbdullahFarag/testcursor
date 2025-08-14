import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Button,
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  Alert,
  AlertDescription,
} from 'mada-design-system';
import { RoleList } from './RoleList';
import { RoleForm } from './RoleForm';
import { RolePermissionMatrix } from './RolePermissionMatrix';
import { useRoleManagement } from '../hooks/useRoleManagement';
import type { Role, CreateRoleRequest, UpdateRoleRequest, Permission } from '../models/role.types';

interface RoleManagementViewProps {
  /** Page title override */
  title?: string;
  /** Show page header */
  showHeader?: boolean;
  /** Additional CSS classes */
  className?: string;
  /** Available permissions */
  availablePermissions?: Permission[];
}

/**
 * RoleManagementView component - Main orchestrator for role management operations
 * 
 * Features:
 * - Role list with search, filtering, and pagination
 * - Create and edit role forms
 * - Modal-based form dialogs
 * - Permission matrix management
 * - System role protection
 * - Real-time data synchronization
 * - Comprehensive error handling
 * - Loading states management
 * - Internationalization support
 * 
 * @param props - RoleManagementViewProps
 */
export const RoleManagementView: React.FC<RoleManagementViewProps> = ({
  title,
  showHeader = true,
  className = '',
  availablePermissions = [],
}) => {
  const { t } = useTranslation('roleManagement');
  const roleManagement = useRoleManagement({
    autoLoad: true,
    initialPageSize: 10,
  });

  // UI state
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [isPermissionModalOpen, setIsPermissionModalOpen] = useState(false);
  const [roleToEdit, setRoleToEdit] = useState<Role | null>(null);
  const [roleToDelete, setRoleToDelete] = useState<Role | null>(null);
  const [roleForPermissions, setRoleForPermissions] = useState<Role | null>(null);
  const [isFormSubmitting, setIsFormSubmitting] = useState(false);
  const [bulkActionLoading, setBulkActionLoading] = useState(false);
  const [localError, setLocalError] = useState<string | null>(null);

  // Derived state
  const hasSelectedRoles = roleManagement.selectedRoles.length > 0;
  const isAllSelected = roleManagement.roles.length > 0 && 
    roleManagement.selectedRoles.length === roleManagement.roles.length;
  
  // Modal handlers
  const openCreateModal = () => {
    setIsCreateModalOpen(true);
  };

  const closeCreateModal = () => {
    setIsCreateModalOpen(false);
  };

  const openEditModal = (role: Role) => {
    setRoleToEdit(role);
    setIsEditModalOpen(true);
  };

  const closeEditModal = () => {
    setRoleToEdit(null);
    setIsEditModalOpen(false);
  };

  const openDeleteModal = (role: Role) => {
    setRoleToDelete(role);
    setIsDeleteModalOpen(true);
  };

  const closeDeleteModal = () => {
    setRoleToDelete(null);
    setIsDeleteModalOpen(false);
  };

  const openPermissionModal = (role: Role) => {
    setRoleForPermissions(role);
    setIsPermissionModalOpen(true);
  };

  const closePermissionModal = () => {
    setRoleForPermissions(null);
    setIsPermissionModalOpen(false);
  };

  // Form handlers
  const handleCreateRole = async (data: CreateRoleRequest) => {
    setIsFormSubmitting(true);
    setLocalError(null);

    try {
      await roleManagement.createRole(data);
      closeCreateModal();
    } catch (err) {
      setLocalError(err instanceof Error ? err.message : 'Failed to create role');
    } finally {
      setIsFormSubmitting(false);
    }
  };

  const handleUpdateRole = async (data: UpdateRoleRequest) => {
    if (!roleToEdit) return;
    
    setIsFormSubmitting(true);
    setLocalError(null);

    try {
      await roleManagement.updateRole(roleToEdit.id, data);
      closeEditModal();
    } catch (err) {
      setLocalError(err instanceof Error ? err.message : 'Failed to update role');
    } finally {
      setIsFormSubmitting(false);
    }
  };

  const handleDeleteRole = async () => {
    if (!roleToDelete) return;
    
    setIsFormSubmitting(true);
    setLocalError(null);

    try {
      await roleManagement.deleteRole(roleToDelete.id);
      closeDeleteModal();
    } catch (err) {
      setLocalError(err instanceof Error ? err.message : 'Failed to delete role');
    } finally {
      setIsFormSubmitting(false);
    }
  };

  const handleUpdatePermissions = async (roleId: number, permissions: string[]) => {
    setIsFormSubmitting(true);
    setLocalError(null);

    try {
      await roleManagement.updateRolePermissions(roleId, permissions);
      closePermissionModal();
    } catch (err) {
      setLocalError(err instanceof Error ? err.message : 'Failed to update permissions');
    } finally {
      setIsFormSubmitting(false);
    }
  };

  // Bulk actions
  const handleBulkDelete = async () => {
    if (!hasSelectedRoles || !window.confirm(t('confirmBulkDelete'))) {
      return;
    }

    setBulkActionLoading(true);
    setLocalError(null);

    try {
      // Process bulk delete sequentially
      for (const roleId of roleManagement.selectedRoles) {
        await roleManagement.deleteRole(roleId);
      }
      
      // Clear selection after successful operation
      roleManagement.deselectAll();
    } catch (err) {
      setLocalError(err instanceof Error ? err.message : 'Failed to delete selected roles');
    } finally {
      setBulkActionLoading(false);
    }
  };

  return (
    <div className={`role-management-container ${className}`}>
      {/* Header */}
      {showHeader && (
        <div className="mb-6">
          <h1 className="text-2xl font-bold">{title || t('roleManagement')}</h1>
          <p className="text-gray-500">{t('roleManagementDescription')}</p>
        </div>
      )}

      {/* Error display */}
      {(roleManagement.error || localError) && (
        <Alert className="mb-4" variant="destructive">
          <AlertDescription>
            {roleManagement.error || localError}
          </AlertDescription>
        </Alert>
      )}

      {/* Action Bar */}
      <div className="flex flex-wrap justify-between items-center gap-2 mb-4">
        <div className="flex flex-wrap items-center gap-2">
          <Button 
            onClick={openCreateModal}
            disabled={roleManagement.loading}
            aria-label={t('createRole')}
          >
            {t('createRole')}
          </Button>
          
          {hasSelectedRoles && (
            <Button 
              variant="outline" 
              onClick={handleBulkDelete}
              disabled={bulkActionLoading}
              aria-label={t('deleteSelected')}
            >
              {t('deleteSelected')} ({roleManagement.selectedRoles.length})
            </Button>
          )}
        </div>
        
        <div className="flex items-center gap-2">
          <Button 
            variant="outline"
            onClick={() => roleManagement.refresh()}
            disabled={roleManagement.loading}
            aria-label={t('refresh')}
          >
            {t('refresh')}
          </Button>
        </div>
      </div>

      {/* Role List */}
      <Card>
        <CardHeader>
          <CardTitle>{t('roles')}</CardTitle>
        </CardHeader>
        <CardContent>
          <RoleList
            roles={roleManagement.roles}
            loading={roleManagement.loading}
            pagination={{
              page: roleManagement.page,
              pageSize: roleManagement.pageSize,
              totalPages: roleManagement.totalPages,
              totalItems: roleManagement.total,
              hasNextPage: roleManagement.hasNextPage,
              hasPreviousPage: roleManagement.hasPreviousPage,
              onPageChange: roleManagement.goToPage,
              onPageSizeChange: roleManagement.setPageSize,
            }}
            selection={{
              selectedIds: roleManagement.selectedRoles,
              onSelect: roleManagement.selectRole,
              onDeselect: roleManagement.deselectRole,
              onSelectAll: roleManagement.selectAll,
              onDeselectAll: roleManagement.deselectAll,
              isAllSelected,
            }}
            filters={{
              searchTerm: roleManagement.searchTerm,
              onSearchChange: roleManagement.setSearchTerm,
              onFilterChange: roleManagement.setFilters,
              onFilterClear: roleManagement.clearFilters,
            }}
            onEdit={openEditModal}
            onDelete={openDeleteModal}
            onManagePermissions={openPermissionModal}
          />
        </CardContent>
      </Card>

      {/* Create Role Modal */}
      <Dialog open={isCreateModalOpen} onOpenChange={setIsCreateModalOpen}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>{t('createRole')}</DialogTitle>
            <DialogDescription>{t('createRoleDescription')}</DialogDescription>
          </DialogHeader>
          
          <RoleForm
            mode="create"
            isLoading={isFormSubmitting}
            availablePermissions={availablePermissions}
            onSubmit={handleCreateRole}
            onCancel={closeCreateModal}
          />
        </DialogContent>
      </Dialog>

      {/* Edit Role Modal */}
      <Dialog open={isEditModalOpen} onOpenChange={setIsEditModalOpen}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>{t('editRole')}</DialogTitle>
            <DialogDescription>{t('editRoleDescription')}</DialogDescription>
          </DialogHeader>
          
          {roleToEdit && (
            <RoleForm
              mode="edit"
              role={roleToEdit}
              isLoading={isFormSubmitting}
              availablePermissions={availablePermissions}
              onSubmit={handleUpdateRole}
              onCancel={closeEditModal}
            />
          )}
        </DialogContent>
      </Dialog>

      {/* Delete Role Modal */}
      <Dialog open={isDeleteModalOpen} onOpenChange={setIsDeleteModalOpen}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>{t('deleteRole')}</DialogTitle>
            <DialogDescription>
              {roleToDelete?.isSystemRole 
                ? t('cannotDeleteSystemRole') 
                : t('deleteRoleConfirmation', { name: roleToDelete?.name })}
            </DialogDescription>
          </DialogHeader>
          
          <DialogFooter>
            <Button
              variant="outline"
              onClick={closeDeleteModal}
              disabled={isFormSubmitting}
              aria-label={t('cancel')}
            >
              {t('cancel')}
            </Button>
            
            <Button
              variant="destructive"
              onClick={handleDeleteRole}
              disabled={isFormSubmitting || (roleToDelete?.isSystemRole ?? false)}
              aria-label={t('delete')}
            >
              {isFormSubmitting ? t('deleting') : t('delete')}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Role Permissions Modal */}
      <Dialog open={isPermissionModalOpen} onOpenChange={setIsPermissionModalOpen}>
        <DialogContent className="sm:max-w-[700px]">
          <DialogHeader>
            <DialogTitle>{t('managePermissions')}</DialogTitle>
            <DialogDescription>
              {t('managePermissionsDescription', { name: roleForPermissions?.name })}
            </DialogDescription>
          </DialogHeader>
          
          {roleForPermissions && (
            <RolePermissionMatrix
              roleId={roleForPermissions.id}
              roleName={roleForPermissions.name}
              isSystemRole={roleForPermissions.isSystemRole}
              availablePermissions={availablePermissions}
              isLoading={isFormSubmitting}
              onSave={handleUpdatePermissions}
              onCancel={closePermissionModal}
            />
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
};
