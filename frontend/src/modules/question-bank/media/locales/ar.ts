/**
 * Arabic translations for media (question-bank media manager)
 * Focused on media management within question bank context
 */

export const mediaAr = {
  // Main interface
  title: 'مدير الوسائط',
  subtitle: 'إدارة ملفات الوسائط للأسئلة والإجابات',
  
  // View modes (used by MediaManager)
  viewMode: {
    grid: 'عرض الشبكة',
    list: 'عرض القائمة',
    gallery: 'عرض المعرض'
  },
  
  // Legacy view mode keys (for backward compatibility)
  gridView: 'عرض الشبكة',
  listView: 'عرض القائمة',
  galleryView: 'عرض المعرض',
  
  // Actions (used by MediaManager)
  actions: {
    upload: 'رفع وسائط',
    manageCategories: 'إدارة الفئات',
    manageCollections: 'إدارة المجموعات'
  },
  
  // Statistics (used by MediaManager)
  stats: {
    images: 'الصور',
    videos: 'الفيديوهات',
    audio: 'الملفات الصوتية',
    documents: 'المستندات',
    total: 'إجمالي الملفات'
  },
  
  // Navigation
  allMedia: 'جميع الوسائط',
  images: 'الصور',
  videos: 'الفيديوهات',
  audio: 'الملفات الصوتية',
  documents: 'المستندات',
  
  // Actions
  upload: 'رفع وسائط',
  addMedia: 'إضافة وسائط',
  selectMedia: 'اختيار وسائط',
  manageCategories: 'إدارة الفئات',
  manageCollections: 'إدارة المجموعات',
  previewMedia: 'معاينة الوسائط',
  
  // File operations
  select: 'اختيار',
  download: 'تحميل',
  delete: 'حذف',
  edit: 'تعديل الخصائص',
  copyUrl: 'نسخ الرابط',
  share: 'مشاركة',
  
  // Search and filters
  searchPlaceholder: 'البحث في ملفات الوسائط...',
  filterByType: 'تصفية حسب النوع',
  filterByCategory: 'تصفية حسب الفئة',
  filterByDate: 'تصفية حسب التاريخ',
  sortBy: 'ترتيب حسب',
  
  // Sort options
  sortByName: 'الاسم',
  sortByDate: 'تاريخ الإضافة',
  sortBySize: 'حجم الملف',
  sortByType: 'نوع الملف',
  ascending: 'تصاعدي',
  descending: 'تنازلي',
  
  // Media types
  image: 'صورة',
  video: 'فيديو',
  audioFile: 'ملف صوتي',
  document: 'مستند',
  other: 'أخرى',
  
  // File info
  fileName: 'اسم الملف',
  fileSize: 'حجم الملف',
  fileType: 'نوع الملف',
  uploadDate: 'تاريخ الرفع',
  dimensions: 'الأبعاد',
  duration: 'المدة',
  
  // Selection
  selectAll: 'اختيار الكل',
  deselectAll: 'إلغاء اختيار الكل',
  selectedItems: 'العناصر المحددة',
  maxSelectionReached: 'تم الوصول للحد الأقصى للاختيار',
  
  // Upload
  uploadTitle: 'رفع ملفات الوسائط',
  dragDropText: 'اسحب وأفلت الملفات هنا أو انقر للتصفح',
  selectFiles: 'اختيار الملفات',
  uploadProgress: 'جاري الرفع... {{progress}}%',
  uploadComplete: 'اكتمل الرفع',
  uploadFailed: 'فشل الرفع',
  
  // Categories
  categories: 'الفئات',
  createCategory: 'إنشاء فئة',
  editCategory: 'تعديل الفئة',
  categoryName: 'اسم الفئة',
  uncategorized: 'غير مصنف',
  
  // Collections
  collections: 'المجموعات',
  createCollection: 'إنشاء مجموعة',
  addToCollection: 'إضافة إلى المجموعة',
  
  // Status messages
  loading: 'جاري تحميل الوسائط...',
  noMediaFound: 'لم يتم العثور على ملفات وسائط',
  searchNoResults: 'لا توجد ملفات تطابق البحث',
  
  // Error messages
  loadError: 'فشل في تحميل ملفات الوسائط',
  uploadError: 'فشل في رفع الملف',
  deleteError: 'فشل في حذف الملف',
  
  // Success messages
  uploadSuccess: 'تم رفع الملف بنجاح',
  deleteSuccess: 'تم حذف الملف بنجاح',
  
  // Confirmation
  confirmDelete: 'هل أنت متأكد من حذف هذا الملف؟',
  confirmBulkDelete: 'هل أنت متأكد من حذف {{count}} ملف؟',
  
  // Buttons
  cancel: 'إلغاء',
  confirm: 'تأكيد',
  save: 'حفظ',
  close: 'إغلاق'
};

export default mediaAr;
