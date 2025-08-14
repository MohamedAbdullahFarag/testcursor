/**
 * NotificationCenter component - Main notification management center
 * Provides central hub for all notification interactions including list, filters, and actions
 * Following established patterns from shared components with proper accessibility
 */

import React, { useState, useCallback, useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';

import { useNotifications, useNotificationPreferences } from '../hooks/useNotifications';
import { NotificationFilterDto, NotificationType, NotificationPriority } from '../types/notification.types';

// Component interfaces
interface NotificationCenterProps {
  className?: string;
  initialFilter?: NotificationFilterDto;
  showPreferences?: boolean;
  maxHeight?: string;
  onNotificationClick?: (notificationId: string) => void;
}

interface FilterControlsProps {
  filter: NotificationFilterDto;
  onFilterChange: (filter: Partial<NotificationFilterDto>) => void;
  unreadCount: number;
}

interface NotificationHeaderProps {
  unreadCount: number;
  onMarkAllAsRead: () => void;
  onRefresh: () => void;
  isMarkingAllAsRead: boolean;
  isRefreshing: boolean;
}

// ==================== SUB-COMPONENTS ====================

/**
 * Filter controls for notifications
 */
const FilterControls: React.FC<FilterControlsProps> = React.memo(({
  filter,
  onFilterChange,
  unreadCount
}) => {
  const { t } = useTranslation('notifications');

  const handleUnreadToggle = useCallback(() => {
    onFilterChange({ unreadOnly: !filter.unreadOnly });
  }, [filter.unreadOnly, onFilterChange]);

  const handleTypeFilter = useCallback((type: NotificationType | '') => {
    onFilterChange({ notificationType: type || undefined });
  }, [onFilterChange]);

  const handlePriorityFilter = useCallback((priority: NotificationPriority | '') => {
    onFilterChange({ priority: priority || undefined });
  }, [onFilterChange]);

  const handleSearchChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    onFilterChange({ searchTerm: value || undefined });
  }, [onFilterChange]);

  return (
    <div className="space-y-4 p-4 border-b border-gray-200 bg-gray-50">
      {/* Search Input */}
      <div className="relative">
        <input
          type="text"
          placeholder={t('filters.searchPlaceholder')}
          value={filter.searchTerm || ''}
          onChange={handleSearchChange}
          className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          aria-label={t('filters.searchAriaLabel')}
        />
        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
          <svg className="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
        </div>
      </div>

      {/* Filter Controls Row */}
      <div className="flex flex-wrap gap-4 items-center">
        {/* Unread Only Toggle */}
        <label className="flex items-center space-x-2 cursor-pointer">
          <input
            type="checkbox"
            checked={filter.unreadOnly || false}
            onChange={handleUnreadToggle}
            className="w-4 h-4 text-blue-600 bg-gray-100 border-gray-300 rounded focus:ring-blue-500"
            aria-describedby="unread-count"
          />
          <span className="text-sm font-medium text-gray-700">
            {t('filters.unreadOnly')}
          </span>
          {unreadCount > 0 && (
            <span 
              id="unread-count"
              className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800"
            >
              {unreadCount}
            </span>
          )}
        </label>

        {/* Type Filter */}
        <div className="flex items-center space-x-2">
          <label htmlFor="type-filter" className="text-sm font-medium text-gray-700">
            {t('filters.type')}:
          </label>
          <select
            id="type-filter"
            value={filter.notificationType || ''}
            onChange={(e) => handleTypeFilter(e.target.value as NotificationType | '')}
            className="border border-gray-300 rounded-md px-2 py-1 text-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option value="">{t('filters.allTypes')}</option>
            <option value={NotificationType.ExamReminder}>{t('types.examReminder')}</option>
            <option value={NotificationType.ExamStart}>{t('types.examStart')}</option>
            <option value={NotificationType.ExamEnd}>{t('types.examEnd')}</option>
            <option value={NotificationType.GradingComplete}>{t('types.gradingComplete')}</option>
            <option value={NotificationType.DeadlineReminder}>{t('types.deadlineReminder')}</option>
            <option value={NotificationType.SystemAlert}>{t('types.systemAlert')}</option>
            <option value={NotificationType.Welcome}>{t('types.welcome')}</option>
            <option value={NotificationType.PasswordReset}>{t('types.passwordReset')}</option>
          </select>
        </div>

        {/* Priority Filter */}
        <div className="flex items-center space-x-2">
          <label htmlFor="priority-filter" className="text-sm font-medium text-gray-700">
            {t('filters.priority')}:
          </label>
          <select
            id="priority-filter"
            value={filter.priority || ''}
            onChange={(e) => handlePriorityFilter(e.target.value as NotificationPriority | '')}
            className="border border-gray-300 rounded-md px-2 py-1 text-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option value="">{t('filters.allPriorities')}</option>
            <option value={NotificationPriority.Low}>{t('priorities.low')}</option>
            <option value={NotificationPriority.Medium}>{t('priorities.medium')}</option>
            <option value={NotificationPriority.High}>{t('priorities.high')}</option>
            <option value={NotificationPriority.Critical}>{t('priorities.critical')}</option>
          </select>
        </div>
      </div>
    </div>
  );
});

FilterControls.displayName = 'FilterControls';

/**
 * Notification header with action buttons
 */
const NotificationHeader: React.FC<NotificationHeaderProps> = React.memo(({
  unreadCount,
  onMarkAllAsRead,
  onRefresh,
  isMarkingAllAsRead,
  isRefreshing
}) => {
  const { t } = useTranslation('notifications');

  return (
    <div className="flex items-center justify-between p-4 border-b border-gray-200 bg-white">
      <div className="flex items-center space-x-3">
        <h2 className="text-lg font-semibold text-gray-900">
          {t('center.title')}
        </h2>
        {unreadCount > 0 && (
          <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-blue-100 text-blue-800">
            {t('center.unreadCount', { count: unreadCount })}
          </span>
        )}
      </div>

      <div className="flex items-center space-x-2">
        {/* Refresh Button */}
        <button
          onClick={onRefresh}
          disabled={isRefreshing}
          className="inline-flex items-center px-3 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
          aria-label={t('center.refreshAriaLabel')}
        >
          <svg 
            className={classNames('h-4 w-4 mr-1', { 'animate-spin': isRefreshing })} 
            fill="none" 
            stroke="currentColor" 
            viewBox="0 0 24 24"
          >
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          {t('center.refresh')}
        </button>

        {/* Mark All as Read Button */}
        {unreadCount > 0 && (
          <button
            onClick={onMarkAllAsRead}
            disabled={isMarkingAllAsRead}
            className="inline-flex items-center px-3 py-2 border border-transparent rounded-md text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
            aria-label={t('center.markAllReadAriaLabel')}
          >
            {isMarkingAllAsRead ? (
              <svg className="animate-spin h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            ) : (
              <svg className="h-4 w-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
              </svg>
            )}
            {t('center.markAllRead')}
          </button>
        )}
      </div>
    </div>
  );
});

NotificationHeader.displayName = 'NotificationHeader';

// ==================== MAIN COMPONENT ====================

/**
 * NotificationCenter - Central notification management component
 */
export const NotificationCenter: React.FC<NotificationCenterProps> = React.memo(({
  className,
  initialFilter = {},
  showPreferences = false,
  maxHeight = '600px',
  onNotificationClick
}) => {
  const { t } = useTranslation('notifications');
  
  // State for active tab
  const [activeTab, setActiveTab] = useState<'notifications' | 'preferences'>('notifications');

  // Hooks for data management
  const {
    notifications,
    unreadCount,
    totalCount,
    currentPage,
    pageSize,
    filter,
    isLoading,
    isFetching,
    error,
    markAsRead,
    markAllAsRead,
    deleteNotification,
    setFilter,
    setPage,
    refresh,
    isMarkingAllAsRead,
    isDeletingNotification
  } = useNotifications(undefined, initialFilter);

  const {
    isLoading: preferencesLoading,
    error: preferencesError
  } = useNotificationPreferences();

  // Memoized handlers
  const handleFilterChange = useCallback((newFilter: Partial<NotificationFilterDto>) => {
    setFilter(newFilter);
  }, [setFilter]);

  const handleNotificationClick = useCallback((notificationId: string) => {
    // Mark as read when clicked
    markAsRead(notificationId);
    // Call external handler if provided
    onNotificationClick?.(notificationId);
  }, [markAsRead, onNotificationClick]);

  const handleMarkAllAsRead = useCallback(() => {
    markAllAsRead();
  }, [markAllAsRead]);

  const handleRefresh = useCallback(() => {
    refresh();
  }, [refresh]);

  const handlePageChange = useCallback((page: number) => {
    setPage(page);
  }, [setPage]);

  // Computed values
  const hasNextPage = useMemo(() => {
    return currentPage * pageSize < totalCount;
  }, [currentPage, pageSize, totalCount]);

  const showLoadMore = useMemo(() => {
    return hasNextPage && !isLoading && !isFetching;
  }, [hasNextPage, isLoading, isFetching]);

  // Loading and error states
  if (error) {
    return (
      <div className={classNames('bg-white rounded-lg shadow-sm border border-gray-200', className)}>
        <div className="p-6 text-center">
          <div className="text-red-600 mb-2">
            <svg className="h-8 w-8 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <h3 className="text-lg font-medium text-gray-900 mb-1">
            {t('center.errorTitle')}
          </h3>
          <p className="text-gray-600 mb-4">{error}</p>
          <button
            onClick={handleRefresh}
            className="inline-flex items-center px-4 py-2 border border-transparent rounded-md text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            {t('center.tryAgain')}
          </button>
        </div>
      </div>
    );
  }

  return (
    <div 
      className={classNames('bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden', className)}
      style={{ maxHeight }}
    >
      {/* Tab Navigation (if preferences are shown) */}
      {showPreferences && (
        <div className="border-b border-gray-200">
          <nav className="-mb-px flex" aria-label={t('center.tabNavigation')}>
            <button
              onClick={() => setActiveTab('notifications')}
              className={classNames(
                'w-1/2 py-2 px-1 text-center border-b-2 font-medium text-sm focus:outline-none focus:ring-2 focus:ring-blue-500',
                activeTab === 'notifications'
                  ? 'border-blue-500 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              )}
              aria-current={activeTab === 'notifications' ? 'page' : undefined}
            >
              {t('center.notificationsTab')}
            </button>
            <button
              onClick={() => setActiveTab('preferences')}
              className={classNames(
                'w-1/2 py-2 px-1 text-center border-b-2 font-medium text-sm focus:outline-none focus:ring-2 focus:ring-blue-500',
                activeTab === 'preferences'
                  ? 'border-blue-500 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              )}
              aria-current={activeTab === 'preferences' ? 'page' : undefined}
            >
              {t('center.preferencesTab')}
            </button>
          </nav>
        </div>
      )}

      {/* Notifications Tab Content */}
      {(!showPreferences || activeTab === 'notifications') && (
        <>
          <NotificationHeader
            unreadCount={unreadCount}
            onMarkAllAsRead={handleMarkAllAsRead}
            onRefresh={handleRefresh}
            isMarkingAllAsRead={isMarkingAllAsRead}
            isRefreshing={isFetching}
          />

          <FilterControls
            filter={filter}
            onFilterChange={handleFilterChange}
            unreadCount={unreadCount}
          />

          {/* Notification List Container */}
          <div className="overflow-y-auto" style={{ maxHeight: '400px' }}>
            {isLoading ? (
              <div className="p-6 text-center">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto mb-2"></div>
                <p className="text-gray-600">{t('center.loading')}</p>
              </div>
            ) : notifications.length === 0 ? (
              <div className="p-6 text-center">
                <div className="text-gray-400 mb-2">
                  <svg className="h-12 w-12 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-5 5v-5zM4 6h16v12H4V6z" />
                  </svg>
                </div>
                <h3 className="text-lg font-medium text-gray-900 mb-1">
                  {filter.unreadOnly ? t('center.noUnreadNotifications') : t('center.noNotifications')}
                </h3>
                <p className="text-gray-600">
                  {filter.unreadOnly ? t('center.noUnreadDescription') : t('center.noNotificationsDescription')}
                </p>
              </div>
            ) : (
              <div className="divide-y divide-gray-200">
                {notifications.map((notification) => (
                  <div
                    key={notification.id}
                    className={classNames(
                      'p-4 hover:bg-gray-50 cursor-pointer transition-colors duration-150',
                      !notification.isRead && 'bg-blue-50 border-l-4 border-l-blue-500'
                    )}
                    onClick={() => handleNotificationClick(notification.id)}
                    role="button"
                    tabIndex={0}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter' || e.key === ' ') {
                        e.preventDefault();
                        handleNotificationClick(notification.id);
                      }
                    }}
                    aria-label={t('center.notificationAriaLabel', { subject: notification.subject })}
                  >
                    <div className="flex items-start justify-between">
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center space-x-2 mb-1">
                          <h4 className={classNames(
                            'text-sm font-medium truncate',
                            notification.isRead ? 'text-gray-900' : 'text-gray-900 font-semibold'
                          )}>
                            {notification.subject}
                          </h4>
                          {!notification.isRead && (
                            <span className="flex-shrink-0 w-2 h-2 bg-blue-600 rounded-full" aria-label={t('center.unreadIndicator')}></span>
                          )}
                        </div>
                        <p className="text-sm text-gray-600 line-clamp-2">
                          {notification.message}
                        </p>
                        <div className="flex items-center space-x-4 mt-2 text-xs text-gray-500">
                          <span>{new Date(notification.createdAt).toLocaleDateString()}</span>
                          <span className={classNames(
                            'px-2 py-1 rounded-full text-xs font-medium',
                            notification.priority === NotificationPriority.Critical && 'bg-red-100 text-red-800',
                            notification.priority === NotificationPriority.High && 'bg-orange-100 text-orange-800',
                            notification.priority === NotificationPriority.Medium && 'bg-blue-100 text-blue-800',
                            notification.priority === NotificationPriority.Low && 'bg-gray-100 text-gray-800'
                          )}>
                            {t(`priorities.${notification.priority.toLowerCase()}`)}
                          </span>
                        </div>
                      </div>
                      <div className="flex items-center space-x-2 ml-4">
                        {/* Delete Button */}
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            deleteNotification(notification.id);
                          }}
                          disabled={isDeletingNotification}
                          className="text-gray-400 hover:text-red-600 transition-colors duration-150 focus:outline-none focus:ring-2 focus:ring-red-500 rounded"
                          aria-label={t('center.deleteNotificationAriaLabel')}
                        >
                          <svg className="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                          </svg>
                        </button>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}

            {/* Load More Button */}
            {showLoadMore && (
              <div className="p-4 border-t border-gray-200 text-center">
                <button
                  onClick={() => handlePageChange(currentPage + 1)}
                  className="inline-flex items-center px-4 py-2 border border-transparent rounded-md text-sm font-medium text-blue-600 bg-blue-50 hover:bg-blue-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                >
                  {t('center.loadMore')} ({totalCount - notifications.length} {t('center.remaining')})
                </button>
              </div>
            )}
          </div>
        </>
      )}

      {/* Preferences Tab Content */}
      {showPreferences && activeTab === 'preferences' && (
        <div className="p-6">
          {preferencesLoading ? (
            <div className="text-center">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto mb-2"></div>
              <p className="text-gray-600">{t('preferences.loading')}</p>
            </div>
          ) : preferencesError ? (
            <div className="text-center text-red-600">
              <p>{preferencesError}</p>
            </div>
          ) : (
            <div>
              <h3 className="text-lg font-medium text-gray-900 mb-4">
                {t('preferences.title')}
              </h3>
              <p className="text-gray-600 mb-4">
                {t('preferences.description')}
              </p>
              {/* TODO: Add NotificationPreferences component here */}
              <div className="bg-gray-50 rounded-lg p-4 text-center text-gray-600">
                {t('preferences.comingSoon')}
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
});

NotificationCenter.displayName = 'NotificationCenter';

export default NotificationCenter;
