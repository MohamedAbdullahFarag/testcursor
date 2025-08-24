import React from 'react';
import { useAuth } from '../hooks/useAuth';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Container,
  Typography,
  Button,
  Paper,
  Avatar,
  Chip,
} from '@mui/material';
import { useTranslation } from 'react-i18next';

export const DashboardPage: React.FC = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const { t } = useTranslation('auth');

  const handleLogout = async () => {
    try {
      await logout();
      navigate('/login');
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  if (!user) {
    return (
      <Container>
        <Typography>Loading user data...</Typography>
      </Container>
    );
  }

  return (
    <Container maxWidth="md">
      <Box sx={{ mt: 4 }}>
        <Paper elevation={3} sx={{ p: 4 }}>
          <Box display="flex" alignItems="center" mb={3}>
            <Avatar sx={{ mr: 2, width: 64, height: 64 }}>
              {user.fullName.charAt(0).toUpperCase()}
            </Avatar>
            <Box>
              <Typography variant="h4" gutterBottom>
                Welcome, {user.fullName}!
              </Typography>
              <Typography variant="body1" color="text.secondary">
                {user.email}
              </Typography>
            </Box>
          </Box>

          <Box mb={3}>
            <Typography variant="h6" gutterBottom>
              Your Roles:
            </Typography>
            <Box display="flex" gap={1} flexWrap="wrap">
              {user.roles.map((role) => (
                <Chip key={role} label={role} color="primary" variant="outlined" />
              ))}
            </Box>
          </Box>

          <Box display="flex" gap={2}>
            <Button
              variant="contained"
              color="primary"
              onClick={() => navigate('/profile')}
            >
              {t('profile')}
            </Button>
            <Button
              variant="outlined"
              color="secondary"
              onClick={() => navigate('/settings')}
            >
              {t('settings')}
            </Button>
            <Button
              variant="outlined"
              color="error"
              onClick={handleLogout}
            >
              {t('logout')}
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
};
