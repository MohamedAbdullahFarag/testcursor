/**
 * Notification module components index
 * Central export point for all notification components
 */

// Notification Components
export { NotificationList } from './NotificationList';
export { NotificationPreferences } from './NotificationPreferences';
export { NotificationTemplateManager } from './NotificationTemplateManager';
export { NotificationModal } from './NotificationModal';
export { NotificationToast } from './NotificationToast';
export { NotificationSystem } from './NotificationSystem';

// Re-export types for convenience
export type { Notification, NotificationTemplate, NotificationPreferenceDto } from '../types';
