import React, { useState, useCallback, useMemo } from 'react';
import {
  Box,
  Grid,
  Card,
  CardContent,
  CardActions,
  Typography,
  Chip,
  IconButton,
  Tooltip,
  Checkbox,
  FormControlLabel,
  useTheme,
  useMediaQuery,
} from '@mui/material';
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  Preview as PreviewIcon,
  Visibility as VisibilityIcon,
  VisibilityOff as VisibilityOffIcon,
  MoreVert as MoreVertIcon,
} from '@mui/icons-material';

import { Question, QuestionType, QuestionStatus, QuestionDifficulty } from '../types';
import { getQuestionTypeDisplayName, getQuestionTypeColor, getStatusColor, formatDate, formatRelativeTime } from '../utils';

interface QuestionGridProps {
  questions: Question[];
  loading?: boolean;
  onQuestionSelect?: (question: Question, selected: boolean) => void;
  onQuestionEdit?: (question: Question) => void;
  onQuestionDelete?: (question: Question) => void;
  onQuestionPreview?: (question: Question) => void;
  selectedQuestions?: Question[];
  onSelectionChange?: (questions: Question[]) => void;
  columns?: number;
  showActions?: boolean;
  showMetadata?: boolean;
  compact?: boolean;
}

const QuestionGrid: React.FC<QuestionGridProps> = ({
  questions,
  loading = false,
  onQuestionSelect,
  onQuestionEdit,
  onQuestionDelete,
  onQuestionPreview,
  selectedQuestions = [],
  onSelectionChange,
  columns = 3,
  showActions = true,
  showMetadata = true,
  compact = false,
}) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const isTablet = useMediaQuery(theme.breakpoints.down('md'));

  // State
  const [hoveredQuestion, setHoveredQuestion] = useState<number | null>(null);

  // Computed values
  const gridColumns = useMemo(() => {
    if (isMobile) return 1;
    if (isTablet) return 2;
    return columns;
  }, [isMobile, isTablet, columns]);

  const isQuestionSelected = useCallback((question: Question) => {
    return selectedQuestions.some(q => q.id === question.id);
  }, [selectedQuestions]);

  const selectedCount = selectedQuestions.length;
  const isAllSelected = questions.length > 0 && selectedCount === questions.length;
  const isIndeterminate = selectedCount > 0 && selectedCount < questions.length;

  // Event handlers
  const handleSelectAll = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.checked) {
      const newSelected = [...questions];
      onSelectionChange?.(newSelected);
    } else {
      onSelectionChange?.([]);
    }
  }, [questions, onSelectionChange]);

  const handleSelectQuestion = useCallback((question: Question, selected: boolean) => {
    if (selected) {
      const newSelected = [...selectedQuestions, question];
      onSelectionChange?.(newSelected);
    } else {
      const newSelected = selectedQuestions.filter(q => q.id !== question.id);
      onSelectionChange?.(newSelected);
    }
  }, [selectedQuestions, onSelectionChange]);

  const handleEdit = useCallback((question: Question) => {
    onQuestionEdit?.(question);
  }, [onQuestionEdit]);

  const handleDelete = useCallback((question: Question) => {
    onQuestionDelete?.(question);
  }, [onQuestionDelete]);

  const handlePreview = useCallback((question: Question) => {
    onQuestionPreview?.(question);
  }, [onQuestionPreview]);

  const handleMouseEnter = useCallback((questionId: number) => {
    setHoveredQuestion(questionId);
  }, []);

  const handleMouseLeave = useCallback(() => {
    setHoveredQuestion(null);
  }, []);

  // Render functions
  const renderQuestionType = (type: QuestionType) => (
    <Chip
      label={getQuestionTypeDisplayName(type)}
      size="small"
      sx={{
        backgroundColor: getQuestionTypeColor(type),
        color: 'white',
        fontWeight: 'bold',
        fontSize: compact ? '0.65rem' : '0.75rem',
      }}
    />
  );

  const renderDifficulty = (difficulty: QuestionDifficulty) => (
    <Chip
      label={difficulty}
      size="small"
      variant="outlined"
      color={
        difficulty === QuestionDifficulty.Easy ? 'success' :
        difficulty === QuestionDifficulty.Medium ? 'warning' :
        difficulty === QuestionDifficulty.Hard ? 'error' : 'default'
      }
      sx={{ fontSize: compact ? '0.65rem' : '0.75rem' }}
    />
  );

  const renderStatus = (status: QuestionStatus) => (
    <Chip
      label={status}
      size="small"
      sx={{
        backgroundColor: getStatusColor(status),
        color: 'white',
        fontSize: compact ? '0.65rem' : '0.75rem',
      }}
    />
  );

  const renderTags = (tags: string[]) => (
    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
      {tags.slice(0, compact ? 2 : 3).map((tag, index) => (
        <Chip
          key={index}
          label={tag}
          size="small"
          variant="outlined"
          sx={{ fontSize: compact ? '0.6rem' : '0.7rem' }}
        />
      ))}
      {tags.length > (compact ? 2 : 3) && (
        <Chip
          label={`+${tags.length - (compact ? 2 : 3)}`}
          size="small"
          variant="outlined"
          sx={{ fontSize: compact ? '0.6rem' : '0.7rem' }}
        />
      )}
    </Box>
  );

  const renderQuestionCard = (question: Question) => {
    const isSelected = isQuestionSelected(question);
    const isHovered = hoveredQuestion === question.id;

    return (
      <Card
        key={question.id}
        sx={{
          height: '100%',
          display: 'flex',
          flexDirection: 'column',
          transition: 'all 0.2s ease-in-out',
          cursor: 'pointer',
          '&:hover': {
            transform: 'translateY(-2px)',
            boxShadow: theme.shadows[8],
          },
          border: isSelected ? `2px solid ${theme.palette.primary.main}` : 'none',
          position: 'relative',
        }}
        onMouseEnter={() => handleMouseEnter(question.id)}
        onMouseLeave={handleMouseLeave}
      >
        {/* Selection Checkbox */}
        {onQuestionSelect && (
          <Box
            sx={{
              position: 'absolute',
              top: 8,
              left: 8,
              zIndex: 1,
              backgroundColor: 'rgba(255, 255, 255, 0.9)',
              borderRadius: '50%',
            }}
          >
            <Checkbox
              checked={isSelected}
              onChange={(e) => handleSelectQuestion(question, e.target.checked)}
              color="primary"
              size="small"
            />
          </Box>
        )}

        <CardContent sx={{ flexGrow: 1, pb: compact ? 1 : 2, pt: onQuestionSelect ? 4 : 2 }}>
          {/* Question Header */}
          <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', mb: 1 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
              {renderQuestionType(question.type)}
              {renderDifficulty(question.difficulty)}
            </Box>
            {renderStatus(question.status)}
          </Box>

          {/* Question Text */}
          <Typography
            variant={compact ? "body2" : "body1"}
            sx={{
              mb: 1,
              fontWeight: 500,
              lineHeight: 1.4,
              display: '-webkit-box',
              WebkitLineClamp: compact ? 2 : 3,
              WebkitBoxOrient: 'vertical',
              overflow: 'hidden',
              cursor: 'pointer',
            }}
            onClick={() => handlePreview(question)}
          >
            {question.text}
          </Typography>

          {/* Category */}
          <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 1 }}>
            Category: {question.categoryName}
          </Typography>

          {/* Tags */}
          <Box sx={{ mb: 1 }}>
            {renderTags(question.tags)}
          </Box>

          {/* Metadata */}
          {showMetadata && (
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
              <Typography variant="caption" color="text.secondary">
                {question.metadata.points ? `${question.metadata.points} pts` : ''}
                {question.metadata.timeLimit ? ` • ${question.metadata.timeLimit}s` : ''}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                {compact ? formatDate(question.createdAt) : formatRelativeTime(question.createdAt)}
              </Typography>
            </Box>
          )}

          {/* Explanation Preview */}
          {question.explanation && !compact && (
            <Typography
              variant="caption"
              color="text.secondary"
              sx={{
                display: '-webkit-box',
                WebkitLineClamp: 2,
                WebkitBoxOrient: 'vertical',
                overflow: 'hidden',
                fontStyle: 'italic',
              }}
            >
              {question.explanation}
            </Typography>
          )}
        </CardContent>

        {/* Actions */}
        {showActions && (
          <CardActions sx={{ p: compact ? 1 : 2, justifyContent: 'space-between' }}>
            <Box sx={{ display: 'flex', gap: 0.5 }}>
              <Tooltip title="Preview">
                <IconButton
                  size="small"
                  onClick={() => handlePreview(question)}
                  sx={{ color: theme.palette.info.main }}
                >
                  <PreviewIcon fontSize="small" />
                </IconButton>
              </Tooltip>
              <Tooltip title="Edit">
                <IconButton
                  size="small"
                  onClick={() => handleEdit(question)}
                  sx={{ color: theme.palette.primary.main }}
                >
                  <EditIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            </Box>
            <Box sx={{ display: 'flex', gap: 0.5 }}>
              <Tooltip title="Delete">
                <IconButton
                  size="small"
                  onClick={() => handleDelete(question)}
                  sx={{ color: theme.palette.error.main }}
                >
                  <DeleteIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            </Box>
          </CardActions>
        )}
      </Card>
    );
  };

  if (loading) {
    return (
      <Box>
        <Grid container spacing={2}>
          {Array.from({ length: gridColumns * 2 }).map((_, index) => (
            <Grid item xs={12} sm={isTablet ? 6 : 4} md={isTablet ? 4 : 3} key={index}>
              <Card sx={{ height: 200 }}>
                <CardContent>
                  <Typography variant="h6" color="text.secondary">
                    Loading...
                  </Typography>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      </Box>
    );
  }

  if (questions.length === 0) {
    return (
      <Box sx={{ textAlign: 'center', py: 4 }}>
        <Typography variant="h6" color="text.secondary" sx={{ mb: 2 }}>
          No questions found
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Create your first question to get started
        </Typography>
      </Box>
    );
  }

  return (
    <Box>
      {/* Select All Checkbox */}
      {onQuestionSelect && questions.length > 0 && (
        <Box sx={{ mb: 2 }}>
          <FormControlLabel
            control={
              <Checkbox
                indeterminate={isIndeterminate}
                checked={isAllSelected}
                onChange={handleSelectAll}
                color="primary"
              />
            }
            label={`Select All (${selectedCount}/${questions.length})`}
          />
        </Box>
      )}

      {/* Questions Grid */}
      <Grid container spacing={compact ? 1 : 2}>
        {questions.map(renderQuestionCard)}
      </Grid>
    </Box>
  );
};

export default QuestionGrid;
