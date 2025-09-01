import { apiClient } from '../../../shared/services/apiClient';
import { PagedResult } from '../../../shared/types/common';
import { ApiResponse } from '../../../shared/models/api';
import { AuditLog, AuditLogFilter, AuditLogExportFormat, AuditLogIntegrityResult } from '../types/auditLogs';

// Backend response structure for audit logs
interface BackendPagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

/**
 * Service for interacting with audit log API
 */
export const auditLogService = {
  // Use the API URL from environment variables
  baseUrl: import.meta.env.VITE_API_URL || 'https://localhost:7001/api',

  /**
   * Get audit logs with filtering and pagination
   * @param filter Filter options
   * @returns Paged result of audit logs
   */
  async getAuditLogs(filter: AuditLogFilter): Promise<PagedResult<AuditLog>> {
    try {
      // Convert 0-based page to 1-based for backend
      const backendFilter = {
        ...filter,
        page: filter.page + 1
      };
      
      // The audit logs API returns the PagedResult directly, not wrapped in ApiResponse
      const response = await apiClient.get(`${this.baseUrl}/audit-logs`, { 
        params: backendFilter
      });
      
      // Check if this is a direct PagedResult (not wrapped in ApiResponse)
      let backendData: BackendPagedResult<AuditLog>;
      
      if ('success' in response && response.success !== undefined) {
        // This is an ApiResponse wrapper
        const apiResponse = response as ApiResponse<BackendPagedResult<AuditLog>>;
        if (!apiResponse.success) {
          const errorMsg = apiResponse.message || 'API request failed';
          throw new Error(errorMsg);
        }
        backendData = apiResponse.data as BackendPagedResult<AuditLog>;
      } else {
        // This is a direct PagedResult
        backendData = response as unknown as BackendPagedResult<AuditLog>;
      }
      
      if (!backendData) {
        return {
          data: [],
          total: 0,
          page: 0,
          pageSize: 25,
          totalPages: 0,
          hasNext: false,
          hasPrevious: false
        };
      }
      
      return {
        data: backendData.items || [],
        total: backendData.totalCount || 0,
        page: (backendData.pageNumber || 1) - 1, // Convert to 0-based
        pageSize: backendData.pageSize || 25,
        totalPages: backendData.totalPages || Math.ceil((backendData.totalCount || 0) / (backendData.pageSize || 25)),
        hasNext: backendData.hasNextPage || false,
        hasPrevious: backendData.hasPreviousPage || false
      };
    } catch (error) {
      console.error('Error fetching audit logs:', error);
      throw error;
    }
  },

  /**
   * Get security events for a time period
   * @param fromDate Start date
   * @param toDate End date
   * @returns Security events
   */
  async getSecurityEvents(fromDate?: Date, toDate?: Date): Promise<AuditLog[]> {
    const params: Record<string, string> = {};
    if (fromDate) params.fromDate = fromDate.toISOString();
    if (toDate) params.toDate = toDate.toISOString();
    
    try {
      const response = await apiClient.get<{data: AuditLog[]}>(`${this.baseUrl}/audit-logs/security-events`, { 
        params
      });
      return response.data!.data;
    } catch (error) {
      console.error('Error fetching security events:', error);
      throw error;
    }
  },

  /**
   * Get audit logs for a specific user
   * @param userId User ID
   * @param fromDate Start date
   * @param toDate End date
   * @param page Page number (0-based)
   * @param pageSize Page size
   * @returns User-specific audit logs
   */
  async getUserAuditLogs(
    userId: number,
    fromDate?: Date,
    toDate?: Date,
    page: number = 0,
    pageSize: number = 25
  ): Promise<PagedResult<AuditLog>> {
    const params: Record<string, string | number> = {
      page: page + 1, // Convert to 1-based for backend
      pageSize
    };
    
    if (fromDate) params.fromDate = fromDate.toISOString();
    if (toDate) params.toDate = toDate.toISOString();
    
    try {
      const response: ApiResponse<BackendPagedResult<AuditLog>> = await apiClient.get(
        `${this.baseUrl}/audit-logs/user/${userId}`, 
        { params }
      );
      
      // Check API response success
      if (!response.success) {
        throw new Error(response.message || 'API request failed');
      }
      
      if (!response.data) {
        throw new Error('No data received from API');
      }
      
      // Transform backend response to match frontend pagination structure
      const backendData = response.data;
      return {
        data: backendData.items || [],
        total: backendData.totalCount || 0,
        page: (backendData.pageNumber || 1) - 1, // Convert to 0-based
        pageSize: backendData.pageSize || 25,
        totalPages: backendData.totalPages || Math.ceil((backendData.totalCount || 0) / (backendData.pageSize || 25)),
        hasNext: backendData.hasNextPage || false,
        hasPrevious: backendData.hasPreviousPage || false
      };
    } catch (error) {
      console.error(`Error fetching audit logs for user ${userId}:`, error);
      throw error;
    }
  },

  /**
   * Get audit logs for a specific entity
   * @param entityType Entity type
   * @param entityId Entity ID
   * @param page Page number
   * @param pageSize Page size
   * @returns Entity-specific audit logs
   */
  async getEntityAuditLogs(
    entityType: string,
    entityId: string,
    page: number = 0,
    pageSize: number = 25
  ): Promise<PagedResult<AuditLog>> {
    const params = { 
      page: page + 1, // Convert to 1-based for backend
      pageSize 
    };
    try {
      const response: ApiResponse<BackendPagedResult<AuditLog>> = await apiClient.get(
        `${this.baseUrl}/audit-logs/entity/${encodeURIComponent(entityType)}/${encodeURIComponent(entityId)}`,
        { params }
      );
      
      // Check API response success
      if (!response.success) {
        throw new Error(response.message || 'API request failed');
      }
      
      if (!response.data) {
        throw new Error('No data received from API');
      }
      
      // Transform backend response to match frontend pagination structure
      const backendData = response.data;
      return {
        data: backendData.items || [],
        total: backendData.totalCount || 0,
        page: (backendData.pageNumber || 1) - 1, // Convert to 0-based
        pageSize: backendData.pageSize || 25,
        totalPages: backendData.totalPages || Math.ceil((backendData.totalCount || 0) / (backendData.pageSize || 25)),
        hasNext: backendData.hasNextPage || false,
        hasPrevious: backendData.hasPreviousPage || false
      };
    } catch (error) {
      console.error(`Error fetching audit logs for entity ${entityType}/${entityId}:`, error);
      throw error;
    }
  },

  /**
   * Export audit logs based on filter criteria
   * @param filter Filter options
   * @param format Export format
   * @returns File blob for download
   */
  async exportAuditLogs(filter: AuditLogFilter, format: AuditLogExportFormat): Promise<Blob> {
    try {
      const response = await apiClient.post<Blob>(
        `${this.baseUrl}/audit-logs/export`, 
        filter, 
        {
          params: { format },
          responseType: 'blob'
        }
      );
      return response.data!;
    } catch (error) {
      console.error('Error exporting audit logs:', error);
      throw error;
    }
  },

  /**
   * Archive old audit logs
   * @param retentionDays Number of days to retain logs
   * @returns Archive result
   */
  async archiveOldLogs(retentionDays: number = 365): Promise<{ archivedCount: number; message: string }> {
    try {
      const response = await apiClient.post<{data: { archivedCount: number; message: string }}>(
        `${this.baseUrl}/audit-logs/archive`,
        {},
        { 
          params: { retentionDays }
        }
      );
      return response.data!.data;
    } catch (error) {
      console.error('Error archiving old logs:', error);
      throw error;
    }
  },

  /**
   * Verify integrity of audit logs
   * @param fromDate Start date
   * @param toDate End date
   * @returns Verification results
   */
  async verifyLogsIntegrity(
    fromDate?: Date,
    toDate?: Date
  ): Promise<AuditLogIntegrityResult> {
    const params: Record<string, string> = {};
    if (fromDate) params.fromDate = fromDate.toISOString();
    if (toDate) params.toDate = toDate.toISOString();
    
    try {
      const response = await apiClient.get<{data: AuditLogIntegrityResult}>(
        `${this.baseUrl}/audit-logs/verify-integrity`, 
        { params }
      );
      return response.data!.data;
    } catch (error) {
      console.error('Error verifying logs integrity:', error);
      throw error;
    }
  }
};
