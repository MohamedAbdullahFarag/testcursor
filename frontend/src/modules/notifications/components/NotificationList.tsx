/**
 * NotificationList component - Reusable notification list with infinite scroll
 * Displays notifications with customizable layout and actions
 * Following established patterns with accessibility and performance optimization
 */

import React, { useMemo, useCallback, useEffect, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';

import { NotificationDto } from '../types/notification.types';

// Temporary NotificationItem component - replace with actual implementation
const NotificationItem: React.FC<{
  notification: NotificationDto;
  onClick?: (notification: NotificationDto) => void;
  onMarkAsRead?: (notificationId: string) => void;
  onMarkAsUnread?: (notificationId: string) => void;
  onDelete?: (notificationId: string) => void;
  showActions?: boolean;
  showTimestamp?: boolean;
  compact?: boolean;
  className?: string;
}> = ({ notification, onClick, className }) => (
  <div 
    className={classNames('p-4 border-b border-gray-100', className)}
    onClick={() => onClick?.(notification)}
  >
    <h4 className="font-medium text-gray-900">{notification.subject}</h4>
    <p className="text-sm text-gray-600 mt-1">{notification.message}</p>
    <p className="text-xs text-gray-500 mt-1">
      {new Date(notification.createdAt).toLocaleDateString()}
    </p>
  </div>
);

// Placeholder components - these should be replaced with actual shared components
const LoadingSpinner: React.FC<{ size?: 'sm' | 'md' | 'lg'; className?: string }> = ({ 
  size = 'md', 
  className 
}) => (
  <div 
    className={classNames(
      'animate-spin rounded-full border-2 border-gray-300 border-t-blue-600',
      {
        'h-4 w-4': size === 'sm',
        'h-6 w-6': size === 'md',
        'h-8 w-8': size === 'lg'
      },
      className
    )}
  />
);

const Button: React.FC<{
  children: React.ReactNode;
  variant?: 'primary' | 'outline';
  onClick?: () => void;
  disabled?: boolean;
  className?: string;
}> = ({ 
  children, 
  variant = 'primary', 
  onClick, 
  disabled = false, 
  className 
}) => (
  <button
    onClick={onClick}
    disabled={disabled}
    className={classNames(
      'px-4 py-2 rounded-md font-medium transition-colors duration-150',
      {
        'bg-blue-600 text-white hover:bg-blue-700 disabled:bg-blue-400': variant === 'primary',
        'border border-gray-300 text-gray-700 bg-white hover:bg-gray-50 disabled:bg-gray-100': variant === 'outline'
      },
      'disabled:cursor-not-allowed',
      className
    )}
  >
    {children}
  </button>
);

// Component interfaces
interface NotificationListProps {
  notifications: NotificationDto[];
  isLoading?: boolean;
  hasNextPage?: boolean;
  isLoadingMore?: boolean;
  onLoadMore?: () => void;
  onNotificationClick?: (notification: NotificationDto) => void;
  onMarkAsRead?: (notificationId: string) => void;
  onMarkAsUnread?: (notificationId: string) => void;
  onDelete?: (notificationId: string) => void;
  showActions?: boolean;
  showTimestamp?: boolean;
  compact?: boolean;
  virtualizeThreshold?: number;
  className?: string;
  emptyStateMessage?: string;
  errorMessage?: string;
  onRetry?: () => void;
}

interface VirtualizedListProps {
  notifications: NotificationDto[];
  itemHeight: number;
  containerHeight: number;
  onNotificationClick?: (notification: NotificationDto) => void;
  onMarkAsRead?: (notificationId: string) => void;
  onMarkAsUnread?: (notificationId: string) => void;
  onDelete?: (notificationId: string) => void;
  showActions?: boolean;
  showTimestamp?: boolean;
  compact?: boolean;
}

// ==================== SUB-COMPONENTS ====================

/**
 * Virtualized list for large notification sets
 */
const VirtualizedNotificationList: React.FC<VirtualizedListProps> = React.memo(({
  notifications,
  itemHeight,
  containerHeight,
  onNotificationClick,
  onMarkAsRead,
  onMarkAsUnread,
  onDelete,
  showActions = true,
  showTimestamp = true,
  compact = false
}) => {
  const [scrollTop, setScrollTop] = React.useState(0);
  const listRef = useRef<HTMLDivElement>(null);

  // Calculate visible range
  const visibleRange = useMemo(() => {
    const startIndex = Math.floor(scrollTop / itemHeight);
    const visibleCount = Math.ceil(containerHeight / itemHeight) + 1; // +1 for buffer
    const endIndex = Math.min(startIndex + visibleCount, notifications.length);
    
    return { startIndex, endIndex };
  }, [scrollTop, itemHeight, containerHeight, notifications.length]);

  // Handle scroll
  const handleScroll = useCallback((e: React.UIEvent<HTMLDivElement>) => {
    setScrollTop(e.currentTarget.scrollTop);
  }, []);

  // Visible notifications
  const visibleNotifications = useMemo(() => {
    return notifications.slice(visibleRange.startIndex, visibleRange.endIndex);
  }, [notifications, visibleRange]);

  return (
    <div
      ref={listRef}
      className="overflow-auto"
      style={{ height: containerHeight }}
      onScroll={handleScroll}
      role="list"
      aria-label="Notifications list"
    >
      {/* Virtual spacer before visible items */}
      <div style={{ height: visibleRange.startIndex * itemHeight }} />
      
      {/* Visible notifications */}
      {visibleNotifications.map((notification) => (
        <div
          key={notification.id}
          style={{ height: itemHeight }}
          className="flex items-stretch"
        >
          <NotificationItem
            notification={notification}
            onClick={onNotificationClick}
            onMarkAsRead={onMarkAsRead}
            onMarkAsUnread={onMarkAsUnread}
            onDelete={onDelete}
            showActions={showActions}
            showTimestamp={showTimestamp}
            compact={compact}
            className="flex-1"
          />
        </div>
      ))}
      
      {/* Virtual spacer after visible items */}
      <div 
        style={{ 
          height: (notifications.length - visibleRange.endIndex) * itemHeight 
        }} 
      />
    </div>
  );
});

VirtualizedNotificationList.displayName = 'VirtualizedNotificationList';

/**
 * Empty state component
 */
const EmptyState: React.FC<{
  message?: string;
  isError?: boolean;
  onRetry?: () => void;
}> = React.memo(({ message, isError = false, onRetry }) => {
  const { t } = useTranslation('notifications');

  return (
    <div className="flex flex-col items-center justify-center py-12 px-4">
      <div className="text-gray-400 mb-4">
        {isError ? (
          <svg className="h-12 w-12" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path 
              strokeLinecap="round" 
              strokeLinejoin="round" 
              strokeWidth={2} 
              d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" 
            />
          </svg>
        ) : (
          <svg className="h-12 w-12" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path 
              strokeLinecap="round" 
              strokeLinejoin="round" 
              strokeWidth={2} 
              d="M15 17h5l-5 5v-5zM4 6h16v12H4V6z" 
            />
          </svg>
        )}
      </div>
      
      <h3 className="text-lg font-medium text-gray-900 mb-2">
        {isError ? t('empty.error.title') : t('empty.title')}
      </h3>
      
      <p className="text-gray-600 text-center mb-4 max-w-md">
        {message || (isError ? t('empty.error.subtitle') : t('empty.subtitle'))}
      </p>
      
      {isError && onRetry && (
        <Button variant="outline" onClick={onRetry}>
          {t('common.retry')}
        </Button>
      )}
    </div>
  );
});

EmptyState.displayName = 'EmptyState';

/**
 * Load more button component
 */
const LoadMoreButton: React.FC<{
  onLoadMore: () => void;
  isLoading: boolean;
  hasMore: boolean;
}> = React.memo(({ onLoadMore, isLoading, hasMore }) => {
  const { t } = useTranslation('notifications');

  if (!hasMore) return null;

  return (
    <div className="flex justify-center py-4">
      <Button
        variant="outline"
        onClick={onLoadMore}
        disabled={isLoading}
        className="min-w-32"
      >
        {isLoading ? (
          <>
            <LoadingSpinner size="sm" className="mr-2" />
            {t('center.loadingMore')}
          </>
        ) : (
          t('center.loadMoreButton')
        )}
      </Button>
    </div>
  );
});

LoadMoreButton.displayName = 'LoadMoreButton';

// ==================== MAIN COMPONENT ====================

/**
 * NotificationList - Reusable notification list component
 */
export const NotificationList: React.FC<NotificationListProps> = React.memo(({
  notifications,
  isLoading = false,
  hasNextPage = false,
  isLoadingMore = false,
  onLoadMore,
  onNotificationClick,
  onMarkAsRead,
  onMarkAsUnread,
  onDelete,
  showActions = true,
  showTimestamp = true,
  compact = false,
  virtualizeThreshold = 100,
  className,
  emptyStateMessage,
  errorMessage,
  onRetry
}) => {
  const { t } = useTranslation('notifications');
  const listContainerRef = useRef<HTMLDivElement>(null);

  // Intersection observer for infinite scroll
  const loadMoreRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!onLoadMore || !hasNextPage || isLoadingMore) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          onLoadMore();
        }
      },
      { threshold: 0.1 }
    );

    const currentRef = loadMoreRef.current;
    if (currentRef) {
      observer.observe(currentRef);
    }

    return () => {
      if (currentRef) {
        observer.unobserve(currentRef);
      }
    };
  }, [onLoadMore, hasNextPage, isLoadingMore]);

  // Loading state
  if (isLoading && notifications.length === 0) {
    return (
      <div className={classNames('flex justify-center py-8', className)}>
        <div className="flex items-center space-x-2">
          <LoadingSpinner size="md" />
          <span className="text-gray-600">{t('common.loading')}</span>
        </div>
      </div>
    );
  }

  // Error state
  if (errorMessage && notifications.length === 0) {
    return (
      <div className={className}>
        <EmptyState
          message={errorMessage}
          isError={true}
          onRetry={onRetry}
        />
      </div>
    );
  }

  // Empty state
  if (notifications.length === 0) {
    return (
      <div className={className}>
        <EmptyState message={emptyStateMessage} />
      </div>
    );
  }

  // Determine if we should use virtualization
  const shouldVirtualize = notifications.length > virtualizeThreshold;
  const itemHeight = compact ? 80 : 120;
  const containerHeight = shouldVirtualize ? 600 : undefined;

  return (
    <div className={classNames('bg-white', className)} ref={listContainerRef}>
      {shouldVirtualize ? (
        <VirtualizedNotificationList
          notifications={notifications}
          itemHeight={itemHeight}
          containerHeight={containerHeight!}
          onNotificationClick={onNotificationClick}
          onMarkAsRead={onMarkAsRead}
          onMarkAsUnread={onMarkAsUnread}
          onDelete={onDelete}
          showActions={showActions}
          showTimestamp={showTimestamp}
          compact={compact}
        />
      ) : (
        <div className="divide-y divide-gray-100" role="list" aria-label="Notifications">
          {notifications.map((notification) => (
            <NotificationItem
              key={notification.id}
              notification={notification}
              onClick={onNotificationClick}
              onMarkAsRead={onMarkAsRead}
              onMarkAsUnread={onMarkAsUnread}
              onDelete={onDelete}
              showActions={showActions}
              showTimestamp={showTimestamp}
              compact={compact}
              className="py-4 px-4 hover:bg-gray-50 transition-colors duration-150"
            />
          ))}
        </div>
      )}

      {/* Load more section for non-virtualized lists */}
      {!shouldVirtualize && onLoadMore && (
        <>
          <LoadMoreButton
            onLoadMore={onLoadMore}
            isLoading={isLoadingMore}
            hasMore={hasNextPage}
          />
          {/* Intersection observer target */}
          <div ref={loadMoreRef} className="h-1" />
        </>
      )}

      {/* Loading indicator for loading more */}
      {isLoadingMore && shouldVirtualize && (
        <div className="flex justify-center py-4">
          <div className="flex items-center space-x-2">
            <LoadingSpinner size="sm" />
            <span className="text-sm text-gray-600">{t('center.loadingMore')}</span>
          </div>
        </div>
      )}
    </div>
  );
});

NotificationList.displayName = 'NotificationList';

export default NotificationList;
