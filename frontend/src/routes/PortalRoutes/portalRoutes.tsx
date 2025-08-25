import { lazyRetry } from '@/shared/utils/lazyRetry'
import { lazy } from 'react'

export const ErrorPage = lazy(() => lazyRetry(() => import('@/pages/ErrorPage')))
export const NotFoundPage = lazy(() => lazyRetry(() => import('@/pages/NotFoundPage')))

export const Login = lazy(() => lazyRetry(() => import('@/modules/auth/views/Login')))
export const PortalLayout = lazy(() => lazyRetry(() => import('@/layout/PortalLayout')))
export const SSOCallBack = lazy(() => lazyRetry(() => import('@/modules/auth/components/OidcAuthCallback')))
export const PortalSupport = lazy(() => lazyRetry(() => import('@/modules/support/views/PortalSupport')))
export const TicketDetails = lazy(() => lazyRetry(() => import('@/modules/support/views/PortalTicketDetails')))
export const TicketInquiry = lazy(() => lazyRetry(() => import('@/modules/support/views/Inquiry')))
export const TicketCreate = lazy(() => lazyRetry(() => import('@/modules/support/views/PortalCreate')))
export const FAQS = lazy(() => lazyRetry(() => import('@/modules/faq/views/index')))
export const TermsAndConditions = lazy(() => lazyRetry(() => import('@/modules/termsAndConditions/views/index')))
export const PrivacyPolicy = lazy(() => lazyRetry(() => import('@/modules/privacyPolicy/views/index')))
export const Home = lazy(() => lazyRetry(() => import('@/modules/home/views/index')))
export const EParticipation = lazy(() => lazyRetry(() => import('@/modules/eparticipation/views/index')))
export const EParticipationPortal = lazy(() => lazyRetry(() => import('@/modules/eparticipation/views/index')))
