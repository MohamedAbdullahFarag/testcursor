/**
 * Test file for useNotifications hook
 * 
 * Tests notification hook functionality including data fetching,
 * mutations, error handling, and optimistic updates.
 */

import { renderHook, waitFor, act } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest';
import React from 'react';

import { useNotifications, useNotificationCount } from '../useNotifications';
import { notificationService } from '../../services/notificationService';
import { 
  NotificationType, 
  NotificationPriority, 
  NotificationStatus,
  NotificationChannel,
  NotificationDeliveryStatus 
} from '../../types/notification.types';

// Mock the notification service
vi.mock('../../services/notificationService');
const mockNotificationService = vi.mocked(notificationService);

// Mock translation hook
vi.mock('react-i18next', () => ({
  useTranslation: () => ({
    t: (key: string, options?: Record<string, unknown>) => `${key}${options ? ` ${JSON.stringify(options)}` : ''}`,
  }),
}));

// Mock toast provider
vi.mock('../../components/Toast.provider', () => ({
  useToast: () => ({
    addToast: vi.fn(),
  }),
}));

// Test wrapper with QueryClient
const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
      mutations: {
        retry: false,
      },
    },
  });

  const TestWrapper = ({ children }: { children: React.ReactNode }) => (
    React.createElement(QueryClientProvider, { client: queryClient }, children)
  );

  return TestWrapper;
};

// Sample notification data
const mockNotification = {
  id: '1',
  title: 'Test Notification',
  message: 'This is a test notification',
  type: NotificationType.Info,
  priority: NotificationPriority.Normal,
  status: NotificationStatus.Sent,
  isRead: false,
  createdAt: new Date('2024-01-01T12:00:00Z'),
  updatedAt: new Date('2024-01-01T12:00:00Z'),
  createdBy: 'user1',
  channels: [NotificationChannel.InApp],
  deliveryStatus: {
    [NotificationChannel.Email]: NotificationDeliveryStatus.Pending,
    [NotificationChannel.Sms]: NotificationDeliveryStatus.Pending,
    [NotificationChannel.Push]: NotificationDeliveryStatus.Pending,
    [NotificationChannel.InApp]: NotificationDeliveryStatus.Sent,
  },
};

const mockPaginatedResponse = {
  data: [mockNotification],
  pagination: {
    currentPage: 1,
    totalPages: 1,
    totalCount: 1,
    pageSize: 10,
    hasNextPage: false,
    hasPreviousPage: false,
  },
  success: true,
};

describe('useNotifications Hook', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Basic functionality', () => {
    it('should fetch notifications successfully', async () => {
      mockNotificationService.getNotifications.mockResolvedValue(mockPaginatedResponse);
      mockNotificationService.getUnreadCount.mockResolvedValue(1);

      const wrapper = createWrapper();
      const { result } = renderHook(() => useNotifications(), { wrapper });

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.notifications).toEqual([mockNotification]);
      expect(result.current.unreadCount).toBe(1);
      expect(result.current.error).toBeNull();
    });

    it('should handle loading state correctly', () => {
      mockNotificationService.getNotifications.mockImplementation(
        () => new Promise(() => {}) // Never resolves
      );
      mockNotificationService.getUnreadCount.mockImplementation(
        () => new Promise(() => {}) // Never resolves
      );

      const wrapper = createWrapper();
      const { result } = renderHook(() => useNotifications(), { wrapper });

      expect(result.current.isLoading).toBe(true);
      expect(result.current.notifications).toEqual([]);
      expect(result.current.unreadCount).toBe(0);
    });

    it('should handle error state correctly', async () => {
      const errorMessage = 'Failed to fetch notifications';
      mockNotificationService.getNotifications.mockRejectedValue(new Error(errorMessage));
      mockNotificationService.getUnreadCount.mockRejectedValue(new Error(errorMessage));

      const wrapper = createWrapper();
      const { result } = renderHook(() => useNotifications(), { wrapper });

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.error).toContain(errorMessage);
      expect(result.current.notifications).toEqual([]);
    });
  });

  describe('Notification actions', () => {
    it('should mark notification as read', async () => {
      mockNotificationService.getNotifications.mockResolvedValue(mockPaginatedResponse);
      mockNotificationService.getUnreadCount.mockResolvedValue(1);
      mockNotificationService.markAsRead.mockResolvedValue(undefined);

      const wrapper = createWrapper();
      const { result } = renderHook(() => useNotifications(), { wrapper });

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      await act(async () => {
        await result.current.markAsRead('1');
      });

      expect(mockNotificationService.markAsRead).toHaveBeenCalledWith('1');
      
      // Check optimistic update
      const updatedNotification = result.current.notifications.find(n => n.id === '1');
      expect(updatedNotification?.isRead).toBe(true);
      expect(updatedNotification?.readAt).toBeInstanceOf(Date);
    });

    it('should delete notification', async () => {
      mockNotificationService.getNotifications.mockResolvedValue(mockPaginatedResponse);
      mockNotificationService.getUnreadCount.mockResolvedValue(1);
      mockNotificationService.deleteNotification.mockResolvedValue(undefined);

      const wrapper = createWrapper();
      const { result } = renderHook(() => useNotifications(), { wrapper });

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      await act(async () => {
        await result.current.deleteNotification('1');
      });

      expect(mockNotificationService.deleteNotification).toHaveBeenCalledWith('1');
      
      // Check optimistic update - notification should be removed
      expect(result.current.notifications).toEqual([]);
    });
  });

  describe('Filtering and options', () => {
    it('should pass filter options to service', async () => {
      const filter = {
        type: NotificationType.Error,
        isRead: false,
      };

      mockNotificationService.getNotifications.mockResolvedValue(mockPaginatedResponse);
      mockNotificationService.getUnreadCount.mockResolvedValue(1);

      const wrapper = createWrapper();
      renderHook(() => useNotifications({ filter }), { wrapper });

      await waitFor(() => {
        expect(mockNotificationService.getNotifications).toHaveBeenCalledWith(
          1, // page
          10, // pageSize
          filter
        );
      });
    });

    it('should respect custom pageSize', async () => {
      mockNotificationService.getNotifications.mockResolvedValue(mockPaginatedResponse);
      mockNotificationService.getUnreadCount.mockResolvedValue(1);

      const wrapper = createWrapper();
      renderHook(() => useNotifications({ pageSize: 20 }), { wrapper });

      await waitFor(() => {
        expect(mockNotificationService.getNotifications).toHaveBeenCalledWith(
          1, // page
          20, // pageSize
          {} // filter
        );
      });
    });
  });
});

describe('useNotificationCount Hook', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should fetch unread count only', async () => {
    mockNotificationService.getUnreadCount.mockResolvedValue(5);

    const wrapper = createWrapper();
    const { result } = renderHook(() => useNotificationCount(), { wrapper });

    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });

    expect(result.current.unreadCount).toBe(5);
    expect(result.current.error).toBeNull();
    expect(mockNotificationService.getUnreadCount).toHaveBeenCalled();
    expect(mockNotificationService.getNotifications).not.toHaveBeenCalled();
  });

  it('should handle error when fetching count', async () => {
    const errorMessage = 'Failed to fetch count';
    mockNotificationService.getUnreadCount.mockRejectedValue(new Error(errorMessage));

    const wrapper = createWrapper();
    const { result } = renderHook(() => useNotificationCount(), { wrapper });

    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });

    expect(result.current.error).toContain(errorMessage);
    expect(result.current.unreadCount).toBe(0);
  });
});
