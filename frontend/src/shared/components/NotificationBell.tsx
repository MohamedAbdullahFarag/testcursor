/**
 * NotificationBell Component
 * 
 * A notification bell icon with unread count badge that shows in the header.
 * Displays unread notification count and provides quick access to notifications.
 */

import React, { memo } from 'react';
import { useTranslation } from 'react-i18next';

import { useNotificationCount } from '../hooks/useNotifications';

interface NotificationBellProps {
  onClick?: () => void;
  className?: string;
  size?: 'sm' | 'md' | 'lg';
  showCount?: boolean;
  maxCount?: number;
}

/**
 * Notification Bell component with unread count badge
 */
const NotificationBell: React.FC<NotificationBellProps> = memo(({
  onClick,
  className = '',
  size = 'md',
  showCount = true,
  maxCount = 99,
}) => {
  const { t } = useTranslation();
  const { unreadCount, isLoading } = useNotificationCount();

  // Size configurations
  const sizeConfig = {
    sm: {
      icon: 'h-5 w-5',
      badge: 'h-4 w-4 text-xs',
      position: '-top-1 -right-1',
    },
    md: {
      icon: 'h-6 w-6',
      badge: 'h-5 w-5 text-xs',
      position: '-top-2 -right-2',
    },
    lg: {
      icon: 'h-8 w-8',
      badge: 'h-6 w-6 text-sm',
      position: '-top-2 -right-2',
    },
  };

  const config = sizeConfig[size];
  const hasUnread = unreadCount > 0;
  const displayCount = unreadCount > maxCount ? `${maxCount}+` : unreadCount.toString();

  const handleClick = () => {
    onClick?.();
  };

  const handleKeyDown = (event: React.KeyboardEvent) => {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      handleClick();
    }
  };

  return (
    <div className="relative">
      <button
        type="button"
        onClick={handleClick}
        onKeyDown={handleKeyDown}
        disabled={isLoading}
        className={`
          relative inline-flex items-center justify-center
          rounded-full p-2 transition-colors duration-200
          hover:bg-gray-100 dark:hover:bg-gray-800
          focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2
          disabled:opacity-50 disabled:cursor-not-allowed
          ${className}
        `}
        aria-label={t('notifications.bell_aria_label', { count: unreadCount })}
        title={
          hasUnread 
            ? t('notifications.unread_count', { count: unreadCount })
            : t('notifications.no_unread')
        }
      >
        {/* Bell Icon */}
        {hasUnread ? (
          <svg 
            className={`${config.icon} text-primary-600 dark:text-primary-400`}
            fill="currentColor" 
            viewBox="0 0 24 24"
            aria-hidden="true"
          >
            <path d="M12 22c1.1 0 2-.9 2-2h-4c0 1.1.89 2 2 2zm6-6v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2z"/>
          </svg>
        ) : (
          <svg 
            className={`${config.icon} text-gray-600 dark:text-gray-400`}
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
            aria-hidden="true"
          >
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-5-5-5 5h5zm0 0v-2a6 6 0 00-6-6H7a6 6 0 00-6 6v2"/>
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13.73 21a2 2 0 01-3.46 0"/>
          </svg>
        )}

        {/* Loading indicator */}
        {isLoading && (
          <div className={`absolute inset-0 flex items-center justify-center`}>
            <div className="h-3 w-3 animate-spin rounded-full border-2 border-primary-600 border-t-transparent" />
          </div>
        )}

        {/* Unread count badge */}
        {showCount && hasUnread && !isLoading && (
          <span
            className={`
              absolute flex items-center justify-center
              ${config.badge} ${config.position}
              bg-red-500 text-white font-semibold
              rounded-full ring-2 ring-white dark:ring-gray-900
              animate-pulse
            `}
            aria-hidden="true"
          >
            {displayCount}
          </span>
        )}

        {/* Visual unread indicator (dot) when count is hidden */}
        {!showCount && hasUnread && !isLoading && (
          <span
            className={`
              absolute ${config.position}
              h-3 w-3 bg-red-500 rounded-full
              ring-2 ring-white dark:ring-gray-900
              animate-pulse
            `}
            aria-hidden="true"
          />
        )}
      </button>

      {/* Screen reader announcement for count changes */}
      <div 
        className="sr-only" 
        aria-live="polite" 
        aria-atomic="true"
      >
        {hasUnread && (
          t('notifications.unread_count_sr', { count: unreadCount })
        )}
      </div>
    </div>
  );
});

NotificationBell.displayName = 'NotificationBell';

export default NotificationBell;
