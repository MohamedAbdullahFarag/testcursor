// useAuth Hook Tests
// Testing authentication hook functionality

import { renderHook, act } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest';
import { useAuth } from '../hooks/useAuth';
import { useAuthStore } from '../store/authStore';
import { authService } from '../services/authService';

// Mock the auth service
vi.mock('../services/authService');

// Mock the auth store
vi.mock('../store/authStore');

const mockAuthStore = {
  user: null,
  accessToken: null,
  isAuthenticated: false,
  isLoading: false,
  error: null,
  login: vi.fn(),
  clearAuth: vi.fn(),
  refreshAccessToken: vi.fn(),
  setLoading: vi.fn(),
  setError: vi.fn(),
};

describe('useAuth', () => {
  beforeEach(() => {
    vi.mocked(useAuthStore).mockReturnValue(mockAuthStore);
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.resetAllMocks();
  });

  it('returns auth state and functions', () => {
    const { result } = renderHook(() => useAuth());

    expect(result.current).toEqual({
      user: null,
      accessToken: null,
      isAuthenticated: false,
      isLoading: false,
      error: null,
      login: expect.any(Function),
      logout: expect.any(Function),
      refreshToken: expect.any(Function),
      clearError: expect.any(Function),
    });
  });

  it('handles successful login', async () => {
    const mockUser = {
      id: '1',
      fullName: 'Test User',
      email: 'test@example.com',
      roles: ['user'],
    };

    const mockAuthResult = {
      success: true,
      data: {
        user: mockUser,
        accessToken: 'access-token',
        refreshToken: 'refresh-token',
      },
    };

    vi.mocked(authService.login).mockResolvedValueOnce(mockAuthResult);

    const { result } = renderHook(() => useAuth());

    await act(async () => {
      await result.current.login({
        email: 'test@example.com',
        password: 'password123',
      });
    });

    expect(authService.login).toHaveBeenCalledWith({
      email: 'test@example.com',
      password: 'password123',
    });

    expect(mockAuthStore.login).toHaveBeenCalledWith({
      user: mockUser,
      accessToken: 'access-token',
      refreshToken: 'refresh-token',
    });

    expect(mockAuthStore.setLoading).toHaveBeenCalledWith(true);
    expect(mockAuthStore.setLoading).toHaveBeenCalledWith(false);
    expect(mockAuthStore.setError).toHaveBeenCalledWith(null);
  });

  it('handles login failure', async () => {
    const mockError = {
      success: false,
      error: {
        message: 'Invalid credentials',
        statusCode: 401,
      },
    };

    vi.mocked(authService.login).mockResolvedValueOnce(mockError);

    const { result } = renderHook(() => useAuth());

    await expect(
      act(async () => {
        await result.current.login({
          email: 'test@example.com',
          password: 'wrongpassword',
        });
      })
    ).rejects.toThrow('Invalid credentials');

    expect(mockAuthStore.setError).toHaveBeenCalledWith('Invalid credentials');
    expect(mockAuthStore.setLoading).toHaveBeenCalledWith(false);
    expect(mockAuthStore.login).not.toHaveBeenCalled();
  });

  it('handles successful logout', async () => {
    vi.mocked(authService.logout).mockResolvedValueOnce({
      success: true,
    });

    const { result } = renderHook(() => useAuth());

    await act(async () => {
      await result.current.logout();
    });

    expect(authService.logout).toHaveBeenCalled();
    expect(mockAuthStore.clearAuth).toHaveBeenCalled();
    expect(mockAuthStore.setLoading).toHaveBeenCalledWith(true);
    expect(mockAuthStore.setLoading).toHaveBeenCalledWith(false);
  });

  it('handles logout failure gracefully', async () => {
    vi.mocked(authService.logout).mockRejectedValueOnce(new Error('Network error'));

    const { result } = renderHook(() => useAuth());

    await act(async () => {
      await result.current.logout();
    });

    // Should still clear auth even if API call fails
    expect(mockAuthStore.clearAuth).toHaveBeenCalled();
    expect(mockAuthStore.setLoading).toHaveBeenCalledWith(false);
  });

  it('handles successful token refresh', async () => {
    mockAuthStore.refreshAccessToken.mockResolvedValueOnce(true);

    const { result } = renderHook(() => useAuth());

    await act(async () => {
      await result.current.refreshToken();
    });

    expect(mockAuthStore.refreshAccessToken).toHaveBeenCalled();
  });

  it('handles token refresh failure', async () => {
    mockAuthStore.refreshAccessToken.mockResolvedValueOnce(false);

    const { result } = renderHook(() => useAuth());

    await expect(
      act(async () => {
        await result.current.refreshToken();
      })
    ).rejects.toThrow('Token refresh failed');

    expect(mockAuthStore.refreshAccessToken).toHaveBeenCalled();
  });

  it('clears error when clearError is called', async () => {
    const { result } = renderHook(() => useAuth());

    act(() => {
      result.current.clearError();
    });

    expect(mockAuthStore.setError).toHaveBeenCalledWith(null);
  });

  it('handles network errors during login', async () => {
    vi.mocked(authService.login).mockRejectedValueOnce(new Error('Network error'));

    const { result } = renderHook(() => useAuth());

    await expect(
      act(async () => {
        await result.current.login({
          email: 'test@example.com',
          password: 'password123',
        });
      })
    ).rejects.toThrow('Network error');

    expect(mockAuthStore.setError).toHaveBeenCalledWith('Network error');
    expect(mockAuthStore.setLoading).toHaveBeenCalledWith(false);
  });
});
