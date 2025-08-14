/**
 * Notification-related TypeScript types and interfaces
 * 
 * This file contains all type definitions for the notification system,
 * including entities, requests, responses, and filter types.
 */

// Base notification status enum
export enum NotificationStatus {
  Draft = 'Draft',
  Scheduled = 'Scheduled',
  Sent = 'Sent',
  Failed = 'Failed',
  Cancelled = 'Cancelled'
}

// Notification type enum
export enum NotificationType {
  Info = 'Info',
  Warning = 'Warning',
  Error = 'Error',
  Success = 'Success',
  Reminder = 'Reminder',
  Update = 'Update',
  Marketing = 'Marketing'
}

// Notification priority enum
export enum NotificationPriority {
  Low = 'Low',
  Normal = 'Normal',
  High = 'High',
  Critical = 'Critical'
}

// Notification channel enum
export enum NotificationChannel {
  Email = 'Email',
  Sms = 'Sms',
  Push = 'Push',
  InApp = 'InApp'
}

// Notification delivery status enum
export enum NotificationDeliveryStatus {
  Pending = 'Pending',
  Sent = 'Sent',
  Delivered = 'Delivered',
  Failed = 'Failed',
  Bounced = 'Bounced',
  Read = 'Read'
}

/**
 * Core notification entity
 */
export interface Notification {
  id: string;
  title: string;
  message: string;
  type: NotificationType;
  priority: NotificationPriority;
  status: NotificationStatus;
  isRead: boolean;
  readAt?: Date;
  scheduledAt?: Date;
  sentAt?: Date;
  expiresAt?: Date;
  targetUserId?: string;
  targetRole?: string;
  metadata?: Record<string, unknown>;
  createdAt: Date;
  updatedAt: Date;
  createdBy: string;
  channels: NotificationChannel[];
  deliveryStatus: Record<NotificationChannel, NotificationDeliveryStatus>;
}

/**
 * Notification response DTO
 */
export interface NotificationResponse {
  id: string;
  title: string;
  message: string;
  type: NotificationType;
  priority: NotificationPriority;
  status: NotificationStatus;
  isRead: boolean;
  readAt?: Date;
  scheduledAt?: Date;
  sentAt?: Date;
  expiresAt?: Date;
  createdAt: Date;
  updatedAt: Date;
  channels: NotificationChannel[];
  deliveryStatus: Record<NotificationChannel, NotificationDeliveryStatus>;
}

/**
 * Request to create a new notification
 */
export interface CreateNotificationRequest {
  title: string;
  message: string;
  type: NotificationType;
  priority: NotificationPriority;
  targetUserId?: string;
  targetRole?: string;
  scheduledAt?: Date;
  expiresAt?: Date;
  channels: NotificationChannel[];
  metadata?: Record<string, unknown>;
}

/**
 * Request to update an existing notification
 */
export interface UpdateNotificationRequest {
  title?: string;
  message?: string;
  type?: NotificationType;
  priority?: NotificationPriority;
  scheduledAt?: Date;
  expiresAt?: Date;
  channels?: NotificationChannel[];
  metadata?: Record<string, unknown>;
}

/**
 * User notification preferences
 */
export interface NotificationPreference {
  id: string;
  userId: string;
  notificationType: NotificationType;
  channel: NotificationChannel;
  isEnabled: boolean;
  createdAt: Date;
  updatedAt: Date;
}

/**
 * Filter options for notification queries
 */
export interface NotificationFilter {
  type?: NotificationType;
  priority?: NotificationPriority;
  status?: NotificationStatus;
  isRead?: boolean;
  channel?: NotificationChannel;
  startDate?: Date;
  endDate?: Date;
  searchTerm?: string;
}

/**
 * Paginated response for notifications
 */
export interface PaginatedResponse<T> {
  data: T[];
  pagination: {
    currentPage: number;
    totalPages: number;
    totalCount: number;
    pageSize: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
  };
  success: boolean;
  message?: string;
  errors?: string[];
}

/**
 * Hook return type for useNotifications
 */
export interface UseNotificationsResult {
  notifications: Notification[];
  unreadCount: number;
  isLoading: boolean;
  error: string | null;
  hasNextPage: boolean;
  refetch: () => Promise<void>;
  markAsRead: (id: string) => Promise<void>;
  markAllAsRead: () => Promise<void>;
  deleteNotification: (id: string) => Promise<void>;
  loadMore: () => Promise<void>;
}

/**
 * Hook return type for useNotificationPreferences
 */
export interface UseNotificationPreferencesResult {
  preferences: NotificationPreference[];
  isLoading: boolean;
  error: string | null;
  updatePreferences: (preferences: Partial<NotificationPreference>[]) => Promise<void>;
  refetch: () => Promise<void>;
}
