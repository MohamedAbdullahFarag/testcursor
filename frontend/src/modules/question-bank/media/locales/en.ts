/**
 * English translations for media (question-bank media manager)
 * Focused on media management within question bank context
 */

export const mediaEn = {
  // Main interface
  title: 'Media Manager',
  subtitle: 'Manage media files for questions and answers',
  
  // View modes (used by MediaManager)
  viewMode: {
    grid: 'Grid View',
    list: 'List View', 
    gallery: 'Gallery View'
  },
  
  // Legacy view mode keys (for backward compatibility)
  gridView: 'Grid View',
  listView: 'List View',
  galleryView: 'Gallery View',
  
  // Actions (used by MediaManager)
  actions: {
    upload: 'Upload Media',
    manageCategories: 'Manage Categories',
    manageCollections: 'Manage Collections'
  },
  
  // Statistics (used by MediaManager)
  stats: {
    images: 'Images',
    videos: 'Videos', 
    audio: 'Audio Files',
    documents: 'Documents',
    total: 'Total Files'
  },
  
  // Navigation
  allMedia: 'All Media',
  images: 'Images',
  videos: 'Videos',
  audio: 'Audio',
  documents: 'Documents',
  
  // Actions
  upload: 'Upload Media',
  addMedia: 'Add Media',
  selectMedia: 'Select Media',
  manageCategories: 'Manage Categories',
  manageCollections: 'Manage Collections',
  previewMedia: 'Preview Media',
  
  // File operations
  select: 'Select',
  download: 'Download',
  delete: 'Delete',
  edit: 'Edit Properties',
  copyUrl: 'Copy URL',
  share: 'Share',
  
  // Search and filters
  searchPlaceholder: 'Search media files...',
  filterByType: 'Filter by Type',
  filterByCategory: 'Filter by Category',
  filterByDate: 'Filter by Date',
  sortBy: 'Sort By',
  
  // Sort options
  sortByName: 'Name',
  sortByDate: 'Date Added',
  sortBySize: 'File Size',
  sortByType: 'File Type',
  ascending: 'Ascending',
  descending: 'Descending',
  
  // Media types
  image: 'Image',
  video: 'Video',
  audioFile: 'Audio',
  document: 'Document',
  other: 'Other',
  
  // File info
  fileName: 'File Name',
  fileSize: 'File Size',
  fileType: 'File Type',
  uploadDate: 'Upload Date',
  dimensions: 'Dimensions',
  duration: 'Duration',
  
  // Selection
  selectAll: 'Select All',
  deselectAll: 'Deselect All',
  selectedItems: 'Selected Items',
  maxSelectionReached: 'Maximum selection limit reached',
  
  // Upload
  uploadTitle: 'Upload Media Files',
  dragDropText: 'Drag and drop files here or click to browse',
  selectFiles: 'Select Files',
  uploadProgress: 'Uploading... {{progress}}%',
  uploadComplete: 'Upload Complete',
  uploadFailed: 'Upload Failed',
  
  // Categories
  categories: 'Categories',
  createCategory: 'Create Category',
  editCategory: 'Edit Category',
  categoryName: 'Category Name',
  uncategorized: 'Uncategorized',
  
  // Collections
  collections: 'Collections',
  createCollection: 'Create Collection',
  addToCollection: 'Add to Collection',
  
  // Status messages
  loading: 'Loading media...',
  noMediaFound: 'No media files found',
  searchNoResults: 'No files match your search',
  
  // Error messages
  loadError: 'Failed to load media files',
  uploadError: 'Failed to upload file',
  deleteError: 'Failed to delete file',
  
  // Success messages
  uploadSuccess: 'File uploaded successfully',
  deleteSuccess: 'File deleted successfully',
  
  // Confirmation
  confirmDelete: 'Are you sure you want to delete this file?',
  confirmBulkDelete: 'Are you sure you want to delete {{count}} files?',
  
  // Buttons
  cancel: 'Cancel',
  confirm: 'Confirm',
  save: 'Save',
  close: 'Close'
};

export default mediaEn;
