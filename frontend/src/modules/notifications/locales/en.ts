/**
 * English translations for notifications module
 * Comprehensive translation keys for all notification components
 */

export const notificationsEn = {
  // Common labels
  common: {
    notification: 'Notification',
    notifications: 'Notifications',
    read: 'Read',
    unread: 'Unread',
    markAsRead: 'Mark as Read',
    markAsUnread: 'Mark as Unread',
    delete: 'Delete',
    close: 'Close',
    refresh: 'Refresh',
    loadMore: 'Load More',
    viewAll: 'View All',
    noNotifications: 'No notifications',
    loading: 'Loading notifications...',
    error: 'Failed to load notifications',
    retry: 'Retry'
  },

  // Notification Bell Component
  bell: {
    bellAriaLabel: 'Notifications',
    bellWithUnreadAriaLabel: 'Notifications - {{count}} unread',
    unreadCountAriaLabel: '{{count}} unread notifications',
    dropdownAriaLabel: 'Notifications menu',
    notificationsTitle: 'Notifications',
    markAllRead: 'Mark all as read',
    markAllReadAriaLabel: 'Mark all notifications as read',
    markingAsRead: 'Marking as read...',
    unreadCountText: '{{count}} unread',
    noNotifications: 'No new notifications',
    moreNotifications: '+{{count}} more',
    viewAll: 'View All',
    notificationItemAriaLabel: 'Notification: {{subject}}'
  },

  // Notification Center Component
  center: {
    title: 'Notification Center',
    subtitle: 'Stay updated with your latest notifications',
    markAllAsRead: 'Mark All as Read',
    markingAllAsRead: 'Marking all as read...',
    refresh: 'Refresh',
    refreshing: 'Refreshing...',
    filters: 'Filters',
    showFilters: 'Show Filters',
    hideFilters: 'Hide Filters',
    clearFilters: 'Clear Filters',
    noNotifications: 'You have no notifications',
    noNotificationsSubtext: 'When you receive notifications, they will appear here',
    loadingMore: 'Loading more notifications...',
    loadMoreButton: 'Load More',
    errorLoading: 'Failed to load notifications',
    errorLoadingMore: 'Failed to load more notifications',
    tryAgain: 'Try Again'
  },

  // Filter Controls
  filters: {
    status: 'Status',
    statusAll: 'All',
    statusRead: 'Read',
    statusUnread: 'Unread',
    type: 'Type',
    typeAll: 'All Types',
    priority: 'Priority',
    priorityAll: 'All Priorities',
    dateRange: 'Date Range',
    dateRangeAll: 'All Time',
    dateRangeToday: 'Today',
    dateRangeWeek: 'This Week',
    dateRangeMonth: 'This Month',
    searchPlaceholder: 'Search notifications...',
    searchClear: 'Clear search',
    resultsCount: '{{count}} notifications found',
    noResults: 'No notifications match your filters',
    resetFilters: 'Reset Filters'
  },

  // Notification Types
  types: {
    ExamReminder: 'Exam Reminder',
    ExamStart: 'Exam Started',
    ExamCompleted: 'Exam Completed',
    GradePublished: 'Grade Published',
    SystemMaintenance: 'System Maintenance',
    AccountUpdate: 'Account Update',
    PasswordChanged: 'Password Changed',
    LoginAlert: 'Login Alert',
    General: 'General',
    Security: 'Security'
  },

  // Priority Levels
  priorities: {
    Low: 'Low',
    Normal: 'Normal',
    High: 'High',
    Critical: 'Critical'
  },

  // Notification Status
  status: {
    sent: 'Sent',
    delivered: 'Delivered',
    read: 'Read',
    failed: 'Failed'
  },

  // Channels
  channels: {
    Email: 'Email',
    SMS: 'SMS',
    Push: 'Push Notification',
    InApp: 'In-App'
  },

  // Actions
  actions: {
    markAsRead: 'Mark as Read',
    markAsUnread: 'Mark as Unread',
    delete: 'Delete',
    archive: 'Archive',
    reply: 'Reply',
    forward: 'Forward',
    report: 'Report',
    block: 'Block',
    mute: 'Mute',
    unmute: 'Unmute'
  },

  // Success Messages
  success: {
    markedAsRead: 'Notification marked as read',
    markedAsUnread: 'Notification marked as unread',
    deleted: 'Notification deleted',
    archived: 'Notification archived',
    allMarkedAsRead: 'All notifications marked as read',
    preferencesUpdated: 'Notification preferences updated'
  },

  // Error Messages
  errors: {
    loadFailed: 'Failed to load notifications',
    markReadFailed: 'Failed to mark notification as read',
    markUnreadFailed: 'Failed to mark notification as unread',
    deleteFailed: 'Failed to delete notification',
    archiveFailed: 'Failed to archive notification',
    markAllReadFailed: 'Failed to mark all notifications as read',
    networkError: 'Network error. Please check your connection.',
    unknownError: 'An unexpected error occurred'
  },

  // Pagination
  pagination: {
    page: 'Page',
    of: 'of',
    showing: 'Showing',
    to: 'to',
    of_total: 'of {{total}} notifications',
    previous: 'Previous',
    next: 'Next',
    first: 'First',
    last: 'Last',
    itemsPerPage: 'Items per page'
  },

  // Empty States
  empty: {
    title: 'No notifications yet',
    subtitle: 'You\'ll see notifications here when you receive them',
    filtered: {
      title: 'No matching notifications',
      subtitle: 'Try adjusting your filters to see more results'
    },
    error: {
      title: 'Unable to load notifications',
      subtitle: 'Please try again later or contact support if the problem persists'
    }
  },

  // Preferences (for future implementation)
  preferences: {
    title: 'Notification Preferences',
    subtitle: 'Choose how you want to receive notifications',
    emailNotifications: 'Email Notifications',
    smsNotifications: 'SMS Notifications',
    pushNotifications: 'Push Notifications',
    inAppNotifications: 'In-App Notifications',
    examReminders: 'Exam Reminders',
    gradeUpdates: 'Grade Updates',
    systemAlerts: 'System Alerts',
    securityAlerts: 'Security Alerts',
    save: 'Save Preferences',
    saving: 'Saving...',
    cancel: 'Cancel'
  },

  // Time formatting
  time: {
    justNow: 'Just now',
    minutesAgo: '{{count}} minute ago',
    minutesAgo_plural: '{{count}} minutes ago',
    hoursAgo: '{{count}} hour ago',
    hoursAgo_plural: '{{count}} hours ago',
    daysAgo: '{{count}} day ago',
    daysAgo_plural: '{{count}} days ago',
    weeksAgo: '{{count}} week ago',
    weeksAgo_plural: '{{count}} weeks ago',
    monthsAgo: '{{count}} month ago',
    monthsAgo_plural: '{{count}} months ago',
    yearsAgo: '{{count}} year ago',
    yearsAgo_plural: '{{count}} years ago'
  }
};

export default notificationsEn;
