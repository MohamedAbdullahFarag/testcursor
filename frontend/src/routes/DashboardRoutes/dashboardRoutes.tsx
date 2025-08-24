import { lazyRetry } from '@/shared/utils/lazyRetry'
import { lazy } from 'react'

export const Dashboard = lazy(() => lazyRetry(() => import('@/modules/dashboard/views')))
export const ErrorPage = lazy(() => lazyRetry(() => import('@/pages/ErrorPage')))
export const NotFoundPage = lazy(() => lazyRetry(() => import('@/pages/NotFoundPage')))
export const DashboardLayout = lazy(() => lazyRetry(() => import('@/layout/DashboardLayout')))
export const ContentManagment = lazy(() => lazyRetry(() => import('@/modules/content-managment/views')))
export const Support = lazy(() => lazyRetry(() => import('@/modules/support/views/DashboardSupport')))
export const UserManagement = lazy(() => lazyRetry(() => import('@/modules/user-management/views/UserManagementView')))
export const RoleManagement = lazy(() => lazyRetry(() => import('@/modules/role-management/components/RoleManagementView')))
export const AuditLogs = lazy(() => lazyRetry(() => import('@/modules/audit/pages/AuditLogsPage')))

// New feature exports
export const MediaManagement = lazy(() => lazyRetry(() => import('@/modules/media-management/components/MediaManagementPage')))
export const QuestionBank = lazy(() => lazyRetry(() => import('@/modules/question-bank/components/QuestionBankTree')))
export const Notifications = lazy(() => lazyRetry(() => import('@/modules/notifications/components/NotificationCenter')))

// System exports - using placeholder components for now
export const SystemSettings = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder
export const SystemApiDocs = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder
export const SystemHealth = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder

// Analytics exports - using placeholder components for now
export const AnalyticsDashboard = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder
export const AnalyticsUsers = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder
export const AnalyticsContent = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder

// Customer Experience exports - using placeholder components for now
export const CustomerExperienceSurveys = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder
export const CustomerExperienceFeedback = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder

// E-Participation exports - using placeholder components for now
export const EParticipationPortal = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder
export const EParticipationInitiatives = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder

// Help & Legal exports - using placeholder components for now
export const HelpFaq = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder
export const HelpTerms = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder
export const HelpPrivacy = lazy(() => lazyRetry(() => import('@/modules/dashboard/views'))) // Placeholder
