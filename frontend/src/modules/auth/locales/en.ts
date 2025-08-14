export const authEn = {
    auth: {
        // Login form
        loginTitle: 'Welcome to the Unified Portal Management System',
        loginDescription: 'Manage services easily and efficiently. Login to continue',
        login: 'Login',
        logout: 'Logout',
        
        // Form fields
        email: 'Email Address',
        password: 'Password',
        emailPlaceholder: 'Enter your email address',
        passwordPlaceholder: 'Enter your password',
        
        // Loading states
        loggingIn: 'Signing in...',
        loggingOut: 'Signing out...',
        
        // Error messages
        loginError: 'Login Failed',
        invalidCredentials: 'Invalid email or password',
        networkError: 'Network error occurred. Please try again.',
        sessionExpired: 'Your session has expired. Please login again.',
        tokenRefreshFailed: 'Session refresh failed. Please login again.',
        
        // Validation messages
        emailRequired: 'Email is required',
        emailInvalid: 'Please enter a valid email address',
        passwordRequired: 'Password is required',
        passwordMinLength: 'Password must be at least 6 characters',
        
        // Success messages
        loginSuccess: 'Welcome back!',
        logoutSuccess: 'You have been logged out successfully',
        
        // Role-based access
        accessDenied: 'Access Denied',
        insufficientPermissions: 'You do not have sufficient permissions to access this resource',
        
        // Misc
        rememberMe: 'Remember me',
        forgotPassword: 'Forgot your password?',
        noAccount: "Don't have an account?",
        signUp: 'Sign up',
        backToLogin: 'Back to login',
    },
} as const
