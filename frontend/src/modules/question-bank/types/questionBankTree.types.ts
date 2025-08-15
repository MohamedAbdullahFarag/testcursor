// Question Bank Tree Management Types
// Comprehensive type definitions for tree structure, categories, and operations

export interface QuestionBankCategoryDto {
  categoryId: number;
  name: string;
  description?: string;
  parentCategoryId?: number;
  treePath?: string;
  treeLevel: number;
  sortOrder: number;
  isActive: boolean;
  color?: string;
  icon?: string;
  metadata?: string;
  questionCount: number;
  createdAt: string;
  modifiedAt?: string;
  parentCategoryName?: string;
  children: QuestionBankCategoryDto[];
  hasChildren?: boolean;
}

export interface CreateQuestionBankCategoryDto {
  name: string;
  description?: string;
  parentCategoryId?: number;
  sortOrder?: number;
  isActive?: boolean;
  color?: string;
  icon?: string;
  metadata?: string;
}

export interface UpdateQuestionBankCategoryDto {
  name: string;
  description?: string;
  sortOrder?: number;
  isActive?: boolean;
  color?: string;
  icon?: string;
  metadata?: string;
}

export interface QuestionBankTreeDto {
  categories: QuestionBankCategoryDto[];
  totalCategories: number;
  maxDepth: number;
  lastModified: string;
}

export interface TreeOperationDto {
  categoryId: number;
  operation: 'move' | 'copy' | 'delete' | 'reorder';
  targetParentId?: number;
  newName?: string;
  newSortOrder?: number;
}

export interface MoveCategoryDto {
  newParentId?: number;
  newName?: string;
  newSortOrder?: number;
}

export interface CopyCategoryDto {
  newParentId?: number;
  newName?: string;
  includeChildren?: boolean;
  includeQuestions?: boolean;
}

export interface ReorderCategoriesDto {
  categoryIds: number[];
  parentId?: number;
}

export interface CategorySearchDto {
  query: string;
  parentId?: number;
  includeInactive?: boolean;
  maxResults?: number;
  includeQuestions?: boolean;
}

export interface CategoryFilterDto {
  parentId?: number;
  isActive?: boolean;
  treeLevel?: number;
  hasQuestions?: boolean;
  createdAfter?: string;
  createdBefore?: string;
  modifiedAfter?: string;
  modifiedBefore?: string;
  page?: number;
  pageSize?: number;
}

export interface TreeValidationResultDto {
  isValid: boolean;
  errors: string[];
  warnings: string[];
  totalCategories: number;
  maxDepth: number;
  orphanedCategories: number[];
  circularReferences: string[];
  validationTimestamp: string;
}

export interface TreeStatisticsDto {
  totalCategories: number;
  activeCategories: number;
  inactiveCategories: number;
  maxDepth: number;
  averageDepth: number;
  categoriesWithQuestions: number;
  totalQuestions: number;
  recentlyModified: number;
  lastModified: string;
}

export interface QuestionCategorizationDto {
  questionCategorizationId: number;
  questionId: number;
  categoryId: number;
  isPrimary: boolean;
  assignedAt: string;
  assignedBy: number;
  notes?: string;
  category?: QuestionBankCategoryDto;
}

export interface BulkOperationDto {
  categoryIds: number[];
  operation: 'activate' | 'deactivate' | 'delete' | 'move' | 'copy';
  targetParentId?: number;
  newNames?: Record<number, string>;
}

export interface TreeSearchResultDto {
  categories: QuestionBankCategoryDto[];
  questions: any[]; // Question type will be defined separately
  totalResults: number;
  searchQuery: string;
  searchTimestamp: string;
}

export interface CategoryTemplateDto {
  templateId: number;
  name: string;
  description?: string;
  structure: QuestionBankCategoryDto[];
  isSystemTemplate: boolean;
  createdBy?: number;
  createdAt: string;
}

export interface CreateCategoryFromTemplateDto {
  templateId: number;
  name: string;
  description?: string;
  parentCategoryId?: number;
  customizations?: Record<string, any>;
}

// Enums
export enum CategoryType {
  Subject = 1,
  Chapter = 2,
  Topic = 3,
  Subtopic = 4,
  Skill = 5,
  Objective = 6
}

export enum CategoryLevel {
  Level1 = 1,
  Level2 = 2,
  Level3 = 3,
  Level4 = 4,
  Level5 = 5,
  Level6 = 6
}

export enum ChildHandlingStrategy {
  ReassignToParent = 1,
  DeleteWithParent = 2,
  ReassignToRoot = 3,
  PreventDeletion = 4
}

// API Response Types
export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
  errors?: string[];
}

export interface PagedResult<T> {
  data: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// Tree Node Types for UI Components
export interface TreeNodeData {
  id: string;
  label: string;
  type: 'category' | 'question';
  data: QuestionBankCategoryDto | any;
  children?: TreeNodeData[];
  isExpanded?: boolean;
  isSelected?: boolean;
  isDraggable?: boolean;
  isDroppable?: boolean;
}

export interface TreeDragDropData {
  draggedNode: TreeNodeData;
  targetNode: TreeNodeData;
  dropPosition: 'before' | 'after' | 'inside';
}

// Event Types
export interface CategoryEvent {
  type: 'created' | 'updated' | 'deleted' | 'moved' | 'copied';
  category: QuestionBankCategoryDto;
  previousData?: Partial<QuestionBankCategoryDto>;
  timestamp: string;
}

export interface TreeChangeEvent {
  type: 'structure' | 'content' | 'selection' | 'expansion';
  data: any;
  timestamp: string;
}

// Utility Types
export type CategoryId = number;
export type TreePath = string;
export type SortOrder = number;

// Component Props Types
export interface QuestionBankTreeProps {
  onCategorySelect?: (category: QuestionBankCategoryDto) => void;
  onQuestionSelect?: (questionId: number) => void;
  selectedCategoryId?: number;
  readonly?: boolean;
  showQuestions?: boolean;
  enableDragDrop?: boolean;
  maxDepth?: number;
  className?: string;
  initialExpandedNodes?: string[];
  searchEnabled?: boolean;
  filterEnabled?: boolean;
}

export interface TreeNodeProps {
  category: QuestionBankCategoryDto;
  isSelected: boolean;
  isExpanded: boolean;
  readonly?: boolean;
  showQuestions?: boolean;
  enableDragDrop?: boolean;
  onCategoryCreate: (parentId: number | null, data: CreateQuestionBankCategoryDto) => Promise<void>;
  onCategoryUpdate: (categoryId: number, data: UpdateQuestionBankCategoryDto) => Promise<void>;
  onCategoryDelete: (categoryId: number) => Promise<boolean>;
  onCategoryMove: (categoryId: number, data: MoveCategoryDto) => Promise<boolean>;
  onQuestionAssign: (questionId: number, categoryId: number, isPrimary?: boolean) => Promise<boolean>;
  onQuestionRemove: (questionId: number, categoryId: number) => Promise<boolean>;
  searchQuery?: string;
  depth?: number;
  maxDepth?: number;
}

export interface CategoryManagerProps {
  parentCategoryId?: number;
  onClose: () => void;
  onCategoryCreated: (parentId: number | null, category: CreateQuestionBankCategoryDto) => Promise<void>;
  initialData?: Partial<CreateQuestionBankCategoryDto>;
  mode?: 'create' | 'edit';
  categoryId?: number;
}

export interface TreeSearchProps {
  onSearch: (query: string) => void;
  placeholder?: string;
  className?: string;
  disabled?: boolean;
  searchDelay?: number;
}

export interface TreeActionsProps {
  onCategoryCreate: () => void;
  onRefresh: () => void;
  selectedCategoryId?: number;
  onBulkOperations?: () => void;
  onExport?: () => void;
  onImport?: () => void;
}

export interface BreadcrumbNavigationProps {
  categoryId: number;
  onCategorySelect: (category: QuestionBankCategoryDto) => void;
  className?: string;
  maxItems?: number;
  showHome?: boolean;
}

export interface TreeDragDropProps {
  onDrop: (data: TreeDragDropData) => Promise<boolean>;
  enabled?: boolean;
  allowedTypes?: string[];
  className?: string;
}

export interface CategoryFiltersProps {
  onFilterChange: (filters: CategoryFilterDto) => void;
  filters: CategoryFilterDto;
  className?: string;
  showAdvanced?: boolean;
}
