/**
 * Notification service for the frontend notification module
 * Handles all API communication with the backend notification system
 * Following established patterns from the shared notification service
 */

import { apiClient } from '@/shared/services/apiClient';
import {
  NotificationDto,
  CreateNotificationDto,
  UpdateNotificationDto,
  NotificationFilterDto,
  NotificationPagedResponse,
  NotificationUnreadCountResponse,
  BulkNotificationResponse,
  NotificationSendResponse,
  NotificationPreferenceDto,
  UpdateNotificationPreferenceDto,
  NotificationTemplateDto,
  NotificationHistoryDto
} from '../types/notification.types';

/**
 * Comprehensive notification service for frontend module
 * Provides full CRUD operations and notification management
 */
export class NotificationService {
  private readonly baseUrl = '/api/notifications';
  private readonly preferencesUrl = '/api/notifications/preferences';
  private readonly templatesUrl = '/api/notifications/templates';

  // ==================== CORE NOTIFICATION OPERATIONS ====================

  /**
   * Gets paginated notifications for a user with filtering
   */
  async getUserNotifications(
    userId: number,
    filter: NotificationFilterDto = {}
  ): Promise<NotificationPagedResponse> {
    try {
      const queryParams = new URLSearchParams();
      
      // Add pagination params
      if (filter.page) queryParams.append('page', filter.page.toString());
      if (filter.pageSize) queryParams.append('pageSize', filter.pageSize.toString());
      
      // Add filter params
      if (filter.unreadOnly) queryParams.append('unreadOnly', filter.unreadOnly.toString());
      if (filter.notificationType) queryParams.append('notificationType', filter.notificationType);
      if (filter.priority) queryParams.append('priority', filter.priority);
      if (filter.status) queryParams.append('status', filter.status);
      if (filter.fromDate) queryParams.append('fromDate', filter.fromDate);
      if (filter.toDate) queryParams.append('toDate', filter.toDate);
      if (filter.searchTerm) queryParams.append('searchTerm', filter.searchTerm);
      if (filter.entityType) queryParams.append('entityType', filter.entityType);
      if (filter.entityId) queryParams.append('entityId', filter.entityId.toString());

      const url = `${this.baseUrl}/user/${userId}?${queryParams.toString()}`;
      const response = await apiClient.get<NotificationPagedResponse>(url);
      return response.data;
    } catch (error) {
      console.error('Error fetching user notifications:', error);
      throw error;
    }
  }

  /**
   * Gets a specific notification by ID
   */
  async getNotificationById(notificationId: string): Promise<NotificationDto | null> {
    try {
      const response = await apiClient.get<NotificationDto>(`${this.baseUrl}/${notificationId}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching notification:', error);
      return null;
    }
  }

  /**
   * Creates a new notification
   */
  async createNotification(notification: CreateNotificationDto): Promise<NotificationDto | null> {
    try {
      const response = await apiClient.post<NotificationDto>(this.baseUrl, notification);
      return response.data;
    } catch (error) {
      console.error('Error creating notification:', error);
      return null;
    }
  }

  /**
   * Updates an existing notification
   */
  async updateNotification(
    notificationId: string, 
    updates: UpdateNotificationDto
  ): Promise<boolean> {
    try {
      await apiClient.put(`${this.baseUrl}/${notificationId}`, updates);
      return true;
    } catch (error) {
      console.error('Error updating notification:', error);
      return false;
    }
  }

  /**
   * Deletes a notification
   */
  async deleteNotification(notificationId: string): Promise<boolean> {
    try {
      await apiClient.delete(`${this.baseUrl}/${notificationId}`);
      return true;
    } catch (error) {
      console.error('Error deleting notification:', error);
      return false;
    }
  }

  // ==================== NOTIFICATION ACTIONS ====================

  /**
   * Marks a notification as read
   */
  async markAsRead(notificationId: string, userId: number): Promise<boolean> {
    try {
      await apiClient.put(`${this.baseUrl}/${notificationId}/read`, { userId });
      return true;
    } catch (error) {
      console.error('Error marking notification as read:', error);
      return false;
    }
  }

  /**
   * Marks all notifications as read for a user
   */
  async markAllAsRead(userId: number): Promise<boolean> {
    try {
      await apiClient.put(`${this.baseUrl}/read-all`, { userId });
      return true;
    } catch (error) {
      console.error('Error marking all notifications as read:', error);
      return false;
    }
  }

  /**
   * Gets unread notification count for a user
   */
  async getUnreadCount(userId: number): Promise<number> {
    try {
      const response = await apiClient.get<NotificationUnreadCountResponse>(
        `${this.baseUrl}/unread-count/${userId}`
      );
      return response.data.unreadCount;
    } catch (error) {
      console.error('Error fetching unread count:', error);
      return 0;
    }
  }

  /**
   * Sends an immediate notification
   */
  async sendImmediateNotification(
    notification: CreateNotificationDto
  ): Promise<NotificationSendResponse> {
    try {
      const response = await apiClient.post<NotificationSendResponse>(
        `${this.baseUrl}/send-immediate`,
        notification
      );
      return response.data;
    } catch (error) {
      console.error('Error sending immediate notification:', error);
      throw error;
    }
  }

  /**
   * Sends bulk notifications
   */
  async sendBulkNotifications(
    notifications: CreateNotificationDto[]
  ): Promise<BulkNotificationResponse> {
    try {
      const response = await apiClient.post<BulkNotificationResponse>(
        `${this.baseUrl}/bulk`,
        notifications
      );
      return response.data;
    } catch (error) {
      console.error('Error sending bulk notifications:', error);
      throw error;
    }
  }

  /**
   * Schedules a notification for future delivery
   */
  async scheduleNotification(
    notification: CreateNotificationDto,
    scheduleTime: string
  ): Promise<boolean> {
    try {
      await apiClient.post(`${this.baseUrl}/schedule`, {
        ...notification,
        scheduledAt: scheduleTime
      });
      return true;
    } catch (error) {
      console.error('Error scheduling notification:', error);
      return false;
    }
  }

  /**
   * Cancels a scheduled notification
   */
  async cancelNotification(notificationId: string): Promise<boolean> {
    try {
      await apiClient.put(`${this.baseUrl}/${notificationId}/cancel`);
      return true;
    } catch (error) {
      console.error('Error cancelling notification:', error);
      return false;
    }
  }

  // ==================== NOTIFICATION PREFERENCES ====================

  /**
   * Gets notification preferences for a user
   */
  async getUserPreferences(userId: number): Promise<NotificationPreferenceDto[]> {
    try {
      const response = await apiClient.get<NotificationPreferenceDto[]>(
        `${this.preferencesUrl}/${userId}`
      );
      return response.data;
    } catch (error) {
      console.error('Error fetching notification preferences:', error);
      return [];
    }
  }

  /**
   * Updates notification preferences for a user
   */
  async updateUserPreferences(
    userId: number,
    preferences: UpdateNotificationPreferenceDto[]
  ): Promise<boolean> {
    try {
      await apiClient.put(`${this.preferencesUrl}/${userId}`, preferences);
      return true;
    } catch (error) {
      console.error('Error updating notification preferences:', error);
      return false;
    }
  }

  /**
   * Updates a single notification preference
   */
  async updateSinglePreference(
    userId: number,
    preference: UpdateNotificationPreferenceDto
  ): Promise<boolean> {
    try {
      await apiClient.put(`${this.preferencesUrl}/${userId}/single`, preference);
      return true;
    } catch (error) {
      console.error('Error updating notification preference:', error);
      return false;
    }
  }

  /**
   * Resets user preferences to defaults
   */
  async resetPreferencesToDefaults(userId: number): Promise<boolean> {
    try {
      await apiClient.post(`${this.preferencesUrl}/${userId}/reset`);
      return true;
    } catch (error) {
      console.error('Error resetting preferences to defaults:', error);
      return false;
    }
  }

  // ==================== NOTIFICATION TEMPLATES ====================

  /**
   * Gets all notification templates
   */
  async getTemplates(): Promise<NotificationTemplateDto[]> {
    try {
      const response = await apiClient.get<NotificationTemplateDto[]>(this.templatesUrl);
      return response.data;
    } catch (error) {
      console.error('Error fetching notification templates:', error);
      return [];
    }
  }

  /**
   * Gets templates by type
   */
  async getTemplatesByType(type: string): Promise<NotificationTemplateDto[]> {
    try {
      const response = await apiClient.get<NotificationTemplateDto[]>(
        `${this.templatesUrl}/type/${type}`
      );
      return response.data;
    } catch (error) {
      console.error('Error fetching templates by type:', error);
      return [];
    }
  }

  /**
   * Creates a new notification template
   */
  async createTemplate(template: Partial<NotificationTemplateDto>): Promise<NotificationTemplateDto | null> {
    try {
      const response = await apiClient.post<NotificationTemplateDto>(this.templatesUrl, template);
      return response.data;
    } catch (error) {
      console.error('Error creating notification template:', error);
      return null;
    }
  }

  /**
   * Updates a notification template
   */
  async updateTemplate(
    templateId: string,
    updates: Partial<NotificationTemplateDto>
  ): Promise<boolean> {
    try {
      await apiClient.put(`${this.templatesUrl}/${templateId}`, updates);
      return true;
    } catch (error) {
      console.error('Error updating notification template:', error);
      return false;
    }
  }

  /**
   * Deletes a notification template
   */
  async deleteTemplate(templateId: string): Promise<boolean> {
    try {
      await apiClient.delete(`${this.templatesUrl}/${templateId}`);
      return true;
    } catch (error) {
      console.error('Error deleting notification template:', error);
      return false;
    }
  }

  // ==================== NOTIFICATION HISTORY ====================

  /**
   * Gets delivery history for a notification
   */
  async getNotificationHistory(notificationId: string): Promise<NotificationHistoryDto[]> {
    try {
      const response = await apiClient.get<NotificationHistoryDto[]>(
        `${this.baseUrl}/${notificationId}/history`
      );
      return response.data;
    } catch (error) {
      console.error('Error fetching notification history:', error);
      return [];
    }
  }

  /**
   * Gets delivery analytics for a date range
   */
  async getDeliveryAnalytics(fromDate: string, toDate: string): Promise<any> {
    try {
      const response = await apiClient.get(
        `${this.baseUrl}/analytics/delivery?fromDate=${fromDate}&toDate=${toDate}`
      );
      return response.data;
    } catch (error) {
      console.error('Error fetching delivery analytics:', error);
      return null;
    }
  }

  // ==================== SYSTEM OPERATIONS ====================

  /**
   * Gets system notification statistics
   */
  async getSystemStats(fromDate: string, toDate: string): Promise<any> {
    try {
      const response = await apiClient.get(
        `${this.baseUrl}/system/stats?fromDate=${fromDate}&toDate=${toDate}`
      );
      return response.data;
    } catch (error) {
      console.error('Error fetching system stats:', error);
      return null;
    }
  }

  /**
   * Tests notification delivery for a specific channel
   */
  async testNotificationDelivery(
    userId: number,
    channel: string,
    testMessage: string
  ): Promise<boolean> {
    try {
      await apiClient.post(`${this.baseUrl}/test`, {
        userId,
        channel,
        message: testMessage
      });
      return true;
    } catch (error) {
      console.error('Error testing notification delivery:', error);
      return false;
    }
  }

  // ==================== EVENT-DRIVEN NOTIFICATIONS ====================

  /**
   * Sends exam reminder notification
   */
  async sendExamReminder(examId: number, reminderMinutes: number): Promise<boolean> {
    try {
      await apiClient.post(`${this.baseUrl}/events/exam-reminder`, {
        examId,
        reminderMinutes
      });
      return true;
    } catch (error) {
      console.error('Error sending exam reminder:', error);
      return false;
    }
  }

  /**
   * Sends exam start notification
   */
  async sendExamStartNotification(examId: number): Promise<boolean> {
    try {
      await apiClient.post(`${this.baseUrl}/events/exam-start`, { examId });
      return true;
    } catch (error) {
      console.error('Error sending exam start notification:', error);
      return false;
    }
  }

  /**
   * Sends exam end notification
   */
  async sendExamEndNotification(examId: number): Promise<boolean> {
    try {
      await apiClient.post(`${this.baseUrl}/events/exam-end`, { examId });
      return true;
    } catch (error) {
      console.error('Error sending exam end notification:', error);
      return false;
    }
  }

  /**
   * Sends grading complete notification
   */
  async sendGradingCompleteNotification(
    examId: number,
    studentId: number,
    score: number,
    grade: string
  ): Promise<boolean> {
    try {
      await apiClient.post(`${this.baseUrl}/events/grading-complete`, {
        examId,
        studentId,
        score,
        grade
      });
      return true;
    } catch (error) {
      console.error('Error sending grading complete notification:', error);
      return false;
    }
  }

  /**
   * Sends welcome notification to new user
   */
  async sendWelcomeNotification(userId: number): Promise<boolean> {
    try {
      await apiClient.post(`${this.baseUrl}/events/welcome`, { userId });
      return true;
    } catch (error) {
      console.error('Error sending welcome notification:', error);
      return false;
    }
  }

  /**
   * Sends password reset notification
   */
  async sendPasswordResetNotification(
    userId: number,
    resetToken: string,
    resetUrl: string
  ): Promise<boolean> {
    try {
      await apiClient.post(`${this.baseUrl}/events/password-reset`, {
        userId,
        resetToken,
        resetUrl
      });
      return true;
    } catch (error) {
      console.error('Error sending password reset notification:', error);
      return false;
    }
  }
}

// Export singleton instance
export const notificationService = new NotificationService();
export default notificationService;
