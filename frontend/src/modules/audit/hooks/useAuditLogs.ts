import { useState, useEffect, useCallback } from 'react';
import { auditLogService } from '../services/auditLogService';
import { AuditLog, AuditLogFilter, AuditLogExportFormat } from '../types/auditLogs';
import { PagedResult } from '../../../shared/types/common';

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
  // Default filter state
  const defaultFilter: AuditLogFilter = {
    page: 0,
    pageSize: 25,
    sortBy: 'timestamp',
    sortDirection: 'desc',
    searchText: '',
    severity: undefined,
    category: undefined,
    userId: undefined,
    entityType: undefined,
    entityId: undefined,
    fromDate: undefined,
    toDate: undefined
  };

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
    sortBy: 'timestamp',
    sortDirection: 'desc',
    searchText: '',
    severity: undefined,
    category: undefined,
    userId: undefined,
    entityType: undefined,
    entityId: undefined,
    fromDate: undefined,
    toDate: undefined
  });

  /**
   * Load audit logs based on current filter
   */
  const refreshLogs = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
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
    } catch (err) {
      console.error('Error loading audit logs:', err);
      setError('Failed to load audit logs');
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
