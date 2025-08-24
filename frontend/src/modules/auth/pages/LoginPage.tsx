import React from 'react';
import { LoginForm } from '../components/LoginForm';
import { Box, Container, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';

export const LoginPage: React.FC = () => {
  const { t } = useTranslation('auth');

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
        <Typography component="h1" variant="h3" gutterBottom>
          {t('login')}
        </Typography>
        <Typography variant="body1" color="text.secondary" align="center" sx={{ mb: 4 }}>
          Welcome to Ikhtibar - Sign in to your account
        </Typography>
        
        <LoginForm />
      </Box>
    </Container>
  );
};
