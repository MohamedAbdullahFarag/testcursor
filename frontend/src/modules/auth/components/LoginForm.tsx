// LoginForm Component - Authentication form component
// Following Single Responsibility Principle - only login form UI and validation
// NOTE: Pre-filled with test credentials in development mode only for easier testing

import React, { useState, useCallback, memo } from 'react';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../hooks/useAuth';
import type { LoginFormData, LoginFormErrors } from '../models/auth.types';

// Form validation helper
const validateForm = (data: LoginFormData): LoginFormErrors => {
  const errors: LoginFormErrors = {};

  if (!data.email.trim()) {
    errors.email = 'email is required';
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(data.email)) {
    errors.email = 'Please enter a valid email address';
  }

  if (!data.password.trim()) {
    errors.password = 'password is required';
  } else if (data.password.length < 6) {
    errors.password = 'Password must be at least 6 characters';
  }

  return errors;
};

interface LoginFormProps {
  onSuccess?: () => void;
  className?: string;
}

export const LoginForm: React.FC<LoginFormProps> = memo(({ onSuccess, className = '' }) => {
  const { t } = useTranslation();
  const { login, isLoading, error } = useAuth();
  
  const [formData, setFormData] = useState<LoginFormData>({
    email: import.meta.env.DEV ? 'sysadmin@example.com' : '', // Default test user for easier development
    password: import.meta.env.DEV ? 'P@ssw0rdHash!' : '',   // Default test password for easier development
  });
  
  const [formErrors, setFormErrors] = useState<LoginFormErrors>({});
  const [touched, setTouched] = useState<Record<keyof LoginFormData, boolean>>({
    email: false,
    password: false,
  });

  /**
   * Handle input field changes
   */
  const handleChange = useCallback((field: keyof LoginFormData) => (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const value = event.target.value;
    setFormData(prev => ({ ...prev, [field]: value }));
    
    // Clear field error when user starts typing
    if (formErrors[field]) {
      setFormErrors(prev => ({ ...prev, [field]: undefined }));
    }
  }, [formErrors]);

  /**
   * Handle input field blur
   */
  const handleBlur = useCallback((field: keyof LoginFormData) => () => {
    setTouched(prev => ({ ...prev, [field]: true }));
    
    // Validate field on blur
    const fieldErrors = validateForm(formData);
    if (fieldErrors[field]) {
      setFormErrors(prev => ({ ...prev, [field]: fieldErrors[field] }));
    }
  }, [formData]);

  /**
   * Handle form submission
   */
  const handleSubmit = useCallback(async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    
    // Mark all fields as touched and validate
    const newTouched = { email: true, password: true };
    const errors = validateForm(formData);
    
    // Update both states together
    setTouched(newTouched);
    setFormErrors(errors);
    
    // Use a small delay to ensure state updates have processed
    await new Promise(resolve => setTimeout(resolve, 0));
    
    // Stop if validation errors
    if (Object.keys(errors).length > 0) {
      return;
    }

    try {
      await login({
        email: formData.email.trim(),
        password: formData.password,
      });
      
      // Clear form on success
      setFormData({ email: '', password: '' });
      setFormErrors({});
      setTouched({ email: false, password: false });
      
      // Call success callback
      onSuccess?.();
    } catch (error) {
      // Error is handled by the auth hook and displayed via error state
      console.error('Login failed:', error);
    }
  }, [formData, login, onSuccess]);

  return (
    <form onSubmit={handleSubmit} className={`space-y-6 ${className}`.trim()}>
      <div className="space-y-4">
        <h2 className="text-2xl font-bold text-gray-900 text-center">
          {t('auth.loginTitle')}
        </h2>
        <p className="text-gray-600 text-center">
          {t('auth.loginDescription')}
        </p>
      </div>

      {/* Global error message */}
      {error && (
        <div className="bg-red-50 border border-red-200 rounded-md p-4">
          <div className="flex">
            <div className="ml-3">
              <h3 className="text-sm font-medium text-red-800">
                {t('auth.loginError')}
              </h3>
              <div className="mt-2 text-sm text-red-700">
                {error}
              </div>
            </div>
          </div>
        </div>
      )}

      <div className="space-y-4">
        {/* Email field */}
        <div>
          <label htmlFor="email" className="block text-sm font-medium text-gray-700">
            {t('auth.email')}
          </label>
          <div className="mt-1">
            <input
              id="email"
              name="email"
              type="email"
              autoComplete="email"
              required
              value={formData.email}
              onChange={handleChange('email')}
              onBlur={handleBlur('email')}
              className={`
                appearance-none block w-full px-3 py-2 border rounded-md shadow-sm 
                placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm
                ${touched.email && formErrors.email ? 'border-red-300' : 'border-gray-300'}
              `.trim()}
              placeholder={t('auth.emailPlaceholder')}
              disabled={isLoading}
            />
            {touched.email && formErrors.email && (
              <p className="mt-2 text-sm text-red-600">{formErrors.email}</p>
            )}
          </div>
        </div>

        {/* Password field */}
        <div>
          <label htmlFor="password" className="block text-sm font-medium text-gray-700">
            {t('auth.password')}
          </label>
          <div className="mt-1">
            <input
              id="password"
              name="password"
              type="password"
              autoComplete="current-password"
              required
              value={formData.password}
              onChange={handleChange('password')}
              onBlur={handleBlur('password')}
              className={`
                appearance-none block w-full px-3 py-2 border rounded-md shadow-sm 
                placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm
                ${touched.password && formErrors.password ? 'border-red-300' : 'border-gray-300'}
              `.trim()}
              placeholder={t('auth.passwordPlaceholder')}
              disabled={isLoading}
            />
            {touched.password && formErrors.password && (
              <p className="mt-2 text-sm text-red-600">{formErrors.password}</p>
            )}
          </div>
        </div>
      </div>

      {/* Submit button */}
      <div>
        <button
          type="submit"
          disabled={isLoading}
          className={`
            group relative w-full flex justify-center py-2 px-4 border border-transparent 
            text-sm font-medium rounded-md text-white focus:outline-none focus:ring-2 
            focus:ring-offset-2 focus:ring-blue-500
            ${isLoading 
              ? 'bg-gray-400 cursor-not-allowed' 
              : 'bg-blue-600 hover:bg-blue-700'
            }
          `.trim()}
        >
          {isLoading ? (
            <>
              <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              {t('auth.loggingIn')}
            </>
          ) : (
            t('auth.login')
          )}
        </button>
      </div>
    </form>
  );
});

LoginForm.displayName = 'LoginForm';

// ❌ DON'T: Handle routing in form component
// ❌ DON'T: Store auth state in component state
// ❌ DON'T: Mix business logic with UI logic
// ❌ DON'T: Skip form validation
// ❌ DON'T: Forget to handle loading states
