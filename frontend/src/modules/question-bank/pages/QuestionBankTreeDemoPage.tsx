import React, { useState, useCallback } from 'react';
import { 
  Box, 
  Typography, 
  Paper, 
  Button, 
  Tabs, 
  Tab, 
  Divider,
  Alert,
  Chip,
  Grid
} from '@mui/material';
import { 
  TreeView, 
  Folder, 
  Assignment, 
  TrendingUp,
  DragIndicator
} from '@mui/icons-material';
import { QuestionBankTree } from '../components/QuestionBankTree';
import { TreeDragDrop } from '../components/TreeDragDrop';
import { CategoryFilters } from '../components/CategoryFilters';
import { TreeActions } from '../components/TreeActions';
import { BreadcrumbNavigation } from '../components/BreadcrumbNavigation';
import { CategoryManager } from '../components/CategoryManager';
import { useQuestionBankTree } from '../hooks/useQuestionBankTree';
import { 
  QuestionBankCategoryDto, 
  CreateCategoryDto, 
  UpdateCategoryDto,
  CategoryFilterDto,
  MoveCategoryDto
} from '../types/questionBankTree.types';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`question-bank-tabpanel-${index}`}
      aria-labelledby={`question-bank-tab-${index}`}
      {...other}
    >
      {value === index && (
        <Box sx={{ p: 3 }}>
          {children}
        </Box>
      )}
    </div>
  );
}

export const QuestionBankTreeDemoPage: React.FC = () => {
  const [tabValue, setTabValue] = useState(0);
  const [showCategoryManager, setShowCategoryManager] = useState(false);
  const [editingCategory, setEditingCategory] = useState<QuestionBankCategoryDto | null>(null);
  const [parentCategoryId, setParentCategoryId] = useState<number | null>(null);
  const [filters, setFilters] = useState<CategoryFilterDto>({
    query: '',
    categoryType: undefined,
    level: undefined,
    isActive: true,
    allowQuestions: undefined,
    hasQuestions: undefined,
    minQuestionCount: undefined,
    maxQuestionCount: undefined,
    createdAfter: undefined,
    createdBefore: undefined,
    updatedAfter: undefined,
    updatedBefore: undefined,
    curriculumCode: '',
    gradeLevel: '',
    subject: '',
    sortBy: 'name',
    sortOrder: 'asc',
    page: 1,
    pageSize: 25
  });

  const {
    treeData,
    categories,
    isLoading,
    error,
    refreshTree,
    createCategory,
    updateCategory,
    deleteCategory,
    moveCategory,
    searchCategories,
    assignQuestionToCategory,
    removeQuestionFromCategory
  } = useQuestionBankTree();

  const handleTabChange = useCallback((event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  }, []);

  const handleCategoryCreate = useCallback(async (parentId: number | null, categoryData: CreateCategoryDto) => {
    try {
      await createCategory(categoryData);
      setShowCategoryManager(false);
      setParentCategoryId(null);
    } catch (error) {
      console.error('Error creating category:', error);
    }
  }, [createCategory]);

  const handleCategoryUpdate = useCallback(async (categoryId: number, categoryData: UpdateCategoryDto) => {
    try {
      await updateCategory(categoryId, categoryData);
      setShowCategoryManager(false);
      setEditingCategory(null);
    } catch (error) {
      console.error('Error updating category:', error);
    }
  }, [updateCategory]);

  const handleCategoryDelete = useCallback(async (categoryId: number) => {
    try {
      const success = await deleteCategory(categoryId);
      if (success) {
        console.log('Category deleted successfully');
      }
    } catch (error) {
      console.error('Error deleting category:', error);
    }
  }, [deleteCategory]);

  const handleCategoryMove = useCallback(async (categoryId: number, moveData: MoveCategoryDto) => {
    try {
      const success = await moveCategory(categoryId, moveData);
      if (success) {
        console.log('Category moved successfully');
      }
    } catch (error) {
      console.error('Error moving category:', error);
    }
  }, [moveCategory]);

  const handleFiltersChange = useCallback((newFilters: CategoryFilterDto) => {
    setFilters(newFilters);
    console.log('Filters changed:', newFilters);
  }, []);

  const handleExport = useCallback(async (format: string) => {
    console.log(`Exporting tree in ${format} format`);
    // Implementation would call the export service
  }, []);

  const handleImport = useCallback(async (file: File) => {
    console.log('Importing file:', file.name);
    // Implementation would call the import service
  }, []);

  const handleSettings = useCallback(() => {
    console.log('Opening tree settings');
    // Implementation would open settings dialog
  }, []);

  const openCreateCategory = useCallback((parentId?: number) => {
    setParentCategoryId(parentId || null);
    setEditingCategory(null);
    setShowCategoryManager(true);
  }, []);

  const openEditCategory = useCallback((category: QuestionBankCategoryDto) => {
    setEditingCategory(category);
    setParentCategoryId(null);
    setShowCategoryManager(true);
  }, []);

  const treeStatistics = treeData ? {
    totalCategories: treeData.totalCategories,
    totalQuestions: categories.reduce((sum, cat) => sum + (cat.questionCount || 0), 0),
    maxDepth: treeData.maxDepth,
    lastUpdated: treeData.lastUpdated
  } : null;

  return (
    <Box sx={{ p: 3 }}>
      {/* Header */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h4" gutterBottom>
          Question Bank Tree Management System
        </Typography>
        <Typography variant="body1" color="text.secondary" paragraph>
          Comprehensive hierarchical tree management system for organizing questions, categories, subjects, and topics.
        </Typography>
        
        <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
          <Chip label="Tree Structure" color="primary" />
          <Chip label="Category Management" color="secondary" />
          <Chip label="Drag & Drop" color="success" />
          <Chip label="Search & Filter" color="info" />
          <Chip label="Import/Export" color="warning" />
          <Chip label="Curriculum Alignment" color="error" />
        </Box>
      </Paper>

      {/* Navigation Tabs */}
      <Paper sx={{ mb: 3 }}>
        <Tabs value={tabValue} onChange={handleTabChange} aria-label="Question Bank Tree Tabs">
          <Tab label="Tree View" icon={<TreeView />} />
          <Tab label="Drag & Drop" icon={<DragIndicator />} />
          <Tab label="Filters" icon={<Assignment />} />
          <Tab label="Actions" icon={<TrendingUp />} />
          <Tab label="Navigation" icon={<Folder />} />
        </Tabs>
      </Paper>

      {/* Tab Content */}
      <TabPanel value={tabValue} index={0}>
        <Typography variant="h5" gutterBottom>
          Tree View
        </Typography>
        <Typography variant="body2" color="text.secondary" paragraph>
          Interactive tree view with expandable nodes, category management, and question assignment.
        </Typography>
        
        <QuestionBankTree
          onCategorySelect={(category) => console.log('Selected category:', category)}
          onQuestionSelect={(questionId) => console.log('Selected question:', questionId)}
          readonly={false}
          showQuestions={true}
          enableDragDrop={false}
          maxDepth={6}
        />
      </TabPanel>

      <TabPanel value={tabValue} index={1}>
        <Typography variant="h5" gutterBottom>
          Drag & Drop Interface
        </Typography>
        <Typography variant="body2" color="text.secondary" paragraph>
          Visual drag and drop interface for reorganizing the tree structure with validation and confirmation.
        </Typography>
        
        <TreeDragDrop
          categories={categories}
          onCategoryMove={handleCategoryMove}
          readonly={false}
          maxDepth={6}
        />
      </TabPanel>

      <TabPanel value={tabValue} index={2}>
        <Typography variant="h5" gutterBottom>
          Advanced Filters
        </Typography>
        <Typography variant="body2" color="text.secondary" paragraph>
          Comprehensive filtering system with saved filters, advanced options, and real-time search.
        </Typography>
        
        <CategoryFilters
          filters={filters}
          onFiltersChange={handleFiltersChange}
          showAdvanced={true}
          maxDepth={6}
        />
      </TabPanel>

      <TabPanel value={tabValue} index={3}>
        <Typography variant="h5" gutterBottom>
          Tree Actions
        </Typography>
        <Typography variant="body2" color="text.secondary" paragraph>
          Complete set of tree management actions including create, refresh, expand/collapse, and import/export.
        </Typography>
        
        <TreeActions
          selectedCategoryId={null}
          onCategoryCreate={openCreateCategory}
          onRefresh={refreshTree}
          onExpandAll={() => console.log('Expand all')}
          onCollapseAll={() => console.log('Collapse all')}
          onFilterChange={(filters) => console.log('Filter change:', filters)}
          onSortChange={(field, order) => console.log('Sort change:', field, order)}
          onExport={handleExport}
          onImport={handleImport}
          onSettings={handleSettings}
          showAdvancedActions={true}
          treeStatistics={treeStatistics}
        />
      </TabPanel>

      <TabPanel value={tabValue} index={4}>
        <Typography variant="h5" gutterBottom>
          Breadcrumb Navigation
        </Typography>
        <Typography variant="body2" color="text.secondary" paragraph>
          Contextual navigation breadcrumbs showing the current location in the tree hierarchy.
        </Typography>
        
        <BreadcrumbNavigation
          categoryId={1} // Mock category ID
          onCategorySelect={(category) => console.log('Navigated to:', category)}
          showIcons={true}
          showTypeChips={true}
        />
      </TabPanel>

      {/* Category Manager Modal */}
      <CategoryManager
        open={showCategoryManager}
        onClose={() => {
          setShowCategoryManager(false);
          setEditingCategory(null);
          setParentCategoryId(null);
        }}
        category={editingCategory}
        parentCategoryId={parentCategoryId}
        onCategoryCreated={handleCategoryCreate}
        onCategoryUpdated={handleCategoryUpdate}
        isEditing={!!editingCategory}
      />

      {/* System Status */}
      <Paper sx={{ p: 3, mt: 3 }}>
        <Typography variant="h6" gutterBottom>
          System Status
        </Typography>
        
        <Grid container spacing={2}>
          <Grid item xs={12} md={6}>
            <Typography variant="subtitle2" gutterBottom>
              Tree Data Status
            </Typography>
            {error ? (
              <Alert severity="error">{error}</Alert>
            ) : isLoading ? (
              <Alert severity="info">Loading tree data...</Alert>
            ) : treeData ? (
              <Alert severity="success">
                Tree loaded successfully with {treeData.totalCategories} categories
              </Alert>
            ) : (
              <Alert severity="warning">No tree data available</Alert>
            )}
          </Grid>
          
          <Grid item xs={12} md={6}>
            <Typography variant="subtitle2" gutterBottom>
              Quick Actions
            </Typography>
            <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
              <Button
                size="small"
                variant="outlined"
                onClick={() => openCreateCategory()}
              >
                Create Root Category
              </Button>
              <Button
                size="small"
                variant="outlined"
                onClick={refreshTree}
                disabled={isLoading}
              >
                Refresh Tree
              </Button>
              <Button
                size="small"
                variant="outlined"
                onClick={() => setTabValue(1)}
              >
                Open Drag & Drop
              </Button>
            </Box>
          </Grid>
        </Grid>
      </Paper>

      {/* Features Overview */}
      <Paper sx={{ p: 3, mt: 3 }}>
        <Typography variant="h6" gutterBottom>
          Features Overview
        </Typography>
        
        <Grid container spacing={2}>
          <Grid item xs={12} md={4}>
            <Typography variant="subtitle2" gutterBottom>
              Core Functionality
            </Typography>
            <Box component="ul" sx={{ pl: 2 }}>
              <Typography component="li">Hierarchical tree structure (6 levels)</Typography>
              <Typography component="li">Category CRUD operations</Typography>
              <Typography component="li">Question assignment & management</Typography>
              <Typography component="li">Tree validation & integrity checks</Typography>
            </Box>
          </Grid>
          
          <Grid item xs={12} md={4}>
            <Typography variant="subtitle2" gutterBottom>
              User Experience
            </Typography>
            <Box component="ul" sx={{ pl: 2 }}>
              <Typography component="li">Drag & drop reorganization</Typography>
              <Typography component="li">Advanced search & filtering</Typography>
              <Typography component="li">Breadcrumb navigation</Typography>
              <Typography component="li">Responsive Material-UI design</Typography>
            </Box>
          </Grid>
          
          <Grid item xs={12} md={4}>
            <Typography variant="subtitle2" gutterBottom>
              Advanced Features
            </Typography>
            <Box component="ul" sx={{ pl: 2 }}>
              <Typography component="li">Import/Export (CSV, Excel, JSON)</Typography>
              <Typography component="li">Curriculum alignment support</Typography>
              <Typography component="li">Bulk operations</Typography>
              <Typography component="li">Audit logging & permissions</Typography>
            </Box>
          </Grid>
        </Grid>
      </Paper>
    </Box>
  );
};
