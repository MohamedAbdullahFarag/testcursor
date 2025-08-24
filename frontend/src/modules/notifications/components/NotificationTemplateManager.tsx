import React, { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Chip,
  Alert,
  CircularProgress,
  Tooltip,
  Divider
} from '@mui/material';
import {
  Add,
  Edit,
  Delete,
  ContentCopy,
  Notifications,
  Save,
  Cancel
} from '@mui/icons-material';
import { NotificationTemplate, NotificationType, NotificationChannel } from '../types';

interface NotificationTemplateManagerProps {
  className?: string;
  showCreateButton?: boolean;
  onTemplateChange?: (templates: NotificationTemplate[]) => void;
}

interface TemplateFormData {
  id?: number;
  name: string;
  title: string;
  message: string;
  notificationType: NotificationType;
  channel: NotificationChannel;
  isActive: boolean;
  variables: string[];
}

export const NotificationTemplateManager: React.FC<NotificationTemplateManagerProps> = ({
  className = '',
  showCreateButton = true,
  onTemplateChange
}) => {
  const [templates, setTemplates] = useState<NotificationTemplate[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingTemplate, setEditingTemplate] = useState<TemplateFormData | null>(null);
  const [formData, setFormData] = useState<TemplateFormData>({
    name: '',
    title: '',
    message: '',
    notificationType: NotificationType.SystemAlert,
    channel: NotificationChannel.Email,
    isActive: true,
    variables: []
  });

  // Mock data for demonstration - in real app, this would come from API
  useEffect(() => {
    setTemplates([
      {
        id: 1,
        name: 'Exam Reminder',
        title: 'Exam Reminder: {examName}',
        message: 'Your exam {examName} is scheduled for {examDate} at {examTime}. Please ensure you are prepared.',
        notificationType: NotificationType.ExamReminder,
        channel: NotificationChannel.Email,
        isActive: true,
        variables: ['examName', 'examDate', 'examTime'],
        createdAt: new Date().toISOString(),
        modifiedAt: new Date().toISOString()
      },
      {
        id: 2,
        name: 'Grading Complete',
        title: 'Grading Complete: {assignmentName}',
        message: 'Grading has been completed for {assignmentName}. Your score: {score}/{totalPoints}.',
        notificationType: NotificationType.GradingComplete,
        channel: NotificationChannel.InApp,
        isActive: true,
        variables: ['assignmentName', 'score', 'totalPoints'],
        createdAt: new Date().toISOString(),
        modifiedAt: new Date().toISOString()
      }
    ]);
  }, []);

  const handleCreateTemplate = () => {
    setEditingTemplate(null);
    setFormData({
      name: '',
      title: '',
      message: '',
      notificationType: NotificationType.SystemAlert,
      channel: NotificationChannel.Email,
      isActive: true,
      variables: []
    });
    setIsDialogOpen(true);
  };

  const handleEditTemplate = (template: NotificationTemplate) => {
    setEditingTemplate(template);
    setFormData({
      id: template.id,
      name: template.name,
      title: template.title,
      message: template.message,
      notificationType: template.notificationType,
      channel: template.channel,
      isActive: template.isActive,
      variables: template.variables
    });
    setIsDialogOpen(true);
  };

  const handleDeleteTemplate = (templateId: number) => {
    if (window.confirm('Are you sure you want to delete this template?')) {
      setTemplates(prev => prev.filter(t => t.id !== templateId));
      onTemplateChange?.(templates.filter(t => t.id !== templateId));
    }
  };

  const handleCopyTemplate = (template: NotificationTemplate) => {
    const newTemplate = {
      ...template,
      id: Date.now(),
      name: `${template.name} (Copy)`,
      createdAt: new Date().toISOString(),
      modifiedAt: new Date().toISOString()
    };
    setTemplates(prev => [...prev, newTemplate]);
    onTemplateChange?.([...templates, newTemplate]);
  };

  const handleSaveTemplate = () => {
    if (!formData.name || !formData.title || !formData.message) {
      setError('Please fill in all required fields');
      return;
    }

    if (editingTemplate) {
      // Update existing template
      setTemplates(prev => prev.map(t => 
        t.id === editingTemplate.id 
          ? { ...t, ...formData, modifiedAt: new Date().toISOString() }
          : t
      ));
    } else {
      // Create new template
      const newTemplate: NotificationTemplate = {
        id: Date.now(),
        ...formData,
        createdAt: new Date().toISOString(),
        modifiedAt: new Date().toISOString()
      };
      setTemplates(prev => [...prev, newTemplate]);
    }

    onTemplateChange?.(editingTemplate ? 
      templates.map(t => t.id === editingTemplate.id ? { ...t, ...formData, modifiedAt: new Date().toISOString() } : t) :
      [...templates, { id: Date.now(), ...formData, createdAt: new Date().toISOString(), modifiedAt: new Date().toISOString() }]
    );

    setIsDialogOpen(false);
    setError(null);
  };

  const handleCancel = () => {
    setIsDialogOpen(false);
    setEditingTemplate(null);
    setFormData({
      name: '',
      title: '',
      message: '',
      notificationType: NotificationType.SystemAlert,
      channel: NotificationChannel.Email,
      isActive: true,
      variables: []
    });
    setError(null);
  };

  const getNotificationTypeLabel = (type: NotificationType): string => {
    const labels: Record<NotificationType, string> = {
      [NotificationType.ExamReminder]: 'Exam Reminder',
      [NotificationType.ExamStart]: 'Exam Start',
      [NotificationType.ExamEnd]: 'Exam End',
      [NotificationType.GradingComplete]: 'Grading Complete',
      [NotificationType.DeadlineReminder]: 'Deadline Reminder',
      [NotificationType.SystemAlert]: 'System Alert',
      [NotificationType.UserWelcome]: 'Welcome',
      [NotificationType.PasswordReset]: 'Password Reset',
      [NotificationType.AccountActivation]: 'Account Activation',
      [NotificationType.RoleAssignment]: 'Role Assignment'
    };
    return labels[type] || 'Unknown';
  };

  const getChannelLabel = (channel: NotificationChannel): string => {
    switch (channel) {
      case NotificationChannel.Email:
        return 'Email';
      case NotificationChannel.Sms:
        return 'SMS';
      case NotificationChannel.Push:
        return 'Push';
      case NotificationChannel.InApp:
        return 'In-App';
      default:
        return 'Unknown';
    }
  };

  const formatDate = (dateString: string): string => {
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <Box className={className}>
      <Card>
        <CardContent>
          <Box display="flex" alignItems="center" justifyContent="space-between" mb={3}>
            <Box display="flex" alignItems="center">
              <Notifications sx={{ mr: 1, color: 'primary.main' }} />
              <Typography variant="h6" component="h2">
                Notification Templates
              </Typography>
            </Box>
            
            {showCreateButton && (
              <Button
                variant="contained"
                startIcon={<Add />}
                onClick={handleCreateTemplate}
              >
                Create Template
              </Button>
            )}
          </Box>

          {isLoading ? (
            <Box display="flex" justifyContent="center" p={3}>
              <CircularProgress />
            </Box>
          ) : (
            <TableContainer component={Paper} variant="outlined">
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Name</TableCell>
                    <TableCell>Type</TableCell>
                    <TableCell>Channel</TableCell>
                    <TableCell>Title</TableCell>
                    <TableCell>Variables</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell>Modified</TableCell>
                    <TableCell align="center">Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {templates.map((template) => (
                    <TableRow key={template.id}>
                      <TableCell>
                        <Typography variant="subtitle2" fontWeight="medium">
                          {template.name}
                        </Typography>
                      </TableCell>
                      <TableCell>
                        <Chip
                          label={getNotificationTypeLabel(template.notificationType)}
                          size="small"
                          color="primary"
                          variant="outlined"
                        />
                      </TableCell>
                      <TableCell>
                        <Chip
                          label={getChannelLabel(template.channel)}
                          size="small"
                          variant="outlined"
                        />
                      </TableCell>
                      <TableCell>
                        <Typography variant="body2" noWrap sx={{ maxWidth: 200 }}>
                          {template.title}
                        </Typography>
                      </TableCell>
                      <TableCell>
                        <Box display="flex" gap={0.5} flexWrap="wrap">
                          {template.variables.map((variable, index) => (
                            <Chip
                              key={index}
                              label={variable}
                              size="small"
                              variant="outlined"
                              color="secondary"
                            />
                          ))}
                        </Box>
                      </TableCell>
                      <TableCell>
                        <Chip
                          label={template.isActive ? 'Active' : 'Inactive'}
                          size="small"
                          color={template.isActive ? 'success' : 'default'}
                        />
                      </TableCell>
                      <TableCell>
                        <Typography variant="caption" color="text.secondary">
                          {formatDate(template.modifiedAt)}
                        </Typography>
                      </TableCell>
                      <TableCell align="center">
                        <Box display="flex" gap={0.5} justifyContent="center">
                          <Tooltip title="Edit Template">
                            <IconButton
                              size="small"
                              onClick={() => handleEditTemplate(template)}
                            >
                              <Edit fontSize="small" />
                            </IconButton>
                          </Tooltip>
                          
                          <Tooltip title="Copy Template">
                            <IconButton
                              size="small"
                              onClick={() => handleCopyTemplate(template)}
                            >
                              <ContentCopy fontSize="small" />
                            </IconButton>
                          </Tooltip>
                          
                          <Tooltip title="Delete Template">
                            <IconButton
                              size="small"
                              color="error"
                              onClick={() => handleDeleteTemplate(template.id)}
                            >
                              <Delete fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        </Box>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          )}
        </CardContent>
      </Card>

      {/* Template Form Dialog */}
      <Dialog
        open={isDialogOpen}
        onClose={handleCancel}
        maxWidth="md"
        fullWidth
        PaperProps={{
          sx: { borderRadius: 2 }
        }}
      >
        <DialogTitle>
          {editingTemplate ? 'Edit Template' : 'Create Template'}
        </DialogTitle>
        
        <DialogContent>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          <Box display="grid" gridTemplateColumns="1fr 1fr" gap={2} mt={1}>
            <TextField
              label="Template Name"
              value={formData.name}
              onChange={(e) => setFormData(prev => ({ ...prev, name: e.target.value }))}
              fullWidth
              required
            />
            
            <FormControl fullWidth required>
              <InputLabel>Notification Type</InputLabel>
              <Select
                value={formData.notificationType}
                onChange={(e) => setFormData(prev => ({ ...prev, notificationType: e.target.value as NotificationType }))}
                label="Notification Type"
              >
                {Object.values(NotificationType).filter(type => typeof type === 'number').map((type) => (
                  <MenuItem key={type} value={type}>
                    {getNotificationTypeLabel(type as NotificationType)}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            <FormControl fullWidth required>
              <InputLabel>Channel</InputLabel>
              <Select
                value={formData.channel}
                onChange={(e) => setFormData(prev => ({ ...prev, channel: e.target.value as NotificationChannel }))}
                label="Channel"
              >
                {Object.values(NotificationChannel).filter(channel => typeof channel === 'number').map((channel) => (
                  <MenuItem key={channel} value={channel}>
                    {getChannelLabel(channel as NotificationChannel)}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            <TextField
              label="Active"
              select
              value={formData.isActive}
              onChange={(e) => setFormData(prev => ({ ...prev, isActive: e.target.value === 'true' }))}
              fullWidth
            >
              <MenuItem value={true}>Active</MenuItem>
              <MenuItem value={false}>Inactive</MenuItem>
            </TextField>
          </Box>

          <TextField
            label="Title Template"
            value={formData.title}
            onChange={(e) => setFormData(prev => ({ ...prev, title: e.target.value }))}
            fullWidth
            required
            sx={{ mt: 2 }}
            helperText="Use {variableName} for dynamic content"
          />

          <TextField
            label="Message Template"
            value={formData.message}
            onChange={(e) => setFormData(prev => ({ ...prev, message: e.target.value }))}
            fullWidth
            required
            multiline
            rows={4}
            sx={{ mt: 2 }}
            helperText="Use {variableName} for dynamic content"
          />

          <TextField
            label="Variables (comma-separated)"
            value={formData.variables.join(', ')}
            onChange={(e) => setFormData(prev => ({ 
              ...prev, 
              variables: e.target.value.split(',').map(v => v.trim()).filter(v => v)
            }))}
            fullWidth
            sx={{ mt: 2 }}
            helperText="List the variables used in the template (e.g., examName, examDate)"
          />
        </DialogContent>

        <DialogActions sx={{ p: 2, pt: 1 }}>
          <Button variant="outlined" onClick={handleCancel}>
            Cancel
          </Button>
          <Button
            variant="contained"
            onClick={handleSaveTemplate}
            startIcon={<Save />}
          >
            {editingTemplate ? 'Update' : 'Create'} Template
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};
