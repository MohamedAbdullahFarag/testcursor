import { BaseEntity } from '../../../shared/models/api';

/**
 * Audit log severity levels (numeric enum matching backend)
 */
export enum AuditSeverity {
  Critical = 0,
  High = 1,
  Medium = 2,
  Low = 3
}

/**
 * Audit log categories (numeric enum matching backend)
 */
export enum AuditCategory {
  Authentication = 0,
  Authorization = 1,
  UserManagement = 2,
  DataAccess = 3,
  System = 4,
  Security = 5
}

/**
 * Utility functions to convert enum values to display labels
 */
export const AuditSeverityLabels: Record<AuditSeverity, string> = {
  [AuditSeverity.Critical]: 'Critical',
  [AuditSeverity.High]: 'High',
  [AuditSeverity.Medium]: 'Medium',
  [AuditSeverity.Low]: 'Low'
};

export const AuditCategoryLabels: Record<AuditCategory, string> = {
  [AuditCategory.Authentication]: 'Authentication',
  [AuditCategory.Authorization]: 'Authorization',
  [AuditCategory.UserManagement]: 'User Management',
  [AuditCategory.DataAccess]: 'Data Access',
  [AuditCategory.System]: 'System',
  [AuditCategory.Security]: 'Security'
};

/**
 * Helper functions to get severity and category labels
 */
export const getSeverityLabel = (severity: number | AuditSeverity): string => {
  return AuditSeverityLabels[severity as AuditSeverity] || `Unknown (${severity})`;
};

export const getCategoryLabel = (category: number | AuditCategory): string => {
  return AuditCategoryLabels[category as AuditCategory] || `Unknown (${category})`;
};

/**
 * Audit log export formats
 */
export enum AuditLogExportFormat {
  CSV = 'CSV',
  Excel = 'Excel',
  JSON = 'JSON'
}

/**
 * Audit log data transfer object - matches backend AuditLogDto
 */
export interface AuditLog extends BaseEntity {
  auditLogId: number;
  timestamp: string;
  userId?: number;
  userIdentifier?: string;
  action: string;
  details?: string;
  entityType?: string;
  entityId?: string;
  oldValues?: string;
  newValues?: string;
  ipAddress?: string;
  userAgent?: string;
  sessionId?: string;
  severity: number; // Backend sends numeric enum values
  category: number; // Backend sends numeric enum values
  isSystemAction: boolean;
}

/**
 * Audit log filter options - matches backend AuditLogFilter
 */
export interface AuditLogFilter {
  userId?: number;
  userIdentifier?: string;
  action?: string;
  entityType?: string;
  entityId?: string;
  severity?: number; // Use numeric values for backend compatibility
  category?: number; // Use numeric values for backend compatibility
  fromDate?: string;
  toDate?: string;
  ipAddress?: string;
  includeSystemActions?: boolean;
  searchText?: string;
  page: number;
  pageSize: number;
  sortBy: string;
  sortDirection: string;
}

/**
 * Audit log integrity verification result
 */
export interface AuditLogIntegrityResult {
  [auditLogId: number]: boolean;
}
