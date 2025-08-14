/**
 * User Management Service
 * Handles all API communication for user-related operations
 * Follows established frontend patterns and provides comprehensive error handling
 */

import { apiClient } from '../../../shared/services/apiClient';
import type {
  User,
  CreateUserRequest,
  UpdateUserRequest,
  UserListResponse,
  UserPaginationParams,
  BulkUserActionResult,
  UserExportFormat,
  UserActivityLog,
} from '../models/user.types';

/**
 * User service class for API operations
 * Provides complete CRUD operations with error handling and type safety
 */
class UserService {
  private readonly baseUrl = '/api/users';

  /**
   * Get paginated list of users with optional filtering
   */
  async getUsers(params: UserPaginationParams): Promise<UserListResponse> {
    try {
      const searchParams = new URLSearchParams({
        page: params.page.toString(),
        pageSize: params.pageSize.toString(),
      });

      // Add filter parameters if provided
      if (params.filters) {
        const { filters } = params;
        
        if (filters.searchTerm) {
          searchParams.append('searchTerm', filters.searchTerm);
        }
        
        if (filters.roles && filters.roles.length > 0) {
          searchParams.append('roles', filters.roles.join(','));
        }
        
        if (filters.isActive !== undefined) {
          searchParams.append('isActive', filters.isActive.toString());
        }
        
        if (filters.emailVerified !== undefined) {
          searchParams.append('emailVerified', filters.emailVerified.toString());
        }
        
        if (filters.preferredLanguage) {
          searchParams.append('preferredLanguage', filters.preferredLanguage);
        }
        
        if (filters.sortBy) {
          searchParams.append('sortBy', filters.sortBy);
        }
        
        if (filters.sortDirection) {
          searchParams.append('sortDirection', filters.sortDirection);
        }
      }

      const response = await apiClient.get<UserListResponse>(`${this.baseUrl}?${searchParams.toString()}`);
      
      if (!response.success) {
        throw new Error(response.message || 'Failed to fetch users');
      }

      return response.data;
    } catch (error) {
      console.error('Error fetching users:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while fetching users');
    }
  }

  /**
   * Get a single user by ID
   */
  async getUserById(id: number): Promise<User> {
    try {
      const response = await apiClient.get<User>(`${this.baseUrl}/${id}`);
      
      if (!response.success) {
        throw new Error(response.message || 'Failed to fetch user');
      }

      return response.data;
    } catch (error) {
      console.error('Error fetching user:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while fetching user');
    }
  }

  /**
   * Create a new user
   */
  async createUser(userData: CreateUserRequest): Promise<User> {
    try {
      const response = await apiClient.post<User>(this.baseUrl, userData);
      
      if (!response.success) {
        throw new Error(response.message || 'Failed to create user');
      }

      return response.data;
    } catch (error) {
      console.error('Error creating user:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while creating user');
    }
  }

  /**
   * Update an existing user
   */
  async updateUser(userData: UpdateUserRequest): Promise<User> {
    try {
      const response = await apiClient.put<User>(`${this.baseUrl}/${userData.id}`, userData);
      
      if (!response.success) {
        throw new Error(response.message || 'Failed to update user');
      }

      return response.data;
    } catch (error) {
      console.error('Error updating user:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while updating user');
    }
  }

  /**
   * Delete a user by ID
   */
  async deleteUser(id: number): Promise<boolean> {
    try {
      const response = await apiClient.delete<boolean>(`${this.baseUrl}/${id}`);
      
      if (!response.success) {
        throw new Error(response.message || 'Failed to delete user');
      }

      return response.data;
    } catch (error) {
      console.error('Error deleting user:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while deleting user');
    }
  }

  /**
   * Bulk delete multiple users
   */
  async bulkDeleteUsers(userIds: number[]): Promise<BulkUserActionResult> {
    try {
      const response = await apiClient.post<BulkUserActionResult>(`${this.baseUrl}/bulk-delete`, { userIds });
      
      if (!response.success) {
        throw new Error(response.message || 'Failed to delete users');
      }

      return response.data;
    } catch (error) {
      console.error('Error bulk deleting users:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while deleting users');
    }
  }

  /**
   * Export users in specified format
   */
  async exportUsers(format: UserExportFormat, params?: UserPaginationParams): Promise<Blob> {
    try {
      const searchParams = new URLSearchParams({
        format,
      });

      // Add filter parameters if provided
      if (params?.filters) {
        const { filters } = params;
        
        if (filters.searchTerm) {
          searchParams.append('searchTerm', filters.searchTerm);
        }
        
        if (filters.roles && filters.roles.length > 0) {
          searchParams.append('roles', filters.roles.join(','));
        }
        
        if (filters.isActive !== undefined) {
          searchParams.append('isActive', filters.isActive.toString());
        }
        
        if (filters.emailVerified !== undefined) {
          searchParams.append('emailVerified', filters.emailVerified.toString());
        }
        
        if (filters.preferredLanguage) {
          searchParams.append('preferredLanguage', filters.preferredLanguage);
        }
      }

      const response = await apiClient.get<Blob>(`${this.baseUrl}/export?${searchParams.toString()}`, {
        responseType: 'blob'
      });
      
      if (!response.success) {
        throw new Error(response.message || 'Failed to export users');
      }

      return response.data;
    } catch (error) {
      console.error('Error exporting users:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while exporting users');
    }
  }

  /**
   * Get user activity logs
   */
  async getUserActivity(userId: number, params?: { 
    page?: number; 
    pageSize?: number; 
    startDate?: Date; 
    endDate?: Date 
  }): Promise<UserActivityLog[]> {
    try {
      const searchParams = new URLSearchParams();
      
      if (params?.page) {
        searchParams.append('page', params.page.toString());
      }
      
      if (params?.pageSize) {
        searchParams.append('pageSize', params.pageSize.toString());
      }
      
      if (params?.startDate) {
        searchParams.append('startDate', params.startDate.toISOString());
      }
      
      if (params?.endDate) {
        searchParams.append('endDate', params.endDate.toISOString());
      }

      const response = await apiClient.get<UserActivityLog[]>(`${this.baseUrl}/${userId}/activity?${searchParams.toString()}`);
      
      if (!response.success) {
        throw new Error(response.message || 'Failed to fetch user activity');
      }

      return response.data;
    } catch (error) {
      console.error('Error fetching user activity:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while fetching user activity');
    }
  }

  /**
   * Activate a user account
   */
  async activateUser(id: number): Promise<User> {
    try {
      const response = await apiClient.put<User>(`${this.baseUrl}/${id}/activate`);
      
      if (!response.success) {
        throw new Error(response.message || 'Failed to activate user');
      }

      return response.data;
    } catch (error) {
      console.error('Error activating user:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while activating user');
    }
  }

  /**
   * Deactivate a user account
   */
  async deactivateUser(id: number): Promise<User> {
    try {
      const response = await apiClient.put<User>(`${this.baseUrl}/${id}/deactivate`);
      
      if (!response.success) {
        throw new Error(response.message || 'Failed to deactivate user');
      }

      return response.data;
    } catch (error) {
      console.error('Error deactivating user:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while deactivating user');
    }
  }

  /**
   * Reset user password
   */
  async resetUserPassword(id: number, newPassword: string): Promise<boolean> {
    try {
      const response = await apiClient.post<boolean>(`${this.baseUrl}/${id}/reset-password`, { 
        newPassword 
      });
      
      if (!response.success) {
        throw new Error(response.message || 'Failed to reset user password');
      }

      return response.data;
    } catch (error) {
      console.error('Error resetting user password:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while resetting user password');
    }
  }

  /**
   * Send password reset email
   */
  async sendPasswordResetEmail(email: string): Promise<boolean> {
    try {
      const response = await apiClient.post<boolean>(`${this.baseUrl}/send-password-reset`, { 
        email 
      });
      
      if (!response.success) {
        throw new Error(response.message || 'Failed to send password reset email');
      }

      return response.data;
    } catch (error) {
      console.error('Error sending password reset email:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while sending password reset email');
    }
  }
}

/**
 * Export singleton instance of the user service
 */
export const userService = new UserService();

/**
 * Export the class for testing purposes
 */
export { UserService };
