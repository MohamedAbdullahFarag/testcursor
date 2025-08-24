import React, { useState, useCallback, useMemo } from 'react';
import {
  Box,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  Checkbox,
  IconButton,
  Tooltip,
  Chip,
  useTheme,
} from '@mui/material';
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  Preview as PreviewIcon,
  Visibility as VisibilityIcon,
  VisibilityOff as VisibilityOffIcon,
} from '@mui/icons-material';

import { Question, QuestionType, QuestionStatus, QuestionDifficulty } from '../types';
import { getQuestionTypeDisplayName, getQuestionTypeColor, getStatusColor, formatDate } from '../utils';

interface QuestionListProps {
  questions: Question[];
  loading?: boolean;
  onQuestionSelect?: (question: Question, selected: boolean) => void;
  onQuestionEdit?: (question: Question) => void;
  onQuestionDelete?: (question: Question) => void;
  onQuestionPreview?: (question: Question) => void;
  selectedQuestions?: Question[];
  onSelectionChange?: (questions: Question[]) => void;
  showPagination?: boolean;
  pageSize?: number;
  onPageChange?: (page: number) => void;
  onPageSizeChange?: (pageSize: number) => void;
  currentPage?: number;
  totalCount?: number;
}

const QuestionList: React.FC<QuestionListProps> = ({
  questions,
  loading = false,
  onQuestionSelect,
  onQuestionEdit,
  onQuestionDelete,
  onQuestionPreview,
  selectedQuestions = [],
  onSelectionChange,
  showPagination = true,
  pageSize = 20,
  onPageChange,
  onPageSizeChange,
  currentPage = 0,
  totalCount = 0,
}) => {
  const theme = useTheme();

  // State
  const [page, setPage] = useState(currentPage);
  const [rowsPerPage, setRowsPerPage] = useState(pageSize);

  // Computed values
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

  const handleChangePage = useCallback((event: unknown, newPage: number) => {
    setPage(newPage);
    onPageChange?.(newPage);
  }, [onPageChange]);

  const handleChangeRowsPerPage = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
    const newPageSize = parseInt(event.target.value, 10);
    setRowsPerPage(newPageSize);
    setPage(0);
    onPageSizeChange?.(newPageSize);
  }, [onPageSizeChange]);

  const handleEdit = useCallback((question: Question) => {
    onQuestionEdit?.(question);
  }, [onQuestionEdit]);

  const handleDelete = useCallback((question: Question) => {
    onQuestionDelete?.(question);
  }, [onQuestionDelete]);

  const handlePreview = useCallback((question: Question) => {
    onQuestionPreview?.(question);
  }, [onQuestionPreview]);

  // Render functions
  const renderQuestionType = (type: QuestionType) => (
    <Chip
      label={getQuestionTypeDisplayName(type)}
      size="small"
      sx={{
        backgroundColor: getQuestionTypeColor(type),
        color: 'white',
        fontWeight: 'bold',
        fontSize: '0.75rem',
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
      sx={{ fontSize: '0.75rem' }}
    />
  );

  const renderStatus = (status: QuestionStatus) => (
    <Chip
      label={status}
      size="small"
      sx={{
        backgroundColor: getStatusColor(status),
        color: 'white',
        fontSize: '0.75rem',
      }}
    />
  );

  const renderTags = (tags: string[]) => (
    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
      {tags.slice(0, 2).map((tag, index) => (
        <Chip
          key={index}
          label={tag}
          size="small"
          variant="outlined"
          sx={{ fontSize: '0.65rem' }}
        />
      ))}
      {tags.length > 2 && (
        <Chip
          label={`+${tags.length - 2}`}
          size="small"
          variant="outlined"
          sx={{ fontSize: '0.65rem' }}
        />
      )}
    </Box>
  );

  const renderActions = (question: Question) => (
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
  );

  if (loading) {
    return (
      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" sx={{ mb: 2 }}>Loading questions...</Typography>
      </Paper>
    );
  }

  if (questions.length === 0) {
    return (
      <Paper sx={{ p: 4, textAlign: 'center' }}>
        <Typography variant="h6" color="text.secondary" sx={{ mb: 2 }}>
          No questions found
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Create your first question to get started
        </Typography>
      </Paper>
    );
  }

  return (
    <Paper sx={{ width: '100%', overflow: 'hidden' }}>
      <TableContainer>
        <Table stickyHeader>
          <TableHead>
            <TableRow>
              <TableCell padding="checkbox">
                <Checkbox
                  indeterminate={isIndeterminate}
                  checked={isAllSelected}
                  onChange={handleSelectAll}
                  color="primary"
                />
              </TableCell>
              <TableCell>Question</TableCell>
              <TableCell>Type</TableCell>
              <TableCell>Difficulty</TableCell>
              <TableCell>Category</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Tags</TableCell>
              <TableCell>Points</TableCell>
              <TableCell>Created</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {questions.map((question) => {
              const isSelected = isQuestionSelected(question);
              return (
                <TableRow
                  key={question.id}
                  hover
                  selected={isSelected}
                  sx={{
                    '&:hover': {
                      backgroundColor: theme.palette.action.hover,
                    },
                  }}
                >
                  <TableCell padding="checkbox">
                    <Checkbox
                      checked={isSelected}
                      onChange={(e) => handleSelectQuestion(question, e.target.checked)}
                      color="primary"
                    />
                  </TableCell>
                  <TableCell>
                    <Box>
                      <Typography
                        variant="body2"
                        sx={{
                          fontWeight: 500,
                          lineHeight: 1.4,
                          display: '-webkit-box',
                          WebkitLineClamp: 2,
                          WebkitBoxOrient: 'vertical',
                          overflow: 'hidden',
                          maxWidth: 300,
                        }}
                      >
                        {question.text}
                      </Typography>
                      {question.explanation && (
                        <Typography
                          variant="caption"
                          color="text.secondary"
                          sx={{
                            display: '-webkit-box',
                            WebkitLineClamp: 1,
                            WebkitBoxOrient: 'vertical',
                            overflow: 'hidden',
                            maxWidth: 300,
                          }}
                        >
                          {question.explanation}
                        </Typography>
                      )}
                    </Box>
                  </TableCell>
                  <TableCell>
                    {renderQuestionType(question.type)}
                  </TableCell>
                  <TableCell>
                    {renderDifficulty(question.difficulty)}
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2">
                      {question.categoryName}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    {renderStatus(question.status)}
                  </TableCell>
                  <TableCell>
                    {renderTags(question.tags)}
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2">
                      {question.metadata.points || '-'}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="caption" color="text.secondary">
                      {formatDate(question.createdAt)}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    {renderActions(question)}
                  </TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </TableContainer>

      {showPagination && (
        <TablePagination
          rowsPerPageOptions={[10, 20, 50, 100]}
          component="div"
          count={totalCount || questions.length}
          rowsPerPage={rowsPerPage}
          page={page}
          onPageChange={handleChangePage}
          onRowsPerPageChange={handleChangeRowsPerPage}
          labelRowsPerPage="Rows per page:"
          labelDisplayedRows={({ from, to, count }) =>
            `${from}-${to} of ${count !== -1 ? count : `more than ${to}`}`
          }
        />
      )}
    </Paper>
  );
};

export default QuestionList;
