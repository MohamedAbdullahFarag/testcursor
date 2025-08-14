/**
 * Notification module main exports
 * Central export point for the entire notifications module
 */

// Types
export * from './types/notification.types';

// Services
export * from './services/notificationService';

// Hooks
export * from './hooks/useNotifications';

// Components
export * from './components';

// Locales
export * from './locales';

// Re-export main components for convenience
export { NotificationCenter } from './components/NotificationCenter';
export { NotificationBell } from './components/NotificationBell';
export { NotificationList } from './components/NotificationList';
export { NotificationItem } from './components/NotificationItem';
