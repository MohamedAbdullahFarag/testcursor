import React from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
  Chip,
  Divider,
  IconButton,
  Tooltip,
  Avatar
} from '@mui/material';
import {
  Close,
  Notifications,
  Email,
  Sms,
  PushPin,
  Schedule,
  CheckCircle,
  Error,
  Warning,
  Info
} from '@mui/icons-material';
import { Notification, NotificationType, NotificationPriority, NotificationChannel } from '../types';

interface NotificationModalProps {
  notification: Notification | null;
  open: boolean;
  onClose: () => void;
  onMarkAsRead?: (notificationId: number) => void;
  onMarkAsUnread?: (notificationId: number) => void;
  onDelete?: (notificationId: number) => void;
  showActions?: boolean;
}

export const NotificationModal: React.FC<NotificationModalProps> = ({
  notification,
  open,
  onClose,
  onMarkAsRead,
  onMarkAsUnread,
  onDelete,
  showActions = true
}) => {
  if (!notification) return null;

  const getPriorityIcon = (priority: NotificationPriority) => {
    switch (priority) {
      case NotificationPriority.Critical:
        return <Error color="error" />;
      case NotificationPriority.High:
        return <Warning color="warning" />;
      case NotificationPriority.Medium:
        return <Info color="info" />;
      case NotificationPriority.Low:
        return <CheckCircle color="success" />;
      default:
        return <Info color="info" />;
    }
  };

  const getPriorityColor = (priority: NotificationPriority): 'error' | 'warning' | 'info' | 'success' => {
    switch (priority) {
      case NotificationPriority.Critical:
        return 'error';
      case NotificationPriority.High:
        return 'warning';
      case NotificationPriority.Medium:
        return 'info';
      case NotificationPriority.Low:
        return 'success';
      default:
        return 'info';
    }
  };

  const getTypeLabel = (type: NotificationType): string => {
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
    return labels[type] || 'Notification';
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

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleString();
  };

  const handleMarkAsRead = () => {
    if (onMarkAsRead && !notification.isRead) {
      onMarkAsRead(notification.id);
    }
  };

  const handleMarkAsUnread = () => {
    if (onMarkAsUnread && notification.isRead) {
      onMarkAsUnread(notification.id);
    }
  };

  const handleDelete = () => {
    if (onDelete) {
      onDelete(notification.id);
      onClose();
    }
  };

  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="md"
      fullWidth
      PaperProps={{
        sx: {
          borderRadius: 2,
          boxShadow: 3
        }
      }}
    >
      <DialogTitle>
        <Box display="flex" alignItems="center" justifyContent="space-between">
          <Box display="flex" alignItems="center">
            <Avatar sx={{ bgcolor: 'primary.main', mr: 2 }}>
              <Notifications />
            </Avatar>
            <Box>
              <Typography variant="h6" component="h2">
                {notification.title}
              </Typography>
              <Box display="flex" alignItems="center" gap={1} mt={0.5}>
                <Chip
                  label={getTypeLabel(notification.notificationType)}
                  size="small"
                  color="primary"
                  variant="outlined"
                />
                <Chip
                  label={notification.priority}
                  size="small"
                  color={getPriorityColor(notification.priority)}
                  icon={getPriorityIcon(notification.priority)}
                />
                {notification.channel && (
                  <Chip
                    label={getChannelLabel(notification.channel)}
                    size="small"
                    variant="outlined"
                    icon={getChannelIcon(notification.channel)}
                  />
                )}
              </Box>
            </Box>
          </Box>
          <IconButton onClick={onClose} size="small">
            <Close />
          </IconButton>
        </Box>
      </DialogTitle>

      <DialogContent>
        <Box mb={3}>
          <Typography variant="body1" paragraph>
            {notification.message}
          </Typography>
        </Box>

        <Divider sx={{ my: 2 }} />

        <Box>
          <Typography variant="subtitle2" color="text.secondary" gutterBottom>
            Notification Details
          </Typography>
          
          <Box display="grid" gridTemplateColumns="1fr 1fr" gap={2}>
            <Box>
              <Typography variant="caption" color="text.secondary">
                Created
              </Typography>
              <Typography variant="body2" display="flex" alignItems="center" gap={1}>
                <Schedule fontSize="small" />
                {formatDate(notification.createdAt)}
              </Typography>
            </Box>

            {notification.scheduledAt && (
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Scheduled For
                </Typography>
                <Typography variant="body2" display="flex" alignItems="center" gap={1}>
                  <Schedule fontSize="small" />
                  {formatDate(notification.scheduledAt)}
                </Typography>
              </Box>
            )}

            {notification.sentAt && (
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Sent At
                </Typography>
                <Typography variant="body2" display="flex" alignItems="center" gap={1}>
                  <CheckCircle fontSize="small" />
                  {formatDate(notification.sentAt)}
                </Typography>
              </Box>
            )}

            {notification.readAt && (
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Read At
                </Typography>
                <Typography variant="body2" display="flex" alignItems="center" gap={1}>
                  <CheckCircle fontSize="small" />
                  {formatDate(notification.readAt)}
                </Typography>
              </Box>
            )}

            {notification.entityType && notification.entityId && (
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Related Entity
                </Typography>
                <Typography variant="body2">
                  {notification.entityType} #{notification.entityId}
                </Typography>
              </Box>
            )}

            {notification.metadata && (
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Metadata
                </Typography>
                <Typography variant="body2" sx={{ wordBreak: 'break-word' }}>
                  {notification.metadata}
                </Typography>
              </Box>
            )}
          </Box>
        </Box>
      </DialogContent>

      {showActions && (
        <DialogActions sx={{ p: 2, pt: 1 }}>
          <Box display="flex" gap={1} flex={1}>
            {!notification.isRead ? (
              <Button
                variant="outlined"
                onClick={handleMarkAsRead}
                startIcon={<CheckCircle />}
              >
                Mark as Read
              </Button>
            ) : (
              <Button
                variant="outlined"
                onClick={handleMarkAsUnread}
                startIcon={<Notifications />}
              >
                Mark as Unread
              </Button>
            )}
            
            {onDelete && (
              <Button
                variant="outlined"
                color="error"
                onClick={handleDelete}
              >
                Delete
              </Button>
            )}
          </Box>
          
          <Button variant="contained" onClick={onClose}>
            Close
          </Button>
        </DialogActions>
      )}
    </Dialog>
  );
};
