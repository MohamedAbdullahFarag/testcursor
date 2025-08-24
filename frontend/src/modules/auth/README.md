# Authentication Module

This module provides a complete authentication system for the Ikhtibar frontend application.

## Features

- **Login Form**: Material-UI based login form with validation
- **Authentication Context**: React context for managing auth state
- **Protected Routes**: Route guarding with PrivateRoute component
- **Token Management**: Secure token storage and refresh
- **Internationalization**: English and Arabic support
- **Error Handling**: Comprehensive error handling and user feedback

## Components

### LoginForm
A responsive login form with email and password fields, validation, and error display.

### PrivateRoute
A route wrapper that protects routes requiring authentication. Redirects unauthenticated users to the login page.

### DashboardPage
A sample dashboard page that demonstrates authenticated user experience.

### AuthDemoPage
A demo page showcasing the authentication system functionality.

## Hooks

### useAuth
Custom hook providing authentication state and functions:

```typescript
const { user, accessToken, login, logout, isLoading, error } = useAuth();
```

## Services

### authService
API service for authentication operations:

- `login(credentials)`: Authenticate user
- `refresh()`: Refresh access token
- `logout()`: Logout user
- `validateToken(token)`: Validate token validity

## Usage

### 1. Wrap your app with AuthProvider

```tsx
import { AuthProvider } from './modules/auth/hooks/useAuth';

function App() {
  return (
    <AuthProvider>
      {/* Your app components */}
    </AuthProvider>
  );
}
```

### 2. Use authentication in components

```tsx
import { useAuth } from './modules/auth/hooks/useAuth';

function MyComponent() {
  const { user, login, logout } = useAuth();
  
  if (!user) {
    return <LoginForm />;
  }
  
  return (
    <div>
      <h1>Welcome, {user.fullName}!</h1>
      <button onClick={logout}>Logout</button>
    </div>
  );
}
```

### 3. Protect routes

```tsx
import { PrivateRoute } from './shared/components/PrivateRoute';

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginForm />} />
      <Route 
        path="/dashboard" 
        element={
          <PrivateRoute>
            <DashboardPage />
          </PrivateRoute>
        } 
      />
    </Routes>
  );
}
```

## Configuration

### Environment Variables

Set the following environment variables:

```env
VITE_API_BASE_URL=https://localhost:7001
```

### i18n Support

The authentication system supports both English and Arabic languages. Translation files are located in:

- `locales/en.ts` - English translations
- `locales/ar.ts` - Arabic translations

## Testing

Run the authentication tests:

```bash
npm run test src/modules/auth
```

## Security Features

- Secure token storage (localStorage for development, httpOnly cookies for production)
- Automatic token validation
- Protected route access
- Error handling for expired/invalid tokens

## Backend Integration

The authentication system integrates with the Ikhtibar backend API:

- **Login**: `POST /api/auth/login`
- **Refresh**: `POST /api/auth/refresh`
- **Logout**: `POST /api/auth/logout`
- **Validate**: `GET /api/auth/validate`

## Development Notes

- The system currently uses localStorage for token storage (suitable for development)
- For production, consider implementing httpOnly cookies for enhanced security
- Token refresh logic can be enhanced with automatic background refresh
- Error handling can be extended with retry mechanisms and user notifications
