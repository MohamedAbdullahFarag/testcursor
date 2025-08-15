// Question Bank Tree Component
// Main component for displaying and managing the question bank tree structure

import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { TreeView, TreeItem } from '@mui/x-tree-view';
import { 
  ExpandMore, 
  ChevronRight, 
  Folder, 
  FolderOpen, 
  Assignment,
  Add,
  Edit,
  Delete,
  ContentCopy,
  DragIndicator,
  Search,
  FilterList,
  Refresh,
  Settings
} from '@mui/icons-material';
import { 
  Box, 
  Typography, 
  Button, 
  TextField, 
  IconButton, 
  Tooltip, 
  Chip, 
  Alert,
  CircularProgress,
  Divider,
  Paper
} from '@mui/material';
import { useQuestionBankTree } from '../hooks/useQuestionBankTree';
import { TreeNode } from './TreeNode';
import { TreeSearch } from './TreeSearch';
import { CategoryManager } from './CategoryManager';
import { BreadcrumbNavigation } from './BreadcrumbNavigation';
import { TreeActions } from './TreeActions';
import { CategoryFilters } from './CategoryFilters';
import { 
  QuestionBankCategoryDto, 
  CreateQuestionBankCategoryDto,
  UpdateQuestionBankCategoryDto,
  MoveCategoryDto,
  QuestionBankTreeProps
} from '../types/questionBankTree.types';
import { 
  SUCCESS_MESSAGES, 
  ERROR_MESSAGES, 
  INFO_MESSAGES,
  MAX_TREE_DEPTH,
  TREE_NODE_TYPES
} from '../constants/treeOperations';

export const QuestionBankTree: React.FC<QuestionBankTreeProps> = ({
  onCategorySelect,
  onQuestionSelect,
  selectedCategoryId,
  readonly = false,
  showQuestions = true,
  enableDragDrop = false,
  maxDepth = MAX_TREE_DEPTH,
  className = '',
  initialExpandedNodes = [],
  searchEnabled = true,
  filterEnabled = true
}) => {
  // Local state
  const [showCategoryManager, setShowCategoryManager] = useState(false);
  const [editingCategory, setEditingCategory] = useState<QuestionBankCategoryDto | null>(null);
  const [showFilters, setShowFilters] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [notification, setNotification] = useState<{
    type: 'success' | 'error' | 'info' | 'warning';
    message: string;
  } | null>(null);

  // Tree management hook
  const {
    treeData,
    categories,
    selectedCategory,
    expandedNodes,
    isLoading,
    isUpdating,
    error,
    refreshTree,
    createCategory,
    updateCategory,
    deleteCategory,
    moveCategory,
    copyCategory,
    searchCategories,
    selectCategory,
    toggleNodeExpansion,
    expandPathToCategory,
    setSearchQuery: setHookSearchQuery,
    updateFilters,
    clearSearch,
    clearFilters,
    canAddChild,
    canMoveCategory
  } = useQuestionBankTree();

  // Initialize expanded nodes
  useEffect(() => {
    if (initialExpandedNodes.length > 0) {
      // This would be handled by the hook, but we can add additional logic here
    }
  }, [initialExpandedNodes]);

  // Handle selected category changes
  useEffect(() => {
    if (selectedCategoryId && selectedCategoryId !== selectedCategory?.categoryId) {
      const category = categories.find(c => c.categoryId === selectedCategoryId);
      if (category) {
        selectCategory(category);
        expandPathToCategory(selectedCategoryId);
      }
    }
  }, [selectedCategoryId, selectedCategory, categories, selectCategory, expandPathToCategory]);

  // Handle external category selection
  useEffect(() => {
    if (onCategorySelect && selectedCategory) {
      onCategorySelect(selectedCategory);
    }
  }, [selectedCategory, onCategorySelect]);

  // Show notifications
  const showNotification = useCallback((type: 'success' | 'error' | 'info' | 'warning', message: string) => {
    setNotification({ type, message });
    setTimeout(() => setNotification(null), 5000);
  }, []);

  // Handle category creation
  const handleCategoryCreate = useCallback(async (parentId: number | null, categoryData: CreateQuestionBankCategoryDto) => {
    try {
      const newCategory = await createCategory(categoryData);
      setShowCategoryManager(false);
      showNotification('success', SUCCESS_MESSAGES.CATEGORY_CREATED);
      
      // Expand parent and select new category
      if (parentId) {
        toggleNodeExpansion(`category-${parentId}`);
      }
      selectCategory(newCategory);
      
      if (onCategorySelect) {
        onCategorySelect(newCategory);
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : ERROR_MESSAGES.OPERATION_FAILED;
      showNotification('error', errorMessage);
    }
  }, [createCategory, toggleNodeExpansion, selectCategory, onCategorySelect, showNotification]);

  // Handle category update
  const handleCategoryUpdate = useCallback(async (categoryId: number, categoryData: UpdateQuestionBankCategoryDto) => {
    try {
      const updatedCategory = await updateCategory(categoryId, categoryData);
      setEditingCategory(null);
      showNotification('success', SUCCESS_MESSAGES.CATEGORY_UPDATED);
      
      if (onCategorySelect) {
        onCategorySelect(updatedCategory);
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : ERROR_MESSAGES.OPERATION_FAILED;
      showNotification('error', errorMessage);
    }
  }, [updateCategory, onCategorySelect, showNotification]);

  // Handle category deletion
  const handleCategoryDelete = useCallback(async (categoryId: number) => {
    if (!window.confirm('Are you sure you want to delete this category? This action cannot be undone.')) {
      return;
    }

    try {
      const success = await deleteCategory(categoryId);
      if (success) {
        showNotification('success', SUCCESS_MESSAGES.CATEGORY_DELETED);
        
        // Clear selection if deleted category was selected
        if (selectedCategory?.categoryId === categoryId) {
          selectCategory(null);
        }
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : ERROR_MESSAGES.OPERATION_FAILED;
      showNotification('error', errorMessage);
    }
  }, [deleteCategory, selectedCategory, selectCategory, showNotification]);

  // Handle category move
  const handleCategoryMove = useCallback(async (categoryId: number, newParentId: number | null) => {
    try {
      const success = await moveCategory(categoryId, { newParentId });
      if (success) {
        showNotification('success', SUCCESS_MESSAGES.CATEGORY_MOVED);
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : ERROR_MESSAGES.OPERATION_FAILED;
      showNotification('error', errorMessage);
    }
  }, [moveCategory, showNotification]);

  // Handle category copy
  const handleCategoryCopy = useCallback(async (categoryId: number, newParentId: number | null, newName?: string) => {
    try {
      const copiedCategory = await copyCategory(categoryId, { 
        newParentId, 
        newName: newName || `Copy of ${categories.find(c => c.categoryId === categoryId)?.name}`,
        includeChildren: true,
        includeQuestions: false
      });
      
      showNotification('success', SUCCESS_MESSAGES.CATEGORY_COPIED);
      
      // Expand parent and select new category
      if (newParentId) {
        toggleNodeExpansion(`category-${newParentId}`);
      }
      selectCategory(copiedCategory);
      
      if (onCategorySelect) {
        onCategorySelect(copiedCategory);
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : ERROR_MESSAGES.OPERATION_FAILED;
      showNotification('error', errorMessage);
    }
  }, [copyCategory, categories, toggleNodeExpansion, selectCategory, onCategorySelect, showNotification]);

  // Handle search
  const handleSearch = useCallback(async (query: string) => {
    setSearchQuery(query);
    setHookSearchQuery(query);
    
    if (query.trim()) {
      try {
        const searchResults = await searchCategories({ 
          query, 
          includeQuestions: showQuestions,
          maxResults: 50
        });
        
        // Expand nodes containing search results
        const resultIds = searchResults.map(r => `category-${r.categoryId}`);
        resultIds.forEach(id => {
          if (!expandedNodes.includes(id)) {
            toggleNodeExpansion(id);
          }
        });
        
        showNotification('info', `Found ${searchResults.length} matching categories`);
      } catch (error) {
        const errorMessage = error instanceof Error ? error.message : ERROR_MESSAGES.OPERATION_FAILED;
        showNotification('error', errorMessage);
      }
    }
  }, [searchCategories, showQuestions, expandedNodes, toggleNodeExpansion, setHookSearchQuery, showNotification]);

  // Handle node selection
  const handleNodeSelect = useCallback((event: React.SyntheticEvent, nodeIds: string | string[] | null) => {
    if (typeof nodeIds === 'string') {
      if (nodeIds.startsWith('category-')) {
        const categoryId = parseInt(nodeIds.replace('category-', ''));
        const category = categories.find(c => c.categoryId === categoryId);
        if (category) {
          selectCategory(category);
          if (onCategorySelect) {
            onCategorySelect(category);
          }
        }
      } else if (nodeIds.startsWith('question-')) {
        const questionId = parseInt(nodeIds.replace('question-', ''));
        if (onQuestionSelect) {
          onQuestionSelect(questionId);
        }
      }
    }
  }, [categories, selectCategory, onCategorySelect, onQuestionSelect]);

  // Handle node toggle
  const handleNodeToggle = useCallback((event: React.SyntheticEvent, nodeIds: string[]) => {
    // This will be handled by the individual TreeNode components
  }, []);

  // Render tree node
  const renderTreeNode = useCallback((category: QuestionBankCategoryDto, depth: number = 0): React.ReactNode => {
    const nodeId = `category-${category.categoryId}`;
    const isExpanded = expandedNodes.includes(nodeId);
    const isSelected = selectedCategory?.categoryId === category.categoryId;
    const canAdd = canAddChild(category.categoryId);
    const canMove = canMoveCategory(category.categoryId);

    return (
      <TreeNode
        key={nodeId}
        category={category}
        isSelected={isSelected}
        isExpanded={isExpanded}
        readonly={readonly}
        showQuestions={showQuestions}
        enableDragDrop={enableDragDrop}
        depth={depth}
        maxDepth={maxDepth}
        canAddChild={canAdd}
        canMove={canMove}
        onCategoryCreate={handleCategoryCreate}
        onCategoryUpdate={handleCategoryUpdate}
        onCategoryDelete={handleCategoryDelete}
        onCategoryMove={handleCategoryMove}
        onCategoryCopy={handleCategoryCopy}
        onToggleExpansion={() => toggleNodeExpansion(nodeId)}
        onSelect={() => {
          selectCategory(category);
          if (onCategorySelect) {
            onCategorySelect(category);
          }
        }}
        searchQuery={searchQuery}
      />
    );
  }, [
    expandedNodes,
    selectedCategory,
    readonly,
    showQuestions,
    enableDragDrop,
    maxDepth,
    canAddChild,
    canMoveCategory,
    handleCategoryCreate,
    handleCategoryUpdate,
    handleCategoryDelete,
    handleCategoryMove,
    handleCategoryCopy,
    toggleNodeExpansion,
    selectCategory,
    onCategorySelect,
    searchQuery
  ]);

  // Render tree structure recursively
  const renderTreeStructure = useCallback((categories: QuestionBankCategoryDto[], depth: number = 0): React.ReactNode => {
    return categories
      .filter(category => category.isActive || !readonly) // Show inactive categories in edit mode
      .sort((a, b) => a.sortOrder - b.sortOrder)
      .map(category => renderTreeNode(category, depth));
  }, [renderTreeNode, readonly]);

  // Memoized tree content
  const treeContent = useMemo(() => {
    if (!treeData || !treeData.categories) {
      return null;
    }

    return renderTreeStructure(treeData.categories);
  }, [treeData, renderTreeStructure]);

  // Loading state
  if (isLoading) {
    return (
      <Box className={`question-bank-tree ${className}`} p={3}>
        <Box display="flex" alignItems="center" justifyContent="center" minHeight={200}>
          <CircularProgress size={40} />
          <Typography variant="body1" ml={2}>
            Loading question bank tree...
          </Typography>
        </Box>
      </Box>
    );
  }

  // Error state
  if (error) {
    return (
      <Box className={`question-bank-tree ${className}`} p={3}>
        <Alert severity="error" action={
          <Button color="inherit" size="small" onClick={refreshTree}>
            Retry
          </Button>
        }>
          Error loading tree: {error}
        </Alert>
      </Box>
    );
  }

  return (
    <Box className={`question-bank-tree ${className}`}>
      {/* Notification */}
      {notification && (
        <Alert 
          severity={notification.type} 
          onClose={() => setNotification(null)}
          sx={{ mb: 2 }}
        >
          {notification.message}
        </Alert>
      )}

      {/* Header */}
      <Paper elevation={1} sx={{ p: 2, mb: 2 }}>
        <Box display="flex" alignItems="center" justifyContent="space-between" mb={2}>
          <Typography variant="h5" component="h2">
            Question Bank Categories
          </Typography>
          
          {!readonly && (
            <Box display="flex" gap={1}>
              <Tooltip title="Create Category">
                <IconButton 
                  color="primary" 
                  onClick={() => setShowCategoryManager(true)}
                  disabled={isUpdating}
                >
                  <Add />
                </IconButton>
              </Tooltip>
              
              <Tooltip title="Refresh Tree">
                <IconButton 
                  onClick={refreshTree}
                  disabled={isLoading || isUpdating}
                >
                  <Refresh />
                </IconButton>
              </Tooltip>
              
              {filterEnabled && (
                <Tooltip title="Toggle Filters">
                  <IconButton 
                    onClick={() => setShowFilters(!showFilters)}
                    color={showFilters ? 'primary' : 'default'}
                  >
                    <FilterList />
                  </IconButton>
                </Tooltip>
              )}
            </Box>
          )}
        </Box>

        {/* Search Bar */}
        {searchEnabled && (
          <TreeSearch
            onSearch={handleSearch}
            placeholder="Search categories and topics..."
            value={searchQuery}
            onClear={clearSearch}
            disabled={isLoading || isUpdating}
          />
        )}

        {/* Filters */}
        {filterEnabled && showFilters && (
          <Box mt={2}>
            <CategoryFilters
              onFilterChange={updateFilters}
              onClear={clearFilters}
              disabled={isLoading || isUpdating}
            />
          </Box>
        )}
      </Paper>

      {/* Breadcrumb Navigation */}
      {selectedCategory && (
        <Box mb={2}>
          <BreadcrumbNavigation
            categoryId={selectedCategory.categoryId}
            onCategorySelect={selectCategory}
          />
        </Box>
      )}

      {/* Tree View */}
      <Paper elevation={1} sx={{ p: 2 }}>
        {treeData && treeData.categories.length > 0 ? (
          <TreeView
            defaultCollapseIcon={<ExpandMore />}
            defaultExpandIcon={<ChevronRight />}
            expanded={expandedNodes}
            selected={selectedCategory ? `category-${selectedCategory.categoryId}` : undefined}
            onNodeToggle={handleNodeToggle}
            onNodeSelect={handleNodeSelect}
            multiSelect={false}
            className="tree-view"
            sx={{
              '& .MuiTreeItem-root': {
                '& .MuiTreeItem-content': {
                  padding: '4px 8px',
                  borderRadius: '4px',
                  '&:hover': {
                    backgroundColor: 'action.hover',
                  },
                  '&.Mui-selected': {
                    backgroundColor: 'primary.main',
                    color: 'primary.contrastText',
                    '&:hover': {
                      backgroundColor: 'primary.dark',
                    },
                  },
                },
              },
            }}
          >
            {treeContent}
          </TreeView>
        ) : (
          <Box textAlign="center" py={4}>
            <Folder sx={{ fontSize: 48, color: 'text.secondary', mb: 1 }} />
            <Typography variant="h6" color="text.secondary" gutterBottom>
              No categories found
            </Typography>
            <Typography variant="body2" color="text.secondary" mb={2}>
              Get started by creating your first category
            </Typography>
            {!readonly && (
              <Button
                variant="contained"
                startIcon={<Add />}
                onClick={() => setShowCategoryManager(true)}
                disabled={isUpdating}
              >
                Create First Category
              </Button>
            )}
          </Box>
        )}
      </Paper>

      {/* Tree Statistics */}
      {treeData && (
        <Paper elevation={1} sx={{ p: 2, mt: 2 }}>
          <Box display="flex" justifyContent="space-between" alignItems="center">
            <Typography variant="body2" color="text.secondary">
              Total Categories: {treeData.totalCategories} | 
              Max Depth: {treeData.maxDepth} | 
              Last Modified: {new Date(treeData.lastModified).toLocaleDateString()}
            </Typography>
            
            {!readonly && (
              <Box display="flex" gap={1}>
                <Tooltip title="Tree Statistics">
                  <IconButton size="small">
                    <Settings />
                  </IconButton>
                </Tooltip>
              </Box>
            )}
          </Box>
        </Paper>
      )}

      {/* Category Manager Modal */}
      {showCategoryManager && (
        <CategoryManager
          parentCategoryId={selectedCategory?.categoryId || undefined}
          onClose={() => setShowCategoryManager(false)}
          onCategoryCreated={handleCategoryCreate}
          mode="create"
        />
      )}

      {/* Category Editor Modal */}
      {editingCategory && (
        <CategoryManager
          parentCategoryId={editingCategory.parentCategoryId || undefined}
          onClose={() => setEditingCategory(null)}
          onCategoryCreated={handleCategoryUpdate}
          mode="edit"
          categoryId={editingCategory.categoryId}
          initialData={editingCategory}
        />
      )}
    </Box>
  );
};
