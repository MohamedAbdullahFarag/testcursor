import React, { useState } from 'react';
import {
  Box,
  Button,
  Typography,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Alert,
  Container,
  Paper
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { UserList } from '../components/UserList';
import { UserForm } from '../components/UserForm';
import { useUserManagement } from '../hooks/useUserManagement';
import { User, CreateUserRequest, UpdateUserRequest } from '../models/user.types';

export const UserManagementView: React.FC = () => {
  const {
    users,
    total,
    loading,
    error,
    create,
    update,
    remove
  } = useUserManagement();

  const [showForm, setShowForm] = useState(false);
  const [editingUser, setEditingUser] = useState<User | undefined>();
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null);

  const handleCreateUser = async (data: CreateUserRequest) => {
    try {
      await create(data);
      setShowForm(false);
    } catch (error) {
      console.error('Failed to create user:', error);
    }
  };

  const handleUpdateUser = async (data: UpdateUserRequest) => {
    try {
      await update(data);
      setShowForm(false);
      setEditingUser(undefined);
    } catch (error) {
      console.error('Failed to update user:', error);
    }
  };

  const handleDeleteUser = async (id: string) => {
    try {
      await remove(id);
      setDeleteConfirm(null);
    } catch (error) {
      console.error('Failed to delete user:', error);
    }
  };

  const handleEditUser = (user: User) => {
    setEditingUser(user);
    setShowForm(true);
  };

  const handleCloseForm = () => {
    setShowForm(false);
    setEditingUser(undefined);
  };

  const handleOpenCreateForm = () => {
    setEditingUser(undefined);
    setShowForm(true);
  };

  return (
    <Container maxWidth="xl">
      <Box sx={{ py: 3 }}>
        {/* Header */}
        <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
          <Typography variant="h4" component="h1">
            User Management
          </Typography>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={handleOpenCreateForm}
          >
            Create User
          </Button>
        </Box>

        {/* Error Display */}
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {/* User List */}
        <Paper sx={{ p: 2 }}>
          <UserList
            users={users}
            loading={loading}
            onEdit={handleEditUser}
            onDelete={(id) => setDeleteConfirm(id)}
          />
        </Paper>

        {/* Create/Edit User Form Dialog */}
        <Dialog
          open={showForm}
          onClose={handleCloseForm}
          maxWidth="md"
          fullWidth
        >
          <DialogTitle>
            {editingUser ? 'Edit User' : 'Create New User'}
          </DialogTitle>
          <DialogContent>
            <UserForm
              user={editingUser}
              onSubmit={editingUser ? handleUpdateUser : handleCreateUser}
              onCancel={handleCloseForm}
              loading={loading}
            />
          </DialogContent>
        </Dialog>

        {/* Delete Confirmation Dialog */}
        <Dialog
          open={!!deleteConfirm}
          onClose={() => setDeleteConfirm(null)}
        >
          <DialogTitle>Confirm Delete</DialogTitle>
          <DialogContent>
            <Typography>
              Are you sure you want to delete this user? This action cannot be undone.
            </Typography>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setDeleteConfirm(null)}>
              Cancel
            </Button>
            <Button
              onClick={() => deleteConfirm && handleDeleteUser(deleteConfirm)}
              color="error"
              variant="contained"
            >
              Delete
            </Button>
          </DialogActions>
        </Dialog>
      </Box>
    </Container>
  );
};
