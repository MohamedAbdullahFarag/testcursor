// LoginForm Component - Authentication form component
// Following Single Responsibility Principle - only login form UI and validation
// NOTE: Pre-filled with test credentials in development mode only for easier testing

import React, { useState, memo } from 'react';
import { useAuth } from '../hooks/useAuth';
import { LoginRequest } from '../models/auth.types';
import { useTranslation } from 'react-i18next';
import {
  Box,
  TextField,
  Button,
  Typography,
  Alert,
  Paper,
  Container,
  CircularProgress,
} from '@mui/material';
import { useNavigate } from 'react-router-dom';

export const LoginForm: React.FC = memo(() => {
  const { t } = useTranslation('auth');
  const { login, isLoading, error } = useAuth();
  const navigate = useNavigate();
  
  const [formData, setFormData] = useState<LoginRequest>({
    email: '',
    password: '',
  });
  
  const [validationErrors, setValidationErrors] = useState<Partial<LoginRequest>>({});

  const validateForm = (): boolean => {
    const errors: Partial<LoginRequest> = {};
    
    if (!formData.email) {
      errors.email = t('emailRequired');
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      errors.email = t('emailInvalid');
    }
    
    if (!formData.password) {
      errors.password = t('passwordRequired');
    } else if (formData.password.length < 6) {
      errors.password = t('passwordMinLength');
    }
    
    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleInputChange = (field: keyof LoginRequest) => (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const value = event.target.value;
    setFormData(prev => ({ ...prev, [field]: value }));
    
    // Clear validation error when user starts typing
    if (validationErrors[field]) {
      setValidationErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    
    if (!validateForm()) {
      return;
    }
    
    try {
      await login(formData);
      // Redirect to dashboard or intended page after successful login
      navigate('/dashboard');
    } catch (err) {
      // Error is handled by the auth context
      console.error('Login failed:', err);
    }
  };

  return (
    <Container maxWidth="sm">
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        <Paper
          elevation={3}
          sx={{
            padding: 4,
            width: '100%',
            maxWidth: 400,
          }}
        >
          <Typography component="h1" variant="h4" align="center" gutterBottom>
            {t('login')}
          </Typography>
          
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}
          
          <Box component="form" onSubmit={handleSubmit} sx={{ mt: 1 }}>
            <TextField
              margin="normal"
              required
              fullWidth
              id="email"
              label={t('email')}
              name="email"
              autoComplete="email"
              autoFocus
              value={formData.email}
              onChange={handleInputChange('email')}
              error={!!validationErrors.email}
              helperText={validationErrors.email}
              disabled={isLoading}
            />
            
            <TextField
              margin="normal"
              required
              fullWidth
              name="password"
              label={t('password')}
              type="password"
              id="password"
              autoComplete="current-password"
              value={formData.password}
              onChange={handleInputChange('password')}
              error={!!validationErrors.password}
              helperText={validationErrors.password}
              disabled={isLoading}
            />
            
            <Button
              type="submit"
              fullWidth
              variant="contained"
              sx={{ mt: 3, mb: 2 }}
              disabled={isLoading}
            >
              {isLoading ? (
                <CircularProgress size={24} color="inherit" />
              ) : (
                t('login')
              )}
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
});

LoginForm.displayName = 'LoginForm';
