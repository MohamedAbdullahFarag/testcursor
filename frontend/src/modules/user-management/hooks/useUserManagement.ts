/**
 * User Management Hook
 * Custom React hook for managing user operations with state management,
 * error handling, loading states, and data caching
 */

import { useState, useEffect, useCallback, useRef } from 'react';
import { userService } from '../services/userService';
import type {
  User,
  CreateUserDto,
  UpdateUserDto,
  UserFilters,
  PagedResult,
  BulkUserActionResult,
  UserExportFormat,
} from '../models/user.types';

/**
 * Configuration options for the useUserManagement hook
 */
export interface UseUserManagementOptions {
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
 * Return type for the useUserManagement hook
 */
export interface UseUserManagementReturn {
  // Data state
  users: User[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  
  // Loading and error state
  loading: boolean;
  error: string | null;
  isRefreshing: boolean;
  
  // Filter state
  filters: UserFilterOptions;
  
  // Selected users for bulk operations
  selectedUsers: number[];
  
  // Actions
  loadUsers: (page?: number, filters?: UserFilterOptions) => Promise<void>;
  refreshUsers: () => Promise<void>;
  createUser: (userData: CreateUserRequest) => Promise<User>;
  updateUser: (userData: UpdateUserRequest) => Promise<User>;
  deleteUser: (id: number) => Promise<boolean>;
  bulkDeleteUsers: (userIds: number[]) => Promise<BulkUserActionResult>;
  exportUsers: (format: UserExportFormat) => Promise<void>;
  
  // Filter actions
  setFilters: (filters: Partial<UserFilterOptions>) => void;
  resetFilters: () => void;
  searchUsers: (searchTerm: string) => void;
  
  // Pagination actions
  goToPage: (page: number) => Promise<void>;
  goToNextPage: () => Promise<void>;
  goToPreviousPage: () => Promise<void>;
  setPageSize: (pageSize: number) => Promise<void>;
  
  // Selection actions
  selectUser: (userId: number) => void;
  deselectUser: (userId: number) => void;
  selectAllUsers: () => void;
  deselectAllUsers: () => void;
  toggleUserSelection: (userId: number) => void;
  
  // Utility functions
  clearError: () => void;
  isUserSelected: (userId: number) => boolean;
  getSelectedUsersCount: () => number;
}

/**
 * Default filter options
 */
const DEFAULT_FILTERS: UserFilterOptions = {
  searchTerm: '',
  roles: [],
  isActive: undefined,
  emailVerified: undefined,
  preferredLanguage: undefined,
  sortBy: 'createdAt',
  sortDirection: 'desc',
};

/**
 * Custom hook for user management operations
 * Provides comprehensive state management and API integration for user CRUD operations
 */
export const useUserManagement = (
  options: UseUserManagementOptions = {}
): UseUserManagementReturn => {
  const {
    initialPageSize = 10,
    autoLoad = true,
    searchDebounceMs = 300,
    enableCache = true,
    cacheDuration = 300000, // 5 minutes
  } = options;

  // Core state
  const [users, setUsers] = useState<User[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSizeState] = useState(initialPageSize);
  const [totalPages, setTotalPages] = useState(0);
  const [hasNextPage, setHasNextPage] = useState(false);
  const [hasPreviousPage, setHasPreviousPage] = useState(false);
  
  // UI state
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isRefreshing, setIsRefreshing] = useState(false);
  
  // Filter state
  const [filters, setFiltersState] = useState<UserFilterOptions>(DEFAULT_FILTERS);
  
  // Selection state
  const [selectedUsers, setSelectedUsers] = useState<number[]>([]);
  
  // Refs for cleanup and caching
  const searchTimeoutRef = useRef<NodeJS.Timeout>();
  const cacheRef = useRef<Map<string, { data: UserListResponse; timestamp: number }>>(new Map());
  const abortControllerRef = useRef<AbortController>();

  /**
   * Generate cache key for current request parameters
   */
  const getCacheKey = useCallback((currentPage: number, currentFilters: UserFilterOptions): string => {
    return JSON.stringify({ page: currentPage, pageSize, filters: currentFilters });
  }, [pageSize]);

  /**
   * Check if cached data is still valid
   */
  const isCacheValid = useCallback((timestamp: number): boolean => {
    return enableCache && (Date.now() - timestamp) < cacheDuration;
  }, [enableCache, cacheDuration]);

  /**
   * Load users with optional caching
   */
  const loadUsers = useCallback(async (
    targetPage = page, 
    targetFilters = filters
  ): Promise<void> => {
    // Cancel any ongoing request
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }
    abortControllerRef.current = new AbortController();

    try {
      setLoading(true);
      setError(null);

      // Check cache first
      const cacheKey = getCacheKey(targetPage, targetFilters);
      const cached = cacheRef.current.get(cacheKey);
      
      if (cached && isCacheValid(cached.timestamp)) {
        const { data } = cached;
        setUsers(data.items);
        setTotal(data.total);
        setPage(data.page);
        setTotalPages(data.totalPages);
        setHasNextPage(data.hasNextPage);
        setHasPreviousPage(data.hasPreviousPage);
        setLoading(false);
        return;
      }

      // Fetch fresh data
      const response = await userService.getUsers({
        page: targetPage,
        pageSize,
        filters: targetFilters,
      });

      console.log('🔍 useUserManagement Debug - Service Response:', {
        responseType: typeof response,
        hasItems: !!response.items,
        itemsCount: response.items?.length || 0,
        total: response.total,
        page: response.page
      });

      // Update state
      setUsers(response.items);
      setTotal(response.total);
      setPage(response.page);
      setTotalPages(response.totalPages);
      setHasNextPage(response.hasNextPage);
      setHasPreviousPage(response.hasPreviousPage);

      console.log('✅ useUserManagement Debug - State Updated:', {
        usersLength: response.items.length,
        total: response.total,
        hasAnyUsers: response.items.length > 0
      });

      // Update cache
      if (enableCache) {
        cacheRef.current.set(cacheKey, {
          data: response,
          timestamp: Date.now(),
        });
      }

      // Clear selection when data changes
      setSelectedUsers([]);
      
    } catch (err) {
      if (err instanceof Error && err.name !== 'AbortError') {
        console.error('❌ useUserManagement Error:', {
          errorName: err.name,
          errorMessage: err.message,
          errorStack: err.stack
        });
        setError(err.message);
        console.error('Error loading users:', err);
      }
    } finally {
      setLoading(false);
    }
  }, [page, filters, pageSize, getCacheKey, isCacheValid, enableCache]);

  /**
   * Refresh current data
   */
  const refreshUsers = useCallback(async (): Promise<void> => {
    setIsRefreshing(true);
    // Clear cache for current parameters
    const cacheKey = getCacheKey(page, filters);
    cacheRef.current.delete(cacheKey);
    
    try {
      await loadUsers(page, filters);
    } finally {
      setIsRefreshing(false);
    }
  }, [loadUsers, page, filters, getCacheKey]);

  /**
   * Create a new user
   */
  const createUser = useCallback(async (userData: CreateUserRequest): Promise<User> => {
    try {
      setError(null);
      const newUser = await userService.createUser(userData);
      
      // Refresh the list to show the new user
      await refreshUsers();
      
      return newUser;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to create user';
      setError(errorMessage);
      throw err;
    }
  }, [refreshUsers]);

  /**
   * Update an existing user
   */
  const updateUser = useCallback(async (userData: UpdateUserRequest): Promise<User> => {
    try {
      setError(null);
      const updatedUser = await userService.updateUser(userData);
      
      // Update the user in the current list if it exists
      setUsers(currentUsers => 
        currentUsers.map(user => 
          user.id === updatedUser.id ? updatedUser : user
        )
      );
      
      // Clear cache to ensure fresh data on next load
      cacheRef.current.clear();
      
      return updatedUser;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to update user';
      setError(errorMessage);
      throw err;
    }
  }, []);

  /**
   * Delete a user
   */
  const deleteUser = useCallback(async (id: number): Promise<boolean> => {
    try {
      setError(null);
      const success = await userService.deleteUser(id);
      
      if (success) {
        // Remove user from selection if selected
        setSelectedUsers(current => current.filter(userId => userId !== id));
        
        // Refresh the list to reflect the deletion
        await refreshUsers();
      }
      
      return success;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to delete user';
      setError(errorMessage);
      throw err;
    }
  }, [refreshUsers]);

  /**
   * Bulk delete multiple users
   */
  const bulkDeleteUsers = useCallback(async (userIds: number[]): Promise<BulkUserActionResult> => {
    try {
      setError(null);
      const result = await userService.bulkDeleteUsers(userIds);
      
      // Clear selection
      setSelectedUsers([]);
      
      // Refresh the list to reflect the deletions
      await refreshUsers();
      
      return result;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to delete users';
      setError(errorMessage);
      throw err;
    }
  }, [refreshUsers]);

  /**
   * Export users
   */
  const exportUsers = useCallback(async (format: UserExportFormat): Promise<void> => {
    try {
      setError(null);
      const blob = await userService.exportUsers(format, { page, pageSize, filters });
      
      // Create download link
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `users-export.${format}`;
      document.body.appendChild(link);
      link.click();
      
      // Cleanup
      window.URL.revokeObjectURL(url);
      document.body.removeChild(link);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to export users';
      setError(errorMessage);
      throw err;
    }
  }, [page, pageSize, filters]);

  /**
   * Update filters
   */
  const setFilters = useCallback((newFilters: Partial<UserFilterOptions>) => {
    setFiltersState(current => ({ ...current, ...newFilters }));
    setPage(1); // Reset to first page when filters change
  }, []);

  /**
   * Reset filters to default
   */
  const resetFilters = useCallback(() => {
    setFiltersState(DEFAULT_FILTERS);
    setPage(1);
  }, []);

  /**
   * Search users with debouncing
   */
  const searchUsers = useCallback((searchTerm: string) => {
    // Clear existing timeout
    if (searchTimeoutRef.current) {
      clearTimeout(searchTimeoutRef.current);
    }

    // Set new timeout for debounced search
    searchTimeoutRef.current = setTimeout(() => {
      setFilters({ searchTerm });
    }, searchDebounceMs);
  }, [setFilters, searchDebounceMs]);

  /**
   * Navigation actions
   */
  const goToPage = useCallback(async (targetPage: number): Promise<void> => {
    if (targetPage >= 1 && targetPage <= totalPages && targetPage !== page) {
      setPage(targetPage);
      await loadUsers(targetPage, filters);
    }
  }, [totalPages, page, loadUsers, filters]);

  const goToNextPage = useCallback(async (): Promise<void> => {
    if (hasNextPage) {
      await goToPage(page + 1);
    }
  }, [hasNextPage, page, goToPage]);

  const goToPreviousPage = useCallback(async (): Promise<void> => {
    if (hasPreviousPage) {
      await goToPage(page - 1);
    }
  }, [hasPreviousPage, page, goToPage]);

  const setPageSize = useCallback(async (newPageSize: number): Promise<void> => {
    setPageSizeState(newPageSize);
    setPage(1); // Reset to first page when page size changes
    // Clear cache as page size has changed
    cacheRef.current.clear();
    await loadUsers(1, filters);
  }, [loadUsers, filters]);

  /**
   * Selection actions
   */
  const selectUser = useCallback((userId: number) => {
    setSelectedUsers(current => {
      if (!current.includes(userId)) {
        return [...current, userId];
      }
      return current;
    });
  }, []);

  const deselectUser = useCallback((userId: number) => {
    setSelectedUsers(current => current.filter(id => id !== userId));
  }, []);

  const selectAllUsers = useCallback(() => {
    setSelectedUsers(users.map(user => user.id));
  }, [users]);

  const deselectAllUsers = useCallback(() => {
    setSelectedUsers([]);
  }, []);

  const toggleUserSelection = useCallback((userId: number) => {
    setSelectedUsers(current => {
      if (current.includes(userId)) {
        return current.filter(id => id !== userId);
      } else {
        return [...current, userId];
      }
    });
  }, []);

  /**
   * Utility functions
   */
  const clearError = useCallback(() => {
    setError(null);
  }, []);

  const isUserSelected = useCallback((userId: number): boolean => {
    return selectedUsers.includes(userId);
  }, [selectedUsers]);

  const getSelectedUsersCount = useCallback((): number => {
    return selectedUsers.length;
  }, [selectedUsers]);

  /**
   * Effect to load data when filters change
   */
  useEffect(() => {
    loadUsers(page, filters);
  }, [filters]); // Don't include loadUsers in dependencies to avoid infinite loop

  /**
   * Effect to auto-load data on mount
   */
  useEffect(() => {
    if (autoLoad) {
      loadUsers();
    }
  }, []); // Only run on mount

  /**
   * Cleanup effect
   */
  useEffect(() => {
    return () => {
      // Cancel any ongoing requests
      if (abortControllerRef.current) {
        abortControllerRef.current.abort();
      }
      
      // Clear search timeout
      if (searchTimeoutRef.current) {
        clearTimeout(searchTimeoutRef.current);
      }
    };
  }, []);

  return {
    // Data state
    users,
    total,
    page,
    pageSize,
    totalPages,
    hasNextPage,
    hasPreviousPage,
    
    // Loading and error state
    loading,
    error,
    isRefreshing,
    
    // Filter state
    filters,
    
    // Selected users
    selectedUsers,
    
    // Actions
    loadUsers,
    refreshUsers,
    createUser,
    updateUser,
    deleteUser,
    bulkDeleteUsers,
    exportUsers,
    
    // Filter actions
    setFilters,
    resetFilters,
    searchUsers,
    
    // Pagination actions
    goToPage,
    goToNextPage,
    goToPreviousPage,
    setPageSize,
    
    // Selection actions
    selectUser,
    deselectUser,
    selectAllUsers,
    deselectAllUsers,
    toggleUserSelection,
    
    // Utility functions
    clearError,
    isUserSelected,
    getSelectedUsersCount,
  };
};
