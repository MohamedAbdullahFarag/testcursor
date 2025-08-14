import AuthProvider from '@/modules/auth/components/AuthProvider'
import { pathNames } from '@/shared/constants/pathNames'
import { RouteObject } from 'react-router-dom'
import { AuditLogs, ContentManagment, Dashboard, DashboardLayout, ErrorPage, NotFoundPage, RoleManagement, Support, UserManagement } from './dashboardRoutes'

const dashboardRoutes: RouteObject = {
    path: pathNames.dasbhaord,
    element: <AuthProvider />,
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
