import React, { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Switch,
  FormControlLabel,
  FormGroup,
  Divider,
  Button,
  Alert,
  CircularProgress,
  Chip,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Grid,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel
} from '@mui/material';
import { ExpandMore, Notifications, Email, Sms, PushPin } from '@mui/icons-material';
import { useNotificationPreferences } from '../hooks/useNotificationPreferences';
import { NotificationPreferenceDto, NotificationChannel, NotificationType } from '../types';

interface NotificationPreferencesProps {
  userId?: number;
  className?: string;
  showSaveButton?: boolean;
  onPreferencesChange?: (preferences: NotificationPreferenceDto[]) => void;
}

export const NotificationPreferences: React.FC<NotificationPreferencesProps> = ({
  userId,
  className = '',
  showSaveButton = true,
  onPreferencesChange
}) => {
  const {
    preferences,
    isLoading,
    error,
    updatePreferences,
    savePreferences,
    resetToDefaults
  } = useNotificationPreferences(userId);

  const [localPreferences, setLocalPreferences] = useState<NotificationPreferenceDto[]>([]);
  const [hasChanges, setHasChanges] = useState(false);
  const [saveStatus, setSaveStatus] = useState<'idle' | 'saving' | 'success' | 'error'>('idle');

  useEffect(() => {
    if (preferences) {
      setLocalPreferences([...preferences]);
    }
  }, [preferences]);

  const handlePreferenceChange = (
    notificationType: NotificationType,
    channel: NotificationChannel,
    enabled: boolean
  ) => {
    setLocalPreferences(prev => 
      prev.map(pref => 
        pref.notificationType === notificationType && pref.channel === channel
          ? { ...pref, isEnabled: enabled }
          : pref
      )
    );
    setHasChanges(true);
  };

  const handleSave = async () => {
    if (!hasChanges) return;

    setSaveStatus('saving');
    try {
      await savePreferences(localPreferences);
      setHasChanges(false);
      setSaveStatus('success');
      onPreferencesChange?.(localPreferences);
      
      // Reset success status after 3 seconds
      setTimeout(() => setSaveStatus('idle'), 3000);
    } catch (err) {
      setSaveStatus('error');
      console.error('Failed to save preferences:', err);
    }
  };

  const handleReset = () => {
    setLocalPreferences([...preferences]);
    setHasChanges(false);
  };

  const getNotificationTypeLabel = (type: NotificationType): string => {
    const labels: Record<NotificationType, string> = {
      [NotificationType.ExamReminder]: 'Exam Reminders',
      [NotificationType.ExamStart]: 'Exam Start Notifications',
      [NotificationType.ExamEnd]: 'Exam End Notifications',
      [NotificationType.GradingComplete]: 'Grading Completion',
      [NotificationType.DeadlineReminder]: 'Deadline Reminders',
      [NotificationType.SystemAlert]: 'System Alerts',
      [NotificationType.UserWelcome]: 'Welcome Messages',
      [NotificationType.PasswordReset]: 'Password Reset',
      [NotificationType.AccountActivation]: 'Account Activation',
      [NotificationType.RoleAssignment]: 'Role Assignments'
    };
    return labels[type] || 'Unknown';
  };

  const getChannelIcon = (channel: NotificationChannel) => {
    switch (channel) {
      case NotificationChannel.Email:
        return <Email fontSize="small" />;
      case NotificationChannel.Sms:
        return <Sms fontSize="small" />;
      case NotificationChannel.Push:
        return <PushPin fontSize="small" />;
      case NotificationChannel.InApp:
        return <Notifications fontSize="small" />;
      default:
        return <Notifications fontSize="small" />;
    }
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

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" p={3}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error" sx={{ mb: 2 }}>
        Failed to load notification preferences: {error}
      </Alert>
    );
  }

  const notificationTypes = Object.values(NotificationType).filter(type => 
    typeof type === 'number'
  ) as NotificationType[];

  const channels = Object.values(NotificationChannel).filter(channel => 
    typeof channel === 'number'
  ) as NotificationChannel[];

  return (
    <Box className={className}>
      <Card>
        <CardContent>
          <Box display="flex" alignItems="center" mb={3}>
            <Notifications sx={{ mr: 1, color: 'primary.main' }} />
            <Typography variant="h6" component="h2">
              Notification Preferences
            </Typography>
          </Box>

          {saveStatus === 'success' && (
            <Alert severity="success" sx={{ mb: 2 }}>
              Preferences saved successfully!
            </Alert>
          )}

          {saveStatus === 'error' && (
            <Alert severity="error" sx={{ mb: 2 }}>
              Failed to save preferences. Please try again.
            </Alert>
          )}

          <Typography variant="body2" color="text.secondary" mb={3}>
            Customize how and when you receive notifications. You can enable or disable specific 
            notification types for different channels.
          </Typography>

          {notificationTypes.map((notificationType) => (
            <Accordion key={notificationType} defaultExpanded>
              <AccordionSummary expandIcon={<ExpandMore />}>
                <Typography variant="subtitle1" fontWeight="medium">
                  {getNotificationTypeLabel(notificationType)}
                </Typography>
              </AccordionSummary>
              <AccordionDetails>
                <Grid container spacing={2}>
                  {channels.map((channel) => {
                    const preference = localPreferences.find(
                      pref => pref.notificationType === notificationType && pref.channel === channel
                    );
                    
                    return (
                      <Grid item xs={12} sm={6} md={4} key={channel}>
                        <FormControlLabel
                          control={
                            <Switch
                              checked={preference?.isEnabled ?? false}
                              onChange={(e) => handlePreferenceChange(
                                notificationType,
                                channel,
                                e.target.checked
                              )}
                              color="primary"
                            />
                          }
                          label={
                            <Box display="flex" alignItems="center">
                              {getChannelIcon(channel)}
                              <Typography variant="body2" ml={1}>
                                {getChannelLabel(channel)}
                              </Typography>
                            </Box>
                          }
                        />
                      </Grid>
                    );
                  })}
                </Grid>
              </AccordionDetails>
            </Accordion>
          ))}

          <Divider sx={{ my: 3 }} />

          {showSaveButton && (
            <Box display="flex" gap={2} justifyContent="flex-end">
              <Button
                variant="outlined"
                onClick={handleReset}
                disabled={!hasChanges}
              >
                Reset Changes
              </Button>
              <Button
                variant="contained"
                onClick={handleSave}
                disabled={!hasChanges || saveStatus === 'saving'}
                startIcon={saveStatus === 'saving' ? <CircularProgress size={16} /> : null}
              >
                {saveStatus === 'saving' ? 'Saving...' : 'Save Preferences'}
              </Button>
            </Box>
          )}

          {hasChanges && (
            <Alert severity="info" sx={{ mt: 2 }}>
              You have unsaved changes. Click "Save Preferences" to apply your changes.
            </Alert>
          )}
        </CardContent>
      </Card>
    </Box>
  );
};
