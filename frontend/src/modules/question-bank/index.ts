// Components
export { QuestionBankTree } from './components/QuestionBankTree';
export { TreeNode } from './components/TreeNode';
export { TreeSearch } from './components/TreeSearch';
export { CategoryManager } from './components/CategoryManager';
export { BreadcrumbNavigation } from './components/BreadcrumbNavigation';
export { TreeActions } from './components/TreeActions';
export { CategoryFilters } from './components/CategoryFilters';
export { TreeDragDrop } from './components/TreeDragDrop';

// Hooks
export { useQuestionBankTree } from './hooks/useQuestionBankTree';

// Services
export { questionBankTreeService } from './services/questionBankTreeService';

// Types
export type {
  QuestionBankCategoryDto,
  QuestionBankTreeDto,
  CreateCategoryDto,
  UpdateCategoryDto,
  MoveCategoryDto,
  CategorySearchDto,
  CategoryFilterDto,
  TreeOperationDto,
  CategoryType,
  CategoryLevel,
  PagedResult,
  TreeValidationResult
} from './types/questionBankTree.types';

// Constants
export { TREE_OPERATIONS, CATEGORY_TYPES, CATEGORY_LEVELS } from './constants/treeOperations';

// Module configuration
export const QUESTION_BANK_MODULE = 'question-bank';
export const DEFAULT_PAGE_SIZE = 25;
export const SEARCH_DEBOUNCE_MS = 300;
export const MAX_TREE_DEPTH = 6;

/**
 * Module configuration object
 */
export const questionBankConfig = {
  module: QUESTION_BANK_MODULE,
  defaultPageSize: DEFAULT_PAGE_SIZE,
  searchDebounceMs: SEARCH_DEBOUNCE_MS,
  maxTreeDepth: MAX_TREE_DEPTH,
  supportedExportFormats: ['csv', 'excel', 'json', 'xml'] as const,
  supportedLanguages: ['en', 'ar'] as const,
  categoryTypes: ['Subject', 'Chapter', 'Topic', 'Subtopic', 'Skill', 'Objective'] as const,
  categoryLevels: [1, 2, 3, 4, 5, 6] as const,
} as const;
