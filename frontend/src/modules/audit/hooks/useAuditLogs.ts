import { useState, useEffect, useCallback } from 'react';
import { auditLogService } from '../services/auditLogService';
import { AuditLog, AuditLogFilter, AuditLogExportFormat } from '../types/auditLogs';
import { PagedResult } from '../../../shared/types/common';
import { API_FEATURE_FLAGS } from '../../../shared/config/apiFeatureFlags';

/**
 * Interface for useAuditLogs hook return value
 */
export interface UseAuditLogsReturn {
  auditLogs: PagedResult<AuditLog>;
  loading: boolean;
  error: string | null;
  filter: AuditLogFilter;
  setFilter: React.Dispatch<React.SetStateAction<AuditLogFilter>>;
  refreshLogs: () => Promise<void>;
  exportLogs: (format: AuditLogExportFormat) => Promise<Blob>;
  archiveOldLogs: (retentionDays: number) => Promise<{ archivedCount: number; message: string }>;
  verifyIntegrity: (fromDate?: Date, toDate?: Date) => Promise<Record<number, boolean>>;
}

/**
 * Hook for managing audit logs
 * @param initialFilter Initial filter state
 */
export const useAuditLogs = (initialFilter?: Partial<AuditLogFilter>): UseAuditLogsReturn => {
  // State
  const [auditLogs, setAuditLogs] = useState<PagedResult<AuditLog>>({
    data: [],
    total: 0,
    page: 0,
    pageSize: 25,
    totalPages: 0,
    hasNext: false,
    hasPrevious: false
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<AuditLogFilter>({
    page: 0,
    pageSize: 25,
    sortBy: 'Timestamp',
    sortDirection: 'desc',
    searchText: '',
    severity: undefined,
    category: undefined,
    userId: undefined,
    action: undefined,
    entityType: undefined,
    entityId: undefined,
    fromDate: undefined,
    toDate: undefined,
    ipAddress: undefined,
    includeSystemActions: undefined,
    userIdentifier: undefined,
    ...initialFilter
  });

  /**
   * Load audit logs based on current filter
   */
  const refreshLogs = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      // Check if audit logs API is disabled
      if (!API_FEATURE_FLAGS.AUDIT_LOGS_ENABLED && API_FEATURE_FLAGS.USE_MOCK_DATA) {
        // Use mock data when API is disabled
        console.log('Using mock audit logs data (API disabled)');
        // TODO: Update mock data to match new AuditLog interface
        // setAuditLogs(MOCK_AUDIT_LOGS);
        return;
      }

      const result = await auditLogService.getAuditLogs(filter);
      const pageSize = result.pageSize || 25;
      const total = result.total || 0;
      const page = result.page || 0;
      const totalPages = Math.ceil(total / pageSize);
      
      setAuditLogs({
        data: result.data || [],
        total,
        page,
        pageSize,
        totalPages,
        hasNext: page < totalPages - 1,
        hasPrevious: page > 0
      });
    } catch (err: any) {
      console.error('Error loading audit logs:', err);
      
      // Handle specific error types
      if (err?.response?.status === 403) {
        setError('You do not have permission to view audit logs. Please contact your administrator.');
      } else if (err?.response?.status === 401) {
        setError('Authentication required. Please log in again.');
      } else if (err?.response?.status >= 500) {
        setError('Server error occurred. Please try again later.');
      } else {
        setError('Failed to load audit logs. Please try again.');
      }
      
      // Fallback to mock data on error if enabled
      if (API_FEATURE_FLAGS.USE_MOCK_DATA && API_FEATURE_FLAGS.DEV_MODE_ENABLED) {
        console.log('Falling back to mock audit logs data due to API error');
        // TODO: Update mock data to match new AuditLog interface
        // setAuditLogs(MOCK_AUDIT_LOGS);
      }
    } finally {
      setLoading(false);
    }
  }, [filter]);

  /**
   * Export logs in specified format
   */
  const exportLogs = useCallback(async (format: AuditLogExportFormat): Promise<Blob> => {
    setLoading(true);
    
    try {
      const blob = await auditLogService.exportAuditLogs(filter, format);
      return blob;
    } catch (err) {
      console.error('Error exporting audit logs:', err);
      setError('Failed to export audit logs');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [filter]);

  /**
   * Archive old logs
   */
  const archiveOldLogs = useCallback(async (retentionDays: number = 365) => {
    setLoading(true);
    
    try {
      const result = await auditLogService.archiveOldLogs(retentionDays);
      return result;
    } catch (err) {
      console.error('Error archiving audit logs:', err);
      setError('Failed to archive audit logs');
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  /**
   * Verify integrity of logs
   */
  const verifyIntegrity = useCallback(async (fromDate?: Date, toDate?: Date) => {
    setLoading(true);
    
    try {
      const result = await auditLogService.verifyLogsIntegrity(fromDate, toDate);
      return result;
    } catch (err) {
      console.error('Error verifying audit logs integrity:', err);
      setError('Failed to verify audit logs integrity');
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  // Load logs on component mount and when filter changes
  useEffect(() => {
    refreshLogs();
  }, [refreshLogs]);

  return {
    auditLogs,
    loading,
    error,
    filter,
    setFilter,
    refreshLogs,
    exportLogs,
    archiveOldLogs,
    verifyIntegrity
  };
};

export default useAuditLogs;
