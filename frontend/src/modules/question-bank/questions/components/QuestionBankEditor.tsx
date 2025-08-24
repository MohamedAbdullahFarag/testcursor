import React, { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Chip,
  FormControlLabel,
  Switch,
  Divider,
  IconButton,
  useTheme,
  Alert,
} from '@mui/material';
import {
  Save as SaveIcon,
  Cancel as CancelIcon,
  Add as AddIcon,
  Delete as DeleteIcon,
  Folder as FolderIcon,
} from '@mui/icons-material';

import { QuestionBank, QuestionBankStatus } from '../types';

interface QuestionBankEditorProps {
  questionBank?: QuestionBank;
  onSave: (bank: Partial<QuestionBank>) => void;
  onCancel: () => void;
  loading?: boolean;
  error?: string | null;
}

const QuestionBankEditor: React.FC<QuestionBankEditorProps> = ({
  questionBank,
  onSave,
  onCancel,
  loading = false,
  error = null,
}) => {
  const theme = useTheme();
  const isEditing = !!questionBank;

  // Form state
  const [formData, setFormData] = useState({
    name: questionBank?.name || '',
    description: questionBank?.description || '',
    categoryId: questionBank?.categoryId || 0,
    status: questionBank?.status || QuestionBankStatus.Draft,
    isPublic: questionBank?.isPublic || false,
    tags: questionBank?.tags || [],
    subject: questionBank?.metadata?.subject || '',
    grade: questionBank?.metadata?.grade || '',
    curriculum: questionBank?.metadata?.curriculum || '',
    academicYear: questionBank?.metadata?.academicYear || '',
    semester: questionBank?.metadata?.semester || '',
    estimatedDuration: questionBank?.metadata?.estimatedDuration || 0,
    maxAttempts: questionBank?.metadata?.maxAttempts || 1,
  });

  const [newTag, setNewTag] = useState('');
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});

  // Categories (mock data - would come from API)
  const categories = [
    { id: 1, name: 'Mathematics' },
    { id: 2, name: 'Science' },
    { id: 3, name: 'History' },
    { id: 4, name: 'Language Arts' },
    { id: 5, name: 'Geography' },
  ];

  // Grades (mock data)
  const grades = [
    'Kindergarten', '1st Grade', '2nd Grade', '3rd Grade', '4th Grade',
    '5th Grade', '6th Grade', '7th Grade', '8th Grade', '9th Grade',
    '10th Grade', '11th Grade', '12th Grade', 'College', 'University'
  ];

  // Academic years (mock data)
  const academicYears = [
    '2023-2024', '2024-2025', '2025-2026', '2026-2027', '2027-2028'
  ];

  // Semesters (mock data)
  const semesters = ['Fall', 'Spring', 'Summer', 'Full Year'];

  // Event handlers
  const handleInputChange = (field: string, value: any) => {
    setFormData(prev => ({
      ...prev,
      [field]: value,
    }));

    // Clear validation error when user starts typing
    if (validationErrors[field]) {
      setValidationErrors(prev => ({
        ...prev,
        [field]: '',
      }));
    }
  };

  const handleAddTag = () => {
    if (newTag.trim() && !formData.tags.includes(newTag.trim())) {
      setFormData(prev => ({
        ...prev,
        tags: [...prev.tags, newTag.trim()],
      }));
      setNewTag('');
    }
  };

  const handleRemoveTag = (tagToRemove: string) => {
    setFormData(prev => ({
      ...prev,
      tags: prev.tags.filter(tag => tag !== tagToRemove),
    }));
  };

  const handleKeyPress = (event: React.KeyboardEvent) => {
    if (event.key === 'Enter') {
      event.preventDefault();
      handleAddTag();
    }
  };

  const validateForm = () => {
    const errors: Record<string, string> = {};

    if (!formData.name.trim()) {
      errors.name = 'Question bank name is required';
    }

    if (formData.name.trim().length < 3) {
      errors.name = 'Question bank name must be at least 3 characters';
    }

    if (formData.name.trim().length > 100) {
      errors.name = 'Question bank name must be less than 100 characters';
    }

    if (formData.description && formData.description.length > 500) {
      errors.description = 'Description must be less than 500 characters';
    }

    if (formData.categoryId === 0) {
      errors.categoryId = 'Please select a category';
    }

    if (formData.estimatedDuration < 0) {
      errors.estimatedDuration = 'Estimated duration cannot be negative';
    }

    if (formData.maxAttempts < 1) {
      errors.maxAttempts = 'Maximum attempts must be at least 1';
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSave = () => {
    if (!validateForm()) {
      return;
    }

    const bankData: Partial<QuestionBank> = {
      name: formData.name.trim(),
      description: formData.description.trim() || undefined,
      categoryId: formData.categoryId,
      status: formData.status,
      isPublic: formData.isPublic,
      tags: formData.tags,
      metadata: {
        subject: formData.subject || undefined,
        grade: formData.grade || undefined,
        curriculum: formData.curriculum || undefined,
        academicYear: formData.academicYear || undefined,
        semester: formData.semester || undefined,
        estimatedDuration: formData.estimatedDuration || undefined,
        maxAttempts: formData.maxAttempts || undefined,
      },
    };

    onSave(bankData);
  };

  const handleCancel = () => {
    onCancel();
  };

  // Reset form when questionBank prop changes
  useEffect(() => {
    if (questionBank) {
      setFormData({
        name: questionBank.name,
        description: questionBank.description || '',
        categoryId: questionBank.categoryId,
        status: questionBank.status,
        isPublic: questionBank.isPublic,
        tags: questionBank.tags,
        subject: questionBank.metadata?.subject || '',
        grade: questionBank.metadata?.grade || '',
        curriculum: questionBank.metadata?.curriculum || '',
        academicYear: questionBank.metadata?.academicYear || '',
        semester: questionBank.metadata?.semester || '',
        estimatedDuration: questionBank.metadata?.estimatedDuration || 0,
        maxAttempts: questionBank.metadata?.maxAttempts || 1,
      });
    }
  }, [questionBank]);

  return (
    <Paper sx={{ p: 4, maxWidth: 800, mx: 'auto' }}>
      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
        <FolderIcon color="primary" fontSize="large" />
        <Box>
          <Typography variant="h4" sx={{ fontWeight: 600 }}>
            {isEditing ? 'Edit Question Bank' : 'Create Question Bank'}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {isEditing ? 'Update the question bank details' : 'Set up a new question bank'}
          </Typography>
        </Box>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}

      {/* Basic Information */}
      <Typography variant="h6" sx={{ mb: 2, color: 'primary.main' }}>
        Basic Information
      </Typography>
      
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12}>
          <TextField
            fullWidth
            label="Question Bank Name *"
            value={formData.name}
            onChange={(e) => handleInputChange('name', e.target.value)}
            error={!!validationErrors.name}
            helperText={validationErrors.name || 'Enter a descriptive name for the question bank'}
            disabled={loading}
          />
        </Grid>

        <Grid item xs={12}>
          <TextField
            fullWidth
            label="Description"
            value={formData.description}
            onChange={(e) => handleInputChange('description', e.target.value)}
            multiline
            rows={3}
            error={!!validationErrors.description}
            helperText={validationErrors.description || 'Optional description of the question bank'}
            disabled={loading}
          />
        </Grid>

        <Grid item xs={12} md={6}>
          <FormControl fullWidth error={!!validationErrors.categoryId}>
            <InputLabel>Category *</InputLabel>
            <Select
              value={formData.categoryId}
              onChange={(e) => handleInputChange('categoryId', e.target.value)}
              label="Category *"
              disabled={loading}
            >
              <MenuItem value={0}>Select a category</MenuItem>
              {categories.map((category) => (
                <MenuItem key={category.id} value={category.id}>
                  {category.name}
                </MenuItem>
              ))}
            </Select>
            {validationErrors.categoryId && (
              <Typography variant="caption" color="error" sx={{ mt: 1, display: 'block' }}>
                {validationErrors.categoryId}
              </Typography>
            )}
          </FormControl>
        </Grid>

        <Grid item xs={12} md={6}>
          <FormControl fullWidth>
            <InputLabel>Status</InputLabel>
            <Select
              value={formData.status}
              onChange={(e) => handleInputChange('status', e.target.value)}
              label="Status"
              disabled={loading}
            >
              <MenuItem value={QuestionBankStatus.Draft}>Draft</MenuItem>
              <MenuItem value={QuestionBankStatus.Active}>Active</MenuItem>
              <MenuItem value={QuestionBankStatus.Inactive}>Inactive</MenuItem>
              <MenuItem value={QuestionBankStatus.Archived}>Archived</MenuItem>
            </Select>
          </FormControl>
        </Grid>

        <Grid item xs={12}>
          <FormControlLabel
            control={
              <Switch
                checked={formData.isPublic}
                onChange={(e) => handleInputChange('isPublic', e.target.checked)}
                disabled={loading}
              />
            }
            label="Make this question bank public"
          />
          <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 1 }}>
            Public question banks can be accessed by other users
          </Typography>
        </Grid>
      </Grid>

      <Divider sx={{ my: 4 }} />

      {/* Academic Information */}
      <Typography variant="h6" sx={{ mb: 2, color: 'primary.main' }}>
        Academic Information
      </Typography>
      
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            label="Subject"
            value={formData.subject}
            onChange={(e) => handleInputChange('subject', e.target.value)}
            placeholder="e.g., Mathematics, Science"
            disabled={loading}
          />
        </Grid>

        <Grid item xs={12} md={6}>
          <FormControl fullWidth>
            <InputLabel>Grade Level</InputLabel>
            <Select
              value={formData.grade}
              onChange={(e) => handleInputChange('grade', e.target.value)}
              label="Grade Level"
              disabled={loading}
            >
              <MenuItem value="">Select grade level</MenuItem>
              {grades.map((grade) => (
                <MenuItem key={grade} value={grade}>
                  {grade}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>

        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            label="Curriculum"
            value={formData.curriculum}
            onChange={(e) => handleInputChange('curriculum', e.target.value)}
            placeholder="e.g., Common Core, IB, AP"
            disabled={loading}
          />
        </Grid>

        <Grid item xs={12} md={6}>
          <FormControl fullWidth>
            <InputLabel>Academic Year</InputLabel>
            <Select
              value={formData.academicYear}
              onChange={(e) => handleInputChange('academicYear', e.target.value)}
              label="Academic Year"
              disabled={loading}
            >
              <MenuItem value="">Select academic year</MenuItem>
              {academicYears.map((year) => (
                <MenuItem key={year} value={year}>
                  {year}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>

        <Grid item xs={12} md={6}>
          <FormControl fullWidth>
            <InputLabel>Semester</InputLabel>
            <Select
              value={formData.semester}
              onChange={(e) => handleInputChange('semester', e.target.value)}
              label="Semester"
              disabled={loading}
            >
              <MenuItem value="">Select semester</MenuItem>
              {semesters.map((semester) => (
                <MenuItem key={semester} value={semester}>
                  {semester}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>

        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            type="number"
            label="Estimated Duration (minutes)"
            value={formData.estimatedDuration}
            onChange={(e) => handleInputChange('estimatedDuration', parseInt(e.target.value) || 0)}
            error={!!validationErrors.estimatedDuration}
            helperText={validationErrors.estimatedDuration || 'Estimated time to complete all questions'}
            disabled={loading}
          />
        </Grid>

        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            type="number"
            label="Maximum Attempts"
            value={formData.maxAttempts}
            onChange={(e) => handleInputChange('maxAttempts', parseInt(e.target.value) || 1)}
            error={!!validationErrors.maxAttempts}
            helperText={validationErrors.maxAttempts || 'Maximum number of attempts allowed'}
            disabled={loading}
          />
        </Grid>
      </Grid>

      <Divider sx={{ my: 4 }} />

      {/* Tags */}
      <Typography variant="h6" sx={{ mb: 2, color: 'primary.main' }}>
        Tags
      </Typography>
      
      <Box sx={{ mb: 3 }}>
        <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
          <TextField
            fullWidth
            label="Add Tag"
            value={newTag}
            onChange={(e) => setNewTag(e.target.value)}
            onKeyPress={handleKeyPress}
            placeholder="Enter a tag and press Enter"
            disabled={loading}
          />
          <Button
            variant="outlined"
            onClick={handleAddTag}
            disabled={!newTag.trim() || loading}
            startIcon={<AddIcon />}
          >
            Add
          </Button>
        </Box>

        {formData.tags.length > 0 && (
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
            {formData.tags.map((tag, index) => (
              <Chip
                key={index}
                label={tag}
                onDelete={() => handleRemoveTag(tag)}
                disabled={loading}
                color="primary"
                variant="outlined"
              />
            ))}
          </Box>
        )}
      </Box>

      {/* Actions */}
      <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end', pt: 2 }}>
        <Button
          variant="outlined"
          onClick={handleCancel}
          disabled={loading}
          startIcon={<CancelIcon />}
        >
          Cancel
        </Button>
        <Button
          variant="contained"
          onClick={handleSave}
          disabled={loading}
          startIcon={<SaveIcon />}
        >
          {loading ? 'Saving...' : (isEditing ? 'Update Bank' : 'Create Bank')}
        </Button>
      </Box>
    </Paper>
  );
};

export default QuestionBankEditor;
