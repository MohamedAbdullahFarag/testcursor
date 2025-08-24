import React, { useState } from 'react';
import {
  Box,
  Container,
  Typography,
  Paper,
  Button,
  Grid,
  Card,
  CardContent,
  CardActions,
  Divider,
  Alert
} from '@mui/material';
import {
  Notifications,
  Email,
  Sms,
  PushPin,
  Settings
} from '@mui/icons-material';
import { NotificationSystem } from '../components';
import { Notification, NotificationType, NotificationPriority, NotificationChannel } from '../types';

const NotificationDemoPage: React.FC = () => {
  const [showSystem, setShowSystem] = useState(false);
  const [demoNotifications, setDemoNotifications] = useState<Notification[]>([
    {
      id: 1,
      title: 'Welcome to Ikhtibar!',
      message: 'Thank you for joining our educational platform. We hope you have a great learning experience.',
      type: NotificationType.UserWelcome,
      priority: NotificationPriority.Info,
      channel: NotificationChannel.InApp,
      isRead: false,
      isSent: true,
      createdAt: new Date().toISOString(),
      scheduledAt: null,
      sentAt: new Date().toISOString(),
      readAt: null,
      userId: 1,
      relatedEntityType: null,
      relatedEntityId: null,
      metadata: {}
    },
    {
      id: 2,
      title: 'Exam Reminder: Mathematics 101',
      message: 'Your Mathematics 101 exam is scheduled for tomorrow at 10:00 AM. Please ensure you are prepared.',
      type: NotificationType.ExamReminder,
      priority: NotificationPriority.High,
      channel: NotificationChannel.Email,
      isRead: true,
      isSent: true,
      createdAt: new Date(Date.now() - 86400000).toISOString(), // 1 day ago
      scheduledAt: null,
      sentAt: new Date(Date.now() - 86400000).toISOString(),
      readAt: new Date(Date.now() - 43200000).toISOString(), // 12 hours ago
      userId: 1,
      relatedEntityType: 'Exam',
      relatedEntityId: 101,
      metadata: { examName: 'Mathematics 101', examDate: '2025-02-01', examTime: '10:00 AM' }
    }
  ]);

  const handleShowSystem = () => {
    setShowSystem(true);
  };

  const handleHideSystem = () => {
    setShowSystem(false);
  };

  const addDemoNotification = () => {
    const newNotification: Notification = {
      id: Date.now(),
      title: 'Demo Notification',
      message: 'This is a demo notification created at ' + new Date().toLocaleTimeString(),
      type: NotificationType.SystemAlert,
      priority: NotificationPriority.Medium,
      channel: NotificationChannel.InApp,
      isRead: false,
      isSent: true,
      createdAt: new Date().toISOString(),
      scheduledAt: null,
      sentAt: new Date().toISOString(),
      readAt: null,
      userId: 1,
      relatedEntityType: null,
      relatedEntityId: null,
      metadata: {}
    };
    setDemoNotifications(prev => [newNotification, ...prev]);
  };

  const clearNotifications = () => {
    setDemoNotifications([]);
  };

  if (showSystem) {
    return (
      <Container maxWidth="xl">
        <Box sx={{ mb: 3 }}>
          <Button
            variant="outlined"
            onClick={handleHideSystem}
            startIcon={<Settings />}
          >
            Back to Demo
          </Button>
        </Box>
        
        <NotificationSystem
          showTabs={true}
          defaultTab={0}
          showToastNotifications={true}
        />
      </Container>
    );
  }

  return (
    <Container maxWidth="lg">
      <Box sx={{ my: 4 }}>
        <Typography variant="h3" component="h1" gutterBottom align="center">
          Notification System Demo
        </Typography>
        <Typography variant="h6" component="h2" gutterBottom align="center" color="text.secondary">
          Explore the comprehensive notification system components
        </Typography>
      </Box>

      <Alert severity="info" sx={{ mb: 3 }}>
        This demo showcases the notification system components. Click "Launch Full System" to see the complete integrated system.
      </Alert>

      <Grid container spacing={3}>
        {/* Overview Card */}
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Box display="flex" alignItems="center" mb={2}>
                <Notifications sx={{ mr: 1, color: 'primary.main' }} />
                <Typography variant="h6" component="h3">
                  System Overview
                </Typography>
              </Box>
              <Typography variant="body2" color="text.secondary" paragraph>
                The notification system provides a comprehensive solution for managing user notifications across multiple channels.
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Features include real-time notifications, user preferences, template management, and multi-channel delivery.
              </Typography>
            </CardContent>
            <CardActions>
              <Button
                variant="contained"
                onClick={handleShowSystem}
                startIcon={<Notifications />}
                fullWidth
              >
                Launch Full System
              </Button>
            </CardActions>
          </Card>
        </Grid>

        {/* Demo Controls */}
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" component="h3" gutterBottom>
                Demo Controls
              </Typography>
              <Typography variant="body2" color="text.secondary" paragraph>
                Test the notification system with these demo actions.
              </Typography>
            </CardContent>
            <CardActions>
              <Button
                variant="outlined"
                onClick={addDemoNotification}
                startIcon={<Email />}
                sx={{ mr: 1 }}
              >
                Add Demo Notification
              </Button>
              <Button
                variant="outlined"
                color="secondary"
                onClick={clearNotifications}
                startIcon={<Sms />}
              >
                Clear All
              </Button>
            </CardActions>
          </Card>
        </Grid>

        {/* Current Notifications */}
        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Typography variant="h6" component="h3" gutterBottom>
                Current Demo Notifications ({demoNotifications.length})
              </Typography>
              {demoNotifications.length === 0 ? (
                <Typography variant="body2" color="text.secondary">
                  No notifications to display. Use the demo controls to add some.
                </Typography>
              ) : (
                <Box>
                  {demoNotifications.map((notification, index) => (
                    <Box key={notification.id}>
                      <Box display="flex" alignItems="center" mb={1}>
                        <Box
                          sx={{
                            width: 8,
                            height: 8,
                            borderRadius: '50%',
                            backgroundColor: notification.isRead ? 'text.disabled' : 'primary.main',
                            mr: 1
                          }}
                        />
                        <Typography variant="subtitle2" fontWeight="medium">
                          {notification.title}
                        </Typography>
                        <Box sx={{ ml: 'auto' }}>
                          <Typography variant="caption" color="text.secondary">
                            {new Date(notification.createdAt).toLocaleString()}
                          </Typography>
                        </Box>
                      </Box>
                      <Typography variant="body2" color="text.secondary" paragraph>
                        {notification.message}
                      </Typography>
                      <Box display="flex" gap={1} mb={2}>
                        <Typography variant="caption" color="primary">
                          {notification.type}
                        </Typography>
                        <Typography variant="caption" color="secondary">
                          {notification.priority}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          {notification.channel}
                        </Typography>
                      </Box>
                      {index < demoNotifications.length - 1 && <Divider sx={{ my: 2 }} />}
                    </Box>
                  ))}
                </Box>
              )}
            </CardContent>
          </Card>
        </Grid>

        {/* Component Features */}
        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Typography variant="h6" component="h3" gutterBottom>
                Component Features
              </Typography>
              <Grid container spacing={2}>
                <Grid item xs={12} sm={6} md={3}>
                  <Box textAlign="center">
                    <Notifications sx={{ fontSize: 40, color: 'primary.main', mb: 1 }} />
                    <Typography variant="subtitle2" gutterBottom>
                      NotificationList
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      Display and manage notifications with filtering and actions
                    </Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Box textAlign="center">
                    <Settings sx={{ fontSize: 40, color: 'primary.main', mb: 1 }} />
                    <Typography variant="subtitle2" gutterBottom>
                      Preferences
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      User notification preferences and channel settings
                    </Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Box textAlign="center">
                    <Email sx={{ fontSize: 40, color: 'primary.main', mb: 1 }} />
                    <Typography variant="subtitle2" gutterBottom>
                      Templates
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      Admin template management for consistent messaging
                    </Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Box textAlign="center">
                    <PushPin sx={{ fontSize: 40, color: 'primary.main', mb: 1 }} />
                    <Typography variant="subtitle2" gutterBottom>
                      Toast & Modal
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      Real-time notifications and detailed views
                    </Typography>
                  </Box>
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Container>
  );
};

export default NotificationDemoPage;
