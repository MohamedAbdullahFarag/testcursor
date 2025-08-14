/**
 * Example implementation showing how to use the notification system
 * 
 * This file demonstrates the integration of notification components
 * and hooks in a real application scenario.
 */

import React from 'react';
import { useTranslation } from 'react-i18next';

import { useNotifications } from '../hooks/useNotifications';
import NotificationBell from '../components/NotificationBell';
import NotificationList from '../components/NotificationList';
import { NotificationType } from '../types/notification.types';

/**
 * Example header component with notification bell
 */
const ExampleHeader: React.FC = () => {
  const { t } = useTranslation();
  const [showNotifications, setShowNotifications] = React.useState(false);

  const handleBellClick = () => {
    setShowNotifications(!showNotifications);
  };

  return (
    <header className="bg-white shadow-sm border-b border-gray-200 px-4 py-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold text-gray-900">
          {t('app.title')}
        </h1>
        
        <div className="flex items-center space-x-4">
          {/* Notification Bell */}
          <div className="relative">
            <NotificationBell 
              onClick={handleBellClick}
              size="md"
              showCount={true}
              maxCount={99}
            />
            
            {/* Notification Dropdown */}
            {showNotifications && (
              <div className="absolute right-0 mt-2 w-96 bg-white rounded-lg shadow-xl border border-gray-200 z-50">
                <NotificationList 
                  pageSize={5}
                  className="max-h-96"
                  onNotificationClick={(notification) => {
                    console.log('Notification clicked:', notification);
                    setShowNotifications(false);
                  }}
                />
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  );
};

/**
 * Example notification management page
 */
const ExampleNotificationPage: React.FC = () => {
  const { t } = useTranslation();
  const {
    notifications,
    unreadCount,
    isLoading,
    error,
    hasNextPage,
    markAsRead,
    markAllAsRead,
    loadMore,
    refetch,
  } = useNotifications({ pageSize: 10 });

  const [filter, setFilter] = React.useState({
    type: undefined as NotificationType | undefined,
    isRead: undefined as boolean | undefined,
  });

  const handleFilterChange = (newFilter: typeof filter) => {
    setFilter(newFilter);
    // In a real implementation, you would pass this filter to the useNotifications hook
    refetch();
  };

  if (isLoading && notifications.length === 0) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500 mx-auto mb-4"></div>
          <p className="text-gray-600">{t('notifications.loading')}</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-lg p-4">
        <div className="flex items-center">
          <svg className="w-5 h-5 text-red-400 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
          </svg>
          <div>
            <h3 className="text-sm font-medium text-red-800">
              {t('notifications.error_title')}
            </h3>
            <p className="text-sm text-red-700 mt-1">{error}</p>
            <button
              onClick={refetch}
              className="mt-2 bg-red-100 hover:bg-red-200 text-red-800 text-sm font-medium px-3 py-1 rounded"
            >
              {t('notifications.retry')}
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900 mb-2">
          {t('notifications.page_title')}
        </h1>
        <p className="text-gray-600">
          {t('notifications.page_description')}
        </p>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">
          {t('notifications.filters_title')}
        </h2>
        
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {/* Type Filter */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              {t('notifications.filter_type')}
            </label>
            <select
              value={filter.type || ''}
              onChange={(e) => handleFilterChange({
                ...filter,
                type: e.target.value ? e.target.value as NotificationType : undefined
              })}
              className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            >
              <option value="">{t('notifications.filter_all_types')}</option>
              <option value={NotificationType.Info}>{t('notifications.type_info')}</option>
              <option value={NotificationType.Success}>{t('notifications.type_success')}</option>
              <option value={NotificationType.Warning}>{t('notifications.type_warning')}</option>
              <option value={NotificationType.Error}>{t('notifications.type_error')}</option>
            </select>
          </div>

          {/* Read Status Filter */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              {t('notifications.filter_status')}
            </label>
            <select
              value={filter.isRead === undefined ? '' : filter.isRead.toString()}
              onChange={(e) => handleFilterChange({
                ...filter,
                isRead: e.target.value === '' ? undefined : e.target.value === 'true'
              })}
              className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            >
              <option value="">{t('notifications.filter_all_status')}</option>
              <option value="false">{t('notifications.filter_unread')}</option>
              <option value="true">{t('notifications.filter_read')}</option>
            </select>
          </div>

          {/* Actions */}
          <div className="flex items-end space-x-2">
            {unreadCount > 0 && (
              <button
                onClick={markAllAsRead}
                className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm font-medium"
              >
                {t('notifications.mark_all_read')} ({unreadCount})
              </button>
            )}
            <button
              onClick={refetch}
              className="bg-gray-100 hover:bg-gray-200 text-gray-700 px-4 py-2 rounded-lg text-sm font-medium"
            >
              {t('notifications.refresh')}
            </button>
          </div>
        </div>
      </div>

      {/* Notifications List */}
      <div className="bg-white rounded-lg shadow-sm border border-gray-200">
        <NotificationList 
          pageSize={10}
          onNotificationClick={(notification) => {
            console.log('Notification clicked:', notification);
            if (!notification.isRead) {
              markAsRead(notification.id);
            }
          }}
        />
        
        {/* Load More Button */}
        {hasNextPage && (
          <div className="p-4 border-t border-gray-200">
            <button
              onClick={loadMore}
              disabled={isLoading}
              className="w-full bg-gray-50 hover:bg-gray-100 text-gray-700 py-2 rounded-lg text-sm font-medium disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading ? t('notifications.loading') : t('notifications.load_more')}
            </button>
          </div>
        )}
      </div>

      {/* Statistics */}
      <div className="mt-6 grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="bg-blue-50 rounded-lg p-4">
          <div className="flex items-center">
            <svg className="w-8 h-8 text-blue-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
              <path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
            <div>
              <p className="text-2xl font-bold text-blue-700">{notifications.length}</p>
              <p className="text-sm text-blue-600">{t('notifications.total_loaded')}</p>
            </div>
          </div>
        </div>

        <div className="bg-red-50 rounded-lg p-4">
          <div className="flex items-center">
            <svg className="w-8 h-8 text-red-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
              <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
            </svg>
            <div>
              <p className="text-2xl font-bold text-red-700">{unreadCount}</p>
              <p className="text-sm text-red-600">{t('notifications.unread_count')}</p>
            </div>
          </div>
        </div>

        <div className="bg-green-50 rounded-lg p-4">
          <div className="flex items-center">
            <svg className="w-8 h-8 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
              <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
            </svg>
            <div>
              <p className="text-2xl font-bold text-green-700">{notifications.length - unreadCount}</p>
              <p className="text-sm text-green-600">{t('notifications.read_count')}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export { ExampleHeader, ExampleNotificationPage };
