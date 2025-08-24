import React, { useState, useCallback, useMemo } from 'react';
import {
  Box,
  Container,
  Typography,
  Paper,
  Grid,
  Card,
  CardContent,
  CardActions,
  Button,
  Chip,
  IconButton,
  Tooltip,
  Divider,
  Alert,
  Skeleton,
  useTheme,
  useMediaQuery,
} from '@mui/material';
import {
  Add as AddIcon,
  Search as SearchIcon,
  FilterList as FilterIcon,
  ViewList as ListIcon,
  ViewModule as GridIcon,
  Sort as SortIcon,
  Refresh as RefreshIcon,
  Settings as SettingsIcon,
  Help as HelpIcon,
  Info as InfoIcon,
} from '@mui/icons-material';

import { Question, QuestionType, QuestionStatus, QuestionDifficulty } from '../types';
import { QUESTION_TYPES, QUESTION_DIFFICULTIES, QUESTION_STATUSES } from '../constants';
import { 
  getQuestionTypeDisplayName, 
  getQuestionTypeColor, 
  getStatusColor, 
  getStatusIcon,
  formatDate,
  formatRelativeTime
} from '../utils';

// Mock data for development
const mockQuestions: Question[] = [
  {
    id: 1,
    text: 'What is the capital of France?',
    type: QuestionType.MultipleChoice,
    difficulty: QuestionDifficulty.Easy,
    categoryId: 1,
    categoryName: 'Geography',
    tags: ['geography', 'europe', 'capitals'],
    options: [
      { id: 1, text: 'London', isCorrect: false, explanation: '' },
      { id: 2, text: 'Paris', isCorrect: true, explanation: 'Paris is the capital of France' },
      { id: 3, text: 'Berlin', isCorrect: false, explanation: '' },
      { id: 4, text: 'Madrid', isCorrect: false, explanation: '' },
    ],
    correctAnswer: ['Paris'],
    explanation: 'Paris is the capital and largest city of France.',
    metadata: {
      timeLimit: 60,
      points: 1,
      subject: 'Geography',
      grade: '9-12',
    },
    status: QuestionStatus.Published,
    version: 1,
    createdAt: new Date('2024-01-15T10:00:00Z'),
    updatedAt: new Date('2024-01-15T10:00:00Z'),
    createdBy: 1,
    isActive: true,
  },
  {
    id: 2,
    text: 'Solve the quadratic equation: x² + 5x + 6 = 0',
    type: QuestionType.ShortAnswer,
    difficulty: QuestionDifficulty.Medium,
    categoryId: 2,
    categoryName: 'Mathematics',
    tags: ['mathematics', 'algebra', 'quadratic'],
    correctAnswer: ['x = -2 or x = -3'],
    explanation: 'Using factoring: (x + 2)(x + 3) = 0, so x = -2 or x = -3',
    metadata: {
      timeLimit: 120,
      points: 3,
      subject: 'Mathematics',
      grade: '10-12',
    },
    status: QuestionStatus.UnderReview,
    version: 1,
    createdAt: new Date('2024-01-14T14:30:00Z'),
    updatedAt: new Date('2024-01-14T14:30:00Z'),
    createdBy: 2,
    isActive: true,
  },
  {
    id: 3,
    text: 'Explain the process of photosynthesis in plants.',
    type: QuestionType.Essay,
    difficulty: QuestionDifficulty.Hard,
    categoryId: 3,
    categoryName: 'Biology',
    tags: ['biology', 'plants', 'photosynthesis'],
    correctAnswer: ['Photosynthesis is the process by which plants convert light energy into chemical energy...'],
    explanation: 'Photosynthesis is a complex biochemical process that occurs in the chloroplasts of plant cells.',
    metadata: {
      timeLimit: 600,
      points: 10,
      subject: 'Biology',
      grade: '11-12',
    },
    status: QuestionStatus.Draft,
    version: 1,
    createdAt: new Date('2024-01-13T09:15:00Z'),
    updatedAt: new Date('2024-01-13T09:15:00Z'),
    createdBy: 1,
    isActive: true,
  },
];

interface QuestionManagerProps {
  title?: string;
  subtitle?: string;
  showActions?: boolean;
  showFilters?: boolean;
  showSearch?: boolean;
  showStats?: boolean;
  maxQuestions?: number;
  onQuestionSelect?: (question: Question) => void;
  onQuestionEdit?: (question: Question) => void;
  onQuestionDelete?: (question: Question) => void;
  onQuestionPreview?: (question: Question) => void;
  onAddQuestion?: () => void;
  onRefresh?: () => void;
  loading?: boolean;
  error?: string | null;
}

const QuestionManager: React.FC<QuestionManagerProps> = ({
  title = 'Question Management',
  subtitle = 'Manage and organize your question bank',
  showActions = true,
  showFilters = true,
  showSearch = true,
  showStats = true,
  maxQuestions,
  onQuestionSelect,
  onQuestionEdit,
  onQuestionDelete,
  onQuestionPreview,
  onAddQuestion,
  onRefresh,
  loading = false,
  error = null,
}) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const isTablet = useMediaQuery(theme.breakpoints.down('lg'));

  // State
  const [viewMode, setViewMode] = useState<'list' | 'grid'>('list');
  const [selectedQuestions, setSelectedQuestions] = useState<Set<number>>(new Set());
  const [searchQuery, setSearchQuery] = useState('');
  const [filters, setFilters] = useState({
    types: [] as QuestionType[],
    difficulties: [] as QuestionDifficulty[],
    statuses: [] as QuestionStatus[],
    categories: [] as number[],
  });

  // Computed values
  const filteredQuestions = useMemo(() => {
    let filtered = [...mockQuestions];

    // Apply search filter
    if (searchQuery) {
      const query = searchQuery.toLowerCase();
      filtered = filtered.filter(question =>
        question.text.toLowerCase().includes(query) ||
        question.explanation?.toLowerCase().includes(query) ||
        question.tags.some(tag => tag.toLowerCase().includes(query)) ||
        question.categoryName?.toLowerCase().includes(query)
      );
    }

    // Apply type filter
    if (filters.types.length > 0) {
      filtered = filtered.filter(question => filters.types.includes(question.type));
    }

    // Apply difficulty filter
    if (filters.difficulties.length > 0) {
      filtered = filtered.filter(question => filters.difficulties.includes(question.difficulty));
    }

    // Apply status filter
    if (filters.statuses.length > 0) {
      filtered = filtered.filter(question => filters.statuses.includes(question.status));
    }

    // Apply category filter
    if (filters.categories.length > 0) {
      filtered = filtered.filter(question => filters.categories.includes(question.categoryId));
    }

    // Apply max questions limit
    if (maxQuestions) {
      filtered = filtered.slice(0, maxQuestions);
    }

    return filtered;
  }, [mockQuestions, searchQuery, filters, maxQuestions]);

  const questionStats = useMemo(() => {
    const total = mockQuestions.length;
    const published = mockQuestions.filter(q => q.status === QuestionStatus.Published).length;
    const draft = mockQuestions.filter(q => q.status === QuestionStatus.Draft).length;
    const underReview = mockQuestions.filter(q => q.status === QuestionStatus.UnderReview).length;
    const byType = QUESTION_TYPES.reduce((acc, type) => {
      acc[type] = mockQuestions.filter(q => q.type === type).length;
      return acc;
    }, {} as Record<QuestionType, number>);
    const byDifficulty = QUESTION_DIFFICULTIES.reduce((acc, difficulty) => {
      acc[difficulty] = mockQuestions.filter(q => q.difficulty === difficulty).length;
      return acc;
    }, {} as Record<QuestionDifficulty, number>);

    return {
      total,
      published,
      draft,
      underReview,
      byType,
      byDifficulty,
    };
  }, [mockQuestions]);

  // Event handlers
  const handleQuestionSelect = useCallback((questionId: number, selected: boolean) => {
    const newSelected = new Set(selectedQuestions);
    if (selected) {
      newSelected.add(questionId);
    } else {
      newSelected.delete(questionId);
    }
    setSelectedQuestions(newSelected);
  }, [selectedQuestions]);

  const handleSelectAll = useCallback((selected: boolean) => {
    if (selected) {
      setSelectedQuestions(new Set(filteredQuestions.map(q => q.id)));
    } else {
      setSelectedQuestions(new Set());
    }
  }, [filteredQuestions]);

  const handleSearchChange = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchQuery(event.target.value);
  }, []);

  const handleFilterChange = useCallback((filterType: keyof typeof filters, value: any) => {
    setFilters(prev => ({
      ...prev,
      [filterType]: value,
    }));
  }, []);

  const handleRefresh = useCallback(() => {
    if (onRefresh) {
      onRefresh();
    }
  }, [onRefresh]);

  const handleAddQuestion = useCallback(() => {
    if (onAddQuestion) {
      onAddQuestion();
    }
  }, [onAddQuestion]);

  // Render functions
  const renderQuestionCard = (question: Question) => (
    <Card
      key={question.id}
      sx={{
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        transition: 'all 0.2s ease-in-out',
        '&:hover': {
          transform: 'translateY(-2px)',
          boxShadow: theme.shadows[8],
        },
        border: selectedQuestions.has(question.id) ? `2px solid ${theme.palette.primary.main}` : 'none',
      }}
    >
      <CardContent sx={{ flexGrow: 1, pb: 1 }}>
        {/* Question Header */}
        <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', mb: 1 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
            <Chip
              label={getQuestionTypeDisplayName(question.type)}
              size="small"
              sx={{
                backgroundColor: getQuestionTypeColor(question.type),
                color: 'white',
                fontWeight: 'bold',
              }}
            />
            <Chip
              label={question.difficulty}
              size="small"
              variant="outlined"
              color={
                question.difficulty === QuestionDifficulty.Easy ? 'success' :
                question.difficulty === QuestionDifficulty.Medium ? 'warning' :
                question.difficulty === QuestionDifficulty.Hard ? 'error' : 'default'
              }
            />
          </Box>
          <Chip
            icon={<InfoIcon />}
            label={question.status}
            size="small"
            sx={{
              backgroundColor: getStatusColor(question.status),
              color: 'white',
            }}
          />
        </Box>

        {/* Question Text */}
        <Typography
          variant="body2"
          sx={{
            mb: 1,
            fontWeight: 500,
            lineHeight: 1.4,
            display: '-webkit-box',
            WebkitLineClamp: 3,
            WebkitBoxOrient: 'vertical',
            overflow: 'hidden',
          }}
        >
          {question.text}
        </Typography>

        {/* Category and Tags */}
        <Box sx={{ mb: 1 }}>
          <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 0.5 }}>
            Category: {question.categoryName}
          </Typography>
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
            {question.tags.slice(0, 3).map((tag, index) => (
              <Chip
                key={index}
                label={tag}
                size="small"
                variant="outlined"
                sx={{ fontSize: '0.7rem' }}
              />
            ))}
            {question.tags.length > 3 && (
              <Chip
                label={`+${question.tags.length - 3}`}
                size="small"
                variant="outlined"
                sx={{ fontSize: '0.7rem' }}
              />
            )}
          </Box>
        </Box>

        {/* Metadata */}
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="caption" color="text.secondary">
            {question.metadata.points ? `${question.metadata.points} pts` : ''}
            {question.metadata.timeLimit ? ` • ${question.metadata.timeLimit}s` : ''}
          </Typography>
          <Typography variant="caption" color="text.secondary">
            {formatRelativeTime(question.createdAt)}
          </Typography>
        </Box>
      </CardContent>

      <Divider />

      <CardActions sx={{ p: 1, justifyContent: 'space-between' }}>
        <Box sx={{ display: 'flex', gap: 0.5 }}>
          <Tooltip title="Preview">
            <IconButton
              size="small"
              onClick={() => onQuestionPreview?.(question)}
              sx={{ color: theme.palette.info.main }}
            >
              <InfoIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Edit">
            <IconButton
              size="small"
              onClick={() => onQuestionEdit?.(question)}
              sx={{ color: theme.palette.primary.main }}
            >
              <AddIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Box>
        <Box sx={{ display: 'flex', gap: 0.5 }}>
          <Tooltip title="Select">
            <IconButton
              size="small"
              onClick={() => handleQuestionSelect(question.id, !selectedQuestions.has(question.id))}
              sx={{
                color: selectedQuestions.has(question.id) ? theme.palette.primary.main : theme.palette.text.secondary,
              }}
            >
              <AddIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Box>
      </CardActions>
    </Card>
  );

  const renderQuestionList = (question: Question) => (
    <Card
      key={question.id}
      sx={{
        mb: 2,
        transition: 'all 0.2s ease-in-out',
        '&:hover': {
          backgroundColor: theme.palette.action.hover,
        },
        border: selectedQuestions.has(question.id) ? `2px solid ${theme.palette.primary.main}` : 'none',
      }}
    >
      <CardContent sx={{ py: 2, '&:last-child': { pb: 2 } }}>
        <Grid container spacing={2} alignItems="center">
          {/* Selection Checkbox */}
          <Grid item xs={1} sm={1}>
            <IconButton
              size="small"
              onClick={() => handleQuestionSelect(question.id, !selectedQuestions.has(question.id))}
              sx={{
                color: selectedQuestions.has(question.id) ? theme.palette.primary.main : theme.palette.text.secondary,
              }}
            >
              <AddIcon fontSize="small" />
            </IconButton>
          </Grid>

          {/* Question Content */}
          <Grid item xs={8} sm={6}>
            <Typography
              variant="body1"
              sx={{
                fontWeight: 500,
                lineHeight: 1.4,
                display: '-webkit-box',
                WebkitLineClamp: 2,
                WebkitBoxOrient: 'vertical',
                overflow: 'hidden',
              }}
            >
              {question.text}
            </Typography>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mt: 1 }}>
              <Typography variant="caption" color="text.secondary">
                {question.categoryName}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                •
              </Typography>
              <Typography variant="caption" color="text.secondary">
                {formatRelativeTime(question.createdAt)}
              </Typography>
            </Box>
          </Grid>

          {/* Question Type */}
          <Grid item xs={3} sm={2}>
            <Chip
              label={getQuestionTypeDisplayName(question.type)}
              size="small"
              sx={{
                backgroundColor: getQuestionTypeColor(question.type),
                color: 'white',
                fontWeight: 'bold',
                width: '100%',
              }}
            />
          </Grid>

          {/* Difficulty */}
          <Grid item xs={3} sm={1}>
            <Chip
              label={question.difficulty}
              size="small"
              variant="outlined"
              color={
                question.difficulty === QuestionDifficulty.Easy ? 'success' :
                question.difficulty === QuestionDifficulty.Medium ? 'warning' :
                question.difficulty === QuestionDifficulty.Hard ? 'error' : 'default'
              }
            />
          </Grid>

          {/* Status */}
          <Grid item xs={3} sm={1}>
            <Chip
              label={question.status}
              size="small"
              sx={{
                backgroundColor: getStatusColor(question.status),
                color: 'white',
              }}
            />
          </Grid>

          {/* Actions */}
          <Grid item xs={3} sm={1}>
            <Box sx={{ display: 'flex', gap: 0.5, justifyContent: 'flex-end' }}>
              <Tooltip title="Preview">
                <IconButton
                  size="small"
                  onClick={() => onQuestionPreview?.(question)}
                  sx={{ color: theme.palette.info.main }}
                >
                  <InfoIcon fontSize="small" />
                </IconButton>
              </Tooltip>
              <Tooltip title="Edit">
                <IconButton
                  size="small"
                  onClick={() => onQuestionEdit?.(question)}
                  sx={{ color: theme.palette.primary.main }}
                >
                  <AddIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            </Box>
          </Grid>
        </Grid>
      </CardContent>
    </Card>
  );

  const renderStats = () => (
    <Grid container spacing={2} sx={{ mb: 3 }}>
      <Grid item xs={6} sm={3}>
        <Card sx={{ textAlign: 'center', p: 2 }}>
          <Typography variant="h4" color="primary" sx={{ fontWeight: 'bold' }}>
            {questionStats.total}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Total Questions
          </Typography>
        </Card>
      </Grid>
      <Grid item xs={6} sm={3}>
        <Card sx={{ textAlign: 'center', p: 2 }}>
          <Typography variant="h4" color="success.main" sx={{ fontWeight: 'bold' }}>
            {questionStats.published}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Published
          </Typography>
        </Card>
      </Grid>
      <Grid item xs={6} sm={3}>
        <Card sx={{ textAlign: 'center', p: 2 }}>
          <Typography variant="h4" color="warning.main" sx={{ fontWeight: 'bold' }}>
            {questionStats.underReview}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Under Review
          </Typography>
        </Card>
      </Grid>
      <Grid item xs={6} sm={3}>
        <Card sx={{ textAlign: 'center', p: 2 }}>
          <Typography variant="h4" color="text.secondary" sx={{ fontWeight: 'bold' }}>
            {questionStats.draft}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Draft
          </Typography>
        </Card>
      </Grid>
    </Grid>
  );

  const renderToolbar = () => (
    <Box sx={{ mb: 3 }}>
      <Grid container spacing={2} alignItems="center">
        {/* Search */}
        {showSearch && (
          <Grid item xs={12} sm={6} md={4}>
            <Box sx={{ position: 'relative' }}>
              <SearchIcon
                sx={{
                  position: 'absolute',
                  left: 12,
                  top: '50%',
                  transform: 'translateY(-50%)',
                  color: 'text.secondary',
                }}
              />
              <input
                type="text"
                placeholder="Search questions..."
                value={searchQuery}
                onChange={handleSearchChange}
                style={{
                  width: '100%',
                  padding: '12px 12px 12px 44px',
                  border: `1px solid ${theme.palette.divider}`,
                  borderRadius: theme.shape.borderRadius,
                  fontSize: '14px',
                  outline: 'none',
                }}
              />
            </Box>
          </Grid>
        )}

        {/* View Mode Toggle */}
        <Grid item xs={6} sm={3} md={2}>
          <Box sx={{ display: 'flex', gap: 1 }}>
            <Tooltip title="List View">
              <IconButton
                onClick={() => setViewMode('list')}
                color={viewMode === 'list' ? 'primary' : 'default'}
                size="small"
              >
                <ListIcon />
              </IconButton>
            </Tooltip>
            <Tooltip title="Grid View">
              <IconButton
                onClick={() => setViewMode('grid')}
                color={viewMode === 'grid' ? 'primary' : 'default'}
                size="small"
              >
                <GridIcon />
              </IconButton>
            </Tooltip>
          </Box>
        </Grid>

        {/* Actions */}
        {showActions && (
          <Grid item xs={6} sm={3} md={2}>
            <Box sx={{ display: 'flex', gap: 1 }}>
              <Tooltip title="Add Question">
                <Button
                  variant="contained"
                  startIcon={<AddIcon />}
                  onClick={handleAddQuestion}
                  size="small"
                >
                  Add
                </Button>
              </Tooltip>
            </Box>
          </Grid>
        )}

        {/* Right Side Actions */}
        <Grid item xs={12} sm={12} md={4}>
          <Box sx={{ display: 'flex', gap: 1, justifyContent: 'flex-end' }}>
            <Tooltip title="Refresh">
              <IconButton onClick={handleRefresh} size="small">
                <RefreshIcon />
              </IconButton>
            </Tooltip>
            <Tooltip title="Filters">
              <IconButton size="small">
                <FilterIcon />
              </IconButton>
            </Tooltip>
            <Tooltip title="Sort">
              <IconButton size="small">
                <SortIcon />
              </IconButton>
            </Tooltip>
            <Tooltip title="Settings">
              <IconButton size="small">
                <SettingsIcon />
              </IconButton>
            </Tooltip>
            <Tooltip title="Help">
              <IconButton size="small">
                <HelpIcon />
              </IconButton>
            </Tooltip>
          </Box>
        </Grid>
      </Grid>
    </Box>
  );

  if (loading) {
    return (
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <Skeleton variant="text" width="60%" height={40} sx={{ mb: 2 }} />
        <Skeleton variant="text" width="40%" height={24} sx={{ mb: 4 }} />
        <Grid container spacing={3}>
          {Array.from({ length: 6 }).map((_, index) => (
            <Grid item xs={12} sm={6} md={4} key={index}>
              <Skeleton variant="rectangular" height={200} />
            </Grid>
          ))}
        </Grid>
      </Container>
    );
  }

  return (
    <Container maxWidth="xl" sx={{ py: 4 }}>
      {/* Header */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 'bold', mb: 1 }}>
          {title}
        </Typography>
        <Typography variant="body1" color="text.secondary" sx={{ mb: 2 }}>
          {subtitle}
        </Typography>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}
      </Box>

      {/* Statistics */}
      {showStats && renderStats()}

      {/* Toolbar */}
      {renderToolbar()}

      {/* Questions */}
      <Box>
        {filteredQuestions.length === 0 ? (
          <Paper sx={{ p: 4, textAlign: 'center' }}>
            <Typography variant="h6" color="text.secondary" sx={{ mb: 2 }}>
              No questions found
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              {searchQuery || Object.values(filters).some(f => f.length > 0)
                ? 'Try adjusting your search or filters'
                : 'Get started by creating your first question'
              }
            </Typography>
            {onAddQuestion && (
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                onClick={handleAddQuestion}
              >
                Create Question
              </Button>
            )}
          </Paper>
        ) : (
          <>
            {/* Selection Summary */}
            {selectedQuestions.size > 0 && (
              <Box sx={{ mb: 2, p: 2, backgroundColor: theme.palette.primary.light, borderRadius: 1 }}>
                <Typography variant="body2" color="primary.contrastText">
                  {selectedQuestions.size} question{selectedQuestions.size !== 1 ? 's' : ''} selected
                </Typography>
              </Box>
            )}

            {/* Questions Grid/List */}
            {viewMode === 'grid' ? (
              <Grid container spacing={3}>
                {filteredQuestions.map(renderQuestionCard)}
              </Grid>
            ) : (
              <Box>
                {filteredQuestions.map(renderQuestionList)}
              </Box>
            )}
          </>
        )}
      </Box>
    </Container>
  );
};

export default QuestionManager;
