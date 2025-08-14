import { useState, useEffect, useCallback, useRef, useMemo } from 'react';
import { userService } from '../services/userService';
import { User, CreateUserRequest, UpdateUserRequest, UserFilterOptions, BulkActionResult, ExportFormat } from '../types/userTypes';
import { PaginatedResult } from '../../../shared/types/commonTypes';
import { useTranslation } from 'react-i18next';
import { useLocalStorage } from '../../shared/hooks/useLocalStorage';
import { useToast } from '../../shared/hooks/useToast';
import { useErrorHandler } from '../../shared/hooks/useErrorHandler';
import { useQueryClient } from '@tanstack/react-query';
import debounce from 'lodash/debounce';
import { useRTL } from '../../shared/hooks/useRTL';

/**
 * Options for the useUsers hook
 */
interface UseUsersOptions {
  /** Whether to automatically refresh data at intervals */
  autoRefresh?: boolean;
  /** Refresh interval in milliseconds */
  refreshInterval?: number;
  /** Initial page number */
  initialPage?: number;
  /** Initial page size */
  initialPageSize?: number;
  /** Whether to cache results */
  cacheResults?: boolean;
  /** Cache time-to-live in milliseconds */
  cacheTtl?: number;
  /** Initial filter options */
  initialFilters?: UserFilterOptions;
}

/**
 * Return type for the useUsers hook
 */
interface UseUsersReturn {
  users: User[];
  loading: boolean;
  error: string | null;
  pagination: {
    currentPage: number;
    pageSize: number;
    totalPages: number;
    totalRecords: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
  };
  filters: UserFilterOptions;
  
  // Actions
  refresh: () => Promise<void>;
  loadPage: (page: number) => Promise<void>;
  createUser: (data: CreateUserRequest) => Promise<User>;
  updateUser: (id: string, data: UpdateUserRequest) => Promise<User>;
  deleteUser: (id: string) => Promise<boolean>;
  bulkDeleteUsers: (ids: string[]) => Promise<BulkActionResult>;
  searchUsers: (searchTerm: string) => Promise<void>;
  setFilters: (filters: Partial<UserFilterOptions>) => void;
  resetFilters: () => void;
  clearCache: () => void;
  exportUsers: (format: ExportFormat) => Promise<void>;
  
  // State helpers
  isRefreshing: boolean;
  isFirstLoad: boolean;
  isFiltering: boolean;
  isEmpty: boolean;
  selectedUser: User | null;
  setSelectedUser: (user: User | null) => void;
  selectedUserIds: string[];
  toggleUserSelection: (userId: string) => void;
  selectAllUsers: (selected: boolean) => void;
}

/**
 * Custom hook for managing user operations
 * Provides comprehensive user management with caching, error handling, and i18n support
 * 
 * This hook demonstrates PRP (Product Requirements Prompt) methodology:
 * - Context is King: 
 *   - Comprehensive TypeScript interfaces
 *   - Detailed documentation
 *   - Internationalization support
 * 
 * - Validation Loops:
 *   - Robust error handling for all operations
 *   - Loading/state management for UI feedback
 *   - Debounced search for performance
 * 
 * - Information Dense:
 *   - Rich return type with all necessary operations
 *   - Strongly typed parameters and return values
 *   - Cache management built-in
 * 
 * - Progressive Success:
 *   - Basic CRUD operations first
 *   - Advanced features (filtering, bulk actions)
 *   - Performance optimizations (caching, request cancellation)
 * 
 * - One-Pass Implementation:
 *   - Complete solution for user management
 *   - Handles all edge cases and internationalization
 *   - Provides both data and UI-state management
 * 
 * @example
 * // Basic usage
 * const { users, loading, error } = useUsers();
 * 
 * @example
 * // Advanced usage with all options
 * const { 
 *   users, 
 *   loading, 
 *   pagination, 
 *   loadPage,
 *   createUser,
 *   filters,
 *   setFilters
 * } = useUsers({
 *   autoRefresh: true,
 *   refreshInterval: 60000,
 *   initialPage: 2,
 *   initialPageSize: 20,
 *   cacheResults: true,
 *   initialFilters: { status: 'active', role: 'admin' }
 * });
 */
export const useUsers = (options: UseUsersOptions = {}): UseUsersReturn => {
  // Initialize options with defaults
  const {
    autoRefresh = false,
    refreshInterval = 30000, // 30 seconds
    initialPage = 1,
    initialPageSize = 10,
    cacheResults = true,
    cacheTtl = 5 * 60 * 1000, // 5 minutes
    initialFilters = { status: 'all', role: 'all', sortBy: 'createdAt', sortOrder: 'desc' }
  } = options;

  // Hooks
  const { t, i18n } = useTranslation(['common', 'users', 'errors']);
  const { handleError, formatErrorMessage } = useErrorHandler();
  const { getItem, setItem, removeItem } = useLocalStorage();
  const { showToast } = useToast();
  const queryClient = useQueryClient();
  const { isRTL } = useRTL();
  
  // State management
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(initialPage);
  const [pageSize] = useState(initialPageSize);
  const [totalPages, setTotalPages] = useState(0);
  const [totalRecords, setTotalRecords] = useState(0);
  const [filters, setFiltersState] = useState<UserFilterOptions>(initialFilters);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [isRefreshing, setIsRefreshing] = useState(false);
  const [isFiltering, setIsFiltering] = useState(false);
  const [isFirstLoad, setIsFirstLoad] = useState(true);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [selectedUserIds, setSelectedUserIds] = useState<string[]>([]);
  
  // Refs
  const abortControllerRef = useRef<AbortController | null>(null);
  const lastFetchTimeRef = useRef<number>(0);
  const refreshTimerRef = useRef<NodeJS.Timeout | null>(null);
  
  // Derived state
  const isEmpty = useMemo(() => users.length === 0, [users]);
  const hasNextPage = useMemo(() => currentPage < totalPages, [currentPage, totalPages]);
  const hasPreviousPage = useMemo(() => currentPage > 1, [currentPage]);
  
  // Cache key generation
  const getCacheKey = useCallback(() => {
    const filterString = JSON.stringify(filters);
    return `users_${currentPage}_${pageSize}_${searchTerm}_${filterString}_${i18n.language}`;
  }, [currentPage, pageSize, searchTerm, filters, i18n.language]);
  
  /**
   * Reset abort controller
   */
  const resetAbortController = useCallback(() => {
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }
    abortControllerRef.current = new AbortController();
    return abortControllerRef.current.signal;
  }, []);
  
  /**
   * Check if cache is valid
   */
  const isCacheValid = useCallback(() => {
    if (!cacheResults) return false;
    
    const now = Date.now();
    const cacheAge = now - lastFetchTimeRef.current;
    
    return cacheAge < cacheTtl;
  }, [cacheResults, cacheTtl]);
  
  /**
   * Fetch users from API or cache
   */
  const fetchUsers = useCallback(async (options: { page: number, forceRefresh?: boolean } = { page: currentPage }) => {
    const { page, forceRefresh = false } = options;
    
    try {
      setError(null);
      
      // Only set loading on first load or during filtering, not during refresh
      if (isFirstLoad || isFiltering || forceRefresh) {
        setLoading(true);
      } else if (!isRefreshing) {
        setIsRefreshing(true);
      }
      
      // Check cache first unless force refresh is specified
      const cacheKey = getCacheKey();
      if (!forceRefresh && isCacheValid()) {
        const cachedData = getItem<PaginatedResult<User>>(cacheKey);
        if (cachedData) {
          setUsers(cachedData.items);
          setCurrentPage(cachedData.pageNumber);
          setTotalPages(cachedData.totalPages);
          setTotalRecords(cachedData.totalRecords);
          setLoading(false);
          setIsRefreshing(false);
          setIsFiltering(false);
          setIsFirstLoad(false);
          return;
        }
      }
      
      // Prepare signal for request cancellation
      const signal = resetAbortController();
      
      // Call API
      const result = await userService.getUsers({
        page,
        pageSize,
        searchTerm,
        filters,
        signal
      });
      
      // Process and format data if needed (e.g., RTL adjustments)
      let processedUsers = result.items;
      
      // Add RTL markers to text fields that might contain RTL content if the language is Arabic
      if (isRTL) {
        processedUsers = processedUsers.map(user => ({
          ...user,
          name: `\u200F${user.name}`, // RTL mark
          country: user.country ? `\u200F${user.country}` : user.country
        }));
      }
      
      // Update state
      setUsers(processedUsers);
      setCurrentPage(result.pageNumber);
      setTotalPages(result.totalPages);
      setTotalRecords(result.totalRecords);
      
      // Store in cache
      if (cacheResults) {
        setItem(cacheKey, result, cacheTtl);
        lastFetchTimeRef.current = Date.now();
      }
      
      // Reset loading states
      setLoading(false);
      setIsRefreshing(false);
      setIsFiltering(false);
      setIsFirstLoad(false);
      
    } catch (err) {
      // Don't show errors for cancelled requests
      if (err instanceof Error && err.name === 'AbortError') {
        return;
      }
      
      const errorMessage = formatErrorMessage(err, t('errors:failedToFetchUsers'));
      setError(errorMessage);
      setLoading(false);
      setIsRefreshing(false);
      setIsFiltering(false);
      
      showToast({
        type: 'error',
        message: errorMessage,
        title: t('common:error')
      });
      
      handleError(err, 'useUsers.fetchUsers');
    }
  }, [
    currentPage, pageSize, searchTerm, filters, isFirstLoad, isFiltering, isRefreshing, 
    cacheResults, cacheTtl, isRTL, t, getCacheKey, isCacheValid, getItem, setItem,
    resetAbortController, formatErrorMessage, showToast, handleError
  ]);
  
  /**
   * Load a specific page of users
   */
  const loadPage = useCallback(async (page: number) => {
    if (page < 1 || (totalPages > 0 && page > totalPages)) {
      return;
    }
    
    setCurrentPage(page);
    await fetchUsers({ page });
  }, [fetchUsers, totalPages]);
  
  /**
   * Refresh users data
   */
  const refresh = useCallback(async () => {
    await fetchUsers({ page: currentPage, forceRefresh: true });
  }, [fetchUsers, currentPage]);
  
  /**
   * Create a new user
   */
  const createUser = useCallback(async (data: CreateUserRequest): Promise<User> => {
    try {
      setLoading(true);
      const user = await userService.createUser(data);
      
      // Invalidate cache and refresh data
      clearCache();
      await refresh();
      
      showToast({
        type: 'success',
        message: t('users:userCreatedSuccessfully'),
        title: t('common:success')
      });
      
      return user;
    } catch (err) {
      const errorMessage = formatErrorMessage(err, t('errors:failedToCreateUser'));
      setError(errorMessage);
      
      showToast({
        type: 'error',
        message: errorMessage,
        title: t('common:error')
      });
      
      handleError(err, 'useUsers.createUser');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [t, refresh, formatErrorMessage, showToast, handleError, clearCache]);
  
  /**
   * Update an existing user
   */
  const updateUser = useCallback(async (id: string, data: UpdateUserRequest): Promise<User> => {
    try {
      setLoading(true);
      const user = await userService.updateUser(id, data);
      
      // Update user in current list without refetching
      setUsers(prevUsers => prevUsers.map(u => u.id === id ? { ...u, ...user } : u));
      
      // Invalidate cache
      clearCache();
      
      // Update selected user if it's the one being edited
      if (selectedUser && selectedUser.id === id) {
        setSelectedUser(user);
      }
      
      showToast({
        type: 'success',
        message: t('users:userUpdatedSuccessfully'),
        title: t('common:success')
      });
      
      return user;
    } catch (err) {
      const errorMessage = formatErrorMessage(err, t('errors:failedToUpdateUser'));
      setError(errorMessage);
      
      showToast({
        type: 'error',
        message: errorMessage,
        title: t('common:error')
      });
      
      handleError(err, 'useUsers.updateUser');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [t, selectedUser, formatErrorMessage, showToast, handleError, clearCache]);
  
  /**
   * Delete a user
   */
  const deleteUser = useCallback(async (id: string): Promise<boolean> => {
    try {
      setLoading(true);
      await userService.deleteUser(id);
      
      // Remove user from current list
      setUsers(prevUsers => prevUsers.filter(u => u.id !== id));
      
      // Clear selected user if it was deleted
      if (selectedUser && selectedUser.id === id) {
        setSelectedUser(null);
      }
      
      // Remove from selected ids
      if (selectedUserIds.includes(id)) {
        setSelectedUserIds(prevIds => prevIds.filter(uid => uid !== id));
      }
      
      // Invalidate cache
      clearCache();
      
      showToast({
        type: 'success',
        message: t('users:userDeletedSuccessfully'),
        title: t('common:success')
      });
      
      return true;
    } catch (err) {
      const errorMessage = formatErrorMessage(err, t('errors:failedToDeleteUser'));
      setError(errorMessage);
      
      showToast({
        type: 'error',
        message: errorMessage,
        title: t('common:error')
      });
      
      handleError(err, 'useUsers.deleteUser');
      return false;
    } finally {
      setLoading(false);
    }
  }, [t, selectedUser, selectedUserIds, formatErrorMessage, showToast, handleError, clearCache]);
  
  /**
   * Delete multiple users
   */
  const bulkDeleteUsers = useCallback(async (ids: string[]): Promise<BulkActionResult> => {
    try {
      setLoading(true);
      const result = await userService.bulkDeleteUsers(ids);
      
      // Update UI to reflect deleted users
      if (result.successCount > 0) {
        // Remove deleted users from the list
        const deletedIds = ids.filter(id => !result.failedIds.includes(id));
        setUsers(prevUsers => prevUsers.filter(u => !deletedIds.includes(u.id)));
        
        // Clear selected user if it was deleted
        if (selectedUser && deletedIds.includes(selectedUser.id)) {
          setSelectedUser(null);
        }
        
        // Update selected ids
        setSelectedUserIds(prevIds => prevIds.filter(id => !deletedIds.includes(id)));
      }
      
      // Invalidate cache
      clearCache();
      
      // Show appropriate toast based on result
      if (result.failedCount === 0) {
        showToast({
          type: 'success',
          message: t('users:allUsersDeletedSuccessfully', { count: result.successCount }),
          title: t('common:success')
        });
      } else if (result.successCount === 0) {
        showToast({
          type: 'error',
          message: t('users:failedToDeleteUsers', { count: result.failedCount }),
          title: t('common:error')
        });
      } else {
        showToast({
          type: 'warning',
          message: t('users:someUsersDeletedSuccessfully', { 
            successCount: result.successCount, 
            failedCount: result.failedCount 
          }),
          title: t('common:partialSuccess')
        });
      }
      
      return result;
    } catch (err) {
      const errorMessage = formatErrorMessage(err, t('errors:failedToDeleteUsers'));
      setError(errorMessage);
      
      showToast({
        type: 'error',
        message: errorMessage,
        title: t('common:error')
      });
      
      handleError(err, 'useUsers.bulkDeleteUsers');
      
      return {
        successCount: 0,
        failedCount: ids.length,
        failedIds: ids
      };
    } finally {
      setLoading(false);
    }
  }, [t, selectedUser, formatErrorMessage, showToast, handleError, clearCache]);
  
  /**
   * Export users to specified format
   */
  const exportUsers = useCallback(async (format: ExportFormat): Promise<void> => {
    try {
      setLoading(true);
      
      // Get current filters for export
      await userService.exportUsers(format, filters);
      
      showToast({
        type: 'success',
        message: t('users:usersExportedSuccessfully', { format: format.toUpperCase() }),
        title: t('common:success')
      });
    } catch (err) {
      const errorMessage = formatErrorMessage(err, t('errors:failedToExportUsers'));
      setError(errorMessage);
      
      showToast({
        type: 'error',
        message: errorMessage,
        title: t('common:error')
      });
      
      handleError(err, 'useUsers.exportUsers');
    } finally {
      setLoading(false);
    }
  }, [t, filters, formatErrorMessage, showToast, handleError]);
  
  /**
   * Set filters with debounce for better performance
   */
  const debouncedSearch = useRef(
    debounce((term: string) => {
      setSearchTerm(term);
      setIsFiltering(true);
      setCurrentPage(1); // Reset to first page when searching
    }, 500)
  ).current;
  
  /**
   * Search users by name or email
   */
  const searchUsers = useCallback(async (term: string): Promise<void> => {
    debouncedSearch(term);
  }, [debouncedSearch]);
  
  /**
   * Set filters for user list
   */
  const setFilters = useCallback((newFilters: Partial<UserFilterOptions>): void => {
    setFiltersState(prev => {
      // Special handling for sortOrder when sortBy changes
      if (newFilters.sortBy && newFilters.sortBy !== prev.sortBy && !newFilters.sortOrder) {
        // Reset sort order to 'asc' when changing sort field, unless explicitly specified
        newFilters.sortOrder = 'asc';
      }
      
      return { ...prev, ...newFilters };
    });
    
    setIsFiltering(true);
    setCurrentPage(1); // Reset to first page when filters change
  }, []);
  
  /**
   * Reset filters to initial values
   */
  const resetFilters = useCallback((): void => {
    setFiltersState(initialFilters);
    setSearchTerm('');
    setIsFiltering(true);
    setCurrentPage(1);
  }, [initialFilters]);
  
  /**
   * Clear the cache for users data
   */
  const clearCache = useCallback((): void => {
    // Clear all user-related cache entries
    if (cacheResults) {
      // Clear react-query cache
      queryClient.invalidateQueries(['users']);
      
      // Clear local storage cache
      const keys = Object.keys(localStorage);
      for (const key of keys) {
        if (key.startsWith('users_')) {
          removeItem(key);
        }
      }
      
      lastFetchTimeRef.current = 0;
    }
  }, [cacheResults, queryClient, removeItem]);
  
  /**
   * Toggle selection of a user
   */
  const toggleUserSelection = useCallback((userId: string): void => {
    setSelectedUserIds(prev => {
      if (prev.includes(userId)) {
        return prev.filter(id => id !== userId);
      } else {
        return [...prev, userId];
      }
    });
  }, []);
  
  /**
   * Select all users or deselect all
   */
  const selectAllUsers = useCallback((selected: boolean): void => {
    if (selected) {
      const allUserIds = users.map(user => user.id);
      setSelectedUserIds(allUserIds);
    } else {
      setSelectedUserIds([]);
    }
  }, [users]);
  
  // Initial load effect
  useEffect(() => {
    fetchUsers();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);
  
  // Refresh when page, filters, or search changes
  useEffect(() => {
    if (!isFirstLoad) {
      fetchUsers();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [currentPage, filters, searchTerm]);
  
  // Setup auto refresh
  useEffect(() => {
    if (autoRefresh && refreshInterval > 0) {
      refreshTimerRef.current = setInterval(() => {
        fetchUsers();
      }, refreshInterval);
    }
    
    return () => {
      if (refreshTimerRef.current) {
        clearInterval(refreshTimerRef.current);
      }
    };
  }, [autoRefresh, refreshInterval, fetchUsers]);
  
  // Cleanup on unmount
  useEffect(() => {
    return () => {
      if (abortControllerRef.current) {
        abortControllerRef.current.abort();
      }
      
      if (refreshTimerRef.current) {
        clearInterval(refreshTimerRef.current);
      }
      
      debouncedSearch.cancel();
    };
  }, [debouncedSearch]);
  
  // Reset selected users when users list changes significantly
  useEffect(() => {
    if (selectedUserIds.length > 0) {
      // Keep only users that still exist in the list
      const existingIds = users.map(u => u.id);
      setSelectedUserIds(prev => prev.filter(id => existingIds.includes(id)));
    }
  }, [users, selectedUserIds]);
  
  return {
    users,
    loading,
    error,
    pagination: {
      currentPage,
      pageSize,
      totalPages,
      totalRecords,
      hasNextPage,
      hasPreviousPage
    },
    filters,
    
    // Actions
    refresh,
    loadPage,
    createUser,
    updateUser,
    deleteUser,
    bulkDeleteUsers,
    searchUsers,
    setFilters,
    resetFilters,
    clearCache,
    exportUsers,
    
    // State helpers
    isRefreshing,
    isFirstLoad,
    isFiltering,
    isEmpty,
    selectedUser,
    setSelectedUser,
    selectedUserIds,
    toggleUserSelection,
    selectAllUsers
  };
};