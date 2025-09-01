import { apiClient } from '../../../shared/services/apiClient';
import { User, CreateUserRequest, UpdateUserRequest, UserListResponse } from '../models/user.types';
import { ApiResponse } from '../../../shared/models';

// Define the backend user response structure
interface BackendUser {
  userId: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  preferredLanguage?: string;
  isActive: boolean;
  emailVerified: boolean;
  phoneVerified: boolean;
  roles?: string[];
  permissions?: string[];
  createdAt: string;
  updatedAt?: string;
}

// Define the backend list response structure
interface BackendUserListResponse {
  items: BackendUser[];
  total: number;
  page: number;
  pageSize: number;
}

export const userService = {
  list: async (page = 1, filter = ''): Promise<UserListResponse> => {
    const res: ApiResponse<BackendUserListResponse> = await apiClient.get('/api/users', {
      params: { page, pageSize: 10, filter }
    });
    
    // Handle the actual API response structure from SuccessResponse
    if (res.success && res.data) {
      // Transform the users to match frontend expectations
      const transformedUsers: User[] = res.data.items?.map((user: BackendUser) => ({
        id: user.userId,  // Map userId to id
        userId: user.userId,
        username: user.username || '',
        email: user.email || '',
        firstName: user.firstName || '',
        lastName: user.lastName || '',
        fullName: `${user.firstName || ''} ${user.lastName || ''}`.trim() || user.username || user.email,
        phoneNumber: user.phoneNumber,
        preferredLanguage: user.preferredLanguage,
        isActive: user.isActive,
        emailVerified: user.emailVerified,
        phoneVerified: user.phoneVerified,
        roles: user.roles || [],
        permissions: user.permissions || [],
        createdAt: user.createdAt,
        updatedAt: user.updatedAt
      })) || [];

      return {
        items: transformedUsers,
        total: res.data.total || 0,
        page: res.data.page || 1,
        pageSize: res.data.pageSize || 10
      };
    }
    
    // Fallback if the response structure is unexpected
    throw new Error('Failed to load users: Invalid response format');
  },

  create: async (data: CreateUserRequest): Promise<User> => {
    const createDto = {
      username: data.username,
      email: data.email,
      firstName: data.firstName,
      lastName: data.lastName,
      password: data.password,
      phoneNumber: data.phoneNumber,
      preferredLanguage: data.preferredLanguage,
      roleIds: data.roleIds
    };
    const res = await apiClient.post<User>('/api/users', createDto);
    if (!res.success) throw new Error(res.message || 'Failed to create user');
    return res.data;
  },

  update: async (data: UpdateUserRequest): Promise<User> => {
    const updateDto = {
      username: data.username,
      email: data.email,
      firstName: data.firstName,
      lastName: data.lastName,
      phoneNumber: data.phoneNumber,
      preferredLanguage: data.preferredLanguage,
      isActive: data.isActive,
      roleIds: data.roleIds
    };
    const res = await apiClient.put<User>(`/api/users/${data.id}`, updateDto);
    if (!res.success) throw new Error(res.message || 'Failed to update user');
    return res.data;
  },

  remove: async (id: string): Promise<boolean> => {
    const res = await apiClient.delete(`/api/users/${id}`);
    if (!res.success) throw new Error(res.message || 'Failed to delete user');
    return true;
  },

  getById: async (id: string): Promise<User> => {
    const res = await apiClient.get<User>(`/api/users/${id}`);
    if (!res.success) throw new Error(res.message || 'Failed to load user');
    return res.data;
  }
};
