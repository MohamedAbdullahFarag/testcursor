import axios from 'axios';
import { PagedResult } from '../../../modules/notifications/types/notification.types';
import { AuditLog, AuditLogFilter, AuditLogExportFormat, AuditLogIntegrityResult } from '../types/auditLogs';

// Use axios directly with the API base URL
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

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
    const response = await axios.get<{data: PagedResult<AuditLog>}>(`${API_URL}/api/audit-logs`, { 
      params: filter,
      headers: this.getAuthHeaders() 
    });
    return response.data.data;
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
    
    const response = await axios.get<{data: AuditLog[]}>(`${API_URL}/api/audit-logs/security-events`, { 
      params,
      headers: this.getAuthHeaders() 
    });
    return response.data.data;
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
    pageSize: number = 20
  ): Promise<PagedResult<AuditLog>> {
    const params: Record<string, string | number> = {
      page,
      pageSize
    };
    
    if (fromDate) params.fromDate = fromDate.toISOString();
    if (toDate) params.toDate = toDate.toISOString();
    
    const response = await axios.get<{data: PagedResult<AuditLog>}>(
      `${API_URL}/api/audit-logs/user/${userId}`, 
      { params, headers: this.getAuthHeaders() }
    );
    return response.data.data;
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
    pageSize: number = 20
  ): Promise<PagedResult<AuditLog>> {
    const params = { page, pageSize };
    const response = await axios.get<{data: PagedResult<AuditLog>}>(
      `${API_URL}/api/audit-logs/entity/${encodeURIComponent(entityType)}/${encodeURIComponent(entityId)}`,
      { params, headers: this.getAuthHeaders() }
    );
    return response.data.data;
  },

  /**
   * Export audit logs based on filter criteria
   * @param filter Filter options
   * @param format Export format
   * @returns File blob for download
   */
  async exportAuditLogs(filter: AuditLogFilter, format: AuditLogExportFormat): Promise<Blob> {
    const response = await axios.post(
      `${API_URL}/api/audit-logs/export`, 
      filter, 
      {
        params: { format },
        responseType: 'blob',
        headers: this.getAuthHeaders()
      }
    );
    return response.data;
  },

  /**
   * Archive old audit logs
   * @param retentionDays Number of days to retain logs
   * @returns Archive result
   */
  async archiveOldLogs(retentionDays: number = 365): Promise<{ archivedCount: number; message: string }> {
    const response = await axios.post<{data: { archivedCount: number; message: string }}>(
      `${API_URL}/api/audit-logs/archive`,
      {},
      { 
        params: { retentionDays },
        headers: this.getAuthHeaders()
      }
    );
    return response.data.data;
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
    
    const response = await axios.get<{data: AuditLogIntegrityResult}>(
      `${API_URL}/api/audit-logs/verify-integrity`, 
      { params, headers: this.getAuthHeaders() }
    );
    return response.data.data;
  },
  
  /**
   * Get authentication headers for API requests
   */
  getAuthHeaders() {
    const token = localStorage.getItem('ikhtibar_access_token');
    return token ? { Authorization: `Bearer ${token}` } : {};
  }
};
