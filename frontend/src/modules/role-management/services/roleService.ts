/**
 * Role Service
 * Handles all role-related API operations
 */

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

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

/**
 * Role Service implementation
 */
export const roleService = {
  /**
   * Get roles with pagination and filtering
   */
  async getRoles(params: RolePaginationParams): Promise<RoleListResponse> {
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

    const response = await fetch(`${API_BASE_URL}/api/roles?${queryParams}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch roles: ${response.statusText}`);
    }

    return response.json();
  },

  /**
   * Get a single role by ID
   */
  async getRole(id: number): Promise<Role> {
    const response = await fetch(`${API_BASE_URL}/api/roles/${id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
    });

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error(`Role with ID ${id} not found`);
      }
      throw new Error(`Failed to fetch role: ${response.statusText}`);
    }

    return response.json();
  },

  /**
   * Create a new role
   */
  async createRole(role: CreateRoleRequest): Promise<Role> {
    const response = await fetch(`${API_BASE_URL}/api/roles`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
      body: JSON.stringify(role),
    });

    if (!response.ok) {
      throw new Error(`Failed to create role: ${response.statusText}`);
    }

    return response.json();
  },

  /**
   * Update an existing role
   */
  async updateRole(id: number, role: UpdateRoleRequest): Promise<Role> {
    const response = await fetch(`${API_BASE_URL}/api/roles/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
      body: JSON.stringify(role),
    });

    if (!response.ok) {
      throw new Error(`Failed to update role: ${response.statusText}`);
    }

    return response.json();
  },

  /**
   * Delete a role
   */
  async deleteRole(id: number): Promise<boolean> {
    const response = await fetch(`${API_BASE_URL}/api/roles/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to delete role: ${response.statusText}`);
    }

    return true;
  },

  /**
   * Assign a role to a user
   */
  async assignRoleToUser(request: AssignRoleRequest): Promise<boolean> {
    const response = await fetch(`${API_BASE_URL}/api/user-roles`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      throw new Error(`Failed to assign role: ${response.statusText}`);
    }

    return true;
  },

  /**
   * Remove a role from a user
   */
  async removeRoleFromUser(userId: number, roleId: number): Promise<boolean> {
    const response = await fetch(`${API_BASE_URL}/api/user-roles/${userId}/${roleId}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to remove role: ${response.statusText}`);
    }

    return true;
  },

  /**
   * Get roles assigned to a user
   */
  async getUserRoles(userId: number): Promise<Role[]> {
    const response = await fetch(`${API_BASE_URL}/api/users/${userId}/roles`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch user roles: ${response.statusText}`);
    }

    return response.json();
  },

  /**
   * Update user roles (batch assignment)
   */
  async updateUserRoles(request: UpdateUserRolesRequest): Promise<boolean> {
    const response = await fetch(`${API_BASE_URL}/api/users/${request.userId}/roles`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
      body: JSON.stringify({ roleIds: request.roleIds }),
    });

    if (!response.ok) {
      throw new Error(`Failed to update user roles: ${response.statusText}`);
    }

    return true;
  },

  /**
   * Get all available permissions
   */
  async getPermissions(): Promise<Permission[]> {
    const response = await fetch(`${API_BASE_URL}/api/permissions`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch permissions: ${response.statusText}`);
    }

    return response.json();
  },

  /**
   * Get role permission matrix
   */
  async getRolePermissionMatrix(roleId: number): Promise<RolePermissionMatrix> {
    const response = await fetch(`${API_BASE_URL}/api/roles/${roleId}/permissions`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch role permission matrix: ${response.statusText}`);
    }

    return response.json();
  },

  /**
   * Update role permissions
   */
  async updateRolePermissions(roleId: number, permissionCodes: string[]): Promise<boolean> {
    const response = await fetch(`${API_BASE_URL}/api/roles/${roleId}/permissions`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
      body: JSON.stringify({ permissionCodes }),
    });

    if (!response.ok) {
      throw new Error(`Failed to update role permissions: ${response.statusText}`);
    }

    return true;
  },

  /**
   * Get users with a specific role
   */
  async getRoleUsers(roleId: number, page: number = 1, pageSize: number = 10): Promise<RoleUsersResponse> {
    const queryParams = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
    });

    const response = await fetch(`${API_BASE_URL}/api/roles/${roleId}/users?${queryParams}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch role users: ${response.statusText}`);
    }

    return response.json();
  },
};
