import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Button,
  Alert,
  AlertDescription,
} from 'mada-design-system';
import { Popup } from '../../../shared/components';
import { UserList } from './UserList';
import { UserForm, Role } from './UserForm';
import { useUserManagement } from '../hooks/useUserManagement';
import { User, CreateUserRequest, UpdateUserRequest } from '../models/user.types';
import { roleService } from '../../role-management/services/roleService';

// ❌ DON'T: Mix UI state with business logic
// ❌ DON'T: Create components that do everything
// ❌ DON'T: Forget to handle loading and error states
// ❌ DON'T: Skip proper TypeScript typing
// ❌ DON'T: Ignore accessibility requirements

interface UserManagementViewProps {
  /** Available roles for user assignment */
  availableRoles?: Role[];
  /** Page title override */
  title?: string;
  /** Show page header */
  showHeader?: boolean;
  /** Additional CSS classes */
  className?: string;
}

/**
 * UserManagementView component - Main orchestrator for user management operations
 * 
 * Features:
 * - User list with search, filtering, and pagination
 * - Create and edit user forms
 * - Modal-based form dialogs
 * - Bulk operations support
 * - Real-time data synchronization
 * - Comprehensive error handling
 * - Loading states management
 * - Internationalization support
 * 
 * @param props - UserManagementViewProps
 */
export const UserManagementView: React.FC<UserManagementViewProps> = ({
  availableRoles = [],
  title,
  showHeader = true,
  className = '',
}) => {
  const { t } = useTranslation('userManagement');
  const userManagement = useUserManagement();
  // Load roles for assignment
  const [rolesList, setRolesList] = useState<Role[]>(availableRoles);
  useEffect(() => {
    const fetchRoles = async () => {
      try {
        const response = await roleService.getRoles({ page: 1, pageSize: 1000 });
        setRolesList(response.items.map(r => ({ id: r.id, name: r.name, description: r.description })));
      } catch (error) {
        console.error('Failed to load roles:', error);
      }
    };
    fetchRoles();
  }, []);

  // UI state
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [userToEdit, setUserToEdit] = useState<User | null>(null);
  const [userToDelete, setUserToDelete] = useState<User | null>(null);
  const [isFormSubmitting, setIsFormSubmitting] = useState(false);


  /**
   * Handles creating a new user
   */
  const handleCreateUser = async (data: CreateUserRequest | UpdateUserRequest) => {
    setIsFormSubmitting(true);
    try {
      if ('password' in data) {
        // It's a CreateUserRequest
        await userManagement.createUser(data as CreateUserRequest);
      }
      setIsCreateModalOpen(false);
      // Success message is handled by the hook
    } catch (error) {
      // Error is handled by the hook and will be passed to the form
      throw error;
    } finally {
      setIsFormSubmitting(false);
    }
  };

  /**
   * Handles updating an existing user
   */
  const handleUpdateUser = async (data: CreateUserRequest | UpdateUserRequest) => {
    setIsFormSubmitting(true);
    try {
      if ('id' in data) {
        // It's an UpdateUserRequest
        await userManagement.updateUser(data as UpdateUserRequest);
      }
      setIsEditModalOpen(false);
      setUserToEdit(null);
      // Success message is handled by the hook
    } catch (error) {
      // Error is handled by the hook and will be passed to the form
      throw error;
    } finally {
      setIsFormSubmitting(false);
    }
  };

  /**
   * Handles deleting a user
   */
  const handleDeleteUser = async () => {
    if (!userToDelete) return;

    setIsFormSubmitting(true);
    try {
      await userManagement.deleteUser(userToDelete.id);
      setIsDeleteModalOpen(false);
      setUserToDelete(null);
      // Success message is handled by the hook
    } catch (error) {
      // Error is handled by the hook
      console.error('Failed to delete user:', error);
    } finally {
      setIsFormSubmitting(false);
    }
  };

  /**
   * Opens the edit modal for a specific user
   */
  const handleEditUser = (user: User) => {
    setUserToEdit(user);
    setIsEditModalOpen(true);
  };

  /**
   * Opens the delete confirmation modal for a specific user
   */
  const handleDeleteUserConfirm = (user: User) => {
    setUserToDelete(user);
    setIsDeleteModalOpen(true);
  };

  /**
   * Closes all modals and resets state
   */
  const handleCloseModals = () => {
    setIsCreateModalOpen(false);
    setIsEditModalOpen(false);
    setIsDeleteModalOpen(false);
    setUserToEdit(null);
    setUserToDelete(null);
  };

  return (
    <div className={`space-y-6 ${className}`}>
      {/* Page Header */}
      {showHeader && (
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold">
              {title || t('view.title')}
            </h1>
            <p className="text-gray-600 mt-1">
              {t('view.description')}
            </p>
          </div>
          
          <Button
            onClick={() => setIsCreateModalOpen(true)}
            disabled={userManagement.loading}
            className="flex items-center gap-2"
          >
            <span>➕</span>
            {t('view.actions.createUser')}
          </Button>
        </div>
      )}

      {/* Global Error Display */}
      {userManagement.error && (
        <Alert variant="default">
          <span>⚠️</span>
          <AlertDescription>
            {userManagement.error}
          </AlertDescription>
        </Alert>
      )}

      {/* User List */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <span>👥</span>
            {t('list.title')}
          </CardTitle>
        </CardHeader>
        <CardContent>
          <UserList
            users={userManagement.users}
            total={userManagement.total}
            page={userManagement.page}
            pageSize={userManagement.pageSize}
            totalPages={userManagement.totalPages}
            loading={userManagement.loading}
            error={userManagement.error}
            filters={userManagement.filters}
            selectedUsers={userManagement.selectedUsers}
            onUserSelect={userManagement.selectUser}
            onSelectAll={(selected) => selected ? userManagement.selectAllUsers() : userManagement.deselectAllUsers()}
            onEdit={handleEditUser}
            onDelete={handleDeleteUserConfirm}
            onBulkDelete={userManagement.bulkDeleteUsers}
            onExport={(format) => userManagement.exportUsers(format as any)}
            onSearch={userManagement.searchUsers}
            onFiltersChange={userManagement.setFilters}
            onPageChange={userManagement.goToPage}
            onPageSizeChange={userManagement.setPageSize}
            onRefresh={userManagement.refreshUsers}
            enableBulkActions={true}
            enableExport={true}
          />
        </CardContent>
      </Card>

      {/* Create User Modal */}
      <Popup 
        isOpen={isCreateModalOpen} 
        onClose={() => setIsCreateModalOpen(false)}
        title={t('form.createTitle')}
        description={t('form.createDescription')}
        size="4xl"
        className="max-h-[90vh] overflow-y-auto"
      >
      <UserForm
          mode="create"
          availableRoles={rolesList}
          isLoading={isFormSubmitting}
          onSubmit={handleCreateUser}
          onCancel={handleCloseModals}
        />
      </Popup>

      {/* Edit User Modal */}
      <Popup 
        isOpen={isEditModalOpen} 
        onClose={() => setIsEditModalOpen(false)}
        title={t('form.editTitle')}
        description={t('form.editDescription', { userName: userToEdit?.fullName })}
        size="4xl"
        className="max-h-[90vh] overflow-y-auto"
      >
        {userToEdit && (
          <UserForm
            mode="edit"
            user={userToEdit}
            availableRoles={rolesList}
            isLoading={isFormSubmitting}
            onSubmit={handleUpdateUser}
            onCancel={handleCloseModals}
          />
        )}
      </Popup>

      {/* Delete Confirmation Modal */}
      <Popup 
        isOpen={isDeleteModalOpen} 
        onClose={() => setIsDeleteModalOpen(false)}
        title={`⚠️ ${t('deleteModal.title')}`}
        description={t('deleteModal.message', { 
          userName: userToDelete?.fullName,
          email: userToDelete?.email 
        })}
        size="lg"
        animation="fade"
      >
        <div className="space-y-4">
          <div className="bg-yellow-50 border border-yellow-200 rounded-md p-4">
            <p className="text-sm text-yellow-800">
              {t('deleteModal.warning')}
            </p>
          </div>

          <div className="flex justify-end gap-3 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={handleCloseModals}
              disabled={isFormSubmitting}
            >
              {t('deleteModal.actions.cancel')}
            </Button>
            <Button
              type="button"
              variant="default"
              onClick={handleDeleteUser}
              disabled={isFormSubmitting}
              className="bg-red-600 hover:bg-red-700"
            >
              {isFormSubmitting && <span className="mr-2">⏳</span>}
              {t('deleteModal.actions.delete')}
            </Button>
          </div>
        </div>
      </Popup>
    </div>
  );
};

export default UserManagementView;
