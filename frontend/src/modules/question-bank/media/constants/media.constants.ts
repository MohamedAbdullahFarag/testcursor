// Media Management Constants
// Configuration and default values for the media management system

export const MEDIA_CONSTANTS = {
  // File upload limits
  UPLOAD: {
    MAX_FILE_SIZE: 100 * 1024 * 1024, // 100MB
    MAX_FILES_PER_UPLOAD: 50,
    MAX_CONCURRENT_UPLOADS: 5,
    CHUNK_SIZE: 1024 * 1024, // 1MB chunks
    RETRY_ATTEMPTS: 3,
    TIMEOUT: 300000, // 5 minutes
  },

  // Supported file formats
  SUPPORTED_FORMATS: {
    IMAGES: ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp', '.svg', '.tiff', '.tif'],
    VIDEOS: ['.mp4', '.avi', '.mov', '.wmv', '.flv', '.webm', '.mkv', '.m4v'],
    AUDIO: ['.mp3', '.wav', '.flac', '.aac', '.ogg', '.wma', '.m4a'],
    DOCUMENTS: ['.pdf', '.doc', '.docx', '.xls', '.xlsx', '.ppt', '.pptx', '.txt', '.rtf'],
    ARCHIVES: ['.zip', '.rar', '.7z', '.tar', '.gz', '.bz2'],
  },

  // Thumbnail sizes
  THUMBNAIL_SIZES: {
    SMALL: { width: 150, height: 150, quality: 80 },
    MEDIUM: { width: 300, height: 300, quality: 85 },
    LARGE: { width: 600, height: 600, quality: 90 },
    EXTRA_LARGE: { width: 1200, height: 1200, quality: 95 },
  },

  // Processing job priorities
  JOB_PRIORITIES: {
    LOW: 1,
    NORMAL: 5,
    HIGH: 8,
    URGENT: 10,
  },

  // Pagination defaults
  PAGINATION: {
    DEFAULT_PAGE_SIZE: 20,
    PAGE_SIZE_OPTIONS: [10, 20, 50, 100],
    MAX_PAGE_SIZE: 100,
  },

  // Search and filtering
  SEARCH: {
    MIN_SEARCH_LENGTH: 2,
    MAX_SEARCH_LENGTH: 100,
    SEARCH_DELAY: 300, // milliseconds
    MAX_SEARCH_HISTORY: 10,
    MAX_RECENT_SEARCHES: 5,
  },

  // View modes
  VIEW_MODES: {
    GRID: 'grid',
    LIST: 'list',
    GALLERY: 'gallery',
    TIMELINE: 'timeline',
  },

  // Media types
  MEDIA_TYPES: {
    IMAGE: 1,
    VIDEO: 2,
    AUDIO: 3,
    DOCUMENT: 4,
    ARCHIVE: 5,
    OTHER: 6,
  },

  // Media statuses
  MEDIA_STATUS: {
    PENDING: 1,
    PROCESSING: 2,
    READY: 3,
    FAILED: 4,
    DELETED: 5,
  },

  // Collection types
  COLLECTION_TYPES: {
    ALBUM: 1,
    PLAYLIST: 2,
    GALLERY: 3,
    PORTFOLIO: 4,
    ARCHIVE: 5,
  },

  // Thumbnail sizes
  THUMBNAIL_SIZE: {
    SMALL: 1,
    MEDIUM: 2,
    LARGE: 3,
    EXTRA_LARGE: 4,
    CUSTOM: 5,
  },

  // Processing job types
  PROCESSING_JOB_TYPES: {
    THUMBNAIL_GENERATION: 1,
    VIDEO_TRANSCODING: 2,
    AUDIO_TRANSCODING: 3,
    IMAGE_OPTIMIZATION: 4,
    METADATA_EXTRACTION: 5,
    VIRUS_SCAN: 6,
  },

  // Processing job statuses
  PROCESSING_JOB_STATUS: {
    QUEUED: 1,
    PROCESSING: 2,
    COMPLETED: 3,
    FAILED: 4,
    CANCELLED: 5,
  },

  // Access actions
  ACCESS_ACTIONS: {
    VIEW: 1,
    DOWNLOAD: 2,
    EDIT: 3,
    DELETE: 4,
    SHARE: 5,
  },

  // File size units
  FILE_SIZE_UNITS: {
    BYTES: 'B',
    KILOBYTES: 'KB',
    MEGABYTES: 'MB',
    GIGABYTES: 'GB',
    TERABYTES: 'TB',
  },

  // Date formats
  DATE_FORMATS: {
    DISPLAY: 'MMM dd, yyyy',
    DISPLAY_WITH_TIME: 'MMM dd, yyyy HH:mm',
    ISO: 'yyyy-MM-dd',
    ISO_WITH_TIME: "yyyy-MM-dd'T'HH:mm:ss",
  },

  // Sort options
  SORT_OPTIONS: {
    FILE_NAME: 'fileName',
    FILE_SIZE: 'fileSizeBytes',
    UPLOAD_DATE: 'uploadedAt',
    MODIFIED_DATE: 'modifiedAt',
    MEDIA_TYPE: 'mediaType',
    STATUS: 'status',
  },

  // Sort orders
  SORT_ORDERS: {
    ASC: 'asc',
    DESC: 'desc',
  },

  // Bulk action limits
  BULK_ACTIONS: {
    MAX_SELECTION: 1000,
    WARNING_THRESHOLD: 100,
  },

  // Cache settings
  CACHE: {
    MEDIA_FILES_TTL: 5 * 60 * 1000, // 5 minutes
    CATEGORIES_TTL: 10 * 60 * 1000, // 10 minutes
    COLLECTIONS_TTL: 10 * 60 * 1000, // 10 minutes
    STATS_TTL: 2 * 60 * 1000, // 2 minutes
  },

  // Error codes
  ERROR_CODES: {
    FILE_TOO_LARGE: 'FILE_TOO_LARGE',
    UNSUPPORTED_FORMAT: 'UNSUPPORTED_FORMAT',
    VIRUS_DETECTED: 'VIRUS_DETECTED',
    DUPLICATE_FILE: 'DUPLICATE_FILE',
    UPLOAD_FAILED: 'UPLOAD_FAILED',
    PROCESSING_FAILED: 'PROCESSING_FAILED',
    STORAGE_FULL: 'STORAGE_FULL',
    PERMISSION_DENIED: 'PERMISSION_DENIED',
  },

  // Success messages
  SUCCESS_MESSAGES: {
    UPLOAD_COMPLETED: 'Files uploaded successfully',
    PROCESSING_COMPLETED: 'Processing completed successfully',
    CATEGORY_CREATED: 'Category created successfully',
    CATEGORY_UPDATED: 'Category updated successfully',
    CATEGORY_DELETED: 'Category deleted successfully',
    COLLECTION_CREATED: 'Collection created successfully',
    COLLECTION_UPDATED: 'Collection updated successfully',
    COLLECTION_DELETED: 'Collection deleted successfully',
  },

  // Warning messages
  WARNING_MESSAGES: {
    LARGE_FILE_UPLOAD: 'Large file upload may take some time',
    PROCESSING_DELAY: 'Processing may be delayed due to high load',
    STORAGE_WARNING: 'Storage space is running low',
  },

  // Info messages
  INFO_MESSAGES: {
    UPLOADING: 'Uploading files...',
    PROCESSING: 'Processing files...',
    GENERATING_THUMBNAILS: 'Generating thumbnails...',
    EXTRACTING_METADATA: 'Extracting metadata...',
  },
} as const;

// Media type labels
export const MEDIA_TYPE_LABELS = {
  [MEDIA_CONSTANTS.MEDIA_TYPES.IMAGE]: 'Image',
  [MEDIA_CONSTANTS.MEDIA_TYPES.VIDEO]: 'Video',
  [MEDIA_CONSTANTS.MEDIA_TYPES.AUDIO]: 'Audio',
  [MEDIA_CONSTANTS.MEDIA_TYPES.DOCUMENT]: 'Document',
  [MEDIA_CONSTANTS.MEDIA_TYPES.ARCHIVE]: 'Archive',
  [MEDIA_CONSTANTS.MEDIA_TYPES.OTHER]: 'Other',
} as const;

// Media status labels
export const MEDIA_STATUS_LABELS = {
  [MEDIA_CONSTANTS.MEDIA_STATUS.PENDING]: 'Pending',
  [MEDIA_CONSTANTS.MEDIA_STATUS.PROCESSING]: 'Processing',
  [MEDIA_CONSTANTS.MEDIA_STATUS.READY]: 'Ready',
  [MEDIA_CONSTANTS.MEDIA_STATUS.FAILED]: 'Failed',
  [MEDIA_CONSTANTS.MEDIA_STATUS.DELETED]: 'Deleted',
} as const;

// Collection type labels
export const COLLECTION_TYPE_LABELS = {
  [MEDIA_CONSTANTS.COLLECTION_TYPES.ALBUM]: 'Album',
  [MEDIA_CONSTANTS.COLLECTION_TYPES.PLAYLIST]: 'Playlist',
  [MEDIA_CONSTANTS.COLLECTION_TYPES.GALLERY]: 'Gallery',
  [MEDIA_CONSTANTS.COLLECTION_TYPES.PORTFOLIO]: 'Portfolio',
  [MEDIA_CONSTANTS.COLLECTION_TYPES.ARCHIVE]: 'Archive',
} as const;

// Thumbnail size labels
export const THUMBNAIL_SIZE_LABELS = {
  [MEDIA_CONSTANTS.THUMBNAIL_SIZE.SMALL]: 'Small',
  [MEDIA_CONSTANTS.THUMBNAIL_SIZE.MEDIUM]: 'Medium',
  [MEDIA_CONSTANTS.THUMBNAIL_SIZE.LARGE]: 'Large',
  [MEDIA_CONSTANTS.THUMBNAIL_SIZE.EXTRA_LARGE]: 'Extra Large',
  [MEDIA_CONSTANTS.THUMBNAIL_SIZE.CUSTOM]: 'Custom',
} as const;

// Processing job type labels
export const PROCESSING_JOB_TYPE_LABELS = {
  [MEDIA_CONSTANTS.PROCESSING_JOB_TYPES.THUMBNAIL_GENERATION]: 'Thumbnail Generation',
  [MEDIA_CONSTANTS.PROCESSING_JOB_TYPES.VIDEO_TRANSCODING]: 'Video Transcoding',
  [MEDIA_CONSTANTS.PROCESSING_JOB_TYPES.AUDIO_TRANSCODING]: 'Audio Transcoding',
  [MEDIA_CONSTANTS.PROCESSING_JOB_TYPES.IMAGE_OPTIMIZATION]: 'Image Optimization',
  [MEDIA_CONSTANTS.PROCESSING_JOB_TYPES.METADATA_EXTRACTION]: 'Metadata Extraction',
  [MEDIA_CONSTANTS.PROCESSING_JOB_TYPES.VIRUS_SCAN]: 'Virus Scan',
} as const;

// Processing job status labels
export const PROCESSING_JOB_STATUS_LABELS = {
  [MEDIA_CONSTANTS.PROCESSING_JOB_STATUS.QUEUED]: 'Queued',
  [MEDIA_CONSTANTS.PROCESSING_JOB_STATUS.PROCESSING]: 'Processing',
  [MEDIA_CONSTANTS.PROCESSING_JOB_STATUS.COMPLETED]: 'Completed',
  [MEDIA_CONSTANTS.PROCESSING_JOB_STATUS.FAILED]: 'Failed',
  [MEDIA_CONSTANTS.PROCESSING_JOB_STATUS.CANCELLED]: 'Cancelled',
} as const;

// Access action labels
export const ACCESS_ACTION_LABELS = {
  [MEDIA_CONSTANTS.ACCESS_ACTIONS.VIEW]: 'View',
  [MEDIA_CONSTANTS.ACCESS_ACTIONS.DOWNLOAD]: 'Download',
  [MEDIA_CONSTANTS.ACCESS_ACTIONS.EDIT]: 'Edit',
  [MEDIA_CONSTANTS.ACCESS_ACTIONS.DELETE]: 'Delete',
  [MEDIA_CONSTANTS.ACCESS_ACTIONS.SHARE]: 'Share',
} as const;

// Sort option labels
export const SORT_OPTION_LABELS = {
  [MEDIA_CONSTANTS.SORT_OPTIONS.FILE_NAME]: 'File Name',
  [MEDIA_CONSTANTS.SORT_OPTIONS.FILE_SIZE]: 'File Size',
  [MEDIA_CONSTANTS.SORT_OPTIONS.UPLOAD_DATE]: 'Upload Date',
  [MEDIA_CONSTANTS.SORT_OPTIONS.MODIFIED_DATE]: 'Modified Date',
  [MEDIA_CONSTANTS.SORT_OPTIONS.MEDIA_TYPE]: 'Media Type',
  [MEDIA_CONSTANTS.SORT_OPTIONS.STATUS]: 'Status',
} as const;

// Sort order labels
export const SORT_ORDER_LABELS = {
  [MEDIA_CONSTANTS.SORT_ORDERS.ASC]: 'Ascending',
  [MEDIA_CONSTANTS.SORT_ORDERS.DESC]: 'Descending',
} as const;

// Job priority labels
export const JOB_PRIORITY_LABELS = {
  [MEDIA_CONSTANTS.JOB_PRIORITIES.LOW]: 'Low',
  [MEDIA_CONSTANTS.JOB_PRIORITIES.NORMAL]: 'Normal',
  [MEDIA_CONSTANTS.JOB_PRIORITIES.HIGH]: 'High',
  [MEDIA_CONSTANTS.JOB_PRIORITIES.URGENT]: 'Urgent',
} as const;

// View mode labels
export const VIEW_MODE_LABELS = {
  [MEDIA_CONSTANTS.VIEW_MODES.GRID]: 'Grid View',
  [MEDIA_CONSTANTS.VIEW_MODES.LIST]: 'List View',
  [MEDIA_CONSTANTS.VIEW_MODES.GALLERY]: 'Gallery View',
  [MEDIA_CONSTANTS.VIEW_MODES.TIMELINE]: 'Timeline View',
} as const;

