import React, { useState, useEffect, useCallback } from 'react';
import {
  Breadcrumbs,
  Link,
  Typography,
  Box,
  Chip,
  Skeleton,
  Tooltip,
  IconButton
} from '@mui/material';
import {
  Home,
  NavigateNext,
  Folder,
  Assignment,
  TrendingUp,
  Refresh
} from '@mui/icons-material';
import { QuestionBankCategoryDto } from '../types/questionBankTree.types';

interface BreadcrumbNavigationProps {
  categoryId?: number;
  onCategorySelect?: (category: QuestionBankCategoryDto) => void;
  className?: string;
  maxItems?: number;
  showIcons?: boolean;
  showTypeChips?: boolean;
  onRefresh?: () => void;
}

interface BreadcrumbItem {
  id: number;
  name: string;
  code?: string;
  type: string;
  level: number;
  category: QuestionBankCategoryDto;
}

export const BreadcrumbNavigation: React.FC<BreadcrumbNavigationProps> = ({
  categoryId,
  onCategorySelect,
  className = "",
  maxItems = 6,
  showIcons = true,
  showTypeChips = true,
  onRefresh
}) => {
  const [breadcrumbs, setBreadcrumbs] = useState<BreadcrumbItem[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Mock function to get category ancestors - this would be replaced with actual API call
  const fetchCategoryAncestors = useCallback(async (categoryId: number): Promise<QuestionBankCategoryDto[]> => {
    // Simulate API call delay
    await new Promise(resolve => setTimeout(resolve, 100));
    
    // Mock data - replace with actual API call
    const mockAncestors: QuestionBankCategoryDto[] = [
      {
        id: 1,
        name: "Mathematics",
        code: "MATH",
        description: "Mathematics subject area",
        type: "Subject",
        level: 1,
        parentId: null,
        sortOrder: 1,
        isActive: true,
        allowQuestions: true,
        treePath: "/",
        questionCount: 150,
        children: []
      },
      {
        id: 2,
        name: "Algebra",
        code: "MATH-ALG",
        description: "Algebraic concepts and operations",
        type: "Chapter",
        level: 2,
        parentId: 1,
        sortOrder: 1,
        isActive: true,
        allowQuestions: true,
        treePath: "/1/",
        questionCount: 75,
        children: []
      },
      {
        id: 3,
        name: "Linear Equations",
        code: "MATH-ALG-LIN",
        description: "Linear equations and inequalities",
        type: "Topic",
        level: 3,
        parentId: 2,
        sortOrder: 1,
        isActive: true,
        allowQuestions: true,
        treePath: "/1/2/",
        questionCount: 25,
        children: []
      }
    ];

    // Find the path to the current category
    const currentCategoryIndex = mockAncestors.findIndex(c => c.id === categoryId);
    if (currentCategoryIndex === -1) {
      return mockAncestors;
    }

    return mockAncestors.slice(0, currentCategoryIndex + 1);
  }, []);

  const loadBreadcrumbs = useCallback(async () => {
    if (!categoryId) {
      setBreadcrumbs([]);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const ancestors = await fetchCategoryAncestors(categoryId);
      
      const breadcrumbItems: BreadcrumbItem[] = ancestors.map(category => ({
        id: category.id,
        name: category.name,
        code: category.code,
        type: category.type,
        level: category.level,
        category
      }));

      setBreadcrumbs(breadcrumbItems);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load breadcrumbs');
      console.error('Error loading breadcrumbs:', err);
    } finally {
      setIsLoading(false);
    }
  }, [categoryId, fetchCategoryAncestors]);

  useEffect(() => {
    loadBreadcrumbs();
  }, [loadBreadcrumbs]);

  const handleCategoryClick = useCallback((category: QuestionBankCategoryDto) => {
    if (onCategorySelect) {
      onCategorySelect(category);
    }
  }, [onCategorySelect]);

  const handleRefresh = useCallback(() => {
    if (onRefresh) {
      onRefresh();
    } else {
      loadBreadcrumbs();
    }
  }, [onRefresh, loadBreadcrumbs]);

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

  const getCategoryColor = (type: string) => {
    switch (type) {
      case 'Subject': return 'primary';
      case 'Chapter': return 'secondary';
      case 'Topic': return 'success';
      case 'Subtopic': return 'info';
      case 'Skill': return 'warning';
      case 'Objective': return 'error';
      default: return 'default';
    }
  };

  if (isLoading) {
    return (
      <Box className={`breadcrumb-navigation ${className}`} sx={{ mb: 2 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <Skeleton variant="circular" width={20} height={20} />
          <Skeleton variant="text" width={100} height={24} />
          <Skeleton variant="circular" width={16} height={16} />
          <Skeleton variant="text" width={80} height={24} />
          <Skeleton variant="circular" width={16} height={16} />
          <Skeleton variant="text" width={120} height={24} />
        </Box>
      </Box>
    );
  }

  if (error) {
    return (
      <Box className={`breadcrumb-navigation ${className}`} sx={{ mb: 2 }}>
        <Box sx={{ 
          display: 'flex', 
          alignItems: 'center', 
          gap: 1,
          p: 1,
          backgroundColor: 'error.light',
          borderRadius: 1,
          color: 'error.contrastText'
        }}>
          <Typography variant="body2">
            Error loading navigation: {error}
          </Typography>
          <IconButton
            size="small"
            onClick={handleRefresh}
            sx={{ color: 'inherit' }}
          >
            <Refresh fontSize="small" />
          </IconButton>
        </Box>
      </Box>
    );
  }

  if (breadcrumbs.length === 0) {
    return null;
  }

  return (
    <Box className={`breadcrumb-navigation ${className}`} sx={{ mb: 2 }}>
      <Box sx={{ 
        display: 'flex', 
        alignItems: 'center', 
        gap: 1,
        flexWrap: 'wrap'
      }}>
        {/* Home/Root */}
        <Link
          component="button"
          variant="body2"
          onClick={() => handleCategoryClick(breadcrumbs[0])}
          sx={{
            display: 'flex',
            alignItems: 'center',
            gap: 0.5,
            textDecoration: 'none',
            color: 'primary.main',
            '&:hover': {
              textDecoration: 'underline',
              color: 'primary.dark'
            }
          }}
        >
          {showIcons && <Home fontSize="small" />}
          <Typography variant="body2" fontWeight={500}>
            {breadcrumbs[0]?.name || 'Root'}
          </Typography>
        </Link>

        {/* Breadcrumb Separator and Items */}
        {breadcrumbs.slice(1).map((item, index) => (
          <React.Fragment key={item.id}>
            <NavigateNext fontSize="small" color="action" />
            
            <Link
              component="button"
              variant="body2"
              onClick={() => handleCategoryClick(item.category)}
              sx={{
                display: 'flex',
                alignItems: 'center',
                gap: 0.5,
                textDecoration: 'none',
                color: 'text.primary',
                '&:hover': {
                  textDecoration: 'underline',
                  color: 'primary.main'
                }
              }}
            >
              {showIcons && getCategoryIcon(item.type)}
              
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                <Typography variant="body2">
                  {item.name}
                </Typography>
                
                {item.code && (
                  <Typography 
                    variant="caption" 
                    sx={{ 
                      fontFamily: 'monospace',
                      color: 'text.secondary'
                    }}
                  >
                    ({item.code})
                  </Typography>
                )}
              </Box>

              {showTypeChips && (
                <Chip
                  label={item.type}
                  size="small"
                  color={getCategoryColor(item.type) as any}
                  variant="outlined"
                  sx={{ height: 18, fontSize: '0.7rem' }}
                />
              )}
            </Link>
          </React.Fragment>
        ))}

        {/* Refresh Button */}
        {onRefresh && (
          <Tooltip title="Refresh navigation">
            <IconButton
              size="small"
              onClick={handleRefresh}
              sx={{ ml: 1 }}
            >
              <Refresh fontSize="small" />
            </IconButton>
          </Tooltip>
        )}
      </Box>

      {/* Current Location Summary */}
      {breadcrumbs.length > 0 && (
        <Box sx={{ 
          mt: 1, 
          p: 1, 
          backgroundColor: 'action.hover', 
          borderRadius: 1,
          display: 'flex',
          alignItems: 'center',
          gap: 1,
          flexWrap: 'wrap'
        }}>
          <Typography variant="caption" color="text.secondary">
            Current Location:
          </Typography>
          
          <Chip
            label={`Level ${breadcrumbs[breadcrumbs.length - 1]?.level}`}
            size="small"
            variant="outlined"
            color="primary"
          />
          
          <Chip
            label={`${breadcrumbs[breadcrumbs.length - 1]?.type}`}
            size="small"
            variant="outlined"
            color="secondary"
          />
          
          {breadcrumbs[breadcrumbs.length - 1]?.category.questionCount > 0 && (
            <Chip
              label={`${breadcrumbs[breadcrumbs.length - 1]?.category.questionCount} questions`}
              size="small"
              variant="outlined"
              color="success"
            />
          )}
        </Box>
      )}
    </Box>
  );
};
