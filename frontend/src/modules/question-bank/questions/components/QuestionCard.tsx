import React, { useState, useCallback } from 'react';
import {
  Box,
  Card,
  CardContent,
  CardActions,
  Typography,
  Chip,
  IconButton,
  Tooltip,
  Collapse,
  Divider,
  useTheme,
  Grid,
} from '@mui/material';
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  Preview as PreviewIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
  Visibility as VisibilityIcon,
  VisibilityOff as VisibilityOffIcon,
  Star as StarIcon,
  StarBorder as StarBorderIcon,
} from '@mui/icons-material';

import { Question, QuestionType, QuestionStatus, QuestionDifficulty, QuestionOption } from '../types';
import { getQuestionTypeDisplayName, getQuestionTypeColor, getStatusColor, formatDate, formatRelativeTime } from '../utils';

interface QuestionCardProps {
  question: Question;
  onEdit?: (question: Question) => void;
  onDelete?: (question: Question) => void;
  onPreview?: (question: Question) => void;
  onSelect?: (question: Question, selected: boolean) => void;
  onToggleFavorite?: (question: Question, favorite: boolean) => void;
  selected?: boolean;
  favorite?: boolean;
  showActions?: boolean;
  showMetadata?: boolean;
  showExplanation?: boolean;
  showOptions?: boolean;
  compact?: boolean;
  expandable?: boolean;
  variant?: 'default' | 'detailed' | 'minimal';
}

const QuestionCard: React.FC<QuestionCardProps> = ({
  question,
  onEdit,
  onDelete,
  onPreview,
  onSelect,
  onToggleFavorite,
  selected = false,
  favorite = false,
  showActions = true,
  showMetadata = true,
  showExplanation = true,
  showOptions = true,
  compact = false,
  expandable = false,
  variant = 'default',
}) => {
  const theme = useTheme();

  // State
  const [expanded, setExpanded] = useState(false);

  // Event handlers
  const handleEdit = useCallback(() => {
    onEdit?.(question);
  }, [question, onEdit]);

  const handleDelete = useCallback(() => {
    onDelete?.(question);
  }, [question, onDelete]);

  const handlePreview = useCallback(() => {
    onPreview?.(question);
  }, [question, onPreview]);

  const handleSelect = useCallback(() => {
    onSelect?.(question, !selected);
  }, [question, selected, onSelect]);

  const handleToggleFavorite = useCallback(() => {
    onToggleFavorite?.(question, !favorite);
  }, [question, favorite, onToggleFavorite]);

  const handleToggleExpand = useCallback(() => {
    setExpanded(!expanded);
  }, [expanded]);

  // Render functions
  const renderQuestionType = () => (
    <Chip
      label={getQuestionTypeDisplayName(question.type)}
      size="small"
      sx={{
        backgroundColor: getQuestionTypeColor(question.type),
        color: 'white',
        fontWeight: 'bold',
        fontSize: compact ? '0.65rem' : '0.75rem',
      }}
    />
  );

  const renderDifficulty = () => (
    <Chip
      label={question.difficulty}
      size="small"
      variant="outlined"
      color={
        question.difficulty === QuestionDifficulty.Easy ? 'success' :
        question.difficulty === QuestionDifficulty.Medium ? 'warning' :
        question.difficulty === QuestionDifficulty.Hard ? 'error' : 'default'
      }
      sx={{ fontSize: compact ? '0.65rem' : '0.75rem' }}
    />
  );

  const renderStatus = () => (
    <Chip
      label={question.status}
      size="small"
      sx={{
        backgroundColor: getStatusColor(question.status),
        color: 'white',
        fontSize: compact ? '0.65rem' : '0.75rem',
      }}
    />
  );

  const renderTags = () => (
    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5, mb: 1 }}>
      {question.tags.slice(0, compact ? 2 : 3).map((tag, index) => (
        <Chip
          key={index}
          label={tag}
          size="small"
          variant="outlined"
          sx={{ fontSize: compact ? '0.6rem' : '0.7rem' }}
        />
      ))}
      {question.tags.length > (compact ? 2 : 3) && (
        <Chip
          label={`+${question.tags.length - (compact ? 2 : 3)}`}
          size="small"
          variant="outlined"
          sx={{ fontSize: compact ? '0.6rem' : '0.7rem' }}
        />
      )}
    </Box>
  );

  const renderOptions = () => {
    if (!showOptions || !question.options || question.options.length === 0) {
      return null;
    }

    return (
      <Box sx={{ mt: 2 }}>
        <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 1 }}>
          Answer Options:
        </Typography>
        {question.options.map((option, index) => (
          <Box
            key={option.id}
            sx={{
              display: 'flex',
              alignItems: 'center',
              gap: 1,
              mb: 0.5,
              p: 1,
              backgroundColor: option.isCorrect ? theme.palette.success.light : theme.palette.grey[100],
              borderRadius: 1,
              border: option.isCorrect ? `1px solid ${theme.palette.success.main}` : '1px solid transparent',
            }}
          >
            <Typography
              variant="caption"
              sx={{
                fontWeight: option.isCorrect ? 'bold' : 'normal',
                color: option.isCorrect ? theme.palette.success.dark : 'inherit',
              }}
            >
              {String.fromCharCode(65 + index)}. {option.text}
            </Typography>
            {option.isCorrect && (
              <Chip
                label="Correct"
                size="small"
                color="success"
                sx={{ fontSize: '0.6rem' }}
              />
            )}
          </Box>
        ))}
      </Box>
    );
  };

  const renderMetadata = () => {
    if (!showMetadata) return null;

    return (
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
        <Typography variant="caption" color="text.secondary">
          {question.metadata.points ? `${question.metadata.points} pts` : ''}
          {question.metadata.timeLimit ? ` • ${question.metadata.timeLimit}s` : ''}
        </Typography>
        <Typography variant="caption" color="text.secondary">
          {compact ? formatDate(question.createdAt) : formatRelativeTime(question.createdAt)}
        </Typography>
      </Box>
    );
  };

  const renderActions = () => {
    if (!showActions) return null;

    return (
      <CardActions sx={{ p: compact ? 1 : 2, justifyContent: 'space-between' }}>
        <Box sx={{ display: 'flex', gap: 0.5 }}>
          <Tooltip title="Preview">
            <IconButton
              size="small"
              onClick={handlePreview}
              sx={{ color: theme.palette.info.main }}
            >
              <PreviewIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Edit">
            <IconButton
              size="small"
              onClick={handleEdit}
              sx={{ color: theme.palette.primary.main }}
            >
              <EditIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Box>
        <Box sx={{ display: 'flex', gap: 0.5 }}>
          {onToggleFavorite && (
            <Tooltip title={favorite ? 'Remove from favorites' : 'Add to favorites'}>
              <IconButton
                size="small"
                onClick={handleToggleFavorite}
                sx={{ color: favorite ? theme.palette.warning.main : theme.palette.text.secondary }}
              >
                {favorite ? <StarIcon fontSize="small" /> : <StarBorderIcon fontSize="small" />}
              </Tooltip>
            </IconButton>
          )}
          {onSelect && (
            <Tooltip title={selected ? 'Deselect' : 'Select'}>
              <IconButton
                size="small"
                onClick={handleSelect}
                sx={{
                  color: selected ? theme.palette.primary.main : theme.palette.text.secondary,
                }}
              >
                {selected ? <VisibilityIcon fontSize="small" /> : <VisibilityOffIcon fontSize="small" />}
              </IconButton>
            </Tooltip>
          )}
          <Tooltip title="Delete">
            <IconButton
              size="small"
              onClick={handleDelete}
              sx={{ color: theme.palette.error.main }}
            >
              <DeleteIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Box>
      </CardActions>
    );
  };

  const renderExpandButton = () => {
    if (!expandable) return null;

    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 1 }}>
        <IconButton
          size="small"
          onClick={handleToggleExpand}
          sx={{
            transform: expanded ? 'rotate(180deg)' : 'rotate(0deg)',
            transition: 'transform 0.2s ease-in-out',
          }}
        >
          <ExpandMoreIcon />
        </IconButton>
      </Box>
    );
  };

  const renderExpandedContent = () => {
    if (!expandable || !expanded) return null;

    return (
      <Collapse in={expanded} timeout="auto" unmountOnExit>
        <Divider sx={{ my: 1 }} />
        <Box sx={{ p: 2 }}>
          {/* Detailed Metadata */}
          <Box sx={{ mb: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 1 }}>Additional Information</Typography>
            <Grid container spacing={2}>
              <Grid item xs={6}>
                <Typography variant="caption" color="text.secondary">Subject:</Typography>
                <Typography variant="body2">{question.metadata.subject || 'Not specified'}</Typography>
              </Grid>
              <Grid item xs={6}>
                <Typography variant="caption" color="text.secondary">Grade Level:</Typography>
                <Typography variant="body2">{question.metadata.grade || 'Not specified'}</Typography>
              </Grid>
              <Grid item xs={6}>
                <Typography variant="caption" color="text.secondary">Curriculum:</Typography>
                <Typography variant="body2">{question.metadata.curriculum || 'Not specified'}</Typography>
              </Grid>
              <Grid item xs={6}>
                <Typography variant="caption" color="text.secondary">Version:</Typography>
                <Typography variant="body2">{question.version}</Typography>
              </Grid>
            </Grid>
          </Box>

          {/* Learning Objectives */}
          {question.metadata.learningObjectives && question.metadata.learningObjectives.length > 0 && (
            <Box sx={{ mb: 2 }}>
              <Typography variant="subtitle2" sx={{ mb: 1 }}>Learning Objectives</Typography>
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                {question.metadata.learningObjectives.map((objective, index) => (
                  <Chip
                    key={index}
                    label={objective}
                    size="small"
                    variant="outlined"
                    sx={{ fontSize: '0.7rem' }}
                  />
                ))}
              </Box>
            </Box>
          )}

          {/* Prerequisites */}
          {question.metadata.prerequisites && question.metadata.prerequisites.length > 0 && (
            <Box sx={{ mb: 2 }}>
              <Typography variant="subtitle2" sx={{ mb: 1 }}>Prerequisites</Typography>
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                {question.metadata.prerequisites.map((prerequisite, index) => (
                  <Chip
                    key={index}
                    label={prerequisite}
                    size="small"
                    variant="outlined"
                    sx={{ fontSize: '0.7rem' }}
                  />
                ))}
              </Box>
            </Box>
          )}
        </Box>
      </Collapse>
    );
  };

  // Determine card height based on variant
  const getCardHeight = () => {
    switch (variant) {
      case 'minimal':
        return compact ? 120 : 150;
      case 'detailed':
        return 'auto';
      default:
        return compact ? 200 : 250;
    }
  };

  return (
    <Card
      sx={{
        height: getCardHeight(),
        display: 'flex',
        flexDirection: 'column',
        transition: 'all 0.2s ease-in-out',
        cursor: expandable ? 'pointer' : 'default',
        '&:hover': {
          transform: expandable ? 'translateY(-2px)' : 'none',
          boxShadow: expandable ? theme.shadows[8] : theme.shadows[1],
        },
        border: selected ? `2px solid ${theme.palette.primary.main}` : 'none',
        position: 'relative',
      }}
      onClick={expandable ? handleToggleExpand : undefined}
    >
      <CardContent sx={{ flexGrow: 1, pb: compact ? 1 : 2 }}>
        {/* Question Header */}
        <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', mb: 1 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
            {renderQuestionType()}
            {renderDifficulty()}
          </Box>
          {renderStatus()}
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
          }}
        >
          {question.text}
        </Typography>

        {/* Category */}
        <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 1 }}>
          Category: {question.categoryName}
        </Typography>

        {/* Tags */}
        {renderTags()}

        {/* Metadata */}
        {renderMetadata()}

        {/* Options */}
        {renderOptions()}

        {/* Explanation */}
        {showExplanation && question.explanation && (
          <Box sx={{ mt: 2 }}>
            <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 1 }}>
              Explanation:
            </Typography>
            <Typography
              variant="caption"
              sx={{
                display: '-webkit-box',
                WebkitLineClamp: 2,
                WebkitBoxOrient: 'vertical',
                overflow: 'hidden',
                fontStyle: 'italic',
                color: 'text.secondary',
              }}
            >
              {question.explanation}
            </Typography>
          </Box>
        )}
      </CardContent>

      {/* Actions */}
      {renderActions()}

      {/* Expand Button */}
      {renderExpandButton()}

      {/* Expanded Content */}
      {renderExpandedContent()}
    </Card>
  );
};

export default QuestionCard;
