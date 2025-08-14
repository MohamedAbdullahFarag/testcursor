/**
 * Arabic translations for notifications module
 * RTL-optimized translations with proper Arabic grammar
 */

export const notificationsAr = {
  // Common labels
  common: {
    notification: 'إشعار',
    notifications: 'الإشعارات',
    read: 'مقروء',
    unread: 'غير مقروء',
    markAsRead: 'تمييز كمقروء',
    markAsUnread: 'تمييز كغير مقروء',
    delete: 'حذف',
    close: 'إغلاق',
    refresh: 'تحديث',
    loadMore: 'تحميل المزيد',
    viewAll: 'عرض الكل',
    noNotifications: 'لا توجد إشعارات',
    loading: 'جاري تحميل الإشعارات...',
    error: 'فشل في تحميل الإشعارات',
    retry: 'إعادة المحاولة'
  },

  // Notification Bell Component
  bell: {
    bellAriaLabel: 'الإشعارات',
    bellWithUnreadAriaLabel: 'الإشعارات - {{count}} غير مقروء',
    unreadCountAriaLabel: '{{count}} إشعارات غير مقروءة',
    dropdownAriaLabel: 'قائمة الإشعارات',
    notificationsTitle: 'الإشعارات',
    markAllRead: 'تمييز الكل كمقروء',
    markAllReadArياLabel: 'تمييز جميع الإشعارات كمقروءة',
    markingAsRead: 'جاري التمييز كمقروء...',
    unreadCountText: '{{count}} غير مقروء',
    noNotifications: 'لا توجد إشعارات جديدة',
    moreNotifications: '+{{count}} أكثر',
    viewAll: 'عرض الكل',
    notificationItemAriaLabel: 'إشعار: {{subject}}'
  },

  // Notification Center Component
  center: {
    title: 'مركز الإشعارات',
    subtitle: 'ابق على اطلاع بأحدث إشعاراتك',
    markAllAsRead: 'تمييز الكل كمقروء',
    markingAllAsRead: 'جاري تمييز الكل كمقروء...',
    refresh: 'تحديث',
    refreshing: 'جاري التحديث...',
    filters: 'المرشحات',
    showFilters: 'إظهار المرشحات',
    hideFilters: 'إخفاء المرشحات',
    clearFilters: 'مسح المرشحات',
    noNotifications: 'ليس لديك إشعارات',
    noNotificationsSubtext: 'عندما تتلقى إشعارات، ستظهر هنا',
    loadingMore: 'جاري تحميل المزيد من الإشعارات...',
    loadMoreButton: 'تحميل المزيد',
    errorLoading: 'فشل في تحميل الإشعارات',
    errorLoadingMore: 'فشل في تحميل المزيد من الإشعارات',
    tryAgain: 'حاول مرة أخرى'
  },

  // Filter Controls
  filters: {
    status: 'الحالة',
    statusAll: 'الكل',
    statusRead: 'مقروء',
    statusUnread: 'غير مقروء',
    type: 'النوع',
    typeAll: 'جميع الأنواع',
    priority: 'الأولوية',
    priorityAll: 'جميع الأولويات',
    dateRange: 'نطاق التاريخ',
    dateRangeAll: 'كل الوقت',
    dateRangeToday: 'اليوم',
    dateRangeWeek: 'هذا الأسبوع',
    dateRangeMonth: 'هذا الشهر',
    searchPlaceholder: 'البحث في الإشعارات...',
    searchClear: 'مسح البحث',
    resultsCount: 'تم العثور على {{count}} إشعار',
    noResults: 'لا توجد إشعارات تطابق مرشحاتك',
    resetFilters: 'إعادة تعيين المرشحات'
  },

  // Notification Types
  types: {
    ExamReminder: 'تذكير بالامتحان',
    ExamStart: 'بدء الامتحان',
    ExamCompleted: 'انتهاء الامتحان',
    GradePublished: 'نشر الدرجة',
    SystemMaintenance: 'صيانة النظام',
    AccountUpdate: 'تحديث الحساب',
    PasswordChanged: 'تغيير كلمة المرور',
    LoginAlert: 'تنبيه تسجيل الدخول',
    General: 'عام',
    Security: 'أمان'
  },

  // Priority Levels
  priorities: {
    Low: 'منخفض',
    Normal: 'عادي',
    High: 'عالي',
    Critical: 'حرج'
  },

  // Notification Status
  status: {
    sent: 'مُرسل',
    delivered: 'مُسلّم',
    read: 'مقروء',
    failed: 'فشل'
  },

  // Channels
  channels: {
    Email: 'البريد الإلكتروني',
    SMS: 'رسالة نصية',
    Push: 'إشعار فوري',
    InApp: 'داخل التطبيق'
  },

  // Actions
  actions: {
    markAsRead: 'تمييز كمقروء',
    markAsUnread: 'تمييز كغير مقروء',
    delete: 'حذف',
    archive: 'أرشفة',
    reply: 'رد',
    forward: 'إعادة توجيه',
    report: 'إبلاغ',
    block: 'حظر',
    mute: 'كتم',
    unmute: 'إلغاء الكتم'
  },

  // Success Messages
  success: {
    markedAsRead: 'تم تمييز الإشعار كمقروء',
    markedAsUnread: 'تم تمييز الإشعار كغير مقروء',
    deleted: 'تم حذف الإشعار',
    archived: 'تم أرشفة الإشعار',
    allMarkedAsRead: 'تم تمييز جميع الإشعارات كمقروءة',
    preferencesUpdated: 'تم تحديث تفضيلات الإشعارات'
  },

  // Error Messages
  errors: {
    loadFailed: 'فشل في تحميل الإشعارات',
    markReadFailed: 'فشل في تمييز الإشعار كمقروء',
    markUnreadFailed: 'فشل في تمييز الإشعار كغير مقروء',
    deleteFailed: 'فشل في حذف الإشعار',
    archiveFailed: 'فشل في أرشفة الإشعار',
    markAllReadFailed: 'فشل في تمييز جميع الإشعارات كمقروءة',
    networkError: 'خطأ في الشبكة. يرجى التحقق من اتصالك.',
    unknownError: 'حدث خطأ غير متوقع'
  },

  // Pagination
  pagination: {
    page: 'صفحة',
    of: 'من',
    showing: 'عرض',
    to: 'إلى',
    of_total: 'من {{total}} إشعار',
    previous: 'السابق',
    next: 'التالي',
    first: 'الأول',
    last: 'الأخير',
    itemsPerPage: 'عناصر في الصفحة'
  },

  // Empty States
  empty: {
    title: 'لا توجد إشعارات بعد',
    subtitle: 'ستراى الإشعارات هنا عند استلامها',
    filtered: {
      title: 'لا توجد إشعارات مطابقة',
      subtitle: 'حاول تعديل مرشحاتك لرؤية المزيد من النتائج'
    },
    error: {
      title: 'غير قادر على تحميل الإشعارات',
      subtitle: 'يرجى المحاولة مرة أخرى لاحقاً أو الاتصال بالدعم إذا استمرت المشكلة'
    }
  },

  // Preferences (for future implementation)
  preferences: {
    title: 'تفضيلات الإشعارات',
    subtitle: 'اختر كيف تريد تلقي الإشعارات',
    emailNotifications: 'إشعارات البريد الإلكتروني',
    smsNotifications: 'إشعارات الرسائل النصية',
    pushNotifications: 'الإشعارات الفورية',
    inAppNotifications: 'الإشعارات داخل التطبيق',
    examReminders: 'تذكيرات الامتحانات',
    gradeUpdates: 'تحديثات الدرجات',
    systemAlerts: 'تنبيهات النظام',
    securityAlerts: 'تنبيهات الأمان',
    save: 'حفظ التفضيلات',
    saving: 'جاري الحفظ...',
    cancel: 'إلغاء'
  },

  // Time formatting
  time: {
    justNow: 'الآن',
    minutesAgo: 'منذ {{count}} دقيقة',
    minutesAgo_plural: 'منذ {{count}} دقائق',
    hoursAgo: 'منذ {{count}} ساعة',
    hoursAgo_plural: 'منذ {{count}} ساعات',
    daysAgo: 'منذ {{count}} يوم',
    daysAgo_plural: 'منذ {{count}} أيام',
    weeksAgo: 'منذ {{count}} أسبوع',
    weeksAgo_plural: 'منذ {{count}} أسابيع',
    monthsAgo: 'منذ {{count}} شهر',
    monthsAgo_plural: 'منذ {{count}} أشهر',
    yearsAgo: 'منذ {{count}} سنة',
    yearsAgo_plural: 'منذ {{count}} سنوات'
  }
};

export default notificationsAr;
