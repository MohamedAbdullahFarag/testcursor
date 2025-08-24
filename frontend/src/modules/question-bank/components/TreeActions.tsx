import React, { useState, useCallback } from 'react';
import {
  Box,
  Button,
  IconButton,
  Tooltip,
  Menu,
  MenuItem,
  ListItemIcon,
  ListItemText,
  Divider,
  Chip,
  Badge,
  Typography,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControl,
  InputLabel,
  Select,
  Alert,
  CircularProgress
} from '@mui/material';
import {
  Add,
  Refresh,
  ExpandAll,
  CollapseAll,
  FilterList,
  Sort,
  Download,
  Upload,
  Settings,
  MoreVert,
  Folder,
  Assignment,
  TrendingUp,
  Save,
  Close,
  CloudDownload,
  CloudUpload,
  FileDownload,
  FileUpload,
  SortByAlpha,
  SortByAlphaOutlined,
  FilterAlt,
  FilterAltOutlined
} from '@mui/icons-material';
import { QuestionBankCategoryDto, CategoryType, CategoryLevel } from '../types/questionBankTree.types';

interface TreeActionsProps {
  selectedCategoryId?: number;
  onCategoryCreate?: (parentId: number | null) => void;
  onRefresh?: () => void;
  onExpandAll?: () => void;
  onCollapseAll?: () => void;
  onFilterChange?: (filters: any) => void;
  onSortChange?: (sortBy: string, sortOrder: 'asc' | 'desc') => void;
  onExport?: (format: string) => void;
  onImport?: (file: File) => void;
  onSettings?: () => void;
  className?: string;
  showAdvancedActions?: boolean;
  treeStatistics?: {
    totalCategories: number;
    totalQuestions: number;
    maxDepth: number;
    lastUpdated: Date;
  };
}

interface FilterState {
  categoryType?: CategoryType;
  level?: CategoryLevel;
  isActive?: boolean;
  allowQuestions?: boolean;
  hasQuestions?: boolean;
}

interface SortState {
  field: string;
  order: 'asc' | 'desc';
}

export const TreeActions: React.FC<TreeActionsProps> = ({
  selectedCategoryId,
  onCategoryCreate,
  onRefresh,
  onExpandAll,
  onCollapseAll,
  onFilterChange,
  onSortChange,
  onExport,
  onImport,
  onSettings,
  className = "",
  showAdvancedActions = false,
  treeStatistics
}) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [showFilters, setShowFilters] = useState(false);
  const [showSort, setShowSort] = useState(false);
  const [showExport, setShowExport] = useState(false);
  const [showImport, setShowImport] = useState(false);
  
  const [filters, setFilters] = useState<FilterState>({});
  const [sortState, setSortState] = useState<SortState>({ field: 'name', order: 'asc' });
  const [exportFormat, setExportFormat] = useState<string>('csv');
  const [importFile, setImportFile] = useState<File | null>(null);
  const [isProcessing, setIsProcessing] = useState(false);

  const handleMenuOpen = useCallback((event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  }, []);

  const handleMenuClose = useCallback(() => {
    setAnchorEl(null);
  }, []);

  const handleCreateCategory = useCallback(() => {
    if (onCategoryCreate) {
      onCategoryCreate(selectedCategoryId || null);
    }
    handleMenuClose();
  }, [onCategoryCreate, selectedCategoryId, handleMenuClose]);

  const handleRefresh = useCallback(() => {
    if (onRefresh) {
      onRefresh();
    }
  }, [onRefresh]);

  const handleExpandAll = useCallback(() => {
    if (onExpandAll) {
      onExpandAll();
    }
  }, [onExpandAll]);

  const handleCollapseAll = useCallback(() => {
    if (onCollapseAll) {
      onCollapseAll();
    }
  }, [onCollapseAll]);

  const handleFilterToggle = useCallback(() => {
    setShowFilters(!showFilters);
  }, [showFilters]);

  const handleSortToggle = useCallback(() => {
    setShowSort(!showSort);
  }, [showSort]);

  const handleFilterChange = useCallback((key: keyof FilterState, value: any) => {
    const newFilters = { ...filters, [key]: value };
    setFilters(newFilters);
    if (onFilterChange) {
      onFilterChange(newFilters);
    }
  }, [filters, onFilterChange]);

  const handleSortChange = useCallback((field: string, order: 'asc' | 'desc') => {
    const newSortState = { field, order };
    setSortState(newSortState);
    if (onSortChange) {
      onSortChange(field, order);
    }
    setShowSort(false);
  }, [onSortChange]);

  const handleExport = useCallback(async () => {
    if (!onExport) return;
    
    setIsProcessing(true);
    try {
      await onExport(exportFormat);
      setShowExport(false);
    } catch (error) {
      console.error('Export failed:', error);
    } finally {
      setIsProcessing(false);
    }
  }, [onExport, exportFormat]);

  const handleImport = useCallback(async () => {
    if (!onImport || !importFile) return;
    
    setIsProcessing(true);
    try {
      await onImport(importFile);
      setShowImport(false);
      setImportFile(null);
    } catch (error) {
      console.error('Import failed:', error);
    } finally {
      setIsProcessing(false);
    }
  }, [onImport, importFile]);

  const handleFileSelect = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setImportFile(file);
    }
  }, []);

  const getCategoryTypeLabel = (type: CategoryType) => {
    switch (type) {
      case 'Subject': return 'Subject';
      case 'Chapter': return 'Chapter';
      case 'Topic': return 'Topic';
      case 'Subtopic': return 'Subtopic';
      case 'Skill': return 'Skill';
      case 'Objective': return 'Objective';
      default: return 'All';
    }
  };

  const getLevelLabel = (level: CategoryLevel) => {
    return `Level ${level}`;
  };

  return (
    <Box className={`tree-actions ${className}`}>
      {/* Primary Actions */}
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={handleCreateCategory}
          size="small"
        >
          {selectedCategoryId ? 'Add Subcategory' : 'Add Category'}
        </Button>

        <Button
          variant="outlined"
          startIcon={<Refresh />}
          onClick={handleRefresh}
          size="small"
        >
          Refresh
        </Button>

        <Button
          variant="outlined"
          startIcon={<ExpandAll />}
          onClick={handleExpandAll}
          size="small"
        >
          Expand All
        </Button>

        <Button
          variant="outlined"
          startIcon={<CollapseAll />}
          onClick={handleCollapseAll}
          size="small"
        >
          Collapse All
        </Button>

        {/* Advanced Actions Menu */}
        {showAdvancedActions && (
          <>
            <IconButton
              size="small"
              onClick={handleFilterToggle}
              color={showFilters ? 'primary' : 'default'}
            >
              {showFilters ? <FilterAlt /> : <FilterAltOutlined />}
            </IconButton>

            <IconButton
              size="small"
              onClick={handleSortToggle}
              color={showSort ? 'primary' : 'default'}
            >
              {sortState.order === 'asc' ? <SortByAlpha /> : <SortByAlphaOutlined />}
            </IconButton>

            <IconButton
              size="small"
              onClick={handleMenuOpen}
            >
              <MoreVert />
            </IconButton>

            <Menu
              anchorEl={anchorEl}
              open={Boolean(anchorEl)}
              onClose={handleMenuClose}
              anchorOrigin={{
                vertical: 'bottom',
                horizontal: 'right',
              }}
              transformOrigin={{
                vertical: 'top',
                horizontal: 'right',
              }}
            >
              <MenuItem onClick={() => setShowExport(true)}>
                <ListItemIcon>
                  <Download fontSize="small" />
                </ListItemIcon>
                <ListItemText>Export Tree</ListItemText>
              </MenuItem>

              <MenuItem onClick={() => setShowImport(true)}>
                <ListItemIcon>
                  <Upload fontSize="small" />
                </ListItemIcon>
                <ListItemText>Import Tree</ListItemText>
              </MenuItem>

              <Divider />

              <MenuItem onClick={onSettings}>
                <ListItemIcon>
                  <Settings fontSize="small" />
                </ListItemIcon>
                <ListItemText>Tree Settings</ListItemText>
              </MenuItem>
            </Menu>
          </>
        )}
      </Box>

      {/* Tree Statistics */}
      {treeStatistics && (
        <Box sx={{ 
          display: 'flex', 
          alignItems: 'center', 
          gap: 2, 
          p: 1, 
          backgroundColor: 'action.hover', 
          borderRadius: 1,
          mb: 2
        }}>
          <Chip
            label={`${treeStatistics.totalCategories} Categories`}
            size="small"
            variant="outlined"
            color="primary"
          />
          
          <Chip
            label={`${treeStatistics.totalQuestions} Questions`}
            size="small"
            variant="outlined"
            color="success"
          />
          
          <Chip
            label={`Max Depth: ${treeStatistics.maxDepth}`}
            size="small"
            variant="outlined"
            color="info"
          />
          
          <Typography variant="caption" color="text.secondary">
            Last updated: {treeStatistics.lastUpdated.toLocaleDateString()}
          </Typography>
        </Box>
      )}

      {/* Filters Panel */}
      {showFilters && (
        <Box sx={{ 
          p: 2, 
          border: 1, 
          borderColor: 'divider', 
          borderRadius: 1,
          mb: 2,
          backgroundColor: 'background.paper'
        }}>
          <Typography variant="subtitle2" gutterBottom sx={{ display: 'flex', alignItems: 'center' }}>
            <FilterList sx={{ mr: 1, fontSize: '1rem' }} />
            Filters
          </Typography>
          
          <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
            <FormControl size="small" sx={{ minWidth: 120 }}>
              <InputLabel>Category Type</InputLabel>
              <Select
                value={filters.categoryType || ''}
                onChange={(e) => handleFilterChange('categoryType', e.target.value)}
                label="Category Type"
              >
                <MenuItem value="">All Types</MenuItem>
                <MenuItem value="Subject">Subject</MenuItem>
                <MenuItem value="Chapter">Chapter</MenuItem>
                <MenuItem value="Topic">Topic</MenuItem>
                <MenuItem value="Subtopic">Subtopic</MenuItem>
                <MenuItem value="Skill">Skill</MenuItem>
                <MenuItem value="Objective">Objective</MenuItem>
              </Select>
            </FormControl>

            <FormControl size="small" sx={{ minWidth: 120 }}>
              <InputLabel>Level</InputLabel>
              <Select
                value={filters.level || ''}
                onChange={(e) => handleFilterChange('level', e.target.value)}
                label="Level"
              >
                <MenuItem value="">All Levels</MenuItem>
                <MenuItem value={1}>Level 1</MenuItem>
                <MenuItem value={2}>Level 2</MenuItem>
                <MenuItem value={3}>Level 3</MenuItem>
                <MenuItem value={4}>Level 4</MenuItem>
                <MenuItem value={5}>Level 5</MenuItem>
                <MenuItem value={6}>Level 6</MenuItem>
              </Select>
            </FormControl>

            <FormControl size="small" sx={{ minWidth: 120 }}>
              <InputLabel>Status</InputLabel>
              <Select
                value={filters.isActive !== undefined ? filters.isActive.toString() : ''}
                onChange={(e) => handleFilterChange('isActive', e.target.value === 'true')}
                label="Status"
              >
                <MenuItem value="">All</MenuItem>
                <MenuItem value="true">Active</MenuItem>
                <MenuItem value="false">Inactive</MenuItem>
              </Select>
            </FormControl>

            <FormControl size="small" sx={{ minWidth: 120 }}>
              <InputLabel>Questions</InputLabel>
              <Select
                value={filters.hasQuestions !== undefined ? filters.hasQuestions.toString() : ''}
                onChange={(e) => handleFilterChange('hasQuestions', e.target.value === 'true')}
                label="Questions"
              >
                <MenuItem value="">All</MenuItem>
                <MenuItem value="true">With Questions</MenuItem>
                <MenuItem value="false">Without Questions</MenuItem>
              </Select>
            </FormControl>
          </Box>
        </Box>
      )}

      {/* Sort Panel */}
      {showSort && (
        <Box sx={{ 
          p: 2, 
          border: 1, 
          borderColor: 'divider', 
          borderRadius: 1,
          mb: 2,
          backgroundColor: 'background.paper'
        }}>
          <Typography variant="subtitle2" gutterBottom sx={{ display: 'flex', alignItems: 'center' }}>
            <Sort sx={{ mr: 1, fontSize: '1rem' }} />
            Sort Options
          </Typography>
          
          <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
            <Button
              variant={sortState.field === 'name' ? 'contained' : 'outlined'}
              size="small"
              onClick={() => handleSortChange('name', sortState.field === 'name' && sortState.order === 'asc' ? 'desc' : 'asc')}
            >
              Name {sortState.field === 'name' && (sortState.order === 'asc' ? '↑' : '↓')}
            </Button>

            <Button
              variant={sortState.field === 'type' ? 'contained' : 'outlined'}
              size="small"
              onClick={() => handleSortChange('type', sortState.field === 'type' && sortState.order === 'asc' ? 'desc' : 'asc')}
            >
              Type {sortState.field === 'type' && (sortState.order === 'asc' ? '↑' : '↓')}
            </Button>

            <Button
              variant={sortState.field === 'level' ? 'contained' : 'outlined'}
              size="small"
              onClick={() => handleSortChange('level', sortState.field === 'level' && sortState.order === 'asc' ? 'desc' : 'asc')}
            >
              Level {sortState.field === 'level' && (sortState.order === 'asc' ? '↑' : '↓')}
            </Button>

            <Button
              variant={sortState.field === 'questionCount' ? 'contained' : 'outlined'}
              size="small"
              onClick={() => handleSortChange('questionCount', sortState.field === 'questionCount' && sortState.order === 'asc' ? 'desc' : 'asc')}
            >
              Questions {sortState.field === 'questionCount' && (sortState.order === 'asc' ? '↑' : '↓')}
            </Button>
          </Box>
        </Box>
      )}

      {/* Export Dialog */}
      <Dialog open={showExport} onClose={() => setShowExport(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Export Question Bank Tree</DialogTitle>
        <DialogContent>
          <FormControl fullWidth sx={{ mt: 2 }}>
            <InputLabel>Export Format</InputLabel>
            <Select
              value={exportFormat}
              onChange={(e) => setExportFormat(e.target.value)}
              label="Export Format"
            >
              <MenuItem value="csv">CSV</MenuItem>
              <MenuItem value="excel">Excel</MenuItem>
              <MenuItem value="json">JSON</MenuItem>
              <MenuItem value="xml">XML</MenuItem>
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setShowExport(false)}>Cancel</Button>
          <Button 
            onClick={handleExport} 
            variant="contained" 
            disabled={isProcessing}
            startIcon={isProcessing ? <CircularProgress size={16} /> : <Download />}
          >
            {isProcessing ? 'Exporting...' : 'Export'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Import Dialog */}
      <Dialog open={showImport} onClose={() => setShowImport(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Import Question Bank Tree</DialogTitle>
        <DialogContent>
          <Alert severity="info" sx={{ mb: 2 }}>
            Supported formats: CSV, Excel, JSON. The file should contain category information with proper hierarchy.
          </Alert>
          
          <input
            accept=".csv,.xlsx,.xls,.json"
            style={{ display: 'none' }}
            id="import-file-input"
            type="file"
            onChange={handleFileSelect}
          />
          
          <label htmlFor="import-file-input">
            <Button
              variant="outlined"
              component="span"
              startIcon={<Upload />}
              fullWidth
              sx={{ mb: 2 }}
            >
              Choose File
            </Button>
          </label>
          
          {importFile && (
            <Box sx={{ p: 2, border: 1, borderColor: 'divider', borderRadius: 1 }}>
              <Typography variant="body2">
                Selected file: {importFile.name}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                Size: {(importFile.size / 1024).toFixed(2)} KB
              </Typography>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setShowImport(false)}>Cancel</Button>
          <Button 
            onClick={handleImport} 
            variant="contained" 
            disabled={!importFile || isProcessing}
            startIcon={isProcessing ? <CircularProgress size={16} /> : <Upload />}
          >
            {isProcessing ? 'Importing...' : 'Import'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};
