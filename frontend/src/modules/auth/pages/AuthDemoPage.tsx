import React, { useState } from 'react';
import { Box, Button, Typography, Container, Paper } from '@mui/material';
import { LoginForm } from '../components/LoginForm';
import { DashboardPage } from './DashboardPage';
import { useAuth } from '../hooks/useAuth';

export const AuthDemoPage: React.FC = () => {
  const { user, accessToken, logout } = useAuth();
  const [showLogin, setShowLogin] = useState(false);

  const handleLogout = async () => {
    try {
      await logout();
      setShowLogin(false);
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  if (user && accessToken) {
    return (
      <Container maxWidth="lg">
        <Box sx={{ mt: 4 }}>
          <Paper elevation={3} sx={{ p: 4, mb: 4 }}>
            <Typography variant="h4" gutterBottom>
              Authentication Demo
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              You are currently logged in. Here's your dashboard:
            </Typography>
            <Button
              variant="outlined"
              color="error"
              onClick={handleLogout}
              sx={{ mb: 3 }}
            >
              Logout
            </Button>
          </Paper>
          
          <DashboardPage />
        </Box>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg">
      <Box sx={{ mt: 4 }}>
        <Paper elevation={3} sx={{ p: 4, mb: 4 }}>
          <Typography variant="h4" gutterBottom>
            Authentication Demo
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
            This page demonstrates the authentication system. Click the button below to show the login form.
          </Typography>
          
          {!showLogin ? (
            <Button
              variant="contained"
              color="primary"
              onClick={() => setShowLogin(true)}
            >
              Show Login Form
            </Button>
          ) : (
            <Button
              variant="outlined"
              onClick={() => setShowLogin(false)}
              sx={{ mb: 3 }}
            >
              Hide Login Form
            </Button>
          )}
        </Paper>

        {showLogin && (
          <Paper elevation={3} sx={{ p: 4 }}>
            <Typography variant="h5" gutterBottom>
              Login Form
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Try logging in with test credentials (note: backend needs to be running)
            </Typography>
            <LoginForm />
          </Paper>
        )}
      </Box>
    </Container>
  );
};
