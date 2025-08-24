// Question Bank Tree Component
// Simplified version for frontend foundation

import React, { useState } from 'react';
import { 
  Box, 
  Typography, 
  Button, 
  Paper,
  CircularProgress
} from '@mui/material';

// Simplified types for foundation
interface QuestionBankTreeProps {
  onCategorySelect?: (category: any) => void;
  onQuestionSelect?: (question: any) => void;
  selectedCategoryId?: number;
  readonly?: boolean;
  showQuestions?: boolean;
  enableDragDrop?: boolean;
  maxDepth?: number;
  className?: string;
  initialExpandedNodes?: number[];
  searchEnabled?: boolean;
  filterEnabled?: boolean;
}

export const QuestionBankTree: React.FC<QuestionBankTreeProps> = ({
  onCategorySelect,
  onQuestionSelect,
  selectedCategoryId,
  readonly = false,
  showQuestions = true,
  enableDragDrop = false,
  maxDepth = 5,
  className = '',
  initialExpandedNodes = [],
  searchEnabled = true,
  filterEnabled = true
}) => {
  const [isLoading, setIsLoading] = useState(false);

  // Placeholder implementation for foundation
  const handleCategorySelect = (category: any) => {
    if (onCategorySelect) {
      onCategorySelect(category);
    }
  };

  const handleQuestionSelect = (question: any) => {
    if (onQuestionSelect) {
      onQuestionSelect(question);
    }
  };

  return (
    <Box className={className}>
      <Paper elevation={1} sx={{ p: 2 }}>
        <Typography variant="h6" gutterBottom>
          Question Bank Tree
        </Typography>
        
        <Typography variant="body2" color="text.secondary" gutterBottom>
          Foundation component - Tree functionality to be implemented
        </Typography>

        {isLoading ? (
          <Box display="flex" justifyContent="center" p={2}>
            <CircularProgress size={24} />
          </Box>
        ) : (
          <Box>
            <Typography variant="body2" gutterBottom>
              • Selected Category ID: {selectedCategoryId || 'None'}
            </Typography>
            <Typography variant="body2" gutterBottom>
              • Readonly: {readonly ? 'Yes' : 'No'}
            </Typography>
            <Typography variant="body2" gutterBottom>
              • Show Questions: {showQuestions ? 'Yes' : 'No'}
            </Typography>
            <Typography variant="body2" gutterBottom>
              • Enable Drag & Drop: {enableDragDrop ? 'Yes' : 'No'}
            </Typography>
            <Typography variant="body2" gutterBottom>
              • Max Depth: {maxDepth}
            </Typography>
            <Typography variant="body2" gutterBottom>
              • Search Enabled: {searchEnabled ? 'Yes' : 'No'}
            </Typography>
            <Typography variant="body2" gutterBottom>
              • Filter Enabled: {filterEnabled ? 'Yes' : 'No'}
            </Typography>
            <Typography variant="body2" gutterBottom>
              • Initial Expanded Nodes: {initialExpandedNodes.length}
            </Typography>
          </Box>
        )}

        <Box mt={2}>
          <Button 
            variant="outlined" 
            size="small"
            onClick={() => handleCategorySelect({ id: 1, name: 'Sample Category' })}
            disabled={readonly}
          >
            Select Sample Category
          </Button>
          
          <Button 
            variant="outlined" 
            size="small"
            sx={{ ml: 1 }}
            onClick={() => handleQuestionSelect({ id: 1, title: 'Sample Question' })}
            disabled={!showQuestions}
          >
            Select Sample Question
          </Button>
        </Box>
      </Paper>
    </Box>
  );
};
