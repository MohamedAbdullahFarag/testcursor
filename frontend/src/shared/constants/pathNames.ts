const dashboardPath = '/dashboard'
const portalPath = '/'

export const pathNames = {
    dashboard: dashboardPath,
    contentManagment: `${dashboardPath}/content-managment`,
    userManagement: `${dashboardPath}/user-management`,
    roleManagement: `${dashboardPath}/role-management`,
    auditLogs: `${dashboardPath}/audit-logs`,
    portal: portalPath,
    support: `${dashboardPath}/support`,
    portalSupport: `/support`,
    supportDetails: '/support/details',
    portalSupportCreate: `/support/create`,
    portalSupportInquiry: `/support/inquiry`,
    notAuthorized: '/notAuthorized',
    faqs: '/faq',
    termsAndConditions: '/TermsAndConditions',
    privacyPolicy: '/privacyPolicy',
    login: '/login',
    // Portal-level e-participation route (public portal)
    eparticipation: '/eparticipation',
    eParticipation: `${dashboardPath}/e-participation`,
    // New paths for completed features
    mediaManagement: `${dashboardPath}/media`,
    mediaCollections: `${dashboardPath}/media/collections`,
    questionBank: `${dashboardPath}/question-bank`,
    questionBankTree: `${dashboardPath}/question-bank/tree`,
    questionBankCategories: `${dashboardPath}/question-bank/categories`,
    notifications: `${dashboardPath}/notifications`,
    notificationPreferences: `${dashboardPath}/notifications/preferences`,
    // System paths
    system: `${dashboardPath}/system`,
    systemSettings: `${dashboardPath}/system/settings`,
    systemApiDocs: `${dashboardPath}/system/api-docs`,
    systemHealth: `${dashboardPath}/system/health`,
    // Analytics paths
    analytics: `${dashboardPath}/analytics`,
    analyticsDashboard: `${dashboardPath}/analytics/dashboard`,
    analyticsUsers: `${dashboardPath}/analytics/users`,
    analyticsContent: `${dashboardPath}/analytics/content`,
    // Customer Experience paths
    customerExperience: `${dashboardPath}/customer-experience`,
    customerExperienceSurveys: `${dashboardPath}/customer-experience/surveys`,
    customerExperienceFeedback: `${dashboardPath}/customer-experience/feedback`,
    // E-Participation paths
    eParticipationPortal: `${dashboardPath}/e-participation/portal`,
    eParticipationInitiatives: `${dashboardPath}/e-participation/initiatives`,
    // Help & Legal paths
    help: `${dashboardPath}/help`,
    helpFaq: `${dashboardPath}/help/faq`,
    helpTerms: `${dashboardPath}/help/terms`,
    helpPrivacy: `${dashboardPath}/help/privacy`,
}
