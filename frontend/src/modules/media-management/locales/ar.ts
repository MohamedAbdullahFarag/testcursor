/**
 * Arabic translations for media-management module
 * Comprehensive translation keys for all media management components
 */

export const mediaManagementAr = {
  // Main page
  pageTitle: 'إدارة الوسائط',
  subtitle: 'إدارة ملفات الوسائط والأصول الخاصة بك',
  
  // Page section (nested structure for component usage)
  page: {
    title: 'إدارة الوسائط',
    description: 'إدارة ملفات الوسائط والأصول الخاصة بك'
  },
  
  // Tabs
  tabs: {
    library: 'مكتبة الوسائط',
    upload: 'رفع الوسائط',
    categories: 'الفئات',
    collections: 'المجموعات',
    analytics: 'الإحصائيات',
    settings: 'الإعدادات'
  },
  
  // Actions
  actions: {
    quickUpload: 'رفع سريع',
    newCategory: 'فئة جديدة',
    newCollection: 'مجموعة جديدة'
  },
  
  // Statistics
  stats: {
    totalFiles: 'إجمالي الملفات',
    totalSize: 'المساحة المستخدمة',
    categories: 'الفئات',
    collections: 'المجموعات'
  },
  
  // Content section
  content: {
    title: 'محتوى الوسائط'
  },
  
  // Collections
  collections: {
    title: 'مجموعات الوسائط',
    description: 'تجميع ملفات الوسائط ذات الصلة معًا',
    newCollection: 'مجموعة جديدة'
  },
  
  // Upload
  upload: {
    title: 'رفع الوسائط',
    description: 'إضافة ملفات وسائط جديدة إلى مكتبتك',
    dragDropText: 'اسحب وأفلت الملفات هنا أو انقر للتصفح',
    supportedFormats: 'التنسيقات المدعومة: JPG, PNG, GIF, MP4, PDF',
    maxFileSize: 'الحد الأقصى لحجم الملف: 10 ميجابايت'
  },
  
  // Settings
  settings: {
    title: 'إعدادات الوسائط',
    description: 'تكوين تفضيلات إدارة الوسائط',
    storage: 'إعدادات التخزين',
    maxFileSize: 'الحد الأقصى لحجم الملف (ميجابايت)',
    allowedTypes: 'أنواع الملفات المسموحة',
    processing: 'إعدادات المعالجة',
    autoThumbnails: 'إنشاء صور مصغرة تلقائيًا',
    security: 'إعدادات الأمان',
    notifications: 'إعدادات الإشعارات'
  },
  
  // Navigation tabs (removed duplicates)
  library: 'مكتبة الوسائط',
  analytics: 'التحليلات',
  
  // Library view
  libraryTitle: 'مكتبة الوسائط',
  librarySubtitle: 'تصفح وإدارة ملفات الوسائط الخاصة بك',
  searchPlaceholder: 'البحث في ملفات الوسائط...',
  filterByType: 'تصفية حسب النوع',
  filterByCategory: 'تصفية حسب الفئة',
  filterByDate: 'تصفية حسب التاريخ',
  allTypes: 'جميع الأنواع',
  allCategories: 'جميع الفئات',
  allDates: 'جميع التواريخ',
  
  // File types
  types: {
    image: 'صورة',
    video: 'فيديو',
    audio: 'صوتي',
    document: 'مستند',
    other: 'أخرى'
  },
  
  // Upload
  uploadTitle: 'رفع الوسائط',
  uploadSubtitle: 'إضافة ملفات وسائط جديدة إلى مكتبتك',
  dragDropText: 'اسحب وأفلت الملفات هنا أو انقر للتصفح',
  selectFiles: 'اختيار الملفات',
  uploadProgress: 'جاري الرفع {{progress}}%',
  uploadComplete: 'اكتمل الرفع',
  uploadFailed: 'فشل الرفع',
  uploadCancelled: 'تم إلغاء الرفع',
  maxFileSize: 'الحد الأقصى لحجم الملف: {{size}}',
  allowedFormats: 'التنسيقات المسموحة: {{formats}}',
  
  // Categories
  categoriesTitle: 'فئات الوسائط',
  categoriesSubtitle: 'تنظيم الوسائط الخاصة بك بالفئات',
  createCategory: 'إنشاء فئة',
  editCategory: 'تعديل الفئة',
  deleteCategory: 'حذف الفئة',
  categoryName: 'اسم الفئة',
  categoryDescription: 'الوصف',
  categoryColor: 'لون الفئة',
  categoryIcon: 'رمز الفئة',
  noCategoriesFound: 'لم يتم العثور على فئات',
  
  // Collections
  collectionsTitle: 'مجموعات الوسائط',
  collectionsSubtitle: 'تجميع ملفات الوسائط ذات الصلة معًا',
  createCollection: 'إنشاء مجموعة',
  editCollection: 'تعديل المجموعة',
  deleteCollection: 'حذف المجموعة',
  collectionName: 'اسم المجموعة',
  collectionDescription: 'الوصف',
  addToCollection: 'إضافة إلى المجموعة',
  removeFromCollection: 'إزالة من المجموعة',
  noCollectionsFound: 'لم يتم العثور على مجموعات',
  
  // Analytics
  analyticsTitle: 'تحليلات الوسائط',
  analyticsSubtitle: 'عرض إحصائيات حول استخدام الوسائط',
  totalFiles: 'إجمالي الملفات',
  storageUsed: 'المساحة المستخدمة',
  popularFiles: 'الملفات الشائعة',
  recentUploads: 'الرفوعات الحديثة',
  downloadCount: 'التحميلات',
  viewCount: 'المشاهدات',
  
  // Settings
  settingsTitle: 'إعدادات الوسائط',
  settingsSubtitle: 'تكوين تفضيلات إدارة الوسائط',
  storageLimit: 'حد التخزين',
  autoOptimize: 'تحسين الصور تلقائيًا',
  watermark: 'تطبيق العلامة المائية',
  backupEnabled: 'تفعيل النسخ الاحتياطي',
  compressionQuality: 'جودة الضغط',
  
  // File operations
  download: 'تحميل',
  delete: 'حذف',
  edit: 'تعديل',
  share: 'مشاركة',
  copy: 'نسخ',
  move: 'نقل',
  rename: 'إعادة تسمية',
  preview: 'معاينة',
  details: 'التفاصيل',
  
  // File details
  fileName: 'اسم الملف',
  fileSize: 'حجم الملف',
  fileType: 'نوع الملف',
  dateUploaded: 'تاريخ الرفع',
  lastModified: 'آخر تعديل',
  dimensions: 'الأبعاد',
  resolution: 'الدقة',
  duration: 'المدة',
  bitrate: 'معدل البت',
  
  // Actions
  select: 'اختيار',
  selectAll: 'اختيار الكل',
  deselectAll: 'إلغاء اختيار الكل',
  bulkDelete: 'حذف متعدد',
  bulkMove: 'نقل متعدد',
  bulkDownload: 'تحميل متعدد',
  
  // Status messages
  loading: 'جاري التحميل...',
  loadingStates: {
    mediaFiles: 'جاري تحميل ملفات الوسائط...',
    categories: 'جاري تحميل الفئات...',
    collections: 'جاري تحميل المجموعات...',
    uploads: 'جاري معالجة الرفوعات...'
  },
  saving: 'جاري الحفظ...',
  processing: 'جاري المعالجة...',
  success: 'نجح',
  
  // Error messages
  error: {
    loadingMediaFiles: 'خطأ في تحميل ملفات الوسائط',
    fileNotFound: 'لم يتم العثور على الملف',
    uploadError: 'فشل في رفع الملف',
    deleteError: 'فشل في حذف الملف',
    updateError: 'فشل في تحديث الملف',
    networkError: 'حدث خطأ في الشبكة',
    permissionError: 'تم رفض الإذن',
    storageFull: 'تم تجاوز حد التخزين'
  },
  
  // Error messages
  fileNotFound: 'لم يتم العثور على الملف',
  uploadError: 'فشل في رفع الملف',
  deleteError: 'فشل في حذف الملف',
  updateError: 'فشل في تحديث الملف',
  networkError: 'حدث خطأ في الشبكة',
  permissionError: 'تم رفض الإذن',
  storageFull: 'تم تجاوز حد التخزين',
  
  // Success messages
  fileUploaded: 'تم رفع الملف بنجاح',
  fileDeleted: 'تم حذف الملف بنجاح',
  fileUpdated: 'تم تحديث الملف بنجاح',
  categoryCreated: 'تم إنشاء الفئة بنجاح',
  collectionCreated: 'تم إنشاء المجموعة بنجاح',
  
  // Confirmation dialogs
  confirmDelete: 'هل أنت متأكد من حذف هذا الملف؟',
  confirmBulkDelete: 'هل أنت متأكد من حذف {{count}} ملف؟',
  confirmDeleteCategory: 'هل أنت متأكد من حذف هذه الفئة؟',
  confirmDeleteCollection: 'هل أنت متأكد من حذف هذه المجموعة؟',
  
  // Buttons
  cancel: 'إلغاء',
  confirm: 'تأكيد',
  save: 'حفظ',
  uploadButton: 'رفع',
  browse: 'تصفح',
  clear: 'مسح',
  reset: 'إعادة تعيين',
  apply: 'تطبيق',
  
  // Empty states
  noFiles: 'لا توجد ملفات',
  noFilesDescription: 'ارفع ملفك الأول للبدء',
  noSearchResults: 'لا توجد ملفات تطابق البحث',
  noSearchResultsDescription: 'جرب تعديل مصطلحات البحث أو المرشحات'
};

export default mediaManagementAr;
