import { AuthProvider } from '@/modules/auth/hooks/useAuth'
import AuthGuard from '@/modules/auth/components/AuthProvider'
import { pathNames } from '@/shared/constants/pathNames'
import { RouteObject } from 'react-router-dom'
import { 
    AuditLogs, 
    ContentManagment, 
    Dashboard, 
    DashboardLayout, 
    ErrorPage, 
    NotFoundPage, 
    RoleManagement, 
    Support, 
    UserManagement,
    MediaManagement,
    QuestionBank,
    Notifications,
    SystemSettings,
    SystemApiDocs,
    SystemHealth,
    AnalyticsDashboard,
    AnalyticsUsers,
    AnalyticsContent,
    CustomerExperienceSurveys,
    CustomerExperienceFeedback,
    EParticipationPortal,
    EParticipationInitiatives,
    HelpFaq,
    HelpTerms,
    HelpPrivacy,
    SystemCategoryPage,
    AnalyticsCategoryPage,
    CustomerExperienceCategoryPage,
    HelpCategoryPage
} from './dashboardRoutes'

const dashboardRoutes: RouteObject = {
    path: pathNames.dashboard,
    element: (
        <AuthProvider>
            <AuthGuard />
        </AuthProvider>
    ),
    errorElement: <ErrorPage />,
    children: [
        {
            element: <DashboardLayout />,
            children: [
                {
                    index: true,
                    element: <Dashboard />,
                },
                {
                    path: pathNames.contentManagment,
                    element: <ContentManagment />,
                },
                {
                    path: pathNames.support,
                    element: <Support />,
                },
                {
                    path: pathNames.userManagement,
                    element: <UserManagement />,
                },
                {
                    path: pathNames.roleManagement,
                    element: <RoleManagement />,
                },
                {
                    path: pathNames.auditLogs,
                    element: <AuditLogs />,
                },
                // New feature routes
                {
                    path: pathNames.mediaManagement,
                    element: <MediaManagement />,
                },
                {
                    path: pathNames.questionBank,
                    element: <QuestionBank />,
                },
                {
                    path: pathNames.notifications,
                    element: <Notifications />,
                },
                // Parent category routes
                {
                    path: pathNames.system,
                    element: <SystemCategoryPage />,
                },
                {
                    path: pathNames.analytics,
                    element: <AnalyticsCategoryPage />,
                },
                {
                    path: pathNames.customerExperience,
                    element: <CustomerExperienceCategoryPage />,
                },
                {
                    path: pathNames.help,
                    element: <HelpCategoryPage />,
                },
                // System routes
                {
                    path: pathNames.systemSettings,
                    element: <SystemSettings />,
                },
                {
                    path: pathNames.systemApiDocs,
                    element: <SystemApiDocs />,
                },
                {
                    path: pathNames.systemHealth,
                    element: <SystemHealth />,
                },
                // Analytics routes
                {
                    path: pathNames.analyticsDashboard,
                    element: <AnalyticsDashboard />,
                },
                {
                    path: pathNames.analyticsUsers,
                    element: <AnalyticsUsers />,
                },
                {
                    path: pathNames.analyticsContent,
                    element: <AnalyticsContent />,
                },
                // Customer Experience routes
                {
                    path: pathNames.customerExperienceSurveys,
                    element: <CustomerExperienceSurveys />,
                },
                {
                    path: pathNames.customerExperienceFeedback,
                    element: <CustomerExperienceFeedback />,
                },
                // E-Participation routes
                {
                    path: pathNames.eParticipationPortal,
                    element: <EParticipationPortal />,
                },
                {
                    path: pathNames.eParticipationInitiatives,
                    element: <EParticipationInitiatives />,
                },
                // Help & Legal routes
                {
                    path: pathNames.helpFaq,
                    element: <HelpFaq />,
                },
                {
                    path: pathNames.helpTerms,
                    element: <HelpTerms />,
                },
                {
                    path: pathNames.helpPrivacy,
                    element: <HelpPrivacy />,
                },
                {
                    path: 'error/*',
                    element: <ErrorPage />,
                },
                {
                    path: '*',
                    element: <NotFoundPage />,
                },
            ],
        },
    ],
}

export default dashboardRoutes
