import React, { useState, useCallback } from 'react';
import { 
  TreeItem, 
  TreeItemContent, 
  TreeItemGroup 
} from '@mui/x-tree-view';
import { 
  Folder, 
  FolderOpen, 
  Assignment, 
  MoreVert, 
  Add, 
  Edit, 
  Delete, 
  DragIndicator 
} from '@mui/icons-material';
import { 
  IconButton, 
  Menu, 
  MenuItem, 
  Typography, 
  Box, 
  Chip,
  Tooltip,
  Badge
} from '@mui/material';
import { QuestionBankCategoryDto, CreateCategoryDto, UpdateCategoryDto } from '../types/questionBankTree.types';

interface TreeNodeProps {
  category: QuestionBankCategoryDto;
  isSelected: boolean;
  isExpanded: boolean;
  readonly?: boolean;
  showQuestions?: boolean;
  enableDragDrop?: boolean;
  searchQuery?: string;
  onCategoryCreate: (parentId: number | null, categoryData: CreateCategoryDto) => Promise<void>;
  onCategoryUpdate: (categoryId: number, categoryData: UpdateCategoryDto) => Promise<void>;
  onCategoryDelete: (categoryId: number) => Promise<boolean>;
  onCategoryMove: (categoryId: number, newParentId: number | null) => Promise<boolean>;
  onQuestionAssign: (questionId: number, categoryId: number, isPrimary?: boolean) => Promise<boolean>;
  onQuestionRemove: (questionId: number, categoryId: number) => Promise<boolean>;
}

export const TreeNode: React.FC<TreeNodeProps> = ({
  category,
  isSelected,
  isExpanded,
  readonly = false,
  showQuestions = true,
  enableDragDrop = false,
  searchQuery = '',
  onCategoryCreate,
  onCategoryUpdate,
  onCategoryDelete,
  onCategoryMove,
  onQuestionAssign,
  onQuestionRemove
}) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [showEditForm, setShowEditForm] = useState(false);

  const handleMenuOpen = useCallback((event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  }, []);

  const handleMenuClose = useCallback(() => {
    setAnchorEl(null);
  }, []);

  const handleCreateSubcategory = useCallback(() => {
    setShowCreateForm(true);
    handleMenuClose();
  }, []);

  const handleEditCategory = useCallback(() => {
    setShowEditForm(true);
    handleMenuClose();
  }, []);

  const handleDeleteCategory = useCallback(async () => {
    if (window.confirm(`Are you sure you want to delete "${category.name}"? This action cannot be undone.`)) {
      try {
        await onCategoryDelete(category.id);
      } catch (error) {
        console.error('Error deleting category:', error);
      }
    }
    handleMenuClose();
  }, [category.id, category.name, onCategoryDelete]);

  const handleMoveCategory = useCallback(async (newParentId: number | null) => {
    try {
      await onCategoryMove(category.id, newParentId);
    } catch (error) {
      console.error('Error moving category:', error);
    }
    handleMenuClose();
  }, [category.id, onCategoryMove]);

  const isSearchMatch = searchQuery && 
    (category.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
     category.code.toLowerCase().includes(searchQuery.toLowerCase()) ||
     category.description?.toLowerCase().includes(searchQuery.toLowerCase()));

  const getCategoryIcon = () => {
    if (category.type === 'Subject') return isExpanded ? <FolderOpen /> : <Folder />;
    if (category.type === 'Chapter') return <Folder />;
    if (category.type === 'Topic') return <Assignment />;
    return <Folder />;
  };

  const getCategoryColor = () => {
    switch (category.type) {
      case 'Subject': return 'primary';
      case 'Chapter': return 'secondary';
      case 'Topic': return 'success';
      case 'Subtopic': return 'info';
      case 'Skill': return 'warning';
      case 'Objective': return 'error';
      default: return 'default';
    }
  };

  const renderCategoryContent = () => (
    <Box
      sx={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        width: '100%',
        py: 0.5,
        px: 1,
        backgroundColor: isSelected ? 'action.selected' : 'transparent',
        borderRadius: 1,
        '&:hover': {
          backgroundColor: 'action.hover',
        },
      }}
    >
      <Box sx={{ display: 'flex', alignItems: 'center', flex: 1, minWidth: 0 }}>
        {enableDragDrop && (
          <DragIndicator 
            sx={{ 
              mr: 1, 
              cursor: 'grab',
              color: 'action.disabled',
              '&:hover': { color: 'action.active' }
            }} 
          />
        )}
        
        <Box sx={{ mr: 1, color: 'primary.main' }}>
          {getCategoryIcon()}
        </Box>
        
        <Box sx={{ minWidth: 0, flex: 1 }}>
          <Typography
            variant="body2"
            sx={{
              fontWeight: category.level === 1 ? 600 : 400,
              color: isSearchMatch ? 'error.main' : 'text.primary',
              textDecoration: isSearchMatch ? 'underline' : 'none',
            }}
            noWrap
          >
            {category.name}
          </Typography>
          
          {category.code && (
            <Typography
              variant="caption"
              sx={{ 
                color: 'text.secondary',
                display: 'block',
                fontFamily: 'monospace'
              }}
            >
              {category.code}
            </Typography>
          )}
        </Box>
      </Box>

      <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
        {/* Question Count Badge */}
        {showQuestions && category.questionCount > 0 && (
          <Tooltip title={`${category.questionCount} questions`}>
            <Badge
              badgeContent={category.questionCount}
              color="primary"
              sx={{ '& .MuiBadge-badge': { fontSize: '0.7rem' } }}
            >
              <Assignment sx={{ fontSize: '1rem', color: 'action.disabled' }} />
            </Badge>
          </Tooltip>
        )}

        {/* Category Type Chip */}
        <Chip
          label={category.type}
          size="small"
          color={getCategoryColor() as any}
          variant="outlined"
          sx={{ height: 20, fontSize: '0.7rem' }}
        />

        {/* Action Menu */}
        {!readonly && (
          <>
            <IconButton
              size="small"
              onClick={handleMenuOpen}
              sx={{ 
                opacity: 0.7,
                '&:hover': { opacity: 1 },
                color: 'action.active'
              }}
            >
              <MoreVert fontSize="small" />
            </IconButton>
            
            <Menu
              anchorEl={anchorEl}
              open={Boolean(anchorEl)}
              onClose={handleMenuClose}
              anchorOrigin={{
                vertical: 'bottom',
                horizontal: 'right',
              }}
              transformOrigin={{
                vertical: 'top',
                horizontal: 'right',
              }}
            >
              <MenuItem onClick={handleCreateSubcategory}>
                <Add sx={{ mr: 1, fontSize: '1rem' }} />
                Add Subcategory
              </MenuItem>
              
              <MenuItem onClick={handleEditCategory}>
                <Edit sx={{ mr: 1, fontSize: '1rem' }} />
                Edit Category
              </MenuItem>
              
              {category.parentId && (
                <MenuItem onClick={() => handleMoveCategory(null)}>
                  <Assignment sx={{ mr: 1, fontSize: '1rem' }} />
                  Move to Root
                </MenuItem>
              )}
              
              <MenuItem 
                onClick={handleDeleteCategory}
                sx={{ color: 'error.main' }}
              >
                <Delete sx={{ mr: 1, fontSize: '1rem' }} />
                Delete Category
              </MenuItem>
            </Menu>
          </>
        )}
      </Box>
    </Box>
  );

  return (
    <TreeItem
      itemId={`category-${category.id}`}
      label={renderCategoryContent()}
      sx={{
        '& .MuiTreeItem-content': {
          padding: 0,
          minHeight: 'auto',
        },
        '& .MuiTreeItem-group': {
          marginLeft: 2,
        },
      }}
    >
      {category.children && category.children.length > 0 && (
        <TreeItemGroup>
          {category.children
            .sort((a, b) => a.sortOrder - b.sortOrder)
            .map((child) => (
              <TreeNode
                key={`category-${child.id}`}
                category={child}
                isSelected={false}
                isExpanded={false}
                readonly={readonly}
                showQuestions={showQuestions}
                enableDragDrop={enableDragDrop}
                searchQuery={searchQuery}
                onCategoryCreate={onCategoryCreate}
                onCategoryUpdate={onCategoryUpdate}
                onCategoryDelete={onCategoryDelete}
                onCategoryMove={onCategoryMove}
                onQuestionAssign={onQuestionAssign}
                onQuestionRemove={onQuestionRemove}
              />
            ))}
        </TreeItemGroup>
      )}
      
      {/* TODO: Add CategoryCreator and CategoryEditor modals here */}
      {/* These will be implemented in separate components */}
    </TreeItem>
  );
};
