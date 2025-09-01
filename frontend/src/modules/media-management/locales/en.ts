/**
 * English translations for media-management module
 * Comprehensive translation keys for all media management components
 */

export const mediaManagementEn = {
  // Main page
  pageTitle: 'Media Management',
  subtitle: 'Manage your media files and assets',
  
  // Page section (nested structure for component usage)
  page: {
    title: 'Media Management',
    description: 'Manage your media files and assets'
  },
  
  // Tabs
  tabs: {
    library: 'Media Library',
    upload: 'Upload Media',
    categories: 'Categories',
    collections: 'Collections',
    analytics: 'Analytics',
    settings: 'Settings'
  },
  
  // Actions
  actions: {
    quickUpload: 'Quick Upload',
    newCategory: 'New Category',
    newCollection: 'New Collection'
  },
  
  // Statistics
  stats: {
    totalFiles: 'Total Files',
    totalSize: 'Storage Used',
    categories: 'Categories',
    collections: 'Collections'
  },
  
  // Content section
  content: {
    title: 'Media Content'
  },
  
  // Collections
  collections: {
    title: 'Media Collections',
    description: 'Group related media files together',
    newCollection: 'New Collection'
  },
  
  // Upload
  upload: {
    title: 'Upload Media',
    description: 'Add new media files to your library',
    dragDropText: 'Drag and drop files here or click to browse',
    supportedFormats: 'Supported formats: JPG, PNG, GIF, MP4, PDF',
    maxFileSize: 'Maximum file size: 10MB'
  },
  
  // Settings
  settings: {
    title: 'Media Settings',
    description: 'Configure media management preferences',
    storage: 'Storage Settings',
    maxFileSize: 'Maximum File Size (MB)',
    allowedTypes: 'Allowed File Types',
    processing: 'Processing Settings',
    autoThumbnails: 'Auto-generate thumbnails',
    security: 'Security Settings',
    notifications: 'Notification Settings'
  },
  
  // Navigation tabs (removed duplicates)
  library: 'Media Library',
  analytics: 'Analytics',
  
  // Library view
  libraryTitle: 'Media Library',
  librarySubtitle: 'Browse and manage your media files',
  searchPlaceholder: 'Search media files...',
  filterByType: 'Filter by type',
  filterByCategory: 'Filter by category',
  filterByDate: 'Filter by date',
  allTypes: 'All Types',
  allCategories: 'All Categories',
  allDates: 'All Dates',
  
  // File types
  types: {
    image: 'Image',
    video: 'Video',
    audio: 'Audio',
    document: 'Document',
    other: 'Other'
  },
  
  // Upload
  uploadTitle: 'Upload Media',
  uploadSubtitle: 'Add new media files to your library',
  dragDropText: 'Drag and drop files here or click to browse',
  selectFiles: 'Select Files',
  uploadProgress: 'Uploading {{progress}}%',
  uploadComplete: 'Upload complete',
  uploadFailed: 'Upload failed',
  uploadCancelled: 'Upload cancelled',
  maxFileSize: 'Maximum file size: {{size}}',
  allowedFormats: 'Allowed formats: {{formats}}',
  
  // Categories
  categoriesTitle: 'Media Categories',
  categoriesSubtitle: 'Organize your media with categories',
  createCategory: 'Create Category',
  editCategory: 'Edit Category',
  deleteCategory: 'Delete Category',
  categoryName: 'Category Name',
  categoryDescription: 'Description',
  categoryColor: 'Category Color',
  categoryIcon: 'Category Icon',
  noCategoriesFound: 'No categories found',
  
  // Collections
  collectionsTitle: 'Media Collections',
  collectionsSubtitle: 'Group related media files together',
  createCollection: 'Create Collection',
  editCollection: 'Edit Collection',
  deleteCollection: 'Delete Collection',
  collectionName: 'Collection Name',
  collectionDescription: 'Description',
  addToCollection: 'Add to Collection',
  removeFromCollection: 'Remove from Collection',
  noCollectionsFound: 'No collections found',
  
  // Analytics
  analyticsTitle: 'Media Analytics',
  analyticsSubtitle: 'View insights about your media usage',
  totalFiles: 'Total Files',
  storageUsed: 'Storage Used',
  popularFiles: 'Popular Files',
  recentUploads: 'Recent Uploads',
  downloadCount: 'Downloads',
  viewCount: 'Views',
  
  // Settings
  settingsTitle: 'Media Settings',
  settingsSubtitle: 'Configure media management preferences',
  storageLimit: 'Storage Limit',
  autoOptimize: 'Auto-optimize images',
  watermark: 'Apply watermark',
  backupEnabled: 'Enable backup',
  compressionQuality: 'Compression Quality',
  
  // File operations
  download: 'Download',
  delete: 'Delete',
  edit: 'Edit',
  share: 'Share',
  copy: 'Copy',
  move: 'Move',
  rename: 'Rename',
  preview: 'Preview',
  details: 'Details',
  
  // File details
  fileName: 'File Name',
  fileSize: 'File Size',
  fileType: 'File Type',
  dateUploaded: 'Date Uploaded',
  lastModified: 'Last Modified',
  dimensions: 'Dimensions',
  resolution: 'Resolution',
  duration: 'Duration',
  bitrate: 'Bitrate',
  
  // Actions
  select: 'Select',
  selectAll: 'Select All',
  deselectAll: 'Deselect All',
  bulkDelete: 'Bulk Delete',
  bulkMove: 'Bulk Move',
  bulkDownload: 'Bulk Download',
  
  // Status messages
  loading: 'Loading...',
  loadingStates: {
    mediaFiles: 'Loading media files...',
    categories: 'Loading categories...',
    collections: 'Loading collections...',
    uploads: 'Processing uploads...'
  },
  saving: 'Saving...',
  processing: 'Processing...',
  success: 'Success',
  
  // Error messages
  error: {
    loadingMediaFiles: 'Error loading media files',
    fileNotFound: 'File not found',
    uploadError: 'Failed to upload file',
    deleteError: 'Failed to delete file',
    updateError: 'Failed to update file',
    networkError: 'Network error occurred',
    permissionError: 'Permission denied',
    storageFull: 'Storage limit exceeded'
  },
  
  // Success messages
  fileUploaded: 'File uploaded successfully',
  fileDeleted: 'File deleted successfully',
  fileUpdated: 'File updated successfully',
  categoryCreated: 'Category created successfully',
  collectionCreated: 'Collection created successfully',
  
  // Confirmation dialogs
  confirmDelete: 'Are you sure you want to delete this file?',
  confirmBulkDelete: 'Are you sure you want to delete {{count}} files?',
  confirmDeleteCategory: 'Are you sure you want to delete this category?',
  confirmDeleteCollection: 'Are you sure you want to delete this collection?',
  
  // Buttons
  cancel: 'Cancel',
  confirm: 'Confirm',
  save: 'Save',
  uploadButton: 'Upload',
  browse: 'Browse',
  clear: 'Clear',
  reset: 'Reset',
  apply: 'Apply',
  
  // Empty states
  noFiles: 'No files found',
  noFilesDescription: 'Upload your first file to get started',
  noSearchResults: 'No files match your search',
  noSearchResultsDescription: 'Try adjusting your search terms or filters'
};

export default mediaManagementEn;
