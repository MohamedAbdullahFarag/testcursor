/**
 * useNotifications Hook
 * 
 * Custom React hook for managing user notifications with real-time updates,
 * pagination, filtering, and state management using React Query.
 */

import { useCallback, useState, useEffect } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { useToast } from '../components/Toast.provider';

import { notificationService } from '../services/notificationService';
import {
  UseNotificationsResult,
  Notification,
  NotificationFilter
} from '../types/notification.types';

// Query keys for react-query
const QUERY_KEYS = {
  notifications: ['notifications'] as const,
  unreadCount: ['notifications', 'unread-count'] as const,
  preferences: ['notifications', 'preferences'] as const,
};

interface UseNotificationsOptions {
  userId?: string;
  pageSize?: number;
  filter?: NotificationFilter;
  enableRealTime?: boolean;
  refetchInterval?: number;
}

/**
 * Hook for managing user notifications
 */
export const useNotifications = (options: UseNotificationsOptions = {}): UseNotificationsResult => {
  const {
    userId,
    pageSize = 10,
    filter = {},
    enableRealTime = true,
    refetchInterval = 30000, // 30 seconds
  } = options;

  const { t } = useTranslation();
  const { addToast } = useToast();
  const queryClient = useQueryClient();

  // Local state for pagination
  const [currentPage, setCurrentPage] = useState(1);
  const [allNotifications, setAllNotifications] = useState<Notification[]>([]);

  // Query for notifications with pagination
  const {
    data: notificationsData,
    isLoading: isLoadingNotifications,
    error: notificationsError,
    refetch: refetchNotifications,
  } = useQuery({
    queryKey: [...QUERY_KEYS.notifications, currentPage, pageSize, filter, userId],
    queryFn: () => notificationService.getNotifications(currentPage, pageSize, filter),
    staleTime: enableRealTime ? 0 : 5 * 60 * 1000, // 5 minutes if not real-time
    refetchInterval: enableRealTime ? refetchInterval : false,
    retry: 3,
    retryDelay: (attemptIndex) => Math.min(1000 * 2 ** attemptIndex, 30000),
  });

  // Query for unread count
  const {
    data: unreadCountData,
    isLoading: isLoadingUnreadCount,
    error: unreadCountError,
    refetch: refetchUnreadCount,
  } = useQuery({
    queryKey: QUERY_KEYS.unreadCount,
    queryFn: () => notificationService.getUnreadCount(),
    staleTime: enableRealTime ? 0 : 2 * 60 * 1000, // 2 minutes if not real-time
    refetchInterval: enableRealTime ? refetchInterval : false,
    retry: 3,
  });

  // Update all notifications when new data arrives
  useEffect(() => {
    if (notificationsData?.data) {
      if (currentPage === 1) {
        // Reset for first page
        setAllNotifications(notificationsData.data);
      } else {
        // Append for subsequent pages
        setAllNotifications(prev => {
          const newNotifications = notificationsData.data.filter(
            newNotification => !prev.some(existing => existing.id === newNotification.id)
          );
          return [...prev, ...newNotifications];
        });
      }
    }
  }, [notificationsData, currentPage]);

  // Mark notification as read mutation
  const markAsReadMutation = useMutation({
    mutationFn: (id: string) => notificationService.markAsRead(id),
    onSuccess: (_, id) => {
      // Update local state optimistically
      setAllNotifications(prev =>
        prev.map(notification =>
          notification.id === id
            ? { ...notification, isRead: true, readAt: new Date() }
            : notification
        )
      );

      // Invalidate queries to refresh data
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.notifications });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.unreadCount });

      addToast({
        type: 'success',
        message: t('notifications.marked_as_read')
      });
    },
    onError: (error) => {
      console.error('Error marking notification as read:', error);
      addToast({
        type: 'error',
        message: t('notifications.mark_read_error')
      });
    },
  });

  // Mark all notifications as read mutation
  const markAllAsReadMutation = useMutation({
    mutationFn: () => notificationService.markAllAsRead(),
    onSuccess: () => {
      // Update local state optimistically
      setAllNotifications(prev =>
        prev.map(notification => ({
          ...notification,
          isRead: true,
          readAt: new Date()
        }))
      );

      // Invalidate queries to refresh data
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.notifications });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.unreadCount });

      addToast({
        type: 'success',
        message: t('notifications.all_marked_as_read')
      });
    },
    onError: (error) => {
      console.error('Error marking all notifications as read:', error);
      addToast({
        type: 'error',
        message: t('notifications.mark_all_read_error')
      });
    },
  });

  // Delete notification mutation
  const deleteNotificationMutation = useMutation({
    mutationFn: (id: string) => notificationService.deleteNotification(id),
    onSuccess: (_, id) => {
      // Update local state optimistically
      setAllNotifications(prev => prev.filter(notification => notification.id !== id));

      // Invalidate queries to refresh data
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.notifications });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.unreadCount });

      addToast({
        type: 'success',
        message: t('notifications.deleted')
      });
    },
    onError: (error) => {
      console.error('Error deleting notification:', error);
      addToast({
        type: 'error',
        message: t('notifications.delete_error')
      });
    },
  });

  // Memoized callbacks
  const markAsRead = useCallback(async (id: string) => {
    return markAsReadMutation.mutateAsync(id);
  }, [markAsReadMutation]);

  const markAllAsRead = useCallback(async () => {
    return markAllAsReadMutation.mutateAsync();
  }, [markAllAsReadMutation]);

  const deleteNotification = useCallback(async (id: string) => {
    return deleteNotificationMutation.mutateAsync(id);
  }, [deleteNotificationMutation]);

  const loadMore = useCallback(async () => {
    if (notificationsData?.pagination?.hasNextPage) {
      setCurrentPage(prev => prev + 1);
    }
  }, [notificationsData?.pagination?.hasNextPage]);

  const refetch = useCallback(async () => {
    setCurrentPage(1); // Reset to first page
    setAllNotifications([]); // Clear current notifications
    await Promise.all([
      refetchNotifications(),
      refetchUnreadCount(),
    ]);
  }, [refetchNotifications, refetchUnreadCount]);

  // Cleanup effect
  useEffect(() => {
    return () => {
      setAllNotifications([]);
      setCurrentPage(1);
    };
  }, []);

  // Combine loading states and errors
  const isLoading = isLoadingNotifications || isLoadingUnreadCount;
  const error = notificationsError || unreadCountError;

  return {
    notifications: allNotifications,
    unreadCount: unreadCountData || 0,
    isLoading,
    error: error ? String(error) : null,
    hasNextPage: notificationsData?.pagination?.hasNextPage || false,
    refetch,
    markAsRead,
    markAllAsRead,
    deleteNotification,
    loadMore,
  };
};

/**
 * Hook for getting notification count only (lighter weight)
 */
export const useNotificationCount = () => {
  const { data: unreadCount = 0, isLoading, error } = useQuery({
    queryKey: QUERY_KEYS.unreadCount,
    queryFn: () => notificationService.getUnreadCount(),
    staleTime: 2 * 60 * 1000, // 2 minutes
    refetchInterval: 30000, // 30 seconds
    retry: 3,
  });

  return {
    unreadCount,
    isLoading,
    error: error ? String(error) : null,
  };
};
