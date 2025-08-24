import { apiClient } from '../../../shared/services/apiClient';
import { PagedResult } from '../../../modules/notifications/types/notification.types';
import { AuditLog, AuditLogFilter, AuditLogExportFormat, AuditLogIntegrityResult } from '../types/auditLogs';

/**
 * Service for interacting with audit log API
 */
export const auditLogService = {
  /**
   * Get audit logs with filtering and pagination
   * @param filter Filter options
   * @returns Paged result of audit logs
   */
  async getAuditLogs(filter: AuditLogFilter): Promise<PagedResult<AuditLog>> {
    try {
      const response = await apiClient.get<{data: PagedResult<AuditLog>}>(`/api/audit-logs`, { 
        params: filter
      });
      return response.data!.data;
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
      const response = await apiClient.get<{data: AuditLog[]}>(`/api/audit-logs/security-events`, { 
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
   * @param page Page number
   * @param pageSize Page size
   * @returns User-specific audit logs
   */
  async getUserAuditLogs(
    userId: number,
    fromDate?: Date,
    toDate?: Date,
    page: number = 1,
    pageSize: number = 25
  ): Promise<PagedResult<AuditLog>> {
    const params: Record<string, string | number> = {
      page,
      pageSize
    };
    
    if (fromDate) params.fromDate = fromDate.toISOString();
    if (toDate) params.toDate = toDate.toISOString();
    
    try {
      const response = await apiClient.get<{data: PagedResult<AuditLog>}>(
        `/api/audit-logs/user/${userId}`, 
        { params }
      );
      return response.data!.data;
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
    page: number = 1,
    pageSize: number = 25
  ): Promise<PagedResult<AuditLog>> {
    const params = { page, pageSize };
    try {
      const response = await apiClient.get<{data: PagedResult<AuditLog>}>(
        `/api/audit-logs/entity/${encodeURIComponent(entityType)}/${encodeURIComponent(entityId)}`,
        { params }
      );
      return response.data!.data;
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
        `/api/audit-logs/export`, 
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
        `/api/audit-logs/archive`,
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
        `/api/audit-logs/verify-integrity`, 
        { params }
      );
      return response.data!.data;
    } catch (error) {
      console.error('Error verifying logs integrity:', error);
      throw error;
    }
  }
};
