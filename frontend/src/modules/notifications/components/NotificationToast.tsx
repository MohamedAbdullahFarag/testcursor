import React, { useEffect, useState } from 'react';
import {
  Box,
  Typography,
  IconButton,
  Collapse,
  Paper,
  Chip,
  Avatar
} from '@mui/material';
import {
  Close,
  Notifications,
  Email,
  Sms,
  PushPin,
  CheckCircle,
  Error,
  Warning,
  Info
} from '@mui/icons-material';
import { Notification, NotificationPriority, NotificationChannel, NotificationType } from '../types';

interface NotificationToastProps {
  notification: Notification;
  onClose: (notificationId: number) => void;
  onMarkAsRead?: (notificationId: number) => void;
  autoHideDuration?: number;
  position?: 'top-right' | 'top-left' | 'bottom-right' | 'bottom-left' | 'top-center' | 'bottom-center';
  className?: string;
}

export const NotificationToast: React.FC<NotificationToastProps> = ({
  notification,
  onClose,
  onMarkAsRead,
  autoHideDuration = 5000,
  position = 'top-right',
  className = ''
}) => {
  const [isVisible, setIsVisible] = useState(true);
  const [isExpanded, setIsExpanded] = useState(false);

  useEffect(() => {
    if (autoHideDuration > 0) {
      const timer = setTimeout(() => {
        handleClose();
      }, autoHideDuration);

      return () => clearTimeout(timer);
    }
  }, [autoHideDuration]);

  const handleClose = () => {
    setIsVisible(false);
    setTimeout(() => {
      onClose(notification.id);
    }, 300); // Wait for collapse animation
  };

  const handleMarkAsRead = () => {
    if (onMarkAsRead && !notification.isRead) {
      onMarkAsRead(notification.id);
    }
  };

  const handleToggleExpand = () => {
    setIsExpanded(!isExpanded);
  };

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

  const formatTime = (dateString: string): string => {
    const date = new Date(dateString);
    const now = new Date();
    const diffInMinutes = Math.floor((now.getTime() - date.getTime()) / (1000 * 60));

    if (diffInMinutes < 1) return 'Just now';
    if (diffInMinutes < 60) return `${diffInMinutes}m ago`;
    if (diffInMinutes < 1440) return `${Math.floor(diffInMinutes / 60)}h ago`;
    return date.toLocaleDateString();
  };

  const getPositionStyles = () => {
    const baseStyles = {
      position: 'fixed' as const,
      zIndex: 9999,
      maxWidth: '400px',
      width: '100%',
      m: 2
    };

    switch (position) {
      case 'top-right':
        return { ...baseStyles, top: 0, right: 0 };
      case 'top-left':
        return { ...baseStyles, top: 0, left: 0 };
      case 'bottom-right':
        return { ...baseStyles, bottom: 0, right: 0 };
      case 'bottom-left':
        return { ...baseStyles, bottom: 0, left: 0 };
      case 'top-center':
        return { ...baseStyles, top: 0, left: '50%', transform: 'translateX(-50%)' };
      case 'bottom-center':
        return { ...baseStyles, bottom: 0, left: '50%', transform: 'translateX(-50%)' };
      default:
        return { ...baseStyles, top: 0, right: 0 };
    }
  };

  return (
    <Collapse in={isVisible}>
      <Paper
        elevation={3}
        sx={{
          ...getPositionStyles(),
          borderRadius: 2,
          overflow: 'hidden',
          cursor: 'pointer',
          '&:hover': {
            elevation: 6
          }
        }}
        className={className}
        onClick={handleToggleExpand}
      >
        <Box
          sx={{
            p: 2,
            borderLeft: 4,
            borderColor: getPriorityColor(notification.priority),
            backgroundColor: 'background.paper'
          }}
        >
          <Box display="flex" alignItems="flex-start" gap={2}>
            <Avatar
              sx={{
                bgcolor: `${getPriorityColor(notification.priority)}.light`,
                color: `${getPriorityColor(notification.priority)}.dark`,
                width: 32,
                height: 32
              }}
            >
              {getPriorityIcon(notification.priority)}
            </Avatar>

            <Box flex={1} minWidth={0}>
              <Box display="flex" alignItems="center" justifyContent="space-between" mb={1}>
                <Typography variant="subtitle2" fontWeight="medium" noWrap>
                  {notification.title}
                </Typography>
                <IconButton
                  size="small"
                  onClick={(e) => {
                    e.stopPropagation();
                    handleClose();
                  }}
                  sx={{ ml: 1 }}
                >
                  <Close fontSize="small" />
                </IconButton>
              </Box>

              <Typography
                variant="body2"
                color="text.secondary"
                sx={{
                  display: '-webkit-box',
                  WebkitLineClamp: isExpanded ? 'unset' : 2,
                  WebkitBoxOrient: 'vertical',
                  overflow: 'hidden',
                  lineHeight: 1.4
                }}
              >
                {notification.message}
              </Typography>

              <Box display="flex" alignItems="center" gap={1} mt={1.5} flexWrap="wrap">
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
                />
                {notification.channel && (
                  <Chip
                    label={getChannelLabel(notification.channel)}
                    size="small"
                    variant="outlined"
                    icon={getChannelIcon(notification.channel)}
                  />
                )}
                <Typography variant="caption" color="text.secondary">
                  {formatTime(notification.createdAt)}
                </Typography>
              </Box>

              {isExpanded && (
                <Box mt={2}>
                  <Typography variant="caption" color="text.secondary">
                    Click to collapse • {formatTime(notification.createdAt)}
                  </Typography>
                </Box>
              )}
            </Box>
          </Box>

          {!isExpanded && (
            <Box mt={1.5}>
              <Typography variant="caption" color="text.secondary">
                Click to expand • {formatTime(notification.createdAt)}
              </Typography>
            </Box>
          )}
        </Box>
      </Paper>
    </Collapse>
  );
};
