// Question Management Types and Interfaces

// Core Question Types
export interface Question {
  id: number;
  text: string;
  type: QuestionType;
  difficulty: QuestionDifficulty;
  categoryId: number;
  categoryName?: string;
  tags: string[];
  options?: QuestionOption[];
  correctAnswer: string | string[];
  explanation?: string;
  metadata: QuestionMetadata;
  status: QuestionStatus;
  version: number;
  createdAt: Date;
  updatedAt: Date;
  createdBy: number;
  updatedBy?: number;
  isActive: boolean;
}

export interface QuestionOption {
  id: number;
  text: string;
  isCorrect: boolean;
  explanation?: string;
}

export interface QuestionMetadata {
  timeLimit?: number;
  points?: number;
  difficultyLevel?: string;
  subject?: string;
  grade?: string;
  curriculum?: string;
  learningObjectives?: string[];
  prerequisites?: string[];
  relatedQuestions?: number[];
  mediaAttachments?: MediaAttachment[];
}

export interface MediaAttachment {
  id: number;
  type: MediaType;
  url: string;
  filename: string;
  size: number;
  mimeType: string;
  thumbnailUrl?: string;
}

// Question Bank Types
export interface QuestionBank {
  id: number;
  name: string;
  description?: string;
  categoryId: number;
  categoryName?: string;
  questionCount: number;
  totalPoints: number;
  averageDifficulty: number;
  status: QuestionBankStatus;
  isPublic: boolean;
  createdBy: number;
  createdAt: Date;
  updatedAt: Date;
  tags: string[];
  metadata: QuestionBankMetadata;
}

export interface QuestionBankMetadata {
  subject?: string;
  grade?: string;
  curriculum?: string;
  academicYear?: string;
  semester?: string;
  learningOutcomes?: string[];
  prerequisites?: string[];
  estimatedDuration?: number;
  maxAttempts?: number;
}

// Question Template Types
export interface QuestionTemplate {
  id: number;
  name: string;
  description?: string;
  type: QuestionType;
  structure: QuestionTemplateStructure;
  metadata: QuestionTemplateMetadata;
  isActive: boolean;
  createdBy: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface QuestionTemplateStructure {
  fields: TemplateField[];
  validationRules: ValidationRule[];
  defaultValues: Record<string, any>;
}

export interface TemplateField {
  name: string;
  type: FieldType;
  label: string;
  required: boolean;
  validation?: string;
  options?: string[];
  defaultValue?: any;
}

export interface ValidationRule {
  field: string;
  rule: string;
  message: string;
  parameters?: any[];
}

// Question Version Types
export interface QuestionVersion {
  id: number;
  questionId: number;
  version: number;
  text: string;
  options?: QuestionOption[];
  correctAnswer: string | string[];
  explanation?: string;
  metadata: QuestionMetadata;
  changeDescription?: string;
  status: VersionStatus;
  isCurrent: boolean;
  createdBy: number;
  createdAt: Date;
  approvedBy?: number;
  approvedAt?: Date;
  publishedAt?: Date;
}

// Question Validation Types
export interface QuestionValidation {
  id: number;
  questionId: number;
  validatorId: number;
  status: ValidationStatus;
  score: number;
  feedback: string;
  criteria: ValidationCriteria[];
  isValid: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface ValidationCriteria {
  name: string;
  description: string;
  weight: number;
  score: number;
  maxScore: number;
  feedback?: string;
}

// Question Import/Export Types
export interface QuestionImportBatch {
  id: number;
  filename: string;
  format: ImportFormat;
  status: ImportStatus;
  totalQuestions: number;
  importedQuestions: number;
  failedQuestions: number;
  errors: ImportError[];
  createdBy: number;
  createdAt: Date;
  completedAt?: Date;
}

export interface ImportError {
  row: number;
  field: string;
  value: string;
  error: string;
  suggestion?: string;
}

export interface QuestionExportRequest {
  format: ExportFormat;
  filters: QuestionFilters;
  includeMetadata: boolean;
  includeHistory: boolean;
  includeAnalytics: boolean;
}

// Question Search Types
export interface QuestionSearchRequest {
  query?: string;
  filters: QuestionFilters;
  sorting: QuestionSorting;
  pagination: PaginationOptions;
  includeMetadata: boolean;
  includeAnalytics: boolean;
}

export interface QuestionFilters {
  categories?: number[];
  types?: QuestionType[];
  difficulties?: QuestionDifficulty[];
  tags?: string[];
  status?: QuestionStatus[];
  dateRange?: DateRange;
  createdBy?: number[];
  metadata?: Record<string, any>;
}

export interface QuestionSorting {
  field: string;
  direction: SortDirection;
}

export interface PaginationOptions {
  page: number;
  pageSize: number;
  totalCount?: number;
}

export interface DateRange {
  start: Date;
  end: Date;
}

// Question Review Types
export interface QuestionReview {
  id: number;
  questionId: number;
  reviewerId: number;
  status: ReviewStatus;
  score: number;
  feedback: string;
  criteria: ReviewCriteria[];
  isApproved: boolean;
  createdAt: Date;
  updatedAt: Date;
  submittedAt?: Date;
  approvedAt?: Date;
  rejectedAt?: Date;
}

export interface ReviewCriteria {
  name: string;
  description: string;
  weight: number;
  score: number;
  maxScore: number;
  feedback?: string;
}

// Question Creation Workflow Types
export interface QuestionCreationWorkflow {
  id: number;
  questionId?: number;
  creatorId: number;
  status: WorkflowStatus;
  currentStep: number;
  totalSteps: number;
  steps: WorkflowStep[];
  progress: number;
  startedAt: Date;
  updatedAt: Date;
  completedAt?: Date;
  cancelledAt?: Date;
}

export interface WorkflowStep {
  stepNumber: number;
  name: string;
  description: string;
  status: StepStatus;
  isRequired: boolean;
  validationRules?: string[];
  completedAt?: Date;
  data?: Record<string, any>;
}

// Question Analytics Types
export interface QuestionAnalytics {
  questionId: number;
  usageCount: number;
  averageScore: number;
  successRate: number;
  timeSpent: number;
  difficultyRating: number;
  qualityRating: number;
  tags: TagAnalytics[];
  performanceByCategory: CategoryPerformance[];
  trends: PerformanceTrend[];
  lastUpdated: Date;
}

export interface TagAnalytics {
  tag: string;
  usageCount: number;
  averageScore: number;
  successRate: number;
}

export interface CategoryPerformance {
  categoryId: number;
  categoryName: string;
  questionCount: number;
  averageScore: number;
  successRate: number;
}

export interface PerformanceTrend {
  date: Date;
  averageScore: number;
  successRate: number;
  usageCount: number;
}

// Question Usage History Types
export interface QuestionUsageHistory {
  id: number;
  questionId: number;
  userId: number;
  examId?: number;
  score: number;
  maxScore: number;
  timeSpent: number;
  attempts: number;
  isCorrect: boolean;
  usedAt: Date;
  context?: Record<string, any>;
}

// Enums
export enum QuestionType {
  MultipleChoice = 'MultipleChoice',
  TrueFalse = 'TrueFalse',
  FillInTheBlank = 'FillInTheBlank',
  ShortAnswer = 'ShortAnswer',
  Essay = 'Essay',
  Matching = 'Matching',
  Ordering = 'Ordering',
  Hotspot = 'Hotspot',
  DragAndDrop = 'DragAndDrop',
  Numeric = 'Numeric'
}

export enum QuestionDifficulty {
  Easy = 'Easy',
  Medium = 'Medium',
  Hard = 'Hard',
  Expert = 'Expert'
}

export enum QuestionStatus {
  Draft = 'Draft',
  PendingReview = 'PendingReview',
  UnderReview = 'UnderReview',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Published = 'Published',
  Archived = 'Archived'
}

export enum QuestionBankStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Archived = 'Archived',
  UnderMaintenance = 'UnderMaintenance'
}

export enum VersionStatus {
  Draft = 'Draft',
  PendingApproval = 'PendingApproval',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Published = 'Published',
  Archived = 'Archived'
}

export enum ValidationStatus {
  Pending = 'Pending',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Failed = 'Failed'
}

export enum ReviewStatus {
  Pending = 'Pending',
  InProgress = 'InProgress',
  Submitted = 'Submitted',
  Approved = 'Approved',
  Rejected = 'Rejected',
  RevisionRequested = 'RevisionRequested'
}

export enum WorkflowStatus {
  NotStarted = 'NotStarted',
  InProgress = 'InProgress',
  Paused = 'Paused',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

export enum StepStatus {
  Pending = 'Pending',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Skipped = 'Skipped',
  Failed = 'Failed'
}

export enum ImportFormat {
  Excel = 'Excel',
  CSV = 'CSV',
  JSON = 'JSON',
  XML = 'XML',
  QTI = 'QTI'
}

export enum ExportFormat {
  Excel = 'Excel',
  CSV = 'CSV',
  JSON = 'JSON',
  XML = 'XML',
  QTI = 'QTI',
  PDF = 'PDF'
}

export enum ImportStatus {
  Pending = 'Pending',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Failed = 'Failed',
  Cancelled = 'Cancelled'
}

export enum FieldType {
  Text = 'Text',
  Number = 'Number',
  Boolean = 'Boolean',
  Select = 'Select',
  MultiSelect = 'MultiSelect',
  Date = 'Date',
  File = 'File',
  RichText = 'RichText'
}

export enum MediaType {
  Image = 'Image',
  Audio = 'Audio',
  Video = 'Video',
  Document = 'Document',
  Interactive = 'Interactive'
}

export enum SortDirection {
  Ascending = 'asc',
  Descending = 'desc'
}

// DTOs for API Communication
export interface CreateQuestionDto {
  text: string;
  type: QuestionType;
  difficulty: QuestionDifficulty;
  categoryId: number;
  tags?: string[];
  options?: CreateQuestionOptionDto[];
  correctAnswer: string | string[];
  explanation?: string;
  metadata?: Partial<QuestionMetadata>;
}

export interface CreateQuestionOptionDto {
  text: string;
  isCorrect: boolean;
  explanation?: string;
}

export interface UpdateQuestionDto {
  text?: string;
  type?: QuestionType;
  difficulty?: QuestionDifficulty;
  categoryId?: number;
  tags?: string[];
  options?: UpdateQuestionOptionDto[];
  correctAnswer?: string | string[];
  explanation?: string;
  metadata?: Partial<QuestionMetadata>;
  isActive?: boolean;
}

export interface UpdateQuestionOptionDto {
  id?: number;
  text: string;
  isCorrect: boolean;
  explanation?: string;
}

export interface CreateQuestionBankDto {
  name: string;
  description?: string;
  categoryId: number;
  isPublic: boolean;
  tags?: string[];
  metadata?: Partial<QuestionBankMetadata>;
}

export interface UpdateQuestionBankDto {
  name?: string;
  description?: string;
  categoryId?: number;
  isPublic?: boolean;
  tags?: string[];
  metadata?: Partial<QuestionBankMetadata>;
  status?: QuestionBankStatus;
}

export interface CreateQuestionTemplateDto {
  name: string;
  description?: string;
  type: QuestionType;
  structure: QuestionTemplateStructure;
  metadata?: Partial<QuestionTemplateMetadata>;
}

export interface UpdateQuestionTemplateDto {
  name?: string;
  description?: string;
  structure?: QuestionTemplateStructure;
  metadata?: Partial<QuestionTemplateMetadata>;
  isActive?: boolean;
}

export interface CreateQuestionVersionDto {
  questionId: number;
  text: string;
  options?: CreateQuestionOptionDto[];
  correctAnswer: string | string[];
  explanation?: string;
  metadata?: Partial<QuestionMetadata>;
  changeDescription?: string;
}

export interface UpdateQuestionVersionDto {
  text?: string;
  options?: UpdateQuestionOptionDto[];
  correctAnswer?: string | string[];
  explanation?: string;
  metadata?: Partial<QuestionMetadata>;
  changeDescription?: string;
}

export interface CreateQuestionReviewDto {
  questionId: number;
  score: number;
  feedback: string;
  criteria: CreateReviewCriteriaDto[];
}

export interface CreateReviewCriteriaDto {
  name: string;
  description: string;
  weight: number;
  score: number;
  maxScore: number;
  feedback?: string;
}

export interface UpdateQuestionReviewDto {
  score?: number;
  feedback?: string;
  criteria?: UpdateReviewCriteriaDto[];
}

export interface UpdateReviewCriteriaDto {
  name?: string;
  description?: string;
  weight?: number;
  score?: number;
  maxScore?: number;
  feedback?: string;
}

export interface CreateQuestionCreationWorkflowDto {
  creatorId: number;
  questionId?: number;
  templateId?: number;
}

export interface UpdateQuestionCreationWorkflowDto {
  status?: WorkflowStatus;
  currentStep?: number;
  steps?: WorkflowStep[];
}

export interface QuestionSearchResponse {
  questions: Question[];
  totalCount: number;
  page: number;
  pageSize: number;
  filters: QuestionFilters;
  sorting: QuestionSorting;
}

export interface QuestionAnalyticsResponse {
  analytics: QuestionAnalytics[];
  summary: {
    totalQuestions: number;
    averageScore: number;
    totalUsage: number;
    successRate: number;
  };
  trends: PerformanceTrend[];
}

// Utility Types
export type QuestionId = number;
export type QuestionBankId = number;
export type QuestionTemplateId = number;
export type QuestionVersionId = number;
export type QuestionReviewId = number;
export type QuestionCreationWorkflowId = number;

// API Response Types
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
  pagination?: PaginationInfo;
}

export interface PaginationInfo {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// Form Types
export interface QuestionFormData {
  basic: {
    text: string;
    type: QuestionType;
    difficulty: QuestionDifficulty;
    categoryId: number;
    tags: string[];
  };
  content: {
    options?: QuestionOption[];
    correctAnswer: string | string[];
    explanation?: string;
  };
  metadata: Partial<QuestionMetadata>;
  settings: {
    isActive: boolean;
    timeLimit?: number;
    points?: number;
  };
}

export interface QuestionBankFormData {
  basic: {
    name: string;
    description?: string;
    categoryId: number;
    isPublic: boolean;
    tags: string[];
  };
  metadata: Partial<QuestionBankMetadata>;
  settings: {
    status: QuestionBankStatus;
    estimatedDuration?: number;
    maxAttempts?: number;
  };
}

// Filter and Search Types
export interface QuestionFilterState {
  searchQuery: string;
  categories: number[];
  types: QuestionType[];
  difficulties: QuestionDifficulty[];
  tags: string[];
  status: QuestionStatus[];
  dateRange: DateRange | null;
  createdBy: number[];
  metadata: Record<string, any>;
}

export interface QuestionSortState {
  field: string;
  direction: SortDirection;
}

// Component Props Types
export interface QuestionCardProps {
  question: Question;
  onEdit?: (question: Question) => void;
  onDelete?: (questionId: number) => void;
  onPreview?: (question: Question) => void;
  onSelect?: (question: Question, selected: boolean) => void;
  selected?: boolean;
  showActions?: boolean;
  showMetadata?: boolean;
}

export interface QuestionListProps {
  questions: Question[];
  loading?: boolean;
  onQuestionSelect?: (question: Question) => void;
  onQuestionEdit?: (question: Question) => void;
  onQuestionDelete?: (question: Question) => void;
  onQuestionPreview?: (question: Question) => void;
  selectedQuestions?: Question[];
  onSelectionChange?: (questions: Question[]) => void;
  showPagination?: boolean;
  pagination?: PaginationInfo;
  onPageChange?: (page: number) => void;
}

export interface QuestionFiltersProps {
  filters: QuestionFilterState;
  onFiltersChange: (filters: QuestionFilterState) => void;
  onReset: () => void;
  onApply: () => void;
  categories: Array<{ id: number; name: string }>;
  tags: string[];
  users: Array<{ id: number; name: string }>;
}

export interface QuestionSearchProps {
  onSearch: (query: string) => void;
  onAdvancedSearch: (filters: QuestionFilterState) => void;
  placeholder?: string;
  showAdvancedOptions?: boolean;
  loading?: boolean;
}

// Hook Return Types
export interface UseQuestionsReturn {
  questions: Question[];
  loading: boolean;
  error: string | null;
  pagination: PaginationInfo;
  filters: QuestionFilterState;
  sorting: QuestionSortState;
  selectedQuestions: Question[];
  loadQuestions: () => Promise<void>;
  createQuestion: (data: CreateQuestionDto) => Promise<Question>;
  updateQuestion: (id: number, data: UpdateQuestionDto) => Promise<Question>;
  deleteQuestion: (id: number) => Promise<boolean>;
  searchQuestions: (query: string) => Promise<void>;
  applyFilters: (filters: QuestionFilterState) => Promise<void>;
  applySorting: (sorting: QuestionSortState) => Promise<void>;
  selectQuestion: (question: Question, selected: boolean) => void;
  selectAllQuestions: (selected: boolean) => void;
  clearSelection: () => void;
}

export interface UseQuestionBanksReturn {
  questionBanks: QuestionBank[];
  loading: boolean;
  error: string | null;
  pagination: PaginationInfo;
  loadQuestionBanks: () => Promise<void>;
  createQuestionBank: (data: CreateQuestionBankDto) => Promise<QuestionBank>;
  updateQuestionBank: (id: number, data: UpdateQuestionBankDto) => Promise<QuestionBank>;
  deleteQuestionBank: (id: number) => Promise<boolean>;
  getQuestionBank: (id: number) => Promise<QuestionBank>;
  getQuestionsInBank: (bankId: number) => Promise<Question[]>;
  addQuestionToBank: (bankId: number, questionId: number) => Promise<boolean>;
  removeQuestionFromBank: (bankId: number, questionId: number) => Promise<boolean>;
}

export interface UseQuestionAnalyticsReturn {
  analytics: QuestionAnalytics[];
  summary: {
    totalQuestions: number;
    averageScore: number;
    totalUsage: number;
    successRate: number;
  };
  trends: PerformanceTrend[];
  loading: boolean;
  error: string | null;
  dateRange: DateRange;
  loadAnalytics: (dateRange: DateRange) => Promise<void>;
  exportAnalytics: (format: ExportFormat) => Promise<void>;
}

// Constants
export const QUESTION_TYPES = Object.values(QuestionType);
export const QUESTION_DIFFICULTIES = Object.values(QuestionDifficulty);
export const QUESTION_STATUSES = Object.values(QuestionStatus);
export const IMPORT_FORMATS = Object.values(ImportFormat);
export const EXPORT_FORMATS = Object.values(ExportFormat);

export const DEFAULT_PAGE_SIZE = 20;
export const MAX_PAGE_SIZE = 100;
export const DEFAULT_SORT_FIELD = 'createdAt';
export const DEFAULT_SORT_DIRECTION = SortDirection.Descending;

export const QUESTION_VALIDATION_CRITERIA = [
  { name: 'Clarity', description: 'Question is clear and unambiguous', weight: 20 },
  { name: 'Accuracy', description: 'Question content is factually correct', weight: 25 },
  { name: 'Difficulty', description: 'Appropriate difficulty level', weight: 15 },
  { name: 'Relevance', description: 'Question is relevant to learning objectives', weight: 20 },
  { name: 'Format', description: 'Proper formatting and structure', weight: 10 },
  { name: 'Options', description: 'Answer options are appropriate', weight: 10 }
];

export const WORKFLOW_STEPS = [
  { stepNumber: 1, name: 'Basic Information', description: 'Enter question text and type', isRequired: true },
  { stepNumber: 2, name: 'Content Creation', description: 'Create answer options and correct answer', isRequired: true },
  { stepNumber: 3, name: 'Metadata', description: 'Add tags, difficulty, and metadata', isRequired: false },
  { stepNumber: 4, name: 'Validation', description: 'Review and validate question', isRequired: true },
  { stepNumber: 5, name: 'Submission', description: 'Submit for review or publish', isRequired: true }
];
