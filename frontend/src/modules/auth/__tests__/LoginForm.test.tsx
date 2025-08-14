// LoginForm Component Tests
// Testing login form functionality and validation

import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { I18nextProvider } from 'react-i18next';
import i18n from '@/shared/i18n';
import { LoginForm } from '../components/LoginForm';
import { useAuth } from '../hooks/useAuth';

// Mock the useAuth hook
vi.mock('../hooks/useAuth');

const mockLogin = vi.fn();
const mockAuthState = {
  user: null,
  accessToken: null,
  isAuthenticated: false,
  isLoading: false,
  error: null,
  login: mockLogin,
  logout: vi.fn(),
  refreshToken: vi.fn(),
  clearError: vi.fn(),
};

const renderLoginForm = (props = {}) => {
  return render(
    <I18nextProvider i18n={i18n}>
      <LoginForm {...props} />
    </I18nextProvider>
  );
};

describe('LoginForm', () => {
  beforeEach(() => {
    vi.mocked(useAuth).mockReturnValue(mockAuthState);
    mockLogin.mockClear();
  });

  it('renders login form elements', () => {
    renderLoginForm();

    expect(screen.getByRole('heading', { name: /welcome to the unified portal/i })).toBeInTheDocument();
    expect(screen.getByLabelText(/email address/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /login/i })).toBeInTheDocument();
  });

  it('validates email field on blur', async () => {
    const user = userEvent.setup();
    renderLoginForm();

    const emailInput = screen.getByLabelText(/email address/i);
    
    await user.click(emailInput);
    await user.tab(); // Blur the field

    expect(screen.getByText(/email is required/i)).toBeInTheDocument();
  });

  it('validates email format', async () => {
    const user = userEvent.setup();
    renderLoginForm();

    const emailInput = screen.getByLabelText(/email address/i);
    
    await user.type(emailInput, 'invalid-email');
    await user.tab();

    expect(screen.getByText(/please enter a valid email address/i)).toBeInTheDocument();
  });

  it('validates password field on blur', async () => {
    const user = userEvent.setup();
    renderLoginForm();

    const passwordInput = screen.getByLabelText(/password/i);
    
    await user.click(passwordInput);
    await user.tab();

    expect(screen.getByText(/password is required/i)).toBeInTheDocument();
  });

  it('validates password minimum length', async () => {
    const user = userEvent.setup();
    renderLoginForm();

    const passwordInput = screen.getByLabelText(/password/i);
    
    await user.type(passwordInput, '123');
    await user.tab();

    expect(screen.getByText(/password must be at least 6 characters/i)).toBeInTheDocument();
  });

  it('submits form with valid data', async () => {
    const user = userEvent.setup();
    const onSuccess = vi.fn();
    mockLogin.mockResolvedValueOnce(undefined);

    renderLoginForm({ onSuccess });

    const emailInput = screen.getByLabelText(/email address/i);
    const passwordInput = screen.getByLabelText(/password/i);
    const submitButton = screen.getByRole('button', { name: /login/i });

    await user.type(emailInput, 'test@example.com');
    await user.type(passwordInput, 'password123');
    await user.click(submitButton);

    await waitFor(() => {
      expect(mockLogin).toHaveBeenCalledWith({
        email: 'test@example.com',
        password: 'password123',
      });
    });

    expect(onSuccess).toHaveBeenCalled();
  });

  it('prevents submission with invalid data', async () => {
    const user = userEvent.setup();
    renderLoginForm();

    const submitButton = screen.getByRole('button', { name: /login/i });
    await user.click(submitButton);

    expect(mockLogin).not.toHaveBeenCalled();
    
    // Wait for validation errors to appear
    await waitFor(() => {
      expect(screen.getByText(/email is required/i)).toBeInTheDocument();
    });
    
    await waitFor(() => {
      expect(screen.getByText(/password is required/i)).toBeInTheDocument();
    });
  });

  it('displays loading state during submission', () => {
    vi.mocked(useAuth).mockReturnValue({
      ...mockAuthState,
      isLoading: true,
    });

    renderLoginForm();

    expect(screen.getByText(/signing in/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /signing in/i })).toBeDisabled();
  });

  it('displays error message when login fails', () => {
    vi.mocked(useAuth).mockReturnValue({
      ...mockAuthState,
      error: 'Invalid credentials',
    });

    renderLoginForm();

    expect(screen.getByText(/login failed/i)).toBeInTheDocument();
    expect(screen.getByText(/invalid credentials/i)).toBeInTheDocument();
  });

  it('clears field errors when user starts typing', async () => {
    const user = userEvent.setup();
    renderLoginForm();

    const emailInput = screen.getByLabelText(/email address/i);
    
    // Trigger validation error
    await user.click(emailInput);
    await user.tab();
    expect(screen.getByText(/email is required/i)).toBeInTheDocument();

    // Start typing to clear error
    await user.type(emailInput, 'test');
    expect(screen.queryByText(/email is required/i)).not.toBeInTheDocument();
  });

  it('disables form elements during loading', () => {
    vi.mocked(useAuth).mockReturnValue({
      ...mockAuthState,
      isLoading: true,
    });

    renderLoginForm();

    expect(screen.getByLabelText(/email address/i)).toBeDisabled();
    expect(screen.getByLabelText(/password/i)).toBeDisabled();
    expect(screen.getByRole('button')).toBeDisabled();
  });
});
