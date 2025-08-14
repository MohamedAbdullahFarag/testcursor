import { lazyRetry } from '@/shared/utils/lazyRetry'
import { lazy } from 'react'

export const Dashboard = lazy(() => lazyRetry(() => import('@/modules/dashboard/views')))
export const ErrorPage = lazy(() => lazyRetry(() => import('@/pages/ErrorPage')))
export const NotFoundPage = lazy(() => lazyRetry(() => import('@/pages/NotFoundPage')))
export const DashboardLayout = lazy(() => lazyRetry(() => import('@/layout/DashboardLayout')))
export const ContentManagment = lazy(() => lazyRetry(() => import('@/modules/content-managment/views')))
export const Support = lazy(() => lazyRetry(() => import('@/modules/support/views/DashboardSupport')))
export const UserManagement = lazy(() => lazyRetry(() => import('@/modules/user-management/components/UserManagementView')))
export const RoleManagement = lazy(() => lazyRetry(() => import('@/modules/role-management/components/RoleManagementView')))
export const AuditLogs = lazy(() => lazyRetry(() => import('@/modules/audit/pages/AuditLogsPage')))
