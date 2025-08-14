/**
 * NotificationList Component
 * 
 * Displays a list of user notifications with actions to mark as read,
 * delete, and load more notifications. Supports infinite scrolling.
 */

import React, { memo, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { useNotifications } from '../hooks/useNotifications';
import { Notification, NotificationPriority, NotificationType } from '../types/notification.types';

interface NotificationListProps {
  userId?: string;
  pageSize?: number;
  className?: string;
  onNotificationClick?: (notification: Notification) => void;
}

/**
 * Individual notification item component
 */
const NotificationItem: React.FC<{
  notification: Notification;
  onMarkAsRead: (id: string) => void;
  onDelete: (id: string) => void;
  onClick?: () => void;
}> = memo(({ notification, onMarkAsRead, onDelete, onClick }) => {
  const { t } = useTranslation();

  const getPriorityColor = (priority: NotificationPriority) => {
    switch (priority) {
      case NotificationPriority.Critical:
        return 'border-l-red-500 bg-red-50 dark:bg-red-900/20';
      case NotificationPriority.High:
        return 'border-l-orange-500 bg-orange-50 dark:bg-orange-900/20';
      case NotificationPriority.Normal:
        return 'border-l-blue-500 bg-blue-50 dark:bg-blue-900/20';
      case NotificationPriority.Low:
        return 'border-l-gray-500 bg-gray-50 dark:bg-gray-900/20';
      default:
        return 'border-l-gray-500 bg-gray-50 dark:bg-gray-900/20';
    }
  };

  const getTypeIcon = (type: NotificationType) => {
    const iconClass = "w-5 h-5";
    
    switch (type) {
      case NotificationType.Success:
        return (
          <svg className={`${iconClass} text-green-500`} fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
          </svg>
        );
      case NotificationType.Error:
        return (
          <svg className={`${iconClass} text-red-500`} fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
          </svg>
        );
      case NotificationType.Warning:
        return (
          <svg className={`${iconClass} text-yellow-500`} fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
          </svg>
        );
      case NotificationType.Info:
        return (
          <svg className={`${iconClass} text-blue-500`} fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
          </svg>
        );
      default:
        return (
          <svg className={`${iconClass} text-gray-500`} fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
          </svg>
        );
    }
  };

  const formatDate = (date: Date) => {
    return new Intl.DateTimeFormat('en-US', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(new Date(date));
  };

  return (
    <div
      className={`
        border-l-4 rounded-lg p-4 mb-3 transition-all duration-200
        hover:shadow-md cursor-pointer
        ${getPriorityColor(notification.priority)}
        ${notification.isRead ? 'opacity-75' : ''}
      `}
      onClick={onClick}
    >
      <div className="flex items-start justify-between">
        <div className="flex items-start space-x-3 flex-1">
          {/* Type Icon */}
          <div className="flex-shrink-0 mt-1">
            {getTypeIcon(notification.type)}
          </div>

          {/* Content */}
          <div className="flex-1 min-w-0">
            <div className="flex items-center space-x-2">
              <h4 className={`text-sm font-medium ${notification.isRead ? 'text-gray-600' : 'text-gray-900 dark:text-white'}`}>
                {notification.title}
              </h4>
              {!notification.isRead && (
                <span className="h-2 w-2 bg-blue-500 rounded-full"></span>
              )}
            </div>
            
            <p className="text-sm text-gray-600 dark:text-gray-400 mt-1">
              {notification.message}
            </p>
            
            <div className="flex items-center space-x-4 mt-2 text-xs text-gray-500">
              <span>{formatDate(notification.createdAt)}</span>
              <span className="capitalize">{notification.priority.toLowerCase()}</span>
              {notification.readAt && (
                <span>{t('notifications.read_at', { date: formatDate(notification.readAt) })}</span>
              )}
            </div>
          </div>
        </div>

        {/* Actions */}
        <div className="flex items-center space-x-2 ml-4">
          {!notification.isRead && (
            <button
              onClick={(e) => {
                e.stopPropagation();
                onMarkAsRead(notification.id);
              }}
              className="text-blue-600 hover:text-blue-800 text-xs font-medium"
              title={t('notifications.mark_as_read')}
            >
              {t('notifications.mark_read')}
            </button>
          )}
          
          <button
            onClick={(e) => {
              e.stopPropagation();
              onDelete(notification.id);
            }}
            className="text-red-600 hover:text-red-800 text-xs font-medium"
            title={t('notifications.delete')}
          >
            {t('notifications.delete')}
          </button>
        </div>
      </div>
    </div>
  );
});

/**
 * Main NotificationList component
 */
const NotificationList: React.FC<NotificationListProps> = memo(({
  userId,
  pageSize = 10,
  className = '',
  onNotificationClick,
}) => {
  const { t } = useTranslation();
  
  const {
    notifications,
    unreadCount,
    isLoading,
    error,
    hasNextPage,
    markAsRead,
    markAllAsRead,
    deleteNotification,
    loadMore,
    refetch,
  } = useNotifications({ userId, pageSize });

  const handleNotificationClick = (notification: Notification) => {
    if (!notification.isRead) {
      markAsRead(notification.id);
    }
    onNotificationClick?.(notification);
  };

  const sortedNotifications = useMemo(() => {
    return [...notifications].sort((a, b) => {
      // Unread first, then by creation date descending
      if (a.isRead !== b.isRead) {
        return a.isRead ? 1 : -1;
      }
      return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
    });
  }, [notifications]);

  if (isLoading && notifications.length === 0) {
    return (
      <div className={`flex items-center justify-center p-8 ${className}`}>
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-500 mx-auto mb-4"></div>
          <p className="text-gray-600 dark:text-gray-400">{t('notifications.loading')}</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={`p-4 ${className}`}>
        <div className="bg-red-50 border border-red-200 rounded-lg p-4">
          <p className="text-red-800 text-sm">{t('notifications.error_loading')}</p>
          <button
            onClick={refetch}
            className="mt-2 text-red-600 hover:text-red-800 text-sm font-medium"
          >
            {t('notifications.retry')}
          </button>
        </div>
      </div>
    );
  }

  if (notifications.length === 0) {
    return (
      <div className={`flex items-center justify-center p-8 ${className}`}>
        <div className="text-center">
          <svg className="w-12 h-12 text-gray-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-5-5-5 5h5zm0 0v-2a6 6 0 00-6-6H7a6 6 0 00-6 6v2"/>
          </svg>
          <p className="text-gray-600 dark:text-gray-400">{t('notifications.no_notifications')}</p>
        </div>
      </div>
    );
  }

  return (
    <div className={className}>
      {/* Header */}
      <div className="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-700">
        <h3 className="text-lg font-semibold text-gray-900 dark:text-white">
          {t('notifications.title')}
        </h3>
        
        <div className="flex items-center space-x-3">
          {unreadCount > 0 && (
            <span className="bg-blue-100 text-blue-800 text-xs font-medium px-2.5 py-0.5 rounded-full">
              {t('notifications.unread_count', { count: unreadCount })}
            </span>
          )}
          
          {unreadCount > 0 && (
            <button
              onClick={markAllAsRead}
              className="text-blue-600 hover:text-blue-800 text-sm font-medium"
            >
              {t('notifications.mark_all_read')}
            </button>
          )}
        </div>
      </div>

      {/* Notifications List */}
      <div className="p-4 max-h-96 overflow-y-auto">
        {sortedNotifications.map((notification) => (
          <NotificationItem
            key={notification.id}
            notification={notification}
            onMarkAsRead={markAsRead}
            onDelete={deleteNotification}
            onClick={() => handleNotificationClick(notification)}
          />
        ))}

        {/* Load More Button */}
        {hasNextPage && (
          <div className="flex justify-center mt-4">
            <button
              onClick={loadMore}
              disabled={isLoading}
              className="bg-primary-600 text-white px-4 py-2 rounded-lg hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading ? t('notifications.loading') : t('notifications.load_more')}
            </button>
          </div>
        )}
      </div>
    </div>
  );
});

NotificationItem.displayName = 'NotificationItem';
NotificationList.displayName = 'NotificationList';

export default NotificationList;
