/**
 * English translations for category pages
 * Shared translations for System, Analytics, Customer Experience, and Help category pages
 */

export const categoryPagesEn = {
  // Common elements
  common: {
    dashboard: 'Dashboard',
    implemented: 'Implemented',
    comingSoon: 'Coming Soon',
    inDevelopment: 'In Development'
  },

  // System category
  system: {
    title: 'System Management',
    description: 'Manage system settings, configurations, and monitoring',
    breadcrumbTitle: 'System Management',
    features: {
      settings: {
        title: 'System Settings',
        description: 'Configure system parameters and preferences'
      },
      apiDocs: {
        title: 'API Documentation', 
        description: 'View API documentation and endpoints'
      },
      health: {
        title: 'System Health',
        description: 'Monitor system performance and health metrics'
      }
    }
  },

  // Analytics category  
  analytics: {
    title: 'Analytics',
    description: 'View comprehensive analytics and reporting',
    breadcrumbTitle: 'Analytics',
    features: {
      dashboard: {
        title: 'Analytics Dashboard',
        description: 'Overview of system analytics and key metrics'
      },
      users: {
        title: 'User Analytics',
        description: 'Analyze user behavior and engagement'
      },
      content: {
        title: 'Content Analytics', 
        description: 'Track content performance and usage statistics'
      }
    }
  },

  // Customer Experience category
  customerExperience: {
    title: 'Customer Experience',
    description: 'Manage customer feedback and satisfaction',
    breadcrumbTitle: 'Customer Experience',
    features: {
      surveys: {
        title: 'Service Surveys',
        description: 'Create and manage service quality surveys'
      },
      feedback: {
        title: 'Content Feedback',
        description: 'Collect and analyze content feedback'
      }
    }
  },

  // Help category
  help: {
    title: 'Help & Support',
    description: 'Access help resources and support information',
    breadcrumbTitle: 'Help & Support',
    features: {
      faq: {
        title: 'FAQ',
        description: 'Frequently asked questions and answers'
      },
      terms: {
        title: 'Terms & Conditions',
        description: 'View terms and conditions of service'
      },
      privacy: {
        title: 'Privacy Policy',
        description: 'Review our privacy policy and data handling'
      }
    }
  }
};

export default categoryPagesEn;
