import React, { useState, useEffect } from 'react';
import {
  TextField,
  Button,
  Box,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  FormControlLabel,
  Checkbox,
  Typography,
  Paper,
  Grid,
  Chip
} from '@mui/material';
import { CreateUserRequest, UpdateUserRequest, User } from '../models/user.types';

interface UserFormProps {
  user?: User;
  onSubmit: (data: CreateUserRequest | UpdateUserRequest) => void;
  onCancel: () => void;
  loading?: boolean;
}

export const UserForm: React.FC<UserFormProps> = ({
  user,
  onSubmit,
  onCancel,
  loading = false
}) => {
  const [formData, setFormData] = useState<CreateUserRequest | UpdateUserRequest>({
    fullName: '',
    email: '',
    password: '',
    roles: [],
    ...(user && { id: user.id, isActive: user.isActive })
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  // Available roles (this would typically come from a service)
  const availableRoles = ['admin', 'user', 'teacher', 'student', 'reviewer'];

  useEffect(() => {
    if (user) {
      setFormData({
        id: user.id,
        fullName: user.fullName,
        email: user.email,
        password: '', // Don't populate password for editing
        roles: user.roles,
        isActive: user.isActive
      });
    }
  }, [user]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.fullName.trim()) {
      newErrors.fullName = 'Full name is required';
    }

    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Email is invalid';
    }

    if (!user && !formData.password) {
      newErrors.password = 'Password is required for new users';
    }

    if (formData.password && formData.password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters';
    }

    if (formData.roles.length === 0) {
      newErrors.roles = 'At least one role is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validateForm()) {
      onSubmit(formData);
    }
  };

  const handleInputChange = (field: string, value: any) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }));
    }
  };

  const handleRoleToggle = (role: string) => {
    const newRoles = formData.roles.includes(role)
      ? formData.roles.filter(r => r !== role)
      : [...formData.roles, role];
    handleInputChange('roles', newRoles);
  };

  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h6" gutterBottom>
        {user ? 'Edit User' : 'Create User'}
      </Typography>

      <Box component="form" onSubmit={handleSubmit} sx={{ mt: 2 }}>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Full Name"
              value={formData.fullName}
              onChange={(e) => handleInputChange('fullName', e.target.value)}
              error={!!errors.fullName}
              helperText={errors.fullName}
              required
            />
          </Grid>

          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Email"
              type="email"
              value={formData.email}
              onChange={(e) => handleInputChange('email', e.target.value)}
              error={!!errors.email}
              helperText={errors.email}
              required
            />
          </Grid>

          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Password"
              type="password"
              value={formData.password}
              onChange={(e) => handleInputChange('password', e.target.value)}
              error={!!errors.password}
              helperText={errors.password || (user ? 'Leave blank to keep current password' : '')}
              required={!user}
            />
          </Grid>

          <Grid item xs={12} sm={6}>
            <FormControl fullWidth>
              <InputLabel>Status</InputLabel>
              <Select
                value={formData.isActive ?? true}
                onChange={(e) => handleInputChange('isActive', e.target.value)}
                label="Status"
              >
                <MenuItem value={true}>Active</MenuItem>
                <MenuItem value={false}>Inactive</MenuItem>
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={12}>
            <Typography variant="subtitle2" gutterBottom>
              Roles *
            </Typography>
            <Box display="flex" gap={1} flexWrap="wrap">
              {availableRoles.map((role) => (
                <Chip
                  key={role}
                  label={role}
                  onClick={() => handleRoleToggle(role)}
                  color={formData.roles.includes(role) ? 'primary' : 'default'}
                  variant={formData.roles.includes(role) ? 'filled' : 'outlined'}
                  clickable
                />
              ))}
            </Box>
            {errors.roles && (
              <Typography color="error" variant="caption">
                {errors.roles}
              </Typography>
            )}
          </Grid>
        </Grid>

        <Box display="flex" gap={2} sx={{ mt: 3 }}>
          <Button
            type="submit"
            variant="contained"
            disabled={loading}
          >
            {loading ? 'Saving...' : (user ? 'Update User' : 'Create User')}
          </Button>
          <Button
            type="button"
            variant="outlined"
            onClick={onCancel}
            disabled={loading}
          >
            Cancel
          </Button>
        </Box>
      </Box>
    </Paper>
  );
};
