import React, { useState, useCallback, useRef, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  Chip,
  Alert,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Divider,
  Tooltip,
  IconButton
} from '@mui/material';
import {
  DragIndicator,
  Folder,
  Assignment,
  TrendingUp,
  CheckCircle,
  Cancel,
  Info,
  Warning,
  Error
} from '@mui/icons-material';
import { QuestionBankCategoryDto, MoveCategoryDto } from '../types/questionBankTree.types';

interface TreeDragDropProps {
  categories: QuestionBankCategoryDto[];
  onCategoryMove?: (categoryId: number, moveData: MoveCategoryDto) => Promise<boolean>;
  onReorder?: (parentId: number, nodeOrders: Record<number, number>) => Promise<boolean>;
  readonly?: boolean;
  className?: string;
  maxDepth?: number;
}

interface DragState {
  isDragging: boolean;
  draggedCategory: QuestionBankCategoryDto | null;
  dragOverCategory: QuestionBankCategoryDto | null;
  dropPosition: 'before' | 'after' | 'inside' | null;
  dragStartY: number;
}

interface DropZone {
  category: QuestionBankCategoryDto;
  position: 'before' | 'after' | 'inside';
  rect: DOMRect;
}

export const TreeDragDrop: React.FC<TreeDragDropProps> = ({
  categories,
  onCategoryMove,
  onReorder,
  readonly = false,
  className = "",
  maxDepth = 6
}) => {
  const [dragState, setDragState] = useState<DragState>({
    isDragging: false,
    draggedCategory: null,
    dragOverCategory: null,
    dropPosition: null,
    dragStartY: 0
  });

  const [dropZones, setDropZones] = useState<DropZone[]>([]);
  const [showMoveDialog, setShowMoveDialog] = useState(false);
  const [pendingMove, setPendingMove] = useState<{
    category: QuestionBankCategoryDto;
    targetCategory: QuestionBankCategoryDto;
    position: 'before' | 'after' | 'inside';
  } | null>(null);

  const containerRef = useRef<HTMLDivElement>(null);
  const dragPreviewRef = useRef<HTMLDivElement>(null);

  // Calculate drop zones for all categories
  useEffect(() => {
    if (!containerRef.current) return;

    const calculateDropZones = () => {
      const zones: DropZone[] = [];
      const categoryElements = containerRef.current?.querySelectorAll('[data-category-id]');

      categoryElements?.forEach((element) => {
        const categoryId = parseInt(element.getAttribute('data-category-id') || '0');
        const category = categories.find(c => c.id === categoryId);
        if (!category) return;

        const rect = element.getBoundingClientRect();
        const elementHeight = rect.height;

        // Before zone (top 25% of element)
        zones.push({
          category,
          position: 'before',
          rect: new DOMRect(rect.x, rect.y, rect.width, elementHeight * 0.25)
        });

        // After zone (bottom 25% of element)
        zones.push({
          category,
          position: 'after',
          rect: new DOMRect(rect.x, rect.y + elementHeight * 0.75, rect.width, elementHeight * 0.25)
        });

        // Inside zone (middle 50% of element)
        zones.push({
          category,
          position: 'inside',
          rect: new DOMRect(rect.x, rect.y + elementHeight * 0.25, rect.width, elementHeight * 0.5)
        });
      });

      setDropZones(zones);
    };

    calculateDropZones();
    window.addEventListener('resize', calculateDropZones);
    return () => window.removeEventListener('resize', calculateDropZones);
  }, [categories]);

  const handleDragStart = useCallback((event: React.DragEvent, category: QuestionBankCategoryDto) => {
    if (readonly) return;

    event.dataTransfer.effectAllowed = 'move';
    event.dataTransfer.setData('text/plain', category.id.toString());

    setDragState(prev => ({
      ...prev,
      isDragging: true,
      draggedCategory: category,
      dragStartY: event.clientY
    }));

    // Create drag preview
    if (dragPreviewRef.current) {
      dragPreviewRef.current.style.display = 'block';
      dragPreviewRef.current.style.left = `${event.clientX}px`;
      dragPreviewRef.current.style.top = `${event.clientY}px`;
    }
  }, [readonly]);

  const handleDragEnd = useCallback(() => {
    setDragState({
      isDragging: false,
      draggedCategory: null,
      dragOverCategory: null,
      dropPosition: null,
      dragStartY: 0
    });

    if (dragPreviewRef.current) {
      dragPreviewRef.current.style.display = 'none';
    }
  }, []);

  const handleDragOver = useCallback((event: React.DragEvent) => {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';

    if (!dragState.isDragging || !dragState.draggedCategory) return;

    const mouseY = event.clientY;
    let closestZone: DropZone | null = null;
    let minDistance = Infinity;

    // Find the closest drop zone
    dropZones.forEach(zone => {
      const zoneCenterY = zone.rect.y + zone.rect.height / 2;
      const distance = Math.abs(mouseY - zoneCenterY);

      if (distance < minDistance) {
        minDistance = distance;
        closestZone = zone;
      }
    });

    if (closestZone && closestZone.category.id !== dragState.draggedCategory.id) {
      // Validate drop operation
      const isValidDrop = validateDropOperation(
        dragState.draggedCategory,
        closestZone.category,
        closestZone.position
      );

      if (isValidDrop) {
        setDragState(prev => ({
          ...prev,
          dragOverCategory: closestZone!.category,
          dropPosition: closestZone!.position
        }));
      }
    }
  }, [dragState.isDragging, dragState.draggedCategory, dropZones]);

  const handleDrop = useCallback((event: React.DragEvent) => {
    event.preventDefault();

    if (!dragState.draggedCategory || !dragState.dragOverCategory || !dragState.dropPosition) {
      return;
    }

    // Validate final drop operation
    const isValidDrop = validateDropOperation(
      dragState.draggedCategory,
      dragState.dragOverCategory,
      dragState.dropPosition
    );

    if (!isValidDrop) {
      return;
    }

    // Set pending move for confirmation
    setPendingMove({
      category: dragState.draggedCategory,
      targetCategory: dragState.dragOverCategory,
      position: dragState.dropPosition
    });

    setShowMoveDialog(true);
  }, [dragState.draggedCategory, dragState.dragOverCategory, dragState.dropPosition]);

  const validateDropOperation = useCallback((
    draggedCategory: QuestionBankCategoryDto,
    targetCategory: QuestionBankCategoryDto,
    position: 'before' | 'after' | 'inside'
  ): boolean => {
    // Prevent dropping on itself
    if (draggedCategory.id === targetCategory.id) {
      return false;
    }

    // Prevent dropping on descendants (would create circular reference)
    if (isDescendant(draggedCategory, targetCategory)) {
      return false;
    }

    // Check depth constraints
    if (position === 'inside') {
      const newDepth = targetCategory.level + 1;
      if (newDepth > maxDepth) {
        return false;
      }
    }

    return true;
  }, [maxDepth]);

  const isDescendant = useCallback((parent: QuestionBankCategoryDto, child: QuestionBankCategoryDto): boolean => {
    if (parent.id === child.id) return false;
    
    const checkDescendants = (category: QuestionBankCategoryDto): boolean => {
      if (!category.children) return false;
      
      for (const descendant of category.children) {
        if (descendant.id === child.id) return true;
        if (checkDescendants(descendant)) return true;
      }
      
      return false;
    };

    return checkDescendants(parent);
  }, []);

  const handleConfirmMove = useCallback(async () => {
    if (!pendingMove || !onCategoryMove) return;

    try {
      const moveData: MoveCategoryDto = {
        newParentId: pendingMove.position === 'inside' ? pendingMove.targetCategory.id : pendingMove.targetCategory.parentId,
        newSortOrder: calculateNewSortOrder(pendingMove.targetCategory, pendingMove.position),
        position: pendingMove.position
      };

      const success = await onCategoryMove(pendingMove.category.id, moveData);
      
      if (success) {
        setShowMoveDialog(false);
        setPendingMove(null);
      }
    } catch (error) {
      console.error('Error moving category:', error);
    }
  }, [pendingMove, onCategoryMove]);

  const calculateNewSortOrder = useCallback((
    targetCategory: QuestionBankCategoryDto,
    position: 'before' | 'after' | 'inside'
  ): number => {
    if (position === 'inside') {
      // Place at the end of children
      const children = targetCategory.children || [];
      return children.length > 0 ? Math.max(...children.map(c => c.sortOrder)) + 1 : 0;
    }

    // Place before or after the target
    const siblings = getSiblings(targetCategory);
    const targetIndex = siblings.findIndex(c => c.id === targetCategory.id);
    
    if (position === 'before') {
      return targetIndex > 0 ? siblings[targetIndex - 1].sortOrder + 1 : 0;
    } else {
      return targetIndex < siblings.length - 1 ? siblings[targetIndex + 1].sortOrder - 1 : targetCategory.sortOrder + 1;
    }
  }, []);

  const getSiblings = useCallback((category: QuestionBankCategoryDto): QuestionBankCategoryDto[] => {
    if (!category.parentId) {
      return categories.filter(c => !c.parentId);
    }
    
    return categories.filter(c => c.parentId === category.parentId);
  }, [categories]);

  const getDropZoneStyle = useCallback((zone: DropZone) => {
    if (!dragState.isDragging || 
        !dragState.draggedCategory || 
        dragState.draggedCategory.id === zone.category.id) {
      return { display: 'none' };
    }

    const isActive = dragState.dragOverCategory?.id === zone.category.id && 
                    dragState.dropPosition === zone.position;

    return {
      position: 'absolute' as const,
      left: `${zone.rect.x}px`,
      top: `${zone.rect.y}px`,
      width: `${zone.rect.width}px`,
      height: `${zone.rect.height}px`,
      backgroundColor: isActive ? 'primary.main' : 'transparent',
      border: `2px dashed ${isActive ? 'primary.main' : 'grey.300'}`,
      borderRadius: 1,
      pointerEvents: 'none' as const,
      zIndex: 1000,
      opacity: isActive ? 0.8 : 0.3,
      transition: 'all 0.2s ease'
    };
  }, [dragState]);

  const getCategoryIcon = useCallback((type: string) => {
    switch (type) {
      case 'Subject': return <Folder color="primary" />;
      case 'Chapter': return <Folder color="secondary" />;
      case 'Topic': return <Assignment color="success" />;
      case 'Subtopic': return <Assignment color="info" />;
      case 'Skill': return <TrendingUp color="warning" />;
      case 'Objective': return <TrendingUp color="error" />;
      default: return <Folder />;
    }
  }, []);

  const renderCategory = useCallback((category: QuestionBankCategoryDto, level: number = 0) => {
    const isDragging = dragState.draggedCategory?.id === category.id;
    const isDropTarget = dragState.dragOverCategory?.id === category.id;

    return (
      <Box
        key={category.id}
        data-category-id={category.id}
        sx={{
          position: 'relative',
          opacity: isDragging ? 0.5 : 1,
          transform: isDragging ? 'rotate(5deg)' : 'none',
          transition: 'all 0.2s ease',
          mb: 1
        }}
      >
        <Paper
          draggable={!readonly}
          onDragStart={(e) => handleDragStart(e, category)}
          onDragEnd={handleDragEnd}
          sx={{
            p: 2,
            ml: level * 3,
            cursor: readonly ? 'default' : 'grab',
            backgroundColor: isDropTarget ? 'action.hover' : 'background.paper',
            border: isDropTarget ? 2 : 1,
            borderColor: isDropTarget ? 'primary.main' : 'divider',
            '&:hover': {
              backgroundColor: readonly ? 'background.paper' : 'action.hover'
            },
            '&:active': {
              cursor: readonly ? 'default' : 'grabbing'
            }
          }}
        >
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            {!readonly && (
              <DragIndicator 
                sx={{ 
                  color: 'action.disabled',
                  cursor: 'grab',
                  '&:hover': { color: 'action.active' }
                }} 
              />
            )}
            
            {getCategoryIcon(category.type)}
            
            <Box sx={{ flex: 1 }}>
              <Typography variant="body2" fontWeight={500}>
                {category.name}
              </Typography>
              {category.code && (
                <Typography variant="caption" color="text.secondary" fontFamily="monospace">
                  {category.code}
                </Typography>
              )}
            </Box>

            <Chip
              label={category.type}
              size="small"
              variant="outlined"
              color="primary"
            />

            {category.questionCount > 0 && (
              <Chip
                label={`${category.questionCount} questions`}
                size="small"
                variant="outlined"
                color="success"
              />
            )}
          </Box>

          {/* Render children recursively */}
          {category.children && category.children.length > 0 && (
            <Box sx={{ mt: 2 }}>
              {category.children
                .sort((a, b) => a.sortOrder - b.sortOrder)
                .map(child => renderCategory(child, level + 1))}
            </Box>
          )}
        </Paper>
      </Box>
    );
  }, [dragState, readonly, handleDragStart, handleDragEnd, getCategoryIcon]);

  return (
    <Box className={`tree-drag-drop ${className}`}>
      {/* Drop Zones Overlay */}
      {dropZones.map((zone, index) => (
        <Box
          key={`${zone.category.id}-${zone.position}-${index}`}
          sx={getDropZoneStyle(zone)}
        />
      ))}

      {/* Drag Preview */}
      <Box
        ref={dragPreviewRef}
        sx={{
          position: 'fixed',
          display: 'none',
          pointerEvents: 'none',
          zIndex: 1001,
          backgroundColor: 'background.paper',
          border: '2px solid primary.main',
          borderRadius: 1,
          p: 1,
          boxShadow: 3,
          maxWidth: 200
        }}
      >
        {dragState.draggedCategory && (
          <Typography variant="body2" noWrap>
            {dragState.draggedCategory.name}
          </Typography>
        )}
      </Box>

      {/* Main Container */}
      <Box
        ref={containerRef}
        onDragOver={handleDragOver}
        onDrop={handleDrop}
        sx={{
          minHeight: 200,
          position: 'relative'
        }}
      >
        {categories.length === 0 ? (
          <Paper sx={{ p: 4, textAlign: 'center' }}>
            <Typography variant="body1" color="text.secondary">
              No categories available for drag and drop
            </Typography>
          </Paper>
        ) : (
          categories
            .filter(c => !c.parentId) // Only root categories
            .sort((a, b) => a.sortOrder - b.sortOrder)
            .map(category => renderCategory(category))
        )}
      </Box>

      {/* Move Confirmation Dialog */}
      <Dialog open={showMoveDialog} onClose={() => setShowMoveDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Confirm Category Move</DialogTitle>
        <DialogContent>
          {pendingMove && (
            <Box>
              <Alert severity="info" sx={{ mb: 2 }}>
                You are about to move a category. This action will reorganize the tree structure.
              </Alert>

              <List>
                <ListItem>
                  <ListItemIcon>
                    <DragIndicator color="primary" />
                  </ListItemIcon>
                  <ListItemText
                    primary="Moving Category"
                    secondary={pendingMove.category.name}
                  />
                </ListItem>

                <Divider />

                <ListItem>
                  <ListItemIcon>
                    {pendingMove.position === 'inside' ? (
                      <Folder color="success" />
                    ) : (
                      <Assignment color="info" />
                    )}
                  </ListItemIcon>
                  <ListItemText
                    primary={`${pendingMove.position === 'inside' ? 'Into' : pendingMove.position === 'before' ? 'Before' : 'After'}`}
                    secondary={pendingMove.targetCategory.name}
                  />
                </ListItem>
              </List>

              <Box sx={{ mt: 2 }}>
                <Typography variant="body2" color="text.secondary">
                  <strong>New Location:</strong> {pendingMove.position === 'inside' ? 
                    `Inside "${pendingMove.targetCategory.name}"` : 
                    `${pendingMove.position === 'before' ? 'Before' : 'After'} "${pendingMove.targetCategory.name}"`
                  }
                </Typography>
              </Box>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setShowMoveDialog(false)}>
            Cancel
          </Button>
          <Button 
            onClick={handleConfirmMove} 
            variant="contained" 
            color="primary"
          >
            Confirm Move
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};
