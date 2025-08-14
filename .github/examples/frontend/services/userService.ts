import { ApiResponse, PaginatedResult } from '../../../shared/types/commonTypes';
import { 
  User, 
  CreateUserRequest, 
  UpdateUserRequest, 
  UserFilterOptions, 
  BulkActionResult,
  ExportFormat
} from '../types/userTypes';
import { api } from '../../../shared/services/api';
import { getAuthHeaders } from '../../../shared/services/authService';
import { i18n } from '../../../shared/i18n';
import { AppConfig } from '../../../shared/config';

/**
 * Service for handling all user-related API calls
 * Implements the folder-per-feature architecture and provides comprehensive API operations
 * with proper error handling, internationalization, and security measures
 * 
 * This service demonstrates PRP (Product Requirements Prompt) methodology:
 * 
 * - Context is King:
 *   - Comprehensive error handling with localized messages
 *   - Clear API endpoint structure following REST conventions
 *   - Complete type safety through TypeScript interfaces
 * 
 * - Validation Loops:
 *   - Client-side validation before API calls
 *   - Structured error handling for all response types
 *   - Support for AbortController for request cancellation
 * 
 * - Information Dense:
 *   - Rich API surface covering all user operations
 *   - Detailed documentation for each method
 *   - Built-in internationalization for error messages
 * 
 * - Progressive Success:
 *   - Basic CRUD operations first (get, create, update, delete)
 *   - Advanced operations (bulk, patch, export) building on core functionality
 *   - Specialized operations (change status, set language) for specific needs
 * 
 * - One-Pass Implementation:
 *   - Complete user management without requiring multiple iterations
 *   - Handles edge cases like file downloads and RTL languages
 *   - Includes security considerations like proper auth headers
 */
class UserService {
  private readonly baseUrl = '/api/users';
  
  /**
   * Get paginated list of users with optional filtering and search
   * @param options Request options for pagination, search, and filters
   */
  async getUsers(options: {
    page: number;
    pageSize: number;
    searchTerm?: string;
    filters?: UserFilterOptions;
    signal?: AbortSignal;
  }): Promise<PaginatedResult<User>> {
    try {
      const { page, pageSize, searchTerm, filters = {}, signal } = options;
      
      // Build query parameters
      const params = new URLSearchParams();
      params.append('page', page.toString());
      params.append('pageSize', pageSize.toString());
      
      if (searchTerm) {
        params.append('searchTerm', searchTerm);
      }
      
      // Add filter parameters if they exist and aren't set to 'all'
      if (filters.status && filters.status !== 'all') {
        params.append('status', filters.status);
      }
      
      if (filters.role && filters.role !== 'all') {
        params.append('role', filters.role);
      }
      
      // Add sorting parameters
      if (filters.sortBy) {
        params.append('sortBy', filters.sortBy);
        params.append('sortOrder', filters.sortOrder || 'asc');
      }
      
      // Add date range filters if provided
      if (filters.startDate) {
        params.append('startDate', filters.startDate);
      }
      
      if (filters.endDate) {
        params.append('endDate', filters.endDate);
      }
      
      // Add country filter if provided
      if (filters.country) {
        params.append('country', filters.country);
      }
      
      // Add language filter if provided
      if (filters.preferredLanguage) {
        params.append('preferredLanguage', filters.preferredLanguage);
      }
      
      // Make API call with AbortController support for cancellation
      const response = await api.get<ApiResponse<PaginatedResult<User>>>(
        `${this.baseUrl}?${params.toString()}`,
        {
          headers: await getAuthHeaders(),
          signal
        }
      );
      
      return response.data.data;
    } catch (error) {
      this.handleError(error, 'Error fetching users');
      throw error;
    }
  }
  
  /**
   * Get user by ID
   * @param id User ID
   */
  async getUserById(id: string): Promise<User> {
    try {
      const response = await api.get<ApiResponse<User>>(
        `${this.baseUrl}/${id}`,
        {
          headers: await getAuthHeaders()
        }
      );
      
      return response.data.data;
    } catch (error) {
      this.handleError(error, `Error fetching user with ID ${id}`);
      throw error;
    }
  }
  
  /**
   * Create a new user
   * @param data User creation data
   */
  async createUser(data: CreateUserRequest): Promise<User> {
    try {
      // Client-side validation
      this.validateUserData(data);
      
      const response = await api.post<ApiResponse<User>>(
        this.baseUrl,
        data,
        {
          headers: await getAuthHeaders()
        }
      );
      
      return response.data.data;
    } catch (error) {
      this.handleError(error, 'Error creating user');
      throw error;
    }
  }
  
  /**
   * Update an existing user
   * @param id User ID
   * @param data User update data
   */
  async updateUser(id: string, data: UpdateUserRequest): Promise<User> {
    try {
      // Client-side validation for update
      this.validateUpdateData(data);
      
      const response = await api.put<ApiResponse<User>>(
        `${this.baseUrl}/${id}`,
        data,
        {
          headers: await getAuthHeaders()
        }
      );
      
      return response.data.data;
    } catch (error) {
      this.handleError(error, `Error updating user with ID ${id}`);
      throw error;
    }
  }
  
  /**
   * Partially update a user with PATCH
   * @param id User ID
   * @param data Partial update data
   */
  async patchUser(id: string, data: Partial<UpdateUserRequest>): Promise<User> {
    try {
      // Only include fields that are provided
      const patchData = Object.fromEntries(
        Object.entries(data).filter(([_, v]) => v !== undefined)
      );
      
      const response = await api.patch<ApiResponse<User>>(
        `${this.baseUrl}/${id}`,
        patchData,
        {
          headers: await getAuthHeaders()
        }
      );
      
      return response.data.data;
    } catch (error) {
      this.handleError(error, `Error patching user with ID ${id}`);
      throw error;
    }
  }
  
  /**
   * Delete a user
   * @param id User ID
   */
  async deleteUser(id: string): Promise<void> {
    try {
      await api.delete<ApiResponse<void>>(
        `${this.baseUrl}/${id}`,
        {
          headers: await getAuthHeaders()
        }
      );
    } catch (error) {
      this.handleError(error, `Error deleting user with ID ${id}`);
      throw error;
    }
  }
  
  /**
   * Delete multiple users in a single request
   * @param ids Array of user IDs to delete
   */
  async bulkDeleteUsers(ids: string[]): Promise<BulkActionResult> {
    try {
      const response = await api.post<ApiResponse<BulkActionResult>>(
        `${this.baseUrl}/bulk-delete`,
        { ids },
        {
          headers: await getAuthHeaders()
        }
      );
      
      return response.data.data;
    } catch (error) {
      this.handleError(error, 'Error deleting multiple users');
      throw error;
    }
  }
  
  /**
   * Change user status (activate/deactivate)
   * @param id User ID
   * @param isActive New active status
   */
  async changeUserStatus(id: string, isActive: boolean): Promise<User> {
    try {
      const response = await api.put<ApiResponse<User>>(
        `${this.baseUrl}/${id}/status`,
        { isActive },
        {
          headers: await getAuthHeaders()
        }
      );
      
      return response.data.data;
    } catch (error) {
      this.handleError(error, `Error changing status for user with ID ${id}`);
      throw error;
    }
  }
  
  /**
   * Set user preferred language
   * @param id User ID
   * @param languageCode Language code ('en' or 'ar')
   */
  async setUserPreferredLanguage(id: string, languageCode: string): Promise<User> {
    try {
      // Validate language code
      if (languageCode !== 'en' && languageCode !== 'ar') {
        throw new Error('Unsupported language code');
      }
      
      const response = await api.put<ApiResponse<User>>(
        `${this.baseUrl}/${id}/language`,
        { languageCode },
        {
          headers: await getAuthHeaders()
        }
      );
      
      return response.data.data;
    } catch (error) {
      this.handleError(error, `Error setting language for user with ID ${id}`);
      throw error;
    }
  }
  
  /**
   * Get user statistics
   * @param includeInactive Whether to include inactive users in stats
   */
  async getUserStatistics(includeInactive = false): Promise<any> {
    try {
      const params = new URLSearchParams();
      params.append('includeInactive', includeInactive.toString());
      
      const response = await api.get<ApiResponse<any>>(
        `${this.baseUrl}/statistics?${params.toString()}`,
        {
          headers: await getAuthHeaders()
        }
      );
      
      return response.data.data;
    } catch (error) {
      this.handleError(error, 'Error fetching user statistics');
      throw error;
    }
  }
  
  /**
   * Export users to specified format
   * @param format Export format (csv, excel, pdf)
   * @param filters Optional filters to apply to export
   */
  async exportUsers(format: ExportFormat, filters?: UserFilterOptions): Promise<void> {
    try {
      // Build query parameters
      const params = new URLSearchParams();
      params.append('format', format);
      
      // Add filter parameters if provided
      if (filters) {
        if (filters.status && filters.status !== 'all') {
          params.append('status', filters.status);
        }
        
        if (filters.role && filters.role !== 'all') {
          params.append('role', filters.role);
        }
        
        if (filters.startDate) {
          params.append('startDate', filters.startDate);
        }
        
        if (filters.endDate) {
          params.append('endDate', filters.endDate);
        }
        
        if (filters.country) {
          params.append('country', filters.country);
        }
        
        if (filters.preferredLanguage) {
          params.append('preferredLanguage', filters.preferredLanguage);
        }
      }
      
      // Get current language for proper localization
      params.append('language', i18n.language);
      
      // Use blob response type for file download
      const response = await api.get(
        `${this.baseUrl}/export?${params.toString()}`,
        {
          headers: await getAuthHeaders(),
          responseType: 'blob'
        }
      );
      
      // Create file name with timestamp
      const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
      const fileName = i18n.language === 'ar' ? `المستخدمين_${timestamp}` : `users_${timestamp}`;
      const fileExtension = this.getFileExtension(format);
      
      // Create file download
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `${fileName}.${fileExtension}`);
      document.body.appendChild(link);
      link.click();
      link.remove();
      
      // Clean up URL object
      setTimeout(() => window.URL.revokeObjectURL(url), 100);
    } catch (error) {
      this.handleError(error, `Error exporting users as ${format}`);
      throw error;
    }
  }
  
  /**
   * Handle API errors with proper logging and internationalization
   * @param error The error object
   * @param fallbackMessage Fallback message if error is not structured
   * @private
   */
  private handleError(error: any, fallbackMessage: string): void {
    // Log error to monitoring service if available
    if (AppConfig.enableErrorLogging) {
      console.error('[UserService]', error);
      
      // Example of sending to monitoring service
      // monitoringService.logError(error, {
      //   component: 'UserService',
      //   additionalInfo: fallbackMessage
      // });
    }
    
    // Throw appropriate error based on response
    if (error.response) {
      // Server responded with error status
      const status = error.response.status;
      const message = error.response.data?.message || fallbackMessage;
      
      if (status === 401) {
        throw new Error(i18n.t('errors:unauthorized', { defaultValue: 'Unauthorized access' }));
      } else if (status === 403) {
        throw new Error(i18n.t('errors:forbidden', { defaultValue: 'Access forbidden' }));
      } else if (status === 404) {
        throw new Error(i18n.t('errors:userNotFound', { defaultValue: 'User not found' }));
      } else if (status === 409) {
        throw new Error(i18n.t('errors:conflictingData', { defaultValue: message }));
      } else if (status === 422) {
        const errors = error.response.data?.errors || [message];
        throw new Error(errors.join('. '));
      } else {
        throw new Error(i18n.t('errors:serverError', { defaultValue: message }));
      }
    } else if (error.request) {
      // Request was made but no response received
      throw new Error(i18n.t('errors:noResponse', { defaultValue: 'No response from server' }));
    } else if (error.name === 'AbortError') {
      // Request was aborted
      throw error;
    } else {
      // Something else happened
      throw new Error(i18n.t('errors:unexpectedError', { defaultValue: fallbackMessage }));
    }
  }
  
  /**
   * Validate user data before sending to API
   * @param data User creation data
   * @private
   */
  private validateUserData(data: CreateUserRequest): void {
    const errors = [];
    
    if (!data.name || data.name.trim() === '') {
      errors.push(i18n.t('users:nameIsRequired', { defaultValue: 'Name is required' }));
    }
    
    if (!data.email || data.email.trim() === '') {
      errors.push(i18n.t('users:emailIsRequired', { defaultValue: 'Email is required' }));
    } else if (!this.isValidEmail(data.email)) {
      errors.push(i18n.t('users:invalidEmailFormat', { defaultValue: 'Invalid email format' }));
    }
    
    if (!data.password || data.password.trim() === '') {
      errors.push(i18n.t('users:passwordIsRequired', { defaultValue: 'Password is required' }));
    } else if (data.password.length < 8) {
      errors.push(i18n.t('users:passwordTooShort', { defaultValue: 'Password must be at least 8 characters' }));
    }
    
    if (data.password !== data.confirmPassword) {
      errors.push(i18n.t('users:passwordsDoNotMatch', { defaultValue: 'Passwords do not match' }));
    }
    
    if (!data.role || data.role.trim() === '') {
      errors.push(i18n.t('users:roleIsRequired', { defaultValue: 'Role is required' }));
    }
    
    if (data.phoneNumber && !this.isValidPhoneNumber(data.phoneNumber)) {
      errors.push(i18n.t('users:invalidPhoneFormat', { defaultValue: 'Invalid phone number format' }));
    }
    
    if (errors.length > 0) {
      throw new Error(errors.join('. '));
    }
  }
  
  /**
   * Validate update data before sending to API
   * @param data User update data
   * @private
   */
  private validateUpdateData(data: UpdateUserRequest): void {
    const errors = [];
    
    if (data.name !== undefined && data.name.trim() === '') {
      errors.push(i18n.t('users:nameIsRequired', { defaultValue: 'Name is required' }));
    }
    
    if (data.email !== undefined) {
      if (data.email.trim() === '') {
        errors.push(i18n.t('users:emailIsRequired', { defaultValue: 'Email is required' }));
      } else if (!this.isValidEmail(data.email)) {
        errors.push(i18n.t('users:invalidEmailFormat', { defaultValue: 'Invalid email format' }));
      }
    }
    
    if (data.phoneNumber !== undefined && data.phoneNumber !== '' && !this.isValidPhoneNumber(data.phoneNumber)) {
      errors.push(i18n.t('users:invalidPhoneFormat', { defaultValue: 'Invalid phone number format' }));
    }
    
    if (data.preferredLanguage !== undefined && data.preferredLanguage !== 'en' && data.preferredLanguage !== 'ar') {
      errors.push(i18n.t('users:unsupportedLanguage', { defaultValue: 'Unsupported language code' }));
    }
    
    if (errors.length > 0) {
      throw new Error(errors.join('. '));
    }
  }
  
  /**
   * Check if email format is valid
   * @param email Email to validate
   * @private
   */
  private isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }
  
  /**
   * Check if phone number format is valid
   * @param phone Phone number to validate
   * @private
   */
  private isValidPhoneNumber(phone: string): boolean {
    // Basic validation allowing for international format
    const phoneRegex = /^\+?[0-9\s\-\(\)]{8,20}$/;
    return phoneRegex.test(phone);
  }
  
  /**
   * Get file extension for export format
   * @param format Export format
   * @private
   */
  private getFileExtension(format: string): string {
    switch (format.toLowerCase()) {
      case 'csv':
        return 'csv';
      case 'excel':
        return 'xlsx';
      case 'pdf':
        return 'pdf';
      default:
        return 'txt';
    }
  }
}

// Export singleton instance
export const userService = new UserService();
