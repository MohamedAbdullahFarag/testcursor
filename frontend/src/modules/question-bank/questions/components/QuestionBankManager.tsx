import React, { useState, useCallback } from 'react';
import {
  Box,
  Paper,
  Typography,
  Button,
  Grid,
  Card,
  CardContent,
  CardActions,
  Chip,
  IconButton,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  useTheme,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Visibility as VisibilityIcon,
  Folder as FolderIcon,
  FolderOpen as FolderOpenIcon,
  Public as PublicIcon,
  Lock as LockIcon,
  Search as SearchIcon,
} from '@mui/icons-material';

import { QuestionBank, QuestionBankStatus } from '../types';

interface QuestionBankManagerProps {
  questionBanks: QuestionBank[];
  onAddBank?: () => void;
  onEditBank?: (bank: QuestionBank) => void;
  onDeleteBank?: (bank: QuestionBank) => void;
  onViewBank?: (bank: QuestionBank) => void;
  loading?: boolean;
  error?: string | null;
}

const QuestionBankManager: React.FC<QuestionBankManagerProps> = ({
  questionBanks,
  onAddBank,
  onEditBank,
  onDeleteBank,
  onViewBank,
  loading = false,
  error = null,
}) => {
  const theme = useTheme();
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedStatus, setSelectedStatus] = useState<string>('all');

  const filteredBanks = questionBanks.filter(bank => {
    if (searchQuery && !bank.name.toLowerCase().includes(searchQuery.toLowerCase())) return false;
    if (selectedStatus !== 'all' && bank.status !== selectedStatus) return false;
    return true;
  });

  const stats = {
    total: questionBanks.length,
    active: questionBanks.filter(b => b.status === QuestionBankStatus.Active).length,
    inactive: questionBanks.filter(b => b.status === QuestionBankStatus.Inactive).length,
    totalQuestions: questionBanks.reduce((sum, b) => sum + (b.questionCount || 0), 0),
  };

  return (
    <Box>
      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 3 }}>
        <Box>
          <Typography variant="h4" sx={{ fontWeight: 600, mb: 1 }}>
            Question Banks
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Manage and organize your question collections
          </Typography>
        </Box>
        
        {onAddBank && (
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={onAddBank}
            size="large"
          >
            Create Question Bank
          </Button>
        )}
      </Box>

      {/* Stats */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Paper sx={{ p: 3, textAlign: 'center', bgcolor: 'primary.light', color: 'white' }}>
            <Typography variant="h4" sx={{ fontWeight: 600, mb: 1 }}>
              {stats.total}
            </Typography>
            <Typography variant="body2">Total Banks</Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Paper sx={{ p: 3, textAlign: 'center', bgcolor: 'success.light', color: 'white' }}>
            <Typography variant="h4" sx={{ fontWeight: 600, mb: 1 }}>
              {stats.active}
            </Typography>
            <Typography variant="body2">Active Banks</Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Paper sx={{ p: 3, textAlign: 'center', bgcolor: 'info.light', color: 'white' }}>
            <Typography variant="h4" sx={{ fontWeight: 600, mb: 1 }}>
              {stats.totalQuestions}
            </Typography>
            <Typography variant="body2">Total Questions</Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Paper sx={{ p: 3, textAlign: 'center', bgcolor: 'warning.light', color: 'white' }}>
                         <Typography variant="h4" sx={{ fontWeight: 600, mb: 1 }}>
               {stats.inactive}
             </Typography>
             <Typography variant="body2">Inactive Banks</Typography>
          </Paper>
        </Grid>
      </Grid>

      {/* Search and Filters */}
      <Paper sx={{ p: 3, mb: 3, bgcolor: 'background.default' }}>
        <Grid container spacing={3} alignItems="center">
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              placeholder="Search question banks..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              InputProps={{
                startAdornment: <SearchIcon color="action" />,
              }}
            />
          </Grid>
          
          <Grid item xs={12} md={3}>
            <FormControl fullWidth size="small">
              <InputLabel>Status</InputLabel>
              <Select
                value={selectedStatus}
                onChange={(e) => setSelectedStatus(e.target.value)}
                label="Status"
              >
                <MenuItem value="all">All Statuses</MenuItem>
                <MenuItem value={QuestionBankStatus.Active}>Active</MenuItem>
                                 <MenuItem value={QuestionBankStatus.Inactive}>Inactive</MenuItem>
                <MenuItem value={QuestionBankStatus.Archived}>Archived</MenuItem>
              </Select>
            </FormControl>
          </Grid>
        </Grid>
      </Paper>

      {/* Content */}
      {loading ? (
        <Typography>Loading question banks...</Typography>
      ) : error ? (
        <Typography color="error">{error}</Typography>
      ) : filteredBanks.length === 0 ? (
        <Paper sx={{ p: 6, textAlign: 'center' }}>
          <Typography variant="h6" color="text.secondary" sx={{ mb: 2 }}>
            No question banks found
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            {searchQuery || selectedStatus !== 'all'
              ? 'Try adjusting your search criteria or filters'
              : 'Create your first question bank to get started'}
          </Typography>
          {onAddBank && (
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={onAddBank}
            >
              Create Question Bank
            </Button>
          )}
        </Paper>
      ) : (
        <Grid container spacing={3}>
          {filteredBanks.map((bank) => (
            <Grid item xs={12} sm={6} md={4} lg={3} key={bank.id}>
              <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
                <CardContent sx={{ flexGrow: 1 }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      {bank.status === QuestionBankStatus.Active ? (
                        <FolderOpenIcon color="primary" />
                      ) : (
                        <FolderIcon color="action" />
                      )}
                      <Typography variant="h6" sx={{ fontWeight: 600 }}>
                        {bank.name}
                      </Typography>
                    </Box>
                  </Box>

                  <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                    {bank.description || 'No description available'}
                  </Typography>

                  <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, mb: 2 }}>
                    <Chip
                      label={bank.status}
                      size="small"
                      color={
                        bank.status === QuestionBankStatus.Active ? 'success' :
                        bank.status === QuestionBankStatus.Inactive ? 'warning' :
                        'default'
                      }
                    />
                                         {bank.isPublic ? (
                       <Chip label="Public" size="small" color="info" icon={<PublicIcon />} />
                     ) : (
                       <Chip label="Private" size="small" color="default" icon={<LockIcon />} />
                     )}
                     {bank.categoryName && (
                       <Chip label={bank.categoryName} size="small" variant="outlined" />
                     )}
                  </Box>

                  <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                         <Typography variant="body2" color="text.secondary">
                       {bank.questionCount || 0} questions
                     </Typography>
                    <Typography variant="caption" color="text.secondary">
                      {new Date(bank.updatedAt).toLocaleDateString()}
                    </Typography>
                  </Box>
                </CardContent>

                <CardActions sx={{ pt: 0 }}>
                  <Button
                    size="small"
                    onClick={() => onViewBank?.(bank)}
                    startIcon={<VisibilityIcon />}
                  >
                    View
                  </Button>
                  {onEditBank && (
                    <Button
                      size="small"
                      onClick={() => onEditBank(bank)}
                      startIcon={<EditIcon />}
                    >
                      Edit
                    </Button>
                  )}
                </CardActions>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
    </Box>
  );
};

export default QuestionBankManager;
