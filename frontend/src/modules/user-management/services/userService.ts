/**
 * User Management Service
 * Handles all API communication for user-related operations
 * Updated to work with INT primary keys and standardized API responses
 */

import { apiClient } from '../../../shared/services/apiClient';
import type {
  UserDto,
  CreateUserDto,
  UpdateUserDto,
  UserFilterOptions,
  PagedResult,
  ApiResponse,
} from '../../../shared/types/core-entities';

/**
 * User service class for API operations
 * Provides complete CRUD operations with error handling and type safety
 */
class UserService {
  private readonly baseUrl = '/api/users';

  /**
   * Get paginated list of users with optional filtering
   */
  async getUsers(
    page: number = 1,
    pageSize: number = 10,
    filters?: UserFilterOptions
  ): Promise<PagedResult<UserDto>> {
    try {
      const searchParams = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
      });

      // Add filter parameters if provided
      if (filters) {
        if (filters.search) {
          searchParams.append('search', filters.search);
        }
        
        if (filters.roleId) {
          searchParams.append('roleId', filters.roleId.toString());
        }
        
        if (filters.isActive !== undefined) {
          searchParams.append('isActive', filters.isActive.toString());
        }
        
        if (filters.emailVerified !== undefined) {
          searchParams.append('emailVerified', filters.emailVerified.toString());
        }
        
        if (filters.createdFrom) {
          searchParams.append('createdFrom', filters.createdFrom);
        }
        
        if (filters.createdTo) {
          searchParams.append('createdTo', filters.createdTo);
        }
      }

      const response = await apiClient.get<ApiResponse<PagedResult<UserDto>>>(
        `${this.baseUrl}?${searchParams.toString()}`
      );

      if (!response.success) {
        throw new Error(response.message || 'Failed to fetch users');
      }

      return response.data!;
    } catch (error) {
      console.error('Error fetching users:', error);
      throw error;
    }
  }

  /**
   * Get a single user by ID
   */
  async getUserById(id: number): Promise<UserDto> {
    try {
      const response = await apiClient.get<ApiResponse<UserDto>>(`${this.baseUrl}/${id}`);

      if (!response.success) {
        throw new Error(response.message || 'Failed to fetch user');
      }

      return response.data!;
    } catch (error) {
      console.error(`Error fetching user ${id}:`, error);
      throw error;
    }
  }

  /**
   * Create a new user
   */
  async createUser(userData: CreateUserDto): Promise<UserDto> {
    try {
      const response = await apiClient.post<ApiResponse<UserDto>>(this.baseUrl, userData);

      if (!response.success) {
        throw new Error(response.message || 'Failed to create user');
      }

      return response.data!;
    } catch (error) {
      console.error('Error creating user:', error);
      throw error;
    }
  }

  /**
   * Update an existing user
   */
  async updateUser(id: number, userData: UpdateUserDto): Promise<UserDto> {
    try {
      const response = await apiClient.put<ApiResponse<UserDto>>(
        `${this.baseUrl}/${id}`,
        userData
      );

      if (!response.success) {
        throw new Error(response.message || 'Failed to update user');
      }

      return response.data!;
    } catch (error) {
      console.error(`Error updating user ${id}:`, error);
      throw error;
    }
  }

  /**
   * Delete a user (soft delete)
   */
  async deleteUser(id: number): Promise<void> {
    try {
      const response = await apiClient.delete<ApiResponse<void>>(`${this.baseUrl}/${id}`);

      if (!response.success) {
        throw new Error(response.message || 'Failed to delete user');
      }
    } catch (error) {
      console.error(`Error deleting user ${id}:`, error);
      throw error;
    }
  }

  /**
   * Activate/deactivate a user
   */
  async toggleUserStatus(id: number, isActive: boolean): Promise<UserDto> {
    try {
      const response = await apiClient.patch<ApiResponse<UserDto>>(
        `${this.baseUrl}/${id}/status`,
        { isActive }
      );

      if (!response.success) {
        throw new Error(response.message || 'Failed to update user status');
      }

      return response.data!;
    } catch (error) {
      console.error(`Error toggling user ${id} status:`, error);
      throw error;
    }
  }

  /**
   * Assign roles to a user
   */
  async assignRoles(userId: number, roleIds: number[]): Promise<UserDto> {
    try {
      const response = await apiClient.post<ApiResponse<UserDto>>(
        `${this.baseUrl}/${userId}/roles`,
        { roleIds }
      );

      if (!response.success) {
        throw new Error(response.message || 'Failed to assign roles');
      }

      return response.data!;
    } catch (error) {
      console.error(`Error assigning roles to user ${userId}:`, error);
      throw error;
    }
  }

  /**
   * Remove roles from a user
   */
  async removeRoles(userId: number, roleIds: number[]): Promise<UserDto> {
    try {
      const response = await apiClient.post<ApiResponse<UserDto>>(
        `${this.baseUrl}/${userId}/roles/remove`,
        { roleIds }
      );

      if (!response.success) {
        throw new Error(response.message || 'Failed to remove roles');
      }

      return response.data!;
    } catch (error) {
      console.error(`Error removing roles from user ${userId}:`, error);
      throw error;
    }
  }

  /**
   * Reset user password
   */
  async resetPassword(userId: number, newPassword: string): Promise<void> {
    try {
      const response = await apiClient.post<ApiResponse<void>>(
        `${this.baseUrl}/${userId}/reset-password`,
        { newPassword }
      );

      if (!response.success) {
        throw new Error(response.message || 'Failed to reset password');
      }
    } catch (error) {
      console.error(`Error resetting password for user ${userId}:`, error);
      throw error;
    }
  }
}

// Export singleton instance
export const userService = new UserService();
export default userService;
