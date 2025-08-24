import React, { useState, useEffect } from 'react';
import {
  Box,
  Tabs,
  Tab,
  Typography,
  Paper,
  Container,
  Divider,
  Alert,
  CircularProgress
} from '@mui/material';
import { Notifications } from '@mui/icons-material';
import { NotificationList } from './NotificationList';
import { NotificationPreferences } from './NotificationPreferences';
import { NotificationTemplateManager } from './NotificationTemplateManager';
import { NotificationToast } from './NotificationToast';
import { useNotifications } from '../hooks/useNotifications';
import { useNotificationPreferences } from '../hooks/useNotificationPreferences';
import { Notification, NotificationTemplate } from '../types';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`notification-tabpanel-${index}`}
      aria-labelledby={`notification-tab-${index}`}
      {...other}
    >
      {value === index && (
        <Box sx={{ p: 3 }}>
          {children}
        </Box>
      )}
    </div>
  );
}

function a11yProps(index: number) {
  return {
    id: `notification-tab-${index}`,
    'aria-controls': `notification-tabpanel-${index}`,
  };
}

interface NotificationSystemProps {
  className?: string;
  showTabs?: boolean;
  defaultTab?: number;
  showToastNotifications?: boolean;
  onNotificationAction?: (action: string, notification: Notification) => void;
}

export const NotificationSystem: React.FC<NotificationSystemProps> = ({
  className = '',
  showTabs = true,
  defaultTab = 0,
  showToastNotifications = true,
  onNotificationAction
}) => {
  const [tabValue, setTabValue] = useState(defaultTab);
  const [toastNotifications, setToastNotifications] = useState<Notification[]>([]);

  // Custom hooks for notifications and preferences
  const {
    notifications,
    isLoading: notificationsLoading,
    error: notificationsError,
    markAsRead,
    markAsUnread,
    deleteNotification,
    refreshNotifications
  } = useNotifications();

  const {
    preferences,
    isLoading: preferencesLoading,
    error: preferencesError,
    updatePreferences,
    savePreferences
  } = useNotificationPreferences();

  // Handle tab changes
  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  // Handle notification actions
  const handleNotificationAction = (action: string, notification: Notification) => {
    switch (action) {
      case 'markAsRead':
        markAsRead(notification.id);
        break;
      case 'markAsUnread':
        markAsUnread(notification.id);
        break;
      case 'delete':
        deleteNotification(notification.id);
        break;
      default:
        break;
    }

    // Call parent callback if provided
    onNotificationAction?.(action, notification);
  };

  // Handle template changes
  const handleTemplateChange = (templates: NotificationTemplate[]) => {
    console.log('Templates updated:', templates);
    // In a real app, this would sync with the backend
  };

  // Show toast notifications for new notifications
  useEffect(() => {
    if (showToastNotifications && notifications.length > 0) {
      const newNotifications = notifications.filter(n => !n.isRead);
      if (newNotifications.length > 0) {
        setToastNotifications(prev => [...prev, ...newNotifications]);
      }
    }
  }, [notifications, showToastNotifications]);

  // Remove toast notifications after they're dismissed
  const handleToastClose = (notificationId: number) => {
    setToastNotifications(prev => prev.filter(n => n.id !== notificationId));
  };

  // Mark toast as read
  const handleToastMarkAsRead = (notificationId: number) => {
    markAsRead(notificationId);
    handleToastClose(notificationId);
  };

  if (showTabs) {
    return (
      <Container maxWidth="xl" className={className}>
        <Paper elevation={1} sx={{ borderRadius: 2 }}>
          {/* Header */}
          <Box sx={{ borderBottom: 1, borderColor: 'divider', p: 2 }}>
            <Box display="flex" alignItems="center" mb={1}>
              <Notifications sx={{ mr: 1, color: 'primary.main' }} />
              <Typography variant="h5" component="h1">
                Notification System
              </Typography>
            </Box>
            <Typography variant="body2" color="text.secondary">
              Manage your notifications, preferences, and templates
            </Typography>
          </Box>

          {/* Tabs */}
          <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tabs
              value={tabValue}
              onChange={handleTabChange}
              aria-label="notification system tabs"
              variant="scrollable"
              scrollButtons="auto"
            >
              <Tab
                label={
                  <Box display="flex" alignItems="center">
                    <Typography>Notifications</Typography>
                    {notifications.filter(n => !n.isRead).length > 0 && (
                      <Box
                        sx={{
                          ml: 1,
                          minWidth: 20,
                          height: 20,
                          borderRadius: '50%',
                          backgroundColor: 'error.main',
                          color: 'white',
                          display: 'flex',
                          alignItems: 'center',
                          justifyContent: 'center',
                          fontSize: '0.75rem',
                          fontWeight: 'bold'
                        }}
                      >
                        {notifications.filter(n => !n.isRead).length}
                      </Box>
                    )}
                  </Box>
                }
                {...a11yProps(0)}
              />
              <Tab label="Preferences" {...a11yProps(1)} />
              <Tab label="Templates" {...a11yProps(2)} />
            </Tabs>
          </Box>

          {/* Tab Panels */}
          <TabPanel value={tabValue} index={0}>
            {notificationsError ? (
              <Alert severity="error" sx={{ mb: 2 }}>
                {notificationsError}
              </Alert>
            ) : (
              <NotificationList
                notifications={notifications}
                isLoading={notificationsLoading}
                onNotificationAction={handleNotificationAction}
                onRefresh={refreshNotifications}
              />
            )}
          </TabPanel>

          <TabPanel value={tabValue} index={1}>
            {preferencesError ? (
              <Alert severity="error" sx={{ mb: 2 }}>
                {preferencesError}
              </Alert>
            ) : (
              <NotificationPreferences
                preferences={preferences}
                isLoading={preferencesLoading}
                onPreferencesChange={updatePreferences}
                onSave={savePreferences}
              />
            )}
          </TabPanel>

          <TabPanel value={tabValue} index={2}>
            <NotificationTemplateManager
              onTemplateChange={handleTemplateChange}
            />
          </TabPanel>
        </Paper>

        {/* Toast Notifications */}
        {showToastNotifications && (
          <Box
            sx={{
              position: 'fixed',
              top: 20,
              right: 20,
              zIndex: 9999,
              display: 'flex',
              flexDirection: 'column',
              gap: 1
            }}
          >
            {toastNotifications.map((notification) => (
              <NotificationToast
                key={notification.id}
                notification={notification}
                onClose={handleToastClose}
                onMarkAsRead={handleToastMarkAsRead}
                autoHideDuration={8000}
                position="top-right"
              />
            ))}
          </Box>
        )}
      </Container>
    );
  }

  // Non-tabbed version - just show notifications
  return (
    <Box className={className}>
      <NotificationList
        notifications={notifications}
        isLoading={notificationsLoading}
        onNotificationAction={handleNotificationAction}
        onRefresh={refreshNotifications}
      />

      {/* Toast Notifications */}
      {showToastNotifications && (
        <Box
          sx={{
            position: 'fixed',
            top: 20,
            right: 20,
            zIndex: 9999,
            display: 'flex',
            flexDirection: 'column',
            gap: 1
          }}
        >
          {toastNotifications.map((notification) => (
            <NotificationToast
              key={notification.id}
              notification={notification}
              onClose={handleToastClose}
              onMarkAsRead={handleToastMarkAsRead}
              autoHideDuration={8000}
              position="top-right"
            />
          ))}
        </Box>
      )}
    </Box>
  );
};
