import React, { useState, useCallback, useEffect } from 'react';
import { 
  TextField, 
  InputAdornment, 
  IconButton, 
  Box, 
  Chip,
  Collapse,
  Typography,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Divider
} from '@mui/material';
import { 
  Search, 
  Clear, 
  Folder, 
  Assignment, 
  TrendingUp,
  FilterList,
  Close
} from '@mui/icons-material';
import { QuestionBankCategoryDto, CategorySearchDto } from '../types/questionBankTree.types';

interface TreeSearchProps {
  onSearch: (query: string) => void;
  onClear?: () => void;
  placeholder?: string;
  className?: string;
  searchResults?: QuestionBankCategoryDto[];
  isLoading?: boolean;
  showAdvancedFilters?: boolean;
  onAdvancedFilterChange?: (filters: CategorySearchDto) => void;
}

export const TreeSearch: React.FC<TreeSearchProps> = ({
  onSearch,
  onClear,
  placeholder = "Search categories and topics...",
  className = "",
  searchResults = [],
  isLoading = false,
  showAdvancedFilters = false,
  onAdvancedFilterChange
}) => {
  const [searchQuery, setSearchQuery] = useState('');
  const [debouncedQuery, setDebouncedQuery] = useState('');
  const [showResults, setShowResults] = useState(false);
  const [advancedFilters, setAdvancedFilters] = useState<CategorySearchDto>({
    query: '',
    categoryType: undefined,
    level: undefined,
    includeQuestions: true,
    includeInactive: false,
    maxDepth: 6
  });

  // Debounce search query
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedQuery(searchQuery);
    }, 300);

    return () => clearTimeout(timer);
  }, [searchQuery]);

  // Trigger search when debounced query changes
  useEffect(() => {
    if (debouncedQuery.trim()) {
      onSearch(debouncedQuery);
      setShowResults(true);
    } else {
      setShowResults(false);
      if (onClear) onClear();
    }
  }, [debouncedQuery, onSearch, onClear]);

  const handleSearchChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchQuery(event.target.value);
  }, []);

  const handleClearSearch = useCallback(() => {
    setSearchQuery('');
    setShowResults(false);
    if (onClear) onClear();
  }, [onClear]);

  const handleAdvancedFilterChange = useCallback((key: keyof CategorySearchDto, value: any) => {
    const newFilters = { ...advancedFilters, [key]: value };
    setAdvancedFilters(newFilters);
    if (onAdvancedFilterChange) {
      onAdvancedFilterChange(newFilters);
    }
  }, [advancedFilters, onAdvancedFilterChange]);

  const handleResultClick = useCallback((category: QuestionBankCategoryDto) => {
    // This will be handled by the parent component
    setShowResults(false);
  }, []);

  const getCategoryIcon = (type: string) => {
    switch (type) {
      case 'Subject': return <Folder color="primary" />;
      case 'Chapter': return <Folder color="secondary" />;
      case 'Topic': return <Assignment color="success" />;
      case 'Subtopic': return <Assignment color="info" />;
      case 'Skill': return <TrendingUp color="warning" />;
      case 'Objective': return <TrendingUp color="error" />;
      default: return <Folder />;
    }
  };

  const getCategoryLevelColor = (level: number) => {
    switch (level) {
      case 1: return 'primary';
      case 2: return 'secondary';
      case 3: return 'success';
      case 4: return 'info';
      case 5: return 'warning';
      case 6: return 'error';
      default: return 'default';
    }
  };

  return (
    <Box className={`tree-search ${className}`} sx={{ position: 'relative' }}>
      {/* Search Input */}
      <TextField
        fullWidth
        size="small"
        value={searchQuery}
        onChange={handleSearchChange}
        placeholder={placeholder}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <Search color="action" />
            </InputAdornment>
          ),
          endAdornment: (
            <InputAdornment position="end">
              {searchQuery && (
                <IconButton
                  size="small"
                  onClick={handleClearSearch}
                  edge="end"
                >
                  <Clear />
                </IconButton>
              )}
            </InputAdornment>
          ),
        }}
        sx={{
          '& .MuiOutlinedInput-root': {
            borderRadius: 2,
            backgroundColor: 'background.paper',
            '&:hover': {
              backgroundColor: 'action.hover',
            },
            '&.Mui-focused': {
              backgroundColor: 'background.paper',
            },
          },
        }}
      />

      {/* Advanced Filters */}
      {showAdvancedFilters && (
        <Box sx={{ mt: 2, p: 2, border: 1, borderColor: 'divider', borderRadius: 1 }}>
          <Typography variant="subtitle2" gutterBottom sx={{ display: 'flex', alignItems: 'center' }}>
            <FilterList sx={{ mr: 1, fontSize: '1rem' }} />
            Advanced Filters
          </Typography>
          
          <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
            <Chip
              label={`Type: ${advancedFilters.categoryType || 'All'}`}
              size="small"
              onClick={() => handleAdvancedFilterChange('categoryType', 'Subject')}
              variant={advancedFilters.categoryType ? 'filled' : 'outlined'}
              color={advancedFilters.categoryType ? 'primary' : 'default'}
            />
            
            <Chip
              label={`Level: ${advancedFilters.level || 'All'}`}
              size="small"
              onClick={() => handleAdvancedFilterChange('level', 1)}
              variant={advancedFilters.level ? 'filled' : 'outlined'}
              color={advancedFilters.level ? 'primary' : 'default'}
            />
            
            <Chip
              label={`Include Questions: ${advancedFilters.includeQuestions ? 'Yes' : 'No'}`}
              size="small"
              onClick={() => handleAdvancedFilterChange('includeQuestions', !advancedFilters.includeQuestions)}
              variant="outlined"
              color={advancedFilters.includeQuestions ? 'success' : 'default'}
            />
            
            <Chip
              label={`Max Depth: ${advancedFilters.maxDepth}`}
              size="small"
              variant="outlined"
            />
          </Box>
        </Box>
      )}

      {/* Search Results */}
      <Collapse in={showResults && searchResults.length > 0}>
        <Box
          sx={{
            position: 'absolute',
            top: '100%',
            left: 0,
            right: 0,
            zIndex: 1000,
            backgroundColor: 'background.paper',
            border: 1,
            borderColor: 'divider',
            borderRadius: 1,
            boxShadow: 3,
            maxHeight: 400,
            overflow: 'auto',
            mt: 1,
          }}
        >
          <Box sx={{ p: 1, borderBottom: 1, borderColor: 'divider' }}>
            <Typography variant="caption" color="text.secondary">
              {searchResults.length} result{searchResults.length !== 1 ? 's' : ''} found
            </Typography>
            <IconButton
              size="small"
              onClick={() => setShowResults(false)}
              sx={{ float: 'right' }}
            >
              <Close fontSize="small" />
            </IconButton>
          </Box>
          
          <List dense>
            {searchResults.map((category, index) => (
              <React.Fragment key={category.id}>
                <ListItem
                  button
                  onClick={() => handleResultClick(category)}
                  sx={{
                    '&:hover': {
                      backgroundColor: 'action.hover',
                    },
                  }}
                >
                  <ListItemIcon>
                    {getCategoryIcon(category.type)}
                  </ListItemIcon>
                  
                  <ListItemText
                    primary={
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Typography variant="body2" noWrap>
                          {category.name}
                        </Typography>
                        <Chip
                          label={category.type}
                          size="small"
                          variant="outlined"
                          color={getCategoryLevelColor(category.level) as any}
                        />
                        {category.level > 1 && (
                          <Chip
                            label={`L${category.level}`}
                            size="small"
                            variant="outlined"
                            color="default"
                          />
                        )}
                      </Box>
                    }
                    secondary={
                      <Box>
                        {category.code && (
                          <Typography variant="caption" display="block" fontFamily="monospace">
                            {category.code}
                          </Typography>
                        )}
                        {category.description && (
                          <Typography variant="caption" color="text.secondary" noWrap>
                            {category.description}
                          </Typography>
                        )}
                        {category.questionCount > 0 && (
                          <Typography variant="caption" color="primary">
                            {category.questionCount} questions
                          </Typography>
                        )}
                      </Box>
                    }
                  />
                </ListItem>
                
                {index < searchResults.length - 1 && <Divider />}
              </React.Fragment>
            ))}
          </List>
        </Box>
      </Collapse>

      {/* Loading State */}
      {isLoading && (
        <Box sx={{ mt: 1, textAlign: 'center' }}>
          <Typography variant="caption" color="text.secondary">
            Searching...
          </Typography>
        </Box>
      )}

      {/* No Results */}
      {showResults && searchQuery && !isLoading && searchResults.length === 0 && (
        <Box sx={{ mt: 1, p: 2, textAlign: 'center', backgroundColor: 'action.hover', borderRadius: 1 }}>
          <Typography variant="body2" color="text.secondary">
            No categories found matching "{searchQuery}"
          </Typography>
          <Typography variant="caption" color="text.secondary">
            Try adjusting your search terms or filters
          </Typography>
        </Box>
      )}
    </Box>
  );
};
