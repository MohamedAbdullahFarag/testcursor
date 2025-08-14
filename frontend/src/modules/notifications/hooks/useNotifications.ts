/**
 * Core notification hooks for the notification module
 * Provides React Query-based state management for all notification operations
 * Following established patterns from auth and shared modules
 */

import { useQuery, useMutation, useQueryClient, UseQueryResult } from '@tanstack/react-query';
import { useState, useCallback, useEffect } from 'react';
import { useTranslation } from 'react-i18next';

import { notificationService } from '../services/notificationService';
import {
  NotificationDto,
  CreateNotificationDto,
  UpdateNotificationDto,
  NotificationFilterDto,
  NotificationPagedResponse,
  NotificationUnreadCountResponse,
  BulkNotificationResponse,
  NotificationSendResponse,
  NotificationPreferenceDto,
  UpdateNotificationPreferenceDto,
  NotificationTemplateDto,
  UseNotificationsReturn,
  UseNotificationPreferencesReturn,
  UseNotificationTemplatesReturn
} from '../types/notification.types';
import { useAuth } from '@/modules/auth/hooks/useAuth';

// Custom notification system (placeholder for toast notifications)
const useNotificationSystem = () => ({
  success: (message: string) => console.log('Success:', message),
  error: (message: string) => console.error('Error:', message),
});

// ==================== QUERY KEYS ====================

export const notificationQueryKeys = {
  all: ['notifications'] as const,
  lists: () => [...notificationQueryKeys.all, 'list'] as const,
  list: (filters: NotificationFilterDto) => [...notificationQueryKeys.lists(), filters] as const,
  details: () => [...notificationQueryKeys.all, 'detail'] as const,
  detail: (id: string) => [...notificationQueryKeys.details(), id] as const,
  unreadCount: (userId: string) => [...notificationQueryKeys.all, 'unreadCount', userId] as const,
  preferences: (userId: string) => [...notificationQueryKeys.all, 'preferences', userId] as const,
  templates: () => [...notificationQueryKeys.all, 'templates'] as const,
  history: (notificationId: string) => [...notificationQueryKeys.all, 'history', notificationId] as const,
};

// Helper function to convert string user ID to number
const parseUserId = (userId: string | undefined): number | undefined => {
  if (!userId) return undefined;
  const parsed = parseInt(userId, 10);
  return isNaN(parsed) ? undefined : parsed;
};

// ==================== MAIN NOTIFICATIONS HOOK ====================

/**
 * Main hook for managing user notifications with React Query
 * Provides paginated notifications, filters, and actions
 */
export const useNotifications = (
  userId?: number,
  initialFilter: NotificationFilterDto = {}
): UseNotificationsReturn => {
  const { t } = useTranslation('notifications');
  const queryClient = useQueryClient();
  const { user } = useAuth();
  const notificationSystem = useNotificationSystem();
  
  // Use provided userId or parse from authenticated user
  const effectiveUserId = userId || parseUserId(user?.id);
  
  // Local state for filters and pagination
  const [filter, setFilter] = useState<NotificationFilterDto>({
    page: 1,
    pageSize: 20,
    unreadOnly: false,
    ...initialFilter
  });

  // Query for notifications
  const {
    data: notificationData,
    isLoading,
    error,
    refetch,
    isFetching
  } = useQuery({
    queryKey: notificationQueryKeys.list(filter),
    queryFn: () => effectiveUserId 
      ? notificationService.getUserNotifications(effectiveUserId, filter)
      : Promise.resolve({ data: [], total: 0, page: 1, pageSize: 20 } as NotificationPagedResponse),
    enabled: !!effectiveUserId,
    staleTime: 30 * 1000, // 30 seconds
    refetchInterval: 60 * 1000, // Refetch every minute for real-time updates
  });

  // Query for unread count
  const { data: unreadCount = 0 } = useQuery({
    queryKey: notificationQueryKeys.unreadCount(user?.id || ''),
    queryFn: () => effectiveUserId 
      ? notificationService.getUnreadCount(effectiveUserId)
      : Promise.resolve(0),
    enabled: !!effectiveUserId,
    staleTime: 30 * 1000,
    refetchInterval: 30 * 1000, // More frequent updates for unread count
  });

  // Mutations for notification actions
  const markAsReadMutation = useMutation({
    mutationFn: (notificationId: string) => 
      effectiveUserId ? notificationService.markAsRead(notificationId, effectiveUserId) : Promise.resolve(false),
    onSuccess: () => {
      // Invalidate relevant queries
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.lists() });
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.unreadCount(user?.id || '') });
      notificationSystem.success(t('actions.markReadSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('actions.markReadError'));
    },
  });

  const markAllAsReadMutation = useMutation({
    mutationFn: () => 
      effectiveUserId ? notificationService.markAllAsRead(effectiveUserId) : Promise.resolve(false),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.lists() });
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.unreadCount(user?.id || '') });
      notificationSystem.success(t('actions.markAllReadSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('actions.markAllReadError'));
    },
  });

  const deleteNotificationMutation = useMutation({
    mutationFn: (notificationId: string) => notificationService.deleteNotification(notificationId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.lists() });
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.unreadCount(user?.id || '') });
      notificationSystem.success(t('actions.deleteSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('actions.deleteError'));
    },
  });

  // Action handlers
  const handleMarkAsRead = useCallback((notificationId: string) => {
    markAsReadMutation.mutate(notificationId);
  }, [markAsReadMutation]);

  const handleMarkAllAsRead = useCallback(() => {
    markAllAsReadMutation.mutate();
  }, [markAllAsReadMutation]);

  const handleDeleteNotification = useCallback((notificationId: string) => {
    deleteNotificationMutation.mutate(notificationId);
  }, [deleteNotificationMutation]);

  // Filter handlers
  const handleFilterChange = useCallback((newFilter: Partial<NotificationFilterDto>) => {
    setFilter(prev => ({ ...prev, ...newFilter, page: 1 })); // Reset to first page on filter change
  }, []);

  const handlePageChange = useCallback((page: number) => {
    setFilter(prev => ({ ...prev, page }));
  }, []);

  const handleToggleUnreadOnly = useCallback(() => {
    setFilter(prev => ({ ...prev, unreadOnly: !prev.unreadOnly, page: 1 }));
  }, []);

  // Refresh function
  const refresh = useCallback(() => {
    refetch();
    queryClient.invalidateQueries({ queryKey: notificationQueryKeys.unreadCount(user?.id || '') });
  }, [refetch, queryClient, user?.id]);

  return {
    // Data
    notifications: notificationData?.data || [],
    unreadCount,
    totalCount: notificationData?.total || 0,
    currentPage: notificationData?.page || 1,
    pageSize: notificationData?.pageSize || 20,
    filter,
    
    // State
    isLoading,
    isFetching,
    error: error?.message || null,
    
    // Actions
    markAsRead: handleMarkAsRead,
    markAllAsRead: handleMarkAllAsRead,
    deleteNotification: handleDeleteNotification,
    
    // Filter functions
    setFilter: handleFilterChange,
    setPage: handlePageChange,
    toggleUnreadOnly: handleToggleUnreadOnly,
    
    // Utility functions
    refresh,
    
    // Loading states for individual actions
    isMarkingAsRead: markAsReadMutation.isPending,
    isMarkingAllAsRead: markAllAsReadMutation.isPending,
    isDeletingNotification: deleteNotificationMutation.isPending,
  };
};

// ==================== NOTIFICATION ACTIONS HOOK ====================

/**
 * Hook for notification CRUD operations
 * Separated from main hook for specific administrative functions
 */
export const useNotificationActions = () => {
  const { t } = useTranslation('notifications');
  const queryClient = useQueryClient();
  const notificationSystem = useNotificationSystem();

  const createMutation = useMutation({
    mutationFn: (notification: CreateNotificationDto) => notificationService.createNotification(notification),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.lists() });
      notificationSystem.success(t('actions.createSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('actions.createError'));
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, updates }: { id: string; updates: UpdateNotificationDto }) => 
      notificationService.updateNotification(id, updates),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.lists() });
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.details() });
      notificationSystem.success(t('actions.updateSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('actions.updateError'));
    },
  });

  const sendImmediateMutation = useMutation({
    mutationFn: (notification: CreateNotificationDto) => 
      notificationService.sendImmediateNotification(notification),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.lists() });
      notificationSystem.success(t('actions.sendSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('actions.sendError'));
    },
  });

  const sendBulkMutation = useMutation({
    mutationFn: (notifications: CreateNotificationDto[]) => 
      notificationService.sendBulkNotifications(notifications),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.lists() });
      notificationSystem.success(t('actions.bulkSendSuccess', { count: data.successful }));
      if (data.failed > 0) {
        notificationSystem.error(t('actions.bulkSendPartialError', { failed: data.failed }));
      }
    },
    onError: () => {
      notificationSystem.error(t('actions.bulkSendError'));
    },
  });

  const scheduleMutation = useMutation({
    mutationFn: ({ notification, scheduleTime }: { notification: CreateNotificationDto; scheduleTime: string }) => 
      notificationService.scheduleNotification(notification, scheduleTime),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.lists() });
      notificationSystem.success(t('actions.scheduleSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('actions.scheduleError'));
    },
  });

  return {
    create: createMutation.mutate,
    update: updateMutation.mutate,
    sendImmediate: sendImmediateMutation.mutate,
    sendBulk: sendBulkMutation.mutate,
    schedule: scheduleMutation.mutate,
    
    isCreating: createMutation.isPending,
    isUpdating: updateMutation.isPending,
    isSending: sendImmediateMutation.isPending,
    isSendingBulk: sendBulkMutation.isPending,
    isScheduling: scheduleMutation.isPending,
  };
};

// ==================== NOTIFICATION PREFERENCES HOOK ====================

/**
 * Hook for managing user notification preferences
 */
export const useNotificationPreferences = (userId?: number): UseNotificationPreferencesReturn => {
  const { t } = useTranslation('notifications');
  const queryClient = useQueryClient();
  const { user } = useAuth();
  const notificationSystem = useNotificationSystem();
  
  const effectiveUserId = userId || parseUserId(user?.id);

  const {
    data: preferences = [],
    isLoading,
    error
  } = useQuery({
    queryKey: notificationQueryKeys.preferences(user?.id || ''),
    queryFn: () => effectiveUserId 
      ? notificationService.getUserPreferences(effectiveUserId)
      : Promise.resolve([]),
    enabled: !!effectiveUserId,
    staleTime: 5 * 60 * 1000, // 5 minutes
  });

  const updateMutation = useMutation({
    mutationFn: (newPreferences: UpdateNotificationPreferenceDto[]) => 
      effectiveUserId ? notificationService.updateUserPreferences(effectiveUserId, newPreferences) : Promise.resolve(false),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.preferences(user?.id || '') });
      notificationSystem.success(t('preferences.updateSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('preferences.updateError'));
    },
  });

  const updateSingleMutation = useMutation({
    mutationFn: (preference: UpdateNotificationPreferenceDto) => 
      effectiveUserId ? notificationService.updateSinglePreference(effectiveUserId, preference) : Promise.resolve(false),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.preferences(user?.id || '') });
      notificationSystem.success(t('preferences.updateSingleSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('preferences.updateSingleError'));
    },
  });

  const resetMutation = useMutation({
    mutationFn: () => 
      effectiveUserId ? notificationService.resetPreferencesToDefaults(effectiveUserId) : Promise.resolve(false),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.preferences(user?.id || '') });
      notificationSystem.success(t('preferences.resetSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('preferences.resetError'));
    },
  });

  return {
    preferences,
    isLoading,
    error: error?.message || null,
    
    updatePreferences: updateMutation.mutate,
    updateSinglePreference: updateSingleMutation.mutate,
    resetToDefaults: resetMutation.mutate,
    
    isUpdating: updateMutation.isPending,
    isUpdatingSingle: updateSingleMutation.isPending,
    isResetting: resetMutation.isPending,
  };
};

// ==================== NOTIFICATION TEMPLATES HOOK ====================

/**
 * Hook for managing notification templates
 */
export const useNotificationTemplates = (): UseNotificationTemplatesReturn => {
  const { t } = useTranslation('notifications');
  const queryClient = useQueryClient();
  const notificationSystem = useNotificationSystem();

  const {
    data: templates = [],
    isLoading,
    error
  } = useQuery({
    queryKey: notificationQueryKeys.templates(),
    queryFn: () => notificationService.getTemplates(),
    staleTime: 10 * 60 * 1000, // 10 minutes
  });

  const createMutation = useMutation({
    mutationFn: (template: Partial<NotificationTemplateDto>) => notificationService.createTemplate(template),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.templates() });
      notificationSystem.success(t('templates.createSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('templates.createError'));
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, updates }: { id: string; updates: Partial<NotificationTemplateDto> }) => 
      notificationService.updateTemplate(id, updates),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.templates() });
      notificationSystem.success(t('templates.updateSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('templates.updateError'));
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (templateId: string) => notificationService.deleteTemplate(templateId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationQueryKeys.templates() });
      notificationSystem.success(t('templates.deleteSuccess'));
    },
    onError: () => {
      notificationSystem.error(t('templates.deleteError'));
    },
  });

  return {
    templates,
    isLoading,
    error: error?.message || null,
    
    createTemplate: (template: Partial<NotificationTemplateDto>) => createMutation.mutate(template),
    updateTemplate: (id: string, updates: Partial<NotificationTemplateDto>) => updateMutation.mutate({ id, updates }),
    deleteTemplate: (templateId: string) => deleteMutation.mutate(templateId),
    
    isCreating: createMutation.isPending,
    isUpdating: updateMutation.isPending,
    isDeleting: deleteMutation.isPending,
  };
};

// ==================== SINGLE NOTIFICATION HOOK ====================

/**
 * Hook for managing a single notification
 */
export const useSingleNotification = (notificationId: string): UseQueryResult<NotificationDto | null> => {
  return useQuery({
    queryKey: notificationQueryKeys.detail(notificationId),
    queryFn: () => notificationService.getNotificationById(notificationId),
    enabled: !!notificationId,
    staleTime: 2 * 60 * 1000, // 2 minutes
  });
};

// ==================== NOTIFICATION HISTORY HOOK ====================

/**
 * Hook for fetching notification delivery history
 */
export const useNotificationHistory = (notificationId: string) => {
  return useQuery({
    queryKey: notificationQueryKeys.history(notificationId),
    queryFn: () => notificationService.getNotificationHistory(notificationId),
    enabled: !!notificationId,
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
};

// ==================== REAL-TIME NOTIFICATIONS HOOK ====================

/**
 * Hook for real-time notification updates (to be extended with SignalR)
 */
export const useRealTimeNotifications = (userId?: number) => {
  const { user } = useAuth();
  const queryClient = useQueryClient();
  const effectiveUserId = userId || parseUserId(user?.id);
  
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    if (!effectiveUserId) return;

    // TODO: Implement SignalR connection for real-time updates
    // This is a placeholder for future SignalR integration
    console.log('Setting up real-time notifications for user:', effectiveUserId);
    
    // Simulate connection status
    setIsConnected(true);

    return () => {
      setIsConnected(false);
    };
  }, [effectiveUserId, queryClient]);

  return {
    isConnected,
  };
};
