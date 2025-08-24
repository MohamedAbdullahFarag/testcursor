import React, { useState, useCallback, useMemo } from 'react';
import {
  Box,
  TextField,
  InputAdornment,
  IconButton,
  Chip,
  Typography,
  Button,
  Collapse,
  Paper,
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Slider,
  FormControlLabel,
  Checkbox,
  Divider,
  useTheme,
  Tooltip,
  Alert,
} from '@mui/material';
import {
  Search as SearchIcon,
  Clear as ClearIcon,
  FilterList as FilterIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
  Save as SaveIcon,
  History as HistoryIcon,
  TrendingUp as TrendingUpIcon,
} from '@mui/icons-material';

import { QuestionSearchRequest, QuestionFilters, QuestionType, QuestionDifficulty, QuestionStatus } from '../types';
import { QUESTION_API_ENDPOINTS, FILTER_OPTIONS } from '../constants';

interface QuestionSearchProps {
  onSearch: (searchRequest: QuestionSearchRequest) => void;
  onClear?: () => void;
  onSaveSearch?: (searchRequest: QuestionSearchRequest, name: string) => void;
  onLoadSavedSearch?: (searchId: string) => void;
  showAdvancedFilters?: boolean;
  showSavedSearches?: boolean;
  showSearchHistory?: boolean;
  showTrendingSearches?: boolean;
  initialFilters?: Partial<QuestionSearchRequest>;
  placeholder?: string;
  disabled?: boolean;
  loading?: boolean;
}

const QuestionSearch: React.FC<QuestionSearchProps> = ({
  onSearch,
  onClear,
  onSaveSearch,
  onLoadSavedSearch,
  showAdvancedFilters = true,
  showSavedSearches = false,
  showSearchHistory = false,
  showTrendingSearches = false,
  initialFilters = {},
  placeholder = "Search questions by text, tags, or content...",
  disabled = false,
  loading = false,
}) => {
  const theme = useTheme();

  // State
  const [searchQuery, setSearchQuery] = useState(initialFilters.searchTerm || '');
  const [showFilters, setShowFilters] = useState(false);
  const [filters, setFilters] = useState<QuestionFilters>({
    questionTypes: initialFilters.filters?.questionTypes || [],
    difficulties: initialFilters.filters?.difficulties || [],
    statuses: initialFilters.filters?.statuses || [],
    categories: initialFilters.filters?.categories || [],
    tags: initialFilters.filters?.tags || [],
    dateRange: initialFilters.filters?.dateRange || null,
    pointsRange: initialFilters.filters?.pointsRange || null,
    timeRange: initialFilters.filters?.timeRange || null,
    hasMedia: initialFilters.filters?.hasMedia || false,
    hasExplanation: initialFilters.filters?.hasExplanation || false,
    hasValidation: initialFilters.filters?.hasValidation || false,
    isPublic: initialFilters.filters?.isPublic || undefined,
    isActive: initialFilters.filters?.isActive || undefined,
  });

  // Event handlers
  const handleSearch = useCallback(() => {
    if (!searchQuery.trim() && Object.keys(filters).length === 0) {
      return;
    }

    const searchRequest: QuestionSearchRequest = {
      searchTerm: searchQuery.trim(),
      filters,
      sorting: {
        field: 'createdAt',
        direction: 'desc',
      },
      pagination: {
        page: 1,
        pageSize: 20,
      },
    };

    onSearch(searchRequest);
  }, [searchQuery, filters, onSearch]);

  const handleClear = useCallback(() => {
    setSearchQuery('');
    setFilters({
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
    });
    onClear?.();
  }, [onClear]);

  const handleKeyPress = useCallback((event: React.KeyboardEvent) => {
    if (event.key === 'Enter') {
      handleSearch();
    }
  }, [handleSearch]);

  const handleFilterChange = useCallback((filterKey: keyof QuestionFilters, value: any) => {
    setFilters(prev => ({
      ...prev,
      [filterKey]: value,
    }));
  }, []);

  const handleSaveSearch = useCallback(() => {
    if (!onSaveSearch) return;

    const searchName = prompt('Enter a name for this search:');
    if (searchName) {
      const searchRequest: QuestionSearchRequest = {
        searchTerm: searchQuery.trim(),
        filters,
        sorting: { field: 'createdAt', direction: 'desc' },
        pagination: { page: 1, pageSize: 20 },
      };
      onSaveSearch(searchRequest, searchName);
    }
  }, [searchQuery, filters, onSaveSearch]);

  // Render functions
  const renderSearchBar = () => (
    <Box sx={{ display: 'flex', gap: 2, alignItems: 'center', mb: 2 }}>
      <TextField
        fullWidth
        value={searchQuery}
        onChange={(e) => setSearchQuery(e.target.value)}
        onKeyPress={handleKeyPress}
        placeholder={placeholder}
        disabled={disabled}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <SearchIcon color="action" />
            </InputAdornment>
          ),
          endAdornment: searchQuery && (
            <InputAdornment position="end">
              <IconButton
                size="small"
                onClick={() => setSearchQuery('')}
                disabled={disabled}
              >
                <ClearIcon />
              </IconButton>
            </InputAdornment>
          ),
        }}
        sx={{
          '& .MuiOutlinedInput-root': {
            borderRadius: 2,
          },
        }}
      />

      <Button
        variant="contained"
        onClick={handleSearch}
        disabled={disabled || loading || (!searchQuery.trim() && Object.keys(filters).length === 0)}
        startIcon={<SearchIcon />}
        sx={{ minWidth: 120 }}
      >
        {loading ? 'Searching...' : 'Search'}
      </Button>

      {showAdvancedFilters && (
        <Tooltip title="Advanced Filters">
          <IconButton
            onClick={() => setShowFilters(!showFilters)}
            color={showFilters ? 'primary' : 'default'}
            disabled={disabled}
          >
            <FilterIcon />
          </IconButton>
        </Tooltip>
      )}

      {onSaveSearch && (
        <Tooltip title="Save Search">
          <IconButton
            onClick={handleSaveSearch}
            disabled={disabled || (!searchQuery.trim() && Object.keys(filters).length === 0)}
          >
            <SaveIcon />
          </IconButton>
        </Tooltip>
      )}
    </Box>
  );

  const renderAdvancedFilters = () => {
    if (!showAdvancedFilters) return null;

    return (
      <Collapse in={showFilters}>
        <Paper sx={{ p: 3, mb: 2, bgcolor: 'background.default' }}>
          <Typography variant="h6" sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
            <FilterIcon />
            Advanced Filters
          </Typography>

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
              />
            </Grid>

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
                  { value: 50, label: '50' },
                  { value: 100, label: '100' },
                ]}
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
                  { value: 300, label: '5m' },
                ]}
              />
            </Grid>

            {/* Boolean Filters */}
            <Grid item xs={12}>
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={filters.hasMedia}
                      onChange={(e) => handleFilterChange('hasMedia', e.target.checked)}
                    />
                  }
                  label="Has Media"
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={filters.hasExplanation}
                      onChange={(e) => handleFilterChange('hasExplanation', e.target.checked)}
                    />
                  }
                  label="Has Explanation"
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={filters.hasValidation}
                      onChange={(e) => handleFilterChange('hasValidation', e.target.checked)}
                    />
                  }
                  label="Has Validation"
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={filters.isPublic === true}
                      onChange={(e) => handleFilterChange('isPublic', e.target.checked ? true : undefined)}
                    />
                  }
                  label="Public Only"
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={filters.isActive === true}
                      onChange={(e) => handleFilterChange('isActive', e.target.checked ? true : undefined)}
                    />
                  }
                  label="Active Only"
                />
              </Box>
            </Grid>
          </Grid>

          <Box sx={{ display: 'flex', gap: 2, mt: 3, justifyContent: 'flex-end' }}>
            <Button
              variant="outlined"
              onClick={handleClear}
              disabled={disabled}
            >
              Clear All
            </Button>
            <Button
              variant="contained"
              onClick={handleSearch}
              disabled={disabled || loading}
            >
              Apply Filters
            </Button>
          </Box>
        </Paper>
      </Collapse>
    );
  };

  const renderSavedSearches = () => {
    if (!showSavedSearches) return null;

    return (
      <Box sx={{ mb: 2 }}>
        <Typography variant="subtitle2" sx={{ mb: 1, display: 'flex', alignItems: 'center', gap: 1 }}>
          <SaveIcon fontSize="small" />
          Saved Searches
        </Typography>
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
          <Chip
            label="Recent Questions"
            size="small"
            variant="outlined"
            onClick={() => onLoadSavedSearch?.('recent')}
            sx={{ cursor: 'pointer' }}
          />
          <Chip
            label="High Difficulty"
            size="small"
            variant="outlined"
            onClick={() => onLoadSavedSearch?.('high-difficulty')}
            sx={{ cursor: 'pointer' }}
          />
          <Chip
            label="With Media"
            size="small"
            variant="outlined"
            onClick={() => onLoadSavedSearch?.('with-media')}
            sx={{ cursor: 'pointer' }}
          />
        </Box>
      </Box>
    );
  };

  const renderSearchHistory = () => {
    if (!showSearchHistory) return null;

    return (
      <Box sx={{ mb: 2 }}>
        <Typography variant="subtitle2" sx={{ mb: 1, display: 'flex', alignItems: 'center', gap: 1 }}>
          <HistoryIcon fontSize="small" />
          Recent Searches
        </Typography>
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
          <Chip
            label="Math problems"
            size="small"
            variant="outlined"
            onClick={() => setSearchQuery('Math problems')}
            sx={{ cursor: 'pointer' }}
          />
          <Chip
            label="Science questions"
            size="small"
            variant="outlined"
            onClick={() => setSearchQuery('Science questions')}
            sx={{ cursor: 'pointer' }}
          />
          <Chip
            label="Easy level"
            size="small"
            variant="outlined"
            onClick={() => handleFilterChange('difficulties', ['Easy'])}
            sx={{ cursor: 'pointer' }}
          />
        </Box>
      </Box>
    );
  };

  const renderTrendingSearches = () => {
    if (!showTrendingSearches) return null;

    return (
      <Box sx={{ mb: 2 }}>
        <Typography variant="subtitle2" sx={{ mb: 1, display: 'flex', alignItems: 'center', gap: 1 }}>
          <TrendingUpIcon fontSize="small" />
          Trending Searches
        </Typography>
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
          <Chip
            label="Algebra"
            size="small"
            variant="outlined"
            onClick={() => setSearchQuery('Algebra')}
            sx={{ cursor: 'pointer' }}
          />
          <Chip
            label="Physics"
            size="small"
            variant="outlined"
            onClick={() => setSearchQuery('Physics')}
            sx={{ cursor: 'pointer' }}
          />
          <Chip
            label="History"
            size="small"
            variant="outlined"
            onClick={() => setSearchQuery('History')}
            sx={{ cursor: 'pointer' }}
          />
        </Box>
      </Box>
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
              onDelete={() => {
                // Remove specific filter logic would go here
                handleClear();
              }}
              sx={{ bgcolor: theme.palette.primary.light, color: 'white' }}
            />
          ))}
        </Box>
      </Box>
    );
  };

  return (
    <Box>
      {renderSearchBar()}
      {renderActiveFilters()}
      {renderAdvancedFilters()}
      {renderSavedSearches()}
      {renderSearchHistory()}
      {renderTrendingSearches()}
    </Box>
  );
};

export default QuestionSearch;
