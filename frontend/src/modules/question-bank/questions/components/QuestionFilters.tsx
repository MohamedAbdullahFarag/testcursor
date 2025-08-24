import React, { useState, useCallback, useMemo } from 'react';
import {
  Box,
  Paper,
  Typography,
  Chip,
  Button,
  Collapse,
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  Slider,
  FormControlLabel,
  Checkbox,
  Divider,
  IconButton,
  useTheme,
  Tooltip,
  Accordion,
  AccordionSummary,
  AccordionDetails,
} from '@mui/material';
import {
  FilterList as FilterIcon,
  Clear as ClearIcon,
  ExpandMore as ExpandMoreIcon,
  Save as SaveIcon,
  Refresh as RefreshIcon,
  Settings as SettingsIcon,
} from '@mui/icons-material';

import { QuestionFilters as QuestionFiltersType, QuestionType, QuestionDifficulty, QuestionStatus } from '../types';
import { FILTER_OPTIONS } from '../constants';

interface QuestionFiltersProps {
  filters: QuestionFiltersType;
  onFiltersChange: (filters: QuestionFiltersType) => void;
  onClear?: () => void;
  onSaveFilters?: (filters: QuestionFiltersType, name: string) => void;
  onLoadSavedFilters?: (filterId: string) => void;
  showAdvanced?: boolean;
  showPresets?: boolean;
  showSaved?: boolean;
  disabled?: boolean;
  loading?: boolean;
}

const QuestionFilters: React.FC<QuestionFiltersProps> = ({
  filters,
  onFiltersChange,
  onClear,
  onSaveFilters,
  onLoadSavedFilters,
  showAdvanced = true,
  showPresets = true,
  showSaved = false,
  disabled = false,
  loading = false,
}) => {
  const theme = useTheme();
  const [expandedSections, setExpandedSections] = useState<Set<string>>(new Set(['basic']));

  // Event handlers
  const handleFilterChange = useCallback((filterKey: keyof QuestionFiltersType, value: any) => {
    onFiltersChange({
      ...filters,
      [filterKey]: value,
    });
  }, [filters, onFiltersChange]);

  const handleClear = useCallback(() => {
    const clearedFilters: QuestionFiltersType = {
      questionTypes: [],
      difficulties: [],
      statuses: [],
      categories: [],
      tags: [],
      dateRange: null,
      pointsRange: null,
      timeRange: null,
      hasMedia: false,
      hasExplanation: false,
      hasValidation: false,
      isPublic: undefined,
      isActive: undefined,
    };
    onFiltersChange(clearedFilters);
    onClear?.();
  }, [onFiltersChange, onClear]);

  const handlePresetFilter = useCallback((preset: string) => {
    let newFilters: QuestionFiltersType = { ...filters };

    switch (preset) {
      case 'recent':
        newFilters = {
          ...newFilters,
          dateRange: { start: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000), end: new Date() },
        };
        break;
      case 'high-difficulty':
        newFilters = {
          ...newFilters,
          difficulties: ['Hard'],
        };
        break;
      case 'with-media':
        newFilters = {
          ...newFilters,
          hasMedia: true,
        };
        break;
      case 'with-explanation':
        newFilters = {
          ...newFilters,
          hasExplanation: true,
        };
        break;
      case 'public-only':
        newFilters = {
          ...newFilters,
          isPublic: true,
        };
        break;
      case 'active-only':
        newFilters = {
          ...newFilters,
          isActive: true,
        };
        break;
      case 'multiple-choice':
        newFilters = {
          ...newFilters,
          questionTypes: ['MultipleChoice'],
        };
        break;
      case 'essay':
        newFilters = {
          ...newFilters,
          questionTypes: ['Essay'],
        };
        break;
    }

    onFiltersChange(newFilters);
  }, [filters, onFiltersChange]);

  const handleSaveFilters = useCallback(() => {
    if (!onSaveFilters) return;

    const filterName = prompt('Enter a name for these filters:');
    if (filterName) {
      onSaveFilters(filters, filterName);
    }
  }, [filters, onSaveFilters]);

  const toggleSection = useCallback((section: string) => {
    setExpandedSections(prev => {
      const newSet = new Set(prev);
      if (newSet.has(section)) {
        newSet.delete(section);
      } else {
        newSet.add(section);
      }
      return newSet;
    });
  }, []);

  // Render functions
  const renderBasicFilters = () => (
    <Accordion
      expanded={expandedSections.has('basic')}
      onChange={() => toggleSection('basic')}
      sx={{ mb: 2 }}
    >
      <AccordionSummary expandIcon={<ExpandMoreIcon />}>
        <Typography variant="h6" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <FilterIcon />
          Basic Filters
        </Typography>
      </AccordionSummary>
      <AccordionDetails>
        <Grid container spacing={3}>
          {/* Question Types */}
          <Grid item xs={12} md={6}>
            <FormControl fullWidth size="small">
              <InputLabel>Question Types</InputLabel>
              <Select
                multiple
                value={filters.questionTypes}
                onChange={(e) => handleFilterChange('questionTypes', e.target.value)}
                renderValue={(selected) => (
                  <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                    {selected.map((value) => (
                      <Chip key={value} label={value} size="small" />
                    ))}
                  </Box>
                )}
                disabled={disabled}
              >
                {Object.values(QuestionType).map((type) => (
                  <MenuItem key={type} value={type}>
                    {type}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          {/* Difficulties */}
          <Grid item xs={12} md={6}>
            <FormControl fullWidth size="small">
              <InputLabel>Difficulty Levels</InputLabel>
              <Select
                multiple
                value={filters.difficulties}
                onChange={(e) => handleFilterChange('difficulties', e.target.value)}
                renderValue={(selected) => (
                  <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                    {selected.map((value) => (
                      <Chip key={value} label={value} size="small" />
                    ))}
                  </Box>
                )}
                disabled={disabled}
              >
                {Object.values(QuestionDifficulty).map((difficulty) => (
                  <MenuItem key={difficulty} value={difficulty}>
                    {difficulty}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          {/* Statuses */}
          <Grid item xs={12} md={6}>
            <FormControl fullWidth size="small">
              <InputLabel>Status</InputLabel>
              <Select
                multiple
                value={filters.statuses}
                onChange={(e) => handleFilterChange('statuses', e.target.value)}
                renderValue={(selected) => (
                  <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                    {selected.map((value) => (
                      <Chip key={value} label={value} size="small" />
                    ))}
                  </Box>
                )}
                disabled={disabled}
              >
                {Object.values(QuestionStatus).map((status) => (
                  <MenuItem key={status} value={status}>
                    {status}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          {/* Categories */}
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              size="small"
              label="Categories"
              placeholder="Enter categories (comma-separated)"
              value={filters.categories.join(', ')}
              onChange={(e) => handleFilterChange('categories', e.target.value.split(',').map(s => s.trim()).filter(Boolean))}
              helperText="Separate multiple categories with commas"
              disabled={disabled}
            />
          </Grid>

          {/* Tags */}
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              size="small"
              label="Tags"
              placeholder="Enter tags (comma-separated)"
              value={filters.tags.join(', ')}
              onChange={(e) => handleFilterChange('tags', e.target.value.split(',').map(s => s.trim()).filter(Boolean))}
              helperText="Separate multiple tags with commas"
              disabled={disabled}
            />
          </Grid>
        </Grid>
      </AccordionDetails>
    </Accordion>
  );

  const renderAdvancedFilters = () => {
    if (!showAdvanced) return null;

    return (
      <Accordion
        expanded={expandedSections.has('advanced')}
        onChange={() => toggleSection('advanced')}
        sx={{ mb: 2 }}
      >
        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
          <Typography variant="h6" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <SettingsIcon />
            Advanced Filters
          </Typography>
        </AccordionSummary>
        <AccordionDetails>
          <Grid container spacing={3}>
            {/* Points Range */}
            <Grid item xs={12} md={6}>
              <Typography gutterBottom>Points Range</Typography>
              <Slider
                value={filters.pointsRange || [0, 100]}
                onChange={(_, value) => handleFilterChange('pointsRange', value)}
                valueLabelDisplay="auto"
                min={0}
                max={100}
                step={1}
                marks={[
                  { value: 0, label: '0' },
                  { value: 25, label: '25' },
                  { value: 50, label: '50' },
                  { value: 75, label: '75' },
                  { value: 100, label: '100' },
                ]}
                disabled={disabled}
              />
            </Grid>

            {/* Time Range */}
            <Grid item xs={12} md={6}>
              <Typography gutterBottom>Time Limit Range (seconds)</Typography>
              <Slider
                value={filters.timeRange || [0, 300]}
                onChange={(_, value) => handleFilterChange('timeRange', value)}
                valueLabelDisplay="auto"
                min={0}
                max={300}
                step={15}
                marks={[
                  { value: 0, label: '0s' },
                  { value: 60, label: '1m' },
                  { value: 120, label: '2m' },
                  { value: 180, label: '3m' },
                  { value: 300, label: '5m' },
                ]}
                disabled={disabled}
              />
            </Grid>

            {/* Date Range */}
            <Grid item xs={12} md={6}>
              <Typography gutterBottom>Date Range</Typography>
              <Box sx={{ display: 'flex', gap: 2 }}>
                <TextField
                  type="date"
                  label="From"
                  size="small"
                  value={filters.dateRange?.start?.toISOString().split('T')[0] || ''}
                  onChange={(e) => {
                    const start = e.target.value ? new Date(e.target.value) : null;
                    const end = filters.dateRange?.end || null;
                    handleFilterChange('dateRange', start && end ? { start, end } : null);
                  }}
                  disabled={disabled}
                  InputLabelProps={{ shrink: true }}
                />
                <TextField
                  type="date"
                  label="To"
                  size="small"
                  value={filters.dateRange?.end?.toISOString().split('T')[0] || ''}
                  onChange={(e) => {
                    const end = e.target.value ? new Date(e.target.value) : null;
                    const start = filters.dateRange?.start || null;
                    handleFilterChange('dateRange', start && end ? { start, end } : null);
                  }}
                  disabled={disabled}
                  InputLabelProps={{ shrink: true }}
                />
              </Box>
            </Grid>

            {/* Boolean Filters */}
            <Grid item xs={12}>
              <Typography gutterBottom>Additional Filters</Typography>
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={filters.hasMedia}
                      onChange={(e) => handleFilterChange('hasMedia', e.target.checked)}
                      disabled={disabled}
                    />
                  }
                  label="Has Media"
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={filters.hasExplanation}
                      onChange={(e) => handleFilterChange('hasExplanation', e.target.checked)}
                      disabled={disabled}
                    />
                  }
                  label="Has Explanation"
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={filters.hasValidation}
                      onChange={(e) => handleFilterChange('hasValidation', e.target.checked)}
                      disabled={disabled}
                    />
                  }
                  label="Has Validation"
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={filters.isPublic === true}
                      onChange={(e) => handleFilterChange('isPublic', e.target.checked ? true : undefined)}
                      disabled={disabled}
                    />
                  }
                  label="Public Only"
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={filters.isActive === true}
                      onChange={(e) => handleFilterChange('isActive', e.target.checked ? true : undefined)}
                      disabled={disabled}
                    />
                  }
                  label="Active Only"
                />
              </Box>
            </Grid>
          </Grid>
        </AccordionDetails>
      </Accordion>
    );
  };

  const renderPresetFilters = () => {
    if (!showPresets) return null;

    return (
      <Accordion
        expanded={expandedSections.has('presets')}
        onChange={() => toggleSection('presets')}
        sx={{ mb: 2 }}
      >
        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
          <Typography variant="h6">Quick Filters</Typography>
        </AccordionSummary>
        <AccordionDetails>
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
            <Chip
              label="Recent (30 days)"
              size="small"
              variant="outlined"
              onClick={() => handlePresetFilter('recent')}
              sx={{ cursor: 'pointer' }}
              disabled={disabled}
            />
            <Chip
              label="High Difficulty"
              size="small"
              variant="outlined"
              onClick={() => handlePresetFilter('high-difficulty')}
              sx={{ cursor: 'pointer' }}
              disabled={disabled}
            />
            <Chip
              label="With Media"
              size="small"
              variant="outlined"
              onClick={() => handlePresetFilter('with-media')}
              sx={{ cursor: 'pointer' }}
              disabled={disabled}
            />
            <Chip
              label="With Explanation"
              size="small"
              variant="outlined"
              onClick={() => handlePresetFilter('with-explanation')}
              sx={{ cursor: 'pointer' }}
              disabled={disabled}
            />
            <Chip
              label="Public Only"
              size="small"
              variant="outlined"
              onClick={() => handlePresetFilter('public-only')}
              sx={{ cursor: 'pointer' }}
              disabled={disabled}
            />
            <Chip
              label="Active Only"
              size="small"
              variant="outlined"
              onClick={() => handlePresetFilter('active-only')}
              sx={{ cursor: 'pointer' }}
              disabled={disabled}
            />
            <Chip
              label="Multiple Choice"
              size="small"
              variant="outlined"
              onClick={() => handlePresetFilter('multiple-choice')}
              sx={{ cursor: 'pointer' }}
              disabled={disabled}
            />
            <Chip
              label="Essay Questions"
              size="small"
              variant="outlined"
              onClick={() => handlePresetFilter('essay')}
              sx={{ cursor: 'pointer' }}
              disabled={disabled}
            />
          </Box>
        </AccordionDetails>
      </Accordion>
    );
  };

  const renderSavedFilters = () => {
    if (!showSaved) return null;

    return (
      <Accordion
        expanded={expandedSections.has('saved')}
        onChange={() => toggleSection('saved')}
        sx={{ mb: 2 }}
      >
        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
          <Typography variant="h6">Saved Filter Sets</Typography>
        </AccordionSummary>
        <AccordionDetails>
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
            <Chip
              label="Math Filters"
              size="small"
              variant="outlined"
              onClick={() => onLoadSavedFilters?.('math')}
              sx={{ cursor: 'pointer' }}
              disabled={disabled}
            />
            <Chip
              label="Science Filters"
              size="small"
              variant="outlined"
              onClick={() => onLoadSavedFilters?.('science')}
              sx={{ cursor: 'pointer' }}
              disabled={disabled}
            />
            <Chip
              label="History Filters"
              size="small"
              variant="outlined"
              onClick={() => onLoadSavedFilters?.('history')}
              sx={{ cursor: 'pointer' }}
              disabled={disabled}
            />
          </Box>
        </AccordionDetails>
      </Accordion>
    );
  };

  const renderActiveFilters = () => {
    const activeFilters = [];

    if (filters.questionTypes.length > 0) {
      activeFilters.push(`Types: ${filters.questionTypes.join(', ')}`);
    }
    if (filters.difficulties.length > 0) {
      activeFilters.push(`Difficulty: ${filters.difficulties.join(', ')}`);
    }
    if (filters.statuses.length > 0) {
      activeFilters.push(`Status: ${filters.statuses.join(', ')}`);
    }
    if (filters.categories.length > 0) {
      activeFilters.push(`Categories: ${filters.categories.join(', ')}`);
    }
    if (filters.tags.length > 0) {
      activeFilters.push(`Tags: ${filters.tags.join(', ')}`);
    }
    if (filters.hasMedia) activeFilters.push('Has Media');
    if (filters.hasExplanation) activeFilters.push('Has Explanation');
    if (filters.hasValidation) activeFilters.push('Has Validation');
    if (filters.isPublic === true) activeFilters.push('Public Only');
    if (filters.isActive === true) activeFilters.push('Active Only');

    if (activeFilters.length === 0) return null;

    return (
      <Box sx={{ mb: 2 }}>
        <Typography variant="subtitle2" sx={{ mb: 1 }}>
          Active Filters:
        </Typography>
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
          {activeFilters.map((filter, index) => (
            <Chip
              key={index}
              label={filter}
              size="small"
              onDelete={() => handleClear()}
              sx={{ bgcolor: theme.palette.primary.light, color: 'white' }}
            />
          ))}
        </Box>
      </Box>
    );
  };

  const renderActions = () => (
    <Box sx={{ display: 'flex', gap: 2, justifyContent: 'space-between', alignItems: 'center' }}>
      <Box sx={{ display: 'flex', gap: 1 }}>
        <Button
          variant="outlined"
          startIcon={<ClearIcon />}
          onClick={handleClear}
          disabled={disabled}
          size="small"
        >
          Clear All
        </Button>
        {onSaveFilters && (
          <Button
            variant="outlined"
            startIcon={<SaveIcon />}
            onClick={handleSaveFilters}
            disabled={disabled}
            size="small"
          >
            Save Filters
          </Button>
        )}
      </Box>
      <Button
        variant="outlined"
        startIcon={<RefreshIcon />}
        onClick={() => window.location.reload()}
        disabled={disabled}
        size="small"
      >
        Reset
      </Button>
    </Box>
  );

  return (
    <Paper sx={{ p: 3, bgcolor: 'background.default' }}>
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h5" sx={{ fontWeight: 600, display: 'flex', alignItems: 'center', gap: 1 }}>
          <FilterIcon />
          Question Filters
        </Typography>
        <Tooltip title="Filter Settings">
          <IconButton size="small">
            <SettingsIcon />
          </IconButton>
        </Tooltip>
      </Box>

      {renderActiveFilters()}

      {renderBasicFilters()}
      {renderAdvancedFilters()}
      {renderPresetFilters()}
      {renderSavedFilters()}

      <Divider sx={{ my: 3 }} />
      {renderActions()}
    </Paper>
  );
};

export default QuestionFilters;
