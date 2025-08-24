// Question Management Constants

// API Endpoints
export const QUESTION_API_ENDPOINTS = {
  // Questions
  QUESTIONS: '/api/questions',
  QUESTION_BY_ID: (id: number) => `/api/questions/${id}`,
  QUESTION_SEARCH: '/api/questions/search',
  QUESTION_VALIDATE: (id: number) => `/api/questions/${id}/validate`,
  
  // Question Banks
  QUESTION_BANKS: '/api/questionbanks',
  QUESTION_BANK_BY_ID: (id: number) => `/api/questionbanks/${id}`,
  QUESTIONS_IN_BANK: (bankId: number) => `/api/questionbanks/${bankId}/questions`,
  ADD_QUESTION_TO_BANK: (bankId: number, questionId: number) => 
    `/api/questionbanks/${bankId}/questions/${questionId}`,
  
  // Question Search
  QUESTION_SEARCH_ENDPOINT: '/api/questionsearch',
  SEARCH_SUGGESTIONS: '/api/questionsearch/suggestions',
  SEARCH_STATISTICS: '/api/questionsearch/statistics',
  
  // Question Import/Export
  QUESTION_IMPORT: '/api/questionimport',
  QUESTION_EXPORT: '/api/questionexport',
  IMPORT_HISTORY: '/api/questionimport/history',
  EXPORT_HISTORY: '/api/questionexport/history',
  SUPPORTED_FORMATS: '/api/questionimport/formats',
  EXPORT_TEMPLATES: '/api/questionexport/templates',
  
  // Question Validation
  QUESTION_VALIDATION: '/api/questionvalidation',
  VALIDATE_QUESTION: (id: number) => `/api/questionvalidation/${id}`,
  VALIDATION_CRITERIA: '/api/questionvalidation/criteria',
  
  // Question Versions
  QUESTION_VERSIONS: '/api/questionversions',
  QUESTION_VERSION_BY_ID: (id: number) => `/api/questionversions/${id}`,
  VERSIONS_BY_QUESTION: (questionId: number) => `/api/questionversions/question/${questionId}`,
  CURRENT_VERSION: (questionId: number) => `/api/questionversions/question/${questionId}/current`,
  ACTIVATE_VERSION: (id: number) => `/api/questionversions/${id}/activate`,
  PUBLISH_VERSION: (id: number) => `/api/questionversions/${id}/publish`,
  ARCHIVE_VERSION: (id: number) => `/api/questionversions/${id}/archive`,
  COMPARE_VERSIONS: '/api/questionversions/compare',
  REVERT_TO_VERSION: (questionId: number, versionId: number) => 
    `/api/questionversions/question/${questionId}/revert/${versionId}`,
  
  // Question Review
  QUESTION_REVIEW: '/api/questionreview',
  REVIEW_BY_ID: (id: number) => `/api/questionreview/${id}`,
  REVIEWS_BY_QUESTION: (questionId: number) => `/api/questionreview/question/${questionId}`,
  REVIEWS_BY_REVIEWER: (reviewerId: number) => `/api/questionreview/reviewer/${reviewerId}`,
  PENDING_REVIEWS: '/api/questionreview/pending',
  REVIEWS_BY_STATUS: (status: string) => `/api/questionreview/status/${status}`,
  SUBMIT_REVIEW: (id: number) => `/api/questionreview/${id}/submit`,
  APPROVE_REVIEW: (id: number) => `/api/questionreview/${id}/approve`,
  REJECT_REVIEW: (id: number) => `/api/questionreview/${id}/reject`,
  REQUEST_REVISION: (id: number) => `/api/questionreview/${id}/request-revision`,
  ASSIGN_REVIEWER: (id: number) => `/api/questionreview/${id}/assign-reviewer`,
  REVIEW_WORKFLOW: (id: number) => `/api/questionreview/${id}/workflow`,
  REVIEW_TIMELINE: (id: number) => `/api/questionreview/${id}/timeline`,
  ADD_REVIEW_COMMENT: (id: number) => `/api/questionreview/${id}/comments`,
  REVIEW_COMMENTS: (id: number) => `/api/questionreview/${id}/comments`,
  REVIEW_DASHBOARD: '/api/questionreview/dashboard',
  REVIEW_REPORTS: '/api/questionreview/reports',
  
  // Question Creation Workflow
  QUESTION_CREATION_WORKFLOW: '/api/questioncreationworkflow',
  WORKFLOW_BY_ID: (id: number) => `/api/questioncreationworkflow/${id}`,
  WORKFLOWS_BY_CREATOR: (creatorId: number) => `/api/questioncreationworkflow/creator/${creatorId}`,
  WORKFLOWS_BY_STATUS: (status: string) => `/api/questioncreationworkflow/status/${status}`,
  ACTIVE_WORKFLOWS: '/api/questioncreationworkflow/active',
  START_WORKFLOW: (id: number) => `/api/questioncreationworkflow/${id}/start`,
  PAUSE_WORKFLOW: (id: number) => `/api/questioncreationworkflow/${id}/pause`,
  RESUME_WORKFLOW: (id: number) => `/api/questioncreationworkflow/${id}/resume`,
  COMPLETE_WORKFLOW: (id: number) => `/api/questioncreationworkflow/${id}/complete`,
  CANCEL_WORKFLOW: (id: number) => `/api/questioncreationworkflow/${id}/cancel`,
  NEXT_STEP: (id: number) => `/api/questioncreationworkflow/${id}/next-step`,
  PREVIOUS_STEP: (id: number) => `/api/questioncreationworkflow/${id}/previous-step`,
  WORKFLOW_STEPS: (id: number) => `/api/questioncreationworkflow/${id}/steps`,
  CURRENT_STEP: (id: number) => `/api/questioncreationworkflow/${id}/current-step`,
  WORKFLOW_PROGRESS: (id: number) => `/api/questioncreationworkflow/${id}/progress`,
  WORKFLOW_TIMELINE: (id: number) => `/api/questioncreationworkflow/${id}/timeline`,
  WORKFLOW_STATISTICS: '/api/questioncreationworkflow/statistics',
  WORKFLOW_DASHBOARD: '/api/questioncreationworkflow/dashboard',
  WORKFLOW_REPORTS: '/api/questioncreationworkflow/reports',
} as const;

// HTTP Methods
export const HTTP_METHODS = {
  GET: 'GET',
  POST: 'POST',
  PUT: 'PUT',
  DELETE: 'DELETE',
  PATCH: 'PATCH',
} as const;

// HTTP Status Codes
export const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  NO_CONTENT: 204,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  CONFLICT: 409,
  UNPROCESSABLE_ENTITY: 422,
  INTERNAL_SERVER_ERROR: 500,
  SERVICE_UNAVAILABLE: 503,
} as const;

// Pagination
export const PAGINATION = {
  DEFAULT_PAGE: 1,
  DEFAULT_PAGE_SIZE: 20,
  MIN_PAGE_SIZE: 5,
  MAX_PAGE_SIZE: 100,
  PAGE_SIZE_OPTIONS: [10, 20, 50, 100],
} as const;

// Sorting
export const SORTING = {
  DEFAULT_FIELD: 'createdAt',
  DEFAULT_DIRECTION: 'desc',
  FIELDS: {
    CREATED_AT: 'createdAt',
    UPDATED_AT: 'updatedAt',
    TEXT: 'text',
    TYPE: 'type',
    DIFFICULTY: 'difficulty',
    STATUS: 'status',
    CATEGORY: 'categoryId',
    CREATED_BY: 'createdBy',
    SCORE: 'score',
    USAGE_COUNT: 'usageCount',
  },
  DIRECTIONS: {
    ASC: 'asc',
    DESC: 'desc',
  },
} as const;

// Filter Options
export const FILTER_OPTIONS = {
  DATE_RANGE_PRESETS: {
    TODAY: 'today',
    YESTERDAY: 'yesterday',
    LAST_7_DAYS: 'last7Days',
    LAST_30_DAYS: 'last30Days',
    THIS_MONTH: 'thisMonth',
    LAST_MONTH: 'lastMonth',
    THIS_YEAR: 'thisYear',
    LAST_YEAR: 'lastYear',
    CUSTOM: 'custom',
  },
  STATUS_GROUPS: {
    ACTIVE: ['Draft', 'PendingReview', 'UnderReview', 'Approved', 'Published'],
    INACTIVE: ['Rejected', 'Archived'],
    WORKFLOW: ['NotStarted', 'InProgress', 'Paused', 'Completed', 'Cancelled'],
  },
  DIFFICULTY_GROUPS: {
    EASY: ['Easy'],
    MEDIUM: ['Medium'],
    HARD: ['Hard', 'Expert'],
  },
} as const;

// Validation Rules
export const VALIDATION_RULES = {
  QUESTION: {
    TEXT_MIN_LENGTH: 10,
    TEXT_MAX_LENGTH: 2000,
    EXPLANATION_MAX_LENGTH: 1000,
    TAGS_MIN_COUNT: 1,
    TAGS_MAX_COUNT: 10,
    TAG_MAX_LENGTH: 50,
    OPTIONS_MIN_COUNT: 2,
    OPTIONS_MAX_COUNT: 10,
    OPTION_TEXT_MAX_LENGTH: 500,
    METADATA_MAX_LENGTH: 1000,
  },
  QUESTION_BANK: {
    NAME_MIN_LENGTH: 3,
    NAME_MAX_LENGTH: 100,
    DESCRIPTION_MAX_LENGTH: 500,
    TAGS_MAX_COUNT: 20,
  },
  WORKFLOW: {
    STEP_NAME_MAX_LENGTH: 100,
    STEP_DESCRIPTION_MAX_LENGTH: 500,
    COMMENT_MAX_LENGTH: 1000,
  },
} as const;

// Error Messages
export const ERROR_MESSAGES = {
  COMMON: {
    REQUIRED_FIELD: 'This field is required',
    INVALID_FORMAT: 'Invalid format',
    MIN_LENGTH: (field: string, min: number) => `${field} must be at least ${min} characters`,
    MAX_LENGTH: (field: string, max: number) => `${field} must be no more than ${max} characters`,
    MIN_VALUE: (field: string, min: number) => `${field} must be at least ${min}`,
    MAX_VALUE: (field: string, max: number) => `${field} must be no more than ${max}`,
    INVALID_EMAIL: 'Please enter a valid email address',
    INVALID_URL: 'Please enter a valid URL',
    INVALID_DATE: 'Please enter a valid date',
    INVALID_NUMBER: 'Please enter a valid number',
    NETWORK_ERROR: 'Network error. Please check your connection and try again.',
    SERVER_ERROR: 'Server error. Please try again later.',
    UNAUTHORIZED: 'You are not authorized to perform this action.',
    FORBIDDEN: 'Access denied. You do not have permission to perform this action.',
    NOT_FOUND: 'The requested resource was not found.',
    VALIDATION_ERROR: 'Please check your input and try again.',
  },
  QUESTIONS: {
    CREATE_FAILED: 'Failed to create question. Please try again.',
    UPDATE_FAILED: 'Failed to update question. Please try again.',
    DELETE_FAILED: 'Failed to delete question. Please try again.',
    LOAD_FAILED: 'Failed to load questions. Please try again.',
    SEARCH_FAILED: 'Search failed. Please try again.',
    VALIDATION_FAILED: 'Question validation failed. Please check your input.',
    DUPLICATE_OPTION: 'Duplicate answer options are not allowed.',
    INVALID_CORRECT_ANSWER: 'Please select at least one correct answer.',
    OPTIONS_REQUIRED: 'Answer options are required for this question type.',
    EXPLANATION_REQUIRED: 'Explanation is required for this question type.',
  },
  QUESTION_BANKS: {
    CREATE_FAILED: 'Failed to create question bank. Please try again.',
    UPDATE_FAILED: 'Failed to update question bank. Please try again.',
    DELETE_FAILED: 'Failed to delete question bank. Please try again.',
    LOAD_FAILED: 'Failed to load question banks. Please try again.',
    ADD_QUESTION_FAILED: 'Failed to add question to bank. Please try again.',
    REMOVE_QUESTION_FAILED: 'Failed to remove question from bank. Please try again.',
    NAME_EXISTS: 'A question bank with this name already exists.',
  },
  WORKFLOW: {
    START_FAILED: 'Failed to start workflow. Please try again.',
    STEP_FAILED: 'Failed to proceed to next step. Please try again.',
    COMPLETE_FAILED: 'Failed to complete workflow. Please try again.',
    CANCEL_FAILED: 'Failed to cancel workflow. Please try again.',
    INVALID_STEP: 'Invalid workflow step.',
    STEP_REQUIRED: 'This step is required to continue.',
    WORKFLOW_LOCKED: 'Workflow is currently locked. Please try again later.',
  },
  IMPORT_EXPORT: {
    IMPORT_FAILED: 'Import failed. Please check your file and try again.',
    EXPORT_FAILED: 'Export failed. Please try again.',
    INVALID_FILE_FORMAT: 'Invalid file format. Please use a supported format.',
    FILE_TOO_LARGE: 'File is too large. Please use a smaller file.',
    INVALID_FILE_STRUCTURE: 'Invalid file structure. Please check the file format.',
    NO_QUESTIONS_FOUND: 'No questions found in the file.',
    PARTIAL_IMPORT: 'Some questions could not be imported. Please check the errors.',
  },
} as const;

// Success Messages
export const SUCCESS_MESSAGES = {
  COMMON: {
    CREATED: 'Successfully created',
    UPDATED: 'Successfully updated',
    DELETED: 'Successfully deleted',
    SAVED: 'Successfully saved',
    SUBMITTED: 'Successfully submitted',
    APPROVED: 'Successfully approved',
    REJECTED: 'Successfully rejected',
    PUBLISHED: 'Successfully published',
    ARCHIVED: 'Successfully archived',
    ACTIVATED: 'Successfully activated',
    DEACTIVATED: 'Successfully deactivated',
  },
  QUESTIONS: {
    CREATED: 'Question created successfully',
    UPDATED: 'Question updated successfully',
    DELETED: 'Question deleted successfully',
    PUBLISHED: 'Question published successfully',
    APPROVED: 'Question approved successfully',
    REJECTED: 'Question rejected successfully',
    VALIDATED: 'Question validated successfully',
    VERSIONED: 'Question version created successfully',
    IMPORTED: 'Questions imported successfully',
    EXPORTED: 'Questions exported successfully',
  },
  QUESTION_BANKS: {
    CREATED: 'Question bank created successfully',
    UPDATED: 'Question bank updated successfully',
    DELETED: 'Question bank deleted successfully',
    QUESTION_ADDED: 'Question added to bank successfully',
    QUESTION_REMOVED: 'Question removed from bank successfully',
  },
  WORKFLOW: {
    STARTED: 'Workflow started successfully',
    STEP_COMPLETED: 'Step completed successfully',
    COMPLETED: 'Workflow completed successfully',
    CANCELLED: 'Workflow cancelled successfully',
    PAUSED: 'Workflow paused successfully',
    RESUMED: 'Workflow resumed successfully',
  },
} as const;

// UI Constants
export const UI = {
  ANIMATION: {
    DURATION: {
      FAST: 150,
      NORMAL: 300,
      SLOW: 500,
    },
    EASING: {
      EASE_IN: 'ease-in',
      EASE_OUT: 'ease-out',
      EASE_IN_OUT: 'ease-in-out',
    },
  },
  BREAKPOINTS: {
    XS: 0,
    SM: 600,
    MD: 960,
    LG: 1280,
    XL: 1920,
  },
  SPACING: {
    XS: 4,
    SM: 8,
    MD: 16,
    LG: 24,
    XL: 32,
    XXL: 48,
  },
  BORDER_RADIUS: {
    SM: 4,
    MD: 8,
    LG: 12,
    XL: 16,
    ROUND: '50%',
  },
  SHADOWS: {
    SM: '0 1px 3px rgba(0,0,0,0.12), 0 1px 2px rgba(0,0,0,0.24)',
    MD: '0 3px 6px rgba(0,0,0,0.16), 0 3px 6px rgba(0,0,0,0.23)',
    LG: '0 10px 20px rgba(0,0,0,0.19), 0 6px 6px rgba(0,0,0,0.23)',
    XL: '0 14px 28px rgba(0,0,0,0.25), 0 10px 10px rgba(0,0,0,0.22)',
  },
  Z_INDEX: {
    DROPDOWN: 1000,
    STICKY: 1020,
    FIXED: 1030,
    MODAL_BACKDROP: 1040,
    MODAL: 1050,
    POPOVER: 1060,
    TOOLTIP: 1070,
  },
} as const;

// Form Constants
export const FORM = {
  DEBOUNCE_DELAY: 300,
  AUTO_SAVE_DELAY: 5000,
  MAX_FILE_SIZE: 10 * 1024 * 1024, // 10MB
  ALLOWED_FILE_TYPES: {
    IMAGE: ['.jpg', '.jpeg', '.png', '.gif', '.webp'],
    DOCUMENT: ['.pdf', '.doc', '.docx', '.txt'],
    SPREADSHEET: ['.xls', '.xlsx', '.csv'],
    PRESENTATION: ['.ppt', '.pptx'],
  },
  VALIDATION: {
    SHOW_ON_BLUR: true,
    SHOW_ON_CHANGE: false,
    SHOW_ON_SUBMIT: true,
  },
} as const;

// Local Storage Keys
export const STORAGE_KEYS = {
  QUESTION_FILTERS: 'question_filters',
  QUESTION_SORTING: 'question_sorting',
  QUESTION_PAGINATION: 'question_pagination',
  QUESTION_SELECTION: 'question_selection',
  WORKFLOW_STATE: 'workflow_state',
  DRAFT_QUESTION: 'draft_question',
  USER_PREFERENCES: 'user_preferences',
  RECENT_SEARCHES: 'recent_searches',
  FAVORITE_QUESTIONS: 'favorite_questions',
} as const;

// Feature Flags
export const FEATURE_FLAGS = {
  ADVANCED_SEARCH: true,
  BULK_OPERATIONS: true,
  QUESTION_TEMPLATES: true,
  QUESTION_VERSIONING: true,
  QUESTION_REVIEW: true,
  WORKFLOW_MANAGEMENT: true,
  ANALYTICS_DASHBOARD: true,
  IMPORT_EXPORT: true,
  REAL_TIME_COLLABORATION: false,
  AI_ASSISTED_QUESTION_CREATION: false,
  ADVANCED_VALIDATION: true,
  PERFORMANCE_MONITORING: true,
} as const;

// Performance Constants
export const PERFORMANCE = {
  DEBOUNCE_DELAY: 300,
  THROTTLE_DELAY: 100,
  INFINITE_SCROLL_THRESHOLD: 100,
  VIRTUALIZATION_THRESHOLD: 1000,
  CACHE_TTL: 5 * 60 * 1000, // 5 minutes
  MAX_CONCURRENT_REQUESTS: 5,
  REQUEST_TIMEOUT: 30000, // 30 seconds
  RETRY_ATTEMPTS: 3,
  RETRY_DELAY: 1000, // 1 second
} as const;

// Accessibility Constants
export const ACCESSIBILITY = {
  ARIA_LABELS: {
    QUESTION_CARD: 'Question card',
    QUESTION_ACTIONS: 'Question actions',
    QUESTION_FILTERS: 'Question filters',
    QUESTION_SEARCH: 'Question search',
    QUESTION_LIST: 'Question list',
    QUESTION_PAGINATION: 'Question pagination',
    WORKFLOW_STEPS: 'Workflow steps',
    WORKFLOW_PROGRESS: 'Workflow progress',
  },
  KEYBOARD_NAVIGATION: {
    TAB_INDEX: {
      INTERACTIVE: 0,
      FOCUSABLE: 0,
      NON_INTERACTIVE: -1,
    },
    SHORTCUTS: {
      SEARCH: 'Ctrl+K',
      NEW_QUESTION: 'Ctrl+N',
      SAVE: 'Ctrl+S',
      CANCEL: 'Escape',
      NEXT: 'Tab',
      PREVIOUS: 'Shift+Tab',
    },
  },
  SCREEN_READER: {
    ANNOUNCEMENTS: {
      QUESTION_LOADED: 'Question loaded',
      QUESTIONS_LOADED: 'Questions loaded',
      FILTERS_APPLIED: 'Filters applied',
      SORTING_APPLIED: 'Sorting applied',
      PAGE_CHANGED: 'Page changed',
      WORKFLOW_STEP_COMPLETED: 'Workflow step completed',
    },
  },
} as const;

// Internationalization Constants
export const I18N = {
  DEFAULT_LOCALE: 'en',
  SUPPORTED_LOCALES: ['en', 'ar'],
  NAMESPACES: {
    COMMON: 'common',
    QUESTIONS: 'questions',
    QUESTION_BANKS: 'questionBanks',
    WORKFLOW: 'workflow',
    VALIDATION: 'validation',
    ERRORS: 'errors',
    SUCCESS: 'success',
  },
  DATE_FORMATS: {
    SHORT: 'MM/DD/YYYY',
    MEDIUM: 'MMM DD, YYYY',
    LONG: 'MMMM DD, YYYY',
    TIME: 'HH:mm:ss',
    DATETIME: 'MMM DD, YYYY HH:mm',
  },
  NUMBER_FORMATS: {
    DECIMAL: {
      MIN_FRACTION_DIGITS: 0,
      MAX_FRACTION_DIGITS: 2,
    },
    PERCENT: {
      MIN_FRACTION_DIGITS: 0,
      MAX_FRACTION_DIGITS: 1,
    },
  },
} as const;

// Export all constants
export default {
  QUESTION_API_ENDPOINTS,
  HTTP_METHODS,
  HTTP_STATUS,
  PAGINATION,
  SORTING,
  FILTER_OPTIONS,
  VALIDATION_RULES,
  ERROR_MESSAGES,
  SUCCESS_MESSAGES,
  UI,
  FORM,
  STORAGE_KEYS,
  FEATURE_FLAGS,
  PERFORMANCE,
  ACCESSIBILITY,
  I18N,
};
