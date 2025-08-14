/**
 * Comprehensive notification types for the Ikhtibar notification module
 * Aligned with backend implementation patterns and DTOs
 */

// ==================== BASE INTERFACES ====================

/**
 * Generic paginated result interface
 */
export interface PagedResult<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
}

// ==================== ENUMS ====================

export enum NotificationType {
  ExamReminder = 'ExamReminder',
  ExamStart = 'ExamStart', 
  ExamEnd = 'ExamEnd',
  GradingComplete = 'GradingComplete',
  DeadlineReminder = 'DeadlineReminder',
  SystemAlert = 'SystemAlert',
  Welcome = 'Welcome',
  PasswordReset = 'PasswordReset',
  AccountActivation = 'AccountActivation',
  RoleAssignment = 'RoleAssignment'
}

export enum NotificationPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Critical = 'Critical'
}

export enum NotificationStatus {
  Pending = 'Pending',
  Scheduled = 'Scheduled',
  Sent = 'Sent',
  Delivered = 'Delivered',
  Read = 'Read',
  Failed = 'Failed',
  Cancelled = 'Cancelled'
}

export enum NotificationChannel {
  Email = 'Email',
  Sms = 'Sms',
  InApp = 'InApp',
  Push = 'Push',
  WhatsApp = 'WhatsApp'
}

export enum NotificationDeliveryStatus {
  Pending = 'Pending',
  Sent = 'Sent',
  Delivered = 'Delivered',
  Failed = 'Failed',
  Bounced = 'Bounced',
  Opened = 'Opened',
  Clicked = 'Clicked'
}

// ==================== CORE INTERFACES ====================

/**
 * Main notification DTO matching backend NotificationDto
 */
export interface NotificationDto {
  id: string;
  userId: number;
  notificationType: NotificationType;
  priority: NotificationPriority;
  status: NotificationStatus;
  subject: string;
  message: string;
  isRead: boolean;
  readAt?: string;
  sentAt?: string;
  scheduledAt?: string;
  entityType?: string;
  entityId?: number;
  variables: Record<string, any>;
  channelData: Record<string, any>;
  metadata: Record<string, any>;
  createdAt: string;
  updatedAt: string;
}

/**
 * Create notification DTO for new notifications
 */
export interface CreateNotificationDto {
  userId: number;
  notificationType: NotificationType;
  priority: NotificationPriority;
  subject: string;
  message: string;
  scheduledAt?: string;
  entityType?: string;
  entityId?: number;
  variables?: Record<string, any>;
  channelData?: Record<string, any>;
  metadata?: Record<string, any>;
}

/**
 * Update notification DTO for editing notifications
 */
export interface UpdateNotificationDto {
  subject?: string;
  message?: string;
  priority?: NotificationPriority;
  scheduledAt?: string;
  metadata?: Record<string, any>;
}

/**
 * Notification filter DTO for querying notifications
 */
export interface NotificationFilterDto {
  page?: number;
  pageSize?: number;
  unreadOnly?: boolean;
  notificationType?: NotificationType;
  priority?: NotificationPriority;
  status?: NotificationStatus;
  fromDate?: string;
  toDate?: string;
  searchTerm?: string;
  entityType?: string;
  entityId?: number;
}

/**
 * Notification preference DTO
 */
export interface NotificationPreferenceDto {
  id: string;
  userId: number;
  notificationType: NotificationType;
  emailEnabled: boolean;
  smsEnabled: boolean;
  inAppEnabled: boolean;
  pushEnabled: boolean;
  createdAt: string;
  updatedAt: string;
}

/**
 * Update notification preference DTO
 */
export interface UpdateNotificationPreferenceDto {
  notificationType: NotificationType;
  emailEnabled: boolean;
  smsEnabled: boolean;
  inAppEnabled: boolean;
  pushEnabled: boolean;
}

/**
 * Notification history DTO
 */
export interface NotificationHistoryDto {
  id: string;
  notificationId: string;
  channel: NotificationChannel;
  status: NotificationDeliveryStatus;
  attemptedAt: string;
  deliveredAt?: string;
  errorMessage?: string;
  externalId?: string;
  responseData?: string;
}

/**
 * Notification template DTO
 */
export interface NotificationTemplateDto {
  id: string;
  name: string;
  type: NotificationType;
  subjectTemplate: string;
  messageTemplate: string;
  language: string;
  isActive: boolean;
  variables?: Record<string, any>;
  description?: string;
  createdAt: string;
  updatedAt: string;
}

// ==================== RESPONSE INTERFACES ====================

/**
 * Paginated notification response
 */
export type NotificationPagedResponse = PagedResult<NotificationDto>;

/**
 * Notification unread count response
 */
export interface NotificationUnreadCountResponse {
  unreadCount: number;
  lastUpdated: string;
}

/**
 * Bulk notification response
 */
export interface BulkNotificationResponse {
  successful: number;
  failed: number;
  errors: string[];
  notificationIds: string[];
}

/**
 * Notification send response
 */
export interface NotificationSendResponse {
  success: boolean;
  notificationId?: string;
  errorMessage?: string;
  deliveryChannels: NotificationChannel[];
}

// ==================== COMPONENT PROPS INTERFACES ====================

/**
 * Notification item component props
 */
export interface NotificationItemProps {
  notification: NotificationDto;
  onRead?: (notificationId: string) => void;
  onDelete?: (notificationId: string) => void;
  onClick?: (notification: NotificationDto) => void;
  showActions?: boolean;
  compact?: boolean;
}

/**
 * Notification list component props
 */
export interface NotificationListProps {
  notifications?: NotificationDto[];
  loading?: boolean;
  error?: string | null;
  onLoadMore?: () => void;
  onRefresh?: () => void;
  onNotificationClick?: (notification: NotificationDto) => void;
  onMarkAsRead?: (notificationId: string) => void;
  onMarkAllAsRead?: () => void;
  onDelete?: (notificationId: string) => void;
  showPagination?: boolean;
  compact?: boolean;
  className?: string;
}

/**
 * Notification bell component props
 */
export interface NotificationBellProps {
  unreadCount?: number;
  onClick?: () => void;
  loading?: boolean;
  size?: 'sm' | 'md' | 'lg';
  showCount?: boolean;
  maxCount?: number;
  className?: string;
}

/**
 * Notification center component props
 */
export interface NotificationCenterProps {
  isOpen: boolean;
  onClose: () => void;
  userId?: number;
  maxHeight?: string;
  showPreferences?: boolean;
  showTemplates?: boolean;
  className?: string;
}

/**
 * Notification preferences component props
 */
export interface NotificationPreferencesProps {
  preferences?: NotificationPreferenceDto[];
  loading?: boolean;
  onSave?: (preferences: UpdateNotificationPreferenceDto[]) => void;
  onReset?: () => void;
  className?: string;
}

/**
 * Notification toast component props
 */
export interface NotificationToastProps {
  notification: NotificationDto;
  onDismiss?: () => void;
  onAction?: (actionType: string) => void;
  duration?: number;
  position?: 'top-right' | 'top-left' | 'bottom-right' | 'bottom-left';
  className?: string;
}

// ==================== HOOK RETURN TYPES ====================

/**
 * useNotifications hook return type
 */
export interface UseNotificationsReturn {
  // Data
  notifications: NotificationDto[];
  unreadCount: number;
  totalCount: number;
  currentPage: number;
  pageSize: number;
  filter: NotificationFilterDto;
  
  // State
  isLoading: boolean;
  isFetching: boolean;
  error: string | null;
  
  // Actions
  markAsRead: (notificationId: string) => void;
  markAllAsRead: () => void;
  deleteNotification: (notificationId: string) => void;
  
  // Filter functions
  setFilter: (filter: Partial<NotificationFilterDto>) => void;
  setPage: (page: number) => void;
  toggleUnreadOnly: () => void;
  
  // Utility functions
  refresh: () => void;
  
  // Loading states for individual actions
  isMarkingAsRead: boolean;
  isMarkingAllAsRead: boolean;
  isDeletingNotification: boolean;
}

/**
 * useNotificationPreferences hook return type
 */
export interface UseNotificationPreferencesReturn {
  preferences: NotificationPreferenceDto[];
  isLoading: boolean;
  error: string | null;
  
  // Actions
  updatePreferences: (preferences: UpdateNotificationPreferenceDto[]) => void;
  updateSinglePreference: (preference: UpdateNotificationPreferenceDto) => void;
  resetToDefaults: () => void;
  
  // Loading states
  isUpdating: boolean;
  isUpdatingSingle: boolean;
  isResetting: boolean;
}

/**
 * useNotificationTemplates hook return type
 */
export interface UseNotificationTemplatesReturn {
  templates: NotificationTemplateDto[];
  isLoading: boolean;
  error: string | null;
  
  // Actions
  createTemplate: (template: Partial<NotificationTemplateDto>) => void;
  updateTemplate: (id: string, updates: Partial<NotificationTemplateDto>) => void;
  deleteTemplate: (templateId: string) => void;
  
  // Loading states
  isCreating: boolean;
  isUpdating: boolean;
  isDeleting: boolean;
}

/**
 * useRealTimeNotifications hook return type
 */
export interface UseRealTimeNotificationsReturn {
  isConnected: boolean;
  connectionError: string | null;
  newNotifications: NotificationDto[];
  
  // Actions
  connect: () => void;
  disconnect: () => void;
  clearNewNotifications: () => void;
}

// ==================== UTILITY TYPES ====================

/**
 * Notification type display info
 */
export interface NotificationTypeInfo {
  type: NotificationType;
  label: string;
  icon: string;
  color: string;
  description: string;
}

/**
 * Notification priority display info
 */
export interface NotificationPriorityInfo {
  priority: NotificationPriority;
  label: string;
  color: string;
  urgency: number;
}

/**
 * Notification channel display info
 */
export interface NotificationChannelInfo {
  channel: NotificationChannel;
  label: string;
  icon: string;
  description: string;
  isEnabled: boolean;
}

// ==================== API RESPONSE TYPES ====================

/**
 * API error response
 */
export interface NotificationApiError {
  message: string;
  code: string;
  details?: Record<string, any>;
}

/**
 * API success response wrapper
 */
export interface NotificationApiResponse<T = any> {
  success: boolean;
  data?: T;
  error?: NotificationApiError;
  meta?: {
    timestamp: string;
    requestId: string;
  };
}

// ==================== CONFIGURATION TYPES ====================

/**
 * Notification system configuration
 */
export interface NotificationConfig {
  realTimeEnabled: boolean;
  pollingInterval: number;
  maxNotificationsPerPage: number;
  toastDuration: number;
  enableSounds: boolean;
  enableDesktopNotifications: boolean;
  retryAttempts: number;
  retryDelay: number;
}

/**
 * Notification filter presets
 */
export interface NotificationFilterPreset {
  name: string;
  filter: NotificationFilterDto;
  isDefault: boolean;
}

export default NotificationDto;
