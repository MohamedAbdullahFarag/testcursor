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
import type { ApiResponse } from '../../../shared/models';

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
      const response = await fetch(`${this.baseUrl}/bulk-delete`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ userIds }),
      });

      if (!response.ok) {
        throw new Error(`Failed to bulk delete users: ${response.status} ${response.statusText}`);
      }

      const data = await response.json() as ApiResponse<BulkUserActionResult>;
      
      if (!data.success) {
        throw new Error(data.message || 'Failed to bulk delete users');
      }

      return data.data;
    } catch (error) {
      console.error('Error bulk deleting users:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while bulk deleting users');
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
      }

      const response = await fetch(`${this.baseUrl}/export?${searchParams.toString()}`, {
        method: 'GET',
        headers: {
          'Accept': this.getExportMimeType(format),
        },
      });

      if (!response.ok) {
        throw new Error(`Failed to export users: ${response.status} ${response.statusText}`);
      }

      return await response.blob();
    } catch (error) {
      console.error('Error exporting users:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while exporting users');
    }
  }

  /**
   * Get user activity log
   */
  async getUserActivityLog(userId: number, page = 1, pageSize = 10): Promise<{
    items: UserActivityLog[];
    total: number;
    page: number;
    pageSize: number;
    totalPages: number;
  }> {
    try {
      const searchParams = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
      });

      const response = await fetch(`${this.baseUrl}/${userId}/activity?${searchParams.toString()}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        if (response.status === 404) {
          throw new Error('User not found');
        }
        throw new Error(`Failed to fetch user activity: ${response.status} ${response.statusText}`);
      }

      const data = await response.json();
      return data.data;
    } catch (error) {
      console.error('Error fetching user activity:', error);
      throw error instanceof Error ? error : new Error('Unknown error occurred while fetching user activity');
    }
  }

  /**
   * Get MIME type for export format
   */
  private getExportMimeType(format: UserExportFormat): string {
    switch (format) {
      case 'csv':
        return 'text/csv';
      case 'excel':
        return 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
      case 'pdf':
        return 'application/pdf';
      default:
        return 'application/octet-stream';
    }
  }
}

/**
 * Export singleton instance of UserService
 */
export const userService = new UserService();

/**
 * Export the UserService class for testing purposes
 */
export { UserService };

/**
 * Export default instance for convenience
 */
export default userService;
