import React, { useState, useCallback } from 'react';
import {
  Box,
  Paper,
  Typography,
  Button,
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  Chip,
  FormControlLabel,
  Switch,
  Divider,
  useTheme,
  Alert,
  LinearProgress,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  IconButton,
  Tooltip,
} from '@mui/material';
import {
  Upload as UploadIcon,
  Download as DownloadIcon,
  FileUpload as FileUploadIcon,
  FileDownload as FileDownloadIcon,
  CheckCircle as CheckCircleIcon,
  Error as ErrorIcon,
  Warning as WarningIcon,
  Info as InfoIcon,
  Delete as DeleteIcon,
  Refresh as RefreshIcon,
  Settings as SettingsIcon,
} from '@mui/icons-material';

import { 
  QuestionImportBatch, 
  ImportError, 
  QuestionExportRequest, 
  ImportFormat, 
  ExportFormat,
  QuestionBank 
} from '../types';
import { QUESTION_API_ENDPOINTS } from '../constants';

interface QuestionImportExportProps {
  questionBanks: QuestionBank[];
  onImport: (file: File, options: ImportOptions) => Promise<QuestionImportBatch>;
  onExport: (request: QuestionExportRequest) => Promise<void>;
  onCancelImport?: (batchId: string) => void;
  onRetryImport?: (batchId: string) => void;
  importHistory?: QuestionImportBatch[];
  loading?: boolean;
  error?: string | null;
}

interface ImportOptions {
  format: ImportFormat;
  questionBankId?: number;
  overwriteExisting: boolean;
  validateBeforeImport: boolean;
  createQuestionBank: boolean;
  newQuestionBankName?: string;
  newQuestionBankDescription?: string;
}

const QuestionImportExport: React.FC<QuestionImportExportProps> = ({
  questionBanks,
  onImport,
  onExport,
  onCancelImport,
  onRetryImport,
  importHistory = [],
  loading = false,
  error = null,
}) => {
  const theme = useTheme();

  // State
  const [activeTab, setActiveTab] = useState<'import' | 'export'>('import');
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [importOptions, setImportOptions] = useState<ImportOptions>({
    format: ImportFormat.Excel,
    questionBankId: undefined,
    overwriteExisting: false,
    validateBeforeImport: true,
    createQuestionBank: false,
    newQuestionBankName: '',
    newQuestionBankDescription: '',
  });

  const [exportOptions, setExportOptions] = useState<QuestionExportRequest>({
    questionBankId: 0,
    format: ExportFormat.Excel,
    includeMetadata: true,
    includeAnswers: true,
    includeExplanations: true,
    includeValidation: true,
    dateRange: null,
    questionTypes: [],
    difficulties: [],
    statuses: [],
    tags: [],
  });

  const [dragActive, setDragActive] = useState(false);

  // Event handlers
  const handleFileSelect = useCallback((file: File) => {
    setSelectedFile(file);
    
    // Auto-detect format based on file extension
    const extension = file.name.split('.').pop()?.toLowerCase();
    if (extension === 'xlsx' || extension === 'xls') {
      setImportOptions(prev => ({ ...prev, format: ImportFormat.Excel }));
    } else if (extension === 'csv') {
      setImportOptions(prev => ({ ...prev, format: ImportFormat.CSV }));
    } else if (extension === 'json') {
      setImportOptions(prev => ({ ...prev, format: ImportFormat.JSON }));
    }
  }, []);

  const handleDragOver = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    setDragActive(true);
  }, []);

  const handleDragLeave = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    setDragActive(false);
  }, []);

  const handleDrop = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    setDragActive(false);
    
    const files = Array.from(e.dataTransfer.files);
    if (files.length > 0) {
      handleFileSelect(files[0]);
    }
  }, [handleFileSelect]);

  const handleFileInputChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      handleFileSelect(files[0]);
    }
  }, [handleFileSelect]);

  const handleImport = useCallback(async () => {
    if (!selectedFile) return;

    try {
      await onImport(selectedFile, importOptions);
      setSelectedFile(null);
      setImportOptions({
        format: ImportFormat.Excel,
        questionBankId: undefined,
        overwriteExisting: false,
        validateBeforeImport: true,
        createQuestionBank: false,
        newQuestionBankName: '',
        newQuestionBankDescription: '',
      });
    } catch (error) {
      console.error('Import failed:', error);
    }
  }, [selectedFile, importOptions, onImport]);

  const handleExport = useCallback(async () => {
    if (exportOptions.questionBankId === 0) return;

    try {
      await onExport(exportOptions);
    } catch (error) {
      console.error('Export failed:', error);
    }
  }, [exportOptions, onExport]);

  const handleImportOptionChange = useCallback((field: keyof ImportOptions, value: any) => {
    setImportOptions(prev => ({
      ...prev,
      [field]: value,
    }));
  }, []);

  const handleExportOptionChange = useCallback((field: keyof QuestionExportRequest, value: any) => {
    setExportOptions(prev => ({
      ...prev,
      [field]: value,
    }));
  }, []);

  // Render functions
  const renderImportSection = () => (
    <Box>
      {/* File Upload */}
      <Paper 
        sx={{ 
          p: 4, 
          mb: 3, 
          border: `2px dashed ${dragActive ? theme.palette.primary.main : theme.palette.divider}`,
          bgcolor: dragActive ? 'primary.50' : 'background.default',
          transition: 'all 0.2s ease',
          cursor: 'pointer',
        }}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        onClick={() => document.getElementById('file-input')?.click()}
      >
        <input
          id="file-input"
          type="file"
          accept=".xlsx,.xls,.csv,.json,.xml,.qti"
          onChange={handleFileInputChange}
          style={{ display: 'none' }}
        />
        
        <Box sx={{ textAlign: 'center' }}>
          <FileUploadIcon sx={{ fontSize: 48, color: 'primary.main', mb: 2 }} />
          <Typography variant="h6" sx={{ mb: 1 }}>
            {selectedFile ? selectedFile.name : 'Drop files here or click to browse'}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Supported formats: Excel (.xlsx, .xls), CSV (.csv), JSON (.json), XML (.xml), QTI (.qti)
          </Typography>
          {selectedFile && (
            <Chip
              label={`${selectedFile.name} (${(selectedFile.size / 1024 / 1024).toFixed(2)} MB)`}
              color="primary"
              variant="outlined"
              onDelete={() => setSelectedFile(null)}
            />
          )}
        </Box>
      </Paper>

      {/* Import Options */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
          <SettingsIcon />
          Import Options
        </Typography>

        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            <FormControl fullWidth>
              <InputLabel>File Format</InputLabel>
              <Select
                value={importOptions.format}
                onChange={(e) => handleImportOptionChange('format', e.target.value)}
                label="File Format"
                disabled={loading}
              >
                <MenuItem value={ImportFormat.Excel}>Excel (.xlsx, .xls)</MenuItem>
                <MenuItem value={ImportFormat.CSV}>CSV (.csv)</MenuItem>
                <MenuItem value={ImportFormat.JSON}>JSON (.json)</MenuItem>
                <MenuItem value={ImportFormat.XML}>XML (.xml)</MenuItem>
                <MenuItem value={ImportFormat.QTI}>QTI (.qti)</MenuItem>
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={12} md={6}>
            <FormControl fullWidth>
              <InputLabel>Question Bank</InputLabel>
              <Select
                value={importOptions.questionBankId || ''}
                onChange={(e) => handleImportOptionChange('questionBankId', e.target.value)}
                label="Question Bank"
                disabled={loading || importOptions.createQuestionBank}
              >
                <MenuItem value="">Select a question bank</MenuItem>
                {questionBanks.map((bank) => (
                  <MenuItem key={bank.id} value={bank.id}>
                    {bank.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={12}>
            <FormControlLabel
              control={
                <Switch
                  checked={importOptions.createQuestionBank}
                  onChange={(e) => handleImportOptionChange('createQuestionBank', e.target.checked)}
                  disabled={loading}
                />
              }
              label="Create new question bank"
            />
          </Grid>

          {importOptions.createQuestionBank && (
            <>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="New Question Bank Name"
                  value={importOptions.newQuestionBankName}
                  onChange={(e) => handleImportOptionChange('newQuestionBankName', e.target.value)}
                  disabled={loading}
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="Description"
                  value={importOptions.newQuestionBankDescription}
                  onChange={(e) => handleImportOptionChange('newQuestionBankDescription', e.target.value)}
                  disabled={loading}
                />
              </Grid>
            </>
          )}

          <Grid item xs={12}>
            <FormControlLabel
              control={
                <Switch
                  checked={importOptions.validateBeforeImport}
                  onChange={(e) => handleImportOptionChange('validateBeforeImport', e.target.checked)}
                  disabled={loading}
                />
              }
              label="Validate questions before import"
            />
          </Grid>

          <Grid item xs={12}>
            <FormControlLabel
              control={
                <Switch
                  checked={importOptions.overwriteExisting}
                  onChange={(e) => handleImportOptionChange('overwriteExisting', e.target.checked)}
                  disabled={loading}
                />
              }
              label="Overwrite existing questions with same ID"
            />
          </Grid>
        </Grid>

        <Box sx={{ display: 'flex', gap: 2, mt: 3, justifyContent: 'flex-end' }}>
          <Button
            variant="outlined"
            onClick={() => setSelectedFile(null)}
            disabled={loading || !selectedFile}
          >
            Clear
          </Button>
          <Button
            variant="contained"
            onClick={handleImport}
            disabled={loading || !selectedFile || (!importOptions.questionBankId && !importOptions.createQuestionBank)}
            startIcon={<UploadIcon />}
          >
            {loading ? 'Importing...' : 'Import Questions'}
          </Button>
        </Box>
      </Paper>

      {/* Import History */}
      {importHistory.length > 0 && (
        <Paper sx={{ p: 3 }}>
          <Typography variant="h6" sx={{ mb: 2 }}>
            Import History
          </Typography>
          <List>
            {importHistory.map((batch) => (
              <ListItem key={batch.id} divider>
                <ListItemIcon>
                  {batch.status === 'Completed' ? (
                    <CheckCircleIcon color="success" />
                  ) : batch.status === 'Failed' ? (
                    <ErrorIcon color="error" />
                  ) : batch.status === 'InProgress' ? (
                    <InfoIcon color="info" />
                  ) : (
                    <WarningIcon color="warning" />
                  )}
                </ListItemIcon>
                <ListItemText
                  primary={`${batch.fileName} - ${batch.status}`}
                  secondary={`${batch.totalQuestions} questions, ${batch.successCount} successful, ${batch.errorCount} errors`}
                />
                <Box sx={{ display: 'flex', gap: 1 }}>
                  {batch.status === 'InProgress' && (
                    <LinearProgress sx={{ width: 100 }} />
                  )}
                  {batch.status === 'Failed' && onRetryImport && (
                    <Tooltip title="Retry Import">
                      <IconButton
                        size="small"
                        onClick={() => onRetryImport(batch.id)}
                        disabled={loading}
                      >
                        <RefreshIcon />
                      </IconButton>
                    </Tooltip>
                  )}
                  {onCancelImport && (
                    <Tooltip title="Cancel Import">
                      <IconButton
                        size="small"
                        onClick={() => onCancelImport(batch.id)}
                        disabled={loading || batch.status !== 'InProgress'}
                      >
                        <DeleteIcon />
                      </IconButton>
                    </Tooltip>
                  )}
                </Box>
              </ListItem>
            ))}
          </List>
        </Paper>
      )}
    </Box>
  );

  const renderExportSection = () => (
    <Box>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
          <FileDownloadIcon />
          Export Options
        </Typography>

        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            <FormControl fullWidth>
              <InputLabel>Question Bank *</InputLabel>
              <Select
                value={exportOptions.questionBankId}
                onChange={(e) => handleExportOptionChange('questionBankId', e.target.value)}
                label="Question Bank *"
                disabled={loading}
              >
                <MenuItem value={0}>Select a question bank</MenuItem>
                {questionBanks.map((bank) => (
                  <MenuItem key={bank.id} value={bank.id}>
                    {bank.name} ({bank.questionCount} questions)
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={12} md={6}>
            <FormControl fullWidth>
              <InputLabel>Export Format</InputLabel>
              <Select
                value={exportOptions.format}
                onChange={(e) => handleExportOptionChange('format', e.target.value)}
                label="Export Format"
                disabled={loading}
              >
                <MenuItem value={ExportFormat.Excel}>Excel (.xlsx)</MenuItem>
                <MenuItem value={ExportFormat.CSV}>CSV (.csv)</MenuItem>
                <MenuItem value={ExportFormat.JSON}>JSON (.json)</MenuItem>
                <MenuItem value={ExportFormat.XML}>XML (.xml)</MenuItem>
                <MenuItem value={ExportFormat.QTI}>QTI (.qti)</MenuItem>
                <MenuItem value={ExportFormat.PDF}>PDF (.pdf)</MenuItem>
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={12}>
            <Typography variant="subtitle2" sx={{ mb: 2 }}>
              Include in Export:
            </Typography>
            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
              <FormControlLabel
                control={
                  <Switch
                    checked={exportOptions.includeMetadata}
                    onChange={(e) => handleExportOptionChange('includeMetadata', e.target.checked)}
                    disabled={loading}
                  />
                }
                label="Question Metadata"
              />
              <FormControlLabel
                control={
                  <Switch
                    checked={exportOptions.includeAnswers}
                    onChange={(e) => handleExportOptionChange('includeAnswers', e.target.checked)}
                    disabled={loading}
                  />
                }
                label="Correct Answers"
              />
              <FormControlLabel
                control={
                  <Switch
                    checked={exportOptions.includeExplanations}
                    onChange={(e) => handleExportOptionChange('includeExplanations', e.target.checked)}
                    disabled={loading}
                  />
                }
                label="Explanations"
              />
              <FormControlLabel
                control={
                  <Switch
                    checked={exportOptions.includeValidation}
                    onChange={(e) => handleExportOptionChange('includeValidation', e.target.checked)}
                    disabled={loading}
                  />
                }
                label="Validation Rules"
              />
            </Box>
          </Grid>
        </Grid>

        <Box sx={{ display: 'flex', gap: 2, mt: 3, justifyContent: 'flex-end' }}>
          <Button
            variant="contained"
            onClick={handleExport}
            disabled={loading || exportOptions.questionBankId === 0}
            startIcon={<DownloadIcon />}
          >
            {loading ? 'Exporting...' : 'Export Questions'}
          </Button>
        </Box>
      </Paper>
    </Box>
  );

  return (
    <Box>
      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 3 }}>
        <Box>
          <Typography variant="h4" sx={{ fontWeight: 600, mb: 1 }}>
            Question Import & Export
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Import questions from external sources or export existing questions
          </Typography>
        </Box>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}

      {/* Tab Navigation */}
      <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Button
            variant={activeTab === 'import' ? 'contained' : 'outlined'}
            onClick={() => setActiveTab('import')}
            startIcon={<UploadIcon />}
          >
            Import Questions
          </Button>
          <Button
            variant={activeTab === 'export' ? 'contained' : 'outlined'}
            onClick={() => setActiveTab('export')}
            startIcon={<DownloadIcon />}
          >
            Export Questions
          </Button>
        </Box>
      </Box>

      {/* Content */}
      {activeTab === 'import' ? renderImportSection() : renderExportSection()}
    </Box>
  );
};

export default QuestionImportExport;
