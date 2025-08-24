/**
 * Role Service
 * Handles all role-related API operations
 */

import { apiClient } from '../../../shared/services/apiClient';
import { 
  Role, 
  CreateRoleRequest, 
  UpdateRoleRequest, 
  RoleListResponse, 
  RolePaginationParams,
  AssignRoleRequest,
  UpdateUserRolesRequest,
  UserRoleSummary,
  RoleUsersResponse,
  Permission,
  RolePermissionMatrix
} from '../models/role.types';

/**
 * Role Service implementation
 */
export const roleService = {
  /**
   * Get roles with pagination and filtering
   */
  async getRoles(params: RolePaginationParams): Promise<RoleListResponse> {
    try {
      const queryParams = new URLSearchParams({
        page: params.page.toString(),
        pageSize: params.pageSize.toString(),
      });

      if (params.filters) {
        if (params.filters.searchTerm) {
          queryParams.append('searchTerm', params.filters.searchTerm);
        }
        if (params.filters.isSystemRole !== undefined) {
          queryParams.append('isSystemRole', params.filters.isSystemRole.toString());
        }
        if (params.filters.isActive !== undefined) {
          queryParams.append('isActive', params.filters.isActive.toString());
        }
        if (params.filters.hasPermission) {
          queryParams.append('hasPermission', params.filters.hasPermission);
        }
      }

      const response = await apiClient.get<RoleListResponse>(`/api/roles?${queryParams}`);
      return response.data!;
    } catch (error) {
      console.error('Error fetching roles:', error);
      throw new Error('Failed to fetch roles');
    }
  },

  /**
   * Get a single role by ID
   */
  async getRole(id: number): Promise<Role> {
    try {
      const response = await apiClient.get<Role>(`/api/roles/${id}`);
      return response.data!;
    } catch (error) {
      console.error(`Error fetching role ${id}:`, error);
      if (error.statusCode === 404) {
        throw new Error(`Role with ID ${id} not found`);
      }
      throw new Error('Failed to fetch role');
    }
  },

  /**
   * Create a new role
   */
  async createRole(role: CreateRoleRequest): Promise<Role> {
    try {
      const response = await apiClient.post<Role>('/api/roles', role);
      return response.data!;
    } catch (error) {
      console.error('Error creating role:', error);
      throw new Error('Failed to create role');
    }
  },

  /**
   * Update an existing role
   */
  async updateRole(id: number, role: UpdateRoleRequest): Promise<Role> {
    try {
      const response = await apiClient.put<Role>(`/api/roles/${id}`, role);
      return response.data!;
    } catch (error) {
      console.error(`Error updating role ${id}:`, error);
      throw new Error('Failed to update role');
    }
  },

  /**
   * Delete a role
   */
  async deleteRole(id: number): Promise<boolean> {
    try {
      await apiClient.delete<void>(`/api/roles/${id}`);
      return true;
    } catch (error) {
      console.error(`Error deleting role ${id}:`, error);
      throw new Error('Failed to delete role');
    }
  },

  /**
   * Assign a role to a user
   */
  async assignRoleToUser(request: AssignRoleRequest): Promise<boolean> {
    try {
      await apiClient.post<void>('/api/user-roles', request);
      return true;
    } catch (error) {
      console.error('Error assigning role:', error);
      throw new Error('Failed to assign role');
    }
  },

  /**
   * Remove a role from a user
   */
  async removeRoleFromUser(userId: number, roleId: number): Promise<boolean> {
    try {
      await apiClient.delete<void>(`/api/user-roles/${userId}/${roleId}`);
      return true;
    } catch (error) {
      console.error(`Error removing role ${roleId} from user ${userId}:`, error);
      throw new Error('Failed to remove role');
    }
  },

  /**
   * Get roles assigned to a user
   */
  async getUserRoles(userId: number): Promise<Role[]> {
    try {
      const response = await apiClient.get<Role[]>(`/api/users/${userId}/roles`);
      return response.data!;
    } catch (error) {
      console.error(`Error fetching roles for user ${userId}:`, error);
      throw new Error('Failed to fetch user roles');
    }
  },

  /**
   * Update user roles (batch assignment)
   */
  async updateUserRoles(request: UpdateUserRolesRequest): Promise<boolean> {
    try {
      await apiClient.put<void>(`/api/users/${request.userId}/roles`, { roleIds: request.roleIds });
      return true;
    } catch (error) {
      console.error(`Error updating roles for user ${request.userId}:`, error);
      throw new Error('Failed to update user roles');
    }
  },

  /**
   * Get all available permissions
   */
  async getPermissions(): Promise<Permission[]> {
    try {
      const response = await apiClient.get<Permission[]>('/api/permissions');
      return response.data!;
    } catch (error) {
      console.error('Error fetching permissions:', error);
      throw new Error('Failed to fetch permissions');
    }
  },

  /**
   * Get role permission matrix
   */
  async getRolePermissionMatrix(roleId: number): Promise<RolePermissionMatrix> {
    try {
      const response = await apiClient.get<RolePermissionMatrix>(`/api/roles/${roleId}/permissions`);
      return response.data!;
    } catch (error) {
      console.error(`Error fetching permission matrix for role ${roleId}:`, error);
      throw new Error('Failed to fetch role permission matrix');
    }
  },

  /**
   * Update role permissions
   */
  async updateRolePermissions(roleId: number, permissionCodes: string[]): Promise<boolean> {
    try {
      await apiClient.put<void>(`/api/roles/${roleId}/permissions`, { permissionCodes });
      return true;
    } catch (error) {
      console.error(`Error updating permissions for role ${roleId}:`, error);
      throw new Error('Failed to update role permissions');
    }
  },

  /**
   * Get users with a specific role
   */
  async getRoleUsers(roleId: number, page: number = 1, pageSize: number = 10): Promise<RoleUsersResponse> {
    try {
      const queryParams = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
      });

      const response = await apiClient.get<RoleUsersResponse>(`/api/roles/${roleId}/users?${queryParams}`);
      return response.data!;
    } catch (error) {
      console.error(`Error fetching users for role ${roleId}:`, error);
      throw new Error('Failed to fetch role users');
    }
  },
};
