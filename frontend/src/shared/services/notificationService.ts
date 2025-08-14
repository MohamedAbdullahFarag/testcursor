/**
 * Notification service for managing user notifications
 * 
 * This service handles all communication with the notification API endpoints
 * and provides methods for fetching, creating, and managing notifications.
 */

import { apiClient } from './apiClient';
import { PaginatedResponse } from '../models/api';
import { 
  Notification,
  NotificationResponse,
  CreateNotificationRequest,
  UpdateNotificationRequest,
  NotificationPreference,
  NotificationFilter
} from '../types/notification.types';

class NotificationService {
  private readonly baseUrl = '/api/notifications';

  /**
   * Get all notifications for the current user with pagination and filtering
   */
  async getNotifications(
    page: number = 1, 
    pageSize: number = 10,
    filter?: NotificationFilter
  ): Promise<PaginatedResponse<Notification>> {
    // Build query parameters
    const queryParams = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
      ...(filter?.type && { type: filter.type }),
      ...(filter?.priority && { priority: filter.priority }),
      ...(filter?.status && { status: filter.status }),
      ...(filter?.isRead !== undefined && { isRead: filter.isRead.toString() }),
      ...(filter?.channel && { channel: filter.channel }),
      ...(filter?.searchTerm && { searchTerm: filter.searchTerm }),
      ...(filter?.startDate && { startDate: filter.startDate.toISOString() }),
      ...(filter?.endDate && { endDate: filter.endDate.toISOString() })
    });

    const response = await apiClient.get<PaginatedResponse<Notification>>(`${this.baseUrl}?${queryParams}`);
    return response.data;
  }

  /**
   * Get a specific notification by ID
   */
  async getNotificationById(id: string): Promise<Notification> {
    const response = await apiClient.get<Notification>(`${this.baseUrl}/${id}`);
    return response.data;
  }

  /**
   * Create a new notification
   */
  async createNotification(notification: CreateNotificationRequest): Promise<NotificationResponse> {
    const response = await apiClient.post<NotificationResponse>(
      this.baseUrl,
      notification
    );
    
    return response.data;
  }

  /**
   * Update an existing notification
   */
  async updateNotification(id: string, notification: UpdateNotificationRequest): Promise<NotificationResponse> {
    const response = await apiClient.put<NotificationResponse>(
      `${this.baseUrl}/${id}`,
      notification
    );
    
    return response.data;
  }

  /**
   * Mark a notification as read
   */
  async markAsRead(id: string): Promise<void> {
    await apiClient.put(`${this.baseUrl}/${id}/read`);
  }

  /**
   * Mark all notifications as read for the current user
   */
  async markAllAsRead(): Promise<void> {
    await apiClient.put(`${this.baseUrl}/read-all`);
  }

  /**
   * Delete a notification
   */
  async deleteNotification(id: string): Promise<void> {
    await apiClient.delete(`${this.baseUrl}/${id}`);
  }

  /**
   * Get user notification preferences
   */
  async getNotificationPreferences(): Promise<NotificationPreference[]> {
    const response = await apiClient.get<NotificationPreference[]>(
      `${this.baseUrl}/preferences`
    );
    
    return response.data;
  }

  /**
   * Update user notification preferences
   */
  async updateNotificationPreferences(
    preferences: Partial<NotificationPreference>[]
  ): Promise<NotificationPreference[]> {
    const response = await apiClient.put<NotificationPreference[]>(
      `${this.baseUrl}/preferences`,
      preferences
    );
    
    return response.data;
  }

  /**
   * Get unread notification count for the current user
   */
  async getUnreadCount(): Promise<number> {
    const response = await apiClient.get<{ count: number }>(
      `${this.baseUrl}/unread-count`
    );
    
    return response.data.count;
  }
}

export const notificationService = new NotificationService();
