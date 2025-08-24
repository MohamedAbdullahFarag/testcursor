/**
 * User Management Service
 * Handles all API communication for user-related operations
 * Updated to work with INT primary keys and standardized API responses
 */

import { apiClient } from '../../../shared/services/apiClient';
import { User, CreateUserRequest, UpdateUserRequest, UserListResponse } from '../models/user.types';

const API = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7001';

export const userService = {
  list: async (page = 1, filter = ''): Promise<UserListResponse> => {
    const res = await apiClient.get<UserListResponse>('/api/users', {
      params: { page, filter }
    });
    if (!res.ok) throw new Error('Failed to load users');
    return res.data;
  },

  create: async (data: CreateUserRequest): Promise<User> => {
    const res = await apiClient.post<User>('/api/users', data);
    if (!res.ok) throw new Error('Failed to create user');
    return res.data;
  },

  update: async (data: UpdateUserRequest): Promise<User> => {
    const res = await apiClient.put<User>(`/api/users/${data.id}`, data);
    if (!res.ok) throw new Error('Failed to update user');
    return res.data;
  },

  remove: async (id: string): Promise<boolean> => {
    const res = await apiClient.delete(`/api/users/${id}`);
    if (!res.ok) throw new Error('Failed to delete user');
    return true;
  },

  getById: async (id: string): Promise<User> => {
    const res = await apiClient.get<User>(`/api/users/${id}`);
    if (!res.ok) throw new Error('Failed to load user');
    return res.data;
  }
};
