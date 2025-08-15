// Question Bank Tree Hook
// Main React hook for tree management, categories, and hierarchy operations

import { useState, useEffect, useCallback, useRef } from 'react';
import { questionBankTreeService } from '../services/questionBankTreeService';
import { 
  QuestionBankTreeDto, 
  QuestionBankCategoryDto,
  CreateQuestionBankCategoryDto,
  UpdateQuestionBankCategoryDto,
  MoveCategoryDto,
  CopyCategoryDto,
  ReorderCategoriesDto,
  CategorySearchDto,
  CategoryFilterDto,
  TreeValidationResultDto,
  TreeStatisticsDto,
  TreeNodeData,
  TreeChangeEvent,
  CategoryEvent
} from '../types/questionBankTree.types';
import { 
  ERROR_MESSAGES, 
  SUCCESS_MESSAGES, 
  WARNING_MESSAGES, 
  INFO_MESSAGES,
  MAX_TREE_DEPTH,
  DEFAULT_PAGE_SIZE
} from '../constants/treeOperations';

interface UseQuestionBankTreeResult {
  // State
  treeData: QuestionBankTreeDto | null;
  categories: QuestionBankCategoryDto[];
  selectedCategory: QuestionBankCategoryDto | null;
  expandedNodes: string[];
  searchQuery: string;
  filters: CategoryFilterDto;
  
  // Loading and error states
  isLoading: boolean;
  isUpdating: boolean;
  error: string | null;
  
  // Tree operations
  refreshTree: () => Promise<void>;
  getCategory: (categoryId: number) => Promise<QuestionBankCategoryDto | null>;
  getChildren: (parentId: number) => Promise<QuestionBankCategoryDto[]>;
  getAncestors: (categoryId: number) => Promise<QuestionBankCategoryDto[]>;
  getDescendants: (categoryId: number, maxDepth?: number) => Promise<QuestionBankCategoryDto[]>;
  getSiblings: (categoryId: number) => Promise<QuestionBankCategoryDto[]>;
  
  // Category CRUD operations
  createCategory: (dto: CreateQuestionBankCategoryDto) => Promise<QuestionBankCategoryDto>;
  updateCategory: (categoryId: number, dto: UpdateQuestionBankCategoryDto) => Promise<QuestionBankCategoryDto>;
  deleteCategory: (categoryId: number, strategy?: number) => Promise<boolean>;
  
  // Tree manipulation operations
  moveCategory: (categoryId: number, dto: MoveCategoryDto) => Promise<boolean>;
  copyCategory: (categoryId: number, dto: CopyCategoryDto) => Promise<QuestionBankCategoryDto>;
  reorderCategories: (dto: ReorderCategoriesDto) => Promise<boolean>;
  
  // Question operations
  assignQuestionToCategory: (questionId: number, categoryId: number, isPrimary?: boolean) => Promise<boolean>;
  removeQuestionFromCategory: (questionId: number, categoryId: number) => Promise<boolean>;
  getQuestionCategories: (questionId: number) => Promise<QuestionBankCategoryDto[]>;
  getCategoryQuestions: (categoryId: number, page?: number, pageSize?: number) => Promise<any[]>;
  
  // Search and filter operations
  searchCategories: (searchDto: CategorySearchDto) => Promise<QuestionBankCategoryDto[]>;
  filterCategories: (filter: CategoryFilterDto) => Promise<QuestionBankCategoryDto[]>;
  clearSearch: () => void;
  clearFilters: () => void;
  
  // Tree validation operations
  validateTreeStructure: () => Promise<boolean>;
  validateCategoryHierarchy: (categoryId: number) => Promise<boolean>;
  getValidationReport: () => Promise<TreeValidationResultDto | null>;
  
  // Tree statistics
  getTreeStatistics: () => Promise<TreeStatisticsDto | null>;
  
  // UI state management
  selectCategory: (category: QuestionBankCategoryDto | null) => void;
  toggleNodeExpansion: (nodeId: string) => void;
  expandNode: (nodeId: string) => void;
  collapseNode: (nodeId: string) => void;
  expandPathToCategory: (categoryId: number) => Promise<void>;
  setSearchQuery: (query: string) => void;
  updateFilters: (newFilters: Partial<CategoryFilterDto>) => void;
  
  // Utility operations
  flattenTreeToCategories: (nodes: QuestionBankCategoryDto[]) => QuestionBankCategoryDto[];
  findCategoryById: (categoryId: number) => QuestionBankCategoryDto | null;
  getCategoryPath: (categoryId: number) => QuestionBankCategoryDto[];
  isCategoryExpanded: (categoryId: number) => boolean;
  canAddChild: (categoryId: number) => boolean;
  canMoveCategory: (categoryId: number, targetParentId?: number) => boolean;
  
  // Event handling
  onCategoryEvent: (event: CategoryEvent) => void;
  onTreeChange: (event: TreeChangeEvent) => void;
  
  // Cache management
  invalidateCache: (categoryId?: number) => void;
  clearAllCache: () => void;
}

export const useQuestionBankTree = (
  initialFilters?: Partial<CategoryFilterDto>,
  autoLoad: boolean = true
): UseQuestionBankTreeResult => {
  // State management
  const [treeData, setTreeData] = useState<QuestionBankTreeDto | null>(null);
  const [categories, setCategories] = useState<QuestionBankCategoryDto[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<QuestionBankCategoryDto | null>(null);
  const [expandedNodes, setExpandedNodes] = useState<string[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [filters, setFilters] = useState<CategoryFilterDto>({
    page: 1,
    pageSize: DEFAULT_PAGE_SIZE,
    isActive: true,
    ...initialFilters
  });
  
  // Loading and error states
  const [isLoading, setIsLoading] = useState(false);
  const [isUpdating, setIsUpdating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // Refs for tracking operations
  const abortControllerRef = useRef<AbortController | null>(null);
  const lastSearchRef = useRef<string>('');
  const lastFiltersRef = useRef<CategoryFilterDto>(filters);

  // Utility function to flatten tree structure
  const flattenTreeToCategories = useCallback((nodes: QuestionBankCategoryDto[]): QuestionBankCategoryDto[] => {
    const result: QuestionBankCategoryDto[] = [];
    
    const traverse = (categories: QuestionBankCategoryDto[]) => {
      for (const category of categories) {
        result.push(category);
        if (category.children && category.children.length > 0) {
          traverse(category.children);
        }
      }
    };
    
    traverse(nodes);
    return result;
  }, []);

  // Utility function to find category by ID
  const findCategoryById = useCallback((categoryId: number): QuestionBankCategoryDto | null => {
    const findInTree = (nodes: QuestionBankCategoryDto[]): QuestionBankCategoryDto | null => {
      for (const node of nodes) {
        if (node.categoryId === categoryId) return node;
        if (node.children && node.children.length > 0) {
          const found = findInTree(node.children);
          if (found) return found;
        }
      }
      return null;
    };

    return treeData ? findInTree(treeData.categories) : null;
  }, [treeData]);

  // Utility function to check if category can have children
  const canAddChild = useCallback((categoryId: number): boolean => {
    const category = findCategoryById(categoryId);
    if (!category) return false;
    
    return category.treeLevel < MAX_TREE_DEPTH;
  }, [findCategoryById]);

  // Utility function to check if category can be moved
  const canMoveCategory = useCallback((categoryId: number, targetParentId?: number): boolean => {
    if (!targetParentId) return true; // Moving to root is always allowed
    
    const category = findCategoryById(categoryId);
    const targetParent = findCategoryById(targetParentId);
    
    if (!category || !targetParent) return false;
    
    // Check if target is not a descendant of the category
    const descendants = categories.filter(c => 
      c.treePath?.includes(`-${categoryId}-`)
    );
    
    return !descendants.some(d => d.categoryId === targetParentId);
  }, [findCategoryById, categories]);

  // Main tree refresh function
  const refreshTree = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      
      // Cancel any ongoing requests
      if (abortControllerRef.current) {
        abortControllerRef.current.abort();
      }
      
      abortControllerRef.current = new AbortController();
      
      const tree = await questionBankTreeService.getCompleteTree();
      setTreeData(tree);
      
      // Flatten tree to categories array
      const flatCategories = flattenTreeToCategories(tree.categories);
      setCategories(flatCategories);
      
      // Restore expanded nodes from localStorage
      const savedExpandedNodes = localStorage.getItem('question-bank-tree-expanded-nodes');
      if (savedExpandedNodes) {
        try {
          const parsed = JSON.parse(savedExpandedNodes);
          if (Array.isArray(parsed)) {
            setExpandedNodes(parsed);
          }
        } catch (e) {
          console.warn('Failed to parse saved expanded nodes');
        }
      }
      
    } catch (err) {
      if (err instanceof Error && err.name === 'AbortError') {
        return; // Request was cancelled
      }
      
      const errorMessage = err instanceof Error ? err.message : 'Failed to load tree';
      setError(errorMessage);
      console.error('Error refreshing tree:', err);
    } finally {
      setIsLoading(false);
    }
  }, [flattenTreeToCategories]);

  // Category operations
  const createCategory = useCallback(async (dto: CreateQuestionBankCategoryDto): Promise<QuestionBankCategoryDto> => {
    try {
      setIsUpdating(true);
      setError(null);
      
      const newCategory = await questionBankTreeService.createCategory(dto);
      
      // Refresh tree to show new category
      await refreshTree();
      
      // Expand parent and select new category
      if (dto.parentCategoryId) {
        expandNode(`category-${dto.parentCategoryId}`);
      }
      selectCategory(newCategory);
      
      return newCategory;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.OPERATION_FAILED;
      setError(errorMessage);
      console.error('Error creating category:', err);
      throw err;
    } finally {
      setIsUpdating(false);
    }
  }, [refreshTree]);

  const updateCategory = useCallback(async (categoryId: number, dto: UpdateQuestionBankCategoryDto): Promise<QuestionBankCategoryDto> => {
    try {
      setIsUpdating(true);
      setError(null);
      
      const updatedCategory = await questionBankTreeService.updateCategory(categoryId, dto);
      
      // Refresh tree to show updates
      await refreshTree();
      
      // Update selected category if it's the one being updated
      if (selectedCategory?.categoryId === categoryId) {
        selectCategory(updatedCategory);
      }
      
      return updatedCategory;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.OPERATION_FAILED;
      setError(errorMessage);
      console.error('Error updating category:', err);
      throw err;
    } finally {
      setIsUpdating(false);
    }
  }, [refreshTree, selectedCategory]);

  const deleteCategory = useCallback(async (categoryId: number, strategy: number = 1): Promise<boolean> => {
    try {
      setIsUpdating(true);
      setError(null);
      
      const success = await questionBankTreeService.deleteCategory(categoryId, strategy);
      
      if (success) {
        // Clear selection if deleted category was selected
        if (selectedCategory?.categoryId === categoryId) {
          selectCategory(null);
        }
        
        // Refresh tree to remove deleted category
        await refreshTree();
      }
      
      return success;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.OPERATION_FAILED;
      setError(errorMessage);
      console.error('Error deleting category:', err);
      throw err;
    } finally {
      setIsUpdating(false);
    }
  }, [refreshTree, selectedCategory]);

  // Tree manipulation operations
  const moveCategory = useCallback(async (categoryId: number, dto: MoveCategoryDto): Promise<boolean> => {
    try {
      setIsUpdating(true);
      setError(null);
      
      const success = await questionBankTreeService.moveCategory(categoryId, dto);
      
      if (success) {
        // Refresh tree to show new structure
        await refreshTree();
      }
      
      return success;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.OPERATION_FAILED;
      setError(errorMessage);
      console.error('Error moving category:', err);
      throw err;
    } finally {
      setIsUpdating(false);
    }
  }, [refreshTree]);

  const copyCategory = useCallback(async (categoryId: number, dto: CopyCategoryDto): Promise<QuestionBankCategoryDto> => {
    try {
      setIsUpdating(true);
      setError(null);
      
      const copiedCategory = await questionBankTreeService.copyCategory(categoryId, dto);
      
      // Refresh tree to show new category
      await refreshTree();
      
      return copiedCategory;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.OPERATION_FAILED;
      setError(errorMessage);
      console.error('Error copying category:', err);
      throw err;
    } finally {
      setIsUpdating(false);
    }
  }, [refreshTree]);

  const reorderCategories = useCallback(async (dto: ReorderCategoriesDto): Promise<boolean> => {
    try {
      setIsUpdating(true);
      setError(null);
      
      const success = await questionBankTreeService.reorderCategories(dto);
      
      if (success) {
        // Refresh tree to show new order
        await refreshTree();
      }
      
      return success;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.OPERATION_FAILED;
      setError(errorMessage);
      console.error('Error reordering categories:', err);
      throw err;
    } finally {
      setIsUpdating(false);
    }
  }, [refreshTree]);

  // Search and filter operations
  const searchCategories = useCallback(async (searchDto: CategorySearchDto): Promise<QuestionBankCategoryDto[]> => {
    try {
      setError(null);
      lastSearchRef.current = searchDto.query;
      
      const results = await questionBankTreeService.searchCategories(searchDto);
      
      // Expand nodes containing search results
      const resultIds = results.map(r => `category-${r.categoryId}`);
      setExpandedNodes(prev => [...new Set([...prev, ...resultIds])]);
      
      return results;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.OPERATION_FAILED;
      setError(errorMessage);
      console.error('Error searching categories:', err);
      return [];
    }
  }, []);

  const filterCategories = useCallback(async (filter: CategoryFilterDto): Promise<QuestionBankCategoryDto[]> => {
    try {
      setError(null);
      lastFiltersRef.current = filter;
      
      const result = await questionBankTreeService.getCategories(filter);
      return result.data;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.OPERATION_FAILED;
      setError(errorMessage);
      console.error('Error filtering categories:', err);
      return [];
    }
  }, []);

  // Tree validation operations
  const validateTreeStructure = useCallback(async (): Promise<boolean> => {
    try {
      setError(null);
      
      const result = await questionBankTreeService.validateTreeStructure();
      return result.isValid;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.VALIDATION_FAILED;
      setError(errorMessage);
      console.error('Error validating tree structure:', err);
      return false;
    }
  }, []);

  const validateCategoryHierarchy = useCallback(async (categoryId: number): Promise<boolean> => {
    try {
      setError(null);
      
      const result = await questionBankTreeService.validateCategoryHierarchy(categoryId);
      return result;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.VALIDATION_FAILED;
      setError(errorMessage);
      console.error('Error validating category hierarchy:', err);
      return false;
    }
  }, []);

  const getValidationReport = useCallback(async (): Promise<TreeValidationResultDto | null> => {
    try {
      setError(null);
      
      const result = await questionBankTreeService.validateTreeStructure();
      return result;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.VALIDATION_FAILED;
      setError(errorMessage);
      console.error('Error getting validation report:', err);
      return null;
    }
  }, []);

  // Tree statistics
  const getTreeStatistics = useCallback(async (): Promise<TreeStatisticsDto | null> => {
    try {
      setError(null);
      
      const result = await questionBankTreeService.getTreeStatistics();
      return result;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.OPERATION_FAILED;
      setError(errorMessage);
      console.error('Error getting tree statistics:', err);
      return null;
    }
  }, []);

  // UI state management
  const selectCategory = useCallback((category: QuestionBankCategoryDto | null) => {
    setSelectedCategory(category);
    
    // Save to localStorage
    if (category) {
      localStorage.setItem('question-bank-tree-selected-node', JSON.stringify(category.categoryId));
    } else {
      localStorage.removeItem('question-bank-tree-selected-node');
    }
  }, []);

  const toggleNodeExpansion = useCallback((nodeId: string) => {
    setExpandedNodes(prev => {
      const newExpanded = prev.includes(nodeId)
        ? prev.filter(id => id !== nodeId)
        : [...prev, nodeId];
      
      // Save to localStorage
      localStorage.setItem('question-bank-tree-expanded-nodes', JSON.stringify(newExpanded));
      return newExpanded;
    });
  }, []);

  const expandNode = useCallback((nodeId: string) => {
    setExpandedNodes(prev => {
      if (prev.includes(nodeId)) return prev;
      
      const newExpanded = [...prev, nodeId];
      localStorage.setItem('question-bank-tree-expanded-nodes', JSON.stringify(newExpanded));
      return newExpanded;
    });
  }, []);

  const collapseNode = useCallback((nodeId: string) => {
    setExpandedNodes(prev => {
      if (!prev.includes(nodeId)) return prev;
      
      const newExpanded = prev.filter(id => id !== nodeId);
      localStorage.setItem('question-bank-tree-expanded-nodes', JSON.stringify(newExpanded));
      return newExpanded;
    });
  }, []);

  const expandPathToCategory = useCallback(async (categoryId: number) => {
    try {
      // Get ancestors and expand them
      const ancestors = await questionBankTreeService.getAncestors(categoryId);
      const ancestorIds = ancestors.map(a => `category-${a.categoryId}`);
      
      setExpandedNodes(prev => {
        const newExpanded = [...new Set([...prev, ...ancestorIds])];
        localStorage.setItem('question-bank-tree-expanded-nodes', JSON.stringify(newExpanded));
        return newExpanded;
      });
    } catch (error) {
      console.error('Error expanding path to category:', error);
    }
  }, []);

  const clearSearch = useCallback(() => {
    setSearchQuery('');
    lastSearchRef.current = '';
  }, []);

  const clearFilters = useCallback(() => {
    const defaultFilters: CategoryFilterDto = {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      isActive: true
    };
    setFilters(defaultFilters);
    lastFiltersRef.current = defaultFilters;
  }, []);

  const updateFilters = useCallback((newFilters: Partial<CategoryFilterDto>) => {
    setFilters(prev => {
      const updated = { ...prev, ...newFilters, page: 1 }; // Reset to first page
      return updated;
    });
  }, []);

  // Utility operations
  const getCategoryPath = useCallback(async (categoryId: number): Promise<QuestionBankCategoryDto[]> => {
    try {
      return await questionBankTreeService.getCategoryPath(categoryId);
    } catch (err) {
      console.error('Error getting category path:', err);
      return [];
    }
  }, []);

  const isCategoryExpanded = useCallback((categoryId: number): boolean => {
    return expandedNodes.includes(`category-${categoryId}`);
  }, [expandedNodes]);

  // Event handling
  const onCategoryEvent = useCallback((event: CategoryEvent) => {
    // Emit custom event for external listeners
    const customEvent = new CustomEvent('question-bank-category-event', { detail: event });
    window.dispatchEvent(customEvent);
  }, []);

  const onTreeChange = useCallback((event: TreeChangeEvent) => {
    // Emit custom event for external listeners
    const customEvent = new CustomEvent('question-bank-tree-change', { detail: event });
    window.dispatchEvent(customEvent);
  }, []);

  // Cache management
  const invalidateCache = useCallback((categoryId?: number) => {
    if (categoryId) {
      questionBankTreeService.invalidateCategoryCache(categoryId);
    } else {
      questionBankTreeService.invalidateTreeCache();
    }
  }, []);

  const clearAllCache = useCallback(() => {
    questionBankTreeService.invalidateTreeCache();
  }, []);

  // Initial load
  useEffect(() => {
    if (autoLoad) {
      refreshTree();
    }
    
    // Restore selected category from localStorage
    const savedSelectedNode = localStorage.getItem('question-bank-tree-selected-node');
    if (savedSelectedNode) {
      try {
        const categoryId = JSON.parse(savedSelectedNode);
        // We'll set the selected category after the tree loads
      } catch (e) {
        console.warn('Failed to parse saved selected node');
      }
    }
  }, [autoLoad, refreshTree]);

  // Cleanup on unmount
  useEffect(() => {
    return () => {
      if (abortControllerRef.current) {
        abortControllerRef.current.abort();
      }
    };
  }, []);

  // Update selected category when tree data changes
  useEffect(() => {
    if (treeData && !selectedCategory) {
      const savedSelectedNode = localStorage.getItem('question-bank-tree-selected-node');
      if (savedSelectedNode) {
        try {
          const categoryId = JSON.parse(savedSelectedNode);
          const category = findCategoryById(categoryId);
          if (category) {
            selectCategory(category);
          }
        } catch (e) {
          console.warn('Failed to parse saved selected node');
        }
      }
    }
  }, [treeData, selectedCategory, findCategoryById, selectCategory]);

  return {
    // State
    treeData,
    categories,
    selectedCategory,
    expandedNodes,
    searchQuery,
    filters,
    
    // Loading and error states
    isLoading,
    isUpdating,
    error,
    
    // Tree operations
    refreshTree,
    getCategory: questionBankTreeService.getCategory.bind(questionBankTreeService),
    getChildren: questionBankTreeService.getChildren.bind(questionBankTreeService),
    getAncestors: questionBankTreeService.getAncestors.bind(questionBankTreeService),
    getDescendants: questionBankTreeService.getDescendants.bind(questionBankTreeService),
    getSiblings: questionBankTreeService.getSiblings.bind(questionBankTreeService),
    
    // Category CRUD operations
    createCategory,
    updateCategory,
    deleteCategory,
    
    // Tree manipulation operations
    moveCategory,
    copyCategory,
    reorderCategories,
    
    // Question operations (placeholder implementations)
    assignQuestionToCategory: async () => true,
    removeQuestionFromCategory: async () => true,
    getQuestionCategories: async () => [],
    getCategoryQuestions: async () => [],
    
    // Search and filter operations
    searchCategories,
    filterCategories,
    clearSearch,
    clearFilters,
    
    // Tree validation operations
    validateTreeStructure,
    validateCategoryHierarchy,
    getValidationReport,
    
    // Tree statistics
    getTreeStatistics,
    
    // UI state management
    selectCategory,
    toggleNodeExpansion,
    expandNode,
    collapseNode,
    expandPathToCategory,
    setSearchQuery,
    updateFilters,
    
    // Utility operations
    flattenTreeToCategories,
    findCategoryById,
    getCategoryPath,
    isCategoryExpanded,
    canAddChild,
    canMoveCategory,
    
    // Event handling
    onCategoryEvent,
    onTreeChange,
    
    // Cache management
    invalidateCache,
    clearAllCache
  };
};
