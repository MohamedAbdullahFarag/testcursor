/**
 * Role Management Hook
 * Custom React hook for managing role operations with state management,
 * error handling, loading states, and data caching
 */

import { useState, useEffect, useCallback, useRef } from 'react';
import { roleService } from '../services/roleService';
import type {
  Role,
  CreateRoleRequest,
  UpdateRoleRequest,
  RoleListResponse,
  RoleFilterOptions,
  Permission,
  RolePermissionMatrix,
  AssignRoleRequest,
  UpdateUserRolesRequest,
  UserRoleSummary,
} from '../models/role.types';

/**
 * Configuration options for the useRoleManagement hook
 */
export interface UseRoleManagementOptions {
  /** Initial page size for pagination */
  initialPageSize?: number;
  /** Whether to automatically load data on mount */
  autoLoad?: boolean;
  /** Debounce delay for search operations (ms) */
  searchDebounceMs?: number;
  /** Whether to enable client-side caching */
  enableCache?: boolean;
  /** Cache duration in milliseconds */
  cacheDuration?: number;
}

/**
 * Return type for the useRoleManagement hook
 */
export interface UseRoleManagementReturn {
  // Data state
  roles: Role[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  
  // Loading and error state
  loading: boolean;
  error: string | null;
  
  // Actions
  loadRoles: (page?: number, pageSize?: number, filters?: RoleFilterOptions) => Promise<void>;
  getRole: (id: number) => Promise<Role>;
  createRole: (role: CreateRoleRequest) => Promise<Role>;
  updateRole: (id: number, role: UpdateRoleRequest) => Promise<Role>;
  deleteRole: (id: number) => Promise<boolean>;
  
  // Search and filtering
  setFilters: (filters: RoleFilterOptions) => void;
  clearFilters: () => void;
  searchTerm: string | undefined;
  setSearchTerm: (term: string) => void;
  
  // Pagination
  goToPage: (page: number) => Promise<void>;
  setPageSize: (size: number) => void;
  refresh: () => Promise<void>;
  
  // Selection
  selectedRoles: number[];
  selectRole: (id: number) => void;
  deselectRole: (id: number) => void;
  selectAll: () => void;
  deselectAll: () => void;
  
  // Role-User Management
  assignRoleToUser: (userId: number, roleId: number) => Promise<boolean>;
  removeRoleFromUser: (userId: number, roleId: number) => Promise<boolean>;
  getUserRoles: (userId: number) => Promise<UserRoleSummary>;
  updateUserRoles: (userId: number, roleIds: number[]) => Promise<boolean>;
  
  // Permissions
  permissions: Permission[];
  loadPermissions: () => Promise<Permission[]>;
  getRolePermissionMatrix: (roleId: number) => Promise<RolePermissionMatrix>;
  updateRolePermissions: (roleId: number, permissionCodes: string[]) => Promise<boolean>;
}

/**
 * Default options for the hook
 */
const DEFAULT_OPTIONS: UseRoleManagementOptions = {
  initialPageSize: 10,
  autoLoad: true,
  searchDebounceMs: 300,
  enableCache: true,
  cacheDuration: 5 * 60 * 1000, // 5 minutes
};

/**
 * Hook for role management operations
 */
export const useRoleManagement = (options: UseRoleManagementOptions = {}): UseRoleManagementReturn => {
  // Merge options with defaults
  const { 
    initialPageSize = DEFAULT_OPTIONS.initialPageSize,
    autoLoad = DEFAULT_OPTIONS.autoLoad,
    searchDebounceMs = DEFAULT_OPTIONS.searchDebounceMs,
    enableCache = DEFAULT_OPTIONS.enableCache,
    cacheDuration = DEFAULT_OPTIONS.cacheDuration,
  } = options;

  // State for role data
  const [roles, setRoles] = useState<Role[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(initialPageSize || 10);
  
  // State for loading and errors
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // State for filters
  const [filters, setFilters] = useState<RoleFilterOptions>({});
  const [searchTerm, setSearchTerm] = useState<string | undefined>(undefined);
  
  // State for selection
  const [selectedRoles, setSelectedRoles] = useState<number[]>([]);
  
  // State for permissions
  const [permissions, setPermissions] = useState<Permission[]>([]);
  
  // Refs for cache and debounce
  const cacheRef = useRef<{ [key: string]: { data: RoleListResponse; timestamp: number } }>({});
  const searchDebounceTimerRef = useRef<number | null>(null);
  
  /**
   * Compute derived pagination values
   */
  const totalPages = Math.ceil(total / pageSize);
  const hasNextPage = page < totalPages;
  const hasPreviousPage = page > 1;

  /**
   * Load roles with pagination and filtering
   */
  const loadRoles = useCallback(async (
    pageNum = page,
    pageSizeNum = pageSize,
    filterOptions = filters
  ): Promise<void> => {
    try {
      setLoading(true);
      setError(null);
      
      // If search term is present, add it to filters
      const mergedFilters = searchTerm 
        ? { ...filterOptions, searchTerm } 
        : filterOptions;
      
      // Create cache key
      const cacheKey = `roles-${pageNum}-${pageSizeNum}-${JSON.stringify(mergedFilters)}`;
      
      // Check cache if enabled
      if (enableCache) {
        const cachedData = cacheRef.current[cacheKey];
        const now = Date.now();
        
        if (cachedData && now - cachedData.timestamp < cacheDuration) {
          setRoles(cachedData.data.items);
          setTotal(cachedData.data.totalCount);
          setPage(cachedData.data.page);
          setPageSize(cachedData.data.pageSize);
          setLoading(false);
          return;
        }
      }
      
      // Fetch from API
      const response = await roleService.getRoles({
        page: pageNum,
        pageSize: pageSizeNum,
        filters: mergedFilters,
      });
      
      // Update state
      setRoles(response.items);
      setTotal(response.totalCount);
      setPage(response.page);
      setPageSize(response.pageSize);
      
      // Update cache if enabled
      if (enableCache) {
        cacheRef.current[cacheKey] = {
          data: response,
          timestamp: Date.now(),
        };
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load roles');
      console.error('Error loading roles:', err);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, filters, searchTerm, enableCache, cacheDuration]);

  /**
   * Get a single role by ID
   */
  const getRole = useCallback(async (id: number): Promise<Role> => {
    try {
      setLoading(true);
      setError(null);
      
      const role = await roleService.getRole(id);
      return role;
    } catch (err) {
      setError(err instanceof Error ? err.message : `Failed to load role ${id}`);
      console.error(`Error loading role ${id}:`, err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  /**
   * Create a new role
   */
  const createRole = useCallback(async (role: CreateRoleRequest): Promise<Role> => {
    try {
      setLoading(true);
      setError(null);
      
      const newRole = await roleService.createRole(role);
      
      // Clear cache on write operations
      if (enableCache) {
        cacheRef.current = {};
      }
      
      // Refresh role list
      await loadRoles();
      
      return newRole;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create role');
      console.error('Error creating role:', err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [loadRoles, enableCache]);

  /**
   * Update an existing role
   */
  const updateRole = useCallback(async (id: number, role: UpdateRoleRequest): Promise<Role> => {
    try {
      setLoading(true);
      setError(null);
      
      const updatedRole = await roleService.updateRole(id, role);
      
      // Clear cache on write operations
      if (enableCache) {
        cacheRef.current = {};
      }
      
      // Refresh role list
      await loadRoles();
      
      return updatedRole;
    } catch (err) {
      setError(err instanceof Error ? err.message : `Failed to update role ${id}`);
      console.error(`Error updating role ${id}:`, err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [loadRoles, enableCache]);

  /**
   * Delete a role
   */
  const deleteRole = useCallback(async (id: number): Promise<boolean> => {
    try {
      setLoading(true);
      setError(null);
      
      const success = await roleService.deleteRole(id);
      
      // Clear cache on write operations
      if (enableCache) {
        cacheRef.current = {};
      }
      
      // Refresh role list
      await loadRoles();
      
      // Remove from selection if present
      if (selectedRoles.includes(id)) {
        setSelectedRoles(selectedRoles.filter(roleId => roleId !== id));
      }
      
      return success;
    } catch (err) {
      setError(err instanceof Error ? err.message : `Failed to delete role ${id}`);
      console.error(`Error deleting role ${id}:`, err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [loadRoles, enableCache, selectedRoles]);

  /**
   * Set filters and reload roles
   */
  const handleSetFilters = useCallback((newFilters: RoleFilterOptions): void => {
    setFilters(newFilters);
    // Reset to page 1 when filters change
    setPage(1);
    loadRoles(1, pageSize, newFilters);
  }, [loadRoles, pageSize]);

  /**
   * Clear all filters and reload roles
   */
  const clearFilters = useCallback((): void => {
    setFilters({});
    setSearchTerm(undefined);
    // Reset to page 1 when filters are cleared
    setPage(1);
    loadRoles(1, pageSize, {});
  }, [loadRoles, pageSize]);

  /**
   * Handle search with debouncing
   */
  const handleSetSearchTerm = useCallback((term: string): void => {
    setSearchTerm(term);
    
    // Clear previous timer
    if (searchDebounceTimerRef.current) {
      window.clearTimeout(searchDebounceTimerRef.current);
    }
    
    // Debounce search
    searchDebounceTimerRef.current = window.setTimeout(() => {
      // Reset to page 1 when search changes
      setPage(1);
      loadRoles(1, pageSize, filters);
    }, searchDebounceMs);
  }, [loadRoles, pageSize, filters, searchDebounceMs]);

  /**
   * Go to a specific page
   */
  const goToPage = useCallback(async (pageNum: number): Promise<void> => {
    if (pageNum < 1 || pageNum > totalPages) return;
    
    setPage(pageNum);
    await loadRoles(pageNum, pageSize, filters);
  }, [loadRoles, pageSize, filters, totalPages]);

  /**
   * Change page size
   */
  const handleSetPageSize = useCallback((size: number): void => {
    setPageSize(size);
    // Reset to page 1 when page size changes
    setPage(1);
    loadRoles(1, size, filters);
  }, [loadRoles, filters]);

  /**
   * Refresh current page
   */
  const refresh = useCallback(async (): Promise<void> => {
    // Clear cache for current page
    if (enableCache) {
      const cacheKey = `roles-${page}-${pageSize}-${JSON.stringify(filters)}`;
      delete cacheRef.current[cacheKey];
    }
    
    await loadRoles();
  }, [loadRoles, page, pageSize, filters, enableCache]);

  /**
   * Selection handling
   */
  const selectRole = useCallback((id: number): void => {
    setSelectedRoles(prev => {
      if (prev.includes(id)) return prev;
      return [...prev, id];
    });
  }, []);

  const deselectRole = useCallback((id: number): void => {
    setSelectedRoles(prev => prev.filter(roleId => roleId !== id));
  }, []);

  const selectAll = useCallback((): void => {
    setSelectedRoles(roles.map(role => role.id));
  }, [roles]);

  const deselectAll = useCallback((): void => {
    setSelectedRoles([]);
  }, []);

  /**
   * Role-User Management
   */
  const assignRoleToUser = useCallback(async (userId: number, roleId: number): Promise<boolean> => {
    try {
      setLoading(true);
      setError(null);
      
      const request: AssignRoleRequest = {
        userId,
        roleId,
      };
      
      const success = await roleService.assignRoleToUser(request);
      return success;
    } catch (err) {
      setError(err instanceof Error ? err.message : `Failed to assign role ${roleId} to user ${userId}`);
      console.error(`Error assigning role ${roleId} to user ${userId}:`, err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const removeRoleFromUser = useCallback(async (userId: number, roleId: number): Promise<boolean> => {
    try {
      setLoading(true);
      setError(null);
      
      const success = await roleService.removeRoleFromUser(userId, roleId);
      return success;
    } catch (err) {
      setError(err instanceof Error ? err.message : `Failed to remove role ${roleId} from user ${userId}`);
      console.error(`Error removing role ${roleId} from user ${userId}:`, err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const getUserRoles = useCallback(async (userId: number): Promise<UserRoleSummary> => {
    try {
      setLoading(true);
      setError(null);
      
      const userRoles = await roleService.getUserRoles(userId);
      return userRoles;
    } catch (err) {
      setError(err instanceof Error ? err.message : `Failed to get roles for user ${userId}`);
      console.error(`Error getting roles for user ${userId}:`, err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const updateUserRoles = useCallback(async (userId: number, roleIds: number[]): Promise<boolean> => {
    try {
      setLoading(true);
      setError(null);
      
      const request: UpdateUserRolesRequest = {
        userId,
        roleIds,
      };
      
      const success = await roleService.updateUserRoles(request);
      return success;
    } catch (err) {
      setError(err instanceof Error ? err.message : `Failed to update roles for user ${userId}`);
      console.error(`Error updating roles for user ${userId}:`, err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  /**
   * Permission Management
   */
  const loadPermissions = useCallback(async (): Promise<Permission[]> => {
    try {
      setLoading(true);
      setError(null);
      
      const perms = await roleService.getPermissions();
      setPermissions(perms);
      return perms;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load permissions');
      console.error('Error loading permissions:', err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const getRolePermissionMatrix = useCallback(async (roleId: number): Promise<RolePermissionMatrix> => {
    try {
      setLoading(true);
      setError(null);
      
      const matrix = await roleService.getRolePermissionMatrix(roleId);
      return matrix;
    } catch (err) {
      setError(err instanceof Error ? err.message : `Failed to load permission matrix for role ${roleId}`);
      console.error(`Error loading permission matrix for role ${roleId}:`, err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const updateRolePermissions = useCallback(async (roleId: number, permissionCodes: string[]): Promise<boolean> => {
    try {
      setLoading(true);
      setError(null);
      
      const success = await roleService.updateRolePermissions(roleId, permissionCodes);
      return success;
    } catch (err) {
      setError(err instanceof Error ? err.message : `Failed to update permissions for role ${roleId}`);
      console.error(`Error updating permissions for role ${roleId}:`, err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  // Initial load
  useEffect(() => {
    if (autoLoad) {
      loadRoles();
    }
    
    // Load permissions
    loadPermissions();
    
    // Cleanup debounce timer on unmount
    return () => {
      if (searchDebounceTimerRef.current) {
        window.clearTimeout(searchDebounceTimerRef.current);
      }
    };
  }, [autoLoad, loadRoles, loadPermissions]);

  return {
    // Data state
    roles,
    total,
    page,
    pageSize,
    totalPages,
    hasNextPage,
    hasPreviousPage,
    
    // Loading and error state
    loading,
    error,
    
    // Actions
    loadRoles,
    getRole,
    createRole,
    updateRole,
    deleteRole,
    
    // Search and filtering
    setFilters: handleSetFilters,
    clearFilters,
    searchTerm,
    setSearchTerm: handleSetSearchTerm,
    
    // Pagination
    goToPage,
    setPageSize: handleSetPageSize,
    refresh,
    
    // Selection
    selectedRoles,
    selectRole,
    deselectRole,
    selectAll,
    deselectAll,
    
    // Role-User Management
    assignRoleToUser,
    removeRoleFromUser,
    getUserRoles,
    updateUserRoles,
    
    // Permissions
    permissions,
    loadPermissions,
    getRolePermissionMatrix,
    updateRolePermissions,
  };
};
