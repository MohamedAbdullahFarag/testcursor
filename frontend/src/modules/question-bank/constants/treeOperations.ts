// Tree Operations Constants
// Defines constants for tree management operations, validation rules, and UI behavior

export const TREE_OPERATIONS = {
  // Maximum tree depth allowed
  MAX_DEPTH: 6,
  
  // Default page sizes for pagination
  DEFAULT_PAGE_SIZE: 50,
  MAX_PAGE_SIZE: 1000,
  
  // Search and filter limits
  MAX_SEARCH_RESULTS: 100,
  MAX_BULK_OPERATIONS: 100,
  
  // Tree validation thresholds
  MAX_ORPHANED_CATEGORIES: 10,
  MAX_CIRCULAR_REFERENCES: 0,
  
  // Performance thresholds
  MAX_CATEGORIES_FOR_INSTANT_LOAD: 500,
  LAZY_LOAD_THRESHOLD: 1000,
  
  // UI behavior constants
  EXPANSION_DELAY: 300,
  SEARCH_DELAY: 500,
  DRAG_DROP_DELAY: 100,
  
  // Animation durations
  EXPAND_ANIMATION_DURATION: 200,
  COLLAPSE_ANIMATION_DURATION: 150,
  DRAG_ANIMATION_DURATION: 100,
  
  // Tree node types
  NODE_TYPES: {
    CATEGORY: 'category',
    QUESTION: 'question',
    ROOT: 'root'
  },
  
  // Drop positions for drag and drop
  DROP_POSITIONS: {
    BEFORE: 'before',
    AFTER: 'after',
    INSIDE: 'inside'
  },
  
  // Category types mapping
  CATEGORY_TYPES: {
    SUBJECT: { id: 1, name: 'Subject', icon: '📚', color: '#3B82F6' },
    CHAPTER: { id: 2, name: 'Chapter', icon: '📖', color: '#10B981' },
    TOPIC: { id: 3, name: 'Topic', icon: '📝', color: '#F59E0B' },
    SUBTOPIC: { id: 4, name: 'Subtopic', icon: '🔍', color: '#8B5CF6' },
    SKILL: { id: 5, name: 'Skill', icon: '🎯', color: '#EF4444' },
    OBJECTIVE: { id: 6, name: 'Objective', icon: '🎯', color: '#06B6D4' }
  },
  
  // Category levels mapping
  CATEGORY_LEVELS: {
    LEVEL1: { id: 1, name: 'Level 1', maxChildren: 50 },
    LEVEL2: { id: 2, name: 'Level 2', maxChildren: 100 },
    LEVEL3: { id: 3, name: 'Level 3', maxChildren: 200 },
    LEVEL4: { id: 4, name: 'Level 4', maxChildren: 500 },
    LEVEL5: { id: 5, name: 'Level 5', maxChildren: 1000 },
    LEVEL6: { id: 6, name: 'Level 6', maxChildren: 2000 }
  },
  
  // Child handling strategies
  CHILD_HANDLING_STRATEGIES: {
    REASSIGN_TO_PARENT: { id: 1, name: 'Reassign to Parent', description: 'Move children to parent category' },
    DELETE_WITH_PARENT: { id: 2, name: 'Delete with Parent', description: 'Delete all children when parent is deleted' },
    REASSIGN_TO_ROOT: { id: 3, name: 'Reassign to Root', description: 'Move children to root level' },
    PREVENT_DELETION: { id: 4, name: 'Prevent Deletion', description: 'Prevent deletion if children exist' }
  },
  
  // Validation rules
  VALIDATION_RULES: {
    NAME_MIN_LENGTH: 2,
    NAME_MAX_LENGTH: 200,
    DESCRIPTION_MAX_LENGTH: 500,
    CODE_MAX_LENGTH: 50,
    METADATA_MAX_LENGTH: 2000,
    TREE_PATH_MAX_LENGTH: 500,
    COLOR_MAX_LENGTH: 20,
    ICON_MAX_LENGTH: 100
  },
  
  // Error messages
  ERROR_MESSAGES: {
    MAX_DEPTH_EXCEEDED: 'Maximum tree depth (6 levels) exceeded',
    CIRCULAR_REFERENCE: 'Circular reference detected in category hierarchy',
    INVALID_PARENT: 'Invalid parent category specified',
    DUPLICATE_CODE: 'Category code must be unique',
    INVALID_TREE_PATH: 'Invalid tree path format',
    CATEGORY_NOT_FOUND: 'Category not found',
    OPERATION_FAILED: 'Operation failed',
    VALIDATION_FAILED: 'Validation failed',
    PERMISSION_DENIED: 'Permission denied for this operation',
    NETWORK_ERROR: 'Network error occurred',
    SERVER_ERROR: 'Server error occurred'
  },
  
  // Success messages
  SUCCESS_MESSAGES: {
    CATEGORY_CREATED: 'Category created successfully',
    CATEGORY_UPDATED: 'Category updated successfully',
    CATEGORY_DELETED: 'Category deleted successfully',
    CATEGORY_MOVED: 'Category moved successfully',
    CATEGORY_COPIED: 'Category copied successfully',
    CATEGORIES_REORDERED: 'Categories reordered successfully',
    TREE_VALIDATED: 'Tree structure validated successfully',
    OPERATION_COMPLETED: 'Operation completed successfully'
  },
  
  // Warning messages
  WARNING_MESSAGES: {
    LARGE_TREE_LOADING: 'Loading large tree structure, please wait...',
    MANY_CHILDREN: 'This category has many children, operations may take time',
    DEEP_HIERARCHY: 'Deep hierarchy detected, consider flattening structure',
    INACTIVE_CATEGORIES: 'Some categories are inactive',
    ORPHANED_CATEGORIES: 'Orphaned categories detected',
    DUPLICATE_NAMES: 'Duplicate category names detected'
  },
  
  // Info messages
  INFO_MESSAGES: {
    TREE_LOADED: 'Tree structure loaded successfully',
    NO_CATEGORIES: 'No categories found',
    SEARCHING: 'Searching categories...',
    VALIDATING: 'Validating tree structure...',
    OPERATION_IN_PROGRESS: 'Operation in progress...',
    SAVING_CHANGES: 'Saving changes...'
  },
  
  // UI constants
  UI: {
    // Tree view settings
    TREE_VIEW: {
      INDENT_SIZE: 24,
      NODE_HEIGHT: 40,
      ICON_SIZE: 20,
      EXPAND_ICON_SIZE: 16,
      SELECTION_INDICATOR_WIDTH: 4
    },
    
    // Search settings
    SEARCH: {
      MIN_QUERY_LENGTH: 2,
      MAX_QUERY_LENGTH: 100,
      SUGGESTION_LIMIT: 10,
      HIGHLIGHT_CLASS: 'search-highlight'
    },
    
    // Drag and drop settings
    DRAG_DROP: {
      DRAG_OPACITY: 0.6,
      DROP_ZONE_HIGHLIGHT_COLOR: '#3B82F6',
      DROP_ZONE_HIGHLIGHT_OPACITY: 0.3,
      DRAG_PREVIEW_SCALE: 0.8
    },
    
    // Modal settings
    MODAL: {
      DEFAULT_WIDTH: 600,
      LARGE_WIDTH: 800,
      SMALL_WIDTH: 400,
      ANIMATION_DURATION: 200
    },
    
    // Form settings
    FORM: {
      LABEL_WIDTH: 120,
      FIELD_SPACING: 16,
      SECTION_SPACING: 24,
      BUTTON_SPACING: 12
    }
  },
  
  // API endpoints
  API_ENDPOINTS: {
    TREE: '/api/question-bank-tree/tree',
    CATEGORIES: '/api/question-bank-tree/categories',
    CATEGORY: '/api/question-bank-tree/categories/{id}',
    CHILDREN: '/api/question-bank-tree/categories/{id}/children',
    PARENT: '/api/question-bank-tree/categories/{id}/parent',
    ANCESTORS: '/api/question-bank-tree/categories/{id}/ancestors',
    DESCENDANTS: '/api/question-bank-tree/categories/{id}/descendants',
    SIBLINGS: '/api/question-bank-tree/categories/{id}/siblings',
    MOVE: '/api/question-bank-tree/categories/{id}/move',
    COPY: '/api/question-bank-tree/categories/{id}/copy',
    REORDER: '/api/question-bank-tree/categories/reorder',
    SEARCH: '/api/question-bank-tree/categories/search',
    FIND_BY_PATH: '/api/question-bank-tree/categories/find-by-path',
    STATISTICS: '/api/question-bank-tree/statistics',
    VALIDATE_STRUCTURE: '/api/question-bank-tree/validate/structure',
    VALIDATE_HIERARCHY: '/api/question-bank-tree/validate/hierarchy/{id}',
    BULK_CREATE: '/api/question-bank-tree/categories/bulk',
    RECENTLY_MODIFIED: '/api/question-bank-tree/categories/recently-modified'
  },
  
  // Local storage keys
  STORAGE_KEYS: {
    EXPANDED_NODES: 'question-bank-tree-expanded-nodes',
    SELECTED_NODE: 'question-bank-tree-selected-node',
    SEARCH_QUERY: 'question-bank-tree-search-query',
    FILTERS: 'question-bank-tree-filters',
    VIEW_SETTINGS: 'question-bank-tree-view-settings',
    USER_PREFERENCES: 'question-bank-tree-user-preferences'
  },
  
  // Event names
  EVENTS: {
    TREE_LOADED: 'question-bank-tree-loaded',
    CATEGORY_SELECTED: 'question-bank-category-selected',
    CATEGORY_CREATED: 'question-bank-category-created',
    CATEGORY_UPDATED: 'question-bank-category-updated',
    CATEGORY_DELETED: 'question-bank-category-deleted',
    CATEGORY_MOVED: 'question-bank-category-moved',
    CATEGORY_COPIED: 'question-bank-category-copied',
    TREE_CHANGED: 'question-bank-tree-changed',
    SEARCH_PERFORMED: 'question-bank-search-performed',
    VALIDATION_COMPLETED: 'question-bank-validation-completed'
  }
} as const;

// Export individual constants for easier access
export const MAX_TREE_DEPTH = TREE_OPERATIONS.MAX_DEPTH;
export const DEFAULT_PAGE_SIZE = TREE_OPERATIONS.DEFAULT_PAGE_SIZE;
export const MAX_SEARCH_RESULTS = TREE_OPERATIONS.MAX_SEARCH_RESULTS;
export const TREE_NODE_TYPES = TREE_OPERATIONS.NODE_TYPES;
export const DROP_POSITIONS = TREE_OPERATIONS.DROP_POSITIONS;
export const CATEGORY_TYPES = TREE_OPERATIONS.CATEGORY_TYPES;
export const CATEGORY_LEVELS = TREE_OPERATIONS.CATEGORY_LEVELS;
export const CHILD_HANDLING_STRATEGIES = TREE_OPERATIONS.CHILD_HANDLING_STRATEGIES;
export const VALIDATION_RULES = TREE_OPERATIONS.VALIDATION_RULES;
export const ERROR_MESSAGES = TREE_OPERATIONS.ERROR_MESSAGES;
export const SUCCESS_MESSAGES = TREE_OPERATIONS.SUCCESS_MESSAGES;
export const WARNING_MESSAGES = TREE_OPERATIONS.WARNING_MESSAGES;
export const INFO_MESSAGES = TREE_OPERATIONS.INFO_MESSAGES;
export const UI_CONSTANTS = TREE_OPERATIONS.UI;
export const API_ENDPOINTS = TREE_OPERATIONS.API_ENDPOINTS;
export const STORAGE_KEYS = TREE_OPERATIONS.STORAGE_KEYS;
export const EVENT_NAMES = TREE_OPERATIONS.EVENTS;
