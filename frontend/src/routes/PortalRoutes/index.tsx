import PortalProvider from '@/modules/auth/components/PortalProvider'
import { pathNames } from '@/shared/constants/pathNames'
import { RouteObject } from 'react-router-dom'
import {
    EParticipationPortal,
    ErrorPage,
    FAQS,
    Home,
    Login,
    NotFoundPage,
    PortalLayout,
    PortalSupport,
    PrivacyPolicy,
    SSOCallBack,
    TermsAndConditions,
    TicketCreate,
    TicketDetails,
    TicketInquiry,
} from './portalRoutes'

const portalRoutes: RouteObject = {
    path: '/',
    element: <PortalProvider />,
    errorElement: <ErrorPage />,
    children: [
        {
            element: <PortalLayout />,
            children: [
                {
                    path: pathNames.portalSupport,
                    children: [
                        {
                            index: true,
                            element: <PortalSupport />,
                        },
                        {
                            path: pathNames.supportDetails,
                            element: <TicketDetails />,
                        },
                        {
                            path: pathNames.portalSupportInquiry,
                            element: <TicketInquiry />,
                        },
                        {
                            path: pathNames.portalSupportCreate,
                            element: <TicketCreate />,
                        },
                    ],
                },

                {
                    index: true,
                    element: <Home />,
                },
                {
                    path: pathNames.termsAndConditions,
                    element: <TermsAndConditions />,
                },
                {
                    path: pathNames.privacyPolicy,
                    element: <PrivacyPolicy />,
                },
                {
                    path: pathNames.faqs,
                    element: <FAQS />,
                },
                {
                    path: pathNames.eParticipation,
                    element: <EParticipationPortal />,
                },
                {
                    // index: true,
                    path: pathNames.login,
                    element: <Login />,
                },
                {
                    path: 'login/sso/callback',
                    element: <SSOCallBack />,
                },
                {
                    path: 'error/*',
                    element: <ErrorPage isErrorPage />,
                },
                {
                    path: '*',
                    element: <NotFoundPage />,
                },
            ],
        },
    ],
}

export default portalRoutes
