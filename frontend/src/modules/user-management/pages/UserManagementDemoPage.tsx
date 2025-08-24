import React from 'react';
import { Box, Typography, Paper, Button } from '@mui/material';
import { UserManagementView } from '../views/UserManagementView';

export const UserManagementDemoPage: React.FC = () => {
  return (
    <Box sx={{ p: 3 }}>
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h4" gutterBottom>
          User Management Demo
        </Typography>
        <Typography variant="body1" color="text.secondary" paragraph>
          This page demonstrates the complete user management functionality including:
        </Typography>
        <Box component="ul" sx={{ pl: 2 }}>
          <Typography component="li">User listing with pagination and filtering</Typography>
          <Typography component="li">Create new users with role assignment</Typography>
          <Typography component="li">Edit existing user information</Typography>
          <Typography component="li">Delete users with confirmation</Typography>
          <Typography component="li">Internationalization support (English/Arabic)</Typography>
          <Typography component="li">Responsive Material-UI design</Typography>
        </Box>
      </Paper>

      <UserManagementView />
    </Box>
  );
};
