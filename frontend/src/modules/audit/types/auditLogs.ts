import { BaseEntity } from '../../../shared/models/api';

/**
 * Audit log severity levels
 */
export enum AuditSeverity {
  Critical = 'Critical',
  High = 'High',
  Medium = 'Medium',
  Low = 'Low'
}

/**
 * Audit log categories
 */
export enum AuditCategory {
  Authentication = 'Authentication',
  Authorization = 'Authorization',
  UserManagement = 'UserManagement',
  DataAccess = 'DataAccess',
  Configuration = 'Configuration',
  API = 'API',
  System = 'System'
}

/**
 * Audit log export formats
 */
export enum AuditLogExportFormat {
  CSV = 'CSV',
  Excel = 'Excel',
  JSON = 'JSON'
}

/**
 * Audit log data transfer object
 */
export interface AuditLog extends BaseEntity {
  timestamp: string;
  userId?: number;
  username?: string;
  action: string;
  description: string;
  entityType?: string;
  entityId?: string;
  ipAddress?: string;
  userAgent?: string;
  data?: string;
  severity: AuditSeverity;
  category: AuditCategory;
  success: boolean;
}

/**
 * Audit log filter options
 */
export interface AuditLogFilter {
  userId?: number;
  action?: string;
  entityType?: string;
  entityId?: string;
  severity?: AuditSeverity;
  category?: AuditCategory;
  fromDate?: string;
  toDate?: string;
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
