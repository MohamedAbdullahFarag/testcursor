import { lazyRetry } from '@/shared/utils/lazyRetry'
import { lazy } from 'react'

export const Dashboard = lazy(() => lazyRetry(() => import('@/modules/dashboard/views')))
export const ErrorPage = lazy(() => lazyRetry(() => import('@/pages/ErrorPage')))
export const NotFoundPage = lazy(() => lazyRetry(() => import('@/pages/NotFoundPage')))
export const ComingSoonPage = lazy(() => lazyRetry(() => import('@/pages/ComingSoonPage')))
export const DashboardLayout = lazy(() => lazyRetry(() => import('@/layout/DashboardLayout')))
export const ContentManagment = lazy(() => lazyRetry(() => import('@/modules/content-managment/views')))
export const Support = lazy(() => lazyRetry(() => import('@/modules/support/views/DashboardSupport')))
export const UserManagement = lazy(() => lazyRetry(() => import('@/modules/user-management/views/UserManagementView')))
export const RoleManagement = lazy(() => lazyRetry(() => import('@/modules/role-management/components/RoleManagementView')))
export const AuditLogs = lazy(() => lazyRetry(() => import('@/modules/audit/pages/AuditLogsPage')))

// New feature exports - implemented features
export const MediaManagement = lazy(() => lazyRetry(() => import('@/modules/media-management/components/MediaManagementPage')))
export const QuestionBank = lazy(() => lazyRetry(() => import('@/modules/question-bank/components/QuestionBankTree')))
export const Notifications = lazy(() => lazyRetry(() => import('@/modules/notifications/components/NotificationCenter')))

// System exports - coming soon components
export const SystemSettings = lazy(() => lazyRetry(() => import('@/pages/SystemSettings')))
export const SystemApiDocs = lazy(() => lazyRetry(() => import('@/pages/SystemApiDocs')))
export const SystemHealth = lazy(() => lazyRetry(() => import('@/pages/SystemHealth')))

// Analytics exports - coming soon components  
export const AnalyticsDashboard = lazy(() => lazyRetry(() => import('@/pages/AnalyticsDashboard')))
export const AnalyticsUsers = lazy(() => lazyRetry(() => import('@/pages/AnalyticsUsers')))
export const AnalyticsContent = lazy(() => lazyRetry(() => import('@/pages/AnalyticsContent')))

// Customer Experience exports - using actual implemented components
export const CustomerExperienceSurveys = lazy(() => lazyRetry(() => import('@/modules/customerExperience/views/ServiceSurvey')))
export const CustomerExperienceFeedback = lazy(() => lazyRetry(() => import('@/modules/customerExperience/views/ContentSurvey')))

// E-Participation exports - using actual implemented component
export const EParticipationPortal = lazy(() => lazyRetry(() => import('@/modules/eparticipation/views')))
export const EParticipationInitiatives = lazy(() => lazyRetry(() => import('@/pages/EParticipationInitiatives')))

// Help & Legal exports - using actual implemented components where available
export const HelpFaq = lazy(() => lazyRetry(() => import('@/modules/faq/views')))
export const HelpTerms = lazy(() => lazyRetry(() => import('@/modules/TermsAndConditions/views')))
export const HelpPrivacy = lazy(() => lazyRetry(() => import('@/modules/PrivacyPolicy/views')))

// Category page exports - parent/overview pages for feature categories
export const SystemCategoryPage = lazy(() => lazyRetry(() => import('@/pages/SystemCategoryPage')))
export const AnalyticsCategoryPage = lazy(() => lazyRetry(() => import('@/pages/AnalyticsCategoryPage')))
export const CustomerExperienceCategoryPage = lazy(() => lazyRetry(() => import('@/pages/CustomerExperienceCategoryPage')))
export const HelpCategoryPage = lazy(() => lazyRetry(() => import('@/pages/HelpCategoryPage')))
