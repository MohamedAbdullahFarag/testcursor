import React, { useState, useCallback, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  FormControlLabel,
  Switch,
  Chip,
  Button,
  IconButton,
  Collapse,
  Divider,
  Slider,
  Grid,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Tooltip,
  Alert
} from '@mui/material';
import {
  FilterList,
  Clear,
  ExpandMore,
  Search,
  Refresh,
  Save,
  Settings,
  FilterAlt,
  FilterAltOutlined
} from '@mui/icons-material';
import { CategoryFilterDto, CategoryType, CategoryLevel } from '../types/questionBankTree.types';

interface CategoryFiltersProps {
  filters: CategoryFilterDto;
  onFiltersChange: (filters: CategoryFilterDto) => void;
  onClearFilters?: () => void;
  onSaveFilters?: (name: string) => void;
  onLoadFilters?: (name: string) => void;
  savedFilterNames?: string[];
  className?: string;
  showAdvanced?: boolean;
  maxDepth?: number;
}

const CATEGORY_TYPES: { value: CategoryType; label: string; description: string }[] = [
  { value: 'Subject', label: 'Subject', description: 'Main academic subject area' },
  { value: 'Chapter', label: 'Chapter', description: 'Major section within a subject' },
  { value: 'Topic', label: 'Topic', description: 'Specific topic or concept' },
  { value: 'Subtopic', label: 'Subtopic', description: 'Detailed subtopic' },
  { value: 'Skill', label: 'Skill', description: 'Specific skill or competency' },
  { value: 'Objective', label: 'Objective', description: 'Learning objective or outcome' }
];

const CATEGORY_LEVELS: { value: CategoryLevel; label: string; description: string }[] = [
  { value: 1, label: 'Level 1', description: 'Root level category' },
  { value: 2, label: 'Level 2', description: 'Second level category' },
  { value: 3, label: 'Level 3', description: 'Third level category' },
  { value: 4, label: 'Level 4', description: 'Fourth level category' },
  { value: 5, label: 'Level 5', description: 'Fifth level category' },
  { value: 6, label: 'Level 6', description: 'Sixth level category' }
];

export const CategoryFilters: React.FC<CategoryFiltersProps> = ({
  filters,
  onFiltersChange,
  onClearFilters,
  onSaveFilters,
  onLoadFilters,
  savedFilterNames = [],
  className = "",
  showAdvanced = false,
  maxDepth = 6
}) => {
  const [localFilters, setLocalFilters] = useState<CategoryFilterDto>(filters);
  const [showAdvancedFilters, setShowAdvancedFilters] = useState(showAdvanced);
  const [showSavedFilters, setShowSavedFilters] = useState(false);
  const [filterName, setFilterName] = useState('');
  const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);

  // Update local filters when props change
  useEffect(() => {
    setLocalFilters(filters);
    setHasUnsavedChanges(false);
  }, [filters]);

  // Check for unsaved changes
  useEffect(() => {
    const hasChanges = JSON.stringify(localFilters) !== JSON.stringify(filters);
    setHasUnsavedChanges(hasChanges);
  }, [localFilters, filters]);

  const handleFilterChange = useCallback((key: keyof CategoryFilterDto, value: any) => {
    const newFilters = { ...localFilters, [key]: value };
    setLocalFilters(newFilters);
  }, [localFilters]);

  const handleApplyFilters = useCallback(() => {
    onFiltersChange(localFilters);
    setHasUnsavedChanges(false);
  }, [localFilters, onFiltersChange]);

  const handleClearFilters = useCallback(() => {
    const clearedFilters: CategoryFilterDto = {
      query: '',
      categoryType: undefined,
      level: undefined,
      isActive: undefined,
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
    };
    
    setLocalFilters(clearedFilters);
    if (onClearFilters) {
      onClearFilters();
    }
    setHasUnsavedChanges(false);
  }, [onClearFilters]);

  const handleSaveFilters = useCallback(() => {
    if (filterName.trim() && onSaveFilters) {
      onSaveFilters(filterName.trim());
      setFilterName('');
      setShowSavedFilters(false);
    }
  }, [filterName, onSaveFilters]);

  const handleLoadFilters = useCallback((name: string) => {
    if (onLoadFilters) {
      onLoadFilters(name);
      setShowSavedFilters(false);
    }
  }, [onLoadFilters]);

  const handleResetToDefaults = useCallback(() => {
    const defaultFilters: CategoryFilterDto = {
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
    };
    
    setLocalFilters(defaultFilters);
    setHasUnsavedChanges(false);
  }, []);

  const getActiveFiltersCount = () => {
    let count = 0;
    if (localFilters.query) count++;
    if (localFilters.categoryType) count++;
    if (localFilters.level) count++;
    if (localFilters.isActive !== undefined) count++;
    if (localFilters.allowQuestions !== undefined) count++;
    if (localFilters.hasQuestions !== undefined) count++;
    if (localFilters.minQuestionCount !== undefined) count++;
    if (localFilters.maxQuestionCount !== undefined) count++;
    if (localFilters.createdAfter) count++;
    if (localFilters.createdBefore) count++;
    if (localFilters.updatedAfter) count++;
    if (localFilters.updatedBefore) count++;
    if (localFilters.curriculumCode) count++;
    if (localFilters.gradeLevel) count++;
    if (localFilters.subject) count++;
    return count;
  };

  const activeFiltersCount = getActiveFiltersCount();

  return (
    <Paper className={`category-filters ${className}`} sx={{ p: 2, mb: 2 }}>
      {/* Header */}
      <Box sx={{ 
        display: 'flex', 
        alignItems: 'center', 
        justifyContent: 'space-between',
        mb: 2
      }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <FilterList color="primary" />
          <Typography variant="h6">Category Filters</Typography>
          {activeFiltersCount > 0 && (
            <Chip
              label={activeFiltersCount}
              size="small"
              color="primary"
              variant="filled"
            />
          )}
        </Box>

        <Box sx={{ display: 'flex', gap: 1 }}>
          <Button
            size="small"
            onClick={() => setShowAdvancedFilters(!showAdvancedFilters)}
            startIcon={showAdvancedFilters ? <FilterAlt /> : <FilterAltOutlined />}
            variant="outlined"
          >
            {showAdvancedFilters ? 'Basic' : 'Advanced'}
          </Button>

          {savedFilterNames.length > 0 && (
            <Button
              size="small"
              onClick={() => setShowSavedFilters(!showSavedFilters)}
              variant="outlined"
            >
              Saved Filters
            </Button>
          )}

          <Button
            size="small"
            onClick={handleResetToDefaults}
            variant="outlined"
          >
            Reset
          </Button>
        </Box>
      </Box>

      {/* Basic Filters */}
      <Grid container spacing={2} sx={{ mb: 2 }}>
        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            size="small"
            label="Search Categories"
            value={localFilters.query || ''}
            onChange={(e) => handleFilterChange('query', e.target.value)}
            placeholder="Search by name, code, or description..."
            InputProps={{
              startAdornment: <Search fontSize="small" color="action" />
            }}
          />
        </Grid>

        <Grid item xs={12} md={3}>
          <FormControl fullWidth size="small">
            <InputLabel>Category Type</InputLabel>
            <Select
              value={localFilters.categoryType || ''}
              onChange={(e) => handleFilterChange('categoryType', e.target.value)}
              label="Category Type"
            >
              <MenuItem value="">All Types</MenuItem>
              {CATEGORY_TYPES.map((type) => (
                <MenuItem key={type.value} value={type.value}>
                  <Box>
                    <Typography variant="body2">{type.label}</Typography>
                    <Typography variant="caption" color="text.secondary">
                      {type.description}
                    </Typography>
                  </Box>
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>

        <Grid item xs={12} md={3}>
          <FormControl fullWidth size="small">
            <InputLabel>Level</InputLabel>
            <Select
              value={localFilters.level || ''}
              onChange={(e) => handleFilterChange('level', e.target.value)}
              label="Level"
            >
              <MenuItem value="">All Levels</MenuItem>
              {CATEGORY_LEVELS.map((level) => (
                <MenuItem key={level.value} value={level.value}>
                  <Box>
                    <Typography variant="body2">{level.label}</Typography>
                    <Typography variant="caption" color="text.secondary">
                      {level.description}
                    </Typography>
                  </Box>
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>
      </Grid>

      {/* Advanced Filters */}
      <Collapse in={showAdvancedFilters}>
        <Box sx={{ borderTop: 1, borderColor: 'divider', pt: 2, mb: 2 }}>
          <Typography variant="subtitle2" gutterBottom>Advanced Filters</Typography>
          
          <Grid container spacing={2}>
            {/* Status Filters */}
            <Grid item xs={12} md={4}>
              <FormControl fullWidth size="small">
                <InputLabel>Status</InputLabel>
                <Select
                  value={localFilters.isActive !== undefined ? localFilters.isActive.toString() : ''}
                  onChange={(e) => handleFilterChange('isActive', e.target.value === 'true')}
                  label="Status"
                >
                  <MenuItem value="">All</MenuItem>
                  <MenuItem value="true">Active</MenuItem>
                  <MenuItem value="false">Inactive</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} md={4}>
              <FormControl fullWidth size="small">
                <InputLabel>Allow Questions</InputLabel>
                <Select
                  value={localFilters.allowQuestions !== undefined ? localFilters.allowQuestions.toString() : ''}
                  onChange={(e) => handleFilterChange('allowQuestions', e.target.value === 'true')}
                  label="Allow Questions"
                >
                  <MenuItem value="">All</MenuItem>
                  <MenuItem value="true">Yes</MenuItem>
                  <MenuItem value="false">No</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} md={4}>
              <FormControl fullWidth size="small">
                <InputLabel>Has Questions</InputLabel>
                <Select
                  value={localFilters.hasQuestions !== undefined ? localFilters.hasQuestions.toString() : ''}
                  onChange={(e) => handleFilterChange('hasQuestions', e.target.value === 'true')}
                  label="Has Questions"
                >
                  <MenuItem value="">All</MenuItem>
                  <MenuItem value="true">With Questions</MenuItem>
                  <MenuItem value="false">Without Questions</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            {/* Question Count Range */}
            <Grid item xs={12} md={6}>
              <Typography variant="body2" gutterBottom>Question Count Range</Typography>
              <Box sx={{ px: 2 }}>
                <Slider
                  value={[
                    localFilters.minQuestionCount || 0,
                    localFilters.maxQuestionCount || 100
                  ]}
                  onChange={(_, newValue) => {
                    const [min, max] = newValue as number[];
                    handleFilterChange('minQuestionCount', min);
                    handleFilterChange('maxQuestionCount', max);
                  }}
                  valueLabelDisplay="auto"
                  min={0}
                  max={100}
                  step={1}
                />
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Typography variant="caption">
                    {localFilters.minQuestionCount || 0}
                  </Typography>
                  <Typography variant="caption">
                    {localFilters.maxQuestionCount || 100}
                  </Typography>
                </Box>
              </Box>
            </Grid>

            {/* Date Range */}
            <Grid item xs={12} md={6}>
              <Typography variant="body2" gutterBottom>Date Range</Typography>
              <Grid container spacing={1}>
                <Grid item xs={6}>
                  <TextField
                    fullWidth
                    size="small"
                    type="date"
                    label="Created After"
                    value={localFilters.createdAfter || ''}
                    onChange={(e) => handleFilterChange('createdAfter', e.target.value)}
                    InputLabelProps={{ shrink: true }}
                  />
                </Grid>
                <Grid item xs={6}>
                  <TextField
                    fullWidth
                    size="small"
                    type="date"
                    label="Created Before"
                    value={localFilters.createdBefore || ''}
                    onChange={(e) => handleFilterChange('createdBefore', e.target.value)}
                    InputLabelProps={{ shrink: true }}
                  />
                </Grid>
              </Grid>
            </Grid>

            {/* Curriculum Alignment */}
            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                size="small"
                label="Curriculum Code"
                value={localFilters.curriculumCode || ''}
                onChange={(e) => handleFilterChange('curriculumCode', e.target.value)}
                placeholder="e.g., CCSS, NGSS"
              />
            </Grid>

            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                size="small"
                label="Grade Level"
                value={localFilters.gradeLevel || ''}
                onChange={(e) => handleFilterChange('gradeLevel', e.target.value)}
                placeholder="e.g., K-5, 6-8, 9-12"
              />
            </Grid>

            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                size="small"
                label="Subject"
                value={localFilters.subject || ''}
                onChange={(e) => handleFilterChange('subject', e.target.value)}
                placeholder="e.g., Mathematics, Science"
              />
            </Grid>
          </Grid>
        </Box>
      </Collapse>

      {/* Saved Filters */}
      <Collapse in={showSavedFilters}>
        <Box sx={{ borderTop: 1, borderColor: 'divider', pt: 2, mb: 2 }}>
          <Typography variant="subtitle2" gutterBottom>Saved Filters</Typography>
          
          <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mb: 2 }}>
            {savedFilterNames.map((name) => (
              <Chip
                key={name}
                label={name}
                size="small"
                variant="outlined"
                onClick={() => handleLoadFilters(name)}
                sx={{ cursor: 'pointer' }}
              />
            ))}
          </Box>

          <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
            <TextField
              size="small"
              label="Filter Name"
              value={filterName}
              onChange={(e) => setFilterName(e.target.value)}
              placeholder="Enter filter name"
              sx={{ flex: 1 }}
            />
            <Button
              size="small"
              onClick={handleSaveFilters}
              disabled={!filterName.trim()}
              startIcon={<Save />}
            >
              Save
            </Button>
          </Box>
        </Box>
      </Collapse>

      {/* Action Buttons */}
      <Box sx={{ 
        display: 'flex', 
        alignItems: 'center', 
        justifyContent: 'space-between',
        pt: 2,
        borderTop: 1,
        borderColor: 'divider'
      }}>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Button
            variant="contained"
            onClick={handleApplyFilters}
            disabled={!hasUnsavedChanges}
            startIcon={<FilterList />}
          >
            Apply Filters
          </Button>

          <Button
            variant="outlined"
            onClick={handleClearFilters}
            startIcon={<Clear />}
          >
            Clear All
          </Button>
        </Box>

        {hasUnsavedChanges && (
          <Alert severity="info" sx={{ py: 0 }}>
            You have unsaved filter changes
          </Alert>
        )}
      </Box>
    </Paper>
  );
};
