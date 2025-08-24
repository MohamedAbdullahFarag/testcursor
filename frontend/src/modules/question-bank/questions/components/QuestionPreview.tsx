import React, { useState, useCallback } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
  Chip,
  Divider,
  IconButton,
  useTheme,
  Grid,
  Paper,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Collapse,
  Alert,
} from '@mui/material';
import {
  Close as CloseIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
  CheckCircle as CheckCircleIcon,
  RadioButtonUnchecked as RadioButtonUncheckedIcon,
  Star as StarIcon,
  StarBorder as StarBorderIcon,
  Edit as EditIcon,
  ContentCopy as ContentCopyIcon,
  Download as DownloadIcon,
  Share as ShareIcon,
} from '@mui/icons-material';

import { Question, QuestionOption } from '../types';
import { getQuestionTypeDisplayName, getQuestionTypeColor, getStatusColor, formatDate } from '../utils';

interface QuestionPreviewProps {
  question: Question | null;
  open: boolean;
  onClose: () => void;
  onEdit?: (question: Question) => void;
  onCopy?: (question: Question) => void;
  onDownload?: (question: Question) => void;
  onShare?: (question: Question) => void;
  showActions?: boolean;
  showMetadata?: boolean;
  showExplanation?: boolean;
  showOptions?: boolean;
  showHistory?: boolean;
  showAnalytics?: boolean;
}

const QuestionPreview: React.FC<QuestionPreviewProps> = ({
  question,
  open,
  onClose,
  onEdit,
  onCopy,
  onDownload,
  onShare,
  showActions = true,
  showMetadata = true,
  showExplanation = true,
  showOptions = true,
  showHistory = false,
  showAnalytics = false,
}) => {
  const theme = useTheme();
  const [expandedSections, setExpandedSections] = useState<Set<string>>(new Set(['question', 'options']));

  // Event handlers
  const handleToggleSection = useCallback((section: string) => {
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

  const handleEdit = useCallback(() => {
    if (question && onEdit) {
      onEdit(question);
      onClose();
    }
  }, [question, onEdit, onClose]);

  const handleCopy = useCallback(() => {
    if (question && onCopy) {
      onCopy(question);
    }
  }, [question, onCopy]);

  const handleDownload = useCallback(() => {
    if (question && onDownload) {
      onDownload(question);
    }
  }, [question, onDownload]);

  const handleShare = useCallback(() => {
    if (question && onShare) {
      onShare(question);
    }
  }, [question, onShare]);

  // Render functions
  const renderQuestionHeader = () => (
    <Box sx={{ mb: 3 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Chip
            label={getQuestionTypeDisplayName(question!.type)}
            size="medium"
            sx={{
              backgroundColor: getQuestionTypeColor(question!.type),
              color: 'white',
              fontWeight: 'bold',
            }}
          />
          <Chip
            label={question!.difficulty}
            size="medium"
            variant="outlined"
            color={
              question!.difficulty === 'Easy' ? 'success' :
              question!.difficulty === 'Medium' ? 'warning' :
              question!.difficulty === 'Hard' ? 'error' : 'default'
            }
          />
          <Chip
            label={question!.status}
            size="medium"
            sx={{
              backgroundColor: getStatusColor(question!.status),
              color: 'white',
            }}
          />
        </Box>
        <Typography variant="h6" color="text.secondary">
          #{question!.id}
        </Typography>
      </Box>

      <Typography variant="h5" sx={{ mb: 1, fontWeight: 600, lineHeight: 1.4 }}>
        {question!.text}
      </Typography>

      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2 }}>
        <Typography variant="body2" color="text.secondary">
          Category: {question!.categoryName}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Points: {question!.metadata.points || 1}
        </Typography>
        {question!.metadata.timeLimit && (
          <Typography variant="body2" color="text.secondary">
            Time: {question!.metadata.timeLimit}s
          </Typography>
        )}
      </Box>

      {question!.tags && question!.tags.length > 0 && (
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
          {question!.tags.map((tag, index) => (
            <Chip
              key={index}
              label={tag}
              size="small"
              variant="outlined"
              sx={{ fontSize: '0.75rem' }}
            />
          ))}
        </Box>
      )}
    </Box>
  );

  const renderQuestionOptions = () => {
    if (!showOptions || !question!.options || question!.options.length === 0) {
      return null;
    }

    return (
      <Box sx={{ mb: 3 }}>
        <Box
          sx={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            cursor: 'pointer',
            mb: 2,
          }}
          onClick={() => handleToggleSection('options')}
        >
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            Answer Options
          </Typography>
          {expandedSections.has('options') ? <ExpandLessIcon /> : <ExpandMoreIcon />}
        </Box>

        <Collapse in={expandedSections.has('options')}>
          <List sx={{ bgcolor: 'background.paper', borderRadius: 1 }}>
            {question!.options.map((option, index) => (
              <ListItem
                key={option.id}
                sx={{
                  border: option.isCorrect ? `2px solid ${theme.palette.success.main}` : '1px solid',
                  borderColor: option.isCorrect ? theme.palette.success.main : theme.palette.divider,
                  borderRadius: 1,
                  mb: 1,
                  bgcolor: option.isCorrect ? theme.palette.success.light : 'transparent',
                }}
              >
                <ListItemIcon sx={{ minWidth: 40 }}>
                  {option.isCorrect ? (
                    <CheckCircleIcon color="success" />
                  ) : (
                    <RadioButtonUncheckedIcon color="action" />
                  )}
                </ListItemIcon>
                <ListItemText
                  primary={
                    <Typography variant="body1" sx={{ fontWeight: option.isCorrect ? 600 : 400 }}>
                      {String.fromCharCode(65 + index)}. {option.text}
                    </Typography>
                  }
                  secondary={
                    option.explanation && (
                      <Typography variant="body2" color="text.secondary" sx={{ mt: 1, fontStyle: 'italic' }}>
                        {option.explanation}
                      </Typography>
                    )
                  }
                />
              </ListItem>
            ))}
          </List>
        </Collapse>
      </Box>
    );
  };

  const renderExplanation = () => {
    if (!showExplanation || !question!.explanation) {
      return null;
    }

    return (
      <Box sx={{ mb: 3 }}>
        <Box
          sx={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            cursor: 'pointer',
            mb: 2,
          }}
          onClick={() => handleToggleSection('explanation')}
        >
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            Explanation
          </Typography>
          {expandedSections.has('explanation') ? <ExpandLessIcon /> : <ExpandMoreIcon />}
        </Box>

        <Collapse in={expandedSections.has('explanation')}>
          <Paper sx={{ p: 2, bgcolor: 'background.default' }}>
            <Typography variant="body1" sx={{ lineHeight: 1.6 }}>
              {question!.explanation}
            </Typography>
          </Paper>
        </Collapse>
      </Box>
    );
  };

  const renderMetadata = () => {
    if (!showMetadata) {
      return null;
    }

    return (
      <Box sx={{ mb: 3 }}>
        <Box
          sx={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            cursor: 'pointer',
            mb: 2,
          }}
          onClick={() => handleToggleSection('metadata')}
        >
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            Additional Information
          </Typography>
          {expandedSections.has('metadata') ? <ExpandLessIcon /> : <ExpandMoreIcon />}
        </Box>

        <Collapse in={expandedSections.has('metadata')}>
          <Grid container spacing={2}>
            <Grid item xs={6}>
              <Typography variant="caption" color="text.secondary">Subject:</Typography>
              <Typography variant="body2">{question!.metadata.subject || 'Not specified'}</Typography>
            </Grid>
            <Grid item xs={6}>
              <Typography variant="caption" color="text.secondary">Grade Level:</Typography>
              <Typography variant="body2">{question!.metadata.grade || 'Not specified'}</Typography>
            </Grid>
            <Grid item xs={6}>
              <Typography variant="caption" color="text.secondary">Curriculum:</Typography>
              <Typography variant="body2">{question!.metadata.curriculum || 'Not specified'}</Typography>
            </Grid>
            <Grid item xs={6}>
              <Typography variant="caption" color="text.secondary">Version:</Typography>
              <Typography variant="body2">{question!.version}</Typography>
            </Grid>
            <Grid item xs={6}>
              <Typography variant="caption" color="text.secondary">Created:</Typography>
              <Typography variant="body2">{formatDate(question!.createdAt)}</Typography>
            </Grid>
            <Grid item xs={6}>
              <Typography variant="caption" color="text.secondary">Last Modified:</Typography>
              <Typography variant="body2">{formatDate(question!.updatedAt)}</Typography>
            </Grid>
          </Grid>

          {question!.metadata.learningObjectives && question!.metadata.learningObjectives.length > 0 && (
            <Box sx={{ mt: 2 }}>
              <Typography variant="subtitle2" sx={{ mb: 1 }}>Learning Objectives</Typography>
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                {question!.metadata.learningObjectives.map((objective, index) => (
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

          {question!.metadata.prerequisites && question!.metadata.prerequisites.length > 0 && (
            <Box sx={{ mt: 2 }}>
              <Typography variant="subtitle2" sx={{ mb: 1 }}>Prerequisites</Typography>
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                {question!.metadata.prerequisites.map((prerequisite, index) => (
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
        </Collapse>
      </Box>
    );
  };

  const renderHistory = () => {
    if (!showHistory) {
      return null;
    }

    return (
      <Box sx={{ mb: 3 }}>
        <Box
          sx={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            cursor: 'pointer',
            mb: 2,
          }}
          onClick={() => handleToggleSection('history')}
        >
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            Version History
          </Typography>
          {expandedSections.has('history') ? <ExpandLessIcon /> : <ExpandLessIcon />}
        </Box>

        <Collapse in={expandedSections.has('history')}>
          <Paper sx={{ p: 2, bgcolor: 'background.default' }}>
            <Typography variant="body2" color="text.secondary">
              Version history information will be displayed here.
            </Typography>
          </Paper>
        </Collapse>
      </Box>
    );
  };

  const renderAnalytics = () => {
    if (!showAnalytics) {
      return null;
    }

    return (
      <Box sx={{ mb: 3 }}>
        <Box
          sx={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            cursor: 'pointer',
            mb: 2,
          }}
          onClick={() => handleToggleSection('analytics')}
        >
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            Usage Analytics
          </Typography>
          {expandedSections.has('analytics') ? <ExpandLessIcon /> : <ExpandMoreIcon />}
        </Box>

        <Collapse in={expandedSections.has('analytics')}>
          <Paper sx={{ p: 2, bgcolor: 'background.default' }}>
            <Typography variant="body2" color="text.secondary">
              Analytics information will be displayed here.
            </Typography>
          </Paper>
        </Collapse>
      </Box>
    );
  };

  const renderActions = () => {
    if (!showActions) {
      return null;
    }

    return (
      <DialogActions sx={{ p: 3, pt: 0 }}>
        <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
          {onEdit && (
            <Button
              variant="outlined"
              startIcon={<EditIcon />}
              onClick={handleEdit}
            >
              Edit Question
            </Button>
          )}
          {onCopy && (
            <Button
              variant="outlined"
              startIcon={<ContentCopyIcon />}
              onClick={handleCopy}
            >
              Copy
            </Button>
          )}
          {onDownload && (
            <Button
              variant="outlined"
              startIcon={<DownloadIcon />}
              onClick={handleDownload}
            >
              Download
            </Button>
          )}
          {onShare && (
            <Button
              variant="outlined"
              startIcon={<ShareIcon />}
              onClick={handleShare}
            >
              Share
            </Button>
          )}
          <Button
            variant="contained"
            onClick={onClose}
          >
            Close
          </Button>
        </Box>
      </DialogActions>
    );
  };

  if (!question) {
    return null;
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="md"
      fullWidth
      PaperProps={{
        sx: {
          borderRadius: 2,
          maxHeight: '90vh',
        }
      }}
    >
      <DialogTitle sx={{ pb: 1 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <Typography variant="h5" sx={{ fontWeight: 600 }}>
            Question Preview
          </Typography>
          <IconButton onClick={onClose} size="small">
            <CloseIcon />
          </IconButton>
        </Box>
      </DialogTitle>

      <DialogContent sx={{ pb: 0 }}>
        {renderQuestionHeader()}
        <Divider sx={{ my: 2 }} />
        {renderQuestionOptions()}
        {renderExplanation()}
        {renderMetadata()}
        {renderHistory()}
        {renderAnalytics()}
      </DialogContent>

      {renderActions()}
    </Dialog>
  );
};

export default QuestionPreview;
