import React, { useState, useEffect, useCallback } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  Box,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  FormControlLabel,
  Switch,
  Typography,
  Grid,
  Chip,
  Alert,
  CircularProgress
} from '@mui/material';
import { Close, Save, Add, Edit } from '@mui/icons-material';
import { 
  QuestionBankCategoryDto, 
  CreateCategoryDto, 
  UpdateCategoryDto,
  CategoryType,
  CategoryLevel
} from '../types/questionBankTree.types';

interface CategoryManagerProps {
  open: boolean;
  onClose: () => void;
  category?: QuestionBankCategoryDto | null;
  parentCategoryId?: number | null;
  onCategoryCreated?: (parentId: number | null, category: QuestionBankCategoryDto) => void;
  onCategoryUpdated?: (categoryId: number, category: QuestionBankCategoryDto) => void;
  isEditing?: boolean;
}

const CATEGORY_TYPES: { value: CategoryType; label: string; description: string }[] = [
  { value: 'Subject', label: 'Subject', description: 'Main academic subject area' },
  { value: 'Chapter', label: 'Chapter', description: 'Major section within a subject' },
  { value: 'Topic', label: 'Topic', description: 'Specific topic or concept' },
  { value: 'Subtopic', label: 'Subtopic', description: 'Detailed subtopic' },
  { value: 'Skill', label: 'Skill', description: 'Specific skill or competency' },
  { value: 'Objective', label: 'Objective', description: 'Learning objective or outcome' }
];

const CATEGORY_LEVELS: { value: CategoryLevel; label: string; description: string }[] = [
  { value: 1, label: 'Level 1', description: 'Root level category' },
  { value: 2, label: 'Level 2', description: 'Second level category' },
  { value: 3, label: 'Level 3', description: 'Third level category' },
  { value: 4, label: 'Level 4', description: 'Fourth level category' },
  { value: 5, label: 'Level 5', description: 'Fifth level category' },
  { value: 6, label: 'Level 6', description: 'Sixth level category' }
];

export const CategoryManager: React.FC<CategoryManagerProps> = ({
  open,
  onClose,
  category,
  parentCategoryId,
  onCategoryCreated,
  onCategoryUpdated,
  isEditing = false
}) => {
  const [formData, setFormData] = useState<CreateCategoryDto>({
    name: '',
    code: '',
    description: '',
    type: 'Topic',
    level: 1,
    parentId: null,
    sortOrder: 0,
    isActive: true,
    allowQuestions: true,
    metadataJson: '',
    curriculumCode: '',
    gradeLevel: '',
    subject: ''
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  // Initialize form data when editing
  useEffect(() => {
    if (category && isEditing) {
      setFormData({
        name: category.name,
        code: category.code,
        description: category.description || '',
        type: category.type,
        level: category.level,
        parentId: category.parentId,
        sortOrder: category.sortOrder,
        isActive: category.isActive,
        allowQuestions: category.allowQuestions,
        metadataJson: category.metadataJson || '',
        curriculumCode: category.curriculumCode || '',
        gradeLevel: category.gradeLevel || '',
        subject: category.subject || ''
      });
    } else if (parentCategoryId !== undefined) {
      // Creating new category under a parent
      setFormData(prev => ({
        ...prev,
        parentId: parentCategoryId,
        level: 2 // Default to level 2 for subcategories
      }));
    }
  }, [category, isEditing, parentCategoryId]);

  const validateForm = useCallback((): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Category name is required';
    } else if (formData.name.length > 200) {
      newErrors.name = 'Category name must be 200 characters or less';
    }

    if (!formData.code.trim()) {
      newErrors.code = 'Category code is required';
    } else if (formData.code.length > 50) {
      newErrors.code = 'Category code must be 50 characters or less';
    } else if (!/^[A-Z0-9_-]+$/.test(formData.code)) {
      newErrors.code = 'Category code must contain only uppercase letters, numbers, hyphens, and underscores';
    }

    if (formData.description && formData.description.length > 500) {
      newErrors.description = 'Description must be 500 characters or less';
    }

    if (formData.curriculumCode && formData.curriculumCode.length > 100) {
      newErrors.curriculumCode = 'Curriculum code must be 100 characters or less';
    }

    if (formData.gradeLevel && formData.gradeLevel.length > 100) {
      newErrors.gradeLevel = 'Grade level must be 100 characters or less';
    }

    if (formData.subject && formData.subject.length > 100) {
      newErrors.subject = 'Subject must be 100 characters or less';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  }, [formData]);

  const handleInputChange = useCallback((field: keyof CreateCategoryDto, value: any) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }));
    }
  }, [errors]);

  const handleSubmit = useCallback(async () => {
    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);
    setSubmitError(null);

    try {
      if (isEditing && category) {
        // Update existing category
        const updateData: UpdateCategoryDto = {
          name: formData.name,
          code: formData.code,
          description: formData.description,
          type: formData.type,
          level: formData.level,
          sortOrder: formData.sortOrder,
          isActive: formData.isActive,
          allowQuestions: formData.allowQuestions,
          metadataJson: formData.metadataJson,
          curriculumCode: formData.curriculumCode,
          gradeLevel: formData.gradeLevel,
          subject: formData.subject
        };

        if (onCategoryUpdated) {
          await onCategoryUpdated(category.id, updateData as any);
        }
      } else {
        // Create new category
        if (onCategoryCreated) {
          await onCategoryCreated(parentCategoryId, formData);
        }
      }

      onClose();
    } catch (error) {
      setSubmitError(error instanceof Error ? error.message : 'An error occurred while saving the category');
    } finally {
      setIsSubmitting(false);
    }
  }, [validateForm, isEditing, category, formData, onCategoryCreated, onCategoryUpdated, parentCategoryId, onClose]);

  const handleClose = useCallback(() => {
    if (!isSubmitting) {
      setFormData({
        name: '',
        code: '',
        description: '',
        type: 'Topic',
        level: 1,
        parentId: null,
        sortOrder: 0,
        isActive: true,
        allowQuestions: true,
        metadataJson: '',
        curriculumCode: '',
        gradeLevel: '',
        subject: ''
      });
      setErrors({});
      setSubmitError(null);
      onClose();
    }
  }, [isSubmitting, onClose]);

  const getTitle = () => {
    if (isEditing) return 'Edit Category';
    if (parentCategoryId) return 'Create Subcategory';
    return 'Create Category';
  };

  const getSubmitButtonText = () => {
    if (isSubmitting) return 'Saving...';
    return isEditing ? 'Update Category' : 'Create Category';
  };

  return (
    <Dialog
      open={open}
      onClose={handleClose}
      maxWidth="md"
      fullWidth
      PaperProps={{
        sx: { borderRadius: 2 }
      }}
    >
      <DialogTitle sx={{ 
        display: 'flex', 
        alignItems: 'center', 
        justifyContent: 'space-between',
        pb: 1
      }}>
        <Box sx={{ display: 'flex', alignItems: 'center' }}>
          {isEditing ? <Edit sx={{ mr: 1 }} /> : <Add sx={{ mr: 1 }} />}
          <Typography variant="h6">{getTitle()}</Typography>
        </Box>
        <Button
          onClick={handleClose}
          disabled={isSubmitting}
          sx={{ minWidth: 'auto', p: 1 }}
        >
          <Close />
        </Button>
      </DialogTitle>

      <DialogContent>
        {submitError && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {submitError}
          </Alert>
        )}

        <Grid container spacing={3}>
          {/* Basic Information */}
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="Category Name *"
              value={formData.name}
              onChange={(e) => handleInputChange('name', e.target.value)}
              error={!!errors.name}
              helperText={errors.name || 'Enter a descriptive name for the category'}
              disabled={isSubmitting}
              sx={{ mb: 2 }}
            />
          </Grid>

          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="Category Code *"
              value={formData.code}
              onChange={(e) => handleInputChange('code', e.target.value.toUpperCase())}
              error={!!errors.code}
              helperText={errors.code || 'Enter a unique code (e.g., MATH001)'}
              disabled={isSubmitting}
              sx={{ mb: 2 }}
            />
          </Grid>

          <Grid item xs={12}>
            <TextField
              fullWidth
              label="Description"
              value={formData.description}
              onChange={(e) => handleInputChange('description', e.target.value)}
              error={!!errors.description}
              helperText={errors.description || 'Optional description of the category'}
              disabled={isSubmitting}
              multiline
              rows={3}
              sx={{ mb: 2 }}
            />
          </Grid>

          {/* Category Configuration */}
          <Grid item xs={12} md={6}>
            <FormControl fullWidth error={!!errors.type} disabled={isSubmitting}>
              <InputLabel>Category Type *</InputLabel>
              <Select
                value={formData.type}
                onChange={(e) => handleInputChange('type', e.target.value)}
                label="Category Type *"
              >
                {CATEGORY_TYPES.map((type) => (
                  <MenuItem key={type.value} value={type.value}>
                    <Box>
                      <Typography variant="body2">{type.label}</Typography>
                      <Typography variant="caption" color="text.secondary">
                        {type.description}
                      </Typography>
                    </Box>
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={12} md={6}>
            <FormControl fullWidth error={!!errors.level} disabled={isSubmitting}>
              <InputLabel>Category Level *</InputLabel>
              <Select
                value={formData.level}
                onChange={(e) => handleInputChange('level', e.target.value)}
                label="Category Level *"
              >
                {CATEGORY_LEVELS.map((level) => (
                  <MenuItem key={level.value} value={level.value}>
                    <Box>
                      <Typography variant="body2">{level.label}</Typography>
                      <Typography variant="caption" color="text.secondary">
                        {level.description}
                      </Typography>
                    </Box>
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          {/* Curriculum Alignment */}
          <Grid item xs={12} md={4}>
            <TextField
              fullWidth
              label="Curriculum Code"
              value={formData.curriculumCode}
              onChange={(e) => handleInputChange('curriculumCode', e.target.value)}
              error={!!errors.curriculumCode}
              helperText={errors.curriculumCode || 'Optional curriculum identifier'}
              disabled={isSubmitting}
              sx={{ mb: 2 }}
            />
          </Grid>

          <Grid item xs={12} md={4}>
            <TextField
              fullWidth
              label="Grade Level"
              value={formData.gradeLevel}
              onChange={(e) => handleInputChange('gradeLevel', e.target.value)}
              error={!!errors.gradeLevel}
              helperText={errors.gradeLevel || 'Optional grade level'}
              disabled={isSubmitting}
              sx={{ mb: 2 }}
            />
          </Grid>

          <Grid item xs={12} md={4}>
            <TextField
              fullWidth
              label="Subject"
              value={formData.subject}
              onChange={(e) => handleInputChange('subject', e.target.value)}
              error={!!errors.subject}
              helperText={errors.subject || 'Optional subject area'}
              disabled={isSubmitting}
              sx={{ mb: 2 }}
            />
          </Grid>

          {/* Options */}
          <Grid item xs={12}>
            <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
              <FormControlLabel
                control={
                  <Switch
                    checked={formData.isActive}
                    onChange={(e) => handleInputChange('isActive', e.target.checked)}
                    disabled={isSubmitting}
                  />
                }
                label="Active Category"
              />

              <FormControlLabel
                control={
                  <Switch
                    checked={formData.allowQuestions}
                    onChange={(e) => handleInputChange('allowQuestions', e.target.checked)}
                    disabled={isSubmitting}
                  />
                }
                label="Allow Questions"
              />
            </Box>
          </Grid>

          {/* Metadata */}
          <Grid item xs={12}>
            <TextField
              fullWidth
              label="Metadata (JSON)"
              value={formData.metadataJson}
              onChange={(e) => handleInputChange('metadataJson', e.target.value)}
              error={!!errors.metadataJson}
              helperText={errors.metadataJson || 'Optional JSON metadata for additional properties'}
              disabled={isSubmitting}
              multiline
              rows={2}
              sx={{ mb: 2 }}
            />
          </Grid>
        </Grid>
      </DialogContent>

      <DialogActions sx={{ p: 2, pt: 1 }}>
        <Button
          onClick={handleClose}
          disabled={isSubmitting}
          variant="outlined"
        >
          Cancel
        </Button>
        
        <Button
          onClick={handleSubmit}
          disabled={isSubmitting}
          variant="contained"
          startIcon={isSubmitting ? <CircularProgress size={16} /> : <Save />}
        >
          {getSubmitButtonText()}
        </Button>
      </DialogActions>
    </Dialog>
  );
};
