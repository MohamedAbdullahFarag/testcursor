// Question Management Utilities

import { 
  Question, 
  QuestionType, 
  QuestionDifficulty, 
  QuestionStatus, 
  QuestionFilters, 
  QuestionSorting, 
  PaginationOptions,
  DateRange,
  QuestionFormData,
  QuestionBankFormData,
  QuestionValidation,
  QuestionVersion,
  QuestionCreationWorkflow,
  WorkflowStatus,
  StepStatus,
  ImportFormat,
  ExportFormat,
  SortDirection
} from '../types';
import { 
  VALIDATION_RULES, 
  PAGINATION, 
  SORTING, 
  FILTER_OPTIONS,
  ERROR_MESSAGES,
  SUCCESS_MESSAGES,
  I18N
} from '../constants';

// Validation Utilities
export const validateQuestion = (question: Partial<Question>): string[] => {
  const errors: string[] = [];

  // Required fields validation
  if (!question.text?.trim()) {
    errors.push(ERROR_MESSAGES.QUESTIONS.CREATE_FAILED);
  } else if (question.text.length < VALIDATION_RULES.QUESTION.TEXT_MIN_LENGTH) {
    errors.push(ERROR_MESSAGES.COMMON.MIN_LENGTH('Question text', VALIDATION_RULES.QUESTION.TEXT_MIN_LENGTH));
  } else if (question.text.length > VALIDATION_RULES.QUESTION.TEXT_MAX_LENGTH) {
    errors.push(ERROR_MESSAGES.COMMON.MAX_LENGTH('Question text', VALIDATION_RULES.QUESTION.TEXT_MAX_LENGTH));
  }

  if (!question.type) {
    errors.push('Question type is required');
  }

  if (!question.difficulty) {
    errors.push('Question difficulty is required');
  }

  if (!question.categoryId) {
    errors.push('Question category is required');
  }

  // Options validation for multiple choice questions
  if (question.type === QuestionType.MultipleChoice) {
    if (!question.options || question.options.length < VALIDATION_RULES.QUESTION.OPTIONS_MIN_COUNT) {
      errors.push(ERROR_MESSAGES.QUESTIONS.OPTIONS_REQUIRED);
    } else if (question.options.length > VALIDATION_RULES.QUESTION.OPTIONS_MAX_COUNT) {
      errors.push(`Question options cannot exceed ${VALIDATION_RULES.QUESTION.OPTIONS_MAX_COUNT}`);
    } else {
      // Check for duplicate options
      const optionTexts = question.options.map(opt => opt.text.trim().toLowerCase());
      const uniqueTexts = new Set(optionTexts);
      if (optionTexts.length !== uniqueTexts.size) {
        errors.push(ERROR_MESSAGES.QUESTIONS.DUPLICATE_OPTION);
      }

      // Check if at least one option is marked as correct
      const hasCorrectOption = question.options.some(opt => opt.isCorrect);
      if (!hasCorrectOption) {
        errors.push(ERROR_MESSAGES.QUESTIONS.INVALID_CORRECT_ANSWER);
      }
    }
  }

  // Tags validation
  if (question.tags) {
    if (question.tags.length < VALIDATION_RULES.QUESTION.TAGS_MIN_COUNT) {
      errors.push(`At least ${VALIDATION_RULES.QUESTION.TAGS_MIN_COUNT} tag is required`);
    } else if (question.tags.length > VALIDATION_RULES.QUESTION.TAGS_MAX_COUNT) {
      errors.push(`Tags cannot exceed ${VALIDATION_RULES.QUESTION.TAGS_MAX_COUNT}`);
    }

    // Check tag length
    const invalidTags = question.tags.filter(tag => tag.length > VALIDATION_RULES.QUESTION.TAG_MAX_LENGTH);
    if (invalidTags.length > 0) {
      errors.push(`Tags cannot exceed ${VALIDATION_RULES.QUESTION.TAG_MAX_LENGTH} characters`);
    }
  }

  // Explanation validation for certain question types
  if (question.type === QuestionType.Essay || question.type === QuestionType.ShortAnswer) {
    if (!question.explanation?.trim()) {
      errors.push(ERROR_MESSAGES.QUESTIONS.EXPLANATION_REQUIRED);
    } else if (question.explanation.length > VALIDATION_RULES.QUESTION.EXPLANATION_MAX_LENGTH) {
      errors.push(ERROR_MESSAGES.COMMON.MAX_LENGTH('Explanation', VALIDATION_RULES.QUESTION.EXPLANATION_MAX_LENGTH));
    }
  }

  return errors;
};

export const validateQuestionBank = (questionBank: Partial<QuestionBank>): string[] => {
  const errors: string[] = [];

  if (!questionBank.name?.trim()) {
    errors.push('Question bank name is required');
  } else if (questionBank.name.length < VALIDATION_RULES.QUESTION_BANK.NAME_MIN_LENGTH) {
    errors.push(ERROR_MESSAGES.COMMON.MIN_LENGTH('Question bank name', VALIDATION_RULES.QUESTION_BANK.NAME_MIN_LENGTH));
  } else if (questionBank.name.length > VALIDATION_RULES.QUESTION_BANK.NAME_MAX_LENGTH) {
    errors.push(ERROR_MESSAGES.COMMON.MAX_LENGTH('Question bank name', VALIDATION_RULES.QUESTION_BANK.NAME_MAX_LENGTH));
  }

  if (!questionBank.categoryId) {
    errors.push('Question bank category is required');
  }

  if (questionBank.description && questionBank.description.length > VALIDATION_RULES.QUESTION_BANK.DESCRIPTION_MAX_LENGTH) {
    errors.push(ERROR_MESSAGES.COMMON.MAX_LENGTH('Description', VALIDATION_RULES.QUESTION_BANK.DESCRIPTION_MAX_LENGTH));
  }

  return errors;
};

export const validateWorkflow = (workflow: Partial<QuestionCreationWorkflow>): string[] => {
  const errors: string[] = [];

  if (!workflow.creatorId) {
    errors.push('Workflow creator is required');
  }

  if (workflow.currentStep && (workflow.currentStep < 1 || workflow.currentStep > (workflow.totalSteps || 1))) {
    errors.push('Invalid workflow step');
  }

  return errors;
};

// Filter and Search Utilities
export const applyQuestionFilters = (
  questions: Question[], 
  filters: QuestionFilters
): Question[] => {
  let filteredQuestions = [...questions];

  // Text search
  if (filters.query) {
    const query = filters.query.toLowerCase();
    filteredQuestions = filteredQuestions.filter(question =>
      question.text.toLowerCase().includes(query) ||
      question.explanation?.toLowerCase().includes(query) ||
      question.tags.some(tag => tag.toLowerCase().includes(query))
    );
  }

  // Category filter
  if (filters.categories && filters.categories.length > 0) {
    filteredQuestions = filteredQuestions.filter(question =>
      filters.categories!.includes(question.categoryId)
    );
  }

  // Type filter
  if (filters.types && filters.types.length > 0) {
    filteredQuestions = filteredQuestions.filter(question =>
      filters.types!.includes(question.type)
    );
  }

  // Difficulty filter
  if (filters.difficulties && filters.difficulties.length > 0) {
    filteredQuestions = filteredQuestions.filter(question =>
      filters.difficulties!.includes(question.difficulty)
    );
  }

  // Status filter
  if (filters.status && filters.status.length > 0) {
    filteredQuestions = filteredQuestions.filter(question =>
      filters.status!.includes(question.status)
    );
  }

  // Tags filter
  if (filters.tags && filters.tags.length > 0) {
    filteredQuestions = filteredQuestions.filter(question =>
      filters.tags!.some(tag => question.tags.includes(tag))
    );
  }

  // Date range filter
  if (filters.dateRange) {
    const { start, end } = filters.dateRange;
    filteredQuestions = filteredQuestions.filter(question => {
      const questionDate = new Date(question.createdAt);
      return questionDate >= start && questionDate <= end;
    });
  }

  // Created by filter
  if (filters.createdBy && filters.createdBy.length > 0) {
    filteredQuestions = filteredQuestions.filter(question =>
      filters.createdBy!.includes(question.createdBy)
    );
  }

  // Metadata filter
  if (filters.metadata) {
    Object.entries(filters.metadata).forEach(([key, value]) => {
      filteredQuestions = filteredQuestions.filter(question => {
        const metadataValue = question.metadata[key as keyof typeof question.metadata];
        if (Array.isArray(value)) {
          return Array.isArray(metadataValue) && value.some(v => metadataValue.includes(v));
        }
        return metadataValue === value;
      });
    });
  }

  return filteredQuestions;
};

export const sortQuestions = (
  questions: Question[], 
  sorting: QuestionSorting
): Question[] => {
  const sortedQuestions = [...questions];

  sortedQuestions.sort((a, b) => {
    let aValue: any;
    let bValue: any;

    switch (sorting.field) {
      case 'text':
        aValue = a.text.toLowerCase();
        bValue = b.text.toLowerCase();
        break;
      case 'type':
        aValue = a.type;
        bValue = b.type;
        break;
      case 'difficulty':
        aValue = a.difficulty;
        bValue = b.difficulty;
        break;
      case 'status':
        aValue = a.status;
        bValue = b.status;
        break;
      case 'categoryId':
        aValue = a.categoryId;
        bValue = b.categoryId;
        break;
      case 'createdBy':
        aValue = a.createdBy;
        bValue = b.createdBy;
        break;
      case 'createdAt':
        aValue = new Date(a.createdAt);
        bValue = new Date(b.createdAt);
        break;
      case 'updatedAt':
        aValue = new Date(a.updatedAt);
        bValue = new Date(b.updatedAt);
        break;
      default:
        aValue = a.createdAt;
        bValue = b.createdAt;
    }

    if (aValue < bValue) {
      return sorting.direction === SortDirection.Ascending ? -1 : 1;
    }
    if (aValue > bValue) {
      return sorting.direction === SortDirection.Ascending ? 1 : -1;
    }
    return 0;
  });

  return sortedQuestions;
};

export const paginateQuestions = (
  questions: Question[], 
  pagination: PaginationOptions
): { questions: Question[]; totalCount: number; totalPages: number } => {
  const { page = PAGINATION.DEFAULT_PAGE, pageSize = PAGINATION.DEFAULT_PAGE_SIZE } = pagination;
  const totalCount = questions.length;
  const totalPages = Math.ceil(totalCount / pageSize);
  const startIndex = (page - 1) * pageSize;
  const endIndex = startIndex + pageSize;

  const paginatedQuestions = questions.slice(startIndex, endIndex);

  return {
    questions: paginatedQuestions,
    totalCount,
    totalPages
  };
};

// Date Utilities
export const createDateRange = (preset: string): DateRange => {
  const now = new Date();
  const start = new Date();
  const end = new Date();

  switch (preset) {
    case FILTER_OPTIONS.DATE_RANGE_PRESETS.TODAY:
      start.setHours(0, 0, 0, 0);
      end.setHours(23, 59, 59, 999);
      break;
    case FILTER_OPTIONS.DATE_RANGE_PRESETS.YESTERDAY:
      start.setDate(start.getDate() - 1);
      start.setHours(0, 0, 0, 0);
      end.setDate(end.getDate() - 1);
      end.setHours(23, 59, 59, 999);
      break;
    case FILTER_OPTIONS.DATE_RANGE_PRESETS.LAST_7_DAYS:
      start.setDate(start.getDate() - 7);
      start.setHours(0, 0, 0, 0);
      break;
    case FILTER_OPTIONS.DATE_RANGE_PRESETS.LAST_30_DAYS:
      start.setDate(start.getDate() - 30);
      start.setHours(0, 0, 0, 0);
      break;
    case FILTER_OPTIONS.DATE_RANGE_PRESETS.THIS_MONTH:
      start.setDate(1);
      start.setHours(0, 0, 0, 0);
      break;
    case FILTER_OPTIONS.DATE_RANGE_PRESETS.LAST_MONTH:
      start.setMonth(start.getMonth() - 1);
      start.setDate(1);
      start.setHours(0, 0, 0, 0);
      end.setDate(0);
      end.setHours(23, 59, 59, 999);
      break;
    case FILTER_OPTIONS.DATE_RANGE_PRESETS.THIS_YEAR:
      start.setMonth(0, 1);
      start.setHours(0, 0, 0, 0);
      break;
    case FILTER_OPTIONS.DATE_RANGE_PRESETS.LAST_YEAR:
      start.setFullYear(start.getFullYear() - 1);
      start.setMonth(0, 1);
      start.setHours(0, 0, 0, 0);
      end.setFullYear(end.getFullYear() - 1);
      end.setMonth(11, 31);
      end.setHours(23, 59, 59, 999);
      break;
    default:
      start.setHours(0, 0, 0, 0);
      end.setHours(23, 59, 59, 999);
  }

  return { start, end };
};

export const formatDate = (date: Date, format: string = I18N.DATE_FORMATS.MEDIUM): string => {
  try {
    return new Intl.DateTimeFormat('en-US', {
      year: 'numeric',
      month: format.includes('MMM') ? 'short' : 'long',
      day: 'numeric',
      hour: format.includes('HH') ? '2-digit' : undefined,
      minute: format.includes('mm') ? '2-digit' : undefined,
      second: format.includes('ss') ? '2-digit' : undefined,
    }).format(date);
  } catch (error) {
    return date.toLocaleDateString();
  }
};

export const formatRelativeTime = (date: Date): string => {
  const now = new Date();
  const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);

  if (diffInSeconds < 60) {
    return 'Just now';
  } else if (diffInSeconds < 3600) {
    const minutes = Math.floor(diffInSeconds / 60);
    return `${minutes} minute${minutes > 1 ? 's' : ''} ago`;
  } else if (diffInSeconds < 86400) {
    const hours = Math.floor(diffInSeconds / 3600);
    return `${hours} hour${hours > 1 ? 's' : ''} ago`;
  } else if (diffInSeconds < 2592000) {
    const days = Math.floor(diffInSeconds / 86400);
    return `${days} day${days > 1 ? 's' : ''} ago`;
  } else if (diffInSeconds < 31536000) {
    const months = Math.floor(diffInSeconds / 2592000);
    return `${months} month${months > 1 ? 's' : ''} ago`;
  } else {
    const years = Math.floor(diffInSeconds / 31536000);
    return `${years} year${years > 1 ? 's' : ''} ago`;
  }
};

// Form Utilities
export const createEmptyQuestionForm = (): QuestionFormData => ({
  basic: {
    text: '',
    type: QuestionType.MultipleChoice,
    difficulty: QuestionDifficulty.Medium,
    categoryId: 0,
    tags: [],
  },
  content: {
    options: [
      { id: 1, text: '', isCorrect: false, explanation: '' },
      { id: 2, text: '', isCorrect: false, explanation: '' },
    ],
    correctAnswer: [],
    explanation: '',
  },
  metadata: {
    timeLimit: undefined,
    points: undefined,
    difficultyLevel: undefined,
    subject: undefined,
    grade: undefined,
    curriculum: undefined,
    learningObjectives: [],
    prerequisites: [],
    relatedQuestions: [],
    mediaAttachments: [],
  },
  settings: {
    isActive: true,
    timeLimit: undefined,
    points: undefined,
  },
});

export const createEmptyQuestionBankForm = (): QuestionBankFormData => ({
  basic: {
    name: '',
    description: '',
    categoryId: 0,
    isPublic: false,
    tags: [],
  },
  metadata: {
    subject: undefined,
    grade: undefined,
    curriculum: undefined,
    academicYear: undefined,
    semester: undefined,
    learningOutcomes: [],
    prerequisites: [],
    estimatedDuration: undefined,
    maxAttempts: undefined,
  },
  settings: {
    status: QuestionBankStatus.Active,
    estimatedDuration: undefined,
    maxAttempts: undefined,
  },
});

export const populateQuestionForm = (question: Question): QuestionFormData => ({
  basic: {
    text: question.text,
    type: question.type,
    difficulty: question.difficulty,
    categoryId: question.categoryId,
    tags: [...question.tags],
  },
  content: {
    options: question.options ? [...question.options] : [],
    correctAnswer: Array.isArray(question.correctAnswer) 
      ? [...question.correctAnswer] 
      : [question.correctAnswer],
    explanation: question.explanation || '',
  },
  metadata: { ...question.metadata },
  settings: {
    isActive: question.isActive,
    timeLimit: question.metadata.timeLimit,
    points: question.metadata.points,
  },
});

export const populateQuestionBankForm = (questionBank: QuestionBank): QuestionBankFormData => ({
  basic: {
    name: questionBank.name,
    description: questionBank.description || '',
    categoryId: questionBank.categoryId,
    isPublic: questionBank.isPublic,
    tags: [...questionBank.tags],
  },
  metadata: { ...questionBank.metadata },
  settings: {
    status: questionBank.status,
    estimatedDuration: questionBank.metadata.estimatedDuration,
    maxAttempts: questionBank.metadata.maxAttempts,
  },
});

// Workflow Utilities
export const getWorkflowProgress = (workflow: QuestionCreationWorkflow): number => {
  if (!workflow.steps || workflow.steps.length === 0) {
    return 0;
  }

  const completedSteps = workflow.steps.filter(step => 
    step.status === StepStatus.Completed
  ).length;

  return Math.round((completedSteps / workflow.steps.length) * 100);
};

export const canProceedToNextStep = (workflow: QuestionCreationWorkflow): boolean => {
  if (!workflow.steps || workflow.currentStep >= workflow.steps.length) {
    return false;
  }

  const currentStep = workflow.steps[workflow.currentStep - 1];
  return currentStep.status === StepStatus.Completed;
};

export const canGoToPreviousStep = (workflow: QuestionCreationWorkflow): boolean => {
  return workflow.currentStep > 1;
};

export const getWorkflowStepStatus = (stepNumber: number, currentStep: number): StepStatus => {
  if (stepNumber < currentStep) {
    return StepStatus.Completed;
  } else if (stepNumber === currentStep) {
    return StepStatus.InProgress;
  } else {
    return StepStatus.Pending;
  }
};

// Question Type Utilities
export const getQuestionTypeDisplayName = (type: QuestionType): string => {
  const displayNames: Record<QuestionType, string> = {
    [QuestionType.MultipleChoice]: 'Multiple Choice',
    [QuestionType.TrueFalse]: 'True/False',
    [QuestionType.FillInTheBlank]: 'Fill in the Blank',
    [QuestionType.ShortAnswer]: 'Short Answer',
    [QuestionType.Essay]: 'Essay',
    [QuestionType.Matching]: 'Matching',
    [QuestionType.Ordering]: 'Ordering',
    [QuestionType.Hotspot]: 'Hotspot',
    [QuestionType.DragAndDrop]: 'Drag & Drop',
    [QuestionType.Numeric]: 'Numeric',
  };

  return displayNames[type] || type;
};

export const getQuestionTypeIcon = (type: QuestionType): string => {
  const icons: Record<QuestionType, string> = {
    [QuestionType.MultipleChoice]: 'radio_button_checked',
    [QuestionType.TrueFalse]: 'check_circle',
    [QuestionType.FillInTheBlank]: 'input',
    [QuestionType.ShortAnswer]: 'short_text',
    [QuestionType.Essay]: 'article',
    [QuestionType.Matching]: 'compare_arrows',
    [QuestionType.Ordering]: 'format_list_numbered',
    [QuestionType.Hotspot]: 'touch_app',
    [QuestionType.DragAndDrop]: 'drag_indicator',
    [QuestionType.Numeric]: 'calculate',
  };

  return icons[type] || 'help';
};

export const getQuestionTypeColor = (type: QuestionType): string => {
  const colors: Record<QuestionType, string> = {
    [QuestionType.MultipleChoice]: '#2196F3',
    [QuestionType.TrueFalse]: '#4CAF50',
    [QuestionType.FillInTheBlank]: '#FF9800',
    [QuestionType.ShortAnswer]: '#9C27B0',
    [QuestionType.Essay]: '#F44336',
    [QuestionType.Matching]: '#607D8B',
    [QuestionType.Ordering]: '#795548',
    [QuestionType.Hotspot]: '#E91E63',
    [QuestionType.DragAndDrop]: '#00BCD4',
    [QuestionType.Numeric]: '#8BC34A',
  };

  return colors[type] || '#757575';
};

// Status Utilities
export const getStatusColor = (status: QuestionStatus | QuestionBankStatus | WorkflowStatus): string => {
  const statusColors: Record<string, string> = {
    // Question Status
    [QuestionStatus.Draft]: '#757575',
    [QuestionStatus.PendingReview]: '#FF9800',
    [QuestionStatus.UnderReview]: '#2196F3',
    [QuestionStatus.Approved]: '#4CAF50',
    [QuestionStatus.Rejected]: '#F44336',
    [QuestionStatus.Published]: '#8BC34A',
    [QuestionStatus.Archived]: '#9E9E9E',
    
    // Question Bank Status
    [QuestionBankStatus.Active]: '#4CAF50',
    [QuestionBankStatus.Inactive]: '#757575',
    [QuestionBankStatus.Archived]: '#9E9E9E',
    [QuestionBankStatus.UnderMaintenance]: '#FF9800',
    
    // Workflow Status
    [WorkflowStatus.NotStarted]: '#757575',
    [WorkflowStatus.InProgress]: '#2196F3',
    [WorkflowStatus.Paused]: '#FF9800',
    [WorkflowStatus.Completed]: '#4CAF50',
    [WorkflowStatus.Cancelled]: '#F44336',
  };

  return statusColors[status] || '#757575';
};

export const getStatusIcon = (status: QuestionStatus | QuestionBankStatus | WorkflowStatus): string => {
  const statusIcons: Record<string, string> = {
    // Question Status
    [QuestionStatus.Draft]: 'edit',
    [QuestionStatus.PendingReview]: 'schedule',
    [QuestionStatus.UnderReview]: 'visibility',
    [QuestionStatus.Approved]: 'check_circle',
    [QuestionStatus.Rejected]: 'cancel',
    [QuestionStatus.Published]: 'publish',
    [QuestionStatus.Archived]: 'archive',
    
    // Question Bank Status
    [QuestionBankStatus.Active]: 'check_circle',
    [QuestionBankStatus.Inactive]: 'pause_circle',
    [QuestionBankStatus.Archived]: 'archive',
    [QuestionBankStatus.UnderMaintenance]: 'build',
    
    // Workflow Status
    [WorkflowStatus.NotStarted]: 'play_circle',
    [WorkflowStatus.InProgress]: 'sync',
    [WorkflowStatus.Paused]: 'pause_circle',
    [WorkflowStatus.Completed]: 'check_circle',
    [WorkflowStatus.Cancelled]: 'cancel',
  };

  return statusIcons[status] || 'help';
};

// Import/Export Utilities
export const getImportFormatDisplayName = (format: ImportFormat): string => {
  const displayNames: Record<ImportFormat, string> = {
    [ImportFormat.Excel]: 'Microsoft Excel (.xlsx)',
    [ImportFormat.CSV]: 'Comma Separated Values (.csv)',
    [ImportFormat.JSON]: 'JavaScript Object Notation (.json)',
    [ImportFormat.XML]: 'Extensible Markup Language (.xml)',
    [ImportFormat.QTI]: 'Question and Test Interoperability (.xml)',
  };

  return displayNames[format] || format;
};

export const getExportFormatDisplayName = (format: ExportFormat): string => {
  const displayNames: Record<ExportFormat, string> = {
    [ExportFormat.Excel]: 'Microsoft Excel (.xlsx)',
    [ExportFormat.CSV]: 'Comma Separated Values (.csv)',
    [ExportFormat.JSON]: 'JavaScript Object Notation (.json)',
    [ExportFormat.XML]: 'Extensible Markup Language (.xml)',
    [ExportFormat.QTI]: 'Question and Test Interoperability (.xml)',
    [ExportFormat.PDF]: 'Portable Document Format (.pdf)',
  };

  return displayNames[format] || format;
};

export const getFileExtension = (format: ImportFormat | ExportFormat): string => {
  const extensions: Record<string, string> = {
    [ImportFormat.Excel]: '.xlsx',
    [ImportFormat.CSV]: '.csv',
    [ImportFormat.JSON]: '.json',
    [ImportFormat.XML]: '.xml',
    [ImportFormat.QTI]: '.xml',
    [ExportFormat.Excel]: '.xlsx',
    [ExportFormat.CSV]: '.csv',
    [ExportFormat.JSON]: '.json',
    [ExportFormat.XML]: '.xml',
    [ExportFormat.QTI]: '.xml',
    [ExportFormat.PDF]: '.pdf',
  };

  return extensions[format] || '';
};

// Storage Utilities
export const saveToLocalStorage = <T>(key: string, value: T): void => {
  try {
    localStorage.setItem(key, JSON.stringify(value));
  } catch (error) {
    console.warn('Failed to save to localStorage:', error);
  }
};

export const loadFromLocalStorage = <T>(key: string, defaultValue: T): T => {
  try {
    const item = localStorage.getItem(key);
    return item ? JSON.parse(item) : defaultValue;
  } catch (error) {
    console.warn('Failed to load from localStorage:', error);
    return defaultValue;
  }
};

export const removeFromLocalStorage = (key: string): void => {
  try {
    localStorage.removeItem(key);
  } catch (error) {
    console.warn('Failed to remove from localStorage:', error);
  }
};

// Performance Utilities
export const debounce = <T extends (...args: any[]) => any>(
  func: T,
  delay: number
): ((...args: Parameters<T>) => void) => {
  let timeoutId: NodeJS.Timeout;

  return (...args: Parameters<T>) => {
    clearTimeout(timeoutId);
    timeoutId = setTimeout(() => func(...args), delay);
  };
};

export const throttle = <T extends (...args: any[]) => any>(
  func: T,
  delay: number
): ((...args: Parameters<T>) => void) => {
  let lastCall = 0;

  return (...args: Parameters<T>) => {
    const now = Date.now();
    if (now - lastCall >= delay) {
      lastCall = now;
      func(...args);
    }
  };
};

// Accessibility Utilities
export const generateAriaLabel = (component: string, action?: string, context?: string): string => {
  let label = component;
  
  if (action) {
    label += ` ${action}`;
  }
  
  if (context) {
    label += ` ${context}`;
  }
  
  return label;
};

export const announceToScreenReader = (message: string): void => {
  // Create a temporary element for screen reader announcements
  const announcement = document.createElement('div');
  announcement.setAttribute('aria-live', 'polite');
  announcement.setAttribute('aria-atomic', 'true');
  announcement.className = 'sr-only';
  announcement.textContent = message;
  
  document.body.appendChild(announcement);
  
  // Remove the element after a short delay
  setTimeout(() => {
    if (announcement.parentNode) {
      announcement.parentNode.removeChild(announcement);
    }
  }, 1000);
};

// Export all utilities
export default {
  // Validation
  validateQuestion,
  validateQuestionBank,
  validateWorkflow,
  
  // Filter and Search
  applyQuestionFilters,
  sortQuestions,
  paginateQuestions,
  
  // Date
  createDateRange,
  formatDate,
  formatRelativeTime,
  
  // Form
  createEmptyQuestionForm,
  createEmptyQuestionBankForm,
  populateQuestionForm,
  populateQuestionBankForm,
  
  // Workflow
  getWorkflowProgress,
  canProceedToNextStep,
  canGoToPreviousStep,
  getWorkflowStepStatus,
  
  // Question Type
  getQuestionTypeDisplayName,
  getQuestionTypeIcon,
  getQuestionTypeColor,
  
  // Status
  getStatusColor,
  getStatusIcon,
  
  // Import/Export
  getImportFormatDisplayName,
  getExportFormatDisplayName,
  getFileExtension,
  
  // Storage
  saveToLocalStorage,
  loadFromLocalStorage,
  removeFromLocalStorage,
  
  // Performance
  debounce,
  throttle,
  
  // Accessibility
  generateAriaLabel,
  announceToScreenReader,
};
