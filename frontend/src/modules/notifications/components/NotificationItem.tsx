/**
 * NotificationItem component - Individual notification item with actions
 * Displays single notification with customizable layout and interactions
 * Following established patterns with accessibility and responsive design
 */

import React, { useCallback, useState } from 'react';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';

import { 
  NotificationDto, 
  NotificationType, 
  NotificationPriority 
} from '../types/notification.types';

// Component interfaces
interface NotificationItemProps {
  notification: NotificationDto;
  onClick?: (notification: NotificationDto) => void;
  onMarkAsRead?: (notificationId: string) => void;
  onMarkAsUnread?: (notificationId: string) => void;
  onDelete?: (notificationId: string) => void;
  showActions?: boolean;
  showTimestamp?: boolean;
  compact?: boolean;
  className?: string;
}

interface NotificationActionsProps {
  notification: NotificationDto;
  onMarkAsRead?: (notificationId: string) => void;
  onMarkAsUnread?: (notificationId: string) => void;
  onDelete?: (notificationId: string) => void;
  isActioning: boolean;
}

// ==================== SUB-COMPONENTS ====================

/**
 * Notification type icon
 */
const NotificationTypeIcon: React.FC<{ 
  type: NotificationType; 
  priority: NotificationPriority;
  className?: string;
}> = React.memo(({ type, priority, className }) => {
  const getIconConfig = () => {
    const priorityColors = {
      [NotificationPriority.Low]: 'text-gray-500',
      [NotificationPriority.Medium]: 'text-blue-500',
      [NotificationPriority.High]: 'text-orange-500',
      [NotificationPriority.Critical]: 'text-red-500'
    };

    const color = priorityColors[priority as keyof typeof priorityColors] || 'text-gray-500';

    switch (type) {
      case NotificationType.ExamReminder:
        return {
          color,
          path: 'M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z'
        };
      case NotificationType.ExamStart:
        return {
          color,
          path: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z'
        };
      case NotificationType.ExamEnd:
        return {
          color,
          path: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z'
        };
      case NotificationType.GradingComplete:
        return {
          color,
          path: 'M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z'
        };
      case NotificationType.SystemAlert:
        return {
          color,
          path: 'M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z'
        };
      default:
        return {
          color,
          path: 'M15 17h5l-5 5v-5zM4.272 7.272a1 1 0 011.414 0L12 13.586l6.314-6.314a1 1 0 111.414 1.414L13.414 15l6.314 6.314a1 1 0 01-1.414 1.414L12 16.414l-6.314 6.314a1 1 0 01-1.414-1.414L10.586 15 4.272 8.686a1 1 0 010-1.414z'
        };
    }
  };

  const { color, path } = getIconConfig();

  return (
    <svg 
      className={classNames('h-5 w-5', color, className)} 
      fill="none" 
      stroke="currentColor" 
      viewBox="0 0 24 24"
      aria-hidden="true"
    >
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d={path} />
    </svg>
  );
});

NotificationTypeIcon.displayName = 'NotificationTypeIcon';

/**
 * Notification actions dropdown
 */
const NotificationActions: React.FC<NotificationActionsProps> = React.memo(({
  notification,
  onMarkAsRead,
  onMarkAsUnread,
  onDelete,
  isActioning
}) => {
  const { t } = useTranslation('notifications');
  const [isOpen, setIsOpen] = useState(false);

  const handleMarkAsRead = useCallback(() => {
    onMarkAsRead?.(notification.id);
    setIsOpen(false);
  }, [notification.id, onMarkAsRead]);

  const handleMarkAsUnread = useCallback(() => {
    onMarkAsUnread?.(notification.id);
    setIsOpen(false);
  }, [notification.id, onMarkAsUnread]);

  const handleDelete = useCallback(() => {
    onDelete?.(notification.id);
    setIsOpen(false);
  }, [notification.id, onDelete]);

  return (
    <div className="relative">
      <button
        onClick={(e) => {
          e.stopPropagation();
          setIsOpen(!isOpen);
        }}
        disabled={isActioning}
        className="p-1 rounded-full text-gray-400 hover:text-gray-600 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50"
        aria-label={t('actions.moreActions')}
        aria-expanded={isOpen}
        aria-haspopup="menu"
      >
        <svg className="h-4 w-4" fill="currentColor" viewBox="0 0 20 20">
          <path d="M10 6a2 2 0 110-4 2 2 0 010 4zM10 12a2 2 0 110-4 2 2 0 010 4zM10 18a2 2 0 110-4 2 2 0 010 4z" />
        </svg>
      </button>

      {isOpen && (
        <>
          {/* Backdrop */}
          <div 
            className="fixed inset-0 z-10" 
            onClick={() => setIsOpen(false)}
            aria-hidden="true"
          />
          
          {/* Menu */}
          <div className="absolute right-0 mt-1 w-48 bg-white rounded-md shadow-lg border border-gray-200 z-20">
            <div className="py-1" role="menu">
              {!notification.isRead ? (
                <button
                  onClick={handleMarkAsRead}
                  className="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                  role="menuitem"
                >
                  {t('actions.markAsRead')}
                </button>
              ) : (
                <button
                  onClick={handleMarkAsUnread}
                  className="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                  role="menuitem"
                >
                  {t('actions.markAsUnread')}
                </button>
              )}
              
              <button
                onClick={handleDelete}
                className="block w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-red-50"
                role="menuitem"
              >
                {t('actions.delete')}
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  );
});

NotificationActions.displayName = 'NotificationActions';

// ==================== MAIN COMPONENT ====================

/**
 * NotificationItem - Individual notification display component
 */
export const NotificationItem: React.FC<NotificationItemProps> = React.memo(({
  notification,
  onClick,
  onMarkAsRead,
  onMarkAsUnread,
  onDelete,
  showActions = true,
  showTimestamp = true,
  compact = false,
  className
}) => {
  const { t } = useTranslation('notifications');
  const [isActioning, setIsActioning] = useState(false);

  // Format relative time
  const formatRelativeTime = useCallback((date: Date) => {
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMinutes = Math.floor(diffMs / (1000 * 60));
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));

    if (diffMinutes < 1) return t('time.justNow');
    if (diffMinutes < 60) return t('time.minutesAgo', { count: diffMinutes });
    if (diffHours < 24) return t('time.hoursAgo', { count: diffHours });
    if (diffDays < 7) return t('time.daysAgo', { count: diffDays });
    
    return date.toLocaleDateString();
  }, [t]);

  // Handle click
  const handleClick = useCallback(() => {
    onClick?.(notification);
  }, [onClick, notification]);

  // Wrapped action handlers with loading state
  const handleMarkAsRead = useCallback(async (notificationId: string) => {
    setIsActioning(true);
    try {
      await onMarkAsRead?.(notificationId);
    } finally {
      setIsActioning(false);
    }
  }, [onMarkAsRead]);

  const handleMarkAsUnread = useCallback(async (notificationId: string) => {
    setIsActioning(true);
    try {
      await onMarkAsUnread?.(notificationId);
    } finally {
      setIsActioning(false);
    }
  }, [onMarkAsUnread]);

  const handleDelete = useCallback(async (notificationId: string) => {
    setIsActioning(true);
    try {
      await onDelete?.(notificationId);
    } finally {
      setIsActioning(false);
    }
  }, [onDelete]);

  const itemClasses = classNames(
    'group relative transition-colors duration-150',
    {
      'cursor-pointer hover:bg-gray-50': onClick,
      'bg-blue-50 border-l-2 border-l-blue-500': !notification.isRead,
      'py-2': compact,
      'py-4': !compact
    },
    className
  );

  return (
    <div
      className={itemClasses}
      onClick={handleClick}
      role={onClick ? 'button' : undefined}
      tabIndex={onClick ? 0 : undefined}
      onKeyDown={onClick ? (e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault();
          handleClick();
        }
      } : undefined}
      aria-label={onClick ? t('bell.notificationItemAriaLabel', { subject: notification.subject }) : undefined}
    >
      <div className="flex items-start space-x-3 px-4">
        {/* Icon */}
        <div className="flex-shrink-0 mt-1">
          <NotificationTypeIcon 
            type={notification.notificationType} 
            priority={notification.priority}
          />
        </div>

        {/* Content */}
        <div className="flex-1 min-w-0">
          <div className="flex items-start justify-between">
            <div className="flex-1 min-w-0">
              {/* Subject */}
              <h4 className={classNames(
                'text-sm font-medium truncate',
                notification.isRead ? 'text-gray-900' : 'text-gray-900 font-semibold'
              )}>
                {notification.subject}
              </h4>

              {/* Message */}
              <p className={classNames(
                'text-sm text-gray-600 mt-1',
                compact ? 'line-clamp-1' : 'line-clamp-2'
              )}>
                {notification.message}
              </p>

              {/* Timestamp */}
              {showTimestamp && (
                <p className="text-xs text-gray-500 mt-1">
                  {formatRelativeTime(new Date(notification.createdAt))}
                </p>
              )}
            </div>

            {/* Unread indicator and actions */}
            <div className="flex items-center space-x-2 ml-4">
              {!notification.isRead && (
                <span className="flex-shrink-0 w-2 h-2 bg-blue-600 rounded-full"></span>
              )}
              
              {showActions && (
                <NotificationActions
                  notification={notification}
                  onMarkAsRead={handleMarkAsRead}
                  onMarkAsUnread={handleMarkAsUnread}
                  onDelete={handleDelete}
                  isActioning={isActioning}
                />
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
});

NotificationItem.displayName = 'NotificationItem';

export default NotificationItem;
