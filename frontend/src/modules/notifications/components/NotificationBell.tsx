/**
 * NotificationBell component - Bell icon with unread count indicator
 * Displays notification bell with live unread count and dropdown menu
 * Following established patterns with proper accessibility and mobile support
 */

import React, { useState, useCallback, useRef, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';

import { useNotifications } from '../hooks/useNotifications';
import { NotificationDto } from '../types/notification.types';

// Component interfaces
interface NotificationBellProps {
  className?: string;
  size?: 'sm' | 'md' | 'lg';
  showDropdown?: boolean;
  onNotificationClick?: (notification: NotificationDto) => void;
  onViewAllClick?: () => void;
  maxDropdownItems?: number;
}

interface NotificationDropdownProps {
  notifications: NotificationDto[];
  unreadCount: number;
  onNotificationClick?: (notification: NotificationDto) => void;
  onViewAllClick?: () => void;
  onMarkAllAsRead: () => void;
  isMarkingAllAsRead: boolean;
  isOpen: boolean;
  onClose: () => void;
  maxItems: number;
}

// ==================== SUB-COMPONENTS ====================

/**
 * Notification dropdown menu
 */
const NotificationDropdown: React.FC<NotificationDropdownProps> = React.memo(({
  notifications,
  unreadCount,
  onNotificationClick,
  onViewAllClick,
  onMarkAllAsRead,
  isMarkingAllAsRead,
  isOpen,
  onClose,
  maxItems
}) => {
  const { t } = useTranslation('notifications');
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        onClose();
      }
    };

    if (isOpen) {
      document.addEventListener('mousedown', handleClickOutside);
      return () => document.removeEventListener('mousedown', handleClickOutside);
    }
  }, [isOpen, onClose]);

  // Close dropdown on escape key
  useEffect(() => {
    const handleEscape = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        onClose();
      }
    };

    if (isOpen) {
      document.addEventListener('keydown', handleEscape);
      return () => document.removeEventListener('keydown', handleEscape);
    }
  }, [isOpen, onClose]);

  const handleNotificationClick = useCallback((notification: NotificationDto) => {
    onNotificationClick?.(notification);
    onClose();
  }, [onNotificationClick, onClose]);

  const handleViewAllClick = useCallback(() => {
    onViewAllClick?.();
    onClose();
  }, [onViewAllClick, onClose]);

  const handleMarkAllAsRead = useCallback(() => {
    onMarkAllAsRead();
  }, [onMarkAllAsRead]);

  if (!isOpen) return null;

  const displayNotifications = notifications.slice(0, maxItems);
  const hasMore = notifications.length > maxItems;

  return (
    <div
      ref={dropdownRef}
      className="absolute right-0 mt-2 w-80 bg-white rounded-lg shadow-lg border border-gray-200 z-50 max-h-96 overflow-hidden"
      role="menu"
      aria-label={t('bell.dropdownAriaLabel')}
    >
      {/* Header */}
      <div className="px-4 py-3 border-b border-gray-200 bg-gray-50 rounded-t-lg">
        <div className="flex items-center justify-between">
          <h3 className="text-sm font-semibold text-gray-900">
            {t('bell.notificationsTitle')}
          </h3>
          {unreadCount > 0 && (
            <button
              onClick={handleMarkAllAsRead}
              disabled={isMarkingAllAsRead}
              className="text-xs text-blue-600 hover:text-blue-800 font-medium disabled:opacity-50"
              aria-label={t('bell.markAllReadAriaLabel')}
            >
              {isMarkingAllAsRead ? t('bell.markingAsRead') : t('bell.markAllRead')}
            </button>
          )}
        </div>
        {unreadCount > 0 && (
          <p className="text-xs text-gray-600 mt-1">
            {t('bell.unreadCountText', { count: unreadCount })}
          </p>
        )}
      </div>

      {/* Notifications List */}
      <div className="max-h-64 overflow-y-auto">
        {displayNotifications.length === 0 ? (
          <div className="px-4 py-6 text-center">
            <div className="text-gray-400 mb-2">
              <svg className="h-8 w-8 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-5 5v-5zM4 6h16v12H4V6z" />
              </svg>
            </div>
            <p className="text-sm text-gray-600">{t('bell.noNotifications')}</p>
          </div>
        ) : (
          <div className="divide-y divide-gray-100">
            {displayNotifications.map((notification) => (
              <div
                key={notification.id}
                className={classNames(
                  'px-4 py-3 hover:bg-gray-50 cursor-pointer transition-colors duration-150',
                  !notification.isRead && 'bg-blue-50 border-l-2 border-l-blue-500'
                )}
                onClick={() => handleNotificationClick(notification)}
                role="menuitem"
                tabIndex={0}
                onKeyDown={(e) => {
                  if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    handleNotificationClick(notification);
                  }
                }}
                aria-label={t('bell.notificationItemAriaLabel', { subject: notification.subject })}
              >
                <div className="flex items-start justify-between">
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center space-x-2">
                      <h4 className={classNames(
                        'text-sm font-medium truncate',
                        notification.isRead ? 'text-gray-900' : 'text-gray-900 font-semibold'
                      )}>
                        {notification.subject}
                      </h4>
                      {!notification.isRead && (
                        <span className="flex-shrink-0 w-2 h-2 bg-blue-600 rounded-full"></span>
                      )}
                    </div>
                    <p className="text-xs text-gray-600 mt-1 line-clamp-2">
                      {notification.message}
                    </p>
                    <p className="text-xs text-gray-500 mt-1">
                      {new Date(notification.createdAt).toLocaleDateString()}
                    </p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Footer */}
      <div className="px-4 py-3 border-t border-gray-200 bg-gray-50 rounded-b-lg">
        <div className="flex items-center justify-between">
          {hasMore && (
            <p className="text-xs text-gray-600">
              {t('bell.moreNotifications', { count: notifications.length - maxItems })}
            </p>
          )}
          <button
            onClick={handleViewAllClick}
            className="text-xs text-blue-600 hover:text-blue-800 font-medium ml-auto"
          >
            {t('bell.viewAll')}
          </button>
        </div>
      </div>
    </div>
  );
});

NotificationDropdown.displayName = 'NotificationDropdown';

// ==================== MAIN COMPONENT ====================

/**
 * NotificationBell - Bell icon with unread count and dropdown
 */
export const NotificationBell: React.FC<NotificationBellProps> = React.memo(({
  className,
  size = 'md',
  showDropdown = true,
  onNotificationClick,
  onViewAllClick,
  maxDropdownItems = 5
}) => {
  const { t } = useTranslation('notifications');
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);

  // Notification hooks
  const {
    notifications,
    unreadCount,
    markAllAsRead,
    isMarkingAllAsRead
  } = useNotifications();

  // Size configurations
  const sizeConfig = {
    sm: {
      icon: 'h-5 w-5',
      badge: 'h-4 w-4 text-xs',
      container: 'p-1'
    },
    md: {
      icon: 'h-6 w-6',
      badge: 'h-5 w-5 text-xs',
      container: 'p-2'
    },
    lg: {
      icon: 'h-7 w-7',
      badge: 'h-6 w-6 text-sm',
      container: 'p-2'
    }
  };

  const config = sizeConfig[size];

  // Handlers
  const handleBellClick = useCallback(() => {
    if (showDropdown) {
      setIsDropdownOpen(prev => !prev);
    } else {
      onViewAllClick?.();
    }
  }, [showDropdown, onViewAllClick]);

  const handleCloseDropdown = useCallback(() => {
    setIsDropdownOpen(false);
  }, []);

  const handleNotificationClick = useCallback((notification: NotificationDto) => {
    onNotificationClick?.(notification);
  }, [onNotificationClick]);

  const handleViewAllClick = useCallback(() => {
    onViewAllClick?.();
  }, [onViewAllClick]);

  const handleMarkAllAsRead = useCallback(() => {
    markAllAsRead();
  }, [markAllAsRead]);

  return (
    <div className={classNames('relative', className)}>
      {/* Bell Button */}
      <button
        onClick={handleBellClick}
        className={classNames(
          'relative rounded-full text-gray-600 hover:text-gray-900 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-colors duration-150',
          config.container
        )}
        aria-label={
          unreadCount > 0 
            ? t('bell.bellWithUnreadAriaLabel', { count: unreadCount })
            : t('bell.bellAriaLabel')
        }
        aria-expanded={showDropdown ? isDropdownOpen : undefined}
        aria-haspopup={showDropdown ? 'menu' : undefined}
      >
        {/* Bell Icon */}
        <svg 
          className={classNames(config.icon)} 
          fill="none" 
          stroke="currentColor" 
          viewBox="0 0 24 24"
          aria-hidden="true"
        >
          <path 
            strokeLinecap="round" 
            strokeLinejoin="round" 
            strokeWidth={2} 
            d="M15 17h5l-5 5v-5zM4 6h16v12H4V6z M7 7h.01M17 7h.01M7 11h.01M17 11h.01" 
          />
        </svg>

        {/* Unread Count Badge */}
        {unreadCount > 0 && (
          <span
            className={classNames(
              'absolute -top-1 -right-1 flex items-center justify-center min-w-0 bg-red-600 text-white font-bold rounded-full',
              config.badge
            )}
            aria-label={t('bell.unreadCountAriaLabel', { count: unreadCount })}
          >
            {unreadCount > 99 ? '99+' : unreadCount}
          </span>
        )}
      </button>

      {/* Dropdown Menu */}
      {showDropdown && (
        <NotificationDropdown
          notifications={notifications}
          unreadCount={unreadCount}
          onNotificationClick={handleNotificationClick}
          onViewAllClick={handleViewAllClick}
          onMarkAllAsRead={handleMarkAllAsRead}
          isMarkingAllAsRead={isMarkingAllAsRead}
          isOpen={isDropdownOpen}
          onClose={handleCloseDropdown}
          maxItems={maxDropdownItems}
        />
      )}
    </div>
  );
});

NotificationBell.displayName = 'NotificationBell';

export default NotificationBell;
